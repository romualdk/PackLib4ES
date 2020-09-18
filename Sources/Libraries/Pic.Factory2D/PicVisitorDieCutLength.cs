#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorDieCutLength : PicFactoryVisitor, IDisposable
    {
        #region Private fields
        Dictionary<PicGraphics.LT, double> _dictionnaryLength = new Dictionary<PicGraphics.LT, double>();
        #endregion

        #region Public constructor
        public PicVisitorDieCutLength()
        {
        }
        #endregion

		#region PicFactoryVisitor overrides
		public override void Initialize(PicFactory factory)
		{
            _dictionnaryLength.Clear();
		}
		public override void ProcessEntity(PicEntity entity)
		{
            if (PicEntity.ECode.PE_BLOCKREF == entity.Code)
            {
                PicBlockRef blockRef = entity as PicBlockRef;
                PicBlock block = blockRef.Block;

                foreach (var blockEntity in block)
                {
                    if (blockEntity is PicTypedDrawable innerDrawable)
                    {
                        double totalLength = innerDrawable.Length;
                        if (_dictionnaryLength.ContainsKey(innerDrawable.LineType))
                            totalLength += _dictionnaryLength[innerDrawable.LineType];
                        _dictionnaryLength[innerDrawable.LineType] = totalLength;
                    }
                }
            }
            else if (PicEntity.ECode.PE_BLOCK == entity.Code)
            {
                PicBlock block = entity as PicBlock;
                foreach (var blockEntity in block)
                {
                    if (blockEntity is PicTypedDrawable innerDrawable)
                    {
                        double totalLength = innerDrawable.Length;
                        if (_dictionnaryLength.ContainsKey(innerDrawable.LineType))
                            totalLength += _dictionnaryLength[innerDrawable.LineType];
                        _dictionnaryLength[innerDrawable.LineType] = totalLength;
                    }
                }
            }
            else if (entity is PicTypedDrawable drawable)
            {
                double totalLength = drawable.Length;
                if (_dictionnaryLength.ContainsKey(drawable.LineType))
                    totalLength += _dictionnaryLength[drawable.LineType];
                _dictionnaryLength[drawable.LineType] = totalLength;
            }
		}
		public override void Finish()
		{
		}
        public void Dispose()
        {
            _dictionnaryLength.Clear();
        }
		#endregion

        #region Public properties
        public Dictionary<PicGraphics.LT, double> Lengths
        { 
            get
            {
                return _dictionnaryLength;
            }
        }
        #endregion
    }
}
