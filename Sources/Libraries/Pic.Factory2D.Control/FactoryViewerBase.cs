#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using Sharp3D.Math.Core;
using log4net;
#endregion

namespace Pic.Factory2D.Control
{
    #region FactoryViewerBase
    public partial class FactoryViewerBase : UserControl, IToolInterface
    {
        #region Constructor
        public FactoryViewerBase()
        {
            // set double buffering
            MethodInfo mi = typeof(System.Windows.Forms.Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            mi.Invoke(this, args);

            InitializeComponent();
        }
        #endregion

        #region User control override (prevent background painting)
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode)  return;

            PicGraphics = new PicGraphicsControl(this)
            {
                BackgroundColor = Properties.Settings.Default.BackgroundColor
            };

            // mouse event handling
            MouseMove += new MouseEventHandler(OnMouseMove);
            MouseWheel += new MouseEventHandler(OnMouseWheel);
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);

            showCotationsToolStripMenuItem.Checked = _showCotationAuto;
            reflectionXToolStripMenuItem.Checked = _reflectionX;
            reflectionYToolStripMenuItem.Checked = _reflectionY;
        }
        #endregion

        #region Public properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PicFactory Factory { get; set; } = new PicFactory();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CardboardFormatLoader CardBoardFormatLoader { set; get; }

        #endregion

        #region IToolInterface
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Box2D BoundingBox
        {
            get { return PicGraphics.DrawingBox; }
        }
        public bool ReflectionX
        {
            get { return _reflectionX; }
            set
            {
                _reflectionX = value;
                reflectionXToolStripMenuItem.Checked = _reflectionX;
                // transform entities
                Factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                // add quotations
                PicAutoQuotation.BuildQuotation(Factory);
                FitView();
            }
        }
        public bool ReflectionY
        {
            get { return _reflectionY; }
            set
            {
                _reflectionY = value;
                reflectionYToolStripMenuItem.Checked = _reflectionY;
                // transform entities
                Factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
                // add quotations
                PicAutoQuotation.BuildQuotation(Factory);
                FitView();
            }
        }
        public bool ShowCotationsAuto
        {
            get { return _showCotationAuto; }
            set
            {
                _showCotationAuto = value;
                showCotationsToolStripMenuItem.Checked = _showCotationAuto;
                Invalidate();
            }
        }
        public bool ShowCotationsCode
        {
            get { return _showCotationsCode;    }
            set { _showCotationsCode = value; Invalidate(); }
        }
        public bool ShowAxes
        {
            get { return _showAxes; }
            set { _showAxes = value; Invalidate(); }
        }
        public bool IsPluginViewer => false;
        public bool HasDependancies => false;
        public bool IsSupportingAutomaticFolding => false;
        #endregion

        #region Private properties
        private Point MousePositionPrev { get; set; }
        #endregion

        #region Public methods
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        public void FitView()
        {
            if (null == PicGraphics || null == Factory || DesignMode) return;
            try
            {
                // compute bounding box
                Box2D box = Tools.BoundingBox(Factory, 0.01);
                if (!box.IsValid) return;
                // update PicGraphics drawing box
                PicGraphics.DrawingBox = box;
                // force redraw
                Invalidate();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        public bool Export(string filePath)
        {
            return WriteExportFile(filePath, Path.GetExtension(filePath));
        }
        public bool WriteExportFile(string filePath, string fileExt)
        {
            try
            {
                // instantiate filter
                PicFilter filter = (_showCotationAuto ? PicFilter.FilterNone : !PicFilter.FilterCotation) & PicFilter.FilterNoZeroEntities;
                // get bounding box
                PicVisitorBoundingBox visitorBoundingBox = new PicVisitorBoundingBox();
                Factory.ProcessVisitor(visitorBoundingBox, filter);
                Box2D box = visitorBoundingBox.Box;
                // add margins : 5 % of the smallest value among height 
                box.AddMarginHorizontal(box.Width * 0.01);
                box.AddMarginVertical(box.Height * 0.01);

                string fileFormat = fileExt.TrimStart('.').ToLower();

                byte[] byteArray;
                // get file content
                if (string.Equals("des", fileFormat))
                {   // DES
                    using (PicVisitorDesOutput visitor = new PicVisitorDesOutput())
                    {
                        visitor.Author = "treeDiM";
                        Factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
                else if (string.Equals("dxf", fileFormat))
                {   // DXF
                    using (PicVisitorDxfOutput visitor = new PicVisitorDxfOutput())
                    {
                        visitor.Author = "treeDiM";
                        Factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
#if PDFSharp
                else if (string.Equals("pdf", fileFormat))
                {   // PDF
                    using (PicGraphicsPdf graphics = new PicGraphicsPdf(box))
                    {
                        graphics.Author = Application.ProductName + "(" + Application.CompanyName + ")";
                        graphics.Title = Path.GetFileNameWithoutExtension(filePath);
                        Factory.Draw(graphics, filter);
                        byteArray = graphics.GetResultByteArray();
                    }
                }
#endif
                else if (string.Equals("ai", fileFormat) || string.Equals("cf2", fileFormat))
                { // AI || CF2
                    using (PicVisitorDiecutOutput visitor = new PicVisitorDiecutOutput(fileExt))
                    {
                        visitor.Author = Application.ProductName + "(" + Application.CompanyName + ")";
                        visitor.Title = Path.GetFileNameWithoutExtension(filePath);
                        Factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
                else
                    throw new Exception("Invalid file format:" + fileFormat);
                // write byte array to stream
                using (FileStream fstream = new FileStream(filePath, FileMode.Create))
                    fstream.Write(byteArray, 0, byteArray.GetLength(0));

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception: {0}", ex.ToString()));
                return false;
            }
        }
        public void ShowAboutBox()
        {
            AboutBox form = new AboutBox();
            if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void BuildLayout()
        {
            if (_reflectionX) Factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (_reflectionY) Factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));

            var form = new FormImposition()
            {
                FormatLoader = CardBoardFormatLoader,
                Factory = Factory
            };
            if (DialogResult.OK == form.ShowDialog()) { }
        }
#endregion

#region Public properties
        public bool ShowNestingMenu
        {
            get
            {
                if (null != impositionToolStripMenuItem)
                    return impositionToolStripMenuItem.Visible;
                else
                    return false;
            }
            set
            {
                if (null != impositionToolStripMenuItem)
                {
                    toolStripSeparator3.Visible = value;
                    impositionToolStripMenuItem.Visible = value;
                }
            }
        }
        public bool ShowAboutMenu
        {
            get
            {
                if (null != aboutToolStripMenuItem)
                    return aboutToolStripMenuItem.Visible;
                else
                    return false;
            }
            set
            {
                if (null != aboutToolStripMenuItem)
                {
                    toolStripSeparator4.Visible = value;
                    aboutToolStripMenuItem.Visible = value;
                }
            }        
        }

        internal PicGraphicsControl PicGraphics { get; set; }
#endregion

#region Mouse event handlers
        void OnMouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Box2D box = new Box2D(PicGraphics.DrawingBox);
                box.Zoom(-e.Delta / 1000.0);
                PicGraphics.DrawingBox = box;
            }
            catch (InvalidBoxException ex)  { _log.Error(ex.ToString()); }
            catch (Exception ex)            { _log.Error(ex.ToString()); }
            finally                         { Invalidate(); }
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point ptDiff = new Point(e.Location.X - MousePositionPrev.X, e.Location.Y - MousePositionPrev.Y);
                    Box2D box = PicGraphics.DrawingBox;
                    Vector2D centerScreen = new Vector2D(
                        box.Center.X - ptDiff.X * box.Width / ClientSize.Width
                        , box.Center.Y + ptDiff.Y * box.Height / ClientSize.Height);
                    box.Center = centerScreen;
                    PicGraphics.DrawingBox = box;

                    Invalidate();
                }
                MousePositionPrev = e.Location;
            }
            catch (InvalidBoxException ex) { _log.Error(ex.ToString()); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
            finally {}
        }
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip.Show(this, e.Location);
            else if (e.Button == MouseButtons.Left)
                Cursor.Current = new Cursor(new MemoryStream(Properties.Resources.CursorPan));
        }
        void OnMouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
#endregion

#region Context menu
        private void OnShowCotations(object sender, EventArgs e)
        {
            ShowCotationsAuto = !_showCotationAuto;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = _showCotationAuto;
        }
        private void OnReflectionX(object sender, EventArgs e)
        {
            try
            {
                ReflectionX = !_reflectionX;
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                item.Checked = _reflectionX;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnReflectionY(object sender, EventArgs e)
        {
            try
            {
                ReflectionY = !_reflectionY;
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                item.Checked = _reflectionY;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnExport(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                switch (item.Text)
                {
                    case "DES": saveFileDialog.Filter = "Picador file|*.des|All Files|*.*"; break;
                    case "DXF": saveFileDialog.Filter = "Autocad dxf|*.dxf|All Files|*.*"; break;
                    case "PDF": saveFileDialog.Filter = "Adobe Acrobat Reader (*.pdf)|*.pdf|All Files|*.*"; break;
                    case "AI": saveFileDialog.Filter = "Adobe Illustrator|*.ai|All Files|*.*"; break;
                    case "CF2":saveFileDialog.Filter = "Common File Format 2|*.cf2|All Files|*.*"; break;
                    default: break;

                }
                if (DialogResult.OK == saveFileDialog.ShowDialog())
                    Export(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception : {0}", ex.ToString()));
            }
        }

        private void OnFitView(object sender, EventArgs e) { FitView(); }
        private void OnBuildLayout(object sender, EventArgs e) { BuildLayout(); }
        private void OnShowAboutBox(object sender, EventArgs e) { ShowAboutBox(); }
#endregion

#region User control override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode || null == PicGraphics)
            {
                e.Graphics.Clear(Color.White);
                return;
            }
            PicGraphics.GdiGraphics = e.Graphics;
            if (null != Factory)
                Factory.Draw(PicGraphics, _showCotationAuto ? PicFilter.FilterNone : !PicFilter.FilterCotation);
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
#endregion

#region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FactoryViewer));
        private bool _reflectionX = false, _reflectionY = false;
        private bool _showCotationAuto = false, _showCotationsCode = false, _showAxes = false;
#endregion
    }
#endregion
}
