using System.Linq;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.ContinuousIntegration;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Demo
{
    [UseReporter(typeof(NCrunchReporter))]
    [TestFixture]
    // ReSharper disable once TestFileNameWarning
    public class LowLevelTests
    {
        [Test]
        public void Multiple_Verbs()
        {
            var feature = new Verb("feature")
            {
                new Verb
                {
                    Name = "start",
                    Parameters = new AParameter[]
                    {
                        new RequiredParameter
                        {
                            Index = 0,
                            LongName = "name",
                            ShortName = "n",
                            Type = typeof(string)
                        },
                        new FlagParameter
                        {
                            LongName = "dont-publish",
                            ShortName = "dp"
                        }
                    }
                }
            };
            var parser = new CommandLineParser
            {
                feature,
                new Verb
                {
                    Name = "rect",
                    Parameters = new AParameter[]
                    {
                        new RequiredParameter
                        {
                            Index = 0,
                            LongName = "x",
                            ShortName = "x",
                            Type = typeof(int)
                        },
                        new RequiredParameter
                        {
                            Index = 1,
                            LongName = "y",
                            ShortName = "y",
                            Type = typeof(int)
                        },
                        new RequiredParameter
                        {
                            Index = 2,
                            LongName = "width",
                            ShortName = "w",
                            Type = typeof(int)
                        },
                        new RequiredParameter
                        {
                            Index = 3,
                            LongName = "height",
                            ShortName = "h",
                            Type = typeof(int)
                        },
                        new OptionalParameter
                        {
                            Index = 4,
                            LongName = "fill",
                            ShortName = "f",
                            Type = typeof(Filling),
                            Default = Filling.None
                        }
                    }
                }
            };
            parser.Configuration.ApplicationName = "sometool";
            parser.Configuration.ApplicationDescription = "somedescription";

            var result = parser.Parse("feature start MyFeature".Split());
            result.HasError.Should().BeFalse();
            result.Arguments.Should()
                .BeEquivalentTo(new Argument
                {
                    Parameter = feature.First().Parameters.First(),
                    Value = "MyFeature",
                    WasPositionalMatch = true
                }, new Argument
                {
                    Parameter = feature.First().Parameters.Last(),
                    Value = false
                });
        }

        [Test]
        public void Multiple_Verbs_Ways_to_trigger_Help()
        {
            var startFeature = new Verb
            {
                Name = "start",
                Parameters = new AParameter[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "name",
                        ShortName = "n",
                        Type = typeof(string)
                    },
                    new FlagParameter
                    {
                        LongName = "dont-publish",
                        ShortName = "dp"
                    }
                }
            };
            var feature = new Verb("feature") {startFeature};
            var drawrectangle = new Verb
            {
                Name = "rect",
                Parameters = new AParameter[]
                {
                    new RequiredParameter
                    {
                        Index = 0,
                        LongName = "x",
                        ShortName = "x",
                        Type = typeof(int)
                    },
                    new RequiredParameter
                    {
                        Index = 1,
                        LongName = "y",
                        ShortName = "y",
                        Type = typeof(int)
                    },
                    new RequiredParameter
                    {
                        Index = 2,
                        LongName = "width",
                        ShortName = "w",
                        Type = typeof(int)
                    },
                    new RequiredParameter
                    {
                        Index = 3,
                        LongName = "height",
                        ShortName = "h",
                        Type = typeof(int)
                    },
                    new OptionalParameter
                    {
                        Index = 4,
                        LongName = "fill",
                        ShortName = "f",
                        Type = typeof(Filling),
                        Default = Filling.None
                    }
                }
            };
            var parser = new CommandLineParser
            {
                feature,
                drawrectangle
            };
            parser.Configuration.ApplicationName = "sometool";
            parser.Configuration.ApplicationDescription = "somedescription";

            parser.Parse("help".Split()).Should().BeEquivalentTo(new VerbCall {IsHelpRequest = true});

            parser.Parse("help feature".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = feature
                });

            parser.Parse("help feature start".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = startFeature
                });

            parser.Parse("help feature staxt".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = feature,
                    UnknownVerb = "staxt"
                });

            parser.Parse("drawcircle".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    UnknownVerb = "drawcircle"
                });
        }
    }
}