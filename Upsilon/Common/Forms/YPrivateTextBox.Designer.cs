namespace Upsilon.Common.Forms
{
    partial class YPrivateTextBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._privateText_TB = new System.Windows.Forms.TextBox();
            this._showPrivateBox_PB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._showPrivateBox_PB)).BeginInit();
            this.SuspendLayout();
            // 
            // _privateText_TB
            // 
            this._privateText_TB.BackColor = System.Drawing.SystemColors.Control;
            this._privateText_TB.Dock = System.Windows.Forms.DockStyle.Fill;
            this._privateText_TB.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._privateText_TB.ForeColor = System.Drawing.Color.Black;
            this._privateText_TB.Location = new System.Drawing.Point(0, 0);
            this._privateText_TB.Name = "_privateText_TB";
            this._privateText_TB.PasswordChar = '¤';
            this._privateText_TB.Size = new System.Drawing.Size(477, 21);
            this._privateText_TB.TabIndex = 0;
            // 
            // _showPrivateBox_PB
            // 
            this._showPrivateBox_PB.BackColor = System.Drawing.Color.Transparent;
            this._showPrivateBox_PB.Dock = System.Windows.Forms.DockStyle.Right;
            this._showPrivateBox_PB.ForeColor = System.Drawing.Color.White;
            this._showPrivateBox_PB.Image = global::Upsilon.Common.Forms.Properties.Resources.WhiteEye;
            this._showPrivateBox_PB.Location = new System.Drawing.Point(477, 0);
            this._showPrivateBox_PB.Name = "_showPrivateBox_PB";
            this._showPrivateBox_PB.Size = new System.Drawing.Size(23, 21);
            this._showPrivateBox_PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._showPrivateBox_PB.TabIndex = 1;
            this._showPrivateBox_PB.TabStop = false;
            // 
            // PrivatetTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._privateText_TB);
            this.Controls.Add(this._showPrivateBox_PB);
            this.Name = "PrivatetTextBox";
            this.Size = new System.Drawing.Size(500, 21);
            ((System.ComponentModel.ISupportInitialize)(this._showPrivateBox_PB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox _showPrivateBox_PB;
        private System.Windows.Forms.TextBox _privateText_TB;
    }
}
