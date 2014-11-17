using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using BorderSource.BetsyContext;

namespace TwkrsToBorderloopParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> settings = new BorderSource.Common.INIFile("C://BorderSoftware//BobAndFriends//settings//baf.ini").GetAllValues();
            Database.Instance.Connect(settings["dbsource"], settings["dbname"], settings["dbuid"], settings["dbpw"]);
            DataTable dt = Database.Instance.Read("SELECT id FROM article WHERE id NOT IN (SELECT article_id FROM cat_article)");
            int count = 0;
            int articleId;
            int tempCatId;
            string catSyn;
            Dictionary<string, string> dic;
            DataTable table;
            foreach(DataRow dr in dt.Rows)
            {
                count++;
                articleId = (int)dr["id"];
                tempCatId = (int)(Database.Instance.Read("SELECT category_id AS id FROM cat_articletemp WHERE article_id = " + articleId).Rows.Count > 0? (int)Database.Instance.Read("SELECT category_id AS id FROM cat_articletemp WHERE article_id = " + articleId).Rows[0]["id"] : 0);
                if (tempCatId == 0) continue;
                catSyn = (string)Database.Instance.Read("SELECT description FROM categorytemp WHERE id = " + tempCatId).Rows[0]["description"];
                dic = new Dictionary<string, string>();
                //dic.Add("@DESC", catSyn);
                table = Database.Instance.Read("SELECT category_id AS id FROM category_synonym WHERE description LIKE '%" + catSyn.Replace(@"'", @"\'") + "%'"/*, dic*/);
                if(!(table.Rows.Count>0))  continue;
                int catId = (int)table.Rows[0]["id"];
                Database.Instance.InstertIntoCatArticle(catId, articleId);
                Console.WriteLine(count + "/" + dt.Rows.Count + " Added entry (" + catId + ", " + articleId + ")");

            }          
        }
    }
}
