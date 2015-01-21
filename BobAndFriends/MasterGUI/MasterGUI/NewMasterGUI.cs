using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BorderSource.BetsyContext;
using BorderSource.Property;
using MySql.Data.MySqlClient;
using System.Data.Entity.Core.EntityClient;

namespace MasterGUI
{
    public partial class NewMasterGUI : Form
    {
        public static BetsyModel Context;

        List<ean> Eans;
        List<article> Articles;
        List<title> Titles;
        List<product> Products;
        List<sku> Skus;
        List<string> VisibleArticleColumns;
        List<string> VisibleProductColumns;
        List<string> VisibleEanColumns;
        List<string> VisibleTitleColumns;
        List<string> VisibleSkuColumns;
        bool JustSearched = false;

        public NewMasterGUI()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Eans = new List<ean>();
            Articles = new List<article>();
            Titles = new List<title>();
            Products = new List<product>();
            Skus = new List<sku>();

            MySqlConnectionStringBuilder providerConnStrBuilder = new MySqlConnectionStringBuilder();
            providerConnStrBuilder.AllowUserVariables = true;
            providerConnStrBuilder.AllowZeroDateTime = true;
            providerConnStrBuilder.ConvertZeroDateTime = true;
            providerConnStrBuilder.MaximumPoolSize = 125;
            providerConnStrBuilder.Pooling = true;
            providerConnStrBuilder.Database = Settings.PropertyList["db_name"].GetValue<string>();
            providerConnStrBuilder.Password = Settings.PropertyList["db_password"].GetValue<string>();
            providerConnStrBuilder.Server = Settings.PropertyList["db_source"].GetValue<string>();
            providerConnStrBuilder.UserID = Settings.PropertyList["db_userid"].GetValue<string>();
            providerConnStrBuilder.Port = (uint)Settings.PropertyList["db_port"].GetValue<int>();

            EntityConnectionStringBuilder entityConnStrBuilder = new EntityConnectionStringBuilder();
            entityConnStrBuilder.Provider = "MySql.Data.MySqlClient";
            entityConnStrBuilder.ProviderConnectionString = providerConnStrBuilder.ToString();
            entityConnStrBuilder.Metadata = "res://*/BetsyContext.BetsyModel.csdl|res://*/BetsyContext.BetsyModel.ssdl|res://*/BetsyContext.BetsyModel.msl";

            Context = new BetsyModel(entityConnStrBuilder.ConnectionString);

            Context.Configuration.LazyLoadingEnabled = false;
            CategoryGridView.DataSource = Context.category.OrderBy(c => c.description).ToList();
            foreach (DataGridViewColumn column in CategoryGridView.Columns)
            {
                if (column.Name == "description") continue;
                column.Visible = false;
            }

            WebshopGridView.DataSource = Context.webshop.OrderBy(w => w.name).ToList();
            List<string> visibleWebshopColumns = new List<string>() {
                    "name", "country_id", "url", "shipping_cost"
                };
            foreach (DataGridViewColumn column in WebshopGridView.Columns)
            {
                if (visibleWebshopColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }

            VisibleArticleColumns = new List<string>()
            {
                "brand", "image_loc"
            };
            VisibleProductColumns = new List<string>()
            {
                "ship_time", "ship_cost", "price", "webshop_url", "direct_link", "last_modified", "valid_until", "affiliate_name", "affiliate_unique_id"
            };
            VisibleEanColumns = new List<string>()
            {
                "ean1"
            };
            VisibleTitleColumns = new List<string>() {
                "title1", "country_id"
            };
            VisibleSkuColumns = new List<string>()
            {
                "sku1"
            };
            this.Refresh();
        }

