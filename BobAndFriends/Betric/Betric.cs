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
        private string cat1Text = "";
        private string cat2Text = "";
        private string cat3Text = "";
        private string cat4Text = "";
        private string cat5Text = "";

        public Betric()
        {            
            InitializeComponent();
            InitializeBetric();
        }

        public void InitializeBetric()
        {
            string db = Microsoft.VisualBasic.Interaction.InputBox("Enter database name", "", "borderloop");
            string pw = Microsoft.VisualBasic.Interaction.InputBox("Enter database password", "", "");
            Database.Instance.Connect("127.0.0.1", db, "root", pw);
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
                DateFilterLabel.Text = fromTime.ToShortDateString() + " tot " + untilTime.ToShortDateString();
            } else {
                DateFilterLabel.Text = "Geen";
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
                queries.Add("SELECT product_id AS id FROM product_clicks WHERE product_id IN (SELECT id FROM product WHERE article_id IN (" + query + "))");

                string categoryPath = cat1Text;
                if (cat2Text != "") categoryPath += " > " + cat2Text;
                if (cat3Text != "") categoryPath += " > " + cat3Text;
                if (cat4Text != "") categoryPath += " > " + cat4Text;
                if (cat5Text != "") categoryPath += " > " + cat5Text;
                CategoryFilterLabel.Text = categoryPath;

            } else {
                 CategoryFilterLabel.Text = "Geen";
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
                string priceFilterText = "";
                if (minPrice == decimal.MinValue) priceFilterText = "Tot €" + maxPrice;
                if (maxPrice == decimal.MaxValue) priceFilterText = "Vanaf €" + minPrice;
                if(minPrice == decimal.MinValue && maxPrice == decimal.MaxValue) priceFilterText = "Geen";
                PriceFilterLabel.Text = priceFilterText == "" ? "€" + minPrice + " tot €" + maxPrice : priceFilterText;
            } else {
                PriceFilterLabel.Text = "Geen";
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
                    queries.Add("SELECT product_id AS id FROM country_price_differences WHERE difference >= @MINDIFFERENCE AND difference <= @MAXDIFFERENCE");
                    parameters.Add(new Param<decimal> { Name = "@MINDIFFERENCE", Value = minPriceDif });
                    parameters.Add(new Param<decimal> { Name = "@MAXDIFFERENCE", Value = maxPriceDif });
                    string priceDifFilterText = "";
                    if (minPriceDif == decimal.MinValue) priceDifFilterText = "Tot €" + maxPriceDif;
                    if (maxPriceDif == decimal.MaxValue) priceDifFilterText = "Vanaf €" + minPriceDif;
                    if (minPriceDif == decimal.MinValue && maxPriceDif == decimal.MaxValue) priceDifFilterText = "Geen";
                    PriceDifferenceFilterLabel.Text = priceDifFilterText == "" ? "€" + minPriceDif + " tot €" + maxPriceDif : priceDifFilterText;
                } 
                else if(RelativePriceDifference.Checked)
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
                    queries.Add("SELECT product_id AS id FROM country_price_differences WHERE difference_percentage >= @MINDIFFERENCEPERC AND difference <= @MAXDIFFERENCEPERC");
                    parameters.Add(new Param<decimal> { Name = "@MINDIFFERENCEPERC", Value = minPriceDif });
                    parameters.Add(new Param<decimal> { Name = "@MAXDIFFERENCEPERC", Value = maxPriceDif });
                    string priceDifFilterText = "";
                    if (minPriceDif == decimal.MinValue) priceDifFilterText = "Tot " + maxPriceDif + "%";
                    if (maxPriceDif == decimal.MaxValue) priceDifFilterText = "Vanaf " + minPriceDif + "%";
                    if (minPriceDif == decimal.MinValue && maxPriceDif == decimal.MaxValue) priceDifFilterText = "Geen";
                    PriceDifferenceFilterLabel.Text = priceDifFilterText == "" ? minPriceDif + "% tot " + maxPriceDif + "%" : priceDifFilterText;
                }
            } else {
                PriceDifferenceFilterLabel.Text = "Geen";
            }
            if(CountryFilter.Checked)
            {
                string sign = "";
                if (DomesticCountryOnly.Checked) sign = "=";
                else if (ForeignCountryOnly.Checked) sign = "!=";
                if (!DomesticCountryOnly.Checked && !ForeignCountryOnly.Checked)
                {
                    MessageBox.Show("Kies binnenland of buitenland of zet landenfilter uit.");
                    return;
                }
                queries.Add("SELECT product_id AS id FROM product_clicks " 
                            + "WHERE position = 1 AND product_id IN " 
                                + "(SELECT product.id FROM product " 
                                + "INNER JOIN webshop ON webshop.url = product.webshop_url " 
                                + "INNER JOIN country ON country.id = webshop.country_id WHERE country.id " + sign + " 1)"
                        );
                CountryFilterLabel.Text = sign == "" ? "Geen" : sign == "=" ? "Alleen binnenland" : "Alleen buitenland";
            }
            else
            {
                CountryFilterLabel.Text = "Geen";
            }

            Refresh();

            
            StringBuilder productQueryBuilder = new StringBuilder();
            bool firstQuery = true;
            foreach(string query in queries)
            {
                if (firstQuery)
                {
                    productQueryBuilder.Append(query);
                    firstQuery = false;
                }
                else
                    productQueryBuilder.Append(" UNION ALL (" + query + ")");    
            }

            string finalQuery = "SELECT product_clicks.clicks, product_clicks.position, country_price_differences.difference, country_price_differences.difference_percentage, webshop.country_id "
                                + "FROM product "
                                + "INNER JOIN country_price_differences ON country_price_differences.product_id = product.id "
                                + "INNER JOIN product_clicks ON product_clicks.product_id = product.id "
                                + "INNER JOIN webshop ON webshop.url = product.webshop_url "
                                + "WHERE product_clicks.product_id IN (" + productQueryBuilder.ToString() + ") " 
                                + "GROUP BY product_clicks.product_id HAVING COUNT(*) > " + queries.Count;
            DataTable data = Database.Instance.Read(finalQuery, parameters);
            if (data == null || data.Rows.Count == 0)
            {
                MessageBox.Show("Geen producten voldoen aan deze criteria. Pas filters aan.");
                return;
            }
            CalculateMetrics(data);
        }

        private void CategoryLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try                
            {
                cat1Text = ((Category)CategoryLevel1.SelectedItem).Name;
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
                cat2Text = ((Category)CategoryLevel2.SelectedItem).Name;
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
                cat3Text = ((Category)CategoryLevel3.SelectedItem).Name;
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
                cat4Text = ((Category)CategoryLevel4.SelectedItem).Name;
                List<Category> L5List = Categories.Where(c => c.CalledBy == ((Category)CategoryLevel4.SelectedItem).Id).ToList();
                CategoryLevel5.DataSource = L5List;
                CategoryLevel5.DisplayMember = "Name";
                CategoryLevel5.ValueMember = "Id";
                CategoryLevel5.SelectedIndex = -1;
            }
            catch (NullReferenceException) { }
        }

        private void CategoryLevel5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            try
            {
                cat5Text = ((Category)CategoryLevel5.SelectedItem).Name;
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

            if (!CategoryLevel2.Visible || CategoryLevel2.SelectedIndex == -1) cat2Text = "";
            if (!CategoryLevel3.Visible || CategoryLevel3.SelectedIndex == -1) cat3Text = "";
            if (!CategoryLevel4.Visible || CategoryLevel4.SelectedIndex == -1) cat4Text = "";
            if (!CategoryLevel5.Visible || CategoryLevel5.SelectedIndex == -1) cat5Text = "";

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
            if (table.Rows.Count != 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    underlyingIds.Add((int)row["id"]);
                    List<int> addition = GetUnderlyingIds((int)row["id"]);
                    if (addition == null) continue;
                    underlyingIds.AddRange(addition);
                }
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
       
        private void CalculateMetrics(DataTable data)
        {
            IEnumerable<DataRow> rowCollection = data.Rows.OfType<DataRow>();
            int DomesticClickAmount = rowCollection.Where(d => (short)d["country_id"] == 1).ToList().Count;
            int ForeignClickAmount = rowCollection.Where(d => (short)d["country_id"] != 1).ToList().Count;
            int TotalClickAmount = DomesticClickAmount + ForeignClickAmount;
            DomesticClickAmountLabel.Text = DomesticClickAmount.ToString();
            ForeignClickAmountLabel.Text = ForeignClickAmount.ToString();
            TotalClickAmountLabel.Text = TotalClickAmount.ToString();

            DomesticClickAmountPercentageLabel.Text = (((double)DomesticClickAmount / (double)TotalClickAmount) * (double)100).ToString();
            ForeignClickAmountPercenageLabel.Text = (((double)ForeignClickAmount / (double)TotalClickAmount) * (double)100).ToString();

            int PositionOneClickAmount = rowCollection.Where(d => (int)d["position"] == 1).ToList().Count;
            int PositionTwoClickAmount = rowCollection.Where(d => (int)d["position"] == 2).ToList().Count;
            int PositionThreeClickAmount = rowCollection.Where(d => (int)d["position"] == 3).ToList().Count;
            int PositionFourClickAmount = rowCollection.Where(d => (int)d["position"] == 4).ToList().Count;
            int PositionFiveClickAmount = rowCollection.Where(d => (int)d["position"] == 5).ToList().Count;
            int RemainingClickAmount = rowCollection.Where(d => (int)d["position"] != 1 && (int)d["position"] != 2 && (int)d["position"] != 3 && (int)d["position"] != 4 && (int)d["position"] != 5).ToList().Count;

            int TotalPositionClickAmount = PositionOneClickAmount + PositionTwoClickAmount + PositionThreeClickAmount + PositionFourClickAmount + PositionFiveClickAmount;

            PositionOneClickAmountLabel.Text = PositionOneClickAmount.ToString();
            PositionTwoClickAmountLabel.Text = PositionTwoClickAmount.ToString();
            PositionThreeClickAmountLabel.Text = PositionThreeClickAmount.ToString();
            PositionFourClickAmountLabel.Text = PositionFourClickAmount.ToString();
            PositionFiveClickAmountLabel.Text = PositionFiveClickAmount.ToString();
            PositionRemainingClickAmountLabel.Text = RemainingClickAmount.ToString();
            PositionClickAmountTotalLabel.Text = TotalPositionClickAmount.ToString();

            PositionOnePercentageAmountLabel.Text = (((double)PositionOneClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();
            PositionTwoPercentageAmountLabel.Text = (((double)PositionTwoClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();
            PositionThreePercentageAmountLabel.Text = (((double)PositionThreeClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();
            PositionFourPercentageAmountLabel.Text = (((double)PositionFourClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();
            PositionFivePercentageAmountLabel.Text = (((double)PositionFiveClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();
            PositionRemainingPercentageAmountLabel.Text = (((double)RemainingClickAmount / (double)TotalPositionClickAmount) * (double)100).ToString();

            int DomesticNumberOneClickAmount = rowCollection.Where(d => (int)d["position"] == 1 && (int)d["country"] == 1).ToList().Count;
            int ForeignNumberOneClickAmount = rowCollection.Where(d => (int)d["position"] == 1 && (int)d["country"] != 1).ToList().Count;
            int TotalNumberOneClickAmount = DomesticNumberOneClickAmount + ForeignNumberOneClickAmount;

            DomesticNumberOneClickAmountLabel.Text = DomesticNumberOneClickAmount.ToString();
            ForeignNumberOneClickAmountLabel.Text = ForeignNumberOneClickAmount.ToString();
            NumberOneClickAmountTotalLabel.Text = TotalNumberOneClickAmount.ToString();

            DomesticNumberOneClickPercentageAmountLabel.Text = (((double)DomesticNumberOneClickAmount / (double)TotalNumberOneClickAmount) * (double)100).ToString();
            ForeignNumberOneClickPercentageAmountLabel.Text = (((double)ForeignNumberOneClickAmount / (double)TotalNumberOneClickAmount) * (double)100).ToString();

            decimal AveragePriceDifference = 0;
            decimal AveragePriceDifferencePercentage = 0;
            int count = 0;
            foreach(DataRow dr in rowCollection)
            {
                count++;
                AveragePriceDifference = (AveragePriceDifference * (count - 1)) / count + ((decimal)dr["difference"]) / count;
                AveragePriceDifferencePercentage = (AveragePriceDifferencePercentage * (count - 1)) / count + ((decimal)dr["difference_percentage"]) / count;
            }

            PriceDifferenceLabel.Text =  "€" + Math.Round(AveragePriceDifference, 2);
            PriceDifferencePercentageLabel.Text = Math.Round(AveragePriceDifferencePercentage, 2) + "%";
        }
    }
}
