#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorComputeLength : PicFactoryVisitor, IDisposable
    {
        #region Data members
        private double _length;
        private List<Segment> _segments = new List<Segment>();
        #endregion

        #region Constructor
        public PicVisitorComputeLength()
        { 
        }
        #endregion

        #region Override PicFactoryVisitor
        public override void Initialize(PicFactory factory)
        {            
        }
        public override void ProcessEntity(PicEntity entity)
        { 
            PicDrawable drawable = entity as PicDrawable;
            if (null == drawable) return;
            PicBlock picBlock = drawable as PicBlock;
            if (null != picBlock)
            {
                foreach (PicEntity e in picBlock)
                {
                    if (e is PicTypedDrawable)
                        AddEntityLength(e as PicTypedDrawable, Transform2D.Identity);
                }
            }
            PicBlockRef blockRef = drawable as PicBlockRef;
            if (null != blockRef)
            {
                PicBlock block = blockRef.Block;
                foreach (PicEntity e in block)
                {
                    if (e is PicTypedDrawable)
                        AddEntityLength(e as PicTypedDrawable, blockRef.BlockTransformation);
                }
            }
            else if (drawable is PicTypedDrawable)
                AddEntityLength(drawable as PicTypedDrawable, Transform2D.Identity);
        }
        public override void Finish()
        {
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        { 
        }
        #endregion

        #region Specific methods
        private void AddEntityLength(PicTypedDrawable drawable, Transform2D transf)
        {
            if (drawable.LineType != PicGraphics.LT.LT_CUT)
                return;
            const double epsilon = 1.0e-03;
            PicSegment seg = drawable as PicSegment;
            if (null != seg)
            {
                Segment s1Init = new Segment( transf.transform(seg.Pt0), transf.transform(seg.Pt1) );

                // build a list for current segment
                List<Segment> segmentCurrent = new List<Segment>();
                segmentCurrent.Add(s1Init);

                foreach (Segment s0 in _segments)
                {
                    // are segment colinear ?
                    if (!SegmentMethods.AreSegmentsColinear(s0, s1Init, epsilon))
                        continue;
                    // get non-overlapping segments ?
                    for (int i1 = 0; i1 < segmentCurrent.Count; ++i1)
                    {
                        Segment s1 = segmentCurrent[i1];
                        // is s1.P0 on s0
                        double coordP0 = Vector2D.DotProduct(s1.P0 - s0.P0, s0.P1 - s0.P0) / (s0.P1 - s0.P0).GetLengthSquared();
                        // is s1.P1 on s0
                        double coordP1 = Vector2D.DotProduct(s1.P1 - s0.P0, s0.P1 - s0.P0) / (s0.P1 - s0.P0).GetLengthSquared();
                        // need to swap s1.P0 and s1.P1 ?
                        bool swapped = false;
                        if (coordP0 > coordP1)
                        {
                            swapped = true;
                            double temp = coordP0;
                            coordP0 = coordP1;
                            coordP1 = temp;
                        }
                        // compute non-overlapping segments
                        if ( (coordP0 <= 0.0 && coordP1 <= 0.0) || (coordP0 >= 1.0 && coordP1 >= 1.0) )
                        {
                            /* no overlapp */
                        }
                        else if ( ( coordP0 >= 0.0 && coordP0 <= 1.0 ) && ( coordP1 >= 1 ) )
                        {
                            //-------S0P0---------S1P0---------S0P1--------S1P1--------
                            segmentCurrent.RemoveAt(i1);
                            segmentCurrent.Add(new Segment(s0.P1, swapped ? s1.P0 : s1.P1));
                        }
                        else if ( ( coordP0 <= 0.0) && (coordP1 >= 0 && coordP1 <=1))
                        {
                            //-------S1P0---------S0P0---------S1P1--------S0P1--------
                            segmentCurrent.RemoveAt(i1);
                            segmentCurrent.Add(new Segment(swapped ? s1.P1 : s1.P0, s0.P0));
                        }
                        else if ((coordP0 <= 0.0) && (coordP1 >= 1.0))
                        {
                            //-------S1P0--------S0P0----------S0P1--------S1P1---------
                            segmentCurrent.RemoveAt(i1);
                            segmentCurrent.Add(new Segment(swapped ? s1.P1 : s1.P0, s0.P0));
                            segmentCurrent.Add(new Segment(s0.P1, swapped ? s1.P0 : s1.P1));
                        }
                        else if ((coordP0 >= 0.0) && (coordP1 <= 1.0))
                        {
                            //-------S0P0--------S1P0----------S1P1--------S0P1---------
                            segmentCurrent.RemoveAt(i1);
                        }
                    }
                }

                foreach (Segment s in segmentCurrent)
                {
                    // length of remaining segment
                    double segLength = (s.P1 - s.P0).GetLength();
                    // add length
                    _length += segLength;
                    // add to 
                    if (segLength > epsilon)
                        _segments.Add(s);
                }
            }
            else
                _length += drawable.Length;        
        }
        public double Length
        {
            get
            {
                return _length; 
            }
        }
        #endregion
    }
}
