using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using BorderSource.BetsyContext;
using BorderSource.Common;

namespace Crapper
{
    public class Program
    {
        static void Main(string[] args)
        {          
            Initialize();
            CleanupEanDupes();
            CleanupTitleDupes();
        }

        static void CleanupEanDupes()
        {
            Console.WriteLine("Started looking for EAN dupes...");
            using (var db = new BetsyModel(BorderSource.Common.Database.Instance.GetConnectionString()))
            {
                db.Configuration.LazyLoadingEnabled = false;
                var duplicateEans = db.ean.GroupBy(e => e.ean1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                string correctTitle;
                string wrongTitle;
                Console.WriteLine("Started cleaning up... ");  
                foreach (var ean in duplicateEans)
                {
                    Console.WriteLine("Getting wrong articles for ean " + ean);                  
                    List<article> wrongArticles = db.article
                        .Where(a => a.ean.Any(e => e.ean1 == ean))
                        .Include(a => a.title.Select(t => t.title_synonym))
                        .Include(a => a.ean)
                        .Include(a => a.sku)
                        .ToList();

                    article correctArticle = wrongArticles.First();
                    wrongArticles.Remove(correctArticle);
                    correctTitle = correctArticle.title.First().title1;
                    
                    foreach (article wrongArticle in wrongArticles)
                    {
                        Console.WriteLine("Busy with: " + ean);       
                        wrongTitle = wrongArticle.title.First().title1;
                        
                        //Set product article id to correctArticle.id
                        List<product> productList = new List<product>();
                        wrongArticle.product.ToList().ForEach(p => productList.Add(p));

                        //Set sku to correct article id
                        List<sku> skuList = new List<sku>();
                        wrongArticle.sku.Where(s => !correctArticle.sku.Any(k => k.sku1.ToLower() == s.sku1.ToLower())).ToList().ForEach(s => skuList.Add(s));

                        //set ean to correct article id
                        List<ean> eanList = new List<ean>();
                        wrongArticle.ean.Where(e => !correctArticle.ean.Any(a => a.ean1 == e.ean1)).ToList().ForEach(e => eanList.Add(e));

                        List<title> titleList = new List<title>();
                        wrongArticle.title.Where(t => !correctArticle.title.Any(tt => tt.title_synonym.Any(ts => ts.title.ToLower() == t.title1.ToLower()))).ToList().ForEach(t => titleList.Add(t));

                        //Add titles of wrongArticle to synonyms of correctArticle.
                        //If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in titleList)
                        {
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower() == title.title1.ToLower())))
                                {
                                    correctArticle.title.ToList().ForEach(t => t.title_synonym.Where(ts => ts.title.ToLower() == title.title1.ToLower()).FirstOrDefault().occurrences++);
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title1.id });
                                }
                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }

