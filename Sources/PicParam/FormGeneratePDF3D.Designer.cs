namespace PicParam
{
    partial class FormGeneratePDF3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGeneratePDF3D));
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnGenerate = new System.Windows.Forms.Button();
            this.labelFileOutput = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fileSelectOutput = new treeDiM.UserControls.FileSelect();
            this.fileSelectPdfTemplate = new treeDiM.UserControls.FileSelect();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.chkbOpenFile = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnGenerate
            // 
            resources.ApplyResources(this.bnGenerate, "bnGenerate");
            this.bnGenerate.Name = "bnGenerate";
            this.bnGenerate.UseVisualStyleBackColor = true;
            this.bnGenerate.Click += new System.EventHandler(this.OnGenerate);
            // 
            // labelFileOutput
            // 
            resources.ApplyResources(this.labelFileOutput, "labelFileOutput");
            this.labelFileOutput.Name = "labelFileOutput";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // fileSelectOutput
            // 
            resources.ApplyResources(this.fileSelectOutput, "fileSelectOutput");
            this.fileSelectOutput.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            this.fileSelectOutput.Name = "fileSelectOutput";
            this.fileSelectOutput.FileNameChanged += new System.EventHandler(this.OnFileNameChanged);
            // 
            // fileSelectPdfTemplate
            // 
            resources.ApplyResources(this.fileSelectPdfTemplate, "fileSelectPdfTemplate");
            this.fileSelectPdfTemplate.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            this.fileSelectPdfTemplate.Name = "fileSelectPdfTemplate";
            this.fileSelectPdfTemplate.FileNameChanged += new System.EventHandler(this.OnFileNameChanged);
            // 
            // richTextBoxOutput
            // 
            resources.ApplyResources(this.richTextBoxOutput, "richTextBoxOutput");
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            // 
            // chkbOpenFile
            // 
            resources.ApplyResources(this.chkbOpenFile, "chkbOpenFile");
            this.chkbOpenFile.Checked = true;
            this.chkbOpenFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbOpenFile.Name = "chkbOpenFile";
            this.chkbOpenFile.UseVisualStyleBackColor = true;
            // 
            // FormGeneratePDF3D
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.Controls.Add(this.chkbOpenFile);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.fileSelectPdfTemplate);
            this.Controls.Add(this.fileSelectOutput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFileOutput);
            this.Controls.Add(this.bnGenerate);
            this.Controls.Add(this.bnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGeneratePDF3D";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnGenerate;
        private System.Windows.Forms.Label labelFileOutput;
        private System.Windows.Forms.Label label2;
        private treeDiM.UserControls.FileSelect fileSelectOutput;
        private treeDiM.UserControls.FileSelect fileSelectPdfTemplate;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.CheckBox chkbOpenFile;
    }
}