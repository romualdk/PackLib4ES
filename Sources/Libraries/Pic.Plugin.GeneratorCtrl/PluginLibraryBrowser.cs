#region Using directives
using System;
using System.Windows.Forms;
#endregion

namespace Pic.Plugin.GeneratorCtrl
{
    public partial class PluginLibraryBrowser : Form
    {
        #region Constructor
        public PluginLibraryBrowser(string libraryPath)
        {
            InitializeComponent();
            browserCtrl.LibraryPath = libraryPath;
        }
        #endregion

        #region Event handlers
        private void OnComponentSelected(object sender, PluginEventArgs e)
        {
            _guid = e.Guid;            
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        #region Data members
        public Guid _guid;
        #endregion
    }
}