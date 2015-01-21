namespace MasterGUI
{
    partial class CreateNewCategoryPopUp
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
            this.label1 = new System.Windows.Forms.Label();
            this.AboveCategory = new System.Windows.Forms.TextBox();
            this.AddNewCategoryButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.NewCategoryBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Above category (optional):";
            // 
            // AboveCategory
            // 
            this.AboveCategory.Location = new System.Drawing.Point(168, 54);
            this.AboveCategory.Name = "AboveCategory";
            this.AboveCategory.Size = new System.Drawing.Size(100, 20);
            this.AboveCategory.TabIndex = 12;
            // 
            // AddNewCategoryButton
            // 
            this.AddNewCategoryButton.Location = new System.Drawing.Point(105, 94);
            this.AddNewCategoryButton.Name = "AddNewCategoryButton";
            this.AddNewCategoryButton.Size = new System.Drawing.Size(116, 50);
            this.AddNewCategoryButton.TabIndex = 22;
            this.AddNewCategoryButton.Text = "Add";
            this.AddNewCategoryButton.UseVisualStyleBackColor = true;
            this.AddNewCategoryButton.Click += new System.EventHandler(this.AddNewCategoryButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(81, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Category name:";
            // 
            // NewCategoryBox
            // 
            this.NewCategoryBox.Location = new System.Drawing.Point(168, 28);
            this.NewCategoryBox.Name = "NewCategoryBox";
            this.NewCategoryBox.Size = new System.Drawing.Size(100, 20);
            this.NewCategoryBox.TabIndex = 23;
            // 
            // CreateNewCategoryPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 156);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.NewCategoryBox);
            this.Controls.Add(this.AddNewCategoryButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AboveCategory);
            this.Name = "CreateNewCategoryPopUp";
            this.Text = "Create a new product.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AboveCategory;
        private System.Windows.Forms.Button AddNewCategoryButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox NewCategoryBox;
    }
}