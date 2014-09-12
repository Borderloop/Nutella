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

        public Database() { }

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
            string query = "SELECT * FROM category";
            DataTable dt = Read(query);

            return dt;
        }
        public DataTable GetCategorySynonyms()
        {
            string query = "SELECT * FROM category_synonyms";
            DataTable dt = Read(query);

            return dt;
        }

        public void Close()
        {
            _conn.Close();
        }
    }
}
