using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Draw2D.Wpf.Utilities
{
    public class YamlDotNetSerializer
    {
        public static string ToYaml(object graph, IDictionary<Type, string> tagMappings)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(new NullNamingConvention())
                    .WithTagMappings(tagMappings)
                    .Build();
                serializer.Serialize(writer, graph);
                return writer.ToString();
            }
        }

        public static T FromYaml<T>(string yaml, IDictionary<Type, string> tagMappings)
        {
            using (var reader = new StringReader(yaml))
            {
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithNamingConvention(new NullNamingConvention())
                    .WithTagMappings(tagMappings)
                    .Build();
                return deserializer.Deserialize<T>(reader);
            }
        }
    }
}
