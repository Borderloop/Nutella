using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Globalization;
using BorderSource.BetsyContext;
using BorderSource.Common;
using BorderSource.Queue;
using BorderSource.ProductAssociation;

namespace BobAndFriends.BobAndFriends
{
    public class BOB : IDisposable
    {
        private BetsyDbContextReader db;

        public BOB()
        {
            db = new BetsyDbContextReader();
        }

        /// <summary>
        /// This is the main method. This is where the algorithm find its chronologically order and
        /// all methods are called in this order. 
        /// </summary>
        public void Process(Package p)
        {
            int countryID = Lookup.WebshopLookup[p.Webshop.ToLower().Trim()].FirstOrDefault().CountryId;
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

            //Filter existing products
            foreach(KeyValuePair<Product, int> pair in db.GetExistingProductIds(p.products, p.Webshop))
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

            if (p.products.All(prod => prod.IsValidated)) return;

            foreach(KeyValuePair<Product, int> pair in db.GetEanMatches(p.products.Where(prod => !prod.IsValidated).ToList()))
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

            if (p.products.All(prod => prod.IsValidated)) return;

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

            if (p.products.All(prod => prod.IsValidated)) return;
            /*
            foreach (Product Record in p.products.Where(prod => !prod.IsValidated).ToList())
            {
                bool ProductIsValid = Record.EAN.Length > 0 && Record.Title.Length > 0 && Record.Brand.Length > 0;
                bool CategoryExists = Lookup.CategoryLookup.Contains(Record.Category.ToLower().Trim()) || Lookup.CategorySynonymLookup.Contains(Record.Category.ToLower().Trim());
                if (ProductIsValid && CategoryExists)
                {
                    ProductValidation validation = new ProductValidation();
                    validation.Product = Record;
                    validation.IsValidAsNewArticle = ProductIsValid && CategoryExists;
                    validation.CategorySynonymExists = Lookup.CategorySynonymLookup.Contains(Record.Category.ToLower().Trim());

                    validation.CategoryId = validation.CategorySynonymExists ? Lookup.CategorySynonymLookup[Record.Category.ToLower().Trim()].First().CategoryId : Lookup.CategoryLookup[Record.Category.ToLower().Trim()].First().Id;

                    ProductValidationQueue.Instance.Enqueue(validation);
                }               
            } */                                          
        }       

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
