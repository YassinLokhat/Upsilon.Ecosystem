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

        public MainForm()
        {
            InitializeComponent();

            this.cbAssembly.Items.Add(string.Empty);
            this.cbAssembly.Items.AddRange(MainForm.Core.Assemblies.Select(x => x.Name).ToArray());

            this.cbAssembly.SelectedIndexChanged += CbAssembly_SelectedIndexChanged;
        }

        private void CbAssembly_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvDependecies.Rows.Clear();

            YAssembly assembly = MainForm.Core.SelectAssembly(this.cbAssembly.Text);

            if (assembly == null)
            {
                return;
            }

            tbVersion.Text = assembly.Version;
            tbDescription.Text = assembly.Description;
            tbBinaryType.Text = assembly.BinaryType;
            tbUrl.Text = assembly.Url;

            foreach (YDependency dependency in assembly.Dependencies)
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
                MainForm.Core.Deploy(cbAssembly.Text, tbVersion.Text, tbDescription.Text, tbBinaryType.Text);
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
