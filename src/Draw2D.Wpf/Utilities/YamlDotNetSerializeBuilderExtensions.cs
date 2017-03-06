using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Draw2D.Wpf.Utilities
{
    public static class YamlDotNetSerializeBuilderExtensions
    {
        public static SerializerBuilder WithTagMappings(this SerializerBuilder builder, IDictionary<Type, string> tagMappings)
        {
            foreach (var tagMapping in tagMappings)
            {
                builder.WithTagMapping(tagMapping.Value, tagMapping.Key);
            }
            return builder;
        }

        public static DeserializerBuilder WithTagMappings(this DeserializerBuilder builder, IDictionary<Type, string> tagMappings)
        {
            foreach (var tagMapping in tagMappings)
            {
                builder.WithTagMapping(tagMapping.Value, tagMapping.Key);
            }
            return builder;
        }
    }
}
