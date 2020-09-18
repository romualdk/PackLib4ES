#region Using directives
using System;
// docking
using WeifenLuo.WinFormsUI.Docking;
// log4net
using log4net;
#endregion

namespace PicParam
{
    public partial class DockLogConsole : DockContent
    {
        #region Constructor
        public DockLogConsole()
        {
            InitializeComponent();
        }
        #endregion
        #region Load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            log4net.Appender.RichTextBoxAppender.SetRichTextBox(richTextBoxLog, "RichTextBoxAppender");
            _log.Info("RichTextBoxAppender ready!");
        }
        #endregion
        #region Private data member
        static readonly ILog _log = LogManager.GetLogger(typeof(DockLogConsole));
        #endregion
    }
}
