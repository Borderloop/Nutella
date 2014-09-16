namespace BobAndFriends
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualBob));
            this.suggestedProducts = new System.Windows.Forms.DataGridView();
            this.suggestedProduct_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suggestedProduct_brand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suggestedProduct_title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suggestedProduct_ean = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suggestedProduct_sku = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectedProduct = new System.Windows.Forms.DataGridView();
            this.selectedProduct_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectedProduct_brand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectedProduct_title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectedProduct_ean = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectedProduct_sku = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.residuButton = new System.Windows.Forms.Button();
            this.selectedProductLabel = new System.Windows.Forms.Label();
            this.suggestedProductsLabel = new System.Windows.Forms.Label();
            this.vBOBLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.suggestedProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vBOBLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // suggestedProducts
            // 
            this.suggestedProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.suggestedProducts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.suggestedProduct_id,
            this.suggestedProduct_brand,
            this.suggestedProduct_title,
            this.suggestedProduct_ean,
            this.suggestedProduct_sku});
            this.suggestedProducts.Location = new System.Drawing.Point(563, 55);
            this.suggestedProducts.Name = "suggestedProducts";
            this.suggestedProducts.Size = new System.Drawing.Size(544, 234);
            this.suggestedProducts.TabIndex = 0;
            // 
            // suggestedProduct_id
            // 
            this.suggestedProduct_id.HeaderText = "id";
            this.suggestedProduct_id.Name = "suggestedProduct_id";
            this.suggestedProduct_id.ReadOnly = true;
            // 
            // suggestedProduct_brand
            // 
            this.suggestedProduct_brand.HeaderText = "brand";
            this.suggestedProduct_brand.Name = "suggestedProduct_brand";
            this.suggestedProduct_brand.ReadOnly = true;
            // 
            // suggestedProduct_title
            // 
            this.suggestedProduct_title.HeaderText = "title";
            this.suggestedProduct_title.Name = "suggestedProduct_title";
            this.suggestedProduct_title.ReadOnly = true;
            // 
            // suggestedProduct_ean
            // 
            this.suggestedProduct_ean.HeaderText = "ean";
            this.suggestedProduct_ean.Name = "suggestedProduct_ean";
            this.suggestedProduct_ean.ReadOnly = true;
            // 
            // suggestedProduct_sku
            // 
            this.suggestedProduct_sku.HeaderText = "sku";
            this.suggestedProduct_sku.Name = "suggestedProduct_sku";
            this.suggestedProduct_sku.ReadOnly = true;
            // 
            // selectedProduct
            // 
            this.selectedProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.selectedProduct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.selectedProduct_id,
            this.selectedProduct_brand,
            this.selectedProduct_title,
            this.selectedProduct_ean,
            this.selectedProduct_sku});
            this.selectedProduct.Location = new System.Drawing.Point(12, 55);
            this.selectedProduct.Name = "selectedProduct";
            this.selectedProduct.Size = new System.Drawing.Size(545, 47);
            this.selectedProduct.TabIndex = 1;
            // 
            // selectedProduct_id
            // 
            this.selectedProduct_id.HeaderText = "id";
            this.selectedProduct_id.Name = "selectedProduct_id";
            // 
            // selectedProduct_brand
            // 
            this.selectedProduct_brand.HeaderText = "brand";
            this.selectedProduct_brand.Name = "selectedProduct_brand";
            // 
            // selectedProduct_title
            // 
            this.selectedProduct_title.HeaderText = "title";
            this.selectedProduct_title.Name = "selectedProduct_title";
            // 
            // selectedProduct_ean
            // 
            this.selectedProduct_ean.HeaderText = "ean";
            this.selectedProduct_ean.Name = "selectedProduct_ean";
            // 
            // selectedProduct_sku
            // 
            this.selectedProduct_sku.HeaderText = "sku";
            this.selectedProduct_sku.Name = "selectedProduct_sku";
            // 
            // matchButton
            // 
            this.matchButton.Location = new System.Drawing.Point(74, 311);
            this.matchButton.Name = "matchButton";
            this.matchButton.Size = new System.Drawing.Size(190, 51);
            this.matchButton.TabIndex = 2;
            this.matchButton.Text = "Match with selected record";
            this.matchButton.UseVisualStyleBackColor = true;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(463, 311);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(190, 51);
            this.createButton.TabIndex = 3;
            this.createButton.Text = "Create new product";
            this.createButton.UseVisualStyleBackColor = true;
            // 
            // residuButton
            // 
            this.residuButton.Location = new System.Drawing.Point(856, 311);
            this.residuButton.Name = "residuButton";
            this.residuButton.Size = new System.Drawing.Size(198, 51);
            this.residuButton.TabIndex = 4;
            this.residuButton.Text = "Send to residu";
            this.residuButton.UseVisualStyleBackColor = true;
            // 
            // selectedProductLabel
            // 
            this.selectedProductLabel.AutoSize = true;
            this.selectedProductLabel.Location = new System.Drawing.Point(12, 39);
            this.selectedProductLabel.Name = "selectedProductLabel";
            this.selectedProductLabel.Size = new System.Drawing.Size(91, 13);
            this.selectedProductLabel.TabIndex = 5;
            this.selectedProductLabel.Text = "Selected product:";
            // 
            // suggestedProductsLabel
            // 
            this.suggestedProductsLabel.AutoSize = true;
            this.suggestedProductsLabel.Location = new System.Drawing.Point(563, 36);
            this.suggestedProductsLabel.Name = "suggestedProductsLabel";
            this.suggestedProductsLabel.Size = new System.Drawing.Size(109, 13);
            this.suggestedProductsLabel.TabIndex = 6;
            this.suggestedProductsLabel.Text = "Suggested Products: ";
            // 
            // vBOBLogo
            // 
            this.vBOBLogo.Image = global::BobAndFriends.Properties.Resources.vBOBLogoNew;
            this.vBOBLogo.InitialImage = ((System.Drawing.Image)(resources.GetObject("vBOBLogo.InitialImage")));
            this.vBOBLogo.Location = new System.Drawing.Point(873, 2);
            this.vBOBLogo.Name = "vBOBLogo";
            this.vBOBLogo.Size = new System.Drawing.Size(234, 50);
            this.vBOBLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.vBOBLogo.TabIndex = 7;
            this.vBOBLogo.TabStop = false;
            // 
            // VisualBob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 393);
            this.Controls.Add(this.vBOBLogo);
            this.Controls.Add(this.suggestedProductsLabel);
            this.Controls.Add(this.selectedProductLabel);
            this.Controls.Add(this.residuButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.matchButton);
            this.Controls.Add(this.selectedProduct);
            this.Controls.Add(this.suggestedProducts);
            this.Name = "VisualBob";
            this.Text = "VisualBob";
            ((System.ComponentModel.ISupportInitialize)(this.suggestedProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vBOBLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView suggestedProducts;
        private System.Windows.Forms.DataGridView selectedProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn suggestedProduct_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn suggestedProduct_brand;
        private System.Windows.Forms.DataGridViewTextBoxColumn suggestedProduct_title;
        private System.Windows.Forms.DataGridViewTextBoxColumn suggestedProduct_ean;
        private System.Windows.Forms.DataGridViewTextBoxColumn suggestedProduct_sku;
        private System.Windows.Forms.DataGridViewTextBoxColumn selectedProduct_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn selectedProduct_brand;
        private System.Windows.Forms.DataGridViewTextBoxColumn selectedProduct_title;
        private System.Windows.Forms.DataGridViewTextBoxColumn selectedProduct_ean;
        private System.Windows.Forms.DataGridViewTextBoxColumn selectedProduct_sku;
        private System.Windows.Forms.Button matchButton;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button residuButton;
        private System.Windows.Forms.Label selectedProductLabel;
        private System.Windows.Forms.Label suggestedProductsLabel;
        private System.Windows.Forms.PictureBox vBOBLogo;
    }
}