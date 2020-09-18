#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Pic.DAL;

using log4net;
#endregion

namespace PicParam
{
    public partial class DocumentTreeView
        : TreeView
        , Pic.DAL.SQLite.TreeInterface
    {

        #region Constructor
        public DocumentTreeView()
        {
            try
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentTreeView));
                // get image for tree
                ImageList = new ImageList();
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(GetType());
                ImageList.Images.Add((Image)(resources.GetObject("CLSDFOLD"))); // 0
                ImageList.Images.Add((Image)(resources.GetObject("OPENFOLD"))); // 1
                ImageList.Images.Add((Image)(resources.GetObject("COMPONENT"))); // 2
                ImageList.Images.Add((Image)(resources.GetObject("DOC"))); // 3
                ImageList.Images.Add((Image)(resources.GetObject("DXF"))); // 4
                ImageList.Images.Add((Image)(resources.GetObject("PDF"))); // 5
                ImageList.Images.Add((Image)(resources.GetObject("STARTPAGE"))); // 6
                ImageList.Images.Add((Image)(resources.GetObject("DOWNLOAD"))); // 7
                ImageList.Images.Add((Image)(resources.GetObject("AI"))); // 8
                ImageList.Images.Add((Image)(resources.GetObject("IMAGE"))); // 9
                ImageList.Images.Add((Image)(resources.GetObject("MSWORD"))); // 10
                ImageList.Images.Add((Image)(resources.GetObject("MSEXCEL"))); // 11
                ImageList.Images.Add((Image)(resources.GetObject("MSPPT"))); // 12
                ImageList.Images.Add((Image)(resources.GetObject("WRITER"))); // 13
                ImageList.Images.Add((Image)(resources.GetObject("CALC"))); // 14
                ImageList.Images.Add((Image)(resources.GetObject("ARD"))); // 15
                ImageList.Images.Add((Image)(resources.GetObject("ROOT"))); // 16
                ImageList.Images.Add((Image)(resources.GetObject("PICADOR3D"))); // 17

                // events
                AfterExpand += new TreeViewEventHandler(DocumentTreeView_AfterExpand);
                AfterSelect += new TreeViewEventHandler(DocumentTreeView_AfterSelect);

                MouseDown += new MouseEventHandler(DocumentTreeView_MouseDown);

                AllowDrop = true;
                ItemDrag += new ItemDragEventHandler(OnItemDrag);
                DragEnter += new DragEventHandler(OnDragEnter);
                DragOver += new DragEventHandler(OnItemDragOver);
                DragDrop += new DragEventHandler(OnDragDrop);
                NodeDropped += new NodeDroppedHandler(OnNodeDropped);

