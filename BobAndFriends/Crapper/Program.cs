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
                var duplicateEans = db.ean.GroupBy(e => e.ean1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                foreach (var ean in duplicateEans)
                {
                    Console.WriteLine("Busy with: " + ean);
                    article correctArticle = db.article.Where(a => a.ean.Any(e => e.ean1 == ean)).FirstOrDefault();
                    List<article> wrongArticles = db.article.Where(a => a.ean.Any(e => e.ean1 == ean)).ToList();
                    wrongArticles.Remove(correctArticle);
                    foreach (article wrongArticle in wrongArticles)
                    {
                        //Set product article id to correctArticle.id
                        db.product.Where(p => p.article_id == wrongArticle.id).ToList().ForEach(p => p.article_id = correctArticle.id);

                        //Add titles of wrongArticle to synonyms of correctArticle.
                        //If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in wrongArticle.title.ToList())
                        {
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title == title.title1)))
                                {
                                    correctArticle.title.ToList().ForEach(t => t.title_synonym.Where(ts => ts.title == title.title1).FirstOrDefault().occurrences++);
                                }
                                else
                                {
                                    correctArticle.title.FirstOrDefault().title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title.id });
                                }

                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }
                        }

                        //Add all title synonyms of wrongArticle to correctArticle withing the correct countryID
                        wrongArticle.title.ToList().ForEach(t => t.title_synonym.ToList().ForEach(ts => correctArticle.title.Where(ct => ct.country_id == t.country_id).FirstOrDefault().title_synonym.Add(ts)));

                        Console.WriteLine("Redirected " + correctArticle.title.First().title1 + " to " + wrongArticle.title.First().title1 + ".");

                        db.article.Remove(wrongArticle);
                    }
                }
                db.SaveChanges();
            }
        }



        static void CleanupTitleDupes()
        {
            Console.WriteLine("Started looking for Title dupes...");
            using (var db = new BetsyModel(BorderSource.Common.Database.Instance.GetConnectionString()))
            {
                var duplicateTitles = db.title.GroupBy(t => t.title1).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                foreach (var dupTitle in duplicateTitles)
                {
                    Console.WriteLine("Busy with: " + dupTitle);                    
                    article correctArticle = db.article.Where(a => a.title.Any(t => t.title1 == dupTitle)).FirstOrDefault();
                    List<article> wrongArticles = db.article.Where(a => a.title.Any(t => t.title1 == dupTitle)).ToList();
                    wrongArticles.Remove(correctArticle);
                    foreach (article wrongArticle in wrongArticles)
                    {
                        //Set product article id to correctArticle.id
                        db.product.Where(p => p.article_id == wrongArticle.id).ToList().ForEach(p => p.article_id = correctArticle.id);

                        //Add titles of wrongArticle to synonyms of correctArticle.
                        //If correctArticle already has a title with the same country id, add it to the synonyms.
                        foreach (title title in wrongArticle.title.ToList())
                        {
                            //First add title to the title or synonym
                            if (correctArticle.title.Any(t => t.country_id == title.country_id))
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(ts => ts.title == title.title1)))
                                {
                                    correctArticle.title.ToList().ForEach(t => t.title_synonym.Where(ts => ts.title == title.title1).FirstOrDefault().occurrences++);
                                }
                                else
                                {
                                    correctArticle.title.FirstOrDefault().title_synonym.Add(new title_synonym { title = title.title1, occurrences = 1, title_id = title.id });
                                }

                            }
                            else
                            {
                                correctArticle.title.Add(title);
                            }

                            //Then add the synonyms of the title to the synonym
                            foreach (title_synonym ts in title.title_synonym)
                            {
                                if (correctArticle.title.Any(t => t.title_synonym.Any(cts => cts.title == ts.title)))
                                {
                                    correctArticle.title.ToList().ForEach(t => t.title_synonym.Where(cts => cts.title == ts.title).FirstOrDefault().occurrences++);
                                }
                                else
                                {
                                    correctArticle.title.FirstOrDefault().title_synonym.Add(ts);
                                }
                            }

                        }

                        Console.WriteLine("Redirected " + correctArticle.title.First().title1 + " to " + wrongArticle.title.First().title1 + ".");

                        db.article.Remove(wrongArticle);
                    }
                }
            }
        }

        static void Initialize()
        {
            Console.WriteLine("Initializing... ");
            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion
        }
    }
}

