namespace AppCore
{
    public interface IDataManager
    {
        IPrefsManager EncryptedPrefsManager { get; }
        IPrefsManager DefaultPrefsManager { get; }
        DataContainer DataContainer { get; }
    }
}