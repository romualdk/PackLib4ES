#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

using log4net;
using Sharp3D.Math.Core;

using Pic.Factory2D;
using Pic.Factory2D.Control;
using System.Security.Permissions;
#endregion

namespace Pic.Plugin.ViewCtrl
{
    [
     ComVisible(false),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     Docking(DockingBehavior.AutoDock)
    ]
    public partial class PluginViewUCtrl : UserControl, IToolInterface
    {
        #region Constructor
        public PluginViewUCtrl()
        {
            InitializeComponent();

            //must use reflection to set double buffering on the child panel controls  
            MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
            mi.Invoke(splitContHorizLeft.Panel1, args);
        }
        #endregion

        #region UserControl override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            splitContainerVert.SplitterWidth = 1;
            splitContHorizLeft.SplitterWidth = 1;
            splitContHorizRight.SplitterWidth = 1;
        }
        protected override void InitLayout()
        {
            base.InitLayout();

            _computeBbox = true;
            // event global cotation properties notification
            PicGlobalCotationProperties.Modified += new PicGlobalCotationProperties.OnGlobalCotationPropertiesModified(Refresh);
            // context menu
            toolStripMIShowCotation.Checked = _showCotationAuto;
            toolStripMIReflectionX.Checked = _reflectionX;
            toolStripMIReflectionY.Checked = _reflectionY;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            splitContainerVert.SplitterDistance = Width - 250;
        }
        private void OnFeedbackEmitted(object sender, List<string> messages)
        {
            splitContHorizLeft.Panel2Collapsed = !messages.Any();
            if (messages.Any())
            {
                var listMessages = messages.Select(m => m.Substring(m.LastIndexOf('|') + 1)).ToList();
                rtbMessages.Text = listMessages.Any() ? listMessages.Aggregate((i, j) => i + "\n" + j) : string.Empty;
            }
        }
        /// <summary>
        /// OnPaintBackground is overriden to be disabled for double buffering
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e) { }
        #endregion

        #region Timer
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
        private void OnTimerTick(object sender, EventArgs e)
        {
            ++_timerStep;
            if (_timerStep < NoStepsTimer)
            {
                double parameterValue = 0;
                int iStep = _timerStep < NoStepsTimer / 2 ? _timerStep : NoStepsTimer - 1 - _timerStep;
                double unitMultiplicator = UnitSystem.Instance.USyst == UnitSystem.EUnit.US ? 1.0 / 25.4 : 1.0; 


                if (_parameterInitialValue > 10)
                    parameterValue = _parameterInitialValue + iStep * 0.1 * unitMultiplicator * _parameterInitialValue;
                else if (_parameterInitialValue <= 10)
                    parameterValue = _parameterInitialValue + iStep * 1 * unitMultiplicator;

                tempParameterStack.SetDoubleParameter(_parameterName, parameterValue);
                Redraw(false);
            }
            else
            {   // reset original parameter stack + stop timer
                tempParameterStack = null;
                timer.Stop();
                // redraw a last timer
                Redraw(false);
            }
        }
        #endregion

        #region Painting
        public void Redraw(bool computeBbox)
        {
            _computeBbox = computeBbox;
            splitContHorizLeft.Panel1.Invalidate();
        }
        public void FitView() => Redraw(true);
        private void OnDrawingPanelPaint(object sender, PaintEventArgs e)
        {
            try
            {
                if (null == Component || DesignMode)
                    return;
                RePaint(e, tempParameterStack ?? CurrentParameterStack);
            }
            catch (Exception ex) { log.Error(ex.ToString()); }
        }
        private void RePaint(PaintEventArgs e, ParameterStack stack)
        {
            try
            {
                SplitterPanel panel1 = splitContHorizLeft.Panel1;

                // instantiate PicGraphics
                if (_picGraphics == null)
                {
                    _picGraphics = new PicGraphicsCtrlBased(panel1.ClientSize, e.Graphics)
                    {
                        BackgroundColor = Factory2D.Control.Properties.Settings.Default.BackgroundColor
                    };
                }
                _picGraphics.GdiGraphics = e.Graphics;
                _picGraphics.Size = panel1.ClientSize;

                // build factory
                using (PicFactory factory = new PicFactory())
                {
                    // create entities
                    Component.CreateFactoryEntities(factory, stack);
                    if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
                    if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
                    // remove existing quotations
                    if (!_showCotationCode)
                        factory.Remove(PicFilter.FilterCotation);
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
                        Box = new Box2D(visitor.Box);
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
            catch (PluginException ex) { _picGraphics.ShowMessage(ex.Message); }
            catch (ComponentSearchException ex) { _picGraphics.ShowMessage(ex.Message); }
            catch (Exception ex) { _picGraphics.ShowMessage(ex.ToString()); log.Error(ex.ToString()); }
        }
        #endregion

        #region Public methods
        public PicFactory GetFactory()
        {
            PicFactory factory = new PicFactory();
            Component.CreateFactoryEntities(factory, CurrentParameterStack);
            if (_reflectionX) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionX));
            if (_reflectionY) factory.ProcessVisitor(new PicVisitorTransform(Transform2D.ReflectionY));
            return factory;
        }

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
                    FormatLoader = CardboardFormatLoader,
                    Factory = factory,
                    Offset = Component.ImpositionOffset(CurrentParameterStack)
                };
                if (DialogResult.OK == form.ShowDialog()) { }
            }
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
        
        public bool GetDimensions(ref double length, ref double width, ref double height)
            => (null != Component) ? Component.GetDimensions(CurrentParameterStack, ref length, ref width, ref height) : false;
        public bool GetFlatDimensions(ref double length, ref double width, ref double height)
           => (null != Component) ? Component.GetFlatDimensions(CurrentParameterStack, ref length, ref width, ref height) : false;
        #endregion

        #region File export
        public void WriteExportFile(string filePath, string fileExt)
        {
            // get export byte array
            byte[] byteArray = GetExportFile(fileExt, CurrentParameterStack);
            // write byte array to stream
            using (FileStream fstream = new FileStream(filePath, FileMode.Create))
                fstream.Write(byteArray, 0, byteArray.GetLength(0));
        }
        public byte[] GetExportFile(string fileExt, ParameterStack stack)
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
                : PicFilter.FilterCotation | !(new PicFilterLineType(PicGraphics.LT.LT_COTATION) | new PicFilterLineType(PicGraphics.LT.LT_AXIS));
            filter = filter & PicFilter.FilterNoZeroEntities;

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

