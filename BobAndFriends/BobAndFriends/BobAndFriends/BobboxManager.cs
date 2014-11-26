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
        private const int PackageSize = 500;
        private int MaxCountryId;
        private int ExistingProducts, EanMatches, SkuMatches = 0;

        public BobboxManager()
        {
            BobBox = new BobBox();          
            Count = 0;
            MaxCountryId = Lookup.WebshopLookup.Max(m => m.Max(n => n.CountryId));
            Console.WriteLine("Found max country id: " + MaxCountryId);
        }

        public void StartValidatingAndSaving()
        {
            ProductValidation validation = ProductValidationQueue.Instance.Dequeue();
            while (validation != null)
            {
                if(validation.Product == null)
                {
                    Console.WriteLine("Found null product.");
                    goto Next;
                }
                
                if ((Count >= PackageSize))
                {
                    Console.WriteLine("Saving changes for " + PackageSize + " INSERTS/UPDATES to database.");
                    Console.WriteLine("Totals; Existing products: {0}, EanMatches: {1}, SkuMatches: {2}", ExistingProducts, EanMatches, SkuMatches);
                    BobBox.CommitAndCreate();
                    Console.WriteLine("Done saving changes.");
                    Count = 0;
                }

                if (validation.CountryId < 1 || validation.CountryId > MaxCountryId) goto Next;
                if (!validation.CategoryMatched) goto Next;

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

                /*if(validation.IsValidAsNewArticle)
                {
                    BobBox.SaveNewArticle(validation.Product, validation.CountryId, validation.CategoryId);
                    goto Next;
                }*/

            Next:
                {
                    GeneralStatisticsMapper.Instance.Increment("Products saved");
                    validation = ProductValidationQueue.Instance.Dequeue();
                    Count++;                  
                }
            }           
            BobBox.CommitAndDispose();
            Console.WriteLine("Done saving to database.");
        }
    }
}