using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility;

/// <summary>
///     Implement this interface and pass it to one of the Create methods in <see cref="ParserFactory" />
///     if you want to customize how the types of parameters are formatted in help texts.
///     Refer to <see cref="DefaultTypeFormatter" /> as an example for how to implement this interface.
/// </summary>
public interface ITypeFormatter
{
    /// <summary>
    ///     Generate the name to display for this type when displaying help for required or optional parameters.
    ///     <para>
    ///         Note that you don't have to worry about <see cref="bool" /> types because they are handled by flag parameters
    ///         and there is no type displayed for them in help texts.
    ///     </para>
    ///     <para>
    ///         The default implementation uses
    ///         <list type="bullet">
    ///             <item>
    ///                 <term>for standard types like <see cref="int" /> or <see cref="string" /></term>
    ///                 <description>the C# type name</description>
    ///             </item>
    ///             <item>
    ///                 <term>for enum values</term>
    ///                 <description>a list of their valid string values</description>
    ///             </item>
    ///             <item>
    ///                 <term>for everything else</term>
    ///                 <description>the type's name</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    string Format(AParameter parameter);
}