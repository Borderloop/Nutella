using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

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

        private int _articleID;

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

        /// <summary>
        /// Return whether the connection is open or not.
        /// </summary>
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
        /// Gets the article id for a given table and record.
        /// </summary>
        /// <param name="table">The table to search the article id in.</param>
        /// <param name="column">The column that is searched for a value.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public int GetArticleNumber(string table, string column, string value)
        {
            string query = "SELECT * FROM  " + table + " WHERE " + column + " = @VALUE";
            //string query = "SELECT * FROM  " + table + " WHERE " + column + " = '" + value + "'";
            //Create the connection.
            _cmd = new MySqlCommand(query, _conn);
            MySqlParameter val = _cmd.Parameters.Add("@VALUE", MySqlDbType.VarChar, 20);
            val.Value = value;

            DataTable _resultTable = new DataTable();
            _resultTable.Load(_cmd.ExecuteReader());

            //Return the article number if found, -1 otherwise.
            if (_resultTable.Rows.Count > 0)
            {
                return Convert.ToInt32(_resultTable.Rows[0]["article_id"]);
            }
            else{
                return -1;
            }

        }

        public bool CheckIfBrandExists(string brand)
        {
            DataTable _resultTable = new DataTable();

            //Create the query
            string query = "SELECT brand FROM article WHERE brand=@BRAND LIMIT 1;";


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

        public int GetAIDFromUAC(Product record)
        {
            DataTable resultTable = new DataTable();

            string query = "SELECT article_id FROM product WHERE affiliate_name='" + record.Affiliate + "' AND affiliate_unique_id='" + record.AffiliateProdID + "'";

            _cmd = new MySqlCommand(query, _conn);

            object obj = _cmd.ExecuteScalar();
            int articleId = ((obj != null && obj != DBNull.Value) ? articleId = Convert.ToInt32(obj) : -1);
            return articleId;
        }


        /// <summary>
        /// Gets table data for a given table and article id.
        /// </summary>
        /// <param name="aid">The article id to search for.</param>
        /// <param name="columns">The columns to select</param>
        /// <param name="table">The table to get the results from</param>
        /// <returns></returns>
        public DataTable GetTableForArticle(int aid, List<string> columns, string table, string whereClause)
        {
            string query = GenerateSelectQuery(columns, table, whereClause, aid.ToString());
            return Read(query);
        }

        /// <summary>
        /// Generates a select query.
        /// </summary>
        /// <param name="columns">The columns to select.</param>
        /// <param name="table">The table to select from.</param>
        /// <param name="whereClause">The where clause that is used.</param>
        /// <param name="value">The value for the where clause.</param>
        /// <returns></returns>
        private string GenerateSelectQuery(List<string> columns, string table, string whereClause = null, string value = null)
        {
            // Base of query
            string query = "SELECT ";

            // Add columns to the query
            foreach (string column in columns)
            {
                query += column + ", ";
            }

            // Remove the last comma and build query further.
            query = query.Remove(query.Length - 2, 2) + " FROM " + table;

            // If the where clause is not empty, a where clause is present so add this also.
            if (whereClause != null)
            {
                query += " WHERE " + whereClause + " = '" + value + "'";
            }

            return query;
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
        public void AddForMatch(String table, String value, int id, int countryId)
        {
            if (table.Equals("title"))
            {
                String query = "INSERT INTO " + table + " (title, country_id, article_id) VALUES (@VALUE, @COUNTRYID, @ID)";

                MySqlCommand _cmd = new MySqlCommand(query, _conn);
                _cmd.Parameters.AddWithValue("@VALUE", value);
                _cmd.Parameters.AddWithValue("@ID", id);
                _cmd.Parameters.AddWithValue("@COUNTRYID", countryId);

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

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public void SendTo(Product p, string tableName, bool rerun = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
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
        public void SaveNewArticle(Product Record, int countryId)
        {
            // We want to insert everything at once to reduce proccessing time, start building the query here.

            // First insert data into the article table and get article id.
            // EAN and title are always present at this point, so add these no matter what.
            // Also add the title to the title synonym table by returning the title id.
            // After that, save the product data right away. Since it's a new article, it won't have any product data yet. 
            string query = "INSERT INTO article (description, brand, image_loc)" +
                           "VALUES (@DESCRIPTION, @BRAND, @IMAGE_LOC);\n " +
                           "SELECT LAST_INSERT_ID() INTO @articleId;\n" +
                           "INSERT INTO ean VALUES (@EAN, @articleId);\n" +
                           "INSERT INTO title (title, country_id, article_id) VALUES (@TITLE, @COUNTRYID, @articleId);\n" +
                           "SELECT LAST_INSERT_ID() INTO @titleId;\n" +
                           "INSERT INTO title_synonym(title, title_id) VALUES (@TITLE, @titleId);\n" +
                           "INSERT INTO product (article_id, ship_time, ship_cost, price, webshop_url, direct_link, affiliate_name, affiliate_unique_id) VALUES (@articleId, @SHIPTIME, @SHIPCOST, @PRICE, @WEBSHOP_URL, @DIRECT_LINK, @AFNAME, @AFID);\n";
                           

            // We need to know if there is an SKU and if so, add it to the query.
            if (Record.SKU != "")
            {
                query += "INSERT INTO sku VALUES (@SKU, @articleId);\n";
            }
            // We need to return the article ID for saving product data later on.
            query += "SELECT @articleId";

            // Create a new command and add all the parameters;
            _cmd = new MySqlCommand(query, _conn);
            _cmd.Parameters.AddWithValue("@DESCRIPTION", Record.Description);
            _cmd.Parameters.AddWithValue("@BRAND", Record.Brand);
            _cmd.Parameters.AddWithValue("@IMAGE_LOC", Record.Image_Loc);
            _cmd.Parameters.AddWithValue("@EAN", Record.EAN);
            _cmd.Parameters.AddWithValue("@TITLE", Record.Title);
            _cmd.Parameters.AddWithValue("@COUNTRYID", countryId);
            _cmd.Parameters.AddWithValue("@SHIPTIME", Record.DeliveryTime);
            _cmd.Parameters.AddWithValue("@SHIPCOST", Record.DeliveryCost);
            _cmd.Parameters.AddWithValue("@PRICE", Record.Price);
            _cmd.Parameters.AddWithValue("@WEBSHOP_URL", Record.Webshop);
            _cmd.Parameters.AddWithValue("@DIRECT_LINK", Record.Url);
            _cmd.Parameters.AddWithValue("@AFNAME", Record.Affiliate);
            _cmd.Parameters.AddWithValue("@AFID", Record.AffiliateProdID);
            if (Record.SKU != "") { _cmd.Parameters.AddWithValue("@SKU", Record.SKU); }

            try
            {
                // Execute query and return the article ID for the article that just got inserted at the same time.
                _articleID = Convert.ToInt32(_cmd.ExecuteScalar());
            }
            catch (MySqlException e)
            {
                Statics.Logger.WriteLine("Saving new article failed for the follwing product:\n" +
                                         "Title:    " + Record.Title + "\n" +
                                         "EAN:      " + Record.EAN + "\n" +
                                         "Which produced the following error:\n" + e);
            }
        }

        /// This method will close the connection with the database.
        /// </summary>
        public void Close()
        {
            //Close the connection.
            _conn.Close();
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
 

        /// <summary>
        /// Used for simple select statements.
        /// </summary>
        /// <param name="columns">The columns to select</param>
        /// <returns></returns>
        public DataTable Select(List<string> columns, string table, string whereClause = null, string value = null) 
        {
            string query = GenerateSelectQuery(columns, table, whereClause, value);

            return Read(query);
        }

        public DataTable GetSuggestedProducts(int productID)
        {
            //parse productID to a string
            string id = productID.ToString();

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
                    query += "(" + articleId + ", " + vBobId + "), ";
            }
            // Remove last comma;
            query = query.Remove(query.Length - 2, 2);

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

        /// <summary>
        /// Saves product data for a given record.
        /// </summary>
        /// <param name="Record">The record of which the product data will be saved.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        public void SaveProductData(Product Record)
        {
            string query = "INSERT INTO product (article_id, ship_time, ship_cost, price, webshop_url, direct_link, affiliate_name, affiliate_unique_id) VALUES (@AID, @SHIPTIME, @SHIPCOST, @PRICE, @WEBSHOP_URL, @DIRECT_LINK, @AFNAME, @AFID)";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);

            decimal? deliveryCost;
            decimal? price;

            try
            {
                deliveryCost = decimal.Parse(Record.DeliveryCost);
            }
            catch
            {
                deliveryCost = null;
            }
            try
            {
                price = decimal.Parse(Record.Price);
            }
            catch
            {
                price = null;
            }

            _cmd.Parameters.AddWithValue("@AID", _articleID);
            _cmd.Parameters.AddWithValue("@SHIPTIME", Record.DeliveryTime);
            _cmd.Parameters.AddWithValue("@SHIPCOST", deliveryCost);
            _cmd.Parameters.AddWithValue("@PRICE", price);
            _cmd.Parameters.AddWithValue("@WEBSHOP_URL", Record.Webshop);
            _cmd.Parameters.AddWithValue("@DIRECT_LINK", Record.Url);
            _cmd.Parameters.AddWithValue("@AFNAME", Record.Affiliate);
            _cmd.Parameters.AddWithValue("@AFID", Record.AffiliateProdID);

            try
            {
                _cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Statics.Logger.WriteLine("Saving product data failed for the follwing product:\n" +
                                         "Title:    " + Record.Title + "\n" +
                                         "EAN:      " + Record.EAN + "\n" +
                                         "Which produced the following error:\n" + e);
            }
        }

        /// <summary>
        /// Gets the product data for a given record, used for comparison.
        /// </summary>
        /// <param name="Record">The record to get product data for.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        /// <returns></returns>
        public DataTable GetProductData(Product Record, int aId)
        {
            string query = "SELECT ship_time AS DeliveryTime, ship_cost AS DeliveryCost, price AS Price, direct_link AS Url FROM product WHERE article_id = " + aId + " AND webshop_url = '" + Record.Webshop + "'";

            DataTable dt = Read(query);
            return dt;
        }

        /// <summary>
        /// Updates product data if changes are found.
        /// </summary>
        /// <param name="query">The query to be executed to the database</param>
        public void UpdateProductData(string query)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            try
            {
                _cmd.ExecuteNonQuery();
            }
            catch (MySqlException){
                Statics.Logger.WriteLine("Updating product data produced error for the below query: \n" + query);
            }
            Statics.Logger.WriteLine("Finished updating product data in: " + sw.Elapsed);
            sw.Stop();
        }

        public int GetCountryId(string webshop)
        {
            string query = "SELECT country_id FROM webshop WHERE url = '" + webshop + "'";

            MySqlCommand _cmd = new MySqlCommand(query, _conn);
            MySqlDataReader rdr = _cmd.ExecuteReader();
            rdr.Read();
            int countryId = rdr.GetInt32(0);
            rdr.Close();

            return countryId;
        }

        public DataTable GetDuplicateEANs()
        {
            return Read("SELECT ean.ean, ean.article_id, COUNT(*) FROM ean GROUP BY ean.ean HAVING COUNT(*) > 1");
        }

        public DataTable GetDuplicateSKUs()
        {
            return Read("SELECT sku.sku, sku.article_id, COUNT(*) FROM sku GROUP BY sku.sku HAVING COUNT(*) > 1");
        }

        public DataTable GetDuplicateTitles()
        {
            return Read("SELECT title_synonym.title, title_synonym.title_id, COUNT(*) FROM title_synonym GROUP BY title_synonym.title HAVING COUNT(*) > 1");
        }

        public int GetCorrectAIDFromTitle(string title)
        {
            return Read("SELECT title.article_id from title where title.title = '" + title + "'").Rows[0].Field<int>("article_id");
        }

        public DataTable GetAIDsFromEAN(string ean, int correctAID)
        {
            return Read("Select ean.article_id from ean where ean.article_id != " + correctAID + " AND ean.ean = " + ean);
        }

        public DataTable GetAIDsFromSKU(string sku, int correctAID)
        {
            return Read("Select sku.article_id from sku where sku.article_id != " + correctAID + " AND sku.sku = " + sku);
        }

        public DataTable GetAIDsFromTitle(string title, int correctAID)
        {
            return Read("Select title.article_id from title where title.article_id != " + correctAID + " AND title.title = '" + title + "'");
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
    }
}
