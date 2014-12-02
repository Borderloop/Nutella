using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Reflection;
using BorderSource.BetsyContext;
using System.Data.Entity.Validation;
using BorderSource.ProductAssociation;
using BorderSource.Statistics;

namespace BorderSource.Common
{
    public class BobBox
    {
        /// <summary>
        /// The string used to make a connection to the database
        /// </summary>
        public string ConnectionString { get; set; }

        private BetsyModel context { get; set; }

        /// <summary>
        /// The constructor
        /// </summary>
        public BobBox(string dbname, string dbpw, string dbsource, string dbuid, int port, int maxpoolsize = 120)
        {
            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = (uint)maxpoolsize;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Port = (uint)port;
            providerConnStrBuilder.Database = dbname;
            providerConnStrBuilder.Password = dbpw;
            providerConnStrBuilder.Server = dbsource;
            providerConnStrBuilder.UserID = dbuid;

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            ConnectionString = entityConnStrBuilder.ConnectionString;
            context = new BetsyModel(ConnectionString);
            context.Configuration.LazyLoadingEnabled = true;
            context.Configuration.AutoDetectChangesEnabled = false;
            //context.Configuration.ValidateOnSaveEnabled = false;
        }

        public void SaveMatch(Product Record, int matchedArticleID, int countryID)
        {
            //First get all data needed for matching. Ean, sku and title_synonym are seperate because they can store multiple values.
            article articleTable = context.article.Where(a => a.id == matchedArticleID).FirstOrDefault();

            context.article.Attach(articleTable);
            var articleEntry = context.Entry(articleTable);
            
            if (articleTable.brand == "" || articleTable.brand == null)
            {
                articleTable.brand = Record.Brand;
                articleEntry.Property(a => a.brand).IsModified = true;
            }
            if (articleTable.description == null)
            {
                articleTable.description = Record.Description;
                articleEntry.Property(a => a.description).IsModified = true;
            }
            if (articleTable.image_loc == null || articleTable.image_loc == "")
            {
                articleTable.image_loc = Record.Image_Loc;
                articleEntry.Property(a => a.image_loc).IsModified = true;
            }
            long ean;
            bool eanIsParsable = long.TryParse(Record.EAN, out ean);
            //Loop through ean and sku collections to check if the ean or sku already exists. If not, add it
            if (eanIsParsable && !(articleTable.ean.Any(e => e.ean1 == ean))) articleTable.ean.Add(new ean { ean1 = ean, article_id = matchedArticleID });
            if (Record.SKU != "" && !(articleTable.sku.Any(s => s.sku1.ToUpper().Trim() == Record.SKU.ToUpper().Trim()))) articleTable.sku.Add(new sku { sku1 = Record.SKU, article_id = matchedArticleID });

            
            title title = articleTable.title.Where(t => t.article_id == matchedArticleID && t.country_id == countryID).FirstOrDefault();

            if (title == default(title))
            {
                title addedTitle = new title { title1 = Record.Title, country_id = (short)countryID };
                addedTitle.title_synonym.Add(new title_synonym { occurrences = 1, title = Record.Title.Trim() });
                articleTable.title.Add(addedTitle);
            }
            else
            {
                title_synonym match = null;
                foreach (title_synonym syn in title.title_synonym)
                {
                    if (syn.title.RemoveDiacriticAccents().ToLower().Trim() == Record.Title.RemoveDiacriticAccents().ToLower().Trim())
                    {
                        match = syn;
                        break;
                    }
                }
                //If any title synonym matches the title, up the occurences.
                if (match != null)
                {
                    match.occurrences++;
                }
                /*if (ts.occurrences > articleTable.title.Max(t => t.title_synonym.Max(ts2 => ts2.occurrences)))
                {
                    title.title1 = ts.title;
                    context.title.Attach(title);
                    var titleEntry = context.Entry(title);
                    titleEntry.Property(t => t.title1).IsModified = true;
                }*/
                //else, add the title to the synonyms.
                else
                {
                    //title.title_synonym.Add(new title_synonym { occurrences = 1, title = Record.Title.Trim() });
                }
            }
            

            decimal castedShipCost;
            decimal castedPrice;
            if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) { }
            if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) { }

            product newProduct = new product
            {
                article_id = matchedArticleID,
                ship_time = Record.DeliveryTime,
                ship_cost = (decimal?)castedShipCost,
                price = castedPrice,
                webshop_url = Record.Webshop,
                direct_link = Record.Url,
                affiliate_name = Record.Affiliate,
                affiliate_unique_id = Record.AffiliateProdID,
                last_modified = System.DateTime.Now
            };

