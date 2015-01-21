using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterGUI
{
    public partial class SelfAddedProductsForm : Form
    {
        private bool ChangesSaved = true;
        public const string FilePath = @"C:/BorderSoftware/Boris/ProductFeeds/Rene's Toppertjes/renestoppertjes.xml";
        List<string> Webshops = NewMasterGUI.Context.webshop.Select(w => w.url).ToList();
        int CurrentIdNumber;
        public SelfAddedProductsForm()
        {
            InitializeComponent();
            SelfAddedProductsXml.ReadXml(FilePath);
            SelfAddedProductsView.DataSource = SelfAddedProductsXml;
            SelfAddedProductsView.DataMember= "Product";
            CurrentIdNumber = GetCurrentMaxId();           
        }

        private int GetCurrentMaxId()
        {
            int max = 0;
            foreach(DataRow row in SelfAddedProductsXml.Tables["Product"].Rows)
            {
                string IdString = row["ProductID"] as string;
                if (IdString == null) continue;
                IdString = IdString.Substring(1);
                int temp;
                if (!int.TryParse(IdString, out temp)) continue;
                else if (temp > max) max = temp;
            }
            return max;
        }
        private void SelfAddedProductsView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ChangesSaved = false;
            if(e.ColumnIndex == SelfAddedProductsXml.Tables["Product"].Columns["Deeplink"].Ordinal)
            {
                string s = SelfAddedProductsXml.Tables["Product"].Rows[e.RowIndex]["Deeplink"] as string;
                if (s == null || s == "") return;
                try
                {
                    Uri uri = new Uri(s);
                    SelfAddedProductsXml.Tables["Product"].Rows[e.RowIndex]["Webshop"] = uri.Host;
                }
                catch
                {
                    SelfAddedProductsXml.Tables["Product"].Rows[e.RowIndex]["Webshop"] = "##VALUE##";
                }
            }           
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var newRow = SelfAddedProductsXml.Tables["Product"].NewRow();
            SelfAddedProductsXml.Tables["Product"].Rows.Add(newRow);
            int index = SelfAddedProductsXml.Tables["Product"].Rows.IndexOf(newRow);
            SelfAddedProductsXml.Tables["Product"].Rows[index]["ProductID"] = "R" + (CurrentIdNumber + 1);
            CurrentIdNumber++;
            ChangesSaved = false;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in SelfAddedProductsView.SelectedRows)
            {
                SelfAddedProductsView.Rows.RemoveAt(item.Index);
            }
            ChangesSaved = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach(DataGridViewRow row in SelfAddedProductsView.Rows)
            {
                if (!DBNull.Value.Equals(row.Cells["Webshop"].Value) && !Webshops.Contains((string)row.Cells["Webshop"].Value))
                {
                    row.Selected = true;
                    MessageBox.Show("The webshop in row " + i + " does not exist in the database. Aborting the save.");
                    return;
                }
                i++;
            }
            SelfAddedProductsXml.WriteXml(FilePath);
            ChangesSaved = true;
            MessageBox.Show("Save successful.");
        }

        private void SelfAddedProductsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!ChangesSaved)
            {
                DialogResult result = MessageBox.Show("Changed were made, but not saved. Are you sure you want to exit?", "Warning", MessageBoxButtons.YesNo);
                if(result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CloneButton_Click(object sender, EventArgs e)
        {
            if (SelfAddedProductsView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to clone.");
                return;
            }
            var desRow = SelfAddedProductsXml.Tables["Product"].NewRow();
            int currentIndex = SelfAddedProductsView.CurrentCell.RowIndex;
            var sourceRow = SelfAddedProductsXml.Tables["Product"].Rows[currentIndex];
            desRow.ItemArray = sourceRow.ItemArray.Clone() as object[];
            SelfAddedProductsXml.Tables["Product"].Rows.Add(desRow);
            int index = SelfAddedProductsXml.Tables["Product"].Rows.IndexOf(desRow);
            SelfAddedProductsXml.Tables["Product"].Rows[index]["ProductID"] = "R" + (CurrentIdNumber + 1);
            SelfAddedProductsXml.Tables["Product"].Rows[index]["Price"] = "";
            SelfAddedProductsXml.Tables["Product"].Rows[index]["Webshop"] = "";
            SelfAddedProductsXml.Tables["Product"].Rows[index]["Deeplink"] = "";
            CurrentIdNumber++;
            Refresh();
        }
    }
}