        private void AddNewArticleButton_Click(object sender, EventArgs e)
        {
            CreateNewArticlePopUp popup = new CreateNewArticlePopUp(Context);
            popup.Show();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            int count = 0;
            ClearDataGridViewsAndLists();
            if (!String.IsNullOrWhiteSpace(TitleSearchBox.Text))
            {
                count++;
                var query = Context.article.Where(a => a.title.Any(t => t.title1.Contains(TitleSearchBox.Text.Trim())));
                Articles.AddRange(query.AsEnumerable());
            }
            if (!String.IsNullOrWhiteSpace(WebshopSearchBox.Text))
            {
                count++;
                var query = Context.article.Where(a => a.product.Any(p => p.webshop_url.Contains(WebshopSearchBox.Text.Trim())));
                Articles.AddRange(query.AsEnumerable());
            }
            if (!String.IsNullOrWhiteSpace(SkuSearchBox.Text))
            {
                count++;
                var query = Context.article.Where(a => a.sku.Any(s => s.sku1.Contains(SkuSearchBox.Text.Trim())));
                Articles.AddRange(query.AsEnumerable());
            }
            if (!String.IsNullOrWhiteSpace(CategorySearchBox.Text))
            {
                count++;
                var query = Context.article.Where(a => a.category.Any(c => c.description.Contains(CategorySearchBox.Text.Trim())));
                Articles.AddRange(query.AsEnumerable());
            }

            List<int> CorrectArticleIds = Articles.GroupBy(a => a.id).Where(x => x.Count() == count).Select(v => v.Key).ToList();
            List<article> CorrectArticles = new List<article>();
            Articles.ForEach(x => 
            {
                if (CorrectArticleIds.Contains(x.id))
                {
                    CorrectArticles.Add(x);
                    CorrectArticleIds.Remove(x.id);
                }
            });
            EanGridView.DataSource = Eans;
            ArticleGridView.DataSource = CorrectArticles;
            ProductGridView.DataSource = Products;
            TitleGridView.DataSource = Titles;
            SkuGridView.DataSource = Skus;
            SetOnlyAllowedColumnsToVisible();
            JustSearched = true;
        }

        private void EanSearchButton_Click(object sender, EventArgs e)
        {
            long ean;
            if (!String.IsNullOrWhiteSpace(EanSearchBox.Text) && long.TryParse(EanSearchBox.Text, out ean))
            {
                Eans = Context.ean.Where(v => v.ean1 == ean).ToList();
                if (!Eans.Any()) { MessageBox.Show("Could not find EAN " + ean); return; }
                int aId = Eans.First().article_id;
                Context.Configuration.LazyLoadingEnabled = true;
                category cat = Context.article.Where(a => a.id == aId).FirstOrDefault().category.FirstOrDefault();
                Context.Configuration.LazyLoadingEnabled = false;
                if (cat != null)
                {
                    foreach (DataGridViewRow row in CategoryGridView.Rows)
                    {
                        if ((int)row.Cells["id"].Value == cat.id)
                        {
                            CategoryGridView.CurrentCell = row.Cells[1];
                            break;
                        }
                    }
                }
                Articles = Context.article.Where(v => v.id == aId).ToList();
                Products = Context.product.Where(v => v.article_id == aId).ToList();
                Titles = Context.title.Where(v => v.article_id == aId).ToList();
                Skus = Context.sku.Where(v => v.article_id == aId).ToList();
                Eans = Context.ean.Where(ean1 => ean1.article_id == aId).ToList();
            }

            EanGridView.DataSource = Eans;
            ArticleGridView.DataSource = Articles;
            ProductGridView.DataSource = Products;
            TitleGridView.DataSource = Titles;
            SkuGridView.DataSource = Skus;
            SetOnlyAllowedColumnsToVisible();
            this.Refresh();
            JustSearched = true;
        }

        private void ArticleGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (ArticleGridView.SelectedRows.Count == 0) return;
            int id = (int) ArticleGridView.SelectedRows[0].Cells[0].Value;
            Context.Configuration.LazyLoadingEnabled = true;
            category cat = Context.article.Where(a => a.id == id).FirstOrDefault().category.FirstOrDefault();
            Context.Configuration.LazyLoadingEnabled = false;
            if (cat != null)
            {
                foreach (DataGridViewRow row in CategoryGridView.Rows)
                {
                    if ((int)row.Cells["id"].Value == cat.id)
                    {
                        CategoryGridView.CurrentCell = row.Cells[1];
                        break;
                    }
                }
            }
            Products = Context.product.Where(p => p.article_id == id).ToList();
            Eans = Context.ean.Where(ean => ean.article_id == id).ToList();
            Titles = Context.title.Where(t => t.article_id == id).ToList();
            Skus = Context.sku.Where(s => s.article_id == id).ToList();
            SkuGridView.DataSource = Skus;
            TitleGridView.DataSource = Titles;
            EanGridView.DataSource = Eans;
            ProductGridView.DataSource = Products;            
            SetOnlyAllowedColumnsToVisible();
            this.Refresh();
        }

