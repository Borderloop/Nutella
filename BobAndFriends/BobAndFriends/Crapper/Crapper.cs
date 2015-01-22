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
using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using BorderSource.Loggers;
using BobAndFriends.Global;
using System.Collections.Concurrent;


namespace BobAndFriends.Crapper
{
    class Crapper
    {
        private static string ConnectionString;

        public static void Parse()
        {
            HashSet<long> addedEans = new HashSet<long>();
            HashSet<string> newCategories = new HashSet<string>();
            Initialize();
            using (var db = new BetsyModel(ConnectionString))
            {
                var eans = db.Database.SqlQuery<long>("SELECT ean FROM (SELECT ean FROM eantemp AS temp UNION ALL SELECT ean FROM ean AS current) AS together GROUP BY ean HAVING count(ean) = 1").ToList();
                Console.WriteLine("Parsing {0} eans...", eans.Count);
                int i = 0;
                foreach (long ean in eans)
                {
                    var aId = db.Database.SqlQuery<long>("SELECT article_id FROM eantemp WHERE ean = @EAN", new MySqlParameter("@EAN", ean)).ToList();
                    var otherEans = db.Database.SqlQuery<long>("SELECT ean FROM eantemp WHERE article_id = @AID", new MySqlParameter("@AID", aId)).ToList();
                    if (otherEans.Count > 1)
                    {
                        var existingEans = db.ean.Where(e => otherEans.Contains(e.ean1)).ToList();
                        if (existingEans.Count == 0)
                        {
                            foreach (long otherEan in otherEans)
                            {
                                if (addedEans.Contains(otherEan)) continue;
                                article newArticle = new article();

                                int categoryId = db.Database.SqlQuery<int>("SELECT category_id FROM cat_articletemp WHERE article_id = @AID", new MySqlParameter("@AID", aId.FirstOrDefault())).FirstOrDefault();
                                string categoryName = db.Database.SqlQuery<string>("SELECT description FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? "";
                                string initialcategoryName = new string(categoryName.ToCharArray());
                                string trimmedCategoryName = categoryName.Trim();
                                category_synonym catSyn = db.category_synonym.Where(cs => cs.web_url == "www.borderloop.nl" && cs.description == trimmedCategoryName).FirstOrDefault();
                                if (catSyn == null)
                                {
                                    while (categoryId != 0 && catSyn == null)
                                    {
                                        categoryId = db.Database.SqlQuery<int?>("SELECT called_by FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? 0;
                                        categoryName = db.Database.SqlQuery<string>("SELECT description FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? "";
                                        trimmedCategoryName = categoryName.Trim();
                                        catSyn = db.category_synonym.Where(cs => cs.web_url == "www.borderloop.nl" && cs.description == trimmedCategoryName).FirstOrDefault();
                                    }
                                    if (categoryId == 0 || catSyn == null)
                                    {
                                        Console.WriteLine("Could not categorize {0} with EAN {1}.", initialcategoryName, ean);
                                        if (!newCategories.Contains(initialcategoryName)) newCategories.Add(initialcategoryName);
                                        i++;
                                        continue;
                                    }
                                }
                                category cat = db.category.Where(c => c.id == catSyn.category_id).FirstOrDefault();
                                newArticle.category.Add(cat);
                                newArticle.ean.Add(new ean { ean1 = otherEan });
                                newArticle.brand = db.Database.SqlQuery<string>("SELECT brand FROM articletemp WHERE id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                                newArticle.description = "";
                                newArticle.image_loc = "";

                                string title = db.Database.SqlQuery<string>("SELECT title FROM titletemp WHERE article_id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                                newArticle.title.Add(new title { title1 = title, country_id = 1 });

                                string sku = db.Database.SqlQuery<string>("SELECT sku FROM skutemp WHERE article_id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                                newArticle.sku.Add(new sku { sku1 = sku });
                                db.article.Add(newArticle);
                                db.Entry(newArticle).State = EntityState.Added;
                                addedEans.Add(otherEan);
                            }
                        }
                        else
                        {
                            if (existingEans.Count > 1)
                            {
                                List<int> aIds = new List<int>();
                                foreach (long existingEan in existingEans.Select(e => e.ean1))
                                {
                                    aId.Add(db.ean.Where(e => e.ean1 == existingEan).FirstOrDefault().article_id);
                                }
                                if (aIds.Count != aIds.Distinct().Count())
                                {
                                    Console.WriteLine("Multiple EANs pointing to different articles - skipping this one.");
                                    i++;
                                    continue;
                                }
                            }
                            article existingArticle = db.article.Where(a => a.id == db.ean.Where(e => e.ean1 == existingEans.First().ean1).FirstOrDefault().article_id).FirstOrDefault();
                            foreach (long otherEan in otherEans)
                            {
                                ean newEan = new ean { ean1 = otherEan, article_id = existingArticle.id };
                                addedEans.Add(otherEan);
                                db.ean.Add(newEan);
                                db.Entry(newEan).State = EntityState.Added;
                            }
                        }
                    }
                    else
                    {
                        if (addedEans.Contains(ean)) continue;
                        article newArticle = new article();

                        int categoryId = db.Database.SqlQuery<int>("SELECT category_id FROM cat_articletemp WHERE article_id = @AID", new MySqlParameter("@AID", aId.FirstOrDefault())).FirstOrDefault();
                        string categoryName = db.Database.SqlQuery<string>("SELECT description FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? "";
                        string initialcategoryName = new string(categoryName.ToCharArray());
                        string trimmedCategoryName = categoryName.Trim();
                        category_synonym catSyn = db.category_synonym.Where(cs => cs.web_url == "www.borderloop.nl" && cs.description == trimmedCategoryName).FirstOrDefault();
                        if (catSyn == null)
                        {
                            while(categoryId != 0 && catSyn == null)
                            {
                                categoryId = db.Database.SqlQuery<int?>("SELECT called_by FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? 0;
                                categoryName = db.Database.SqlQuery<string>("SELECT description FROM categorytemp WHERE id = @CATID", new MySqlParameter("@CATID", categoryId)).FirstOrDefault() ?? "";
                                trimmedCategoryName = categoryName.Trim();
                                catSyn = db.category_synonym.Where(cs => cs.web_url == "www.borderloop.nl" && cs.description == trimmedCategoryName).FirstOrDefault();
                            }
                            if (categoryId == 0 || catSyn == null)
                            {
                                Console.WriteLine("Could not categorize {0} with EAN {1}.", initialcategoryName, ean);
                                if (!newCategories.Contains(initialcategoryName)) newCategories.Add(initialcategoryName);
                                i++;
                                continue;
                            }
                        }
                        category cat = db.category.Where(c => c.id == catSyn.category_id).FirstOrDefault();
                        newArticle.category.Add(cat);
                        newArticle.ean.Add(new ean { ean1 = ean });
                        newArticle.brand = db.Database.SqlQuery<string>("SELECT brand FROM articletemp WHERE id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                        newArticle.description = "";
                        newArticle.image_loc = "";

                        string title = db.Database.SqlQuery<string>("SELECT title FROM titletemp WHERE article_id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                        newArticle.title.Add(new title { title1 = title, country_id = 1 });

                        string sku = db.Database.SqlQuery<string>("SELECT sku FROM skutemp WHERE article_id = @AID", new MySqlParameter("@AID", aId)).FirstOrDefault() ?? "";
                        newArticle.sku.Add(new sku { sku1 = sku });
                        db.article.Add(newArticle);
                        db.Entry(newArticle).State = EntityState.Added;
                        addedEans.Add(ean);
                    }

                    i++;
                    Console.WriteLine("Parsed {0}/{1}", i, eans.Count);
                    if(i % 1000 == 0)
                    {
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();
                Console.WriteLine("Could not match {0} categories. Writing them to C:/MissingCategories.txt.", newCategories.Count);
                using(System.IO.StreamWriter writer = new System.IO.StreamWriter("C:/MissingCategories.txt"))
                {
                    foreach (string s in newCategories)
                        writer.WriteLine(s);

                    writer.Flush();
                }
            }
        }        

        public static void CleanUp(DateTime time)
        {          
            Initialize();
            try
            {
                CleanupOldProducts(time);
                CleanupEmtpyTitles();
                //CleanupDuplicateWebshopsPerArticle();
                CleanupEanDupes();
                CleanupUrlDupes();
                //CleanupUniqueIdDupes();                          
                //CleanupTitleDupes();               
            }
            catch (Exception e)
            {
                Logger.Instance.WriteLine("Crapper threw an exception: " + e.Message);
                Logger.Instance.WriteLine("InnerException: " + e.InnerException);
                Logger.Instance.WriteLine("StackTrace: " + e.StackTrace);               
            }
        }

        private static void CleanupDuplicateWebshopsPerArticle()
        {
            Console.WriteLine("Cleaning up duplicate webshops per article...");
            using (var db = new BetsyModel(ConnectionString))
            {
                foreach (KeyValuePair<int, ConcurrentBag<string>> pair in GlobalVariables.AddedProducts)
                {
                    if (pair.Value.GroupBy(p => p).Any(x => x.Count() > 1))
                    {
                        IEnumerable<string> dupes = pair.Value.GroupBy(p => p).Where(x => x.Count() > 1).Select(s => s.Key);
                        foreach (string dupe in dupes)
                        {
                            db.product.RemoveRange(db.product.Where(p => p.article_id == pair.Key && p.webshop_url == dupe));
                        }
                    }
                }
                db.SaveChanges();
            }
            Console.WriteLine("Done cleaning up duplicate webshops per article.");
        }

        private static void CleanupEmtpyTitles()
        {
            Console.WriteLine("Started looking empty titles...");
            using (var db = new BetsyModel(ConnectionString))
            {
                var emptyTitles = db.title.Where(t => t.title1 == "");
                Console.WriteLine("Removing empty titles...");
                db.title.RemoveRange(emptyTitles);
                db.SaveChanges();
            }
            Console.WriteLine("Done removing empty titles.");
        }

        private static void CleanupEanDupes()
        {
            Console.WriteLine("Started looking for EAN dupes...");
            using (var db = new BetsyModel(ConnectionString))
            {
                var duplicateEans = db.ean.GroupBy(e => e.ean1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                string correctTitle;
                string wrongTitle = "";
                Console.WriteLine("Started cleaning up... ");
                foreach (var ean in duplicateEans)
                {
                    Console.WriteLine("Getting wrong articles for ean " + ean);
                    List<article> wrongArticles = db.article
                        .Where(a => a.ean.Any(e => e.ean1 == ean))
                        .ToList();

                    article correctArticle = wrongArticles.First();
                    wrongArticles.Remove(correctArticle);
                    correctTitle = correctArticle.title.First().title1;

                    foreach (article wrongArticle in wrongArticles)
                    {
                        Console.WriteLine("Busy with: " + ean);
                        title wrongTitleEntity = wrongArticle.title.FirstOrDefault();
                        if (!(wrongTitleEntity == null)) wrongTitle = wrongTitleEntity.title1;

                        // Set product article id to correctArticle.id
                        List<product> productList = new List<product>();
                        wrongArticle.product.ToList().ForEach(p => productList.Add(p));

                        // Set sku to correct article id
                        List<sku> skuList = new List<sku>();
                        wrongArticle.sku.Where(s => !correctArticle.sku.Any(k => k.sku1.ToLower().Trim() == s.sku1.ToLower().Trim())).ToList().ForEach(s => skuList.Add(s));

                        // set ean to correct article id
                        List<ean> eanList = new List<ean>();
                        wrongArticle.ean.Where(e => !correctArticle.ean.Any(a => a.ean1 == e.ean1)).ToList().ForEach(e => eanList.Add(e));

                        List<title> titleList = new List<title>();
                        wrongArticle.title.Where(t => !correctArticle.title.Any(tt => tt.title_synonym.Any(ts => ts.title.ToLower().Trim() == t.title1.ToLower().Trim()))).ToList().ForEach(t => titleList.Add(t));

                        // Add titles of wrongArticle to synonyms of correctArticle.
                        // If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in titleList)
                        {
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower().RemoveDiacriticAccents().Trim() == title.title1.ToLower().RemoveDiacriticAccents().Trim())))
                                {
                                    correctArticle.title.ToList().ForEach(t =>
                                    {
                                        title_synonym syn = t.title_synonym.Where(ts => ts.title.ToLower().RemoveDiacriticAccents().Trim() == title.title1.ToLower().RemoveDiacriticAccents().Trim()).FirstOrDefault();
                                        if (syn != null) syn.occurrences++;
                                    });
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title1.id, title1 = title1 });
                                }
                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }

                            foreach (title_synonym syn in title.title_synonym)
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.ToLower().RemoveDiacriticAccents().Trim() == syn.title.ToLower().RemoveDiacriticAccents().Trim())))
                                {
                                    List<title> titles = correctArticle.title.ToList();
                                    foreach (title t in titles)
                                    {
                                        
                                        title_synonym syn2 = t.title_synonym.Where(ts => ts.title.ToLower().Trim() == syn.title.ToLower().Trim()).FirstOrDefault();
                                        if (syn2 == default(title_synonym)) continue;
                                        syn2.occurrences++;
                                        
                                    }
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = syn.title, occurrences = 1, title_id = title1.id, title1 = title1 });
                                }
                            }
                        }
                        foreach (ean wrongEan in wrongArticle.ean.ToList()) db.ean.Remove(wrongEan);
                        foreach (sku wrongSku in wrongArticle.sku.ToList()) db.sku.Remove(wrongSku);
                        foreach (product wrongProduct in wrongArticle.product.ToList()) db.product.Remove(wrongProduct);
                        // db.biggest_price_differences.RemoveRange(wrongArticle.biggest_price_differences);
                        // db.country_price_differences.RemoveRange(wrongArticle.country_price_differences);

                        List<title_synonym> syns = new List<title_synonym>();
                        wrongArticle.title.ToList().ForEach(t => t.title_synonym.ToList().ForEach(ts => syns.Add(ts)));

                        foreach (title_synonym wTs in syns) db.title_synonym.Remove(wTs);
                        foreach (title wTitle in wrongArticle.title.ToList()) db.title.Remove(wTitle);

                        // db.vbob_suggested.RemoveRange(wrongArticle.vbob_suggested)

                        db.article.Remove(wrongArticle);

                        foreach (ean ean1 in eanList)
                        {
                            correctArticle.ean.Add(new ean { ean1 = ean1.ean1, article_id = correctArticle.id, article = correctArticle});
                        }
                        foreach (sku sku1 in skuList)
                        {
                            correctArticle.sku.Add(new sku { sku1 = sku1.sku1, article_id = correctArticle.id, article = correctArticle });
                        }
                        foreach (product product1 in productList)
                        {
                            if (correctArticle.product.Any(p => p.webshop_url == product1.webshop_url)) continue;
                            correctArticle.product.Add(new product
                            {
                                affiliate_name = product1.affiliate_name,
                                affiliate_unique_id = product1.affiliate_unique_id,
                                direct_link = product1.direct_link,
                                price = product1.price,
                                ship_cost = product1.ship_cost,
                                ship_time = product1.ship_time,
                                webshop_url = product1.webshop_url,
                                article_id = correctArticle.id,
                                article = correctArticle
                            });
                        }

                        if (db.ChangeTracker.HasChanges()) db.SaveChanges();

                        Console.WriteLine("Redirected " + wrongTitle + " to " + correctTitle + ".");
                    }
                }
                db.SaveChanges();
            }
            Console.WriteLine("Done with the EAN dupes.");
        }



        private static void CleanupTitleDupes()
        {
            Console.WriteLine("Started looking for Title dupes...");
            using (var db = new BetsyModel(ConnectionString))
            {
                var duplicateTitles = db.title.GroupBy(t => t.title1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                string correctTitle;
                string wrongTitle;
                Console.WriteLine("Started cleaning up... ");  
                foreach (var dupTitle in duplicateTitles)
                {
                    if (String.IsNullOrWhiteSpace(dupTitle)) continue;

                    Console.WriteLine("Getting wrong articles for title " + dupTitle);
                    List<article> wrongArticles = db.article
                        .Where(a => a.title.Any(t => t.title1 == dupTitle))
                        .ToList();

                    article correctArticle = db.article.Where(a => a.title.Any(t => t.title1.ToLower().Trim() == dupTitle.ToLower().Trim())).FirstOrDefault();
                    wrongArticles.Remove(correctArticle);
                    correctTitle = correctArticle.title.First().title1;

                    foreach (article wrongArticle in wrongArticles)
                    {
                        Console.WriteLine("Busy with: " + dupTitle);    
                        wrongTitle = wrongArticle.title.First().title1;

                        // Set product article id to correctArticle.id
                        List<product> productList = new List<product>();
                        wrongArticle.product.ToList().ForEach(p => productList.Add(p));

                        // Set sku to correct article id
                        List<sku> skuList = new List<sku>();
                        wrongArticle.sku.Where(s => !correctArticle.sku.Any(k => k.sku1.ToLower().Trim() == s.sku1.ToLower().Trim())).ToList().ForEach(s => skuList.Add(s));

                        // set ean to correct article id
                        List<ean> eanList = new List<ean>();
                        wrongArticle.ean.Where(e => !correctArticle.ean.Any(a => a.ean1 == e.ean1)).ToList().ForEach(e => eanList.Add(e));

                        List<title> titleList = new List<title>();
                        wrongArticle.title.Where(t => !correctArticle.title.Any(tt => tt.title_synonym.Any(ts => ts.title.ToLower().Trim() == t.title1.ToLower().Trim()))).ToList().ForEach(t => titleList.Add(t));

                        // Add titles of wrongArticle to synonyms of correctArticle.
                        // If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in titleList)
                        {
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.RemoveDiacriticAccents().ToLower().Trim() == title.title1.RemoveDiacriticAccents().ToLower().Trim())))
                                {
                                    correctArticle.title.ToList().ForEach(t =>
                                        {
                                            title_synonym syn = t.title_synonym.Where(ts => ts.title.ToLower().RemoveDiacriticAccents().Trim() == title.title1.ToLower().RemoveDiacriticAccents().Trim()).FirstOrDefault();
                                            if (syn != null) syn.occurrences++;
                                        });
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title1.id, title1 = title1 });
                                }
                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }

                            foreach (title_synonym syn in title.title_synonym)
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title.RemoveDiacriticAccents().ToLower().Trim() == syn.title.RemoveDiacriticAccents().ToLower().Trim())))
                                {
                                    List<title> titles = correctArticle.title.ToList();
                                    foreach (title t in titles)
                                    {
                                        title_synonym syn2 = t.title_synonym.Where(ts => ts.title.RemoveDiacriticAccents().ToLower().Trim() == syn.title.RemoveDiacriticAccents().ToLower().Trim()).FirstOrDefault();                                      
                                        if (syn2 == default(title_synonym)) continue;
                                        syn2.occurrences++;
                                    }
                                }
                                else
                                {
                                    title title1 = correctArticle.title.Where(t => t.country_id == title.country_id).FirstOrDefault();
                                    title1.title_synonym.Add(new title_synonym { title = syn.title, occurrences = 1, title_id = title1.id, title1 = title1 });
                                }
                            }
                        }

                        foreach (ean wrongEan in wrongArticle.ean.ToList()) db.ean.Remove(wrongEan);
                        foreach (sku wrongSku in wrongArticle.sku.ToList()) db.sku.Remove(wrongSku);
                        foreach (product wrongProduct in wrongArticle.product.ToList()) db.product.Remove(wrongProduct);
                        // db.biggest_price_differences.RemoveRange(wrongArticle.biggest_price_differences);
                        // db.country_price_differences.RemoveRange(wrongArticle.country_price_differences);

                        List<title_synonym> syns = new List<title_synonym>();
                        wrongArticle.title.ToList().ForEach(t => t.title_synonym.ToList().ForEach(ts => syns.Add(ts)));

                        foreach (title_synonym wTs in syns) db.title_synonym.Remove(wTs);
                        foreach (title wTitle in wrongArticle.title.ToList()) db.title.Remove(wTitle);

                        // db.vbob_suggested.RemoveRange(wrongArticle.vbob_suggested)

                        db.article.Remove(wrongArticle);

                        foreach (ean ean1 in eanList)
                        {
                            correctArticle.ean.Add(new ean { ean1 = ean1.ean1, article_id = correctArticle.id, article = correctArticle });
                        }
                        foreach (sku sku1 in skuList)
                        {
                            correctArticle.sku.Add(new sku { sku1 = sku1.sku1, article_id = correctArticle.id, article = correctArticle });
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
                                article_id = correctArticle.id,
                                article = correctArticle
                            });
                        }

                        if (db.ChangeTracker.HasChanges()) db.SaveChanges();

                        Console.WriteLine("Redirected " + wrongTitle + " to " + correctTitle + ".");
                    }
                }
            }
            Console.WriteLine("Done with the title dupes.");
        }

        private static void CleanupOldProducts(DateTime time)
        {
            Console.WriteLine("Started with removing old products.");
            using (var db = new BetsyModel(ConnectionString))
            {
                var oldProducts = db.product.Where(p => p.last_modified < time);
                Console.WriteLine("Removing all old products, this may take a while...");
                db.product.RemoveRange(oldProducts);
                db.SaveChanges();
            }
            Console.WriteLine("Done with removing old products.");
        }

        private static void CleanupUniqueIdDupes()
        {
            Console.WriteLine("Started with removing duplicate unique ids.");
            using (var db = new BetsyModel(ConnectionString))
            {
                var duplicateUniqueIds = db.product.GroupBy(p => new AffiliateIdentifier{ Id = p.affiliate_unique_id, Name = p.affiliate_name }).Where(x => x.Count() > 1).Select(val => val.Key).AsEnumerable();
                Console.WriteLine("Removing duplicate unique Id's now, this can take a while...");
                foreach (var dupe in duplicateUniqueIds)
                {
                    var products = db.product.Where(p => p.affiliate_unique_id == dupe.Id && p.affiliate_name == dupe.Name).ToList();
                    if (products != null) db.product.RemoveRange(products.Skip(1));
                }
                db.SaveChanges();
            }
            Console.WriteLine("Done with removing duplicate unique ids.");
        }

        private static void CleanupUrlDupes()
        {
            using (var db = new BetsyModel(ConnectionString))
            {
                Console.WriteLine("Started with removing empty urls...");
                var emptyUrls = db.product.Where(p => p.direct_link == "");
                db.product.RemoveRange(emptyUrls);
                db.SaveChanges();
                Console.WriteLine("Done with removing empty urls.");
                /*Console.WriteLine("Started with looking up duplicate urls...");
                var duplicateUrls = db.product.GroupBy(produit => produit.direct_link).Where(x => x.Count() > 1).Select(val => val.Key);
                Console.WriteLine("Removing now, this can take a while.");
                foreach (string dupe in duplicateUrls)
                {
                    var produits = db.product.Where(produit => produit.direct_link == dupe).ToList();
                    if (produits != null) db.product.RemoveRange(produits.Skip(1));
                }
                db.SaveChanges();
                Console.WriteLine("Done with removing duplicate urls.");*/
            }         
        }

        private static void Initialize()
        {

            #region ConnectionString
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = (uint)Properties.PropertyList["db_max_pool_size"].GetValue<int>();
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Database = Properties.PropertyList["db_name"].GetValue<string>();
            providerConnStrBuilder.Password = Properties.PropertyList["db_password"].GetValue<string>();
            providerConnStrBuilder.Server = Properties.PropertyList["db_source"].GetValue<string>();
            providerConnStrBuilder.UserID = Properties.PropertyList["db_userid"].GetValue<string>();
            providerConnStrBuilder.Port = (uint)Properties.PropertyList["db_port"].GetValue<int>();


            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
            #endregion ConnectionString
        }
    }

    public class AffiliateIdentifier
    {
        public string Id;
        public string Name;
    }
}
