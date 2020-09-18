#region Using directives
using Pic.Factory2D.Properties;
using System.Collections;
#endregion

namespace Pic.Factory2D
{
    public class UnitSystem
    {
        #region Enum
        public enum EUnit { METRIC, US };
        #endregion
        #region Private constructor
        private UnitSystem(int iUnitSystem)
        {
            switch (iUnitSystem)
            {
                case 1: USyst = EUnit.US; break;
                default: USyst = EUnit.METRIC; break;
            }
        }
        #endregion
        #region Singleton members
        public static UnitSystem Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new UnitSystem(Settings.Default.UnitSystem);
                return _instance;
            }
        }
        private static UnitSystem _instance;

        public string UnitLength
        {
            get
            {
                switch (Instance.USyst)
                {
                    case EUnit.US: return "in";
                    default: return "mm";
                }
            }
        }
        public string UnitArea
        {
            get
            {
                switch (Instance.USyst)
                {
                    case EUnit.US: return "in²";
                    default: return "mm²";
                }
            }
        }

        public string CumulativeLength(double length)
        {
            switch (Instance.USyst)
            {
                case EUnit.US: return $"{length:0.#} in";
                default: return $"{length/1000.0:0.#} m";
            }
        }
        public string Area(double area)
        {
            switch (Instance.USyst)
            {
                case EUnit.US: return $"{area:0.##} in²";
                default: return $"{area*1.0E-06:0.###} m²";
            }
        }
        #endregion
        #region Public properties
        public EUnit USyst { get; set; }
        #endregion
    }
}
