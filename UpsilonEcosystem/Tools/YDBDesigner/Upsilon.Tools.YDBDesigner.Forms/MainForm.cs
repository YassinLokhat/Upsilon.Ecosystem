using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Tools.YDBDesigner.Core;

namespace Upsilon.Tools.YDBDesigner.Forms
{
    public partial class MainForm : Form
    {
        private Core.Core _core = null;

        public MainForm()
        {
            InitializeComponent();

            this._core = new Core.Core();
        }
    }
}
