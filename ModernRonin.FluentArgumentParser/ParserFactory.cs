using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;

namespace ModernRonin.FluentArgumentParser
{
    public static class ParserFactory
    {
        public static BindingCommandLineParser
            Create(string applicationName, string applicationDescription) =>
            Create(new ParserConfiguration
            {
                ApplicationName = applicationName,
                ApplicationDescription = applicationDescription
            });

        public static BindingCommandLineParser Create(ParserConfiguration configuration) =>
            Create(configuration, new DefaultNamingStrategy());

        public static BindingCommandLineParser Create(ParserConfiguration configuration,
            INamingStrategy namingStrategy) =>
            Create(configuration, namingStrategy, new DefaultTypeFormatter(),
                new DefaultExampleValueProvider());

        public static BindingCommandLineParser Create(ParserConfiguration configuration,
            INamingStrategy namingStrategy,
            ITypeFormatter typeFormatter,
            IExampleValueProvider exampleValueProvider) =>
            Create(namingStrategy, typeFormatter, exampleValueProvider,
                new CommandLineParser {Configuration = configuration});

        public static BindingCommandLineParser Create(INamingStrategy namingStrategy,
            ITypeFormatter typeFormatter,
            IExampleValueProvider exampleValueProvider,
            ICommandLineParser parser) =>
            new BindingCommandLineParser(parser, new VerbFactory(namingStrategy),
                new HelpAndErrorInterpreter(new HelpMaker(typeFormatter, exampleValueProvider)));
    }
}