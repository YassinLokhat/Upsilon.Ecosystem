using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Forms;
using Upsilon.Tools.YDBDesigner.Core;

namespace Upsilon.Tools.YDBDesigner.Forms
{
    public partial class MainForm : Form
    {
        private static YDBDesignerCore _core = null;
        public static YDBDesignerCore Core
        {
            get
            {
                if (MainForm._core == null)
                {
                    MainForm._core = new();
                }

                return MainForm._core;
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.Translate();

            tscbTables.SelectedIndexChanged += TscbTables_SelectedIndexChanged;

            if (args.Length > 0)
            {
                string filename = args[0];
                string key = string.Empty;

                if (args.Length > 1)
                {
                    key = args[1];
                }
                else
                {
                    if (YInputBox.ShowDialog(MainForm.Core.Translator["database_key"], MainForm.Core.Translator["ask_database_key"], out key, YInputBox.YInputType.Password) != DialogResult.OK)
                    {
                        Environment.Exit(0);
                    }
                }

                tscbTables.Items.Clear();
                tscbTables.Items.AddRange(MainForm.Core.Open(filename, key));
                tscbTables.SelectedIndex = 0;
            }

            this.FormClosing += MainForm_FormClosing;
        }

        private void TscbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvFields.Rows.Clear();
            dgvRecords.Rows.Clear();
            for (int i = 1; dgvRecords.Columns.Count != 1;)
            {
                dgvRecords.Columns.RemoveAt(i);
            }

            string[][] tableDefinition = MainForm.Core.GetTableDefinition(tscbTables.SelectedItem.ToString());
            foreach (string[] row in tableDefinition)
            {
                dgvFields.Rows.Add(row);

                dgvRecords.Columns.Add(row[0], row[0]);
                dgvRecords.Columns[row[0]].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            string[][] tableData = MainForm.Core.GetTableData(tscbTables.SelectedItem.ToString());
            foreach (string[] row in tableData)
            {
                dgvRecords.Rows.Add(row);
            }
        }

        private void Translate()
        {
            this.Text = MainForm.Core.Translator["application_title"];
            this.fileToolStripMenuItem.Text = MainForm.Core.Translator["menu_file"];
            this.openToolStripMenuItem.Text = MainForm.Core.Translator["menu_file_open"];
            this.saveToolStripMenuItem.Text = MainForm.Core.Translator["menu_file_save"];
            this.saveAsToolStripMenuItem.Text = MainForm.Core.Translator["menu_file_save_as"];
            this.quitToolStripMenuItem.Text = MainForm.Core.Translator["menu_file_quit"];
            this.helpToolStripMenuItem.Text = MainForm.Core.Translator["menu_help"];
            this.tablesToolStripMenuItem.Text = MainForm.Core.Translator["menu_tables"];
            this.addToolStripMenuItem.Text = MainForm.Core.Translator["menu_tables_add"];
            this.renameToolStripMenuItem.Text = MainForm.Core.Translator["menu_tables_rename"];
            this.deleteToolStripMenuItem.Text = MainForm.Core.Translator["menu_tables_delete"];
            this.rebuildInternalIndexToolStripMenuItem.Text = MainForm.Core.Translator["menu_tables_rebuild"];
            this.gbTableDefinition.Text = MainForm.Core.Translator["group_table_definition"];
            this.ColumnName.HeaderText = MainForm.Core.Translator["column_field_name"];
            this.ColumnType.HeaderText = MainForm.Core.Translator["column_field_type"];
            this.gbTableData.Text = MainForm.Core.Translator["group_table_data"];
            this.ColumnInternalIndex.HeaderText = MainForm.Core.Translator["column_internal_index"];
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = MainForm.Core.Translator["open_database_file"],
                Filter = $"{MainForm.Core.Translator["upsilon_database_file"]}|*.ydb",
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (YInputBox.ShowDialog(MainForm.Core.Translator["database_key"], MainForm.Core.Translator["ask_database_key"], out string key, YInputBox.YInputType.Password) != DialogResult.OK)
            {
                return;
            }

            tscbTables.Items.Clear();
            tscbTables.Items.AddRange(MainForm.Core.Open(openFileDialog.FileName, key));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.Core.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = MainForm.Core.Translator["save_database_file"],
                Filter = $"{MainForm.Core.Translator["upsilon_database_file"]}|*.ydb",
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (YInputBox.ShowDialog(MainForm.Core.Translator["database_key"], MainForm.Core.Translator["ask_database_key"], out string key, YInputBox.YInputType.Password) != DialogResult.OK)
            {
                return;
            }

            MainForm.Core.SaveAs(saveFileDialog.FileName, key);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Core.Close();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (YInputBox.ShowDialog(MainForm.Core.Translator["database_key"], MainForm.Core.Translator["ask_database_key"], out string tablename, YInputBox.YInputType.TextBox) != DialogResult.OK)
            {
                return;
            }

            MainForm.Core.AddTable(tablename);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (YInputBox.ShowDialog(MainForm.Core.Translator["database_key"], MainForm.Core.Translator["ask_database_key"], out string tablename, YInputBox.YInputType.TextBox) != DialogResult.OK)
            {
                return;
            }

            MainForm.Core.RenameTable(tscbTables.SelectedItem.ToString(), tablename);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.Core.DeleteTable(tscbTables.SelectedItem.ToString());
        }

        private void rebuildInternalIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.Core.RebuildInternalIndex(tscbTables.SelectedItem.ToString());
        }
    }
}
