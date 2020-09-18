#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.ThreadCallback;
using treeDiM.UserControls;
#endregion

namespace Pic.Factory2D.Control
{
    public partial class FormImposition : Form, IEntitySupplier
    {
        #region Constructor
        public FormImposition()
        {
            InitializeComponent();

            // set FactoryViewer.EntitySupplier
            factoryViewer.EntitySupplier = this;
        }
        #endregion
        #region IEntitySupplier implementation
        public void CreateEntities(PicFactory factory)
        {
            if (!(listBoxSolutions.SelectedItem is ImpositionSolution solution)) return;
            solution.CreateEntities(factory);
 
            // insert dimensions
            short grp = 0;
            if (PicCotation.GetBBCotations(solution.Bbox, CotTypeBB, 0.1, out Vector2D pt0, out Vector2D pt1, out Vector2D pt2, out Vector2D pt3, out double delta0, out double delta1))
            {
                PicCotationDistance cotH = factory.AddCotation(PicCotation.CotType.COT_HORIZONTAL, grp, 0, pt0, pt1, delta0, "", 1) as PicCotationDistance;
                cotH.UseShortLines = true;
                cotH.Auto = true;

                PicCotationDistance cotV = factory.AddCotation(PicCotation.CotType.COT_VERTICAL, grp, 0, pt2, pt3, delta1, "", 1) as PicCotationDistance;
                cotV.UseShortLines = true;
                cotV.Auto = true;
            }


            if (null != Format && PicCotation.GetBBCotations(new Box2D(solution.CardboardPosition, solution.CardboardDimensions), CotTypeFormat, 0.1, out Vector2D ptf0, out Vector2D ptf1, out Vector2D ptf2, out Vector2D ptf3, out double deltaf0, out double deltaf1))
            {
                PicCotationDistance cotH = factory.AddCotation(PicCotation.CotType.COT_HORIZONTAL, grp, 0, ptf0, ptf1, deltaf0, "", 1) as PicCotationDistance;
                cotH.UseShortLines = true;
                cotH.Auto = false;

                PicCotationDistance cotV = factory.AddCotation(PicCotation.CotType.COT_VERTICAL, grp, 0, ptf2, ptf3, deltaf1, "", 1) as PicCotationDistance;
                cotV.UseShortLines = true;
                cotV.Auto = false;
            }
            factory.InsertCardboardFormat(solution.CardboardPosition, solution.CardboardDimensions);
        }
        #endregion
        #region Public properties
        public List<ImpositionSolution> Solutions { set { solutions = value; } }
        public CardboardFormatLoader FormatLoader { get; set; }
        public CardboardFormat Format => cbCardboardFormat.SelectedItem as CardboardFormat;
        public ImpositionSettings.HAlignment HorizontalAlignment => Mode == 0 ? (ImpositionSettings.HAlignment)cbRightLeft.SelectedIndex : ImpositionSettings.HAlignment.HALIGN_LEFT;
        public ImpositionSettings.VAlignment VerticalAlignment => Mode == 0 ? (ImpositionSettings.VAlignment)cbTopBottom.SelectedIndex : ImpositionSettings.VAlignment.VALIGN_BOTTOM;
        private int NumberDirX => (int)nudNumberX.Value;
        private int NumberDirY => (int)nudNumberY.Value;
        private int Mode => rbCardboardFormat.Checked ? 0 : 1;
        private Vector2D ImpSpace => new Vector2D((double)nudSpaceX.Value, (double)nudSpaceY.Value);
        public Vector2D Offset
        {
            get { return new Vector2D((double)nudOffsetX.Value, (double)nudOffsetY.Value); }
            set { nudOffsetX.Value = (decimal)value.X; nudOffsetY.Value = (decimal)value.Y; }
        }
        private Vector2D ImpMargin => new Vector2D((double)nudLeftRightMargin.Value, (double)nudTopBottomMargin.Value);
        private Vector2D ImpRemainingMargin => new Vector2D((double)nudLeftRightRemaining.Value, (double)nudTopBottomRemaining.Value);
        public bool AllowRowRotation => chkbAllowRowRotation.Checked;
        public bool AllowColumnRotation => chkbAllowColumnRotation.Checked;
        public bool AllowBothDirection => chkbAllowBothDirection.Checked;
        public IEntityContainer Factory { get; set; }
        private ImpositionSettings ImpSettings => (0 == Mode) ? new ImpositionSettings(Format): new ImpositionSettings(NumberDirX, NumberDirY);
        private bool ShowCotations => cbCotationsBB.SelectedIndex > 0 || cbCotationsFormat.SelectedIndex > 0;
        private PicCotation.CotBBType CotTypeBB => (PicCotation.CotBBType)cbCotationsBB.SelectedIndex;
        private PicCotation.CotBBType CotTypeFormat => (PicCotation.CotBBType)cbCotationsFormat.SelectedIndex;
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            // make ToolStripButton available
            toolStripButtonOceProCut.Available = Properties.Settings.Default.TSButtonAvailableOceProCut;
            // fill combo
            FillCombo();
            // mode
            rbCardboardFormat.Checked = Properties.Settings.Default.LayoutMode == 0;
            // number of rows and columns
            nudNumberX.Value = Properties.Settings.Default.NumberDirX;
            nudNumberY.Value = Properties.Settings.Default.NumberDirY;
            // initialize margins
            cbTopBottom.SelectedIndex = Properties.Settings.Default.ComboTopBottomIndex;
            cbRightLeft.SelectedIndex = Properties.Settings.Default.ComboLeftRightIndex;
            nudTopBottomMargin.Value = Properties.Settings.Default.ImpositionMarginTopBottom;
            nudTopBottomRemaining.Value = Properties.Settings.Default.ImpositionRemainingMarginTopBottom;
            nudLeftRightMargin.Value = Properties.Settings.Default.ImpositionMarginLeftRight;
            nudLeftRightRemaining.Value = Properties.Settings.Default.ImpositionRemainingMarginLeftRight;
            nudSpaceX.Value = Properties.Settings.Default.ImpositionSpaceX;
            nudSpaceY.Value = Properties.Settings.Default.ImpositionSpaceY;
            chkbAllowBothDirection.Checked = Properties.Settings.Default.AllowBothDirections;

