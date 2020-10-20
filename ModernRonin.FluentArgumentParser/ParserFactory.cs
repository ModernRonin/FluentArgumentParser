using ModernRonin.FluentArgumentParser.Definition;
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
            Create(configuration, new Services());

        /// <summary>
        ///     Use this if you want to use any of the extensibility points.
        /// </summary>
        public static IBindingCommandLineParser
            Create(ParserConfiguration configuration, Services services) =>
            Create(services,
                new CommandLineParser(services.ArgumentPreprocessor) {Configuration = configuration});

        /// <summary>
        ///     Use this if all the other overloads are not enough and you also want to customize parsing behavior itself,
        ///     beyond what <see cref="Services.ArgumentPreprocessor" /> allows you.
        /// </summary>
        public static IBindingCommandLineParser Create(Services services,
            ICommandLineParser parser) =>
            new BindingCommandLineParser(parser, new VerbFactory(services.NamingStrategy),
                new HelpAndErrorInterpreter(new HelpMaker(services.TypeFormatter,
                    services.ExampleValueProvider)));
    }
}