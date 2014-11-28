using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using BorderSource.BetsyContext;
using BorderSource.Common;
using BorderSource.ProductAssociation;

namespace VisualBob
{
    public partial class VisualBob : Form
    {
        /// <summary>
        /// This List contains all suggested products for the selected product. It will
        /// be updated regularly.
        /// </summary>
        private IEnumerable<DbDataRecord> suggestedProducts;

        /// <summary>
        /// This datatable represents the current product to be manually matched.
        /// </summary>
        private vbobdata selectedProduct;

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
            //Database.Instance.Connect("127.0.0.1", "test2", "**", "**");
            selectedProductDataGrid.AutoGenerateColumns = true;
            suggestedProductsDataGrid.AutoGenerateColumns = true;

            selProdBind = new BindingSource();
            sugProdBind = new BindingSource();

            ShowNext();
        }

        /// <summary>
        /// This method will load the next product in VisualBob
        /// </summary>
        private void ShowNext()
        {
            if(!this.Visible)
            {
                this.Visible = true;
            }
            //selectedProduct = Database.Instance.GetNextVBobProduct();
            if (selectedProduct.Equals(default(vbobdata)))
            {
                MessageBox.Show("There are no more products in the database. Closing VisualBob.");
                Application.Exit();
                return;
            }
            //suggestedProducts = Database.Instance.GetSuggestedProducts(selectedProduct.id);

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
            article selected = (article)suggestedProductsDataGrid.SelectedRows[0].DataBoundItem;
            // TO BE IMPLEMENTED
            //Database.Instance.SaveMatch(ToProduct(selectedProduct), selected.id, (int)selectedProduct.country_id);
            //Database.Instance.DeleteFromVbobData(selectedProduct.id);
            ShowNext();
        }

        private void rerunButton_Click(object sender, EventArgs e)
        {
            //Database.Instance.RerunVbobEntry(selectedProduct);
            ShowNext();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            //TO BE IMPLEMENTED
            //Database.Instance.SaveNewArticle(ToProduct(selectedProduct),(int)selectedProduct.country_id);
            //Database.Instance.DeleteFromVbobData(selectedProduct.id);
            ShowNext();
        }

        private void residuButton_Click(object sender, EventArgs e)
        {
            //Database.Instance.SendToResidue(ToProduct(selectedProduct));
            //Database.Instance.DeleteFromVbobData(selectedProduct.id);
            
            ShowNext();
        }
      

        /// <summary>
        /// This method will return a product object given a DataTable containing product info/
        /// </summary>
        /// <param name="productTable">The DataTable containing the product info</param>
        /// <param name="rowNumber">the rownumber, default = 0</param>
        /// <returns>A product object with product data</returns>
        private Product ToProduct(vbobdata vbobproduct)
        {
            //Create a product object
            Product p = new Product();

            //Put data in the Product object
            p.Title = vbobproduct.title; ;
            p.EAN = vbobproduct.ean;
            p.SKU = vbobproduct.sku;
            p.Brand = vbobproduct.brand;
            p.Category = vbobproduct.category;
            p.Description = vbobproduct.description;
            p.Image_Loc = vbobproduct.image_loc;

            //Return the product object.
            return p;
        }
    }
}