            cbCotationsBB.SelectedIndex = 0;
            cbCotationsFormat.SelectedIndex = 0;

            if (listBoxSolutions.Items.Count > 0)
                listBoxSolutions.SelectedIndex = 0;
            // initialize factoryViewer
            factoryViewer.ReflectionX = false;
            factoryViewer.ReflectionY = false;
            factoryViewer.ShowCotations = false;
            factoryViewer.ShowNestingMenu = false;
            factoryViewer.ShowAboutMenu = false;
            factoryViewer.Refresh();

            listBoxSolutions.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);
            factoryViewer.ShowCotationsToggled += new FactoryViewer.StatusChanged(ShowCotationsToggled);

            base.OnLoad(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // cardboard format
            Properties.Settings.Default.ImpositionCarboardFormat = cbCardboardFormat.SelectedIndex;
            // margins
            Properties.Settings.Default.ComboTopBottomIndex = cbTopBottom.SelectedIndex;
            Properties.Settings.Default.ComboLeftRightIndex = cbRightLeft.SelectedIndex;
            Properties.Settings.Default.ImpositionMarginTopBottom = nudTopBottomMargin.Value;
            Properties.Settings.Default.ImpositionRemainingMarginTopBottom = nudTopBottomRemaining.Value;
            Properties.Settings.Default.ImpositionMarginLeftRight = nudLeftRightMargin.Value;
            Properties.Settings.Default.ImpositionRemainingMarginLeftRight = nudLeftRightRemaining.Value;
            Properties.Settings.Default.ImpositionSpaceX = nudSpaceX.Value;
            Properties.Settings.Default.ImpositionSpaceY = nudSpaceY.Value;
            Properties.Settings.Default.NumberDirX = NumberDirX;
            Properties.Settings.Default.NumberDirX = NumberDirX;
            Properties.Settings.Default.LayoutMode = rbCardboardFormat.Checked ? 0 : 1;
        }
        #endregion
        #region Handlers
        void ShowCotationsToggled(object sender, EventArgsStatus e)   {}
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Z:
                    factoryViewer.FitView();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        public void OnClose(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
        #region Paint event handlers
        /// <summary>
        /// Paint event handler for Cut length label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCutPaint(object sender, PaintEventArgs e)
        {
            Size sz = pbCut.Size;
            e.Graphics.DrawLine(new Pen(Color.Red), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        /// <summary>
        /// Paint event handler for Fold length
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFoldPaint(object sender, PaintEventArgs e)
        {
            Size sz = pbFold.Size;
            e.Graphics.DrawLine(new Pen(Color.Blue), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        #endregion
        #region listBoxSolutions event handlers
        /// <summary>
        /// listBoxSolution: selected item changed
        /// </summary>
        void OnSelectedValueChanged(object sender, EventArgs e)
        {
            factoryViewer.ShowCotations = ShowCotations;
            factoryViewer.Refresh();

            if (!(listBoxSolutions.SelectedItem is ImpositionSolution solution)) return;

            // numbers
            lblValueCardboardFormat.Text = string.Format(": {0} x {1}",
                (int)Math.Ceiling(solution.CardboardDimensions.X),
                (int)Math.Ceiling(solution.CardboardDimensions.Y));
            lblValueCardboardEfficiency.Text = string.Format(": {0:0.#} %",
                100.0 * solution.Area / (solution.CardboardDimensions.X * solution.CardboardDimensions.Y));
            lblValueNumbers.Text = string.Format(": {0} ({1} x {2})",
                solution.PositionCount, solution.Rows, solution.Cols);
            // lengthes
            lblValueLengthCut.Text = string.Format(": {0:0.###} m", solution.LengthCut / 1000.0);
            lblValueLengthFold.Text = string.Format(": {0:0.###} m ", solution.LengthFold / 1000.0);
            lblValueLengthTotal.Text = string.Format(": {0:0.###} m", (solution.LengthCut + solution.LengthFold) / 1000.0);
            // area
            lblValueArea.Text = string.Format(": {0:0.###} m²", solution.Area * 1.0E-06);
            lblValueFormat.Text = string.Format(": {0:0.#} x {1:0.#}", solution.Width, solution.Height);
            lblValueEfficiency.Text = string.Format(": {0:0.#} %", 100.0 * solution.Area / (solution.Width * solution.Height));
        }
        /// <summary>
        /// Owner draw mode item drawing method
        /// </summary>
        private void ListBoxSolutionsDrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            int itemHeight = listBoxSolutions.ItemHeight;
            // draw image
            Rectangle _drawRect = new Rectangle(0, 0, itemHeight, itemHeight);
            Rectangle scaledRect = _drawRect;
            Rectangle imageRect = e.Bounds;
            imageRect.Y += 1;
            imageRect.Height = scaledRect.Height;
            imageRect.X += 2;
            imageRect.Width = scaledRect.Width;

            if (null != solutions[e.Index].Thumbnail)
                g.DrawImage(solutions[e.Index].Thumbnail, imageRect);
            g.DrawRectangle(Pens.Black, imageRect);

            // draw string
            Rectangle textRect = new Rectangle(
                imageRect.Right + 2
                , imageRect.Y
                , e.Bounds.Width - imageRect.Width - 4
                , itemHeight);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                _textBrush.Color = SystemColors.Highlight;
                g.FillRectangle(_textBrush, textRect);
                _textBrush.Color = SystemColors.HighlightText;
            }
            else
            {
                _textBrush.Color = SystemColors.Window;
                g.FillRectangle(_textBrush, textRect);
                _textBrush.Color = SystemColors.WindowText;
            }
            g.DrawString(
                solutions[e.Index].ToString()
                , e.Font
                , _textBrush
                , textRect);
        }
        #endregion
        #region ToolStrip / Menu
        private void ToolStripButtonPicGEOM_Click(object sender, EventArgs e)
        {   ExportAndOpenExtension("des");   }
        private void ToolStripButtonDXF_Click(object sender, EventArgs e)
        {   ExportAndOpenExtension("dxf");   }
        private void ToolStripButtonAI_Click(object sender, EventArgs e)
        {   ExportAndOpenExtension("ai");    }
        private void ToolStripButtonCFF2_Click(object sender, EventArgs e)
        {   ExportAndOpenExtension("cf2");   }
        private void ToolStripButtonPDF_Click(object sender, EventArgs e)
        {   ExportAndOpenExtension("pdf");   }
        private void ExportAndOpenExtension(string fileExt)
        {
            try
            {
                string applicationPath = string.Empty;
                switch (fileExt)
                {
                    case "des": applicationPath = Properties.Settings.Default.FileOutputAppDES; break;
                    case "dxf": applicationPath = Properties.Settings.Default.FileOutputAppDXF; break;
                    case "ai": applicationPath = Properties.Settings.Default.FileOutputAppAI; break;
                    case "cf2": applicationPath = Properties.Settings.Default.FileOutputAppCF2; break;
                    case "pdf": break;
                    default: break;
                }
                ExportAndOpen(fileExt, applicationPath);
            }
            catch (Win32Exception ex)
            {   MessageBox.Show(ex.Message, Application.ProductName); }
            catch (Exception ex)
            {   MessageBox.Show(ex.ToString()); }        
        }
        private void ExportAndOpen(string fileExt, string sPathExectable)
        {
            if (!string.IsNullOrEmpty(sPathExectable) && File.Exists(sPathExectable))
            {
                // build temp file path
                string tempFilePath = Path.ChangeExtension(Path.GetTempFileName(), fileExt);
                factoryViewer.WriteExportFile(tempFilePath, fileExt);
                // open using existing file path
                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = sPathExectable;
                    proc.StartInfo.Arguments = "\"" + tempFilePath + "\"";
                    proc.Start();
                }
            }
            else
            {
                string fileExportDir = Properties.Settings.Default.FileExportDirectory;
                SaveFileDialog fd = new SaveFileDialog
                {
                    DefaultExt = fileExt,
                    InitialDirectory = fileExportDir,
                    Filter = "PicGEOM (*.des)|*.des"
                            + "|Autodesk dxf (*.dxf)|*.dxf"
                            + "|Adobe Illustrator (*.ai)|*.ai"
                            + "|Common File Format (*.cf2)|*.cf2"
                            + "|Adobe Acrobat Reader (*.pdf)|*.pdf"
                            + "|All Files|*.*"
                };
                switch (fileExt)
                {
                    case "des": fd.FilterIndex = 1; break;
                    case "dxf": fd.FilterIndex  = 2; break;
                    case "ai": fd.FilterIndex   = 3; break;
                    case "cf2": fd.FilterIndex = 4; break;
                    case "pdf": fd.FilterIndex = 5; break;
                    default: break;
                }
                // shaow SaveFileDialog
                if (DialogResult.OK == fd.ShowDialog())
                    factoryViewer.WriteExportFile(fd.FileName, fileExt);
            }        
        }
        private void OnOceProCut(object sender, EventArgs e)
        {
            // initialize SaveFileDialog
            SaveFileDialog fd = new SaveFileDialog
            {
                FileName = Properties.Settings.Default.FileExportDirectory,
                Filter = "Adobe Illustrator (*.ai)|*.ai|All Files|*.*",
                FilterIndex = 0
            };
            // show save file dialog
            if (DialogResult.OK == fd.ShowDialog())
            {
                factoryViewer.WriteExportFile(fd.FileName, "des");
                Properties.Settings.Default.FileExportDirectory = Path.GetDirectoryName(fd.FileName);
            }
        }
        private void OnToggleCotations(object sender, EventArgs e)
        {
            factoryViewer.ToggleCotations();
        }
        private void OnExport(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            factoryViewer.ExportDialog(item.Text);
        }
        #endregion
        #region Handlers
        private void OnBnClickEditCardboardFormats(object sender, EventArgs e)
        {
            try
            {
                FormatLoader.EditCardboardFormats();
                FillCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OnAlignmentChanged(object sender, EventArgs e)
        {
            // cbTopBottom
            switch (cbTopBottom.SelectedIndex)
            {
                case 0: lbRemainingBottomTop.Text = Properties.Resources.ID_MINMARGINTOP; break;
                case 1: lbRemainingBottomTop.Text = Properties.Resources.ID_MINMARGINBOTTOM; break;
                default: break;
            }
            lbRemainingBottomTop.Visible = cbTopBottom.SelectedIndex != 2;
            nudTopBottomRemaining.Visible = cbTopBottom.SelectedIndex != 2;
            lbmm2.Visible = cbTopBottom.SelectedIndex != 2;

            // cbRightLeft
            switch (cbRightLeft.SelectedIndex)
            {
                case 0: lbRemainingLeftRight.Text = Properties.Resources.ID_MINMARGINRIGHT; break;
                case 1: lbRemainingLeftRight.Text = Properties.Resources.ID_MINMARGINLEFT; break;
                default: break;
            }
            lbRemainingLeftRight.Visible = cbRightLeft.SelectedIndex != 2;
            nudLeftRightRemaining.Visible = cbRightLeft.SelectedIndex != 2;
            lbmm4.Visible = cbRightLeft.SelectedIndex != 2;

            OnPropertyChanged(sender, e);
        }
        private void OnLayoutModeChanged(object sender, EventArgs e)
        {
            // controls first mode
            cbCardboardFormat.Enabled = rbCardboardFormat.Checked;
            bnFormats.Enabled = rbCardboardFormat.Checked;

            // controls second mode
            nudNumberX.Enabled = rbRowsColumns.Checked;
            nudNumberY.Enabled = rbRowsColumns.Checked;

            OnPropertyChanged(sender, e);
        }
        private void OnPropertyChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            timer.Stop();
            Compute();
        }
        #endregion
        #region Helpers
        private void FillCombo()
        {
            // fill combo
            cbCardboardFormat.Items.Clear();
            if (null == FormatLoader)
            {
                _log.Debug("Format loader is not set! Can not retrieve any cardboard formats!");
                return;
            }
            cbCardboardFormat.Items.AddRange(FormatLoader.LoadCardboardFormats());
            // select first item
            int selectedIndex = Math.Min(Properties.Settings.Default.ImpositionCarboardFormat, cbCardboardFormat.Items.Count - 1);
            if (cbCardboardFormat.Items.Count > selectedIndex)
                cbCardboardFormat.SelectedIndex = selectedIndex;
        }
        private void Compute()
        {
            // build imposition solutions
            impositionTool = ImpositionTool.Instantiate(Factory, ImpSettings);
            // -> margins
            impositionTool.SpaceBetween = ImpSpace;
            impositionTool.Margin = ImpMargin;
            impositionTool.MinMargin = ImpRemainingMargin;
            impositionTool.HorizontalAlignment = HorizontalAlignment;
            impositionTool.VerticalAlignment = VerticalAlignment;
            // -> patterns
            impositionTool.AllowRotationInColumns = AllowColumnRotation;
            impositionTool.AllowRotationInRows = AllowRowRotation;
            impositionTool.AllowBothDirection = AllowBothDirection;
            // -> offset
            impositionTool.ImpositionOffset = Offset;
            try
            {
                // instantiate ProgressWindow and launch process
                ProgressWindow progress = new ProgressWindow();
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateImpositionSolutions), progress);
                progress.ShowDialog();

                LoadSolutions();
            }
            catch (PicToolTooLongException /*ex*/)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.ID_ABANDONPROCESSING, PicToolArea.MaxNoSegments)
                    , Application.ProductName
                    , MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void GenerateImpositionSolutions(object status)
        {
            IProgressCallback callback = status as IProgressCallback;
            impositionTool.GenerateSortedSolutionList(callback, out solutions);
        }
        private void LoadSolutions()
        {
            // insert in listbox
            listBoxSolutions.Items.Clear();
            listBoxSolutions.Items.AddRange(solutions.ToArray());
            // select first item
            if (listBoxSolutions.Items.Count > 0)
                listBoxSolutions.SelectedIndex = 0;
        }
        #endregion
        #region Data members
        private List<ImpositionSolution> solutions;
        private ImpositionTool impositionTool;
        private static SolidBrush _textBrush = new SolidBrush(SystemColors.WindowText);
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FormImposition));
        #endregion
    }
}