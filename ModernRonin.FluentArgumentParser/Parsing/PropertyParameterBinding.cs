using System.Reflection;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Parsing;

public class PropertyParameterBinding<TTarget> : IPropertyParameterBinding where TTarget : new()
{
    readonly PropertyInfo _propertyInfo;

    public PropertyParameterBinding(INamingStrategy namingStrategy,
        PropertyInfo propertyInfo,
        string[] shortNamesToBeExcluded)
    {
        _propertyInfo = propertyInfo;
        var type = _propertyInfo.PropertyType;
        Parameter = parameter();
        Parameter.LongName = namingStrategy.GetLongName(_propertyInfo);
        Parameter.ShortName = namingStrategy.GetShortName(_propertyInfo, shortNamesToBeExcluded);

        AParameter parameter()
        {
            if (type == typeof(bool)) return new FlagParameter();
            var exampleInstance = new TTarget();
            var propertyValue = propertyInfo.GetValue(exampleInstance);
            var defaultValue = type.DefaultValue();
            return hasPropertyExplicitlyDefinedDefaultValue()
                ? new RequiredParameter { Type = type }
                : new OptionalParameter
                {
                    Type = type,
                    Default = propertyValue
                };

            bool hasPropertyExplicitlyDefinedDefaultValue()
            {
                if (propertyValue == null) return defaultValue == null;
                if (defaultValue  == null) return false;
                return propertyValue.Equals(defaultValue);
            }
        }
    }

    public AParameter Parameter { get; set; }

    public bool DoesMatch(PropertyInfo propertyInfo) =>
        /*
           a simple 
           _propertyInfo == propertyInfo
           doesn't work reliably, see https://github.com/dotnet/runtime/issues/43254
        */
        _propertyInfo.HasSameMetadataDefinitionAs(propertyInfo);

    public void SetIn(TTarget target, object value) => _propertyInfo.SetValue(target, value);
}