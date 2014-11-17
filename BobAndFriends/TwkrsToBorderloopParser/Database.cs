using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace TwkrsToBorderloopParser
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
            int i = 5;
            //Make the connection
            _conn = new MySqlConnection(@"Data Source=" + ip + ";Database=" + db + ";Uid=" + uid + ";Pwd=" + pw + ";Allow User Variables=True");
            i *= 2;
            //Open the connection
            _conn.Open();
        }

        /// <summary>
        /// This method will execute the given query and will return the result given from the database
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The result given from the database</returns>
        public DataTable Read(string query, Dictionary<string, string> param = null)
        {
            DataTable _resultTable = new DataTable();

            //Only procede if there is a connection. Return null otherwise.
            if (_conn == null)
            {
                return null;
            }

            //Create the command with the gien query
            _cmd = new MySqlCommand(query, _conn);

            if (!(param == null))
            {
                foreach (KeyValuePair<string, string> pair in param)
                {
                    _cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                }
            }
            //We need MySqlDataAdapter to store all rows in the datatable
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(_cmd))
            {
                adapter.Fill(_resultTable);
            }

            //Return the result.
            return _resultTable;
        }


        public void InstertIntoSKU(int aid, string sku)
        {
            string query = "INSERT INTO sku VALUES (@SKU, @AID)";
            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@SKU", sku);
            _cmd.Parameters.AddWithValue("@AID", aid);

            _cmd.ExecuteNonQuery();
        }
        
        public void RemoveSKU(string sku)
        {
            string query = "DELETE FROM sku WHERE sku = @SKU";
            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@SKU", sku);

            _cmd.ExecuteNonQuery();
        }

        public void InstertIntoCatArticle(int catId, int articleId)
        {
            string query = "INSERT INTO cat_article VALUES (@CATID, @AID)";
            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@CATID", catId);
            _cmd.Parameters.AddWithValue("@AID", articleId);

            _cmd.ExecuteNonQuery();
        }
    }
}
