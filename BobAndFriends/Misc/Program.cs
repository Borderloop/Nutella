using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using BorderSource.ProductAssociation;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace Misc
{
    class Program
    {

        static void Main(string[] args)
        {
            foreach(List<Product> products in ReadFromFile(@"C:\Product Feeds\Bol\bolcom_elektronica.csv"))
            {

            }
        }

        public static IEnumerable<List<Product>> ReadFromFile(string file)
        {
            using(var sr = new StreamReader(file))
            {
                sr.ReadLine();
                using (CsvReader reader = new CsvReader(sr, false, '|'))
                {
                    List<Product> products = new List<Product>();
                    string[] headers;
                    switch (Path.GetFileNameWithoutExtension(file).Split('_')[1])
                    {
                        case "games":
                            headers = new string[]{
                                    "Unit", "Cat1", "Cat2", "Cat3", 
                                    "TypeCode", "Title", "Subtitle", 
                                    "FreeGift", "Publisher", "Price", 
                                    "Shipping", "DeliveryTime", "Deliverable", 
                                    "Stock", "MPN", "EAN", "ID", "Image", "Url" };
                            break;
                        case "elektronica":
                            headers = new string[]{
                                    "Unit", "Cat1", "Cat2", "Cat3", 
                                    "TypeCode", "Title", "Subtitle", 
                                    "FreeGift", "Brand", "Colour", "Price", 
                                    "Shipping", "DeliveryTime", "Deliverable", 
                                    "Stock", "MPN", "EAN", "ID", "Image", "Url" };
                            while (reader.ReadNextRecord())
                            {

                                int fc = reader.FieldCount;
                                Product p = new Product();
                                p.Affiliate = "Bol";
                                p.AffiliateProdID = reader[17];
                                p.Brand = reader[8];
                                p.Category = reader[1] + " -> " + reader[2] + " -> " + reader[3];
                                p.Currency = "EUR";
                                p.DeliveryCost = reader[11];
                                p.DeliveryTime = reader[12];
                                p.EAN = reader[16];
                                p.FileName = file;
                                p.Image_Loc = reader[18];
                                p.Price = reader[10];
                                p.Stock = reader[14];
                                p.Title = reader[5];
                                p.Url = reader[19];
                                p.Webshop = "www.bol.com";

                                products.Add(p);
                                if (products.Count >= 10)
                                {
                                    yield return products;
                                    products.Clear();
                                }
                            }
                            yield return products;
                            break;
                    }
                }
            }
        }

    }
}
