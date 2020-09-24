#region Using directives
using Pic.Factory2D;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Pic.Plugin
{
    #region Profile
    public class Profile
    {
        #region Constructor
        public Profile(string name)
        {
            Name = name;
        }
        #endregion
        #region Object overrides
        public override string ToString() => Name;
        #endregion
        #region Data members
        public string Name { get; }
        public string Description { get; set; }
        public double Thickness { get; set; }
        #endregion
    }
    #endregion

    #region ProfileLoader
    public abstract class ProfileLoader
    {
        #region Constructor
        protected ProfileLoader()
        {
            Reset();
        }
        #endregion

        #region Public methods
        public bool HasParameter(Parameter param)
        {
            if (null == _dictMajoration)
                _dictMajoration = LoadMajorationList();
            return _dictMajoration.ContainsKey(param.Name);
        }

        public void GetParameterValue(ref Parameter param)
        {
            if (null == _dictMajoration)
                _dictMajoration = LoadMajorationList();
            if (param is ParameterDouble parameterDouble)
                parameterDouble.Value = ConvertUnit(_dictMajoration[param.Name]);
        }

        public double GetParameterDValue(string name)
        {
            if (null == _dictMajoration)
                _dictMajoration = LoadMajorationList();
            if (_dictMajoration.ContainsKey(name))
                return ConvertUnit(_dictMajoration[name]);
            else
                throw new Exception(string.Format("ProfileLoader : No majoration with name = {0}!", name));
        }

        abstract public void SetComponent(Component comp);
        #endregion

        #region Unit conversion helpers
        private double ConvertUnit(double dValue)
        {
            switch (UnitSystem.Instance.USyst)
            {
                case UnitSystem.EUnit.US: return dValue / 25.4;
                default: return 1.0;
            }
        }
        #endregion

        #region Abstract method
        protected abstract Profile[] LoadProfiles();
        protected abstract Dictionary<string, double> LoadMajorationList();
        public virtual bool CanEditMajorations { get { return false; } }
        public abstract void EditMajorations();
        public abstract void BuildCardboardProfileDictionary();
        #endregion

        #region Public properties
        public Profile Selected
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                _dictMajoration = null;
            }
        }
        public Profile[] Profiles
        {
            get
            {
                if (null == _profiles)
                    _profiles = LoadProfiles();
                return _profiles;
            }
        }

        public string[] ProfileNames => Profiles.Where(x => x != null).Select(x => x.Name).ToArray();
        #endregion

        #region Private methods
        private void Reset()
        {
            _selectedProfile = null;
            _dictMajoration = null;
        }
        #endregion

        #region Notification mechanism
        public delegate void MajorationModifiedHandler(object sender);
        public event MajorationModifiedHandler MajorationModified;

        public virtual void NotifyModifications()
        {
            BuildCardboardProfileDictionary();
            MajorationModified?.Invoke(this);
        }
        #endregion

        #region Data members
        // list of profiles
        protected Profile[] _profiles;
        // selected profile
        protected Profile _selectedProfile;
        protected Dictionary<string, double> _dictMajoration;
        #endregion
    }
    #endregion
}
