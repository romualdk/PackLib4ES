#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
#endregion

namespace Pic
{
    namespace Factory2D
    {
        public class PicCotationRadius : PicCotation
        {
            #region Protected constructor
            protected PicCotationRadius(uint id, short noDecimals)
                : base(id, noDecimals)
            { 
            }
            #endregion

            #region PicTypedDrawable
            protected override void DrawSpecific(PicGraphics graphics)
            {
            }
            protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
            {
            }
            public override double Value()
            {
                throw new NotImplementedException();
            }
            public override void Transform(Transform2D transform)
            {
                SetModified();
                throw new NotImplementedException();
            }
            /// <returns>An entity to be saved in a new factory</returns>
            public override PicEntity Clone(IEntityContainer factory)
            {
                throw new NotImplementedException();
            }
            public override double Length
            {
                get => 0.0;
            }
            public override Box2D ComputeBox(Transform2D transform)
            {
                Box2D box = Box2D.Initial;
                return box;
            }

            public override Segment[] Segments
            {
                get { return new Segment[0]; }
            }
            #endregion

            /// <returns>A value of enum eCode</returns>
            protected override ECode GetCode()
            {
                return ECode.PE_COTATIONRADIUSEXT;
            }

            /// <returns>A string representation of this object.</returns>
            public override string ToString()
            {
                return base.ToString();
            }
        }
    }
}
