#region Using directives
using System;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

using treeDiM.ThreadCallback;
#endregion

namespace treeDiM.UserControls
{
    public partial class ThreadWindow : Form, IThreadCallback
    {
        #region Constructor
        public ThreadWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region IThreadCallback implementation
        public void Begin()
        {
            initEvent.WaitOne();
            Invoke(new MethodInvoker(DoBegin));
        }
        public void SetText(string text)
        {
            Invoke(new SetTextInvoker(DoSetText), new object[] { text });
        }
        public bool IsAborting
        {
            get { return abortEvent.WaitOne(0, false); }
        }
        public void End()
        {
            if (requiresClose)
                Invoke(new MethodInvoker(DoEnd));
        }
        #endregion

        #region Implementation members invoked on the owner thread
        private void DoSetText(string text)
        {
            rtbThreadOutput.AppendText(text + Environment.NewLine);
            rtbThreadOutput.SelectionStart = rtbThreadOutput.Text.Length;
            rtbThreadOutput.ScrollToCaret();
        }
        private void DoBegin()
        {
            bnCancel.Enabled = true;
            ControlBox = true;
        }
        private void DoEnd()
        {
            Close();
        }
        #endregion

        #region Form overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ControlBox = false;
            initEvent.Set();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UserAborted = true;
            AbortWork();
            base.OnClosing(e);
        }
        #endregion

        #region Implementation utilities
        /// <summary>
        /// Utility function to terminate the thread
        /// </summary>
        private void AbortWork()
        {
            abortEvent.Set();
        }
        /// <summary>
        /// True if user clicked 'Cancel' button
        /// </summary>
        public bool UserAborted { get; private set; } = false;
        #endregion

        #region Event handlers
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UserAborted = true;
            AbortWork();
        }
        #endregion

        #region Data members
        private bool requiresClose = true;
        private ManualResetEvent initEvent = new ManualResetEvent(false);
        private ManualResetEvent abortEvent = new ManualResetEvent(false);

        public delegate void SetTextInvoker(string text);
        #endregion
    }
}
