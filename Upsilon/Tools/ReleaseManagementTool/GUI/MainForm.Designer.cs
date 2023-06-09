﻿
namespace Upsilon.Tools.ReleaseManagementTool.GUI
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
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.cbBinaryType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbVersion = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cbSolutions = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bDeploy = new System.Windows.Forms.ToolStripButton();
            this.bDownload = new System.Windows.Forms.ToolStripButton();
            this.bComputeDeployedAssembliesJson = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.ckbOpenOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.bDotfuscator = new System.Windows.Forms.ToolStripMenuItem();
            this.bInnoSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.bServerUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.lbRequiredFiles = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.cbAssembly.Size = new System.Drawing.Size(329, 23);
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
            this.dgvDependecies.RowHeadersWidth = 51;
            this.dgvDependecies.RowTemplate.Height = 25;
            this.dgvDependecies.Size = new System.Drawing.Size(910, 223);
            this.dgvDependecies.TabIndex = 2;
            // 
            // DependecyName
            // 
            this.DependecyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DependecyName.HeaderText = "Dependency Name";
            this.DependecyName.MinimumWidth = 6;
            this.DependecyName.Name = "DependecyName";
            this.DependecyName.ReadOnly = true;
            // 
            // MinimalVersion
            // 
            this.MinimalVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MinimalVersion.HeaderText = "Minimal Version";
            this.MinimalVersion.MinimumWidth = 6;
            this.MinimalVersion.Name = "MinimalVersion";
            this.MinimalVersion.Width = 107;
            // 
            // MaximalVersion
            // 
            this.MaximalVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MaximalVersion.HeaderText = "Maximal Version";
            this.MaximalVersion.MinimumWidth = 6;
            this.MaximalVersion.Name = "MaximalVersion";
            this.MaximalVersion.Width = 109;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbDescription);
            this.groupBox1.Controls.Add(this.cbBinaryType);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbUrl);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbVersion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbAssembly);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(910, 115);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assembly info";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(85, 51);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(819, 23);
            this.tbDescription.TabIndex = 4;
            // 
            // cbBinaryType
            // 
            this.cbBinaryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBinaryType.FormattingEnabled = true;
            this.cbBinaryType.Location = new System.Drawing.Point(691, 22);
            this.cbBinaryType.Name = "cbBinaryType";
            this.cbBinaryType.Size = new System.Drawing.Size(213, 23);
            this.cbBinaryType.TabIndex = 10;
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
            this.tbUrl.Size = new System.Drawing.Size(819, 23);
            this.tbUrl.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(613, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Binary type :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Description :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(436, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Version :";
            // 
            // tbVersion
            // 
            this.tbVersion.Location = new System.Drawing.Point(493, 22);
            this.tbVersion.Name = "tbVersion";
            this.tbVersion.Size = new System.Drawing.Size(100, 23);
            this.tbVersion.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cbSolutions,
            this.toolStripSeparator1,
            this.bDeploy,
            this.bDownload,
            this.bComputeDeployedAssembliesJson,
            this.toolStripSeparator2,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(934, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(57, 22);
            this.toolStripLabel1.Text = "Solution :";
            // 
            // cbSolutions
            // 
            this.cbSolutions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSolutions.Items.AddRange(new object[] {
            "New Solution"});
            this.cbSolutions.MaxDropDownItems = 100;
            this.cbSolutions.Name = "cbSolutions";
            this.cbSolutions.Size = new System.Drawing.Size(250, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bDeploy
            // 
            this.bDeploy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bDeploy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bDeploy.Name = "bDeploy";
            this.bDeploy.Size = new System.Drawing.Size(48, 22);
            this.bDeploy.Text = "Deploy";
            this.bDeploy.Click += new System.EventHandler(this._bDeploy_Click);
            // 
            // bDownload
            // 
            this.bDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bDownload.Image = ((System.Drawing.Image)(resources.GetObject("bDownload.Image")));
            this.bDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bDownload.Name = "bDownload";
            this.bDownload.Size = new System.Drawing.Size(65, 22);
            this.bDownload.Text = "Download";
            this.bDownload.Click += new System.EventHandler(this._bDownload_Click);
            // 
            // bComputeDeployedAssembliesJson
            // 
            this.bComputeDeployedAssembliesJson.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bComputeDeployedAssembliesJson.Image = ((System.Drawing.Image)(resources.GetObject("bComputeDeployedAssembliesJson.Image")));
            this.bComputeDeployedAssembliesJson.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bComputeDeployedAssembliesJson.Name = "bComputeDeployedAssembliesJson";
            this.bComputeDeployedAssembliesJson.Size = new System.Drawing.Size(201, 22);
            this.bComputeDeployedAssembliesJson.Text = "Compute Deployed.Assemblies.json";
            this.bComputeDeployedAssembliesJson.Click += new System.EventHandler(this._bComputeDeployedAssembliesJson_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ckbOpenOutput,
            this.bDotfuscator,
            this.bInnoSetup,
            this.bServerUrl});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton1.Text = "Settings";
            // 
            // ckbOpenOutput
            // 
            this.ckbOpenOutput.CheckOnClick = true;
            this.ckbOpenOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ckbOpenOutput.Name = "ckbOpenOutput";
            this.ckbOpenOutput.Size = new System.Drawing.Size(275, 22);
            this.ckbOpenOutput.Text = "Open output folder";
            // 
            // bDotfuscator
            // 
            this.bDotfuscator.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bDotfuscator.Name = "bDotfuscator";
            this.bDotfuscator.Size = new System.Drawing.Size(275, 22);
            this.bDotfuscator.Text = "Browse for Dotfuscator";
            this.bDotfuscator.Click += new System.EventHandler(this._bDotfuscator_Click);
            // 
            // bInnoSetup
            // 
            this.bInnoSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bInnoSetup.Name = "bInnoSetup";
            this.bInnoSetup.Size = new System.Drawing.Size(275, 22);
            this.bInnoSetup.Text = "Browse for InnoSetup";
            this.bInnoSetup.Click += new System.EventHandler(this._bInnoSetup_Click);
            // 
            // bServerUrl
            // 
            this.bServerUrl.Name = "bServerUrl";
            this.bServerUrl.Size = new System.Drawing.Size(275, 22);
            this.bServerUrl.Text = "Set the Deployed Assemblies Json URL";
            this.bServerUrl.Click += new System.EventHandler(this._bServerUrl_Click);
            // 
            // lbRequiredFiles
            // 
            this.lbRequiredFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRequiredFiles.FormattingEnabled = true;
            this.lbRequiredFiles.ItemHeight = 15;
            this.lbRequiredFiles.Location = new System.Drawing.Point(3, 18);
            this.lbRequiredFiles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbRequiredFiles.Name = "lbRequiredFiles";
            this.lbRequiredFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbRequiredFiles.Size = new System.Drawing.Size(900, 148);
            this.lbRequiredFiles.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbRequiredFiles);
            this.groupBox2.Location = new System.Drawing.Point(10, 376);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(906, 168);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Required files";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 559);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvDependecies);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(950, 598);
            this.MinimumSize = new System.Drawing.Size(950, 598);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Release Management Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDependecies)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.DataGridViewTextBoxColumn DependecyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinimalVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaximalVersion;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbSolutions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton bDeploy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem ckbOpenOutput;
        private System.Windows.Forms.ToolStripMenuItem bDotfuscator;
        private System.Windows.Forms.ToolStripMenuItem bInnoSetup;
        private System.Windows.Forms.ToolStripMenuItem bServerUrl;
        private System.Windows.Forms.ToolStripButton bDownload;
        private System.Windows.Forms.ComboBox cbBinaryType;
        private System.Windows.Forms.ListBox lbRequiredFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripButton bComputeDeployedAssembliesJson;
    }
}

