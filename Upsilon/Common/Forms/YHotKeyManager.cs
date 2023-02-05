using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Upsilon.Common.Forms
{
    /// <summary>
    /// A class implementing the Hotkey manager.
    /// </summary>
    public class YHotKeyManager
    {
        [DllImport("user32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// The Hotkey pressed event.
        /// </summary>
        public static event EventHandler<HotKeyEventArgs> HotKeyPressed = null;

        /// <summary>
        /// Register a new hotkey.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>The hotkey id.</returns>
        public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            int id = System.Threading.Interlocked.Increment(ref _id);
            RegisterHotKey(_wnd.Handle, id, (uint)modifiers, (uint)key);
            return id;
        }

        /// <summary>
        /// Unregister a hotkey.
        /// </summary>
        /// <param name="id">The hotkey id.</param>
        /// <returns>Return <c>true</c> if the hotkey has been unregistered or <c>false</c> else.</returns>
        public static bool UnregisterHotKey(int id)
        {
            return UnregisterHotKey(_wnd.Handle, id);
        }

        /// <summary>
        /// The Hotkey pressed event.
        /// </summary>
        /// <param name="e"></param>
        protected static void OnHotKeyPressed(HotKeyEventArgs e)
        {
            if (HotKeyPressed != null)
            {
                HotKeyPressed(null, e);
            }
        }

        private static MessageWindow _wnd = new MessageWindow();

        private class MessageWindow : Form
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                    OnHotKeyPressed(e);
                }

                base.WndProc(ref m);
            }

            private const int WM_HOTKEY = 0x312;
        }

        private static int _id = 0;
    }

    /// <summary>
    /// The Hotkey event arguments
    /// </summary>
    public class HotKeyEventArgs : EventArgs
    {
        /// <summary>
        /// The key.
        /// </summary>
        public readonly Keys Key;

        /// <summary>
        /// The modifiers.
        /// </summary>
        public readonly KeyModifiers Modifiers;

        /// <summary>
        /// Create a new <c><see cref="HotKeyEventArgs"/></c>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        internal HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint)hotKeyParam.ToInt64();
            Key = (Keys)((param & 0xffff0000) >> 16);
            Modifiers = (KeyModifiers)(param & 0x0000ffff);
        }
    }
    /// <summary>
    /// The hotkey modifiers
    /// </summary>
    [Flags]
    public enum KeyModifiers
    {
        /// <summary>
        /// The Alt key
        /// </summary>
        Alt = 1,

        /// <summary>
        /// The Control key
        /// </summary>
        Control = 2,

        /// <summary>
        /// The Shift key
        /// </summary>
        Shift = 4,

        /// <summary>
        /// The Windows key
        /// </summary>
        Windows = 8,

        /// <summary>
        /// Set the no-repeat key
        /// </summary>
        NoRepeat = 0x4000
    }
}