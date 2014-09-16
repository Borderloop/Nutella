using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProductFeedReader
{
    public class Database
    {
        private MySqlConnection _conn;
        private MySqlCommand _cmd;

        private static Database _instance;

        private Database() { }

        public static Database Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Database();
                }
                return _instance;
            }
        }

        public void Connect(string conStr)
        {
            _conn = new MySqlConnection(conStr);
            _conn.Open();
        }

        public void Connect(string ip, string db, string uid, string pw)
        {
            _conn = new MySqlConnection(@"Data Source=" + ip + ";Database=" + db + ";Uid=" + uid + ";Pwd=" + pw);
            _conn.Open();
        }

        public DataTable Read(string query)
        {
            if(_conn == null)
            {
                return null;
            }
          
            _cmd = new MySqlCommand(query, _conn);

            DataTable res = new DataTable();
            res.Load(_cmd.ExecuteReader());
            return res;
        }

        public void AddProduct(Product p, string table)
        {
            string query = 
                @"INSERT INTO " + table 
                + " (ean, price, direct_link, sku)"
                + " VALUES (@EAN, @PRICE, @URL, @SKU)";

            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@EAN", p.EAN);
            _cmd.Parameters.AddWithValue("@PRICE", p.Price);
            _cmd.Parameters.AddWithValue("@URL", p.Url);
            _cmd.Parameters.AddWithValue("@SKU", p.SKU);
            
            _cmd.ExecuteNonQuery();
        }

        public DataTable GetCategories()
        {
            string query = "SELECT description FROM category";
            DataTable dt = Read(query);

            return dt;
        }
        public DataTable GetCategorySynonyms()
        {
            string query = "SELECT description FROM category_synonyms";
            DataTable dt = Read(query);

            return dt;
        }

        /// <summary>
        /// Method used to get a single product from the database, used for saving a match.
        /// </summary>
        /// <param name="id">The article id of the matched product</param>
        /// <returns></returns>
        public DataTable GetProduct(int id)
        {
            string query = "SELECT id as 'article-id', brand as 'article-Brand', description as " +
                            "'article-Description', title.title as 'title-Title', " +
                            "ean.ean as 'ean-EAN', sku.sku as 'sku-SKU' FROM article \n" +
                            "LEFT JOIN ean ON ean.article_id = article.id \n" +
                            "LEFT JOIN title ON title.article_id = article.id \n" +
                            "LEFT JOIN sku ON sku.article_id = article.id \n" +
                            "WHERE id = '" + id + "'";

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// Method used to update an article that misses data.
        /// </summary>
        /// <param name="table">Table to be updated.</param>
        /// <param name="column">column of the article to be updated.</param>
        /// <param name="value">Value of the column to be updated.</param>
        /// <param name="idColumn">Defines the name of the id column. Can be 'id' or 'article_id'</param>
        /// <param name="id">id of the article to be updated.</param>
        public void Update(String table, String column, String value, int id)
        {
            String query = "UPDATE " + table + " SET " + column + " = @VALUE WHERE id = @ID";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@VALUE", value);
            _cmd.Parameters.AddWithValue("@ID", id);

            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// This method adds a record for a found match. This is always for the title, ean or sku
        /// table. This is because when this code is reached, no record is found for in one of those
        /// tables for an article.
        /// </summary>
        public void AddForMatch(String table, String value, int id)
        {
            String query = "INSERT INTO " + table + " VALUES (@VALUE, @id)";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@VALUE", value);
            _cmd.Parameters.AddWithValue("@ID", id);

            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the category (from the Borderloop category tree) for an article.
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns></returns>
        public DataTable GetCategoryForArticle(int id)
        {
            string query = "SELECT category.id, category.description FROM category " +
                           "INNER JOIN cat_article ON category.id = cat_article.category_id " +
                           "INNER JOIN article ON article.id = cat_article.article_id " +
                           "WHERE article.id = "+ id;

            return Read(query);
        }

        /// <summary>
        /// Gets tje category synonyms for an article
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns></returns>
        public DataTable GetCategorySynonymsForArticle(int id)
        {
            string query = "SELECT cs.description FROM category_synonyms as cs " +
                           "INNER JOIN category as c ON c.id = cs.category_id " +
                           "INNER JOIN cat_article as ca ON ca.category_id = c.id " +
                           "INNER JOIN article as a ON a.id = ca.article_id " +
                           "WHERE a.id = " + id;

            return Read(query);
        }

        /// <summary>
        /// Method to save a found category synonym to the database.
        /// </summary>
        /// <param name="id">The category id, which was retrieved at GetCategoryForArticle()</param>
        /// <param name="description">The description of the category, which is the synonym</param>
        public void SaveCategorySynonym(int id, string description)
        {
            string query = "INSERT into category_synonyms VALUES (@ID, @DESCRIPTION)";

            _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@ID", id);
            _cmd.Parameters.AddWithValue("@DESCRIPTION", description);

            _cmd.ExecuteNonQuery();
        }

        public void Close()
        {
            _conn.Close();
        }
    }
}
