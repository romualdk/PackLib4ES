#region Using directives
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Sharp3D.Math.Core;
using Sharp3D.Math.Geometry2D;
#endregion

namespace Pic.Factory2D
{
    public class PicBlock : PicDrawable, IEnumerable<PicEntity>, IDisposable, IEntityContainer
    {
        #region Data members
        private List<PicEntity> _entities = new List<PicEntity>();
        private uint _idEntities;
        #endregion

        #region Constructors
        protected PicBlock(uint id)
            :base(id)
        { 
        }
        #endregion

        #region PicEntity implementation (GetCode(), Clone(), ...)
        protected override PicEntity.ECode GetCode()
        {
            return PicEntity.ECode.PE_BLOCK;
        }
        public override PicEntity Clone(IEntityContainer factory)
        {
            return null;
        }
        public static PicBlock CreateNewBlock(uint id, IEntityContainer container, Transform2D transf)
        {
            PicBlock block = new PicBlock(id);
            // copies each entity from container (either block or factory) in block
            foreach (PicEntity entity in container)
            {
                PicTypedDrawable entityNew = entity.Clone(block) as PicTypedDrawable;
                entityNew.Transform(transf);
                block._entities.Add(entityNew);
            }
            return block;
        }

        protected override bool Evaluate()
        {
            _box.Reset();
            foreach (PicEntity entity in _entities)
            {
                if (entity is PicDrawable drawable)
                {
                    if (!(drawable is PicTypedDrawable typeDrawable) || typeDrawable.LineType != PicGraphics.LT.LT_CONSTRUCTION)
                        _box.Extend(drawable.ComputeBox(Transform2D.Identity));
                }
            }
            return true;
        }
        #endregion

        #region Public properties
        public short ExportedGroup
        {
            get
            {
                foreach (PicEntity entity in _entities)
                {
                    if (entity is PicTypedDrawable typedDrawable)
                        return typedDrawable.Group;
                }
                return 0;
            }
        }
        #endregion

        #region IEntityContainer implementation
        public PicEntity GetEntityById(uint id)
        {
            foreach (PicEntity entity in _entities)
            {
                if (null != entity && id == entity.Id)
                    return entity;
            }
            throw new Exception("There is no such thing as entity #" + id);
        }

        public void RemoveEntityById(uint id)
        {
            GetEntityById(id).Deleted = true;
        }

        public void Clear()
        {
            _entities.Clear();
            _idEntities = 0;
        }

        public uint GetNewEntityId()
        {
            return ++_idEntities;
        }

        public void AddEntities(IEntityContainer container, Transform2D transform)
        {
            foreach (PicEntity entityIn in container)
            {
                if (null == entityIn) continue;
                PicTypedDrawable entityOut = (entityIn.Clone(this)) as PicTypedDrawable;
                entityOut.Transform(transform);
                _entities.Add(entityOut);
            }
            SetModified();
        }

        public List<PicEntity> Entities
        { get { return _entities; } }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            _entities.Clear();
        }
        #endregion

        #region IEnumerable implementation
        public IEnumerator<PicEntity> GetEnumerator()
        {
            return new PicEntityEnumerator(_entities); 
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PicEntityEnumerator(_entities);
        }
        #endregion

        #region PicDrawable overrides
        protected override void DrawSpecific(PicGraphics graphics)
        {
            foreach (var entity in _entities)
            {
                if (entity is PicDrawable drawable && !(drawable is PicCotation))
                    drawable.Draw(graphics);
            }
        }
        protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
        { 
            foreach (var entity in _entities)
            {
                if (entity is PicDrawable drawable && !(drawable is PicCotation))
                    drawable.Draw(graphics, transform);
            }
        }

        public override void Transform(Transform2D transf)
        {
            foreach (var entity in _entities)
            {
                if (entity is PicDrawable drawable)
                    drawable.Transform(transf);
            }
        }