            articleTable.product.Add(newProduct);
        }

        /// This method will send a product to the residu.
        /// </summary>
        /// <param name="p">The product to be send to the residu.</param>
        public int SendToVBobData(Product p)
        {
            vbobdata res = new vbobdata
            {
                title = p.Title,
                description = p.Description,
                category = p.Category,
                ean = p.EAN,
                sku = p.SKU,
                brand = p.Brand,
                rerun = false,
                image_loc = p.Image_Loc
            };
            context.vbobdata.Add(res);
            return res.id;

        }

        /// <summary>
        /// Used to save a new article.
        /// </summary>
        /// <param name="Record">The article to be saved</param>
        public void SaveNewArticle(Product Record, int countryId, int categoryId)
        {
            category cat = context.category.Where(c => c.id == categoryId).FirstOrDefault();

            if (cat == default(category))
            {
                Console.WriteLine("Could not find category id {0}, aborting the save.", categoryId);
                return;
            }
            article art = new article
            {
                description = Record.Description,
                brand = Record.Brand,
                image_loc = Record.Image_Loc,
            };
            art.category.Add(cat);
            //Do not modify this as this is neccessary to get the last id.
            

            long eanVal;
            bool eanIsParsable = long.TryParse(Record.EAN, out eanVal);

            if (eanIsParsable)
            {
                ean ean = new ean
                {
                    ean1 = eanVal,
                };
                art.ean.Add(ean);
            }

            title title = new title
            {
                title1 = Record.Title,
                country_id = (short)countryId,
            };          

            title_synonym ts = new title_synonym
            {
                title = Record.Title,
                occurrences = 1
            };
            title.title_synonym.Add(ts);
            art.title.Add(title);

            if (Record.SKU != "")
            {
                sku sku = new sku
                {
                    sku1 = Record.SKU,
                };
                art.sku.Add(sku);
            }

            decimal castedShipCost;
            decimal castedPrice;
            decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost);
            decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice);

            product product = new product
            {
                ship_cost = castedShipCost,
                ship_time = Record.DeliveryTime,
                price = castedPrice,
                webshop_url = Record.Webshop,
                direct_link = Record.Url,
                affiliate_name = Record.Affiliate,
                affiliate_unique_id = Record.AffiliateProdID,
                last_modified = System.DateTime.Now
            };
            art.product.Add(product);

            context.article.Add(art);
        }

        public void DeleteFromVbobData(int id)
        {
            var sugdata = context.vbob_suggested.Where(vs => vs.vbob_id == id).FirstOrDefault();
            context.vbob_suggested.Remove(sugdata);

            var vbobdata = context.vbobdata.Where(v => v.id == id).FirstOrDefault();
            context.vbobdata.Remove(vbobdata);

        }

        public void RerunVbobEntry(vbobdata rerun)
        {
            context.vbobdata.Add(rerun);
        }


        /// <summary>
        /// Inserts the found matches for the product in the vbob_suggested table
        /// </summary>
        /// <param name="vBobId">The id of the record as stored in the vbobdata table</param>
        /// <param name="articleIds">List of article_ids that match the record. First id has the most relevance, last is the least.</param>
        private void InsertInVBobSuggested(int vBobId, List<article> articleIds)
        {
            foreach (article a in articleIds)
            {
                context.vbob_suggested.Add(new vbob_suggested { vbob_id = vBobId, article_id = a.id });
            }
        }

        /// <summary>
        /// Saves product data for a given record.
        /// </summary>
        /// <param name="Record">The record of which the product data will be saved.</param>
        /// <param name="aId">The article id the record belongs to.</param>
        public void SaveProductData(Product Record, int matchedArticleId)
        {
            decimal castedShipCost;
            decimal castedPrice;
            if (!(decimal.TryParse(Record.DeliveryCost, NumberStyles.Any, CultureInfo.InvariantCulture, out castedShipCost))) { }
            if (!(decimal.TryParse(Record.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out castedPrice))) { }

            product UpdatedProduct = new product
            {
                article_id = matchedArticleId,
                ship_time = Record.DeliveryTime,
                ship_cost = (decimal?)castedShipCost,
                price = castedPrice,
                webshop_url = Record.Webshop,
                direct_link = Record.Url,
                affiliate_name = Record.Affiliate,
                affiliate_unique_id = Record.AffiliateProdID,
                last_modified = System.DateTime.Now
            };

            var original = context.product.Where(p => p.article_id == matchedArticleId && p.affiliate_unique_id == Record.AffiliateProdID).FirstOrDefault();          
            if (original != null)
            {
                original.ship_cost = UpdatedProduct.ship_cost;
                original.ship_time = UpdatedProduct.ship_time;
                original.price = UpdatedProduct.price;
                original.direct_link = UpdatedProduct.direct_link;
                original.affiliate_name = UpdatedProduct.affiliate_name;
                original.affiliate_unique_id = UpdatedProduct.affiliate_unique_id;
                original.last_modified = UpdatedProduct.last_modified;
            }
            else
            {
                Console.WriteLine("Trying to save product where it should've been updated!");
                context.product.Add(UpdatedProduct);
            }
        }

        /// <summary>
        /// This method add category to category_synonym
        /// </summary>
        /// <param name="catid">Category_id</param>
        /// <param name="description">Description</param>
        /// <param name="web_url">Webshop</param>
        public void InsertIntoCatSynonyms(int catid, string description, string web_url)
        {
            var original = context.category_synonym.Where(cs => cs.description == description && cs.web_url == web_url).FirstOrDefault();
            if (original == null)
            {
                context.category_synonym.Add(new category_synonym { category_id = catid, description = description, web_url = web_url });
            }
        }

        /// <summary>
        /// This method insert the category id and article id thats matched and insert this to the cat-article table.
        /// </summary>
        /// <param name="category_id">The category id from the article</param>
        /// <param name="article_id">The article id of the inserted product</param>
        public void InsertNewCatArtile(int category_id, int article_id)
        {
            category catToAdd = context.category.Where(c => c.id == category_id).FirstOrDefault();
            context.article.Where(a => a.id == article_id).FirstOrDefault().category.Add(catToAdd);
        }

        public void Commit()
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Caught DbUpdateConcurrenryException, trying refresh.");
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                try
                {
                    objContext.Refresh(RefreshMode.ClientWins, ex.Entries.Select(e => e.Entity));
                }
                catch (InvalidOperationException ioe)
                {
                    Console.WriteLine("Refresh didn't work, lost 500 products. Error: " + ioe.Message);
                    Console.WriteLine("Moving on.");
                }
            }
        }

        public void CommitAndDispose()
        {
            try
            {
                context.SaveChanges();               
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;

            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Caught DbUpdateConcurrenryException, trying refresh.");
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                try
                {
                    objContext.Refresh(RefreshMode.ClientWins, ex.Entries.Select(e => e.Entity));
                }
                catch (InvalidOperationException ioe)
                {
                    Console.WriteLine("Refresh didn't work, lost 500 products. Error: " + ioe.Message);
                    Console.WriteLine("Moving on.");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException.Message.Contains("Duplicate")) Console.WriteLine("Threw duplicate exception.");
                else
                {
                    Console.WriteLine("Lost 500 products due to an error; " + e.Message);
                    Console.WriteLine("Inner: " + e.InnerException.Message);
                    Console.WriteLine("InnerInner: " + e.InnerException.InnerException.Message);
                    GeneralStatisticsMapper.Instance.Increment("Amount of products not updated because of an error (x500)");
                }
            }
            finally
            {
                context.Dispose();
            }
        }

        public void CommitAndCreate()
        {
            try
            {
                context.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;

            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("Caught DbUpdateConcurrenryException, trying refresh.");
                var objContext = ((IObjectContextAdapter)context).ObjectContext;
                try
                {
                    objContext.Refresh(RefreshMode.ClientWins, ex.Entries.Select(e => e.Entity));
                }
                catch(InvalidOperationException ioe)
                {
                    Console.WriteLine("Refresh didn't work, lost 500 products. Error: " + ioe.Message);
                    Console.WriteLine("Moving on.");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException.Message.Contains("Duplicate")) Console.WriteLine("Threw duplicate exception.");
                else
                {
                    Console.WriteLine("Lost 500 products due to an error; " + e.Message);
                    Console.WriteLine("Inner: " + e.InnerException.Message);
                    Console.WriteLine("InnerInner: " + e.InnerException.InnerException.Message);
                    GeneralStatisticsMapper.Instance.Increment("Amount of products not updated because of an error (x500)");
                }
            }
            finally
            {
                context.Dispose();
                context = new BetsyModel(ConnectionString);
            }

        }

    
    }
}
