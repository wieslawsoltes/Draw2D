using System;
using System.IO;
using Newtonsoft.Json;

namespace Core2D.Serializer
{
    public class JsonSerializerNewtonsoft : IJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings;

        static JsonSerializerNewtonsoft()
        {
            Settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public T Open<T>(string path)
        {
            var json = File.ReadAllText(path);
            return Deserialize<T>(json);
        }

        public void Save<T>(string path, T obj)
        {
            var json = Serialize<T>(obj);
            File.WriteAllText(path, json);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        public T Clone<T>(object obj)
        {
            var json = Serialize(obj);
            return Deserialize<T>(json);
        }
    }
}
