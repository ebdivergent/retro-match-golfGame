using System;
using UnityEngine;

namespace AppCore
{
    public class DefaultPrefsManager : IPrefsManager
    {
        public DefaultPrefsManager(IJsonConverter jsonConverter)
        {
            JsonConverter = jsonConverter;
        }

        public IJsonConverter JsonConverter { get; private set; }

        public T Get<T>(PrefsKey prefsKey)
        {
            try
            {
                return JsonConverter.Deserialize<T>(GetString(prefsKey));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default(T);
            }
        }

        public T Get<T>(PrefsKey prefsKey, T defaultValue)
        {
            string val = GetString(prefsKey, "");

            if (string.IsNullOrEmpty(val))
                return defaultValue;

            try
            {
                return JsonConverter.Deserialize<T>(val);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default(T);
            }
        }

        public float GetFloat(PrefsKey prefsKey)
        {
            return PlayerPrefs.GetFloat(prefsKey.ToString());
        }

        public float GetFloat(PrefsKey prefsKey, float defaultValue)
        {
            return PlayerPrefs.GetFloat(prefsKey.ToString(), defaultValue);
        }

        public int GetInt(PrefsKey prefsKey)
        {
            return PlayerPrefs.GetInt(prefsKey.ToString());
        }

        public int GetInt(PrefsKey prefsKey, int defaultValue)
        {
            return PlayerPrefs.GetInt(prefsKey.ToString(), defaultValue);
        }
        
        public string GetString(PrefsKey prefsKey)
        {
            return PlayerPrefs.GetString(prefsKey.ToString());
        }

        public string GetString(PrefsKey prefsKey, string defaultValue)
        {
            return PlayerPrefs.GetString(prefsKey.ToString(), defaultValue);
        }

        public void Set<T>(PrefsKey prefsKey, T value)
        {
            SetString(prefsKey, JsonConverter.Serialize(value));
        }

        public void SetFloat(PrefsKey prefsKey, float value)
        {
            PlayerPrefs.SetFloat(prefsKey.ToString(), value);
        }

        public void SetInt(PrefsKey prefsKey, int value)
        {
            PlayerPrefs.SetInt(prefsKey.ToString(), value);
        }

        public void SetString(PrefsKey prefsKey, string value)
        {
            PlayerPrefs.SetString(prefsKey.ToString(), value);
        }

        public bool HasKey(PrefsKey key)
        {
            string keyToStr = key.ToString();
            return PlayerPrefs.HasKey(keyToStr);
        }

        public void DeleteKey(PrefsKey key)
        {
            PlayerPrefs.DeleteKey(key.ToString());
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}