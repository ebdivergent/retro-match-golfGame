using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    public class DataManager : MonoBehaviour, IDataManager
    {
        public IJsonConverter DefaultJsonConverter { get; private set; }
        public IPrefsManager EncryptedPrefsManager { get; private set; }
        public IPrefsManager DefaultPrefsManager { get; private set; }
        public DataContainer DataContainer { get; private set; }

        private Dictionary<PrefsKey, IPrefsManager> _keyManagers;

        public static IDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _keyManagers = new Dictionary<PrefsKey, IPrefsManager>();

            foreach (PrefsKey key in (PrefsKey[])System.Enum.GetValues(typeof(PrefsKey)))
            {
                string typeName;
                string managerName;
                string propName;

                string keyToString = key.ToString();

                if (!ParseKeyString(keyToString, out propName, out typeName, out managerName))
                {
                    Log.Error($"Failed to parse key {keyToString}!");
                    continue;
                }

                var manager = GetManager(managerName);

                if (manager != null)
                    _keyManagers.Add(key, manager);
                else
                    Log.Error("Key's manager not found!");
            }

            DefaultJsonConverter = new UnityJsonConverter();

            EncryptedPrefsManager = new EncryptedPlayerPrefs(DefaultJsonConverter);
            DefaultPrefsManager = new DefaultPrefsManager(DefaultJsonConverter);
            DataContainer = new DataContainer(this);
        }
        public static bool ParseKeyString(string keyEnumName, out string propertyName, out string type, out string prefsManagerName)
        {
            propertyName = "";
            type = "";
            prefsManagerName = "";

            int firstIndexOfUnderscore = keyEnumName.IndexOf('_');

            if (firstIndexOfUnderscore == -1)
                return false;

            if (firstIndexOfUnderscore >= keyEnumName.Length - 1)
                return false;

            int lastIndexOfUnderscore = keyEnumName.LastIndexOf('_');

            if (lastIndexOfUnderscore >= keyEnumName.Length - 1)
                return false;

            propertyName = keyEnumName.Substring(0, firstIndexOfUnderscore);

            bool hasOtherPrefsManager = firstIndexOfUnderscore != lastIndexOfUnderscore;

            if (hasOtherPrefsManager && keyEnumName.Count(x => x == '_') > 2)
                return false;

            type = keyEnumName.Substring(lastIndexOfUnderscore + 1, keyEnumName.Length - (lastIndexOfUnderscore + 1));

            char prefsManager = 'd';

            if (hasOtherPrefsManager)
                prefsManager = keyEnumName[firstIndexOfUnderscore + 1];


            switch (prefsManager)
            {
                case 'e':
                case 'E':
                    prefsManagerName = "_dataManager.EncryptedPrefsManager";
                    break;
                case 'd':
                case 'D':
                default:
                    prefsManagerName = "_dataManager.DefaultPrefsManager";
                    break;
            }

            if (string.IsNullOrEmpty(prefsManagerName))
                return false;


            return true;
        }

        public static IPrefsManager GetManager(string prefsManager)
        {
            IPrefsManager manager = null;

            switch (prefsManager)
            {
                case "_dataManager.EncryptedPrefsManager":
                    manager = DataManager.Instance?.EncryptedPrefsManager ?? new EncryptedPlayerPrefs(new UnityJsonConverter());
                    break;
                case "_dataManager.DefaultPrefsManager":
                    manager = DataManager.Instance?.DefaultPrefsManager ?? new DefaultPrefsManager(new UnityJsonConverter());
                    break;
            }

            return manager;
        }
        public IPrefsManager GetManager(PrefsKey prefsKey)
        {
            IPrefsManager manager;

            _keyManagers.TryGetValue(prefsKey, out manager);

            return manager;
        }

        public static bool GetKeyValue(PrefsKey prefsKey, out string value, out bool json)
        {
            string propertyName;
            string prefsManager;
            string type;
            value = "";
            json = false;

            if (!ParseKeyString(prefsKey.ToString(), out propertyName, out type, out prefsManager))
            {
                return false;
            }

            IPrefsManager manager = GetManager(prefsManager);

            if (manager == null)
                return false;

            switch (type)
            {
                case "Int":
                case "int":
                    value = manager.GetInt(prefsKey, 0).ToString();
                    break;

                case "bool":
                case "Bool":
                    value = manager.GetInt(prefsKey, 0) == 1 ? "true" : "false";
                    break;

                case "Float":
                case "float":
                    value = manager.GetFloat(prefsKey, 0f).ToString();
                    break;

                case "String":
                case "string":
                    value = manager.Get(prefsKey, "");
                    break;

                default:
                    json = true;
                    goto case "string";
            }

            return true;
        }
    }
}