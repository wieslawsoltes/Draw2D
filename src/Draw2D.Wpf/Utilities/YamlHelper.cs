using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Draw2D.Wpf.Utilities
{
    public class YamlHelper
    {
        public static string ToYaml(object graph)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new SerializerBuilder()
                    .EnsureRoundtrip()
                    .WithNamingConvention(new NullNamingConvention())
                    .Build();
                serializer.Serialize(writer, graph);
                return writer.ToString();
            }
        }

        public static T FromYaml<T>(string yaml)
        {
            using (var reader = new StringReader(yaml))
            {
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithNamingConvention(new NullNamingConvention())
                    .Build();
                return deserializer.Deserialize<T>(reader);
            }
        }
    }
}
