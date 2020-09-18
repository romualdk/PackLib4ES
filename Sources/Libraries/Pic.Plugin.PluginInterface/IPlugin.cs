#region Using directives
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

using Sharp3D.Math.Core;

using Pic.Factory2D;
using System.Linq;
#endregion

namespace Pic.Plugin
{
    #region Exceptions
    #region PluginException
    public class PluginException : Exception
    {
        #region Constructors
        public PluginException()
            :base()
        { 
        }
        public PluginException(string message)
            : base(message)
        {
        }
        public PluginException(string message, Exception innerException)
            :base(message, innerException)
        { 
        }
        #endregion
    }
    #endregion

    #region ParameterException
    public class ParameterNotFound : PluginException
    {
        public ParameterNotFound(string parameterName)
            : base(string.Format("{0} does not exist.", parameterName))
        {
        }
    }
    #endregion

    #region ParameterNoValidValue
    public class ParameterNoValidValue : PluginException
    {
        public ParameterNoValidValue(string parameterName, double minValue, double maxValue)
            : base(string.Format("Parameter {0} has invalid min/max values ({1}/{2})", parameterName, minValue, maxValue))
        {
        }
    }
    #endregion
    #region ParameterInvalidType
    public class ParameterInvalidType : PluginException
    {
        public ParameterInvalidType(string parameterName, string type)
            : base(string.Format("Parameter {0} exists but has not {1} type", parameterName, type))
        {            
        }
    }
    #endregion
    #endregion

    #region Parameter classes
    [Serializable]
    [XmlInclude(typeof(Parameter))]
    public abstract class Parameter
	{

        #region Private fields
        private Parameter _parent;
        private static int _offset = 10;
		#endregion

