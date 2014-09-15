using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MariaDBConnector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void affiliatesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.affiliatesBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.borderloopDataSet);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'borderloopServerDataSet.operatingcountry' table. You can move, or remove it, as needed.
            this.operatingcountryTableAdapter.Fill(this.borderloopServerDataSet.operatingcountry);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.affiliates' table. You can move, or remove it, as needed.
            this.affiliatesTableAdapter1.Fill(this.borderloopServerDataSet.affiliates);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.sender' table. You can move, or remove it, as needed.
            this.senderTableAdapter1.Fill(this.borderloopServerDataSet.sender);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.payment_methods' table. You can move, or remove it, as needed.
            this.payment_methodsTableAdapter1.Fill(this.borderloopServerDataSet.payment_methods);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.marks' table. You can move, or remove it, as needed.
            this.marksTableAdapter1.Fill(this.borderloopServerDataSet.marks);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.languages' table. You can move, or remove it, as needed.
            this.languagesTableAdapter1.Fill(this.borderloopServerDataSet.languages);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.country' table. You can move, or remove it, as needed.
            this.countryTableAdapter1.Fill(this.borderloopServerDataSet.country);
            // TODO: This line of code loads data into the 'borderloopServerDataSet.webshop' table. You can move, or remove it, as needed.
            this.webshopTableAdapter1.Fill(this.borderloopServerDataSet.webshop);
        }

        private void button1_Click(object sender, EventArgs e)
        {


            StreamReader reader = File.OpenText(@"D:/School/Jaar 3/Stage Borderloop/Webshop overzicht (Borderloop).txt");
            String[] headers = { "Website", "Land", "Verzender", "Verzendprijs", "Keurmerk(en)", "Taal", "Betaalmogelijkheid"," ", "Approved", "Affiliate" };
            int dataCounter = 0;
            int rowCounter = 0;

            while (!reader.EndOfStream)
            {
                String currentLine = reader.ReadLine();         // Reads the current line of data
                String[] splitText = currentLine.Split(';');    // Splits all the current line data into an array
                String[] dataArray = new String[10];
                try { Array.Copy(splitText, dataArray, 10); }
                catch (ArgumentException) {foreach (String x in splitText){Debug.Write(x);} }// Removes the last 3 unneeded lines from the array

                if (splitText[0] != "" & splitText[0] != "Website") // Makes sure the headers and empty rows are not matched
                {
                    rowCounter++;
                    String[] rowData = new String[10];
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
            borderloopServerDataSetTableAdapters.languagesTableAdapter languageTableAdapter = new borderloopServerDataSetTableAdapters.languagesTableAdapter();
            borderloopServerDataSetTableAdapters.webshopTableAdapter webshopTableAdapter = new borderloopServerDataSetTableAdapters.webshopTableAdapter();
            borderloopServerDataSetTableAdapters.payment_methodsTableAdapter payment_methodsTableAdapter = new borderloopServerDataSetTableAdapters.payment_methodsTableAdapter();
            borderloopServerDataSetTableAdapters.marksTableAdapter marksTableAdapter = new borderloopServerDataSetTableAdapters.marksTableAdapter();
            borderloopServerDataSetTableAdapters.senderTableAdapter senderTableAdapter = new borderloopServerDataSetTableAdapters.senderTableAdapter();
            borderloopServerDataSetTableAdapters.affiliatesTableAdapter affiliatesTableAdapter = new borderloopServerDataSetTableAdapters.affiliatesTableAdapter();
            borderloopServerDataSetTableAdapters.countryTableAdapter countryTableAdapter = new borderloopServerDataSetTableAdapters.countryTableAdapter();
            borderloopServerDataSetTableAdapters.operatingcountryTableAdapter operatingCountryTableAdapter = new borderloopServerDataSetTableAdapters.operatingcountryTableAdapter();

            String inputCountry = inputData[1];
            if (inputCountry == "EN") {inputCountry="UK";}
            byte webshopCountry = Convert.ToByte(countryTableAdapter.getCountryId(inputCountry.ToLower()));


            String webshopName = UpperFirst(inputData[0].Split('.')[1]);
            webshopTableAdapter.Insert(webshopName, inputData[0], "0","0",webshopCountry);
            int webshopID = Convert.ToInt32(webshopTableAdapter.getLastId());


            String inputLanguage = inputData[5].Trim();
            if (inputData[1] == "UK") { inputData[1] = "EN"; }
            if (inputLanguage == null || inputLanguage == "-" || inputLanguage == "")
            {
                inputLanguage = inputData[1];
                languageTableAdapter.Insert(inputLanguage, webshopID);
            }
            else 
            {
                if (!inputData[5].Contains(inputData[1])) { inputLanguage = inputLanguage + "," + inputData[1]; }
                String[] languageArray = inputLanguage.Split(',');
                foreach (String language in languageArray) 
                {
                    String templg = language.Trim();    // Temporary language string which was trimmed of whitespaces
                    languageTableAdapter.Insert(templg, webshopID);
                    if (templg == "EN")
                    { 
                        String tempcountry = "Verenigd Koninkrijk";
                        operatingCountryTableAdapter.Insert(tempcountry, webshopID);
                    }
                    else if (templg != null)
                    {
                        
                        String tempcountry = countryTableAdapter1.getCountryName(templg.ToLower());
                        operatingCountryTableAdapter.Insert(tempcountry, webshopID);
                    }
                    
                }
            }
            

            if (inputData[6] != null & inputData[6] != "-" & inputData[6] != "")
            {
                String[] paymentMethods = inputData[6].Split(',');
                foreach (String pm in paymentMethods)
                {
                    String tempData = pm.Trim();
                    payment_methodsTableAdapter.Insert(tempData, webshopID);
                }
            }


            if (inputData[4] != null & inputData[4] != "-" & inputData[4] != "")
            {
                String[] markArray = inputData[4].Split(',');
                foreach (String mark in markArray)
                {
                    String tempMark = mark.Trim();
                    marksTableAdapter.Insert(tempMark,"0", "0",webshopID);
                }
                
            }

            String inputShippingCost = inputData[3].Trim();
            if (inputData[2] == "-")
            {
                senderTableAdapter.Insert("Onbekend", inputShippingCost, webshopID);
            }
            else if (inputData[2] != null & inputData[2] != "") 
            {
                senderTableAdapter.Insert(inputData[2], inputShippingCost, webshopID);
            }


            if (inputData[9] != null & inputData[9] != "-" & inputData[9] != "") 
            {
                String[] inputAffiliates = inputData[9].Split(',');
                foreach (String affiliate in inputAffiliates)
                {
                    if (affiliate == "Eigen" || affiliate == "(eigen affiliate)") 
                    {
                        affiliatesTableAdapter.Insert("Client Partner Program", inputData[8], webshopID,"0");
                    }
                    else if (affiliate == "Onze")
                    {
                        affiliatesTableAdapter.Insert("Betsy", inputData[8], webshopID, "0");
                    }
                    else
                    {
                        String tempAffiliate = affiliate.Trim();
                        affiliatesTableAdapter.Insert(tempAffiliate, inputData[8], webshopID, "0");
                    }
                }
            }
        }

        private string UpperFirst(string text)
        {
            return char.ToUpper(text[0]) +
                ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
        }
    }
}
