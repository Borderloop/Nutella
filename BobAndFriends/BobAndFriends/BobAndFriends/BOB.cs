using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Globalization;
using BorderSource.BetsyContext;
using BorderSource.Common;
using BorderSource.Queue;
using BorderSource.ProductAssociation;
using BobAndFriends.Global;

namespace BobAndFriends.BobAndFriends
{
    public class BOB : IDisposable
    {
        private BetsyDbContextReader db;

        private int _maxValidationQueueSize;

        public BOB()
        {
            string dbName = Properties.PropertyList["db_name"].GetValue<string>();
            string dbPassword = Properties.PropertyList["db_password"].GetValue<string>();
            string dbSource = Properties.PropertyList["db_source"].GetValue<string>();
            string dbUserId = Properties.PropertyList["db_userid"].GetValue<string>();
            int dbPort = Properties.PropertyList["db_port"].GetValue<int>();
            int maxPoolSize = Properties.PropertyList["db_max_pool_size"].GetValue<int>();
            db = new BetsyDbContextReader(dbName, dbPassword, dbSource, dbUserId, dbPort, maxPoolSize);
            _maxValidationQueueSize = Properties.PropertyList["max_validationqueue_size"].GetValue<int>();
        }

        /// <summary>
        /// This is the main method. This is where the algorithm find its chronologically order and
        /// all methods are called in this order. 
        /// </summary>
        public void Process(Package p)
        {
            while (_maxValidationQueueSize == 0 ? false : ProductValidationQueue.Instance.Queue.Count > _maxValidationQueueSize) Thread.Sleep(1000);
            Webshop ws = Lookup.WebshopLookup[p.Webshop.ToLower().Trim()].FirstOrDefault();
            if (ws.Equals(null)) return;
            int countryID = ws.CountryId;

            Lookup.CategorySynonymLookup = db.GetCategorySynonymsForWebshop(p.Webshop.ToLower().Trim());

            //Cleanup titles
            foreach(Product Record in p.products)
            {
                if (Record.Brand != "" && Record.Title.Contains(Record.Brand, StringComparison.OrdinalIgnoreCase))
                {
                    string[] s = Record.Title.Split(new string[] { Record.Brand }, StringSplitOptions.RemoveEmptyEntries);
                    Record.Title = String.Concat(s).Trim();
                }
            }

            if (Properties.PropertyList["update_enabled"].GetValue<bool>())
            {
                //Filter existing products
                foreach (KeyValuePair<Product, int> pair in db.GetExistingProductIds(p.products, p.Webshop))
                {
                    if (pair.Key == null) continue;
                    ProductValidation validation = new ProductValidation();
                    validation.Product = pair.Key;
                    validation.CountryId = countryID;
                    validation.ArticleNumberOfExistingProduct = pair.Value;
                    validation.CategoryId = db.GetCategoryNumber(pair.Value);
                    ProductValidationQueue.Instance.Enqueue(validation);
                    pair.Key.IsValidated = true;
                }
            }
            
            if (p.products.All(prod => prod.IsValidated)) return;

            if (Properties.PropertyList["insert_match_enabled"].GetValue<bool>())
            {
                foreach (KeyValuePair<Product, int> pair in db.GetEanMatches(p.products.Where(prod => !prod.IsValidated).ToList()))
                {
                    if (pair.Key == null) continue;
                    ProductValidation validation = new ProductValidation();
                    validation.Product = pair.Key;
                    validation.CountryId = countryID;
                    validation.ArticleNumberOfEanMatch = pair.Value;
                    validation.CategoryId = db.GetCategoryNumber(pair.Value);
                    ProductValidationQueue.Instance.Enqueue(validation);
                    pair.Key.IsValidated = true;
                }
            }

            if (p.products.All(prod => prod.IsValidated)) return;

            /*
            if (Properties.PropertyList["insert_match_enabled"].GetValue<bool>()) 
            {
            foreach (KeyValuePair<Product, int> pair in db.GetSkuMatches(p.products.Where(prod => !prod.IsValidated).ToList()))
            {
                if (pair.Key == null) continue;
                ProductValidation validation = new ProductValidation();
                validation.Product = pair.Key;
                validation.CountryId = countryID;
                validation.ArticleNumberOfSkuMatch = pair.Value;
                validation.CategoryId = db.GetCategoryNumber(pair.Value);
                ProductValidationQueue.Instance.Enqueue(validation);
                pair.Key.IsValidated = true;
            }
            }
            
            if (p.products.All(prod => prod.IsValidated)) return;
            */

            if (Properties.PropertyList["insert_new_enabled"].GetValue<bool>())
            {
                foreach (Product Record in p.products.Where(prod => !prod.IsValidated).ToList())
                {
                    bool ProductIsValid = Record.EAN.Length > 0 && Record.Title.Length > 0 && Record.Brand.Length > 0;
                    bool CategoryExists = Lookup.CategoryLookup.Contains(Record.Category.ToLower().Trim()) || Lookup.CategorySynonymLookup.Contains(Record.Category.ToLower().Trim());

                    int catId = -1;
                    CategorySynonym catSyn = Lookup.CategorySynonymLookup[Record.Category.ToLower().Trim()].FirstOrDefault();
                    if (catSyn == null)
                    {
                        Category cat = Lookup.CategoryLookup[Record.Category.ToLower().Trim()].FirstOrDefault();
                        if (cat == null) continue;
                        catId = cat.Id;
                    }
                    else
                    {
                        catId = catSyn.CategoryId;
                    }

                    if (ProductIsValid && CategoryExists)
                    {
                        ProductValidation validation = new ProductValidation();
                        validation.Product = Record;
                        validation.CountryId = countryID;
                        validation.IsValidAsNewArticle = ProductIsValid && CategoryExists;
                        validation.CategorySynonymExists = Lookup.CategorySynonymLookup.Contains(Record.Category.ToLower().Trim());

                        validation.CategoryId = catId;
                        ProductValidationQueue.Instance.Enqueue(validation);
                    }
                }
            }                          
        }       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        ~BOB()
        {
            Dispose(false);
        }
    }
}
