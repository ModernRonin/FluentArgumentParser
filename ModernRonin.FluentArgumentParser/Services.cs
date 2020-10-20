using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser
{
    public class Services
    {
        public IExampleValueProvider ExampleValueProvider { get; set; } = new DefaultExampleValueProvider();
        public ITypeFormatter TypeFormatter { get; set; } = new DefaultTypeFormatter();
        public INamingStrategy NamingStrategy { get; set; } = new DefaultNamingStrategy();
        public IArgumentPreprocessor ArgumentPreprocessor { get; set; } = new NullArgumentPreprocessor();
    }
}