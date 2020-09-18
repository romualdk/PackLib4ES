﻿#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
#endregion

namespace Pic.DAL.LibraryLoader
{
    public partial class FormMergeLib : Form, IProcessingCallback
    {
        #region Enums
        public enum Mode
        { 
            Mode_Merge
            , Mode_Overwrite
        }
        #endregion

        #region Constructor
        public FormMergeLib(Mode mode)
        {
            InitializeComponent();
            MergeMode = mode;
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            // set caption
            Text = string.Format("Downloading file {0}...", FileName);
            // start download
            DownLoadFileInBackground(UriLibraryFile, LocalLibraryFile);
            base.OnLoad(e);
        }
        #endregion

        #region ITreeProcessingCallback implementation
        private int _errorCount = 0;
        public bool HadErrors
        {
            get { return _errorCount > 0; }
        }
        public void Begin()
        {
            initEvent.WaitOne();
            Invoke(new AppendTextInvoker(DoAppendText), new object[] { "Merging database : begin...", Color.Black });
        }
        public void End()
        {
            initEvent.WaitOne();
            Invoke(new EndInvoker(DoEnd));
        }

        public void Info(string text)
        {
            initEvent.WaitOne();
            Invoke(new AppendTextInvoker(DoAppendText), new object[] {text, Color.Black}); 
        }
        public void Error(string text)
        {
            initEvent.WaitOne();
            Invoke(new ErrorIncrementInvoker(IncrementErrorCount));
            Invoke(new AppendTextInvoker(DoAppendText), new object[] { text, Color.Red });
        }
        public bool IsAborting { get { return false; } }

        private void DoEnd()
        {
            string text = string.Empty;
            switch (MergeMode)
            {
                case Mode.Mode_Merge: text = string.Format("Merge complete -- {0} errors", _errorCount); break;
                case Mode.Mode_Overwrite: text = string.Format("Overwrite complete -- {0} errors", _errorCount); break;
                default: break;
            }
            DoAppendText(text, HadErrors ? Color.Red : Color.Black);
            if (!HadErrors)
            {
                Thread.Sleep(1000);
                Close();
            }
        }
        private void DoAppendText(string text, Color textColor)
        {
            richTextBox.SelectionColor = textColor;
            richTextBox.AppendText(text + System.Environment.NewLine);
            // scroll to last line...
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        private void IncrementErrorCount()
        { 
            ++_errorCount;
        }
        #endregion

        #region Public properties
        public Mode MergeMode { get; set; }
        public string LibraryName { get; set; }
        public string FileName { get; set; }
        private string LocalLibraryFile
        {   get { return Path.Combine(Path.GetTempPath(), FileName); } }

        private string UriLibraryFile
        {
            get
            {
                string uriPackLib = Settings.Default.UriPlmPackLib;
                if (!uriPackLib.EndsWith("/"))
                    uriPackLib += "/";
                return uriPackLib + "lib/" + FileName;
            }
        }
        #endregion

        #region Download methods
        private void DownLoadFileInBackground(string uriAddress, string localFilePath)
        {
            _client = new WebClient();
            Uri uri = new Uri(uriAddress);

            // Specify that the DownloadFileCallback method gets called 
            // when the download completes.
            _client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleteCallback);
            // Specify a progress notification handler.
            _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
            _client.DownloadFileAsync(uri, localFilePath);
        }
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // update progress bar value
            progressBar.Value = e.ProgressPercentage;
            // Displays the operation identifier, and the transfer progress.
            labelProgressBar.Text = string.Format("{0} downloaded {1:0.00} of {2:0.00} MB. {3} % complete..."
                , (string)e.UserState
                , e.BytesReceived / (1024.0*1024.0)
                , e.TotalBytesToReceive / (1024.0*1024.0)
                , e.ProgressPercentage);
        }

        private void DownloadCompleteCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (!File.Exists(LocalLibraryFile))
            {
                labelProgressBar.Text = "Download failed!";
            }
            else
            {
                initEvent.Set();
                // download complete
                labelProgressBar.Text = "Download successful! Extracting...";
                // start merging library
                ThreadPool.QueueUserWorkItem(new WaitCallback(ExtractZip), this);
            }
            
        }

        private void ExtractZip(object status)
        {
            IProcessingCallback callback = status as IProcessingCallback;
            callback.Begin();
			string sOpName = string.Empty;
            try
            {
                switch (MergeMode)
                {
                    case Mode.Mode_Overwrite:
						sOpName = "Updating with file ";
                        BackupRestore.Overwrite(
                            LocalLibraryFile
                            , this);
                        break;
                    case Mode.Mode_Merge:
						sOpName = "Merging with file ";
                        BackupRestore.Merge(
                            LocalLibraryFile
                            , this);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (null != callback)
                    callback.Error(string.Format("{0} {1} failed with error: {2}"
						, sOpName, LocalLibraryFile, ex.Message));
            }
            // ending
            callback.End();
        }
         #endregion

        #region Handlers
        private void OnCancel(object sender, EventArgs e)
        {
            if (null != _client)
                _client.CancelAsync();
            Close();
        }

        #endregion
        #region Data members
        /// <summary>
        /// WebClient object used to download file
        /// </summary>
        WebClient _client;
        /// <summary>
        /// notifies one or more threads that an event has occured
        /// </summary>
        private ManualResetEvent initEvent = new ManualResetEvent(false);
        /// <summary>
        /// delegate to be used in Control.Invoke() calls to append text in richTextControl
        /// </summary>
        public delegate void AppendTextInvoker(string text, Color textColor);
        /// <summary>
        /// delegate to be used in Control.Invoke() calls to initialize worker thread job
        /// </summary>
        public delegate void BeginInvoker();
        /// <summary>
        /// delegate to be used in Control.Invoke() calls to notify worker thread job end
        /// and close form
        /// </summary>
        public delegate void EndInvoker();
        /// <summary>
        /// delegate to be used in Control.Invoke() calls to increment error count
        /// </summary>
        public delegate void ErrorIncrementInvoker();
        #endregion
    }
}
