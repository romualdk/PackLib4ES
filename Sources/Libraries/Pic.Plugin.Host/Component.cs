#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

using Pic.Factory2D;
using Sharp3D.Math.Core;
#endregion

namespace Pic.Plugin
{
    /// <summary>
    /// Wraps a IPlugin + IPluginExt component
    /// </summary>
    public class Component : IDisposable
    {
        #region Data members
        private IPluginExt1 _ext1 = null;
        private IPluginExt2 _ext2 = null;
        private IPluginExt3 _ext3 = null;
        private IPluginExt4 _ext4 = null;
        private List<Guid> _dependancyList = new List<Guid>();
        private List<string> MessageList { get; set; } = new List<string>();
        #endregion

        #region Events
        public delegate void EmitFeedBack(object sender, List<string> messages);
        public event EmitFeedBack FeedBack;
        #endregion

        #region Static data members
        static readonly string _SC1_to_SC3_ = 
@"
/// <summary>
/// Is supporting palletization ?
/// </summary>
public bool IsSupportingPalletization { get { return false; } }
/// <summary>
/// Outer dimensions
/// Method should only be called if component supports palletization
/// </summary>
public void OuterDimensions(ParameterStack stack, out double[] dimensions)
{
    dimensions = new double[3];    
}
/// <summary>
/// Returns case type to be used for ECT computation 
/// </summary>
public int CaseType { get { return 0; } }
/// <summary>
/// Is supporting automatic folding
/// </summary>
public bool IsSupportingAutomaticFolding { get { return false; } }
/// <summary>
/// Reference point that defines anchored face
/// </summary>
public List<Vector2D> ReferencePoints(ParameterStack stack)
{
    List<Vector2D> ltPoints = new List<Vector2D>();
    return ltPoints;
}
///
/// flat palletization
///
public bool IsSupportingFlatPalletization
{   get { return false; }   }
///
/// flat dimensions
///
public void FlatDimensions(ParameterStack stack, out double[] flatDimensions)
{
    flatDimensions = new double[3];
}
/// <summary>
/// Number of parts
/// </summary>
public int NoParts
{   get { return 1; } }
/// <summary>
/// Part name
/// </summary>
public string PartName(int i)
{
    string[] partNames = {""Part0""};
    return partNames[i];
}
/// <summary>
/// Layer name
/// </summary>
public string LayerName(int i)
{
    string[] layerName = {""Layer0""};
    return layerName[i];
}";
        static readonly string _SC2_to_SC3_ =
@"
///
/// flat palletization
///
public bool IsSupportingFlatPalletization
{   get { return false; }   }
///
/// flat dimensions
///
public void FlatDimensions(ParameterStack stack, out double[] flatDimensions)
{
    flatDimensions = new double[3];
}
/// <summary>
/// Number of parts
/// </summary>
public int NoParts
{   get { return 1; } }
/// <summary>
/// Part name
/// </summary>
public string PartName(int i)
{
    string[] partNames = {""Part0""};
    return partNames[i];
}
/// <summary>
/// Layer name
/// </summary>
public string LayerName(int i)
{
    string[] layerName = {""Layer0""};
    return layerName[i];
}";
        #endregion

        #region Private constructor
        internal Component(IPlugin instance)
        {
            Plugin = instance;

            // depending on plugin interface version, an other interface might be available
            if (Plugin.GetType().GetInterface("Pic.Plugin.IPluginExt1") != null)
                _ext1 = Plugin as IPluginExt1;
            else if (Plugin.GetType().GetInterface("Pic.Plugin.IPluginExt2") != null)
                _ext2 = Plugin as IPluginExt2;
            else if (Plugin.GetType().GetInterface("Pic.Plugin.IPluginExt3") != null)
                _ext3 = Plugin as IPluginExt3;
            else if (Plugin.GetType().GetInterface("Pic.Plugin.IPluginExt4") != null)
                _ext4 = Plugin as IPluginExt4;
            else
                throw new PluginException(string.Format("Failed to load plugin {0}\nOne of the following interface is expected: \n{1}"
                    , Plugin.Name
                    , "Pic.Plugin.IPluginExt1\nPic.Plugin.IPluginExt2\nPic.Plugin.IPluginExt3"));
        }
        #endregion

