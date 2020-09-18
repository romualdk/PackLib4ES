#region Using directives
using System;
using System.Collections.Generic;

using Dxflib4NET;

using Sharp3D.Math.Geometry2D;
using Sharp3D.Math.Core;
#endregion

namespace Pic.Factory2D
{
    public class PicVisitorDxfOutput: PicVisitorOutput, IDisposable
    {
        #region Public constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PicVisitorDxfOutput()
        {
        }
        #endregion

        #region Helpers
        public string LineTypeToDxfLayer(PicGraphics.LT lType)
        { 
            switch (lType)
            {
                case PicGraphics.LT.LT_CUT:             return "L5-113";
                case PicGraphics.LT.LT_PERFOCREASING:   return "L6-133";
                case PicGraphics.LT.LT_CONSTRUCTION:    return "LCN-27";
                case PicGraphics.LT.LT_PERFO:           return "EC1-193";
                case PicGraphics.LT.LT_HALFCUT:         return "LI5-103";
                case PicGraphics.LT.LT_CREASING:        return "L8-123";
                case PicGraphics.LT.LT_AXIS:            return "L2-106";
                case PicGraphics.LT.LT_COTATION:        return "LDM-4";
                case PicGraphics.LT.LT_GRID:            return "L2-106";
                default:    return "";
            }

            // **
            // "Cut", "L5-113"
            // "Perfo-Crease", "L6-133"
            // "Construction", "LCN-27"
            // "Perfo", "EC1-193"
            // "Half-Cut", "LI5-103"
            // "Crease", "L8-123"
            // "Axis", "L2-106"
            // "Dimension", "LDM-4"
            // **
        }
        #endregion

