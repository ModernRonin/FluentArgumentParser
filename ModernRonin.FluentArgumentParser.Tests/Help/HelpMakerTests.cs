using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.ContinuousIntegration;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;
using NSubstitute;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Help
{
    [UseReporter(typeof(NCrunchReporter))]
    [TestFixture]
    public class HelpMakerTests
    {
        enum Color
        {
            Red,
            Green,
            Blue
        }

        [Test]
        public void GenerateFor_Overview_With_Multiple_Verbs()
        {
            var parser = Substitute.For<ICommandLineParser>();
            parser.Configuration.Returns(new ParserConfiguration
            {
                ApplicationName = "cooltool",
                ApplicationDescription = "another super cool tool blablabla"
            });
            parser.GetEnumerator()
                .Returns(new List<Verb>
                {
                    new Verb
                    {
                        Name = "sing",
                        HelpText = "sing you a song"
                    },
                    new Verb
                    {
                        Name = "dance",
                        HelpText = "dance to your tune"
                    },
                    new Verb
                    {
                        Name = "explode",
                        HelpText = "explode right into your face"
                    }
                }.GetEnumerator());

            var actual = new HelpMaker().GenerateFor(parser);

            Approvals.Verify(actual);
        }

        [Test]
        public void GenerateFor_SingleVerb()
        {
            var verb = new Verb
            {
                Name = "rect",
                HelpText = "Draws a rectangle.",
                Parameters = new AParameter[]
                {
                    new RequiredParameter
                    {
                        Index = 1,
                        LongName = "y",
                        ShortName = "y",
                        Type = typeof(int),
                        HelpText = "the y-coordinate"
                    },
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "x",
                        ShortName = "x",
                        Type = typeof(int),
                        HelpText = "the x-coordinate"
                    },
                    new OptionalParameter
                    {
                        Index = 2,
                        LongName = "width",
                        ShortName = "w",
                        Type = typeof(int),
                        Default = 10,
                        HelpText = "the width"
                    },
                    new OptionalParameter
                    {
                        Index = 3,
                        LongName = "height",
                        ShortName = "h",
                        Type = typeof(int),
                        Default = 20,
                        HelpText = "the height"
                    },
                    new OptionalParameter
                    {
                        Index = 4,
                        LongName = "color",
                        ShortName = "c",
                        Type = typeof(Color),
                        Default = Color.Green,
                        HelpText = "the color to use"
                    },
                    new FlagParameter
                    {
                        LongName = "do-fill",
                        ShortName = "f",
                        HelpText = "fill the rectangle"
                    }
                }
            };
            var actual =
                new HelpMaker().GenerateFor(verb, new ParserConfiguration {ApplicationName = "cooltool"});

            Approvals.Verify(actual);
        }

        [Test]
        public void GenerateFor_SingleVerb_if_no_HelpTexts_are_set()
        {
            var verb = new Verb
            {
                Name = "rect",
                Parameters = new AParameter[]
                {
                    new RequiredParameter
                    {
                        Index = 1,
                        LongName = "y",
                        ShortName = "y",
                        Type = typeof(int)
                    },
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "x",
                        ShortName = "x",
                        Type = typeof(int)
                    },
                    new OptionalParameter
                    {
                        Index = 2,
                        LongName = "width",
                        ShortName = "w",
                        Type = typeof(int),
                        Default = 10
                    },
                    new OptionalParameter
                    {
                        Index = 3,
                        LongName = "height",
                        ShortName = "h",
                        Type = typeof(int),
                        Default = 20
                    },
                    new OptionalParameter
                    {
                        Index = 4,
                        LongName = "color",
                        ShortName = "c",
                        Type = typeof(Color),
                        Default = Color.Green
                    },
                    new FlagParameter
                    {
                        LongName = "do-fill",
                        ShortName = "f"
                    }
                }
            };
            var actual =
                new HelpMaker().GenerateFor(verb, new ParserConfiguration {ApplicationName = "cooltool"});

            Approvals.Verify(actual);
        }

        [Test]
        public void GenerateFor_SingleVerb_With_Children()
        {
            var verb = new Verb
            {
                Name = "consume",
                HelpText = "consume nutrition one way or another"
            };
            verb.Add(new Verb
            {
                Name = "eat",
                HelpText = "consume by eating",
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "dish",
                        ShortName = "d",
                        Type = typeof(string),
                        HelpText = "the dish to eat"
                    }
                }
            });
            verb.Add(new Verb
            {
                Name = "drink",
                HelpText = "consume by drinking",
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "drink",
                        ShortName = "d",
                        Type = typeof(string),
                        HelpText = "the drink to drink"
                    }
                }
            });

            var actual =
                new HelpMaker().GenerateFor(verb, new ParserConfiguration {ApplicationName = "cooltool"});

            Approvals.Verify(actual);
        }

        [Test]
        public void GenerateFor_SingleVerb_With_Children_For_Child()
        {
            var consume = new Verb
            {
                Name = "consume",
                HelpText = "consume nutrition one way or another"
            };
            consume.Add(new Verb
            {
                Name = "eat",
                HelpText = "consume by eating",
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "dish",
                        ShortName = "d",
                        Type = typeof(string),
                        HelpText = "the dish to eat"
                    }
                }
            });
            var drink = new Verb
            {
                Name = "drink",
                HelpText = "consume by drinking",
                Parameters = new[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "drink",
                        ShortName = "d",
                        Type = typeof(string),
                        HelpText = "the drink to drink"
                    }
                }
            };
            consume.Add(drink);

            var actual =
                new HelpMaker().GenerateFor(drink, new ParserConfiguration {ApplicationName = "cooltool"});

            Approvals.Verify(actual);
        }
    }
}