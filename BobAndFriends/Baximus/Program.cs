using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BorderSource.BetsyContext;
using BorderSource.Common;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using MySql.Data.MySqlClient;

namespace Baximus
{
    class Program
    {
        static List<article> articleList;
        static string ConnectionString;

        static void Main(string[] args)
        {
            Initialize();          
            CalculateBiggestDifferences();
            CalculateBiggestDifferencesPerCountry();          
        }

        static void FillArticleList()
        {          
            Console.Write("Connecting to database...");
            using (var db = new BetsyModel(ConnectionString))
            {
                Console.Write(" Done\n");
                Console.Write("Fetching foreign products...");
                var foreignProductList = db.product.Where(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id != 1).ToList();

                Console.Write(" Done\n");

                Console.Write("Fetching articles with foreign products and dutch products...");
                foreach (product prod in foreignProductList)
                {
                    if (db.article.Where(a => a.id == prod.article_id).FirstOrDefault().product.Any(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id == 1))
                    {
                        articleList.Add(db.article.Where(a => a.id == prod.article_id).FirstOrDefault());
                    }
                }
                Console.Write(" Done\n");
            }
        }

        static void CalculateBiggestDifferences()
        {
            Console.WriteLine("----- STARTED COMPARING BIGGEST PRICE DIFFERENCE PER PRODUCT -----");
            Console.Write("Connecting to database...");
            using (var db = new BetsyModel(ConnectionString))
            {
                Console.Write(" Done\n");

                Console.WriteLine("Started calculating biggest price differences...");
                //Calculate biggest price differences
                foreach (article article in articleList)
                {
                    biggest_price_differences bpd = new biggest_price_differences();
                    bpd.highest_price = Int32.MaxValue;
                    bpd.lowest_price = Int32.MaxValue;

                    //We can now safely assume there is at least 1 foreign and 1 dutch product in the article

                    //Highest price is the lowest dutch price.
                    bpd.highest_price = article.product.Where(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id == 1).Min(p => p.price);

                    //Get the lowest foreign product as a whole, because we need more data from it.
                    product lowest = article.product.Where(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id != 1).OrderByDescending(p => p.price).Reverse().FirstOrDefault();

                    bpd.lowest_price = lowest.price;
                    bpd.product_id = lowest.id;
                    bpd.country_id = db.webshop.Where(w => w.url == lowest.webshop_url).FirstOrDefault().country_id;

                    bpd.article_id = article.id;
                    bpd.difference = bpd.highest_price - bpd.lowest_price;
                    bpd.last_updated = System.DateTime.Now;

                    biggest_price_differences entry;
                    if ((entry = db.biggest_price_differences.Where(b => b.article_id == bpd.article_id).FirstOrDefault()) != default(biggest_price_differences))
                    {
                        //Update entry
                        foreach (PropertyInfo p in bpd.GetType().GetProperties())
                        {
                            if (p.Name == "id") continue;
                            if (p.Name == "article") continue;
                            if (p.Name == "country") continue;
                            if (p.Name == "product") continue;
                            var entryValue = entry.GetType().GetProperty(p.Name).GetValue(entry);
                            var bpdValue = bpd.GetType().GetProperty(p.Name).GetValue(bpd);
                            if (!entryValue.Equals(bpdValue))
                            {
                                entry.GetType().GetProperty(p.Name).SetValue(entry, bpdValue);
                            }
                        }
                        Console.WriteLine("Updated article " + entry.article_id + " with difference of " + entry.difference);
                    }
                    else
                    {
                        db.biggest_price_differences.Add(bpd);
                        Console.WriteLine("Added article " + bpd.article_id + " with difference of " + bpd.difference);
                    }

                }
                Console.Write("Saving changes to database...");
                db.SaveChanges();
                Console.Write(" Done\n");

                Console.WriteLine("----- FINISHED COMPARING BIGGEST PRICE DIFFERENCE PER PRODUCT -----");
            }
        }

        static void CalculateBiggestDifferencesPerCountry()
        {
            Console.WriteLine("----- STARTED COMPARING BIGGEST PRICE DIFFERENCE PER COUNTRY ------");
            Console.Write("Connecting to database...");
            using (var db = new BetsyModel(ConnectionString))
            {              
                Console.Write(" Done\n");

                Console.WriteLine("Started calculating biggest price differences...");
                foreach (article article in articleList)
                {
                    decimal dutchPrice = article.product.Where(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id == 1).OrderByDescending(p => p.price).Reverse().FirstOrDefault().price;

                    foreach (product product in article.product)
                    {
                        country_price_differences cpd = new country_price_differences();

                        short country_id = db.webshop.Where(w => w.url == product.webshop_url).FirstOrDefault().country_id;
                        if (country_id == 1) continue;

                        decimal difference = dutchPrice - product.price;
                        if (db.country_price_differences.Any(c => c.article_id == article.id && c.country_id == country_id))
                        {
                            //There already exists a record for this country - check if the difference is bigger, if yes then add, else, just continue
                            country_price_differences entry;
                            if ((entry = db.country_price_differences.Where(c => c.article_id == article.id && c.country_id == country_id).FirstOrDefault()).difference >= difference)
                            {
                                Console.WriteLine("Did not update article " + article.id + " (bigger/equal difference already in database)");
                                continue;
                            }

                            entry.difference = difference;
                            entry.product_id = product.id;
                            entry.last_updated = System.DateTime.Now;

                            Console.WriteLine("Updated article " + article.id + " with difference of " + difference);
                        }
                        else
                        {

                            cpd.article_id = article.id;
                            cpd.country_id = country_id;
                            cpd.difference = difference;
                            cpd.product_id = product.id;
                            cpd.last_updated = System.DateTime.Now;

                            db.country_price_differences.Add(cpd);
                            Console.WriteLine("Added article " + article.id + " with difference of " + difference);
                        }
                    }
                }

                Console.Write("Saving changes to database...");
                db.SaveChanges();
                Console.Write(" Done\n");

                Console.WriteLine("----- FINISHED COMPARING BIGGEST PRICE DIFFERENCE PER COUNTRY -----");
            }
        }
        

        static void Initialize()
        {
            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion

            #region Loggers
            Statics.SqlLogger = new QueryLogger(Statics.settings["logpath"] + "\\querydump" + DateTime.Now.ToString("MMddHHmm") + ".txt");
            #endregion 

            #region articleList
            articleList = new List<article>();
            FillArticleList();
            #endregion

            #region ConnectionString
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = 32767;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Database = Statics.settings["dbname"];
            providerConnStrBuilder.Password = Statics.settings["dbpw"];
            providerConnStrBuilder.Server = Statics.settings["dbsource"];
            providerConnStrBuilder.UserID = Statics.settings["dbuid"];

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
            #endregion ConnectionString

        }
    }
}
