namespace Pic.Plugin.ViewCtrl
{
    partial class PluginViewUCtrl
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
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.splitContHorizLeft = new System.Windows.Forms.SplitContainer();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.splitContHorizRight = new System.Windows.Forms.SplitContainer();
            this.factoryDataCtrl = new Pic.Factory2D.Control.FactoryDataCtrl();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMIFitView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMIReflectionX = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMIReflectionY = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMIShowCotation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.impositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContHorizLeft)).BeginInit();
            this.splitContHorizLeft.Panel2.SuspendLayout();
            this.splitContHorizLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContHorizRight)).BeginInit();
            this.splitContHorizRight.Panel2.SuspendLayout();
            this.splitContHorizRight.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerVert
            // 
            this.splitContainerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerVert.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerVert.Location = new System.Drawing.Point(0, 0);
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.splitContHorizLeft);
            this.splitContainerVert.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnDrawingPanelPaint);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.splitContHorizRight);
            this.splitContainerVert.Size = new System.Drawing.Size(800, 500);
            this.splitContainerVert.SplitterDistance = 503;
            this.splitContainerVert.TabIndex = 0;
            // 
            // splitContHorizLeft
            // 
            this.splitContHorizLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContHorizLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContHorizLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContHorizLeft.Name = "splitContHorizLeft";
            this.splitContHorizLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContHorizLeft.Panel1
            // 
            this.splitContHorizLeft.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnDrawingPanelPaint);
            this.splitContHorizLeft.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.splitContHorizLeft.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.splitContHorizLeft.Panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
            // 
            // splitContHorizLeft.Panel2
            // 
            this.splitContHorizLeft.Panel2.Controls.Add(this.rtbMessages);
            this.splitContHorizLeft.Size = new System.Drawing.Size(503, 500);
            this.splitContHorizLeft.SplitterDistance = 465;
            this.splitContHorizLeft.TabIndex = 0;
            // 
            // rtbMessages
            // 
            this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessages.ForeColor = System.Drawing.Color.Red;
            this.rtbMessages.Location = new System.Drawing.Point(0, 0);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.ReadOnly = true;
            this.rtbMessages.Size = new System.Drawing.Size(503, 31);
            this.rtbMessages.TabIndex = 0;
            this.rtbMessages.Text = "";
            // 
            // splitContHorizRight
            // 
            this.splitContHorizRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContHorizRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContHorizRight.Location = new System.Drawing.Point(0, 0);
            this.splitContHorizRight.Name = "splitContHorizRight";
            this.splitContHorizRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContHorizRight.Panel2
            // 
            this.splitContHorizRight.Panel2.Controls.Add(this.factoryDataCtrl);
            this.splitContHorizRight.Size = new System.Drawing.Size(293, 500);
            this.splitContHorizRight.SplitterDistance = 390;
            this.splitContHorizRight.TabIndex = 0;
            // 
            // factoryDataCtrl
            // 
            this.factoryDataCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factoryDataCtrl.Location = new System.Drawing.Point(0, 0);
            this.factoryDataCtrl.Name = "factoryDataCtrl";
            this.factoryDataCtrl.Size = new System.Drawing.Size(293, 106);
            this.factoryDataCtrl.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMIFitView,
            this.toolStripSeparator1,
            this.toolStripMIReflectionX,
            this.toolStripMIReflectionY,
            this.toolStripSeparator2,
            this.toolStripMIShowCotation,
            this.toolStripSeparator3,
            this.impositionToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(151, 132);
            // 
            // toolStripMIFitView
            // 
            this.toolStripMIFitView.Name = "toolStripMIFitView";
            this.toolStripMIFitView.Size = new System.Drawing.Size(150, 22);
            this.toolStripMIFitView.Text = "Fit view";
            this.toolStripMIFitView.Click += new System.EventHandler(this.OnFitView);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // toolStripMIReflectionX
            // 
            this.toolStripMIReflectionX.Name = "toolStripMIReflectionX";
            this.toolStripMIReflectionX.Size = new System.Drawing.Size(150, 22);
            this.toolStripMIReflectionX.Text = "Reflection X";
            this.toolStripMIReflectionX.Click += new System.EventHandler(this.OnReflectionX);
            // 
            // toolStripMIReflectionY
            // 
            this.toolStripMIReflectionY.Name = "toolStripMIReflectionY";
            this.toolStripMIReflectionY.Size = new System.Drawing.Size(150, 22);
            this.toolStripMIReflectionY.Text = "Reflection Y";
            this.toolStripMIReflectionY.Click += new System.EventHandler(this.OnReflectionY);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // toolStripMIShowCotation
            // 
            this.toolStripMIShowCotation.Name = "toolStripMIShowCotation";
            this.toolStripMIShowCotation.Size = new System.Drawing.Size(150, 22);
            this.toolStripMIShowCotation.Text = "Show cotation";
            this.toolStripMIShowCotation.Click += new System.EventHandler(this.OnShowCotations);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(147, 6);
            // 
            // impositionToolStripMenuItem
            // 
            this.impositionToolStripMenuItem.Name = "impositionToolStripMenuItem";
            this.impositionToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.impositionToolStripMenuItem.Text = "Imposition";
            this.impositionToolStripMenuItem.Click += new System.EventHandler(this.OnImposition);
            // 
            // PluginViewUCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerVert);
            this.Name = "PluginViewUCtrl";
            this.Size = new System.Drawing.Size(800, 500);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.splitContHorizLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContHorizLeft)).EndInit();
            this.splitContHorizLeft.ResumeLayout(false);
            this.splitContHorizRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContHorizRight)).EndInit();
            this.splitContHorizRight.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.SplitContainer splitContHorizRight;
        private System.Windows.Forms.SplitContainer splitContHorizLeft;
        private Factory2D.Control.FactoryDataCtrl factoryDataCtrl;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.ToolStripMenuItem toolStripMIShowCotation;
        private System.Windows.Forms.ToolStripMenuItem toolStripMIReflectionX;
        private System.Windows.Forms.ToolStripMenuItem toolStripMIReflectionY;
        private System.Windows.Forms.ToolStripMenuItem toolStripMIFitView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem impositionToolStripMenuItem;
    }
}
