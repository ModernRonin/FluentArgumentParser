using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Definition
{
    /// <summary>
    ///     This encapsulates the easiest level of customization and used in conjunction with
    ///     <see cref="ParserFactory.Create(ParserConfiguration)" />.
    /// </summary>
    public class ParserConfiguration
    {
        /// <summary>
        ///     How do we expect the user to prefix long argument names? Default: <code>--</code>
        /// </summary>
        public string LongNamePrefix { get; set; } = "--";

        /// <summary>
        ///     How do we expect the user to prefix short argument names? Default: <code>-</code>
        /// </summary>
        public string ShortNamePrefix { get; set; } = "-";

        /// <summary>
        ///     How do we expect the user to separate argument values from argument names? Default: <code>=</code>
        /// </summary>
        public string ValueDelimiter { get; set; } = "=";

        /// <summary>
        ///     Should verb names be case-sensitive? Default: <code>false</code>
        /// </summary>
        public bool AreVerbNamesCaseSensitive { get; set; }

        /// <summary>
        ///     Should long argument names be case-sensitive? Default: <code>true</code>
        /// </summary>
        public bool AreLongParameterNamesCaseSensitive { get; set; } = true;

        /// <summary>
        ///     Should short argument names be case-sensitive? Default: <code>false</code>
        /// </summary>

        public bool AreShortParameterNamesCaseSensitive { get; set; }

        /// <summary>
        ///     This should be the name with which the application is called, so typically the name of your executable.
        ///     (But for example for a global dotnet tool, it should be set to what you configured for ToolCommandName, or for
        ///     a local dotnet tool it should be <pre>dotnet &lt;ToolCommandName&gt;</pre>.)
        ///     <para>this must be set explicitly and is not optional</para>
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     A general description of what you app is used for.
        ///     <para>this must be set explicitly and is not optional</para>
        /// </summary>
        public string ApplicationDescription { get; set; } = string.Empty;

        /// <summary>
        ///     Set this if you need to do special processing of parameters before they are passed onto verbs.
        /// </summary>
        public IArgumentPreprocessor ArgumentPreprocessor { get; set; } = new NullArgumentPreprocessor();
    }
}