        private void ClearDataGridViewsAndLists()
        {
            EanGridView.DataSource = null;
            ArticleGridView.DataSource = null;
            ProductGridView.DataSource = null;
            TitleGridView.DataSource = null;
            SkuGridView.DataSource = null;
            Articles.Clear();
            Eans.Clear();
            Products.Clear();
            Titles.Clear();
            Skus.Clear();
        }

        private void SetOnlyAllowedColumnsToVisible()
        {
            foreach (DataGridViewColumn column in ArticleGridView.Columns)
            {
                if (VisibleArticleColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }
            foreach (DataGridViewColumn column in EanGridView.Columns)
            {
                if (VisibleEanColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }
            foreach (DataGridViewColumn column in ProductGridView.Columns)
            {
                if (VisibleProductColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }
            foreach (DataGridViewColumn column in TitleGridView.Columns)
            {
                if (VisibleTitleColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }
            foreach (DataGridViewColumn column in SkuGridView.Columns)
            {
                if (VisibleSkuColumns.Contains(column.Name)) continue;
                column.Visible = false;
            }
        }

        private void WebshopGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateArticleList();
        }

        private void CategoryGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateArticleList();
        }

        private void UpdateArticleList()
        {
            if (JustSearched) return;
            ClearDataGridViewsAndLists();
            int count = 0;
            if (WebshopGridView.SelectedRows.Count != 0)
            {
                count++;
                string webshop = (string)WebshopGridView.SelectedRows[0].Cells[2].Value;
                Articles.AddRange(Context.article.Where(a => a.product.Any(p => p.webshop_url.Contains(webshop.Trim()))).AsEnumerable());
            }
            if (CategoryGridView.SelectedRows.Count != 0)
            {
                count++;
                int catId = (int)CategoryGridView.SelectedRows[0].Cells[0].Value;
                Articles.AddRange(Context.article.Where(a => a.category.Any(c => c.id == catId)).AsEnumerable());
            }
            List<int> CorrectArticleIds = Articles.GroupBy(a => a.id).Where(x => x.Count() == count).Select(v => v.Key).ToList();
            List<article> CorrectArticles = new List<article>();
            Articles.ForEach(x =>
            {
                if (CorrectArticleIds.Contains(x.id))
                {
                    CorrectArticles.Add(x);
                    CorrectArticleIds.Remove(x.id);
                }
            });

            ArticleGridView.DataSource = CorrectArticles;
            SetOnlyAllowedColumnsToVisible();
            this.Refresh();
        }

        private void CategoryGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int catId = (int) CategoryGridView.Rows[e.RowIndex].Cells[0].Value;
            var original = Context.category.Where(c => c.id == catId).FirstOrDefault();
            if(original != null)
            {
                original.description = (string)CategoryGridView.Rows[e.RowIndex].Cells["description"].Value;
            }
            Context.SaveChanges();
        }

        private void AddNewCategoryButton_Click(object sender, EventArgs e)
        {
            CreateNewCategoryPopUp popup = new CreateNewCategoryPopUp(Context);
            popup.Show();
        }

        private void CategoryGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            JustSearched = false;
        }

        private void WebshopGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            JustSearched = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelfAddedProductsForm form = new SelfAddedProductsForm();
            form.Show();
        }

        private void AddNewEanButton_Click(object sender, EventArgs e)
        {
            if (ArticleGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("No article selected.");
            }
            else
            {
                int aid = (int)ArticleGridView.SelectedRows[0].Cells["id"].Value;
                AddNewEanPopUp popup = new AddNewEanPopUp(Context, aid);
                popup.Show();
                popup.FormClosed += NewEanFormClosed;              
            }
        }

        public void NewEanFormClosed(object sender, EventArgs e)
        {
            int aid = (int)ArticleGridView.SelectedRows[0].Cells["id"].Value;
            EanGridView.DataSource = Context.ean.Where(ean => ean.article_id == aid).ToList();
        }
    }




    public class ArticleEqualityComparer : EqualityComparer<article>
    {
        public override bool Equals(article a1, article a2)
        {
            if (a1 == null || a2 == null) return false;
            return a1.id == a2.id;
        }

        public override int GetHashCode(article a)
        {
            int hCode = a.id ^ a.brand.Length;
            return hCode.GetHashCode();
        }
    }
}
