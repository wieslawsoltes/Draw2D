using System;

namespace Core2D.Serializer
{
    public interface IJsonSerializer
    {
        T Open<T>(string path);
        void Save<T>(string path, T obj);
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
        object Deserialize(string json, Type type);
        T Clone<T>(object obj);
    }
}
