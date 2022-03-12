using System;
using System.Linq;
using System.Reflection;
using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing;

class TestNamingStrategy : INamingStrategy
{
    public string GetLongName(PropertyInfo propertyInfo) => propertyInfo.Name.ToUpperInvariant();

    public string GetShortName(PropertyInfo propertyInfo, string[] shortNamesToBeExcluded)
    {
        var result = propertyInfo.Name.ToUpperInvariant().First().ToString();
        return shortNamesToBeExcluded.Contains(result) ? $"{result}{result}" : result;
    }

    public string GetVerbName(Type type) => type.Name.ToUpperInvariant();
}