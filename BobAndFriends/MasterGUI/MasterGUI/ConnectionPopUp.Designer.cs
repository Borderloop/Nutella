namespace MasterGUI
{
    partial class ConnectionPopUp
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
            this.DbName = new System.Windows.Forms.TextBox();
            this.DbPort = new System.Windows.Forms.TextBox();
            this.DbUser = new System.Windows.Forms.TextBox();
            this.DbPassword = new System.Windows.Forms.MaskedTextBox();
            this.DbIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //  
            //  DbName
            //  
            this.DbName.Location = new System.Drawing.Point(105, 55);
            this.DbName.Name = "DbName";
            this.DbName.Size = new System.Drawing.Size(100, 20);
            this.DbName.TabIndex = 0;
            //  
            //  DbPort
            //  
            this.DbPort.Location = new System.Drawing.Point(105, 82);
            this.DbPort.Name = "DbPort";
            this.DbPort.Size = new System.Drawing.Size(100, 20);
            this.DbPort.TabIndex = 1;
            //  
            //  DbUser
            //  
            this.DbUser.Location = new System.Drawing.Point(105, 109);
            this.DbUser.Name = "DbUser";
            this.DbUser.Size = new System.Drawing.Size(100, 20);
            this.DbUser.TabIndex = 2;
            //  
            //  DbPassword
            //  
            this.DbPassword.Location = new System.Drawing.Point(105, 135);
            this.DbPassword.Name = "DbPassword";
            this.DbPassword.PasswordChar = '*';
            this.DbPassword.Size = new System.Drawing.Size(100, 20);
            this.DbPassword.TabIndex = 4;
            //  
            //  DbIp
            //  
            this.DbIp.Location = new System.Drawing.Point(105, 29);
            this.DbIp.Name = "DbIp";
            this.DbIp.Size = new System.Drawing.Size(100, 20);
            this.DbIp.TabIndex = 5;
            //  
            //  label1
            //  
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Server:";
            //  
            //  label2
            //  
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name:";
            //  
            //  label3
            //  
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port:";
            //  
            //  label4
            //  
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "User:";
            //  
            //  label5
            //  
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Password:";
            //  
            //  ConnectButton
            //  
            this.ConnectButton.Location = new System.Drawing.Point(74, 180);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(93, 41);
            this.ConnectButton.TabIndex = 11;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            //  
            //  ConnectionPopUp
            //  
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 233);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DbIp);
            this.Controls.Add(this.DbPassword);
            this.Controls.Add(this.DbUser);
            this.Controls.Add(this.DbPort);
            this.Controls.Add(this.DbName);
            this.Name = "ConnectionPopUp";
            this.Text = "Manage connection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DbName;
        private System.Windows.Forms.TextBox DbPort;
        private System.Windows.Forms.TextBox DbUser;
        private System.Windows.Forms.MaskedTextBox DbPassword;
        private System.Windows.Forms.TextBox DbIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ConnectButton;
    }
}