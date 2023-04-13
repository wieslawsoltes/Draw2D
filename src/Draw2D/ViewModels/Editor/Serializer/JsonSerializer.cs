﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Draw2D.Serializer;

public class JsonSerializer
{
    internal class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) ||
                                                    (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
            {
                return base.CreateArrayContract(objectType);
            }
            return base.CreateContract(objectType);
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ObservableCollection<>)
                        .MakeGenericType(type.GenericTypeArguments[0]));
            }
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ISet<>))
            {
                return base
                    .ResolveContract(typeof(HashSet<>)
                        .MakeGenericType(type.GenericTypeArguments[0]));
            }
            return base.ResolveContract(type);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return new List<JsonProperty>(base.CreateProperties(type, memberSerialization).Where(p => p.Writable));
        }
    }

    private static readonly JsonSerializerSettings Settings;

    static JsonSerializer()
    {
        Settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new CustomContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new KeyValuePairConverter() }
        };
    }

    public static string ToJson<T>(T value)
    {
        return JsonConvert.SerializeObject(value, Settings);
    }

    public static T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public static T FromJsonFile<T>(string path)
    {
        var json = File.ReadAllText(path);
        return FromJson<T>(json);
    }

    public static void ToJsonFile<T>(string path, T value)
    {
        var json = ToJson<T>(value);
        File.WriteAllText(path, json);
    }
}