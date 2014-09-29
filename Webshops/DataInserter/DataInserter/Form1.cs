using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataInserter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

         private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'borderloopDataSet.webshop' table. You can move, or remove it, as needed.
            this.webshopTableAdapter.Fill(this.borderloopDataSet.webshop);
            // TODO: This line of code loads data into the 'borderloopDataSet.web_sendingcountry' table. You can move, or remove it, as needed.
            this.web_sendingcountryTableAdapter.Fill(this.borderloopDataSet.web_sendingcountry);
            // TODO: This line of code loads data into the 'borderloopDataSet.web_sender' table. You can move, or remove it, as needed.
            this.web_senderTableAdapter.Fill(this.borderloopDataSet.web_sender);
            // TODO: This line of code loads data into the 'borderloopDataSet.web_payment' table. You can move, or remove it, as needed.
            this.web_paymentTableAdapter.Fill(this.borderloopDataSet.web_payment);
            // TODO: This line of code loads data into the 'borderloopDataSet.web_mark' table. You can move, or remove it, as needed.
            this.web_markTableAdapter.Fill(this.borderloopDataSet.web_mark);
            // TODO: This line of code loads data into the 'borderloopDataSet.web_language' table. You can move, or remove it, as needed.
            this.web_languageTableAdapter.Fill(this.borderloopDataSet.web_language);
            // TODO: This line of code loads data into the 'borderloopDataSet.sender' table. You can move, or remove it, as needed.
            this.senderTableAdapter.Fill(this.borderloopDataSet.sender);
            // TODO: This line of code loads data into the 'borderloopDataSet.payment_method' table. You can move, or remove it, as needed.
            this.payment_methodTableAdapter.Fill(this.borderloopDataSet.payment_method);
            // TODO: This line of code loads data into the 'borderloopDataSet.mark' table. You can move, or remove it, as needed.
            this.markTableAdapter.Fill(this.borderloopDataSet.mark);
            // TODO: This line of code loads data into the 'borderloopDataSet.language' table. You can move, or remove it, as needed.
            this.languageTableAdapter.Fill(this.borderloopDataSet.language);
            // TODO: This line of code loads data into the 'borderloopDataSet.country' table. You can move, or remove it, as needed.
            this.countryTableAdapter.Fill(this.borderloopDataSet.country);
            // TODO: This line of code loads data into the 'borderloopDataSet.affiliate' table. You can move, or remove it, as needed.
            this.affiliateTableAdapter.Fill(this.borderloopDataSet.affiliate);

        }
        private void button1_Click(object sender, EventArgs e)
        {


            StreamReader reader = File.OpenText(@"Webshops3.txt");
            String[] headers = { "Website", "Land", "Verzender", "Verzendprijs", "Keurmerk(en)", "Taal", "Betaalmogelijkheid", " ", "Approved", "Affiliate" };
            int dataCounter = 0;
            int rowCounter = 0;

            while (!reader.EndOfStream)
            {
                String currentLine = reader.ReadLine();         // Reads the current line of data
                String[] splitText = currentLine.Split(';');    // Splits all the current line data into an array
                String[] dataArray = new String[11];
                try { Array.Copy(splitText, dataArray, 11); }
                catch (ArgumentException) { foreach (String x in splitText) { Debug.Write(x); } }// Removes the last 3 unneeded lines from the array

                if (splitText[0] != "" & splitText[0] != "Website") // Makes sure the headers and empty rows are not matched
                {
                    rowCounter++;
                    String[] rowData = new String[11];
                    foreach (String data in dataArray)
                    {
                        rowData[dataCounter] = data;
                        dataCounter++;
                    }
                    dataInserter(rowData);
                    dataCounter = 0;
                }
            }
        }

        private void dataInserter(String[] inputData)
        {
            
            borderloopDataSetTableAdapters.webshopTableAdapter webshopTableAdapter = new borderloopDataSetTableAdapters.webshopTableAdapter();
            borderloopDataSetTableAdapters.languageTableAdapter languageTableAdapter = new borderloopDataSetTableAdapters.languageTableAdapter();
            borderloopDataSetTableAdapters.payment_methodTableAdapter payment_methodTableAdapter = new borderloopDataSetTableAdapters.payment_methodTableAdapter();
            borderloopDataSetTableAdapters.markTableAdapter markTableAdapter = new borderloopDataSetTableAdapters.markTableAdapter();
            borderloopDataSetTableAdapters.senderTableAdapter senderTableAdapter = new borderloopDataSetTableAdapters.senderTableAdapter();
            borderloopDataSetTableAdapters.countryTableAdapter countryTableAdapter = new borderloopDataSetTableAdapters.countryTableAdapter();

            borderloopDataSetTableAdapters.affiliateTableAdapter affiliateTableAdapter = new borderloopDataSetTableAdapters.affiliateTableAdapter();

            borderloopDataSetTableAdapters.web_sendingcountryTableAdapter web_sendingcountryTableAdapter = new borderloopDataSetTableAdapters.web_sendingcountryTableAdapter();
            borderloopDataSetTableAdapters.web_languageTableAdapter web_languageTableAdapter = new borderloopDataSetTableAdapters.web_languageTableAdapter();
            borderloopDataSetTableAdapters.web_paymentTableAdapter web_paymentTableAdapter = new borderloopDataSetTableAdapters.web_paymentTableAdapter();
            borderloopDataSetTableAdapters.web_markTableAdapter web_markTableAdapter = new borderloopDataSetTableAdapters.web_markTableAdapter();
            borderloopDataSetTableAdapters.web_senderTableAdapter web_senderTableAdapter = new borderloopDataSetTableAdapters.web_senderTableAdapter();


            String inputCountry = inputData[1].Trim();
            Debug.WriteLine("Inputcountry:" + inputCountry);
            byte countryID;
            if (inputCountry.Equals("EN"))
                {
                    countryID = Convert.ToByte(countryTableAdapter.getCountryId("uk"));
                }
                else
                {
                    countryID = Convert.ToByte(countryTableAdapter.getCountryId(inputCountry.ToLower()));
                }


            String webshopName = UpperFirst(inputData[0].Split('.')[1]);
            String inputShippingCost = inputData[4].Trim();
            inputShippingCost = inputShippingCost.Replace(',', '.');
            if (inputShippingCost != null & inputShippingCost != "" & inputShippingCost != " " & inputShippingCost != "-")
            {

                Decimal decPrice = 0;
                if (inputShippingCost.Substring(0, 1).Equals("€"))
                {
                    inputShippingCost = inputShippingCost.Substring(1).Trim();
                    inputShippingCost = inputShippingCost.Split(' ')[0];
                    decPrice = decimal.Parse(inputShippingCost);
                }
                else if (inputShippingCost.Substring(0, 1).Equals("£"))
                {
                    inputShippingCost = inputShippingCost.Substring(1).Trim();
                    inputShippingCost = inputShippingCost.Split(' ')[0];
                    decPrice = decimal.Parse(inputShippingCost) * 1.27201m;  // Convert to euro's
                }
                else if (inputShippingCost.Substring(0, 1).Equals("$"))
                {
                    inputShippingCost = inputShippingCost.Substring(1).Trim();
                    inputShippingCost = inputShippingCost.Split(' ')[0];
                    decPrice = decimal.Parse(inputShippingCost) * 0.775833m;  // Convert to euro's
                }
                else
                {
                    inputShippingCost = inputShippingCost.Substring(1).Trim();
                    inputShippingCost = inputShippingCost.Split(' ')[0];
                    decimal number = 0;
                    bool canConvert = decimal.TryParse(inputShippingCost, out number);
                    if (canConvert)
                    {
                        decPrice = decimal.Parse(inputShippingCost);
                    }
                }
                webshopTableAdapter.Insert(webshopName, inputData[0], "0", "0", decPrice,countryID);
            }
            else
            {   
                Debug.WriteLine("CountryID:" + countryID+" Shippng costs:"+inputShippingCost);
                webshopTableAdapter.Insert(webshopName, inputData[0], "0", "0", null,countryID);
            }
            

            int webshopID = Convert.ToInt32(webshopTableAdapter.getLastId());

            String inputsendingCountry = inputData[3].Trim();
            if (inputsendingCountry == null || inputsendingCountry == "-" || inputsendingCountry == "")
            {
                inputsendingCountry = inputData[1];
                byte sendingcountryID = Convert.ToByte(countryTableAdapter.getCountryId(inputsendingCountry.ToLower())); ;
                web_sendingcountryTableAdapter.Insert(sendingcountryID, webshopID);
            }
            else
            {
                if (!inputData[3].Contains(inputData[1])) { inputsendingCountry = inputsendingCountry + "," + inputData[1]; }
                String[] sendingcountryArray = inputsendingCountry.Split(',');
                foreach (String country in sendingcountryArray)
                {
                    String tempcountry = country.Trim();    // Temporary language string which was trimmed of whitespaces
                    byte sendingcountryID;
                    sendingcountryID = Convert.ToByte(countryTableAdapter.getCountryId(tempcountry.ToLower()));
                    try
                    {
                        web_sendingcountryTableAdapter.Insert(sendingcountryID, webshopID);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        Debug.WriteLine("Error:" + e);
                        break;
                    }
                }
            }





            if (inputData[6] != null)
            {
                String inputLanguage = inputData[6].Trim();
                if (inputLanguage == null || inputLanguage == "-" || inputLanguage == "")
                {
                    inputLanguage = inputData[1];
                    byte languageID;
                    switch (inputLanguage)
                    {
                        case "WW":
                        case "EU":
                        case "UK":
                            languageID = Convert.ToByte(languageTableAdapter.getLanguageId("en"));
                            break;
                        case "AT":
                        case "CH":
                            languageID = Convert.ToByte(languageTableAdapter.getLanguageId("de"));
                            break;
                        default:
                            languageID = Convert.ToByte(languageTableAdapter.getLanguageId(inputLanguage.ToLower()));
                            break;
                    }
                    web_languageTableAdapter.Insert(languageID, webshopID);
                }

                else
                {
                    if (!inputData[6].Contains(inputData[1])) { inputLanguage = inputLanguage + "," + inputData[1]; }
                    String[] languageArray = inputLanguage.Split(',');
                    foreach (String language in languageArray)
                    {
                        String templg = language.Trim();    // Temporary language string which was trimmed of whitespaces
                        byte languageID;
                        if (templg.Equals("UK"))
                        {
                            languageID = Convert.ToByte(languageTableAdapter.getLanguageId("en"));
                        }
                        else
                        {
                            languageID = Convert.ToByte(languageTableAdapter.getLanguageId(templg.ToLower()));
                        }
                        try
                        {
                            web_languageTableAdapter.Insert(languageID, webshopID);
                        }
                        catch (MySql.Data.MySqlClient.MySqlException)
                        {
                            break;
                        }
                    }
                }
            }


            if (inputData[7] != null & inputData[7] != "-" & inputData[7] != "")
            {
                String[] paymentMethods = inputData[7].Split(',');
                foreach (String pm in paymentMethods)
                {
                    String tempData = pm.Trim();
                    byte methodID = Convert.ToByte(payment_methodTableAdapter.getMethodId(tempData));
                    Debug.WriteLine("PM:" + tempData);
                    web_paymentTableAdapter.Insert(methodID, webshopID);
                }
            }

            if (inputData[5] != null & inputData[5] != "-" & inputData[5] != "")
            {
                String[] markArray = inputData[5].Split(',');
                foreach (String mark in markArray)
                {
                    String tempMark = mark.Trim();
                    Debug.WriteLine("Tempmark:" + tempMark);
                    byte markID = Convert.ToByte(markTableAdapter.getMarkId(tempMark));
                    Debug.WriteLine(tempMark+" "+markID);
                    web_markTableAdapter.Insert(markID,webshopID);
                }
            }


            if (inputData[10] != null & inputData[10] != "-" & inputData[10] != "")
            {
                String[] inputAffiliates = inputData[10].Split(',');
                foreach (String affiliate in inputAffiliates)
                {   short approved = 0;
                    try { 
                        approved = Convert.ToInt16(inputData[9]);
                    }
                    catch (FormatException){}
                    if (affiliate == "Eigen" || affiliate == "(eigen affiliate)")
                    {
                        affiliateTableAdapter.Insert("Client Partner Program", approved, webshopID, "0");
                    }
                    else if (affiliate == "Onze")
                    {
                        affiliateTableAdapter.Insert("Betsy", approved, webshopID, "0");
                    }
                    else
                    {
                        String tempAffiliate = affiliate.Trim();
                        affiliateTableAdapter.Insert(tempAffiliate, approved, webshopID, "0");
                    }
                }
            }

        }

        private string UpperFirst(string text)
        {
            return char.ToUpper(text[0]) +
                ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
        }

        private void affiliateBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.affiliateBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.borderloopDataSet);

        }

       
    }
}
