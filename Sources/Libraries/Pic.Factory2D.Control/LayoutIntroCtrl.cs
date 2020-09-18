#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.ThreadCallback;
using treeDiM.UserControls;

using DesLib4NET;
#endregion

namespace Pic.Factory2D.Control
{
    #region LayoutIntroCtrl
    [Guid("097304D8-999D-45D3-9153-1D1F971E15C1")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Pic.Factory2D.Control.LayoutIntroCtrl")]
    [ComVisible(true)]
    public partial class LayoutIntroCtrl : UserControl
    {

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(LayoutIntroCtrl));
        private ImpositionTool impositionTool;
        #endregion

        #region PluginValidated event (+ associated delegate)
        public delegate void LayoutCtlrHandler(object sender, LayoutCtrlEventArgs e);
        public event LayoutCtlrHandler LayoutValidated;
        public event LayoutCtlrHandler LayoutCanceled;
        #endregion

        #region Constructor
        public LayoutIntroCtrl()
        {
            InitializeComponent();
        }
        #endregion

        #region Private properties
        private int NumberDirX => (int)nudNumberDirX.Value;
        private int NumberDirY => (int)nudNumberDirY.Value;
        private int Mode
        {
            set
            {
                rbLayoutMode1.Checked = (0 == value);
                rbLayoutMode2.Checked = (1 == value);
            }
            get => rbLayoutMode1.Checked ? 0 : 1;
        }
        private ImpositionSettings ImpSettings => (Mode == 0) ? new ImpositionSettings(CurrentCardboardFormat) : new ImpositionSettings(NumberDirX, NumberDirY);
        #endregion

        #region UserControl override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetFormatsFileName(@"C:\Picador\CardboardFormats.xml");

            // initialize number of rows/columns
            nudNumberDirX.Value = 2;
            nudNumberDirY.Value = 2;

            // update mode
            rbLayoutMode1.Checked = true;
            cbTopBottom.SelectedIndex = 0;
            cbRightLeft.SelectedIndex = 0;

            OnLayoutModeChanged(this, null);
        }
        #endregion

        #region Implementation ILayoutCtrl
        public void SetDrawingName(string drawingName)
        {
            DrawingName = drawingName;
        }
        public int Result { get; set; }
        public string GetOutputFile()
        {
            return OutputFilePath;
        }
        public void SetFormatsFileName(string fileNameFormats)
        {
            if (!File.Exists(fileNameFormats))
                return;
            FileNameFormats = fileNameFormats;

            // load cardboard formats from xml file
            CardboardFormats formats = CardboardFormats.LoadFromFile(FileNameFormats);
            foreach (fCardboardFormat cf in formats.fCardboardFormat)
                cbCardboardFormat.Items.Add(cf);
            if (cbCardboardFormat.Items.Count > 0)
                cbCardboardFormat.SelectedIndex = 0;
        }
        #endregion

        #region Private properties

        private CardboardFormat CurrentCardboardFormat
        {
            get
            {
                fCardboardFormat fcb = cbCardboardFormat.Items[cbCardboardFormat.SelectedIndex] as fCardboardFormat;
                return new CardboardFormat(0, fcb.name, string.Empty, fcb.dimensions[0], fcb.dimensions[1]);
            }
        }
        private ImpositionSettings.HAlignment HorizontalAlignment => Mode == 0 ? (ImpositionSettings.HAlignment)cbRightLeft.SelectedIndex : ImpositionSettings.HAlignment.HALIGN_LEFT;
        private ImpositionSettings.VAlignment VerticalAlignment => Mode == 0 ? (ImpositionSettings.VAlignment)cbTopBottom.SelectedIndex : ImpositionSettings.VAlignment.VALIGN_BOTTOM;
        private double ImpSpaceX => (double)nudSpaceX.Value;
        private double ImpSpaceY => (double)nudSpaceY.Value;
        private double ImpMarginLeftRight => (double)nudLeftRightMargin.Value;
        private double ImpMarginBottomTop => (double)nudTopBottomMargin.Value;
        private double ImpRemainingMarginLeftRight => (double)nudLeftRightRemaining.Value;
        private double ImpRemainingMarginBottomTop => (double)nudTopBottomRemaining.Value;
        private Vector2D Offset
        {
            get => new Vector2D((double)nudOffsetX.Value, (double)nudOffsetY.Value); 
            set { nudOffsetX.Value = (decimal)value.X; nudOffsetY.Value = (decimal)value.Y; }
        }
        private bool AllowColumnRotation => chkbColRotations.Checked;
        private bool AllowRowRotation => chkbColRotations.Checked;
        private bool AllowBothDirection => chkbBothDirections.Checked;

        public string OutputFilePath { get; set; }
        public string FileNameFormats { get; set; }
        public string DrawingName { get; set; }

