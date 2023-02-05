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
    public partial class YPrivateTextBox : UserControl
    {
        [Category("Private Text"), Description("The private text.")]
        public new string Text
        {
            get { return _privateText_TB.Text; }
            set { _privateText_TB.Text = value; }
        }

        private char _privateChar = '\0';
        [Category("Private Text"), Description("The private text character to be displayed.")]
        public char PasswordChar
        {
            get { return _privateChar; }
            set
            {
                _privateChar = value;

                if (_privateText_TB.PasswordChar != '\0'
                    || !_privateTextVisible)
                {
                    _privateText_TB.PasswordChar = _privateChar;
                }

                _showPrivateBox_PB.Visible = _privateChar != '\0';
            }
        }

        private bool _holdToShow = true;
        [Category("Private Text"), Description("Hold the button to show the private text.")]
        public bool HoldToShow
        {
            get { return _holdToShow; }
            set { _holdToShow = value; }
        }

        private bool _useDarkIcon = false;
        [Category("Private Text"), Description("Use the dark item.")]
        public bool UseDarkIcon
        {
            get
            {
                return _useDarkIcon;
            }
            set 
            {
                _useDarkIcon = value;
                _showPrivateBox_PB.Image = _releasedButtonImage = _useDarkIcon ? Properties.Resources.BlackEye : Properties.Resources.WhiteEye;
                _pressedButtonImage = _useDarkIcon ? Properties.Resources.WhiteEye : Properties.Resources.BlackEye;
            }
        }

        private bool _privateTextVisible = false;
        private Bitmap _releasedButtonImage = Properties.Resources.BlackEye;
        private Bitmap _pressedButtonImage = Properties.Resources.WhiteEye;

        public YPrivateTextBox()
        {
            InitializeComponent();

            _forwardAllEvent();

            this.BackColorChanged += PrivateTextBox_BackColorChanged;
            this.ForeColorChanged += PrivateTextBox_ForeColorChanged;

            _showPrivateBox_PB.MouseMove += _showPrivateBox_PB_MouseMove;
            _showPrivateBox_PB.MouseUp += _showPrivateText_B_MouseUp;
            _showPrivateBox_PB.MouseDown += _showPrivateText_B_MouseDown;
            _showPrivateBox_PB.Click += _showPrivateText_B_Click;
        }

        private void PrivateTextBox_BackColorChanged(object? sender, EventArgs e)
        {
            _privateText_TB.BackColor = this.BackColor;
        }

        private void PrivateTextBox_ForeColorChanged(object? sender, EventArgs e)
        {
            _privateText_TB.ForeColor = this.ForeColor;
        }

        private void _showPrivateBox_PB_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!HoldToShow
                || !_privateTextVisible)
            {
                return;
            }

            if (!_showPrivateBox_PB.DisplayRectangle.Contains(e.Location))
            {
                _hidePrivateText();
            }
        }

        private void _showPrivateText_B_MouseUp(object? sender, EventArgs e)
        {
            if (!HoldToShow)
            {
                return;
            }

            _hidePrivateText();
        }

        private void _showPrivateText_B_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!HoldToShow)
            {
                return;
            }

            _showPrivateText();
        }

        private void _showPrivateText_B_Click(object? sender, EventArgs e)
        {
            if (HoldToShow)
            {
                return;
            }

            if (!_privateTextVisible)
            {
                _showPrivateText();
            }
            else
            {
                _hidePrivateText();
            }
        }

        private void _showPrivateText()
        {
            _privateTextVisible = true;

            _showPrivateBox_PB.Image = _pressedButtonImage;
            _privateText_TB.PasswordChar = '\0';
        }

        private void _hidePrivateText()
        {
            _privateTextVisible = false;

            _showPrivateBox_PB.Image = _releasedButtonImage;
            _privateText_TB.PasswordChar = PasswordChar;
        }

        private void _forwardAllEvent()
        {
            //_privateText_TB.AcceptsTabChanged += (s, e) => { this.OnAcceptsTabChanged(e); };
            //_privateText_TB.BorderStyleChanged += (s, e) => { this.OnBorderStyleChanged(e); };
            //_privateText_TB.Disposed += (s, e) => { this.OnDisposed(e); };
            //_privateText_TB.HideSelectionChanged += (s, e) => { this.OnHideSelectionChanged(e); };
            //_privateText_TB.ModifiedChanged += (s, e) => { this.OnModifiedChanged(e); };
            //_privateText_TB.MultilineChanged += (s, e) => { this.OnMultilineChanged(e); };
            //_privateText_TB.QueryAccessibilityHelp += (s, e) => { this.OnQueryAccessibilityHelp(e); };
            //_privateText_TB.ReadOnlyChanged += (s, e) => { this.OnReadOnlyChanged(e); };
            //_privateText_TB.TextAlignChanged += (s, e) => { this.OnTextAlignChanged(e); };

            //_privateText_TB.BindingContextChanged += (s, e) => { this.OnBindingContextChanged(e); };
            //_privateText_TB.VisibleChanged += (s, e) => { this.OnVisibleChanged(e); };

            _privateText_TB.AutoSizeChanged += (s, e) => { this.OnAutoSizeChanged(e); };
            _privateText_TB.BackColorChanged += (s, e) => { this.OnBackColorChanged(e); };
            _privateText_TB.BackgroundImageChanged += (s, e) => { this.OnBackgroundImageChanged(e); };
            _privateText_TB.BackgroundImageLayoutChanged += (s, e) => { this.OnBackgroundImageLayoutChanged(e); };
            _privateText_TB.CausesValidationChanged += (s, e) => { this.OnCausesValidationChanged(e); };
            _privateText_TB.ChangeUICues += (s, e) => { this.OnChangeUICues(e); };
            _privateText_TB.Click += (s, e) => { this.OnClick(e); };
            _privateText_TB.ClientSizeChanged += (s, e) => { this.OnClientSizeChanged(e); };
            _privateText_TB.ContextMenuStripChanged += (s, e) => { this.OnContextMenuStripChanged(e); };
            _privateText_TB.ControlAdded += (s, e) => { this.OnControlAdded(e); };
            _privateText_TB.ControlRemoved += (s, e) => { this.OnControlRemoved(e); };
            _privateText_TB.CursorChanged += (s, e) => { this.OnCursorChanged(e); };
            _privateText_TB.DockChanged += (s, e) => { this.OnDockChanged(e); };
            _privateText_TB.DoubleClick += (s, e) => { this.OnDoubleClick(e); };
            _privateText_TB.DpiChangedAfterParent += (s, e) => { this.OnDpiChangedAfterParent(e); };
            _privateText_TB.DpiChangedBeforeParent += (s, e) => { this.OnDpiChangedBeforeParent(e); };
            _privateText_TB.DragDrop += (s, e) => { this.OnDragDrop(e); };
            _privateText_TB.DragEnter += (s, e) => { this.OnDragEnter(e); };
            _privateText_TB.DragLeave += (s, e) => { this.OnDragLeave(e); };
            _privateText_TB.DragOver += (s, e) => { this.OnDragOver(e); };
            _privateText_TB.EnabledChanged += (s, e) => { this.OnEnabledChanged(e); };
            _privateText_TB.Enter += (s, e) => { this.OnEnter(e); };
            _privateText_TB.FontChanged += (s, e) => { this.OnFontChanged(e); };
            _privateText_TB.ForeColorChanged += (s, e) => { this.OnForeColorChanged(e); };
            _privateText_TB.GiveFeedback += (s, e) => { this.OnGiveFeedback(e); };
            _privateText_TB.GotFocus += (s, e) => { this.OnGotFocus(e); };
            _privateText_TB.HandleCreated += (s, e) => { this.OnHandleCreated(e); };
            _privateText_TB.HandleDestroyed += (s, e) => { this.OnHandleDestroyed(e); };
            _privateText_TB.HelpRequested += (s, e) => { this.OnHelpRequested(e); };
            _privateText_TB.ImeModeChanged += (s, e) => { this.OnImeModeChanged(e); };
            _privateText_TB.Invalidated += (s, e) => { this.OnInvalidated(e); };
            _privateText_TB.KeyDown += (s, e) => { this.OnKeyDown(e); };
            _privateText_TB.KeyPress += (s, e) => { this.OnKeyPress(e); };
            _privateText_TB.KeyUp += (s, e) => { this.OnKeyUp(e); };
            _privateText_TB.Layout += (s, e) => { this.OnLayout(e); };
            _privateText_TB.Leave += (s, e) => { this.OnLeave(e); };
            _privateText_TB.LocationChanged += (s, e) => { this.OnLocationChanged(e); };
            _privateText_TB.LostFocus += (s, e) => { this.OnLostFocus(e); };
            _privateText_TB.MarginChanged += (s, e) => { this.OnMarginChanged(e); };
            _privateText_TB.MouseCaptureChanged += (s, e) => { this.OnMouseCaptureChanged(e); };
            _privateText_TB.MouseClick += (s, e) => { this.OnMouseClick(e); };
            _privateText_TB.MouseDoubleClick += (s, e) => { this.OnMouseDoubleClick(e); };
            _privateText_TB.MouseDown += (s, e) => { this.OnMouseDown(e); };
            _privateText_TB.MouseEnter += (s, e) => { this.OnMouseEnter(e); };
            _privateText_TB.MouseHover += (s, e) => { this.OnMouseHover(e); };
            _privateText_TB.MouseLeave += (s, e) => { this.OnMouseLeave(e); };
            _privateText_TB.MouseMove += (s, e) => { this.OnMouseMove(e); };
            _privateText_TB.MouseUp += (s, e) => { this.OnMouseUp(e); };
            _privateText_TB.MouseWheel += (s, e) => { this.OnMouseWheel(e); };
            _privateText_TB.Move += (s, e) => { this.OnMove(e); };
            _privateText_TB.PaddingChanged += (s, e) => { this.OnPaddingChanged(e); };
            _privateText_TB.Paint += (s, e) => { this.OnPaint(e); };
            _privateText_TB.ParentChanged += (s, e) => { this.OnParentChanged(e); };
            _privateText_TB.PreviewKeyDown += (s, e) => { this.OnPreviewKeyDown(e); };
            _privateText_TB.QueryContinueDrag += (s, e) => { this.OnQueryContinueDrag(e); };
            _privateText_TB.RegionChanged += (s, e) => { this.OnRegionChanged(e); };
            _privateText_TB.Resize += (s, e) => { this.OnResize(e); };
            _privateText_TB.RightToLeftChanged += (s, e) => { this.OnRightToLeftChanged(e); };
            _privateText_TB.SizeChanged += (s, e) => { this.OnSizeChanged(e); };
            _privateText_TB.StyleChanged += (s, e) => { this.OnStyleChanged(e); };
            _privateText_TB.SystemColorsChanged += (s, e) => { this.OnSystemColorsChanged(e); };
            _privateText_TB.TabIndexChanged += (s, e) => { this.OnTabIndexChanged(e); };
            _privateText_TB.TabStopChanged += (s, e) => { this.OnTabStopChanged(e); };
            _privateText_TB.TextChanged += (s, e) => { this.OnTextChanged(e); };
            _privateText_TB.Validated += (s, e) => { this.OnValidated(e); };
            _privateText_TB.Validating += (s, e) => { this.OnValidating(e); };
        }
    }
}
