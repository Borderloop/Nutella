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
using BorderSource.Loggers;
using BorderSource.Property;
using System.Data.Entity.Infrastructure;

namespace Baximus
{
    class Program
    {
        static List<article> articleList;
        static string ConnectionString;
        static decimal positiveAveragePrice;
        static decimal positiveAveragePercentage;
        static decimal negativeAveragePrice;
        static decimal negativeAveragePercentage;
        static decimal averagePrice;
        static decimal averagePercentage;
        static BetsyModel db;

        static void Main(string[] args)
        {
            Initialize();          
            CalculateBiggestDifferences();
            // CalculateBiggestDifferencesPerCountry();    
            db.Dispose();      
        }

        static void FillArticleList()
        {
            Console.Write("Connecting to database...");
            Console.Write(" Done\n");
            var foreignWebshopQuery = db.webshop.Where(w => w.country_id != 1).Select(w => w.url);
            var domesticWebshopQuery = db.webshop.Where(w => w.country_id == 1).Select(w => w.url);
            var foreignProductQuery = db.product.Where(p => foreignWebshopQuery.Contains(p.webshop_url));
            var domesticProductQuery = db.product.Where(p => domesticWebshopQuery.Contains(p.webshop_url));

            var multiMatches = from a in foreignProductQuery join b in domesticProductQuery on a.article_id equals b.article_id select a.article_id;

            Console.Write("Fetching all articles with multimatches...");
            articleList.AddRange(db.article.Where(a => multiMatches.Contains(a.id)));

            Console.Write(" Done\n");

        }

        static void CalculateBiggestDifferences()
        {
            Console.WriteLine("----- STARTED COMPARING BIGGEST PRICE DIFFERENCE PER PRODUCT -----");
            Console.Write("Connecting to database...");
            Console.Write(" Done\n");

            Console.WriteLine("Started calculating biggest price differences...");
            int count = 0;
            int perc = 10;
            var foreignWebshopQuery = db.webshop.Where(w => w.country_id != 1).Select(w => w.url);
            var domesticWebshopQuery = db.webshop.Where(w => w.country_id == 1).Select(w => w.url);
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE biggest_price_differences");
            // Calculate biggest price differences
            foreach (article article in articleList)
            {
                count++;
                biggest_price_differences bpd = new biggest_price_differences();
                bpd.highest_price = Int32.MaxValue;
                bpd.lowest_price = Int32.MaxValue;

                // We can now safely assume there is at least 1 foreign and 1 dutch product in the article

                // Highest price is the lowest dutch price.
                bpd.highest_price = article.product.Where(p => domesticWebshopQuery.Contains(p.webshop_url)).Min(p => p.price);

                // Get the lowest foreign product as a whole, because we need more data from it.
                product lowest = article.product.Where(p => foreignWebshopQuery.Contains(p.webshop_url)).OrderByDescending(p => p.price).Reverse().FirstOrDefault();

                bpd.lowest_price = lowest.price;
                bpd.product_id = lowest.id;
                bpd.country_id = db.webshop.Where(w => w.url == lowest.webshop_url).FirstOrDefault().country_id;

                bpd.article_id = article.id;
                bpd.difference = bpd.highest_price - bpd.lowest_price;
                bpd.last_updated = System.DateTime.Now;

                biggest_price_differences entry;
                if ((entry = db.biggest_price_differences.Where(b => b.article_id == bpd.article_id).FirstOrDefault()) != default(biggest_price_differences))
                {
                    // Update entry
                    foreach (PropertyInfo p in bpd.GetType().GetProperties())
                    {
                        if (p.Name == "id") continue;
                        if (p.Name == "article") continue;
                        if (p.Name == "country") continue;
                        if (p.Name == "product") continue;
                        var entryValue = entry.GetType().GetProperty(p.Name).GetValue(entry);
                        var bpdValue = p.GetValue(bpd);
                        if (!entryValue.Equals(bpdValue))
                        {
                            entry.GetType().GetProperty(p.Name).SetValue(entry, bpdValue);
                            db.Entry(entry).Property(p.Name).IsModified = true;
                        }
                    }
                }
                else
                {
                    db.biggest_price_differences.Add(bpd);
                    db.Entry(bpd).State = EntityState.Added;
                }

                if ((int)(((double)count / (double)articleList.Count) * 100) >= perc)
                {
                    Console.Write(perc + "% ");
                    perc += 10;
                }

                if (count % 1000 == 0)
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Console.WriteLine("Caught DbUpdateConcurrenryException, trying refresh.");
                        var objContext = ((IObjectContextAdapter)db).ObjectContext;
                        try
                        {
                            objContext.Refresh(RefreshMode.StoreWins, ex.Entries.Select(e => e.Entity));
                        }
                        catch (InvalidOperationException ioe)
                        {
                            Console.WriteLine("Refresh didn't work, lost 500 products. Error: " + ioe.Message);
                            Console.WriteLine("Moving on.");
                        }
                    }
            }
            Console.Write("Saving changes to database...");
            db.SaveChanges();
            Console.Write(" Done\n");

