﻿using NJsonSchema.Generation;
using System.Text;

namespace FastEndpoints.Swagger;

internal class DefaultSchemaNameGenerator : ISchemaNameGenerator
{
    private readonly bool shortSchemaNames;

    public DefaultSchemaNameGenerator(bool shortSchemaNames)
    {
        this.shortSchemaNames = shortSchemaNames;
    }

    public string? Generate(Type type)
    {
        bool isGeneric = type.IsGenericType;
        string fullNameWithoutGenericArgs;

        if (isGeneric)
            fullNameWithoutGenericArgs = type.FullName![..type.FullName.IndexOf('`')];
        else
            fullNameWithoutGenericArgs = type.FullName!;

        if (shortSchemaNames)
        {
            var index = fullNameWithoutGenericArgs.LastIndexOf('.');
            index = index == -1 ? 0 : index;
            var shortName = fullNameWithoutGenericArgs[index..];
            if (isGeneric)
                return shortName + GenericArgString(type);
            return shortName;
        }
        else
        {
            var sanitizedFullName = fullNameWithoutGenericArgs.Replace(".", string.Empty);
            if (isGeneric)
                return sanitizedFullName + GenericArgString(type);
            return sanitizedFullName;
        }

        static string? GenericArgString(Type type)
        {
            if (type.IsGenericType)
            {
                var sb = new StringBuilder();
                var args = type.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    sb.Append("Of")
                      .Append(TypeNameWithoutGenericArgs(arg))
                      .Append(GenericArgString(arg));
                    if (i < args.Length - 1) sb.Append("And");
                }
                return sb.ToString();
            }
            return type.Name;

            static string TypeNameWithoutGenericArgs(Type type)
            {
                var index = type.Name.IndexOf('`');
                index = index == -1 ? 0 : index;
                return type.Name![..index];
            }
        }
    }
}
