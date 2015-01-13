namespace VisualBob
{
    partial class VisualBob
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualBob));
            this.suggestedProductsDataGrid = new System.Windows.Forms.DataGridView();
            this.selectedProductDataGrid = new System.Windows.Forms.DataGridView();
            this.matchButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.residuButton = new System.Windows.Forms.Button();
            this.selectedProductLabel = new System.Windows.Forms.Label();
            this.suggestedProductsLabel = new System.Windows.Forms.Label();
            this.vBOBLogo = new System.Windows.Forms.PictureBox();
            this.rerunButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.suggestedProductsDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedProductDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vBOBLogo)).BeginInit();
            this.SuspendLayout();
            //  
            //  suggestedProductsDataGrid
            //  
            this.suggestedProductsDataGrid.AllowUserToAddRows = false;
            this.suggestedProductsDataGrid.AllowUserToDeleteRows = false;
            this.suggestedProductsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.suggestedProductsDataGrid.Location = new System.Drawing.Point(566, 52);
            this.suggestedProductsDataGrid.Name = "suggestedProductsDataGrid";
            this.suggestedProductsDataGrid.Size = new System.Drawing.Size(645, 234);
            this.suggestedProductsDataGrid.TabIndex = 0;
            //  
            //  selectedProductDataGrid
            //  
            this.selectedProductDataGrid.AllowUserToAddRows = false;
            this.selectedProductDataGrid.AllowUserToDeleteRows = false;
            this.selectedProductDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.NullValue = "0";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.selectedProductDataGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.selectedProductDataGrid.Location = new System.Drawing.Point(12, 55);
            this.selectedProductDataGrid.Name = "selectedProductDataGrid";
            this.selectedProductDataGrid.Size = new System.Drawing.Size(545, 75);
            this.selectedProductDataGrid.TabIndex = 1;
            //  
            //  matchButton
            //  
            this.matchButton.Location = new System.Drawing.Point(74, 311);
            this.matchButton.Name = "matchButton";
            this.matchButton.Size = new System.Drawing.Size(190, 51);
            this.matchButton.TabIndex = 2;
            this.matchButton.Text = "Match with selected record";
            this.matchButton.UseVisualStyleBackColor = true;
            this.matchButton.Click += new System.EventHandler(this.matchButton_Click);
            //  
            //  createButton
            //  
            this.createButton.Location = new System.Drawing.Point(683, 311);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(190, 51);
            this.createButton.TabIndex = 3;
            this.createButton.Text = "Create new product";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            //  
            //  residuButton
            //  
            this.residuButton.Location = new System.Drawing.Point(971, 311);
            this.residuButton.Name = "residuButton";
            this.residuButton.Size = new System.Drawing.Size(198, 51);
            this.residuButton.TabIndex = 4;
            this.residuButton.Text = "Send to residu";
            this.residuButton.UseVisualStyleBackColor = true;
            this.residuButton.Click += new System.EventHandler(this.residuButton_Click);
            //  
            //  selectedProductLabel
            //  
            this.selectedProductLabel.AutoSize = true;
            this.selectedProductLabel.Location = new System.Drawing.Point(12, 39);
            this.selectedProductLabel.Name = "selectedProductLabel";
            this.selectedProductLabel.Size = new System.Drawing.Size(91, 13);
            this.selectedProductLabel.TabIndex = 5;
            this.selectedProductLabel.Text = "Selected product:";
            //  
            //  suggestedProductsLabel
            //  
            this.suggestedProductsLabel.AutoSize = true;
            this.suggestedProductsLabel.Location = new System.Drawing.Point(563, 36);
            this.suggestedProductsLabel.Name = "suggestedProductsLabel";
            this.suggestedProductsLabel.Size = new System.Drawing.Size(109, 13);
            this.suggestedProductsLabel.TabIndex = 6;
            this.suggestedProductsLabel.Text = "Suggested Products: ";
            //  
            //  vBOBLogo
            //  
            this.vBOBLogo.Image = global::VisualBob.Properties.Resources.vBOBLogoNew;
            this.vBOBLogo.InitialImage = ((System.Drawing.Image)(resources.GetObject("vBOBLogo.InitialImage")));
            this.vBOBLogo.Location = new System.Drawing.Point(971, 2);
            this.vBOBLogo.Name = "vBOBLogo";
            this.vBOBLogo.Size = new System.Drawing.Size(234, 50);
            this.vBOBLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.vBOBLogo.TabIndex = 7;
            this.vBOBLogo.TabStop = false;
            //  
            //  rerunButton
            //  
            this.rerunButton.Location = new System.Drawing.Point(381, 311);
            this.rerunButton.Name = "rerunButton";
            this.rerunButton.Size = new System.Drawing.Size(190, 51);
            this.rerunButton.TabIndex = 8;
            this.rerunButton.Text = "Rerun this record";
            this.rerunButton.UseVisualStyleBackColor = true;
            this.rerunButton.Click += new System.EventHandler(this.rerunButton_Click);
            //  
            //  VisualBob
            //  
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 393);
            this.Controls.Add(this.rerunButton);
            this.Controls.Add(this.vBOBLogo);
            this.Controls.Add(this.suggestedProductsLabel);
            this.Controls.Add(this.selectedProductLabel);
            this.Controls.Add(this.residuButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.matchButton);
            this.Controls.Add(this.selectedProductDataGrid);
            this.Controls.Add(this.suggestedProductsDataGrid);
            this.Name = "VisualBob";
            this.Text = "VisualBob";
            ((System.ComponentModel.ISupportInitialize)(this.suggestedProductsDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedProductDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vBOBLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView suggestedProductsDataGrid;
        private System.Windows.Forms.DataGridView selectedProductDataGrid;
        private System.Windows.Forms.Button matchButton;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button residuButton;
        private System.Windows.Forms.Label selectedProductLabel;
        private System.Windows.Forms.Label suggestedProductsLabel;
        private System.Windows.Forms.PictureBox vBOBLogo;
        private System.Windows.Forms.Button rerunButton;
    }
}