#region Using directives
using System;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
#endregion

namespace Pic
{
    namespace Factory2D
    {
        public class PicArc : PicTypedDrawable 
        {
            #region Protected Constructors
            protected PicArc(uint id, PicGraphics.LT lType)
                : base(id, lType)
            {
            }
            #endregion

            #region Pic.Factory.PicEntity Overrides
            protected override ECode GetCode()
            {
                return ECode.PE_ARC;
            }
            #endregion

            #region PicDrawable overrides
            public override Box2D ComputeBox(Transform2D transform)
            {
                // instantiate bounding box
                Box2D box = Box2D.Initial;

                // 6 points to be considered
                // arc extremities
                _box.Extend(transform.transform(PointAtAngle(AngleBegActual)));
                _box.Extend(transform.transform(PointAtAngle(AngleEndActual)));
                // horizontal and vertical tangent points
                double[] tab = new double[] { 0.0, 90.0, 180.0, 270.0 };
                foreach (double d in tab)
                    if (PointOnArc(d))
                        box.Extend(transform.transform(PointAtAngle(d)));
                return box;
            }

            public override Segment[] Segments
            {
                get
                {
                    double angleStep = 5.0;
                    int iNoStep = (int)((AngleEnd - AngleBeg) / angleStep);
                    if (iNoStep < 1) iNoStep = 1;
                    angleStep = (AngleEnd - AngleBeg) / iNoStep;

                    Segment[] segs = null;
                    if (LineType == PicGraphics.LT.LT_CUT)
                    {
                        segs = new Segment[iNoStep];
                        for (int i = 0; i < iNoStep; ++i)
                        {
                            Vector2D pt0 = PointAtAngle(AngleBeg + i * angleStep);
                            Vector2D pt1 = PointAtAngle(AngleBeg + (i + 1) * angleStep);
                            segs[i] = new Segment(pt0, pt1);
                        }
                    }
                    else
                        segs = new Segment[0];
                    return segs;
                }
            }
            #endregion

            #region Public Methods
            public static PicArc CreateNewArc(uint id, PicGraphics.LT lType, Vector2D center, double radius, double angleBeg, double angleEnd)
            {
                PicArc arc = new PicArc(id, lType)
                {
                    Center = center,
                    Radius = radius,
                    AngleBeg = angleBeg,
                    AngleEnd = angleEnd
                };
                return arc;
            }
            public static PicArc CreateNewArc(uint id, PicGraphics.LT lType, Vector2D center, Vector2D pt0, Vector2D pt1)
            {
                PicArc arc = new PicArc(id, lType)
                {
                    Center = center,
                    Radius = (pt0 - center).GetLength(),
                    AngleBeg = VecToAngleDeg(pt0 - center),
                    AngleEnd = VecToAngleDeg(pt1 - center)
                };
                while (arc.AngleBeg < 0.0)                 arc.AngleBeg += 360.0;
                while (arc.AngleEnd < arc.AngleBeg)       arc.AngleEnd += 360.0;
                return arc;
            }
            public double Angle(Vector2D pt0, Vector2D pt1)
            {
                bool sign = (pt0 - Source).GetLength() < 1.0E-03;
                return (sign ? 1.0 : -1.0 )* (AngleEnd - AngleBeg);
            }
			#endregion

            #region Private methods
            protected Vector2D PointAtAngle(double angle)
            { 
                return new Vector2D(
                    Center.X + Radius * Math.Cos(angle * Math.PI / 180.0),
                    Center.Y + Radius * Math.Sin(angle * Math.PI / 180.0) );
            }
            protected bool PointOnArc(double angle)
            {
                double openingAngle = AngleEnd - AngleBeg;
                if (openingAngle >= 0)
                {
                    return ((AngleBeg <= angle) && (angle - AngleBeg <= openingAngle))
                        || ((AngleBeg > 360.0) && (angle + 360.0 < AngleEnd) && (AngleEnd - angle - 360.0 <= openingAngle));
                }
                else
                    return (angle <= AngleBeg) && (angle - AngleBeg >= openingAngle);
            }

