#region Using directives
using System.IO;
using System.Reflection;
#endregion

namespace Dxflib4NET
{
    public class DL_Dxf
    {
        #region Data members
        DL_Codes.dxfversion _version;
        #endregion

        #region Constructor
        public DL_Dxf()
        {
            _version = DL_Codes.dxfversion.AC1027;
        }
        #endregion

        #region Write methods
        public void WritePredefinedHeader(DL_Writer dw)
        {
            string dxfHeaderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dxfHeader.dxf");
            string s = File.ReadAllText(dxfHeaderPath);
            dw.DxfDirectString(s);        
        }

        public void WriteHeader(DL_Writer dw)
        {
            dw.Comment(string.Concat("dxflib ", DL_Codes.DL_VERSION));
            dw.SectionHeader();

            dw.DxfString(9, "$ACADVER");
            switch (_version)
            {
                case DL_Codes.dxfversion.AC1009:
                    dw.DxfString(1, "AC1009");
                    break;
                case DL_Codes.dxfversion.AC1012:
                    dw.DxfString(1, "AC1012");
                    break;
                case DL_Codes.dxfversion.AC1014:
                    dw.DxfString(1, "AC1014");
                    break;
                case DL_Codes.dxfversion.AC1015:
                    dw.DxfString(1, "AC1015");
                    break;
                case DL_Codes.dxfversion.AC1027:
                    dw.DxfString(1, "AC1027");
                    break;
                default:
                    break;
            }
        }

        public void WriteVPort(DL_Writer dw)
        {
            dw.DxfString(0, "TABLE");
            dw.DxfString(2, "VPORT");

            if (_version == DL_Codes.VER_2000)
                dw.DxfHex(5, 0x8);

            if (_version == DL_Codes.VER_2000)
                dw.DxfString(100, "AcDbSymbolTable");

            dw.DxfInt(70, 1);
            dw.DxfString(0, "VPORT");

            if (_version == DL_Codes.VER_2000)
            {
                dw.DxfString(100, "AcDbSymbolTableRecord");
                dw.DxfString(100, "AcDbViewportTablerecord");
            }
            dw.DxfString(2, "*Active");
            dw.DxfInt(70, 0);
            dw.DxfReal(10, 0.0);
            dw.DxfReal(20, 0.0);
            dw.DxfReal(11, 1.0);
            dw.DxfReal(21, 1.0);
            dw.DxfReal(12, 286.3055555555555);
            dw.DxfReal(22, 148.5);
            dw.DxfReal(13, 0.0);
            dw.DxfReal(23, 0.0);
            dw.DxfReal(14, 10.0);
            dw.DxfReal(24, 10.0);
            dw.DxfReal(15, 10.0);
            dw.DxfReal(25, 10.0);
            dw.DxfReal(16, 0.0);
            dw.DxfReal(26, 0.0);
            dw.DxfReal(36, 1.0);
            dw.DxfReal(17, 0.0);
            dw.DxfReal(27, 0.0);
            dw.DxfReal(37, 0.0);
            dw.DxfReal(40, 297.0);
            dw.DxfReal(41, 1.92798353909465);
            dw.DxfReal(42, 50.0);
            dw.DxfReal(43, 0.0);
            dw.DxfReal(44, 0.0);
            dw.DxfReal(50, 0.0);
            dw.DxfReal(51, 0.0);
            dw.DxfInt(71, 0);
            dw.DxfInt(72, 100);
            dw.DxfInt(73, 1);
            dw.DxfInt(74, 3);
            dw.DxfInt(75, 1);
            dw.DxfInt(76, 1);
            dw.DxfInt(77, 0);
            dw.DxfInt(78, 0);

            if (_version == DL_Codes.VER_2000)
            {
                dw.DxfInt(281, 0);
                dw.DxfInt(65, 1);
                dw.DxfReal(110, 0.0);
                dw.DxfReal(120, 0.0);
                dw.DxfReal(130, 0.0);
                dw.DxfReal(111, 1.0);
                dw.DxfReal(121, 0.0);
                dw.DxfReal(131, 0.0);
                dw.DxfReal(112, 0.0);
                dw.DxfReal(122, 1.0);
                dw.DxfReal(132, 0.0);
                dw.DxfInt(79, 0);
                dw.DxfReal(146, 0.0);
            }
            dw.DxfString(0, "ENDTAB");
        }