#endregion

#region Control event handlers
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
                FitView();
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
                FitView();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
        private void OnNudGotFocus(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud)
                nud.Select(0, nud.ToString().Length);
        }
        protected void OnNudLostFocus(object sender, EventArgs e) => Redraw(true);

        void OnBnEditMajorations(object sender, EventArgs e)
        {
            try
            {
                if (null != ProfileLoader)
                    ProfileLoader.EditMajorations();
                SetParametersDirty();
                FitView();
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
                if (null != ProfileLoader)
                    ProfileLoader.Selected = _comboProfile.SelectedItem as Profile;
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

#region Mouse event handlers
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Box2D box = new Box2D(_picGraphics.DrawingBox);
                box.Zoom(-e.Delta / 1000.0);
                _picGraphics.DrawingBox = box;
            }
            catch (InvalidBoxException /*ex*/) { _computeBbox = true; }
            catch (Exception ex) { log.Debug(ex.ToString()); }
            finally { splitContHorizLeft.Panel1.Invalidate(); }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            SplitterPanel panel1 = splitContHorizLeft.Panel1;
            panel1.Focus();
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point ptDiff = new Point(e.Location.X - _mousePositionPrev.X, e.Location.Y - _mousePositionPrev.Y);
                    double aspectRatio = 1.0;

                    Box2D box = _picGraphics.DrawingBox;
                    box.Center = new Vector2D(
                        box.Center.X - ptDiff.X * box.Width / (double)panel1.ClientSize.Width
                        , box.Center.Y + ptDiff.Y * box.Height / ((double)panel1.ClientSize.Height * aspectRatio));
                    _picGraphics.DrawingBox = box;

                    panel1.Invalidate();
                }
                _mousePositionPrev = e.Location;
            }
            catch (InvalidBoxException /*ex*/) { _computeBbox = true; }
            catch (Exception ex) { log.Debug(ex.ToString()); }
            finally { }
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip.Show(this, e.Location);
            else if (e.Button == MouseButtons.Left)
                Cursor.Current = new Cursor(new System.IO.MemoryStream(Properties.Resource.CursorPan));
        }
        private void OnMouseEnter(object sender, EventArgs e)
        {
            SplitterPanel panelLeft = splitContainerVert.Panel1;
            SplitterPanel panelRight = splitContHorizRight.Panel1;
            if (panelLeft.Focused)
                panelRight.Focus();
        }
#endregion

