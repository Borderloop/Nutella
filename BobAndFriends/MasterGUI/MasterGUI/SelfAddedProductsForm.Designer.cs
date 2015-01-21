namespace MasterGUI
{
    partial class SelfAddedProductsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelfAddedProductsForm));
            this.SelfAddedProductsView = new System.Windows.Forms.DataGridView();
            this.MenuStrip = new System.Windows.Forms.ToolStrip();
            this.AddButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.RemoveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CloneButton = new System.Windows.Forms.ToolStripButton();
            this.SelfAddedProductsXml = new System.Data.DataSet();
            ((System.ComponentModel.ISupportInitialize)(this.SelfAddedProductsView)).BeginInit();
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelfAddedProductsXml)).BeginInit();
            this.SuspendLayout();
            // 
            // SelfAddedProductsView
            // 
            this.SelfAddedProductsView.AllowUserToAddRows = false;
            this.SelfAddedProductsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SelfAddedProductsView.Location = new System.Drawing.Point(12, 28);
            this.SelfAddedProductsView.Name = "SelfAddedProductsView";
            this.SelfAddedProductsView.Size = new System.Drawing.Size(1397, 429);
            this.SelfAddedProductsView.TabIndex = 0;
            this.SelfAddedProductsView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.SelfAddedProductsView_CellValueChanged);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddButton,
            this.toolStripSeparator2,
            this.RemoveButton,
            this.toolStripSeparator1,
            this.SaveButton,
            this.toolStripSeparator3,
            this.CloneButton});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(1421, 25);
            this.MenuStrip.TabIndex = 1;
            this.MenuStrip.Text = "toolStrip1";
            // 
            // AddButton
            // 
            this.AddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddButton.Image = ((System.Drawing.Image)(resources.GetObject("AddButton.Image")));
            this.AddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(33, 22);
            this.AddButton.Text = "Add";
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // RemoveButton
            // 
            this.RemoveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveButton.Image")));
            this.RemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(54, 22);
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(35, 22);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // CloneButton
            // 
            this.CloneButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CloneButton.Image = ((System.Drawing.Image)(resources.GetObject("CloneButton.Image")));
            this.CloneButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CloneButton.Name = "CloneButton";
            this.CloneButton.Size = new System.Drawing.Size(42, 22);
            this.CloneButton.Text = "Clone";
            this.CloneButton.Click += new System.EventHandler(this.CloneButton_Click);
            // 
            // SelfAddedProductsXml
            // 
            this.SelfAddedProductsXml.DataSetName = "SelfAddedProductsXml";
            // 
            // SelfAddedProductsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1421, 476);
            this.Controls.Add(this.MenuStrip);
            this.Controls.Add(this.SelfAddedProductsView);
            this.Name = "SelfAddedProductsForm";
            this.Text = "SelfAddedProductsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelfAddedProductsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SelfAddedProductsView)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelfAddedProductsXml)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView SelfAddedProductsView;
        private System.Windows.Forms.ToolStrip MenuStrip;
        private System.Windows.Forms.ToolStripButton AddButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton RemoveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Data.DataSet SelfAddedProductsXml;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton CloneButton;
    }
}