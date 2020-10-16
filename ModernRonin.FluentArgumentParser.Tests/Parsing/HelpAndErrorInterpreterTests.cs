using System;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;
using NSubstitute;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    [TestFixture]
    public class HelpAndErrorInterpreterTests
    {
        [SetUp]
        public void Setup()
        {
            _helpMaker = Substitute.For<IHelpMaker>();
            _underTest = new HelpAndErrorInterpreter(_helpMaker);
            _parser = Substitute.For<ICommandLineParser>();
            _parser.Configuration.Returns(new ParserConfiguration());

            _helpMaker.GenerateFor(_parser).Returns("overview");
            _helpMaker.GenerateFor(_verb, _parser.Configuration).Returns("verbhelp");
        }

        IHelpMaker _helpMaker;
        HelpAndErrorInterpreter _underTest;
        ICommandLineParser _parser;
        readonly Verb _verb = new Verb();

        [Test]
        public void HasError_returns_verb_help_and_errors()
        {
            _underTest.Interpret(new VerbCall
                {
                    Verb = _verb,
                    Arguments = new[]
                    {
                        new Argument {Error = "some error"},
                        new Argument {Error = "another error"}
                    }
                }, _parser)
                .Should()
                .BeEquivalentTo(new HelpResult
                {
                    Text = "verbhelp" + Environment.NewLine + "some error" + Environment.NewLine +
                           "another error",
                    IsResultOfInvalidInput = true
                });
        }

        [Test]
        public void
            IsHelpRequest_with_Verb_and_UnknownVerb_returns_verb_help_and_hint_about_unknown_nested_verb()
        {
            _underTest.Interpret(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = _verb,
                    UnknownVerb = "quaxi"
                }, _parser)
                .Should()
                .BeEquivalentTo(new HelpResult
                {
                    Text = "verbhelp" + Environment.NewLine + "Unknown verb 'quaxi'",
                    IsResultOfInvalidInput = true
                });
        }

        [Test]
        public void IsHelpRequest_with_Verb_without_UnknownVerb_returns_verb_help()
        {
            _underTest.Interpret(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = _verb
                }, _parser)
                .Should()
                .BeEquivalentTo(new HelpResult {Text = "verbhelp"});
        }

        [Test]
        public void IsHelpRequest_without_Verb_and_UnknownVerb_returns_overview()
        {
            _underTest.Interpret(new VerbCall {IsHelpRequest = true}, _parser)
                .Should()
                .BeEquivalentTo(new HelpResult {Text = "overview"});
        }

        [Test]
        public void IsHelpRequest_without_Verb_with_UnknownVerb_returns_overview_and_hint_about_unknown_verb()
        {
            _underTest.Interpret(new VerbCall
                {
                    IsHelpRequest = true,
                    UnknownVerb = "quaxi"
                }, _parser)
                .Should()
                .BeEquivalentTo(new HelpResult
                {
                    Text = "overview" + Environment.NewLine + "Unknown verb 'quaxi'",
                    IsResultOfInvalidInput = true
                });
        }
    }
}