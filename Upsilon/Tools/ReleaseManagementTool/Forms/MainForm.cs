using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Library;
using Upsilon.Tools.ReleaseManagementTool.Core;

namespace Upsilon.Tools.ReleaseManagementTool.Forms
{
    public partial class MainForm : Form
    {
        private static YReleaseManagementToolCore _core = null;
        public static YReleaseManagementToolCore Core
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

        private YAssembly _assembly = null;

        public MainForm()
        {
            InitializeComponent();

            this.cbAssembly.Items.Add(string.Empty);
            this.cbAssembly.Items.AddRange(MainForm.Core.Assemblies.Select(x => x.Name).ToArray());

            this.cbAssembly.SelectedIndexChanged += CbAssembly_SelectedIndexChanged;
            this.dgvDependecies.CellValueChanged += DgvDependecies_CellValueChanged;
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

            this._assembly = MainForm.Core.SelectAssembly(this.cbAssembly.Text);

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

        private void deployToolStripMenuItem_Click(object sender, EventArgs e)
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
                MainForm.Core.Deploy(this._assembly);
                MessageBox.Show($"Assembly '{cbAssembly.Text}' deployed successfully.", "Deployment success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void assembliesjsonToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void forTheCurrentAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.Core.GenerateAssemblyInfo(this.cbAssembly.Text);
            MessageBox.Show($"assembly.info for '{this.cbAssembly.Text}' generated successfully.", "Generation success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void forAllAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.Core.GenerateAssemblyInfo();
            MessageBox.Show($"assembly.info for all assemblies generated successfully.", "Generation success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
