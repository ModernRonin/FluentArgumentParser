using System;
using System.Linq;
using System.Reflection;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public class DefaultNamingStrategy : INamingStrategy
    {
        public string GetLongName(PropertyInfo propertyInfo) => propertyInfo.Name.KebabCased();

        public string GetShortName(PropertyInfo propertyInfo, string[] shortNamesToBeExcluded)
        {
            return propertyInfo.Name.ToLowerInvariant()
                .FirstOrDefault(c => !shortNamesToBeExcluded.Contains(c.ToString()))
                .ToString();
        }

        public string GetVerbName(Type type) => type.Name.ToLowerInvariant();
    }
}