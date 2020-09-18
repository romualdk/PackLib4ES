#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
#endregion

namespace Pic
{
    namespace Factory2D
    {
        public class PicPoint : PicTypedDrawable 
        {
            #region Private fields
            private Vector2D Pt { get; set; }
            #endregion

            #region Constructors
            protected PicPoint(uint id, PicGraphics.LT lType)
                : base(id, lType)
            {
            }
            #endregion

            #region Pic.Factory2D.PicEntity Overrides 
            protected override ECode GetCode()
            {
                return ECode.PE_POINT;
            }
            public override PicEntity Clone(IEntityContainer factory)
            {
                return new PicPoint(factory.GetNewEntityId(), LineType) {  Pt = this.Pt };
            }
            #endregion

            #region Creation methods
            public static PicPoint CreateNewPoint(uint id, PicGraphics.LT lType, Vector2D pt)
            {
                return new PicPoint(id, lType);
            }
            #endregion

            #region PicDrawable overrides
            protected override void DrawSpecific(PicGraphics graphics)
            {
                graphics.DrawPoint(LineType, Pt);
            }
            protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
			{
				graphics.DrawPoint(LineType, transform.transform(Pt));
			}
            public override void Transform(Transform2D transform)
            {
                Pt = transform.transform(Pt);
                SetModified();
            }
            public override double Length
            {
                get { return 0.0; }
            }
            public override Box2D  ComputeBox(Transform2D transform)
            {
                Box2D box = Box2D.Initial;
                box.Extend(transform.transform(Pt));
                return box;
            }
            public override Segment[] Segments
            {
                get { return Array.Empty<Segment>(); }
            }
            #endregion

            #region Public properties
            public Vector2D Coord
            {
                get => Pt;
                set => Pt = value;
            }
            #endregion

            #region System.Object Overrides
            public override string ToString() => $"Point id = {Id}, coord = {Pt}\n";
            #endregion
        }
    }
}
