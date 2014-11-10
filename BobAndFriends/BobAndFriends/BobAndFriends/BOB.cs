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

namespace BobAndFriends.BobAndFriends
{
    public class BOB
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
            int matchedArticleID;
            int countryID = Lookup.WebshopLookup[p.Webshop].FirstOrDefault().CountryId;
            Lookup.CategorySynonymLookup = db.GetCategorySynonymsForWebshop(p.Webshop);
            foreach (Product Record in p.products)
            {      
                ProductValidation validation = new ProductValidation();
                validation.Product = Record;
                validation.CountryId = countryID;

                if (Record.Brand != "" && Record.Title.Contains(Record.Brand, StringComparison.OrdinalIgnoreCase))
                {
                    string[] s = Record.Title.Split(new string[] { Record.Brand }, StringSplitOptions.RemoveEmptyEntries);
                    Record.Title = String.Concat(s).Trim();
                }

                bool ProductExists = (matchedArticleID = CheckIfProductExists(Record)) != -1;
                validation.ArticleNumberOfExistingProduct = ProductExists ? matchedArticleID : -1;              

                if (ProductExists)
                {                                      
                    ProductValidationQueue.Instance.Enqueue(validation);
                    continue;
                }
                
                bool SkuMatched = ((Record.SKU.Length >= 3) && (matchedArticleID = checkSKU(Record.SKU)) != -1);
                validation.ArticleNumberOfSkuMatch = SkuMatched ? matchedArticleID : -1;

                bool EanMatched = ((!Record.EAN.Equals("")) && (matchedArticleID = checkEAN(Record.EAN)) != -1);
                validation.ArticleNumberOfEanMatch = EanMatched ? matchedArticleID : -1;

                bool ProductIsValid = Record.EAN.Length > 0 && Record.Title.Length > 0 && Record.Brand.Length > 0;
                bool CategoryExists = Lookup.CategoryLookup.Contains(Record.Category) || Lookup.CategorySynonymLookup.Contains(Record.Category);

                validation.IsValidAsNewArticle = !(EanMatched || SkuMatched) && ProductIsValid && CategoryExists;

                if(!(EanMatched || SkuMatched || validation.IsValidAsNewArticle)) continue;
            
                validation.CategoryId = validation.IsValidAsNewArticle ? 
                                                    Lookup.CategorySynonymLookup.Contains(Record.Category) ? 
                                                    Lookup.CategorySynonymLookup[Record.Category].First().CategoryId : Lookup.CategoryLookup[Record.Category].First().Id 
                                            : GetCategoryId(matchedArticleID);

                validation.CategorySynonymExists = Lookup.CategorySynonymLookup.Contains(Record.Category);

                ProductValidationQueue.Instance.Enqueue(validation);

                /*
                int catId;
                bool ProductIsValid = Record.EAN.Length > 0 && Record.Title.Length > 0 && Record.Brand.Length > 0;
                TimeStatisticsMapper.Instance.StartTimeMeasure("Category lookup");
                if(Lookup.CategoryLookup.Contains(Record.Category) && ProductIsValid)
                {
                    TimeStatisticsMapper.Instance.StopTimeMeasure("Category lookup");
                    TimeStatisticsMapper.Instance.StartTimeMeasure("SaveNewArticle method");
                    SaveNewArticle(Record, Lookup.CategoryLookup[Record.Category].FirstOrDefault().Id);
                    TimeStatisticsMapper.Instance.StopTimeMeasure("SaveNewArticle method");
                    GeneralStatisticsMapper.Instance.Increment("New article saves from Borderloop categories");
                    return;
                }
                TimeStatisticsMapper.Instance.StopTimeMeasure("Category lookup");
                TimeStatisticsMapper.Instance.StartTimeMeasure("Category synonym lookup");
                if((catId = checkCategorySynonym(Record.Category, Record.Webshop)) != -1 && ProductIsValid)
                {
                    TimeStatisticsMapper.Instance.StopTimeMeasure("Category synonym lookup");
                    TimeStatisticsMapper.Instance.StartTimeMeasure("SaveNewArticle method");
                    SaveNewArticle(Record, catId);
                    TimeStatisticsMapper.Instance.StopTimeMeasure("SaveNewArticle method");
                    GeneralStatisticsMapper.Instance.Increment("New article saves from category synonyms");
                    return;
                }
                TimeStatisticsMapper.Instance.StopTimeMeasure("Category synonym lookup");
                */              
              
            /*
                TimeStatisticsMapper.Instance.StartTimeMeasure("Unique ID check");
                TimeStatisticsMapper.Instance.StopTimeMeasure("Unique ID check");
                GeneralStatisticsMapper.Instance.Increment("Existing products found");
                Console.WriteLine(Record.Webshop + ": existing product for " + Record.Title.Truncate(10));
                TimeStatisticsMapper.Instance.StartTimeMeasure("SKU check");
                TimeStatisticsMapper.Instance.StopTimeMeasure("SKU check");
                GeneralStatisticsMapper.Instance.Increment("Unmatched products");
                GeneralStatisticsMapper.Instance.Increment("SKU matches found");
                TimeStatisticsMapper.Instance.StartTimeMeasure("Category check");
                TimeStatisticsMapper.Instance.StopTimeMeasure("Category check");
                TimeStatisticsMapper.Instance.StopTimeMeasure("EAN check");
                TimeStatisticsMapper.Instance.StartTimeMeasure("EAN check");
                GeneralStatisticsMapper.Instance.Increment("EAN matches founds");
             */
            }
        }               

        private int CheckIfProductExists(Product record)
        {
            return db.CheckIfProductExists(record);
        }  

        /// <summary>
        /// This method is used to check if the given SKU exists in the database. If so, it will return
        /// the article number of the found product. It will return -1 otherwise.
        /// </summary>
        /// <param name="sku">The SKU that has to be checked.</param>
        /// <returns>The article number if found, -1 otherwise.</returns>
        private int checkSKU(string sku)
        {
            //Return the article number, or -1 otherwise
            return db.GetArticleNumber("sku", sku);
        }

        /// <summary>
        /// This method is used to check if the given EAN exists in the database. If so, it will return
        /// the article number of the found product. It will return -1 otherwise.
        /// </summary>
        /// <param name="ean">The EAN that has to be checked.</param>
        /// <returns>The article number if found, -1 otherwise.</returns>
        private int checkEAN(string ean)
        {
            //Return the article number, or -1 otherwise.
            return db.GetArticleNumber("ean", ean);
        }

        /// <summary>
        /// This method is used to check if the given title exists in the database. If so, it will return
        /// the article number of the found product. It will return -1 otherwise.
        /// </summary>
        /// <param name="title">The title that has to be checked.</param>
        /// <returns>The article number if found, -1 otherwise.</returns>
        private int checkTitle(string title)
        {
            //Return the article number, or -1 otherwise.
            return db.GetArticleNumber("title", title);
        }

        /// <summary>
        /// This method return the category id if this exist in cat_article. 
        /// </summary>
        /// <param name="category">Webshop</param>
        /// <returns></returns>
        private int GetCategoryId(int articleID)
        {
            //Return the article number, or -1 otherwise.
            return db.GetCategoryNumber(articleID);
        }

        /// <summary>
        /// This method checks if category exist in category_synonym or cat-article.
        /// </summary>
        /// <param name="p">Webshop</param>
        /// <param name="webshop">Webshop</param>
        /// <returns></returns>
        private bool checkCategorySynonym(string description, string webshop)
        {
            return db.CheckCategorySynonym(description, webshop);
        } 





        /*

        /// <summary>
        /// This method will match the product by relevance and then show it in the GUI
        /// </summary>
        /// <param name="Record">The record to be matched</param>
        private void MatchByRelevance(Product Record)
        {
            //                          IMPORTANT!!!!!!!!!!
            //FIX THIS METHOD!!!! VBOBDATA NEEDS TO BE REMOVED IF NEW ARTICLE IS SAVED
            int lastInserted = BobBox.Instance.SendToVBobData(Record);
            bool match = BobBox.Instance.GetRelevantMatches(Record, lastInserted);

            // If no matches are found for vbob, save the record as a new article.
            if (match == false)
            {
                //SaveNewArticle(Record);
                //                  DELETE VBOB DATA HERE !!!!!!!!!!!!
            }
        }


        private void CompareProductData(Product Record)
        {
            //First select the product data from the product table for the given record
            //product productData = BobBox.Instance.GetProductData(Record, _matchedArticleID);

            // If there are no rows returned, there is not yet any deliveryinfo for this record. Save it.
            //if (productData == default(product))
            {
                Console.WriteLine("No product found for " + Record.Title.Truncate(10) + "... for webshop " + Record.Webshop);
                //SaveProductData(Record, _matchedArticleID);
            }
            //else// Else there already is product data for this record: Compare the product data with the record data for each column.
            {
                TimeStatisticsMapper.Instance.StartTimeMeasure("UpdateProductData method");
                //BobBox.Instance.UpdateProductData(productData, Record);
                TimeStatisticsMapper.Instance.StopTimeMeasure("UpdateProductData method");
            }
        }
         */
    }
}