        #region PicFactoryVisitor overrides
        /// <summary>
        /// Initialize string builder with initial sections + line types
        /// Instantiate DL_Dxf class and DL_Writer class
        /// </summary>
        /// <param name="factory"></param>
        public override void Initialize(PicFactory factory)
        {
            DL_Codes.dxfversion version = DL_Codes.dxfversion.AC1012;
            dw = new DL_Writer(version);
            dxf = new DL_Dxf();
            dxf.WriteHeader(dw);
            dw.SectionEnd();
            // opening the table section
            dw.SectionTables();
            // writing viewports
            dxf.WriteVPort(dw);
            // writing line types
            dw.TableLineTypes(25);
            dxf.WriteLineType(dw, new DL_LineTypeData("BYBLOCK", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("BYLAYER", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("CONTINUOUS", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("ACAD_ISO02W100", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("ACAD_ISO03W100", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("ACAD_ISO04W100", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("ACAD_ISO05W100", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("BORDER", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("BORDER2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("BORDERX2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("CENTER", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("CENTER2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("CENTERX2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHDOT", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHDOT2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHDOTX2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHED", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHED2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DASHEDX2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DIVIDE", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DIVIDE2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DIVIDEX2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DOT", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DOT2", 0));
            dxf.WriteLineType(dw, new DL_LineTypeData("DOTX2", 0));
            dw.TableEnd();
            // writing the layers
            int numberOfLayers = 3;
            dw.tableLayers(numberOfLayers);
            // CUT
            dxf.WriteLayer(dw, new DL_LayerData("L5-113", 0),
                new DL_Attributes("",                       // leave empty
                    (int)DL_Codes.dxfcolor.red,             // default color
                    100,                                    // default width
                    "CONTINUOUS"));                         // default line style
            // FOLD
            dxf.WriteLayer(dw, new DL_LayerData("L8-123", 0),
                new DL_Attributes("",                       // leave empty
                    (int)DL_Codes.dxfcolor.blue,            // default color
                    100,                                    // default width
                    "CONTINUOUS"));                         // default line style
            // COTATION
            dxf.WriteLayer(dw, new DL_LayerData("LDM-4", 0),
                new DL_Attributes("",                       // leave empty
                    (int)DL_Codes.dxfcolor.green,           // default color
                    100,                                    // default width
                    "CONTINUOUS"));                         // default line style
            dw.TableEnd();
            dw.SectionEnd();
            // write all entities
            dw.SectionEntities();
        }
        /// <summary>
        /// ProcessEntity : write entity corresponding dxf description in line buffer 
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void ProcessEntity(PicEntity entity)
        {
            PicTypedDrawable drawable = (PicTypedDrawable)entity;
            if (null != drawable)
            {
                switch (drawable.Code)
                {
                    case PicEntity.ECode.PE_POINT:
                        break;
                    case PicEntity.ECode.PE_SEGMENT:
                        {
                            PicSegment seg = (PicSegment)entity;
                             dxf.WriteLine(
                                dw
                                , new DL_LineData(
                                    seg.Pt0.X		// start point
                                    , seg.Pt0.Y
                                    , 0.0
                                    , seg.Pt1.X	// end point
                                    , seg.Pt1.Y
                                    , 0.0
                                    , 256
                                    , LineTypeToDxfLayer(seg.LineType)
                                    )
                                , new DL_Attributes(LineTypeToDxfLayer(seg.LineType), 256, -1, "BYLAYER")
                                );

                        }
                        break;
                    case PicEntity.ECode.PE_ARC:
                        {
                            PicArc arc = (PicArc)entity;
                            double ang = arc.AngleEnd - arc.AngleBeg, angd = arc.AngleBeg, ango = arc.AngleEnd - arc.AngleBeg;
                           	if ( ang <  0.0 )
	                        {
		                        angd += ang;
		                        ango = - ang;
	                        }
	                        else
		                        ango = ang;
                            
                            dxf.writeArc(dw,
                                new DL_ArcData(
                                    arc.Center.X, arc.Center.Y, 0.0,
                                    arc.Radius,
                                    angd, angd + ango,
                                    256,
                                    LineTypeToDxfLayer(arc.LineType)
                                    ),
                                new DL_Attributes(LineTypeToDxfLayer(arc.LineType), 256, -1, "BYLAYER")
                            );
                        }
                        break;
                    case PicEntity.ECode.PE_COTATIONDISTANCE:
                    case PicEntity.ECode.PE_COTATIONHORIZONTAL:
                    case PicEntity.ECode.PE_COTATIONVERTICAL:
                        {
                            PicCotationDistance cotation = entity as PicCotationDistance;
                            List<Segment> segments = new List<Segment>();
                            Vector2D textPt = Vector2D.Zero;
                            double textSize = 0.0;
                            cotation.DrawSeg(ref segments, ref textPt, ref textSize);
                            // draw segments
                            foreach (Segment seg in segments)
                            {
                                dxf.WriteLine(dw,
                                    new DL_LineData(
                                        seg.P0.X, seg.P0.Y, 0.0,
                                        seg.P1.X, seg.P1.Y, 0.0,
                                        256,
                                        LineTypeToDxfLayer(PicGraphics.LT.LT_COTATION)
                                        ),
                                    new DL_Attributes(LineTypeToDxfLayer(PicGraphics.LT.LT_COTATION), 256, -1, "BYLAYER")
                                    );
                            }
                            // draw text
                            dxf.writeText(dw,
                                new DL_TextData(
                                    textPt.X, textPt.Y, 0.0,
                                    textPt.X, textPt.Y, 0.0,
                                    textSize, 1.0, 0,
                                    1, 2, cotation.Text, "STANDARD", 0.0),
                                new DL_Attributes(LineTypeToDxfLayer(PicGraphics.LT.LT_COTATION), 256, -1, "BYLAYER"));
                        }
                        break;
                    case PicEntity.ECode.PE_ELLIPSE:
                        break;
                    case PicEntity.ECode.PE_NURBS:
                        break;
                    default:
                        throw new Exception("Can not export this kind of entity!");
                }
            }
        }
        /// <summary>
        /// Finish : writes end file statements to the stringbuilder
        /// </summary>
        public override void Finish()
        {
            // end section entities
            dw.SectionEnd();

            // writing the object section
            dxf.WriteObjects(dw);
            dxf.WriteObjectsEnd(dw);

            // ending and closing the file
            dw.DxfEOF();
            dw.Close();
        }

        public void Dispose()
        {
        }
        #endregion

        #region Public methods
        public override byte[] GetResultByteArray()
        {
            // convert string to byte array
            string textOutput = dw.ToString();
            byte[] byteArray = new byte[textOutput.Length];
            for (int i = 0; i < textOutput.Length; ++i)
                byteArray[i] = Convert.ToByte(textOutput[i]);
            return byteArray;
        }
        #endregion

        #region Public properties
        public String TextOutput
        {
            get
            {
                return dw.ToString();
            }
        }
        #endregion

        #region Private fields
        private DL_Dxf dxf;
        private DL_Writer dw;
        #endregion
    }
}
