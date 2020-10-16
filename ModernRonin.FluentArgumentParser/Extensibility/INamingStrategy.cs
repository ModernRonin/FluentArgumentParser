using System;
using System.Reflection;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    /// <summary>
    ///     Implement this interface and pass it to one of the Create methods in <see cref="ParserFactory" />
    ///     if you want to customize how verb and parameter names are generated from your POCOs.
    ///     Refer to <see cref="DefaultNamingStrategy" /> as an example for how to implement this interface.
    /// </summary>
    public interface INamingStrategy
    {
        /// <summary>
        ///     Generate the long parameter name for a property.
        ///     <para>
        ///         The default implementation uses the property's name
        ///         in kebab-cases form, so for example <i>MySpecialName</i> becomes <i>my-special-name</i>.
        ///     </para>
        /// </summary>
        string GetLongName(PropertyInfo propertyInfo);

        /// <summary>
        ///     Generate the short parameter name for a property. <paramref name="shortNamesToBeExcluded" /> contains all short
        ///     names
        ///     that are already claimed by other parameters on the same verb.
        ///     <para>
        ///         The default implementation takes the first character of the property's lower-cased name that has not been used
        ///         yet. In the
        ///         (hopefully very unlikely) event that there is no such character, it will use <seealso cref="char.MinValue" />
        ///         and additional configuration
        ///         will be required to enable short-name usage.
        ///     </para>
        /// </summary>
        string GetShortName(PropertyInfo propertyInfo, string[] shortNamesToBeExcluded);

        /// <summary>
        ///     Generate the verb name for a POCO.
        ///     <para>The default implementation uses the lower-cased type's name </para>
        /// </summary>
        string GetVerbName(Type type);
    }
}