                            foreach(title_synonym syn in title.title_synonym)
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower() == syn.title.ToLower())))
                                {
                                    List<title> titles = correctArticle.title.ToList();
                                    foreach (title t in titles)
                                    {
                                        title_synonym syn2 = t.title_synonym.Where(ts => ts.title.ToLower() == syn.title.ToLower()).FirstOrDefault();
                                        if (syn2 == default(title_synonym)) continue;
                                        syn2.occurrences++;
                                        db.title_synonym.Attach(syn2);
                                        var entry = db.Entry(syn2);
                                        entry.Property(ts => ts.occurrences).IsModified = true;
                                    }
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = syn.title, occurrences = 1, title_id = title1.id});
                                }
                            }
                        }                       
                                            
                        db.ean.RemoveRange(wrongArticle.ean);
                        db.sku.RemoveRange(wrongArticle.sku);
                        db.product.RemoveRange(wrongArticle.product);
                        db.biggest_price_differences.RemoveRange(wrongArticle.biggest_price_differences);
                        db.country_price_differences.RemoveRange(wrongArticle.country_price_differences);

                        List<title_synonym> syns = new List<title_synonym>();
                        wrongArticle.title.ToList().ForEach(t => t.title_synonym.ToList().ForEach(ts => syns.Add(ts)));

                        db.title_synonym.RemoveRange(syns);
                        db.title.RemoveRange(wrongArticle.title);

                        db.vbob_suggested.RemoveRange(wrongArticle.vbob_suggested);
                        db.category.RemoveRange(wrongArticle.category);
                                            
                        db.article.Remove(wrongArticle);

                        foreach(ean ean1 in eanList)
                        {
                            correctArticle.ean.Add(new ean { ean1 = ean1.ean1, article_id = correctArticle.id });
                        }
                        foreach(sku sku1 in skuList)
                        {
                            correctArticle.sku.Add(new sku { sku1 = sku1.sku1, article_id = correctArticle.id });
                        }
                        foreach(product product1 in productList)
                        {
                            correctArticle.product.Add(new product
                            {
                                affiliate_name = product1.affiliate_name,
                                affiliate_unique_id = product1.affiliate_unique_id,
                                direct_link = product1.direct_link,
                                price = product1.price,
                                ship_cost = product1.ship_cost,
                                ship_time = product1.ship_time,
                                webshop_url = product1.webshop_url,
                                article_id = correctArticle.id
                            });
                        }

                        db.SaveChanges();

                        Console.WriteLine("Redirected " + wrongTitle + " to " + correctTitle + ".");
                    }
                }
                db.SaveChanges();
            }
            Console.WriteLine("Done with the EAN dupes.");
        }



        static void CleanupTitleDupes()
        {
            Console.WriteLine("Started looking for Title dupes...");
            using (var db = new BetsyModel(BorderSource.Common.Database.Instance.GetConnectionString()))
            {
                var duplicateTitles = db.title.GroupBy(t => t.title1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                string correctTitle;
                string wrongTitle;
                Console.WriteLine("Started cleaning up... ");  
                foreach (var dupTitle in duplicateTitles)
                {
                    Console.WriteLine("Getting wrong articles for title " + dupTitle);
                    List<article> wrongArticles = db.article
                        .Where(a => a.title.Any(t => t.title1.ToLower() == dupTitle.ToLower()))
                        .Include(a => a.title.Select(t => t.title_synonym))
                        .Include(a => a.ean)
                        .Include(a => a.sku)
                        .ToList();
           
                    article correctArticle = db.article.Where(a => a.title.Any(t => t.title1.ToLower() == dupTitle.ToLower())).FirstOrDefault();
                    wrongArticles.Remove(correctArticle);
                    correctTitle = correctArticle.title.First().title1;

                    foreach (article wrongArticle in wrongArticles)
                    {
                        Console.WriteLine("Busy with: " + dupTitle);    
                        wrongTitle = wrongArticle.title.First().title1;

                        //Set product article id to correctArticle.id
                        List<product> productList = new List<product>();
                        wrongArticle.product.ToList().ForEach(p => productList.Add(p));

                        //Set sku to correct article id
                        List<sku> skuList = new List<sku>();
                        wrongArticle.sku.Where(s => !correctArticle.sku.Any(k => k.sku1.ToLower() == s.sku1.ToLower())).ToList().ForEach(s => skuList.Add(s));

                        //set ean to correct article id
                        List<ean> eanList = new List<ean>();
                        wrongArticle.ean.Where(e => !correctArticle.ean.Any(a => a.ean1 == e.ean1)).ToList().ForEach(e => eanList.Add(e));

                        List<title> titleList = new List<title>();
                        wrongArticle.title.Where(t => !correctArticle.title.Any(tt => tt.title_synonym.Any(ts => ts.title.ToLower() == t.title1.ToLower()))).ToList().ForEach(t => titleList.Add(t));

                        //Add titles of wrongArticle to synonyms of correctArticle.
                        //If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in titleList)
                        {
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower() == title.title1.ToLower())))
                                {
                                    correctArticle.title.ToList().ForEach(t => t.title_synonym.Where(ts => ts.title.ToLower() == title.title1.ToLower()).FirstOrDefault().occurrences++);
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title1.id });
                                }
                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }

                            foreach (title_synonym syn in title.title_synonym)
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower() == syn.title.ToLower())))
                                {
                                    List<title> titles = correctArticle.title.ToList();
                                    foreach(title t in titles)
                                    {
                                        title_synonym syn2 = t.title_synonym.Where(ts => ts.title.ToLower() == syn.title.ToLower()).FirstOrDefault();                                      
                                        if (syn2 == default(title_synonym)) continue;
                                        syn2.occurrences++;
                                        db.title_synonym.Attach(syn2);
                                        var entry = db.Entry(syn2);
                                        entry.Property(ts => ts.occurrences).IsModified = true;
                                    }
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = syn.title, occurrences = 1, title_id = title1.id });
                                }
                            }
                        }

                        db.ean.RemoveRange(wrongArticle.ean);
                        db.sku.RemoveRange(wrongArticle.sku);
                        db.product.RemoveRange(wrongArticle.product);
                        db.biggest_price_differences.RemoveRange(wrongArticle.biggest_price_differences);
                        db.country_price_differences.RemoveRange(wrongArticle.country_price_differences);

                        List<title_synonym> syns = new List<title_synonym>();
                        wrongArticle.title.ToList().ForEach(t => t.title_synonym.ToList().ForEach(ts => syns.Add(ts)));

                        db.title_synonym.RemoveRange(syns);
                        db.title.RemoveRange(wrongArticle.title);

                        db.vbob_suggested.RemoveRange(wrongArticle.vbob_suggested);
                        db.category.RemoveRange(wrongArticle.category);

                        db.article.Remove(wrongArticle);

                        foreach (ean ean1 in eanList)
                        {
                            correctArticle.ean.Add(new ean { ean1 = ean1.ean1, article_id = correctArticle.id });
                        }
                        foreach (sku sku1 in skuList)
                        {
                            correctArticle.sku.Add(new sku { sku1 = sku1.sku1, article_id = correctArticle.id });
                        }
                        foreach (product product1 in productList)
                        {
                            correctArticle.product.Add(new product
                            {
                                affiliate_name = product1.affiliate_name,
                                affiliate_unique_id = product1.affiliate_unique_id,
                                direct_link = product1.direct_link,
                                price = product1.price,
                                ship_cost = product1.ship_cost,
                                ship_time = product1.ship_time,
                                webshop_url = product1.webshop_url,
                                article_id = correctArticle.id
                            });
                        }

                        db.SaveChanges();

                        Console.WriteLine("Redirected " + wrongTitle + " to " + correctTitle + ".");
                    }
                }
            }
            Console.WriteLine("Done with the title dupes.");
        }

        static void Initialize()
        {
            Console.WriteLine("Initializing... ");
            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion

            #region Loggers
            Statics.SqlLogger = new QueryLogger(Statics.settings["logpath"] + "\\querydump" + DateTime.Now.ToString("MMddHHmm") + ".txt");
            #endregion 
        }
    }
}

