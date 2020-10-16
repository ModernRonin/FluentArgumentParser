using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;

namespace ModernRonin.FluentArgumentParser
{
    /// <summary>
    ///     This is the main entry point into FluentArgumentParser and allows you to create parsers with different levels of
    ///     customization.
    /// </summary>
    public static class ParserFactory
    {
        /// <summary>
        ///     Use this if you are fine with the default behavior of FluentArgumentParser.
        ///     The two parameters are both required for the generation of help texts.
        /// </summary>
        /// <param name="applicationName">
        ///     this should be what your users use to call your application, so in most cases the name of
        ///     the executable
        /// </param>
        /// <param name="applicationDescription">
        ///     this is the general description of what your tool does; empty values are not
        ///     allowed - because any good command-line tool should give their users an overview of its purpose and scope
        /// </param>
        /// <returns></returns>
        public static IBindingCommandLineParser
            Create(string applicationName, string applicationDescription) =>
            Create(new ParserConfiguration
            {
                ApplicationName = applicationName,
                ApplicationDescription = applicationDescription
            });

        /// <summary>
        ///     Use this if you want to customize things like case-sensitivity of verbs and arguments etc.
        /// </summary>
        public static IBindingCommandLineParser Create(ParserConfiguration configuration) =>
            Create(configuration, new DefaultNamingStrategy());

        /// <summary>
        ///     Use this if you want to customize how verb and short/long argument names are generated from
        ///     your POCOs.
        /// </summary>
        public static IBindingCommandLineParser Create(ParserConfiguration configuration,
            INamingStrategy namingStrategy) =>
            Create(configuration, namingStrategy, new DefaultTypeFormatter(),
                new DefaultExampleValueProvider());

        /// <summary>
        ///     Use this if you want to customize how verb and short/long argument names and help texts are generated from
        ///     your POCOs.
        /// </summary>
        public static IBindingCommandLineParser Create(ParserConfiguration configuration,
            INamingStrategy namingStrategy,
            ITypeFormatter typeFormatter,
            IExampleValueProvider exampleValueProvider) =>
            Create(namingStrategy, typeFormatter, exampleValueProvider,
                new CommandLineParser {Configuration = configuration});

        /// <summary>
        ///     Use this if all the other overloads are not enough and you also want to customize parsing behavior itself,
        ///     beyond what <see cref="ParserConfiguration.ArgumentPreprocessor" /> allows you.
        /// </summary>
        public static IBindingCommandLineParser Create(INamingStrategy namingStrategy,
            ITypeFormatter typeFormatter,
            IExampleValueProvider exampleValueProvider,
            ICommandLineParser parser) =>
            new BindingCommandLineParser(parser, new VerbFactory(namingStrategy),
                new HelpAndErrorInterpreter(new HelpMaker(typeFormatter, exampleValueProvider)));
    }
}