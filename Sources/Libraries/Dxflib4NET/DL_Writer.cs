#region Using directives
using System;
using System.Text;
using System.Collections.Generic;
#endregion

namespace Dxflib4NET
{
    /// <summary>
    /// Dxf
    /// </summary>
    public class DL_Writer
    {
        #region Data members
        StringBuilder _fileBuilder;
        DL_Codes.dxfversion _version;
        long m_handle;
        ulong _modelSpaceHandle;
        ulong _paperSpaceHandle;
        ulong _paperSpace0Handle;

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        public DL_Writer(DL_Codes.dxfversion version)
        {
            _fileBuilder = new StringBuilder();
            _version = version;
            m_handle = 0x100;
            _modelSpaceHandle = 0;
            _paperSpaceHandle = 0;
            _paperSpace0Handle = 0;
        }
        public void Close()
        {
        }

        /// <summary>
        /// Generic section for section 'name'
        /// </summary>
        /// <param name="name">Section name</param>
        public void Section(string name)
        {
            DxfString(0, "SECTION");
            DxfString(2, name); 
        }
        /// <summary>
        /// Section HEADER.
        /// </summary>
        public void SectionHeader()
        {
            Section("HEADER");
        }
        /// <summary>
        /// Section TABLES.
        /// </summary>
        public void SectionTables()
        {
            Section("TABLES");
        }
        /// <summary>
        /// Section BLOCKS.
        /// </summary>
        public void SectionBlocks()
        {
            Section("BLOCKS");
        }
        /// <summary>
        /// Section ENTITIES.
        /// </summary>
        public void SectionEntities()
        {
            Section("ENTITIES");
        }
        /// <summary>
        /// Section CLASSES.
        /// </summary>
        public void SectionClasses()
        {
            Section("CLASSES");
        }
        /// <summary>
        /// Section OBJECTS.
        /// </summary>
        public void SectionObjects()
        {
            Section("OBJECTS");
        }
        /// <summary>
        /// End of a section.
        /// </summary>
        public void SectionEnd()
        {
            DxfString(0, "ENDSEC");
        }
        /// <summary>
        /// Generic table for table 'name' with 'num' entries
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="num">Number</param>
        /// <param name="handle">Handle</param>
        public void Table(string name, int num, int handle)
        {
            DxfString(0, "TABLE");
            DxfString(2, name);
            DxfHex(5, handle);
            DxfString(100, "AcDbSymbolTable");
            DxfInt(70, num);
        }
        /// <summary>
        /// Table for layers.
        /// </summary>
        /// <param name="num">Number of layers in total.</param>
        public void tableLayers(int num)
        {
            Table("LAYER", num, 2);
        }
        /// <summary>
        /// Table for line types.
        /// </summary>
        /// <param name="num">Number of line types in total.</param>
        public void TableLineTypes(int num)
        {
            Table("LTYPE", num, 5);
        }
        /// <summary>
        /// Table for application id.
        /// </summary>
        /// <param name="num">Number of applications registered in total.</param>
        public void TableAppid(int num)
        {
            Table("APPID", num, 9);
        }
        /// <summary>
        /// End of a table
        /// </summary>
        public void TableEnd()
        {
            DxfString(0, "ENDTAB");
        }
        /// <summary>
        /// End of DXF file.
        /// </summary>
        public void DxfEOF()
        {
            DxfString(0, "EOF");
        }
        /// <summary>
        /// Comment
        /// </summary>
        /// <param name="text"></param>
        public void Comment(string text)
        {
            DxfString(999, text);
        }
        /// <summary>
        /// Entity
        /// </summary>
        /// <param name="entTypeName">Entity type name</param>
        public void Entity(string entTypeName)
        {
            DxfString(0, entTypeName);
            Handle(5);
        }
        /// <summary>
        /// Attributes of an entity
        /// </summary>
        /// <param name="attrib"></param>
        public void EntityAttributes(DL_Attributes attrib)
        { 
            // layer name
            DxfString(8, attrib.Layer);
            DxfString(6, attrib.LineType);
            DxfInt(62, attrib.Color);
            DxfReal(370, attrib.Width != -1.0 ? attrib.Width : 0.0);
        }
        /// <summary>
        /// Subclass
        /// </summary>
        /// <param name="sub">Subclass name</param>
        public void SubClass(string sub)
        {
            DxfString(100, sub);
        }
        /// <summary>
        /// Layer (must be in the TABLES section LAYER).
        /// </summary>
        /// <param name="h">Layer handle</param>
        public void TableLayerEntry(long h)
        {
            DxfString(0, "LAYER");
            if (_version >= DL_Codes.VER_2000)
            {
                if (h == 0)
                    Handle();
                else
                    DxfHex(5, h);
                DxfString(100, "AcDbSymbolTablerecord");
                DxfString(100, "AcDbLayerTableRecord");
            }
        }
        public void TableLayerEntry()
        { 
            DxfString(0, "LAYER");
            if (_version >= DL_Codes.VER_2000)
            {
                Handle();
                DxfString(100, "AcDbSymbolTablerecord");
                DxfString(100, "AcDbLayerTableRecord");
            }
        }
        /// <summary>
        /// Line type (must be in the TABLES section LTYPE)
        /// </summary>
        /// <param name="h">Line type handle</param>
        public void TableLineTypeEntry(long h = 0)
        {
            DxfString(0, "LTYPE");
            if (h == 0)
                Handle();
            else
                DxfHex(5, h);
            DxfString(100, "AcDbSymbolTableRecord");
            DxfString(100, "AcDbLinetypeTableRecord");
        }
        /// <summary>
        /// Appid (must be in the TABLES section APPID).
        /// </summary>
        /// <param name="h">APPID</param>
        public void TableAppidEntry(long h)
        {
            DxfString(0, "APPID");
            if (_version>=DL_Codes.VER_2000) {
                if (h==0)
                    Handle();
                else
                    DxfHex(5, h);
                DxfString(100, "AcDbSymbolTableRecord");
                DxfString(100, "AcDbRegAppTableRecord");
            }
        }
        public void TableAppidEntry()
        {
            DxfString(0, "APPID");
            if (_version >= DL_Codes.VER_2000)
            {
                Handle();
                DxfString(100, "AcDbSymbolTableRecord");
                DxfString(100, "AcDbRegAppTableRecord");
            }
        }       
        /// <summary>
        /// Block (must be in the section BLOCKS).
        /// </summary>
        /// <param name="h"></param>
        public void SectionBlockEntry(int h)
        {
            DxfString(0, "BLOCK");
            if (_version >= DL_Codes.VER_2000)
            {
                if (h == 0)
                    Handle(5);
                else
                    DxfHex(5, h);
                DxfString(100, "AcDbEntity");
                if (h == 0x1C)
                    DxfInt(67, 1);
            }
        }
        /// <summary>
        /// End of block (must be in the section BLOCKS).
        /// </summary>
        /// <param name="h"></param>
        public void SectionBlockEntryEnd(int h)
        {
            DxfString(0, "ENDBLK");
            if (_version >= DL_Codes.VER_2000)
            {
                if (h == 0)
                    Handle();
                else
                    DxfHex(5, h);
                DxfString(100, "AcDbEntity");
                if (h == 0x1D)
                    DxfInt(67, 1);
                DxfString(8, "0");
                DxfString(100, "AcDbBlockEnd");
            }
        }
        public void SectionBlockEntryEnd()
        {
            DxfString(0, "ENDBLK");
            if (_version >= DL_Codes.VER_2000)
            {
                Handle();
                DxfString(100, "AcDbEntity");
                DxfString(8, "0");
                DxfString(100, "AcDbBlockEnd");
            }
        }
        public void Color(int col)
        {
            DxfInt(62, col);
        }
        public void LineType(string lt)
        {
            DxfString(6, lt);
        }
        public void LineWeight(int lw)
        {
            DxfInt(370, lw);
        }
        public void Coord(uint gc, double x, double y, double z)
        {
            DxfReal(gc, x);
            DxfReal(gc+10, y);
            DxfReal(gc+20, z);
        }
        /// <summary>
        /// Reset handle
        /// </summary>
        public void ResetHandle()
        {
            m_handle = 1;        
        }
        /// <summary>
        /// Write a unique handle and returns it.
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <returns></returns>
        public long Handle(uint gc)
        {
            // handle has to be Hex
            DxfHex(gc, m_handle);
            return m_handle++;
        }
        /// <summary>
        /// Write a unique handle and returns it.
        /// </summary>
        /// <returns>Handle</returns>
        public long Handle()
        {
            DxfHex(5, m_handle);
            return m_handle++;
        }
        /// <summary>
        /// Next handle that will be written
        /// </summary>
        /// <returns>Handle to be written</returns>
        public long GetNextHandle()
        {
            return m_handle;
        }
        /// <summary>
        /// Increase handle so that the handle returned remains available.
        /// </summary>
        /// <returns></returns>
        public long IncHandle()
        { 
            return m_handle++;
        }
        /// <summary>
        /// Sets the handle of the model space.
        /// Entities refer to this handle.
        /// </summary>
        /// <param name="h">Handle</param>
        public void SetModelSpaceHandle(ulong h)
        {
            _modelSpaceHandle = h;
        }
        public ulong GetModelSpaceHandle()
        {
            return _modelSpaceHandle;
        }
        /// <summary>
        /// Sets the handle of the paper space.
        /// Some special blocks refer to this handle.
        /// </summary>
        /// <param name="h">Handle</param>
        public void SetPaperSpaceHandle(ulong h)
        {
            _paperSpaceHandle = h;
        }
        public ulong GetPaperSpaceHandle()
        {
            return _paperSpaceHandle;
        }
        /// <summary>
        /// Sets the handle of the paper space 0;
        /// Some special blocks refer to this handle.
        /// </summary>
        /// <param name="h"></param>
        public void SetPaperSpace0Handle(ulong h)
        {
            _paperSpace0Handle = h;
        }
        public ulong GetPaperSpace0Handle()
        {
            return _paperSpace0Handle;
        }
        /// <summary>
        /// Writes a hex int variable to the DXF file.
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <param name="value">Int value</param>
        public virtual void DxfHex(uint gc, int value)
        {
            DxfString(gc, string.Format("{0,4:X}", value));
        }
        /// <summary>
        /// Writes a hex int variable to the DXF file.
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <param name="value">Long value</param>
        public virtual void DxfHex(uint gc, long value)
        {
            DxfString(gc, string.Format("{0,4:X}", value));
        }
        /// <summary>
        /// Writes an int variable to the DXF file.
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <param name="value">Int value</param>
        public virtual void DxfInt(uint gc, int value)
        {
            DxfString(gc, value.ToString());
        }

        /// <summary>
        /// Writes a real (double) value to the DXF file.
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <param name="value">Double value</param>
        public virtual void DxfReal(uint gc, double value)
        {
            string str = string.Format("{0}", value);
            // replace "," with "." if any
            str = str.Replace(",", ".");
            // Cut away those zeros at the end:
            if (str.Contains("."))
            {
                str.TrimEnd('0');
                while (str.Length - 1 - str.LastIndexOf('.') < 2)
                    str += '0';
            }
            DxfString(gc, str);
         }
        /// <summary>
        /// Writes a string value to the DXF file
        /// </summary>
        /// <param name="gc">Group code</param>
        /// <param name="value">String value</param>
        public virtual void DxfString(uint gc, string value)
        {
            _fileBuilder.Append(gc < 10 ? "  " : (gc < 100 ? " " : ""));
            _fileBuilder.Append(gc);
            _fileBuilder.AppendLine();
            _fileBuilder.Append(value);
            _fileBuilder.AppendLine();
        }
        public virtual void DxfDirectString(string value)
        {
            _fileBuilder.Append(value);
            _fileBuilder.AppendLine();
        }

        public override string ToString()
        {
            return _fileBuilder.ToString();
        }
    }
}
