using System;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    /// <summary>
    /// An input toolbox.
    /// </summary>
    public static class YInputBox
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
        /// Show a dialog box aking to enter a specific type of input.
        /// </summary>
        /// <param name="title">The title of the dialog box.</param>
        /// <param name="message">The message of the dialog box.</param>
        /// <param name="result">The default text on the input dialog and the answer of the user.</param>
        /// <param name="inputType">The type of the requested input.</param>
        /// <returns>The <c><see cref="DialogResult"/></c> of the dialog box.</returns>
        public static DialogResult ShowDialog(string title, string message, ref string result, YInputType inputType)
        {
            return InputBox.ShowDialog(message, title, ref result, inputType == YInputType.Password ? '*' : '\0');
        }
    }
}
