using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Reflection;

namespace ProductFeedReader
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
        /// If a match if found, store the ID in this field.
        /// </summary>
        private int _matchedArticleID;

        public BOB()
        {
            Initialize();
        }

        /// <summary>
        /// This is the main method. This is where the algorithm find its chronologically order and
        /// all methods are called in this order. 
        /// </summary>
        public void Process(Product Record = null)
        {
            //First test - EAN/SKU match and perfect title matching.

            //If checkSKU() return true, the record matches with a product in the database and its data
            //can be added to the product. It is done then.
            if (!Record.SKU.Equals("") && (_matchedArticleID = checkSKU(Record.SKU)) != -1)
            {
                //The product has an SKU and it's a match.
                SaveMatch(Record, _matchedArticleID);
            }

            //If the first check does not go well, check for the ean.
            if (!Record.EAN.Equals("") && !Record.SKU.Equals("") && (_matchedArticleID = checkEAN(Record.EAN)) != -1)
            {
                //Check for a partial SKU match
                if ((_matchedArticleID = checkPartialSKU(Record.SKU)) != -1)
                {
                    //We have an EAN and a partial SKU, enough for the database
                    SaveMatch(Record, _matchedArticleID);
                }
            }

            //The product has no valid EAN and no valid SKU, therefore we will match titles.
            if ((_matchedArticleID = checkTitle(Record.Title)) != -1)
            {
                //We found a perfect title match. Awesome!
                SaveMatch(Record, _matchedArticleID);
            }

            //Product did not pass the first few tests - category test is up next.

            /*if (match == true)
            {
                saveMatch();
            }*/

            // If checkCategory() returns false, the record category doesn't match any of the categories 
            // from the Borderloop category tree. Send record to residue and stop execution of method.
            if (!CheckCategory(Record))
            {
                Debug.WriteLine("Sending record to residue due to category check fail");
                return;
            }

            // If checkCategory() returns true, the record category matches with one of the 
            // Borderloop category tree. Continue with the brand check.

            Debug.WriteLine("Category check succes, continuing brand check");

            // If checkBrand() returns false, the record doesn't contain a brand. Send record
            // to residue and stop execution of method.
            if (!CheckBrand(Record))
            {
                Debug.WriteLine("Sending record to residue due to missing brand");
                return;
            }

            Console.WriteLine("Closing connection with database...");

            //Close the database.
            Database.Instance.Close();
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

        }

        /// <summary>
        /// Method to open the connection with the database.
        /// </summary>
        private void OpenDatabaseConnection()
        {
            Console.WriteLine("Opening connection with database...");

            try
            {
                //Open the database connection. The program should stop if this fails.
                Database.Instance.Connect(Statics.settings["dbsource"], Statics.settings["dbname"], Statics.settings["dbuid"], Statics.settings["dbpw"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed.");
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("From: " + e.Source);
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                Environment.Exit(0);
            }

            Console.WriteLine("Connection opened.");
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
            bool categoryMatch = Categories.AsEnumerable().Any(row => Record.Category == row.Field<String>("description"));
            bool categorySynonymMatch = CategorySynonyms.AsEnumerable().Any(row => Record.Category == row.Field<String>("description"));

            // If categoryMatch or categorySynonymMatch equals true, a match is found.
            return (categoryMatch || categorySynonymMatch);
        }

        /// <summary>
        /// This method is called when a match is found. It saves the found match to the database.
        /// It adds missing data to the found article and adds synonyms.
        /// </summary>
        private void SaveMatch(Product Record, int _matchedArticleID)
        {
            DataTable MatchedArticle = Database.Instance.GetProduct(_matchedArticleID);

            // First, check if there are null  or empty values present in the matched article.
            // If there are, check if the record has values for the missing data and update these values.
            foreach (DataColumn column in MatchedArticle.Columns)
            {
                Object o = MatchedArticle.Rows[0][column];
                if (MatchedArticle.Rows.OfType<DataRow>().Any(r => r.IsNull(column) || o.ToString() == "")) // If matched article has no value for this column...
                {
                    String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
                    String recordValue = GetPropValue(Record, splitted[1]).ToString();

                    // If the TABLE name doesn't equal 'article', it's either the ean, sku or titles table. Also meaning that 
                    // there is no record in this table at all for the matched article. Because of this, an update won't work:
                    // Insert instead.
                    if (recordValue != "")
                    {
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
                // Else the column has a value, so check if the record value differs from the article value
                // and if so, save it.
                else
                {
                    String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
                    if (splitted[1].ToString() != "id")
                    {
                        String recordValue = GetPropValue(Record, splitted[1]).ToString();
                        String matchedValue = o.ToString();

                        if (recordValue.ToLower() != matchedValue.ToLower() && splitted[0] != "article") // Different value, insert it.
                        {
                            Database.Instance.AddForMatch(splitted[0], recordValue, 1);
                        }

                    }
                }
            }

            // Finally check if the record category is the same as the article category. If it's not, check if
            // there's a match in the category_synonyms table for the matched article. If that's also not the case,
            // add the category from the record to the category_synonyms.
            DataTable category = Database.Instance.GetCategoryForArticle(1);
            bool containsCategory = category.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());

            if (containsCategory == false)
            {
                DataTable categorySynonyms = Database.Instance.GetCategorySynonymsForArticle(1);
                bool containsCategorySynonym = categorySynonyms.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());

                if (containsCategorySynonym == false) // If containsCategorySynonym equals false, a category synonym is found: Save it.
                {
                    Object o = category.Rows[0][0];
                    string id = o.ToString();

                    Database.Instance.SaveCategorySynonym(Convert.ToInt32(id), Record.Category);
                }
            }
        }

        /// <summary>
        /// Gets the value of a 
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
        /// This method is used to check if the given eAN exists in the database. If so, it will return
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

    }
}

