using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CategoryMatcher
{
    public partial class CategoryMatcherForm : Form
    {

        private DataTable selectedCategory;
        private DataTable inkomendDescription;
        private DataTable matchProduct;

        private BindingSource selCatBind;
        private BindingSource inkDesBind;
        private BindingSource matProduct;

        public CategoryMatcherForm()
        {
            InitializeComponent();

            // this.TopMost = true;
            // this.FormBorderStyle = FormBorderStyle.None;
            // this.WindowState = FormWindowState.Maximized;

            //  Data From Table Category
            selectedCategory = Database.Instance.Read("SELECT * FROM category");
            selCatBind = new BindingSource();
            selCatBind.DataSource = selectedCategory;
            categoryGridView.DataSource = selCatBind;
            categoryGridView.DataSource = selectedCategory;

            categoryGridView.AutoGenerateColumns = true;

            //   Date From Residue 
            // inkomendDescription = Database.Instance.Read("SELECT title,category,ean,sku,web_url FROM residue WHERE category NOT IN(SELECT description FROM category_synonym WHERE description NOT IN (SELECT description FROM category))");
            inkomendDescription = Database.Instance.Read("SELECT title,category,ean,sku,web_url FROM residue WHERE category NOT IN(SELECT description FROM category_synonym) AND category NOT IN (SELECT description FROM category)");
            inkDesBind = new BindingSource();
            inkDesBind.DataSource = inkomendDescription;
            inkomendGridView.DataSource = inkDesBind;
            inkomendGridView.DataSource = inkomendDescription;

            inkomendGridView.AutoGenerateColumns = true;

            //  Products that fall under Category
            DataTable matchProduct = Database.Instance.Read("SELECT * FROM residue LIMIT 10");
            matProduct = new BindingSource();
            matProduct.DataSource = matchProduct;
            productCategoryGridView.DataSource = matProduct;
            productCategoryGridView.DataSource = matchProduct;

            productCategoryGridView.AutoGenerateColumns = true;
        }

        /// <summary>
        /// This Method matched category from Webshop with category of Borderloop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (categoryGridView.SelectedRows.Count == 0 || inkomendGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("No rows selected. Select a row to match the product to a sugestion.");
                    return;
                }

                Database.Instance.InsertIntoCatSynonyms((int)categoryGridView.SelectedRows[0].Cells["id"].Value, (string)inkomendGridView.SelectedRows[0].Cells["category"].Value, (string)inkomendGridView.SelectedRows[0].Cells["web_url"].Value);
            }

            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.ToString() + "Duplicate");
                MessageBox.Show("Duplicate entry for key");
            }
        }

        /// <summary>
        /// This Method look for a specifically webshop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            DataTable inkomendDescription = Database.Instance.getCategoryInkomend(txt_search.Text);

            inkDesBind.DataSource = inkomendDescription;
            inkomendGridView.DataSource = inkDesBind;
            inkomendGridView.DataSource = inkomendDescription;

            RefreshAll();
        }

        /// <summary>
        /// This Method refreshed all datagridviews
        /// </summary>
        private void RefreshAll()
        {
            this.Refresh();
            categoryGridView.Refresh();
            inkomendGridView.Refresh();
            productCategoryGridView.Refresh();
        }

        /// <summary>
        /// This Method looks for products with a specifically category which is not matched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkedProduct_Click(object sender, EventArgs e)
        {
            matchProduct = Database.Instance.LinkedProductCategory((string)inkomendGridView.SelectedRows[0].Cells["web_url"].Value);

            matProduct.DataSource = matchProduct;
            productCategoryGridView.DataSource = matProduct;
            productCategoryGridView.DataSource = matchProduct;
            productCategoryGridView.AutoResizeColumns();

            RefreshAll();
        }

        /// <summary>
        /// This Method refreshed datagridview Inkomend 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGridInkomend_Click(object sender, EventArgs e)
        {
            // inkomendDescription = Database.Instance.Read("SELECT title,category,ean,sku,web_url FROM residue WHERE category NOT IN(SELECT description FROM category_synonym WHERE description NOT IN (SELECT description FROM category))");
            inkomendDescription = Database.Instance.Read("SELECT title,category,ean,sku,web_url FROM residue WHERE category NOT IN(SELECT description FROM category_synonym) AND category NOT IN (SELECT description FROM category)");
            inkDesBind = new BindingSource();
            inkDesBind.DataSource = inkomendDescription;
            inkomendGridView.DataSource = inkDesBind;
            inkomendGridView.DataSource = inkomendDescription;
        }
    }
}
