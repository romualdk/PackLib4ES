#region Using directives
using System;
using log4net;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorBoundingBox : PicFactoryVisitor, IDisposable
	{
		#region Public constructor
		public PicVisitorBoundingBox()
		{
		}
		#endregion

        #region Public properties
        public bool TakePicBlocksIntoAccount
        {
            get { return _takePicBlocsIntoAccount; }
            set { _takePicBlocsIntoAccount = value; }
        }
        #endregion

        #region PicFactoryVisitor overrides
        public override void Initialize(PicFactory factory)
		{
			Box.Reset();
            try
            {
                if (null != factory.Format)
                    Box.Extend(factory.Format.BBox);
                else if (factory.IsEmpty)
                    Box.Extend(Sharp3D.Math.Core.Vector2D.Zero);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
		}
		public override void ProcessEntity(PicEntity entity)
		{
            // non drawable not taken into account
            if (!(entity is PicDrawable drawable))
                return;
            if (drawable is PicBlockRef)
            { 
                PicBlockRef blockRef = entity as PicBlockRef;
                Box.Extend(blockRef.Box);
            }
            else if (drawable is PicBlock )
            {
                if (_takePicBlocsIntoAccount)
                {
                    PicBlock block = entity as PicBlock;
                    Box.Extend(block.Box);
                }
            }
            else
            {
                if (drawable is PicTypedDrawable typedDrawable
                    && typedDrawable.LineType != PicGraphics.LT.LT_CONSTRUCTION)
                    Box.Extend(drawable.Box);
            }
		}
		public override void Finish()
		{
		}
        public void Dispose()
        {
        }
        #endregion

        #region Public properties
        public Box2D Box { get; } = new Box2D();
        #endregion

        #region Private fields
        protected static readonly ILog _log = LogManager.GetLogger(typeof(PicVisitorBoundingBox));
        protected bool _takePicBlocsIntoAccount = true;
		#endregion
    }

    public partial class Tools
    {
        public static Box2D BoundingBox(PicFactory factory, double marginRatio = 0.0, bool takePicBlocsIntoAccount = true)
        {
            var visitor = new PicVisitorBoundingBox() { TakePicBlocksIntoAccount = takePicBlocsIntoAccount };
            factory.ProcessVisitor(visitor, !PicFilter.FilterCotation);
            Box2D box = visitor.Box;
            if (box.IsValid && marginRatio > 0.0)
                box.AddMarginRatio(marginRatio);
            return box;
        }
        public static Box2D BoundingBox(PicFactory factory, PicFilter filter, double marginRatio = 0.0)
        {
            var visitor = new PicVisitorBoundingBox() { TakePicBlocksIntoAccount = true };
            factory.ProcessVisitor(visitor, filter);
            Box2D box = visitor.Box;
            if (box.IsValid && marginRatio > 0.0)
                box.AddMarginRatio(marginRatio);
            return box;
        }
    }
}
