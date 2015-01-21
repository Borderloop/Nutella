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
    public partial class AddNewEanPopUp : Form
    {
        BetsyModel Context;
        int ArticleId;
        public AddNewEanPopUp()
        {
            InitializeComponent();
        }

        public AddNewEanPopUp(BetsyModel context, int aid)
        {
            InitializeComponent();
            Context = context;
            ArticleId = aid;
        }

        private void AddNewEANButton_Click(object sender, EventArgs e)
        {
            Refresh();
            long newEAN;
            if(!long.TryParse(NewEANTextBox.Text, out newEAN))
            {
                MessageBox.Show("Incorrect EAN");
                return;
            }
            Context.ean.Add(new ean
            {
                ean1 = newEAN,
                article_id = ArticleId
            });
            Context.SaveChanges();
            this.Close();
        }
    }
}
