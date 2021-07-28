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

        public MainForm()
        {
            InitializeComponent();

            this.cbSolutions.Items.AddRange(Program.Core.Solutions);
            if(File.Exists(Program.Core.Solutions.FirstOrDefault()))
            {
                this.cbSolutions.SelectedItem = Program.Core.Solutions.FirstOrDefault();
            }

            this.cbSolutions.SelectedIndexChanged += CbSolutions_SelectedIndexChanged;
            this.cbAssembly.SelectedIndexChanged += CbAssembly_SelectedIndexChanged;
            this.dgvDependecies.CellValueChanged += DgvDependecies_CellValueChanged;
        }

        private void CbSolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.cbSolutions.Text))
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
                    this.cbSolutions.Text = string.Empty;
                    return;
                }

                this.cbSolutions.Items.Add(openFileDialog.FileName);
                this.cbSolutions.SelectedItem = openFileDialog.FileName;
            }
            else
            {
                try
                {
                    Program.Core.LoadSolution(this.cbSolutions.Text);

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

        private void DgvDependecies_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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

        private void CbAssembly_SelectedIndexChanged(object sender, EventArgs e)
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

        private void bDeploy_Click(object sender, EventArgs e)
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
    }
}
