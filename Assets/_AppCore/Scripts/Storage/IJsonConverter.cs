namespace AppCore
{
    public interface IJsonConverter
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}