#region Context menu event handlers
        private void OnFitView(object sender, EventArgs e) => FitView();
        private void OnShowCotations(object sender, EventArgs e)
        {
            ShowCotationsAuto = !ShowCotationsAuto;
            if (sender is ToolStripMenuItem item)
                item.Checked = ShowCotationsAuto;
        }
        private void OnReflectionX(object sender, EventArgs e)
        {
            ReflectionX = !ReflectionX;
            if (sender is ToolStripMenuItem item)
                item.Checked = ReflectionX;
        }
        private void OnReflectionY(object sender, EventArgs e)
        {
            ReflectionY = !ReflectionY;
            if (sender is ToolStripMenuItem item)
                item.Checked = ReflectionY;
        }
        private void OnImposition(object sender, EventArgs e) => BuildLayout();
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

                OnFeedbackEmitted(this, new List<string>());
                if (null != _component)
                    _component.FeedBack += OnFeedbackEmitted;
                // update panel2 controls
                splitContHorizRight.Panel1.AutoScroll = true;
                CreatePluginControls();
                Redraw(true);
            }
        }

        [Browsable(false)]
        [Category("PluginViewUCtrl")]
        [DisplayName("PluginPath")]
        [DefaultValue("")]
        public string PluginPath
        {
            set
            {
                ClearParameterStack();
                using (ComponentLoader loader = new ComponentLoader())
                {
                    loader.SearchMethod = SearchMethod;
                    _component = loader.LoadComponent(value);
                    if (null != _component)
                        _component.FeedBack += OnFeedbackEmitted;
                }
                ProfileLoader?.SetComponent(_component);
                CreatePluginControls();
                FitView();
            }
        }
        public string LoadedComponentName => _component.Name;

        public CardboardFormatLoader CardboardFormatLoader { get; set; }
        public IComponentSearchMethod SearchMethod { get; set; }
        public ProfileLoader ProfileLoader { get; set; }

        public bool AllowPalletization => true;
        public bool AllowFlatPalletization => true;
#endregion

#region Dependancies
        /// <summary>
        /// Build a list of component dependancie GUIDs
        /// </summary>
        public List<Guid> Dependancies
        {
            get
            {
                if (null == _component)
                    return new List<Guid>();
                else
                    return _component.GetDependancies(CurrentParameterStack);
            }
        }
#endregion

#region IToolInterface
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
        public bool ReflectionX { get => _reflectionX; set { _reflectionX = value; Redraw(); } }
        public bool ReflectionY { get => _reflectionY; set { _reflectionY = value; Redraw(); } }
        public bool ShowCotationsAuto { get => _showCotationAuto; set { _showCotationAuto = value; Redraw(); } }
        public bool ShowCotationsCode { get => _showCotationCode; set { _showCotationCode = value; Redraw(); } }
        public bool ShowAxes { get => _showAxes; set { _showAxes = value; Redraw(); } }
        public bool IsPluginViewer => true;
        public bool HasDependancies
        {
            get => _hasDependancies;
            set { if (value != _hasDependancies) DependancyStatusChanged?.Invoke(this, value); _hasDependancies = value; }
        }

        public bool IsSupportingAutomaticFolding => _component == null ? false : _component.IsSupportingAutomaticFolding;
#endregion

