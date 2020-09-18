#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using Sharp3D.Math.Core;

using log4net;
#endregion

namespace Pic.Factory2D
{
    public abstract class PicGraphicsGdiPlus : PicGraphics
    {
        #region Protected and private fields
        private Graphics _gdiGraphics;
        private Dictionary<LT, Pen> _ltToPen = new Dictionary<LT, Pen>();
        protected static ILog _log = LogManager.GetLogger(typeof(PicGraphicsGdiPlus));
        #endregion

        #region Protected constructors
        protected PicGraphicsGdiPlus()
        {
            BackgroundColor = Color.White;
        }
        #endregion

        #region Pic.Factory2D.PicGraphics overrides
        public override void Initialize()
        {
            if (null == GdiGraphics)
                throw new Exception("PicGraphics size must be set before attempting to draw.");
            InitializeLineTypeToPenDictionary();
            PicCotation.GlobalCotationProperties.ArrowLength = 5.0;
        }
        private void InitializeLineTypeToPenDictionary()
        {
            _ltToPen.Clear();

            _ltToPen.Add(LT.LT_CUT, new Pen(ColorTranslator.FromWin32(255), 1.0f));
            _ltToPen.Add(LT.LT_PERFOCREASING, new Pen(ColorTranslator.FromWin32(16711680), 1.0f));
            float[] dashValuesPerfoCreasing = { 14.0f, 3.0f };
            _ltToPen[LT.LT_PERFOCREASING].DashPattern = dashValuesPerfoCreasing;
            _ltToPen.Add(LT.LT_CONSTRUCTION, new Pen(ColorTranslator.FromWin32(7798903), 1.0f));
            _ltToPen.Add(LT.LT_PERFO, new Pen(ColorTranslator.FromWin32(255), 1.0f));
            float[] dashValuesPerfo = { 14.0f, 3.0f };
            _ltToPen[LT.LT_PERFO].DashPattern = dashValuesPerfo;
            _ltToPen.Add(LT.LT_HALFCUT, new Pen(ColorTranslator.FromWin32(16776960), 1.0f));
            _ltToPen.Add(LT.LT_CREASING, new Pen(ColorTranslator.FromWin32(16711680), 1.0f));
            _ltToPen.Add(LT.LT_AXIS, new Pen(ColorTranslator.FromWin32(16711680), 1.0f));
            float[] dashValuesAxis = { 3.0f, 4.0f, 10.0f, 4.0f };
            _ltToPen[LT.LT_AXIS].DashPattern = dashValuesAxis;
            _ltToPen.Add(LT.LT_COTATION, new Pen(Color.FromArgb(0, 140, 0), 1.0f));
            _ltToPen.Add(LT.LT_GRID, new Pen(ColorTranslator.FromWin32(8388608), 1.0f));


            // **
            // (1) :    spen="0,0,12,2,255,Coupant";
            // (2) :    spen="1,0,14,3,16711680,Perfo. Rainant";
            // (3) :    spen="0,1,5,6,7798903,Construction.";
            // (4) :    spen="1,0,4,1,255,Perfo";
            // (5) :    spen="0,0,11,5,16776960,Mi-chair";
            // (6) :    spen="0,0,14,3,16711680,Rainant";
            // (7) :    spen="3,0,9,3,16711680,Axe";
            // (8) :    spen="0,0,2,4,8453888,Cotation";
            // (9) :    spen="0,0,7,0,8421504,Origine";
            //(10) :    spen="0,0,1,0,8388608,Grille";
            //(11) :    spen="0,0,10,0,16742777,Ponts";
            //default:  spen="0,0,10,0,33023,Defaut";
            // **
        }
        /// <summary>
        /// Draw a point (actually a small cross) using defined line type
        /// </summary>
        /// <param name="lineType"></param>
        /// <param name="pt"></param>
        public override void DrawPoint(LT lineType, Vector2D pt)
        {
        }
        public override void DrawLine(LT lineType, Vector2D pointBeg, Vector2D pointEnd)
        {
            try
            { _gdiGraphics.DrawLine(ToPen(lineType), ToPointF(pointBeg), ToPointF(pointEnd)); }
            catch (Exception)
            {}
        }
        public override void DrawArc(LT lineType, Vector2D ptCenter, double radius, double angle0, double angle1)
		{
            if (radius > 0.0)
            {
                try
                {
                    _gdiGraphics.DrawArc(ToPen(lineType)
                        , X(ptCenter.X) - DX(radius)
                        , Y(ptCenter.Y) - DY(radius)
                        , (float)(2.0 * DX(radius))     // width
                        , (float)(2.0 * DY(radius))     // height
                        , -(float)angle0                // start angle
                        , -(float)(angle1 - angle0));   // sweep angle
                }
                catch (Exception)
                {
                }
            }
		}

