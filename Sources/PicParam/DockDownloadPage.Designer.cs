namespace PicParam
{
    partial class DockDownloadPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockDownloadPage));
            this.bnMerge = new System.Windows.Forms.Button();
            this.bnOverwrite = new System.Windows.Forms.Button();
            this.listBoxLibraries = new ListBoxLibraries();
            this.bnInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnMerge
            // 
            resources.ApplyResources(this.bnMerge, "bnMerge");
            this.bnMerge.Name = "bnMerge";
            this.bnMerge.UseVisualStyleBackColor = true;
            this.bnMerge.Click += new System.EventHandler(this.OnButtonMerge);
            // 
            // bnOverwrite
            // 
            resources.ApplyResources(this.bnOverwrite, "bnOverwrite");
            this.bnOverwrite.Name = "bnOverwrite";
            this.bnOverwrite.UseVisualStyleBackColor = true;
            this.bnOverwrite.Click += new System.EventHandler(this.OnButtonOverwrite);
            // 
            // listBoxLibraries
            // 
            resources.ApplyResources(this.listBoxLibraries, "listBoxLibraries");
            this.listBoxLibraries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxLibraries.FormattingEnabled = true;
            this.listBoxLibraries.Name = "listBoxLibraries";
            // 
            // bnInfo
            // 
            resources.ApplyResources(this.bnInfo, "bnInfo");
            this.bnInfo.Name = "bnInfo";
            this.bnInfo.UseVisualStyleBackColor = true;
            this.bnInfo.Click += new System.EventHandler(this.OnButtonInfo);
            // 
            // FormDownloadPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnInfo);
            this.Controls.Add(this.listBoxLibraries);
            this.Controls.Add(this.bnOverwrite);
            this.Controls.Add(this.bnMerge);
            this.Name = "FormDownloadPage";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBoxLibraries listBoxLibraries;
        private System.Windows.Forms.Button bnMerge;
        private System.Windows.Forms.Button bnOverwrite;
        private System.Windows.Forms.Button bnInfo;
    }
}
