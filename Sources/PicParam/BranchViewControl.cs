#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Pic.Factory2D;

using log4net;
#endregion

namespace PicParam
{
    public partial class BranchViewControl : UserControl
    {
        #region Constructor
        public BranchViewControl()
        {
            InitializeComponent();
            // set thumbnail static parameters
            switch (Properties.Settings.Default.ThumbnailAnnotationMode)
            {
                case 0: Pic.DAL.SQLite.Thumbnail.AnnotationMode = Pic.DAL.SQLite.Thumbnail.AnnotateMode.ANNOTATE_NONE; break;
                case 1: Pic.DAL.SQLite.Thumbnail.AnnotationMode = Pic.DAL.SQLite.Thumbnail.AnnotateMode.ANNOTATE_NOBACKGROUND; break;
                case 2: Pic.DAL.SQLite.Thumbnail.AnnotationMode = Pic.DAL.SQLite.Thumbnail.AnnotateMode.ANNOTATE_WITHBACKGOUND; break;
                default: break;
            }
            Pic.DAL.SQLite.Thumbnail.FontSize = Properties.Settings.Default.ThumbnailAnnotationFont;
            // layout
            AutoScroll = true;
            timer.Interval = 1;
            timer.Tick += new EventHandler(OnTimerTick);
        }
        #endregion

        #region Overrides
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AutoScrollPosition = Point.Empty;
            int x = 0, y = 0;
            foreach (Control cntl in Controls)
            {
                cntl.Location = new Point(x, y) + (Size)AutoScrollPosition;
                AdjustXY(ref x, ref y);
            }
        }
        #endregion

        #region Display methods
        public void ShowBranch(NodeTag nodeTag)
        {
            // save current branch ID
            CurrentNodeTag = nodeTag;
            // clear existing buttons
            Controls.Clear();
            tooltip.RemoveAll();
            _listTreeNodes = new List<NodeTag>();

            try
            {
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                Pic.DAL.SQLite.TreeNode branch = Pic.DAL.SQLite.TreeNode.GetById(db, nodeTag.TreeNode);

                foreach (Pic.DAL.SQLite.TreeNode tn in branch.Childrens(db))
                {
                    // annotate image
                    Image thumbnailImage = tn.Thumbnail.GetImage();

                    Pic.DAL.SQLite.Thumbnail.Annotate(thumbnailImage, tn.Name);
                    // insert node
                    _listTreeNodes.Add(
                        new NodeTag(
                            tn.IsDocument ? NodeTag.NType.NTDocument : NodeTag.NType.NTBranch
                            , tn.ID
                            , tn.Name
                            , tn.Description
                            , thumbnailImage)
                            );
                }
            }
            catch (Exception /*ex*/)
            {
            }
            i = x = y = 0;
            timer.Start();
        }
        #endregion

        #region Event handlers
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (i == _listTreeNodes.Count)
            {
                timer.Stop();
                return;
            }

            Image image;
            SizeF sizef;
            try
            {
                image = _listTreeNodes[i].GetThumbnail();
                sizef = new SizeF(image.Width / image.HorizontalResolution, image.Height / image.VerticalResolution);
                float fScale = Math.Min(cxButton / sizef.Width, cyButton / sizef.Height);
                sizef.Width *= fScale;
                sizef.Height *= fScale;
            }
            catch (Exception ex)
            {
                _log.Debug(ex.ToString());
                ++i;
                return;
            }
            // convert image to small size for button
            Bitmap bitmap = new Bitmap(image, Size.Ceiling(sizef));
            image.Dispose();

            // create button and add to panel
            Button btn = new Button();
            btn.Image = bitmap;
            btn.Location = new Point(x, y) + (Size)AutoScrollPosition;
            btn.Size = new Size(cxButton, cyButton);
            btn.Tag = _listTreeNodes[i];
            btn.Click += new EventHandler(OnThumbnailClick);
            Controls.Add(btn);

            // give button a tooltip
            tooltip.SetToolTip(btn, string.Format("{0}\n{1}", _listTreeNodes[i].Name, _listTreeNodes[i].Description));