        #region Constructors
        public Parameter()
        { 
        }
        public Parameter(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public Parameter Parent
        {
            set { _parent = value; }
        }
        public int IndentValue
        {
            get
            {
                if (null == _parent) return 0;
                else return _parent.IndentValue + _offset;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// parameter name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// parameter description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// check if parameter value is valid
        /// </summary>
        abstract public bool IsValid { get; }
        /// <summary>
        /// a parameter is a majoration if:
        /// - its name starts with "m", 
        /// - next characters can be parsed as an integer
        /// </summary>
        public bool IsMajoration => Name.StartsWith("m") && Int32.TryParse(Name.Substring(1), out int iIndex);
        public bool IsThickness => new string[] { "th1", "ep1" }.Contains(Name.ToLower().Trim());
        /// <summary>
        /// copy property generate a clone
        /// </summary>
        public abstract Parameter Clone();
		#endregion
	}

    [Serializable]
    [XmlInclude(typeof(ParameterDouble))]
    public class ParameterDouble : Parameter
	{

        #region Private fields
        private double _valueMin;
        private double _valueMax;
        private double _value;
		#endregion

		#region Constructor
		public ParameterDouble()
		{
			ValueDefault = 0;
			_value = 0;
			_valueMin = double.MinValue;
			_valueMax = double.MaxValue;
		}
        public ParameterDouble(string name, string description, bool hasValueMin, double valueMin, bool hasValueMax, double valueMax, double valueDefault)
            : base(name, description)
        {
            HasValueMin = hasValueMin;
            _valueMin = valueMin;
            HasValueMax = hasValueMax;
            _valueMax = valueMax;
            ValueDefault = valueDefault;
            _value = valueDefault;
        }

        #endregion

        #region Public Properties
        public double ValueDefault { get; set; }

        public double ValueMin
        {
            get { return _valueMin; }
            set { HasValueMin = true; _valueMin = value; }
        }

        public double ValueMax
        {
            get { return _valueMax; }
            set { HasValueMax = true; _valueMax = value; }
        }

        public double Value
        {
            get
            {
                if (HasValueMin && _value < _valueMin) return _valueMin;
                if (HasValueMax && _value > _valueMax) return _valueMax;
                return _value; 
            }
            set
            {
                _value = value;
                if (HasValueMin && _value < _valueMin) _value = _valueMin;
                if (HasValueMax && _value > _valueMax) _value = _valueMax;
            }
		}

        public bool HasValueMin { get; set; }

        public bool HasValueMax { get; set; }

        public override bool IsValid
        {
            get { return (!HasValueMin || _valueMin <= _value) && (!HasValueMax && _value <= _valueMax); }
        }
        #endregion

        #region Override parameter
        public override Parameter Clone()
        {
            ParameterDouble param =  new ParameterDouble(Name, Description, HasValueMin, _valueMin, HasValueMax, _valueMax, ValueDefault);
            param._value = _value;
            return param;
        }
        #endregion
	}

    [Serializable]
    [XmlInclude(typeof(ParameterInt))]
    public class ParameterInt : Parameter
    {

        #region Private fields
        private int _valueMin;
        private int _valueMax;
        #endregion

        #region Constructor
        public ParameterInt()
        {
            ValueDefault = 0;
            HasValueMin = false;
            _valueMin = Int32.MinValue;
            HasValueMax = false;
            _valueMax = Int32.MaxValue;
            Value = 0;
        }
        public ParameterInt(string name, string description, bool hasValueMin, int valueMin, bool hasValueMax, int valueMax, int valueDefault)
            : base(name, description)
        {
            HasValueMin = hasValueMin;
            _valueMin = valueMin;
            HasValueMax = hasValueMax;
            _valueMax = valueMax;
            ValueDefault = valueDefault;
            Value = valueDefault;
        }
        #endregion

        #region Public Properties
        public int ValueDefault { get; set; }
        public bool HasValueMin { get; private set; }
        public int ValueMin
        {
            get { return _valueMin; }
            set { HasValueMin = true; _valueMin = value; }
        }
        public bool HasValueMax { get; private set; }
        public int ValueMax
        {
            get { return _valueMax; }
            set { HasValueMax = true; _valueMax = value; }
        }
        public int Value { get; set; }
        public override bool IsValid
        {
            get { return (!HasValueMin || _valueMin <= Value) && (!HasValueMax && Value <= _valueMax); }
        }
		#endregion

        #region Override parameter
        public override Parameter Clone()
        {
            return new ParameterInt(Name, Description, HasValueMin, _valueMin, HasValueMax, _valueMax, Value);
        }
        #endregion
    }

    [Serializable]
    [XmlInclude(typeof(ParameterBool))]
    public class ParameterBool : Parameter
	{
		#region Private fields
		bool _value;
		bool _valueDefault;
		#endregion

		#region Constructors
		public ParameterBool()
		{
			_value = false;
			_valueDefault = false;
		}
		public ParameterBool(string name, string description, bool bValue)
            : base(name, description)
		{
			_value = bValue;
            _valueDefault = bValue;
		}
		#endregion

		#region Public properties
		public bool Value
		{
			get { return _value; }
			set { _value = value; }
		}
		public bool ValueDefault
		{
			get { return _valueDefault; }
			set { _valueDefault = value;}
		}
        public override bool IsValid
        {
            get { return true; }
        }
		#endregion

        #region Override parameter
        public override Parameter Clone()
        {
            return new ParameterBool(Name, Description, _value);
        }
        #endregion
	}
    [Serializable]
    [XmlInclude(typeof(ParameterString))]
    public class ParameterString : Parameter
    {
        #region Private fields
        private string _defaultValue;
        private string _value;
        private bool _allowEmpty;
        #endregion

        #region Constructors
        public ParameterString()
        {
            _allowEmpty = true;
        }
        public ParameterString(string name, string description, string s)
            :base(name, description)
        {
            _value = s;
            _defaultValue = s;
        }
        #endregion

        #region Public properties
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public override bool IsValid
        {
            get { return _allowEmpty || _value.Length > 0; }
        }
        #endregion

        #region Override parameter
        public override Parameter Clone()
        {
            return new ParameterString(Name, Description, _value);
        }
        #endregion
    }
    [Serializable]
    [XmlInclude(typeof(ParameterMulti))]
    public class ParameterMulti : Parameter
    {
        #region Private fields
        List<string> _valueList = new List<string>();
        List<string> _displayList = new List<string>();
        int _value;
        #endregion

        #region Constructors
        public ParameterMulti(string name, string description, string[] displayList, string[] valueList, int defaultValue)
            :base(name,description)
        {
            _displayList.AddRange(displayList);
            _valueList.AddRange(valueList);
            _value = defaultValue;
        }
        #endregion

        #region Public properties
        public int Value
        {
            get { return _value; }
            set
            {
                if (value >= 0 && value < _valueList.Count)
                    _value = value;
                else
                    throw new PluginException(string.Format("Value {0} is out of range (0, {1})", value, _valueList.Count - 1 ));
            }
        }
        public override bool IsValid
        {
            get { return null != _valueList && 0 <= _value && _value < _valueList.Count; }
        }
        public string[] ValueList
        {
            get { return _valueList.ToArray(); }
        }
        public string[] DisplayList
        {
            get { return _displayList.ToArray(); }
        }
        #endregion
        #region Override parameter
        public override Parameter Clone()
        {
            return new ParameterMulti(Name, Description, _displayList.ToArray(), _valueList.ToArray(), _value);
        }
        #endregion
    }
    #endregion

    #region ParameterStack class
    [Serializable]
    [XmlInclude(typeof(ParameterDouble))]
    [XmlInclude(typeof(ParameterInt))]
    [XmlInclude(typeof(ParameterBool))]
    [XmlInclude(typeof(ParameterString))]
    [XmlInclude(typeof(ParameterStack))]
    public class ParameterStack: IEnumerable
    {

        #region Private fields
        private int _version;
		#endregion

		#region Constructor
		public ParameterStack()
        {
            ParameterList = new List<Parameter>();
            _version = 1;
		}
		#endregion

        #region Clear
        public void Clear()
        {
            ParameterList.Clear();
        }
        #endregion

        #region Public properties
        public List<Parameter> ParameterList { get; set; }
        public int Count
        {
            get
            {
                if (null != ParameterList)
                    return ParameterList.Count;
                else
                    return 0;
            }
        }
        public ParameterStack Clone()
        {
            ParameterStack stackCopy = new ParameterStack();
            foreach (Parameter p in ParameterList)
                stackCopy.AddParameter(p.Clone());
            return stackCopy;
        }
        #endregion

        #region Adding parameter
        public void AddParameter(Parameter p)
        {
            ParameterList.Add(p);
        }
        public void AddDoubleParameter(string name, string description, double valueDefaut, double valueMin, double valueMax)
        {
            AddDoubleParameter(name, description, valueDefaut, true, valueMin, true, valueMax);
        }

        public void AddDoubleParameter(string name, string description, double valueDefault, double valueMin)
        {
            AddDoubleParameter(name, description, valueDefault, true, valueMin, false, Double.MaxValue);
        }

        public void AddDoubleParameter(string name, string description, double valueDefault)
        {
            AddDoubleParameter(name, description, valueDefault, false, Double.MinValue, false, Double.MaxValue);
        }

        public void AddDoubleParameter(string name, string description, double valueDefault, bool hasMinValue, double valueMin, bool hasMaxValue, double valueMax)
        {
            ParameterDouble param = new ParameterDouble
            {
                Name = name,
                Description = description,
                ValueDefault = valueDefault,
                Value = valueDefault
            };
            if (hasMinValue)
                param.ValueMin      = valueMin;
            if (hasMaxValue)
                param.ValueMax      = valueMax;
            ParameterList.Add(param);
		}

        public void AddIntParameter(string name, string description, int valueDefault, bool hasMinValue, int valueMin, bool hasMaxValue, int valueMax)
        {
            ParameterInt param = new ParameterInt
            {
                Name = name,
                Description = description,
                ValueDefault = valueDefault,
                Value = valueDefault
            };
            if (hasMinValue)
                param.ValueMin = valueMin;
            if (hasMaxValue)
                param.ValueMax = valueMax;
            ParameterList.Add(param);
        }

		public void AddBoolParameter(string name, string description, bool valueDefault)
		{
            ParameterList.Add(new ParameterBool(name, description, valueDefault));
		}

        public void AddMultiParameter(string name, string description, string[] displayList, string[] valueList, int value)
        {
            ParameterList.Add(new ParameterMulti(name, description, displayList, valueList, value));
        }
        public void AddMultiParameterFront(string name, string description, string[] displayList, string[] valueList, int value)
        {
            ParameterList.Insert(0, new ParameterMulti(name, description, displayList, valueList, value));
        }
        #endregion

        #region Setting parameters
        public bool HasParameter(string name)
        {
            Parameter param = ParameterList.Find(x => x.Name == name);
            return (null != param); 
        }
        public Parameter GetParameterByName(string name)
        {
            Parameter param = ParameterList.Find(x => x.Name == name);
            if (null == param)
                throw new ParameterNotFound(name);
            return param;
        }
		public void SetDoubleParameter(string name, double value)
        {
            if (!(GetParameterByName(name) is ParameterDouble param))
                throw new ParameterInvalidType(name, "ParameterDouble");
            param.Value = value;
		}
        public void SetIntParameter(string name, int value)
        {
            if (!(GetParameterByName(name) is ParameterInt param))
                throw new ParameterInvalidType(name, "ParameterInt");
            param.Value = value;
        }
		public void SetBoolParameter(string name, bool value)
		{
            if (!(GetParameterByName(name) is ParameterBool param))
                throw new ParameterInvalidType(name, "ParameterBool");
            param.Value = value;
		}
        public void SetIntegerParameter(string name, int value)
        {
            if (!(GetParameterByName(name) is ParameterInt param))
                throw new ParameterInvalidType(name, "ParameterInt");
            param.Value = value;
        }
        public void SetMultiParameter(string name, string value)
        {
            if (!(GetParameterByName(name) is ParameterMulti param))
                throw new ParameterInvalidType(name, "ParameterMulti");
            for (int index = 0; index < param.ValueList.Length; ++index)
            {
                if (0 == string.Compare(param.ValueList[index], value))
                {
                    param.Value = index;
                    return;
                }
            }
            string sMessage = string.Format("Could not find '{0}' among the available values : ", value);
            foreach (string s in param.ValueList)
                sMessage += s + " ";
            throw new PluginException(sMessage);
        }
        public void SetMultiParameter(string name, int index)
        {
            if (!(GetParameterByName(name) is ParameterMulti param)) throw new ParameterInvalidType(name, "ParameterMulti");
            param.Value = index;
        }
        /// <summary>
        /// checks if parameter stack contains some majorations
        /// i.e. parameter with name equal to m[1..16]
        /// </summary>
        public bool HasMajorations
        {
            get
            {
                foreach (Parameter p in ParameterList)
                    if (p.IsMajoration) return true;
                return false;
            }
        }

        public int MajorationCount
        {
            get
            {
                int majoCount = 0;
                foreach (Parameter p in ParameterList)
                    majoCount += p.IsMajoration ? 1 : 0;
                return majoCount;
            }
        }
		#endregion

		#region Accessing parameters
		public double GetDoubleParameterValue(string name)
        {
            if (!(GetParameterByName(name) is ParameterDouble param))
                throw new ParameterInvalidType(name, "ParameterDouble");
            return param.Value;
        }

        public int GetIntParameterValue(string name)
        {
            if (!(GetParameterByName(name) is ParameterInt param))
                throw new ParameterInvalidType(name, "ParameterInt");
            return param.Value;
        }

        public bool GetBoolParameterValue(string name)
        {
            if (!(GetParameterByName(name) is ParameterBool param))
                throw new ParameterInvalidType(name, "ParameterBool");
            return param.Value;
        }

        public int GetMultiParameterValue(string name)
        {
            if (!(GetParameterByName(name) is ParameterMulti param))
                throw new ParameterInvalidType(name, "ParameterMulti");
            return param.Value;
        }

        public string GetMultiParameterValueString(string name)
        {
            if (!(GetParameterByName(name) is ParameterMulti param))
                throw new ParameterInvalidType(name, "ParameterMulti");
            return param.ValueList[param.Value];
        }
		#endregion

        #region IEnumerable overrides
        public IEnumerator GetEnumerator() {  return new Enumerator(this); }
        #endregion

        #region Enumerator class
        private class Enumerator : IEnumerator
        {

            public Enumerator(ParameterStack obj)
            {
                oThis = obj;
                cursor = -1;
                version = oThis._version;
            }

            public bool MoveNext()
            {
                ++cursor;
                if (cursor > (oThis.ParameterList.Count - 1))
                {
                    return false;
                }
                return true;
            }
            public void Reset()
            {
                cursor = -1;
            }

            public object Current
            {
                get
                {
                    if (oThis._version != version)
                    {
                        throw new InvalidOperationException("Collection was modified");
                    }
                    if (cursor > (oThis.ParameterList.Count - 1))
                    {
                        throw new InvalidOperationException("Enumeration already finished");
                    }
                    if (cursor == -1)
                    {
                        throw new InvalidOperationException("Enumeration not started");
                    }
                    return oThis.ParameterList[cursor];
                }
            }

            private readonly int version;
            private int cursor;
            private ParameterStack oThis;
        }
        #endregion

        #region Object override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Parameter p in this)
            {
                string sValue = string.Empty;
                ParameterBool pBool = p as ParameterBool;
                if (null != pBool) sValue = pBool.Value.ToString();
                ParameterInt pInt = p as ParameterInt;
                if (null != pInt) sValue = pInt.Value.ToString();
                ParameterDouble pDouble = p as ParameterDouble;
                if (null != pDouble) sValue = pDouble.Value.ToString();
                ParameterMulti pMulti = p as ParameterMulti;
                if (null != pMulti) sValue = pMulti.Value.ToString() + " " + string.Join("|", pMulti.ValueList);


                sb.AppendFormat("{0} = {1}\n", p.Name, sValue);
            }
            return sb.ToString();
        }
        #endregion
    }
	#endregion

    #region ParameterStackUpdater
    public class ParameterStackUpdater
    {
        #region Constructor
        public ParameterStackUpdater(ParameterStack stackIn)
        {
            _stackIn = stackIn ?? new ParameterStack();
            UpdatedStack = new ParameterStack();
        }
        #endregion

        #region Creating methods
        public ParameterDouble CreateDoubleParameter(string name, string description, double valueDefault, bool hasMinValue, double valueMin, bool hasMaxValue, double valueMax)
        {
            UpdatedStack.AddDoubleParameter(name, description
                , _stackIn.HasParameter(name) ? _stackIn.GetDoubleParameterValue(name) : valueDefault
                , hasMinValue, valueMin, hasMaxValue, valueMax);
            return UpdatedStack.GetParameterByName(name) as ParameterDouble;
        }
        public ParameterDouble CreateDoubleParameter(string name, string description, double valueDefault)
        {   return CreateDoubleParameter(name, description, valueDefault, false, double.MinValue, false, double.MaxValue);  }
        public ParameterDouble CreateDoubleParameter(string name, string description, double valueDefault, double minValue)
        {   return CreateDoubleParameter(name, description, valueDefault, true, minValue, false, double.MaxValue);  }
        public ParameterDouble CreateDoubleParameter(string name, string description, double valueDefault, double minValue, double maxValue)
        {   return CreateDoubleParameter(name, description, valueDefault, true, minValue, true, maxValue);  }
        public ParameterInt CreateIntParameter(string name, string description, int valueDefault, bool hasMinValue, int minValue, bool hasMaxValue, int maxValue)
        {
            UpdatedStack.AddIntParameter(name, description
                , _stackIn.HasParameter(name) ? _stackIn.GetIntParameterValue(name) : valueDefault
                , hasMinValue, minValue, hasMaxValue, maxValue);
            return UpdatedStack.GetParameterByName(name) as ParameterInt;
        }
        public ParameterBool CreateBoolParameter(string name, string description, bool valueDefault)
        {
            UpdatedStack.AddBoolParameter(name, description
                , _stackIn.HasParameter(name) ? _stackIn.GetBoolParameterValue(name) : valueDefault);
            return UpdatedStack.GetParameterByName(name) as ParameterBool;
        }
        public ParameterMulti CreateMultiParameter(string name, string description, string[] displayList, string[] valueList, int valueDefault)
        {
            UpdatedStack.AddMultiParameter(name, description, displayList, valueList
                , _stackIn.HasParameter(name) ? _stackIn.GetMultiParameterValue(name) : valueDefault); 
            return UpdatedStack.GetParameterByName(name) as ParameterMulti;
        }
        #endregion

        #region Accessing parameters
        public double GetDoubleParameterValue(string name)
        {   return UpdatedStack.GetDoubleParameterValue(name);  }
        public int GetIntParameterValue(string name)
        {   return UpdatedStack.GetIntParameterValue(name);    }
        public bool GetBoolParameterValue(string name)
        {   return UpdatedStack.GetBoolParameterValue(name);   }
        public int GetMultiParameterValue(string name)
        {   return UpdatedStack.GetMultiParameterValue(name);  }
        #endregion

        #region Retrieve updated stack
        /// <summary>
        /// return the updated stack
        /// </summary>
        public ParameterStack UpdatedStack { get; }
        #endregion

        #region Data members
        private ParameterStack _stackIn;
        #endregion
    }
    #endregion

    #region Topology
    public class Loop : List<Vector2D>
    {
    }
    public class Face
    {
        public Face(int id, List<Loop> loops)
        { ID = id; Loops = loops; }
        public int ID { get; set; }
        public List<Loop> Loops { get; set; }
    }
    public class Fold
    {
        public int ID { get; set; }
        public Vector2D Pt1 { get; set; }
        public Vector2D Pt2 { get; set; }
        public Face Face1 { get; set; }
        public Face Face2 { get; set; }
        public List<double> Steps { get; }
        public bool HasFace(Face f) { return f == Face1 || f == Face2; }
        public Face OtherFace(Face f)
        {
            if (f == Face1) return Face2;
            else if (f == Face2) return Face1;
            else return null;
        }
    }
    public class Topology
    {
        public Topology(List<Face> faces, List<Fold> folds)
        { Faces = faces; Folds = folds; }
        public List<Face> Faces { get; set; }
        public List<Fold> Folds { get; set; }
        public List<Face> GetChildrens(Face f, Face faceFrom)
        {
            List<Face> fChildrens = new List<Face>();
            foreach (Fold fold in Folds)
            {
                if (fold.HasFace(f) && !fold.HasFace(faceFrom))
                    fChildrens.Add(fold.OtherFace(f));
            }
            return fChildrens;
        }
    }
    #endregion

    #region IPlugin common interface
    /// <summary>
    /// http://www.codeproject.com/KB/cs/pluginsincsharp.aspx
    /// </summary>
    public interface IPlugin
    { 
        /// <summary>
        /// Component name
        /// </summary>
        string Name {get;}
        /// <summary>
        /// Component description
        /// </summary>
        string Description {get;}
        /// <summary>
        /// Author / Company
        /// </summary>
        string Author {get;}
        /// <summary>
        /// Plugin version
        /// </summary>
        string Version {get;}
        /// <summary>
        /// Plugin guid used to access it (unique name)
        /// </summary>
        Guid Guid { get;}    
        /// <summary>
        /// List of parameters with their default values
        /// </summary>
        ParameterStack Parameters {get;}
        /// <summary>
        /// Create factory entities
        /// </summary>
        /// <param name="factory">Factory in which entities should be created</param>
        /// <param name="stack">List of parameters with their associated values</param>
        void CreateFactoryEntities(PicFactory factory, ParameterStack stack, Transform2D transform);
        /// <summary>
        /// Method called before generating entities
        /// </summary>
        void Initialize();
        /// <summary>
        /// Method called by destructor
        /// </summary>
        void Dispose();
        /// <summary>
        /// IPluginHost accessor
        /// </summary>
        IPluginHost Host { get;set;}
    }
    #endregion

    #region IPluginExt1 (additional interface version1) --> Deprecated
    public interface IPluginExt1
    {
        /// <summary>
        /// source code used to build component
        /// </summary>
        string SourceCode {get;}
        /// <summary>
        /// return true if file has an embedded bitmap
        /// </summary>
        bool HasEmbeddedThumbnail {get;}
        /// <summary>
        /// embedded bitmap
        /// </summary>
        Bitmap Thumbnail { get;}
        /// <summary>
        /// Method called by destructor
        /// </summary>
        void Dispose();
        /// <summary>
        /// recommended X/Y offsets for imposition
        /// </summary>
        double ImpositionOffsetX(ParameterStack stack);
        double ImpositionOffsetY(ParameterStack stack);
    }
    #endregion

    #region IPluginExt2 (additional interface version2) --> Comes as a replacement of IPluginExt1
    public interface IPluginExt2
    {
        #region Methods available in IPluginExt1
        /// <summary>
        /// source code used to build component
        /// </summary>
        string SourceCode { get; }
        /// <summary>
        /// return true if file has an embedded bitmap
        /// </summary>
        bool HasEmbeddedThumbnail { get; }
        /// <summary>
        /// embedded bitmap
        /// </summary>
        Bitmap Thumbnail { get; }
        /// <summary>
        /// Method called by destructor
        /// </summary>
        void Dispose();
        /// <summary>
        /// recommended X/Y offsets for imposition
        /// </summary>
        double ImpositionOffsetX(ParameterStack stack);
        double ImpositionOffsetY(ParameterStack stack);
        #endregion

        #region Additional methods
        /// <summary>
        /// Build / rebuild parameter stack 
        /// </summary>
        ParameterStack BuildParameterStack(ParameterStack stackIn);
        /// <summary>
        /// Is supporting palletization ?
        /// </summary>
        bool IsSupportingPalletization { get; }
        /// <summary>
        /// Outer dimensions
        /// Method should only be called if component supports palletization
        /// </summary>
        void OuterDimensions(ParameterStack stack, out double[] dimensions);
        /// <summary>
        /// Returns case type to be used for ECT computation 
        /// </summary>
        int CaseType { get; }
        /// <summary>
        /// Is supporting automatic folding
        /// </summary>
        bool IsSupportingAutomaticFolding { get; }
        /// <summary>
        /// Reference point that defines anchored face
        /// </summary>
        List<Vector2D> ReferencePoints(ParameterStack stack);
        #endregion
    }
    #endregion

    #region IPluginExt3 (additional interface version3) --> Comes as a replacement of IPluginExt2
    public interface IPluginExt3
    {
        #region Methods available in IPluginExt1
        /// <summary>
        /// source code used to build component
        /// </summary>
        string SourceCode { get; }
        /// <summary>
        /// return true if file has an embedded bitmap
        /// </summary>
        bool HasEmbeddedThumbnail { get; }
        /// <summary>
        /// embedded bitmap
        /// </summary>
        Bitmap Thumbnail { get; }
        /// <summary>
        /// Method called by destructor
        /// </summary>
        void Dispose();
        /// <summary>
        /// recommended X/Y offsets for imposition
        /// </summary>
        double ImpositionOffsetX(ParameterStack stack);
        double ImpositionOffsetY(ParameterStack stack);
        #endregion

        #region Methods available in IPluginExt2
        /// <summary>
        /// Build / rebuild parameter stack 
        /// </summary>
        ParameterStack BuildParameterStack(ParameterStack stackIn);
        /// <summary>
        /// Is supporting palletization ?
        /// </summary>
        bool IsSupportingPalletization { get; }
        /// <summary>
        /// Outer dimensions
        /// Method should only be called if component supports palletization
        /// </summary>
        void OuterDimensions(ParameterStack stack, out double[] dimensions);
        /// <summary>
        /// Returns case type to be used for ECT computation 
        /// </summary>
        int CaseType { get; }
        /// <summary>
        /// Is supporting automatic folding
        /// </summary>
        bool IsSupportingAutomaticFolding { get; }
        /// <summary>
        /// Reference point that defines anchored face
        /// </summary>
        List<Vector2D> ReferencePoints(ParameterStack stack);
        #endregion

        #region Additional methods & properties
        /// <summary>
        /// Is supporting bundling
        /// </summary>
        bool IsSupportingFlatPalletization { get; }
        /// <summary>
        /// Flat dimensions
        /// </summary>
        void FlatDimensions(ParameterStack stack, out double[] flatDimensions);
        /// <summary>
        /// Number of parts
        /// </summary>
        int NoParts { get; }
        /// <summary>
        /// Part name
        /// </summary>
        string PartName(int i);
        /// <summary>
        /// Layer name
        /// </summary>
        string LayerName(int i);
        #endregion
    }
    #endregion

    #region IPluginExt4 (additional interface version4) --> Comes as a replacement of IPluginExt3
    public interface IPluginExt4
    {
        #region Methods available in IPluginExt1
        /// <summary>
        /// source code used to build component
        /// </summary>
        string SourceCode { get; }
        /// <summary>
        /// return true if file has an embedded bitmap
        /// </summary>
        bool HasEmbeddedThumbnail { get; }
        /// <summary>
        /// embedded bitmap
        /// </summary>
        Bitmap Thumbnail { get; }
        /// <summary>
        /// Method called by destructor
        /// </summary>
        void Dispose();
        /// <summary>
        /// recommended X/Y offsets for imposition
        /// </summary>
        double ImpositionOffsetX(ParameterStack stack);
        double ImpositionOffsetY(ParameterStack stack);
        #endregion

        #region Methods available in IPluginExt2
        /// <summary>
        /// Build / rebuild parameter stack 
        /// </summary>
        ParameterStack BuildParameterStack(ParameterStack stackIn);
        /// <summary>
        /// Is supporting palletization ?
        /// </summary>
        bool IsSupportingPalletization { get; }
        /// <summary>
        /// Outer dimensions
        /// Method should only be called if component supports palletization
        /// </summary>
        void OuterDimensions(ParameterStack stack, out double[] dimensions);
        /// <summary>
        /// Returns case type to be used for ECT computation 
        /// </summary>
        int CaseType { get; }
        /// <summary>
        /// Is supporting automatic folding
        /// </summary>
        bool IsSupportingAutomaticFolding { get; }
        /// <summary>
        /// Reference point that defines anchored face
        /// </summary>
        List<Vector2D> ReferencePoints(ParameterStack stack);
        #endregion

        #region Methods available in IPluginExt3
        /// <summary>
        /// Is supporting bundling
        /// </summary>
        bool IsSupportingFlatPalletization { get; }
        /// <summary>
        /// Flat dimensions
        /// </summary>
        void FlatDimensions(ParameterStack stack, out double[] flatDimensions);
        /// <summary>
        /// Number of parts
        /// </summary>
        int NoParts { get; }
        /// <summary>
        /// Part name
        /// </summary>
        string PartName(int i);
        /// <summary>
        /// Layer name
        /// </summary>
        string LayerName(int i);
        #endregion

        #region Additional methods & properties
        /// <summary>
        /// Has a topology
        /// </summary>
        bool HasTopology { get; }
        /// <summary>
        /// Access topology
        /// </summary>
        void GetTopology(ParameterStack stack, out Topology topology);
        #endregion
    }
    #endregion

    #region IPluginHost
    public interface IPluginHost
    {
        void Feedback(string message, IPlugin Plugin);
        IPlugin GetPluginByGuid(Guid guid);
        IPlugin GetPluginByGuid(string sGuid);
        ParameterStack GetInitializedParameterStack(IPlugin plugin);
    }
    #endregion
}
