using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BobAndFriends
{

    public class DatabaseJanitor
    {

        public DatabaseJanitor()
        { }

        public void Cleanup()
        {
            //For debugging, open a conneciont    
            Database.Instance.Connect(Statics.settings["dbsource"], Statics.settings["dbname"], Statics.settings["dbuid"], Statics.settings["dbpw"]);

            //Throw an exception if the database is not conencted.
            if(!Database.Instance.isConnected)
            {                         
                throw new DatabaseNotConnectedException("Database connection is not open.");
            }

            //Cleanup EAN
            //If there are multiple EANs, save them as matches with the first.
            DataTable dirtyEANS = Database.Instance.GetDuplicateEANs();
            
            //The table will give an ean and the first article ID.
            foreach(DataRow row in dirtyEANS.Rows)
            {
                //First, get the article number 
                string ean = row.Field<string>("ean");
                int correctAID = row.Field<int>("article_id");

                //Get all the other aid's, which are wrongly implemented. Delete them.
                DataTable wrongAIDs = Database.Instance.GetAIDsFromEAN(ean, correctAID);
                foreach(DataRow row2 in wrongAIDs.Rows)
                {
                    int wrongAID = row2.Field<int>("article_id");
                    //Update product to point to the right article
                    Database.Instance.UpdateProductAID(wrongAID, correctAID);

                    //Add title of the wrong product to the title synonyms of the correct product.
                    Database.Instance.AddTitleSynonym(wrongAID, correctAID);

                    //Delete all other entries of the article
                    Database.Instance.DeleteArticle(wrongAID);
                }
            }

            //Cleanup SKU
            //If there are multiple SKUs, save them as matches with the first.
            DataTable dirtySKUS = Database.Instance.GetDuplicateSKUs();

            //The table will give an ean and the first article ID.
            foreach (DataRow row in dirtyEANS.Rows)
            {
                //First, get the article number 
                string ean = row.Field<string>("sku");
                int correctAID = row.Field<int>("article_id");

                //Get all the other aid's, which are wrongly implemented. Delete them.
                DataTable wrongAIDs = Database.Instance.GetAIDsFromSKU(ean, correctAID);
                foreach (DataRow row2 in wrongAIDs.Rows)
                {
                    int wrongAID = row2.Field<int>("article_id");
                    //Update product to point to the right article
                    Database.Instance.UpdateProductAID(wrongAID, correctAID);

                    //Add title of the wrong product to the title synonyms of the correct product.
                    Database.Instance.AddTitleSynonym(wrongAID, correctAID);

                    //Delete all other entries of the article
                    Database.Instance.DeleteArticle(wrongAID);
                }
            }

            //Cleanup title
            //If there are perfect title matches, save them as matches with the first.


        }
    }

    class DatabaseNotConnectedException : Exception
    {
        public DatabaseNotConnectedException() : base() { }
        public DatabaseNotConnectedException(string msg) : base(msg) { }
        public DatabaseNotConnectedException(string msg, Exception e) : base(msg, e) { }
        
    }
}
