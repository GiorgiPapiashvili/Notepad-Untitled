using System;
using System.IO;
using System.Windows.Forms;

namespace G14_NotePad
{
    public partial class Form1 : Form
    {
        private string _filePath;
        private bool _isModified;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (value != null)
                {
                    this.Text = $"NotePad - {Path.GetFileName(value)}";
                }
                else
                {
                    this.Text = "NotePad - Untitled.txt";
                }
                _filePath = value;
                _isModified = false;
            }
        }

        public Form1()
        {
            InitializeComponent();

            toolStrip.Visible = Properties.Settings.Default.ToolBarsVisable;
            toolbarToolStripMenuItem.Checked = Properties.Settings.Default.ToolBarsVisable;

            statusStrip.Visible = Properties.Settings.Default.StatusBarVisable;
            statusbarToolStripMenuItem.Checked = Properties.Settings.Default.StatusBarVisable;
        }

        #region Event Handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e) => NewFile();

        private void openToolStripMenuItem_Click(object sender, EventArgs e) => OpenFile();

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile(true);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSave())
            {
                e.Cancel = true;
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            if (!_isModified)
            {
                _isModified = true;
            }
        }

        private void setBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                txtContent.BackColor = colorDialog.Color;
            }
        }

        private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolbarToolStripMenuItem.Checked;
            Properties.Settings.Default.ToolBarsVisable = toolStrip.Visible;
            Properties.Settings.Default.Save();

        }

        private void statusbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusbarToolStripMenuItem.Checked;
            Properties.Settings.Default.StatusBarVisable = statusStrip.Visible;
            Properties.Settings.Default.Save();

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) => txtContent.Cut();

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) => txtContent.Copy();

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) => txtContent.Paste();

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => txtContent.SelectAll();

        #endregion

        private void NewFile()
        {
            if (!ConfirmSave())
            {
                return;
            }
            txtContent.Clear();
            FilePath = null;
        }

        private void OpenFile()
        {
            if (!ConfirmSave())
            {
                return;
            }

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                txtContent.Text = File.ReadAllText(openFileDialog.FileName);
                FilePath = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SaveFile(bool isSaveAs = false)
        {
            if (FilePath == null || isSaveAs)
            {
                saveFileDialog.FileName = Path.GetFileName(FilePath);
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                FilePath = saveFileDialog.FileName;
            }

            try
            {
                File.WriteAllText(FilePath, txtContent.Text);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool ConfirmSave()
        {
            if (!_isModified)
            {
                return true;
            }

            var result = MessageBox.Show(
                "Do you want to save changes?",
                "Confirmation",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button3);

            switch (result)
            {
                case DialogResult.Yes:
                    return SaveFile();
                case DialogResult.No:
                    return true;
                default:
                    return false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            txtContent.Select(5, 3);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchingText = txtSearch.Text;

            int startIndex = txtContent.SelectionStart + txtContent.SelectionLength;


            if (!string.IsNullOrEmpty(searchingText) && startIndex < txtContent.TextLength)
            {

                int index = txtContent.Text.IndexOf(searchingText, startIndex, StringComparison.InvariantCultureIgnoreCase);

                if (index != -1)
                {
                    txtContent.SelectionStart = index;
                    txtContent.SelectionLength = searchingText.Length;
                    txtContent.ScrollToCaret();

                }
                else
                {
                    MessageBox.Show("THIS TEXT" + searchingText + " NOT FOUND! ", "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Information) ;
                }
            }
        }
    }
}
