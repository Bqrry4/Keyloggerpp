using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace Keylogger__
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();
            StreamReader sr = new StreamReader("settings.txt");
            string line = sr.ReadLine();
            if (line.Contains("1"))
            {
                InitUI("light");
            }
            else
            {
                InitUI("dark");
            }
            sr.Close();
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {

        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "KeyLogger++ files|*.kpp|All files|*.*";
            openFileDialog.Title = "Open script file";
            openFileDialog.InitialDirectory = "Documents\\";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                string content = sr.ReadToEnd();
                richTextBoxScript.Text = content;
                sr.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error reading file");
            }
        }

        private void buttonSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.kpp";
            saveFileDialog.Title = "Save script file";
            saveFileDialog.InitialDirectory = "Documents\\";
            saveFileDialog.Filter = "KeyLogger++ files|*.kpp|All files|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                string content = richTextBoxScript.Text;
                sw.Write(content);
                sw.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error writing file");
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.Show();
        }

        private void buttonApplySettings_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader("settings.txt");
                string line = sr.ReadLine();
                if (line.Contains("1"))
                {
                    InitUI("light");
                }
                else
                {
                    InitUI("dark");
                }
                sr.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error reading settings file");
            }
        }

        private void InitUI(string key)
        {
            if (key == "dark")
            {
                this.ForeColor = Color.FromArgb(245, 246, 250);
                buttonRecord.BackColor = Color.FromArgb(47, 54, 64);
                buttonRun.BackColor = Color.FromArgb(47, 54, 64);
                buttonOpenFile.BackColor = Color.FromArgb(47, 54, 64);
                buttonSaveFile.BackColor = Color.FromArgb(47, 54, 64);
                buttonSettings.BackColor = Color.FromArgb(47, 54, 64);
                buttonApplySettings.BackColor = Color.FromArgb(47, 54, 64);
                buttonAbout.BackColor = Color.FromArgb(47, 54, 64);
                buttonHelp.BackColor = Color.FromArgb(47, 54, 64);
                buttonExit.BackColor = Color.FromArgb(47, 54, 64);
                richTextBoxScript.BackColor = Color.FromArgb(47, 54, 64);

                this.BackColor = Color.FromArgb(47, 54, 64);
                buttonRecord.ForeColor = Color.FromArgb(245, 246, 250);
                buttonRun.ForeColor = Color.FromArgb(245, 246, 250);
                buttonOpenFile.ForeColor = Color.FromArgb(245, 246, 250);
                buttonSaveFile.ForeColor = Color.FromArgb(245, 246, 250);
                buttonSettings.ForeColor = Color.FromArgb(245, 246, 250);
                buttonApplySettings.ForeColor = Color.FromArgb(245, 246, 250);
                buttonAbout.ForeColor = Color.FromArgb(245, 246, 250);
                buttonHelp.ForeColor = Color.FromArgb(245, 246, 250);
                buttonExit.ForeColor = Color.FromArgb(245, 246, 250);
                richTextBoxScript.ForeColor = Color.FromArgb(245, 246, 250);
            }
            else
            {
                this.ForeColor = Color.FromArgb(47, 54, 64);
                buttonRecord.BackColor = Color.FromArgb(245, 246, 250);
                buttonRun.BackColor = Color.FromArgb(245, 246, 250);
                buttonOpenFile.BackColor = Color.FromArgb(245, 246, 250);
                buttonSaveFile.BackColor = Color.FromArgb(245, 246, 250);
                buttonSettings.BackColor = Color.FromArgb(245, 246, 250);
                buttonApplySettings.BackColor = Color.FromArgb(245, 246, 250);
                buttonAbout.BackColor = Color.FromArgb(245, 246, 250);
                buttonHelp.BackColor = Color.FromArgb(245, 246, 250);
                buttonExit.BackColor = Color.FromArgb(245, 246, 250);
                richTextBoxScript.BackColor = Color.FromArgb(245, 246, 250);

                this.BackColor = Color.FromArgb(245, 246, 250);
                buttonRecord.ForeColor = Color.FromArgb(47, 54, 64);
                buttonRun.ForeColor = Color.FromArgb(47, 54, 64);
                buttonOpenFile.ForeColor = Color.FromArgb(47, 54, 64);
                buttonSaveFile.ForeColor = Color.FromArgb(47, 54, 64);
                buttonSettings.ForeColor = Color.FromArgb(47, 54, 64);
                buttonApplySettings.ForeColor = Color.FromArgb(47, 54, 64);
                buttonAbout.ForeColor = Color.FromArgb(47, 54, 64);
                buttonHelp.ForeColor = Color.FromArgb(47, 54, 64);
                buttonExit.ForeColor = Color.FromArgb(47, 54, 64);
                richTextBoxScript.ForeColor = Color.FromArgb(47, 54, 64);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {

        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program that allows user to make custom hotkeys", "About keylogger");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
