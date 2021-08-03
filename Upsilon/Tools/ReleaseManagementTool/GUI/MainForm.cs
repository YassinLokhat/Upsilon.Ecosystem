﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Forms;
using Upsilon.Common.Library;
using Upsilon.Tools.ReleaseManagementTool.Core;

namespace Upsilon.Tools.ReleaseManagementTool.GUI
{
    public partial class MainForm : Form
    {
        private YAssembly _assembly = null;
        private bool _locked = false;

        public MainForm()
        {
            InitializeComponent();

            this._checkIntegrity();

            if (Program.Core.ConfigProvider.HasConfiguration(Config.OpenOutput))
            {
                ckbOpenOutput.Checked = Program.Core.ConfigProvider.GetConfiguration<bool>(Config.OpenOutput);
            }

            this.cbSolutions.Items.AddRange(Program.Core.Solutions.Select(x => Path.GetFileName(x)).ToArray());
            if(Program.Core.Solutions.Length != 0)
            {
                this.cbSolutions.SelectedIndex = 1;
            }

            if (Program.Core.SolutionAssemblies != null)
            {
                this.cbAssembly.Items.Add(string.Empty);
                this.cbAssembly.Items.AddRange(Program.Core.SolutionAssemblies.Select(x => x.Name).ToArray());
            }

            this.cbSolutions.SelectedIndexChanged += _cbSolutions_SelectedIndexChanged;
            this.cbAssembly.SelectedIndexChanged += _cbAssembly_SelectedIndexChanged;
            this.dgvDependecies.CellValueChanged += _dgvDependecies_CellValueChanged;
            this.ckbOpenOutput.CheckedChanged += _ckbOpenOutput_CheckedChanged;
        }

        private void _ckbOpenOutput_CheckedChanged(object sender, EventArgs e)
        {
            Program.Core.ConfigProvider.SetConfiguration(Config.OpenOutput, ckbOpenOutput.Checked);
        }

        private void _cbSolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._locked 
                || string.IsNullOrWhiteSpace(this.cbSolutions.Text))
            {
                return;
            }

            if (this.cbSolutions.SelectedIndex == 0)
            {
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Open a Visual Studio Solution",
                    Filter = "Visual Studio Solution|*.sln",
                };

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    this._loadSolution(Program.Core.Solutions[0]);
                    return;
                }

                this._loadSolution(openFileDialog.FileName);
            }
            else
            {
                try
                {
                    string solution = Program.Core.Solutions[this.cbSolutions.SelectedIndex - 1];
                    this._loadSolution(solution);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cbSolutions.Items.Remove(this.cbSolutions.Text);
                }
            }
        }

        private void _checkIntegrity()
        {
            while (true)
            {
                try
                {
                    Program.Core.CheckIntegrity();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("Dotfuscator : '"))
                    {
                        bDotfuscator.PerformClick();
                    }
                    else if (ex.Message.StartsWith("InnoSetup : '"))
                    {
                        bInnoSetup.PerformClick();
                    }
                    else if (ex.Message == "Server URL not set")
                    {
                        bServerUrl.PerformClick();
                    }
                }
            }
        }

        private void _loadSolution(string solution)
        {
            this._locked = true;

            Program.Core.LoadSolution(solution);
            this.cbSolutions.Items.Clear();
            this.cbSolutions.Items.Add("New Solution");
            this.cbSolutions.Items.AddRange(Program.Core.Solutions.Select(x => Path.GetFileName(x)).ToArray());
            this.cbSolutions.SelectedItem = Path.GetFileName(solution);

            this.cbAssembly.Items.Clear();
            this.cbAssembly.Items.Add(string.Empty);
            this.cbAssembly.Items.AddRange(Program.Core.SolutionAssemblies.Select(x => x.Name).ToArray());
            
            this._locked = false;
        }

        private void _dgvDependecies_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this._assembly == null)
            {
                return;
            }

            switch (e.ColumnIndex)
            {
                case 1:
                    this._assembly.Dependencies[e.RowIndex].MinimalVersion = dgvDependecies.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    break;
                case 2:
                    this._assembly.Dependencies[e.RowIndex].MaximalVersion = dgvDependecies.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    break;
            }
        }

        private void _cbAssembly_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvDependecies.Rows.Clear();

            this._assembly = Program.Core.SelectAssembly(this.cbAssembly.Text);

            if (this._assembly == null)
            {
                return;
            }

            tbVersion.Text = this._assembly.Version;
            tbDescription.Text = this._assembly.Description;
            tbBinaryType.Text = this._assembly.BinaryType;
            tbUrl.Text = this._assembly.Url;

            foreach (YDependency dependency in this._assembly.Dependencies)
            {
                this.dgvDependecies.Rows.Add(dependency.Name, dependency.MinimalVersion, dependency.MaximalVersion);
            }
        }

        private void _bDeploy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbAssembly.Text))
            {
                return;
            }

            try
            {
                this._assembly.Version = tbVersion.Text;
                this._assembly.Description = tbDescription.Text;
                this._assembly.BinaryType = tbBinaryType.Text;
                Program.Core.Deploy(this._assembly);
                MessageBox.Show("Deployment success.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _bOpenRepository_Click(object sender, EventArgs e)
        {
            Program.Core.OpenRepository();
        }

        private void _bDotfuscator_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Browse for Dotfuscator",
                Filter = "Dotfuscator|dotfuscatorUI.exe",
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (File.Exists(openFileDialog.FileName))
            {
                Program.Core.ConfigProvider.SetConfiguration(Config.Dotfuscaor, openFileDialog.FileName);
            }
        }

        private void _bInnoSetup_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Browse for InnoSetup",
                Filter = "InnoSetup|ISCC.exe",
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (File.Exists(openFileDialog.FileName))
            {
                Program.Core.ConfigProvider.SetConfiguration(Config.InnoSetup, openFileDialog.FileName);
            }
        }

        private void _bServerUrl_Click(object sender, EventArgs e)
        {
            if (YInputBox.ShowDialog("Server URL", "Set the Server URL", out string url, YInputBox.YInputType.TextBox) == DialogResult.OK
                && !string.IsNullOrWhiteSpace(url))
            {
                Program.Core.ConfigProvider.SetConfiguration(Config.ServerUrl, url);
            }
        }
    }
}
