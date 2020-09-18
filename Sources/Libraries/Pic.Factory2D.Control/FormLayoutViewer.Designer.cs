namespace Pic.Factory2D.Control
{
    partial class FormLayoutViewer
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
            this.grpbLengthes = new System.Windows.Forms.GroupBox();
            this.lblNameTotal = new System.Windows.Forms.Label();
            this.pbFold = new System.Windows.Forms.PictureBox();
            this.pbCut = new System.Windows.Forms.PictureBox();
            this.lblValueLengthTotal = new System.Windows.Forms.Label();
            this.lblValueLengthFold = new System.Windows.Forms.Label();
            this.lblValueLengthCut = new System.Windows.Forms.Label();
            this.grpbArea = new System.Windows.Forms.GroupBox();
            this.lblValueEfficiency = new System.Windows.Forms.Label();
            this.lblNameEfficiency = new System.Windows.Forms.Label();
            this.lblValueFormat = new System.Windows.Forms.Label();
            this.lblNameFormat = new System.Windows.Forms.Label();
            this.lblNameArea = new System.Windows.Forms.Label();
            this.lblValueArea = new System.Windows.Forms.Label();
            this.listBoxSolutions = new System.Windows.Forms.ListBox();
            this.factoryViewer = new Pic.Factory2D.Control.FactoryViewer();
            this.grpbCardboardFormat = new System.Windows.Forms.GroupBox();
            this.lblValueCardboardEfficiency = new System.Windows.Forms.Label();
            this.lblCardboardEfficiency = new System.Windows.Forms.Label();
            this.lblValueCardboardFormat = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblValueNumbers = new System.Windows.Forms.Label();
            this.lblNameNumbers = new System.Windows.Forms.Label();
            this.bnToNewFile = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tabControlData = new System.Windows.Forms.TabControl();
            this.tabPageFormat = new System.Windows.Forms.TabPage();
            this.tabPageLengthes = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.bnToCurrentFile = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.grpbLengthes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCut)).BeginInit();
            this.grpbArea.SuspendLayout();
            this.grpbCardboardFormat.SuspendLayout();
            this.tabControlData.SuspendLayout();
            this.tabPageFormat.SuspendLayout();
            this.tabPageLengthes.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpbLengthes
            // 
            this.grpbLengthes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpbLengthes.Controls.Add(this.lblNameTotal);
            this.grpbLengthes.Controls.Add(this.pbFold);
            this.grpbLengthes.Controls.Add(this.pbCut);
            this.grpbLengthes.Controls.Add(this.lblValueLengthTotal);
            this.grpbLengthes.Controls.Add(this.lblValueLengthFold);
            this.grpbLengthes.Controls.Add(this.lblValueLengthCut);
            this.grpbLengthes.Location = new System.Drawing.Point(7, 6);
            this.grpbLengthes.Name = "grpbLengthes";
            this.grpbLengthes.Size = new System.Drawing.Size(203, 97);
            this.grpbLengthes.TabIndex = 31;
            this.grpbLengthes.TabStop = false;
            this.grpbLengthes.Text = "Lengthes";
            // 
            // lblNameTotal
            // 
            this.lblNameTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNameTotal.AutoSize = true;
            this.lblNameTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNameTotal.Location = new System.Drawing.Point(6, 59);
            this.lblNameTotal.Name = "lblNameTotal";
            this.lblNameTotal.Size = new System.Drawing.Size(31, 13);
            this.lblNameTotal.TabIndex = 9;
            this.lblNameTotal.Text = "Total";
            // 
            // pbFold
            // 
            this.pbFold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFold.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbFold.Location = new System.Drawing.Point(6, 35);
            this.pbFold.Name = "pbFold";
            this.pbFold.Size = new System.Drawing.Size(71, 13);
            this.pbFold.TabIndex = 6;
            this.pbFold.TabStop = false;
            this.pbFold.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPbFold);
            // 
            // pbCut
            // 
            this.pbCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCut.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbCut.Location = new System.Drawing.Point(6, 17);
            this.pbCut.Name = "pbCut";
            this.pbCut.Size = new System.Drawing.Size(71, 13);
            this.pbCut.TabIndex = 5;
            this.pbCut.TabStop = false;
            this.pbCut.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPbCut);
            // 
            // lblValueLengthTotal
            // 
            this.lblValueLengthTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueLengthTotal.AutoSize = true;
            this.lblValueLengthTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueLengthTotal.Location = new System.Drawing.Point(85, 59);
            this.lblValueLengthTotal.Name = "lblValueLengthTotal";
            this.lblValueLengthTotal.Size = new System.Drawing.Size(39, 13);
            this.lblValueLengthTotal.TabIndex = 4;
            this.lblValueLengthTotal.Text = ": 0.0 m";
            // 
            // lblValueLengthFold
            // 
            this.lblValueLengthFold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueLengthFold.AutoSize = true;
            this.lblValueLengthFold.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueLengthFold.Location = new System.Drawing.Point(85, 35);
            this.lblValueLengthFold.Name = "lblValueLengthFold";
            this.lblValueLengthFold.Size = new System.Drawing.Size(39, 13);
            this.lblValueLengthFold.TabIndex = 3;
            this.lblValueLengthFold.Text = ": 0.0 m";
            // 
            // lblValueLengthCut
            // 
            this.lblValueLengthCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueLengthCut.AutoSize = true;
            this.lblValueLengthCut.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueLengthCut.Location = new System.Drawing.Point(85, 17);
            this.lblValueLengthCut.Name = "lblValueLengthCut";
            this.lblValueLengthCut.Size = new System.Drawing.Size(39, 13);
            this.lblValueLengthCut.TabIndex = 1;
            this.lblValueLengthCut.Text = ": 0.0 m";
            // 
            // grpbArea
            // 
            this.grpbArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpbArea.Controls.Add(this.lblValueEfficiency);
            this.grpbArea.Controls.Add(this.lblNameEfficiency);
            this.grpbArea.Controls.Add(this.lblValueFormat);
            this.grpbArea.Controls.Add(this.lblNameFormat);
            this.grpbArea.Controls.Add(this.lblNameArea);
            this.grpbArea.Controls.Add(this.lblValueArea);
            this.grpbArea.Location = new System.Drawing.Point(6, 6);
            this.grpbArea.Name = "grpbArea";
            this.grpbArea.Size = new System.Drawing.Size(177, 77);
            this.grpbArea.TabIndex = 32;
            this.grpbArea.TabStop = false;
            this.grpbArea.Text = "Cutting area";
            // 
            // lblValueEfficiency
            // 
            this.lblValueEfficiency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueEfficiency.AutoSize = true;
            this.lblValueEfficiency.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueEfficiency.Location = new System.Drawing.Point(91, 55);
            this.lblValueEfficiency.Name = "lblValueEfficiency";
            this.lblValueEfficiency.Size = new System.Drawing.Size(36, 13);
            this.lblValueEfficiency.TabIndex = 14;
            this.lblValueEfficiency.Text = ": 0.0%";
            // 
            // lblNameEfficiency
            // 
            this.lblNameEfficiency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNameEfficiency.AutoSize = true;
            this.lblNameEfficiency.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNameEfficiency.Location = new System.Drawing.Point(12, 55);
            this.lblNameEfficiency.Name = "lblNameEfficiency";
            this.lblNameEfficiency.Size = new System.Drawing.Size(53, 13);
            this.lblNameEfficiency.TabIndex = 13;
            this.lblNameEfficiency.Text = "Efficiency";
            // 
            // lblValueFormat
            // 
            this.lblValueFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueFormat.AutoSize = true;
            this.lblValueFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueFormat.Location = new System.Drawing.Point(91, 36);
            this.lblValueFormat.Name = "lblValueFormat";
            this.lblValueFormat.Size = new System.Drawing.Size(54, 13);
            this.lblValueFormat.TabIndex = 12;
            this.lblValueFormat.Text = ": 0.0 x 0.0";
            // 
            // lblNameFormat
            // 
            this.lblNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNameFormat.AutoSize = true;
            this.lblNameFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNameFormat.Location = new System.Drawing.Point(12, 36);
            this.lblNameFormat.Name = "lblNameFormat";
            this.lblNameFormat.Size = new System.Drawing.Size(39, 13);
            this.lblNameFormat.TabIndex = 11;
            this.lblNameFormat.Text = "Format";
            // 
            // lblNameArea
            // 
            this.lblNameArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNameArea.AutoSize = true;
            this.lblNameArea.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNameArea.Location = new System.Drawing.Point(12, 18);
            this.lblNameArea.Name = "lblNameArea";
            this.lblNameArea.Size = new System.Drawing.Size(29, 13);
            this.lblNameArea.TabIndex = 10;
            this.lblNameArea.Text = "Area";
            // 
            // lblValueArea
            // 
            this.lblValueArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueArea.AutoSize = true;
            this.lblValueArea.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueArea.Location = new System.Drawing.Point(91, 18);
            this.lblValueArea.Name = "lblValueArea";
            this.lblValueArea.Size = new System.Drawing.Size(42, 13);
            this.lblValueArea.TabIndex = 8;
            this.lblValueArea.Text = ": 0.0 m²";
            // 
            // listBoxSolutions
            // 
            this.listBoxSolutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSolutions.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxSolutions.FormattingEnabled = true;
            this.listBoxSolutions.ItemHeight = 50;
            this.listBoxSolutions.Location = new System.Drawing.Point(510, 15);
            this.listBoxSolutions.Margin = new System.Windows.Forms.Padding(1);
            this.listBoxSolutions.Name = "listBoxSolutions";
            this.listBoxSolutions.Size = new System.Drawing.Size(224, 304);
            this.listBoxSolutions.TabIndex = 30;
            this.listBoxSolutions.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnDrawItemListBoxSolution);
            // 
            // factoryViewer
            // 
            this.factoryViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.factoryViewer.Location = new System.Drawing.Point(2, 15);
            this.factoryViewer.Name = "factoryViewer";
            this.factoryViewer.ReflectionX = false;
            this.factoryViewer.ReflectionY = false;
            this.factoryViewer.ShowAboutMenu = false;
            this.factoryViewer.ShowCotations = false;
            this.factoryViewer.ShowNestingMenu = false;
            this.factoryViewer.Size = new System.Drawing.Size(503, 541);
            this.factoryViewer.TabIndex = 29;
            // 
            // grpbCardboardFormat
            // 
            this.grpbCardboardFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpbCardboardFormat.Controls.Add(this.lblValueCardboardEfficiency);
            this.grpbCardboardFormat.Controls.Add(this.lblCardboardEfficiency);
            this.grpbCardboardFormat.Controls.Add(this.lblValueCardboardFormat);
            this.grpbCardboardFormat.Controls.Add(this.lblFormat);
            this.grpbCardboardFormat.Controls.Add(this.lblValueNumbers);
            this.grpbCardboardFormat.Controls.Add(this.lblNameNumbers);
            this.grpbCardboardFormat.Location = new System.Drawing.Point(6, 6);
            this.grpbCardboardFormat.Name = "grpbCardboardFormat";
            this.grpbCardboardFormat.Size = new System.Drawing.Size(204, 97);
            this.grpbCardboardFormat.TabIndex = 33;
            this.grpbCardboardFormat.TabStop = false;
            this.grpbCardboardFormat.Text = "Cardboard format";
            // 
            // lblValueCardboardEfficiency
            // 
            this.lblValueCardboardEfficiency.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueCardboardEfficiency.Location = new System.Drawing.Point(91, 42);
            this.lblValueCardboardEfficiency.Name = "lblValueCardboardEfficiency";
            this.lblValueCardboardEfficiency.Size = new System.Drawing.Size(36, 13);
            this.lblValueCardboardEfficiency.TabIndex = 32;
            this.lblValueCardboardEfficiency.Text = ": 0.0%";
            // 
            // lblCardboardEfficiency
            // 
            this.lblCardboardEfficiency.AutoSize = true;
            this.lblCardboardEfficiency.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCardboardEfficiency.Location = new System.Drawing.Point(12, 42);
            this.lblCardboardEfficiency.Name = "lblCardboardEfficiency";
            this.lblCardboardEfficiency.Size = new System.Drawing.Size(53, 13);
            this.lblCardboardEfficiency.TabIndex = 31;
            this.lblCardboardEfficiency.Text = "Efficiency";
            // 
            // lblValueCardboardFormat
            // 
            this.lblValueCardboardFormat.AutoSize = true;
            this.lblValueCardboardFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueCardboardFormat.Location = new System.Drawing.Point(91, 23);
            this.lblValueCardboardFormat.Name = "lblValueCardboardFormat";
            this.lblValueCardboardFormat.Size = new System.Drawing.Size(78, 13);
            this.lblValueCardboardFormat.TabIndex = 30;
            this.lblValueCardboardFormat.Text = ": 0.0 x 0.0 (0.0)";
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFormat.Location = new System.Drawing.Point(12, 23);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(39, 13);
            this.lblFormat.TabIndex = 29;
            this.lblFormat.Text = "Format";
            // 
            // lblValueNumbers
            // 
            this.lblValueNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValueNumbers.AutoSize = true;
            this.lblValueNumbers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblValueNumbers.Location = new System.Drawing.Point(91, 62);
            this.lblValueNumbers.Name = "lblValueNumbers";
            this.lblValueNumbers.Size = new System.Drawing.Size(45, 13);
            this.lblValueNumbers.TabIndex = 27;
            this.lblValueNumbers.Text = ": 0 (0x0)";
            // 
            // lblNameNumbers
            // 
            this.lblNameNumbers.AutoSize = true;
            this.lblNameNumbers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNameNumbers.Location = new System.Drawing.Point(12, 62);
            this.lblNameNumbers.Name = "lblNameNumbers";
            this.lblNameNumbers.Size = new System.Drawing.Size(49, 13);
            this.lblNameNumbers.TabIndex = 7;
            this.lblNameNumbers.Text = "Numbers";
            // 
            // bnToNewFile
            // 
            this.bnToNewFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnToNewFile.Location = new System.Drawing.Point(592, 471);
            this.bnToNewFile.Name = "bnToNewFile";
            this.bnToNewFile.Size = new System.Drawing.Size(142, 23);
            this.bnToNewFile.TabIndex = 34;
            this.bnToNewFile.Text = "> To New File";
            this.bnToNewFile.UseVisualStyleBackColor = true;
            this.bnToNewFile.Click += new System.EventHandler(this.OnSaveAsNewFile);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "des";
            this.saveFileDialog.Filter = "PicGEOM (*.des)|*.des";
            this.saveFileDialog.Title = "Save file...";
            // 
            // tabControlData
            // 
            this.tabControlData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlData.Controls.Add(this.tabPageFormat);
            this.tabControlData.Controls.Add(this.tabPageLengthes);
            this.tabControlData.Controls.Add(this.tabPage3);
            this.tabControlData.Location = new System.Drawing.Point(511, 327);
            this.tabControlData.Name = "tabControlData";
            this.tabControlData.SelectedIndex = 0;
            this.tabControlData.Size = new System.Drawing.Size(224, 135);
            this.tabControlData.TabIndex = 35;
            // 
            // tabPageFormat
            // 
            this.tabPageFormat.Controls.Add(this.grpbCardboardFormat);
            this.tabPageFormat.Location = new System.Drawing.Point(4, 22);
            this.tabPageFormat.Name = "tabPageFormat";
            this.tabPageFormat.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFormat.Size = new System.Drawing.Size(216, 109);
            this.tabPageFormat.TabIndex = 0;
            this.tabPageFormat.Text = "Format";
            this.tabPageFormat.UseVisualStyleBackColor = true;
            // 
            // tabPageLengthes
            // 
            this.tabPageLengthes.Controls.Add(this.grpbLengthes);
            this.tabPageLengthes.Location = new System.Drawing.Point(4, 22);
            this.tabPageLengthes.Name = "tabPageLengthes";
            this.tabPageLengthes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLengthes.Size = new System.Drawing.Size(216, 109);
            this.tabPageLengthes.TabIndex = 1;
            this.tabPageLengthes.Text = "Lengthes";
            this.tabPageLengthes.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.grpbArea);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(216, 109);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Cutting area";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // bnToCurrentFile
            // 
            this.bnToCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnToCurrentFile.Enabled = false;
            this.bnToCurrentFile.Location = new System.Drawing.Point(592, 500);
            this.bnToCurrentFile.Name = "bnToCurrentFile";
            this.bnToCurrentFile.Size = new System.Drawing.Size(142, 23);
            this.bnToCurrentFile.TabIndex = 36;
            this.bnToCurrentFile.Text = "> To current file";
            this.bnToCurrentFile.UseVisualStyleBackColor = true;
            this.bnToCurrentFile.Click += new System.EventHandler(this.OnSaveToCurrentFile);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(592, 530);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(142, 23);
            this.bnCancel.TabIndex = 37;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // FormLayoutViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(739, 561);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnToCurrentFile);
            this.Controls.Add(this.tabControlData);
            this.Controls.Add(this.bnToNewFile);
            this.Controls.Add(this.listBoxSolutions);
            this.Controls.Add(this.factoryViewer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLayoutViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select layout...";
            this.grpbLengthes.ResumeLayout(false);
            this.grpbLengthes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCut)).EndInit();
            this.grpbArea.ResumeLayout(false);
            this.grpbArea.PerformLayout();
            this.grpbCardboardFormat.ResumeLayout(false);
            this.grpbCardboardFormat.PerformLayout();
            this.tabControlData.ResumeLayout(false);
            this.tabPageFormat.ResumeLayout(false);
            this.tabPageLengthes.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpbLengthes;
        private System.Windows.Forms.Label lblNameTotal;
        private System.Windows.Forms.PictureBox pbFold;
        private System.Windows.Forms.PictureBox pbCut;
        private System.Windows.Forms.Label lblValueLengthTotal;
        private System.Windows.Forms.Label lblValueLengthFold;
        private System.Windows.Forms.Label lblValueLengthCut;
        private System.Windows.Forms.GroupBox grpbArea;
        private System.Windows.Forms.Label lblValueEfficiency;
        private System.Windows.Forms.Label lblNameEfficiency;
        private System.Windows.Forms.Label lblValueFormat;
        private System.Windows.Forms.Label lblNameFormat;
        private System.Windows.Forms.Label lblNameArea;
        private System.Windows.Forms.Label lblValueArea;
        private System.Windows.Forms.ListBox listBoxSolutions;
        private FactoryViewer factoryViewer;
        private System.Windows.Forms.GroupBox grpbCardboardFormat;
        private System.Windows.Forms.Label lblValueCardboardEfficiency;
        private System.Windows.Forms.Label lblCardboardEfficiency;
        private System.Windows.Forms.Label lblValueCardboardFormat;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.Label lblValueNumbers;
        private System.Windows.Forms.Label lblNameNumbers;
        private System.Windows.Forms.Button bnToNewFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TabControl tabControlData;
        private System.Windows.Forms.TabPage tabPageFormat;
        private System.Windows.Forms.TabPage tabPageLengthes;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button bnToCurrentFile;
        private System.Windows.Forms.Button bnCancel;
    }
}