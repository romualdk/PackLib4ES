#region Using directives
using System.Drawing;

using Pic.Factory2D;
#endregion

namespace Pic.Plugin.ViewCtrl
{
    class PicGraphicsCtrlBased : PicGraphicsGdiPlus
    {
        #region Public constructor
        public PicGraphicsCtrlBased(Size size, Graphics graph)
        {
            _size = size;
            GdiGraphics = graph;
        }
        #endregion
        #region Public properties
        public Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                // force box recomputation
                if (Box.IsValid)
                    DrawingBox = Box;
            }
        }
        #endregion
        #region Public PicGraphicsGdiPlus overrides
        public override Size GetSize()
        {
            return _size;
        }
        #endregion
        #region Private fields
        private Size _size;
        #endregion
    }
}
