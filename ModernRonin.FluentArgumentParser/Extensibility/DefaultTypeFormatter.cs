using System;
using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public class DefaultTypeFormatter : ATypeFormatter
    {
        readonly Dictionary<Type, string> _primitivesToNames = new Dictionary<Type, string>
        {
            [typeof(int)] = "integer",
            [typeof(string)] = "text"
        };

        protected override string Format(Type type)
        {
            if (type.IsPrimitive) return _primitivesToNames[type];
            if (type.IsEnum) return string.Join(", ", Enum.GetNames(type));
            return type.Name;
        }
    }
}