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
using BorderSource.BetsyContext;
using BorderSource.ProductAssociation;

namespace BorderSource.Common
{
    public class BetsyDbContextReader : IDisposable
    {
        public string ConnectionString { get; set; }

        private BetsyModel db;

        public BetsyDbContextReader(string dbname, string dbpw, string dbsource, string dbuid, int port, int maxpoolsize = 120)
        {
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = (uint)maxpoolsize;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Port = (uint)port;
            providerConnStrBuilder.Database = dbname;
            providerConnStrBuilder.Password = dbpw;
            providerConnStrBuilder.Server = dbsource;
            providerConnStrBuilder.UserID = dbuid;

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
            db = new BetsyModel(ConnectionString);            
        }

      
        public Dictionary<Product, int> GetExistingProductIds(ICollection<Product> products, string webshop)
        {
            Dictionary<Product, int> dic = new Dictionary<Product, int>();
            List<string> ids = products.Select(p => p.AffiliateProdID).ToList();
            var query = db.product.Where(p => p.webshop_url == webshop && ids.Contains(p.affiliate_unique_id));
            foreach (var prod in query)
            {
                Product key = products.Where(p => p.AffiliateProdID == prod.affiliate_unique_id).FirstOrDefault();
                int value = prod.article_id;
                if (key == null || value == 0) continue;
                if (!dic.ContainsKey(key)) dic.Add(key, value);
            }
            return dic;
        }

        public Dictionary<Product, int> GetEanMatches(ICollection<Product> products)
        {
            Dictionary<Product, int> dic = new Dictionary<Product, int>();
            long temp;
            List<long> eans = products.Where(p => long.TryParse(p.EAN, out temp)).Select(p => long.Parse(p.EAN)).ToList();
            var query = db.ean.Where(e => eans.Contains(e.ean1)).ToList();
            foreach (var prod in query)
            {
                Product key = products.Where(p => long.TryParse(p.EAN, out temp)).Where(p => long.Parse(p.EAN) == prod.ean1).FirstOrDefault();
                int value = prod.article_id;               
                if (key == null || value == 0) continue;
                if (!dic.ContainsKey(key)) dic.Add(key, value);
            }
            return dic;
        }           

        public Dictionary<Product, int> GetSkuMatches(ICollection<Product> products)
        {
            Dictionary<Product, int> dic = new Dictionary<Product, int>();
            List<string> skus = products.Select(p => p.SKU).ToList();
            var query = db.sku.Where(s => skus.Contains(s.sku1)).ToList();
            foreach (var prod in query)
            {
                Product key = products.Where(p => p.SKU.ToUpper() == prod.sku1.ToUpper()).FirstOrDefault();
                int value = prod.article_id;
                if (key == null || value == 0) continue;
                if (db.article.Where(a => a.id == value).First().product.Where(p => p.webshop_url == key.Webshop).FirstOrDefault() != null) continue;
                if (!dic.ContainsKey(key)) dic.Add(key, value);
            }
            return dic;
        }

        /// <summary>
        /// This method will get the first-next product from the VBobData table.
        /// </summary>
        /// <returns></returns>
        public vbobdata GetNextVBobProduct()
        {
            vbobdata result;
            result = db.vbobdata.Where(v => v.rerun != null && v.rerun == false).OrderBy(v => v.id).FirstOrDefault();
            return result;
        }

        public IEnumerable<DbDataRecord> GetSuggestedProducts(int productID)
        {
            IEnumerable<DbDataRecord> result;
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

            result = db.Database.SqlQuery<DbDataRecord>(query, productID);

            return result;
        }

        /// <summary>
        /// Gets the most relevant matches for a title from a brand, used for visual bob.
        /// The returned boolean is used to check if a record should be saved as new article.
        /// </summary>
        /// <param name="Record">The record to find relevant matches for</param>
        public bool GetRelevantMatches(Product Record, int lastInserted)
        {

            // Get the most relevant matches for the given product and return their article id'str.
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

            // Invoke method to save suggested matches to database if matches are found
            if (articleIds.Count() > 0)
            {
                // InsertInVBobSuggested(lastInserted, articleIds);
                match = true;
            }
            else //  Else, no matches are found. Save this record to the database.
            {
                match = false;
            }
            return match;

        }

        /// <summary>
        /// Gets the product data for a given record, used for comparison.
        /// </summary>
        /// <param name="Record">The record to get product data for.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        /// <returns></returns>
        public product GetProductData(Product Record, int aId)
        {
            product result;
            result = db.product.Where(product => product.article_id == aId && product.webshop_url == Record.Webshop).FirstOrDefault();
            return result;

        }

        public int GetCountryId(string webshop)
        {
            int result = db.webshop.Where(w => w.url == webshop).FirstOrDefault().country_id;
            return result;
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

        /// <summary>
        /// Get the category id of the searched category if exist
        /// </summary>
        /// <param name="table">The table to search category</param>
        /// <param name="column">The column that is searched for a value.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public int GetCategoryNumber(int articleId)
        {
            article art = db.article.Where(a => a.id == articleId).FirstOrDefault();
            if (art == null)
            {
                Console.WriteLine("Did not find article ID " + articleId);
                return -1;
            }
            category cat = art.category.FirstOrDefault();
            int result = cat == default(category) ? -1 : cat.id;
            return result;
        }

        /// <summary>
        /// This method checks if given category exist in database
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="description">Description</param>
        /// <param name="web_url">Webshop</param>
        /// <param name="produit">Value description</param>
        /// <param name="webshop">Value webshop</param>
        /// <returns></returns>
        public bool CheckCategorySynonym(string description, string webshop)
        {
            category_synonym catSyn = db.category_synonym.Where(cs => cs.web_url == webshop && cs.description == description).FirstOrDefault();
            bool result = catSyn == default(category_synonym);
            return result;

        }

        public Dictionary<string, Webshop> GetAllWebshops()
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<Webshop> names = new List<Webshop>();
                db.webshop.ToList().ForEach(w => names.Add(new Webshop { CountryId = w.country_id, Id = w.id, Url = w.url }));
                db.Database.Connection.Close();
                db.Database.Connection.Dispose();
                db.Dispose();
                return names.ToDictionary(w => w.Url.Trim(), w => w);
            }
        }

        public ILookup<string, Category> GetAllCategories()
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<Category> categories = new List<Category>();
                db.category.ToList().ForEach(c => categories.Add(new Category { Id = c.id, Description = c.description }));
                db.Database.Connection.Close();
                db.Database.Connection.Dispose();
                db.Dispose();
                return categories.ToLookup(c => c.Description);
            }
        }

        public ILookup<string, CategorySynonym> GetCategorySynonymsForWebshop(string webshop)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<CategorySynonym> synonyms = new List<CategorySynonym>();
                db.category_synonym.Where(cs => cs.web_url == webshop).ToList().ForEach(cs => synonyms.Add(new CategorySynonym { CategoryId = cs.category_id, Description = cs.description, WebshopUrl = cs.web_url }));
                db.Database.Connection.Close();
                db.Database.Connection.Dispose();
                db.Dispose();
                return synonyms.ToLookup(c => c.Description.ToLower().Trim());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        ~BetsyDbContextReader()
        {
            Dispose(false);
        }

    }
}
