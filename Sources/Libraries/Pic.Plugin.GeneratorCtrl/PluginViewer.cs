#region Using directives
using System;
using System.Windows.Forms;
#endregion

namespace Pic.Plugin.GeneratorCtrl
{
    public partial class PluginViewer : Form
    {
        #region Constructor
        public PluginViewer(IComponentSearchMethod pluginSearchMethod, string pluginPath)
        {
            InitializeComponent();
            ctrlPluginViewer.SearchMethod = pluginSearchMethod;
            ctrlPluginViewer.PluginPath = pluginPath;

            ctrlPluginViewer.ValidateButtonVisible = true;
            ctrlPluginViewer.CloseButtonVisible = true;
            ctrlPluginViewer.CloseClicked += new EventHandler(OnCloseClicked);
            ctrlPluginViewer.ValidateClicked += new EventHandler(OnValidateClicked);
        }
        #endregion

        #region CtrlPluginViewer event handlers
        void OnCloseClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void OnValidateClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        #region System.Windows.Forms overrides
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Z:
                    ctrlPluginViewer.FitView();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        #endregion
    }
}