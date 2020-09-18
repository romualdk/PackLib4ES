﻿namespace PicParam
{
    partial class DockTreeView
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new PicParam.DocumentTreeView();
            this.factoryViewCtrl = new Pic.Factory2D.Control.FactoryViewerBase();
            this.webBrowser4PDF = new System.Windows.Forms.WebBrowser();
            this.unknownFormatViewCtrl = new PicParam.UnknownFormatViewControl();
            this.branchViewCtrl = new PicParam.BranchViewControl();
            this.pluginViewCtrl = new Pic.Plugin.ViewCtrl.PluginViewUCtrl();
            this.saveFileDialogBackup = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogRestore = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.factoryViewCtrl);
            this.splitContainer.Panel2.Controls.Add(this.webBrowser4PDF);
            this.splitContainer.Panel2.Controls.Add(this.unknownFormatViewCtrl);
            this.splitContainer.Panel2.Controls.Add(this.branchViewCtrl);
            this.splitContainer.Panel2.Controls.Add(this.pluginViewCtrl);
            this.splitContainer.Size = new System.Drawing.Size(800, 450);
            this.splitContainer.SplitterDistance = 266;
            this.splitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.Size = new System.Drawing.Size(266, 450);
            this.treeView.TabIndex = 0;
            // 
            // factoryViewCtrl
            // 
            this.factoryViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factoryViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.factoryViewCtrl.Name = "factoryViewCtrl";
            this.factoryViewCtrl.ReflectionX = false;
            this.factoryViewCtrl.ReflectionY = false;
            this.factoryViewCtrl.ShowAboutMenu = false;
            this.factoryViewCtrl.ShowAxes = false;
            this.factoryViewCtrl.ShowCotationsAuto = false;
            this.factoryViewCtrl.ShowCotationsCode = false;
            this.factoryViewCtrl.ShowNestingMenu = false;
            this.factoryViewCtrl.Size = new System.Drawing.Size(530, 450);
            this.factoryViewCtrl.TabIndex = 1;
            // 
            // webBrowser4PDF
            // 
            this.webBrowser4PDF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser4PDF.Location = new System.Drawing.Point(0, 0);
            this.webBrowser4PDF.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser4PDF.Name = "webBrowser4PDF";
            this.webBrowser4PDF.Size = new System.Drawing.Size(530, 450);
            this.webBrowser4PDF.TabIndex = 1;
            // 
            // unknownFormatViewCtrl
            // 
            this.unknownFormatViewCtrl.FilePath = null;
            this.unknownFormatViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.unknownFormatViewCtrl.Name = "unknownFormatViewCtrl";
            this.unknownFormatViewCtrl.Size = new System.Drawing.Size(300, 300);
            this.unknownFormatViewCtrl.TabIndex = 3;
            // 
            // branchViewCtrl
            // 
            this.branchViewCtrl.AutoScroll = true;
            this.branchViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.branchViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.branchViewCtrl.Name = "branchViewCtrl";
            this.branchViewCtrl.Size = new System.Drawing.Size(530, 450);
            this.branchViewCtrl.TabIndex = 0;
            // 
            // pluginViewCtrl
            // 
            this.pluginViewCtrl.Component = null;
            this.pluginViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginViewCtrl.HasDependancies = false;
            this.pluginViewCtrl.Localizer = null;
            this.pluginViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.pluginViewCtrl.Name = "pluginViewCtrl";
            // 
            // pluginViewCtrl.Panel2
            // 
            this.pluginViewCtrl.ReflectionX = false;
            this.pluginViewCtrl.ReflectionY = false;
            this.pluginViewCtrl.ShowAxes = true;
            this.pluginViewCtrl.ShowCotationsAuto = true;
            this.pluginViewCtrl.ShowCotationsCode = false;
            this.pluginViewCtrl.Size = new System.Drawing.Size(530, 450);
            this.pluginViewCtrl.TabIndex = 2;
            // 
            // saveFileDialogBackup
            // 
            this.saveFileDialogBackup.Filter = "Backup file (*.zip)|*.zip";
            // 
            // openFileDialogRestore
            // 
            this.openFileDialogRestore.Filter = "Backup file (*.zip)|*.zip";
            // 
            // FormTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer);
            this.Name = "FormTreeView";
            this.TabText = "";
            this.Text = "FormTreeView";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pluginViewCtrl.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private DocumentTreeView treeView;
        private BranchViewControl branchViewCtrl;
        private Pic.Factory2D.Control.FactoryViewerBase factoryViewCtrl;
        private Pic.Plugin.ViewCtrl.PluginViewUCtrl pluginViewCtrl;
        private System.Windows.Forms.WebBrowser webBrowser4PDF;
        private PicParam.UnknownFormatViewControl unknownFormatViewCtrl;
        private System.Windows.Forms.SaveFileDialog saveFileDialogBackup;
        private System.Windows.Forms.OpenFileDialog openFileDialogRestore;
    }
}