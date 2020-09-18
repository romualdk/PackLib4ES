#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IxMilia.Dxf;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorDxfOutputIxMilia : PicVisitorOutput, IDisposable
    {
        #region Constructor
        public PicVisitorDxfOutputIxMilia()
        {
        }
        #endregion

        #region Helpers
        public string LineTypeToDxfLayer(PicGraphics.LT lType)
        {
            switch (lType)
            {
                case PicGraphics.LT.LT_CUT: return "L5-113";
                case PicGraphics.LT.LT_PERFOCREASING: return "L6-133";
                case PicGraphics.LT.LT_CONSTRUCTION: return "LCN-27";
                case PicGraphics.LT.LT_PERFO: return "EC1-193";
                case PicGraphics.LT.LT_HALFCUT: return "LI5-103";
                case PicGraphics.LT.LT_CREASING: return "L8-123";
                case PicGraphics.LT.LT_AXIS: return "L2-106";
                case PicGraphics.LT.LT_COTATION: return "LDM-4";
                case PicGraphics.LT.LT_GRID: return "L2-106";
                default: return "";
            }
        }
        #endregion

        #region PicFactoryVisitor
        public override void Initialize(PicFactory factory)
        {
            dxfFile = new DxfFile();
        }
        public override void ProcessEntity(PicEntity entity)
        {
            if (entity is PicTypedDrawable drawable)
            {
                switch (drawable.Code)
                {
                    case PicEntity.ECode.PE_POINT:
                        break;
                    case PicEntity.ECode.PE_SEGMENT:
                        break;
                    case PicEntity.ECode.PE_ARC:
                        break;
                    case PicEntity.ECode.PE_COTATIONDISTANCE:
                        break;
                    case PicEntity.ECode.PE_COTATIONHORIZONTAL:
                        break;
                    case PicEntity.ECode.PE_COTATIONVERTICAL:
                        break;
                    case PicEntity.ECode.PE_ELLIPSE:
                        break;
                    case PicEntity.ECode.PE_NURBS:
                        break;
                    default:
                        break;
                }
            }
        }
        public override void Finish()
        {

        }
        public override byte[] GetResultByteArray()
        {
            byte[] byteArray = null;
            using (var stream = new MemoryStream())
            {

            }
            return byteArray;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        #region Private fields
        private DxfFile dxfFile;
        #endregion
    }
}
