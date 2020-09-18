#region Using directives
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Pic.Factory2D.Control
{
    public partial class DxfLineTypeConvertionCtrl : UserControl
    {
        #region Constructor
        public DxfLineTypeConvertionCtrl()
        {
            InitializeComponent();
        }
        #endregion

        #region Paint event handlers
        public void DxfLineTypePaint(object sender, PaintEventArgs e)
        {
            System.Drawing.Bitmap bmp = sender as System.Drawing.Bitmap;
            Size sz = bmp.Size;
            e.Graphics.DrawLine(new Pen(Color.Red), new Point(0, sz.Height / 2), new Point(sz.Width / 2, sz.Height / 2));
        }
        #endregion
    }
}
