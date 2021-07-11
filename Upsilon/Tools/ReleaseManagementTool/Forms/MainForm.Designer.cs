
namespace Upsilon.Tools.ReleaseManagementTool.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cbAssembly = new System.Windows.Forms.ComboBox();
            this.dgvDependecies = new System.Windows.Forms.DataGridView();
            this.DependecyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinimalVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaximalVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Assembly :";
            // 
            // cbAssembly
            // 
            this.cbAssembly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAssembly.FormattingEnabled = true;
            this.cbAssembly.Location = new System.Drawing.Point(82, 12);
            this.cbAssembly.Name = "cbAssembly";
            this.cbAssembly.Size = new System.Drawing.Size(690, 23);
            this.cbAssembly.TabIndex = 1;
            // 
            // dgvDependecies
            // 
            this.dgvDependecies.AllowUserToAddRows = false;
            this.dgvDependecies.AllowUserToDeleteRows = false;
            this.dgvDependecies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDependecies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DependecyName,
            this.MinimalVersion,
            this.MaximalVersion});
            this.dgvDependecies.Location = new System.Drawing.Point(12, 41);
            this.dgvDependecies.Name = "dgvDependecies";
            this.dgvDependecies.RowTemplate.Height = 25;
            this.dgvDependecies.Size = new System.Drawing.Size(760, 508);
            this.dgvDependecies.TabIndex = 2;
            // 
            // DependecyName
            // 
            this.DependecyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DependecyName.HeaderText = "Dependency Name";
            this.DependecyName.Name = "DependecyName";
            // 
            // MinimalVersion
            // 
            this.MinimalVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MinimalVersion.HeaderText = "Minimal Version";
            this.MinimalVersion.Name = "MinimalVersion";
            this.MinimalVersion.Width = 107;
            // 
            // MaximalVersion
            // 
            this.MaximalVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MaximalVersion.HeaderText = "Maximal Version";
            this.MaximalVersion.Name = "MaximalVersion";
            this.MaximalVersion.Width = 109;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.dgvDependecies);
            this.Controls.Add(this.cbAssembly);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Release Management Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAssembly;
        private System.Windows.Forms.DataGridView dgvDependecies;
        private System.Windows.Forms.DataGridViewTextBoxColumn DependecyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinimalVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaximalVersion;
    }
}

