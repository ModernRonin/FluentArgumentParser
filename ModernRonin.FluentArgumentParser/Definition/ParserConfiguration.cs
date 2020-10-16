using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Definition
{
    public class ParserConfiguration
    {
        public string LongNamePrefix { get; set; } = "--";
        public string ShortNamePrefix { get; set; } = "-";
        public string ValueDelimiter { get; set; } = "=";
        public bool AreVerbNamesCaseSensitive { get; set; }

        public bool AreLongParameterNamesCaseSensitive { get; set; } = true;

        public bool AreShortParameterNamesCaseSensitive { get; set; }

        /// <summary>
        ///     This should be the name with which the application is called.
        /// </summary>
        public string ApplicationName { get; set; }

        public string ApplicationDescription { get; set; } = string.Empty;
        public IArgumentPreprocessor ArgumentPreprocessor { get; set; } = new NullArgumentPreprocessor();
    }
}