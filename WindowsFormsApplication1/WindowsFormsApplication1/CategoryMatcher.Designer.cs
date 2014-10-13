namespace WindowsFormsApplication1
{
    partial class CategoryMatcher
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
            this.categoryGridView = new System.Windows.Forms.DataGridView();
            this.inkomendGridView = new System.Windows.Forms.DataGridView();
            this.match_categorysyn = new System.Windows.Forms.Button();
            this.btn_search = new System.Windows.Forms.Button();
            this.txt_search = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.productCategoryGridView = new System.Windows.Forms.DataGridView();
            this.linkedProduct = new System.Windows.Forms.Button();
            this.RefreshDataGridInkomend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.categoryGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inkomendGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productCategoryGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // categoryGridView
            // 
            this.categoryGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.categoryGridView.Location = new System.Drawing.Point(12, 327);
            this.categoryGridView.Name = "categoryGridView";
            this.categoryGridView.Size = new System.Drawing.Size(483, 291);
            this.categoryGridView.TabIndex = 0;
            // 
            // inkomendGridView
            // 
            this.inkomendGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.inkomendGridView.Location = new System.Drawing.Point(527, 326);
            this.inkomendGridView.Name = "inkomendGridView";
            this.inkomendGridView.Size = new System.Drawing.Size(725, 294);
            this.inkomendGridView.TabIndex = 1;
            // 
            // match_categorysyn
            // 
            this.match_categorysyn.Location = new System.Drawing.Point(527, 626);
            this.match_categorysyn.Name = "match_categorysyn";
            this.match_categorysyn.Size = new System.Drawing.Size(75, 23);
            this.match_categorysyn.TabIndex = 3;
            this.match_categorysyn.Text = "Match";
            this.match_categorysyn.UseVisualStyleBackColor = true;
            this.match_categorysyn.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_search
            // 
            this.btn_search.Location = new System.Drawing.Point(783, 300);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(75, 23);
            this.btn_search.TabIndex = 4;
            this.btn_search.Text = "Search";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // txt_search
            // 
            this.txt_search.Location = new System.Drawing.Point(527, 300);
            this.txt_search.Name = "txt_search";
            this.txt_search.Size = new System.Drawing.Size(250, 20);
            this.txt_search.TabIndex = 5;
            this.txt_search.TextChanged += new System.EventHandler(this.txt_search_TextChanged);
            // 
            // productCategoryGridView
            // 
            this.productCategoryGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.productCategoryGridView.Location = new System.Drawing.Point(12, 12);
            this.productCategoryGridView.Name = "productCategoryGridView";
            this.productCategoryGridView.Size = new System.Drawing.Size(1216, 282);
            this.productCategoryGridView.TabIndex = 7;
            // 
            // linkedProduct
            // 
            this.linkedProduct.Location = new System.Drawing.Point(945, 300);
            this.linkedProduct.Name = "linkedProduct";
            this.linkedProduct.Size = new System.Drawing.Size(132, 23);
            this.linkedProduct.TabIndex = 8;
            this.linkedProduct.Text = "Linked Product";
            this.linkedProduct.UseVisualStyleBackColor = true;
            this.linkedProduct.Click += new System.EventHandler(this.linkedProduct_Click);
            // 
            // RefreshDataGridInkomend
            // 
            this.RefreshDataGridInkomend.Location = new System.Drawing.Point(864, 300);
            this.RefreshDataGridInkomend.Name = "RefreshDataGridInkomend";
            this.RefreshDataGridInkomend.Size = new System.Drawing.Size(75, 23);
            this.RefreshDataGridInkomend.TabIndex = 9;
            this.RefreshDataGridInkomend.Text = "Refresh";
            this.RefreshDataGridInkomend.UseVisualStyleBackColor = true;
            this.RefreshDataGridInkomend.Click += new System.EventHandler(this.RefreshDataGridInkomend_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 300);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "Borderloop Categories";
            // 
            // CategoryMatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 651);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RefreshDataGridInkomend);
            this.Controls.Add(this.linkedProduct);
            this.Controls.Add(this.productCategoryGridView);
            this.Controls.Add(this.txt_search);
            this.Controls.Add(this.btn_search);
            this.Controls.Add(this.match_categorysyn);
            this.Controls.Add(this.inkomendGridView);
            this.Controls.Add(this.categoryGridView);
            this.Name = "CategoryMatcher";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.categoryGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inkomendGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productCategoryGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView categoryGridView;
        private System.Windows.Forms.DataGridView inkomendGridView;
        private System.Windows.Forms.Button match_categorysyn;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.TextBox txt_search;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView productCategoryGridView;
        private System.Windows.Forms.Button linkedProduct;
        private System.Windows.Forms.Button RefreshDataGridInkomend;
        private System.Windows.Forms.Label label1;
    }
}

