using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    /// <summary>
    /// This static class contains the user intereaction dialog box.
    /// </summary>
    public static class YUserInteract
    {
        /// <summary>
        /// The type of input needed.
        /// </summary>
        public enum YInputType
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// A text.
            /// </summary>
            TextBox,

            /// <summary>
            /// A pasword.
            /// </summary>
            Password,

            /// <summary>
            /// A dropdown list.
            /// </summary>
            Dropdown,

            /// <summary>
            /// A combo list.
            /// </summary>
            ComboBox,

            /// <summary>
            /// A number.
            /// </summary>
            Number,
        }

        /// <summary>
        /// Request some information to the user.
        /// </summary>
        /// <param name="title">The title of the dialog box.</param>
        /// <param name="message">The message of the dialog box.</param>
        /// <param name="result">The default text on the input dialog and the answer of the user.</param>
        /// <param name="inputType">The type of the requested input.</param>
        /// <returns>The <c><see cref="DialogResult"/></c> of the dialog box.</returns>
        public static DialogResult RequestInfo(string title, string message, ref string result, YInputType inputType)
        {
            return YInputBox.ShowDialog(message, title, ref result, inputType == YInputType.Password ? '*' : '\0');
        }

        /// <summary>
        /// Inform the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="title">The title of the message.</param>
        /// <returns>The <c><see cref="DialogResult"/></c> of the dialog box.</returns>
        public static DialogResult Inform(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        /// <summary>
        /// Ask something to the user.
        /// </summary>
        /// <param name="message">The message to ask.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="buttons">the button to display. By default the MessageBoxButtons.YesNo</param>
        /// <returns>The <c><see cref="DialogResult"/></c> of the dialog box.</returns>
        public static DialogResult Ask(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.YesNo)
        {
            return MessageBox.Show(message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

        /// <summary>
        /// Allert the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="buttons">the button to display. By default the MessageBoxButtons.OK</param>
        /// <returns>The <c><see cref="DialogResult"/></c> of the dialog box.</returns>
        public static DialogResult Alert(string message, string title = "Error", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(message, title, buttons, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }

    }
}