        public List<ImpositionSolution> _solutions;
        #endregion

        #region Event handlers
        private void OnEditFormats(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", FileNameFormats);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void OnLayoutModeChanged(object sender, EventArgs e)
        {
            cbCardboardFormat.Enabled = (Mode == 0);
            bnEditCardboardFormats.Enabled = (Mode == 0);
            lbCardboardFormat.Enabled = (Mode == 0);

            lbDirX.Enabled = (Mode == 1);
            lbDirY.Enabled = (Mode == 1);
            nudNumberDirX.Enabled = (Mode == 1);
            nudNumberDirY.Enabled = (Mode == 1);
        }

        private void OnOK(object sender, EventArgs e)
        {
            // build factory entities
            try
            {
                // load file
                PicFactory factory = new PicFactory();
                string fileExt = Path.GetExtension(DrawingName);
                if (string.Equals(".des", fileExt, StringComparison.CurrentCultureIgnoreCase))
                {
                    using (DES_FileReader fileReader = new DES_FileReader())
                    {
                        PicLoaderDes picLoaderDes = new PicLoaderDes(factory);
                        fileReader.ReadFile(DrawingName, picLoaderDes);
                    }
                }
                else if (string.Equals(".dxf", fileExt, StringComparison.CurrentCultureIgnoreCase))
                {
                    using (PicLoaderDXF picLoaderDxf = new PicLoaderDXF(factory))
                        picLoaderDxf.Load(DrawingName);
                }
                // build imposition solutions
                impositionTool = ImpositionTool.Instantiate(factory, ImpSettings);
                // -> margins
                impositionTool.SpaceBetween = new Vector2D(ImpSpaceX, ImpSpaceY);
                impositionTool.Margin = new Vector2D(ImpMarginLeftRight, ImpMarginBottomTop);
                impositionTool.MinMargin = new Vector2D(ImpRemainingMarginLeftRight, ImpRemainingMarginBottomTop);
                impositionTool.HorizontalAlignment = HorizontalAlignment;
                impositionTool.VerticalAlignment = VerticalAlignment;
                // -> patterns
                impositionTool.AllowRotationInColumns = AllowColumnRotation;
                impositionTool.AllowRotationInRows = AllowRowRotation;
                impositionTool.AllowBothDirection = AllowBothDirection;
                // -> offset
                impositionTool.ImpositionOffset = Offset;
                _solutions = new List<ImpositionSolution>();

                // instantiate ProgressWindow and launch process
                ProgressWindow progress = new ProgressWindow();
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateImpositionSolutions), progress);
                progress.ShowDialog();

                if (null != _solutions && _solutions.Count > 0)
                {
                    FormLayoutViewer form = new FormLayoutViewer
                    {
                        Solutions = _solutions,
                        Format = CurrentCardboardFormat,
                        DrawingName = Path.GetFileNameWithoutExtension(DrawingName)
                    };
                    if (DialogResult.OK == form.ShowDialog(this))
                    {
                        OutputFilePath = form.OutputFilePath;
                        Result = form.Result;
                    }
                    else
                    {
                        Result = 0;
                        return;
                    }
                }
            }
            catch (PicToolTooLongException /*ex*/)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.ID_ABANDONPROCESSING
                    , PicToolArea.MaxNoSegments)
                    , Application.ProductName
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            LayoutValidated?.Invoke(sender, new LayoutCtrlEventArgs(Result, GetOutputFile()));
        }

        private void GenerateImpositionSolutions(object status)
        {
            IProgressCallback callback = status as IProgressCallback;
            impositionTool.GenerateSortedSolutionList(callback, out _solutions);
        }

        private void OnCancel(object sender, EventArgs e)
        {
            LayoutCanceled?.Invoke(sender, new LayoutCtrlEventArgs(0, string.Empty));
        }
        #endregion

        #region MAPPING_OF_USER32_DLL_SECTION
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(
            int hwnd, uint wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            int hwnd, uint wMsg, int wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            int hwnd, uint wMsg, int wParam, out int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int GetNbFiles(
            int hwnd, uint wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int GetFileNames(
            int hwnd, uint wMsg,
            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] wParam,
            int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            int hwnd, uint wMsg, int wParam, StringBuilder lParam);
        #endregion

        #region ActiveX Dll functions
        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // And create the 'Control' key - this allows it to show up in 
            // the ActiveX control container 
            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry - needed if not string named and GACced.
            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            // Finally close the main key
            k.Close();
        }
        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open HKCR\CLSID\{guid} for write access
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control' key, but don't throw an exception if it does not exist
            k.DeleteSubKey("Control", false);

            // Next open up InprocServer32
            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key, again not throwing if missing 
            k.DeleteSubKey("CodeBase", false);

            // Finally close the main key 
            k.Close();
        }
        #endregion
    }
    #endregion
}
