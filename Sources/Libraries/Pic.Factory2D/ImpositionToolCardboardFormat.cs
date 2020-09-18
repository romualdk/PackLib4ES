#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sharp3D.Math.Core;
using log4net;

//using TreeDim.UserControls;
#endregion

namespace Pic.Factory2D
{
    #region ImpositionToolCardboardFormat
    internal class ImpositionToolCardboardFormat : ImpositionTool
    {
        #region Constructor
        internal ImpositionToolCardboardFormat(IEntityContainer container, CardboardFormat cardboardFormat)
            : base(container)
        {
            FormatDimensions = cardboardFormat.Dimensions;
        }
        internal ImpositionToolCardboardFormat(IEntityContainer container, double width, double height)
            : base(container)
        {
            FormatDimensions = new Vector2D(width, height);
        }
        #endregion

        #region Public format


        #endregion

        #region Override imposition tool
        internal override int NoPatternX(ImpositionPattern pattern)
        {
            return (int)Math.Floor((UsableFormat.Width - pattern.BBox.Width) / pattern.PatternStep.X) + 1;
        }
        internal override int NoPatternY(ImpositionPattern pattern)
        {
            return (int)Math.Floor((UsableFormat.Height - pattern.BBox.Height) / pattern.PatternStep.Y) + 1;
        }
        internal override ImpositionSolution GenerateSolution(ImpositionPattern pattern, bool orthoImp)
        {
            // instantiate solution
            ImpositionSolution solution = new ImpositionSolution(InitialEntities, pattern.Name, orthoImp, pattern.RequiresRotationInRows, pattern.RequiresRotationInColumns);

            // pattern box
            Box2D boxPattern = pattern.BBox;

            // compute max number of patterns
            int noPatternX = NoPatternX(pattern);
            int noPatternY = NoPatternY(pattern);

            int[,] rowNumber = new int[pattern.NoRows, pattern.NoCols];
            int[,] colNumber = new int[pattern.NoRows, pattern.NoCols];

            int iMax = -1, jMax = -1;
            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    if (pattern._bboxes[i, j].XMax + noPatternX * pattern.PatternStep.X - boxPattern.XMin < UsableFormat.Width)
                    {
                        rowNumber[i, j] = noPatternX + 1;
                        iMax = i;
                    }
                    else
                        rowNumber[i, j] = noPatternX;

                    if (pattern._bboxes[i, j].YMax + noPatternY * pattern.PatternStep.Y - boxPattern.YMin < UsableFormat.Height)
                    {
                        colNumber[i, j] = noPatternY + 1;
                        jMax = j;
                    }
                    else
                        colNumber[i, j] = noPatternY;
                }

            // compute actual margin
            Box2D boxGen = Box2D.Initial;
            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    for (int k = 0; k < rowNumber[i, j]; ++k)
                        for (int l = 0; l < colNumber[i, j]; ++l)
                        {
                            Vector2D vPatternStep = new Vector2D(k * pattern.PatternStep.X, l * pattern.PatternStep.Y);
                            boxGen.Extend(pattern._bboxes[i, j].PtMin + vPatternStep);
                            boxGen.Extend(pattern._bboxes[i, j].PtMax + vPatternStep);
                        }
                }
            if (!boxGen.IsValid)
                return solution;

            #region Additional both direction
            // ****
            // get remaining space in length and width
            double remainingX = UsableFormat.Width - boxGen.Width - SpaceBetween.X;
            double remainingY = UsableFormat.Height - boxGen.Height - SpaceBetween.Y;

            // here compute additionnal poses on right
            Box2D bboxRight = Box2D.Initial;
            List<BPosition> lPosRight = new List<BPosition>();
            if (AllowBothDirection)
            {
                Vector2D vLowerLeft = new Vector2D(boxGen.PtMax.X, boxGen.PtMin.Y);
                Vector2D vTopRight = vLowerLeft + new Vector2D(remainingX, UsableFormat.Height);
                if (ComputePositionDefault(InitialEntities, new Box2D(vLowerLeft, vTopRight), SpaceBetween, ref lPosRight, ref bboxRight))
                {
                    boxGen.Extend(bboxRight);
                }
            }
            // here compute additionnal poses on top
            Box2D bboxTop = Box2D.Initial;
            List<BPosition> lPosTop = new List<BPosition>();
            if (AllowBothDirection)
            {
                Vector2D vLowerLeft = new Vector2D(boxGen.PtMin.X, boxGen.PtMax.Y);
                Vector2D vTopRight = vLowerLeft + new Vector2D(UsableFormat.Width, remainingY);

                if (ComputePositionDefault(InitialEntities, new Box2D(vLowerLeft, vTopRight), SpaceBetween, ref lPosTop, ref bboxTop))
                {
                    boxGen.Extend(bboxTop);                    
                }
            }
            // ****
            #endregion

            double xMargin = 0.0;
            switch (HorizontalAlignment)
            {
                case ImpositionSettings.HAlignment.HALIGN_LEFT: xMargin = UsableFormat.XMin; break;
                case ImpositionSettings.HAlignment.HALIGN_RIGHT: xMargin = FormatDimensions.X - Margin.X - boxGen.Width; break;
                case ImpositionSettings.HAlignment.HALIGN_CENTER: xMargin = (FormatDimensions.X - boxGen.Width) * 0.5; break;
                default: break;
            }

            double yMargin = 0.0;
            switch (VerticalAlignment)
            {
                case ImpositionSettings.VAlignment.VALIGN_BOTTOM: yMargin = UsableFormat.YMin; break;
                case ImpositionSettings.VAlignment.VALIGN_TOP: yMargin = FormatDimensions.Y - Margin.Y - boxGen.Height; break;
                case ImpositionSettings.VAlignment.VALIGN_CENTER: yMargin = (FormatDimensions.Y - boxGen.Height) * 0.5; break;
                default: break;
            }

            // compute offsets
            Vector2D vOffset = new Vector2D(xMargin - boxPattern.XMin, yMargin - boxPattern.YMin);

            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    for (int k = 0; k < rowNumber[i, j]; ++k)
                        for (int l = 0; l < colNumber[i, j]; ++l)
                        {
                            BPosition pos = pattern._relativePositions[i, j];
                            pos.Pt = vOffset + new Vector2D(
                                k * pattern.PatternStep.X + pos.Pt.X
                                , l * pattern.PatternStep.Y + pos.Pt.Y);
                            solution.Add(pos);
                        }
                }
            foreach (var p in lPosRight)
                solution.Add(new BPosition(p.Pt + vOffset + SpaceBetween.X * Vector2D.XAxis, p.Angle));
            foreach (var p in lPosTop)
                solution.Add(new BPosition(p.Pt + vOffset + SpaceBetween.Y * Vector2D.YAxis, p.Angle));
            // noRows / noCols
            solution.Rows = pattern.NoCols * noPatternX + iMax + 1;
            solution.Cols = pattern.NoRows * noPatternY + jMax + 1;
            // cardboard position
            solution.CardboardPosition = Vector2D.Zero;
            solution.CardboardDimensions = FormatDimensions;
            return solution;
        }
        public override bool AllowOrthogonalImposition => base.AllowOrthogonalImposition && (FormatDimensions.X != FormatDimensions.Y);
        #endregion
    }
    #endregion
}
