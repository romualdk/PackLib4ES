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
        public class PicCotationDistance : PicCotation
        {
            #region Protected fields
            protected Vector2D _pt0, _pt1;
            protected double _offset;
            protected bool _shortLines = false;
            #endregion

            #region Protected constructor
            protected PicCotationDistance(uint id, Vector2D pt0, Vector2D pt1, double offset, short noDecimals)
                : base(id, noDecimals)
            {
                _pt0 = pt0;
                _pt1 = pt1;
                _offset = offset;
            }
            #endregion

            #region Public properties
            public Vector2D Pt0
            {
                get { return _pt0; }
                set { _pt0 = value; }
            }
            public Vector2D Pt1
            {
                get { return _pt1; }
                set { _pt1 = value; }
            }
            public double Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }
            public bool UseShortLines { get; set; }
            #endregion

            #region PicDrawable overrides
            public override Box2D ComputeBox(Transform2D transform)
            {
                Box2D box = Box2D.Initial;
                box.Extend(transform.transform(_pt0));
                box.Extend(transform.transform(_pt1));
                GetOffsetPoints(out Vector2D pt2, out Vector2D pt3);
                box.Extend(transform.transform(pt2));
                box.Extend(transform.transform(pt3));
                return box;
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

            #region Protected virtual methods
            public virtual void GetOffsetPoints(out Vector2D pt2, out Vector2D pt3)
            {
                // build normal
                Vector2D vI = (_pt1 - _pt0); vI.Normalize();
                Vector2D vJ = new Vector2D(vI.Y, -vI.X);
                double delta = -GlobalCotationProperties.Hrap;

                pt2 = new Vector2D(_pt0 + vJ * _offset + Math.Sign(_offset) * delta * vJ);
                pt3 = new Vector2D(_pt1 + vJ * _offset + Math.Sign(_offset) * delta * vJ);
            }
            protected virtual bool IsMiddleQuotation(Vector2D pt2, Vector2D pt3, double textWidth, double textHeight)
            {
                double length = (pt3 - pt2).GetLength();
                if (Math.Abs(pt3.Y - pt2.Y) < (textHeight / textWidth) * Math.Abs(pt3.X - pt2.X))
                    return Math.Abs((pt3 - pt2).X) > textWidth * GlobalCotationProperties.LengthCoef;
                else
                    return Math.Abs((pt3 - pt2).Y) > textHeight * GlobalCotationProperties.LengthCoef;
            }

            protected virtual void DrawArrowLine(PicGraphics graphics
                , Vector2D pt2, Vector2D pt3, double textWidth, double textHeight, out Vector2D ptText, out bool middle)
            {
                double length = (pt3 - pt2).GetLength();
                if (Math.Abs( pt3.Y - pt2.Y ) < (textHeight / textWidth) * Math.Abs(pt3.X - pt2.X))
                    middle = Math.Abs((pt3-pt2).X) > textWidth * GlobalCotationProperties.LengthCoef;
                else
                    middle = Math.Abs((pt3-pt2).Y) > textHeight * GlobalCotationProperties.LengthCoef;

                if (middle)
                {
                    ptText = 0.5 * (pt2 + pt3);
                    if (Math.Abs((pt3 - pt2).X) > 0)
                    {
                        Vector2D pt2_ = pt2.X < pt3.X ? pt2 : pt3;
                        Vector2D pt3_ = pt2.X < pt3.X ? pt3 : pt2;

                        if (Math.Abs((pt3.Y - pt2.Y) / (pt3.X - pt2.X)) < textHeight / textWidth)
                        {
                            graphics.DrawLine(LineType, pt2_, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.X - pt2_.X - textWidth) / length);
                            graphics.DrawLine(LineType, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.X - pt2_.X + textWidth) / length, pt3_);
                        }
                        else
                        {
                            graphics.DrawLine(LineType, pt2_, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.Y - pt2_.Y - textHeight) / length);
                            graphics.DrawLine(LineType, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.Y - pt2_.Y + textHeight) / length, pt3_);
                        }
                    }
                    else
                    {
                        ptText = 0.5 * (pt2 + pt3);
                        graphics.DrawLine(LineType, pt2, pt2 + (pt3 - pt2) * 0.5 * (length - textHeight) / length);
                        graphics.DrawLine(LineType, pt2 + (pt3 - pt2) * 0.5 * (length + textHeight) / length, pt3);
                    }
                }
                else
                {
                    if (Math.Abs((pt3 - pt2).X) > 0)
                    {
                        Vector2D pt2_ = pt2.X < pt3.X ? pt2 : pt3;
                        Vector2D pt3_ = pt2.X < pt3.X ? pt3 : pt2;
                        graphics.DrawLine(LineType, pt2_, pt2_ + (pt3_ - pt2_) * GlobalCotationProperties.LengthCoef);
                        if (Math.Abs((pt3.Y - pt2.Y) / (pt3.X - pt2.X)) < textHeight / textWidth)
                            ptText = pt2_ + (pt3_ - pt2_) * ((GlobalCotationProperties.LengthCoef * (pt3_-pt2_).X + 0.5 * textWidth) / length);
                        else
                            ptText = pt2_ + (pt3_ - pt2_) * ((GlobalCotationProperties.LengthCoef * (pt3_ - pt2_).Y + 0.5 * textHeight) / length);
                    }
                    else
                    {
                        // text is on top
                        graphics.DrawLine(LineType, pt2, pt2 + (pt3 - pt2) * GlobalCotationProperties.LengthCoef);
                        ptText = pt2 + (pt3 - pt2) * ((GlobalCotationProperties.LengthCoef * length + 0.5 * textHeight) / length);
                    }
                }
            }
            protected virtual void DrawArrowLineSeg(ref List<Segment> segments,
                Vector2D pt2, Vector2D pt3, double textWidth, double textHeight, out Vector2D ptText, out bool middle)
            {
                double length = (pt3 - pt2).GetLength();
                if (Math.Abs( pt3.Y - pt2.Y ) < (textHeight / textWidth) * Math.Abs(pt3.X - pt2.X))
                    middle = Math.Abs((pt3-pt2).X) > textWidth * GlobalCotationProperties.LengthCoef;
                else
                    middle = Math.Abs((pt3-pt2).Y) > textHeight * GlobalCotationProperties.LengthCoef;

                if (middle)
                {
                    ptText = 0.5 * (pt2 + pt3);
                    if (Math.Abs((pt3 - pt2).X) > 0)
                    {
                        Vector2D pt2_ = pt2.X < pt3.X ? pt2 : pt3;
                        Vector2D pt3_ = pt2.X < pt3.X ? pt3 : pt2;

                        if (Math.Abs((pt3.Y - pt2.Y) / (pt3.X - pt2.X)) < textHeight / textWidth)
                        {
                            segments.Add(new Segment(pt2_, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.X - pt2_.X - textWidth) / length));
                            segments.Add(new Segment(pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.X - pt2_.X + textWidth) / length, pt3_));
                        }
                        else
                        {
                            segments.Add(new Segment( pt2_, pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.Y - pt2_.Y - textHeight) / length));
                            segments.Add(new Segment( pt2_ + (pt3_ - pt2_) * 0.5 * (pt3_.Y - pt2_.Y + textHeight) / length, pt3_));
                        }
                    }
                    else
                    {
                        ptText = 0.5 * (pt2 + pt3);
                        segments.Add(new Segment( pt2, pt2 + (pt3 - pt2) * 0.5 * (length - textHeight) / length));
                        segments.Add(new Segment( pt2 + (pt3 - pt2) * 0.5 * (length + textHeight) / length, pt3));
                    }
                }
                else
                {
                    if (Math.Abs((pt3 - pt2).X) > 0)
                    {
                        Vector2D pt2_ = pt2.X < pt3.X ? pt2 : pt3;
                        Vector2D pt3_ = pt2.X < pt3.X ? pt3 : pt2;
                        segments.Add(new Segment( pt2_, pt2_ + (pt3_ - pt2_) * GlobalCotationProperties.LengthCoef));
                        if (Math.Abs((pt3.Y - pt2.Y) / (pt3.X - pt2.X)) < textHeight / textWidth)
                            ptText = pt2_ + (pt3_ - pt2_) * ((GlobalCotationProperties.LengthCoef * (pt3_-pt2_).X + 0.5 * textWidth) / length);
                        else
                            ptText = pt2_ + (pt3_ - pt2_) * ((GlobalCotationProperties.LengthCoef * (pt3_ - pt2_).Y + 0.5 * textHeight) / length);
                    }
                    else
                    {
                        // text is on top
                        segments.Add(new Segment( pt2, pt2 + (pt3 - pt2) * GlobalCotationProperties.LengthCoef));
                        ptText = pt2 + (pt3 - pt2) * ((GlobalCotationProperties.LengthCoef * length + 0.5 * textHeight) / length);
                    }
                }            
            
            }
            #endregion

            #region Public creation method
            public static PicCotation CreateNewCotation(uint id, Vector2D pt0, Vector2D pt1, double offset, short noDecimals)
            {
                return new PicCotationDistance(id, pt0, pt1, offset, noDecimals);
            }
            #endregion

            #region PicTypedDrawable overrides
            protected override bool Evaluate()
            {
                _box.Reset();
                _box.Extend(_pt0);
                _box.Extend(_pt1);
                GetOffsetPoints(out Vector2D pt2, out Vector2D pt3);
                _box.Extend(pt2);
                _box.Extend(pt3);
                return true;
            }
            protected override void DrawSpecific(PicGraphics graphics)
            {
                // get text size
                graphics.GetTextSize(Text, PicGraphics.TextType.FT_COTATION, GlobalCotationProperties.FontSize, out double textWidth, out double textHeight);
                // offset points
                GetOffsetPoints(out Vector2D pt2, out Vector2D pt3);
                // find out if is middle quotation
                bool middle = IsMiddleQuotation(pt2, pt3, textWidth, textHeight);
                if (!middle && (!GlobalCotationProperties._showDeportedCotations || Auto))
                    return;

                // draw extremity segments
                if (UseShortLines || PicGlobalCotationProperties.ShowShortCotationLines)
                {
                    double arrowLength = graphics.DX_inv(GlobalCotationProperties.ArrowLength);

                    Vector2D vecI = _pt0-pt2;
                    vecI.Normalize();
                    vecI *= arrowLength;
                    graphics.DrawLine(LineType, pt2 + vecI, pt2 - vecI);
                    graphics.DrawLine(LineType, pt3 + vecI, pt3 - vecI);
                }
                else
                {
                    graphics.DrawLine(LineType, _pt0, pt2);
                    graphics.DrawLine(LineType, _pt1, pt3);
                }
                // draw arrow line
                DrawArrowLine(graphics, pt2, pt3, textWidth, textHeight, out Vector2D ptText, out middle);
                // draw text
                graphics.DrawText(Text, PicGraphics.TextType.FT_COTATION, GlobalCotationProperties.FontSize, ptText, PicGraphics.HAlignment.HA_CENTER, PicGraphics.VAlignment.VA_MIDDLE, TextDirection);
                // draw arrows heads
                DrawArrowHead(graphics, pt2, (middle ? 1.0 : -1.0) * (pt2 - pt3));
                DrawArrowHead(graphics, pt3, (middle ? 1.0 : -1.0) * (pt3 - pt2));
            }
            protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
            {                
            }
            public override void Transform(Transform2D transform)
            {
                _pt0 = transform.transform(_pt0);
                _pt1 = transform.transform(_pt1);
                SetModified();
            }
            public override PicEntity Clone(IEntityContainer factory)
            {
                return new PicCotationDistance(factory.GetNewEntityId(), new Vector2D(_pt0), new Vector2D(_pt1), _offset, _noDecimals);
            }
            protected override ECode GetCode()
            {
                return ECode.PE_COTATIONDISTANCE;
            }
            public override double Value()
            {
                return (_pt1 - _pt0).GetLength();
            }
            public override double Length
            {
                get
                {
                    GetOffsetPoints(out Vector2D pt2, out Vector2D pt3);
                    return (_pt0 - pt2).GetLength() + (_pt1 - pt3).GetLength() + (pt3 - pt2).GetLength();
                }
            }
            #endregion

            #region Helpers for dxf, ai, cf2 export
            public void DrawSeg(ref List<Segment> segments, ref Vector2D textPt, ref double textSize)
            {
                textSize = 10.0;

                // get text size
                double textWidth = textSize, textHeight = textSize;
                
                // offset points
                Vector2D pt2, pt3;
                GetOffsetPoints(out pt2, out pt3);
                // find out if is middle quotation
                bool middle = IsMiddleQuotation(pt2, pt3, textWidth, textHeight);
                if (!middle && !GlobalCotationProperties._showDeportedCotations)
                    return;

                // draw extremity segments
                if (UseShortLines || PicGlobalCotationProperties.ShowShortCotationLines)
                {
                    double arrowLength = 10.0; // no pixel size -> ?

                    Vector2D vecI = _pt0 - pt2;
                    vecI.Normalize();
                    vecI *= arrowLength;
                    segments.Add(new Segment(pt2 + vecI, pt2 - vecI));
                    segments.Add(new Segment(pt3 + vecI, pt3 - vecI));
                }
                else
                {
                    segments.Add(new Segment(_pt0, pt2));
                    segments.Add(new Segment(_pt1, pt3));
                }
                // draw arrow line
                DrawArrowLineSeg(ref segments, pt2, pt3, textWidth, textHeight, out textPt, out middle);
                // draw arrows heads
                DrawArrowHeadSeg(ref segments, pt2, (middle ? 1.0 : -1.0) * (pt2 - pt3));
                DrawArrowHeadSeg(ref segments, pt3, (middle ? 1.0 : -1.0) * (pt3 - pt2));
            }
            #endregion

            #region System.Object overrides
            /// <returns>A string representation of this object.</returns>
            public override string ToString()
            {
                return string.Format("PicCotationDistance id = {0}, ext0={1}, ext1{2}, offset={3}\n"
                    , this.Id, _pt0, _pt1, _offset);
            }
            #endregion
        }
    }
}
