namespace PicParam
{
    partial class FormRenameNode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRenameNode));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOk = new System.Windows.Forms.Button();
            this.label_description = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.chkCustomImage = new System.Windows.Forms.CheckBox();
            this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
            this.thumbnailSelectCtrl = new treeDiM.UserControls.FileSelect();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOk
            // 
            resources.ApplyResources(this.bnOk, "bnOk");
            this.bnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOk.Name = "bnOk";
            this.bnOk.UseVisualStyleBackColor = true;
            // 
            // label_description
            // 
            resources.ApplyResources(this.label_description, "label_description");
            this.label_description.Name = "label_description";
            // 
            // label_name
            // 
            resources.ApplyResources(this.label_name, "label_name");
            this.label_name.Name = "label_name";
            // 
            // chkCustomImage
            // 
            resources.ApplyResources(this.chkCustomImage, "chkCustomImage");
            this.chkCustomImage.Name = "chkCustomImage";
            this.chkCustomImage.UseVisualStyleBackColor = true;
            this.chkCustomImage.CheckedChanged += new System.EventHandler(this.chkCustomImage_CheckedChanged);
            // 
            // pictureBoxThumbnail
            // 
            resources.ApplyResources(this.pictureBoxThumbnail, "pictureBoxThumbnail");
            this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
            this.pictureBoxThumbnail.TabStop = false;
            // 
            // thumbnailSelectCtrl
            // 
            resources.ApplyResources(this.thumbnailSelectCtrl, "thumbnailSelectCtrl");
            this.thumbnailSelectCtrl.Filter = "Image files (*.bmp;*.gif;*.jpg;*.png)|*.bmp;*.gif;*.jpg;*.png";
            this.thumbnailSelectCtrl.Name = "thumbnailSelectCtrl";
            this.thumbnailSelectCtrl.FileNameChanged += new System.EventHandler(this.thumbnailSelectCtrl_FileNameChanged);
            this.thumbnailSelectCtrl.TextChanged += new System.EventHandler(this.thumbnailSelectCtrl_TextChanged);
            // 
            // FormRenameNode
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.thumbnailSelectCtrl);
            this.Controls.Add(this.pictureBoxThumbnail);
            this.Controls.Add(this.chkCustomImage);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOk);
            this.Controls.Add(this.label_description);
            this.Controls.Add(this.label_name);
            this.Name = "FormRenameNode";
            this.Load += new System.EventHandler(this.FormRenameNode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOk;
        private System.Windows.Forms.Label label_description;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.CheckBox chkCustomImage;
        private System.Windows.Forms.PictureBox pictureBoxThumbnail;
        private treeDiM.UserControls.FileSelect thumbnailSelectCtrl;
    }
}