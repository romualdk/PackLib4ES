﻿namespace PicParam
{
    partial class OptionPanelOutputFileOpening
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionPanelOutputFileOpening));
            this.checkBoxAppDXF = new System.Windows.Forms.CheckBox();
            this.fileSelectOutputDXF = new treeDiM.UserControls.FileSelect();
            this.checkBoxAppAI = new System.Windows.Forms.CheckBox();
            this.fileSelectOutputAI = new treeDiM.UserControls.FileSelect();
            this.checkBoxAppCF2 = new System.Windows.Forms.CheckBox();
            this.fileSelectOutputCF2 = new treeDiM.UserControls.FileSelect();
            this.SuspendLayout();
            // 
            // checkBoxAppDXF
            // 
            resources.ApplyResources(this.checkBoxAppDXF, "checkBoxAppDXF");
            this.checkBoxAppDXF.Name = "checkBoxAppDXF";
            this.checkBoxAppDXF.UseVisualStyleBackColor = true;
            // 
            // fileSelectOutputDXF
            // 
            resources.ApplyResources(this.fileSelectOutputDXF, "fileSelectOutputDXF");
            this.fileSelectOutputDXF.Name = "fileSelectOutputDXF";
            // 
            // checkBoxAppAI
            // 
            resources.ApplyResources(this.checkBoxAppAI, "checkBoxAppAI");
            this.checkBoxAppAI.Name = "checkBoxAppAI";
            this.checkBoxAppAI.UseVisualStyleBackColor = true;
            // 
            // fileSelectOutputAI
            // 
            resources.ApplyResources(this.fileSelectOutputAI, "fileSelectOutputAI");
            this.fileSelectOutputAI.Name = "fileSelectOutputAI";
            // 
            // checkBoxAppCF2
            // 
            resources.ApplyResources(this.checkBoxAppCF2, "checkBoxAppCF2");
            this.checkBoxAppCF2.Name = "checkBoxAppCF2";
            this.checkBoxAppCF2.UseVisualStyleBackColor = true;
            // 
            // fileSelectOutputCF2
            // 
            resources.ApplyResources(this.fileSelectOutputCF2, "fileSelectOutputCF2");
            this.fileSelectOutputCF2.Name = "fileSelectOutputCF2";
            // 
            // OptionPanelOutputFileOpening
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CategoryPath = "Paramètres\\Export des fichiers";
            this.Controls.Add(this.fileSelectOutputCF2);
            this.Controls.Add(this.checkBoxAppCF2);
            this.Controls.Add(this.fileSelectOutputAI);
            this.Controls.Add(this.checkBoxAppAI);
            this.Controls.Add(this.fileSelectOutputDXF);
            this.Controls.Add(this.checkBoxAppDXF);
            this.DisplayName = "Export des fichiers";
            this.Name = "OptionPanelOutputFileOpening";
            this.Load += new System.EventHandler(this.OptionPanelOutputFileOpening_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAppDXF;
        private System.Windows.Forms.CheckBox checkBoxAppAI;
        private System.Windows.Forms.CheckBox checkBoxAppCF2;
        private treeDiM.UserControls.FileSelect fileSelectOutputAI;
        private treeDiM.UserControls.FileSelect fileSelectOutputDXF;
        private treeDiM.UserControls.FileSelect fileSelectOutputCF2;
    }
}