        #region Public properties
        public string Name
        { get { return Plugin.Name; } }
        public string Description
        { get { return Plugin.Description; } }
        public string Author
        { get { return Plugin.Author; } }
        public string Version
        { get { return Plugin.Version; } }
        public Guid Guid
        { get { return Plugin.Guid; } }
        public string SourceCode
        {
            get
            {
                if (null != _ext1)
                    return UpgradeSC1(_ext1.SourceCode);
                else if (null != _ext2)
                    return UpgradeSC2(_ext2.SourceCode);
                else if (null != _ext3)
                    return _ext3.SourceCode;
                else
                    return string.Empty;
            }
        }
        public bool HasEmbeddedThumbnail
        {
            get
            {
                if (null != _ext1)
                    return _ext1.HasEmbeddedThumbnail;
                else if (null != _ext2)
                    return _ext2.HasEmbeddedThumbnail;
                else if (null != _ext3)
                    return _ext3.HasEmbeddedThumbnail;
                else
                    return false;
            }
        }
        public Bitmap Thumbnail
        {
            get
            {
                if (null != _ext1)
                    return _ext1.Thumbnail;
                else if (null != _ext2)
                    return _ext2.Thumbnail;
                else if (null != _ext3)
                    return _ext3.Thumbnail;
                else
                    return null;
            }
        }
        public bool IsSupportingAutomaticFolding
        {
            get
            {
                if (null != _ext1)
                    return false;
                else if (null != _ext2)
                    return _ext2.IsSupportingAutomaticFolding;
                else if (null != _ext3)
                    return _ext3.IsSupportingAutomaticFolding;
                else
                    return false;
            }
        }

        public Vector2D ReferencePoint(ParameterStack stackIn)
        {
            var stack = ConvertIn(stackIn);
            List<Vector2D> l = null;
            if (null != _ext2)
                l = _ext2.ReferencePoints(stack);
            else if (null != _ext3)
                l = _ext3.ReferencePoints(stack);
            if (null == l) return Vector2D.Zero;
            else if (l.Count > 0) return l[0];
            else return Vector2D.Zero;
        }

        public Vector2D ImpositionOffset(ParameterStack stackIn)
        {
            var stack = ConvertIn(stackIn);
            if (null != _ext1)
                return new Vector2D(_ext1.ImpositionOffsetX(stack), _ext1.ImpositionOffsetY(stack));
            else if (null != _ext2)
                return new Vector2D(_ext2.ImpositionOffsetX(stack), _ext2.ImpositionOffsetY(stack));
            else if (null != _ext3)
                return new Vector2D(_ext3.ImpositionOffsetX(stack), _ext3.ImpositionOffsetY(stack));
            else if (null != _ext3)
                return new Vector2D(_ext4.ImpositionOffsetX(stack), _ext4.ImpositionOffsetY(stack));
            else
                return Vector2D.Zero;
        }
 
        /// <summary>
        /// Try to get outer dimensions to use for palletisation
        /// </summary>
        /// <param name="stack">Parameter stack</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>true if a valid set of dimensions could be retrieved</returns>
        public bool GetDimensions(ParameterStack stack, ref double length, ref double width, ref double height)
        {
            if (null != _ext1)
            {
                // search three parameters A/L, B, H 
                bool foundL = false, foundW = false, foundH = false;
                foreach (Parameter param in stack)
                {
                    if (!(param is ParameterDouble paramDouble)) continue;

                    if (0 == string.Compare(paramDouble.Name.ToLower(), "a") || 0 == string.Compare(paramDouble.Name.ToLower(), "l"))
                    {
                        foundL = true;
                        length = paramDouble.Value;
                    }
                    else if (0 == string.Compare(paramDouble.Name.ToLower(), "b"))
                    {
                        foundW = true;
                        width = paramDouble.Value;
                    }
                    else if (0 == string.Compare(paramDouble.Name.ToLower(), "h"))
                    {
                        foundH = true;
                        height = paramDouble.Value;
                    }
                }
                // to support palletization, all three parameters must be found
                return foundL && foundW && foundH;
            }
            else if (null != _ext2)
            {
                if (_ext2.IsSupportingPalletization)
                {
                    _ext2.OuterDimensions(stack, out double[] dim);
                    length = dim[0]; width = dim[1]; height = dim[2];
                    return true;
                }
            }
            else if (null != _ext3)
            {
                if (_ext3.IsSupportingPalletization)
                {
                    _ext3.OuterDimensions(stack, out double[] dim);
                    length = dim[0]; width = dim[1]; height = dim[2];
                    return true;
                }
            }
            return false; 
        }

