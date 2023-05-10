using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Keylogger__
{
    public partial class SettingsForm : Form
    {
        private int _lightMode = 1;
        private int _darkMode = 0;
        private const string _settings = "settings.txt";
        public SettingsForm()
        {
            InitializeComponent();
            StreamReader sr = new StreamReader(_settings);
            string line = sr.ReadLine();
            if (line.Contains("1"))
            {
                radioButtonLightMode.Checked = true;
                radioButtonDarkMode.Checked = false;
            }
            else
            {
                radioButtonDarkMode.Checked = true;
                radioButtonLightMode.Checked = false;
            }
            sr.Close();
        }

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
            StreamWriter sw = new StreamWriter(_settings);
            sw.WriteLine("Light mode = " + _lightMode);
            sw.WriteLine("Dark mode = " + _darkMode);
            sw.Close();
            this.Hide();
        }
    }
}
