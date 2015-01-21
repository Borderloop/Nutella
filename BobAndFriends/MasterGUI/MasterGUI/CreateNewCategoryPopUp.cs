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
    public partial class CreateNewCategoryPopUp : Form
    {
        BetsyModel Context;
        public CreateNewCategoryPopUp()
        {
            InitializeComponent();
        }

        public CreateNewCategoryPopUp(BetsyModel context)
        {
            InitializeComponent();
            Context = context;
        }


        private void AddNewCategoryButton_Click(object sender, EventArgs e)
        {
            category newCat = new category();
            if (!String.IsNullOrWhiteSpace(NewCategoryBox.Text)) newCat.description = NewCategoryBox.Text;
            else { MessageBox.Show("Category is empty"); return; }

            if (!String.IsNullOrWhiteSpace(AboveCategory.Text))
            {
                category existingCategory = Context.category.Where(c => c.description == AboveCategory.Text.Trim()).FirstOrDefault();
                if (existingCategory == null)
                {
                    MessageBox.Show("Could not find above category.");
                    return;
                }
                newCat.menulevel = (sbyte?)(existingCategory.menulevel + 1);
                newCat.called_by = existingCategory.id;
            }
            else
            {
                newCat.menulevel = 1;
                newCat.called_by = 0;
            }
            Context.category.Add(newCat);
            Context.SaveChanges();

            MessageBox.Show("Done.");
        }
    }
}
