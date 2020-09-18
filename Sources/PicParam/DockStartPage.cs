#region Using directives
// Dock content
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace PicParam
{
    public partial class DockStartPage : DockContent
    {
        #region Constructor
        public DockStartPage()
        {
            InitializeComponent();
        }
        #endregion

        #region Public properties
        public System.Uri Url
        {
            get => null;
            set {  }
        }
        #endregion
    }
}
