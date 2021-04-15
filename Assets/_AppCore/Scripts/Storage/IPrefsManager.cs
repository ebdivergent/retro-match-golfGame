namespace AppCore
{
    public interface IPrefsManager
    {
        IJsonConverter JsonConverter { get; }

        int GetInt(PrefsKey prefsKey);
        int GetInt(PrefsKey prefsKey, int defaultValue);
        void SetInt(PrefsKey prefsKey, int value);

        float GetFloat(PrefsKey prefsKey);
        float GetFloat(PrefsKey prefsKey, float defaultValue);
        void SetFloat(PrefsKey prefsKey, float value);

        string GetString(PrefsKey prefsKey);
        string GetString(PrefsKey prefsKey, string defaultValue);
        void SetString(PrefsKey prefsKey, string value);

        T Get<T>(PrefsKey prefsKey);
        T Get<T>(PrefsKey prefsKey, T defaultValue);
        void Set<T>(PrefsKey prefsKey, T value);

        bool HasKey(PrefsKey prefsKey);
        void DeleteKey(PrefsKey prefsKey);
        void Clear();
    }
}