            // floating point remainder function
            static protected double Fmod(double value, double mod)
            {
                // he fmod() function returns the value x - i * y,
                //for some integer i such that, if y is non-zero,
                // the result has the same sign as x and magnitude less than the magnitude of y.
                double i = Math.Floor(Math.Abs(value/mod));
                return Math.Sign(value)*(Math.Abs(value) - i * Math.Abs(mod));
            }
            static protected double Mod360R(double ang)
            {
                ang = Fmod( ang , 360.0) ;
                const double epsilon = 0.001;
	            if(ang >= 360 - epsilon)
		            ang = 0.0;
	            else if ((ang > -epsilon) && (ang < epsilon))
			        ang = 0.0;
		        else if ((ang > 90.0 - epsilon) && (ang < 90.0 + epsilon))
				    ang = 90.0;
			    else if ((ang > 270.0 - epsilon) && (ang < 270.0 + epsilon))
				    ang = 270.0;
				else if ((ang > 180.0 - epsilon) && (ang < 180.0 + epsilon))
				    ang = 180.0;
                return ang;
            }

            static public double VecToAngleDeg(Vector2D vec)
            {
                return VecToAngleRad(vec) * 180.0 / Math.PI;    
            }

            static public double VecToAngleRad(Vector2D vec)
            {
                // handling null vector case
                const double epsilon = 0.001;
                if (vec.GetLengthSquared() < epsilon) return 0.0;
                // non null vector
                vec.Normalize();
                if (vec.Y >= 0.0)
                    return Math.Acos(vec.X);
                else
                    return 2.0 * Math.PI - Math.Acos(vec.X);
            }
            #endregion

            #region Overrides
            protected override void DrawSpecific(PicGraphics graphics)
			{
				graphics.DrawArc(LineType, Center, Radius, AngleBeg, AngleEnd);
			}
            protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
            {                
                Vector2D vecCenterTransformed = Vector2D.Zero;
                double angleBeg = 0.0, angleEnd = 0.0;
                TransformData(transform, Radius, Center, AngleBeg, AngleEnd
                    , ref vecCenterTransformed, ref angleBeg, ref angleEnd);

                graphics.DrawArc(
                    LineType
                    , vecCenterTransformed
                    , Radius
                    , angleBeg
                    , angleEnd
                    );
            }
			protected override bool Evaluate()
			{
                _box.Reset();
                // 6 points to be considered
                // arc extremities
                _box.Extend(PointAtAngle(AngleBeg));
				_box.Extend(PointAtAngle(AngleEnd));
                // horizontal and vertical tangent points
                double[] tab = new double[] { 0.0, 90.0, 180.0, 270.0 };
                foreach (double d in tab)
                    if (PointOnArc(d))
                        _box.Extend(PointAtAngle(d));
 				return true;
			}
            public override void Transform(Transform2D transform)
            {
                Vector2D[] vec4Pts = FourPointDefinition;
                int index = 0;
                foreach (Vector2D vec in vec4Pts)
                    vec4Pts[index++] = transform.transform(vec);
                FourPointDefinition = vec4Pts;
                SetModified();
            }
            public override PicEntity Clone(IEntityContainer factory)
            {
                return new PicArc(factory.GetNewEntityId(), LineType)
                {
                    Center = Center,
                    Radius = Radius,
                    AngleBeg = AngleBeg,
                    AngleEnd = AngleEnd
                };
            }
            public override double Length
            {
                get { return Radius * Math.PI * Math.Abs(AngleEnd - AngleBeg) / 180.0; }
            }
            #endregion

            #region Public properties
            public Vector2D Center { get; private set; }
            public double Radius { get; private set; }
            public double AngleBeg { get; private set; }
            public double AngleEnd { get; private set; }
            public Vector2D Source
            {
                get
                {
                    return new Vector2D(
                        Center.X + Radius * Math.Cos(AngleBeg * Math.PI / 180.0)
                        , Center.Y + Radius * Math.Sin(AngleBeg * Math.PI / 180.0)
                        );
                }
                set
                {
                    double ax = (value.X - Center.X) / Radius;
                    double ay = (value.Y - Center.Y) / Radius;

                    if (ay >= 0.0)
                        AngleBeg = Math.Acos(ax) * 180.0 / Math.PI;
                    else
                        AngleBeg = 360.0 - Math.Acos(ax) * 180.0 / Math.PI;
                }
            }
            public Vector2D Target
            {
                get
                {
                    return new Vector2D(
                        Center.X + Radius * Math.Cos(AngleEnd * Math.PI / 180.0)
                        , Center.Y + Radius * Math.Sin(AngleEnd * Math.PI / 180.0)
                        );
                }
                set
                {
                    double ax = (value.X - Center.X) / Radius;
                    double ay = (value.Y - Center.Y) / Radius;

                    if (ay >= 0.0)
                        AngleEnd = Math.Acos(ax) * 180.0 / Math.PI;
                    else
                        AngleEnd = 360.0 - Math.Acos(ax) * 180.0 / Math.PI;
                }
            }
            public void Swap()
            {
                double temp = AngleBeg;
                AngleBeg = AngleEnd;
                AngleEnd = temp;
            }

