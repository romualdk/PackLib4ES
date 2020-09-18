namespace Pic.Plugin.ViewCtrl
{
    partial class PluginViewCtrl : System.Windows.Forms.SplitContainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginViewCtrl));
            this.fitViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reflectionXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reflectionYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showCotationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.impositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // fitViewToolStripMenuItem
            // 
            this.fitViewToolStripMenuItem.Name = "fitViewToolStripMenuItem";
            resources.ApplyResources(this.fitViewToolStripMenuItem, "fitViewToolStripMenuItem");
            this.fitViewToolStripMenuItem.Click += new System.EventHandler(this.OnFitViewToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // reflectionXToolStripMenuItem
            // 
            this.reflectionXToolStripMenuItem.Name = "reflectionXToolStripMenuItem";
            resources.ApplyResources(this.reflectionXToolStripMenuItem, "reflectionXToolStripMenuItem");
            this.reflectionXToolStripMenuItem.Click += new System.EventHandler(this.ReflectionXToolStripMenuItem_Click);
            // 
            // reflectionYToolStripMenuItem
            // 
            this.reflectionYToolStripMenuItem.Name = "reflectionYToolStripMenuItem";
            resources.ApplyResources(this.reflectionYToolStripMenuItem, "reflectionYToolStripMenuItem");
            this.reflectionYToolStripMenuItem.Click += new System.EventHandler(this.ReflectionYToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // showCotationsToolStripMenuItem
            // 
            this.showCotationsToolStripMenuItem.Name = "showCotationsToolStripMenuItem";
            resources.ApplyResources(this.showCotationsToolStripMenuItem, "showCotationsToolStripMenuItem");
            this.showCotationsToolStripMenuItem.Click += new System.EventHandler(this.OnShowCotations);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // impositionToolStripMenuItem
            // 
            this.impositionToolStripMenuItem.Name = "impositionToolStripMenuItem";
            resources.ApplyResources(this.impositionToolStripMenuItem, "impositionToolStripMenuItem");
            this.impositionToolStripMenuItem.Click += new System.EventHandler(this.ImpositionToolStripMenuItem_Click);
            // 
            // PluginViewCtrl
            // 
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
        }

        protected override void OnResize(System.EventArgs e)
        {
 	        base.OnResize(e);
            this.SplitterDistance = Width - 250;
        }
        #endregion

        private System.Windows.Forms.ToolStripMenuItem fitViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem reflectionXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reflectionYToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem showCotationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem impositionToolStripMenuItem;
    }
}
