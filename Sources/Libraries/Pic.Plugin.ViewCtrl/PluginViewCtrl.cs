#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

using Sharp3D.Math.Core;
using Pic.Factory2D;
using Pic.Factory2D.Control;

using log4net;
#endregion

namespace Pic.Plugin.ViewCtrl
{
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Docking(DockingBehavior.AutoDock)
    ]
    public partial class PluginViewCtrl : IToolInterface
    {
        #region Data members
        [NonSerialized]private Component _component;
        [NonSerialized]private IComponentSearchMethod _searchMethod;
        [NonSerialized]private ProfileLoader _profileLoader;
        [NonSerialized]private CardboardFormatLoader _cardboardFormatLoader;
        [NonSerialized]private PicGraphicsCtrlBased _picGraphics;
        private Point _mousePositionPrev;
        private bool _computeBbox = true;
        private bool _reflectionX = false, _reflectionY= false;
        private bool _buttonCloseVisible = false;
        private bool _buttonValidateVisible = false;
        private ComboBox _comboProfile;
        private bool _showCotationAuto = true, _showCotationCode = false, _showAxes = true;
        private Button btClose;
        private Button btValidate;
        private double _thickness = 0.0;
        [NonSerialized]private FactoryDataCtrl factoryDataCtrl;
        /// <summary>
        /// current parameter stack
        /// </summary>
        [NonSerialized]private ParameterStack _paramStackCurrent;
        [NonSerialized]private bool _dirtyParameters = true;
        [NonSerialized]private bool _hasDependancies = false;
        public delegate void DependancyStatusChangedHandler(Control pluginViewer, bool hasDependancy);
        public event DependancyStatusChangedHandler DependancyStatusChanged;
        // *** parameter animation ***
        protected ParameterStack tempParameterStack = null;
        protected string _parameterName = string.Empty;
        protected int _timerStep = 0;
        protected int _noStepsTimer = 10;
        protected double _parameterInitialValue = 0.0;
        // timer
        protected Timer timer = null;
        // *** parameter animation ***
        // log4net
        protected static readonly ILog log = LogManager.GetLogger(typeof(PluginViewCtrl));
        #endregion

        #region Browsable events
        [Category("Configuration"), Browsable(true), Description("Event raised by user click on Close button")]
        public event EventHandler CloseClicked;
        [Category("Configuration"), Browsable(true), Description("Event raised by user click on Validate button")]
        public event EventHandler ValidateClicked;
        #endregion

        #region Constructor
        public PluginViewCtrl()
        {
            try
            {
                //must use reflection to set double buffering on the child panel controls  
                MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
                mi.Invoke(Panel1, args);
                mi.Invoke(Panel2, args);

                InitializeComponent();

                SplitterWidth = 1;

                if (Properties.Settings.Default.AllowParameterAnimation)
                {
                    // *** timer used for parameter animation
                    timer = new Timer
                    {
                        Interval = 100
                    };
                    timer.Tick += new EventHandler(OnTimerTick);
                    _noStepsTimer = Properties.Settings.Default.NumberOfAnimationSteps;
                }
                else
                {
                    timer = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        protected override void  InitLayout()
        {
 	         base.InitLayout();

            // define event handlers
            Panel1.Paint += new PaintEventHandler(OnPanel1Paint);
            // mouse event handling
            Panel1.MouseDown += new MouseEventHandler(OnMouseDown);
            Panel1.MouseMove += new MouseEventHandler(OnMouseMove);
            Panel1.MouseWheel += new MouseEventHandler(OnMouseWheel);

            _computeBbox = true;

            // event global cotation properties notification
            PicGlobalCotationProperties.Modified += new PicGlobalCotationProperties.OnGlobalCotationPropertiesModified(PicGlobalCotationProperties_Modified);
            // context menu
            showCotationsToolStripMenuItem.Checked = _showCotationAuto;
            reflectionXToolStripMenuItem.Checked = _reflectionX;
            reflectionYToolStripMenuItem.Checked = _reflectionY;
            // splitter
            SplitterWidth = 1;
        }
        void PicGlobalCotationProperties_Modified()
        {
            Refresh();
        }
        #endregion

        #region Mouse event handlers
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Box2D box = new Box2D(_picGraphics.DrawingBox);
                box.Zoom(-e.Delta / 1000.0);
                _picGraphics.DrawingBox = box;
            }
            catch (InvalidBoxException /*ex*/)
            {
                _computeBbox = true;
            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
            }
            finally
            {
                Panel1.Invalidate();
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Panel1.Focus();
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point ptDiff = new Point(e.Location.X - _mousePositionPrev.X, e.Location.Y - _mousePositionPrev.Y);
                    double aspectRatio = 1.0;

                    Box2D box = _picGraphics.DrawingBox;

                    Vector2D centerScreen = new Vector2D(
                        box.Center.X - (double)ptDiff.X * box.Width / (double)Panel1.ClientSize.Width
                        , box.Center.Y + (double)ptDiff.Y * box.Height / ((double)Panel1.ClientSize.Height * aspectRatio));
                    box.Center = centerScreen;
                    _picGraphics.DrawingBox = box;

                    Panel1.Invalidate();
                }
                _mousePositionPrev = e.Location;
            }
            catch (InvalidBoxException /*ex*/)
            {
                _computeBbox = true;
            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
            }
            finally
            {
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Cursor.Current = new Cursor(new MemoryStream(Properties.Resource.CursorPan));
        }

        void Panel2_MouseEnter(object sender, EventArgs e)
        {
            if (Panel1.Focused)
                Panel2.Focus();
        }
        #endregion

        #region Public methods
        public void BuildLayout()
        {
            using (PicFactory factory = new PicFactory())
            {
                // build factory entities
                Component.CreateFactoryEntities(factory, CurrentParameterStack);
                if (ReflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                if (ReflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
                var form = new FormImposition()
                {
                    FormatLoader = _cardboardFormatLoader,
                    Factory = factory,
                    Offset = Component.ImpositionOffset(CurrentParameterStack)
                };
                if (DialogResult.OK == form.ShowDialog()) {}
            }
        }
        #endregion

        #region Context menu
        private void OnShowCotations(object sender, EventArgs e)
        {
            ShowCotationsAuto = !_showCotationAuto;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = _showCotationAuto;
        }
        private void ReflectionXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReflectionX = !_reflectionX;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = _reflectionX;
        }
        private void ReflectionYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReflectionY = !_reflectionY;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = _reflectionY;
        }
        private void OnFitViewToolStripMenuItemClick(object sender, EventArgs e) => Redraw(true);
        private void ImpositionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (Component == null) return;
            BuildLayout();
        }
        #endregion

        #region Setting loaders
        public IComponentSearchMethod SearchMethod { set { _searchMethod = value; } }
        public ProfileLoader ProfileLoader
        {
            set
            {
                _profileLoader = value;
                if (null != _profileLoader)
                    _profileLoader.MajorationModified += new ProfileLoader.MajorationModifiedHandler(_profileLoader_MajorationModified);
            }
        }
        void _profileLoader_MajorationModified(object sender)
        {
            Refresh();
        }
        public CardboardFormatLoader CardboardFormatLoader
        {
            set
            {
                _cardboardFormatLoader = value;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// set/get the current component
        /// </summary>
        [Browsable(false)]
        [Category("PluginViewCtrl")]
        [Description("Access component object")]
        [DisplayName("Component")]
        public Component Component
        {
            get
            {
                if (null != _component)
                    _component.CompUnitSystem = UnitSystem.Instance.USyst;
                 return _component;
            }
            set
            {
                // recomputation of parameter list
                ClearParameterStack();
                // set dependancies
                HasDependancies = false;

                _component = value;
                if (null != _component)
                    _component.CompUnitSystem = UnitSystem.Instance.USyst;

                // update panel2 controls
                Panel2.AutoScroll = true;
                CreatePluginControls();
                Redraw(true);
            }
        }

        /// <summary>
        ///  set the plugin path
        /// </summary>
        [Browsable(true)]
        [Category("PluginViewCtrl")]
        [Description("Sets the plugin path")]
        [DisplayName("PluginPath")]
        [DefaultValue("")]
        public string PluginPath
        {
            set
            {
                // recomputation of parameter list
                ClearParameterStack();
                // set dependancies
                HasDependancies = false;

                // load component
                using (ComponentLoader loader = new ComponentLoader())
                {
                    // load plugin -> will throw exception if plugin file cannot be found
                    if (null != _searchMethod)
                        loader.SearchMethod = _searchMethod;
                    _component = loader.LoadComponent(value);
                }
                // set component in profile loader
                if (null != _profileLoader)
                    _profileLoader.SetComponent(_component);

                // update panel2 controls
                Panel2.AutoScroll = true;
                CreatePluginControls();
                // fit view
                Redraw(true);
            }
            get { return ""; }  // is needed to have "PluginPath" appear as a property of the control
        }
        public bool ShowSummary { get; set; } = false;
        public bool ValidateButtonVisible
        {
            get { return _buttonValidateVisible; }
            set
            {
                _buttonValidateVisible = value;
                if (null != btValidate) btValidate.Visible = value;
                if (_buttonValidateVisible) CloseButtonVisible = true;
            }
        }
        public bool CloseButtonVisible
        {
            get { return _buttonCloseVisible; }
            set
            {
                _buttonCloseVisible = value;
                if (null != btClose) btClose.Visible = value;
            }
        }
        public Dictionary<string, Parameter> ParamValues
        {
            get
            {
                if (DesignMode || null == Component) return null;
                // copy parameters to Dictionary<string, Parameter>
                Dictionary<string, Parameter> dict = new Dictionary<string, Parameter>();
                foreach (Parameter param in CurrentParameterStack)
                {
                    if (param is ParameterDouble || param is ParameterBool || param is ParameterInt)
                        dict.Add(param.Name, param);
                }
                return dict;
            }
            set
            {
                foreach (Control ctrl in Panel2.Controls)
                {
                    if (ctrl is NumericUpDown nud && nud.Name.Contains("nud")
                        && value.ContainsKey(nud.Name.Substring(3)))
                    {
                        Parameter param = value[nud.Name.Substring(3)];
                        if (param is ParameterDouble paramDouble) nud.Value = (decimal)paramDouble.Value;
                        if (param is ParameterInt paramInt) nud.Value = paramInt.Value;
                    }
                    if (ctrl is CheckBox chkb && chkb.Name.Contains("chkb")
                        && value.ContainsKey(chkb.Name.Substring(4)))
                    {
                        Parameter param = value[chkb.Name.Substring(4)];
                        if (param is ParameterBool paramBool) chkb.Checked = paramBool.Value;
                    }
                }
                SetParametersDirty();
                Panel1.Invalidate();
            }
        }
        /// <summary>
        /// Loaded component name
        /// </summary>
        public string LoadedComponentName => _component.Name;
        #endregion

        #region IToolInterface implementation
        /// <summary>
        /// reflection X
        /// </summary>
        public bool ReflectionX
        {
            get { return _reflectionX; }
            set
            {
                if (DesignMode)
                    return;
                try
                {
                    _reflectionX = value;
                    _computeBbox = true;
                    Panel1.Invalidate();
                }
                catch (Exception /*ex*/)
                {
                }
            }
        }
        /// <summary>
        /// reflection Y
        /// </summary>
        public bool ReflectionY
        {
            get { return _reflectionY; }
            set
            {
                if (DesignMode)
                    return;
                try
                {
                    _reflectionY = value;
                    _computeBbox = true;
                    Panel1.Invalidate();
                }
                catch (Exception /*ex*/)
                {
                }
            }
        }
        /// <summary>
        /// show cotation auto
        /// </summary>
        public bool ShowCotationsAuto
        {
            get { return _showCotationAuto; }
            set
            {
                if (DesignMode)
                    return;
                try
                {
                    _showCotationAuto = value;
                    _computeBbox = true;
                    Panel1.Invalidate();
                }
                catch (Exception /*ex*/)
                {
                }
            }
        }
        /// <summary>
        /// show cotation code
        /// </summary>
        public bool ShowCotationsCode
        {
            get { return _showCotationCode; }
            set
            {
                if (DesignMode)
                    return;
                try
                {
                    _showCotationCode = value;
                    _computeBbox = true;
                    Panel1.Invalidate();
                }
                catch (Exception /*ex*/)
                { 
                }
            }
        }
        /// <summary>
        /// Show axes
        /// </summary>
        public bool ShowAxes
        {
            get { return _showAxes; }
            set
            {
                if (DesignMode)
                    return;
                try
                {
                    _showAxes = value;
                    _computeBbox = true;
                    Panel1.Invalidate();
                }
                catch (Exception /*ex*/)
                { 
                }
                SetParametersDirty();
                Panel1.Invalidate();
            }
        }
        /// <summary>
        /// Accessing entities bounding box
        /// </summary>
        public Box2D BoundingBox
        {
            get
            {
                Box2D box;
                using (PicFactory factory = new PicFactory())
                {
                    // generate entities
                    if (Component != null)
                    {
                        Component.CreateFactoryEntities(factory, CurrentParameterStack);
                        if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                        if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
                    }
                    // get bounding box
                    PicVisitorBoundingBox visitor = new PicVisitorBoundingBox();
                    factory.ProcessVisitor(visitor);
                    box = visitor.Box;
                    box.AddMarginRatio(0.05);
                }
                return box;
            }
        }

        public bool IsPluginViewer => true;
        public bool IsSupportingAutomaticFolding => Component.IsSupportingAutomaticFolding;
        #endregion

        #region FitView method
        private void Redraw(bool computeBbox)
        {
            _computeBbox = computeBbox;
            Panel1.Invalidate();
        }
        public void FitView() => Redraw(true);
        #endregion

        #region Painting
        /// <summary>
        /// Paint handler
        /// </summary>
        /// <param name="obj">Sender</param>
        /// <param name="e">Argument</param>
        protected void OnPanel1Paint(object obj, PaintEventArgs e)
        {
            try
            {
                //if (null == Component || DesignMode)
                //    return;
                RePaint(e, tempParameterStack ?? CurrentParameterStack);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void RePaint(PaintEventArgs e, ParameterStack stack)
        { 
            try
            {
                // instantiate PicGraphics
                if (_picGraphics == null)
                {
                    _picGraphics = new PicGraphicsCtrlBased(this.Panel1.ClientSize, e.Graphics)
                    { BackgroundColor = Factory2D.Control.Properties.Settings.Default.BackgroundColor };
                }
                _picGraphics.GdiGraphics = e.Graphics;
                
                _picGraphics.Size = Panel1.ClientSize;

                // build factory
                using (PicFactory factory = new PicFactory())
                {
                    // create entities
                    Component.CreateFactoryEntities(factory, stack);
                    if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                    if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
                    // remove existing quotations
                    if (!_showCotationCode)
                        factory.Remove( PicFilter.FilterCotation );
                    // build auto quotations
                    if (_showCotationAuto)
                        PicAutoQuotation.BuildQuotation(factory);

                    // filter 
                    PicFilter filter = _showAxes ? PicFilter.FilterNone
                        : PicFilter.FilterCotation | !(new PicFilterLineType(PicGraphics.LT.LT_COTATION) | new PicFilterLineType(PicGraphics.LT.LT_AXIS));

                    // update drawing box?
                    if (_computeBbox)
                    {
                        PicVisitorBoundingBox visitor = new PicVisitorBoundingBox();
                        factory.ProcessVisitor(visitor, filter);
                        Box2D box = visitor.Box;
                        box.AddMarginRatio(0.05);
                        _picGraphics.DrawingBox = box;

                        // update factory data
                        if (null != factoryDataCtrl)
                            factoryDataCtrl.Factory = factory;

                        _computeBbox = false;
                    }
                    // draw
                    factory.Draw(_picGraphics, filter);
                }
            }
            catch (PluginException ex)
            {
                // might happen
                _picGraphics.ShowMessage(ex.Message);
            }
            catch (ComponentSearchException ex)
            {
                _picGraphics.ShowMessage(ex.Message);
            }
            catch (Exception ex)
            {
                _picGraphics.ShowMessage(ex.ToString());
                log.Error(ex.ToString());
            }        
        }

        /// <summary>
        /// OnPaintBackground is overriden to be disabled for double buffering
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (null != factoryDataCtrl)
            {
                int controlHeight = 150;
                int ctrlX = 5;
                factoryDataCtrl.Location = new Point(ctrlX, Panel2.Height - (controlHeight + 20));
            }
        }
        #endregion

        #region File export
        public static List<FileFormat> GetExportFormatList()
        {
            List<FileFormat> list = new List<FileFormat>
            {
                new FileFormat("treeDim Picador", "des", "Picador (*.des)|*.des", ""),
                new FileFormat("AutoCAD dxf", "dxf", "AutoCAD (*.dxf)|*.dxf", ""),
                new FileFormat("Adobe Acrobat", "pdf", "Adobe Acrobat (*.pdf)|*.pdf", ""),
                new FileFormat("Adobe Illustrator", "ai", "Adobe Illustrator (*.ai)|*.ai", ""),
                new FileFormat("Common File Format 2", "cf2", "Common File Format 2 (*.cf2)|*.cf2", "")
            };
            return list;
        }

        public void WriteExportFile(string filePath, string fileExt)
        {
            // get export byte array
            byte[] byteArray = GetExportFile(fileExt, CurrentParameterStack);
            // write byte array to stream
            using (FileStream fstream = new FileStream(filePath, FileMode.Create))
                fstream.Write(byteArray, 0, byteArray.GetLength(0));
        }

        public PicFactory GetFactory()
        {
            PicFactory factory = new PicFactory();
            Component.CreateFactoryEntities(factory, CurrentParameterStack);
            if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            return factory;
        }

        public byte[] GetExportFile(string fileExt, Pic.Plugin.ParameterStack stack)
        {
            // build factory
            PicFactory factory = new PicFactory();
            Component.CreateFactoryEntities(factory, stack);
            if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            // remove existing quotations
            if (!_showCotationCode)
                factory.Remove(PicFilter.FilterCotation);
            // build auto quotations
            if (_showCotationAuto)
                PicAutoQuotation.BuildQuotation(factory);

            // instantiate filter
            PicFilter filter = _showAxes ? PicFilter.FilterNone
                : PicFilter.FilterCotation | !(new PicFilterLineType(PicGraphics.LT.LT_COTATION) | new PicFilterLineType(PicGraphics.LT.LT_AXIS) );
            filter &= PicFilter.FilterNoZeroEntities;

            // get bounding box
            PicVisitorBoundingBox visitorBoundingBox = new PicVisitorBoundingBox();
            factory.ProcessVisitor(visitorBoundingBox, filter);
            Box2D box = visitorBoundingBox.Box;
            // add margins : 5 % of the smallest value among height 
            box.AddMarginHorizontal(box.Width * 0.05);
            box.AddMarginVertical(box.Height * 0.05);

            string author = Application.ProductName + " (" + Application.CompanyName + ")";
            string title = Application.ProductName + " export";
            string fileFormat = fileExt.StartsWith(".") ? fileExt.Substring(1) : fileExt;
            fileFormat = fileFormat.ToLower();

            // get file content
            if (string.Equals("des", fileFormat))
            {
                PicVisitorDesOutput visitor = new PicVisitorDesOutput { Author = author };
                factory.ProcessVisitor(visitor, filter);
                return visitor.GetResultByteArray();
            }
            else if (string.Equals("dxf", fileFormat))
            {
                PicVisitorOutput visitor = new PicVisitorDxfOutput { Author = author };
                factory.ProcessVisitor(visitor, filter);
                return visitor.GetResultByteArray();
            }
#if PDFSharp
            else if (string.Equals("pdf", fileFormat))
            {
                PicGraphicsPdf graphics = new PicGraphicsPdf(box)
                {
                    Author = author,
                    Title = title
                };
                factory.Draw(graphics, filter);
                return graphics.GetResultByteArray();
            }
#endif
            else if (string.Equals("ai", fileFormat) || string.Equals("cf2", fileFormat))
            {
                PicVisitorDiecutOutput visitor = new PicVisitorDiecutOutput(fileFormat)
                {
                    Author = author,
                    Title = title
                };
                factory.ProcessVisitor(visitor, filter);
                return visitor.GetResultByteArray();
            }
            else
                throw new Exception("Invalid file format:" + fileFormat);
        }

        public bool GetReferencePointAndThickness(ref Vector2D v, ref double thickness)
        {
            if (!Component.IsSupportingAutomaticFolding)
                return false;
            ParameterStack stack = CurrentParameterStack;
            thickness = stack.GetDoubleParameterValue("th1");
            v = Component.ReferencePoint(stack);
            return true;
        }
#endregion

#region Plugin parameter controls
        /// <summary>
        /// Instantiate all parameters
        /// </summary>
        protected void CreatePluginControls()
        {
            Component component = Component;
            if (component == null)
                return;

            const int lblX = 10, nudX = 160, incY = 26;
            Size lblSize = new Size(150, 18);
            Size lblValueSize = new Size(70, 12);
            Size tbSize = new Size(40, 12);
            Size chkbSize = new Size(150, 18);
            Size nudSize = new Size(70, 12);
            Size lblComboSize = new Size(140, 12);
            Size cbSize = new Size(140, 22);
            int posY = 10;
            int tabIndex = 0;

            ParameterStack stack = CurrentParameterStack;

            // clear controls
            Panel2.Controls.Clear();

            // check if plugin has some majorations
            if (stack.HasMajorations)
            {
                if (null != _profileLoader)
                {
                    Size btSize = new Size(50, 22);

                    // lbl 
                    Panel2.Controls.Add(
                        new Label
                        {
                            Text = Properties.Resource.STR_PROFILE,
                            Location = new Point(lblX, posY),
                            Size = new Size(nudX - cbSize.Width + nudSize.Width - lblX, lblSize.Height),
                            TabIndex = ++tabIndex
                        }
                    );
                    // create combo
                    _comboProfile = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Location = new Point(nudX - cbSize.Width + nudSize.Width, posY),
                        Size = cbSize,
                        TabIndex = ++tabIndex
                    };
                    _comboProfile.Items.AddRange(_profileLoader.Profiles);
                    _comboProfile.SelectedItem = _profileLoader.Selected;
                    _comboProfile.SelectedIndexChanged += new EventHandler(OnProfileSelected);
                    Panel2.Controls.Add(_comboProfile);
                    // increment Y
                    posY += incY;
                    // create edit majorations button
                    Button bt = new Button
                    {
                        Text = Properties.Resource.STR_TOL,
                        Location = new Point(nudX, posY),
                        Size = new Size(nudSize.Width, cbSize.Height)
                    };
                    bt.Click += new EventHandler(OnBnEditMajorations);
                    Panel2.Controls.Add(bt);

                    // increment Y
                    posY += incY;
                }
                else
                    log.Info("Plugin has majorations but no profile loader implementation is available!\nMajoration handled as default parameters");
            }
            else // no majorations
            {
                _comboProfile = null;
				if (stack.HasParameter("th1"))
                	_thickness = stack.GetDoubleParameterValue("th1");

                // lbl
                Label lbl = new Label
                {
                    Text = Properties.Resource.STR_THICKNESS,
                    Location = new Point(lblX, posY),
                    Size = new Size(nudX - cbSize.Width + nudSize.Width - lblX, lblSize.Height),
                    TabIndex = ++tabIndex
                };
                Panel2.Controls.Add(lbl);
                // create edit field
                NumericUpDown nudThickness = new NumericUpDown
                {
                    Name = "nudThickness",
                    Minimum = 0.0M,
                    Maximum = 10000.0M,
                    DecimalPlaces = 2,
                    ThousandsSeparator = false,
                    Value = Convert.ToDecimal(_thickness),
                    Location = new Point(nudX, posY),
                    Size = nudSize,
                    UpDownAlign = LeftRightAlignment.Right
                };
                nudThickness.ValueChanged += new EventHandler(ParameterChanged);
                nudThickness.LostFocus += new EventHandler(OnNudLostFocus);
                nudThickness.GotFocus += new EventHandler(OnNudGotFocus);
                nudThickness.Click += new EventHandler(OnNudGotFocus);
                nudThickness.TabIndex = ++tabIndex;
                Panel2.Controls.Add(nudThickness);

                // increment Y
                posY += incY;
            }

            // create unknown parameters
            foreach (Parameter param in CurrentParameterStack.ParameterList)
            {
                // do not create default parameters
                if (_profileLoader != null && _profileLoader.HasParameter(param)
                    || param.IsMajoration)
                    continue;
                if (param is ParameterAngle paramAngle)
                {
                    Panel2.Controls.Add(
                        new Label()
                        { 
                            Text = Translate(paramAngle.Description) + " (" + paramAngle.Name + ")",
                            Location = new Point(lblX + param.IndentValue, posY),
                            Size = lblSize,
                            TabIndex = ++tabIndex                        
                        }
                        );
                    NumericUpDown nud = new NumericUpDown
                    {
                        Name = "nudAngle" + paramAngle.Name,
                        Minimum = (decimal)paramAngle.ValueMin,
                        Maximum = (decimal)paramAngle.ValueMax,
                        DecimalPlaces = 2,
                        ThousandsSeparator = false,
                        Value = (decimal)paramAngle.Value,
                        Location = new Point(nudX + param.IndentValue, posY),
                        Size = nudSize,
                        UpDownAlign = LeftRightAlignment.Right,
                        TabIndex = ++tabIndex
                    };
                    nud.ValueChanged += new EventHandler(ParameterChanged);
                    nud.LostFocus += new EventHandler(OnNudLostFocus);
                    nud.GotFocus += new EventHandler(OnNudGotFocus);
                    nud.Click += new EventHandler(OnNudGotFocus);
                    if (null != timer)
                        nud.MouseEnter += new EventHandler(OnNudMouseEnter);
                    
                    Panel2.Controls.Add(nud);
                }
                else if (param is ParameterDouble paramDouble)
                {
                    // do not show special parameters
                    if (string.Equals(paramDouble.Name, "ep1") || string.Equals(paramDouble.Name, "th1"))
                        continue;

                    Label lbl = new Label
                    {
                        Text = Translate(paramDouble.Description) + " (" + paramDouble.Name + ")",
                        Location = new Point(lblX + param.IndentValue, posY),
                        Size = lblSize,
                        TabIndex = ++tabIndex
                    };
                    Panel2.Controls.Add(lbl);

                    NumericUpDown nud = new NumericUpDown
                    {
                        Name = "nud" + paramDouble.Name,
                        Minimum = paramDouble.HasValueMin ? (decimal)paramDouble.ValueMin : 0,
                        Maximum = paramDouble.HasValueMax ? (decimal)paramDouble.ValueMax : 10000,
                        DecimalPlaces = 2,
                        ThousandsSeparator = false,
                        Value = (decimal)paramDouble.Value,
                        Location = new Point(nudX + param.IndentValue, posY),
                        Size = nudSize,
                        UpDownAlign = LeftRightAlignment.Right
                    };
                    nud.ValueChanged += new EventHandler(ParameterChanged);
                    nud.LostFocus += new EventHandler(OnNudLostFocus);
                    nud.GotFocus += new EventHandler(OnNudGotFocus);
                    nud.Click +=new EventHandler(OnNudGotFocus);
                    if (null != timer)
                        nud.MouseEnter += new EventHandler(OnNudMouseEnter);
                    nud.TabIndex = ++tabIndex;
                    Panel2.Controls.Add(nud);
                }
                else if (param is ParameterInt paramInt)
                {
                    Panel2.Controls.Add(
                        new Label
                        {
                            Text = Translate(paramInt.Description) + " (" + Translate(paramInt.Name) + ")",
                            Location = new Point(lblX + param.IndentValue, posY),
                            Size = lblSize,
                            TabIndex = ++tabIndex
                        }
                        );

                    NumericUpDown nud = new NumericUpDown
                    {
                        Name = "nud" + paramInt.Name,
                        Minimum = paramInt.HasValueMin ? paramInt.ValueMin : 0,
                        Maximum = paramInt.HasValueMax ? paramInt.ValueMax : 10000,
                        DecimalPlaces = 0,
                        ThousandsSeparator = false,
                        Value = paramInt.Value,
                        Location = new Point(nudX + param.IndentValue, posY),
                        Size = nudSize,
                        UpDownAlign = LeftRightAlignment.Right
                    };
                    nud.ValueChanged += new EventHandler(ParameterChanged_WithRecreate);
                    nud.TabIndex = ++tabIndex;
                    Panel2.Controls.Add(nud);
                }
                else if (param is ParameterBool paramBool)
                {
                    CheckBox chkb = new CheckBox
                    {
                        Name = "chkb" + paramBool.Name,
                        Text = Translate(paramBool.Description) + " (" + Translate(paramBool.Name) + ")",
                        Location = new Point(lblX + param.IndentValue, posY),
                        Size = chkbSize,
                        Checked = paramBool.Value
                    };
                    chkb.CheckedChanged += new EventHandler(ParameterChanged_WithRecreate);
                    chkb.TabIndex = ++tabIndex;
                    Panel2.Controls.Add(chkb);
                }
                else if (param.GetType() == typeof(ParameterMulti))
                {
                    ParameterMulti paramMulti = param as ParameterMulti;

                    Label lbl = new Label
                    {
                        Text = Translate(paramMulti.Description) + " (" + Translate(paramMulti.Name) + ")",
                        Location = new Point(lblX + param.IndentValue, posY),
                        Size = new Size(nudX - cbSize.Width + nudSize.Width - lblX, lblSize.Height),
                        TabIndex = ++tabIndex
                    };
                    Panel2.Controls.Add(lbl);

                    ComboBox combo = new ComboBox
                    {
                        Name = "cb" + paramMulti.Name,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Location = new Point(nudX - cbSize.Width + nudSize.Width + param.IndentValue, posY),
                        Size = cbSize,
                        TabIndex = ++tabIndex
                    };
                    combo.Items.AddRange(Translate(paramMulti.DisplayList));
                    combo.SelectedIndex = paramMulti.Value;
                    combo.SelectedIndexChanged += new EventHandler(ParameterChanged_WithRecreate);
                    Panel2.Controls.Add(combo);
                }
                posY += incY;
            }

            // FactoryDataCtrl
            if (ShowSummary && !_buttonCloseVisible && !_buttonValidateVisible)
            {
                int controlHeight = 150;
                int ctrlX = 5; 
                factoryDataCtrl = new FactoryDataCtrl
                {
                    Location = new Point(ctrlX, Panel2.Height - (controlHeight + 20)),
                    Size = new Size(Panel2.Width - 2 * ctrlX, controlHeight),
                    Visible = true,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left
                };
                factoryDataCtrl.TabChanged += new FactoryDataCtrl.onTabChanged(OnFactoryDataCtrlTabChanged);
                Panel2.Controls.Add(factoryDataCtrl);
            }
            // btValidate
            btValidate = new Button
            {
                Name = "ValidateButton",
                Text = Properties.Resource.STR_VALIDATE,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(Panel2.Width - 80, Panel2.Height - 50),
                Size = new Size(70, 20),
                Visible = _buttonValidateVisible
            };
            btValidate.Click += new EventHandler(OnValidate);
            Panel2.Controls.Add(btValidate);

            // btClose
            btClose = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Name = "CloseButton",
                Text = Properties.Resource.STR_CLOSE,
                Location = new Point(Panel2.Width - 80, Panel2.Height - 25),
                Size = new Size(70, 20),
                Visible = _buttonCloseVisible
            };
            btClose.Click += new EventHandler(OnClose);
            Panel2.Controls.Add(btClose);

            //  force to update parameter stack, once control have been recreated
            SetParametersDirty();
        }

        /// <summary>
        /// gets outer dimensions of a case
        /// </summary>
        /// <param name="length">outer length</param>
        /// <param name="width">outer width</param>
        /// <param name="height">outer height</param>
        /// <returns>true if component supports palletization</returns> 
        public bool GetDimensions(ref double length, ref double width, ref double height)
        {
            Component component = Component;
            if (component == null)  return false;
            return component.GetDimensions(CurrentParameterStack, ref length, ref width, ref height);
        }
        public bool GetFlatDimensions(ref double length, ref double width, ref double height)
        {
            Component component = Component;
            if (component == null) return false;
            return component.GetFlatDimensions(CurrentParameterStack, ref length, ref width, ref height);
        }
        public bool AllowPalletization
        {
            get
            {
                double length = 0.0, width = 0.0, height = 0.0;
                Component component = Component;
                if (component == null) return false;
                return component.GetDimensions(CurrentParameterStack, ref length, ref width, ref height);
            }
        }
        public bool AllowFlatPalletization
        {
            get
            { 
                double length = 0.0, width = 0.0, height = 0.0;
                Component component = Component;
                if (component == null) return false;
                return component.GetFlatDimensions(CurrentParameterStack, ref length, ref width, ref height);                
            }
        }
        private void OnNudGotFocus(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            nud.Select(0, nud.ToString().Length);
        }
        private void OnFactoryDataCtrlTabChanged(int currentIndex)
        {
            _computeBbox = true;
            Panel1.Invalidate();
        }

        /// <summary>
        /// Save control values in parameter stack
        /// </summary>
        /// <param name="stack">Parameter stack passed as ref</param>
        private bool SaveControlValues(ref ParameterStack stack)
        {
            if (null == stack) return false;
            // load some parameters from majorations
            if (null != _profileLoader)
            {
                foreach (Parameter param in stack.ParameterList)
                {
                    if (_profileLoader.HasParameter(param))
                        stack.SetDoubleParameter(param.Name, _profileLoader.GetParameterDValue(param.Name));
                }
            }
            // load other parameters
            foreach (Control ctrl in Panel2.Controls)
            {
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    NumericUpDown nud = ctrl as NumericUpDown;
                    if (nud.Name.Equals("nudThickness"))
                    {
                        _thickness = Convert.ToDouble(nud.Value);
                        if (stack.HasParameter("ep1")) stack.SetDoubleParameter("ep1", _thickness);
                        if (stack.HasParameter("th1")) stack.SetDoubleParameter("th1", _thickness);
                    }
                    else
                        foreach (Parameter param in stack.ParameterList)
                        {
                            if (nud.Name.Equals("nud" + param.Name))
                            {
                                if (param is ParameterDouble paramDouble)
                                {
                                    try { paramDouble.Value = Convert.ToDouble(nud.Value); }
                                    catch (Exception ex) { log.Error(ex.ToString()); }
                                }
                                else if (param is ParameterInt paramInt)
                                {
                                    try { paramInt.Value = Convert.ToInt32(nud.Value); }
                                    catch (Exception ex) { log.Error(ex.ToString()); }
                                }
                            }
                            else if (nud.Name.Equals("nudAngle" + param.Name))
                            {
                                if (param is ParameterAngle paramAngle)
                                {
                                    try { paramAngle.Value = Convert.ToDouble(nud.Value); }
                                    catch (Exception ex) { log.Error(ex.ToString()); }
                                }
                            }
                        }
                }
                else if (ctrl.GetType() == typeof(CheckBox))
                {
                    CheckBox chkb = ctrl as CheckBox;
                    foreach (Parameter param in stack.ParameterList)
                    {
                        if (param is ParameterBool paramBool && chkb.Name.Equals("chkb" + param.Name))
                            paramBool.Value = chkb.Checked;
                    }
                }
                else if (ctrl.GetType() == typeof(ComboBox))
                {
                    ComboBox cb = ctrl as ComboBox;
                    foreach (Parameter param in stack.ParameterList)
                    {
                        if (param is ParameterMulti paramMulti && cb.Name.Equals("cb" + param.Name))
                            paramMulti.Value = cb.SelectedIndex;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// clear current parameter stack
        /// </summary>
        private void ClearParameterStack()
        {
            _paramStackCurrent = null;
            SetParametersDirty();
            Panel2.Controls.Clear();
        }
        /// <summary>
        /// Current parameter stack
        /// </summary>
        public ParameterStack CurrentParameterStack
        {
            get
            {
                // sanity check
                if (null == Component)
                    throw new Exception("No Component loaded! -> Can not get CurrentParameterStack!");
                if (_dirtyParameters || null == _paramStackCurrent)
                {
                    SaveControlValues(ref _paramStackCurrent);
                    _paramStackCurrent = Component.BuildParameterStack(_paramStackCurrent);
                    HasDependancies = Component.GetDependancies(_paramStackCurrent).Count > 0;
                    _dirtyParameters = false;
                }
                System.Diagnostics.Debug.Assert(null != _paramStackCurrent, "Current parameter stack shall not be null");
                return _paramStackCurrent;            
            }
        }

        /// <summary>
        /// this method will force the property CurrentParameterStack
        /// - to save current parameter values
        /// - rebuild parameter stack by component
        /// </summary>
        private void SetParametersDirty()
        {
            _dirtyParameters = true;
            _computeBbox = true;
        }

        public void SetParameterValue(string paramName, double paramValue)
        {
            if (null == _paramStackCurrent)
               _paramStackCurrent = Component.BuildParameterStack(_paramStackCurrent);
            _paramStackCurrent.SetDoubleParameter(paramName, paramValue);

            _picGraphics.DrawingBox = BoundingBox;
            Redraw(true);
        }
#endregion

#region Dependancies
        /// <summary>
        /// Allows checking dependancy status :
        /// - true : component has some dependancies,
        /// - false : component has no dependancy with the current parameter stack
        /// Setting this property also triggers event DependancyStatusChanged
        /// </summary>
        public bool HasDependancies
        {
            get { return _hasDependancies; }
            set
            {
                if (value != _hasDependancies)
                    DependancyStatusChanged?.Invoke(this, value);
                _hasDependancies = value;
            }        
        }
        /// <summary>
        /// Build a list of component dependancie GUIDs
        /// </summary>
        public List<Guid> Dependancies
        {
            get
            {
                if (null == _component) return new List<Guid>();
                return _component.GetDependancies(CurrentParameterStack);  
            }
        }
#endregion

#region Event handlers
        /// <summary>
        /// Handle control value modifications
        /// </summary>
        /// <param name="o">Sender</param>
        /// <param name="e">Event arguments</param>
        protected void ParameterChanged(object o, EventArgs e)
        {
            try
            {
               SetParametersDirty();
                if (o is NumericUpDown nud && UnitSystem.Instance.USyst == UnitSystem.EUnit.METRIC)
                    nud.Increment = (nud.Value < 10.0m) ? 0.5m : 1.0m;
                if (null != _picGraphics)
                    _picGraphics.DrawingBox = BoundingBox;
                Redraw(true);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
        protected void ParameterChanged_WithRecreate(object o, EventArgs e)
        {
            try
            {
                SetParametersDirty();

                if (o is NumericUpDown nud && UnitSystem.Instance.USyst == UnitSystem.EUnit.METRIC)
                    nud.Increment = (nud.Value < 10.0m) ? 0.5m : 1.0m;
                // recreate plugin controls
                CreatePluginControls();
                if (null != _picGraphics)
                    _picGraphics.DrawingBox = BoundingBox;
                Redraw(true);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
        protected void OnNudLostFocus(object sender, EventArgs e) => Redraw(true);
        void OnBnEditMajorations(object sender, EventArgs e)
        {
            try
            {
                _profileLoader?.EditMajorations();
                SetParametersDirty();
                Redraw(true);                
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void OnProfileSelected(object o, EventArgs e)
        {
            try
            {
                if (null != _profileLoader)
                    _profileLoader.Selected = _comboProfile.SelectedItem as Profile;
                SetParametersDirty();
                Redraw(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void OnClose(object o, EventArgs e)
        {
            // send event so that container is notified that the user has pressed Cancel
            CloseClicked?.Invoke(this, e);
        }

        protected void OnValidate(object o, EventArgs e)
        {
            // send event so that container is notified that the user has pressed Validate
            ValidateClicked?.Invoke(this, e);
        }
#endregion

#region Nud enter event handler
        private void OnNudMouseEnter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nudControl && null != timer)
            {
                if (nudControl.Name.StartsWith("nudAngle"))
                    return;
                AnimateParameterName(nudControl.Name.Substring(3));
            }
        }
        public void AnimateParameterName(string parameterName)
        {
            _parameterName = parameterName;
            // copies parameter stack
            tempParameterStack = CurrentParameterStack.Clone();
            // retrieve initial value for parameter
            _parameterInitialValue = tempParameterStack.GetDoubleParameterValue(_parameterName);
            // start timer
            _timerStep = 0;
            timer.Start();
        }
        void OnTimerTick(object sender, EventArgs e)
        {
            ++_timerStep;
            if (_timerStep < _noStepsTimer)
            {
                double parameterValue = 0;
                int iStep = _timerStep < _noStepsTimer / 2 ? _timerStep : _noStepsTimer - 1 - _timerStep;
                double unitMultiplicator = UnitSystem.Instance.USyst == UnitSystem.EUnit.US ? 1.0 / 25.4 : 1.0;

                if (_parameterInitialValue > 10)
                    parameterValue = _parameterInitialValue + iStep * 0.1 *unitMultiplicator *  _parameterInitialValue;
                else if (_parameterInitialValue <= 10)
                    parameterValue = _parameterInitialValue + iStep * 1 * unitMultiplicator;

                tempParameterStack.SetDoubleParameter(_parameterName, parameterValue);
                _computeBbox = false;
                Panel1.Invalidate();
            }
            else
            {   // reset original parameter stack + stop timer
                tempParameterStack = null;
                timer.Stop();
                // redraw a last timer
                Panel1.Invalidate();
            }
        }
#endregion

#region Static methods
        public static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
#endregion

#region Parameter translation
        public ILocalizer Localizer { get; set; } = null;
        private string Translate(string term)
        {
            if (null == Localizer)
                return term;
            else
                return Localizer.GetTranslation(term);
        }
        private string[] Translate(string[] terms)
        {
            if (null == Localizer)
                return terms;
            else
            {
                List<string> sArray = new List<string>();
                foreach (string s in terms)
                    sArray.Add(Translate(s));
                return sArray.ToArray();
            }
        }
#endregion
    }
}
