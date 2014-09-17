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

            Console.WriteLine((int)suggestedProducts.Rows[0]["Suggested article IDs"]);
            Console.WriteLine((int)suggestedProducts.Rows[1]["Suggested article IDs"]);
            selProdBind.DataSource = selectedProduct;
            selectedProductDataGrid.DataSource = selProdBind;
            selectedProductDataGrid.DataSource = selectedProduct;
            /*for (int i = 0; i < selectedProductDataGrid.ColumnCount; ++i)
            {
                selectedProduct.Columns.Add(new DataColumn(selectedProductDataGrid.Columns[i].Name));
                selectedProductDataGrid.Columns[i].DataPropertyName = selectedProductDataGrid.Columns[i].Name;
            }*/

            sugProdBind.DataSource = suggestedProducts;
            suggestedProductsDataGrid.DataSource = sugProdBind;
            suggestedProductsDataGrid.DataSource = suggestedProducts;
            /*for (int i = 0; i < selectedProductDataGrid.ColumnCount; ++i)
            {
                suggestedProducts.Columns.Add(new DataColumn(suggestedProductsDataGrid.Columns[i].Name));
                suggestedProductsDataGrid.Columns[i].DataPropertyName = suggestedProductsDataGrid.Columns[i].Name;
            }*/

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
            }

            //Do work

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
            //Do work
            ShowNext();
        }
    }
}
