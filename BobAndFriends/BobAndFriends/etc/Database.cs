using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Globalization;
using System.Reflection;

namespace BobAndFriends
{
    public class Database
    {
        /// <summary>
        /// The private singleton instance of the database
        /// </summary>
        private static Database _instance;

        /// <summary>
        /// The string used to make a connection to the database
        /// </summary>
        private string _conStr;

        /// <summary>
        /// A list containing residue-objects to be written to the server.
        /// </summary>
        private List<residue> pendingResidue;           

        /// <summary>
        /// The constructor
        /// </summary>
        private Database()
        {
            pendingResidue = new List<residue>();
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = 32767;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Database = Statics.settings["dbname"];
            providerConnStrBuilder.Password = Statics.settings["dbpw"];
            providerConnStrBuilder.Server = Statics.settings["dbsource"];
            providerConnStrBuilder.UserID = Statics.settings["dbuid"];

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyModel.csdl|res://*/BetsyModel.ssdl|res://*/BetsyModel.msl";

            _conStr = entityConnStrBuilder.ConnectionString;
        }

        /// <summary>
        /// The public singleton instance of the database
        /// </summary>
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Create a new instance if it is not already done
                    _instance = new Database();
                }
                return _instance;
            }
        }

        public string GetConnectionString()
        {
            return _conStr;
        }

        public int CountRows(string tableName)
        {
            using(var db = new BetsyModel(_conStr))
            {
                var actualTable = (from table in tableName select table);
                return actualTable == null ? -1 : actualTable.Count();
            }
        }

        /// <summary>
        /// Gets the article id for a given table and record.
        /// </summary>
        /// <param name="table">The table to search the article id in.</param>
        /// <param name="column">The column that is searched for a value.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public int GetArticleNumber(string table, string column, string value)
        {
            using (var db = new BetsyModel(_conStr))
            {
                var actualTable = from t in table where column == value select t;

                if (actualTable == null) return -1;
                if (actualTable is ean) return ((ean)actualTable).article_id;
                if (actualTable is sku) return ((sku)actualTable).article_id;
                if (actualTable is title) return ((title)actualTable).article_id;

                return -1;
            }
        }

        public bool CheckIfBrandExists(string actualBrand)
        {
            using (var db = new BetsyModel(_conStr))
            {
                var table = db.article.Where(a => a.brand == actualBrand).FirstOrDefault();
                return table != default(article);
            }          
        }

        public int GetAIDFromUAC(Product record)
        {
            using (var db = new BetsyModel(_conStr))
            {
                var product = db.product.Where(p => p.affiliate_name == record.Affiliate && p.affiliate_unique_id == record.AffiliateProdID).FirstOrDefault();
                return product == default(product) ? -1 : product.article_id;
            }
        }

        public void SaveMatch(Product Record, int matchedArticleID, int countryID)
        {
            using (var db = new BetsyModel(_conStr))
            {
                //First get all data needed for matching. Ean, sku and title_synonym are seperate because they can store multiple values.
                article articleTable = db.article.Where(a => a.id == matchedArticleID).Include(a => a.ean)
                    .Include(a => a.sku)
                    .Include(t => t.title.Select(ts => ts.title_synonym))
                    .FirstOrDefault();


                //Loop through ean and sku collections to check if the ean or sku already exists. If not, add it
                if (!(articleTable.ean.Any(e => e.ean1 == Record.EAN)) && Record.EAN != "") articleTable.ean.Add(new ean { ean1 = Record.EAN, article_id = matchedArticleID });
                if (!(articleTable.sku.Any(s => s.sku1 == Record.SKU)) && Record.SKU != "") articleTable.sku.Add(new sku { sku1 = Record.SKU, article_id = matchedArticleID });

                int titleid = articleTable.title.Where(t => t.article_id == matchedArticleID && t.country_id == countryID).FirstOrDefault().id;

                //If any title synonym matches the title, up the occurences.
                if (articleTable.title.Any(t => t.title_synonym.Any(ts => ts.title == Record.Title)))
                {
                    title_synonym ts = db.title_synonym.Where(innerTs => innerTs.title == Record.Title).FirstOrDefault();
                    ts.occurrences++;
                    if (ts.occurrences > articleTable.title.Max(t => t.title_synonym.Max(ts2 => ts2.occurrences)))
                    {
                        UpdateTitle(titleid, ts.title);
                    }
                }
                //else, add the title to the synonyms.
                else
                {
                    title_synonym ts = new title_synonym { occurrences = 1, title = Record.Title, title_id = titleid };
                    db.title_synonym.Add(ts);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// This method updates the occurrences column for a given title
        /// </summary>
        /// <param name="titleId">Id of the title belonging to the title synonym</param>
        /// <param name="occurrences">Amount of occurences, this will get updated</param>
        /// <param name="title">The title to be updated</param>
        public void UpdateTitleSynonymOccurrences(int titleId, int occurrences, string title)
        {
            using(var db = new BetsyModel(_conStr))
            {
                var title_syn = db.title_synonym.Where(ts => ts.title_id == titleId && ts.title == title).FirstOrDefault();
                if(title_syn != null)
                {
                    title_syn.occurrences = (short?)occurrences;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Updates the title in the title table. This is done if another title has a higher occurrence then the one in the title table.
        /// </summary>
        /// <param name="titleId">The id of the title to be updated</param>
        /// <param name="title">The new title</param>
        public void UpdateTitle(int titleId, string title)
        {
            using (var db = new BetsyModel(_conStr))
            {
                var newTitle = db.title.Where(t => t.id == titleId).FirstOrDefault();
                newTitle.title1 = title;
                db.SaveChanges();
            }
        }

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public void SendToResidue(Product p)
        {
            Stopwatch sw = new Stopwatch();           
        
            residue res = new residue
            {
                //id = residueId++,
                title = p.Title,
                description = p.Description,
                category = p.Category,
                ean = p.EAN,
                sku = p.SKU,
                brand = p.Brand,
                image = p.Image_Loc
            };

            pendingResidue.Add(res);

            if (pendingResidue.Count > Statics.maxResidueListSize)
            {
                sw.Start();
                Console.WriteLine("Started writing {0} products to residue", pendingResidue.Count);
                foreach (residue residue in pendingResidue)
                {
                    using (var db = new BetsyModel(_conStr))
                    {
                        db.residue.Add(residue);
                        db.SaveChanges();
                    }
                }               
                Console.WriteLine("Saved {0} products to residue in {1}", pendingResidue.Count, sw.Elapsed);
                pendingResidue.Clear();
            }
        }

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public int SendToVBobData(Product p)
        {
            using(var db = new BetsyModel(_conStr))
            {
                vbobdata res = new vbobdata
                {
                    title = p.Title,
                    description = p.Description,
                    category = p.Category,
                    ean = p.EAN,
                    sku = p.SKU,
                    brand = p.Brand,
                    rerun = false,
                    image_loc = p.Image_Loc
                };
                db.vbobdata.Add(res);
                db.SaveChanges();
                return res.id;
            }
        }           

        /// <summary>
        /// Used to save a new article.
        /// </summary>
        /// <param name="Record">The article to be saved</param>
        public void SaveNewArticle(Product Record, int countryId)
        {
            using(var db = new BetsyModel(_conStr))
            {
                country cou = db.country.Where(c => c.id == 1).FirstOrDefault() ;
                webshop webshop = db.webshop.Where(w => w.url == Record.Webshop).FirstOrDefault();

                if(webshop == default(webshop))
                {
                    Console.WriteLine("Could not find webshop {0}, aborting the save.", Record.Webshop);
                    return;
                }

                if(cou == default(country))
                {
                    Console.WriteLine("Could not find country id {0}, aborting the save.",  countryId);
                    return;
                }
                article art = new article
                {
                    description = Record.Description,
                    brand = Record.Brand,
                    image_loc = Record.Image_Loc
                    
                };
                //Do not modify this as this is neccessary to get the last id.
                db.article.Add(art);

                ean ean = new ean
                {
                    ean1 = Record.EAN,
                    article_id = art.id
                };
                db.ean.Add(ean);

                title title = new title
                {
                    title1 = Record.Title,
                    country_id = (short)countryId,
                    article_id = art.id,
                    country = cou
                };
                db.title.Add(title);

                title_synonym ts = new title_synonym
                {
                    title = Record.Title,
                    title_id = title.id
                };
                db.title_synonym.Add(ts);

                if(Record.SKU != "")
                {
                    sku sku = new sku
                    {
                        sku1 = Record.SKU,
                        article_id = art.id
                    };
                    db.sku.Add(sku);
                }

                decimal castedShipCost;
                decimal castedPrice;
                if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) Console.WriteLine("Cannot cast shipping cost " + Record.DeliveryCost + " to decimal.");
                if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) Console.WriteLine("Cannot cast price " + Record.Price + " to decimal."); ;

                product product = new product
                {
                    article_id = art.id,
                    ship_cost = castedShipCost,
                    ship_time = Record.DeliveryTime,
                    price = castedPrice,
                    webshop_url = webshop.url,
                    direct_link = Record.Url,
                    affiliate_name = Record.Affiliate,
                    affiliate_unique_id = Record.AffiliateProdID                  
                };
                db.product.Add(product);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// This method will get the first-next product from the VBobData table.
        /// </summary>
        /// <returns></returns>
        public vbobdata GetNextVBobProduct()
        {
            using (var db = new BetsyModel(_conStr))
            {
                return db.vbobdata.Where(v => v.rerun != null && v.rerun == false).OrderBy(v => v.id).FirstOrDefault();  
            }
        }

        public IEnumerable<DbDataRecord> GetSuggestedProducts(int productID)
        {
            using(var db = new BetsyModel(_conStr))
            {
                string query = " SELECT article.id as 'Article ID',"
                + "article.brand as 'Brand', "
                + "article.description as 'Description' ,"
                + "title.title as 'Title',"
                + "vbob_suggested.id as `vBobSug ID`, "
                + "(SELECT GROUP_CONCAT(ean.ean) FROM ean WHERE ean.article_id = article.id) as 'EANs', "
                + "(SELECT GROUP_CONCAT(sku.sku) FROM sku WHERE sku.article_id = article.id) as 'SKUs'"
                + "FROM vbobdata, vbob_suggested"
                + "INNER JOIN article ON  article.id = vbob_suggested.article_id"
                + "LEFT JOIN ean ON ean.article_id = article.id"
                + "LEFT JOIN title ON title.article_id = article.id"
                + "LEFT JOIN sku ON sku.article_id = article.id"
                + "WHERE vbob_suggested.vbob_id = @ID GROUP BY vbob_suggested.article_id";

                return db.Database.SqlQuery<DbDataRecord>(query, productID);
            }
        }

        public void DeleteFromVbobData(int id)
        {
            using (var db = new BetsyModel(_conStr))
            {
                var sugdata = db.vbob_suggested.Where(vs => vs.vbob_id == id).FirstOrDefault();
                db.vbob_suggested.Remove(sugdata);

                var vbobdata = db.vbobdata.Where(v => v.id == id).FirstOrDefault();
                db.vbobdata.Remove(vbobdata);
                db.SaveChanges();
            }
        }

        public void RerunVbobEntry(vbobdata rerun)
        {
            using(var db = new BetsyModel(_conStr))
            {
                db.vbobdata.Add(rerun);
            }
        }

        public void DeleteFromResidue(Product p)
        {
            using(var db = new BetsyModel(_conStr))
            {
                db.residue.Remove(db.residue.Where(r => r.title == p.Title && r.description == p.Description && r.category == p.Category && r.brand == p.Brand && r.ean == p.EAN && r.sku == p.SKU).FirstOrDefault());
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the most relevant matches for a title from a brand, used for visual bob.
        /// The returned boolean is used to check if a record should be saved as new article.
        /// </summary>
        /// <param name="Record">The record to find relevant matches for</param>
        public bool GetRelevantMatches(Product Record, int lastInserted)
        {
            using (var db = new BetsyModel(_conStr))
            {

                //Get the most relevant matches for the given product and return their article id's.
                string query = "SELECT * FROM article " +
                               "INNER JOIN title ON title.article_id = article.id " +
                               "WHERE title.id IN (SELECT title_id FROM title_synonym as ts " +
                                                  "INNER JOIN title ON title.id = ts.title_id " +
                                                  "WHERE MATCH(ts.title) AGAINST ('@TITLE') " +
                                                  "GROUP BY title.title " +
                                                  "ORDER BY MATCH(ts.title) AGAINST ('@TITLE'))" +
                               "AND article.brand = '@BRAND' " +
                               "LIMIT 10";

                List<article> articleIds = db.Database.SqlQuery<article>(query, Record.Title, Record.Brand).ToList();

                bool match;

                //Invoke method to save suggested matches to database if matches are found
                if (articleIds.Count() > 0)
                { 
                    InsertInVBobSuggested(lastInserted, articleIds);
                    match = true;
                }
                else // Else, no matches are found. Save this record to the database.
                {
                    match = false;
                }


                return match;
            }
        }

        /// <summary>
        /// Inserts the found matches for the product in the vbob_suggested table
        /// </summary>
        /// <param name="vBobId">The id of the record as stored in the vbobdata table</param>
        /// <param name="articleIds">List of article_ids that match the record. First id has the most relevance, last is the least.</param>
        private void InsertInVBobSuggested(int vBobId, List<article> articleIds)
        {
            using (var db = new BetsyModel(_conStr))
            {
                foreach (article a in articleIds)
                {
                    db.vbob_suggested.Add(new vbob_suggested { vbob_id = vBobId, article_id = a.id });
                }
                db.SaveChanges();
            }

        }

        /// <summary>
        /// Saves product data for a given record.
        /// </summary>
        /// <param name="Record">The record of which the product data will be saved.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        public void SaveProductData(Product Record, int matchedArticleId)
        {
            using (var db = new BetsyModel(_conStr))
            {
                decimal castedShipCost;
                decimal castedPrice;
                if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) Statics.Logger.WriteLine("Cannot cast " + Record.DeliveryCost + " to decimal.");
                if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) Statics.Logger.WriteLine("Cannot cast " + Record.Price + " to decimal."); ;

                webshop webshop = db.webshop.Where(w => w.url == Record.Webshop).FirstOrDefault();

                if (webshop == default(webshop))
                {
                    Console.WriteLine("Could not find webshop {0}, aborting the save.", Record.Webshop);
                    return;
                }

                product product = new product
                {
                    article_id = matchedArticleId,
                    ship_time = Record.DeliveryTime,
                    ship_cost = (decimal?)castedShipCost,
                    price = castedPrice,
                    webshop_url = webshop.url,
                    direct_link = Record.Url,
                    affiliate_name = Record.Affiliate,
                    affiliate_unique_id = Record.AffiliateProdID
                };

                db.product.Add(product);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the product data for a given record, used for comparison.
        /// </summary>
        /// <param name="Record">The record to get product data for.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        /// <returns></returns>
        public product GetProductData(Product Record, int aId)
        {
            using( var db = new BetsyModel(_conStr))
            {
                return db.product.Where(product => product.article_id == aId && product.webshop_url == Record.Webshop).FirstOrDefault();
            }
        }

        /// <summary>
        /// This method can be called to externally save changes to the database
        /// </summary>
        public void SaveChanges()
        {
            using(var db = new BetsyModel(_conStr))
            {
                db.SaveChanges();
            }
        }

        public int GetCountryId(string webshop)
        {
            using (var db = new BetsyModel(_conStr))
            {
                return db.webshop.Where(w => w.url == webshop).FirstOrDefault().country_id;
            }
        }

        public void UpdateProductData(product productData, Product Record)
        {
            using (var db = new BetsyModel())
            {
                try
                {
                    db.product.Attach(productData);
                    var entry = db.Entry(productData);

                    //Alter updated product
                    foreach (PropertyInfo p in productData.GetType().GetProperties())
                    {
                        //Loop over things like "article_id", "id", etc.
                        if (!Statics.TwoWayDBProductToBobProductMapping.ContainsKey(p.Name)) continue;

                        object recordValue = GetPropValue(Record, Statics.TwoWayDBProductToBobProductMapping[p.Name]);

                        object dbValue = p.GetValue(productData, null);

                        if (dbValue.GetType().Equals(typeof(System.DateTime)))
                        {
                            DateTime correctValue;
                            if (!(DateTime.TryParse((string)recordValue, out correctValue))) { Console.WriteLine("Cannot convert " + recordValue + " to " + dbValue.GetType()); continue; }
                            else if (!correctValue.Equals(dbValue))
                            {
                                p.SetValue(productData, correctValue);
                                entry.Property(p.Name).IsModified = true;
                            }

                        }
                        else if (dbValue.GetType().Equals(typeof(System.Decimal)))
                        {
                            decimal correctValue;
                            if (!(decimal.TryParse((string)recordValue, NumberStyles.Any, CultureInfo.InvariantCulture, out correctValue))) { Console.WriteLine("Cannot convert " + recordValue + " to " + dbValue.GetType()); continue; }
                            else if (!correctValue.Equals(dbValue))
                            {
                                p.SetValue(productData, correctValue);
                                entry.Property(p.Name).IsModified = true;
                            }
                        }
                        else
                        {
                            if (!recordValue.Equals(dbValue))
                            {
                                p.SetValue(productData, recordValue);
                                entry.Property(p.Name).IsModified = true;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception in UpdateProductData, message: " + e.Message);
                    Console.WriteLine("Trying to force the connection to open...");
                    db.Database.Connection.Open();                   
                }
            } 
              
        }

        /// <summary>
        /// Gets the value of a Record for a given property
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private static object GetPropValue(Product record, string propName)
        {
            return record.GetType().GetProperty(propName).GetValue(record, null);
        }
    }
}
