#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Configuration;
#endregion

namespace Pic.Plugin
{
    #region Configuration (.config file)
    public class PluginPathData : ConfigurationSection
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public PluginPathData()
        {
        }

        [ConfigurationProperty("Path")]
        public string Path
        {
            get { return (string)this["Path"]; }
            set { this["Path"] = value; }
        }
    }
    #endregion // PluginPathData

    #region ComponentLoader
    public class ComponentLoader : IPluginHost, IDisposable
    {
        #region Delegates & Events
        // delegate
        public delegate void PluginAccessed(Guid guidPlugin);
        public delegate void FeedbackEmitted(IPlugin plugin, string message);
        // event called when accessing a component by Guid
        public static event PluginAccessed OnPluginAccessed;
        public static event FeedbackEmitted OnFeedbackEmitted;
        #endregion

        #region Constructor
        public ComponentLoader()
        {
        }
        #endregion

        #region Loading methods
        public Component LoadComponent(byte[] assemblyBytes)
        {
            if (null == assemblyBytes)
                return null;

            return ConvertAssemblyToComponent(Assembly.Load(assemblyBytes));
        }
        public Component LoadComponent(string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            // Preliminary check
            // must exist
            if (!file.Exists)
                throw new PluginException($"File {filePath} does not exist. Cannot load Component.");
            // must be a dll file
            if (!file.Extension.Equals(".dll"))
                throw new PluginException($"File {filePath} is not a dll file. Cannot load Component.");

            // create a new assembly from the plugin file we're adding...
            return ConvertAssemblyToComponent( Assembly.LoadFrom(filePath) );
        }

        private Component ConvertAssemblyToComponent(Assembly pluginAssembly)
        {
            Component component = null;
            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    if (!pluginType.IsAbstract)  //Only look at non-abstract types
                    {
                        //Gets a type object of the interface we need the plugins to match
                        Type typeInterface = pluginType.GetInterface("Pic.Plugin.IPlugin", true);

                        //Make sure the interface we want to use actually exists
                        if (typeInterface != null)
                        {
                            //Create a new available plugin since the type implements the IPlugin interface
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                            if (null != plugin)
                            {
                                component = new Component(plugin);

                                //Set the Plugin's host to this class which inherited IPluginHost
                                plugin.Host = this;

                                //Call the initialization sub of the plugin
                                plugin.Initialize();

                                //cleanup a bit
                                plugin = null;
                            }
                        }
                        typeInterface = null; //Mr. Clean			
                    }
                }
            }

            // check that a component was actually created
            if (null == component)
                throw new PluginException("Failed to load valid component from existing file");

            // insert component in cache for fast retrieval
            InsertInCache(component);

            return component;
        }

        public Component LoadComponent(Guid guid)
        {
            // verify if component 
            Component comp = GetFromCache(guid);
            if (null != comp)
                return comp;
            if (!(SearchMethod is IComponentSearchMethod))
                throw new PluginException("Component loader was not initialized with a valid plugin search method.");
            return LoadComponent(SearchMethod.GetAssemblyBytesFromGuid(guid));
        }

        public Component[] LoadComponents(string dirPath)
        {
            // set search method of this (host for components being loaded)
            SearchMethod = new ComponentSearchDirectory(dirPath);

            List<Component> components = new List<Component>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Component component = LoadComponent(fileInfo.FullName);
                if (null != component)
                    components.Add(component);
            }
            return components.ToArray();
        }
        #endregion

        #region Cache handling
        private static Component GetFromCache(Guid guid)
        {
            foreach (Component comp in _componentCache)
            {
                if (comp.Guid == guid)
                {
                    _componentCache.Remove(comp);
                    _componentCache.Insert(0, comp);
                    return comp;
                }
            }
            return null;
        }
        private static void InsertInCache(Component comp)
        {
            // verify if not already available
            if (null != GetFromCache(comp.Guid))
                return;
            // actually insert component
            _componentCache.Insert(0, comp);
            // clear exceding components
            while (_componentCache.Count > CacheSize)
            {
                _parameterStackCache.Remove(_componentCache[_componentCache.Count - 1].Guid);
                _componentCache.RemoveAt(_componentCache.Count - 1);
            }
        }

        private static ParameterStack GetStackFromCache(Guid guid)
        {
            // verify if not already available
            if (!_parameterStackCache.ContainsKey(guid))
                return null;
            return _parameterStackCache[guid];
        }
        private static void InsertParameterStackInCache(Guid guid, ParameterStack stack)
        {
            _parameterStackCache[guid] = stack;
        }
        public static void ClearCache()
        {
            _componentCache.Clear();
            _parameterStackCache.Clear();
        }
        #endregion

        #region Setting SearchMethod
        public IComponentSearchMethod SearchMethod { set; private get; } = null;
        public static uint CacheSize { get; set; } = 20;
        #endregion

        #region IPluginHost implementation
        public void Feedback(string message, IPlugin plugin)
        {
            OnFeedbackEmitted?.Invoke(plugin, message);
        }
        public IPlugin GetPluginByGuid(Guid guid)
        {
            // check if component not available in cache
            Component comp = GetFromCache(guid);
            IPlugin searchedPlugin;
            if (null != comp) // available from cache
                searchedPlugin = comp.Plugin;
            else // actually load
            {
                if (!(SearchMethod is IComponentSearchMethod))
                    throw new PluginException("Component loader was not initialized with a valid plugin search method.");
                searchedPlugin = LoadComponent(SearchMethod.GetAssemblyBytesFromGuid(guid)).Plugin;
            }
            if (null != searchedPlugin)
                OnPluginAccessed?.Invoke(searchedPlugin.Guid);
            return searchedPlugin;
        }
        public IPlugin GetPluginByGuid(string sGuid)
        {
            return GetPluginByGuid(new Guid(sGuid));
        }

        public ParameterStack GetInitializedParameterStack(IPlugin plugin)
        {
            // check if already available in cache
            ParameterStack stack = GetStackFromCache(plugin.Guid);
            if (null == stack)
            {
                if (!(SearchMethod is IComponentSearchMethod))
                    throw new PluginException("Component loader was not initialized with a valid plugin search method.");

                // get parameter stack
                if (plugin is IPluginExt1 pluginExt1)
                    stack = plugin.Parameters;
                else if (plugin is IPluginExt2 pluginExt2)
                    stack = pluginExt2.BuildParameterStack(null);
                else if (plugin is IPluginExt3 pluginExt3)
                    stack = pluginExt3.BuildParameterStack(null);
                else if (plugin is IPluginExt4 pluginExt4)
                    stack = pluginExt4.BuildParameterStack(null);
                // check parameter stack
                if (null == stack)
                    throw new PluginException("Failed to build initial parameter stack.");

                string grpId = string.Empty;

                // load parameter values from plugins
                foreach (Parameter param in stack)
                {
                    try
                    {
                        if (param is ParameterDouble pd)
                            stack.SetDoubleParameter(pd.Name, SearchMethod.GetDoubleParameterDefaultValue(grpId, plugin.Guid, pd.Name));
                        if (param is ParameterBool pb)
                            stack.SetBoolParameter(pb.Name, SearchMethod.GetBoolParameterDefaultValue(grpId, plugin.Guid, pb.Name));
                        if (param is ParameterInt pi)
                            stack.SetIntParameter(pi.Name, SearchMethod.GetIntParameterDefaultValue(grpId, plugin.Guid, pi.Name));
                        if (param is ParameterMulti pm)
                            stack.SetMultiParameter(pm.Name, SearchMethod.GetMultiParameterDefaultValue(grpId, plugin.Guid, pm.Name));
                    }
                    catch (Exception /*ex*/)
                    {
                    }
                }
                // save in cache
                InsertParameterStackInCache(plugin.Guid, stack);
            }
            return stack.Clone();
        }
        #endregion

        #region IDisposable members
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Data members
        private static readonly List<Component> _componentCache = new List<Component>();
        private static Dictionary<Guid, ParameterStack> _parameterStackCache = new Dictionary<Guid, ParameterStack>();
        #endregion
    }
    #endregion // ComponentLoader
}
