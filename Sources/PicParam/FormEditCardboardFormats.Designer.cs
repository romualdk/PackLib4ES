namespace PicParam
{
    partial class FormEditCardboardFormats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditCardboardFormats));
            this.btClose = new System.Windows.Forms.Button();
            this.btCreate = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btModify = new System.Windows.Forms.Button();
            this.listViewCardboardFormats = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWidth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeight = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btClose
            // 
            resources.ApplyResources(this.btClose, "btClose");
            this.btClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btClose.Name = "btClose";
            this.btClose.UseVisualStyleBackColor = true;
            // 
            // btCreate
            // 
            resources.ApplyResources(this.btCreate, "btCreate");
            this.btCreate.Name = "btCreate";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // btDelete
            // 
            resources.ApplyResources(this.btDelete, "btDelete");
            this.btDelete.Name = "btDelete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // btModify
            // 
            resources.ApplyResources(this.btModify, "btModify");
            this.btModify.Name = "btModify";
            this.btModify.UseVisualStyleBackColor = true;
            // 
            // listViewCardboardFormats
            // 
            resources.ApplyResources(this.listViewCardboardFormats, "listViewCardboardFormats");
            this.listViewCardboardFormats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDescription,
            this.colWidth,
            this.colHeight});
            this.listViewCardboardFormats.FullRowSelect = true;
            this.listViewCardboardFormats.GridLines = true;
            this.listViewCardboardFormats.HideSelection = false;
            this.listViewCardboardFormats.MultiSelect = false;
            this.listViewCardboardFormats.Name = "listViewCardboardFormats";
            this.listViewCardboardFormats.UseCompatibleStateImageBehavior = false;
            this.listViewCardboardFormats.View = System.Windows.Forms.View.Details;
            this.listViewCardboardFormats.SelectedIndexChanged += new System.EventHandler(this.listViewCardboardFormats_SelectedIndexChanged);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // colDescription
            // 
            resources.ApplyResources(this.colDescription, "colDescription");
            // 
            // colWidth
            // 
            resources.ApplyResources(this.colWidth, "colWidth");
            // 
            // colHeight
            // 
            resources.ApplyResources(this.colHeight, "colHeight");
            // 
            // FormEditCardboardFormats
            // 
            this.AcceptButton = this.btClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewCardboardFormats);
            this.Controls.Add(this.btModify);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.btClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditCardboardFormats";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btModify;
        private System.Windows.Forms.ListView listViewCardboardFormats;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colWidth;
        private System.Windows.Forms.ColumnHeader colHeight;
    }
}