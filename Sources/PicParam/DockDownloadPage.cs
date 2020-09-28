#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using log4net;

using Pic.DAL.LibraryLoader;

using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace PicParam
{
    public partial class DockDownloadPage : DockContent
    {
        #region Constructor
        public DockDownloadPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            bnMerge.Enabled = -1 != listBoxLibraries.SelectedIndex;
        }
        private void OnButtonMerge(object sender, EventArgs e)
        {
            try
            {
                // get listbox selected index
                int iSel = listBoxLibraries.SelectedIndex;
                if (-1 == iSel) return;
                // download
                if (libFetcher.DownloadLibraryAndMerge(iSel))
                {
                    DatabaseModified?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnButtonOverwrite(object sender, EventArgs e)
        {
            // warning message
            if (DialogResult.No == MessageBox.Show(
                Properties.Resources.ID_DATABASEOVERWRITE
                , Application.ProductName, MessageBoxButtons.YesNo))
                return;

            try
            {
                // get listbox selected index
                int iSel = listBoxLibraries.SelectedIndex;
                if (-1 == iSel) return;
                // download
                if (libFetcher.DownloadLibraryAndOverwrite(iSel))
                {
                    DatabaseModified?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void OnButtonInfo(object sender, EventArgs e)
        {
            try
            {
                // get url                
                string infoUrl = libFetcher.GetInfoUrl(listBoxLibraries.SelectedIndex);
                System.Diagnostics.Process.Start(infoUrl);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region User control override
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                if (IsWebSiteReachable)
                {
                    libFetcher = new LibraryFetcher();
                    // fill fetcher
                    List<Library> libraries = libFetcher.Libraries;
                    foreach (Library lib in libraries)
                        listBoxLibraries.Items.Add(lib);
                    if (listBoxLibraries.Items.Count > 0)
                        listBoxLibraries.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// This property is aimed at finding out if the machine is connected to the web
        /// It actually checks if StartPageUrl define in configutation file can still be reached
        /// </summary>
        public bool IsWebSiteReachable
        {
            get
            {
                try
                {
                    Uri uri = new Uri(Properties.Settings.Default.UrlStartPage);
                    System.Net.IPHostEntry objIPHE = System.Net.Dns.GetHostEntry(uri.DnsSafeHost);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    return false;
                }
            }
        }
        #endregion

        #region Events
        public delegate void OnModifyDatabase(object sender, EventArgs e);
        public event OnModifyDatabase DatabaseModified;
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(DockDownloadPage));
        private LibraryFetcher libFetcher;
        #endregion
    }
}
