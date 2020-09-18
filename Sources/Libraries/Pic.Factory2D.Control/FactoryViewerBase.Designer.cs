namespace Pic.Factory2D.Control
{
    partial class FactoryViewerBase
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactoryViewerBase));
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fitViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reflectionXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reflectionYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCotationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dxfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.desToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.impositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cf2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fitViewToolStripMenuItem,
            this.toolStripSeparator1,
            this.reflectionXToolStripMenuItem,
            this.reflectionYToolStripMenuItem,
            this.showCotationsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportToolStripMenuItem,
            this.toolStripSeparator3,
            this.impositionToolStripMenuItem,
            this.toolStripSeparator4,
            this.aboutToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            // 
            // fitViewToolStripMenuItem
            // 
            resources.ApplyResources(this.fitViewToolStripMenuItem, "fitViewToolStripMenuItem");
            this.fitViewToolStripMenuItem.Name = "fitViewToolStripMenuItem";
            this.fitViewToolStripMenuItem.Click += new System.EventHandler(this.OnFitView);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // reflectionXToolStripMenuItem
            // 
            resources.ApplyResources(this.reflectionXToolStripMenuItem, "reflectionXToolStripMenuItem");
            this.reflectionXToolStripMenuItem.Name = "reflectionXToolStripMenuItem";
            this.reflectionXToolStripMenuItem.Click += new System.EventHandler(this.OnReflectionX);
            // 
            // reflectionYToolStripMenuItem
            // 
            resources.ApplyResources(this.reflectionYToolStripMenuItem, "reflectionYToolStripMenuItem");
            this.reflectionYToolStripMenuItem.Name = "reflectionYToolStripMenuItem";
            this.reflectionYToolStripMenuItem.Click += new System.EventHandler(this.OnReflectionY);
            // 
            // showCotationsToolStripMenuItem
            // 
            resources.ApplyResources(this.showCotationsToolStripMenuItem, "showCotationsToolStripMenuItem");
            this.showCotationsToolStripMenuItem.Name = "showCotationsToolStripMenuItem";
            this.showCotationsToolStripMenuItem.Click += new System.EventHandler(this.OnShowCotations);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // exportToolStripMenuItem
            // 
            resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dxfToolStripMenuItem,
            this.aiToolStripMenuItem,
            this.cf2ToolStripMenuItem,
            this.pdfToolStripMenuItem,
            this.desToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            // 
            // dxfToolStripMenuItem
            // 
            resources.ApplyResources(this.dxfToolStripMenuItem, "dxfToolStripMenuItem");
            this.dxfToolStripMenuItem.Name = "dxfToolStripMenuItem";
            this.dxfToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // pdfToolStripMenuItem
            // 
            resources.ApplyResources(this.pdfToolStripMenuItem, "pdfToolStripMenuItem");
            this.pdfToolStripMenuItem.Name = "pdfToolStripMenuItem";
            this.pdfToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // desToolStripMenuItem
            // 
            resources.ApplyResources(this.desToolStripMenuItem, "desToolStripMenuItem");
            this.desToolStripMenuItem.Name = "desToolStripMenuItem";
            this.desToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // impositionToolStripMenuItem
            // 
            resources.ApplyResources(this.impositionToolStripMenuItem, "impositionToolStripMenuItem");
            this.impositionToolStripMenuItem.Name = "impositionToolStripMenuItem";
            this.impositionToolStripMenuItem.Click += new System.EventHandler(this.OnBuildLayout);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnShowAboutBox);
            // 
            // aiToolStripMenuItem
            // 
            resources.ApplyResources(this.aiToolStripMenuItem, "aiToolStripMenuItem");
            this.aiToolStripMenuItem.Name = "aiToolStripMenuItem";
            this.aiToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // cf2ToolStripMenuItem
            // 
            resources.ApplyResources(this.cf2ToolStripMenuItem, "cf2ToolStripMenuItem");
            this.cf2ToolStripMenuItem.Name = "cf2ToolStripMenuItem";
            this.cf2ToolStripMenuItem.Click += new System.EventHandler(this.OnExport);
            // 
            // FactoryViewerBase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "FactoryViewerBase";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fitViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem reflectionXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reflectionYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCotationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dxfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem desToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem impositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cf2ToolStripMenuItem;
    }
}
