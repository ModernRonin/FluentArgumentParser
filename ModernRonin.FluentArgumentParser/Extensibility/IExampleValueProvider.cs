using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    /// <summary>
    ///     Implement this interface and pass it to one of the Create methods in <see cref="ParserFactory" />
    ///     if you want to customize how values for example calls in help texts are generated.
    ///     Refer to <see cref="DefaultExampleValueProvider" /> as an example for how to implement this interface.
    /// </summary>
    public interface IExampleValueProvider
    {
        /// <summary>
        ///     Generate an example value for a parameter.
        ///     <para>
        ///         The default implementation picks from a list of constants for numeric types and for strings, uses the last
        ///         label for enums and just <i>...</i> for everything else.
        ///     </para>
        /// </summary>
        object For(AParameter parameter);
    }
}