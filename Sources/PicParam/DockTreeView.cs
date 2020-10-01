#region Using directives
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.IO;
// logging
using log4net;
// Dock content
using WeifenLuo.WinFormsUI.Docking;

using Pic.Factory2D;
using Pic.Plugin;
using Pic.Plugin.ViewCtrl;

using Pic.DAL.SQLite;

using DesLib4NET;
//using treeDiM.Processor;

using PicParam.Properties;
//using treeDiM.StackBuilder.GUIExtension;
#endregion

namespace PicParam
{
    public partial class DockTreeView : DockContent
    {
        #region Events
        public delegate void UpdateToolCommands(DockTreeView formTV);
        public event UpdateToolCommands SelectionChanged;
        #endregion

        #region Constructor
        public DockTreeView()
        {
            InitializeComponent();
            // set localizer instance
            pluginViewCtrl.Localizer = LocalizerImpl.Instance;
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // cardboard format loader
            CardboardFormatLoader formatLoader = new CardboardFormatLoaderImpl();
            pluginViewCtrl.CardboardFormatLoader = formatLoader;
            factoryViewCtrl.CardBoardFormatLoader = formatLoader;

            pluginViewCtrl.Localizer = LocalizerImpl.Instance;
            pluginViewCtrl.DependancyStatusChanged += new PluginViewUCtrl.DependancyStatusChangedHandler(_main.DependancyStatusChanged);
            // profile loader
            pluginViewCtrl.ProfileLoader = _profileLoaderImpl;
            // search method
            ComponentSearchMethodDB searchmethod = new ComponentSearchMethodDB();
            pluginViewCtrl.SearchMethod = new ComponentSearchMethodDB();

            treeView.SelectionChanged += new DocumentTreeView.SelectionChangedHandler(branchViewCtrl.OnSelectionChanged);
            treeView.SelectionChanged += new DocumentTreeView.SelectionChangedHandler(OnSelectionChanged);

            branchViewCtrl.SelectionChanged += new BranchViewControl.SelectionChangedHandler(treeView.OnSelectionChanged);
            branchViewCtrl.SelectionChanged += new BranchViewControl.SelectionChangedHandler(OnSelectionChanged);
            branchViewCtrl.TreeNodeCreated += new BranchViewControl.TreeNodeCreatedHandler(treeView.OnTreeNodeCreated);

            // construct tree
            // this line was moved here from the treeview contructor
            // to avoid running this code while in design mode
            treeView.RefreshTree();
            treeView.CollapseRootChildrens();

            SelectionChanged?.Invoke(this);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            try { splitContainer.SplitterDistance = 200; }
            catch (Exception ex) { _log.Error(ex.Message); }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Z:
                    if (factoryViewCtrl.Visible) factoryViewCtrl.FitView();
                    if (pluginViewCtrl.Visible) pluginViewCtrl.FitView();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        #endregion

        #region Control event handlers
        private void OnBranchViewBranchSelected(object sender, NodeEventArgs e)
        {
            treeView.PopulateAndSelectNode(new NodeTag(NodeTag.NType.NTBranch, e.Node));
        }
        private void OnSelectionChanged(object sender, NodeEventArgs e, string name)
        {
            try
            {
                // storing current tag
                TagCurrent = e.NodeTag;
                // change view name (seen in tab)
                Text = TagCurrent.Name;
                // show / hide browsing controls
                branchViewCtrl.Visible = IsBranch;
                pluginViewCtrl.Visible = false;
                factoryViewCtrl.Visible = false;
                webBrowser4PDF.Visible = false;
                unknownFormatViewCtrl.Visible = false;

                if (IsDocument)
                {
                    PPDataContext db = new PPDataContext();
                    Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, e.NodeTag.TreeNode);
                    Document doc = treeNode.Documents(db)[0];

                    // select document handler depending on document type
                    string docTypeName = doc.DocumentType.Name;
                    string filePath = doc.File.Path(db);
                    string fileExt = Path.GetExtension(filePath).Trim('.').ToLower();

                    log4net.Config.XmlConfigurator.Configure();
                    if (!System.IO.File.Exists(filePath))
                        MessageBox.Show($"File {filePath} not found!");
                    else
                        _log.Info($"Loading file {filePath}...");

                    switch (fileExt)
                    {
                        case "dll":
                            LoadComponent(filePath);
                            break;
                        case "des":
                        case "dxf":
                        case "cf2":
                        case "cff2":
                            LoadDrawing(filePath, fileExt);
                            break;
                        case "pdf":
                            LoadPdf(filePath);
                            break;
                        case "png":
                        case "bmp":
                        case "jpg":
                        case "jpeg":
                            LoadImageFile(filePath);
                            break;
                        case "des3":
                        case "doc":
                        case "xls":
                        case "dwg":
                        case "ai":
                        case "sxw":
                        case "stw":
                        case "sxc":
                        case "stc":
                        case "ard":
                        default:
                            LoadUnknownFileFormat(filePath, TagCurrent.Name);
                            break;
                    }
                }
                // update other branch/tree view control
                if (sender != branchViewCtrl) branchViewCtrl.ShowBranch(TagCurrent);
                if (sender != treeView) treeView.SelectNode(TagCurrent);
            }
            catch (ParameterNotFound ex)
            { _log.Error(ex.Message); }
            catch (Exception ex)
            { _log.Error(ex.ToString()); }

            SelectionChanged?.Invoke(this);
        }

