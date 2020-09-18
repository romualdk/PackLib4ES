#region Using directives
using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Pic.Factory2D
{
    public class PicText : PicTypedDrawable
    {
        #region Constructor
        protected PicText(uint id, PicGraphics.LT lType,
            string text,
            Vector2D pt,
            PicGraphics.HAlignment hAlignment,
            PicGraphics.VAlignment vAlignment)
            : base(id, lType)
        {
            Text = text;
            Point = pt;
            HAlignment = hAlignment;
            VAlignment = vAlignment;
        }
        #endregion

        #region Public methods
        public Vector2D Point { get; set; }
        public PicGraphics.HAlignment HAlignment { get; set; }
        public PicGraphics.VAlignment VAlignment { get; set; }
        public string Text { get; set; }
        public float TextDirection { get; set; } = 0.0f;
        public float FontSize { get; set; } = 8.0f;
        #endregion

        #region PicTypedDrawable override
        protected override ECode GetCode()
        {
            return ECode.PE_TEXT;
        }
        public override double Length => 0.0;
        public override Segment[] Segments => new List<Segment>().ToArray();
        public override PicEntity Clone(IEntityContainer factory)
        {
            return new PicText(Id, LineType, Text, Point, HAlignment, VAlignment);
        }
        public override Box2D ComputeBox(Transform2D transform) => Box2D.Initial;
        public override void Transform(Transform2D transform)
        {
            Point = transform.transform(Point);
        }
        protected override void DrawSpecific(PicGraphics graphics)
        {
            float fontSize = (Math.Abs(FontSize) < 0.1f) ? PicCotation.GlobalCotationProperties.FontSize : graphics.FontSizePt(FontSize);
            graphics.DrawText(Text, PicGraphics.TextType.FT_COTATION, fontSize, Point + graphics.OffsetFont(FontSize, VAlignment), HAlignment, VAlignment, TextDirection);
        }
        protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
        {
            float fontSize = (Math.Abs(FontSize)< 0.1f) ? PicCotation.GlobalCotationProperties.FontSize : graphics.FontSizePt(FontSize);
            graphics.DrawText(Text, PicGraphics.TextType.FT_COTATION, fontSize, Point + graphics.OffsetFont(FontSize, VAlignment), HAlignment, VAlignment, TextDirection);
        }
        #endregion
        #region Public creation method
        public static PicText CreateNewText(uint id, PicGraphics.LT lineType,
            string text,
            Vector2D point,
            PicGraphics.HAlignment hAlignment,
            PicGraphics.VAlignment vAlignment = PicGraphics.VAlignment.VA_BOTTOM)
        {
            return new PicText(id, lineType, text, point, hAlignment, vAlignment);
        }
        #endregion


    }
}
