﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

            YVersion version = new(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            this.Text = $"Release Management Tool v{version.ToString(YVersionFormat.Simple)}";

            this.cbBinaryType.Items.AddRange(YStaticMethods.GetEnumValues<YBinaryType>().Select(x => x.ToString()).ToArray());

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
                    if (Program.Core.Solutions.Any())
                    {
                        this._loadSolution(Program.Core.Solutions[0]); 
                    }
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
                    else if (ex.Message == "Deployed Assemblies json not set")
                    {
                        bServerUrl.PerformClick();
                    }
                    else if (ex.Message == "Upload Tool not set")
                    {
                        bUploadTool.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
        }

        private void _loadSolution(string solution)
        {
            try
            {
                this._locked = true;

                this.cbAssembly.Items.Clear();
                this.cbAssembly.Items.Add(string.Empty);

                Program.Core.LoadSolution(solution);
                this.cbSolutions.Items.Clear();
                this.cbSolutions.Items.Add("New Solution");
                this.cbSolutions.Items.AddRange(Program.Core.Solutions.Select(x => Path.GetFileName(x)).ToArray());
                this.cbSolutions.SelectedItem = Path.GetFileName(solution);

                this.cbAssembly.Items.AddRange(Program.Core.SolutionAssemblies.Select(x => x.Name).ToArray());
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
            finally
            {
                this._locked = false;
            }
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
            cbBinaryType.SelectedItem = this._assembly.BinaryType.ToString();
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
                this._assembly.BinaryType = (YBinaryType)Enum.Parse(typeof(YBinaryType), cbBinaryType.Text);
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
            Program.Core.OpenUploadTool();
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
            string url = Program.Core.ConfigProvider.GetConfiguration<string>(Config.DeployedAssemblies);
            if (YInputBox.ShowDialog("Deployed Assemblies Json", "Set the Deployed Assemblies Json URL", ref url, YInputBox.YInputType.TextBox) == DialogResult.OK
                && !string.IsNullOrWhiteSpace(url))
            {
                Program.Core.ConfigProvider.SetConfiguration(Config.DeployedAssemblies, url);
            }
        }

        private void _bDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbAssembly.Text))
            {
                return;
            }

            if (!Program.Core.DeployedAssemblies.ContainsKey(cbAssembly.Text))
            {
                MessageBox.Show($"The '{cbAssembly.Text}' is missing in the deployed assembly list.", "Missing assembly", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            YAssembly assembly = Program.Core.DeployedAssemblies[cbAssembly.Text].Find(x => x.Version == tbVersion.Text);

            if (assembly == null)
            {
                MessageBox.Show($"The version '{tbVersion.Text}' of the '{cbAssembly.Text}' is missing in the deployed assembly list.", "Missing version", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FolderBrowserDialog folderBrowserDialog = new()
            {
                Description = $"Download {assembly.Name} v{assembly.Version}",
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                Program.Core.DownloadAssembly(assembly, folderBrowserDialog.SelectedPath);

                if (Program.Core.ConfigProvider.GetConfiguration<bool>(Config.OpenOutput))
                {
                    Process.Start("explorer.exe", "\"" + folderBrowserDialog.SelectedPath + "\"");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _bRepository_Click(object sender, EventArgs e)
        {
            string url = Program.Core.ConfigProvider.GetConfiguration<string>(Config.UploadTool);
            if (YInputBox.ShowDialog("Upload Tool", "Set the Upload Tool", ref url, YInputBox.YInputType.TextBox) == DialogResult.OK
                && !string.IsNullOrWhiteSpace(url))
            {
                Program.Core.ConfigProvider.SetConfiguration(Config.UploadTool, url);
            }
        }
    }
}
