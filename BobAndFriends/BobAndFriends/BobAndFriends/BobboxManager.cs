using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Common;
using BorderSource.BetsyContext;
using BorderSource.Queue;

namespace BobAndFriends.BobAndFriends
{
    public class BobboxManager
    {
        private BobBox BobBox;
        private HashSet<string> AddedCategorySynonyms;
        private int Count;
        private const int PackageSize = 1000;

        public BobboxManager()
        {
            BobBox = new BobBox();
            AddedCategorySynonyms = new HashSet<string>();
            Count = 0;
        }

        public void StartValidatingAndSaving()
        {
            ProductValidation validation = ProductValidationQueue.Instance.Dequeue();
            string CurrentWebshop = validation.Product.Webshop;
            while (validation != null)
            {
                if ((CurrentWebshop != validation.Product.Webshop) || (Count >= PackageSize))
                {
                    Console.WriteLine("Saving changes for " + CurrentWebshop + " to database...");
                    BobBox.CommitAndCreate();
                    if (Count < PackageSize) AddedCategorySynonyms.Clear();
                    Console.WriteLine("Done saving changes for " + CurrentWebshop);
                    Count = 0;
                }

                CurrentWebshop = validation.Product.Webshop;

                if (validation.IsAmbiguous())
                {
                    using (Logger logger = new Logger(Statics.settings["logpath"] + "\\ambiguous-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"))
                    {
                        logger.WriteLine("Ambiguous product matched " + validation.ArticleNumberOfEanMatch + " with " + validation.ArticleNumberOfSkuMatch + ".");
                    }
                }

                if (!validation.CategoryMatched) goto Next;

                if (validation.ProductAlreadyExists)
                {
                    BobBox.SaveProductData(validation.Product, validation.ArticleNumberOfExistingProduct);
                    goto Next;
                }

                if (validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists && !AddedCategorySynonyms.Contains(validation.Product.Category))
                    {
                        AddedCategorySynonyms.Add(validation.Product.Category);
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category, validation.Product.Webshop);
                    }
                    BobBox.SaveMatch(validation.Product, validation.ArticleNumberOfEanMatch, validation.CountryId);
                    goto Next;
                }

                if (validation.SkuMatched && !validation.EanMatched)
                {
                    if (!validation.CategorySynonymExists && !AddedCategorySynonyms.Contains(validation.Product.Category))
                    {
                        AddedCategorySynonyms.Add(validation.Product.Category);
                        BobBox.InsertIntoCatSynonyms(validation.CategoryId, validation.Product.Category, validation.Product.Webshop);
                    }
                    BobBox.SaveMatch(validation.Product, validation.ArticleNumberOfSkuMatch, validation.CountryId);
                    goto Next;
                }

                if(validation.IsValidAsNewArticle)
                {
                    //BobBox.SaveNewArticle(validation.Product, validation.CountryId, validation.CategoryId);
                    goto Next;
                }

            Next:
                {
                    validation = ProductValidationQueue.Instance.Dequeue();
                    Count++;                  
                }
            }

            BobBox.CommitAndDispose();
        }
    }
}