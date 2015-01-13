using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Common;
using BorderSource.BetsyContext;
using BorderSource.Queue;
using BorderSource.ProductAssociation;
using BorderSource.Statistics;
using BobAndFriends.Global;

namespace BobAndFriends.BobAndFriends
{
    public class BobboxManager
    {
        private BobBox BobBox;
        private int Count;
        private int PackageSize;
        private int MaxCountryId;
        private int ExistingProducts, EanMatches, SkuMatches, NewProducts, NoCategory, WrongCountry = 0;

        public BobboxManager()
        {
            string dbName = Properties.PropertyList["db_name"].GetValue<string>();
            string dbPassword = Properties.PropertyList["db_password"].GetValue<string>();
            string dbSource = Properties.PropertyList["db_source"].GetValue<string>();
            string dbUserId = Properties.PropertyList["db_userid"].GetValue<string>();
            int dbPort = Properties.PropertyList["db_port"].GetValue<int>();
            int maxPoolSize = Properties.PropertyList["db_max_pool_size"].GetValue<int>();
            BobBox = new BobBox(dbName, dbPassword, dbSource, dbUserId, dbPort, maxPoolSize);       
            Count = 0;
            MaxCountryId = Lookup.WebshopLookup.Max(m => m.Value.CountryId);
            PackageSize = Properties.PropertyList["validations_per_save"].GetValue<int>();
        }

        public void StartValidatingAndSaving(int id)
        {
            ProductValidation validation = ProductValidationQueue.Instance.Dequeue();
            while (validation != null)
            {
                if (validation.Product == null)
                {
                    Console.WriteLine("Found null product.");
                    goto Next;
                }
                
                if ((Count >= PackageSize))
                {
                    Console.WriteLine(id + ": Saving changes for " + Count + " INSERTS/UPDATES to database.");
                    Console.WriteLine(id + ": Totals; EP: {0}, EM: {1}, New: {2}, NC: {3}, WC: {4}", ExistingProducts, EanMatches, NewProducts, NoCategory, WrongCountry);
                    BobBox.CommitAndCreate();
                    Console.WriteLine(id + ": Done saving changes.");
                    Count = 0;
                }

                if (validation.CountryId < 1 || validation.CountryId > MaxCountryId)
                {
                    WrongCountry++;
                    goto Next;
                }
                if (!validation.CategoryMatched)
                {
                    NoCategory++;
                    goto Next;
                }

                if (validation.ProductAlreadyExists)
                {
                    GeneralStatisticsMapper.Instance.Increment("Existing products");
                    ExistingProducts++;
                    BobBox.SaveProductData(validation.Product, validation.ArticleNumberOfExistingProduct);
                    goto Next;
                }

                if (validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists && !GlobalVariables.AddedCategorySynonyms.Contains(validation.Product.Category))
                    {
                        GlobalVariables.AddedCategorySynonyms.Add(validation.Product.Category);
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category.ToLower().Trim(), validation.Product.Webshop.ToLower().Trim());
                    }
                    GeneralStatisticsMapper.Instance.Increment("EAN matches");
                    EanMatches++;
                    BobBox.SaveMatch(validation.Product, validation.ArticleNumberOfEanMatch, validation.CountryId);
                    goto Next;
                }

                if (validation.SkuMatched && !validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists && !GlobalVariables.AddedCategorySynonyms.Contains(validation.Product.Category))
                    {
                        GlobalVariables.AddedCategorySynonyms.Add(validation.Product.Category);
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category.ToLower().Trim(), validation.Product.Webshop.ToLower().Trim());
                    }
                    GeneralStatisticsMapper.Instance.Increment("SKU matches");
                    SkuMatches++;
                    BobBox.SaveMatch(validation.Product, validation.ArticleNumberOfSkuMatch, validation.CountryId);
                    goto Next;
                }

                if (validation.IsValidAsNewArticle)
                {
                    GeneralStatisticsMapper.Instance.Increment("New products");
                    NewProducts++;
                    BobBox.SaveNewArticle(validation.Product, validation.CountryId, validation.CategoryId);
                    goto Next;
                }

            Next:
                {
                    GeneralStatisticsMapper.Instance.Increment("Products saved");
                    validation = ProductValidationQueue.Instance.Dequeue();
                    Count++;                  
                }
            }           
            BobBox.CommitAndDispose();
            Console.WriteLine(id + ": Done saving last changes to database.");
        }
    }
}