        public bool GetFlatDimensions(ParameterStack stack, ref double length, ref double width, ref double height)
        {
            if (null != _ext3 && _ext3.IsSupportingFlatPalletization)
            {
                _ext3.FlatDimensions(ConvertIn(stack), out double[] dim);
                length = ConvertOut(dim[0]); width = ConvertOut(dim[1]); height = ConvertOut(dim[2]);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Internal plugin accessor
        /// </summary>
        internal IPlugin Plugin { get; private set; } = null;
        /// <summary>
        /// Build parameter stack
		/// IPluginExt1 simply returns a static list of parameters or the same current list
		/// IpluginExt2 builds a new list depending on the current list state
        /// </summary>
        public ParameterStack BuildParameterStack(ParameterStack stackIn1)
        {
            ParameterStack stackIn = (null != stackIn1) ? ConvertIn(stackIn1) : null;
            ParameterStack stackOut;
            if (null != _ext1)
            {
                if (null != stackIn && stackIn.Count > 0)
                    stackOut = stackIn;
                else
                    stackOut = Plugin.Parameters;
            }
            else if (null != _ext2)
                stackOut = _ext2.BuildParameterStack(stackIn);
            else if (null != _ext3)
                stackOut = _ext3.BuildParameterStack(stackIn);
            else if (null != _ext4)
                stackOut = _ext4.BuildParameterStack(stackIn);
            else
                stackOut = new ParameterStack();

            // adding thickness parameters 
            // these parameters are always available but are not shown in the list of parameters in the plugin viewer
            // ep1 and th1 are reserved name
            double defaultThickness = Host.Properties.Settings.Default.Thickness;
            if (!stackOut.HasParameter("th1"))
                stackOut.AddDoubleParameter("th1", "Thickness"
                    , null != stackIn && stackIn.HasParameter("th1") ? stackIn.GetDoubleParameterValue("th1") : defaultThickness);
            if (!stackOut.HasParameter("ep1"))
                stackOut.AddDoubleParameter("ep1", "Thickness"
                    , null != stackIn && stackIn.HasParameter("ep1") ? stackIn.GetDoubleParameterValue("ep1") : defaultThickness);

            return ConvertOut(stackOut);
        }
        public ParameterStack GetParameters(IComponentSearchMethod compSearchMethod)
        {
            ParameterStack parameters = Plugin.Parameters;
            string grpId = string.Empty;
            if (null != compSearchMethod)
            {
                foreach (Parameter p in parameters)
                {
                    try
                    {
                        if (p is ParameterAngle pa)
                            parameters.SetAngleParameter(pa.Name, compSearchMethod.GetAngleParameterDefaultValue(grpId, Guid, pa.Name));
                        if (p is ParameterDouble pd)
                            parameters.SetDoubleParameter(pd.Name, compSearchMethod.GetDoubleParameterDefaultValue(grpId, Guid, pd.Name));
                        if (p is ParameterBool pb)
                            parameters.SetBoolParameter(pb.Name, compSearchMethod.GetBoolParameterDefaultValue(grpId, Guid, pb.Name));
                        if (p is ParameterInt pi)
                            parameters.SetIntegerParameter(pi.Name, compSearchMethod.GetIntParameterDefaultValue(grpId, Guid, pi.Name));
                        if (p is ParameterMulti pm)
                            parameters.SetMultiParameter(pm.Name, 0);
                    }
                    catch (Exception /*ex*/)
                    {
                    }
                }
            }
            return parameters;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates factory entities (mostly segment, arcs, cotations...) in <see cref="PicFactory"/> class instance passed as first argument
        /// </summary>
        /// <param name="factory">Instance of class <see cref="PicFactory"/> to be populated with entities</param>
        /// <param name="stack">Array of <see cref="Paramerter"/> to apply while generating entities</param>
        public void CreateFactoryEntities(PicFactory factory, ParameterStack stackIn)
        {
            ParameterStack stack = ConvertIn(stackIn);
            var tempList = new List<string>(MessageList);
            MessageList.Clear();   
            var handler = new ComponentLoader.FeedbackEmitted(OnFeedbackAdded);
            ComponentLoader.OnFeedbackEmitted += handler;
            Plugin.CreateFactoryEntities(factory, stack, Transform2D.Identity);

            // --- US units ---
            stackIn = ConvertOut(stack);
            ConvertOutRef(factory);
            // ---

            ComponentLoader.OnFeedbackEmitted -= handler;

            if (!MessageList.SequenceEqual(tempList))
                FeedBack?.Invoke(this, MessageList);
        }

        public List<Guid> GetDependancies(ParameterStack stack)
        {
            // clear list of dependancies
            _dependancyList.Clear();
            // instanciate handler of ComponentLoader.OnPluginAccessed
            ComponentLoader.PluginAccessed handler = new ComponentLoader.PluginAccessed(OnGuidLoaded);
            ComponentLoader.OnPluginAccessed += handler;
            try
            {
                // instantiate factory
                PicFactory factory = new PicFactory();
                Plugin.CreateFactoryEntities(factory, ConvertIn(stack), Transform2D.Identity);
            }
            catch (Exception)
            {
            }
            // on plugin accessed
            ComponentLoader.OnPluginAccessed -= handler; 
            // dependancy list
            return _dependancyList;
        }
        public void OnGuidLoaded(Guid pluginGuid)
        {
            if (!_dependancyList.Exists(g => g == pluginGuid))
                _dependancyList.Add(pluginGuid);
        }
        public void OnFeedbackAdded(IPlugin plugin, string message)
        {
            MessageList.Add($"({plugin.Name}) | {message}");
        }
        #endregion

        #region Private helper methods
        private string UpgradeSC1(string sourceCode)
        {
            sourceCode = MoveSemiColumnsToPrevLine(sourceCode.Trim());

            List<string> sList = new List<string>();
            using (StringReader reader = new StringReader(sourceCode))
            {
                string line = null;
                while (null != (line = reader.ReadLine()))
                {
                    if (line.Contains("public ParameterStack Parameters"))
                    {
                        while (!(line = reader.ReadLine()).Contains("new ParameterStack(")) ;
                        sList.Add(@"public ParameterStack BuildParameterStack(ParameterStack stackIn)" );
                        sList.Add(@"{");
                        sList.Add(@"    ParameterStackUpdater paramUpdater = new ParameterStackUpdater(stackIn);");

                        // change AddDoubleParameter / AddIntParameter / AddBoolParameter / AddMultiParameter
                        while (!(line = reader.ReadLine()).Contains("}"))
                        {
                            if (line.Contains("}")) { /* do nothing */ }
                            else if (line.Contains("AddDoubleParameter("))
                            {
                                string sEndLine = line.Substring(line.IndexOf("(") + 1);
                                sList.Add(@"    paramUpdater.CreateDoubleParameter(" + sEndLine);
                            }
                            else if (line.Contains("AddIntParameter"))
                            {
                                string sEndLine = line.Substring(line.IndexOf("(") + 1);
                                sList.Add(@"    paramUpdater.CreateIntParameter(" + sEndLine);
                            }
                            else if (line.Contains("AddBoolParameter"))
                            {
                                string sEndLine = line.Substring(line.IndexOf("(") + 1);
                                sList.Add(@"    paramUpdater.CreateBoolParameter(" + sEndLine);
                            }
                            else if (line.Contains("AddMultiParameter"))
                            {
                                string sEndLine = line.Substring(line.IndexOf("(") + 1);
                                sList.Add(@"    paramUpdater.CreateMultiParameter(" + sEndLine);
                            }
                        }
                        sList.Add("    return paramUpdater.UpdatedStack;");
                    }
                    else
                        sList.Add(line);
                }
            }
            // rebuild string
            StringWriter sw = new StringWriter();
            foreach (string s in sList)
                sw.WriteLine(s);
            sw.Write(_SC1_to_SC3_); // additional features
            return sw.ToString();
        }
        private string UpgradeSC2(string sourceCode)
        {
            sourceCode = MoveSemiColumnsToPrevLine(sourceCode.Trim());
            StringWriter sw = new StringWriter();
            sw.Write(sourceCode);
            sw.Write(_SC2_to_SC3_); // additional features
            return sw.ToString();
        }
        private string MoveSemiColumnsToPrevLine(string sourceCode)
        {
            List<string> sList = new List<string>();
            using (StringReader reader = new StringReader(sourceCode))
            {
                string line = null;
                while (null != (line = reader.ReadLine()))
                {
                    string trimmedLine = line.Trim();
                    if (string.Equals(trimmedLine, ";"))
                        sList[sList.Count - 1] = sList[sList.Count - 1] + ";";
                    else
                        sList.Add(line);
                }
            }
            StringWriter writer = new StringWriter();
            foreach (string s in sList)
                writer.WriteLine(s);
            return writer.ToString();            
        }
        #endregion

        #region Unit conversion helpers
        public UnitSystem.EUnit CompUnitSystem { get; set; } = UnitSystem.EUnit.METRIC;
        private ParameterStack ConvertDistances(ParameterStack stack, double multiplicator)
        {
            switch (CompUnitSystem)
            {
                case UnitSystem.EUnit.US:
                    {
                        var paramStack = new ParameterStack();
                        foreach (var param in stack.ParameterList)
                        {
                            if (param is ParameterDouble pd)
                            {
                                paramStack.AddDoubleParameter(
                                    pd.Name, pd.Description,
                                    pd.Value * multiplicator,
                                    pd.HasValueMin, pd.ValueMin * multiplicator,
                                    pd.HasValueMax, pd.ValueMax * multiplicator);
                            }
                            else
                                paramStack.AddParameter(param);
                        }
                        return paramStack;
                    }
                default: return stack;
            }
        }
        public ParameterStack ConvertIn(ParameterStack stack) => ConvertDistances(stack, 25.4);
        public ParameterStack ConvertOut(ParameterStack stack) => ConvertDistances(stack, 1.0/25.4);
        public PicFactory ConvertOut(PicFactory factory)
        {
            switch (CompUnitSystem)
            {
                case UnitSystem.EUnit.US:
                {
                    var factoryConv = new PicVisitorConvertDistances(1.0 / 25.4);
                    factory.ProcessVisitor(factoryConv);
                    return factoryConv.FactoryOut;
                };
                default: return factory;
            }
        }
        public void ConvertOutRef(PicFactory factory)
        {
            switch (CompUnitSystem)
            {
                case UnitSystem.EUnit.US:
                    {
                        double dMult = 1.0 / 25.4;
                        var visitor = new PicVisitorTransform( new Transform2D( new Matrix3D(dMult, 0.0, 0.0, 0.0, dMult, 0.0, 0.0, 0.0, 1.0) ) );
                        factory.ProcessVisitor(visitor);
                        break;
                    };
                default: break;
            }
        }
        public double ConvertIn(double dval)
        {
            switch (CompUnitSystem)
            {
                case UnitSystem.EUnit.US: return dval*25.4;
                default: return dval;
            }
        }
        public double ConvertOut(double dval)
        {
            switch (CompUnitSystem)
            {
                case UnitSystem.EUnit.US: return dval/25.4;
                default: return dval;
            }
        }
        #endregion

        #region IDisposable members
        /// <summary>
        /// Implements IDisposable Dispose methos
        /// </summary>
        public void Dispose()
        {
            Plugin.Dispose();
            Plugin = null;

            _ext1.Dispose();
            _ext1 = null;
        }
        #endregion
    }
}