        private void OnGeneratePic3D(object sender, EventArgs e)
        {
            Pic.Plugin.Component comp = pluginViewCtrl.Component;
            if (!pluginViewCtrl.Visible || null == comp || !comp.IsSupportingAutomaticFolding)
                return;
            // get documents at the same lavel
            if (!(treeView.SelectedNode.Tag is NodeTag nodeTag)) return;
            PPDataContext db = new PPDataContext();
            Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, nodeTag.TreeNode);
            List<Document> des3Docs = treeNode.GetBrothersWithExtension(db, "des3");
            if (0 == des3Docs.Count)
            { MessageBox.Show("Missing des3 for sample pattern files"); return; }
            List<string> filePathes = new List<string>();
            foreach (Document d in des3Docs)
                filePathes.Add(d.File.Path(db));

            SaveFileDialog fd = new SaveFileDialog
            {
                Filter = "Picador 3D files (*.des3)|*.des3|All files (*.*)|*.*",
                FilterIndex = 0,
                DefaultExt = "des3",
                FileName = treeNode.Name + ".des3"
            };

            if (DialogResult.OK == fd.ShowDialog())
            {
                string des3File = fd.FileName;
                string desFile = Path.ChangeExtension(des3File, "des");
                string xmlFile = Path.ChangeExtension(des3File, "xml");
                string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // generate des file
                Export(desFile);
                // reference point
                double thickness = 0.0;
                Sharp3D.Math.Core.Vector2D v = Sharp3D.Math.Core.Vector2D.Zero;
                if (pluginViewCtrl.GetReferencePointAndThickness(ref v, ref thickness))
                {
                    // #### job file
                    Pic3DExporter.Job job = new Pic3DExporter.Job();
                    // **** FILES BEGIN ****
                    job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-1", path = desFile, type = Pic3DExporter.pathType.FILE });
                    job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = "FID-2", path = des3File, type = Pic3DExporter.pathType.FILE });

                    int fid = 2;
                    foreach (string filePath in filePathes)
                    {
                        job.Pathes.Add(new Pic3DExporter.PathItem() { pathID = string.Format("FID-{0}", ++fid), path = filePath, type = Pic3DExporter.pathType.FILE });
                    }
                    // **** FILES END ****
                    // **** TASKS BEGIN ****
                    // DES -> DES3
                    Pic3DExporter.task_2D_TO_DES3 task_2D_to_DES3 = new Pic3DExporter.task_2D_TO_DES3() { id = "TID-1" };
                    task_2D_to_DES3.Inputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-1", role = "input des", deleteAfterUsing = false });
                    task_2D_to_DES3.Outputs.Add(new Pic3DExporter.PathRef() { pathID = "FID-2", role = "output des3", deleteAfterUsing = false });
                    task_2D_to_DES3.autoparameters.thicknessSpecified = true;
                    task_2D_to_DES3.autoparameters.thickness = (float)thickness;
                    task_2D_to_DES3.autoparameters.foldPositionSpecified = true;
                    task_2D_to_DES3.autoparameters.foldPosition = (float)0.5;
                    task_2D_to_DES3.autoparameters.pointRef.Add((float)v.X);
                    task_2D_to_DES3.autoparameters.pointRef.Add((float)v.Y);
                    fid = 2;
                    foreach (string filePath in filePathes)
                    {
                        task_2D_to_DES3.autoparameters.modelFiles.Add(
                            new Pic3DExporter.PathRef() { pathID = string.Format("FID-{0}", ++fid), role = "model files", deleteAfterUsing = false });
                    }
                    job.Tasks.Add(task_2D_to_DES3);
                    // **** TASKS END ****
                    job.SaveToFile(xmlFile);

