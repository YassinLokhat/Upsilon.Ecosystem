
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
            this.pBar = new System.Windows.Forms.Panel();
            this.lTitle = new System.Windows.Forms.Label();
            this.bMinimize = new System.Windows.Forms.Button();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.bMaximize = new System.Windows.Forms.Button();
            this.bClose = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pContainer = new System.Windows.Forms.Panel();
            this.pBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pBar
            // 
            this.pBar.Controls.Add(this.lTitle);
            this.pBar.Controls.Add(this.bMinimize);
            this.pBar.Controls.Add(this.pbIcon);
            this.pBar.Controls.Add(this.bMaximize);
            this.pBar.Controls.Add(this.bClose);
            this.pBar.Controls.Add(this.panel2);
            this.pBar.Controls.Add(this.panel3);
            this.pBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pBar.Location = new System.Drawing.Point(0, 0);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(700, 25);
            this.pBar.TabIndex = 0;
            // 
            // lTitle
            // 
            this.lTitle.AutoSize = true;
            this.lTitle.Location = new System.Drawing.Point(46, 5);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(42, 15);
            this.lTitle.TabIndex = 4;
            this.lTitle.Text = "YForm";
            // 
            // bMinimize
            // 
            this.bMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.bMinimize.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.bMinimize.Location = new System.Drawing.Point(604, 0);
            this.bMinimize.Name = "bMinimize";
            this.bMinimize.Size = new System.Drawing.Size(25, 25);
            this.bMinimize.TabIndex = 5;
            this.bMinimize.Text = "_";
            this.bMinimize.UseVisualStyleBackColor = true;
            // 
            // pbIcon
            // 
            this.pbIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbIcon.Location = new System.Drawing.Point(15, 0);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(25, 25);
            this.pbIcon.TabIndex = 1;
            this.pbIcon.TabStop = false;
            // 
            // bMaximize
            // 
            this.bMaximize.Dock = System.Windows.Forms.DockStyle.Right;
            this.bMaximize.Location = new System.Drawing.Point(629, 0);
            this.bMaximize.Name = "bMaximize";
            this.bMaximize.Size = new System.Drawing.Size(25, 25);
            this.bMaximize.TabIndex = 3;
            this.bMaximize.Text = "O";
            this.bMaximize.UseVisualStyleBackColor = true;
            // 
            // bClose
            // 
            this.bClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.bClose.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.bClose.Location = new System.Drawing.Point(654, 0);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(25, 25);
            this.bClose.TabIndex = 2;
            this.bClose.Text = "X";
            this.bClose.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(679, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(21, 25);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(15, 25);
            this.panel3.TabIndex = 2;
            // 
            // pContainer
            // 
            this.pContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pContainer.Location = new System.Drawing.Point(0, 25);
            this.pContainer.Name = "pContainer";
            this.pContainer.Size = new System.Drawing.Size(700, 313);
            this.pContainer.TabIndex = 1;
            // 
            // YForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.pContainer);
            this.Controls.Add(this.pBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "YForm";
            this.Text = "YForm";
            this.pBar.ResumeLayout(false);
            this.pBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pBar;
        private System.Windows.Forms.Button bMaximize;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Label lTitle;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Button bMinimize;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pContainer;
    }
}