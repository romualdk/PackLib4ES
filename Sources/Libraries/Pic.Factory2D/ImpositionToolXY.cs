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
    #region ImpositionToolXY
    internal class ImpositionToolXY : ImpositionTool
    {
        #region Constructor
        internal ImpositionToolXY(IEntityContainer container, int noInX, int noInY)
            : base(container)
        {
            NoColsExpected = noInX;
            NoRowsExpected = noInY;
        }
        #endregion

        #region Override ImpositionTool
        public override bool AllowBothDirection { get => false; set => base.AllowBothDirection = value; }
        #endregion

        #region Private properties
        private int NoColsExpected { get; }
        private int NoRowsExpected { get; }
        #endregion

        #region Override imposition tool
        internal override int NoPatternX(ImpositionPattern pattern) => NoColsExpected / pattern.NoCols;
        internal override int NoPatternY(ImpositionPattern pattern) => NoRowsExpected / pattern.NoRows; 

        internal override ImpositionSolution GenerateSolution(ImpositionPattern pattern, bool orthoImposition)
        {
            // instantiate solution
            var solution = new ImpositionSolution(InitialEntities, pattern.Name, orthoImposition, pattern.RequiresRotationInRows, pattern.RequiresRotationInColumns)
            {
                Rows = NoRowsExpected,
                Cols = NoColsExpected
            };
            // pattern box
            Box2D boxPattern = pattern.BBox;

            // number of each row / col of the pattern
            int[,] rowNumber = new int[pattern.NoRows, pattern.NoCols];
            int[,] colNumber = new int[pattern.NoRows, pattern.NoCols];

            int iCount = 0;
            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    colNumber[i ,j] = NoRowsExpected / pattern.NoRows + (NoRowsExpected % pattern.NoRows) /  (i+1);
                    rowNumber[i, j] = NoColsExpected / pattern.NoCols + (NoColsExpected % pattern.NoCols) / (j+1);

                    iCount += rowNumber[i, j] * colNumber[i, j];
                }
            // verify count
            System.Diagnostics.Debug.Assert(iCount == NoRowsExpected * NoColsExpected);

            // compute actual margin
            var boxGen = Box2D.Initial;
            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    for (int k = 0; k < rowNumber[i, j]; ++k)
                        for (int l = 0; l < colNumber[i, j]; ++l)
                        {
                            var vPatternStep = new Vector2D(k * pattern.PatternStep.X, l * pattern.PatternStep.Y);
                            boxGen.Extend(pattern._bboxes[i, j].PtMin + vPatternStep);
                            boxGen.Extend(pattern._bboxes[i, j].PtMax + vPatternStep);
                        }
                }
            if (FormatDimensions != Vector2D.Zero && (boxGen.Width > FormatDimensions.X || boxGen.Height > FormatDimensions.Y))
            {
                solution.Rows = solution.Cols = 0;
                solution.CardboardDimensions = FormatDimensions;
                return solution;
            }

            var formatDimensions = FormatDimensions != Vector2D.Zero
                ? FormatDimensions
                : new Vector2D(boxGen.XMax - boxGen.XMin + Margin.X + MinMargin.X,
                    boxGen.YMax - boxGen.YMin + Margin.Y + MinMargin.Y);

            double xMargin = 0.0;
            switch (HorizontalAlignment)
            {
                case ImpositionSettings.HAlignment.HALIGN_LEFT: xMargin = UsableFormat.XMin; break;
                case ImpositionSettings.HAlignment.HALIGN_RIGHT: xMargin = formatDimensions.X - Margin.X - boxGen.Width; break;
                case ImpositionSettings.HAlignment.HALIGN_CENTER: xMargin = (formatDimensions.X - boxGen.Width) * 0.5; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            double yMargin = 0.0;
            switch (VerticalAlignment)
            {
                case ImpositionSettings.VAlignment.VALIGN_BOTTOM: yMargin = UsableFormat.YMin; break;
                case ImpositionSettings.VAlignment.VALIGN_TOP: yMargin = formatDimensions.Y - Margin.Y - boxGen.Height; break;
                case ImpositionSettings.VAlignment.VALIGN_CENTER: yMargin = (formatDimensions.Y - boxGen.Height) * 0.5; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // compute offsets
            var vOffset = new Vector2D(xMargin - boxPattern.XMin, yMargin - boxPattern.YMin);

            // format bounding box
            var box = new Box2D();
            for (int i = 0; i < pattern.NoRows; ++i)
                for (int j = 0; j < pattern.NoCols; ++j)
                {
                    Box2D localBox = pattern._bboxes[i, j];
                    for (int k = 0; k < rowNumber[i, j]; ++k)
                        for (int l = 0; l < colNumber[i, j]; ++l)
                        {
                            // insert Box position
                            BPosition pos = pattern._relativePositions[i, j];
                            pos.Pt = vOffset + new Vector2D(
                                 k * pattern.PatternStep.X + pos.Pt.X
                                , l * pattern.PatternStep.Y + pos.Pt.Y
                                );
                            solution.Add(pos);

                            // extend bounding box
                            var vOffset1 = new Vector2D(vOffset.X + k * pattern.PatternStep.X, vOffset.Y + l * pattern.PatternStep.Y);
                            box.Extend(new Box2D(vOffset1 + localBox.PtMin, vOffset1 + localBox.PtMax));
                        }
                }
            // noRows / noCols
            solution.Rows = NoRowsExpected;
            solution.Cols = NoColsExpected;

            // cardboard position
            solution.CardboardPosition = new Vector2D(xMargin, yMargin);
            solution.CardboardDimensions = FormatDimensions;

            if (FormatDimensions != Vector2D.Zero)
                solution.CardboardDimensions = FormatDimensions;
            else
                solution.CardboardDimensions = new Vector2D(
                    box.XMax - box.XMin + Margin.X + MinMargin.X
                    , box.YMax - box.YMin + Margin.Y + MinMargin.Y);
            return solution;
        } 
        #endregion
    }
    #endregion
}
