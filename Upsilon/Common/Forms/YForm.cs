using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    public partial class YForm : Form
    {
        public new Icon Icon
        {
            get
            {
                return base.Icon;
            }
            set
            {
                base.Icon = value;

                _icon_PB.Visible = value is not null;
                _icon_PB.Image = value?.ToBitmap();
            }
        }
        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = _title_L.Text = value;
            }
        }
        public new bool MinimizeBox
        {
            get
            {
                return base.MinimizeBox;
            }
            set
            {
                base.MinimizeBox = _minimize_B.Visible = value;
            }
        }

        private bool _inMove = false;
        private Point _mouse = new Point();

        public YForm()
        {
            InitializeComponent();

            _minimize_B.Click += _minimize_B_Click;
            _close_B.Click += _close_B_Click;

            this.MouseUp += _mouseUp;
            this.MouseDown += _mouseDown;
            this.MouseMove += _mouseMove;
            _title_L.MouseUp += _mouseUp;
            _title_L.MouseDown += _mouseDown;
            _title_L.MouseMove += _mouseMove;
        }

        public void SetMainControl(Control control)
        {
            this.Text = control.Text;

            this.ControlsPanel.SuspendLayout();
            this.ControlsPanel.Controls.Add(control);
            this.ControlsPanel.ResumeLayout(false);

            this.MaximumSize = this.MinimumSize = this.Size = new Size()
            {
                Width = this.Width - this.ControlsPanel.Width + control.Width,
                Height = this.Height - this.ControlsPanel.Height + control.Height,
            };

            control.TextChanged += Control_TextChanged;
        }

        private void Control_TextChanged(object? sender, EventArgs e)
        {
            var control = sender as Control;
            this.Text = control.Text;
        }

        private void _minimize_B_Click(object? sender, EventArgs e)
        {
            if (this.MinimizeBox)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        private void _close_B_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void _mouseMove(object? sender, MouseEventArgs e)
        {
            if (_inMove)
            {
                Point temp = new Point
                {
                    X = this.Location.X - (_mouse.X - e.X),
                    Y = this.Location.Y - (_mouse.Y - e.Y)
                };
                this.Location = temp;
            }
        }
        private void _mouseUp(object? sender, MouseEventArgs e)
        {
            if (_inMove)
            {
                _mouse.X = this.Location.X + e.X - _mouse.X;
                _mouse.Y = this.Location.Y + e.Y - _mouse.Y;

                this.Location = _mouse;
            }
            _inMove = false;
        }
        private void _mouseDown(object? sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            _inMove = true;
            _mouse = e.Location;
        }
    }
}
