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
        /// Contains all data of the products which have been fixed and are to be rerunned again.
        /// </summary>
        private DataTable Rerunnables;

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
            */
            Console.WriteLine("Busy with: " + Record.Title);
            if (Record.Title.Contains(Record.Brand) && Record.Brand != "")
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
            //Get country id
            _countryID = GetCountryId(Record.Webshop);
            Console.WriteLine("Finished getting country id in: {0}", sw.Elapsed);

            //First test - EAN/SKU match and perfect title matching.
            sw.Restart();
            //If checkSKU() return true, the record matches with a product in the database and its data
            //can be added to the product. It is done then.
            if ((Record.SKU.Length >= 3) && (_matchedArticleID = checkSKU(Record.SKU)) != -1)
            {
                //The product has an SKU and it's a match.
                SaveMatch(Record);
                return;
            }
            Console.WriteLine("Finished unsuccesfull sku check: {0}", sw.Elapsed);
            sw.Restart();
            //If the first check does not go well, check for the ean.
            if (!Record.EAN.Equals("") && (_matchedArticleID = checkEAN(Record.EAN)) != -1)
            {
                SaveMatch(Record);
                return;
            }
            Console.WriteLine("Finished unsuccesfull ean check: {0}", sw.Elapsed);
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
            Console.WriteLine("Finished brand check: {0}", sw.Elapsed);
            sw.Restart();
            //Run a brand check. If it exists, we can go on to match the product by relevance.
            //If it doesn't. however, we have to create a new product.
            if (CheckBrandInDatabase(Record))
            {
                Console.WriteLine("Brand is in database");
                //MatchByRelevance(Record);
                SaveNewArticle(Record);
                return;
            }
            Console.WriteLine("Finished checking if brand is in database in: {0}", sw.Elapsed);
            if (Record.Title != "" && Record.EAN != null)
            {
                //The product has a brand name which doesnt exist in the and has a title, so save it to the database
                SaveNewArticle(Record);
                return;
            }

            else // Log the website that was not present
            {
                Console.WriteLine("Webshop not found in database: " + Record.Webshop);
            }
        }
        

        /// <summary>
        /// Also initializes dummy data. For test purposes only
        /// </summary>
        private void Initialize()
        {
            // Open the connection with the database
            OpenDatabaseConnection();
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
            List<string> columns = new List<string>();
            columns.Add("*");

            Rerunnables = Database.Instance.Select(columns, "vbobdata", "rerun", "1");
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
            List<string> columns = new List<string>();
            columns.Add("*");
            DataTable residue = Database.Instance.Select(columns, "residue");

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
        /// This method is called when a match is found. It saves the found match to the database.
        /// It adds missing data to the found article and adds synonyms.
        /// </summary>
        private void  SaveMatch(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> columns = new List<string>();
            // Add columns for article table, afterwards clear the columns and do the same for ean, sku and title.
            columns.Add("brand AS 'article-Brand'");
            columns.Add("description AS 'article-Description'");
            columns.Add("image_loc AS 'article-Image_Loc'");

            //First get all data needed for matching. Ean, sku and title_synonym are seperate because they can store multiple values.
            DataTable articleTable = Database.Instance.GetTableForArticle(_matchedArticleID, columns, "article", "id");

            columns.Clear();
            columns.Add("ean AS 'ean-EAN'");
            DataTable eanTable = Database.Instance.GetTableForArticle(_matchedArticleID, columns, "ean", "article_id");

            columns.Clear();
            columns.Add("sku AS 'sku-SKU'");
            DataTable skuTable = Database.Instance.GetTableForArticle(_matchedArticleID, columns, "sku", "article_id");
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

            Statics.Logger.WriteLine("Finished saving match in: {0}", sw.Elapsed);
            sw.Stop();
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
                    Database.Instance.AddForMatch(splitted[0], recordValue, _matchedArticleID, _countryID);
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
                Database.Instance.AddForMatch(splitted[0], recordValue, _matchedArticleID, _countryID);
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
                int matchedTitleId = Convert.ToInt32(foundRow[0]["title_id"]);

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
                int titleId = Database.Instance.GetTitleId(_matchedArticleID, _countryID);
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
            Database.Instance.SendTo(p, "residue");
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
            Database.Instance.SendTo(Record, "vbobdata");
            bool match = Database.Instance.GetRelevantMatches(Record);

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
        private void SaveProductData(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Database.Instance.SaveProductData(Record);
            Console.WriteLine("Finished saving product data in: {0}", sw.Elapsed);
        }

        private void CompareProductData(Product Record)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //First select the product data from the product table for the given record
            DataTable productData = Database.Instance.GetProductData(Record, _matchedArticleID);

            // If there are no rows returned, there is not yet any deliveryinfo for this record. Save it.
            if (productData.Rows.Count == 0)
            {
                Console.WriteLine("Finished comparing, no similar data found. Time: {0}", sw.Elapsed);
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

                    string dbValue = o.ToString().Replace(',', '.');

                    // If the record value and article value don't match for this column, the data has changed.
                    if (recordValue != dbValue)
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
                    Console.WriteLine("Finished comparing, different values found in: {0}", sw.Elapsed);
                    Console.WriteLine(query);

                    Database.Instance.UpdateProductData(query);
                }
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
