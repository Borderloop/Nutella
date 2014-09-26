using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Reflection;

namespace BobAndFriends
{
    public class BOB
    {
        /// <summary>
        /// Contains all the categories from the Borderloop category tree.
        /// </summary>
        private DataTable Categories;

        /// <summary>
        /// Contains all the category synonyms for the Borderloop category tree.
        /// </summary>
        private DataTable CategorySynonyms;

        /// <summary>
        /// Contains all data of the products which have been fixed and are to be rerunned again.
        /// </summary>
        private DataTable Rerunnables;

        /// <summary>
        /// If a match if found, store the ID in this field.
        /// </summary>
        private int _matchedArticleID = 1;

        /// <summary>
        /// If a category match is found, store its id in this field.
        /// </summary>
        private int _categoryID;

        /// <summary>
        /// A flag to keep track of when we're rerunning through the residue.
        /// </summary>
        private bool rerunningResidue = false;

        private int count;

        public BOB()
        {
            //Initialize BOB
            Initialize();

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
            if (Record.Title.Contains(Record.SKU) && Record.SKU != "")
            {
                //Split the title with the SKU, leaving at least two strings
                //These splitted strings can be empty; therefore, remove them using StringSplitOptions.RemoveEmptyEntries.
                string[] s = Record.Title.Split(new string[] { Record.SKU }, StringSplitOptions.RemoveEmptyEntries);
                Record.Title = String.Concat(s);
            }

            if (Record.Title.Contains(Record.Brand) && Record.Brand != "")
            {
                //Split the title with the Brand, leaving at least two strings
                //These splitted strings can be empty; therefore, remove them using StringSplitOptions.RemoveEmptyEntries.
                string[] s = Record.Title.Split(new string[] { Record.Brand }, StringSplitOptions.RemoveEmptyEntries);
                Record.Title = String.Concat(s);
            }
            */

            if (false)//Database.Instance.HasArticles())
            {
                //First test - EAN/SKU match and perfect title matching.

                //If the first check does not go well, check for the ean.
                if (!Record.EAN.Equals(""))
                {
                    //Check for a partial SKU match
                    //if ((_matchedArticleID = checkPartialSKU(Record.SKU)) != -1)
                    //{
                    //We have an EAN and a partial SKU, enough for the database
                    //SaveMatch(Record);

                    //If checkSKU() return true, the record matches with a product in the database and its data
                    //can be added to the product. It is done then.
                    if ((Record.SKU.Length >= 3) && (_matchedArticleID = checkSKU(Record.SKU)) != -1)
                    {
                        //The product has an SKU and it's a match.
                        SaveMatch(Record);
                    }

                    //If the first check does not go well, check for the ean.
                    if (!Record.EAN.Equals("") && (_matchedArticleID = checkEAN(Record.EAN)) != -1)
                    {
                        //Check for a partial SKU match
                        //if ((_matchedArticleID = checkPartialSKU(Record.SKU)) != -1)
                        //{
                        //We have an EAN and a partial SKU, enough for the database
                        SaveMatch(Record);
                        //}
                    }

                    //The product has no valid EAN and no valid SKU, therefore we will match titles.
                    if ((_matchedArticleID = checkTitle(Record.Title)) != -1)
                    {
                        //We found a perfect title match. Awesome!
                        SaveMatch(Record);
                    }

                    //Product did not pass the first few tests - category test is up next.

                    // If checkCategory() returns false, the record category doesn't match any of the categories 
                    // from the Borderloop category tree. Send record to residue and stop execution of method.
                    // If checkCategory() returns true, the record category matches with one of the 
                    // Borderloop category tree. Continue with the brand check. 
                    //if (!CheckCategory(Record))
                    //{
                    //    sendToResidue(Record);
                    //    return;
                    //}

                }

            }

            // If checkBrand() returns false, the record doesn't contain a brand. Send record
            // to residue and stop execution of method.
            if (CheckBrand(Record))
            {
                sendToResidue(Record);
                return;
            }

            //Run a brand check. If it exists, we can go on to match the product by relevance.
            //If it doesn't. however, we have to create a new product.
            //if (CheckBrandInDatabase(Record))
            //{
            //    MatchByRelevance(Record);
            //}
            if (Record.Title != "" && Record.EAN != "")
            {
                //The product has a brand name which doesnt exist in the and has a title, so save it to the database
                SaveNewArticle(Record);
            }

            count++;

            if (count % 1000 == 0)
            {
                Console.WriteLine("\n\t\t\t\t\tProcessed products: " + count);
            }
        }

