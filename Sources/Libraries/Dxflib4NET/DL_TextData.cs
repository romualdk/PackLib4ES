namespace Dxflib4NET
{
    public class DL_TextData
    {
        #region Constructor
        public DL_TextData(
            double tipx, double tipy, double tipz,
            double tapx, double tapy, double tapz,
            double tHeight,
            double 	tXScaleFactor,
            int	tTextGenerationFlags,
            int	tHJustification,
            int	tVJustification,
            string tText,
            string tStyle,
            double 	tAngle)
        {
            ipx = tipx; ipy = tipy; ipz = tipz;
            apx = tapx; apy = tapy; apz = tapz;
            height = tHeight;
            xScaleFactor = tXScaleFactor;
            textGenerationFlags = tTextGenerationFlags;
            hJustification = tHJustification; vJustification = tVJustification;
            text = tText;
            style = tStyle;
            angle = tAngle;
        }
        #endregion
        #region Data members
        public double ipx, ipy, ipz;
        public double apx, apy, apz;
        public double height;
        public double xScaleFactor;
        public int textGenerationFlags;
        public int hJustification;
        public int vJustification;
        public string text;
        public string style;
        public double angle;
        #endregion
    }
}
