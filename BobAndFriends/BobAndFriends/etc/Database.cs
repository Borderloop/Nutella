using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace BobAndFriends
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
        /// The constructor
        /// </summary>
        private Database()
        {
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

        public bool isConnected
        {
            get { return _conn.State == ConnectionState.Open; }
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
            _conn = new MySqlConnection(@"Data Source=" + ip + ";Database=" + db + ";Uid=" + uid + ";Pwd=" + pw + ";Allow User Variables=True");

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
            DataTable _resultTable = new DataTable();

            //Only procede if there is a connection. Return null otherwise.
            if (_conn == null)
            {
                return null;
            }

            //Create the command with the gien query
            _cmd = new MySqlCommand(query, _conn);

            //We need MySqlDataAdapter to store all rows in the datatable
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(_cmd))
            {
                adapter.Fill(_resultTable);
            }

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

        public int CountRows(string tableName)
        {
            //Create the query
            string query = "SELECT COUNT(*) FROM " + tableName;

            //Create an integer to return
            int count;

            //Create the command
            _cmd = new MySqlCommand(query, _conn);

            //Execute the command and store the value in an object
            object obj = _cmd.ExecuteScalar();

            //Return the value of the object if it's not null, -1 otherwise.
            return (count = ((obj != null || obj != DBNull.Value) ? count = Convert.ToInt32(obj) : -1));
        }

        /// <summary>
        /// This method will return the article number found when searching for an SKU, and -1 otherwise.
        /// </summary>
        /// <param name="sku">The SKU that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfSKU(string sku)
        {
            DataTable _resultTable = new DataTable();

            //Create the query
            string query = @"SELECT * FROM sku WHERE sku=@SKU";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@SKU", sku);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return (_resultTable.Rows.Count > 0 ? (int)_resultTable.Rows[0]["article_id"] : -1);
        }

        /// <summary>
        /// This method will return the article number found when searching for an EAN, and -1 otherwise.
        /// </summary>
        /// <param name="ean">The EAN that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfEAN(string ean)
        {
            DataTable _resultTable = new DataTable();

            //Create the query
            string query = @"SELECT * FROM ean WHERE ean=@ean";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@ean", ean);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return (_resultTable.Rows.Count > 0 ? (int)_resultTable.Rows[0]["article_id"] : -1);
        }

        /// <summary>
        /// This method will return the article number found when searching for a title, and -1 otherwise.
        /// </summary>
        /// <param name="title">The title that needs to be matched.</param>
        /// <returns>The found article number, -1 otherwise</returns>
        public int GetArticleNumberOfTitle(string title)
        {
            DataTable _resultTable = new DataTable();

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
            DataTable _resultTable = new DataTable();

            //Create the query
            string query = @"SELECT * FROM title WHERE title LIKE '%@SKU%'";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@SKU", sku);

            //Load the result in a datatable
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            return Int32.Parse(_resultTable.Rows.Count > 0 ? _resultTable.Rows[0].ToString() : "-1");
        }

        public bool CheckIfBrandExists(string brand)
        {
            DataTable _resultTable = new DataTable();

            //Create the query
            string query = "SELECT brand FROM article WHERE brand=@BRAND";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add the parameters to the command.
            _cmd.Parameters.AddWithValue("@BRAND", brand);

            //Load the result in a datatable
            //We need MySqlDataAdapter to store all rows in the datatable
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(_cmd))
            {
                adapter.Fill(_resultTable);
            }

            //Return the article number if found, -1 otherwise.
            return _resultTable.Rows.Count > 0;
        }

        /// <summary>
        /// Gets the data from the article table for the given article.
        /// </summary>
        /// <param name="aid">The article id to get the data from</param>
        /// <returns></returns>
        public DataTable GetArticleTableForArticle(int aid)
        {
            string query = "SELECT brand AS 'article-Brand', description AS 'article-Description', " +
                           "image_loc AS 'article-Image_Loc' FROM article WHERE id = " + aid;

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// Gets the data from the ean table for the given article.
        /// </summary>
        /// <param name="aid">The article id to get the data from</param>
        /// <returns></returns>
        public DataTable GetEANTableForArticle(int aid)
        {
            string query = "SELECT ean AS 'ean-EAN' FROM ean WHERE article_id = " + aid;

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// Gets the data from the sku table for the given article.
        /// </summary>
        /// <param name="aid">The article id to get the data from</param>
        /// <returns></returns>
        public DataTable GetSKUTableForArticle(int aid)
        {
            string query = "SELECT sku AS 'sku-SKU' FROM sku WHERE article_id = " + aid;

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// Gets the data from the title_synonym table for the given article.
        /// </summary>
        /// <param name="aid">The article id to get the data from</param>
        /// <returns></returns>
        public DataTable GetTitleSynonymTableForArticle(int aid)
        {
            string query = "SELECT title_synonym.title, occurrences, title_id FROM title_synonym " +
                           "INNER JOIN title ON title.id = title_synonym.title_id " +
                           "WHERE title.article_id = " + aid;

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// This method updates the occurrences column for a given title
        /// </summary>
        /// <param name="titleId">Id of the title belonging to the title synonym</param>
        /// <param name="occurrences">Amount of occurences, this will get updated</param>
        /// <param name="title">The title to be updated</param>
        public void UpdateTitleSynonymOccurrences(int titleId, int occurrences, string title)
        {
            string query = "UPDATE title_synonym SET occurrences = @OCCURRENCES WHERE title_id = @TITLEID AND title = @TITLE";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@TITLEID", titleId);
            _cmd.Parameters.AddWithValue("@TITLE", title);
            _cmd.Parameters.AddWithValue("@OCCURRENCES", occurrences);

            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the occurrences for a given title
        /// </summary>
        /// <param name="titleId">The is of the title to get the occurrences from</param>
        /// <returns></returns>
        public int GetOccurrencesForTitle(int titleId)
        {
            string query = "SELECT occurrences FROM title_synonym AS ts " +
                           "INNER JOIN title ON title.id = ts.title_id " +
                           "WHERE title.id = @TITLEID AND ts.title = title.title";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@TITLEID", titleId);
            MySqlDataReader rdr = _cmd.ExecuteReader();
            rdr.Read();
            int occurrences = rdr.GetInt32(0);
            rdr.Close();

            return occurrences;
        }

        /// <summary>
        /// Updates the title in the title table. This is done if another title has a higher occurrence then the one in the title table.
        /// </summary>
        /// <param name="titleId">The id of the title to be updated</param>
        /// <param name="title">The new title</param>
        public void UpdateTitle(int titleId, string title)
        {
            string query = "UPDATE title SET title = @TITLE " +
                           "WHERE id = @TITLEID";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@TITLE", title);
            _cmd.Parameters.AddWithValue("@TITLEID", titleId);

            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the title id for an article
        /// </summary>
        /// <param name="aId">The article id</param>
        /// <param name="countryId">The country id for the title, this is needed because every title is </param>
        /// <returns></returns>
        public int GetTitleId(int aId, int countryId)
        {
            string query = "SELECT id FROM title WHERE article_id = " + aId + " AND country_id = " + countryId;

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            MySqlDataReader rdr = _cmd.ExecuteReader();
            rdr.Read();
            int aid = rdr.GetInt32(0);
            rdr.Close();

            return aid;
        }

        /// <summary>
        /// Inserts a new title in the title_synonym table if a new title is found.
        /// </summary>
        /// <param name="titleId">The title id which the synonym belongs to</param>
        /// <param name="title">The title to be inserted</param>
        public void InsertNewTitle(int titleId, string title)
        {
            string query = "INSERT INTO title_synonym (title, title_id) VALUES (@TITLE, @TITLEID)";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@TITLE", title);
            _cmd.Parameters.AddWithValue("@TITLEID", titleId);

            _cmd.ExecuteNonQuery();
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
        /// This method adds a record for a found match. This is always for the ean or sku
        /// table. This is because when this code is reached, no record is found for in one of those
        /// tables for an article.
        /// </summary>
        public void AddForMatch(String table, String value, int id)
        {
            if (table.Equals("title"))
            {
                String query = "INSERT INTO " + table + " (title, country_id, article_id) VALUES (@VALUE, 1, @ID)";

                MySqlCommand _cmd = new MySqlCommand(query, _conn);
                _cmd.Parameters.AddWithValue("@VALUE", value);
                _cmd.Parameters.AddWithValue("@ID", id);

                _cmd.ExecuteNonQuery();
            }
            else
            {
                String query = "INSERT INTO " + table + " VALUES (@VALUE, @ID)";

                MySqlCommand _cmd = new MySqlCommand(query, _conn);
                _cmd.Parameters.AddWithValue("@VALUE", value);
                _cmd.Parameters.AddWithValue("@ID", id);

                _cmd.ExecuteNonQuery();
            }
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
        /// Gets the category synonyms for an article
        /// </summary>
        /// <param name="id">The article id</param>
        /// <returns></returns>
        public DataTable GetCategorySynonymsForArticle(int id)
        {
            string query = "SELECT cs.description FROM category_synonym as cs " +
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
            string query = "INSERT into category_synonym VALUES (@ID, @DESCRIPTION)";

            _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@ID", id);
            _cmd.Parameters.AddWithValue("@DESCRIPTION", description);

            _cmd.ExecuteNonQuery();
        }

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public void SendTo(Product p, string tableName, bool rerun = false)
        {
            //Create a dictionary containing column names and values
            Dictionary<string, object> col_vals = new Dictionary<string, object>();

            //Add names/values to the dictionary
            col_vals.Add("title", p.Title);
            col_vals.Add("description", p.Description);
            col_vals.Add("category", p.Category);
            col_vals.Add("ean", p.EAN);
            col_vals.Add("sku", p.SKU);
            col_vals.Add("brand", p.Brand);

            if (tableName.Equals("vbobdata"))
            {
                col_vals.Add("rerun", rerun);
                col_vals.Add("image_loc", p.Image_Loc);
            }
            else
            {
                col_vals.Add("image", p.Image_Loc);
            }

            //Declare string for the columns and values
            string columns = "";
            string values = "";

            //First entry should be treated special (without a comma), therefore catch it using this bool
            bool first = true;

            //Loop through each KeyValuePair
            foreach (KeyValuePair<string, object> pair in col_vals)
            {
                //Only add the pair if the value is valid
                if (!(pair.Value == null))
                {
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
            }

            //Build the query
            string query = @"INSERT INTO " + tableName + " (" + columns + ") VALUES (" + values + ")";

            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);

            //Add parameters to the command
            //Loop through each KeyValuePair
            foreach (KeyValuePair<string, object> pair in col_vals)
            {
                //Only add the pair if the value is valid
                if (!(pair.Value == null))
                {
                    if (!pair.Value.Equals(""))
                    {
                        if (pair.Key.Equals("rerun"))
                        {
                            _cmd.Parameters.AddWithValue("@" + pair.Key.ToUpper(), pair.Value);
                            continue;
                        }
                        _cmd.Parameters.AddWithValue("@" + pair.Key.ToUpper(), Util.ToLiteral((string)pair.Value));
                    }
                }
            }

            //Execute the query
            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Used to save a new article.
        /// </summary>
        /// <param name="Record">The article to be saved</param>
        public void SaveNewArticle(Product Record, int categoryID)
        {
            // First insert data into the article table
            string query = "INSERT INTO article (description, brand, image_loc) " +
                           "VALUES (@DESCRIPTION, @BRAND, @IMAGE_LOC)";

            _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@DESCRIPTION", Record.Description);
            _cmd.Parameters.AddWithValue("@BRAND", Record.Brand);
            _cmd.Parameters.AddWithValue("@IMAGE_LOC", Record.Image_Loc);
            _cmd.ExecuteNonQuery();

            // Then get the article id for the inserted article.
            string query2 = "SELECT LAST_INSERT_ID()";
            MySqlCommand _cmd2 = new MySqlCommand(query2, _conn);
            MySqlDataReader rdr = _cmd2.ExecuteReader();
            rdr.Read();
            int aid = rdr.GetInt32(0);
            rdr.Close();

            // Insert remaining data now the article id is available, but only
            // add them if they're not empty to prevent errors.
            string query3 = "";

            if (Record.EAN != null)
            {
                query3 += "INSERT INTO ean VALUES (@EAN, @AID);";
            }
            if (Record.SKU != "")
            {
                query3 += "INSERT INTO sku VALUES (@SKU, @AID); ";
            }
            if (Record.Title != "")
            {
                query3 += "INSERT INTO title (title, country_id, article_id) VALUES (@TITLE, 1, @AID); " +
                          "SELECT LAST_INSERT_ID() INTO @titleId; " +
                          "INSERT INTO title_synonym(title, title_id) VALUES (@TITLE, @titleId);";
            }
            // Don't execute when none of the records are present, which means
            // the query is empty.
            if (query3 != "")
            {
                MySqlCommand _cmd3 = new MySqlCommand(query3, _conn);
                _cmd3.Parameters.AddWithValue("@AID", aid);
                if (Record.EAN != null) { _cmd3.Parameters.AddWithValue("@EAN", Record.EAN); }
                if (Record.SKU != "") { _cmd3.Parameters.AddWithValue("@SKU", Record.SKU); }
                if (Record.Title != "") { _cmd3.Parameters.AddWithValue("@TITLE", Record.Title); }

                _cmd3.ExecuteNonQuery();
            }



            // Now save the category.
            //string query4 = "INSERT INTO cat_article VALUES(@CATID, @AID)";
            //MySqlCommand _cmd4 = new MySqlCommand(query4, _conn);
            //_cmd4.Parameters.AddWithValue("@CATID", categoryID);
            //_cmd4.Parameters.AddWithValue("@AID", aid);

            //_cmd4.ExecuteNonQuery();

        }

        /// <summary>
        /// This method returns the category id for a record. 
        /// </summary>
        /// <param name="category">The name of the category</param>
        /// <param name="categoryCheck">If this is true, the category is in the category table</param>
        /// <param name="categorySynonymCheck">If this is true, the category is in the category_synonym table</param>
        public int GetCategoryID(string category, bool categoryCheck, bool categorySynonymCheck)
        {
            string query = "";
            if (categoryCheck == true && categorySynonymCheck == false) // Category in category table
            {
                query = "SELECT id FROM category WHERE description = @CATEGORY";
            }
            else if (categoryCheck == false && categorySynonymCheck == true) // Category in category_synonym table
            {
                query = "SELECT id FROM category " +
                        "INNER JOIN category_synonym as cs ON cs.category_id = category.id " +
                        "WHERE cs.description = @CATEGORY ";
            }

            _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@CATEGORY", category);
            int id = 0;
            object obj = _cmd.ExecuteScalar();
            if (obj != null && obj != DBNull.Value)
            {
                id = Convert.ToInt32(obj);
            }
            return id;
        }

        /// This method will close the connection with the database.
        /// </summary>
        public void Close()
        {
            //Close the connection.
            _conn.Close();
        }

        /// <summary>
        /// Gets all records from VBobData which have to be rerunned.
        /// </summary>
        /// <returns>All products that have to be rerunned</returns>
        public DataTable GetRerunnables()
        {
            return Read("SELECT * FROM vbobdata WHERE rerun=1");
        }

        /// <summary>
        /// Returns all products from the residue
        /// </summary>
        /// <returns>All products from the residue</returns>
        public DataTable GetAllProductsFromResidue()
        {
            return Read("Select * FROM residue");
        }

        /// <summary>
        /// This method will get the first-next product from the VBobData table.
        /// </summary>
        /// <returns></returns>
        public DataTable GetNextVBobProduct()
        {
            //Create the query
            string query = "SELECT MIN(id) AS ID, title as Title, ean AS EAN, sku as SKU, brand AS Brand, category AS Category, description as Description, image_loc as ImageLocation FROM vbobdata WHERE NOT rerun = 1 OR rerun IS NULL";

            //Execute it and return the datatable.
            return Read(query);
        }

        public DataTable GetSuggestedProducts(int productID)
        {
            //parse productID to a string
            string id = "" + productID;

            //Create the query
            string query = " SELECT article.id as 'Article ID',"
                + "article.brand as 'Brand', "
                + "article.description as 'Description' ,"
                + "title.title as 'Title',"
                + "vbob_suggested.id as `vBobSug ID`, "
                + "(SELECT GROUP_CONCAT(ean.ean) FROM ean WHERE ean.article_id = article.id) as 'EANs', "
                + "(SELECT GROUP_CONCAT(sku.sku) FROM sku WHERE sku.article_id = article.id) as 'SKUs'"
                + " FROM vbobdata, vbob_suggested"
                + " INNER JOIN article ON  article.id = vbob_suggested.article_id"
                + " LEFT JOIN ean ON ean.article_id = article.id"
                + " LEFT JOIN title ON title.article_id = article.id"
                + " LEFT JOIN sku ON sku.article_id = article.id"
                + " WHERE vbob_suggested.vbob_id = " + id + " GROUP BY vbob_suggested.article_id";

            //Execute it and return the datatable.
            return Read(query);
        }

        public void DeleteFromVbobData(int id)
        {
            string query = "DELETE FROM vbob_suggested WHERE vbob_suggested.vbob_id = " + id + ";";
            query += "DELETE FROM vbobdata WHERE vbobdata.id = " + id + ";";
            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();
        }

        public void RerunVbobEntry(int id, string[] updates)
        {
            string query = "UPDATE vbobdata SET rerun = 1, title = @TITLE"
                + ", ean = @EAN"
                + ", sku = @SKU"
                + ", brand = @BRAND"
                + ", category = @CATEGORY"
                + ", description = @DESCRIPTION"
                + ", image_loc = @IMAGELOC"
                + " WHERE id = " + id;

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@TITLE", updates[0]);        
            _cmd.Parameters.AddWithValue("@SKU", updates[1] == " " ? null : updates[1]);
            _cmd.Parameters.AddWithValue("@BRAND", updates[2]);
            _cmd.Parameters.AddWithValue("@CATEGORY", updates[3]);
            _cmd.Parameters.AddWithValue("@DESCRIPTION", updates[4]);
            _cmd.Parameters.AddWithValue("@IMAGELOC", updates[5]);
            _cmd.Parameters.AddWithValue("@EAN", updates[6]);

            _cmd.ExecuteNonQuery();
        }

        public void DeleteFromResidue(Product p)
        {
            string query = "DELETE FROM residue WHERE residue.title = " + p.Title
                + " AND residue.description = " + p.Description
                + " AND residue.category = " + p.Category
                + " AND residue.brand = " + p.Brand
                + " AND residue.ean = " + p.EAN
                + " AND residue.sku = " + p.SKU;
            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the most relevant matches for a title from a brand, used for visual bob.
        /// The returned boolean is used to check if a record should be saved as new article.
        /// </summary>
        /// <param name="Record">The record to find relevant matches for</param>
        public bool GetRelevantMatches(Product Record)
        {
            //First get the last inserted id, which is the id from the record in the vbobdata table.
            string query = "SELECT LAST_INSERT_ID()";
            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            MySqlDataReader rdr = _cmd.ExecuteReader();
            rdr.Read();
            int vBobId = rdr.GetInt32(0);
            rdr.Close();

            //Get the most relevant matches for the given product and return their article id's.
            string query2 = "SELECT article.id FROM article " +
                           "INNER JOIN title ON title.article_id = article.id " +
                           "WHERE title.id IN (SELECT title_id FROM title_synonym as ts " +
                                              "INNER JOIN title ON title.id = ts.title_id " +
                                              "WHERE MATCH(ts.title) AGAINST ('" + Record.Title + "') " +
                                              "GROUP BY title.title " +
                                              "ORDER BY MATCH(ts.title) AGAINST ('" + Record.Title + "'))" +
                           "AND article.brand = '" + Record.Brand + "' " +
                           "LIMIT 10";

            List<int> articleIds = new List<int>();

            MySqlCommand _cmd2 = new MySqlCommand(query2, _conn);
            MySqlDataReader rdr2 = _cmd2.ExecuteReader();

            while (rdr2.Read())
            {
                articleIds.Add(rdr2.GetInt32(0));
            }

            rdr2.Close();


            bool match;

            //Invoke method to save suggested matches to database if matches are found
            if (articleIds.Count() > 0)
            {
                InsertInVBobSuggested(vBobId, articleIds);
                match = true;
            }
            else // Else, no matches are found. Save this record to the database.
            {
                match = false;
            }

            return match;
        }

        /// <summary>
        /// Inserts the found matches for the product in the vbob_suggested table
        /// </summary>
        /// <param name="vBobId">The id of the record as stored in the vbobdata table</param>
        /// <param name="articleIds">List of article_ids that match the record. First id has the most relevance, last is the least.</param>
        private void InsertInVBobSuggested(int vBobId, List<int> articleIds)
        {
            string query = "INSERT INTO vbob_suggested (article_id, vbob_id) VALUES ";
            int lastId = articleIds[articleIds.Count - 1];
            foreach (int articleId in articleIds)
            {
                if (articleId != lastId)
                {
                    query += "(" + articleId + ", " + vBobId + "), ";
                }
                else
                {
                    query += "(" + articleId + ", " + vBobId + ")";
                }
            }

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();

        }

        public bool HasArticles()
        {
            //Create the query
            string query = "SELECT COUNT(*) FROM article";

            //Create the command
            _cmd = new MySqlCommand(query, _conn);

            //Execute the command and store the value in an object
            object obj = _cmd.ExecuteScalar();

            //Return the value of the object if it's not null, -1 otherwise.
            return (((obj != null || obj != DBNull.Value) ? Convert.ToInt32(obj) > 0 : false));

        }

        public void SaveProductData(Product Record, int aId)
        {
            string query = "INSERT INTO product (article_id, ship_time, ship_cost, price, webshop_url, direct_link) VALUES (@AID, @SHIPTIME, @SHIPCOST, @PRICE, @WEBSHOP_URL, @DIRECT_LINK)";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@AID", aId);
            _cmd.Parameters.AddWithValue("@SHIPTIME", Record.DeliveryTime);
            _cmd.Parameters.AddWithValue("@SHIPCOST", Record.DeliveryCost == "" ? -1 : Convert.ToDouble(Record.DeliveryCost));
            _cmd.Parameters.AddWithValue("@PRICE", Record.Price == "" ? -1 : Convert.ToDouble(Record.Price));
            _cmd.Parameters.AddWithValue("@WEBSHOP_URL", Record.Webshop);
            _cmd.Parameters.AddWithValue("@DIRECT_LINK", Record.Url);

            _cmd.ExecuteNonQuery();
        }

        public DataTable GetProductData(Product Record, int aId)
        {
            string query = "SELECT ship_time AS DeliveryTime, ship_cost AS DeliveryCost, price AS Price, direct_link AS Url FROM product WHERE article_id = " + aId + " AND webshop_url = '" + Record.Webshop + "'";

            DataTable dt = Read(query);
            return dt;
        }

        public void UpdateProductData(string query)
        {
            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();
        }

        public DataTable GetDuplicateEANs()
        {
            return Read("SELECT ean.ean, ean.article_id, COUNT(*) FROM ean GROUP BY ean.ean HAVING COUNT(*) > 1");
        }

        public DataTable GetDuplicateSKUs()
        {
            return Read("SELECT sku.sku, sku.article_id, COUNT(*) FROM sku GROUP BY sku.sku HAVING COUNT(*) > 1");
        }

        public DataTable GetAIDsFromEAN(string ean, int correctAID)
        {
            return Read("Select ean.article_id from ean where ean.article_id != " + correctAID + " AND ean.ean = " + ean);
        }

        public DataTable GetAIDsFromSKU(string ean, int correctAID)
        {
            return Read("Select sku.article_id from sku where sku.article_id != " + correctAID + " AND sku.sku = " + ean);
        }

        public void UpdateProductAID(int wrongAID, int correctAID)
        {
            string query = "UPDATE product SET article_id = @CORRECTID WHERE article_id = @WRONGID";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@CORRECTID", correctAID);
            _cmd.Parameters.AddWithValue("@WRONGID", wrongAID);

            _cmd.ExecuteNonQuery();
        } 

      
        public void AddTitleSynonym(int wrongaid, int correctaid)
        {
            string titleName = Read("SELECT title.title FROM title WHERE title.article_id = " + wrongaid).Rows[0].Field<String>("title");
            int titleID = Read("SELECT title.id FROM title WHERE title.article_id = " + correctaid).Rows[0].Field<Int32>("id");

            //First check if the title already exists in the database
            DataTable dt = Read("SELECT title_synonym.occurrences FROM title_synonym WHERE title_synonym.title_id = " + titleID + " AND title_synonym.title = \"" + titleName + "\"");
            if(dt.Rows.Count > 0)
            {
                //Clearly we found a match, so we can now update the occurences which are stored in the datatable
                UpdateTitleSynonymOccurrences(titleID, (int)dt.Rows[0].Field<Int32>("occurrences") + 1, titleName);

                //We're done.
                return;
            }

            //We didn't find a match, therefore we insert a new title synonym.
            string query = "INSERT INTO title_synonym (title, title_id, occurrences) VALUES (@TITLENAME, @TITLEID, 1)";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@TITLENAME", titleName);
            _cmd.Parameters.AddWithValue("@TITLEID", titleID);

            _cmd.ExecuteNonQuery();        
        
        }

        public void DeleteArticle(int articleID)
        {
            //Delete all entries from the given article ID
            string query = "DELETE FROM ean WHERE ean.article_id = " + articleID + "; ";
            query += "DELETE FROM sku WHERE sku.article_id = " + articleID + "; ";
            
            //Nasty way of getting title_id very quick.
            query += "DELETE FROM title_synonym WHERE title_synonym.title_id = " + Read("SELECT title.id FROM title WHERE title.article_id = " + articleID).Rows[0]["id"] + "; ";
            query += "DELETE FROM title WHERE title.article_id = " + articleID + "; ";
            query += "DELETE FROM article WHERE article.id = " + articleID + "; ";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            _cmd.ExecuteNonQuery();
        }

        public void Cleanup()
        {
            //string query 
        }
    }
}