                // show tool tips
                ShowNodeToolTips = true;
                // allow drag and drop
                AllowDrop = true;
            }
            catch (Pic.DAL.SQLite.ExceptionDAL ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }
        #endregion

        #region Drag and drop handling
        void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            // move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                _draggedNode = e.Item as TreeNode;
                if (null != _draggedNode)
                    DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        void OnDragDrop(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // retrieve the client coordinates of the drop location.
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            // retrieve the node at the drop location.
            TreeNode dropTargetNode = GetNodeAt(targetPoint);
            if (null != dropTargetNode)
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                NodeTag tagTo = dropTargetNode.Tag as NodeTag;
                Pic.DAL.SQLite.TreeNode nodeTo = Pic.DAL.SQLite.TreeNode.GetById(db, tagTo.TreeNode);
                if (!nodeTo.IsDocument)
                    NodeDropped(this, new NodeDroppedArgs(_draggedNode, dropTargetNode));
            }
        }

        void OnItemDragOver(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // retrieve the client coordinates of the mouse position.
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = GetNodeAt(targetPoint);
            if (null != targetNode)
            {
                NodeTag tagTo = targetNode.Tag as NodeTag;
                if (!tagTo.IsDocument)
                {
                    // select the node at the mouse position.
                    SelectedNode = GetNodeAt(targetPoint);
                }
            }
        }

        void OnDragEnter(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // Set the target drop effect to the effect 
            // specified in the ItemDrag event handler.
            e.Effect = e.AllowedEffect;
        }

        void OnNodeDropped(object sender, NodeDroppedArgs args)
        {
            try
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                // get data treenodes
                NodeTag tagFrom = args._treeNodeFrom.Tag as NodeTag;
                Pic.DAL.SQLite.TreeNode nodeFrom = Pic.DAL.SQLite.TreeNode.GetById(db, tagFrom.TreeNode);
                NodeTag tagTo = args._treeNodeTo.Tag as NodeTag;
                Pic.DAL.SQLite.TreeNode nodeTo = Pic.DAL.SQLite.TreeNode.GetById(db, tagTo.TreeNode);

                // check that nodeTo is not a descendant of nodeFrom
                if (nodeTo.IsDescendantOf(db, nodeFrom))
                {
                    MessageBox.Show(string.Format(Properties.Resources.ID_TREEWARNING_CANNOTMOVENODE, nodeFrom.Name, nodeTo.Name)
                        , Application.ProductName
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                    return;
                }

                // confirm ?
                if (DialogResult.Yes == MessageBox.Show(
                    string.Format(Properties.Resources.ID_TREEWARNING_MOVENODE, nodeFrom.Name, nodeTo.Name)
                    , Application.ProductName
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Warning))
                {
                    // move node in the database
                    nodeFrom.Move(db, nodeTo);
                    // rebuild tree
                    Pic.DAL.SQLite.TreeBuilder builder = new Pic.DAL.SQLite.TreeBuilder();
                    builder.BuildTree(this);
                }
            }
            catch (Pic.DAL.SQLite.ExceptionDAL ex)
            {
                MessageBox.Show(ex.ToString());
                _log.Error(string.Format("Exception caught : {0}", ex.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _log.Error(string.Format("Exception caught : {0}", ex.ToString()));
            }
        }
        #endregion

        #region Pic.DAL.SQLite.TreeInterface implementation
        public void Initialize()
        {
            // turn off visual updating
            BeginUpdate();
            // clear tree
            Nodes.Clear();
        }
        public void Finish()
        {
            // turn on visual updating
            EndUpdate();
        }
        public object InsertTreeNode(object parentObj, Pic.DAL.SQLite.TreeNode node)
        {
            TreeNode treeNodeNew = new TreeNode(node.Name, GetImageIndex(node), GetSelectedImageIndex(node))
            {
                Tag = new NodeTag(
                node.IsDocument ? NodeTag.NType.NTDocument : NodeTag.NType.NTBranch
                , node.ID
                , node.Name
                , node.Description
                , node.Thumbnail.GetImage()),
                ToolTipText = node.Description
            };
            if (node.IsDocument)
                treeNodeNew.ContextMenuStrip = node.IsComponent ? GetComponentMenu() : GetLeafMenu();
            else
                treeNodeNew.ContextMenuStrip = GetBranchMenu();
            TreeNode treeNodeParent = parentObj as TreeNode;
            
            if (null == treeNodeParent)
                Nodes.Add(treeNodeNew);
            else
            {
                // find index
                int index = 0;
                foreach (TreeNode tn in treeNodeParent.Nodes)
                {
                    if (!(tn.Tag is NodeTag tag)) continue;
                    if (string.Compare(tn.Text, node.Name) == -1)
                        ++index;
                }
                treeNodeParent.Nodes.Insert(index, treeNodeNew);
            }
            return treeNodeNew;
        }

        public void InsertDummyNode(object parentObj)
        {
            TreeNode treeNodeParent = parentObj as TreeNode;
            TreeNode dummyTreeNode = new TreeNode("_DUMMY_", 0, 0);
            treeNodeParent.Nodes.Add(dummyTreeNode);
        }

        public void AskRootNode()
        {
            FormCreateBranch form = new FormCreateBranch();
            if (DialogResult.OK == form.ShowDialog())
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode.CreateNew(db, form.BranchName, form.BranchDescription, form.BranchImage);
            }
        }
        #endregion

        #region Event handlers
        public void CollapseRoot()
        {
            if (Nodes.Count > 0 && string.Equals(Nodes[0].Text, "Root", StringComparison.CurrentCultureIgnoreCase))
            {
                var nodeRoot = Nodes[0];
                nodeRoot.Nodes.Clear();
                nodeRoot.Nodes.Add(new TreeNode("_DUMMY_", 0, 0));
                nodeRoot.Collapse();

                var rootTag = nodeRoot.Tag as NodeTag;

                SelectNode(rootTag);
                SelectionChanged?.Invoke(this, new NodeEventArgs(rootTag.TreeNode, rootTag.Type), rootTag.Name);
            }
        }
        /// <summary>
        /// Collapse all nodes
        /// except the Root node itself
        /// </summary>
        public void CollapseRootChildrens()
        {
            // collapse each child of the "Root" node
            if (Nodes.Count > 1 && 0 == string.Compare(Nodes[2].Text, "Root"))
            {
                // root node
                TreeNode rootNode = Nodes[2];
                // collapse each node
                foreach (TreeNode tn in rootNode.Nodes)
                    tn.Collapse();
                // expand root node only
                rootNode.Expand();
                // select root node
                if (null != rootNode)
                    SelectedNode = rootNode;
            }
        }

        private void DocumentTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Nodes.Count == 1 && 0 == string.Compare(e.Node.Nodes[0].Text, "_DUMMY_"))
                {   // this node has not yet been populated
                    bool singlethreaded = true;
                    if (singlethreaded)
                        PopulateChildren(e.Node);
                    else // multithreaded
                    {
                        // launch a thread to get the data
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            // load the data into the tree view (on the UI thread)
                            BeginInvoke((Action)delegate
                            {
                                PopulateChildren(e.Node);
                            });
                        });
                    }
                }
                // select node
                SelectedNode = e.Node;
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }

        private void PopulateChildren(TreeNode parent)
        {
            // remove _DUMMY_ tree node
            parent.Nodes.Clear();
            // populate with actual nodes 
            NodeTag tag = parent.Tag as NodeTag;
            Pic.DAL.SQLite.TreeBuilder treeBuilder = new Pic.DAL.SQLite.TreeBuilder();
            treeBuilder.PopulateChildren(this, parent, tag.TreeNode);
        }

        void DocumentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                NodeTag currentTag = GetCurrentTag();
                if (null == currentTag)
                {
                    StartPageTag startPageTag = e.Node.Tag as StartPageTag;
                    if (null != StartPageSelected && null != startPageTag)
                        StartPageSelected(this);
                    DownloadPageTag downloadPageTag = e.Node.Tag as DownloadPageTag;
                    if (null != DownloadPageSelected && null != downloadPageTag)
                        DownloadPageSelected(this);
                }
                else if (!_preventEventTriggering)
                    SelectionChanged?.Invoke(this, new NodeEventArgs(currentTag.TreeNode, currentTag.Type), currentTag.Name);
            }
            catch (Pic.Plugin.PluginException /*ex*/)
            {
                // do nothing
                // -> users sees message on plugin viewer control
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void DocumentTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode tn = GetNodeAt(e.Location);
                SelectedNode = tn;
            }
        }
        #endregion

        #region Handlers
        public void OnSelectionChanged(object sender, NodeEventArgs e, string name)
        {
            PopulateAndSelectNode(new NodeTag(e.Type, e.Node)); 
        }
        public void OnTreeNodeCreated(object sender, NodeEventArgs e)
        {
            Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
            Pic.DAL.SQLite.TreeNode nodeNew = Pic.DAL.SQLite.TreeNode.GetById(db, e.Node);
            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NType.NTBranch, nodeNew.ParentNodeID.Value));
            // clear
            parentNode.Nodes.Clear();
            // insert dummy node
            parentNode.Nodes.Add("_DUMMY_");
            // collapse
            parentNode.Collapse();
            // reload
            parentNode.Expand();
        }
        #endregion

        #region Helpers
        internal NodeTag GetCurrentTag()
        {
            TreeNode currentNode = SelectedNode;
            if (null == currentNode)
                throw new Exception("No node selected");
            return currentNode.Tag as NodeTag;
        }
        #endregion

        #region Tree filling methods
        public void RefreshTree()
        {
            // do not attempt to build tree when in design mode
            if (DesignMode) return;
            try
            {
                Pic.DAL.SQLite.TreeBuilder treeBuilder = new Pic.DAL.SQLite.TreeBuilder();
                treeBuilder.BuildTree(this);

                if (Nodes.Count > 0)
                    SelectedNode = Nodes[0];
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception caught: {0}", ex.ToString()));
            }
        }

        public void SelectNode(NodeTag tag)
        {
            SelectedNode = FindNode(null, tag);
            if (null != SelectedNode)
                SelectedNode.EnsureVisible();
        }

        public void PopulateAndSelectNode(NodeTag tag)
        {
            TreeNode nodeSelected = FindNode(null, tag);
            BeginUpdate();  // this disables redrawing of tree
            if (null == nodeSelected)
            {
                _log.Error(string.Format("Failed to retrieve valid node from NodeTag {0}", tag.ToString()));
                // retrieve db node
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);
                // insert all needed parents and node
                nodeSelected = InsertNodeAndParents(tn, db);
                if (null == nodeSelected)
                {
                    _log.Error(string.Format("Failed to retrieve valid node from NodeTag {0}", tag.ToString()));
                    return; // -> complete failure
                }
            }
            if (null != nodeSelected)
            {
                _preventEventTriggering = true;
                // ################## --> needed to update tree
                nodeSelected.Expand();
                // ################## --> needed to update tree
                SelectedNode = nodeSelected;
                EndUpdate(); // enables the redrawing of the treeview
                _preventEventTriggering = false;
                SelectedNode = nodeSelected;
            }
            else
                EndUpdate(); // enables the redrawing of the treeview
        }

        // recursive insertion mathod
        public TreeNode InsertNodeAndParents(Pic.DAL.SQLite.TreeNode tn, Pic.DAL.SQLite.PPDataContext db)
        {
            // failed !
            if (null == tn)  return null;

            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NType.NTBranch, tn.ParentNodeID.Value));
            if (null == parentNode)
                parentNode = InsertNodeAndParents(Pic.DAL.SQLite.TreeNode.GetById(db, tn.ParentNodeID.Value), db);
            Pic.DAL.SQLite.TreeBuilder treeBuilder = new Pic.DAL.SQLite.TreeBuilder();
            // remove _DUMMY_ tree node
            parentNode.Nodes.Clear();
            treeBuilder.PopulateChildren(this, parentNode, tn.ParentNodeID.Value);

            return FindNode(null, new NodeTag(NodeTag.NType.NTBranch, tn.ID));
        }

        public TreeNode FindNode(TreeNode node, NodeTag tag)
        {
            // check with node itself
            if (null != node)
            {
                NodeTag tagNode = node.Tag as NodeTag;
                if (null != tagNode && tagNode.Equals(tag))
                    return node;
            }
            // check with child nodes
            TreeNodeCollection tnCollection = null == node ? Nodes : node.Nodes;
            foreach (TreeNode tn in tnCollection)
            {
                TreeNode tnResult = FindNode(tn, tag);
                if (null != tnResult)
                    return tnResult;
            }
            return null;
        }

        protected int GetImageIndex(Pic.DAL.SQLite.TreeNode tn)
        {
            if (tn.IsDocument)
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.Document doc = tn.Documents(db)[0];
                string docTypeName = doc.DocumentType.Name.ToLower();

                Dictionary<string, int> format2iconDictionnary = new Dictionary<string, int>()
                {
                    {"parametric component" , 2},
                    {"treedim des"          , 3},
                    {"autodesk dxf"         , 4},
                    {"adobe acrobat"        , 5},
                    {"adobe illustrator"    , 8},
                    {"raster image"         , 9},
                    {"ms word"              , 10},
                    {"ms excel"             , 11},
                    {"ms powerpoint"        , 12},
                    {"open office write"    , 13},
                    {"open office calc"     , 14},
                    {"artioscad"            , 15},
                    {"treedim des3"         , 17}
                };

                if (format2iconDictionnary.ContainsKey(docTypeName))
                    return format2iconDictionnary[docTypeName];
                else
                    return 0;
            }
            else
            {
                if (string.Equals(tn.Name, "Root", StringComparison.CurrentCultureIgnoreCase))
                    return 16; // Root
                else
                    return 0;
            }
        }
        protected int GetSelectedImageIndex(Pic.DAL.SQLite.TreeNode tn)
        {
            if (tn.IsDocument)
                return GetImageIndex(tn);
            else
            {
                if (string.Equals(tn.Name, "Root", StringComparison.CurrentCultureIgnoreCase))
                    return 16; // Root
                else
                    return 1;
            }
        }
        #endregion

        #region Context menu creation
        protected ContextMenuStrip GetBranchMenu()
        {
            if (null == _branchMenu)
            {
                // create the ContextMenuStrip
                _branchMenu = new ContextMenuStrip();
                // create some menu items
                // -- insert new branch
                ToolStripMenuItem tsInsertNewBranch = new ToolStripMenuItem
                {
                    Text = Properties.Resources.ID_TREEMENUNEWBRANCH
                };
                tsInsertNewBranch.Click += new EventHandler(OnInsertNewBranch);
                // -- insert new document
                ToolStripMenuItem tsInsertNewDocument = new ToolStripMenuItem() { Text = Properties.Resources.ID_TREEMENUNEWDOCUMENT };
                tsInsertNewDocument.Click += new EventHandler(OnInsertNewDocument);
                // -- insert new component
                ToolStripMenuItem tsInsertNewComponent = new ToolStripMenuItem() { Text = Properties.Resources.ID_TREEMENUNEWCOMPONENT };
                tsInsertNewComponent.Click += new EventHandler(OnInsertNewComponent);
                // -- separator 1
                ToolStripSeparator separator1 = new ToolStripSeparator();
                // -- backup branch
                ToolStripMenuItem backupBranchLabel = new ToolStripMenuItem
                {
                    Text = string.Format(Properties.Resources.ID_TREEMENUBACKUP, "")
                };
                backupBranchLabel.Click += new EventHandler(BackupNode);
                // -- separator 2
                ToolStripSeparator separator2 = new ToolStripSeparator();
                // -- rename branch
                ToolStripMenuItem renameBranchLabel = new ToolStripMenuItem
                {
                    Text = string.Format(Properties.Resources.ID_TREEMENURENAME, "")
                };
                renameBranchLabel.Click += new EventHandler(RenameNode);
                // -- separator 3
                ToolStripSeparator separator3 = new ToolStripSeparator();
                // -- delete branch
                ToolStripMenuItem deleteLabel = new ToolStripMenuItem
                {
                    Text = Properties.Resources.ID_TREEMENUDELETE
                };
                deleteLabel.Click += new EventHandler(DeleteNode);

                // add the menu items to the menu.
                _branchMenu.Items.AddRange(
                    new ToolStripItem[]{
                        tsInsertNewBranch
                        , tsInsertNewDocument
                        , tsInsertNewComponent
                        , separator1
                        , backupBranchLabel
                        , separator2
                        , renameBranchLabel
                        , separator3
                        , deleteLabel
                    }
                    );
            }
            return _branchMenu;
        }

        protected ContextMenuStrip GetLeafMenu()
        {
            if (null == _leafMenu)
            {
                // create the ContextMenuStrip
                _leafMenu = new ContextMenuStrip();
                // create some menu items
                // *** -> rename
                ToolStripMenuItem renameBranchLabel = new ToolStripMenuItem
                {
                    Text = string.Format(Properties.Resources.ID_TREEMENURENAME, "")
                };
                renameBranchLabel.Click += new EventHandler(RenameNode);
                // *** -> separator1
                ToolStripSeparator separator1 = new ToolStripSeparator();
                // *** -> copy
                ToolStripMenuItem duplicateLabel = new ToolStripMenuItem { Text = Properties.Resources.ID_TREEMENUCOPY };
                duplicateLabel.Click += new EventHandler(DuplicateLabel_Click);
                // *** -> delete
                ToolStripMenuItem deleteLabel = new ToolStripMenuItem { Text = Properties.Resources.ID_TREEMENUDELETE };
                deleteLabel.Click += new EventHandler(DeleteNode);
                // ***
                // add the menu items to the menu.
                _leafMenu.Items.AddRange(
                    new ToolStripItem[]{
                        renameBranchLabel
                        , separator1
                        , duplicateLabel
                        , deleteLabel
                    }
                    );
                // DEBUG MODE
                if (Properties.Settings.Default.ShowLogConsole)
                {
                    // *** -> separator 2
                    ToolStripSeparator separator2 = new ToolStripSeparator();
                    // *** -> send path to clipboard
                    ToolStripMenuItem menuItemSendPathToClipboard = new ToolStripMenuItem
                    { Text = Properties.Resources.ID_TREEMENUSENDPATHTOCLIPBOARD };
                    menuItemSendPathToClipboard.Click += new EventHandler(SendPathToClipboard);
                    // *** -> select in windows explorer
                    ToolStripMenuItem menuItemSelectInWindowsExplorer = new ToolStripMenuItem
                    { Text = Properties.Resources.ID_TREEMENUSELECTINEXPLORER };
                    menuItemSelectInWindowsExplorer.Click += new EventHandler(SelectInWindowsExplorer);
                    // *** 
                    // add the menu items to the menu.
                    _leafMenu.Items.AddRange(new ToolStripItem[]{
                        separator2
                        , menuItemSendPathToClipboard
                        , menuItemSelectInWindowsExplorer
                    }
                    );
                    // -> modify GUID
                    ToolStripMenuItem menuItemModifyGUID = new ToolStripMenuItem { Text = Properties.Resources.ID_TREEMENUSETNEWGUID };
                    menuItemModifyGUID.Click += new EventHandler(SetNewComponentGuid);
                }
            }
            return _leafMenu;
        }

        protected ContextMenuStrip GetComponentMenu()
        {
            if (null == _componentMenu)
            { 
                // create the ContextMenuStrip
                _componentMenu = new ContextMenuStrip();
                // create some menu items
                // *** -> rename
                ToolStripMenuItem renameBranchLabel = new ToolStripMenuItem
                { Text = string.Format(Properties.Resources.ID_TREEMENURENAME, "") };
                renameBranchLabel.Click += new EventHandler(RenameNode);
                // *** -> separator1
                ToolStripSeparator separator1 = new ToolStripSeparator();
                // *** -> copy
                ToolStripMenuItem duplicateLabel = new ToolStripMenuItem
                { Text = Properties.Resources.ID_TREEMENUCOPY };
                duplicateLabel.Click += new EventHandler(DuplicateLabel_Click);
                // *** -> delete
                ToolStripMenuItem deleteLabel = new ToolStripMenuItem
                { Text = Properties.Resources.ID_TREEMENUDELETE };
                deleteLabel.Click += new EventHandler(DeleteNode);
                // ***
                // add the menu items to the menu.
                _componentMenu.Items.AddRange(
                    new ToolStripItem[]{
                        renameBranchLabel
                        , separator1
                        , duplicateLabel
                        , deleteLabel
                    }
                    );
                // DEBUG MODE
                if (Properties.Settings.Default.ShowLogConsole)
                {
                    // *** -> separator 2
                    ToolStripSeparator separator2 = new ToolStripSeparator();
                    // *** -> send path to clipboard
                    ToolStripMenuItem menuItemSendPathToClipboard = new ToolStripMenuItem
                    { Text = Properties.Resources.ID_TREEMENUSENDPATHTOCLIPBOARD };
                    menuItemSendPathToClipboard.Click += new EventHandler(SendPathToClipboard);
                    // *** -> select in windows explorer
                    ToolStripMenuItem menuItemSelectInWindowsExplorer = new ToolStripMenuItem
                    { Text = Properties.Resources.ID_TREEMENUSELECTINEXPLORER };
                    menuItemSelectInWindowsExplorer.Click += new EventHandler(SelectInWindowsExplorer);
                    // *** -> modify GUID
                    ToolStripMenuItem menuItemModifyGUID = new ToolStripMenuItem
                    { Text = Properties.Resources.ID_TREEMENUSETNEWGUID };
                    menuItemModifyGUID.Click += new EventHandler(SetNewComponentGuid);
                    // ***
                     // add the menu items to the menu.
                    _componentMenu.Items.AddRange(new ToolStripItem[]{
                        separator2
                        , menuItemSendPathToClipboard
                        , menuItemSelectInWindowsExplorer
                        , menuItemModifyGUID
                    }
                    );
               }
            }
            return _componentMenu;
        }
        #endregion

        #region Context menu event handlers
        private string GetSelectedNodePath()
        {
            // retrieve database node
            Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
            NodeTag tag = GetCurrentTag();
            Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);
            if (!tn.IsDocument) return string.Empty;
            // get document
            Pic.DAL.SQLite.Document doc = tn.Documents(db)[0];
            return doc.File.Path(db);
        }

        private void SendPathToClipboard(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(GetSelectedNodePath());
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        private void SelectInWindowsExplorer(object sender, EventArgs e)
        {
            try
            {
                // get selected node file path
                string fileToSelect = GetSelectedNodePath();
                if (string.IsNullOrEmpty(fileToSelect)) return;
                // open "windows explorer" with the file selected
                ProcessStartInfo pfi = new ProcessStartInfo(
                    "explorer.exe", string.Format("/Select, {0}", fileToSelect));
                Process.Start(pfi);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        void SetNewComponentGuid(object sender, EventArgs e)
        {
            try
            {
                // get node
                NodeTag tag = GetCurrentTag();
                // retrieve database node
                Pic.DAL.SQLite.PPDataContext db0 = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db0, tag.TreeNode);
                int parentNodeID = tn.ParentNodeID.Value;
                // if not a component -> exit
                if (!tn.IsComponent) return;
                // get document
                Pic.DAL.SQLite.Document doc = tn.Documents(db0)[0];
                // get component
                Pic.DAL.SQLite.Component comp = Pic.DAL.SQLite.Component.GetByDocumentID(db0, doc.ID);
                // initial Guid
                Guid initialGuid = comp.Guid;
                string name = doc.Name;
                string description = doc.Description;
                // store majoration sets
                Dictionary<string, Dictionary<string, double>> majoSets = comp.GetAllMajorationSets(db0);
                // store parameter default values
                Dictionary<string, double> paramDefaultValues = comp.GetParamDefaultValues();
                // copy thumbnail
                string originalThumbnailPath = tn.Thumbnail.File.Path(db0);
                string newThumbnailPath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), "jpg");
                System.IO.File.Copy(originalThumbnailPath, newThumbnailPath);
                // show dialog and ask for new Guid
                FormDefineComponentGUID form = new FormDefineComponentGUID
                {
                    Guid = comp.Guid
                };
                if (DialogResult.OK != form.ShowDialog() || form.Guid == initialGuid)
                    return;

                // ### 0 ### regenerate component with new guid
                string outputPath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), "dll");
                if (!Pic.Plugin.PluginGenerator.Regenerate(doc.File.Path(db0), outputPath, form.Guid, string.Empty, string.Empty))
                {   // on failure -> exit
                    _log.Error(string.Format("Failed to regenerate component {0} ({1})", doc.Name, outputPath));
                    return;
                }
                // ### 1 ### try and delete existing component --------------------------------------------------
                try
                {
                    using (Pic.DAL.SQLite.PPDataContext db1 = new Pic.DAL.SQLite.PPDataContext())
                    {
                        Pic.DAL.SQLite.Component compToDelete = Pic.DAL.SQLite.Component.GetByGuid(db1, initialGuid);
                        compToDelete.Delete(db1, true);
                    }
                }
                catch (Exception /*ex*/) { }

                // ### 2 ### insert new component ---------------------------------------------------------------
                using (Pic.DAL.SQLite.PPDataContext db2 = new Pic.DAL.SQLite.PPDataContext())
                {
                    Pic.DAL.SQLite.TreeNode parentTNode = Pic.DAL.SQLite.TreeNode.GetById(db2, parentNodeID);
                    Pic.DAL.SQLite.Component comp2 = parentTNode.InsertComponent(db2, outputPath, form.Guid, name, description, newThumbnailPath);
                }

                // ### 3 ### recreate majorations if any
                using (Pic.DAL.SQLite.PPDataContext db3 = new Pic.DAL.SQLite.PPDataContext())
                {
                    Pic.DAL.SQLite.Component comp3 = Pic.DAL.SQLite.Component.GetByGuid(db3, form.Guid);
                    comp3.InsertMajorationSets(db3, majoSets);
                }

                // ### 4 ### recreate default values if any
                using (Pic.DAL.SQLite.PPDataContext db4 = new Pic.DAL.SQLite.PPDataContext())
                {
                    Pic.DAL.SQLite.Component comp4 = Pic.DAL.SQLite.Component.GetByGuid(db4, form.Guid);
                    comp4.InsertNewParamDefaultValues(db4, paramDefaultValues);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void DuplicateLabel_Click(object sender, EventArgs e)
        {
            try
            {
                // ### 1 ### Build new document name
                // get node
                NodeTag tag = GetCurrentTag();
                // retrieve database tree node
                Pic.DAL.SQLite.PPDataContext db0 = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db0, tag.TreeNode);
                Pic.DAL.SQLite.TreeNode tnParent = tn.GetParent(db0);
                if (!tn.IsDocument) return;
                // document name
                Pic.DAL.SQLite.Document doc = tn.Documents(db0)[0];
                int i = 1;
                string docNameCopy = string.Format("{0} ({1})", doc.Name, i++);
                while (!tnParent.AllowChildCreation(db0, docNameCopy))
                    docNameCopy = string.Format("{0} ({1})", doc.Name, i++);
                // ### 2 ### retrieve document file path
                string docFilePath = tn.Documents(db0)[0].File.Path(db0);
                string newDocFilePath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), System.IO.Path.GetExtension(docFilePath));
                // copy thumbnail
                string originalThumbnailPath = tn.Thumbnail.File.Path(db0);
                string newThumbnailPath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), "jpg");
                System.IO.File.Copy(originalThumbnailPath, newThumbnailPath);
                if (tn.IsComponent)
                {   // *** COMPONENT
                    // get component
                    Pic.DAL.SQLite.Component comp = Pic.DAL.SQLite.Component.GetByDocumentID(db0, doc.ID);
                    // store parameter default values
                    Dictionary<string, double> paramDefaultValues = comp.GetParamDefaultValues();
                    // store majoration sets
                    Dictionary<string, Dictionary<string, double>> majoSets = comp.GetAllMajorationSets(db0);
                    // regenerate plugin file rather than just make a copy
                    Guid guid = Guid.NewGuid();
                    if (!Pic.Plugin.PluginGenerator.Regenerate(doc.File.Path(db0), newDocFilePath, guid, string.Empty, string.Empty))
                    {   // on failure -> exit
                        _log.Error(string.Format("Failed to regenerate component {0} ({1})", doc.Name, docFilePath));
                        return; 
                    }
                    tnParent.InsertComponent(db0, newDocFilePath, guid, docNameCopy, doc.Description, newThumbnailPath);
                    // insert default values
                    using (Pic.DAL.SQLite.PPDataContext db1 = new Pic.DAL.SQLite.PPDataContext())
                    {
                        Pic.DAL.SQLite.Component comp1 = Pic.DAL.SQLite.Component.GetByGuid(db1, guid);
                        comp1.InsertNewParamDefaultValues(db1, paramDefaultValues);
                    }
                    // insert majorations
                    using (Pic.DAL.SQLite.PPDataContext db2 = new Pic.DAL.SQLite.PPDataContext())
                    {
                        Pic.DAL.SQLite.Component comp2 = Pic.DAL.SQLite.Component.GetByGuid(db2, guid);
                        comp2.InsertMajorationSets(db2, majoSets);
                    }
                    // *** COMPONENT
                }
                else
                {   // *** OTHER DOCUMENT
                    // copy document itself
                    System.IO.File.Copy(docFilePath, newDocFilePath);
                    // insert new document
                    tnParent.InsertDocument(db0, newDocFilePath, docNameCopy, doc.Description, doc.DocumentType.Name, newThumbnailPath);
                    // *** OTHER DOCUMENT
                }
                try
                {
                    // delete copied document + thumbnail
                    System.IO.File.Delete(newDocFilePath);
                    System.IO.File.Delete(newThumbnailPath);
                }
                catch (Exception /*ex*/) { }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            CollapseRoot();
        }

        private void DeleteNode(object sender, EventArgs e)
        {
            try
            {
                // retrieve database node
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                NodeTag tag = GetCurrentTag();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);
                // prepare warning message
                string message = string.Empty;
                if (tn.IsDocument)
                    message = string.Format(Properties.Resources.ID_TREEWARNING_DELETEDOCUMENT, tn.Name);
                else
                    message = string.Format(Properties.Resources.ID_TREEWARNING_DELETEBRANCH, tn.Name);
                // show warning message and proceed with node deletion if necessary
                if (DialogResult.Yes == MessageBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo))
                {
                    // delete from data base
                    tn.Delete(db, true, null);
                    // delete from tree control
                    TreeNode treeNode = FindNode(null, tag);
                    treeNode.Remove();
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        private void BackupNode(object sender, EventArgs e)
        {
            try
            {
                // get node
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                NodeTag tag = GetCurrentTag();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);
                // get backup file name
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Backup file (*.zip)|*.zip|"  + 
                            "All files (*.*)|*.*";
                dlg.FilterIndex = 0;
                dlg.Title = string.Format(Properties.Resources.ID_TREEMENUBACKUP, tn.Name);
                dlg.FileName = string.Format("{0}.zip", tn.Name);

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    List<string> treeNodePath = tn.GetTreeNodePath(db);
                    // backup branch
                    FormWorkerThreadTask.Execute(new TPTBackupBranch(treeNodePath, dlg.FileName));
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        private void RenameNode(object sender, EventArgs e)
        {
            try
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                NodeTag tag = GetCurrentTag();
                Pic.DAL.SQLite.TreeNode tn = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);

                FormRenameNode form = new FormRenameNode();
                form.NodeName = tn.Name;
                form.NodeDescription = tn.Description;
                form.Image = tn.Thumbnail.GetImage();
                if (DialogResult.OK == form.ShowDialog())
                {
                    tn.Name = form.NodeName;
                    tn.Description = form.NodeDescription;
                    db.SubmitChanges();

                    if (form.HasValidThumbnailPath)
                        Pic.DAL.SQLite.TreeNode.ReplaceThumbnail(tag.TreeNode, form.ThumbnailPath);

                    TreeNode treeNode = FindNode(null, tag);
                    treeNode.Text = form.NodeName;
                    treeNode.ToolTipText = form.NodeDescription;
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        private void OnInsertNewDocument(object sender, EventArgs e)
        {
            try
            {
                // current node tag
                var tag = GetCurrentTag();
                if (null == tag || tag.IsDocument)
                    throw new Exception("Invalid TreeNode tag");
                var db = new Pic.DAL.SQLite.PPDataContext();
                var treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);

                // force creation of a profile if no profile exist
                Pic.DAL.SQLite.CardboardProfile[] cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                {
                    FormEditProfiles dlgProfile = new FormEditProfiles();
                    dlgProfile.ShowDialog();
                }
                cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                    return;

                // show dialog
                var form = new FormCreateDocument { TreeNode = tag, ForcedComponentMode = false };
                if (DialogResult.OK == form.ShowDialog())
                {
                    // insert document in treeview
                    List<Pic.DAL.SQLite.TreeNode> tnodes = Pic.DAL.SQLite.TreeNode.GetByDocumentId(db, form.DocumentID);
                    // (re)populate parent node
                    PopulateChildren(SelectedNode);
                    if (form.OpenInsertedDocument)
                    {
                        // select current node
                        NodeTag newDocTag = new NodeTag(NodeTag.NType.NTDocument, tnodes[0].ID);
                        SelectedNode = FindNode(null, newDocTag);  // <- for some reasons, this does not work
                        // however, we can force document opening with the following line
                        SelectionChanged(this, new NodeEventArgs(newDocTag.TreeNode, newDocTag.Type), newDocTag.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }

        private void OnInsertNewBranch(object sender, EventArgs e)
        {
            try
            {
                // get current tree node
                NodeTag tag = GetCurrentTag();
                if (null == tag || tag.IsDocument)
                    throw new Exception("Invalid branch tag");
                // show dialog
                FormCreateBranch dlg = new FormCreateBranch();
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                    // get parent node
                    Pic.DAL.SQLite.TreeNode parentNode = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);
                    // create new branch
                    Pic.DAL.SQLite.TreeNode nodeNew = parentNode.CreateChild(db, dlg.BranchName, dlg.BranchDescription, dlg.BranchImage);
                    // insert branch node
                    InsertTreeNode(SelectedNode, nodeNew);
                    // select current node
                    SelectedNode = FindNode(null, tag);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }

        private void OnInsertNewComponent(object sender, EventArgs e)
        {
            try
            {
                // current node tag
                var tag = GetCurrentTag();
                if (null == tag || tag.IsDocument)
                    throw new Exception("Invalid TreeNode tag");
                var db = new Pic.DAL.SQLite.PPDataContext();
                var treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, tag.TreeNode);

                // force creation of a profile if no profile exist
                Pic.DAL.SQLite.CardboardProfile[] cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                {
                    FormEditProfiles dlgProfile = new FormEditProfiles();
                    dlgProfile.ShowDialog();
                }
                cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                    return;

                // show dialog
                var form = new FormCreateDocument { TreeNode = tag, ForcedComponentMode = true };
                if (DialogResult.OK == form.ShowDialog())
                {
                    // insert document in treeview
                    List<Pic.DAL.SQLite.TreeNode> tnodes = Pic.DAL.SQLite.TreeNode.GetByDocumentId(db, form.DocumentID);
                    // (re)populate parent node
                    PopulateChildren(SelectedNode);
                    if (form.OpenInsertedDocument)
                    {
                        // select current node
                        NodeTag newDocTag = new NodeTag(NodeTag.NType.NTDocument, tnodes[0].ID);
                        SelectedNode = FindNode(null, newDocTag);  // <- for some reasons, this does not work
                        // however, we can force document opening with the following line
                        SelectionChanged(this, new NodeEventArgs(newDocTag.TreeNode, newDocTag.Type), newDocTag.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Delegates
        public delegate void StartPageSelectHandler(object sender);
        public delegate void DownloadPageSelectHandler(object sender);
        public delegate void SelectionChangedHandler(object sender, NodeEventArgs e, string name);
        public delegate void LeafSelectHandler(object sender, NodeEventArgs e);
        public delegate void NodeDroppedHandler(object sender, NodeDroppedArgs e);
        #endregion

        #region Events
        public event StartPageSelectHandler StartPageSelected;
        public event DownloadPageSelectHandler DownloadPageSelected;
        public event SelectionChangedHandler SelectionChanged;
        public event NodeDroppedHandler NodeDropped;
        #endregion

        #region Data members
        private ContextMenuStrip _branchMenu;
        private ContextMenuStrip _leafMenu;
        private ContextMenuStrip _componentMenu;
        private TreeNode _draggedNode;
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(DocumentTreeView));
        public bool _preventEventTriggering = false;
        #endregion
    }

    #region Event argument classes : NodeEventArgs
    public class NodeEventArgs : EventArgs
    {
        public NodeEventArgs(int treeNodeId, NodeTag.NType type)
        {
            Node = treeNodeId;
            Type = type;
        }
        public int Node { get; }
        public NodeTag.NType Type { get; }
        public NodeTag NodeTag => new NodeTag(Type, Node);
    }
    #endregion

    #region Event argument classes : NodeDroppedArgs
    public class NodeDroppedArgs : EventArgs
    {
        public NodeDroppedArgs(TreeNode nodeFrom, TreeNode nodeTo)
        { _treeNodeFrom = nodeFrom; _treeNodeTo = nodeTo; }
        public TreeNode _treeNodeFrom, _treeNodeTo;
    }
    #endregion

    #region Class NodeTag to be used as a TreeNode tag
    public class StartPageTag { }
    public class DownloadPageTag { }
    public class NodeTag
    {
        #region Enums
        public enum NType
        {
            NTBranch
            , NTDocument
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Basic constructor with no cached values 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public NodeTag(NType type, int id)
        {
            TreeNode = id;
            Type = type;
        }
        public NodeTag(NType type, int id, string name, string description, Image thumbnail)
        {
            TreeNode = id;
            Type = type;
            _name = name;
            _description = description;
            _thumbnail = thumbnail;
        }
        #endregion

        #region Accessing inner content
        public NType Type { get; }
        public bool IsDocument => NType.NTDocument == Type; 
        public bool IsBranch => NType.NTBranch == Type;
        public bool IsComponent => NType.NTDocument == Type;

        /// <summary>
        /// TreeNode id
        /// </summary>
        public int TreeNode { get; }
        /// <summary>
        /// Thumbnail
        /// </summary>
        public Image GetThumbnail()
        {
            if (null == _thumbnail)
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode node = Pic.DAL.SQLite.TreeNode.GetById(db, TreeNode);
                Image tempImage = node.Thumbnail.GetImage();
                Pic.DAL.SQLite.Thumbnail.Annotate(tempImage, node.Name);
                _thumbnail = tempImage;
            }
            return _thumbnail;
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                    Pic.DAL.SQLite.TreeNode node = Pic.DAL.SQLite.TreeNode.GetById(db, TreeNode);
                    _name = node.Name;
                }
                return _name;
            }
        }
        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                    Pic.DAL.SQLite.TreeNode node = Pic.DAL.SQLite.TreeNode.GetById(db, TreeNode);
                    _description = node.Description;
                }
                return _description;
            }
        }
        #endregion

        #region System.Object override
        public override bool Equals(object obj)
        {
            NodeTag nodeTag = obj as NodeTag;
            return (TreeNode == nodeTag.TreeNode) /*&& (_type == nodeTag._type)*/;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
        #region Data members
        // Cached values (name / description)
        private string _name, _description;
        // thumbnail image
        private Image _thumbnail;
        #endregion
    }
    #endregion // NodeTag
}
