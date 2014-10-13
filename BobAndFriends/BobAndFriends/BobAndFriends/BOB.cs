using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Globalization;

namespace BobAndFriends
{
    public class BOB
    {
        /// <summary>
        /// If a match if found, store the ID in this field.
        /// </summary>
        private int _matchedArticleID;

        /// <summary>
        /// A flag to keep track of when we're rerunning through the residue.
        /// </summary>
        private bool rerunningResidue = false;

        private int _countryID;

        public BOB()
        {

            //First process the products which are flagged for a rerun by the GUI.
            //These products contain the best data since they are manually configured.
            //ProcessRerunnables();
        }

        /// <summary>
        /// This is the main method. This is where the algorithm find its chronologically order and
        /// all methods are called in this order. 
        /// </summary>
        public void Process(Product Record = null)
        {
            /*
            //Precondition: Record has to be clean; the title should not include
            //the brand name or the sku value. Therefore, we filter them out.
            if (Record.Title.Contains(Record.SKU, StringComparison.OrdinalIgnoreCase) && Record.SKU != "")
            {
                //Split the title with the SKU, leaving at least two strings
                //These splitted strings can be empty; therefore, remove them using StringSplitOptions.RemoveEmptyEntries.
                string[] s = Record.Title.Split(new string[] { Record.SKU }, StringSplitOptions.RemoveEmptyEntries);
                Record.Title = String.Concat(s);
            }
            */
            Console.WriteLine("Busy with: " + Record.Title);

            if (Record.Title.Contains(Record.Brand, StringComparison.OrdinalIgnoreCase) && Record.Brand != "")
            {
                //Split the title with the Brand, leaving at least two strings
                //These splitted strings can be empty; therefore, remove them using StringSplitOptions.RemoveEmptyEntries.
                string[] s = Record.Title.Split(new string[] { Record.Brand }, StringSplitOptions.RemoveEmptyEntries);
                Record.Title = String.Concat(s);
            }
            Stopwatch sw = new Stopwatch();

            if (Record.SKU.Length > 15)
            {
                Record.SKU = "";
            }
            sw.Restart();

            //Check if product already exists in database by affiliate and unique affiliate number.
            if ((_matchedArticleID = GetAIDFromUAC(Record)) != -1)
            {
                //There is a match with a unique article, meaning the record is one. Just update it where necessary.
                CompareProductData(Record);
                return;
            }
            //Console.WriteLine("Finished unsuccesful id in: {0}", sw.Elapsed);
            sw.Restart();

            //Get country id
            _countryID = GetCountryId(Record.Webshop);
            //Console.WriteLine("Finished getting country id in: {0}", sw.Elapsed);

            //First test - EAN/SKU match and perfect title matching.
            sw.Restart();
            //If checkSKU() return true, the record matches with a product in the database and its data
            //can be added to the product. It is done then.
            if ((Record.SKU.Length >= 3) && (_matchedArticleID = checkSKU(Record.SKU)) != -1)
            {
                int catid;
                // After SKU match, check if given category exist in database.
                if ((catid = GetCategoryId(_matchedArticleID)) != -1)
                {
                    int catSynId;
                    // if category id not exist add category to category_synonym.
                    if ((catSynId = checkCategorySynonym(Record.Category, Record.Webshop)) == -1)
                    {
                        Database.Instance.InsertIntoCatSynonyms(catid, Record.Category, Record.Webshop);
                    }
                }
                else
                {
                    // After SKU match, check if given category exists in database. If exist insert category id and article id in cat-article table.
                    if ((catid = checkCategorySynonym(Record.Category, Record.Webshop)) != -1)
                    {
                        Database.Instance.InsertNewCatArtile(catid, _matchedArticleID);
                    }
                    else
                    {
                        sendToResidue(Record);
                        return;
                    }
                }
                //The product has an SKU and it's a match.
                SaveMatch(Record);
                return;
            }
            //Console.WriteLine("Finished unsuccesfull sku check: {0}", sw.Elapsed);
            sw.Restart();

            //If the first check does not go well, check for the ean.
            if (!Record.EAN.Equals("") && (_matchedArticleID = checkEAN(Record.EAN)) != -1)
            {
                int catid;
                // After EAN match, check if given category exist in database.
                if ((catid = GetCategoryId(_matchedArticleID)) != -1)
                {
                    int catSynId;
                    // if category id not exist add category to category_synonym.
                    if ((catSynId = checkCategorySynonym(Record.Category, Record.Webshop)) == -1)
                    {
                        Database.Instance.InsertIntoCatSynonyms(catid, Record.Category, Record.Webshop);
                    }
                }
                else
                {
                    // After EAN matched, check if given category exists in database. If exist insert category id and article id in cat-article table.
                    if ((catid = checkCategorySynonym(Record.Category, Record.Webshop)) != -1)
                    {
                        Database.Instance.InsertNewCatArtile(catid, _matchedArticleID);
                    }
                    else
                    {
                        sendToResidue(Record);
                        return;
                    }
                }
                //The product has an EAN and it's a match.
                SaveMatch(Record);
                return;
            }
            //Console.WriteLine("Finished unsuccesfull ean check: {0}", sw.Elapsed);
            sw.Restart();
            /*//The product has no valid EAN and no valid SKU, therefore we will match titles.
            if ((_matchedArticleID = checkTitle(Record.Title)) != -1)
            {
                //We found a perfect title match. Awesome!
                SaveMatch(Record);
                return;
            }
            Console.WriteLine("Finished unsuccesfull title check in: {0}", sw.Elapsed);
            sw.Restart();*/

            // If checkBrand() returns false, the record doesn't contain a brand. Send record
            // to residue and stop execution of method.
            if (CheckBrand(Record))
            {
                sendToResidue(Record);
                return;
            }
            //Console.WriteLine("Finished brand check: {0}", sw.Elapsed);
            sw.Restart();

            //Run a brand check. If it exists, we can go on to match the product by relevance.
            //If it doesn't. however, we have to create a new product.
            /*if (CheckBrandInDatabase(Record))
            {
                Console.WriteLine("Brand is in database");
                //MatchByRelevance(Record);
                SaveNewArticle(Record);
                return;
            }*/
            //Console.WriteLine("Finished checking if brand is in database in: {0}", sw.Elapsed);
            if (Record.Title != "" && Record.EAN != "")
            {
                //The product has a brand name which doesn't exist in the database and has a title, so save it to the database
                SaveNewArticle(Record);
                return;
            }
            else//           !!!!!!!!!!!!   TEMPORARY, REMOVE WHEN RELEVANCE MATCHER IS TURNED BACK ON    !!!!!!!!!!!
            {
                sendToResidue(Record);
            }
        }       

            
        public void ProcessRerunnables()
        {
            using (var db = new BetsyModel(Database.Instance.GetConnectionString()))
            {
                IEnumerable<vbobdata> Rerunnables = db.vbobdata.Where(v => v.rerun == true).ToList();
                Product p = new Product();
                foreach (vbobdata data in Rerunnables)
                {
                    p.Title = data.title;
                    p.EAN = data.ean;
                    p.SKU = data.sku;
                    p.Brand = data.brand;
                    p.Category = data.category;
                    p.Description = data.description;
                    p.Image_Loc = data.image_loc;

                    Process(p);
                }
            }          
        }

