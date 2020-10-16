using System;
using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public class DefaultTypeFormatter : ATypeFormatter
    {
        readonly Dictionary<Type, string> _primitivesToNames = new Dictionary<Type, string>
        {
            [typeof(string)] = "string",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal"
        };

        protected override string Format(Type type)
        {
            if (_primitivesToNames.ContainsKey(type)) return _primitivesToNames[type];
            if (type.IsEnum) return string.Join(", ", Enum.GetNames(type));
            return type.Name;
        }
    }
}