#region Helpers
        private void Redraw()
        {
            if (DesignMode) return;
            _computeBbox = true;
            splitContHorizLeft.Panel1.Invalidate();
        }

        /// <summary>
        /// clear current parameter stack
        /// </summary>
        private void ClearParameterStack()
        {
            _paramStackCurrent = null;
            SetParametersDirty();
            splitContHorizRight.Panel1.Controls.Clear();
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
        /// <summary>
        /// Save control values in parameter stack
        /// </summary>
        /// <param name="stack">Parameter stack passed as ref</param>
        private bool SaveControlValues(ref ParameterStack stack)
        {
            if (null == stack)
                return false;
            // load some parameters from majorations
            if (null != ProfileLoader)
            {
                foreach (Parameter param in stack.ParameterList)
                {
                    if (ProfileLoader.HasParameter(param))
                        stack.SetDoubleParameter(param.Name, ProfileLoader.GetParameterDValue(param.Name));
                }
            }
            // load other parameters
            foreach (Control ctrl in splitContHorizRight.Panel1.Controls)
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
            SplitterPanel panel2 = splitContHorizRight.Panel1;
            panel2.Controls.Clear();

            // check if plugin has some majorations
            if (stack.HasMajorations)
            {
                if (null != ProfileLoader)
                {
                    Size btSize = new Size(50, 22);

                    // lbl Profile
                    panel2.Controls.Add(
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
                    _comboProfile.Items.AddRange(ProfileLoader.Profiles);
                    _comboProfile.SelectedItem = ProfileLoader.Selected;
                    _comboProfile.SelectedIndexChanged += new EventHandler(OnProfileSelected);
                    panel2.Controls.Add(_comboProfile);
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
                    panel2.Controls.Add(bt);

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
                panel2.Controls.Add(lbl);
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
                panel2.Controls.Add(nudThickness);

                // increment Y
                posY += incY;
            }

            // create unknown parameters
            foreach (Parameter param in CurrentParameterStack.ParameterList)
            {
                // do not create default parameters
                if (ProfileLoader != null && ProfileLoader.HasParameter(param)
                    || param.IsMajoration)
                    continue;

                if (param is ParameterAngle paramAngle)
                {
                    panel2.Controls.Add(
                        new Label
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
                        DecimalPlaces = 1,
                        Increment = 1,
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
                    panel2.Controls.Add(nud);
                }
                else if (param is ParameterDouble paramDouble)
                {
                    // do not show special parameters
                    if (string.Equals(paramDouble.Name, "ep1") || string.Equals(paramDouble.Name, "th1"))
                        continue;

                    panel2.Controls.Add(
                        new Label
                        {
                            Text = Translate(paramDouble.Description) + " (" + paramDouble.Name + ")",
                            Location = new Point(lblX + param.IndentValue, posY),
                            Size = lblSize,
                            TabIndex = ++tabIndex
                        }
                    );

                    NumericUpDown nud = new NumericUpDown
                    {
                        Name = "nud" + paramDouble.Name,
                        Minimum = paramDouble.HasValueMin ? (decimal)paramDouble.ValueMin : 0,
                        Maximum = paramDouble.HasValueMax ? (decimal)paramDouble.ValueMax : 10000,
                        DecimalPlaces = UnitSystem.Instance.USyst == UnitSystem.EUnit.US ? 2 : 1,
                        Increment = UnitSystem.Instance.USyst == UnitSystem.EUnit.US ? 0.1m : 1.0m,
                        ThousandsSeparator = false,
                        Value = (decimal)paramDouble.Value,
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
                    panel2.Controls.Add(nud);
                }
                else if (param.GetType() == typeof(ParameterInt))
                {
                    ParameterInt paramInt = param as ParameterInt;

                    panel2.Controls.Add(
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
                    panel2.Controls.Add(nud);
                }
                else if (param.GetType() == typeof(ParameterBool))
                {
                    ParameterBool paramBool = param as ParameterBool;

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
                    panel2.Controls.Add(chkb);
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
                    panel2.Controls.Add(lbl);

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
                    panel2.Controls.Add(combo);
                }
                posY += incY;
            }

            factoryDataCtrl.TabChanged += new FactoryDataCtrl.onTabChanged(OnFactoryDataCtrlChanged);

            //  force to update parameter stack, once control have been recreated
            SetParametersDirty();
        }

        private void OnFactoryDataCtrlChanged(int currentIndex)
        {
            _computeBbox = true;
            splitContHorizLeft.Panel1.Refresh();
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

        #region Event
        public delegate void DependancyStatusChangedHandler(Control pluginViewer, bool hasDependancy);
        public event DependancyStatusChangedHandler DependancyStatusChanged;
        [Category("Configuration"), Browsable(true), Description("Event raised by user click on Close button")]
        public event EventHandler CloseClicked;
        [Category("Configuration"), Browsable(true), Description("Event raised by user click on Validate button")]
        public event EventHandler ValidateClicked;
        #endregion

        #region Data members
        // log4net
        private static readonly ILog log = LogManager.GetLogger(typeof(PluginViewUCtrl));
        // other
        [NonSerialized] private Component _component;
        [NonSerialized] private PicGraphicsCtrlBased _picGraphics;
        [NonSerialized] private ParameterStack _paramStackCurrent;
        [NonSerialized] private bool _dirtyParameters = true;
        [NonSerialized] private bool _hasDependancies = false;

        private Point _mousePositionPrev;
        private bool _computeBbox = true;
        private bool _reflectionX = false, _reflectionY = false;
        private ComboBox _comboProfile;
        private bool _showCotationAuto = true, _showCotationCode = false, _showAxes = true;
        private double _thickness = 0.0;

        // *** parameter animation ***
        protected ParameterStack tempParameterStack = null;
        protected string _parameterName = string.Empty;

        protected int _timerStep = 0;

        protected int NoStepsTimer { get; set; } = Properties.Settings.Default.NumberOfAnimationSteps;
        public Box2D Box { get; set; }

        protected double _parameterInitialValue = 0.0;
        #endregion
    }
}
