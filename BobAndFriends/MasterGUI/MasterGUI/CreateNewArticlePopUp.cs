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

namespace MasterGUI
{
    public partial class CreateNewArticlePopUp : Form
    {
        BetsyModel Context;
        public CreateNewArticlePopUp()
        {
            InitializeComponent();
        }

        public CreateNewArticlePopUp(BetsyModel context)
        {
            InitializeComponent();
            Context = context;
        }

        private void AddArticleButton_Click(object sender, EventArgs e)
        {
            long ean;
            int catId;
            Article newArticle = new Article();
            if (!String.IsNullOrWhiteSpace(EanBox.Text) && long.TryParse(EanBox.Text, out ean)) newArticle.EAN = ean;
            else { MessageBox.Show("EAN is incorrect."); return; }

            if (!String.IsNullOrWhiteSpace(ImageBox.Text)) newArticle.Image = ImageBox.Text;
            else { MessageBox.Show("Image is empty"); return; }

            if (!String.IsNullOrWhiteSpace(CategoryBox.Text))
            {
                if (!int.TryParse(CategoryBox.Text, out catId))
                {
                    catId = Context.category.Any(c => c.description == CategoryBox.Text) ? Context.category.Where(c => c.description == CategoryBox.Text).FirstOrDefault().id : -1;
                    if (catId == -1) { MessageBox.Show("Could not find given category."); return; }
                    newArticle.CategoryId = catId;
                }
            }
            else { MessageBox.Show("Category is empty"); return; }

            if (!String.IsNullOrWhiteSpace(BrandBox.Text)) newArticle.Brand = BrandBox.Text;
            else { MessageBox.Show("Brand is empty"); return; }

            if (!String.IsNullOrWhiteSpace(TitleBox.Text)) newArticle.Title = TitleBox.Text;
            else { MessageBox.Show("Title is empty"); return; }

            if (!String.IsNullOrWhiteSpace(SkuBox.Text)) newArticle.SKU = SkuBox.Text;

            if (Context.ean.Any(ea => ea.ean1 == ean))
            {
                MessageBox.Show("This EAN already exists!");
                return;
            }

            article art = new article
            {
                brand = newArticle.Brand,
                image_loc = newArticle.Image
            };
            art.ean.Add(new BorderSource.BetsyContext.ean { ean1 = long.Parse(EanBox.Text) });
            art.title.Add(new BorderSource.BetsyContext.title { title1 = TitleBox.Text, country_id = 1 });
            if (String.IsNullOrWhiteSpace(SkuBox.Text)) art.sku.Add(new BorderSource.BetsyContext.sku { sku1 = SkuBox.Text });
            art.category = Context.category.Where(c => c.id == catId).ToList();
            Context.article.Add(art);
            Context.SaveChanges();

            MessageBox.Show("Done.");

        }
    }

    public class Article
    {
        public long EAN { get; set; }
        public string SKU { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
    }
}
