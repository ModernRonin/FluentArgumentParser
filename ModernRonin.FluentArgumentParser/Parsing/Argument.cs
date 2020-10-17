using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public class Argument
    {
        public AParameter Parameter { get; set; }
        public object Value { get; set; }
        public bool WasPositionalMatch { get; set; }
        public bool IsValid => Error == default;
        public string Error { get; set; }
    }
}