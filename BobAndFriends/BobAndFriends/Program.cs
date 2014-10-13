using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Data.Entity.Validation;
using BorderSource.Common;

namespace BobAndFriends
{
    class Program
    {
        /// <summary>
        /// This thread is used by the productfeedreader, being the producer in this application.
        /// </summary>
        public static Thread producer;

        /// <summary>
        /// This thread is used by BOB, being the consumer in the application.
        /// </summary>
        public static Thread consumer;

        /// <summary>
        /// Main method will only start the ProductFeedReader
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            
            //Initialize
            Initialize();

            //DatabaseJanitor crapper = new DatabaseJanitor();
            //crapper.Cleanup();    

            //Create threads
            producer = new Thread(new ThreadStart(ProductFeedReader));
            consumer = new Thread(new ThreadStart(ProductDequeuer));
            
            //Start threads
            producer.Start();
            consumer.Start();

            
            //Application.EnableVisualStyles();
            //Application.Run(new VisualBob());

            //sw = new Stopwatch();
             
            //Application.EnableVisualStyles();
            
/*try
            {
                Application.Run(new VisualBob());
            } 
            catch(Exception e)
            {
                //don't do anything as it is not that important
            }
            finally
            {
                Application.Exit();
            }
 */
        }

        static void ProductDequeuer()
        {
            //Create BOB and start dequeueing items.
            BOB bob = new BOB();

            int errorCount = 0;
            //Remain alive while thread one isnt done
            while (true)
            {
                //Take product from queue
                Product p = ProductQueue.Dequeue();

                //Break out of while loop when the queue gives null. This only happens when 
                //productfeedreader is flagged as done.
                if(p == null)
                {
                    break;
                }
                //Process the product otherwise.
                try
                {
                    bob.Process(p);
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    Console.Read();
                }
                catch (Exception e)
                {
                    Console.WriteLine("THREW EXCEPTION {0}: " + e.ToString() + " FROM " + e.StackTrace, errorCount);
                    Statics.Logger.WriteLine("ERROR#{0} " + e.Message, errorCount);
                    errorCount++;
                    Console.WriteLine("Press enter to clear all pools and continue.");
                    Console.Read();

                    //Probably the underlying connection. 
                    //Trying to clear the pools, then move on.
                    MySql.Data.MySqlClient.MySqlConnection.ClearAllPools();      

                }            
            }

            //Rerun all the products in the residue. We do not need ProductFeedReader for this.
            //bob.RerunResidue();

            //Tidy up and close
            bob.FinalizeAndClose();

            Console.WriteLine("Thread 2 is done.");
        }

        static void ProductFeedReader()
        {
            //Create productfeedreader object.
            ProductFeedReader pfr = new ProductFeedReader();

            //Start it
            pfr.Start();

            Console.WriteLine("Thread 1 is done.");
            Console.WriteLine("Queue size: " + ProductQueue.queue.Count);
        }

        static void Initialize()
        {
            //Initialize all the values for the static variables in the Statics class. These
            //variables are used throughout the whole program.

            #region Settings
            Statics.settings = new INIFile("C:\\BorderSoftware\\BobAndFriends\\settings\\baf.ini").GetAllValues();
            #endregion

            #region Loggers
            Statics.Logger = new Logger(Statics.settings["logpath"] + "\\log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            Statics.SqlLogger = new Logger(Statics.settings["logpath"] + "\\sqldump" + DateTime.Now.ToString("MMddHHmm") + ".txt");
            #endregion

            #region Values
            Statics.TickCount = 0;
            Statics.TicksUntilSleep = Int32.Parse(Statics.settings["ticksuntilsleep"]);
            Statics.maxQueueSize = Int32.Parse(Statics.settings["maxqueuesize"]);
            Statics.maxSkuSize = Int32.Parse(Statics.settings["maxskusize"]);
            Statics.maxBrandSize = Int32.Parse(Statics.settings["maxbrandsize"]);
            Statics.maxTitleSize = Int32.Parse(Statics.settings["maxtitlesize"]);
            Statics.maxImageUrlSize = Int32.Parse(Statics.settings["maximageurlsize"]);
            Statics.maxCategorySize = Int32.Parse(Statics.settings["maxcategorysize"]);
            Statics.maxShipTimeSize = Int32.Parse(Statics.settings["maxshiptimesize"]);
            Statics.maxWebShopUrlSize = Int32.Parse(Statics.settings["maxwebshopurlsize"]);
            Statics.maxDirectLinkSize = Int32.Parse(Statics.settings["maxdirectlinksize"]);
            Statics.maxAffiliateNameSize = Int32.Parse(Statics.settings["maxaffiliatenamesize"]);
            Statics.maxAffiliateProductIdSize = Int32.Parse(Statics.settings["maxaffiliateproductidsize"]);
            Statics.maxResidueListSize = Int32.Parse(Statics.settings["maxresiduelistsize"]);
            #endregion

            #region Mapping of DBProduct to BobProduct
            Statics.TwoWayDBProductToBobProductMapping = new Dictionary<string, string>();
            Statics.TwoWayDBProductToBobProductMapping.Add("ship_time", "DeliveryTime");
            Statics.TwoWayDBProductToBobProductMapping.Add("ship_cost", "DeliveryCost");
            Statics.TwoWayDBProductToBobProductMapping.Add("price", "Price");
            Statics.TwoWayDBProductToBobProductMapping.Add("webshop_url", "Webshop");
            Statics.TwoWayDBProductToBobProductMapping.Add("direct_link", "Url");
            Statics.TwoWayDBProductToBobProductMapping.Add("last_modified", "LastModified");
            Statics.TwoWayDBProductToBobProductMapping.Add("valid_until", "ValidUntil");
            Statics.TwoWayDBProductToBobProductMapping.Add("affiliate_name", "Affiliate");
            Statics.TwoWayDBProductToBobProductMapping.Add("affiliate_unique_id", "AffiliateProdID");
            #endregion

            #region Mapping of BobProduct to DBProduct
            Statics.TwoWayDBProductToBobProductMapping.Add("DeliveryTime", "ship_time");
            Statics.TwoWayDBProductToBobProductMapping.Add("DeliveryCost", "ship_cost");
            Statics.TwoWayDBProductToBobProductMapping.Add("Price", "price");
            Statics.TwoWayDBProductToBobProductMapping.Add("Webshop", "webshop_url");
            Statics.TwoWayDBProductToBobProductMapping.Add("Url", "direct_link");
            Statics.TwoWayDBProductToBobProductMapping.Add("LastModified", "last_modified");
            Statics.TwoWayDBProductToBobProductMapping.Add("ValidUntil", "valid_until");
            Statics.TwoWayDBProductToBobProductMapping.Add("Affiliate", "affiliate_name");
            Statics.TwoWayDBProductToBobProductMapping.Add("AffiliateProdID", "affiliate_unique_id");
            #endregion
        }


    }
}
