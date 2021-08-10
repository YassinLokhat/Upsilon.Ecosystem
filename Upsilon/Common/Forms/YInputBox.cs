using System;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    public static class YInputBox
    {
        public enum YInputType
        {
            None = 0,
            TextBox,
            Password,
            Dropdown,
            ComboBox,
        }

        public static DialogResult ShowDialog(string title, string message, ref string result, YInputType inputType)
        {
            return InputBox.ShowDialog(message, title, ref result, inputType == YInputType.Password ? '*' : '\0');
        }
    }
}
