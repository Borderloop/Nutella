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
        /// <summary>
        /// The connection which is established when connecting to the database.
        /// </summary>
        private MySqlConnection _conn;

        /// <summary>
        /// A command which can be used to execute queries
        /// </summary>
        private MySqlCommand _cmd;

        /// <summary>
        /// The private singleton instance of the database
        /// </summary>
        private static Database _instance;

        /// <summary>
        /// The result DataTable with output from the queries.
        /// </summary>
        private DataTable _resultTable;

        /// <summary>
        /// The constructor
        /// </summary>
        private Database()
        {
            //Initiate the Datatable
            _resultTable = new DataTable();
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

        /// <summary>
        /// This method will let the user connect to the database with a given connection string.
        /// </summary>
        /// <param name="conStr">The connection string</param>
        public void Connect(string conStr)
        {
            //Make the connection
            _conn = new MySqlConnection(conStr);

            //Open the connection
            _conn.Open();
        }

        /// <summary>
        /// This method will let the user connect to the database given specific values.
        /// </summary>
        /// <param name="ip">The IP-address of the database (127.0.0.1 for local)</param>
        /// <param name="db">The name of the database</param>
        /// <param name="uid">The user ID</param>
        /// <param name="pw">The password</param>
        public void Connect(string ip, string db, string uid, string pw)
        {
            //Make the connection
            _conn = new MySqlConnection(@"Data Source=" + ip + ";Database=" + db + ";Uid=" + uid + ";Pwd=" + pw);

            //Open the connection
            _conn.Open();
        }

        /// <summary>
        /// This method will execute the given query and will return the result given from the database
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The result given from the database</returns>
        public DataTable Read(string query)
        {
            //Only procede if there is a connection. Return null otherwise.
            if (_conn == null)
            {
                return null;
            }

            //Create the command with the gien query
            _cmd = new MySqlCommand(query, _conn);

            //Load the datatable in the DataTable object.
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the result.
            return _resultTable;
        }

        /// <summary>
        /// This method will return all categories found in the database.
        /// </summary>
        /// <returns>All categories found in the database.</returns>
        public DataTable GetCategories()
        {
            //Invoke Read() with the appropriate query
            return Read("SELECT description FROM category");
        }

        /// <summary>
        /// This method will return all categorie synonyms found in the database.
        /// </summary>
        /// <returns>All categorie synonyms found in the database.</returns>
        public DataTable GetCategorySynonyms()
        {
            //Invoke Read() with the appropriate query
            return Read("SELECT description FROM category_synonym");
        }

        /// <summary>
        /// This method will return the article number found when searching for an SKU, and -1 otherwise.
        /// </summary>
        /// <param name="sku">The SKU that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfSKU(string sku)
        {
            //Create the query
            string query = @"SELECT * FROM sku WHERE sku=@SKU";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@SKU", sku);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return Int32.Parse(_resultTable.Rows.Count > 0 ? _resultTable.Rows[0].ToString() : "-1");
        }

        /// <summary>
        /// This method will return the article number found when searching for an EAN, and -1 otherwise.
        /// </summary>
        /// <param name="ean">The EAN that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfEAN(string ean)
        {
            //Create the query
            string query = @"SELECT * FROM ean WHERE ean=@ean";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@ean", ean);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return Int32.Parse(_resultTable.Rows.Count > 0 ? _resultTable.Rows[0].ToString() : "-1");
        }

        /// <summary>
        /// This method will return the article number found when searching for a title, and -1 otherwise.
        /// </summary>
        /// <param name="title">The title that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfTitle(string title)
        {
            //Create the query
            string query = @"SELECT * FROM title WHERE title LIKE '%@title%'";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@title", title);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return Int32.Parse(_resultTable.Rows.Count > 0 ? _resultTable.Rows[0].ToString() : "-1");
        }

        /// <summary>
        /// This method will return the article number found when searching for a partial SKU, and -1 otherwise.
        /// </summary>
        /// <param name="sku">The SKU that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfPartialSKU(string sku)
        {
            //Create the query
            string query = @"SELECT * FROM sku WHERE sku LIKE '%@sku%'";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@SKU", sku);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return Int32.Parse(_resultTable.Rows.Count > 0 ? _resultTable.Rows[0].ToString() : "-1");
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
                           "WHERE article.id = " + id;

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

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public void SendToResidu(Product p)
        {
            //Create a dictionary containing column names and values
            Dictionary<string, string> col_vals = new Dictionary<string, string>();

            //Add names/values to the dictionary
            col_vals.Add("title", p.Title);
            col_vals.Add("description", p.Description);
            col_vals.Add("image", p.Image);
            col_vals.Add("category", p.Category);
            col_vals.Add("ean", p.EAN);
            col_vals.Add("sku", p.SKU);
            col_vals.Add("brand", p.Brand);

            //Declare string for the columns and values
            string columns = "";
            string values = "";

            //First entry should be treated special (without a comma), therefore catch it using this bool
            bool first = true;

            //Loop through each KeyValuePair
            foreach (KeyValuePair<string, string> pair in col_vals)
            {
                //Only add the pair if the value is valid
                if (!pair.Value.Equals(""))
                {
                    //Special treatment for the first entry
                    if (first)
                    {
                        //Add values without comma
                        columns += pair.Key;
                        values += "@" + pair.Key.ToUpper();

                        //Set bool to false because the first has passed
                        first = false;

                        //Skip the rest of the loop
                        continue;
                    }

                    //Add values with a comma
                    columns += ", " + pair.Key;
                    values += ", @" + pair.Key.ToUpper();
                }
            }

            //Build the query
            string query = @"INSERT INTO residu (" + columns + ") VALUES (" + values + ")";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add parameters to the command
            //Loop through each KeyValuePair
            foreach (KeyValuePair<string, string> pair in col_vals)
            {
                //Only add the pair if the value is valid
                if (!pair.Value.Equals(""))
                {
                    //Make an exception for EAN, which is the only integer
                    if (pair.Key.Equals("ean"))
                    {
                        _cmd.Parameters.AddWithValue("@" + pair.Key.ToUpper(), Int64.Parse(pair.Value));
                        continue;
                    }
                    _cmd.Parameters.AddWithValue("@" + pair.Key.ToUpper(), Util.ToLiteral(pair.Value));
                }
            }

            //Execute the query
            _cmd.ExecuteNonQuery();

        }

        /// This method will close the connection with the database.
        /// </summary>
        public void Close()
        {
            //Close the connection.
            _conn.Close();
        }
    }
}
