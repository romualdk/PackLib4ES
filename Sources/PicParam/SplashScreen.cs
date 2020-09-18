#region Using directives
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;

using PicParam.Properties;
using System;
#endregion

namespace PicParam
{
    public partial class SplashScreen : Form
    {
        #region Constructor
        public SplashScreen()
        {
            InitializeComponent();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (null != BackgroundImage)
            {
                Bitmap b = BackgroundImage as Bitmap;
                Size = b.Size;

                // make lower right pixel color transparent
                if (Transparent)
                    b.MakeTransparent(b.GetPixel(1, 1));
                else
                    AllowTransparency = false;
                BackgroundImage = b;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// retrieves assembly version
        /// </summary>
        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string RebrandedVersion => Settings.Default.UseRebrandedVersion;
        /// <summary>
        /// set / get transparency
        /// </summary>
        public bool Transparent { get; set; } = false;
        #endregion
    }
}
