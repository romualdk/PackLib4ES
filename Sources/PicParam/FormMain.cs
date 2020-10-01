#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using log4net;
using WeifenLuo.WinFormsUI.Docking;
using Pic.Factory2D;
using Pic.Plugin.ViewCtrl;
using PicParam.Properties;
using MRU;
using System.ComponentModel;
#endregion

namespace PicParam
{
    #region IMain interface
    public interface IMain
    {
        void UpdateToolCommands(NodeTag nodeTag, PluginViewCtrl pluginViewCtrl);
        void DependancyStatusChanged(Control pluginViewCtrl, bool hasDependancy);
    }
    #endregion

    public partial class FormMain : Form, IMain, IMRUClient
    {
        #region Constructor
        public FormMain(bool showRoot)
        {
            // set static instance
            Instance = this;

            try
            {
                InitializeComponent();
                Text = Application.ProductName;

                // set export application
                ApplicationAvailabilityChecker.AppendApplication("PicGEOM", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDES);
                ApplicationAvailabilityChecker.AppendApplication("PicDecoup", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppPicDecoupeDES);
                ApplicationAvailabilityChecker.AppendApplication("Picador3D", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppPic3DDES);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region IMain
        public void UpdateToolCommands(NodeTag nodeTag, PluginViewCtrl pluginViewerCtrl)
        {
        }
        #endregion

        #region Form overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                // load settings only if not in debug mode
                if (!Settings.Default.ShowLogConsole)
                    ToolStripManager.LoadSettings(this, Name);

                // ---
                // initialize menu state
                PicGlobalCotationProperties.ShowShortCotationLines = Settings.Default.UseCotationShortLines;
                _log.Info(string.Format("ShowShortCotationLines initialized with value : {0}", Settings.Default.UseCotationShortLines.ToString()));
                toolStripMenuItemCotationShortLines.Checked = PicGlobalCotationProperties.ShowShortCotationLines;

                // Set DockPanel properties
                dockPanel.DocumentStyle = DocumentStyle.DockingMdi;
                dockPanel.ActiveAutoHideContent = null;
                dockPanel.Parent = this;
                dockPanel.SuspendLayout(true);

                UpdateToolCommands(null, null);

                ShowLogConsole();
                dockPanel.ResumeLayout(true, true);

                if (IsWebSiteReachable)
                {
                    ShowStartPage();
                }
                CreateBasicLayout();

                // update tool bars
                UpdateToolCommands(null);

                // Most recently used databases
                mruManager = new MRUManager();
                mruManager.Initialize(
                    this,                              // owner form
                    databaseToolStripMenuItem,         // Recent Files menu item
                    mnuFileMRU,                        // Recent Files menu item
                    "Software\\treeDiM\\PLMPackLib");  // Registry path to keep MRU list

                mruManager.Add(Pic.DAL.ApplicationConfiguration.CustomSection.DatabasePath);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
            // restore window position
            if (null != Settings.Default.MainFormSettings && !Settings.Default.StartMaximized)
            {
                Settings.Default.MainFormSettings.Restore(this);
            }
            // show maximized
            if (Settings.Default.StartMaximized)
                WindowState = FormWindowState.Maximized;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _cleanUp.ForEach(a => a.Invoke());
            _cleanUp.Clear();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
        #endregion

        #region Docking
        private void CreateBasicLayout()
        {
            DockTreeView formTV = new DockTreeView() { CloseButtonVisible = false };
            formTV.SetMain(this);
            formTV.SelectionChanged += UpdateToolCommands;
            formTV.Show(dockPanel, DockState.Document);
            dockPanel.ResumeLayout(true, true);

            FormDownload.DatabaseModified += formTV.OnToolStripButtonRoot;

            toolStripButtonSearch.Click += formTV.OnToolStripButtonSearch;
            _cleanUp.Add(() => toolStripButtonSearch.Click -= formTV.OnToolStripButtonSearch);
            toolStripButtonCotationsAuto.Click += formTV.OnShowHideCotationsAuto;
            _cleanUp.Add(() => toolStripButtonCotationsAuto.Click -= formTV.OnShowHideCotationsAuto);
            toolStripButtonCotationsCode.Click += formTV.OnShowHideCotationsCode;
            _cleanUp.Add(() => toolStripButtonCotationsCode.Click -= formTV.OnShowHideCotationsCode);
            toolStripButtonAxes.Click += formTV.OnShowHideAxes;
            _cleanUp.Add(() => toolStripButtonAxes.Click -= formTV.OnShowHideAxes);
            toolStripButtonReflectionX.Click += formTV.OnToolStripReflectionX;
            _cleanUp.Add(() => toolStripButtonReflectionX.Click -= formTV.OnToolStripReflectionX);
            toolStripButtonReflectionY.Click += formTV.OnToolStripReflectionY;
            _cleanUp.Add(() => toolStripButtonReflectionY.Click -= formTV.OnToolStripReflectionY);
            toolStripButtonLayout.Click += formTV.OnToolStripLayout;
            _cleanUp.Add(() => toolStripButtonLayout.Click -= formTV.OnToolStripLayout);
            toolStripButtonEditParameters.Click += formTV.OnToolStripEditParameters;
            _cleanUp.Add(() => toolStripButtonEditParameters.Click -= formTV.OnToolStripEditParameters);
            toolStripEditComponentCode.Click += formTV.OnToolStripEditComponentCode;
            _cleanUp.Add(() => toolStripEditComponentCode.Click -= formTV.OnToolStripEditComponentCode);
            toolStripButtonExport.Click += formTV.OnExportFile;
            _cleanUp.Add(() => toolStripButtonExport.Click -= formTV.OnExportFile);

#if TREEDIMEXPORT
            toolStripMI_ExportApp.Click += formTV.OnExportPicGEOM;
            _cleanUp.Add(() => toolStripMI_ExportApp.Click -= formTV.OnExportPicGEOM);
            toolStripMI_PicGEOM.Click += formTV.OnExportPicGEOM;
            _cleanUp.Add(() => toolStripMI_PicGEOM.Click -= formTV.OnExportPicGEOM);
            toolStripMI_Picador3D.Click += formTV.OnExportPicador3D;
            _cleanUp.Add(() => toolStripMI_Picador3D.Click -= formTV.OnExportPicador3D);
            toolStripMI_PicDecoup.Click += formTV.OnExportPicDecoup;
            _cleanUp.Add(() => toolStripMI_PicDecoup.Click -= formTV.OnExportPicDecoup);
#endif

#if PROCESSOR
            toolStripButtonSUMMA.Click += formTV.OnToolStripButtonShowProcessor;
            _cleanUp.Add(() => toolStripButtonSUMMA.Click -= formTV.OnToolStripButtonShowProcessor);
            toolStripButtonARISTO.Click += formTV.OnToolStripButtonShowProcessor;
            _cleanUp.Add(() => toolStripButtonARISTO.Click -= formTV.OnToolStripButtonShowProcessor);
            toolStripButtonOceProCut.Click += formTV.OnToolStripButtonShowProcessor;
            _cleanUp.Add(() => toolStripButtonOceProCut.Click -= formTV.OnToolStripButtonShowProcessor);
            toolStripButtonZUND.Click += formTV.OnToolStripButtonShowProcessor;
            _cleanUp.Add(() => toolStripButtonZUND.Click -= formTV.OnToolStripButtonShowProcessor);
#endif
#if STACKBUILDER
            toolStripMIOptimalCase.Click += formTV.OnToolStripStackBuilderCaseOptimization;
            _cleanUp.Add(() => toolStripMIOptimalCase.Click -= formTV.OnToolStripStackBuilderCaseOptimization);
            toolStripMICasePalletAnalysis.Click += formTV.OnToolStripStackBuilderCasePallet;
            _cleanUp.Add(() => toolStripMICasePalletAnalysis.Click -= formTV.OnToolStripStackBuilderCasePallet);
            toolStripMIBundlePalletAnalysis.Click += formTV.OnToolStripStackBuilderBundlePallet;
            _cleanUp.Add(() => toolStripMIBundlePalletAnalysis.Click -= formTV.OnToolStripStackBuilderBundlePallet);
            toolStripMIBundleCaseAnalysis.Click += formTV.OnToolStripStackBuilderBundleCase;
            _cleanUp.Add(() => toolStripMIBundleCaseAnalysis.Click -= formTV.OnToolStripStackBuilderBundleCase);
#endif
        }
        public void DependancyStatusChanged(Control pluginViewCtrl, bool hasDependancy)
        {
            toolStripButtonEditParameters.Enabled = hasDependancy && pluginViewCtrl.Visible;
        }
#endregion

#region Log console / Start page
        public void ShowLogConsole()
        {
            // show or hide log console ?
            if (AssemblyConf == "debug" || Settings.Default.ShowLogConsole)
                LogConsole.Show(dockPanel, DockState.DockBottom);
            else
                LogConsole.Hide();
        }
        public void ShowStartPage()
        {
            if (!IsWebSiteReachable || null == FormStartPage)
                return;
            FormStartPage.Url = new Uri(Settings.Default.UrlStartPage);
            FormStartPage.Show(dockPanel, DockState.Document);
        }
        private void CloseStartPage()
        {
            if (null != FormStartPage)
                FormStartPage.Close();
            FormStartPage = null;
        }
        public void OnShowDownloadPage(object sender, EventArgs e)
        {
            try
            {
                if (!IsWebSiteReachable)
                    return;
                FormDownload.Show(dockPanel, DockState.Document);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
#endregion

#region Control event handlers
        private void UpdateTextPosition(object sender, EventArgs e)
        {
            // exit when in design mode
            if ((DesignMode) || !(Settings.Default.ShowCenteredTitle)) return;
            // measure text length and compute starting point of text
            Graphics g = CreateGraphics();
            double startingPoint = (Width / 2) - (g.MeasureString(Text.Trim(), Font).Width / 2);
            double widthOfASpace = g.MeasureString(" ", Font).Width;
            string tmp = " ";
            double tmpWidth = 0;

            // loop adds space characters until researched text starting point is reached
            int iTextLength = Text.Length;
            while ((tmpWidth + widthOfASpace) < startingPoint)
            {
                if (tmp.Length < 255 - iTextLength)
                    tmp += " ";
                tmpWidth += widthOfASpace;
            }
            Text = tmp + Text.Trim();
        }
#endregion

#region Menu event handlers
#region File
        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void OnBrowseFile(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fd = new OpenFileDialog
                {
                    Filter = "Picador file|*.des|Autocad dxf|*.dxf|All Files|*.*"
                };
                if (DialogResult.OK == fd.ShowDialog())
                {
                    FormBrowseFile form = new FormBrowseFile(fd.FileName);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }


#endregion
#region Help
        private void OnAboutClick(object sender, EventArgs e)
        {
            FormAboutBox form = new FormAboutBox();
            form.ShowDialog();
        }
        private void OnButtonHelpClick(object sender, EventArgs e) => OpenWebPage(Settings.Default.HelpUrl);
        private void OnHelpDeveloppers(object sender, EventArgs e) => OpenWebPage(Settings.Default.DevelopperHelpUrl);
#endregion
#region Database
        /// <summary>
        /// Backup
        /// </summary>
        private void ToolStripMenuItemBackup_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialogBackup.ShowDialog())
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTBackup(saveFileDialogBackup.FileName));
        }
        /// <summary>
        /// Restore
        /// </summary>
        private void ToolStripMenuItemRestore_Click(object sender, EventArgs e)
        {
            // warning
            if (MessageBox.Show(PicParam.Properties.Resources.ID_RESTOREWARNING
                , ProductName
                , MessageBoxButtons.OKCancel
                , MessageBoxIcon.Warning
                ) == DialogResult.Cancel)
                return;
            if (DialogResult.OK == openFileDialogRestore.ShowDialog())
            {
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTRestore(openFileDialogRestore.FileName));
                /*
                // refresh tree
                _treeViewCtrl.RefreshTree();
                // expand all so that modifications can be visualized
                if (_treeViewCtrl.Nodes.Count > 0)
                    _treeViewCtrl.Nodes[0].ExpandAll();
                */
            }
        }
        /// <summary>
        /// Merge
        /// </summary>
        private void ToolStripMenuItemMerge_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialogRestore.ShowDialog())
            {
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTMerge(openFileDialogRestore.FileName));
                /*
                // refresh tree
                _treeViewCtrl.RefreshTree();
                */
                // back to default cursor
                Cursor.Current = Cursors.Default;
            }
        }
        /// <summary>
        /// Clear
        /// </summary>
        private void ToolStripMenuItemClearDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // form
                if (DialogResult.Yes == MessageBox.Show(Resources.ID_CLEARDATABASEWARNING,
                    ProductName,
                    MessageBoxButtons.YesNo))
                {   // answered 'Yes' 
                    // start thread
                    FormWorkerThreadTask.Execute(new Pic.DAL.TPTClearDatabase());
                    /*
                    // refresh tree
                    _treeViewCtrl.RefreshTree();
                    */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(ex.Message));
            }
        }

        private void ToolStripMenuItemDefineDatabasePath_Click(object sender, EventArgs e)
        {
            try
            {
                // first check
                if (!Helpers.IsUserAdministrator)
                {
                    MessageBox.Show(Resources.ID_INFO_RUNASADMIN, Application.ProductName, MessageBoxButtons.OK);
                    return;
                }
                // edit database path
                FormEditDatabasePath form = new FormEditDatabasePath();
                form.ShowDialog();
            }
            catch (Exception ex)
            { _log.Error(ex.ToString()); }
        }