        public void WriteLineType(DL_Writer dw, DL_LineTypeData data)
        {
            if (data.Name.Length == 0)
                throw new DL_Exception("Line type name must not be empty");

            string sNameUpper = data.Name.ToUpper(); 

        	// write id (not for R12)
            if (sNameUpper.CompareTo("BYBLOCK") == 0) {
                dw.TableLineTypeEntry(0x14);
            } else if (sNameUpper.CompareTo("BYLAYER") == 0) {
                dw.TableLineTypeEntry(0x15);
            } else if (sNameUpper.CompareTo("CONTINUOUS") == 0) {
                dw.TableLineTypeEntry(0x16);
            } else {
                dw.TableLineTypeEntry();
            }

            dw.DxfString(2, data.Name);
    	    dw.DxfInt(70, data.Flags);

            if (sNameUpper.CompareTo("BYBLOCK") == 0)
            {
                dw.DxfString(3, "");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 0);
                dw.DxfReal(40, 0.0);
            }
            else if (sNameUpper.CompareTo("BYLAYER") == 0)
            {
                dw.DxfString(3, "");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 0);
                dw.DxfReal(40, 0.0);
            }
            else if (sNameUpper.CompareTo("CONTINUOUS") == 0)
            {
                dw.DxfString(3, "Solid line");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 0);
                dw.DxfReal(40, 0.0);
            }
            else if (sNameUpper.CompareTo("CREASE") == 0)
            {
                dw.DxfString(3, "_______________");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 0);
                dw.DxfReal(40, 0.0);
            }
            else if (sNameUpper.CompareTo("CUT") == 0)
            {
                dw.DxfString(3, "_______________");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 0);
                dw.DxfReal(40, 0.0);
            }
            else if (sNameUpper.CompareTo("PARTIAL-CUT") == 0)
            {
                dw.DxfString(3, "___ _ ___ _ ___");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 4);
                dw.DxfReal(40, 2.0);
                dw.DxfReal(49, 0.75);
                dw.DxfInt(74, 0);
            }
            else if (sNameUpper.CompareTo("1-2-X-1-2-CUT") == 0)
            {
                dw.DxfString(3, "_ _ _ _ _ _ _ _");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 2);
                dw.DxfReal(40, 1.0);
                dw.DxfReal(49, 0.5);
                dw.DxfInt(74, 0);
            }
            else if (sNameUpper.CompareTo("1-4-X-1-4-CUT") == 0)
            {
                dw.DxfString(3, "_ _ _ _ _ _ _ _");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 2);
                dw.DxfReal(40, 0.5);
                dw.DxfReal(49, 0.25);
                dw.DxfInt(74, 0);
                dw.DxfReal(49, -0.25);
                dw.DxfInt(74, 0);
            }            
            else if (sNameUpper.CompareTo("1-8-X-1-8-CUT") == 0)
            {
                dw.DxfString(3, "_ _ _ _ _ _ _ _");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 2);
                dw.DxfReal(40, 0.25);
                dw.DxfReal(49, 0.125);
                dw.DxfInt(74, 0);
                dw.DxfReal(49, -0.125);
                dw.DxfInt(74, 0);
            }                
            else if (sNameUpper.CompareTo("3-8-X-3-8-CUT") == 0)
            {
                dw.DxfString(3, "_ _ _ _ _ _ _ _");
                dw.DxfInt(72, 65);
                dw.DxfInt(73, 2);
                dw.DxfReal(40, 0.75);
                dw.DxfReal(49, 0.375);
                dw.DxfInt(74, 0);
                dw.DxfReal(49, -0.375);
                dw.DxfInt(74, 0);
            }
            else
                throw new DL_Exception("dxflib warning: DL_Dxf::writeLineType: Unknown Line Type");        }

        public void WritePoint(DL_Writer dw, DL_PointData data, DL_Attributes attrib)
        {
            dw.Entity("POINT");
            dw.DxfString(100, "AcDbEntity");
            dw.EntityAttributes(attrib);
            dw.DxfString(100, "AcDbPoint");
            dw.Coord(DL_Codes.POINT_COORD_CODE, data.x, data.y, data.z);
        }

        public void WriteLine(DL_Writer dw, DL_LineData data, DL_Attributes attrib)
        {
            dw.Entity("LINE");
            dw.DxfString(100, "AcDbEntity");
            dw.EntityAttributes(attrib);
            dw.DxfString(100, "AcDbLine");
            dw.Coord(DL_Codes.LINE_START_CODE, data.x1, data.y1, data.z1);
            dw.Coord(DL_Codes.LINE_END_CODE, data.x2, data.y2, data.z2);
        }

        public void WriteArc(DL_Writer dw, DL_ArcData data, DL_Attributes attrib)
        {
            dw.Entity("ARC");
            dw.DxfString(100, "AcDbEntity");
            dw.EntityAttributes(attrib);
            dw.DxfString(100, "AcDbCircle");
            dw.Coord(10, data.cx, data.cy, data.cz);
            dw.DxfReal(40, data.radius);
            dw.DxfString(100, "AcDbArc");
            dw.DxfReal(50, data.angle1);
            dw.DxfReal(51, data.angle2);
        }

        public void WriteText(DL_Writer dw, DL_TextData data, DL_Attributes attrib)
        {
            dw.Entity("TEXT");
            dw.DxfString(100, "AcDbEntity");
            dw.DxfString(100, "AcDbText");
            dw.EntityAttributes(attrib);
            dw.Coord(10, data.ipx, data.ipy, data.ipz);
            dw.DxfReal(40, data.height);
            dw.DxfString(1, data.text);
            dw.DxfInt(50, 0);
            dw.DxfReal(41, data.xScaleFactor);
            dw.DxfReal(51, data.angle);
            dw.DxfString(7, data.style);
            dw.DxfInt(71, data.textGenerationFlags);
            dw.DxfInt(72, data.hJustification);
            dw.Coord(11, data.apx, data.apy, data.apz);
            dw.DxfInt(73, data.vJustification);
        }

        public void WriteLayer(DL_Writer dw, DL_LayerData data, DL_Attributes attrib)
        {
            if (data.lName.Length == 0)
                throw new DL_Exception("DL_Dxf::writeLayer: Layer name must not be empty\n");

            int color = attrib.Color;
            if (color==0) {
                color = 7;
                throw new DL_Exception("Layer color cannot be 0. Corrected to 7.\n");
            }

            if (data.lName == "0") {
                dw.TableLayerEntry(0x10);
            } else {
                dw.TableLayerEntry();
            }

            dw.DxfString(2, data.lName);
            dw.DxfInt(70, data.lFlag);
            dw.DxfInt(62, color);

            dw.DxfString(6, (attrib.LineType.Length==0 ?
                             "CONTINUOUS" : attrib.LineType));

            if (_version>=DL_Codes.VER_2000) {
                // layer defpoints cannot be plotted
                if (data.lName.ToLower().CompareTo("defpoints") == 0) {
                    dw.DxfInt(290, 0);
                }
            }
            dw.DxfInt(370, attrib.Width);
            if (_version>=DL_Codes.VER_2000) {
                dw.DxfHex(390, 0xF);
            }
        }

        public void WriteObjects(DL_Writer dw)
        {
            dw.DxfString(0, "SECTION");
            dw.DxfString(2, "OBJECTS");
            dw.DxfString(0, "DICTIONARY");
            dw.DxfHex(5, 0xC);                            // C
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(280, 0);
            dw.DxfInt(281, 1);
            dw.DxfString(3, "ACAD_GROUP");
            dw.DxfHex(350, 0xD);          // D
            dw.DxfString(3, "ACAD_LAYOUT");
            dw.DxfHex(350, 0x1A);
            dw.DxfString(3, "ACAD_MLINESTYLE");
            dw.DxfHex(350, 0x17);
            dw.DxfString(3, "ACAD_PLOTSETTINGS");
            dw.DxfHex(350, 0x19);
            dw.DxfString(3, "ACAD_PLOTSTYLENAME");
            dw.DxfHex(350, 0xE);
            dw.DxfString(3, "AcDbVariableDictionary");
            dw.DxfHex(350, dw.GetNextHandle());        // 2C
            dw.DxfString(0, "DICTIONARY");
            dw.DxfHex(5, 0xD);
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(280, 0);
            dw.DxfInt(281, 1);
            dw.DxfString(0, "ACDBDICTIONARYWDFLT");
            dw.DxfHex(5, 0xE);
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(281, 1);
            dw.DxfString(3, "Normal");
            dw.DxfHex(350, 0xF);
            dw.DxfString(100, "AcDbDictionaryWithDefault");
            dw.DxfHex(340, 0xF);
            dw.DxfString(0, "ACDBPLACEHOLDER");
            dw.DxfHex(5, 0xF);
            dw.DxfString(0, "DICTIONARY");
            dw.DxfHex(5, 0x17);
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(280, 0);
            dw.DxfInt(281, 1);
            dw.DxfString(3, "Standard");
            dw.DxfHex(350, 0x18);
            dw.DxfString(0, "MLINESTYLE");
            dw.DxfHex(5, 0x18);
            dw.DxfString(100, "AcDbMlineStyle");
            dw.DxfString(2, "STANDARD");
            dw.DxfInt(70, 0);
            dw.DxfString(3, "");
            dw.DxfInt(62, 256);
            dw.DxfReal(51, 90.0);
            dw.DxfReal(52, 90.0);
            dw.DxfInt(71, 2);
            dw.DxfReal(49, 0.5);
            dw.DxfInt(62, 256);
            dw.DxfString(6, "BYLAYER");
            dw.DxfReal(49, -0.5);
            dw.DxfInt(62, 256);
            dw.DxfString(6, "BYLAYER");
            dw.DxfString(0, "DICTIONARY");
            dw.DxfHex(5, 0x19);
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(280, 0);
            dw.DxfInt(281, 1);
            dw.DxfString(0, "DICTIONARY");
            dw.DxfHex(5, 0x1A);
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(281, 1);
            dw.DxfString(3, "Layout1");
            dw.DxfHex(350, 0x1E);
            dw.DxfString(3, "Layout2");
            dw.DxfHex(350, 0x26);
            dw.DxfString(3, "Model");
            dw.DxfHex(350, 0x22);
            dw.DxfString(0, "LAYOUT");
            dw.DxfHex(5, 0x1E);
            dw.DxfString(100, "AcDbPlotSettings");
            dw.DxfString(1, "");
            dw.DxfString(2, "none_device");
            dw.DxfString(4, "Letter_(8.50_x_11.00_Inches)");
            dw.DxfString(6, "");
            dw.DxfReal(40, 0.0);
            dw.DxfReal(41, 0.0);
            dw.DxfReal(42, 0.0);
            dw.DxfReal(43, 0.0);
            dw.DxfReal(44, 0.0);
            dw.DxfReal(45, 0.0);
            dw.DxfReal(46, 0.0);
            dw.DxfReal(47, 0.0);
            dw.DxfReal(48, 0.0);
            dw.DxfReal(49, 0.0);
            dw.DxfReal(140, 0.0);
            dw.DxfReal(141, 0.0);
            dw.DxfReal(142, 1.0);
            dw.DxfReal(143, 1.0);
            dw.DxfInt(70, 688);
            dw.DxfInt(72, 0);
            dw.DxfInt(73, 0);
            dw.DxfInt(74, 5);
            dw.DxfString(7, "");
            dw.DxfInt(75, 16);
            dw.DxfReal(147, 1.0);
            dw.DxfReal(148, 0.0);
            dw.DxfReal(149, 0.0);
            dw.DxfString(100, "AcDbLayout");
            dw.DxfString(1, "Layout1");
            dw.DxfInt(70, 1);
            dw.DxfInt(71, 1);
            dw.DxfReal(10, 0.0);
            dw.DxfReal(20, 0.0);
            dw.DxfReal(11, 420.0);
            dw.DxfReal(21, 297.0);
            dw.DxfReal(12, 0.0);
            dw.DxfReal(22, 0.0);
            dw.DxfReal(32, 0.0);
            dw.DxfReal(14, 1.000000000000000E+20);
            dw.DxfReal(24, 1.000000000000000E+20);
            dw.DxfReal(34, 1.000000000000000E+20);
            dw.DxfReal(15, -1.000000000000000E+20);
            dw.DxfReal(25, -1.000000000000000E+20);
            dw.DxfReal(35, -1.000000000000000E+20);
            dw.DxfReal(146, 0.0);
            dw.DxfReal(13, 0.0);
            dw.DxfReal(23, 0.0);
            dw.DxfReal(33, 0.0);
            dw.DxfReal(16, 1.0);
            dw.DxfReal(26, 0.0);
            dw.DxfReal(36, 0.0);
            dw.DxfReal(17, 0.0);
            dw.DxfReal(27, 1.0);
            dw.DxfReal(37, 0.0);
            dw.DxfInt(76, 0);
            dw.DxfHex(330, 0x1B);
            dw.DxfString(0, "LAYOUT");
            dw.DxfHex(5, 0x22);
            dw.DxfString(100, "AcDbPlotSettings");
            dw.DxfString(1, "");
            dw.DxfString(2, "none_device");
            dw.DxfString(4, "");
            dw.DxfString(6, "");
            dw.DxfReal(40, 0.0);
            dw.DxfReal(41, 0.0);
            dw.DxfReal(42, 0.0);
            dw.DxfReal(43, 0.0);
            dw.DxfReal(44, 0.0);
            dw.DxfReal(45, 0.0);
            dw.DxfReal(46, 0.0);
            dw.DxfReal(47, 0.0);
            dw.DxfReal(48, 0.0);
            dw.DxfReal(49, 0.0);
            dw.DxfReal(140, 0.0);
            dw.DxfReal(141, 0.0);
            dw.DxfReal(142, 1.0);
            dw.DxfReal(143, 1.0);
            dw.DxfInt(70, 1712);
            dw.DxfInt(72, 0);
            dw.DxfInt(73, 0);
            dw.DxfInt(74, 0);
            dw.DxfString(7, "");
            dw.DxfInt(75, 0);
            dw.DxfReal(147, 1.0);
            dw.DxfReal(148, 0.0);
            dw.DxfReal(149, 0.0);
            dw.DxfString(100, "AcDbLayout");
            dw.DxfString(1, "Model");
            dw.DxfInt(70, 1);
            dw.DxfInt(71, 0);
            dw.DxfReal(10, 0.0);
            dw.DxfReal(20, 0.0);
            dw.DxfReal(11, 12.0);
            dw.DxfReal(21, 9.0);
            dw.DxfReal(12, 0.0);
            dw.DxfReal(22, 0.0);
            dw.DxfReal(32, 0.0);
            dw.DxfReal(14, 0.0);
            dw.DxfReal(24, 0.0);
            dw.DxfReal(34, 0.0);
            dw.DxfReal(15, 0.0);
            dw.DxfReal(25, 0.0);
            dw.DxfReal(35, 0.0);
            dw.DxfReal(146, 0.0);
            dw.DxfReal(13, 0.0);
            dw.DxfReal(23, 0.0);
            dw.DxfReal(33, 0.0);
            dw.DxfReal(16, 1.0);
            dw.DxfReal(26, 0.0);
            dw.DxfReal(36, 0.0);
            dw.DxfReal(17, 0.0);
            dw.DxfReal(27, 1.0);
            dw.DxfReal(37, 0.0);
            dw.DxfInt(76, 0);
            dw.DxfHex(330, 0x1F);
            dw.DxfString(0, "LAYOUT");
            dw.DxfHex(5, 0x26);
            dw.DxfString(100, "AcDbPlotSettings");
            dw.DxfString(1, "");
            dw.DxfString(2, "none_device");
            dw.DxfString(4, "");
            dw.DxfString(6, "");
            dw.DxfReal(40, 0.0);
            dw.DxfReal(41, 0.0);
            dw.DxfReal(42, 0.0);
            dw.DxfReal(43, 0.0);
            dw.DxfReal(44, 0.0);
            dw.DxfReal(45, 0.0);
            dw.DxfReal(46, 0.0);
            dw.DxfReal(47, 0.0);
            dw.DxfReal(48, 0.0);
            dw.DxfReal(49, 0.0);
            dw.DxfReal(140, 0.0);
            dw.DxfReal(141, 0.0);
            dw.DxfReal(142, 1.0);
            dw.DxfReal(143, 1.0);
            dw.DxfInt(70, 688);
            dw.DxfInt(72, 0);
            dw.DxfInt(73, 0);
            dw.DxfInt(74, 5);
            dw.DxfString(7, "");
            dw.DxfInt(75, 16);
            dw.DxfReal(147, 1.0);
            dw.DxfReal(148, 0.0);
            dw.DxfReal(149, 0.0);
            dw.DxfString(100, "AcDbLayout");
            dw.DxfString(1, "Layout2");
            dw.DxfInt(70, 1);
            dw.DxfInt(71, 2);
            dw.DxfReal(10, 0.0);
            dw.DxfReal(20, 0.0);
            dw.DxfReal(11, 12.0);
            dw.DxfReal(21, 9.0);
            dw.DxfReal(12, 0.0);
            dw.DxfReal(22, 0.0);
            dw.DxfReal(32, 0.0);
            dw.DxfReal(14, 0.0);
            dw.DxfReal(24, 0.0);
            dw.DxfReal(34, 0.0);
            dw.DxfReal(15, 0.0);
            dw.DxfReal(25, 0.0);
            dw.DxfReal(35, 0.0);
            dw.DxfReal(146, 0.0);
            dw.DxfReal(13, 0.0);
            dw.DxfReal(23, 0.0);
            dw.DxfReal(33, 0.0);
            dw.DxfReal(16, 1.0);
            dw.DxfReal(26, 0.0);
            dw.DxfReal(36, 0.0);
            dw.DxfReal(17, 0.0);
            dw.DxfReal(27, 1.0);
            dw.DxfReal(37, 0.0);
            dw.DxfInt(76, 0);
            dw.DxfHex(330, 0x23);
            dw.DxfString(0, "DICTIONARY");
            dw.Handle();                           // 2C
            dw.DxfString(100, "AcDbDictionary");
            dw.DxfInt(281, 1);
            dw.DxfString(3, "DIMASSOC");
            dw.DxfHex(350, dw.GetNextHandle() + 1);        // 2E
            dw.DxfString(3, "HIDETEXT");
            dw.DxfHex(350, dw.GetNextHandle());        // 2D
            dw.DxfString(0, "DICTIONARYVAR");
            dw.Handle();                                    // 2E
            dw.DxfString(100, "DictionaryVariables");
            dw.DxfInt(280, 0);
            dw.DxfInt(1, 2);
            dw.DxfString(0, "DICTIONARYVAR");
            dw.Handle();                                    // 2D
            dw.DxfString(100, "DictionaryVariables");
            dw.DxfInt(280, 0);
            dw.DxfInt(1, 1);
        }

        public void WriteObjectsEnd(DL_Writer dw)
        {
            dw.DxfString(0, "ENDSEC");
        }

        #endregion
    }
}
