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

        public new Icon Icon
        {
            get { return base.Icon; }
            set
            {
                base.Icon = value;
                pbIcon.Image = base.Icon.ToBitmap();
            }
        }

        public YForm()
        {
            this.InitializeComponent();

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 30, 30));

            this.Refresh();

            this.TextChanged += YForm_TextChanged;
            this.SizeChanged += YForm_SizeChanged;

            pBar.MouseUp += Form1_MouseUp;
            pBar.MouseDown += Form1_MouseDown;
            pBar.MouseMove += Form1_MouseMove;
            pBar.DoubleClick += BMaximize_Click;
            lTitle.DoubleClick += BMaximize_Click;

            bMaximize.Click += BMaximize_Click;
            bMinimize.Click += BMinimize_Click;
            bClose.Click += BClose_Click;
        }

        private void YForm_SizeChanged(object sender, EventArgs e)
        {
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 30, 30));
        }

        private void BMaximize_Click(object sender, EventArgs e)
        {
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void BMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void YForm_TextChanged(object sender, EventArgs e)
        {
            this.lTitle.Text = this.Text;
        }

        public void Refresh(Control control = null)
        {
            if (control == null)
            {
                pBar.ForeColor = lTitle.ForeColor = this.ForegroundColor;
                pBar.BackColor = lTitle.BackColor = this.BackgroundColor;
                bClose.ForeColor = bMinimize.ForeColor = bMaximize.ForeColor = SystemColors.ControlText;
                bClose.BackColor = bMinimize.BackColor = bMaximize.BackColor = SystemColors.Control;

                control = this.pContainer;
            }

            control.ForeColor = this.ForegroundColor;
            control.BackColor = this.BackgroundColor;

            control.Controls.Cast<Control>().ToList().ForEach(x => { this.Refresh(x); });
        }

        private bool inMove = false;
        private Point mouse = new Point();
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (inMove)
            {
                this.WindowState = FormWindowState.Normal;
                Point temp = new Point
                {
                    X = this.Location.X - (mouse.X - e.X),
                    Y = this.Location.Y - (mouse.Y - e.Y)
                };
                this.Location = temp;
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (inMove)
            {
                mouse.X = this.Location.X + e.X - mouse.X;
                mouse.Y = this.Location.Y + e.Y - mouse.Y;

                this.Location = mouse;
            }
            inMove = false;
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            inMove = true;
            mouse = e.Location;
        }
    }
}
