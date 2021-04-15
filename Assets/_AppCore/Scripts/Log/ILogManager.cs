namespace AppCore
{
    public interface ILogManager
    {
        ILogger DefaultLogger { get; }
        ILogger EditorLogger { get; }
    }
}
