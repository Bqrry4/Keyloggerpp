
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
            this.radioButtonLightMode = new System.Windows.Forms.RadioButton();
            this.radioButtonDarkMode = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioButtonLightMode
            // 
            this.radioButtonLightMode.AutoSize = true;
            this.radioButtonLightMode.Location = new System.Drawing.Point(12, 29);
            this.radioButtonLightMode.Name = "radioButtonLightMode";
            this.radioButtonLightMode.Size = new System.Drawing.Size(77, 17);
            this.radioButtonLightMode.TabIndex = 0;
            this.radioButtonLightMode.TabStop = true;
            this.radioButtonLightMode.Text = "Light mode";
            this.radioButtonLightMode.UseVisualStyleBackColor = true;
            // 
            // radioButtonDarkMode
            // 
            this.radioButtonDarkMode.AutoSize = true;
            this.radioButtonDarkMode.Location = new System.Drawing.Point(12, 52);
            this.radioButtonDarkMode.Name = "radioButtonDarkMode";
            this.radioButtonDarkMode.Size = new System.Drawing.Size(77, 17);
            this.radioButtonDarkMode.TabIndex = 1;
            this.radioButtonDarkMode.TabStop = true;
            this.radioButtonDarkMode.Text = "Dark mode";
            this.radioButtonDarkMode.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(234, 227);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(110, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Save settings";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 262);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.radioButtonDarkMode);
            this.Controls.Add(this.radioButtonLightMode);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonLightMode;
        private System.Windows.Forms.RadioButton radioButtonDarkMode;
        private System.Windows.Forms.Button buttonSave;
    }
}