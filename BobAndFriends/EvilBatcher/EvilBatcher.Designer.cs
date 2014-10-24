namespace EvilBatcher
{
    partial class EvilBatcher
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
            this.evil_goDownButton = new System.Windows.Forms.Button();
            this.evil_goUpButton = new System.Windows.Forms.Button();
            this.borderGridView = new System.Windows.Forms.DataGridView();
            this.evilGridView = new System.Windows.Forms.DataGridView();
            this.matchButton = new System.Windows.Forms.Button();
            this.border_goUpButton = new System.Windows.Forms.Button();
            this.border_goDownButton = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.borderGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.evilGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // evil_goDownButton
            // 
            this.evil_goDownButton.Location = new System.Drawing.Point(461, 33);
            this.evil_goDownButton.Name = "evil_goDownButton";
            this.evil_goDownButton.Size = new System.Drawing.Size(75, 23);
            this.evil_goDownButton.TabIndex = 0;
            this.evil_goDownButton.Text = ">>";
            this.evil_goDownButton.UseVisualStyleBackColor = true;
            // 
            // evil_goUpButton
            // 
            this.evil_goUpButton.Location = new System.Drawing.Point(345, 33);
            this.evil_goUpButton.Name = "evil_goUpButton";
            this.evil_goUpButton.Size = new System.Drawing.Size(75, 23);
            this.evil_goUpButton.TabIndex = 1;
            this.evil_goUpButton.Text = "<<";
            this.evil_goUpButton.UseVisualStyleBackColor = true;
            // 
            // borderGridView
            // 
            this.borderGridView.AllowUserToAddRows = false;
            this.borderGridView.AllowUserToDeleteRows = false;
            this.borderGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.borderGridView.Location = new System.Drawing.Point(12, 62);
            this.borderGridView.Name = "borderGridView";
            this.borderGridView.Size = new System.Drawing.Size(257, 169);
            this.borderGridView.TabIndex = 2;
            // 
            // evilGridView
            // 
            this.evilGridView.AllowUserToAddRows = false;
            this.evilGridView.AllowUserToDeleteRows = false;
            this.evilGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.evilGridView.Location = new System.Drawing.Point(301, 62);
            this.evilGridView.Name = "evilGridView";
            this.evilGridView.Size = new System.Drawing.Size(257, 169);
            this.evilGridView.TabIndex = 3;
            // 
            // matchButton
            // 
            this.matchButton.Location = new System.Drawing.Point(246, 32);
            this.matchButton.Name = "matchButton";
            this.matchButton.Size = new System.Drawing.Size(76, 23);
            this.matchButton.TabIndex = 4;
            this.matchButton.Text = "Match";
            this.matchButton.UseVisualStyleBackColor = true;
            // 
            // border_goUpButton
            // 
            this.border_goUpButton.Location = new System.Drawing.Point(29, 33);
            this.border_goUpButton.Name = "border_goUpButton";
            this.border_goUpButton.Size = new System.Drawing.Size(75, 23);
            this.border_goUpButton.TabIndex = 5;
            this.border_goUpButton.Text = "<<<";
            this.border_goUpButton.UseVisualStyleBackColor = true;
            // 
            // border_goDownButton
            // 
            this.border_goDownButton.Location = new System.Drawing.Point(139, 33);
            this.border_goDownButton.Name = "border_goDownButton";
            this.border_goDownButton.Size = new System.Drawing.Size(75, 23);
            this.border_goDownButton.TabIndex = 6;
            this.border_goDownButton.Text = ">>>";
            this.border_goDownButton.UseVisualStyleBackColor = true;
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateLabel.Location = new System.Drawing.Point(8, 9);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(80, 20);
            this.stateLabel.TabIndex = 7;
            this.stateLabel.Text = "State: idle";
            // 
            // EvilBatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 250);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.border_goDownButton);
            this.Controls.Add(this.border_goUpButton);
            this.Controls.Add(this.matchButton);
            this.Controls.Add(this.evilGridView);
            this.Controls.Add(this.borderGridView);
            this.Controls.Add(this.evil_goUpButton);
            this.Controls.Add(this.evil_goDownButton);
            this.Name = "EvilBatcher";
            this.Text = "EvilBatcher";
            this.Load += new System.EventHandler(this.SizeAllColumns);
            ((System.ComponentModel.ISupportInitialize)(this.borderGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.evilGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button evil_goDownButton;
        private System.Windows.Forms.Button evil_goUpButton;
        private System.Windows.Forms.DataGridView borderGridView;
        private System.Windows.Forms.DataGridView evilGridView;
        private System.Windows.Forms.Button matchButton;
        private System.Windows.Forms.Button border_goUpButton;
        private System.Windows.Forms.Button border_goDownButton;
        private System.Windows.Forms.Label stateLabel;
    }
}

