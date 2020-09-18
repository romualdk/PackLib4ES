#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Configuration;

using System.IO;

using log4net;
#endregion

namespace PicParam
{
    public partial class OptionPanelDebug : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelDebug()
        {
            InitializeComponent();
        }
        #endregion
        #region Load & Save handlers
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // check box
            chkbShowLogConsole.Checked = Properties.Settings.Default.ShowLogConsole;

            _OptionsForm.OptionsSaving += new EventHandler(OptionPanelDebug_OptionsSaving);
        }

        private void OptionPanelDebug_OptionsSaving(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.ShowLogConsole = chkbShowLogConsole.Checked;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region Event handlers
        private void OnUpdateLocalisationFile(object sender, EventArgs e)
        {
            try
            {
                // process all components and collect parameter descriptions
                FormWorkerThreadTask.Execute(new TPTCollectPluginParameterNames());

                // open localisation file
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    Verb = "Open",
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = LocalizerImpl.Instance.LocalisationFileName
                };
                using (Process p = new Process())
                {
                    p.StartInfo = startInfo;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void OnOpenConfigDirectory(object sender, EventArgs e)
        {
            try
            {
                // get config object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                // open folder with windows explorer
                Process.Start( Path.GetDirectoryName(config.FilePath) );
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnShowLogConsoleChecked(object sender, EventArgs e)
        {
            try
            {
                // force setting
                Properties.Settings.Default.ShowLogConsole = chkbShowLogConsole.Checked;
                Properties.Settings.Default.Save();

                // show or hide log console
                FormMain form = FormMain.Instance;
                form.ShowLogConsole();

                ApplicationMustRestart = false;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(OptionPanelDebug));
        #endregion
    }
}
