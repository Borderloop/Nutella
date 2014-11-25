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

namespace BobAndFriends.BobAndFriends
{
    public class BobboxManager
    {
        private BobBox BobBox;
        private int Count;
        private const int PackageSize = 500;
        private int ExistingProducts, EanMatches, SkuMatches = 0;

        public BobboxManager()
        {
            BobBox = new BobBox();          
            Count = 0;
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

                if (!validation.CategoryMatched) goto Next;

                if (validation.ProductAlreadyExists)
                {
                    ExistingProducts++;
                    BobBox.SaveProductData(validation.Product, validation.ArticleNumberOfExistingProduct);
                    goto Next;
                }

                if (validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists)
                    {
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category.ToLower().Trim(), validation.Product.Webshop.ToLower().Trim());
                    }
                    EanMatches++;
                    BobBox.SaveMatch(validation.Product, validation.ArticleNumberOfEanMatch, validation.CountryId);
                    goto Next;
                }

                if (validation.SkuMatched && !validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists)
                    {
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category.ToLower().Trim(), validation.Product.Webshop.ToLower().Trim());
                    }
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
                    GeneralStatisticsMapper.Instance.Increment("Products processed");
                    validation = ProductValidationQueue.Instance.Dequeue();
                    Count++;                  
                }
            }           
            BobBox.CommitAndDispose();
            Console.WriteLine("Done saving to database.");
        }
    }
}