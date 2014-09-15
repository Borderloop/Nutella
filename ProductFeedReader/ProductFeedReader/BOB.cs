using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;

namespace ProductFeedReader
{
    public class BOB
    {
        /// <summary>
        /// The Product contains the data from the Record.
        /// </summary>
        private Product Record;

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

        public void Process(Product p = null)
        {
            //If checkSKU() return true, the record matches with a product in the database and its data
            //can be added to the product. It is done then.
            if ((_matchedArticleID = checkSKU(p.SKU)) != -1)
            {
                //Add missing data-piece comes here.             
            }

            //If the first check does not go well, check for the ean.
            if (!p.SKU.Equals("") && (_matchedArticleID = checkEAN(p.EAN)) != -1)
            {
                //Perform a partial match of the SKU
            }

            //The product has no valid EAN and no valid SKU, therefore we will match titles.
            if ((_matchedArticleID = checkTitle(p.Name)) != -1)
            {
                //We found a perfect title match. Awesome!
                //Add missing data-piece comes here.
            }

            // If checkCategory() returns false, the record category doesn't match any of the categories 
            // from the Borderloop category tree. Send record to residue and stop execution of method.
            if (!checkCategory())
            {
                Debug.WriteLine("Sending record to residue due to category check fail");
                return;
            }

            // If checkCategory() returns true, the record category matches with one of the 
            // Borderloop category tree. Continue with the brand check.

            Debug.WriteLine("Category check succes, continuing brand check");

            // If checkBrand() returns false, the record doesn't contain a brand. Send record
            // to residue and stop execution of method.
            if (!checkBrand())
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
            Record = new Product();

            // Open the connection with the database
            OpenDatabaseConnection();

            // Load all categories and category synonyms from database
            Categories = Database.Instance.GetCategories();
            CategorySynonyms = Database.Instance.GetCategorySynonyms();

            Record.Affiliate = "TradeTracker";
            Record.Brand = "Apple";
            Record.Category = "Computer";
            Record.DeliveryCost = "3.00";
            Record.DeliveryTime = "1 dat";
            Record.Description = "The new Iphone";
            Record.EAN = "23162617";
            Record.Image = "www.coolblue.nl/img/87271.png";
            Record.Name = "Iphone 6";
            Record.Price = "699,99";
            Record.SKU = "CHU8276";
            Record.Stock = "0";
            Record.Url = "www.coolblue.nl/Iphone6";
            Record.Webshop = "Coolblue";
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
        private Boolean checkBrand()
        {
            // If the Product's Name attribute is an empty String, no brand was given.
            return Record.Brand.Equals("");
        }

        /// <summary>
        /// Method to check if a record contains a category. If it does, this method
        /// also checks if the record category matches a category in the Borderloop category tree.
        /// </summary>
        private Boolean checkCategory()
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

        private int checkSKU(string sku)
        {
            return Database.Instance.GetArticleNumberOfSKU(sku);
        }

        private int checkPartialSKU(string sku)
        {
            return Database.Instance.GetArticleNumberOfPartialSKU(sku);
        }

        private int checkEAN(string ean)
        {
            return Database.Instance.GetArticleNumberOfEAN(ean);
        }

        private int checkTitle(string title)
        {
            return Database.Instance.GetArticleNumberOfTitle(title);
        }
    }
}

