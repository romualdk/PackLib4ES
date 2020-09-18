#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using System.IO;
#endregion

namespace Pic.Factory2D.Control
{
    public partial class FormLayoutViewer : Form, IEntitySupplier
    {
        #region Data members
        private List<ImpositionSolution> solutions;
        #endregion
        #region Constructor
        public FormLayoutViewer()
        {
            InitializeComponent();
            listBoxSolutions.SelectedValueChanged += new EventHandler(OnSelectedValueChangedSolutions);

            factoryViewer.ReflectionX = false;
            factoryViewer.ReflectionY = false;
            factoryViewer.ShowCotations = false;

            // hide nesting and about menu
            factoryViewer.ShowNestingMenu = false;
            factoryViewer.ShowAboutMenu = false;
 
        }
        #endregion
        #region IEntitySummplier implementation
        public void CreateEntities(PicFactory factory)
        {
            if (!(listBoxSolutions.SelectedItem is ImpositionSolution solution)) return;
            solution.CreateEntities(factory);
            factory.InsertCardboardFormat(solution.CardboardPosition, solution.CardboardDimensions);
        }
        #endregion
        #region Public properties
        public List<ImpositionSolution> Solutions
        {
            set
            {
                solutions = value;
                // insert in listbox
                listBoxSolutions.Items.Clear();
                listBoxSolutions.Items.AddRange(solutions.ToArray());
            }
        }
        public CardboardFormat Format { get; set; }
        public string DrawingName { get; set; } = string.Empty;
        public int Result { get; private set; } = 0;
        public string OutputFilePath { get; private set; } = string.Empty;
        public static SolidBrush TextBrush { get; set; } = new SolidBrush(SystemColors.WindowText);
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // initialize list solution
            if (listBoxSolutions.Items.Count > 0)
                listBoxSolutions.SelectedIndex = 0;
            // refresh
            factoryViewer.Refresh();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        #endregion
        #region Event handlers
        /// <summary>
        /// listBoxSolution: selected item changed
        /// </summary>
        void OnSelectedValueChangedSolutions(object sender, EventArgs e)
        {
            factoryViewer.Refresh();

            if (!(listBoxSolutions.SelectedItem is ImpositionSolution solution))
                return;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrawItemListBoxSolution(object sender, DrawItemEventArgs e)
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
                TextBrush.Color = SystemColors.Highlight;
                g.FillRectangle(TextBrush, textRect);
                TextBrush.Color = SystemColors.HighlightText;
            }
            else
            {
                TextBrush.Color = SystemColors.Window;
                g.FillRectangle(TextBrush, textRect);
                TextBrush.Color = SystemColors.WindowText;
            }
            g.DrawString(
                solutions[e.Index].ToString()
                , e.Font
                , TextBrush
                , textRect);
        }
        #endregion
        #region Save/cancel buttons
        private void OnSaveAsNewFile(object sender, EventArgs e)
        {
            saveFileDialog.FileName = string.Format("{0}_layout.des", DrawingName);
            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                Result = 1;
                OutputFilePath = saveFileDialog.FileName;
                factoryViewer.WriteExportFile(OutputFilePath, "des");

                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void OnSaveToCurrentFile(object sender, EventArgs e)
        {
            Result = 2;
            OutputFilePath = Path.ChangeExtension(Path.GetTempFileName(), "des");
            factoryViewer.WriteExportFile(OutputFilePath, "des"); 

            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
        #region Paint event handlers
        /// <summary>
        /// Paint event handler for Cut length label
        /// </summary>
        private void OnPaintPbCut(object sender, PaintEventArgs e)
        {
            Size sz = pbCut.Size;
            e.Graphics.DrawLine(new Pen(Color.Red), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        /// <summary>
        /// Paint event handler for Fold length
        /// </summary>
        private void OnPaintPbFold(object sender, PaintEventArgs e)
        {
            Size sz = pbFold.Size;
            e.Graphics.DrawLine(new Pen(Color.Blue), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        #endregion
     }
}
