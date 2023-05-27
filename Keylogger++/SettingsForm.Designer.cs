
using System.Drawing;

namespace Keylogger__
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Font font = new Font("Cascadia Code", 9, FontStyle.Regular);

            this.radioButtonLightMode = new System.Windows.Forms.RadioButton();
            this.radioButtonDarkMode = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxUI = new System.Windows.Forms.GroupBox();
            this.buttonBrowseDir = new System.Windows.Forms.Button();
            this.textBoxDir = new System.Windows.Forms.TextBox();
            this.groupBoxDir = new System.Windows.Forms.GroupBox();
            this.groupBoxUI.SuspendLayout();
            this.groupBoxDir.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonLightMode
            // 
            this.radioButtonLightMode.AutoSize = true;
            this.radioButtonLightMode.Font = font;
            this.radioButtonLightMode.Location = new System.Drawing.Point(6, 19);
            this.radioButtonLightMode.Name = "radioButtonLightMode";
            this.radioButtonLightMode.Size = new System.Drawing.Size(106, 24);
            this.radioButtonLightMode.TabIndex = 0;
            this.radioButtonLightMode.TabStop = true;
            this.radioButtonLightMode.Text = "Light mode";
            this.radioButtonLightMode.UseVisualStyleBackColor = true;
            // 
            // radioButtonDarkMode
            // 
            this.radioButtonDarkMode.AutoSize = true;
            this.radioButtonDarkMode.Font = font;
            this.radioButtonDarkMode.Location = new System.Drawing.Point(6, 48);
            this.radioButtonDarkMode.Name = "radioButtonDarkMode";
            this.radioButtonDarkMode.Size = new System.Drawing.Size(105, 24);
            this.radioButtonDarkMode.TabIndex = 1;
            this.radioButtonDarkMode.TabStop = true;
            this.radioButtonDarkMode.Text = "Dark mode";
            this.radioButtonDarkMode.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Font = font;
            this.buttonSave.Location = new System.Drawing.Point(234, 222);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(110, 28);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save settings";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // groupBoxUI
            // 
            this.groupBoxUI.Controls.Add(this.radioButtonDarkMode);
            this.groupBoxUI.Controls.Add(this.radioButtonLightMode);
            this.groupBoxUI.Location = new System.Drawing.Point(12, 12);
            this.groupBoxUI.Name = "groupBoxUI";
            this.groupBoxUI.Size = new System.Drawing.Size(332, 78);
            this.groupBoxUI.TabIndex = 3;
            this.groupBoxUI.TabStop = false;
            this.groupBoxUI.Text = "UI settings";
            // 
            // buttonBrowseDir
            // 
            this.buttonBrowseDir.Font = font;
            this.buttonBrowseDir.Location = new System.Drawing.Point(251, 21);
            this.buttonBrowseDir.Name = "buttonBrowseDir";
            this.buttonBrowseDir.Size = new System.Drawing.Size(75, 30);
            this.buttonBrowseDir.TabIndex = 4;
            this.buttonBrowseDir.Text = "Browse";
            this.buttonBrowseDir.UseVisualStyleBackColor = true;
            this.buttonBrowseDir.Click += new System.EventHandler(this.buttonBrowseDir_Click);
            // 
            // textBoxDir
            // 
            this.textBoxDir.Font = font;
            this.textBoxDir.Location = new System.Drawing.Point(0, 23);
            this.textBoxDir.Name = "textBoxDir";
            this.textBoxDir.Size = new System.Drawing.Size(245, 26);
            this.textBoxDir.TabIndex = 5;
            // 
            // groupBoxDir
            // 
            this.groupBoxDir.Controls.Add(this.textBoxDir);
            this.groupBoxDir.Controls.Add(this.buttonBrowseDir);
            this.groupBoxDir.Location = new System.Drawing.Point(12, 96);
            this.groupBoxDir.Name = "groupBoxDir";
            this.groupBoxDir.Size = new System.Drawing.Size(332, 67);
            this.groupBoxDir.TabIndex = 6;
            this.groupBoxDir.TabStop = false;
            this.groupBoxDir.Text = "Directory";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 262);
            this.Controls.Add(this.groupBoxDir);
            this.Controls.Add(this.groupBoxUI);
            this.Controls.Add(this.buttonSave);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.groupBoxUI.ResumeLayout(false);
            this.groupBoxUI.PerformLayout();
            this.groupBoxDir.ResumeLayout(false);
            this.groupBoxDir.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonLightMode;
        private System.Windows.Forms.RadioButton radioButtonDarkMode;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.GroupBox groupBoxUI;
        private System.Windows.Forms.Button buttonBrowseDir;
        private System.Windows.Forms.TextBox textBoxDir;
        private System.Windows.Forms.GroupBox groupBoxDir;
    }
}