        public override void DrawText(string text, TextType font, float emSize, Vector2D pt, HAlignment hAlignment, VAlignment vAlignment, float fAngle)
        {
            // string format
            var sf = new StringFormat();
            switch (hAlignment)
            {
                case HAlignment.HA_RIGHT:
                    sf.Alignment = StringAlignment.Far;
                    break;
                case HAlignment.HA_CENTER:
                    sf.Alignment = StringAlignment.Center;
                    break;
                case HAlignment.HA_LEFT:
                    sf.Alignment = StringAlignment.Near;
                    break;
                default:
                    throw new Exception("Unknown horizontal alignment");
            }
            switch (vAlignment)
            { 
                case VAlignment.VA_BOTTOM:
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case VAlignment.VA_MIDDLE:
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case VAlignment.VA_TOP:
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                default:
                    throw new Exception("Unknown vertical alignment");
            }
            // font & brush
            Brush tb;
            switch (font)
            { 
                case TextType.FT_COTATION:
                case TextType.FT_STRING:
                    tb = new SolidBrush(Color.FromArgb(0,140,0));
                    break;
                default:
                    throw new Exception("Unknown text type");
            }

            var ptF = ToPointF(pt /*- 0.5/0.75 * (emSize) * Vector2D.YAxis*/);
            _gdiGraphics.TranslateTransform(ptF.X, ptF.Y);
            _gdiGraphics.RotateTransform(fAngle); 
            
            // draw text
            _gdiGraphics.DrawString(text, ToFont(font, emSize), tb, new PointF(0, 0), sf);

            _gdiGraphics.ResetTransform();
         }

        public override void GetTextSize(string text, TextType font, float emSize, out double width, out double height)
        {
            Size preferredSize = _gdiGraphics.MeasureString(text, ToFont(font, emSize)).ToSize();
            width = DX_inv(preferredSize.Width);
            height = DY_inv(preferredSize.Height);
        }

        public override void Finish()
        {
        } 

        public override void ShowMessage(string message)
        {
            try
            {
                // draw text
                var sf = new StringFormat();
                var tb = new SolidBrush(Color.Red);
                _gdiGraphics.DrawString(message, new Font("Arial", 8), tb, new PointF(100.0f, 100.0f));
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("ShowMessage({0}) failed with error: {1}", message, ex.Message));
            }
        }
        #endregion

        #region Helpers
        public PointF ToPointF(Vector2D vec) => new PointF(X(vec.X), Y(vec.Y));
        public Pen ToPen(LT lineType) => _ltToPen[lineType];
        public Font ToFont(TextType textType, float emSize)
        {
            switch (textType)
            {
                case TextType.FT_COTATION:  return new Font("Arial", emSize, GraphicsUnit.Point);
                default: throw new Exception("Unexpected text type");
            }
        }
        public abstract Size GetSize();
        // coordinates transformation
        protected float X(double x) => (float)((x - Box.XMin) * GetSize().Width / Box.Width);
        protected float Y(double y) => (float)((Box.YMax - y) * GetSize().Height / Box.Height);
        protected float DX(double dx) => (float)(dx * GetSize().Width / (Box.Width));
        protected float DY(double dy) => (float)(dy * GetSize().Height / (Box.Height));
        public override float FontSizePt(float fontSizeUnit) => 0.75f * (float)(fontSizeUnit * GetSize().Height / (Box.Height));
        public override Vector2D OffsetFont(float unitSize, VAlignment vAlignment)
        {
            switch (vAlignment)
            {
                case VAlignment.VA_BOTTOM:  return 0.75 * new Vector2D(0.0, -0.49 * unitSize);
                case VAlignment.VA_TOP: return new Vector2D(0.0, 0.2 * unitSize);
                default:
                    return Vector2D.Zero;
            }
        }

        public override double DX_inv(double dX) => dX * Box.Width / GetSize().Width;
        public override double DY_inv(double dY) => dY * Box.Height / GetSize().Height;
        protected Vector2D ConvertMouseCoordinates(Point pt)=> new Vector2D(Box.XMin + pt.X * Box.Width / GetSize().Width, Box.YMax - pt.Y * (Box.Height) / GetSize().Height);

        // set world coordinate drawing box
        public override Box2D DrawingBox
        {
			set
            {
                double width = value.Width;
                double height = value.Height;
                double xmin, ymin, xmax, ymax;
                if ((width / GetSize().Width) > (height / GetSize().Height))
                {	// width is leading
                    xmin = value.XMin;
                    xmax = value.XMax;
                    height = (width * GetSize().Height) / GetSize().Width;
                    ymin = (value.YMin + value.YMax) * 0.5 - height * 0.5;
                    ymax = (value.YMin + value.YMax) * 0.5 + height * 0.5;
                }
                else
                {	// height is leading
                    ymin = value.YMin;
                    ymax = value.YMax;
                    width = (height * GetSize().Width) / GetSize().Height;
                    xmin = (value.XMin + value.XMax) * 0.5 - width * 0.5;
                    xmax = (value.XMin + value.XMax) * 0.5 + width * 0.5;
                }
                Box = new Box2D(xmin, ymin, xmax, ymax);
            }
            get { return Box; }
        }
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return base.ToString();
        }
        #endregion

        #region Public properties
        public Color BackgroundColor { get; set; }
        public Graphics GdiGraphics
        {
            set
            {
                try
                {
                    _gdiGraphics = value;
                    if (BackgroundColor.IsKnownColor)
                        _gdiGraphics.Clear(BackgroundColor);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
            get => _gdiGraphics;
        }

        #endregion
    }
}
