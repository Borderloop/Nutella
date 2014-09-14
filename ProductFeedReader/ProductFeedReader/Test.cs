using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProductFeedReader
{
    class Test
    {
        private Product Record = new Product();

        public void Start()
        {
            Initialize();

            // If checkCategory() returns false, the record category doesn't match any of the categories 
            // from the Borderloop category tree. Send record to residu.
            if (checkCategory() == false)
            {
                // Send to residu
            }
            else // If checkCategory() returns true, the record category matches with one of the 
                 // Borderloop category tree. Continue with the brand check.
            {
                Boolean brandCheck = checkBrand();
            }
            
        }

        /// <summary>
        /// Initialize dummy data. For test purposes only
        /// </summary>
        private void Initialize()
        {
            Record.Affiliate = "TradeTracker";
            //TestProduct.Brand = "Apple";
            Record.Category = "Smartphone";
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
        /// Method to check if a record contains a brand.
        /// </summary>
        private Boolean checkBrand()
        {
            // If the Product's Name attribute is an empty String, no brand was given.
            if (Record.Brand == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Method to check if a record contains a category. If it does, this method
        /// also checks if the record category matches a category in the Borderloop category tree.
        /// </summary>
        private Boolean checkCategory()
        {
            // If the Product's Category attribute is an empty String, no category was given.
            if (Record.Category == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        
    }
}
