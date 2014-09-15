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
        /// File that contains settings of the ProductFeedReader.
        /// </summary>
        private INIFile _ini;

        /// <summary>
        /// Contains database login credentials.
        /// </summary>
        private Dictionary<string, string> _settings;

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
		
		public BOB() 
        {
            Initialize();
        }

        public void Process(Product p = null)
        {           
            // If checkCategory() returns false, the record category doesn't match any of the categories 
            // from the Borderloop category tree. Send record to residue and stop execution of method.
            if (checkCategory() == false)
            {
                Debug.WriteLine("Sending record to residue due to category check fail");
                return;
            }

            // If checkCategory() returns true, the record category matches with one of the 
            // Borderloop category tree. Continue with the brand check.

            Debug.WriteLine("Category check succes, continuing brand check");

            // If checkBrand() returns false, the record doesn't contain a brand. Send record
            // to residue and stop execution of method.
            if (checkBrand() == false)
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

            // Get all the settings from the INI-file
            _ini = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\pfr.ini");
            _settings = _ini.GetAllValues();

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
                Database.Instance.Connect(_settings["dbsource"], _settings["dbname"], _settings["dbuid"], _settings["dbpw"]);
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
    }
}
