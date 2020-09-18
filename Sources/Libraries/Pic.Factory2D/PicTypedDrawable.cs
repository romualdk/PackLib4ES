#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
#endregion

namespace Pic
{
	namespace Factory2D
	{
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
		public abstract class PicTypedDrawable : PicDrawable
		{
            #region Protected constructors
            protected PicTypedDrawable(uint id, PicGraphics.LT lType)
				: base(id)
			{
                LineType = lType;
                Group = 1;
                Layer = 1;
			}
            #endregion

            #region Public properties
            public PicGraphics.LT LineType { get; set; }
            public abstract double Length { get; }
            public short Group { get; set; }
            public short Layer { get; set; }
            #endregion
        }
	}
}