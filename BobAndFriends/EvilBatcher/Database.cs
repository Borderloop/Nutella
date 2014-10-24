using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace EvilBatcher
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

        private StreamWriter _logger;

        /// <summary>
        /// The constructor
        /// </summary>
        private Database()
        {
            this._logger = new StreamWriter("C://BorderSoftware//BobAndFriends//EvilBatcher//log//log.txt");
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

        public void InsertIntoCatSynonyms(int catid, string description)
        {
            string query = "INSERT INTO category_synonym VALUES (@CATID, @CATDESCR, @WEB_URL)";

            _cmd = new MySqlCommand(query, _conn);

            _cmd.Parameters.AddWithValue("@CATID", catid);
            _cmd.Parameters.AddWithValue("@CATDESCR", description);
            _cmd.Parameters.AddWithValue("@WEB_URL", "www.borderloop.nl");

            try
            {
                _cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                _logger.WriteLine(e.ToString());
            }
        }

        public void DeleteFromCategoryTemp(int id)
        {
            string query = "DELETE FROM categorytemp WHERE id = " + id;
            _cmd = new MySqlCommand(query, _conn);
            _cmd.ExecuteNonQuery();
        }
        public DataTable GetBorderTopLayer()
        {
            return Read("SELECT description, id FROM category WHERE called_by = 0");
        }

        public DataTable GetBorderChildrenFromId(int id)
        {
            return Read("SELECT description, id FROM category where called_by = " + id);
        }

        public DataTable GetEvilTopLayer()
        {
            return Read("SELECT description, id FROM categorytemp WHERE called_by IS NULL");
        }

        public DataTable GetEvilChildrenFromId(int id)
        {
            return Read("SELECT description, id FROM categorytemp where called_by = " + id);
        }

        public void CloseLogger()
        {
            _logger.Close();
        }
    }
}
