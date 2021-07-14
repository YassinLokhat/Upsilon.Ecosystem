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

            //foreach (string[] dep in dependecies)
            //{
            //    this.dgvDependecies.Rows.Add(dep);
            //}
        }
    }
}