        /// <summary>
        /// Also initializes dummy data. For test purposes only
        /// </summary>
        private void Initialize()
        {
            // Open the connection with the database
            OpenDatabaseConnection();

            // Load all categories and category synonyms from database
            Categories = Database.Instance.GetCategories();
            CategorySynonyms = Database.Instance.GetCategorySynonyms();

            count = 0;

        }

        /// <summary>
        /// Method to open the connection with the database.
        /// </summary>
        private void OpenDatabaseConnection()
        {
            Console.WriteLine("\n\t\t\t\t\tOpening connection with database...\n");

            try
            {
                //Open the database connection. The program should stop if this fails.
                Database.Instance.Connect(Statics.settings["dbsource"], Statics.settings["dbname"], Statics.settings["dbuid"], Statics.settings["dbpw"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\t\t\t\t\tConnection failed.\n");
                Console.WriteLine("\t\t\t\t\tError: " + e.Message);
                Console.WriteLine("\t\t\t\t\tFrom: " + e.Source);
                Console.WriteLine("\t\t\t\t\tPress enter to exit.\n");
                Console.Read();
                Environment.Exit(0);
            }

            Console.WriteLine("\n\t\t\t\t\tConnection opened.\n");
        }

        public void ProcessRerunnables()
        {
            Rerunnables = Database.Instance.GetRerunnables();
            Product p = new Product();
            foreach (DataRow row in Rerunnables.Rows)
            {
                p.Title = row.Field<String>("title") ?? "";
                p.EAN = row.Field<String>("ean") ?? "";
                p.SKU = row.Field<String>("sku") ?? "";
                p.Brand = row.Field<String>("brand") ?? "";
                p.Category = row.Field<String>("category") ?? "";
                p.Description = row.Field<String>("description") ?? "";
                p.Image_Loc = row.Field<String>("image_loc") ?? "";

                Process(p);
            }
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

            //Get the whole residue
            DataTable residue = Database.Instance.GetAllProductsFromResidue();

            //Create a product
            Product p = new Product();

            //Loop through each row
            foreach (DataRow row in residue.Rows)
            {
                p.Title = row.Field<String>("title") ?? "";
                p.EAN = row.Field<String>("ean") ?? "";
                p.SKU = row.Field<String>("sku") ?? "";
                p.Brand = row.Field<String>("brand") ?? "";
                p.Category = row.Field<String>("category") ?? "";
                p.Description = row.Field<String>("description") ?? "";
                p.Image_Loc = row.Field<String>("image") ?? "";

                Process(p);
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
        /// Method to check if a record contains a category. If it does, this method
        /// also checks if the record category matches a category in the Borderloop category tree.
        /// </summary>
        private Boolean CheckCategory(Product Record)
        {
            // If the Product's Category attribute is an empty String, no category was given.
            if (Record.Category.Equals(""))
            {
                return false;
            }

            // Check if the record category matches with a category or category synonym from the Borderloop category tree.
            bool categoryMatch = Categories.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());
            bool categorySynonymMatch = CategorySynonyms.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());

            // If categoryMatch or categorySynonymMatch equals true, a match is found. Get the category id, used
            // later on for saving the article.
            if (categoryMatch || categorySynonymMatch)
            {
                _categoryID = Database.Instance.GetCategoryID(Record.Category, categoryMatch, categorySynonymMatch);
            }
            return (categoryMatch || categorySynonymMatch);
        }

        /// <summary>
        /// This method is called when a match is found. It saves the found match to the database.
        /// It adds missing data to the found article and adds synonyms.
        /// </summary>
        private void  SaveMatch(Product Record)
        {
            //First get all data needed for matching. Ean, sku and title_synonym are seperate because they can store multiple values.
            DataTable articleTable = Database.Instance.GetArticleTableForArticle(_matchedArticleID);
            DataTable eanTable = Database.Instance.GetEANTableForArticle(_matchedArticleID);
            DataTable skuTable = Database.Instance.GetSKUTableForArticle(_matchedArticleID);
            DataTable titleSynonymTable = Database.Instance.GetTitleSynonymTableForArticle(_matchedArticleID);

            // Put these DataTables in an array for looping.
            DataTable[] dtArray = new DataTable[3];
            dtArray[0] = articleTable;
            dtArray[1] = eanTable;
            dtArray[2] = skuTable;

            //Loop over each DataTable and its columns
            foreach (DataTable dt in dtArray)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    Object o;
                    try
                    {
                        o = dt.Rows[0][column];
                    }
                    // No records in given table for this article. This always is the ean or sku table. Insert right away and break.
                    catch (IndexOutOfRangeException)  
                    {
                        UpdateEmptyColumn(column, Record);
                        break;
                    }
                    if (dt.Rows.OfType<DataRow>().Any(r => r.IsNull(column) || o.ToString().Equals(""))) // If matched article has no value for this column...
                    {
                        UpdateEmptyColumn(column, Record); //Update the empty column
                    }

                    // Else, the column has a value so check if the article value differs from the record value and if so, add this to the article.
                    // This is only for the ean and sku tables, not for the article table. This table can't and shouldn't contain double data.
                    else if(dt != dtArray[0])
                    {
                        AddDoubleColumnData(dt, column, Record);
                    }
                }
            }

            // Next up is the title_synonym table. Since it has two tables and needs extra processing, this is done seperately from the other tables.
            CheckTitles(titleSynonymTable, Record);

            // Finally compare the product data
            CompareProductData(Record);

            //We've found a match while we were running through the residue, therefore we need to delete the record from there.
            //Doing that at the end of the method makes sure the data will remain stored in the residue in case
            //something in this method goes wrong.
            if (rerunningResidue)
            {
                Database.Instance.DeleteFromResidue(Record);
            }

        }

