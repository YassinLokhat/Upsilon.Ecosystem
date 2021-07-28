using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            if (Program.Core.ConfigProvider.HasConfiguration(Config.OpenOutput))
            {
                ckbOpenOutput.Checked = Program.Core.ConfigProvider.GetConfiguration<bool>(Config.OpenOutput);
            }

            this.cbSolutions.Items.AddRange(Program.Core.Solutions.Select(x => Path.GetFileName(x)).ToArray());
            if(Program.Core.Solutions.Length != 0)
            {
                this.cbSolutions.SelectedIndex = 1;
            }

            if (Program.Core.Assemblies != null)
            {
                this.cbAssembly.Items.Add(string.Empty);
                this.cbAssembly.Items.AddRange(Program.Core.Assemblies.Select(x => x.Name).ToArray());
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

                    this.cbAssembly.Items.Clear();
                    this.cbAssembly.Items.Add(string.Empty);
                    this.cbAssembly.Items.AddRange(Program.Core.Assemblies.Select(x => x.Name).ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cbSolutions.Items.Remove(this.cbSolutions.Text);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _bOpenRepository_Click(object sender, EventArgs e)
        {
            YReleaseManagementToolCore.OpenRepository();
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
    }
}
