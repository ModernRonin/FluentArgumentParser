using System;
using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;
using NSubstitute;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    [TestFixture]
    public class BindingCommandLineParserTests
    {
        [SetUp]
        public void Setup()
        {
            // we don't use DefaultNamingStrategy to prove that the strategy is actually being used
            // we don't use nsubstitute because in this case this would actually be more work
            _namingStrategy = new TestNamingStrategy();
            _parser = Substitute.For<ICommandLineParser>();
            _helpInterpreter = Substitute.For<IHelpAndErrorInterpreter>();
            _underTest =
                new BindingCommandLineParser(_parser, new VerbFactory(_namingStrategy), _helpInterpreter);
        }

        INamingStrategy _namingStrategy;
        ICommandLineParser _parser;
        IHelpAndErrorInterpreter _helpInterpreter;
        BindingCommandLineParser _underTest;

        class Circle
        {
            public int Radius { get; set; }
        }

        class Rectangle
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        class Triangle
        {
            public int Side { get; set; }
            public int Height { get; set; }
        }

        [Test]
        public void AddVerb_Adds_Verb_To_Parser()
        {
            _underTest.AddVerb<Rectangle>();
            _underTest.AddVerb<Triangle>();

            var rect = new Verb("RECTANGLE")
            {
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        LongName = "WIDTH",
                        ShortName = "W",
                        Type = typeof(int),
                        Index = 0
                    },
                    new RequiredParameter
                    {
                        LongName = "HEIGHT",
                        ShortName = "H",
                        Type = typeof(int),
                        Index = 1
                    }
                }
            };
            var tri = new Verb
            {
                Name = "TRIANGLE",
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        LongName = "SIDE",
                        ShortName = "S",
                        Type = typeof(int),
                        Index = 0
                    },
                    new RequiredParameter
                    {
                        LongName = "HEIGHT",
                        ShortName = "H",
                        Type = typeof(int),
                        Index = 1
                    }
                }
            };
            _parser.ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(ICommandLineParser.Add))
                .Select(c => c.GetArguments().Single().As<Verb>())
                .Should()
                .BeEquivalentTo(rect, tri);
        }

        [Test]
        public void AddVerb_Returns_VerbBinding()
        {
            var rectBinding = _underTest.AddVerb<Rectangle>();
            var triBinding = _underTest.AddVerb<Triangle>();

            var receivedVerbs = _parser.ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(ICommandLineParser.Add))
                .Select(c => c.GetArguments().Single().As<Verb>())
                .ToArray();

            rectBinding.Verb.Should().BeSameAs(receivedVerbs.First());
            triBinding.Verb.Should().BeSameAs(receivedVerbs.Last());
        }

        [Test]
        public void DefaultVerb_Returns_DefaultVerb_Binding()
        {
            _underTest.DefaultVerb<Circle>().Verb.Should().BeSameAs(_parser.DefaultVerb);
        }

        [Test]
        public void DefaultVerb_Sets_DefaultVerb_Of_Parser()
        {
            _underTest.DefaultVerb<Circle>();

            _parser.DefaultVerb.Should()
                .BeEquivalentTo(new Verb
                {
                    Name = "CIRCLE",
                    Parameters = new[]
                    {
                        new RequiredParameter
                        {
                            LongName = "RADIUS",
                            ShortName = "R",
                            Type = typeof(int),
                            Index = 0
                        }
                    }
                });
        }

        [Test]
        public void HelpOverview_delegates_to_interpreter()
        {
            _helpInterpreter.GetHelpOverview(_parser).Returns("overview overview");

            _underTest.HelpOverview.Should().Be("overview overview");
        }

        [Test]
        public void Parse_Returns_Filled_Object_For_DefaultVerb()
        {
            _underTest.AddVerb<Rectangle>();
            _underTest.AddVerb<Triangle>();
            _underTest.DefaultVerb<Circle>();

            var defaultVerb = _parser.DefaultVerb;
            _parser.Parse(null)
                .ReturnsForAnyArgs(new VerbCall
                {
                    Verb = defaultVerb,
                    Arguments = new[]
                    {
                        new Argument
                        {
                            Parameter = defaultVerb.Parameters.Single(),
                            Value = 13
                        }
                    }
                });

            var actual = _underTest.Parse("bla bla".Split());

            actual.Should().BeOfType<Circle>().Which.Should().BeEquivalentTo(new Circle {Radius = 13});
        }

        [Test]
        public void Parse_Returns_Filled_Object_For_Regular_Verb()
        {
            _underTest.AddVerb<Rectangle>();
            _underTest.AddVerb<Triangle>();
            _underTest.DefaultVerb<Circle>();

            var verb = _parser.ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(ICommandLineParser.Add))
                .Select(c => c.GetArguments().Single().As<Verb>())
                .First(v => v.Name == "TRIANGLE");
            _parser.Parse(null)
                .ReturnsForAnyArgs(new VerbCall
                {
                    Verb = verb,
                    Arguments = new[]
                    {
                        new Argument
                        {
                            Parameter = verb.Parameters.Single(p => p.LongName == "SIDE"),
                            Value = 13
                        },
                        new Argument
                        {
                            Parameter = verb.Parameters.Single(p => p.LongName == "HEIGHT"),
                            Value = 17
                        }
                    }
                });

            var actual = _underTest.Parse("bla bla".Split());

            actual.Should()
                .BeOfType<Triangle>()
                .Which.Should()
                .BeEquivalentTo(new Triangle
                {
                    Side = 13,
                    Height = 17
                });
        }

        [Test]
        public void Parse_returns_HelpResult_if_helpInterpreter_does()
        {
            _underTest.AddVerb<Rectangle>();
            var call = new VerbCall();
            _parser.Parse(null).ReturnsForAnyArgs(call);
            var help = new HelpResult();
            _helpInterpreter.Interpret(call, _parser).Returns(help);

            _underTest.Parse("bla bla".Split())
                .Should()
                .BeSameAs(help);
        }

        [Test]
        public void Parse_Without_Any_Verb_Throws()
        {
            Action action = () => _underTest.Parse("bla".Split());

            action.Should().Throw<InvalidOperationException>();
        }
    }
}