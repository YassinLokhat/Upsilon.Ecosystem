
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbBinaryType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbVersion = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.deployToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblyinfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forTheCurrentAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forAllAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembliesjsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Assembly :";
            // 
            // cbAssembly
            // 
            this.cbAssembly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAssembly.FormattingEnabled = true;
            this.cbAssembly.Location = new System.Drawing.Point(85, 22);
            this.cbAssembly.Name = "cbAssembly";
            this.cbAssembly.Size = new System.Drawing.Size(282, 23);
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
            this.dgvDependecies.Location = new System.Drawing.Point(12, 148);
            this.dgvDependecies.Name = "dgvDependecies";
            this.dgvDependecies.RowTemplate.Height = 25;
            this.dgvDependecies.Size = new System.Drawing.Size(760, 401);
            this.dgvDependecies.TabIndex = 2;
            // 
            // DependecyName
            // 
            this.DependecyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DependecyName.HeaderText = "Dependency Name";
            this.DependecyName.Name = "DependecyName";
            this.DependecyName.ReadOnly = true;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbUrl);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbBinaryType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbDescription);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbVersion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbAssembly);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 115);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assembly info";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "csproj file :";
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(85, 80);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.ReadOnly = true;
            this.tbUrl.Size = new System.Drawing.Size(669, 23);
            this.tbUrl.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(576, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Binary type :";
            // 
            // tbBinaryType
            // 
            this.tbBinaryType.Location = new System.Drawing.Point(654, 22);
            this.tbBinaryType.Name = "tbBinaryType";
            this.tbBinaryType.Size = new System.Drawing.Size(100, 23);
            this.tbBinaryType.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Description :";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(85, 51);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(669, 23);
            this.tbDescription.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(393, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Version :";
            // 
            // tbVersion
            // 
            this.tbVersion.Location = new System.Drawing.Point(450, 22);
            this.tbVersion.Name = "tbVersion";
            this.tbVersion.Size = new System.Drawing.Size(100, 23);
            this.tbVersion.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deployToolStripMenuItem,
            this.computeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // deployToolStripMenuItem
            // 
            this.deployToolStripMenuItem.Name = "deployToolStripMenuItem";
            this.deployToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.deployToolStripMenuItem.Text = "Deploy";
            this.deployToolStripMenuItem.Click += new System.EventHandler(this.deployToolStripMenuItem_Click);
            // 
            // computeToolStripMenuItem
            // 
            this.computeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.assemblyinfoToolStripMenuItem,
            this.assembliesjsonToolStripMenuItem});
            this.computeToolStripMenuItem.Name = "computeToolStripMenuItem";
            this.computeToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.computeToolStripMenuItem.Text = "Generate";
            // 
            // assemblyinfoToolStripMenuItem
            // 
            this.assemblyinfoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.forTheCurrentAssemblyToolStripMenuItem,
            this.forAllAssembliesToolStripMenuItem});
            this.assemblyinfoToolStripMenuItem.Name = "assemblyinfoToolStripMenuItem";
            this.assemblyinfoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.assemblyinfoToolStripMenuItem.Text = "assembly.info";
            // 
            // forTheCurrentAssemblyToolStripMenuItem
            // 
            this.forTheCurrentAssemblyToolStripMenuItem.Name = "forTheCurrentAssemblyToolStripMenuItem";
            this.forTheCurrentAssemblyToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.forTheCurrentAssemblyToolStripMenuItem.Text = "For the current assembly";
            this.forTheCurrentAssemblyToolStripMenuItem.Click += new System.EventHandler(this.forTheCurrentAssemblyToolStripMenuItem_Click);
            // 
            // forAllAssembliesToolStripMenuItem
            // 
            this.forAllAssembliesToolStripMenuItem.Name = "forAllAssembliesToolStripMenuItem";
            this.forAllAssembliesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.forAllAssembliesToolStripMenuItem.Text = "For all assemblies";
            this.forAllAssembliesToolStripMenuItem.Click += new System.EventHandler(this.forAllAssembliesToolStripMenuItem_Click);
            // 
            // assembliesjsonToolStripMenuItem
            // 
            this.assembliesjsonToolStripMenuItem.Name = "assembliesjsonToolStripMenuItem";
            this.assembliesjsonToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.assembliesjsonToolStripMenuItem.Text = "assemblies.json";
            this.assembliesjsonToolStripMenuItem.Click += new System.EventHandler(this.assembliesjsonToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvDependecies);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Release Management Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAssembly;
        private System.Windows.Forms.DataGridView dgvDependecies;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBinaryType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deployToolStripMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.DataGridViewTextBoxColumn DependecyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinimalVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaximalVersion;
        private System.Windows.Forms.ToolStripMenuItem computeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assemblyinfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forTheCurrentAssemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forAllAssembliesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assembliesjsonToolStripMenuItem;
    }
}

