namespace treeDiM.ThreadCallback
{
    public interface IThreadCallback
    {
        void Begin();
        void SetText(string text);
        bool IsAborting { get; }
        void End();
    }
}
