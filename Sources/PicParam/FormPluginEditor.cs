#region Using directives
using System.Windows.Forms;
using System.IO;
#endregion

namespace PicParam
{
    /// <summary>
    /// Plugin editor form
    /// </summary>
    public partial class FormPluginEditor : Form
    {
        #region Constructor
        /// <summary>
        /// Form constructor
        /// </summary>
        public FormPluginEditor()
        {
            InitializeComponent();
            // set component search method
            _generatorCtrl.ComponentSearchMethod = new ComponentSearchMethodDB();
            _generatorCtrl.PluginValidated += new Pic.Plugin.GeneratorCtrl.GeneratorCtrl.GeneratorCtlrHandler(OnPluginValidated);
            _generatorCtrl.SetComponentDirectory(Path.GetDirectoryName(Pic.DAL.ApplicationConfiguration.CustomSection.DatabasePath));
        }

        #endregion

        #region Plugin generator control event handler
        /// <summary>
        /// Plugin validated handler
        /// </summary>
        void OnPluginValidated(object sender, Pic.Plugin.GeneratorCtrl.GeneratorCtrlEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// input plugin path
        /// </summary>
        public string PluginPath
        {
            set
            {
                _generatorCtrl.SetComponentFilePath(value);
                Text = string.Format(Properties.Resources.ID_EDITCOMPONENT, value); 
            }
        }

        /// <summary>
        /// output plugin path
        /// </summary>
        public string OutputPath
        {
            set { _generatorCtrl.OutputPath = value; }
            get { return _generatorCtrl.OutputPath; }
        }
        #endregion
    }
}
