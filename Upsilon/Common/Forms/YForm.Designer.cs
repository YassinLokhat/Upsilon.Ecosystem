namespace Upsilon.Common.Forms
{
    partial class YForm
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
            this.ControlsPanel = new System.Windows.Forms.Panel();
            this._titleBar_P = new System.Windows.Forms.Panel();
            this._title_L = new System.Windows.Forms.Label();
            this._minimize_B = new System.Windows.Forms.Button();
            this._close_B = new System.Windows.Forms.Button();
            this._icon_PB = new System.Windows.Forms.PictureBox();
            this._titleBar_P.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._icon_PB)).BeginInit();
            this.SuspendLayout();
            // 
            // ControlsPanel
            // 
            this.ControlsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControlsPanel.Location = new System.Drawing.Point(0, 25);
            this.ControlsPanel.Name = "ControlsPanel";
            this.ControlsPanel.Size = new System.Drawing.Size(800, 425);
            this.ControlsPanel.TabIndex = 0;
            // 
            // _titleBar_P
            // 
            this._titleBar_P.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._titleBar_P.Controls.Add(this._title_L);
            this._titleBar_P.Controls.Add(this._minimize_B);
            this._titleBar_P.Controls.Add(this._close_B);
            this._titleBar_P.Controls.Add(this._icon_PB);
            this._titleBar_P.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleBar_P.Location = new System.Drawing.Point(0, 0);
            this._titleBar_P.Name = "_titleBar_P";
            this._titleBar_P.Size = new System.Drawing.Size(800, 25);
            this._titleBar_P.TabIndex = 1;
            // 
            // _title_L
            // 
            this._title_L.Dock = System.Windows.Forms.DockStyle.Fill;
            this._title_L.Location = new System.Drawing.Point(25, 0);
            this._title_L.Name = "_title_L";
            this._title_L.Size = new System.Drawing.Size(727, 23);
            this._title_L.TabIndex = 1;
            this._title_L.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _minimize_B
            // 
            this._minimize_B.Dock = System.Windows.Forms.DockStyle.Right;
            this._minimize_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._minimize_B.Location = new System.Drawing.Point(752, 0);
            this._minimize_B.Name = "_minimize_B";
            this._minimize_B.Size = new System.Drawing.Size(23, 23);
            this._minimize_B.TabIndex = 3;
            this._minimize_B.TabStop = false;
            this._minimize_B.Text = "-";
            this._minimize_B.UseVisualStyleBackColor = true;
            // 
            // _close_B
            // 
            this._close_B.Dock = System.Windows.Forms.DockStyle.Right;
            this._close_B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._close_B.Location = new System.Drawing.Point(775, 0);
            this._close_B.Name = "_close_B";
            this._close_B.Size = new System.Drawing.Size(23, 23);
            this._close_B.TabIndex = 0;
            this._close_B.TabStop = false;
            this._close_B.Text = "X";
            this._close_B.UseVisualStyleBackColor = true;
            // 
            // _icon_PB
            // 
            this._icon_PB.Dock = System.Windows.Forms.DockStyle.Left;
            this._icon_PB.Location = new System.Drawing.Point(0, 0);
            this._icon_PB.Name = "_icon_PB";
            this._icon_PB.Size = new System.Drawing.Size(25, 23);
            this._icon_PB.TabIndex = 0;
            this._icon_PB.TabStop = false;
            this._icon_PB.Visible = false;
            // 
            // CustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ControlsPanel);
            this.Controls.Add(this._titleBar_P);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CustomForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CustomForm";
            this._titleBar_P.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._icon_PB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel _titleBar_P;
        private System.Windows.Forms.Label _title_L;
        private System.Windows.Forms.Button _minimize_B;
        private System.Windows.Forms.Button _close_B;
        private System.Windows.Forms.PictureBox _icon_PB;
        public System.Windows.Forms.Panel ControlsPanel;
    }
}