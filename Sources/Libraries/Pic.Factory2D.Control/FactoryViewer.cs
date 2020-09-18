#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

using Sharp3D.Math.Core;

using log4net;
#endregion

namespace Pic.Factory2D.Control
{
    #region FactoryViewer
    public partial class FactoryViewer : UserControl
    {
        #region Constructor
        public FactoryViewer()
        {
            // set double buffering
            MethodInfo mi = typeof(System.Windows.Forms.Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            mi.Invoke(this, args);

            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // do not process if design mode
            if (DesignMode)
                return;

            _picGraphics = new PicGraphicsControl(this);

            // mouse event handling
            MouseMove += new MouseEventHandler(OnMouseMove);
            MouseWheel += new MouseEventHandler(OnMouseWheel);
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);

            // event global cotation properties notification
            PicGlobalCotationProperties.Modified += new PicGlobalCotationProperties.OnGlobalCotationPropertiesModified(PicGlobalCotationProperties_Modified);

            showCotationsToolStripMenuItem.Checked = _showCotations;
            reflectionXToolStripMenuItem.Checked = _reflectionX;
            reflectionYToolStripMenuItem.Checked = _reflectionY;
        }
        void PicGlobalCotationProperties_Modified()
        {
            Refresh();
        }
        #endregion

        #region Mouse event handlers
        void OnMouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Box2D box = new Box2D(_picGraphics.DrawingBox);
                box.Zoom(-e.Delta / 1000.0);
                _picGraphics.DrawingBox = box;
            }
            catch (InvalidBoxException ex) { _log.Error(ex.ToString()); }
            catch (Exception ex)           { _log.Error(ex.ToString()); }
            finally                        { Invalidate(); }
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point ptDiff = new Point(e.Location.X - _mousePositionPrev.X, e.Location.Y - _mousePositionPrev.Y);
                    Box2D box = _picGraphics.DrawingBox;
                    Vector2D centerScreen = new Vector2D(
                        box.Center.X - ptDiff.X * box.Width / ClientSize.Width
                        , box.Center.Y + ptDiff.Y * box.Height / ClientSize.Height);
                    box.Center = centerScreen;
                    _picGraphics.DrawingBox = box;

