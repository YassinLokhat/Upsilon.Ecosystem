using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Upsilon.Common.Forms
{
    internal static class InputBox
    {
        public static DialogResult ShowDialog(string promptText, string title, ref string value, char passwordChar = '\0', HorizontalAlignment alignment = 0, int linkStart = 0, int linkLength = 0, LinkLabelLinkClickedEventHandler linkClicked = null)
        {
            Form form = new Form();
            LinkLabel label = new LinkLabel();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            label.LinkArea = new LinkArea(linkStart, linkLength);
            if (linkClicked != null)
                label.LinkClicked += linkClicked;
            textBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox.TextAlign = alignment;
            textBox.Text = value;
            if (passwordChar != '\0')
            {
                textBox.PasswordChar = passwordChar;
            }

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            SizeF size = label.CreateGraphics().MeasureString(label.Text, label.Font);
            textBox.SetBounds(12, 36 + (int)size.Height, 400, 20);
            buttonOk.SetBounds(228, 72 + (int)size.Height, 75, 23);
            buttonCancel.SetBounds(309, 72 + (int)size.Height, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(textBox.Width + 24, 107 + (int)size.Height);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(textBox.Width + 24, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.TopLevel = true;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
