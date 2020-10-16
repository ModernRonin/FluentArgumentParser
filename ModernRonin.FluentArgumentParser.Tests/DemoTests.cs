using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.ContinuousIntegration;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests
{
    [UseReporter(typeof(NCrunchReporter))]
    [TestFixture]
    // ReSharper disable once TestFileNameWarning
    public class DemoTests
    {
        public enum Filling
        {
            None,
            Hatched,
            Solid
        }

        public class DrawRectangle
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public Filling Filling { get; set; } = Filling.Solid;
        }

        public class FeatureCommand
        {
            public string Name { get; set; }
        }

        public class StartFeature : FeatureCommand
        {
            public bool DontPublish { get; set; }
        }

        public class PublishFeature : FeatureCommand { }

        [Test]
        public void LowLevel_multiple_verbs_help_calls()
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

        [Test]
        public void LowLevel_MultipleVerbs_ValidCall()
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
        public void ReflectionBased_DefaultVerbOnly_ValidCall()
        {
            var parser = ParserFactory.Create("sometool", "somedescription");

            parser.DefaultVerb<DrawRectangle>();

            parser.Parse("10 11 12 13".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13
                });
            parser.Parse("10 11 -h=13 -w=12 --filling=Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("-x=10 -y=11 -h=13 -w=12 Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
        }

        [Test]
        public void ReflectionBased_MultipleVerbs_ValidCalls()
        {
            var parser = ParserFactory.Create("sometool", "somedescription");

            parser.AddVerb<PublishFeature>();
            parser.AddVerb<StartFeature>();
            parser.AddVerb<DrawRectangle>();

            parser.Parse("publishfeature MyFeature".Split())
                .Should()
                .BeOfType<PublishFeature>()
                .Which.Should()
                .BeEquivalentTo(new PublishFeature {Name = "MyFeature"});

            parser.Parse("startfeature MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature {Name = "MyFeature"});
            parser.Parse("startfeature MyFeature --dont-publish".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });
            parser.Parse("startfeature --dont-publish --name=MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });

            parser.Parse("drawrectangle 10 11 12 13".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13
                });
            parser.Parse("drawrectangle 10 11 -h=13 -w=12 --filling=Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("drawrectangle -x=10 -y=11 -h=13 -w=12 Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
        }

        [Test]
        public void ReflectionBased_NestedMultipleVerbs_ValidCalls()
        {
            var parser = ParserFactory.Create("sometool", "somedescription");

            var feature = parser.AddContainerVerb<FeatureCommand>();
            feature.AddVerb<StartFeature>();
            feature.AddVerb<PublishFeature>();
            parser.AddVerb<DrawRectangle>();

            parser.Parse("featurecommand startfeature MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature {Name = "MyFeature"});
            parser.Parse("featurecommand startfeature MyFeature --dont-publish".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });
            parser.Parse("featurecommand startfeature --dont-publish --name=MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });

            parser.Parse("drawrectangle 10 11 12 13".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13
                });
            parser.Parse("drawrectangle 10 11 -h=13 -w=12 --filling=Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("drawrectangle -x=10 -y=11 -h=13 -w=12 Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
        }

        [Test]
        public void ReflectionBased_unknown_verb_results_in_overview_help()
        {
            var parser = ParserFactory.Create(new ParserConfiguration
            {
                ApplicationName = "myapp",
                ApplicationDescription = "bla bla bla"
            });

            parser.AddVerb<PublishFeature>();
            parser.AddVerb<StartFeature>();
            parser.AddVerb<DrawRectangle>();

            var helpResult = parser.Parse("startfeatur".Split())
                .Should()
                .BeOfType<HelpResult>()
                .Which;
            helpResult.IsResultOfInvalidInput.Should().BeTrue();
            Approvals.Verify(helpResult.Text);
        }

        [Test]
        public void ReflectionBased_verb_call_with_bad_syntax_results_in_help_for_the_verb()
        {
            var parser = ParserFactory.Create(new ParserConfiguration
            {
                ApplicationName = "myapp",
                ApplicationDescription = "bla bla bla"
            });

            parser.AddVerb<PublishFeature>();
            parser.AddVerb<StartFeature>();
            parser.AddVerb<DrawRectangle>();

            var helpResult = parser.Parse("startfeature".Split())
                .Should()
                .BeOfType<HelpResult>()
                .Which;
            helpResult.IsResultOfInvalidInput.Should().BeTrue();
            Approvals.Verify(helpResult.Text);
        }

        [Test]
        public void ReflectionBased_with_fluent_configuration_call_to_general_help()
        {
            var parser = ParserFactory.Create(new ParserConfiguration
            {
                ApplicationName = "coolTool",
                ApplicationDescription = "this super-duper tool serves as a demo for the help generation"
            });

            var feature = parser.AddContainerVerb<FeatureCommand>()
                .Rename("feature")
                .WithHelp("work with feature branches");
            var startFeature = feature.AddVerb<StartFeature>()
                .Rename("start")
                .WithHelp("create a new feature branch");
            startFeature.Parameter(s => s.DontPublish).WithHelp("don't publish the branch to remote");
            startFeature.Parameter(s => s.Name).WithHelp("the name of the branch");
            feature.AddVerb<PublishFeature>()
                .Rename("publish")
                .WithHelp("create a PR from a feature branch")
                .Parameter(s => s.Name)
                .WithHelp("the name of the branch");
            var rect = parser.AddVerb<DrawRectangle>().Rename("rect").WithHelp("draw a rectangle");
            rect.Parameter(r => r.Width)
                .MakeOptional()
                .WithDefault(100)
                .WithShortName("ls")
                .WithLongName("long-side")
                .WithHelp("the long side of the rectangle");
            rect.Parameter(r => r.Height)
                .MakeOptional()
                .WithDefault(50)
                .WithShortName("ss")
                .WithLongName("short-side")
                .WithHelp("the short side of the rectangle");
            rect.Parameter(r => r.X).WithHelp("the x-coordinate");
            rect.Parameter(r => r.Y).WithHelp("the y-coordinate");
            rect.Parameter(r => r.Filling).WithHelp("how to fill the rectangle");

            var helpText = parser.Parse("help".Split())
                .Should()
                .BeOfType<HelpResult>()
                .Subject.Text;

            Approvals.Verify(helpText);
        }

        [Test]
        public void ReflectionBased_with_fluent_configuration_call_to_help_for_verb()
        {
            var parser = ParserFactory.Create(new ParserConfiguration
            {
                ApplicationName = "coolTool",
                ApplicationDescription = "this super-duper tool serves as a demo for the help generation"
            });

            var feature = parser.AddContainerVerb<FeatureCommand>()
                .Rename("feature")
                .WithHelp("work with feature branches");
            var startFeature = feature.AddVerb<StartFeature>()
                .Rename("start")
                .WithHelp("create a new feature branch");
            startFeature.Parameter(s => s.DontPublish).WithHelp("don't publish the branch to remote");
            startFeature.Parameter(s => s.Name).WithHelp("the name of the branch");
            feature.AddVerb<PublishFeature>()
                .Rename("publish")
                .WithHelp("create a PR from a feature branch")
                .Parameter(s => s.Name)
                .WithHelp("the name of the branch");
            var rect = parser.AddVerb<DrawRectangle>().Rename("rect").WithHelp("draw a rectangle");
            rect.Parameter(r => r.Width)
                .MakeOptional()
                .WithDefault(100)
                .WithShortName("ls")
                .WithLongName("long-side")
                .WithHelp("the long side of the rectangle");
            rect.Parameter(r => r.Height)
                .MakeOptional()
                .WithDefault(50)
                .WithShortName("ss")
                .WithLongName("short-side")
                .WithHelp("the short side of the rectangle");
            rect.Parameter(r => r.X).WithHelp("the x-coordinate");
            rect.Parameter(r => r.Y).WithHelp("the y-coordinate");
            rect.Parameter(r => r.Filling).WithHelp("how to fill the rectangle");

            var helpText = parser.Parse("help rect".Split())
                .Should()
                .BeOfType<HelpResult>()
                .Subject.Text;

            Approvals.Verify(helpText);
        }

        [Test]
        public void ReflectionBased_WithFluentConfiguration_DefaultVerbOnly_ValidCall()
        {
            var parser = ParserFactory.Create("sometool", "somedescription");

            var rect = parser.DefaultVerb<DrawRectangle>();
            rect.Parameter(r => r.Width)
                .MakeOptional()
                .WithDefault(100)
                .WithShortName("ls")
                .WithLongName("long-side");
            rect.Parameter(r => r.Height)
                .MakeOptional()
                .WithDefault(50)
                .WithShortName("ss")
                .WithLongName("short-side");

            parser.Parse("10 11 12 13".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13
                });
            parser.Parse("10 11 -ss=13 -ls=12 --filling=Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("-x=10 -y=11 --short-side=13 --long-side=12 Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("10 11".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 100,
                    Height = 50,
                    Filling = Filling.Solid
                });
        }

        [Test]
        public void ReflectionBased_WithFluentConfiguration_NestedMultipleVerbs_ValidCalls()
        {
            var parser = ParserFactory.Create("sometool", "somedescription");

            var feature = parser.AddContainerVerb<FeatureCommand>().Rename("feature");
            feature.AddVerb<StartFeature>().Rename("start");
            feature.AddVerb<PublishFeature>().Rename("publish");
            var rect = parser.AddVerb<DrawRectangle>().Rename("rect");
            rect.Parameter(r => r.Width)
                .MakeOptional()
                .WithDefault(100)
                .WithShortName("ls")
                .WithLongName("long-side");
            rect.Parameter(r => r.Height)
                .MakeOptional()
                .WithDefault(50)
                .WithShortName("ss")
                .WithLongName("short-side");

            parser.Parse("feature start MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature {Name = "MyFeature"});
            parser.Parse("feature start MyFeature --dont-publish".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });
            parser.Parse("feature start --dont-publish --name=MyFeature".Split())
                .Should()
                .BeOfType<StartFeature>()
                .Which.Should()
                .BeEquivalentTo(new StartFeature
                {
                    Name = "MyFeature",
                    DontPublish = true
                });

            parser.Parse("rect 10 11 12 13".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13
                });
            parser.Parse("rect 10 11 -ss=13 -ls=12 --filling=Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("rect -x=10 -y=11 --short-side=13 --long-side=12 Hatched".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 12,
                    Height = 13,
                    Filling = Filling.Hatched
                });
            parser.Parse("rect 10 11".Split())
                .Should()
                .BeOfType<DrawRectangle>()
                .Which.Should()
                .BeEquivalentTo(new DrawRectangle
                {
                    X = 10,
                    Y = 11,
                    Width = 100,
                    Height = 50,
                    Filling = Filling.Solid
                });
        }
    }
}