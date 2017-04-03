// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Draw2D.Utilities
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
