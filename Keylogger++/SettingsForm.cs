/**************************************************************************
 *                                                                        *
 *  File:        SettingsForm.cs                                          *
 *  Copyright:   (c) Păduraru George                                      *
 *               @Kakerou_CLUB                                            *
 *  Description: Settings interface for the Keylogger++ app.              *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Keylogger__
{
    public partial class SettingsForm : Form
    {
        private int _lightMode = 1;
        private int _darkMode = 0;
        private const string _settings = @"../../settings.txt";
        public SettingsForm()
        {
            InitializeComponent();
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
                    radioButtonLightMode.Checked = true;
                    radioButtonDarkMode.Checked = false;
                }
                else
                {
                    radioButtonDarkMode.Checked = true;
                    radioButtonLightMode.Checked = false;
                }
                textBoxDir.Text = strings[2].Substring(20);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Saves settings to the settings file
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (radioButtonLightMode.Checked == true)
            {
                _lightMode = 1;
                _darkMode = 0;
            }
            else if (radioButtonDarkMode.Checked == true)
            {
                _darkMode = 1;
                _lightMode = 0;
            }
            try
            {
                StreamWriter sw = new StreamWriter(_settings);
                sw.WriteLine("Light mode = " + _lightMode);
                sw.WriteLine("Dark mode = " + _darkMode);
                sw.WriteLine("Default directory = " + textBoxDir.Text);
                sw.Close();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Change directory browser
        private void buttonBrowseDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textBoxDir.Text = folderBrowserDialog.SelectedPath;
        }
    }
}
