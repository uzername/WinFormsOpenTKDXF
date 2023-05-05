namespace WinFormsOpenTKDXF
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BrowseBtn = new System.Windows.Forms.Button();
            this.textBoxPathDxf = new System.Windows.Forms.TextBox();
            this.userControlWinFormsdxf1 = new WinFormsOpenTKDXF.UserControlWinFormsDXF();
            this.SuspendLayout();
            // 
            // BrowseBtn
            // 
            this.BrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseBtn.Location = new System.Drawing.Point(538, 421);
            this.BrowseBtn.Name = "BrowseBtn";
            this.BrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.BrowseBtn.TabIndex = 0;
            this.BrowseBtn.Text = "Choose file";
            this.BrowseBtn.UseVisualStyleBackColor = true;
            this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // textBoxPathDxf
            // 
            this.textBoxPathDxf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPathDxf.Location = new System.Drawing.Point(4, 421);
            this.textBoxPathDxf.Name = "textBoxPathDxf";
            this.textBoxPathDxf.Size = new System.Drawing.Size(528, 23);
            this.textBoxPathDxf.TabIndex = 1;
            // 
            // userControlWinFormsdxf1
            // 
            this.userControlWinFormsdxf1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userControlWinFormsdxf1.Location = new System.Drawing.Point(4, 12);
            this.userControlWinFormsdxf1.Name = "userControlWinFormsdxf1";
            this.userControlWinFormsdxf1.Size = new System.Drawing.Size(609, 403);
            this.userControlWinFormsdxf1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 450);
            this.Controls.Add(this.userControlWinFormsdxf1);
            this.Controls.Add(this.textBoxPathDxf);
            this.Controls.Add(this.BrowseBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button BrowseBtn;
        private TextBox textBoxPathDxf;
        private UserControlWinFormsDXF userControlWinFormsdxf1;
    }
}