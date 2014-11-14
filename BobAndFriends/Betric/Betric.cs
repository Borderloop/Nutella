using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Betric
{
    public partial class Betric : Form
    {
        private List<Category> Categories;
        private bool initialized = false;
        public Betric()
        {            
            InitializeComponent();
            InitializeBetric();
        }

        public void InitializeBetric()
        {
            Database.Instance.Connect("127.0.0.1", "test2", "root", "Hoppa123");
            Categories = new List<Category>();
            DataTable CategoryTable = Database.Instance.Read("Select * from category");
            foreach(DataRow dr in CategoryTable.Rows)
            {
                Categories.Add(new Category { Name = (string)dr["description"], Id = (int)dr["id"], CalledBy = (int)dr["called_by"] });
            }

            CategoryLevel1.DataSource = Categories.Where(c => c.CalledBy == 0).ToList();           
            CategoryLevel1.DisplayMember = "Name";
            CategoryLevel1.ValueMember = "Id";
            CategoryLevel1.SelectedIndex = -1;

            initialized = true;
            
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            if (!initialized) return;
            List<string> queries = new List<string>();
            List<IParam> parameters = new List<IParam>();
            if(TimeFilter.Checked)
            {
                DateTime fromTime = FromTime.Value;
                DateTime untilTime = UntilTime.Value;
                queries.Add("SELECT product_id AS id FROM product_clicks WHERE datetime >= @FROMTIME AND datetime <= @UNTILTIME");
                parameters.Add(new Param<DateTime> { Name = "@FROMTIME", Value = fromTime });
                parameters.Add(new Param<DateTime> { Name = "@UNTILTIME", Value = untilTime });
            }
            if(CategoryFilter.Checked)
            {
                int catId = GetHighestCategoryId();
                List<int> catIds = GetUnderlyingIds(catId);
                string query = "SELECT article_id FROM cat_article WHERE category_id IN (";
                bool first = true;
                foreach(int id in catIds.Distinct())
                {
                    if (first)
                    {
                        query += id;
                        first = false;
                    }
                    else
                    {
                        query += ", " + id;
                    }
                }
                query += ")";
                queries.Add("SELECT product_id AS id FROM product_clicks WHERE id IN (SELECT id FROM product WHERE article_id IN (" + query + "))");
            }
            if(PriceFilter.Checked)
            {
                decimal parsed;
                decimal minPrice = MinimumPrice.Checked ? decimal.TryParse(MinimumPriceText.Text.Replace('.', ','), out parsed) ? parsed : decimal.MinValue : decimal.MinValue;
                decimal maxPrice = MaximumPrice.Checked ? decimal.TryParse(MaximumPriceText.Text.Replace('.', ','), out parsed) ? parsed : decimal.MaxValue : decimal.MaxValue;
                if(minPrice > maxPrice)
                {
                    decimal temp = minPrice;
                    minPrice = maxPrice;
                    maxPrice = temp;

                    MinimumPriceText.Text = minPrice.ToString();
                    MaximumPriceText.Text = maxPrice.ToString();
                    Refresh();
                }
                queries.Add("SELECT product_id AS id FROM product_clicks WHERE price >= @MINPRICE AND price <= @MAXPRICE");
                parameters.Add(new Param<decimal> { Name = "@MINPRICE", Value = minPrice });
                parameters.Add(new Param<decimal> { Name = "@MAXPRICE", Value = maxPrice });
            }
            if(PriceDifferenceFilter.Checked)
            {
                if(AbsolutePriceDifference.Checked)
                {
                    decimal parsed;
                    decimal minPriceDif = MinimumPriceDifference.Checked ? decimal.TryParse(MinimumPriceDifferenceText.Text.Replace('.', ','), out parsed) ? parsed : decimal.MinValue : decimal.MinValue;
                    decimal maxPriceDif = MaximumPriceDifference.Checked ? decimal.TryParse(MaximumPriceDifferenceText.Text.Replace('.', ','), out parsed) ? parsed : decimal.MaxValue : decimal.MaxValue;
                    if (minPriceDif > maxPriceDif)
                    {
                        decimal temp = minPriceDif;
                        minPriceDif = maxPriceDif;
                        maxPriceDif = temp;

                        MinimumPriceDifferenceText.Text = minPriceDif.ToString();
                        MaximumPriceDifferenceText.Text = maxPriceDif.ToString();
                        Refresh();
                    }
                    queries.Add("SELECT product_id AS id WHERE difference >= @MINDIFFERENCE AND difference <= @MAXDIFFERENCE");
                    parameters.Add(new Param<decimal> { Name = "@MINDIFFERENCE", Value = minPriceDif });
                    parameters.Add(new Param<decimal> { Name = "@MAXDIFFERENCE", Value = maxPriceDif });
                } 
                else if(RelativePriceDifference.Checked)
                {

                }
            }
            if(CountryFilter.Checked)
            {

            }
            if(DeliveryCostsFilter.Checked)
            {

            }
        }

        private void CategoryLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try                
            {
                List<Category> L2List = Categories.Where(c => c.CalledBy == ((Category)CategoryLevel1.SelectedItem).Id).ToList();
                CategoryLevel2.DataSource = L2List;
                CategoryLevel2.DisplayMember = "Name";
                CategoryLevel2.ValueMember = "Id";
                CategoryLevel2.SelectedIndex = -1;
                SetHigherComboBoxesInvisible(3);
            }
            catch (NullReferenceException) { }
        }

        private void CategoryLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try
            {
                List<Category> L3List = Categories.Where(c => c.CalledBy == ((Category)CategoryLevel2.SelectedItem).Id).ToList();
                CategoryLevel3.DataSource = L3List;
                CategoryLevel3.DisplayMember = "Name";
                CategoryLevel3.ValueMember = "Id";
                CategoryLevel3.SelectedIndex = -1;
                SetHigherComboBoxesInvisible(4);
            }
            catch (NullReferenceException) { }
        }

        private void CategoryLevel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try
            {
                List<Category> L4List = Categories.Where(c => c.CalledBy == ((Category)CategoryLevel3.SelectedItem).Id).ToList();
                CategoryLevel4.DataSource = L4List;
                CategoryLevel4.DisplayMember = "Name";
                CategoryLevel4.ValueMember = "Id";
                CategoryLevel4.SelectedIndex = -1;
                SetHigherComboBoxesInvisible(5);
            }
            catch (NullReferenceException) { }
        }

        private void CategoryLevel4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try
            {              
                List<Category> L5List = Categories.Where(c => c.CalledBy == ((Category)CategoryLevel4.SelectedItem).Id).ToList();
                CategoryLevel5.DataSource = L5List;
                CategoryLevel5.DisplayMember = "Name";
                CategoryLevel5.ValueMember = "Id";
                CategoryLevel5.SelectedIndex = -1;
            }
            catch (NullReferenceException) { }
        }

        private void SetHigherComboBoxesInvisible(int start)
        {
            if (!initialized) return;
            CategoryLevel2.Visible = start > 2 && CategoryLevel2.Items.Count > 0;
            CategoryLevel3.Visible = start > 3 && CategoryLevel3.Items.Count > 0;
            CategoryLevel4.Visible = start > 4 && CategoryLevel4.Items.Count > 0;
            CategoryLevel5.Visible = start > 5 && CategoryLevel5.Items.Count > 0;
            Refresh();
        }

        private void AbsolutePriceDifference_CheckedChanged(object sender, EventArgs e)
        {
            MaximumDifferenceEuroSign.Visible = AbsolutePriceDifference.Checked;
            MinimumDifferenceEuroSign.Visible = AbsolutePriceDifference.Checked;
            MaximumPercentageSignLabel.Visible = !AbsolutePriceDifference.Checked;
            MinimumPercentageSignLabel.Visible = !AbsolutePriceDifference.Checked;

            if (AbsolutePriceDifference.Checked)
            {
                MaximumPriceDifferenceText.TextAlign = HorizontalAlignment.Left;
                MinimumPriceDifferenceText.TextAlign = HorizontalAlignment.Left;
            }
            else
            {
                MaximumPriceDifferenceText.TextAlign = HorizontalAlignment.Right;
                MinimumPriceDifferenceText.TextAlign = HorizontalAlignment.Right;
            }
        }

        private void RelativePriceDifference_CheckedChanged(object sender, EventArgs e)
        {
            MaximumDifferenceEuroSign.Visible = !RelativePriceDifference.Checked;
            MinimumDifferenceEuroSign.Visible = !RelativePriceDifference.Checked;
            MaximumPercentageSignLabel.Visible = RelativePriceDifference.Checked;
            MinimumPercentageSignLabel.Visible = RelativePriceDifference.Checked;
        }

        private int GetHighestCategoryId()
        {
            int result = 0;
            if(CategoryLevel1.Visible && CategoryLevel1.SelectedIndex != -1)
            {
                result = ((Category)CategoryLevel1.SelectedItem).Id;
            }
            if (CategoryLevel2.Visible && CategoryLevel2.SelectedIndex != -1)
            {
                result = ((Category)CategoryLevel2.SelectedItem).Id;
            }
            if (CategoryLevel3.Visible && CategoryLevel3.SelectedIndex != -1)
            {
                result = ((Category)CategoryLevel3.SelectedItem).Id;
            }
            if (CategoryLevel4.Visible && CategoryLevel4.SelectedIndex != -1)
            {
                result = ((Category)CategoryLevel4.SelectedItem).Id;
            }
            if (CategoryLevel5.Visible && CategoryLevel5.SelectedIndex != -1)
            {
                result = ((Category)CategoryLevel5.SelectedItem).Id;
            }
            return result;
        }

        private List<int> GetUnderlyingIds(int catId)
        {
            List<int> underlyingIds = new List<int>();
            DataTable table = Database.Instance.Read("SELECT id FROM category WHERE called_by = " + catId);
            if (table.Rows.Count == 0) return null;
            foreach(DataRow row in table.Rows)
            {
                underlyingIds.Add((int)row["id"]);
                List<int> addition = GetUnderlyingIds((int)row["id"]);
                if(addition == null) continue;
                underlyingIds.AddRange(addition);
            }
            underlyingIds.Add(catId);
            return underlyingIds;
        }

        private void FromTime_ValueChanged(object sender, EventArgs e)
        {
            if(FromTime.Value > UntilTime.Value)
            {
                UntilTime.Value = FromTime.Value;
                Refresh();
            }
        }

        private void UntilTime_ValueChanged(object sender, EventArgs e)
        {
            if (FromTime.Value > UntilTime.Value)
            {
                FromTime.Value = UntilTime.Value;
                Refresh();
            }
        }
    }
}
