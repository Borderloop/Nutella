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
using BorderSource.Common;

namespace BetsyTest
{
    public class BetsyService
    {
        private BetsyEntities _context;
        public BetsyService(BetsyEntities context)
        {
            _context = context;
        }

        public int GetArticleNumber(string table, string column, string value)
        {
            switch (table)
            {
                case "ean":
                    var actualEan = _context.ean.Where(e => e.ean1 == value).FirstOrDefault();
                    return actualEan == default(ean) ? -1 : actualEan.article_id;
                case "sku":
                    var actualSku = _context.sku.Where(s => s.sku1 == value).FirstOrDefault();
                    return actualSku == default(sku) ? -1 : actualSku.article_id;
                case "title":
                    var actualTitle = _context.title.Where(t => t.title1 == value).FirstOrDefault();
                    return actualTitle == default(title) ? -1 : actualTitle.article_id;
                default: return -1;
            }

        }

        public bool CheckIfBrandExists(string actualBrand)
        {
            var table = _context.article.Where(a => a.brand == actualBrand).FirstOrDefault();
            return table != default(article);
        }

        public int GetAIDFromUAC(Product record)
        {
            var product = _context.product.Where(p => p.webshop_url == record.Webshop && p.affiliate_unique_id == record.AffiliateProdID).FirstOrDefault();
            return product == default(product) ? -1 : product.article_id;
        }

        public void SaveMatch(Product Record, int matchedArticleID, int countryID)
        {
            //First get all data needed for matching. Ean, sku and title_synonym are seperate because they can store multiple values.
            article articleTable = _context.article.Where(a => a.id == matchedArticleID).Include(a => a.ean)
                .Include(a => a.sku)
                .Include(t => t.title.Select(ts => ts.title_synonym))
                .FirstOrDefault();


            //Loop through ean and sku collections to check if the ean or sku already exists. If not, add it
            if (!(articleTable.ean.Any(e => e.ean1 == Record.EAN)) && Record.EAN != "") _context.ean.Add(new ean { ean1 = Record.EAN, article_id = matchedArticleID });
            if (!(articleTable.sku.Any(s => s.sku1 == Record.SKU)) && Record.SKU != "") _context.sku.Add(new sku { sku1 = Record.SKU, article_id = matchedArticleID });

            title title = articleTable.title.Where(t => t.article_id == matchedArticleID && t.country_id == countryID).FirstOrDefault();

            if (title == default(title))
            {
                title addedTitle = new title { title1 = Record.Title, country_id = (short)countryID, article_id = matchedArticleID };
                _context.title.Add(addedTitle);
                _context.title_synonym.Add(new title_synonym { occurrences = 1, title = Record.Title, title_id = addedTitle.id });
            }
            else
            {
                //If any title synonym matches the title, up the occurences.
                if (articleTable.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower() == Record.Title.ToLower())))
                {
                    title_synonym ts = _context.title_synonym.Where(innerTs => innerTs.title.ToLower() == Record.Title.ToLower()).FirstOrDefault();
                    ts.occurrences++;
                    //_context.Entry(ts).State = EntityState.Modified;
                    if (ts.occurrences > articleTable.title.Max(t => t.title_synonym.Max(ts2 => ts2.occurrences)))
                    {
                        UpdateTitle(title.id, ts.title);
                    }
                }
                //else, add the title to the synonyms.
                else
                {
                    title_synonym ts = new title_synonym { occurrences = 1, title = Record.Title, title_id = title.id };
                    _context.title_synonym.Add(ts);
                }
            }

