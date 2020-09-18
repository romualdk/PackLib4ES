#region Using directives
using System;
using System.Collections.Generic;

using Sharp3D.Math.Core;

using DesLib4NET;
#endregion

namespace Pic.Factory2D
{
    public class PicLoaderDes : DES_CreationAdapter, IDisposable
    {
        #region Constructor
        public PicLoaderDes(PicFactory factory)
        {
            // initialize data member factory
            Factory = factory;
            // clear factory before loading data
            Factory.Clear();
        }
        #endregion

        #region IDisposable override
        public void Dispose()
        { 
        }
        #endregion

        #region DES_CreationAdapter override
        public void Set2D()
        { 
        }
        public void Set3D()
        { 
        }
        public void SetViewWindow(float xmin, float ymin, float xmax, float ymax)
        {
        }
        public void AddSuperBaseHeader(DES_SuperBaseHeader header)
        { 
        }
        public void AddPoint(DES_Point point)
        { 
        }
        /// <summary>
        /// insert new segment
        /// </summary>
        /// <param name="segment">segment to insert</param>
        public void AddSegment(DES_Segment segment)
        {
            Factory.AddSegment(
                DesPenToLineType(segment._pen), segment._grp, segment._layer,
                new Vector2D(segment.X1, segment.Y1),
                new Vector2D(segment.X2, segment.Y2));
        }
        /// <summary>
        /// insert new arc
        /// </summary>
        /// <param name="arc"></param>
        public void AddArc(DES_Arc arc)
        {
           	float angle0 = arc._dir
		          , angle1 = arc._dir + arc._angle;

	        if (arc._angle > 0.0f)
	        {
	        }
	        else
	        {
		        angle0 += arc._angle;
		        angle1 -= arc._angle;

		        while (angle0 < 0)
		        {
			        angle0 += 360.0f;
			        angle1 += 360.0f;
		        }
	        }

            PicArc picArc = Factory.AddArc(
                DesPenToLineType(arc._pen), arc._grp, arc._layer,
                new Vector2D(arc._x, arc._y),
                arc._dim,
                angle0, angle1
                );
        }
        public void AddBezier(DES_Bezier bezier)
        { 
        }
        public void AddNurbs(DES_Nurbs nurbs)
        { 
        }
        public void AddText(DES_Text text)
        { 
        }
        public void AddPose(DES_Pose pose)
        { 
        }
        public void AddSurface(DES_Surface surface)
        { 
        }
        public void AddDimensionOuterRadius()
        { 
        }
        public void AddDimensionInnerRadius()
        { 
        }
        public void AddDimensionOuterDiameter()
        {        
        }
        public void AddDimensionInnerDiameter()
        { 
        }
        public void AddDimensionDistance(DES_CotationDistance dimension)
        {
            PicCotation.CotType type = PicCotation.CotType.COT_DISTANCE;
            if (Math.Abs(dimension._dir) < 1.0)
                type = PicCotation.CotType.COT_HORIZONTAL;
            else if (Math.Abs(dimension._dir - 90.0) < 1.0)
                type = PicCotation.CotType.COT_VERTICAL;
            else
                type = PicCotation.CotType.COT_DISTANCE;

            Factory.AddCotation(
                type
                , dimension._grp, dimension._layer
                , new Vector2D( dimension.X1, dimension.Y1)
                , new Vector2D( dimension.X2, dimension.Y2)
                , dimension._offset
                , dimension._text
                , dimension._noDecimals);
        }
        public void AddDimensionAngle()
        { 
        }
        public void AddDimensionArrow()
        { 
        }
        public void UpdateQuestionnaire(Dictionary<string, string> questions)
        {
            Factory.UpdateQuestions(questions);
        }
        #endregion

        #region Helpers
        PicGraphics.LT DesPenToLineType(byte pen)
        {
            switch (pen)
            {
                // perfos-rainant (2) , les perfos (4) et les mi-chair (5) se comportent comme des rainants (6)
                case 1: return PicGraphics.LT.LT_CUT;
                case 2: return PicGraphics.LT.LT_PERFOCREASING;
                case 3: return PicGraphics.LT.LT_CONSTRUCTION;
                case 4: return PicGraphics.LT.LT_PERFO;
                case 5: return PicGraphics.LT.LT_HALFCUT;
                case 6: return PicGraphics.LT.LT_CREASING;
                case 7: return PicGraphics.LT.LT_AXIS;
                case 8: return PicGraphics.LT.LT_COTATION;
                case 9: return PicGraphics.LT.LT_ORIGIN;
                case 10: return PicGraphics.LT.LT_GRID;
                case 11: return PicGraphics.LT.LT_BRIDGES;
                default: return PicGraphics.LT.LT_DEFAULT;
            }
        }
        #endregion

        #region Data members
        PicFactory Factory { get; set; }
        #endregion
    }
 
}
