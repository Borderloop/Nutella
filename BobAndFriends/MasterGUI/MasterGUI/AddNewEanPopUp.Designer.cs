namespace MasterGUI
{
    partial class AddNewEanPopUp
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
            this.NewEANTextBox = new System.Windows.Forms.TextBox();
            this.AddNewEANButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "EAN:";
            // 
            // NewEANTextBox
            // 
            this.NewEANTextBox.Location = new System.Drawing.Point(107, 30);
            this.NewEANTextBox.Name = "NewEANTextBox";
            this.NewEANTextBox.Size = new System.Drawing.Size(125, 20);
            this.NewEANTextBox.TabIndex = 7;
            // 
            // AddNewEANButton
            // 
            this.AddNewEANButton.Location = new System.Drawing.Point(89, 74);
            this.AddNewEANButton.Name = "AddNewEANButton";
            this.AddNewEANButton.Size = new System.Drawing.Size(93, 41);
            this.AddNewEANButton.TabIndex = 12;
            this.AddNewEANButton.Text = "Add";
            this.AddNewEANButton.UseVisualStyleBackColor = true;
            this.AddNewEANButton.Click += new System.EventHandler(this.AddNewEANButton_Click);
            // 
            // AddNewEanPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 147);
            this.Controls.Add(this.AddNewEANButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewEANTextBox);
            this.Name = "AddNewEanPopUp";
            this.Text = "Add new EAN";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NewEANTextBox;
        private System.Windows.Forms.Button AddNewEANButton;
    }
}