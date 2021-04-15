using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace AppCore
{
    public class EncryptedPlayerPrefs : IPrefsManager
    {
        public EncryptedPlayerPrefs(IJsonConverter jsonConverter)
        {
            JsonConverter = jsonConverter;
        }

        // Modify this key in this file :
        private string _privateKey = "9ETrEsWaFRach3gexaDr";

        // Add some values to this array before using EncryptedPlayerPrefs
        private static string[] _keys = new string[] {
            "ML23DJKH",
            "5N5QmphC", 
            "Dx8lfmk9", 
            "HYN0dRGU", 
            "dG9rFbn6", 
            "sraoCxMZ",
            "obIU1aYO",
            "DAYvR4oj",
            "AJZCloF1",
            "Z92zctnw",
        };

        private string Md5(string strToEncrypt) 
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);
 
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
 
            string hashString = "";
 
            for (int i = 0; i < hashBytes.Length; i++) {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
 
            return hashString.PadLeft(32, '0');
        }
    
        private void SaveEncryption(string key, string type, string value) 
        {
            int keyIndex = (int)Mathf.Floor(Random.value * _keys.Length);
            string secretKey = _keys[keyIndex];
            string check = Md5(type + "_" + _privateKey + "_" + secretKey + "_" + value);
            PlayerPrefs.SetString(key + "_encryption_check", check);
            PlayerPrefs.SetInt(key + "_used_key", keyIndex);
        }

        private bool CheckEncryption(string key, string type, string value) 
        {
            int keyIndex = PlayerPrefs.GetInt(key + "_used_key");
            string secretKey = _keys[keyIndex];
            string check = Md5(type + "_" + _privateKey + "_" + secretKey + "_" + value);
            if(!PlayerPrefs.HasKey(key + "_encryption_check")) return false;
            string storedCheck = PlayerPrefs.GetString(key + "_encryption_check");
            return storedCheck == check;
        }

        public IJsonConverter JsonConverter { get; private set; }
    
        public void SetInt(PrefsKey prefsKey, int value) 
        {
            string keyToStr = prefsKey.ToString();
            PlayerPrefs.SetInt(keyToStr.ToString(), value);
            SaveEncryption(keyToStr, "int", value.ToString());
        }
    
        public void SetFloat(PrefsKey prefsKey, float value)
        {
            string keyToStr = prefsKey.ToString();
            PlayerPrefs.SetFloat(keyToStr, value);
            SaveEncryption(keyToStr, "float", Mathf.Floor(value*1000).ToString());
        }
    
        public void SetString(PrefsKey prefsKey, string value)
        {
            string keyToStr = prefsKey.ToString();
            PlayerPrefs.SetString(keyToStr, value);
            SaveEncryption(keyToStr, "string", value);
        }
    
        public int GetInt(PrefsKey prefsKey)
        {
            return GetInt(prefsKey, 0);
        }
    
        public float GetFloat(PrefsKey prefsKey) 
        {
            return GetFloat(prefsKey, 0f);
        }
    
        public string GetString(PrefsKey prefsKey) 
        {
            return GetString(prefsKey, "");
        }
    
        public int GetInt(PrefsKey prefsKey,int defaultValue)
        {
            string keyToStr = prefsKey.ToString();
            int value = PlayerPrefs.GetInt(keyToStr);
            if(!CheckEncryption(keyToStr, "int", value.ToString())) return defaultValue;
            return value;
        }
    
        public float GetFloat(PrefsKey prefsKey, float defaultValue)
        {
            string keyToStr = prefsKey.ToString();
            float value = PlayerPrefs.GetFloat(keyToStr);
            if(!CheckEncryption(keyToStr, "float", Mathf.Floor(value*1000).ToString())) return defaultValue;
            return value;
        }
    
        public string GetString(PrefsKey prefsKey, string defaultValue)
        {
            string keyToStr = prefsKey.ToString();
            string value = PlayerPrefs.GetString(keyToStr);
            if(!CheckEncryption(keyToStr, "string", value)) return defaultValue;
            return value;
        }
    
        public bool HasKey(PrefsKey prefsKey)
        {
            string keyToStr = prefsKey.ToString();
            return PlayerPrefs.HasKey(keyToStr);
        }
    
        public void DeleteKey(PrefsKey prefsKey)
        {
            string keyToStr = prefsKey.ToString();
            PlayerPrefs.DeleteKey(keyToStr);
            PlayerPrefs.DeleteKey(keyToStr + "_encryption_check");
            PlayerPrefs.DeleteKey(keyToStr + "_used_key");
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        public T Get<T>(PrefsKey prefsKey)
        {
            return JsonConverter.Deserialize<T>(GetString(prefsKey));
        }

        public T Get<T>(PrefsKey prefsKey, T defaultValue)
        {
            string val = GetString(prefsKey, "");

            if (string.IsNullOrEmpty(val))
                return defaultValue;

            return JsonConverter.Deserialize<T>(GetString(prefsKey));
        }

        public void Set<T>(PrefsKey prefsKey, T value)
        {
            SetString(prefsKey, JsonConverter.Serialize(value));
        }
    }
}