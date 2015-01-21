namespace MasterGUI
{
    partial class CreateNewArticlePopUp
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
            this.BrandBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ImageBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.EanBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TitleBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CategoryBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SkuBox = new System.Windows.Forms.TextBox();
            this.AddArticleButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //  
            //  BrandBox
            //  
            this.BrandBox.Location = new System.Drawing.Point(154, 11);
            this.BrandBox.Name = "BrandBox";
            this.BrandBox.Size = new System.Drawing.Size(100, 20);
            this.BrandBox.TabIndex = 0;
            //  
            //  label1
            //  
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(110, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Brand:";
            //  
            //  ImageBox
            //  
            this.ImageBox.Location = new System.Drawing.Point(154, 38);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(100, 20);
            this.ImageBox.TabIndex = 2;
            //  
            //  label2
            //  
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Image:";
            //  
            //  EanBox
            //  
            this.EanBox.Location = new System.Drawing.Point(154, 65);
            this.EanBox.Name = "EanBox";
            this.EanBox.Size = new System.Drawing.Size(100, 20);
            this.EanBox.TabIndex = 4;
            //  
            //  label3
            //  
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "EAN:";
            //  
            //  TitleBox
            //  
            this.TitleBox.Location = new System.Drawing.Point(154, 92);
            this.TitleBox.Name = "TitleBox";
            this.TitleBox.Size = new System.Drawing.Size(100, 20);
            this.TitleBox.TabIndex = 6;
            //  
            //  label4
            //  
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Title:";
            //  
            //  CategoryBox
            //  
            this.CategoryBox.Location = new System.Drawing.Point(154, 119);
            this.CategoryBox.Name = "CategoryBox";
            this.CategoryBox.Size = new System.Drawing.Size(100, 20);
            this.CategoryBox.TabIndex = 8;
            //  
            //  label5
            //  
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Category (id/name):";
            //  
            //  label6
            //  
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(70, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "SKU (optional):";
            //  
            //  SkuBox
            //  
            this.SkuBox.Location = new System.Drawing.Point(154, 143);
            this.SkuBox.Name = "SkuBox";
            this.SkuBox.Size = new System.Drawing.Size(100, 20);
            this.SkuBox.TabIndex = 11;
            //  
            //  AddArticleButton
            //  
            this.AddArticleButton.Location = new System.Drawing.Point(101, 184);
            this.AddArticleButton.Name = "AddArticleButton";
            this.AddArticleButton.Size = new System.Drawing.Size(126, 54);
            this.AddArticleButton.TabIndex = 12;
            this.AddArticleButton.Text = "Add";
            this.AddArticleButton.UseVisualStyleBackColor = true;
            this.AddArticleButton.Click += new System.EventHandler(this.AddArticleButton_Click);
            //  
            //  CreateNewArticlePopUp
            //  
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 250);
            this.Controls.Add(this.AddArticleButton);
            this.Controls.Add(this.SkuBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CategoryBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TitleBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EanBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BrandBox);
            this.Name = "CreateNewArticlePopUp";
            this.Text = "Create a new article.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BrandBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ImageBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox EanBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TitleBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox CategoryBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox SkuBox;
        private System.Windows.Forms.Button AddArticleButton;
    }
}