using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.ContinuousIntegration;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Help;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Demo;

[UseReporter(typeof(NCrunchReporter))]
[TestFixture]
// ReSharper disable once TestFileNameWarning
public class HighLevelTests
{
    [Test]
    public void Fluent_Configuration_DefaultVerb()
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
    public void Fluent_Configuration_Nested_Verbs()
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
            .BeEquivalentTo(new StartFeature { Name = "MyFeature" });
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

    [Test]
    public void Unknown_Verb_results_in_Help()
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
    public void User_calls_Help_for_Verb()
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
    public void User_calls_Help_overview()
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
    public void Wrong_arguments_result_in_help_for_the_verb()
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
    public void Zero_Configuration_DefaultVerb()
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
    public void Zero_Configuration_Multiple_Verbs()
    {
        var parser = ParserFactory.Create("sometool", "somedescription");

        parser.AddVerb<PublishFeature>();
        parser.AddVerb<StartFeature>();
        parser.AddVerb<DrawRectangle>();

        parser.Parse("publishfeature MyFeature".Split())
            .Should()
            .BeOfType<PublishFeature>()
            .Which.Should()
            .BeEquivalentTo(new PublishFeature { Name = "MyFeature" });

        parser.Parse("startfeature MyFeature".Split())
            .Should()
            .BeOfType<StartFeature>()
            .Which.Should()
            .BeEquivalentTo(new StartFeature { Name = "MyFeature" });
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
    public void Zero_Configuration_Nested_Verbs()
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
            .BeEquivalentTo(new StartFeature { Name = "MyFeature" });
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
}