                    // #### execute Pic3DExporter
                    string exePath = Pic3DExporterPath;
                    if (!System.IO.File.Exists(exePath))
                    {
                        MessageBox.Show(string.Format("File {0} could not be found!", exePath));
                        return;
                    }

                    var procExporter = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = exePath,
                            Arguments = string.Format(" -t \"{0}\"", xmlFile),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = false,
                            RedirectStandardError = false
                        }
                    };
                    procExporter.Start();
                    procExporter.WaitForExit();
                    Thread.Sleep(1000);

                    // delete xml task file
                    try
                    {
                        if (!Settings.Default.ShowLogConsole)
                            System.IO.File.Delete(xmlFile);
                    }
                    catch (Exception) { }
                }
                if (System.IO.File.Exists(des3File))
                {
                    if (ApplicationAvailabilityChecker.IsAvailable("Picador3D"))
                    {
                        var procPic3D = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = des3File,
                                UseShellExecute = true,
                                CreateNoWindow = false,
                                Verb = "open",
                                WindowStyle = ProcessWindowStyle.Normal
                            }
                        };
                        procPic3D.Start();
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to generate {0}", des3File));
                }
            }
        }

        #endregion

        #region Loading methods
        private void LoadDrawing(string filePath, string fileExt)
        {
            _log.Info($"Loading file {filePath}...");
            if (!System.IO.File.Exists(filePath))
            { 
                _log.Error($"File ({filePath}) does not exist.");
                return;
            }
            factoryViewCtrl.Visible = true;
            PicFactory factory = factoryViewCtrl.Factory;
            switch (fileExt.ToLower())
            {
                case "des":
                    {
                        PicLoaderDes picLoaderDes = new PicLoaderDes(factory);
                        using (DES_FileReader fileReader = new DES_FileReader())
                            fileReader.ReadFile(filePath, picLoaderDes);
                        // remove existing quotations
                        factory.Remove((new PicFilterCode(PicEntity.ECode.PE_COTATIONDISTANCE))
                                            | (new PicFilterCode(PicEntity.ECode.PE_COTATIONHORIZONTAL))
                                            | (new PicFilterCode(PicEntity.ECode.PE_COTATIONVERTICAL)));
                        // build autoquotation
                        PicAutoQuotation.BuildQuotation(factory);
                    }
                    break;
                case "dxf":
                    {
                        using (PicLoaderDXF picLoaderDxf = new PicLoaderDXF(factory))
                        {
                            if (!picLoaderDxf.Load(filePath))
                                return;
                        }
                    }
                    break;
                default:
                    break;
            }
            // update factoryViewCtrl
            factoryViewCtrl.FitView();
        }
        private void LoadComponent(string filePath)
        {
            pluginViewCtrl.PluginPath = filePath;
            pluginViewCtrl.Visible = true;
        }
        private void LoadImageFile(string filePath)
        {
            webBrowser4PDF.Url = new Uri(filePath);
            webBrowser4PDF.Size = splitContainer.Panel2.Size;
            webBrowser4PDF.Visible = true;
            webBrowser4PDF.Invalidate();
        }
        private void LoadPdf(string filePath)
        {
            webBrowser4PDF.Url = new Uri(filePath);
            webBrowser4PDF.Size = splitContainer.Panel2.Size;
            webBrowser4PDF.Visible = true;
            webBrowser4PDF.Invalidate();
        }
        private void LoadUnknownFileFormat(string filePath, string nodeName)
        {
            unknownFormatViewCtrl.Visible = true;
            unknownFormatViewCtrl.FilePath = filePath;
            unknownFormatViewCtrl.NodeName = nodeName;
            unknownFormatViewCtrl.Invalidate();
        }
        #endregion

        #region Database
 
        /// <summary>
        /// Backup
        /// </summary>
        private void OnBackup(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialogBackup.ShowDialog())
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTBackup(saveFileDialogBackup.FileName));
        }
        /// <summary>
        /// Restore
        /// </summary>
        private void OnRestore(object sender, EventArgs e)
        {
            // warning
            if (MessageBox.Show(Resources.ID_RESTOREWARNING
                , ProductName
                , MessageBoxButtons.OKCancel
                , MessageBoxIcon.Warning
                ) == DialogResult.Cancel)
                return;
            if (DialogResult.OK == openFileDialogRestore.ShowDialog())
            {
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTRestore(openFileDialogRestore.FileName));
                // refresh tree
                treeView.RefreshTree();
                // expand all so that modifications can be visualized
                if (treeView.Nodes.Count > 0)
                    treeView.Nodes[0].ExpandAll();
            }
        }
        /// <summary>
        /// Merge
        /// </summary>
        private void OnMerge(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialogRestore.ShowDialog())
            {
                FormWorkerThreadTask.Execute(new Pic.DAL.TPTMerge(openFileDialogRestore.FileName));
                // refresh tree
                treeView.RefreshTree();
                // back to default cursor
                Cursor.Current = Cursors.Default;
            }
        }
        /// <summary>
        /// Clear
        /// </summary>
        private void OnClearDatabase(object sender, EventArgs e)
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
                    // refresh tree
                    treeView.RefreshTree();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(ex.Message));
            }
        }

        private void OnEditDatabasePath(object sender, EventArgs e)
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

        #region Export toolbar event handler
        public void OnExportFile(object sender, EventArgs e)
        {
            try
            {
                // #### actually exported file from _pluginViewCtrl or _factoryViewCtrl
                if (pluginViewCtrl.Visible || factoryViewCtrl.Visible)
                {
                    string fileName = string.Empty;
                    if (treeView.SelectedNode.Tag is NodeTag nodeTag)
                        fileName = nodeTag.Name;

                    SaveFileDialog fd = new SaveFileDialog()
                    {
                        FileName = fileName,
                        Filter = "Autocad dxf|*.dxf|All Files|*.*",
                        FilterIndex = 0,
                        DefaultExt = "dxf"
                    };
                    string fileExtension = "dxf";
                    if (DialogResult.OK == fd.ShowDialog())
                    {
                        if (pluginViewCtrl.Visible)
                            pluginViewCtrl.WriteExportFile(fd.FileName, fileExtension);
                        else if (factoryViewCtrl.Visible)
                        {
                            // at first, attempt to copy original file
                            // rather than exporting _factoryViewCtrl content
                            if ("des" == fileExtension && System.IO.File.Exists(DocumentPath))
                                System.IO.File.Copy(DocumentPath, fd.FileName, true);
                            else
                                factoryViewCtrl.WriteExportFile(fd.FileName, fileExtension);
                        }
                        else if (webBrowser4PDF.Visible)
                        {
                            if (null != treeView.SelectedNode)
                            {
                            }
                        }
                        if (false)
                        {
                            using (Process proc = new Process())
                            {
                                if ("des" == fileExtension
                                    || "dxf" == fileExtension
                                    || "ai" == fileExtension
                                    || "cf2" == fileExtension)
                                {
                                    string applicationPath = string.Empty;
                                    switch (fileExtension)
                                    {
                                        case "des": applicationPath = Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDES; break;
                                        case "dxf": applicationPath = Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDXF; break;
                                        case "ai": applicationPath = Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppAI; break;
                                        case "cf2": applicationPath = Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppCF2; break;
                                        default: break;
                                    }
                                    if (string.IsNullOrEmpty(applicationPath) || !System.IO.File.Exists(applicationPath))
                                        return;
                                    proc.StartInfo.FileName = applicationPath;
                                    proc.StartInfo.Arguments = "\"" + fd.FileName + "\"";
                                }
                                else if ("pdf" == fileExtension)
                                {
                                    proc.StartInfo.FileName = fd.FileName;
                                    // actually using shell execute
                                    proc.StartInfo.UseShellExecute = true;
                                    proc.StartInfo.Verb = "open";
                                }
                                // checking if called application can be found
                                if (!proc.StartInfo.UseShellExecute && !System.IO.File.Exists(proc.StartInfo.FileName))
                                {
                                    MessageBox.Show(string.Format("Application {0} could not be found!"
                                        , proc.StartInfo.FileName)
                                        , Application.ProductName
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Error);
                                    return;
                                }
                                proc.Start();
                            }
                        }
                    }
                }
                // #### actually PDF file
                else if (webBrowser4PDF.Visible)
                {
                    SaveFileDialog fd = new SaveFileDialog
                    {
                        Filter = "Adobe Acrobat Reader (*.pdf)|*.pdf|All Files|*.*",
                        FilterIndex = 0,
                        DefaultExt = "pdf",
                        InitialDirectory = Settings.Default.FileExportDirectory
                    };
                    if (DialogResult.OK == fd.ShowDialog())
                    {
                        string filePath = webBrowser4PDF.Url.AbsolutePath;
                        System.IO.File.Copy(filePath, fd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                _log.Error(ex.ToString());
            }
        }
        public void OnExportPicGEOM(object sender, EventArgs e)
        {
            ExportAndOpenExtension("des", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDES);
        }
        public void OnExportPicDecoup(object sender, EventArgs e)
        {
            ExportAndOpenExtension("des", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppPicDecoupeDES);
        }
        public void OnExportPicador3D(object sender, EventArgs e)
        {
            ExportAndOpenExtension("des", Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppPic3DDES);
        }
        public void OnExportDXF(object sender, EventArgs e)     { OnExportFile(1); }
        public void OnExportCFF2(object sender, EventArgs e)    { OnExportFile(2); }
        public void OnExportAI(object sender, EventArgs e)      { OnExportFile(3); }
        public void OnExportPDF(object sender, EventArgs e)     { OnExportFile(4); }

        private void OnExportFile(int filterIndex)
        {
            string[] exts = new string[]
            {
                "des",
                "dxf",
                "cf2",
                "ai",
                "pdf"
            };
            string[] apps = new string[]
            {
                Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDES,
                Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppDXF,
                Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppCF2,
                Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppAI,
                Pic.Factory2D.Control.Properties.Settings.Default.FileOutputAppPDF
            };
            string filter = "Picador (*.des)|*.des|" +
                "Autodesk (*.dxf)|*.dxf|" +
                "Common file format (*.cff2)|*.cff2|" +
                "Adobe Illustrator (*.ai)|*.ai|" +
                "Adobe Reader (*.pdf)|*.pdf|" + 
                "All files (*.*)|*.*";

            string applicationPath = apps[filterIndex];
            string fileExt = exts[filterIndex];
            if (!string.IsNullOrEmpty(applicationPath) && System.IO.File.Exists(applicationPath))
                ExportAndOpenExtension(fileExt, applicationPath);
            else
            {
                // set default file path
                string exportDir = Settings.Default.FileExportDirectory;
                if (string.IsNullOrEmpty(exportDir)) exportDir = Path.GetDirectoryName(Path.GetTempFileName());
                string filePath = Path.Combine(exportDir, DocumentName);
                filePath = Path.ChangeExtension(filePath, fileExt);
                filePath.Replace('*', '_');
                // initialize SaveFileDialog
                SaveFileDialog fd = new SaveFileDialog
                {
                    FileName = filePath,
                    Filter = filter,
                    FilterIndex = filterIndex
                };
                // show save file dialog
                if (DialogResult.OK == fd.ShowDialog())
                    ExportAndOpen(fd.FileName, string.Empty);
                // save directory
                Settings.Default.FileExportDirectory = Path.GetDirectoryName(fd.FileName);
            }
        }
        public void Export(string filePath)
        {
            // write file
            if (pluginViewCtrl.Visible)
                pluginViewCtrl.WriteExportFile(filePath, Path.GetExtension(filePath));
            else if (factoryViewCtrl.Visible)
                factoryViewCtrl.WriteExportFile(filePath, Path.GetExtension(filePath));        
        }
        private void ExportAndOpen(string filePath, string sPathExectable)
        {
            // write file
            if (pluginViewCtrl.Visible)
                pluginViewCtrl.WriteExportFile(filePath, Path.GetExtension(filePath));
            else if (factoryViewCtrl.Visible)
                factoryViewCtrl.WriteExportFile(filePath, Path.GetExtension(filePath));

            // open using existing file path
            try
            {
                using (Process proc = new Process())
                {
                    // test if executing application is available
                    if (!string.IsNullOrEmpty(sPathExectable) && System.IO.File.Exists(sPathExectable))
                    {
                        proc.StartInfo.FileName = sPathExectable;
                        proc.StartInfo.Arguments = "\"" + filePath + "\"";
                    }
                    else // no valid executable path -> attempting shell execute
                    {
                        proc.StartInfo.FileName = filePath;
                        proc.StartInfo.Verb = "open";
                        proc.StartInfo.UseShellExecute = true;
                    }
                    // start process
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                _log.WarnFormat("Failed to open file {0} with exception: {1}", filePath, ex.Message);
            }
        }
        private void ExportAndOpenExtension(string sExt, string sPathExectable)
        {
            // build temp file path
            string tempFilePath = Path.ChangeExtension(Path.GetTempFileName(), sExt);
            ExportAndOpen(tempFilePath, sPathExectable);
        }
        private void ToolStripButtonPDF3D_Click(object sender, EventArgs e)
        {
            Pic.Plugin.Component comp = pluginViewCtrl.Component;
            if (!pluginViewCtrl.Visible || null == comp || !comp.IsSupportingAutomaticFolding)
                return;
            // get documents at the same lavel
            NodeTag nodeTag = treeView.SelectedNode.Tag as NodeTag;
            if (null == nodeTag) return;
            PPDataContext db = new PPDataContext();
            Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, nodeTag.TreeNode);
            List<Document> des3Docs = treeNode.GetBrothersWithExtension(db, "des3");
            if (0 == des3Docs.Count)
            {   MessageBox.Show("Missing des3 for sample pattern files");   return; }
            // get default thickness
            Sharp3D.Math.Core.Vector2D refPoint = Sharp3D.Math.Core.Vector2D.Zero;
            double thickness = 0.0;
            if (!pluginViewCtrl.GetReferencePointAndThickness(ref refPoint, ref thickness))
            {
                MessageBox.Show("Failed to retrieve reference point and thickness");
                return;
            }
            // allow
            if (!System.IO.File.Exists(Pic3DExporterPath))
            {
                MessageBox.Show(string.Format("File {0} could not be found!", Pic3DExporterPath));
                return;
            }
            // show form and generate PDF 3D
            FormGeneratePDF3D form = new FormGeneratePDF3D(this)
            {
                OutputFilePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), treeNode.Name), "pdf"),
                thickness = thickness,
                v = refPoint
            };
            foreach (Document d in des3Docs)
                form.filePathes.Add(d.File.Path(db));
            form.ShowDialog();
        }
        #endregion

        #region Helpers
        public void SetMain(IMain main)
        {
            _main = main;
        }
        public void ResetTree()
        {
        }
        public IToolInterface CurrentTool
        {
            get
            {
                if (pluginViewCtrl.Visible)
                    return pluginViewCtrl;
                else if (factoryViewCtrl.Visible)
                    return factoryViewCtrl;
                else
                    return null;
            }
        }
        #endregion

        #region Public properties
        public string Pic3DExporterPath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "..\\Bin10_2D\\Pic3DExporter.exe");
        public bool TSButtonsEnabled => pluginViewCtrl.Visible || factoryViewCtrl.Visible;
        public bool AllowPalletisation => pluginViewCtrl.Visible && pluginViewCtrl.AllowPalletization;
        public bool AllowFlatPalletisation => pluginViewCtrl.Visible && pluginViewCtrl.AllowFlatPalletization;
        #endregion

        #region Public accessors
        public bool ShowCotationsAuto
        {
            get
            {
                IToolInterface currentView = CurrentTool;
                if (null == currentView) return false;
                return currentView.ShowCotationsAuto;
            }
        }
        public bool ShowCotationsCode
        {
            get
            {
                IToolInterface currentView = CurrentTool;
                if (null == currentView) return false;
                return currentView.ShowCotationsCode;
            }
        }
        public bool ShowAxes
        {
            get
            {
                IToolInterface currentView = CurrentTool;
                if (null == currentView) return false;
                return currentView.ShowAxes;
            }
        }
        private string DocumentName => TagCurrent.Name;
        #endregion

        #region Private properties
        private string DocumentPath { get; set; }
        private Document SelectedDocument
        {
            get
            {
                if (!(treeView.SelectedNode.Tag is NodeTag nodeTag))
                    return null;

                PPDataContext db = new PPDataContext();
                Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, nodeTag.TreeNode);
                if (null == treeNode)
                    return null;    // failed to retrieve valid document
                if (!treeNode.IsDocument)
                    return null;    // not a document
                return treeNode.Documents(db)[0];
            }
        }
        #endregion

        #region Menu handlers
        public void OnShowHideCotationsAuto(object sender, EventArgs e)
        {
            IToolInterface currentView = CurrentTool;
            if (null == currentView) return;
            currentView.ShowCotationsAuto = !currentView.ShowCotationsAuto;
            SelectionChanged?.Invoke(this);
        }
        public void OnShowHideCotationsCode(object sender, EventArgs e)
        {
            IToolInterface currentView = CurrentTool;
            if (null == currentView) return;
            currentView.ShowCotationsCode = !currentView.ShowCotationsCode;
            SelectionChanged?.Invoke(this);
        }
        public void OnShowHideAxes(object sender, EventArgs e)
        {
            IToolInterface currentView = CurrentTool;
            if (null == currentView) return;
            currentView.ShowAxes = !currentView.ShowAxes;
            SelectionChanged?.Invoke(this);
        }
        public void OnToolStripReflectionX(object sender, EventArgs e)
        {
            IToolInterface currentView = CurrentTool;
            if (null == currentView) return;
            currentView.ReflectionX = !currentView.ReflectionX;
            SelectionChanged?.Invoke(this);
        }
        public void OnToolStripReflectionY(object sender, EventArgs e)
        {
            IToolInterface currentView = CurrentTool;
            if (null == currentView) return;
            currentView.ReflectionY = !currentView.ReflectionY;
            SelectionChanged?.Invoke(this);
        }
        public void OnToolStripLayout(object sender, EventArgs e)
        {
            try
            {
                if (pluginViewCtrl.Visible)
                    pluginViewCtrl.BuildLayout();
                else if (factoryViewCtrl.Visible)
                    factoryViewCtrl.BuildLayout();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripEditParameters(object sender, EventArgs e)
        {
            try
            {
                FormEditDefaultParamValues form = new FormEditDefaultParamValues(
                    pluginViewCtrl.Dependancies);
                if (DialogResult.OK == form.ShowDialog())
                {   // also see on Ok button handler
                    // clear plugin loader cache
                    ComponentLoader.ClearCache();

                    if (pluginViewCtrl.Visible)
                        pluginViewCtrl.Refresh();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripEditComponentCode(object sender, EventArgs e)
        {
            try
            {
                if (!(treeView.SelectedNode.Tag is NodeTag nodeTag))
                    return;
                PPDataContext db = new PPDataContext();
                Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, nodeTag.TreeNode);
                if (null == treeNode) return;
                Document doc = treeNode.Documents(db)[0];
                if (null == doc) return;
                Pic.DAL.SQLite.Component comp = doc.Components[0];
                if (null == comp) return;
                // output Guid / path
                Guid outputGuid = Guid.NewGuid();
                string outputPath = Pic.DAL.SQLite.File.GuidToPath(db, outputGuid, "dll");
                // form plugin editor
                FormPluginEditor editorForm = new FormPluginEditor();
                editorForm.PluginPath = doc.File.Path(db);
                editorForm.OutputPath = outputPath;
                if (DialogResult.OK == editorForm.ShowDialog())
                {
                    _log.Info("Component successfully modified!");
                    doc.File.Guid = outputGuid;
                    db.SubmitChanges();
                    // clear component cache in plugin viewer
                    ComponentLoader.ClearCache();
                    // update pluginviewer
                    pluginViewCtrl.PluginPath = outputPath;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripButtonPostProcessor(object sender, EventArgs e)
        {
            try
            {
                // set default file path
                string filePath = Path.Combine(Settings.Default.FileExportDirectory, DocumentName);
                filePath = Path.ChangeExtension(filePath, "ai");
                // initialize SaveFileDialog
                SaveFileDialog fd = new SaveFileDialog
                {
                    FileName = filePath,
                    Filter = "Adobe Illustrator (*.ai)|*.ai|All Files|*.*",
                    FilterIndex = 0
                };
                // show save file dialog
                if (DialogResult.OK == fd.ShowDialog())
                    ExportAndOpen(fd.FileName, string.Empty);
                // save directory
                Settings.Default.FileExportDirectory = Path.GetDirectoryName(fd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#if PROCESSOR
        public void OnToolStripButtonShowProcessor(object sender, EventArgs e)
        {
            try
            {
                string processorName = string.Empty;
                if (sender is ToolStripButton toolStrip)
                {
                    switch (toolStrip.Name)
                    {
                        case "toolStripButtonARISTO": processorName = "ARISTO"; break;
                        case "toolStripButtonSUMMA": processorName = "SUMMA"; break;
                        case "toolStripButtonZUND": processorName = "ZUND"; break;
                        case "toolStripButtonOceProCut": processorName = "OCEPROCUT"; break;
                        default: break;
                    }
                }
                PicFactory factory = null;
                if (pluginViewCtrl.Visible)
                    factory = pluginViewCtrl.GetFactory();
                else if (factoryViewCtrl.Visible)
                    factory = factoryViewCtrl.Factory;
                Generator.ShowProcessor(factory, DocumentName, processorName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#endif
        public void OnToolStripButtonRoot(object sender, EventArgs e)
        {
            try { treeView.CollapseRoot(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void OnToolStripButtonSearch(object sender, EventArgs e)
        {
            try
            {
                FormSearch form = new FormSearch();
                if (DialogResult.OK == form.ShowDialog())
                    treeView.PopulateAndSelectNode( new NodeTag(NodeTag.NType.NTBranch, form.ResultNodeId) );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
#if StackBuilder
        public void OnToolStripStackBuilderCasePallet(object sender, EventArgs e)
        {
            try
            {
                double length = 0.0, width = 0.0, height = 0.0;
                if (pluginViewCtrl.GetDimensions(ref length, ref width, ref height))
                    Palletization.StartCasePalletization(pluginViewCtrl.LoadedComponentName, length: length, width: width, height: height);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripStackBuilderCaseOptimization(object sender, EventArgs e)
        {
            try
            {
                double length = 0.0, width = 0.0, height = 0.0;
                if (pluginViewCtrl.GetDimensions(ref length, ref width, ref height))
                    Palletization.StartCaseOptimization(pluginViewCtrl.LoadedComponentName, length, width, height);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripStackBuilderBundlePallet(object sender, EventArgs e)
        {
            try
            {
                double length = 0.0, width = 0.0, height = 0.0;
                if (pluginViewCtrl.GetFlatDimensions(length: ref length, width: ref width, height: ref height))
                    Palletization.StartBundlePalletAnalysis(pluginViewCtrl.LoadedComponentName, length, width, height);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public void OnToolStripStackBuilderBundleCase(object sender, EventArgs e)
        {
            try
            {
                double length = 0.0, width = 0.0, height = 0.0;
                if (pluginViewCtrl.GetFlatDimensions(length: ref length, width: ref width, height: ref height))
                    Palletization.StartAnalysisBundleCase(pluginViewCtrl.LoadedComponentName, length, width, height);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
#endif
#endregion

        #region Current tag
        private NodeTag TagCurrent { get; set; }
        private bool IsDocument
        {
            get
            {
                if (null == TagCurrent) return false;
                else return TagCurrent.IsDocument;
            }
        }
        private bool IsBranch
        {
            get
            {
                if (null == TagCurrent) return false;
                else return TagCurrent.IsBranch;
            }
        }
        private bool IsComponent
        {
            get
            {
                if (null == TagCurrent) return false;
                else return TagCurrent.IsComponent;
            }
        }
        private bool IsEditable => true;
        #endregion
    
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(DockTreeView));
        private IMain _main;
        private ProfileLoaderImpl _profileLoaderImpl = new ProfileLoaderImpl();
        #endregion
    }
}
