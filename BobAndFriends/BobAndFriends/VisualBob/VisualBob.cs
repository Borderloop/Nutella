using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BobAndFriends
{
    public partial class VisualBob : Form
    {
        /// <summary>
        /// This List contains all suggested products for the selected product. It will
        /// be updated regularly.
        /// </summary>
        private DataTable suggestedProducts;

        /// <summary>
        /// This datatable represents the current product to be manually matched.
        /// </summary>
        private DataTable selectedProduct;

        private BindingSource selProdBind;
        private BindingSource sugProdBind;
 

        public VisualBob()
        {
            InitializeComponent();
            InitializeProducts();
        }

        /// <summary>
        /// This method will initialize the values used by VisualBob
        /// </summary>
        private void InitializeProducts()
        {
            Database.Instance.Connect("149.210.175.211", "test", "root", "border1!LOOP");
            selectedProductDataGrid.AutoGenerateColumns = true;
            suggestedProductsDataGrid.AutoGenerateColumns = true;

            selProdBind = new BindingSource();
            sugProdBind = new BindingSource();
        }

        /// <summary>
        /// This method will load the next product in VisualBob
        /// </summary>
        private void ShowNext()
        {
            selectedProduct = Database.Instance.GetNextVBobProduct();
            suggestedProducts = Database.Instance.GetSuggestedProducts((int)selectedProduct.Rows[0]["id"]);

            selProdBind.DataSource = selectedProduct;
            selectedProductDataGrid.DataSource = selProdBind;
            selectedProductDataGrid.DataSource = selectedProduct;

            sugProdBind.DataSource = suggestedProducts;
            suggestedProductsDataGrid.DataSource = sugProdBind;
            suggestedProductsDataGrid.DataSource = suggestedProducts;

            RefreshAll();
        }

        private void RefreshAll()
        {
            this.Refresh();
            selectedProductDataGrid.Refresh();
            suggestedProductsDataGrid.Refresh();
        }

        private void matchButton_Click(object sender, EventArgs e)
        {
            if(suggestedProductsDataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("No rows selected. Select a row to match the product to a suggestion.");
                return;
            }

            SaveMatch(ToProduct(selectedProduct), (int)suggestedProductsDataGrid.SelectedRows[0].Cells["Article ID"].Value);

            ShowNext();
        }

        private void rerunButton_Click(object sender, EventArgs e)
        {
            //Do work
            ShowNext();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            //Do work
            ShowNext();
        }

        private void residuButton_Click(object sender, EventArgs e)
        {
            Database.Instance.SendTo(ToProduct(selectedProduct), "residue");
            Database.Instance.DeleteFromVbobData((int)selectedProduct.Rows[0]["id"]);
            ShowNext();
        }

        /// <summary>
        /// THIS METHOD COMES FROM BOB.cs
        /// 
        /// This method is called when a match is found. It saves the found match to the database.
        /// It adds missing data to the found article and adds synonyms.
        /// </summary>
        private void SaveMatch(Product Record, int matchedArticleID)
        {
            DataTable MatchedArticle = Database.Instance.GetProduct(matchedArticleID);

            // First, check if there are null  or empty values present in the matched article.
            // If there are, check if the record has values for the missing data and update these values.
            foreach (DataColumn column in MatchedArticle.Columns)
            {
                Object o = MatchedArticle.Rows[0][column];
                if (MatchedArticle.Rows.OfType<DataRow>().Any(r => r.IsNull(column) || o.ToString() == "")) // If matched article has no value for this column...
                {
                    String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
                    String recordValue = Record.GetType().GetProperty(splitted[1]).GetValue(Record, null).ToString();

                    // If the TABLE name doesn't equal 'article', it's either the ean, sku or titles table. Also meaning that 
                    // there is no record in this table at all for the matched article. Because of this, an update won't work:
                    // Insert instead.
                    if (recordValue != "")
                    {
                        if (splitted[0] != "article")
                        {
                            Database.Instance.AddForMatch(splitted[0], recordValue, matchedArticleID);
                        }
                        else
                        {
                            Database.Instance.Update(splitted[0], splitted[1], recordValue, matchedArticleID);
                        }
                    }
                }
                // Else the column has a value, so check if the record value differs from the article value
                // and if so, save it.
                else
                {
                    String[] splitted = column.ToString().Split('-'); // Column name comes with table name and column name, seperated by '-'.
                    if (splitted[0].ToString() != "article") // We only want to add (double) data for ean, sku and titles.
                    {
                        String recordValue = Record.GetType().GetProperty(splitted[1]).GetValue(Record, null).ToString();
                        String matchedValue = o.ToString();
                        bool hasMatch = false;
                        try
                        {
                            hasMatch = MatchedArticle.AsEnumerable().Any(row => recordValue.ToLower() == row.Field<String>(column).ToLower());
                        }
                        catch (InvalidCastException) // An ean is returned, which is int64 instead of string. Convert values for this.
                        {
                            hasMatch = MatchedArticle.AsEnumerable().Any(row => Convert.ToInt64(recordValue) == Convert.ToInt64(row.Field<Int64>(column)));
                        }

                        // If hasMatch is false, a different value is found. Insert this into the database, but only if the record value is not empty.
                        if (hasMatch == false && recordValue != null && recordValue != "")
                        {
                            Database.Instance.AddForMatch(splitted[0], recordValue, matchedArticleID);
                        }

                    }
                }
            }

            // Finally check if the record category is the same as the article category. If it's not, check if
            // there's a match in the category_synonyms table for the matched article. If that's also not the case,
            // add the category from the record to the category_synonyms.
            DataTable category = Database.Instance.GetCategoryForArticle(matchedArticleID);
            bool containsCategory = category.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());

            if (containsCategory == false)
            {
                DataTable categorySynonyms = Database.Instance.GetCategorySynonymsForArticle(matchedArticleID);
                bool containsCategorySynonym = categorySynonyms.AsEnumerable().Any(row => Record.Category.ToLower() == row.Field<String>("description").ToLower());

                if (containsCategorySynonym == false) // If containsCategorySynonym equals false, a category synonym is found: Save it.
                {
                    Object o = category.Rows[0][0];
                    string id = o.ToString(); // category id

                    Database.Instance.SaveCategorySynonym(Convert.ToInt32(id), Record.Category);
                }
            }
        }

        /// <summary>
        /// This method will return a product object given a DataTable containing product info/
        /// </summary>
        /// <param name="productTable">The DataTable containing the product info</param>
        /// <param name="rowNumber">the rownumber, default = 0</param>
        /// <returns>A product object with product data</returns>
        private Product ToProduct(DataTable productTable, int rowNumber = 0)
        {
            //Create a product object
            Product p = new Product();

            //Return null if the datatable is empty
            if(productTable.Rows.Count == 0)
            {
                return null;
            }
            //Put data in the Product object
            p.Title = productTable.Rows[rowNumber].Field<String>("Title") ?? "";
            p.EAN = productTable.Rows[rowNumber].Field<Int64?>("EAN") ?? null;
            p.SKU = productTable.Rows[rowNumber].Field<String>("SKU") ?? "";
            p.Brand = productTable.Rows[rowNumber].Field<String>("Brand") ?? "";
            p.Category = productTable.Rows[rowNumber].Field<String>("Category") ?? "";
            p.Description = productTable.Rows[rowNumber].Field<String>("Description") ?? "";
            p.Image_Loc = productTable.Rows[rowNumber].Field<String>("ImageLocation") ?? "";

            //Return the product object.
            return p;
        }
    }
}