#endregion
#region Tools
        private void OnEditProfiles(object sender, EventArgs e)
        {
            FormEditProfiles form = new FormEditProfiles();
            form.ShowDialog();
        }
        private void OnSettings(object sender, EventArgs e)
        {
            try
            {
                // show option form
                OptionsFormPLMPackLib form = new OptionsFormPLMPackLib();
                DialogResult dres = form.ShowDialog();
                if (DialogResult.OK == dres)
                {
                    // need to force saving of Pic.Factory2D.Properties.Settings
                    Pic.Factory2D.Control.Properties.Settings.Default.Save();
                    // need to force saving of Pic.Plugin.ViewCtrl.Properties.Settings
                    PluginViewCtrl.SaveSettings();
                    // if need to restart application, indicate the user that the application will need to restart before exiting
                    if (form.ApplicationMustRestart)
                    {
                        MessageBox.Show(string.Format(Resources.ID_APPLICATIONMUSTRESTART, Application.ProductName));
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }
        void ToolStripMenuItemEditCardboardFormats_Click(object sender, System.EventArgs e)
        {
            try
            {
                FormEditCardboardFormats form = new FormEditCardboardFormats();
                if (DialogResult.OK == form.ShowDialog())
                { }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }
#endregion
#endregion

#region IMRUClient
        public void OpenMRUFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                mruManager.Remove(fileName);
                return;
            }
            if (Equals(Pic.DAL.ApplicationConfiguration.CustomSection.DatabasePath, fileName))
                return;

            // warn user that database path will be changed
            MessageBox.Show(string.Format(Resources.ID_APPLICATIONEXITING, Application.ProductName));
            // change database
            Pic.DAL.ApplicationConfiguration.SaveDatabasePath(fileName);
            // close and restart application
            Application.Restart();
            Application.ExitThread();
        }
#endregion

#region Update tool commands
        private void UpdateToolCommands(DockTreeView formTV)
        {
            try
            {
                toolStripSBPalletization.Visible = Settings.Default.ShowButtonPalletization;
                toolStripButtonLayout.Visible = Settings.Default.ShowButtonLayout;
                toolStripEditComponentCode.Visible = Settings.Default.ShowButtonEditCode;
                toolStripButtonHelp.Visible = Settings.Default.ShowButtonHelp;

                // for some unknown reasons these data may be changed to 'False' in the user.config file 
                toolStripMain.Visible = true;
                menuStripMain.Visible = true;
                statusStripMain.Visible = true;
                // enable toolbar buttons
                bool buttonsEnabled = false, palletisationAllowed = false, flatPalletisationAllowed = false;
                if (null != formTV)
                {
                    buttonsEnabled = formTV.TSButtonsEnabled;
                    palletisationAllowed = formTV.AllowPalletisation;
                    flatPalletisationAllowed = formTV.AllowFlatPalletisation;
                }
                toolStripButtonCotationsAuto.Enabled = buttonsEnabled;
                toolStripMenuItemCotationsAuto.Enabled = buttonsEnabled;
                toolStripButtonReflectionX.Enabled = buttonsEnabled;
                reflectionXToolStripMenuItem.Enabled = buttonsEnabled;
                toolStripButtonReflectionY.Enabled = buttonsEnabled;
                reflectionYToolStripMenuItem.Enabled = buttonsEnabled;
                toolStripButtonLayout.Enabled = buttonsEnabled;
                toolStripMenuItemCotationShortLines.Enabled = buttonsEnabled;
                toolStripButtonCotationsCode.Enabled = buttonsEnabled;
                toolStripMenuItemCotationsCode.Enabled = buttonsEnabled;
                toolStripButtonAxes.Enabled = buttonsEnabled;
                toolStripMenuItemAxes.Enabled = buttonsEnabled;
                toolStripButtonExport.Enabled = buttonsEnabled;
                // only allow palletization / case optimisation when a component is selected
                if (Settings.Default.ShowButtonPalletization)
                {
                    toolStripMICasePalletAnalysis.Enabled = palletisationAllowed;
                    toolStripMIOptimalCase.Enabled = palletisationAllowed;
                    toolStripMIBundleCaseAnalysis.Enabled = flatPalletisationAllowed;
                    toolStripMIBundlePalletAnalysis.Enabled = flatPalletisationAllowed;
                }
                // enable export toolbar buttons
                toolStripButtonExport.Enabled = true;//buttonsEnabled || _webBrowser4PDF.Visible;
                // "File" menu items
                exportToolStripMenuItem.Enabled = buttonsEnabled;
                toolStripButtonEditParameters.Enabled = buttonsEnabled;

                if (null != formTV)
                {
                    IToolInterface currentTool = formTV.CurrentTool;
                    if (null != currentTool)
                    {
                        // cotation auto
                        toolStripButtonCotationsAuto.CheckState = currentTool.ShowCotationsAuto ? CheckState.Checked : CheckState.Unchecked;
                        toolStripMenuItemCotationsAuto.CheckState = currentTool.ShowCotationsAuto ? CheckState.Checked : CheckState.Unchecked;
                        // cotation code
                        toolStripButtonCotationsCode.CheckState = currentTool.ShowCotationsCode ? CheckState.Checked : CheckState.Unchecked;
                        toolStripMenuItemCotationsCode.CheckState = currentTool.ShowCotationsCode ? CheckState.Checked : CheckState.Unchecked;
                        // axes
                        toolStripButtonAxes.CheckState = currentTool.ShowAxes ? CheckState.Checked : CheckState.Unchecked;
                        toolStripMenuItemAxes.CheckState = currentTool.ShowAxes ? CheckState.Checked : CheckState.Unchecked;
                        // reflectionX
                        toolStripButtonReflectionX.CheckState = currentTool.ReflectionX ? CheckState.Checked : CheckState.Unchecked;
                        reflectionXToolStripMenuItem.CheckState = currentTool.ReflectionX ? CheckState.Checked : CheckState.Unchecked;
                        // reflectionY
                        toolStripButtonReflectionY.CheckState = currentTool.ReflectionY ? CheckState.Checked : CheckState.Unchecked;
                        reflectionYToolStripMenuItem.CheckState = currentTool.ReflectionY ? CheckState.Checked : CheckState.Unchecked;

                        toolStripEditComponentCode.Enabled = currentTool.IsPluginViewer;
                        toolStripButtonEditParameters.Enabled = currentTool.HasDependancies;

                        // PDF3D button
                        bool tools3DGenerate = currentTool.IsSupportingAutomaticFolding  && File.Exists(formTV.Pic3DExporterPath);
                    }
                }
                else
                {
                    _log.Error("FormTV == null");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void ToolStripMenuItemCotationShortLines_Click(object sender, EventArgs e)
        {
            try
            {
                // update static flag
                PicGlobalCotationProperties.ShowShortCotationLines = !PicGlobalCotationProperties.ShowShortCotationLines;
                _log.Info(string.Format("Switched cotation short lines. New value : {0}", PicGlobalCotationProperties.ShowShortCotationLines.ToString()));
                // update menu
                toolStripMenuItemCotationShortLines.Checked = PicGlobalCotationProperties.ShowShortCotationLines;
                // save setting
                Settings.Default.UseCotationShortLines = PicGlobalCotationProperties.ShowShortCotationLines;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        
        private void OnExportDXF(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
#endregion

#region MainForm Load/Close event handling
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save toolstrip settings
            ToolStripManager.SaveSettings(this, Name);
            // do not save window position if StartMaximized property is set
            if (Settings.Default.StartMaximized) return;
            // save window position
            if (null == Settings.Default.MainFormSettings)
            {
                Settings.Default.MainFormSettings = new WindowSettings();
            }
            Settings.Default.MainFormSettings.Record(this);
            Settings.Default.Save();
        }
#endregion

#region Start page related methods / properties
#endregion

#region Debug tools
        private void OnMenuEditDLL(object sender, EventArgs e)
        {
            // open file dialog
            OpenFileDialog fd = new OpenFileDialog
            {
                Filter = "Component (*.dll)|*.dll|All Files|*.*",
                FilterIndex = 0
            };
            if (DialogResult.OK == fd.ShowDialog())
            {
                // make a copy
                string filePathCopy = Path.ChangeExtension(Path.GetTempFileName(), "dll");
                File.Copy(fd.FileName, filePathCopy, true);
                // form plugin editor
                FormPluginEditor editorForm = new FormPluginEditor
                {
                    PluginPath = filePathCopy,
                    OutputPath = fd.FileName
                };
                if (DialogResult.OK == editorForm.ShowDialog()) { }
                // try and delete copy file
                try { File.Delete(filePathCopy); }
                catch (Exception /*ex*/) { }
            }
        }
#endregion

#region Helpers
        private void OpenWebPage(string pageUrl)
        {
            try
            {
                var procBrowser = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pageUrl,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        Verb = "open",
                        WindowStyle = ProcessWindowStyle.Normal
                    }
                };
                procBrowser.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
#endregion

#region Public properties
        public bool IsWebSiteReachable
        {
            get
            {
                try
                {
                    Uri uri = new Uri(Settings.Default.UrlStartPage);
                    System.Net.IPHostEntry objIPHE = System.Net.Dns.GetHostEntry(uri.DnsSafeHost);
                    return true;
                }
                catch (System.Net.Sockets.SocketException /*ex*/)
                {
                    _log.Info(
                        string.Format(
                        "Url '{0}' could not be accessed : is the computer connected to the web?"
                        , Settings.Default.UrlStartPage
                        ));
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    return false;
                }
            }
        }
        public string AssemblyConf
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyConfigurationAttribute confAttribute = (AssemblyConfigurationAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!string.IsNullOrEmpty(confAttribute.Configuration))
                        return confAttribute.Configuration.ToLower();
                }
                return "release";
            }
        }
        public DockLogConsole LogConsole { get; set; } = new DockLogConsole();
        public DockStartPage FormStartPage { get; set; } = new DockStartPage();
        public DockDownloadPage FormDownload { get; set; } = new DockDownloadPage();
        public static FormMain Instance { get; set; }
#endregion

#region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(FormMain));
        private List<Action> _cleanUp = new List<Action>();
        private MRUManager mruManager;

        #endregion
    }
}