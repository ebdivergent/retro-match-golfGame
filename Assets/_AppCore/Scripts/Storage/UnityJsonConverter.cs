using UnityEngine;

namespace AppCore
{
    public class UnityJsonConverter : IJsonConverter
    {
        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}