        private int GetAIDFromUAC(Product record)
        {
            return Database.Instance.GetAIDFromUAC(record);
        }  

        /// <summary>
        /// This method will rerun all the products in the residue through BOB.
        /// This process will be terminated if no products are added during the walkthrough,
        /// meaning it will stop when countBefore == countAfter.
        /// </summary>
        public void RerunResidue()
        {
            Console.WriteLine("Started rerunning the residue.");

            //Set flag to true
            rerunningResidue = true;

            //Count rows before starting
            int rowsBefore = Database.Instance.CountRows("residue");

            using (var db = new BetsyModel(Database.Instance.GetConnectionString()))
            {
                 ;
                Product p = new Product();
                foreach (residue data in db.residue.ToList())
                {
                    p.Title = data.title;
                    p.EAN = data.ean;
                    p.SKU = data.sku;
                    p.Brand = data.brand;
                    p.Category = data.category;
                    p.Description = data.description;
                    p.Image_Loc = data.image;

                    Process(p);
                }
            }
            //count rows afterwards
            int rowsAfter = Database.Instance.CountRows("residue");

            //Check if amounts match. If not, run again.
            if (rowsAfter != rowsBefore)
            {
                Console.WriteLine("Going through again.");
                RerunResidue();
            }

            Console.WriteLine("Done.");
            //Done. Set flag to false.
            rerunningResidue = false;
        }

        /// <summary>
        /// Method to check if a record contains a brand.
        /// </summary>
        private Boolean CheckBrand(Product Record)
        {
            // If the Product's Brand Property is an empty String, no brand was given.
            return Record.Brand.Equals("");
        }

        /// <summary>
        /// This method is called when a match is found. It saves the found match to the database.
        /// It adds missing data to the found article and adds synonyms.
        /// </summary>
        private void  SaveMatch(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Save the match
            Database.Instance.SaveMatch(Record, _matchedArticleID, _countryID);

            // Finally compare the product data
            CompareProductData(Record);

            //We've found a match while we were running through the residue, therefore we need to delete the record from there.
            //Doing that at the end of the method makes sure the data will remain stored in the residue in case
            //something in this method goes wrong.
            if (rerunningResidue)
            {
                Database.Instance.DeleteFromResidue(Record);
            }

            Console.WriteLine("Finished saving match in: {0}", sw.Elapsed);
            sw.Stop();
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
            return Database.Instance.GetArticleNumber("sku", "sku", sku);
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
            return Database.Instance.GetArticleNumber("ean", "ean", ean.ToString());
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
            return Database.Instance.GetArticleNumber("title", "title", title);
        }

