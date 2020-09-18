#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Pic.Factory2D
{
    public class FileFormat
    {
        #region Constructor
        public FileFormat(string fileFormat, string fileExtension, string filter, string fileApplication)
        {
            Filter = filter;
            FormatName = fileFormat;
            FileExtension = fileExtension;
            FileApplication = fileApplication;
        }
        #endregion

        #region Object overrides
        public override string ToString() => $"{FormatName} ({FileExtension})";
        #endregion

        #region Data members
        public string Filter { get; private set; }
        public string FormatName { get; private set; }
        public string FileExtension { get; private set; }
        public string FileApplication { get; private set; }
        #endregion
    }
}
