#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.ThreadCallback;
#endregion

namespace Pic.Factory2D
{
    #region BPosition
    public struct BPosition
    {
        #region Constructor
        public BPosition(Vector2D pt, double angle)
        {
            Pt = pt;
            Angle = angle;
        }
        #endregion
        #region Public properties
        public Vector2D Pt { get; set; }
        public double Angle { get; set; }
        public Transform2D Transformation => Transform2D.Translation(Pt) * Transform2D.Rotation(Angle);
        #endregion
    }
    #endregion

    #region Imposition pattern abstract class
    internal abstract class ImpositionPattern
    {
        #region Data members
        internal BPosition[,] _relativePositions;
        internal Box2D[,] _bboxes;
        protected Vector2D _patternStep;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(ImpositionPattern));
        protected static double EPSILON = 0.0001;
        internal static ImpositionPattern[] All => new ImpositionPattern[]
        {
            new PatternDefault(),
            new PatternRotationInRow(),
            new PatternRotationInColumn()
        };
        #endregion

        #region Abstract methods
        abstract public void GeneratePattern(IEntityContainer container, Vector2D minDistance, Vector2D impositionOffset, bool ortho);
        public Box2D BBox
        {
            get
            {
                Box2D boxGlobal = new Box2D();
                for (int i = 0; i < NoRows; ++i)
                    for (int j = 0; j < NoCols; ++j)
                        boxGlobal.Extend(_bboxes[i, j]);
                return boxGlobal;
            }
        }
        #endregion

        #region Abstract properties
        abstract public bool RequiresRotationInRows { get; }
        abstract public bool RequiresRotationInColumns { get; }
        abstract public int NoRows { get; }
        abstract public int NoCols { get; }
        abstract public string Name { get; }
        #endregion

        #region Public methods
        #endregion