            public void Complement()
            {
                double openingAngle = AngleEnd - AngleBeg;
                if (openingAngle > 0.0)
                {
                    AngleBeg = AngleEnd;
                    AngleEnd = AngleBeg + 360.0 - openingAngle;
                }
                else
                {
                    AngleEnd = AngleBeg + 360.0 + openingAngle;
                }
            }

            public Vector2D[] FourPointDefinition
            {
                get
                {
                    Vector2D[] points = new Vector2D[4];
                    points[0] = Center;
                    points[1] = PointAtAngle(AngleBeg);
                    points[2] = PointAtAngle(0.5 * (AngleBeg + AngleEnd));
                    points[3] = PointAtAngle(AngleEnd);
                    return points;
                }
                set
                {
                    Center = value[0];
                    Radius = (value[1] - value[0]).GetLength();
                    AngleBeg = VecToAngleDeg(value[1]-value[0]);
                    AngleEnd = VecToAngleDeg(value[3]-value[0]);
                    Vector2D ptDiff =  PointAtAngle(0.5 * (AngleEnd +AngleBeg)) - value[2];
                    if (ptDiff.GetLength()/Radius > 0.01)
                        Complement();
                }
            }
            #endregion

            #region Private properties
            private double AngleBegActual
            {
                get { return (AngleBeg < AngleEnd ? AngleBeg : AngleEnd); }
            }
            private double AngleEndActual
            {
                get { return (AngleBeg < AngleEnd ? AngleEnd : AngleBeg); }
            }
            #endregion

            #region System.Object Overrides
            public override string ToString()
            {
                return string.Format("Arc id = {0}, center = {1}, radius = {2}, angleBeg = {3}, angleEnd = {4}\n"
                    , Id, Center, Radius, AngleBeg, AngleEnd);
            }
            #endregion

            #region Helpers
            private void TransformData(Transform2D transf, double radius, Vector2D center, double angleBeg, double angleEnd
                , ref Vector2D centerOut, ref double angleBegOut, ref double angleEndOut)
            { 
                centerOut = transf.transform(center);
                Vector2D ptBeg = transf.transform(PointAtAngle(center, radius, angleBeg));
                Vector2D ptEnd = transf.transform(PointAtAngle(center, radius, angleEnd));
                Vector2D ptIntExpected = transf.transform(PointAtAngle(center, radius, 0.5 * (angleBeg + angleEnd)));
                angleBegOut = VecToAngleDeg(ptBeg - centerOut);
                angleEndOut = VecToAngleDeg(ptEnd - centerOut);
                Vector2D ptInt = PointAtAngle(centerOut, radius, 0.5 * (angleBegOut + angleEndOut));
                if ((ptInt - ptIntExpected).GetLength() / radius > 0.01)
                {
                    double openingAngle = angleEndOut - angleBegOut;
                    if (openingAngle > 0)
                    {
                        angleBegOut = angleEndOut;
                        angleEndOut = angleBegOut + 360.0 - openingAngle;
                    }
                    else
                    {
                        angleEndOut = angleBegOut + 360.0 + openingAngle;
                    }
                }

            }
            private Vector2D PointAtAngle(Vector2D center, double radius, double angle)
            {
                return new Vector2D(
                    center.X + radius * Math.Cos(angle * Math.PI / 180.0),
                    center.Y + radius * Math.Sin(angle * Math.PI / 180.0));
            }
            #endregion
        }
    }
}
