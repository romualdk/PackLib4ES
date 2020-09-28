#region Using directives
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Text;

using log4net;

using Pic.Plugin;
#endregion

namespace PicParam
{
    public class ComponentSearchMethodDB : IComponentSearchMethod
    {
        #region Constructors
        public ComponentSearchMethodDB()
        {
        }
        #endregion

        #region IComponentSearchMethod implementation
        public string GetFilePathFromGuid(Guid guid)
        {
            // get db constext
            Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
            // retrieve component
            Pic.DAL.SQLite.Component comp = Pic.DAL.SQLite.Component.GetByGuid(db, guid);
            if (null == comp)
                throw new PluginException(string.Format("Failed to load Component with Guid = {0}", guid.ToString()));
            // return path
            return comp.Document.File.Path(db);
        }
        public double GetDoubleParameterDefaultValue(Guid guid, string name)
        {
            Pic.DAL.SQLite.PPDataContext db = new Pic.DAL.SQLite.PPDataContext();
            Pic.DAL.SQLite.Component comp = Pic.DAL.SQLite.Component.GetByGuid(db, guid);
            if (null == comp)
                throw new PluginException(string.Format("Failed to load Component with Guid = {0}", guid.ToString()));
            try
            {   return comp.GetParamDefaultValueDouble(db, name); }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                // rethrow exception
                throw ex;
            }
        }
        public int GetIntParameterDefaultValue(Guid guid, string name) => 0;
        public bool GetBoolParameterDefaultValue(Guid guid, string name) => true;
        public int GetMultiParameterDefaultValue(Guid guid, string name) => 0;
        public double GetAngleParameterDefaultValue(string grpId, Guid guid, string name) => GetDoubleParameterDefaultValue(guid, name);
        public double GetDoubleParameterDefaultValue(string grpId, Guid guid, string name) => GetDoubleParameterDefaultValue(guid, name);
        public int GetIntParameterDefaultValue(string grpId, Guid guid, string name) => GetIntParameterDefaultValue(guid, name);
        public bool GetBoolParameterDefaultValue(string grpId, Guid guid, string name) => GetBoolParameterDefaultValue(guid, name);
        public int GetMultiParameterDefaultValue(string grpId, Guid guid, string name) => GetMultiParameterDefaultValue(guid, name);

        public string RepositoryPath => Pic.DAL.ApplicationConfiguration.CustomSection.RepositoryPath;

        public Stream GetFile(Guid g, string fileExt) => new FileStream(FilePath(g.ToString(), fileExt), FileMode.Open);
        private string FilePath(string sGuid, string fileExt) => Path.ChangeExtension(Path.Combine(RepositoryPath, sGuid.Replace("-", "_")), fileExt);

        public byte[] GetAssemblyBytesFromGuid(Guid g) => File.ReadAllBytes(GetFilePathFromGuid(g));
        #endregion

        #region DATA MEMBERS
        protected static readonly ILog _log = LogManager.GetLogger(typeof(ComponentSearchMethodDB));
        #endregion
    }
}
