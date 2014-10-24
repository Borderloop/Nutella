using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace CategoryMatcher
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

        public void InsertIntoCatSynonyms(int catid, string description, string web_url)
        {
            string query = "INSERT INTO category_synonym VALUES (@CATID, @CATDESCR, @WEB_URL)";

            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@CATID", catid);
            _cmd.Parameters.AddWithValue("@CATDESCR", description);
            _cmd.Parameters.AddWithValue("@WEB_URL", web_url);

            _cmd.ExecuteNonQuery();
        }

        public DataTable getCategoryInkomend(string web_url)
        {
            return Read("Select category,web_url from residue where web_url LIKE '%" + web_url + "%'");
        }

        public DataTable LinkedProductCategory(String web_url)
        {
            return Read("SELECT * FROM residue WHERE web_url LIKE '%" + web_url + "%' LIMIT 10");
        }
       
        public void bah()
        {
            List<string> synonyms = new List<string>();

            string query = "SELECT id FROM title WHERE article_id = @ARTICLEID";
            DataTable titleTable = new DataTable();

            using(MySqlConnection connection = new MySqlConnection("some connectionstring"))
            {
                using(MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ARTICLEID", 27);
                    using(MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(titleTable);
                    }
                }
            }

            string query2 = "SELECT title FROM title_synonym WHERE title_id = @TITLEID";
            foreach(DataRow row in titleTable.Rows)
            {
                int titleId = (int)row["id"];
                DataTable synonymTable = new DataTable();

                using (MySqlConnection connection = new MySqlConnection("some connectionstring"))
                {
                    using (MySqlCommand command = new MySqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("TITLEID", titleId);
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(synonymTable);
                        }
                    }
                }
              
                foreach(DataRow synonymRow in synonymTable.Rows)
                {
                    synonyms.Add((string)synonymRow["title"]);
                }
            }

        }
    }
}
