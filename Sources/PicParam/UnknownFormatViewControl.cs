#region Using directives
using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using log4net;

using PicParam.Properties;
#endregion

namespace PicParam
{
    public partial class UnknownFormatViewControl : UserControl
    {

        #region Constructor
        public UnknownFormatViewControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public string FilePath
        {
            set
            {
                _filePath = value;
                bnOpen.Enabled = File.Exists(_filePath);
            }
            get { return _filePath; }
        }
        public string NodeName
        {
            set
            {
                _nodeName = value;
                bnOpen.Text = string.Format( Resources.ID_OPENFILE, Path.ChangeExtension(_nodeName, Path.GetExtension(FilePath) ) );
            }
        }
        #endregion

        #region Event handlers
        private void OnOpenFile(object sender, EventArgs e)
        {
            try
            {
                // build new file path
                string filePathCopy = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(_nodeName, Path.GetExtension(FilePath)) );
                // copy file
                System.IO.File.Copy(FilePath, filePathCopy, true);
                // open using shell execute 'Open'
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.Verb = "Open";
                startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.FileName = filePathCopy;
                using (Process p = new Process())
                {
                    p.StartInfo = startInfo;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region Data members
        public string _nodeName;
        public string _filePath;
        private ILog _log = LogManager.GetLogger(typeof(UnknownFormatViewControl)); 
        #endregion
    }
}