        #region Public properties
        public Vector2D PatternStep { get { return _patternStep; } }
        #endregion
    }
    #endregion

    #region Pattern implementations

    #region Default pattern
    internal class PatternDefault : ImpositionPattern
    {
        public override bool RequiresRotationInRows { get { return false; } }
        public override bool RequiresRotationInColumns { get { return false; } }
        public override int NoRows { get { return 1; } }
        public override int NoCols { get { return 1; } }
        public override string Name { get { return "Default"; } }
        public override void GeneratePattern(IEntityContainer container, Vector2D minDistance, Vector2D impositionOffset, bool ortho)
        {
            using (PicFactory factory = new PicFactory())
            {
                // instantiate block and BlockRef
                PicBlock block = factory.AddBlock(container);
                PicBlockRef blockRef00 = factory.AddBlockRef(block, new Vector2D(0.0, 0.0), ortho ? 90.0 : 0.0);

                Box2D boxEntities = Tools.BoundingBox(factory, 0.0, false);

                // compute default X step
                PicBlockRef blockRef01 = factory.AddBlockRef(block, new Vector2D(5.0 * boxEntities.Width + minDistance.X, 0.0), ortho ? 90.0 : 0.0);
                if (!PicBlockRef.Distance(blockRef00, blockRef01, PicBlockRef.DistDirection.HORIZONTAL_RIGHT, out double horizontalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.X = 5.0 * boxEntities.Width - horizontalDistance + 2.0 * minDistance.X;

                // compute default Y step
                PicBlockRef blockRef10 = factory.AddBlockRef(block, new Vector2D(0.0, 5.0 * boxEntities.Height + minDistance.Y), ortho ? 90.0 : 0.0);
                if (!PicBlockRef.Distance(blockRef00, blockRef10, PicBlockRef.DistDirection.VERTICAL_TOP, out double verticalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.Y = 5.0 * boxEntities.Height - verticalDistance + 2.0 * minDistance.Y;

                // positions
                _relativePositions = new BPosition[1, 1];
                BPosition position = new BPosition(Vector2D.Zero, ortho ? 90.0 : 0.0);
                _relativePositions[0,0]= position;

                // bboxes
                _bboxes = new Box2D[1, 1];
                _bboxes[0, 0] = blockRef00.Box;//boxEntities;
            }
        }
    }
    #endregion

    #region With rotation in rows
    internal class PatternRotationInRow : ImpositionPattern
    {
        public override bool RequiresRotationInRows { get { return true; } }
        public override bool RequiresRotationInColumns { get { return false; } }
        public override int NoRows { get { return 1; } }
        public override int NoCols { get { return 2; } }
        public override string Name { get { return "Rotated in row (1*2)"; } }
        public override void GeneratePattern(IEntityContainer container, Vector2D minDistance, Vector2D impositionOffset, bool ortho)
        {
            //
            // 10 11 12
            // 00 01 02
            //
            using (PicFactory factory = new PicFactory())
            {
                // instantiate block and BlockRef
                PicBlock block = factory.AddBlock(container);
                PicBlockRef blockRef00 = factory.AddBlockRef(block
                    , Vector2D.Zero
                    , ortho ? 90.0 : 0.0);

                // compute bounding box
                Box2D boxEntities = Tools.BoundingBox(factory, 0.0, false);

                // compute second entity position on row 0
                PicBlockRef blockRef01 = factory.AddBlockRef(block
                    , new Vector2D(
                        boxEntities.XMax + boxEntities.XMin + 5.0 * boxEntities.Width + (ortho ? 1.0 : 0.0) * impositionOffset.X
                        , boxEntities.YMax + boxEntities.YMin + (ortho ? 0.0 : 1.0) * impositionOffset.Y)
                    , ortho ? 270.0 : 180.0);
                if (!PicBlockRef.Distance(blockRef00, blockRef01, PicBlockRef.DistDirection.HORIZONTAL_RIGHT, out double horizontalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                Vector2D vecPosition = new Vector2D(
                    boxEntities.XMax + boxEntities.XMin + 5.0 * boxEntities.Width + minDistance.X - horizontalDistance + (ortho ? 1.0 : 0.0) * impositionOffset.X
                    , boxEntities.YMax + boxEntities.YMin + (ortho ? 0.0 : 1.0) * impositionOffset.Y);
                blockRef01.Position = vecPosition;

                // positions
                _relativePositions = new BPosition[1, 2];
                _relativePositions[0, 0] = new BPosition(Vector2D.Zero, ortho ? 90.0 : 0.0);
                _relativePositions[0, 1] = new BPosition(vecPosition, ortho ? 270.0 : 180.0);
                // bboxes
                _bboxes = new Box2D[1, 2];
                _bboxes[0, 0] = blockRef00.Box;//boxEntities;
                _bboxes[0, 1] = blockRef01.Box;

                // compute Y step (row1 / row0)
                // row0
                List<PicBlockRef> listRow0 = new List<PicBlockRef>
                {
                    blockRef00,
                    blockRef01
                };
                // row1
                List<PicBlockRef> listRow1 = new List<PicBlockRef>();
                PicBlockRef blockRef10 = factory.AddBlockRef(
                    block
                    , new Vector2D(0.0, 5.0 * boxEntities.Height + minDistance.Y)
                    , ortho ? 90.0 : 0.0);
                PicBlockRef blockRef11 = factory.AddBlockRef(
                    block
                    , new Vector2D(vecPosition.X, vecPosition.Y + 5.0 * boxEntities.Height + minDistance.Y)
                    , ortho ? 270.0 : 180.0);
                listRow1.Add(blockRef10);
                listRow1.Add(blockRef11);

                if (!PicBlockRef.Distance(listRow0, listRow1, PicBlockRef.DistDirection.VERTICAL_TOP, out double verticalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.Y = 5.0 * boxEntities.Height - verticalDistance + 2.0 * minDistance.Y;

                blockRef10.Position = new Vector2D(0.0, _patternStep.Y);
                blockRef11.Position = new Vector2D(vecPosition.X, vecPosition.Y + _patternStep.Y);

                // compute X step (col1 / col2)
                PicBlockRef blockRef02 = factory.AddBlockRef(
                    block
                    , new Vector2D( boxEntities.XMin + 5.0 * boxEntities.Width + minDistance.X
                        , 0.0)
                    , ortho ? 90.0 : 0.0);
                PicBlockRef blockRef12 = factory.AddBlockRef(
                    block
                    , new Vector2D(boxEntities.XMin + 5.0 * boxEntities.Width + minDistance.X
                        , _patternStep.Y)
                    , ortho ? 90.0 : 0.0);

                List<PicBlockRef> listCol1 = new List<PicBlockRef> { blockRef01, blockRef11 };
                List<PicBlockRef> listCol2 = new List<PicBlockRef> { blockRef02, blockRef12 };

                horizontalDistance = 0.0;
                if (!PicBlockRef.Distance(listCol1, listCol2, PicBlockRef.DistDirection.HORIZONTAL_RIGHT, out horizontalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.X = boxEntities.XMin + 5.0 * boxEntities.Width - horizontalDistance + 2.0 * minDistance.X;
            }
        }
    }
    #endregion

    #region With rotation in colums
    internal class PatternRotationInColumn : ImpositionPattern
    {
        public override bool RequiresRotationInRows { get { return false; } }
        public override bool RequiresRotationInColumns { get { return true; } }
        public override int NoRows { get { return 2; } }
        public override int NoCols { get { return 1; } }
        public override string Name { get { return "Rotated in column"; } }
        public override void GeneratePattern(IEntityContainer container, Vector2D minDistance, Vector2D impositionOffset, bool ortho)
        {
            using (PicFactory factory = new PicFactory())
            {
                // 20 21
                // 10 11 12
                // 00 01 02
                //

                // instantiate block and BlockRef
                PicBlock block = factory.AddBlock(container);
                PicBlockRef blockRef00 = factory.AddBlockRef(block, new Vector2D(0.0, 0.0), ortho ? 90.0 : 0.0);

                // compute bounding box
                Box2D boxEntities = Tools.BoundingBox(factory, 0.0, false);

                // compute second entity position
                PicBlockRef blockRef10 = factory.AddBlockRef(block
                    , new Vector2D(
                    boxEntities.XMin + boxEntities.XMax + (ortho ? 0.0 : 1.0) * impositionOffset.X
                    , boxEntities.YMin + boxEntities.YMax + 5.0 * boxEntities.Height + minDistance.Y + (ortho ? 1.0 : 0.0) * impositionOffset.Y
                    ), ortho ? 270.0 : 180.0);
                if (!PicBlockRef.Distance(blockRef00, blockRef10, PicBlockRef.DistDirection.VERTICAL_TOP, out double verticalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                Vector2D vecPosition = new Vector2D(
                    boxEntities.XMin + boxEntities.XMax + (ortho ? 0.0 : 1.0) * impositionOffset.X
                    , boxEntities.YMin + boxEntities.YMax + 5.0 * boxEntities.Height + 2.0 * minDistance.Y - verticalDistance + (ortho ? 1.0 : 0.0) * impositionOffset.Y);
                blockRef10.Position = vecPosition;

                // positions
                _relativePositions = new BPosition[2, 1];
                _relativePositions[0, 0] = new BPosition(Vector2D.Zero, ortho ? 90.0 : 0.0);
                _relativePositions[1, 0] = new BPosition(vecPosition, ortho ? 270.0 : 180.0);
                // bboxes
                _bboxes = new Box2D[2, 1];
                _bboxes[0, 0] = blockRef00.Box;//boxEntities;
                _bboxes[1, 0] = blockRef10.Box;

                // compute X step (col1 / col0)
                // col0
                List<PicBlockRef> listCol0 = new List<PicBlockRef>
                {
                    blockRef00,
                    blockRef10
                };

                // col1
                PicBlockRef blockRef01 = factory.AddBlockRef(block
                    , new Vector2D(5.0 * boxEntities.Width + minDistance.X, 0.0)
                    , ortho ? 90.0 : 0.0);
                PicBlockRef blockRef11 = factory.AddBlockRef(block
                    , new Vector2D(5.0 * boxEntities.Width + minDistance.X + vecPosition.X, vecPosition.Y)
                    , ortho ? 270.0 : 180.0);
                List<PicBlockRef> listCol1 = new List<PicBlockRef>
                {
                    blockRef01,
                    blockRef11
                };
                double horizontalDistance = 0.0;
                if (!PicBlockRef.Distance(listCol0, listCol1, PicBlockRef.DistDirection.HORIZONTAL_RIGHT, out horizontalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.X = 5.0 * boxEntities.Width - horizontalDistance + 2.0 * minDistance.X;
                blockRef01.Position = vecPosition;
                blockRef11.Position = new Vector2D(vecPosition.X + _patternStep.X, vecPosition.Y);

                // compute Y step (row2 / row1)
                // row1
                List<PicBlockRef> listRow1 = new List<PicBlockRef>
                {
                    blockRef10,
                    blockRef11
                };

                PicBlockRef blockRef20 = factory.AddBlockRef(block
                    , new Vector2D(0.0, 5.0 * (boxEntities.Height + minDistance.Y))
                    , ortho ? 90.0 : 0.0);
                PicBlockRef blockRef21 = factory.AddBlockRef(block
                    , new Vector2D(_patternStep.X, 5.0 * (boxEntities.Height + minDistance.Y))
                    , ortho ? 90.0 : 0.0);

                List<PicBlockRef> listRow2 = new List<PicBlockRef>
                {
                    blockRef20,
                    blockRef21
                };

                verticalDistance = 0.0;
                if (!PicBlockRef.Distance(listRow1, listRow2, PicBlockRef.DistDirection.VERTICAL_TOP, out verticalDistance))
                    throw new Exception("Failed to compute distance between to block refs");
                _patternStep.Y = 5.0 * boxEntities.Height - verticalDistance + 6.0 * minDistance.Y;
            }
        }
    }
    #endregion

    #endregion // Pattern implementations

    #region ImpositionSolution
    public class ImpositionSolution : IDisposable
    {
        #region Constructor
        public ImpositionSolution(
            IEntityContainer container, string patternName,
            bool orthoImposition,
            bool hasRotationInRows, bool hasRotationInCols)
        {
            _container = container;
            PatternName = patternName;
            OrthoImposition = orthoImposition;
            AllowRotationInRows = hasRotationInRows;
            AllowRotationInCols = hasRotationInCols;
        }
        public void Dispose()
        {
            Thumbnail?.Dispose();
        }
        #endregion

        #region Private methods
        public void GenerateThumbnail()
        {
            using (PicFactory factory = new PicFactory())
            {
                CreateEntities(factory);
                // insert format
                factory.InsertCardboardFormat(CardboardPosition, CardboardDimensions);
                // draw thumbnail
                PicGraphicsImage picImage = new PicGraphicsImage(new Size(ThumbnailWidth, ThumbnailWidth), Tools.BoundingBox(factory, PicFilter.FilterNone, 0.01));
                factory.Draw(picImage);
                // save thumbnail
                Thumbnail = picImage.Bitmap;
            }
        }
        #endregion

        #region Public methods
        public void Add(BPosition pos)
        {
            Positions.Add(pos);
        }
        public void CreateEntities(PicFactory factory)
        {
            // sanity check
            if (Positions.Count == 0)
                return; // solution has no position -> exit
            // get first position
            BPosition pos0 = Positions[0];
            // block
            PicBlock block = factory.AddBlock(_container, pos0.Transformation);
            // blockrefs
            for (int i = 1; i < Positions.Count; ++i) // do not insert first position as the block is now displayed
            {
                BPosition pos = Positions[i];
                factory.AddBlockRef(block, pos.Transformation * pos0.Transformation.Inverse());
            }
        }
        #endregion

        #region Private helpers
        private void ComputeLengthCutFold()
        { 
            if (_lengthCut <= 0)
            {
                _lengthCut = 0.0;
                using (PicFactory factory = new PicFactory())
                {
                    // fill factory with entities
                    CreateEntities(factory);
                    // counting added length only
                    PicVisitorComputeLength visitorComputeLength = new PicVisitorComputeLength();
                    factory.ProcessVisitor(visitorComputeLength/*, new PicFilterLineType(PicGraphics.LT.LT_CUT)*/);
                    // cut length
                    _lengthCut = visitorComputeLength.Length;
                }
            }
        }
        private void Translate(Vector2D vecTranslate)
        {
            List<BPosition> tempPositions = new List<BPosition>();
            foreach (BPosition position in Positions)
            {
                BPosition posTemp = position;
                posTemp.Pt += vecTranslate;
                tempPositions.Add(posTemp);
            }
            Positions = tempPositions;
        }
        #endregion

        #region Public properties
        public string PatternName { get; set; }
        public bool OrthoImposition { get; set; }

        public bool AllowRotationInRows { get; set; }
        public bool AllowRotationInCols { get; set; }

        public Vector2D CardboardPosition { get; set; }
        public Vector2D CardboardDimensions { get; set; }

        public bool IsValid => Rows * Cols > 0;
        public int PositionCount => Positions.Count;
        public int Rows { get; set; }
        public int Cols { get; set; }
        public double UnitLengthCut { set; get; }
        public double UnitLengthFold { set; get; }
        public double UnitArea { set; get; }
        public double LengthCut
        {
            get
            {
                ComputeLengthCutFold();
                return _lengthCut;
            }
        }
        public double LengthFold { get {return UnitLengthFold * PositionCount; } }
        public double Area { get { return UnitArea * PositionCount; } }
        public Bitmap Thumbnail { get; set; }
        public static int ThumbnailWidth { get; set; } = 75;
        public double Width  => Bbox.Width;
        public double Height => Bbox.Height; 
        public Box2D Bbox
        {
            get
            {
                if (null == _box)
                {
                    using (PicFactory factory = new PicFactory())
                    {
                        CreateEntities(factory);
                        // compute bounding box without format
                        _box = Tools.BoundingBox(factory, PicFilter.FilterNone, 0.0);
                    }
                }
                return _box; 
            } 
        }
        public double BBoxArea => Bbox.Width * Bbox.Height;
        public List<BPosition> Positions { get; private set; } = new List<BPosition>();
        #endregion

        #region System.Object override
        public override string ToString()
        {
            if (Rows * Cols == PositionCount)
                return $"{PatternName}\n{PositionCount} ({Rows}*{Cols})";
            else
                return $"{PatternName}\n{PositionCount}";
        }
        #endregion

        #region Data members
        private readonly IEntityContainer _container;
        private double _lengthCut = -1.0;
        private Box2D _box;
        #endregion
    }

    /// <summary>
    /// Implementation of <see cref="System.Collections.Generic.IComparer"/> interface used to sort the list of <see cref="ImpositionSolution"/> solutions.
    /// </summary>
    internal class SolutionComparerFormat : IComparer<ImpositionSolution>
    {
        #region IComparer<ImpositionSolution> Members
        /// <summary>
        /// Compares two <see cref="ImpositionSolution"/> solutions.
        /// </summary>
        /// <param name="solution1">First instance of <see cref="ImpositionSolution"/>.</param>
        /// <param name="solution2">>First instance of <see cref="ImpositionSolution"/>.</param>
        /// <returns></returns>
        public int Compare(ImpositionSolution solution1, ImpositionSolution solution2)
        {
            int returnValue = 1;
            if (solution1 is ImpositionSolution && solution2 is ImpositionSolution)
            {
                // order by Position count
                if (solution1.PositionCount > solution2.PositionCount)
                    returnValue = -1;
                else if (solution1.PositionCount < solution2.PositionCount)
                    returnValue = 1;
                else
                {
                    if (solution1.BBoxArea < solution2.BBoxArea)
                        return -1;
                    else if (solution1.BBoxArea > solution2.BBoxArea)
                        return 1;
                    else
                    {
                        // rules:
                        // prefer default configuration than with rotation in rows
                        // prefer rotation in rows than rotation in cols
                        // prefer rotation in cols than rotation in rows + in cols
                        int score1 = 0, score2 = 0;
                        score1 += (solution1.AllowRotationInRows ? 0 : 1);
                        score1 += (solution1.AllowRotationInCols ? 0 : 2);
                        score2 += (solution2.AllowRotationInRows ? 0 : 1);
                        score2 += (solution2.AllowRotationInCols ? 0 : 2);
                        if (score1 > score2)
                            returnValue = -1;
                        else if (score1 < score2)
                            returnValue = 1;
                        else
                            returnValue = 0;
                    }
                }
            }
            return returnValue;
        }
        #endregion
    }
    #endregion

    #region Imposition settings
    public class ImpositionSettings
    {
        #region Enums
        public enum VAlignment
        {
            VALIGN_BOTTOM
            , VALIGN_TOP
            , VALIGN_CENTER
        }
        public enum HAlignment
        {
            HALIGN_LEFT
            , HALIGN_RIGHT
            , HALIGN_CENTER
        }
        public enum Mode
        {
            M_FORMAT,
            M_ROWCOL
        }
        #endregion
        #region Constructors
        public ImpositionSettings(CardboardFormat cardboardFormat)
        {
            Format = cardboardFormat;
            ImpMode = Mode.M_FORMAT;
        }
        public ImpositionSettings(double width, double height)
        {
            Format = new CardboardFormat(0, string.Empty, string.Empty, width, height);
            ImpMode = Mode.M_FORMAT;
        }
        public ImpositionSettings(int noX, int noY)
        {
            NoX = noX; NoY = noY;
            ImpMode = Mode.M_ROWCOL;
        }
        #endregion
        #region Public properties
        public HAlignment AlignmentH { get; set; }
        public VAlignment AlignmentV { get; set; }
        public Vector2D SpaceBetween { get; set; }
        public Vector2D Margin { get; set; }
        public Vector2D MarginRemaining { get; set; }
        public Mode ImpMode { get; }
        public int NoX { get; set; }
        public int NoY { get; set; }
        public CardboardFormat Format { get; set; }
        public bool AllowRotationCol { get; set; }
        public bool AllowRotationRow { get; set; }
        public bool AllowBothDirections { get; set; }
        #endregion
    }
    #endregion

    #region Imposition tool
    public abstract class ImpositionTool
    {
        #region Data members
        private Vector2D _spaceBetween = Vector2D.Zero;
        private const double EPSILON = 1.0E-06;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(ImpositionTool));
        #endregion

        #region Static instantiation
        public static ImpositionTool Instantiate(IEntityContainer container, ImpositionSettings settings)
        {
            switch (settings.ImpMode)
            {
                case ImpositionSettings.Mode.M_FORMAT: return new ImpositionToolCardboardFormat(container, settings.Format);
                case ImpositionSettings.Mode.M_ROWCOL: return new ImpositionToolXY(container, settings.NoX, settings.NoY);
                default: throw new Exception("Unexpected imposition mode");
            }
        }
        #endregion

        #region Constructor
        public ImpositionTool(IEntityContainer container)
        {
            InitialEntities = container;

            using (PicFactory factory = new PicFactory(container))
            { 
                // compute lengthes
                PicVisitorDieCutLength visitor = new PicVisitorDieCutLength();
                factory.ProcessVisitor(visitor);
                Dictionary<PicGraphics.LT, double> lengthes = visitor.Lengths;
                UnitLengthCut = lengthes.ContainsKey(PicGraphics.LT.LT_CUT) ? lengthes[PicGraphics.LT.LT_CUT] : 0.0;
                UnitLengthFold = lengthes.ContainsKey(PicGraphics.LT.LT_CREASING) ? visitor.Lengths[PicGraphics.LT.LT_CREASING] : 0.0;
                // compute area
                try
                {
                    PicToolArea picToolArea = new PicToolArea();
                    factory.ProcessTool(picToolArea);
                    UnitArea = picToolArea.Area;
                }
                catch (PicToolTooLongException /*ex*/)
                {
                    UnitArea = 0;
                }
            }
        }
        #endregion

        #region Private method to get list of patterns
        internal List<ImpositionPattern> GetPatternList()
        {
            List<ImpositionPattern> list = new List<ImpositionPattern>();
            foreach (ImpositionPattern pattern in ImpositionPattern.All)
            {
                if (pattern.RequiresRotationInRows && !AllowRotationInRows)
                    continue;
                if (pattern.RequiresRotationInColumns && !AllowRotationInColumns)
                    continue;
                list.Add(pattern);
            }
            return list;
        }
        internal ImpositionPattern GetPatternByName(string patternName)
        {
            foreach (ImpositionPattern pattern in ImpositionPattern.All)
            {
                if (string.Equals(pattern.Name, patternName, StringComparison.InvariantCultureIgnoreCase))
                    return pattern;
            }
            return null;
        }
        /*
        internal ImpositionPattern GeneratePattern(int id, Vector2D minDistance, Vector2D impositionOffset, bool orthoImp)
        {
            ImpositionPattern pattern = null;

            KeyValuePair<int, bool> kvpIndexOrtho = new KeyValuePair<int, bool>(id, orthoImp);
            switch (id)
            {
                case 0: pattern = new PatternDefault(); break;
                case 1: pattern = new PatternRotationInRow(); break;
                case 2: pattern = new PatternRotationInColumn(); break;
            }
            return pattern;
        }
        */
        #endregion

        #region Public methods
        public bool GenerateEntities(string patternName, bool orthogonalImposition, ref PicFactory factory)
        {
            ImpositionPattern pattern = GetPatternByName(patternName);
            pattern.GeneratePattern(InitialEntities, SpaceBetween, ImpositionOffset, orthogonalImposition);
            ImpositionSolution solution = GenerateSolution(pattern, orthogonalImposition);
            solution.CreateEntities(factory);
            return true;
        }
        public bool GenerateSortedSolutionList(IProgressCallback callback, out List<ImpositionSolution> solutions)
        {
            // get applicable pattern list
            List<ImpositionPattern> patternList = GetPatternList();
            // compute number of expected solutions
            callback?.Begin(0, patternList.Count * (AllowOrthogonalImposition ? 4 : 2));
            // instantiate solution list
            solutions = new List<ImpositionSolution>();
            // process pattern list
            foreach (ImpositionPattern pattern in patternList)
            {
                // ### generate pattern
                try
                {
                    pattern.GeneratePattern(InitialEntities, SpaceBetween, ImpositionOffset, false);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                    continue;
                }

                // default orientation
                ImpositionSolution solution = GenerateSolution(pattern, false);
                if (null != solution && solution.IsValid)
                {
                    solution.UnitLengthCut = UnitLengthCut;
                    solution.UnitLengthFold = UnitLengthFold;
                    solution.UnitArea = UnitArea;
                    solutions.Add(solution);
                }
                // increment progress dialog?
                callback?.Increment(solutions.Count);
                // ###

                // ### generate pattern in orthogonal direction
                if (AllowOrthogonalImposition)
                {
                    try
                    {
                        pattern.GeneratePattern(InitialEntities, SpaceBetween, ImpositionOffset, true);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                        continue;
                    }
                    solution = GenerateSolution(pattern, true);
                    if (null != solution && solution.IsValid)
                    {
                        solution.UnitLengthCut = UnitLengthCut;
                        solution.UnitLengthFold = UnitLengthFold;
                        solution.UnitArea = UnitArea;
                        solutions.Add(solution);
                    }
                    // increment progress dialog?
                    callback?.Increment(solutions.Count);
                }
                // ###
            } // foreach

            // generate thumbnails
            foreach (ImpositionSolution sol in solutions)
            {
                sol.GenerateThumbnail();
                // increment progress dialog?
                callback?.Increment(solutions.Count);
            }

            // sort solution list
            solutions.Sort(new SolutionComparerFormat());

            callback?.End();
            return solutions.Count > 0;
        }

        abstract internal ImpositionSolution GenerateSolution(ImpositionPattern pattern, bool orthoImp);
        #endregion

        #region Abstract methods
        abstract internal int NoPatternX(ImpositionPattern pattern);
        abstract internal int NoPatternY(ImpositionPattern pattern);
        #endregion

        #region Public properties
        public IEntityContainer InitialEntities { get; }
        public Vector2D FormatDimensions { set; get; }
        public ImpositionSettings.HAlignment HorizontalAlignment { get; set; } = ImpositionSettings.HAlignment.HALIGN_CENTER;
        public ImpositionSettings.VAlignment VerticalAlignment { get; set; } = ImpositionSettings.VAlignment.VALIGN_CENTER;
        public Vector2D Margin { get; set; } = Vector2D.Zero;
        public Vector2D MinMargin { get; set; } = Vector2D.Zero;
        public Vector2D SpaceBetween
        {
            get
            {
                return new Vector2D(
                    _spaceBetween.X > 0 ? _spaceBetween.X : 0,
                    _spaceBetween.Y > 0 ? _spaceBetween.Y : 0
                    );
            }
            set { _spaceBetween = value; }
        }
        public bool AllowRotationInRows { get; set; } = true;
        public bool AllowRotationInColumns { get; set; } = true;
        public Vector2D ImpositionOffset { get; set; } = Vector2D.Zero;
        public double UnitLengthCut { get; set; }
        public double UnitLengthFold { get; set; }
        public double UnitArea { get; set; }
        public virtual bool AllowOrthogonalImposition { get; set; } = true;
        public virtual bool AllowBothDirection { get; set; } = false;
        public Box2D UsableFormat
        {
            get
            {
                Box2D box = new Box2D();
                switch (HorizontalAlignment)
                {
                    case ImpositionSettings.HAlignment.HALIGN_LEFT:
                        box.XMin = Margin.X;
                        box.XMax = FormatDimensions.X - MinMargin.X;
                        break;
                    case ImpositionSettings.HAlignment.HALIGN_RIGHT:
                        box.XMin = MinMargin.X;
                        box.XMax = FormatDimensions.X - Margin.X;
                        break;
                    case ImpositionSettings.HAlignment.HALIGN_CENTER:
                        box.XMin = Margin.X;
                        box.XMax = FormatDimensions.X - Margin.X;
                        break;
                    default:
                        break;
                }

                switch (VerticalAlignment)
                {
                    case ImpositionSettings.VAlignment.VALIGN_BOTTOM:
                        box.YMin = Margin.Y;
                        box.YMax = FormatDimensions.Y - MinMargin.Y;
                        break;
                    case ImpositionSettings.VAlignment.VALIGN_TOP:
                        box.YMin = MinMargin.Y;
                        box.YMax = FormatDimensions.Y - Margin.Y;
                        break;
                    case ImpositionSettings.VAlignment.VALIGN_CENTER:
                        box.YMin = Margin.Y;
                        box.YMax = FormatDimensions.Y - Margin.Y;
                        break;
                    default:
                        break;
                }
                return box;
            }
        }
        #endregion

        #region Static methods
        public static bool ComputePositionDefault(IEntityContainer factory, Box2D boxBorder, Vector2D spaceBetween, ref List<BPosition> lPos, ref Box2D bbox)
        {
            if (boxBorder.Width <= 0.0 || boxBorder.Height <= 0.0)
                return false;

            ImpositionTool tool = Instantiate(factory, new ImpositionSettings(boxBorder.Width, boxBorder.Height));
            tool.SpaceBetween = spaceBetween;
            tool.AllowOrthogonalImposition = true;
            tool.AllowBothDirection = false;
            tool.HorizontalAlignment = ImpositionSettings.HAlignment.HALIGN_LEFT;
            tool.VerticalAlignment = ImpositionSettings.VAlignment.VALIGN_BOTTOM;
            tool.GenerateSortedSolutionList(null, out List<ImpositionSolution> solutions);
            if (solutions.Count > 0)
            {
                ImpositionSolution impSol = solutions[0];
                Vector2D cardboardPos = impSol.CardboardPosition;
                foreach (var p in impSol.Positions)
                {
                    BPosition pos = p;
                    pos.Pt += boxBorder.PtMin - cardboardPos;
                    lPos.Add(pos);
                }
                Vector2D ptMin = impSol.Bbox.PtMin + boxBorder.PtMin;
                Vector2D ptMax = impSol.Bbox.PtMax + boxBorder.PtMin;
                bbox = new Box2D(ptMin, ptMax);
            }
            return lPos.Count > 0;
        }
        #endregion
    }
    #endregion

    #region Cardboard format and loader
    public class CardboardFormat
    {
        #region Constructor
        public CardboardFormat(int id, string name, string description, double width, double height)
        {
            Id = id;
            Name = name;
            Description = description;
            Width = width;
            Height = height;
        }
        #endregion

        #region Public properties
        public Vector2D Dimensions { get { return new Vector2D(Width, Height); } }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Width { get; }
        public double Height { get; }
        public double Area { get { return Width * Height; } }
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return string.Format("{0} ({1} * {2})", Name, Width, Height);
        }
        #endregion
    }

    public abstract class CardboardFormatLoader
    {
        #region Abstract methods
        public abstract CardboardFormat[] LoadCardboardFormats();
        public abstract void EditCardboardFormats();
        #endregion
    }

    public class CardboardFormatLoaderDefault : CardboardFormatLoader
    {
        #region Constructor
        public CardboardFormatLoaderDefault()
        {
        }
        #endregion

        #region CardboardFormatLoader implementation
        public override CardboardFormat[] LoadCardboardFormats()
        {
            List<CardboardFormat> listCardboardFormat = new List<CardboardFormat>();
            int id = 0;
            for (int i = 0; i < 5; ++i)
                for (int j = i; j < 5; ++j)
                {
                    string name = string.Format("F_{0}_{1}", i+1, j+1);
                    listCardboardFormat.Add(new CardboardFormat(id, name, name, (i+1) * 1000.0, (j+1) * 1000.0));
                    ++id;
                }
            return listCardboardFormat.ToArray();
        }

        public override void EditCardboardFormats()
        {
            throw new Exception("This method is not implemented in CardboardFormatLoaderDefault");
        }
        #endregion
    }
    #endregion
}