        /// <summary>
        /// This method return the category id if this exist in cat_article. 
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns></returns>
        private int GetCategoryId(int articleID)
        {
            //Return the article number, or -1 otherwise.
            return Database.Instance.GetCategoryNumber("cat_article", "article_id", articleID);
        }

        /// <summary>
        /// This method checks if category exist in category_synonym or cat-article.
        /// </summary>
        /// <param name="p">Category</param>
        /// <param name="webshop">Webshop</param>
        /// <returns></returns>
        private int checkCategorySynonym(string p, string webshop)
        {
            return Database.Instance.CheckCategorySynonym("category_synonym", "description", "web_url", p, webshop);
        } 

        /// <summary>
        /// This method will send a product to the residue
        /// </summary>
        /// <param name="p"></param>
        private void sendToResidue(Product p)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //If we are rerunning through the residue, this product already exists there.
            //No need to add it again.
            if (rerunningResidue)
            {
                return;
            }

            //Call SendToResidue() to do so.
            Database.Instance.SendToResidue(p);
            Console.WriteLine("Finished sending to residue in : " + sw.Elapsed);
            sw.Stop();
        }

        /// <summary>
        /// This method will check if the brand of a product exists in the database
        /// </summary>
        /// <param name="Record">The record to be checked</param>
        /// <returns>true if the brand exists, false otherwise</returns>
        private bool CheckBrandInDatabase(Product Record)
        {
            //Invoke CheckIfBrandExists() from the database object
            return Database.Instance.CheckIfBrandExists(Record.Brand);
        }

        /// <summary>
        /// This method will match the product by relevance and then show it in the GUI
        /// </summary>
        /// <param name="Record">The record to be matched</param>
        private void MatchByRelevance(Product Record)
        {
            //                          IMPORTANT!!!!!!!!!!
            //FIX THIS METHOD!!!! VBOBDATA NEEDS TO BE REMOVED IF NEW ARTICLE IS SAVED
            int lastInserted = Database.Instance.SendToVBobData(Record);
            bool match = Database.Instance.GetRelevantMatches(Record, lastInserted);

            // If no matches are found for vbob, save the record as a new article.
            if (match == false)
            {
                SaveNewArticle(Record);
                //                  DELETE VBOB DATA HERE !!!!!!!!!!!!
            }
            else
            {
                sendToResidue(Record);
            }
        }

        /// <summary>
        /// This method wil create a new product and put it in the database.
        /// </summary>
        /// <param name="Record">The product to be put in the database</param>
        private void SaveNewArticle(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Invoke SaveNewArticle() from the database object.
            Database.Instance.SaveNewArticle(Record, _countryID);
            Console.WriteLine("Finished saving new article in: {0}", sw.Elapsed);
        }

        /// <summary>
        /// This method saves the product data to the database when there's not any product data for
        /// this record.
        /// </summary>
        /// <param name="Record">The product with its product data to be saves</param>
        private void SaveProductData(Product Record, int matchedArticleId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Database.Instance.SaveProductData(Record, matchedArticleId);
            Console.WriteLine("Finished saving product data in: {0}", sw.Elapsed);
        }

        private void CompareProductData(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //First select the product data from the product table for the given record
            product productData = Database.Instance.GetProductData(Record, _matchedArticleID);

            // If there are no rows returned, there is not yet any deliveryinfo for this record. Save it.
            if (productData == default(product))
            {
                Console.WriteLine("Finished comparing, no similar data found. Time: {0}", sw.Elapsed);
                SaveProductData(Record, _matchedArticleID);
            }
            else// Else there already is product data for this record: Compare the product data with the record data for each column.
            {
                Database.Instance.UpdateProductData(productData, Record);
                Console.WriteLine("Finished comparing, different values found in: {0}", sw.Elapsed);
            }
            Console.WriteLine("Finished total comparison in: ", sw.Elapsed);
        }

        private int GetCountryId(string webshop)
        {
            return Database.Instance.GetCountryId(webshop);
        }

        /// <summary>
        /// BOB will clean up everything and close the app here.
        /// </summary>
        public void FinalizeAndClose()
        {
            Console.WriteLine("\n\t\t\t\t\tClosing connection with database...");

            //Close everything and shut down.
            Console.WriteLine("\t\t\t\t\tWriting data to logfile...");
            Statics.Logger.Close();
            Console.WriteLine("\t\t\t\t\tDone.");
            Environment.Exit(1);
        }

    }
}