            Console.WriteLine("----- FINISHED COMPARING BIGGEST PRICE DIFFERENCE PER PRODUCT -----");

        }

        static void CalculateBiggestDifferencesPerCountry()
        {
            Console.WriteLine("----- STARTED COMPARING BIGGEST PRICE DIFFERENCE PER COUNTRY ------");
            Console.Write("Connecting to database...");

            decimal count = 0;
            Console.Write(" Done\n");
            int perc = 10;
            Console.WriteLine("Started calculating biggest price differences...");
            foreach (article article in articleList)
            {
                count++;
                decimal dutchPrice = article.product.Where(p => db.webshop.Where(w => w.url == p.webshop_url).FirstOrDefault().country_id == 1).OrderByDescending(p => p.price).Reverse().FirstOrDefault().price;

                foreach (product product in article.product)
                {
                    country_price_differences cpd = new country_price_differences();

                    short country_id = db.webshop.Where(w => w.url == product.webshop_url).FirstOrDefault().country_id;
                    if (country_id == 1) continue;

                    decimal difference = dutchPrice - product.price;
                    decimal percentage = (decimal)100 - Math.Round((decimal.Divide(product.price, dutchPrice) * (decimal)100), 2);
                    averagePrice = (averagePrice * (count - 1)) / count + difference / count;
                    averagePercentage = (averagePercentage * (count - 1) / count) + percentage / count;
                    if (difference > 0)
                    {
                        positiveAveragePrice = (positiveAveragePrice * (count - 1)) / count + difference / count;
                        positiveAveragePercentage = (positiveAveragePercentage * (count - 1) / count) + percentage / count;
                    }
                    else if (difference < 0)
                    {
                        negativeAveragePrice = (negativeAveragePrice * (count - 1)) / count + difference / count;
                        negativeAveragePercentage = (negativeAveragePercentage * (count - 1) / count) + percentage / count;
                    }

                    if (db.country_price_differences.Any(c => c.article_id == article.id && c.country_id == country_id))
                    {
                        // There already exists a record for this country - check if the difference is bigger, if yes then add, else, just continue
                        country_price_differences entry;
                        if ((entry = db.country_price_differences.Where(c => c.article_id == article.id && c.country_id == country_id).FirstOrDefault()).difference >= difference)
                        {
                            continue;
                        }

                        entry.difference = difference;
                        // entry.difference_percentage = percentage;
                        entry.product_id = product.id;
                        entry.last_updated = System.DateTime.Now;

                    }
                    else
                    {

                        cpd.article_id = article.id;
                        cpd.country_id = country_id;
                        cpd.difference = difference;
                        // cpd.difference_percentage = percentage;
                        cpd.product_id = product.id;
                        cpd.last_updated = System.DateTime.Now;

                        db.country_price_differences.Add(cpd);
                    }
                    if ((int)(((double)count / (double)articleList.Count) * 100) >= perc)
                    {
                        Console.Write(perc + "% ");
                        perc += 10;
                    }
                    if (count % 1000 == 0)
                        db.SaveChanges();
                }
            }


            Console.Write("Saving changes to database...");
            db.SaveChanges();
            Console.Write(" Done\n");

            Console.WriteLine("----- FINISHED COMPARING BIGGEST PRICE DIFFERENCE PER COUNTRY -----");

        }
        

        static void Initialize()
        {
            
            #region Settings
            Dictionary<string, string> settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion  

            #region ConnectionString
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = 125;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Database = settings["db_name"];
            providerConnStrBuilder.Password = settings["db_password"];
            providerConnStrBuilder.Server = settings["db_source"];
            providerConnStrBuilder.UserID = settings["db_userid"];
            providerConnStrBuilder.Port = (uint)Int32.Parse(settings["db_port"]);

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
            #endregion ConnectionString
            db = new BetsyModel(ConnectionString);
            db.Configuration.AutoDetectChangesEnabled = false;
            #region articleList
            articleList = new List<article>();
            FillArticleList();
            #endregion

        }
    }
}
