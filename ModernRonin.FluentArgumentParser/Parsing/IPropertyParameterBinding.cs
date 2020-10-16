using System.Reflection;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IPropertyParameterBinding
    {
        AParameter Parameter { get; set; }
        bool DoesMatch(PropertyInfo propertyInfo);
    }
}