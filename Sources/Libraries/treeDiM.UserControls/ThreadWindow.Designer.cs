namespace treeDiM.UserControls
{
    partial class ThreadWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadWindow));
            this.bnCancel = new System.Windows.Forms.Button();
            this.rtbThreadOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.OnCancelButtonClicked);
            // 
            // rtbThreadOutput
            // 
            resources.ApplyResources(this.rtbThreadOutput, "rtbThreadOutput");
            this.rtbThreadOutput.Name = "rtbThreadOutput";
            // 
            // ThreadWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbThreadOutput);
            this.Controls.Add(this.bnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThreadWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.RichTextBox rtbThreadOutput;
    }
}