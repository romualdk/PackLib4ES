namespace PicParam
{
    partial class OptionPanelCulture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelCulture));
            this.cbLanguages = new System.Windows.Forms.ComboBox();
            this.lbLanguage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbLanguages
            // 
            resources.ApplyResources(this.cbLanguages, "cbLanguages");
            this.cbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguages.FormattingEnabled = true;
            this.cbLanguages.Name = "cbLanguages";
            this.cbLanguages.SelectedIndexChanged += new System.EventHandler(this.onComboSelectionChanged);
            // 
            // lbLanguage
            // 
            resources.ApplyResources(this.lbLanguage, "lbLanguage");
            this.lbLanguage.Name = "lbLanguage";
            // 
            // OptionPanelCulture
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Paramètres\\\\Culture";
            this.Controls.Add(this.cbLanguages);
            this.Controls.Add(this.lbLanguage);
            this.DisplayName = "Culture & langue";
            this.Name = "OptionPanelCulture";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbLanguages;
        private System.Windows.Forms.Label lbLanguage;
    }
}