                    Invalidate();
                }
                _mousePositionPrev = e.Location;
            }
            catch (InvalidBoxException ex)  { _log.Error(ex.ToString()); }
            catch (Exception ex)            { _log.Error(ex.ToString()); }
            finally                         { }
        }
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip.Show(this, e.Location);
            else if (e.Button == MouseButtons.Left)
                Cursor.Current = new Cursor(new System.IO.MemoryStream(Pic.Factory2D.Control.Properties.Resources.CursorPan));
        }
        void OnMouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region Event handlers
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!DesignMode && null != e && null != e.Graphics)
            {
                 _picGraphics.GdiGraphics = e.Graphics;
                _factory.Draw(_picGraphics, _showCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation);
            }

        }
        #endregion

        #region Public methods
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        public override void Refresh()
        {
			// sanity check
			if (null == Parent || null == _picGraphics)
				return;
            try
            {
                // clear factory
                _factory.Clear();
                // create entities
                if (null == EntitySupplier)
                {
                    if (Parent is IEntitySupplier)
                        EntitySupplier = Parent as IEntitySupplier;
                    else
                        _log.Debug(string.Format("Interface IEntitySupplier is not implemented by parent {0}", Parent.Name));
                }
                if (null != EntitySupplier)
                    EntitySupplier.CreateEntities(_factory);                    
                // apply transformations
                if (_reflectionX) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                if (_reflectionY) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            UpdateDrawingBox();
            base.Refresh();            
        }
        public void Refresh(Box2D box)
        {
            // sanity check
            if (null == Parent || null == _picGraphics)
                return;
            try
            {
                // clear factory
                _factory.Clear();
                // create entities
                if (null == EntitySupplier)
                {
                    if (Parent is IEntitySupplier)
                        EntitySupplier = Parent as IEntitySupplier;
                    else
                        _log.Debug(string.Format("Interface IEntitySupplier is not implemented by parent {0}", Parent.Name));
                }
                if (null != EntitySupplier)
                    EntitySupplier.CreateEntities(_factory);
                // apply transformations
                if (_reflectionX) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                if (_reflectionY) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            if (null != box)
                _picGraphics.DrawingBox = box;
            base.Refresh();
        }

        public void FitView()
        {
            if (DesignMode) return;
            UpdateDrawingBox();
            // force redraw
            Invalidate();
        }
        private void UpdateDrawingBox()
        {
            _picGraphics.DrawingBox = Tools.BoundingBox(_factory, MarginPerc); ;
        }

        public void ToggleCotations()
        {
            _showCotations = !_showCotations;
            Refresh();
            ShowCotationsToggled?.Invoke(this, new EventArgsStatus(_showCotations, _reflectionX, _reflectionY));
        }
        public bool Export(string filePath)
        {
            return WriteExportFile(filePath, System.IO.Path.GetExtension(filePath));
        }
        public bool WriteExportFile(string filePath, string fileExt)
        {
            try
            {
                // instantiate filter
                PicFilter filter = (_showCotations ? PicFilter.FilterNone : !PicFilter.FilterCotation) & PicFilter.FilterNoZeroEntities;
                // get bounding box
                PicVisitorBoundingBox visitorBoundingBox = new PicVisitorBoundingBox();
                _factory.ProcessVisitor(visitorBoundingBox, filter);
                Box2D box = visitorBoundingBox.Box;
                box.AddMarginRatio(MarginPerc);

                string author = Application.ProductName + " (" + Application.CompanyName + ")";
                string title = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string fileFormat = fileExt.TrimStart('.').ToLower();

                byte[] byteArray;
                // get file content
                if ("des" == fileFormat)
                {
                    using (PicVisitorDesOutput visitor = new PicVisitorDesOutput())
                    {
                        visitor.Author = author;
                        _factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
                else if ("dxf" == fileFormat)
                {
                    using (PicVisitorDxfOutput visitor = new PicVisitorDxfOutput())
                    {
                        visitor.Author = author;
                        _factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
#if PDFSharp
                else if ("pdf" == fileFormat)
                {
                    using (PicGraphicsPdf graphics = new PicGraphicsPdf(box))
                    {
                        graphics.Author = author;
                        graphics.Title = title;
                        _factory.Draw(graphics, filter);
                        byteArray = graphics.GetResultByteArray();
                    }
                }
#endif
                else if ("ai" == fileFormat || "cf2" == fileFormat)
                { 
                    using (PicVisitorDiecutOutput visitor = new PicVisitorDiecutOutput(fileExt))
                    {
                        visitor.Author = author;
                        visitor.Title = title;
                        _factory.ProcessVisitor(visitor, filter);
                        byteArray = visitor.GetResultByteArray();
                    }
                }
                else
                    throw new Exception("Invalid file format: " + fileFormat);
                // write byte array to stream
                using (System.IO.FileStream fstream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
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
            if (DialogResult.OK == form.ShowDialog()) {}
        }
        public void BuildLayout()
        {
            // build factory entities
            if (_reflectionX) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (_reflectionY) _factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));

            var form = new FormImposition()
            {
                FormatLoader = _cardboardFormatLoader,
                Factory = _factory
            };
            if (DialogResult.OK == form.ShowDialog()) { }
        }
        private void ComputeLengthAndArea(ref double unitLengthCut, ref double unitLengthFold, ref double unitArea)
        { 
            unitLengthCut = 0.0;
            unitLengthFold = 0.0;
            unitArea = 0.0;
        }
#endregion
        
#region User control override
        protected override void OnPaintBackground(PaintEventArgs e) { }
#endregion

#region Public properties
        public IEntitySupplier EntitySupplier { set; private get; }
        public Box2D BoundingBox
        {
            get { return _picGraphics.DrawingBox; }
        }
        public bool ReflectionX
        {
            get { return _reflectionX; }
            set
            {
                _reflectionX = value;
                reflectionXToolStripMenuItem.Checked = _reflectionX;
                Refresh();
                ReflectionXToggled?.Invoke(this, new EventArgsStatus(_showCotations, _reflectionX, _reflectionY));

            }
        }
        public bool ReflectionY
        {
            get { return _reflectionY; }
            set
            {
                _reflectionY = value;
                reflectionYToolStripMenuItem.Checked = _reflectionY;
                Refresh();
                ReflectionYToggled?.Invoke(this, new EventArgsStatus(_showCotations, _reflectionX, _reflectionY));
            }
        }
        public bool ShowCotations
        {
            get { return _showCotations; }
            set
            {
                _showCotations = value;
                showCotationsToolStripMenuItem.Checked = _showCotations;
                Refresh();
                ShowCotationsToggled?.Invoke(this, new EventArgsStatus(_showCotations, _reflectionX, _reflectionY));
            }
        }
        public CardboardFormatLoader CardBoardFormatLoader
        {
            set
            {
                _cardboardFormatLoader = value; 
            }
        }

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
#endregion
        
#region Context menu
        private void ShowCotationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCotations = !_showCotations;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = _showCotations;
        }

        private void ReflectionXToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ReflectionYToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void FitViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FitView();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            ExportDialog( item.Text );
        }

        public void ExportDialog(string fileExt)
        {
            // filter string
            if (string.IsNullOrEmpty(saveFileDialog.Filter))
                saveFileDialog.Filter = "Picador file (*.des)|*.des|" +
                                        "Autocad dxf (*.dxf)|*.dxf|" +
                                        "Adobe Illustrator (*.ai)|*.ai|" +
                                        "Common File Format2 (*.cf2)|*.cf2|" +
                                        "Adobe Acrobat Reader (*.pdf)|*.pdf|" +
                                        "All Files (*.*)|*.*";
            // filter index
            if (string.Equals("DES", fileExt))
                saveFileDialog.FilterIndex = 1;
            else if (string.Equals("DXF", fileExt, StringComparison.CurrentCultureIgnoreCase))
                saveFileDialog.FilterIndex = 2;
            else if (string.Equals("AI", fileExt, StringComparison.CurrentCultureIgnoreCase))
                saveFileDialog.FilterIndex = 3;
            else if (string.Equals("CF2", fileExt, StringComparison.CurrentCultureIgnoreCase))
                saveFileDialog.FilterIndex = 4;
            else if (string.Equals("PDF", fileExt, StringComparison.CurrentCultureIgnoreCase))
                saveFileDialog.FilterIndex = 5;
            // show dialog and export
            if (DialogResult.OK == saveFileDialog.ShowDialog())
                Export(saveFileDialog.FileName);           
        }

        private void ImpositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildLayout();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }
#endregion

#region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FactoryViewer));
        PicGraphicsControl _picGraphics;
        PicFactory _factory = new PicFactory();
        private Point _mousePositionPrev;
        private bool _reflectionX = false, _reflectionY = false, _showCotations = false;
        private CardboardFormatLoader _cardboardFormatLoader;

        public delegate void StatusChanged(object sender, EventArgsStatus e);
        public event StatusChanged ShowCotationsToggled;
        public event StatusChanged ReflectionXToggled;
        public event StatusChanged ReflectionYToggled;

        private readonly double MarginPerc = 0.01;
#endregion
    }
#endregion

#region EventArgsStatus class
    public class EventArgsStatus : EventArgs
    {
#region Constructors
        public EventArgsStatus(bool showCotations, bool reflectionX, bool reflectionY)
            :base()
        {
            ShowCotations = showCotations;
            ReflectionX = reflectionX;
            ReflectionY = reflectionY;
        }
#endregion

#region Public properties
        public bool ShowCotations { get; }
        public bool ReflectionX { get; }
        public bool ReflectionY { get; }

#endregion
    }
#endregion

#region PicGraphicsControl
    internal class PicGraphicsControl : PicGraphicsGdiPlus
    {
#region Contructor
        public PicGraphicsControl(System.Windows.Forms.Control parentControl)
        {
            _parentControl = parentControl;
            _parentControl.SizeChanged += new EventHandler(OnParentControlSizeChanged);
        }
#endregion

#region Handlers
        void OnParentControlSizeChanged(object sender, EventArgs e)
        {
            // force box recomputation
            if (Box.IsValid)
                DrawingBox = Box;
        }
#endregion

#region PicGraphicsGdiPlus override
        public override Size GetSize()
        {
            return _parentControl.Size;
        }
#endregion

#region Fields
        private System.Windows.Forms.Control _parentControl;
#endregion
    }
#endregion

#region  IEntitySupplier interface
    public interface IEntitySupplier
    {
        void CreateEntities(PicFactory factory);
    }
#endregion
}