using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.BetsyContext;
using BorderSource.Common;

namespace Baximus
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
            CalculateBiggestDifferences();
            CalculateBiggestDifferencesPerCountry();          
        }

        static void CalculateBiggestDifferences()
        {
            using (var db = new BetsyModel(Database.Instance.GetConnectionString()))
            {
                int count = 0;
                //Calculate biggest price differences
                foreach (article article in db.article)
                {
                    biggest_price_differences bpd = new biggest_price_differences();
                    bpd.highest_price = Int32.MaxValue;
                    bpd.lowest_price = Int32.MaxValue;

                    //We need at least 2 products and 1 dutch product to compare
                    if (article.product.Count() < 2) continue;
                    if (!article.product.Any(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id == 1)) continue;
                    if (!article.product.Any(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id != 1)) continue;

                    //We can now safely assume there is at least 1 foreign and 1 dutch product in the article

                    //Highest price is the lowest dutch price.
                    bpd.highest_price = article.product.Where(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id == 1).Min(p => p.price);

                    //Get the lowest foreign product as a whole, because we need more data from it.
                    product lowest = article.product.Where(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id != 1).OrderByDescending(p => p.price).Reverse().FirstOrDefault();

                    bpd.lowest_price = lowest.price;
                    bpd.product_id = lowest.id;
                    bpd.country_id = db.webshop.Where(w => w.name == lowest.webshop_url).FirstOrDefault().country_id;

                    bpd.article_id = article.id;
                    bpd.difference = bpd.highest_price - bpd.lowest_price;

                    db.biggest_price_differences.Add(bpd);
                    count++;

                    //Only save changes every 5000th product
                    if (count == 5000)
                    {
                        db.SaveChanges();
                    }
                }

                //Sve the remaining articles
                db.SaveChanges();
            }
        }

        static void CalculateBiggestDifferencesPerCountry()
        {
            using (var db = new BetsyModel(Database.Instance.GetConnectionString()))
            {
                int count = 0;
                foreach (article article in db.article)
                {                   
                    //We need at least 1 foreign product and 1 dutch product to compare
                    if (article.product.Count < 2) continue;
                    if (!article.product.Any(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id == 1)) continue;
                    if (!article.product.Any(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id != 1)) continue;

                    //We can now safely assume there is at least 1 foreign and 1 dutch product in the article

                    decimal dutchPrice = article.product.Where(p => db.webshop.Where(w => w.name == p.webshop_url).FirstOrDefault().id == 1).FirstOrDefault().price;

                    foreach(product product in article.product)
                    {
                        country_price_differences cpd = new country_price_differences();

                        short country_id = db.webshop.Where(w => w.name == product.webshop_url).FirstOrDefault().country_id;
                        if (country_id == 1) continue;

                        decimal difference = dutchPrice - product.price;
                        if (db.country_price_differences.Any(c => c.article_id == article.id && c.country_id == country_id))
                        {
                            //There already exists a record for this country - check if the difference is bigger, if yes then add, else, just continue
                            if (db.country_price_differences.Where(c => c.article_id == article.id && c.country_id == country_id).FirstOrDefault().difference > difference) continue;

                            db.country_price_differences.Attach(cpd);
                            var entry = db.Entry(cpd);
                            cpd.difference = difference;
                            entry.Property("difference").IsModified = true;
                            count++;
                        }
                        else
                        {

                            cpd.article_id = article.id;
                            cpd.country_id = country_id;
                            cpd.difference = difference;
                            cpd.product_id = product.id;

                            db.country_price_differences.Add(cpd);
                            count++;
                        }
                        //Only save changes every 5000th product
                        if (count == 5000)
                        {
                            db.SaveChanges();
                        }
                    } 
                }

                //Sve the remaining articles
                db.SaveChanges();
            }
        }

        static void Initialize()
        {
            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion
        }
    }
}