        /// <summary>
        /// This method updates an empty column for an article, or inserts an entry if
        /// the title or sku table is empty
        /// </summary>
        /// <param name="column">The column to be updated</param>
        /// <param name="Record">The record that contains the to be inserted data</param>
        private void UpdateEmptyColumn(DataColumn column, Product Record)
        {
            String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
            String recordValue = GetPropValue(Record, splitted[1]).ToString();

            //If the record value is empty, an update or insert won't be neccesary at all.
            if (recordValue != "")
            {
                // If the TABLE name doesn't equal 'article', it's either the ean, sku or titles table. Also meaning that 
                // there is no record in this table at all for the matched article. Because of this, an update won't work:
                // Insert instead.
                if (splitted[0] != "article")
                {
                    Database.Instance.AddForMatch(splitted[0], recordValue, _matchedArticleID);
                }
                else
                {
                    Database.Instance.Update(splitted[0], splitted[1], recordValue, _matchedArticleID);
                }
            }
        }

        /// <summary>
        /// Adds an ean or sku to an article if it is not yet present in the database. 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column"></param>
        /// <param name="Record"></param>
        private void AddDoubleColumnData(DataTable dt, DataColumn column, Product Record)
        {
            String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
            String recordValue = GetPropValue(Record, splitted[1]).ToString();

            bool hasMatch;

            try
            {
                hasMatch = dt.AsEnumerable().Any(row => recordValue.ToLower() == row.Field<String>(column).ToLower());
            }
            catch (InvalidCastException) // An ean is returned, which is int64 instead of string. Convert values for this.
            {
                //This should not be thrown anymore since we changed EAN to be a string.
                hasMatch = dt.AsEnumerable().Any(row => Convert.ToInt64(recordValue) == row.Field<Int64>(column));
            }

            // If hasMatch is false, a different value is found. Insert this into the database, but only if the record value is not empty.
            if (hasMatch == false && recordValue != null && recordValue != "")
            {
                Database.Instance.AddForMatch(splitted[0], recordValue, _matchedArticleID);
            }
        }

        /// <summary>
        /// This method checks if there are article titles similar to the record titles. If so, the match occurrence is
        /// updated. The occurrence is used to check which title to use for the main title, which is also done in this method.
        /// If there's no match, add a new match.
        /// </summary>
        /// <param name="titleSynonymTable"></param>
        /// <param name="Record"></param>
        private void CheckTitles(DataTable titleSynonymTable, Product Record)
        {
            DataRow[] foundRow = titleSynonymTable.Select("Title = '" + Record.Title + "'");

            // If the returned rows are bigger then 0, they're always one and found a match. We need to update the occurance column
            if (foundRow.Count() > 0)
            {
                // Always one row, which is the match, get the title, occurences and title_id for it.
                string matchedTitle = foundRow[0]["title"].ToString();
                int matchedOccurrences = Convert.ToInt32(foundRow[0]["occurrences"]);
                int matchedTitleId = Convert.ToInt16(foundRow[0]["title_id"]);

                // Add one for the occurance and update this in the database
                matchedOccurrences++;
                Database.Instance.UpdateTitleSynonymOccurrences(matchedTitleId, matchedOccurrences, matchedTitle);

                // Now it needs to check if the updated title has a higher occurrences value than the one in the title table.
                // If this is the case, the title in the title table must be replaced with this title.
                int titleOccurrences = Database.Instance.GetOccurrencesForTitle(matchedTitleId);
                if (matchedOccurrences > titleOccurrences)
                {
                    Database.Instance.UpdateTitle(matchedTitleId, matchedTitle);
                }


            }
            else // Else no match is found, so insert a new record 
            {
                // First get the title id, then insert a new record with that title id;
                int titleId = Database.Instance.GetTitleId(_matchedArticleID, 1);
                Database.Instance.InsertNewTitle(titleId, Record.Title);
            }
        }

