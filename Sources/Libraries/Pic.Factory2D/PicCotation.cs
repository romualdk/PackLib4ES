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
        #region PicGlobalCotationProperties
        public class PicGlobalCotationProperties
        {
            #region Constructor
            public PicGlobalCotationProperties()
            {
                Hrap = 1.0;
                ArrowLength = 5.0F;
                ArrowHeadAngle = 30.0;
                LengthCoef = 1.4;
            }
            #endregion

            #region Change notification
            public delegate void OnGlobalCotationPropertiesModified();
            public static event OnGlobalCotationPropertiesModified Modified;
            #endregion

            #region Public properties
            public static bool ShowShortCotationLines
            {
                get { return _showShortLines; }
                set
                {
                    _showShortLines = value;
                    Modified?.Invoke();
                }
            }
            public bool ShowDeportedCotations
            {
                get { return _showDeportedCotations; }
                set
                {
                    _showDeportedCotations = value;
                    Modified?.Invoke();
                }
            }

            public double LengthCoef { get; set; }
            public double Hrap { get; set; }
            public double ArrowLength { get; set; }
            public double ArrowHeadAngle { get; set; }
            public float FontSize { get; set; } = 8.0f;

            #endregion
            #region Public fields
            public bool _showDeportedCotations = true;
            static bool _showShortLines = true;
            #endregion
        }
        #endregion

        public abstract class PicCotation : PicTypedDrawable
        {
            #region Public enums
            public enum CotType
            {
                COT_DISTANCE
                , COT_HORIZONTAL
                , COT_VERTICAL
                , COT_RADIUS_INT
                , COT_RADIUS_EXT
            }

            public enum CotBBType
            {
                COT_NONE,
                COT_LOWERLEFT,
                COT_LOWERRIGHT,
                COT_UPPERRIGHT,
                COT_UPPERLEFT
            }
            #endregion

            #region Private fields
            private string _text;
            protected short _noDecimals;
            #endregion

            #region Global cotation properties
            static public PicGlobalCotationProperties GlobalCotationProperties = new PicGlobalCotationProperties();
            #endregion

            #region Protected Constructors
            protected PicCotation(uint id, short noDecimals)
                : base(id, PicGraphics.LT.LT_COTATION)
            {
                _noDecimals = noDecimals;
            }
            #endregion

            #region Helpers
            protected void DrawArrowHead(Pic.Factory2D.PicGraphics graphics, Vector2D pt, double angle)
            {
                DrawArrowHead(graphics, pt, new Vector2D(Math.Cos(angle * Math.PI / 180.0), Math.Sin(angle * Math.PI / 180.0)));
            }
            protected void DrawArrowHead(Pic.Factory2D.PicGraphics graphics, Vector2D pt, Vector2D director)
            {
                director.Normalize();
                Vector2D normal = new Vector2D(-director.Y, director.X);

                double angleRad = GlobalCotationProperties.ArrowHeadAngle * Math.PI / 180.0;
                double arrowLength = graphics.DX_inv(GlobalCotationProperties.ArrowLength);

                // lower part of arrow head
                graphics.DrawLine(LineType
                    , pt - arrowLength * (Math.Cos(angleRad) * director + Math.Sin(angleRad) * normal)
                    , pt);
                // upper part of arrow head
                graphics.DrawLine(LineType
                    , pt - arrowLength * (Math.Cos(angleRad) * director - Math.Sin(angleRad) * normal)
                    , pt);
            }
            protected void DrawArrowHeadSeg(ref List<Segment> segments, Vector2D pt, Vector2D director)
            {
                director.Normalize();
                Vector2D normal = new Vector2D(-director.Y, director.X);

                double angleRad = GlobalCotationProperties.ArrowHeadAngle * Math.PI / 180.0;
                double arrowLength = GlobalCotationProperties.ArrowLength;

                segments.Add(new Segment(pt - arrowLength * (Math.Cos(angleRad) * director + Math.Sin(angleRad) * normal), pt));
                segments.Add(new Segment(pt - arrowLength * (Math.Cos(angleRad) * director - Math.Sin(angleRad) * normal), pt));
            }
            #endregion

            #region Public properties
            public abstract double Value();

            public string Text
            {
                get
                {
                    if (null != _text && _text.Length > 0)
                        return _text;
                    else
                    {
                        string pSpecifier = string.Format("f{0}", _noDecimals);
                        double value = Value();
                        return value.ToString(pSpecifier); 
                    }
                }
                set { _text = value; }
            }
            /// <summary>
            /// gets text direction
            /// </summary>
            public virtual float TextDirection => 0.0f;
            /// <summary>
            /// This field needs to be set to true when the cotation is meant 
            /// to be an automatic cotation.
            /// </summary>
            public bool Auto { get; set; } = false;
            #endregion

            #region Cotation bounding box
            public static bool GetBBCotations(
                Box2D bbox, CotBBType cotBBType, double offsetRatio,
                out Vector2D pt0, out Vector2D pt1, out Vector2D pt2, out Vector2D pt3,
                out double delta0, out double delta1)
            {
                double deltaHoriz = offsetRatio * bbox.Width;
                double deltaVert = offsetRatio * bbox.Height;

                Vector2D v0 = bbox.PtMin;
                Vector2D v1 = new Vector2D(bbox.XMax, bbox.XMin);
                Vector2D v2 = bbox.PtMax;
                Vector2D v3 = new Vector2D(bbox.XMin, bbox.YMax);

                switch (cotBBType)
                {
                    case CotBBType.COT_LOWERLEFT:
                        pt0 = v0; pt1 = v1;  pt2 = v3; pt3 = v0;
                        delta0 = -deltaVert; delta1 = deltaHoriz;
                        return true;
                    case CotBBType.COT_LOWERRIGHT:
                        pt0 = v0; pt1 = v1; pt2 = v1; pt3 = v2;
                        delta0 = -deltaVert; delta1 = -deltaHoriz;
                        return true;
                    case CotBBType.COT_UPPERRIGHT:
                        pt0 = v2; pt1 = v3; pt2 = v1; pt3 = v2;
                        delta0 = deltaVert; delta1 = -deltaHoriz;
                        return true;
                    case CotBBType.COT_UPPERLEFT:
                        pt0 = v2; pt1 = v3; pt2 = v3; pt3 = v0;
                        delta0 = deltaVert; delta1 = deltaHoriz;
                        return true;
                    default:
                        pt0 = Vector2D.Zero; pt1 = Vector2D.Zero; pt2 = Vector2D.Zero; pt3 = Vector2D.Zero;
                        delta0 = 0.0; delta1 = 0.0;
                        return false;
                }
            }
            #endregion
        }
    }
}
