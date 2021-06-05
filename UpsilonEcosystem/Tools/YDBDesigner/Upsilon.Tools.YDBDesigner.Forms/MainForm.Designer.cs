
namespace Upsilon.Tools.YDBDesigner.Forms
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tscbTables = new System.Windows.Forms.ToolStripComboBox();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildInternalIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTableDefinition = new System.Windows.Forms.GroupBox();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbTableData = new System.Windows.Forms.GroupBox();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.ColumnInternalIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            this.gbTableDefinition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.gbTableData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.tablesToolStripMenuItem,
            this.tscbTables});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1375, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "menu_file";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.openToolStripMenuItem.Text = "menu_file_open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.saveToolStripMenuItem.Text = "menu_file_save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.saveAsToolStripMenuItem.Text = "menu_file_save_as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.quitToolStripMenuItem.Text = "menu_file_quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(30, 24);
            this.helpToolStripMenuItem.Text = "menu_help";
            // 
            // tablesToolStripMenuItem
            // 
            this.tablesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.rebuildInternalIndexToolStripMenuItem});
            this.tablesToolStripMenuItem.Name = "tablesToolStripMenuItem";
            this.tablesToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.tablesToolStripMenuItem.Text = "menu_tables";
            // 
            // tscbTables
            // 
            this.tscbTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbTables.Name = "tscbTables";
            this.tscbTables.Size = new System.Drawing.Size(121, 28);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(237, 26);
            this.addToolStripMenuItem.Text = "menu_tables_add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(237, 26);
            this.renameToolStripMenuItem.Text = "menu_tables_rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(237, 26);
            this.deleteToolStripMenuItem.Text = "menu_tables_delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // rebuildInternalIndexToolStripMenuItem
            // 
            this.rebuildInternalIndexToolStripMenuItem.Name = "rebuildInternalIndexToolStripMenuItem";
            this.rebuildInternalIndexToolStripMenuItem.Size = new System.Drawing.Size(237, 26);
            this.rebuildInternalIndexToolStripMenuItem.Text = "menu_tables_rebuild";
            this.rebuildInternalIndexToolStripMenuItem.Click += new System.EventHandler(this.rebuildInternalIndexToolStripMenuItem_Click);
            // 
            // gbTableDefinition
            // 
            this.gbTableDefinition.Controls.Add(this.dgvFields);
            this.gbTableDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTableDefinition.Location = new System.Drawing.Point(0, 0);
            this.gbTableDefinition.Name = "gbTableDefinition";
            this.gbTableDefinition.Size = new System.Drawing.Size(1375, 189);
            this.gbTableDefinition.TabIndex = 1;
            this.gbTableDefinition.TabStop = false;
            this.gbTableDefinition.Text = "group_table_definition";
            // 
            // dgvFields
            // 
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnType});
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.Location = new System.Drawing.Point(3, 23);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.RowHeadersWidth = 51;
            this.dgvFields.RowTemplate.Height = 29;
            this.dgvFields.Size = new System.Drawing.Size(1369, 163);
            this.dgvFields.TabIndex = 0;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnName.HeaderText = "column_field_name";
            this.ColumnName.MinimumWidth = 6;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.Width = 114;
            // 
            // ColumnType
            // 
            this.ColumnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnType.HeaderText = "column_field_type";
            this.ColumnType.MinimumWidth = 6;
            this.ColumnType.Name = "ColumnType";
            // 
            // gbTableData
            // 
            this.gbTableData.Controls.Add(this.dgvRecords);
            this.gbTableData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTableData.Location = new System.Drawing.Point(0, 0);
            this.gbTableData.Name = "gbTableData";
            this.gbTableData.Size = new System.Drawing.Size(1375, 426);
            this.gbTableData.TabIndex = 2;
            this.gbTableData.TabStop = false;
            this.gbTableData.Text = "group_table_data";
            // 
            // dgvRecords
            // 
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnInternalIndex});
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(3, 23);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.RowHeadersWidth = 51;
            this.dgvRecords.RowTemplate.Height = 29;
            this.dgvRecords.Size = new System.Drawing.Size(1369, 400);
            this.dgvRecords.TabIndex = 0;
            // 
            // ColumnInternalIndex
            // 
            this.ColumnInternalIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnInternalIndex.Frozen = true;
            this.ColumnInternalIndex.HeaderText = "column_internal_index";
            this.ColumnInternalIndex.MinimumWidth = 6;
            this.ColumnInternalIndex.Name = "ColumnInternalIndex";
            this.ColumnInternalIndex.ReadOnly = true;
            this.ColumnInternalIndex.Width = 128;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbTableDefinition);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbTableData);
            this.splitContainer1.Size = new System.Drawing.Size(1375, 619);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1375, 649);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "application_title";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbTableDefinition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.gbTableData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox tscbTables;
        private System.Windows.Forms.GroupBox gbTableDefinition;
        private System.Windows.Forms.GroupBox gbTableData;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInternalIndex;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebuildInternalIndexToolStripMenuItem;
    }
}

