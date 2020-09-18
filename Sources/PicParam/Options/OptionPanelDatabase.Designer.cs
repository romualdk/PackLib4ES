namespace PicParam
{
    partial class OptionPanelDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelDatabase));
            this.fileSelect = new treeDiM.UserControls.FileSelect();
            this.folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.labelDatabasePath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fileSelect
            // 
            resources.ApplyResources(this.fileSelect, "fileSelect");
            this.fileSelect.Name = "fileSelect";
            // 
            // folderBrowserDlg
            // 
            resources.ApplyResources(this.folderBrowserDlg, "folderBrowserDlg");
            // 
            // labelDatabasePath
            // 
            resources.ApplyResources(this.labelDatabasePath, "labelDatabasePath");
            this.labelDatabasePath.Name = "labelDatabasePath";
            // 
            // OptionPanelDatabase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Paramètres\\Base de données";
            this.Controls.Add(this.labelDatabasePath);
            this.Controls.Add(this.fileSelect);
            this.DisplayName = "Base de données";
            this.Name = "OptionPanelDatabase";
            this.Load += new System.EventHandler(this.OptionPanelDatabase_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private treeDiM.UserControls.FileSelect fileSelect;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDlg;
        private System.Windows.Forms.Label labelDatabasePath;
    }
}
