#region Using directives
using System;
using System.IO;
#endregion

namespace Pic.Plugin
{
    #region ComponentSearchException
    public class ComponentSearchException : Exception
    {
        #region Constructor
        // constructor
        public ComponentSearchException(Guid guid)
            : base(string.Format("Component Guid='{0}' could not be found!", guid.ToString()))
        {
            _guid = guid;
        }
        #endregion

        #region Data members
        private Guid _guid;
        #endregion
    }
    #endregion

    #region IComponentSearchMethod (interface)
    public interface IComponentSearchMethod
    {
        /// <summary>
        /// get parameter default value
        /// </summary>
        double GetDoubleParameterDefaultValue(string grpId, Guid g, string name);
        int GetIntParameterDefaultValue(string grpId, Guid g, string name);
        bool GetBoolParameterDefaultValue(string grpId, Guid g, string name);
        int GetMultiParameterDefaultValue(string grpId, Guid g, string name);
        /// <summary>
        /// get file stream
        /// </summary>
        Stream GetFile(Guid g, string fileExt);
        /// <summary>
        /// get plugin byte array
        /// </summary>
        byte[] GetAssemblyBytesFromGuid(Guid g);
    }
    #endregion

    #region ComponentSearchDirectory (implements IComponentSearchMethod)
    public class ComponentSearchDirectory : IComponentSearchMethod
    {
        #region Constructors
        public ComponentSearchDirectory(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }
        #endregion

        #region IComponentSearchMethod implementation
        public byte[] GetAssemblyBytesFromGuid(Guid g)
        {
            ComponentLoader loader = new ComponentLoader();

            DirectoryInfo dirInfo = new DirectoryInfo(DirectoryPath);
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Component component = loader.LoadComponent(fileInfo.FullName);
                if (g == component.Guid)
                    return File.ReadAllBytes(fileInfo.FullName);
            }
            throw new PluginException($"Failed to load Component with Guid = {g.ToString()} in directory {DirectoryPath}");
        }
        public double GetDoubleParameterDefaultValue(string grpId, Guid g, string name)
        {
            Component comp = GetComponentFromGuid(g);
            return comp.BuildParameterStack(null).GetDoubleParameterValue(name);
        }
        public int GetIntParameterDefaultValue(string grpId, Guid g, string name)
        {
            Component comp = GetComponentFromGuid(g);
            return comp.BuildParameterStack(null).GetIntParameterValue(name);
        }
        public bool GetBoolParameterDefaultValue(string grpId, Guid g, string name)
        {
            Component comp = GetComponentFromGuid(g);
            return comp.BuildParameterStack(null).GetBoolParameterValue(name);
        }
        public int GetMultiParameterDefaultValue(string grpId, Guid g, string name)
        {
            Component comp = GetComponentFromGuid(g);
            return comp.BuildParameterStack(null).GetMultiParameterValue(name);
        }
        public Stream GetFile(Guid g, string fileExt)
        {
            return new FileStream(FilePath(g.ToString(), fileExt), FileMode.Open);
        }
        #endregion

        #region Helpers
        public Component GetComponentFromGuid(Guid g)
        {
            ComponentLoader loader = new ComponentLoader();

            DirectoryInfo dirInfo = new DirectoryInfo(DirectoryPath);
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Component component = loader.LoadComponent(fileInfo.FullName);
                if (g == component.Guid)
                    return component;
            }
            throw new PluginException(string.Format("Failed to load Component with Guid = {0} in directory {1}", g.ToString(), DirectoryPath));
        }
        private string FilePath(string sGuid, string fileExt) => Path.ChangeExtension(Path.Combine(DirectoryPath, sGuid.Replace("-", "_")), fileExt);
        #endregion

        #region Data members
        private string DirectoryPath { get; set; }
        #endregion
    }
    #endregion
}
