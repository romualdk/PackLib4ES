#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;

using System.Runtime.InteropServices;
#endregion

namespace Pic.Factory2D
{
    /// <summary>
    /// Represents a cardboard format
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class PicCardboardFormat : PicTypedDrawable
    {
        #region Private fields
        #endregion

        #region Constructor
        internal PicCardboardFormat(uint id)
            : base(id, PicGraphics.LT.LT_CUT)
        {
        }
        internal PicCardboardFormat(uint id, Vector2D position, Vector2D dimensions)
            : base(id, PicGraphics.LT.LT_CUT)
        {
            Position = position;
            Dimensions = dimensions;
        }
        #endregion

        #region PicDrawable overrides
        public override double Length
        { get { return 2.0 * (Dimensions.X + Dimensions.Y); } }

        protected override bool Evaluate()
        {
            _box = ComputeBox(Transform2D.Identity);
            return true;
        }
        public override Box2D ComputeBox(Transform2D transform)
        {
            return new Box2D(Vector2D.Zero, Dimensions).Transform(transform);
        }

        protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
        {
            graphics.DrawLine(
                LineType
                , transform.transform(new Vector2D(0.0, 0.0))
                , transform.transform(new Vector2D(Dimensions.X, 0.0))
                );
            graphics.DrawLine(
                LineType
                , transform.transform(new Vector2D(Dimensions.X, 0.0))
                , transform.transform(new Vector2D(Dimensions.X, Dimensions.Y))
                );
            graphics.DrawLine(
                LineType
                , transform.transform(new Vector2D(Dimensions.X, Dimensions.Y))
                , transform.transform(new Vector2D(0.0, Dimensions.Y))
                );
            graphics.DrawLine(
                LineType
                , transform.transform(new Vector2D(0.0, Dimensions.Y))
                , transform.transform(new Vector2D(0.0, 0.0))
                );
        }

        protected override void DrawSpecific(PicGraphics graphics)
        {
            DrawSpecific(graphics, Transform2D.Identity);
        }

        public override void Transform(Transform2D transform)
        {
            Position = transform.transform(Position);
        }

        public override Segment[] Segments
        {
            get
            {
                Segment[] segments = new Segment[0];
                return segments;
            }
        }
        #endregion

        #region PicEntity overrides
        protected override ECode GetCode()
        {
            return ECode.PE_CARDBOARDFORMAT;
        }
        public override PicEntity Clone(IEntityContainer factory)
        {
            PicCardboardFormat cbf = new PicCardboardFormat(factory.GetNewEntityId(), Vector2D.Zero, Dimensions);
            return cbf;
        }
        #endregion

        #region Object overrides
        public override int GetHashCode()
        {
            return Dimensions.GetHashCode() ^ Position.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("Format : {0} * {1},  Position : ({2},{3})", Dimensions.X, Dimensions.Y, Position.X, Position.Y);
        }
        #endregion

        #region Specific properties
        public Vector2D Dimensions { get; private set; }
        public double Width { get { return Dimensions.X; } }
        public double Height { get { return Dimensions.Y; } }
        public Vector2D Position { get; private set; }
        public double Thickness => 0.0;
        public Box2D BBox => new Box2D(Vector2D.Zero, Dimensions);
        #endregion
    }
}
