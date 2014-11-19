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

namespace BorderSource.Common
{
    public class BetsyDbContextReader
    {
        public string ConnectionString { get; set; }

        public BetsyDbContextReader()
        {
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = 32767;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Port = 3306;
            providerConnStrBuilder.Database = Statics.settings["dbname"];
            providerConnStrBuilder.Password = Statics.settings["dbpw"];
            providerConnStrBuilder.Server = Statics.settings["dbsource"];
            providerConnStrBuilder.UserID = Statics.settings["dbuid"];

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
        }

        /// <summary>
        /// Gets the article id for a given table and record.
        /// </summary>
        /// <param name="table">The table to search the article id in.</param>
        /// <param name="column">The column that is searched for a value.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public int GetArticleNumber(string table, string value)
        {
            if (value == null) return -1;
            int result;
            using (var db = new BetsyModel(ConnectionString))
            {
                switch (table)
                {
                    case "ean":
                        long ean;
                        if (!long.TryParse(value, out ean)) return -1;
                        var actualEan = db.ean.Where(e => e.ean1 == ean).FirstOrDefault();
                        result = actualEan == default(ean) ? -1 : actualEan.article_id;
                        break;
                    case "sku":
                        var actualSku = db.sku.Where(s => s.sku1.ToLower().Trim() == value.ToLower().Trim()).FirstOrDefault();
                        result = actualSku == default(sku) ? -1 : actualSku.article_id;
                        break;
                    case "title":
                        var actualTitle = db.title.Where(t => t.title1.ToLower().Trim() == value.ToLower().Trim()).FirstOrDefault();
                        result = actualTitle == default(title) ? -1 : actualTitle.article_id;
                        break;
                    default:
                        result = -1;
                        break;
                }
            }
            return result;
        }


        public int CheckIfProductExists(Product record)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                var product = db.product.Where(p => p.webshop_url.ToLower().Trim() == record.Webshop.ToLower().Trim() && p.affiliate_unique_id.ToLower().Trim() == record.AffiliateProdID.ToLower().Trim()).FirstOrDefault();
                int result = product == default(product) ? -1 : product.article_id;
                return result;
            }
        }

        /// <summary>
        /// This method will get the first-next product from the VBobData table.
        /// </summary>
        /// <returns></returns>
        public vbobdata GetNextVBobProduct()
        {
            vbobdata result;
            using (var db = new BetsyModel(ConnectionString))
            {
                result = db.vbobdata.Where(v => v.rerun != null && v.rerun == false).OrderBy(v => v.id).FirstOrDefault();
                db.Database.Connection.Dispose();
                db.Dispose();
            }
            return result;
        }

        public IEnumerable<DbDataRecord> GetSuggestedProducts(int productID)
        {
            IEnumerable<DbDataRecord> result;
            using (var db = new BetsyModel(ConnectionString))
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

                result = db.Database.SqlQuery<DbDataRecord>(query, productID);
                db.Database.Connection.Dispose();
                db.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Gets the most relevant matches for a title from a brand, used for visual bob.
        /// The returned boolean is used to check if a record should be saved as new article.
        /// </summary>
        /// <param name="Record">The record to find relevant matches for</param>
        public bool GetRelevantMatches(Product Record, int lastInserted)
        {
            using (var db = new BetsyModel(ConnectionString))
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
                    //InsertInVBobSuggested(lastInserted, articleIds);
                    match = true;
                }
                else // Else, no matches are found. Save this record to the database.
                {
                    match = false;
                }

                db.Database.Connection.Dispose();
                db.Dispose();
                return match;
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
            using (var db = new BetsyModel(ConnectionString))
            {
                product result;
                result = db.product.Where(product => product.article_id == aId && product.webshop_url.ToLower().Trim() == Record.Webshop.ToLower().Trim()).FirstOrDefault();
                db.Database.Connection.Dispose();
                db.Dispose();
                return result;
            }
        }

        public int GetCountryId(string webshop)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                int result = db.webshop.Where(w => w.url.ToLower().Trim() == webshop.ToLower().Trim()).FirstOrDefault().country_id;
                db.Database.Connection.Dispose();
                db.Dispose();
                return result;
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

        /// <summary>
        /// Get the category id of the searched category if exist
        /// </summary>
        /// <param name="table">The table to search category</param>
        /// <param name="column">The column that is searched for a value.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public int GetCategoryNumber(int articleId)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                article art = db.article.Where(a => a.id == articleId).FirstOrDefault();
                if (art == null) return -1;
                category cat = art.category.FirstOrDefault();
                int result = cat == default(category) ? -1 : cat.id;
                db.Database.Connection.Dispose();
                db.Dispose();
                return result;
            }
        }

        /// <summary>
        /// This method checks if given category exist in database
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="description">Description</param>
        /// <param name="web_url">Webshop</param>
        /// <param name="p">Value description</param>
        /// <param name="webshop">Value webshop</param>
        /// <returns></returns>
        public bool CheckCategorySynonym(string description, string webshop)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                category_synonym catSyn = db.category_synonym.Where(cs => cs.web_url.ToLower().Trim() == webshop.ToLower().Trim() && cs.description.ToLower().Trim() == description.ToLower().Trim()).FirstOrDefault();
                bool result = catSyn == default(category_synonym);
                db.Database.Connection.Dispose();
                db.Dispose();
                return result;
            }
        }

        public ILookup<string, Webshop> GetAllWebshops()
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<Webshop> names = new List<Webshop>();
                db.webshop.ToList().ForEach(w => names.Add(new Webshop { CountryId = w.country_id, Id = w.id, Url = w.url }));
                db.Database.Connection.Dispose();
                db.Dispose();
                return names.ToLookup(w => w.Url.ToLower().Trim());
            }
        }

        public ILookup<string, Category> GetAllCategories()
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<Category> categories = new List<Category>();
                db.category.ToList().ForEach(c => categories.Add(new Category { Id = c.id, Description = c.description }));
                db.Database.Connection.Dispose();
                db.Dispose();
                return categories.ToLookup(c => c.Description.ToLower().Trim());
            }
        }

        public ILookup<string, CategorySynonym> GetCategorySynonymsForWebshop(string webshop)
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                List<CategorySynonym> synonyms = new List<CategorySynonym>();
                db.category_synonym.Where(cs => cs.web_url.ToLower().Trim() == webshop.ToLower().Trim()).ToList().ForEach(cs => synonyms.Add(new CategorySynonym { CategoryId = cs.category_id, Description = cs.description, WebshopUrl = cs.web_url }));
                db.Database.Connection.Dispose();
                db.Dispose();
                return synonyms.ToLookup(c => c.Description.ToLower().Trim());
            }
        }

    }
}
