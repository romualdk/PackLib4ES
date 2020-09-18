namespace Pic.Factory2D.Control
{
    #region Public event args
    public class LayoutCtrlEventArgs
    {
        #region Constructor
        public LayoutCtrlEventArgs(int result, string outputPath)
        {
            Result = result;
            OutputPath = outputPath;
        }
        #endregion

        #region Public properties
        public string OutputPath { get; private set; }
        public int Result { get; private set; }
        #endregion
    }
    #endregion
}