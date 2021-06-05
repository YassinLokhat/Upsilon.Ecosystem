using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    public partial class YForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        private Color _foregroundColor = Color.White;
        public Color ForegroundColor
        {
            get
            {
                return this._foregroundColor;
            }
            set
            {
                this._foregroundColor = value;
                this.Refresh();
            }
        }

        private Color _backgroundColor = Color.Black;
        public Color BackgroundColor
        {
            get
            {
                return this._backgroundColor;
            }
            set
            {
                this._backgroundColor = value;
                this.Refresh();
            }
        }

        public YForm()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 30, 30));

            this.Refresh();
        }

        public new void Refresh()
        {
            this.ForeColor = this.ForegroundColor;
            this.BackColor = this.BackgroundColor;

            foreach (Control control in this.Controls)
            {
                control.ForeColor = this.ForegroundColor;
                control.BackColor = this.BackgroundColor;
            }
        }
    }
}
