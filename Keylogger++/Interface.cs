/**************************************************************************
 *                                                                        *
 *  File:        Interface.cs                                             *
 *  Copyright:   (c) Păduraru George                                      *
 *               @Kakerou_CLUB                                            *
 *  Description: Interface for the Keylogger++ app.                       *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using IntermediaryFacade;

namespace Keylogger__
{
    public partial class Interface : Form
    {
        /// <summary>
        /// The Controller part like in the MVC
        /// </summary>
        private LogFace _controller = new LogFace();

        private string _directory;
        private const string _settings = @"../../settings.txt";
        public Interface()
        {
            InitializeComponent();
            DarkTitleBarClass.UseImmersiveDarkMode(this.Handle, true);
            try
            {
                StreamReader sr = new StreamReader(_settings);
                List<string> strings = new List<string>();
                string line = sr.ReadLine();
                while (line != null)
                {
                    strings.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
                if (strings[0].Contains("1"))
                {
                    InitUI("light");
                }
                else
                {
                    InitUI("dark");
                }
                _directory = strings[2].Substring(20);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Icon = new Icon(@"../../../favicon.ico");

        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            if (buttonRecord.Text == "Start recording")
            {
                buttonRecord.Text = "Stop recording";
                richTextBoxScript.ReadOnly = true;
                _controller.setOutput(new RichTextBoxWriter(richTextBoxScript));
                _controller.StartRecording();
            }
            else
            {
                _controller.StopRecording();
                richTextBoxScript.ReadOnly = false;
                buttonRecord.Text = "Start recording";
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (buttonRun.Text == "Start running")
            {
                buttonRun.Text = "Stop running";
                
                _controller.StartRunning(richTextBoxScript.Text);
            }
            else
            {
                _controller.StopRunning();
                buttonRun.Text = "Start running";
            }
        }
        // Opens a .klpp file
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "KeyLogger++ files|*.klpp|All files|*.*";
            openFileDialog.Title = "Open script file";
            openFileDialog.InitialDirectory = _directory;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
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
        // Saves the script as a *.klpp file
        private void buttonSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.klpp";
            saveFileDialog.Title = "Save script file";
            saveFileDialog.InitialDirectory = _directory;
            saveFileDialog.Filter = "KeyLogger++ files|*.klpp|All files|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
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

        // Opens settings dialog
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.Show();
        }

        // Applies settings
        private void buttonApplySettings_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader(_settings);
                List<string> strings = new List<string>();
                string line = sr.ReadLine();
                while (line != null)
                {
                    strings.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
                if (strings[0].Contains("1"))
                {
                    InitUI("light");
                }
                else
                {
                    InitUI("dark");
                }
                _directory = strings[2].Substring(20);
            }
            catch (Exception)
            {
                MessageBox.Show("Error reading settings file");
            }
        }
        /// <summary>
        /// Initializes the UI based on settings
        /// </summary>
        /// <param name="key"></param> Get the type of theme
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
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string file = Path.Combine(currentDir, @"../../../Keylogger++.chm");
            string filePath = Path.GetFullPath(file);
            try
            {
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // About the program section
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