        public override Segment[] Segments
        {
            get
            {
                List<Segment> segments = new List<Segment>();
                foreach (var entity in _entities)
                {
                    if (entity is PicDrawable drawable)
                        segments.AddRange(drawable.Segments);
                }
                return segments.ToArray();
            }
        }

        public override Box2D ComputeBox(Transform2D transf)
        {
            Box2D box = new Box2D();
            foreach (var entity in _entities)
            {
                if (entity is PicDrawable drawable)
                    box.Extend(drawable.ComputeBox(transf));
            }
            return box;
        }
        #endregion

        #region System.Object overrides
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"PicBlock id={Id} - Count = {_entities.Count}");
            foreach (var entity in _entities)
                sb.Append("   " + entity.ToString());
            return sb.ToString();
        }
        #endregion
    }

    public class PicBlockRef : PicDrawable
    {
        #region Protected constructors
        protected PicBlockRef(uint id, PicBlock block, Vector2D coord, double angle)
            : base(id)
        {
            Block = block;
            Position = coord;
            Angle = angle;
            SetModified();
        }
        public static PicBlockRef CreateNewBlockRef(uint id, PicBlock block, Vector2D coord, double angle)
        {
            return new PicBlockRef(id, block, coord, angle);
        }
        public static PicBlockRef CreateNewBlockRef(uint id, PicBlock block, Transform2D transf)
        {
            return new PicBlockRef(id, block, transf.TranslationVector, transf.RotationAngle);
        }
        #endregion

        #region Public properties
        public Vector2D Position { get; set; }
        public double Angle { get; private set; }
        public PicBlock Block { get; }
        #endregion

        #region Helpers
        public Transform2D BlockTransformation
        {
            get { return Transform2D.Translation(Position) * Transform2D.Rotation(Angle); }
        }
        #endregion

        #region PicEntity overrides
        protected override ECode GetCode()
        {
            return ECode.PE_BLOCKREF;
        }
        public override PicEntity Clone(IEntityContainer factory)
        {
            return null;
        }
        #endregion

        #region PicDrawable overrides
        protected override void DrawSpecific(PicGraphics graphics)
        {
            foreach (var entity in Block)
            {
                if (entity is PicDrawable drawable && !(drawable is PicCotation))
                    drawable.Draw(graphics, BlockTransformation);
            }
        }
        protected override void DrawSpecific(PicGraphics graphics, Transform2D transform)
        {
            foreach (var entity in Block)
            {
                if (entity is PicDrawable drawable && !(drawable is PicCotation))
                    drawable.Draw(graphics, transform * BlockTransformation);
            }
        }
        public override void Transform(Transform2D transform)
        {
            Position = transform.transform(Position);
            Angle = transform.transform(Angle);
            SetModified();
        }
        protected override bool Evaluate()
        {
            _box.Reset();
            foreach (PicEntity entity in Block)
            {
                if (entity is PicDrawable drawable)
                {
                    if (!(drawable is PicTypedDrawable typeDrawable)
                        || typeDrawable.LineType != PicGraphics.LT.LT_CONSTRUCTION)
                        _box.Extend(drawable.ComputeBox(BlockTransformation));
                }
            }
            return true;
        }
        public override Box2D ComputeBox(Transform2D transform)
        {
            Box2D box = Box2D.Initial;
            foreach (var entity in Block)
            {
                if (entity is PicDrawable drawable)
                {
                    if (!(drawable is PicTypedDrawable typeDrawable)
                        || typeDrawable.LineType != PicGraphics.LT.LT_CONSTRUCTION)
                        box.Extend(drawable.ComputeBox(transform * BlockTransformation));
                }
            }
            return box;
        }

        public override Segment[] Segments
        {
            get
            {
                List<Segment> listSegments = new List<Segment>();
                foreach (var entity in Block)
                {
                    if (entity is PicDrawable picDrawable)
                        foreach (Segment seg in picDrawable.Segments)
                        {
                            seg.Transform(BlockTransformation);
                            listSegments.Add(seg);
                        }
                }
                return listSegments.ToArray();
            }
        }
        #endregion

        #region Distance methods
        public enum DistDirection
        {
            HORIZONTAL_RIGHT
            , HORIZONTAL_LEFT
            , VERTICAL_TOP
            , VERTICAL_BOTTOM
        }

        public static bool Distance(PicBlockRef blockRef1, PicBlockRef blockRef2, DistDirection direction, out double distance)
        {
            // compute needed rotation
            double angle = 0.0;
            switch (direction)
            {
                case DistDirection.HORIZONTAL_RIGHT: angle = 90.0;  break;
                case DistDirection.HORIZONTAL_LEFT: angle = 270.0;  break;
                case DistDirection.VERTICAL_TOP: angle = 0.0;       break;
                case DistDirection.VERTICAL_BOTTOM: angle = 180.0;  break;
                default:
                    break;
            }

            List<Segment> segList1 = BlockRefToSegmentList(blockRef1, angle);
            List<Segment> segList2 = BlockRefToSegmentList(blockRef2, angle);

            // search distance between 2 BlockRef
            distance = double.MaxValue;
            bool success = false;
            foreach (var seg1 in segList1)
            {
                foreach (Segment seg2 in segList2)
                {
                    double dist = double.MaxValue;
                    if ( VerticalDistance.SegmentToAboveSegment(seg1, seg2, ref dist))
                    {
                        distance = Math.Min(distance, dist);
                        success = true;
                    }
                }
            }
            return success;
        }

        private static List<Segment> BlockRefToSegmentList(PicBlockRef blockRef, double angle)
        {
            Transform2D transform = Transform2D.Rotation(angle);
            List<Segment> segList = new List<Segment>();
            Transform2D transformBlock = transform * blockRef.BlockTransformation;

            // convert bolckRef1 to segList1
            foreach (var entity1 in blockRef.Block)
            {
                if (entity1 is PicSegment picSeg1)
                {
                    Segment seg = new Segment(picSeg1.Pt0, picSeg1.Pt1);
                    seg.Transform(transformBlock);
                    segList.Add(seg);
                }
                if (entity1 is PicArc picArc1)
                {
                    Arc arc = new Arc(picArc1.Center, (float)picArc1.Radius, (float)(picArc1.AngleBeg), (float)(picArc1.AngleEnd));
                    List<Segment> arcExploded = arc.Explode(20);
                    foreach (Segment seg in arcExploded)
                    {
                        seg.Transform(transformBlock);
                        segList.Add(seg);
                    }
                }
            }
            return segList;
        }

        public static bool Distance(List<PicBlockRef> blockRefList1, List<PicBlockRef> blockRefList2, DistDirection direction, out double distance)
        {
            distance = double.MaxValue;
            // compute needed rotation
            double angle = 0.0;
            switch (direction)
            {
                case DistDirection.HORIZONTAL_RIGHT: angle = 90.0; break;
                case DistDirection.HORIZONTAL_LEFT: angle = 270.0; break;
                case DistDirection.VERTICAL_TOP: angle = 0.0; break;
                case DistDirection.VERTICAL_BOTTOM: angle = 180.0; break;
                default:
                    break;
            }

            List<Segment> segList1 = new List<Segment>();
            foreach (var blockRef1 in blockRefList1)
                segList1.AddRange(BlockRefToSegmentList(blockRef1, angle));

            List<Segment> segList2 = new List<Segment>();
            foreach (var blockRef2 in blockRefList2)
                segList2.AddRange(BlockRefToSegmentList(blockRef2, angle));

            // search distance between 2 BlockRef
            bool success = false;
            foreach (var seg1 in segList1)
            {
                foreach (var seg2 in segList2)
                {
                    double dist = double.MaxValue;
                    if (VerticalDistance.SegmentToAboveSegment(seg1, seg2, ref dist))
                    {
                        distance = Math.Min(distance, dist);
                        success = true;
                    }
                }
            }
            return success;
        }
        #endregion

        #region System.Object overrides
        public override string ToString()
        {
            return string.Format("PicBlockRef id = {0}", this.Id);
        }
        #endregion
    }
}
