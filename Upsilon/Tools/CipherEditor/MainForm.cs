using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upsilon.Common.Library;
using System.Text.Json;
using System.Web;

namespace Upsilon.Tools.CipherEditor
{
    public partial class MainForm : Form
    {
        private bool _locked = false;

        public MainForm()
        {
            InitializeComponent();

            ((TextBox)tbPassword.Control).PasswordChar = '*';

            tbCipherText.MaxLength = tbClearText.MaxLength = int.MaxValue;

            this.tbClearText.TextChanged += _tbClearText_TextChanged;
            this.tbCipherText.TextChanged += _tbCipherText_TextChanged;

            this.tbCipherText.DragDrop += _tbTextBox_DragDrop;
            this.tbCipherText.DragEnter += _tbTextBox_DragEnter;
            this.tbClearText.DragDrop += _tbTextBox_DragDrop;
            this.tbClearText.DragEnter += _tbTextBox_DragEnter;

            this.tbPassword.TextChanged += _tbPassword_TextChanged;

            this.bOpen.Click += _bOpen_Click;
            this.bSave.Click += _bSave_Click;
        }

        private void _bSave_Click(object sender, EventArgs e)
        {
            TextBox textBox = this.tbCipherText;
   
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Save the cipher text to a file",
                Filter = "All files|*.*",
            };

            if (this.tbClearText.Focused)
            {
                textBox = this.tbClearText;
                saveFileDialog.Title = "Save the clear text to a file";
            }

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this._save(saveFileDialog.FileName, textBox);
        }

        private void _bOpen_Click(object sender, EventArgs e)
        {
            TextBox textBox = this.tbCipherText;

            OpenFileDialog openFileDialog = new()
            {
                Title = "Open a cipher file",
                Filter = "All files|*.*",
            };

            if (this.tbClearText.Focused)
            {
                textBox = this.tbClearText;
                openFileDialog.Title = "Open a clear text file";
            }

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this._open(openFileDialog.FileName, textBox);
        }

        private void _tbPassword_TextChanged(object sender, EventArgs e)
        {
            this._locked = true;
            this.tbClearText.Text = tbCipherText.Text = string.Empty;
            this._locked = false;
        }

        private void _open(string file, TextBox textBox)
        {
            try
            {
                if (textBox == tbCipherText)
                {
                    textBox.Text = this._encode(File.ReadAllText(file));
                }
                else
                {
                    textBox.Text = File.ReadAllText(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _save(string file, TextBox textBox)
        {
            if (textBox == tbCipherText)
            {
                File.WriteAllText(file, this._decode(textBox.Text));
            }
            else
            {
                File.WriteAllText(file, textBox.Text);
            }
        }

        private void _tbTextBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0)
            {
                return;
            }

            this._open(files[0], (TextBox)sender);
        }

        private void _tbTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void _tbClearText_TextChanged(object sender, EventArgs e)
        {
            if (this._locked)
            {
                return;
            }

            try
            {
                this._locked = true;
                this.tbCipherText.Text = this._encode(this.tbClearText.Text.Cipher_Aes(this.tbPassword.Text));
            }
            catch (Exception ex)
            {
                ex.ToString();
                this.tbCipherText.Text = string.Empty;
                this.tbCipherText.SelectionLength = 0;
                this.tbCipherText.SelectionStart = this.tbCipherText.Text.Length;
            }
            finally
            {
                this._locked = false;
            }
        }

        private void _tbCipherText_TextChanged(object sender, EventArgs e)
        {
            if (this._locked)
            {
                return;
            }

            try
            {
                this._locked = true;
                this.tbClearText.Text = this._decode(this.tbCipherText.Text).Uncipher_Aes(this.tbPassword.Text);
                this.tbClearText.SelectionLength = 0;
                this.tbClearText.SelectionStart = this.tbCipherText.Text.Length;
            }
            catch (Exception ex)
            {
                ex.ToString();
                this.tbClearText.Text = string.Empty;
            }
            finally
            {
                this._locked = false;
            }
        }

        private string _encode(string str)
        {
            if (tbPassword.Text == string.Empty)
            {
                return str;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        private string _decode(string str)
        {
            if (tbPassword.Text == string.Empty)
            {
                return str;
            }

            byte[] bytes = str.Split(' ').Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
