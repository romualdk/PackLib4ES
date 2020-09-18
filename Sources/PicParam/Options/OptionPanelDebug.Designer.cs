namespace PicParam
{
    partial class OptionPanelDebug
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelDebug));
            this.chkbShowLogConsole = new System.Windows.Forms.CheckBox();
            this.bnUpdateLocalisationFile = new System.Windows.Forms.Button();
            this.bnOpenConfigDirectory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkbShowLogConsole
            // 
            resources.ApplyResources(this.chkbShowLogConsole, "chkbShowLogConsole");
            this.chkbShowLogConsole.Name = "chkbShowLogConsole";
            this.chkbShowLogConsole.UseVisualStyleBackColor = true;
            this.chkbShowLogConsole.CheckedChanged += new System.EventHandler(this.OnShowLogConsoleChecked);
            // 
            // bnUpdateLocalisationFile
            // 
            resources.ApplyResources(this.bnUpdateLocalisationFile, "bnUpdateLocalisationFile");
            this.bnUpdateLocalisationFile.Name = "bnUpdateLocalisationFile";
            this.bnUpdateLocalisationFile.UseVisualStyleBackColor = true;
            this.bnUpdateLocalisationFile.Click += new System.EventHandler(this.OnUpdateLocalisationFile);
            // 
            // bnOpenConfigDirectory
            // 
            resources.ApplyResources(this.bnOpenConfigDirectory, "bnOpenConfigDirectory");
            this.bnOpenConfigDirectory.Name = "bnOpenConfigDirectory";
            this.bnOpenConfigDirectory.UseVisualStyleBackColor = true;
            this.bnOpenConfigDirectory.Click += new System.EventHandler(this.OnOpenConfigDirectory);
            // 
            // OptionPanelDebug
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Paramètres\\Débogage";
            this.Controls.Add(this.bnOpenConfigDirectory);
            this.Controls.Add(this.bnUpdateLocalisationFile);
            this.Controls.Add(this.chkbShowLogConsole);
            this.DisplayName = "Débogage";
            this.Name = "OptionPanelDebug";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkbShowLogConsole;
        private System.Windows.Forms.Button bnUpdateLocalisationFile;
        private System.Windows.Forms.Button bnOpenConfigDirectory;
    }
}