        /// <summary>
        /// Gets the value of a Record for a given property
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private static object GetPropValue(Product record, string propName)
        {
            return record.GetType().GetProperty(propName).GetValue(record, null);
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
            return Database.Instance.GetArticleNumberOfSKU(sku);
        }

        /// <summary>
        /// This method is used to check if the given SKU partially exists in the database. If so, it 
        /// will return the article number of the found product. It will return -1 otherwise.
        /// </summary>
        /// <param name="sku">The partial SKU that has to be checked.</param>
        /// <returns>The article number if found, -1 otherwise.</returns>
        private int checkPartialSKU(string sku)
        {
            //Return the article number, or -1 otherwise.
            return Database.Instance.GetArticleNumberOfPartialSKU(sku);
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
            return Database.Instance.GetArticleNumberOfEAN(ean);
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
            return Database.Instance.GetArticleNumberOfTitle(title);
        }

        /// <summary>
        /// This method will send a product to the residue
        /// </summary>
        /// <param name="p"></param>
        private void sendToResidue(Product p)
        {
            //If we are rerunning through the residue, this product already exists there.
            //No need to add it again.
            if (rerunningResidue)
            {
                return;
            }

            //Call SendToResidue() to do so.
            Database.Instance.SendTo(p, "residue");
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
            Database.Instance.SendTo(Record, "vbobdata");
            bool match = Database.Instance.GetRelevantMatches(Record);

            // If no matches are found for vbob, save the record as a new article.
            if (match == false)
            {
                SaveNewArticle(Record);
            }
        }

        /// <summary>
        /// This method wil create a new product and put it in the database.
        /// </summary>
        /// <param name="Record">The product to be put in the database</param>
        private void SaveNewArticle(Product Record)
        {
            // Invoke SaveNewArticle() from the database object.
            Database.Instance.SaveNewArticle(Record, _categoryID);

            // Save product data to database.
            //SaveProductData(Record);
        }

        /// <summary>
        /// This method saves the product data to the database when there's not any product data for
        /// this record.
        /// </summary>
        /// <param name="Record">The product with its product data to be saves</param>
        private void SaveProductData(Product Record)
        {
            Database.Instance.SaveProductData(Record, _matchedArticleID);
        }

        private void CompareProductData(Product Record)
        {
            //First select the product data from the product table for the given record
            DataTable productData = Database.Instance.GetProductData(Record, _matchedArticleID);

            // If there are no rows returned, there is not yet any deliveryinfo for this record. Save it.
            if (productData.Rows.Count == 0)
            {
                SaveProductData(Record);
            }
            else// Else there already is product data for this record: Compare the product data with the record data for each column.
            {
                string query = "UPDATE product SET";
                string columnName = "";
                foreach (DataColumn column in productData.Columns)
                {
                    String recordValue = GetPropValue(Record, column.ToString()).ToString();
                    Object o = productData.Rows[0][column].ToString();

                    // If the record value and article value don't match for this column, the data has changed.
                    if (recordValue != o.ToString())
                    {
                        // First rename aliasses to the right column names.
                        if (column.ToString().Equals("DeliveryTime"))
                        {
                            columnName = "ship_time";
                        }
                        else if (column.ToString().Equals("DeliveryCost"))
                        {
                            columnName = "ship_cost";
                        }
                        else if (column.ToString().Equals("Url"))
                        {
                            columnName = "direct_link";
                        }
                        else
                        {
                            columnName = column.ToString();
                        }
                        query += " " + columnName + " = '" + recordValue + "',";
                    }
                }

                // If the query is not its default value, different data is found but there is a comma on the last character spot. 
                // Delete the last comma, add WHERE clause and call update method for Database.
                if (!query.Equals("UPDATE product SET"))
                {
                    query = query.Remove(query.Length - 1, 1);
                    query += " WHERE article_id = " + _matchedArticleID + " AND webshop_url = '" + Record.Webshop + "'";

                    Database.Instance.UpdateProductData(query);
                }
            }
        }

        /// <summary>
        /// BOB will clean up everything and close the app here.
        /// </summary>
        public void FinalizeAndClose()
        {
            Console.WriteLine("\n\t\t\t\t\tClosing connection with database...");

            //Close the database.
            Database.Instance.Close();

            //Close everything and shut down.
            Console.WriteLine("\t\t\t\t\tWriting data to logfile...");
            Statics.Logger.Close();
            Console.WriteLine("\t\t\t\t\tDone.");
            Environment.Exit(1);
        }

    }
}
