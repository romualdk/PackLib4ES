namespace Dxflib4NET
{
    public class DL_Attributes
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public DL_Attributes()
        {
            Color = 0;
            Width = 0;
            LineType = "BYLAYER";
        }
        /// <summary>
        /// Constructor for DXF attributes.
        /// </summary>
        /// <param name="layer">Layer name for this entity or empty for no layer. Every entity should be on a named layer</param>
        /// <param name="color">Color number (0..255). 0 = BYBLOCK, 256 = BYLAYER.</param>
        /// <param name="width">Line thickness. Defaults to zero.</param>
        /// <param name="lineType">Line type name or "BYLAYER" or "BYBLOCK". Defaults to "BYLAYER"</param>
        public DL_Attributes(string layer, int color, int width, string lineType)
        {
            Layer = layer;
            Color = color;
            Width = width;
            LineType = lineType;        
        }
        #endregion
        #region Public properties
        /// <summary>
        /// Sets/gets the layer.
        /// </summary>
        public string Layer { get; set; }
        /// <summary>
        /// Sets/gets the color (see DL_Codes, dxfColors)
        /// </summary>
        public int Color { get; set; }
        /// <summary>
        /// Sets/gets the width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Sets/gets the line type. This can be any string and is not checked to be a valid line type.
        /// </summary>
        public string LineType
        {
            get
            {
                if (lineType.Length == 0)
                    return "BYLAYER";
                else
                    return lineType;
            }
            set { lineType = value; }
        }

        #endregion
        #region Data members
        private string lineType;
        #endregion
    }
}