            // adjust i, x and y for next image
            AdjustXY(ref x, ref y);
            ++i;
        }
        /// <summary>
        /// Handle user click on button and trigger event to other control
        /// </summary>
        private void OnThumbnailClick(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Button btn)) return;
                NodeTag nodeTag = btn.Tag as NodeTag;

                if (nodeTag.IsBranch)
                    ShowBranch(nodeTag);
                // trigger event
                // mainform : hide branchview + show adapted viewer
                // DocumentTreeView -> show adapted 
                SelectionChanged?.Invoke(this, new NodeEventArgs(nodeTag.TreeNode, nodeTag.Type), nodeTag.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _log.Debug(ex.ToString());
            }
        }

        public void OnSelectionChanged(object sender, NodeEventArgs e, string name)
        {
            ShowBranch(e.NodeTag);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// computes position of next button
        /// </summary>
        /// <param name="x">Abscissa</param>
        /// <param name="y">Ordinate</param>
        private void AdjustXY(ref int x, ref int y)
        {
            x += cxButton;
            if (x + cxButton > Width - SystemInformation.VerticalScrollBarWidth)
            {
                x = 0;
                y += cyButton;
            }
        }
        #endregion

        #region Delegates
        public delegate void SelectionChangedHandler(object sender, NodeEventArgs e, string name);
        public delegate void TreeNodeCreatedHandler(object sender, NodeEventArgs e);
        public delegate void DocumentCreatedHandler(object sender, NodeEventArgs e);
        #endregion

        #region Events
        public event SelectionChangedHandler SelectionChanged;
        public event TreeNodeCreatedHandler TreeNodeCreated;
        #endregion

        #region Context menu handlers
        private void OnTSMenuItemNewBranch(object sender, EventArgs e)
        {
            try
            {
                // show dialog
                FormCreateBranch dlg = new FormCreateBranch();
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                    // get parent node
                    Pic.DAL.SQLite.TreeNode parentNode = Pic.DAL.SQLite.TreeNode.GetById(db, CurrentNodeTag.TreeNode);
                    // create new branch
                    Pic.DAL.SQLite.TreeNode nodeNew = parentNode.CreateChild(db, dlg.BranchName, dlg.BranchDescription, dlg.BranchImage);
                    // redraw branch
                    ShowBranch(CurrentNodeTag);
                    // event
                    TreeNodeCreated?.Invoke(this, new NodeEventArgs(nodeNew.ID, NodeTag.NType.NTBranch));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            } 
        }
        private void OnTSMenuItemNewDocument(object sender, EventArgs e)
        {
            try
            {
                // get data context
                Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
                // get current branch node
                Pic.DAL.SQLite.TreeNode treeNode = Pic.DAL.SQLite.TreeNode.GetById(db, CurrentNodeTag.TreeNode);
                // force creation of a profile if no profile exist
                Pic.DAL.SQLite.CardboardProfile[] cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                {
                    FormEditProfiles dlgProfile = new FormEditProfiles();
                    dlgProfile.ShowDialog();
                }
                cardboardProfiles = Pic.DAL.SQLite.CardboardProfile.GetAll(db);
                if (cardboardProfiles.Length == 0)
                    return; // no profile -> exit
                // show document creation dialog
                FormCreateDocument dlg = new FormCreateDocument
                {
                    TreeNode = new NodeTag(NodeTag.NType.NTBranch, CurrentNodeTag.TreeNode)
                };
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    // redraw current branch
                    ShowBranch(CurrentNodeTag);
                    // get current document Id
                    int documentId = dlg.DocumentID;
                    // get TreeNode from documentID
                    List<Pic.DAL.SQLite.TreeNode> listTreeNodeNew = Pic.DAL.SQLite.TreeNode.GetByDocumentId(db, dlg.DocumentID);
                    if (listTreeNodeNew.Count == 0)
                    {
                        _log.Error(string.Format("Failed to retrieve treeNode from document ID : {0}", dlg.DocumentID));
                        return; // -> failed to retrieve node 
                    }
                    TreeNodeCreated?.Invoke(this, new NodeEventArgs(listTreeNodeNew[0].ID, NodeTag.NType.NTBranch));
                } 
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            } 
        }
        #endregion

        #region Constants
        private const int cxButton = 150, cyButton = 150;   // image button size
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(BranchViewControl));
        private Timer timer = new Timer();
        private ToolTip tooltip = new ToolTip();
        private int i, x, y;
        private List<NodeTag> _listTreeNodes;
        private NodeTag CurrentNodeTag { get; set; }
        #endregion
    }

    public class DocTreeItem
    {
    }
}