            _context.SaveChanges();
        }

        public void UpdateTitleSynonymOccurrences(int titleId, int occurrences, string title)
        {
            var title_syn = _context.title_synonym.Where(ts => ts.title_id == titleId && ts.title == title).FirstOrDefault();
            if (title_syn != null)
            {
                title_syn.occurrences = (short?)occurrences;
                _context.SaveChanges();
            }
        }

        public void UpdateTitle(int titleId, string title)
        {
            var newTitle = _context.title.Where(t => t.id == titleId).FirstOrDefault();
            newTitle.title1 = title;
            //_context.Entry(newTitle).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void SendToResidue(Product p)
        {
            residue res = new residue
            {
                title = p.Title,
                description = p.Description,
                category = p.Category,
                ean = p.EAN,
                sku = p.SKU,
                brand = p.Brand,
                image = p.Image_Loc
            };

            _context.residue.Add(res);
            _context.SaveChanges();
        }

        public int SendToVBobData(Product p)
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
            _context.vbobdata.Add(res);
            _context.SaveChanges();
            return res.id;
        }

        public void SaveNewArticle(Product Record, int countryId)
        {
            country cou = _context.country.Where(c => c.id == countryId).FirstOrDefault();
            webshop webshop = _context.webshop.Where(w => w.url == Record.Webshop).FirstOrDefault();

            if (webshop == default(webshop))
            {
                return;
            }

            if (cou == default(country))
            {
                Console.WriteLine("Could not find country id {0}, aborting the save.", countryId);
                return;
            }
            article art = new article
            {
                description = Record.Description,
                brand = Record.Brand,
                image_loc = Record.Image_Loc

            };
            //Do not modify this as this is neccessary to get the last id.
            _context.article.Add(art);

            ean ean = new ean
            {
                ean1 = Record.EAN,
                article_id = art.id
            };
            _context.ean.Add(ean);

            title title = new title
            {
                title1 = Record.Title,
                country_id = (short)countryId,
                article_id = art.id,
                country = cou
            };
            _context.title.Add(title);

            title_synonym ts = new title_synonym
            {
                title = Record.Title,
                title_id = title.id,
                occurrences = 1
            };
            _context.title_synonym.Add(ts);

            if (Record.SKU != "")
            {
                sku sku = new sku
                {
                    sku1 = Record.SKU,
                    article_id = art.id
                };
                _context.sku.Add(sku);
            }

            decimal castedShipCost;
            decimal castedPrice;
            if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) Console.WriteLine("Cannot cast shipping cost " + Record.DeliveryCost + " to decimal.");
            if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) Console.WriteLine("Cannot cast price " + Record.Price + " to decimal.");

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
            _context.product.Add(product);
            _context.SaveChanges();
        }

        public vbobdata GetNextVBobProduct()
        {
            return _context.vbobdata.Where(v => v.rerun != null && v.rerun == false).OrderBy(v => v.id).FirstOrDefault();
        }

        public IEnumerable<DbDataRecord> GetSuggestedProducts(int productID)
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

            return _context.Database.SqlQuery<DbDataRecord>(query, productID);
        }

        public void DeleteFromVbobData(int id)
        {
            var sugdata = _context.vbob_suggested.Where(vs => vs.vbob_id == id).FirstOrDefault();
            _context.vbob_suggested.Remove(sugdata);

            var vbobdata = _context.vbobdata.Where(v => v.id == id).FirstOrDefault();
            _context.vbobdata.Remove(vbobdata);
            _context.SaveChanges();
        }

        public void RerunVbobEntry(vbobdata rerun)
        {
            _context.vbobdata.Add(rerun);
        }

        public void DeleteFromResidue(Product p)
        {
            _context.residue.Remove(_context.residue.Where(r => r.title == p.Title && r.description == p.Description && r.category == p.Category && r.brand == p.Brand && r.ean == p.EAN && r.sku == p.SKU).FirstOrDefault());
            _context.SaveChanges();
        }

        public bool GetRelevantMatches(Product Record, int lastInserted)
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

            List<article> articleIds = _context.Database.SqlQuery<article>(query, Record.Title, Record.Brand).ToList();

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

        private void InsertInVBobSuggested(int vBobId, List<article> articleIds)
        {
            foreach (article a in articleIds)
            {
                _context.vbob_suggested.Add(new vbob_suggested { vbob_id = vBobId, article_id = a.id });
            }
            _context.SaveChanges();

        }

        public void SaveProductData(Product Record, int matchedArticleId)
        {
            decimal castedShipCost;
            decimal castedPrice;
            if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) { }
            if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) { }

            webshop webshop = _context.webshop.Where(w => w.url == Record.Webshop).FirstOrDefault();

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

            _context.product.Add(product);
            _context.SaveChanges();
        }

        public product GetProductData(Product Record, int aId)
        {
            return _context.product.Where(product => product.article_id == aId && product.webshop_url == Record.Webshop).FirstOrDefault();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public int GetCountryId(string webshop)
        {
            return _context.webshop.Where(w => w.url == webshop).FirstOrDefault().country_id;
        }

        public void UpdateProductData(product productData, Product Record)
        {
            _context.product.Attach(productData);
            var entry = _context.Entry(productData);

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
            _context.SaveChanges();
        }

        private static object GetPropValue(Product record, string propName)
        {
            return record.GetType().GetProperty(propName).GetValue(record, null);
        }

        public int GetCategoryNumber(string table, string column, int articleId)
        {
            category cat = _context.article.Where(a => a.id == articleId).FirstOrDefault().category.FirstOrDefault();
            return cat == default(category) ? -1 : cat.id;
        }

        public int CheckCategorySynonym(string table, string description, string web_url, string p, string webshop)
        {
            category_synonym catSyn = _context.category_synonym.Where(cs => cs.web_url == webshop && cs.description == p).FirstOrDefault();
            return catSyn == default(category_synonym) ? -1 : catSyn.category_id;
        }

        public void InsertIntoCatSynonyms(int catid, string description, string web_url)
        {
            _context.category_synonym.Add(new category_synonym { category_id = catid, description = description, web_url = web_url });
        }

        public void InsertNewCatArtile(int category_id, int article_id)
        {
            category catToAdd = _context.category.Where(c => c.id == category_id).FirstOrDefault();
            _context.article.Where(a => a.id == article_id).FirstOrDefault().category.Add(catToAdd);
            _context.SaveChanges();
        }
    }
}

