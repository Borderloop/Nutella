using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvilBatcher
{
    public partial class EvilBatcher : Form
    {
        private DataTable borderloopCategories;
        private DataTable evilCategories;

        private BindingSource borderBind;
        private BindingSource evilBind;

        private Stack<int> evilCalls;
        private Stack<int> borderCalls;

        private int currentBorderLevel;
        private int currentEvilLevel;

        public EvilBatcher()
        {
            InitializeComponent();
            InitializeProperties();
            InitializeDataGridViews();
        }

        private void InitializeProperties()
        {
            borderloopCategories = new DataTable();
            evilCategories = new DataTable();

            borderBind = new BindingSource();
            evilBind = new BindingSource();         
            borderGridView.AutoGenerateColumns = true;
            evilGridView.AutoGenerateColumns = true;

            evilCalls = new Stack<int>();
            borderCalls = new Stack<int>();
        }

        private void InitializeDataGridViews()
        {
            borderloopCategories = Database.Instance.GetBorderTopLayer();
            evilCategories = Database.Instance.GetEvilTopLayer();

            borderBind.DataSource = borderloopCategories;
            borderGridView.DataSource = borderBind;
            borderGridView.DataSource = borderloopCategories;

            evilBind.DataSource = evilCategories;
            evilGridView.DataSource = evilBind;
            evilGridView.DataSource = evilCategories;

            this.evil_goDownButton.Click += new System.EventHandler(this.SizeAllColumns);
            this.evil_goDownButton.Click += new System.EventHandler(this.evil_goDownButton_Click);
            this.evil_goUpButton.Click += new System.EventHandler(this.SizeAllColumns);
            this.evil_goUpButton.Click += new System.EventHandler(this.evil_goUpButton_Click);
            this.matchButton.Click += new System.EventHandler(this.SizeAllColumns);
            this.matchButton.Click += new System.EventHandler(this.matchButton_Click);
            this.border_goUpButton.Click += new System.EventHandler(this.SizeAllColumns);
            this.border_goUpButton.Click += new System.EventHandler(this.border_goUpButton_Click);
            this.border_goDownButton.Click += new System.EventHandler(this.SizeAllColumns);
            this.border_goDownButton.Click += new System.EventHandler(this.border_goDownButton_Click);

            RefreshAll();
        }

        private void evil_goDownButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = evilGridView.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).FirstOrDefault();
            if(selectedRow == null)
            {
                MessageBox.Show("No row selected. Please select a row.");
                return;
            }
            int call = (int)selectedRow.Cells["id"].Value;
            evilCategories = Database.Instance.GetEvilChildrenFromId(call);

            evilCalls.Push(call);
            currentEvilLevel = call;
            RefreshAll();
        }

        private void evil_goUpButton_Click(object sender, EventArgs e)
        {
            if (evilCalls.Count > 0)
                if (currentEvilLevel == evilCalls.Peek())
                    evilCalls.Pop();

            evilCategories = (evilCalls.Count > 0) ? Database.Instance.GetEvilChildrenFromId(evilCalls.Pop()) : Database.Instance.GetEvilTopLayer();
            RefreshAll();
        }

        private void matchButton_Click(object sender, EventArgs e)
        {
            UpdateState("Saving match...");

            DataGridViewRow evilSelectedRow = evilGridView.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).FirstOrDefault();
            DataGridViewRow borderloopSelectedRow = borderGridView.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).FirstOrDefault();



            if (evilSelectedRow == null)
            {
                MessageBox.Show("No EvilBoris category row selected. Please select a row.");
                return;
            }
            if (borderloopSelectedRow == null)
            {
                MessageBox.Show("No Borderloop category row selected. Please select a row.");
                return;
            }

            int catId = (int)borderloopSelectedRow.Cells["id"].Value;
            int evilId = (int)evilSelectedRow.Cells["id"].Value;
            Database.Instance.InsertIntoCatSynonyms(catId, (string)evilSelectedRow.Cells["description"].Value);
            AddAllChildrenToCatSyn(evilId, catId);

            UpdateState("Removing matched categories from database...");

            DeleteAllChildrenFromCategoryTemp(evilId);

            UpdateState("idle");
        }

        private void AddAllChildrenToCatSyn(int id, int catId)
        {
            DataTable children = Database.Instance.GetEvilChildrenFromId(id);
            if (children == null) return;
            foreach(DataRow row in children.Rows)
            {              
                AddAllChildrenToCatSyn((int)row["id"], catId);
                Database.Instance.InsertIntoCatSynonyms(catId, (string)row["description"]);
            }
        }

        private void DeleteAllChildrenFromCategoryTemp(int id)
        {
            DataTable children = Database.Instance.GetEvilChildrenFromId(id);
            if (children == null) return;
            foreach (DataRow row in children.Rows)
            {              
                DeleteAllChildrenFromCategoryTemp((int)row["id"]);
                Database.Instance.DeleteFromCategoryTemp((int)row["id"]);
            }
            Database.Instance.DeleteFromCategoryTemp(id);
        }
       
        private void border_goUpButton_Click(object sender, EventArgs e)
        {
            if (borderCalls.Count > 0)
                if (currentBorderLevel == borderCalls.Peek()) 
                    borderCalls.Pop();

            borderloopCategories = (borderCalls.Count > 0) ? Database.Instance.GetBorderChildrenFromId(borderCalls.Pop()) : Database.Instance.GetBorderTopLayer();

            

            RefreshAll();
        }

        private void border_goDownButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = borderGridView.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).FirstOrDefault();
            if (selectedRow == null)
            {
                MessageBox.Show("No row selected. Please select a row.");
                return;
            }
            int call = (int)selectedRow.Cells["id"].Value;
            borderloopCategories = Database.Instance.GetBorderChildrenFromId(call);

            borderCalls.Push(call);
            currentBorderLevel = call;
            RefreshAll();
        }

        private void SizeAllColumns(Object sender, EventArgs e)
        {
            borderGridView.AutoResizeColumns(
                DataGridViewAutoSizeColumnsMode.AllCells);
            evilGridView.AutoResizeColumns(
                DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void UpdateState(string state)
        {
            this.stateLabel.Text = "State: " + state;
            RefreshAll();
        }

        private void RefreshAll()
        {
            this.Refresh();

            borderBind.DataSource = borderloopCategories;
            borderGridView.DataSource = borderBind;
            borderGridView.DataSource = borderloopCategories;

            evilBind.DataSource = evilCategories;
            evilGridView.DataSource = evilBind;
            evilGridView.DataSource = evilCategories;

            borderGridView.Refresh();
            evilGridView.Refresh();
            if(evilGridView.Rows.Count == 0 && evilCalls.Count == 0)
            {
                MessageBox.Show("No more categories to be matched.");
            }
        }
    }
}
