using System;
using System.Reflection;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public interface INamingStrategy
    {
        string GetLongName(PropertyInfo propertyInfo);
        string GetShortName(PropertyInfo propertyInfo, string[] shortNamesToBeExcluded);
        string GetVerbName(Type type);
    }
}