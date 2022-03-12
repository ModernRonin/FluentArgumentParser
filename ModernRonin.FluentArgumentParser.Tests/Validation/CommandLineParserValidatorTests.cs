using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Parsing;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class CommandLineParserValidatorTests
{
    static TestValidationResult<CommandLineParser> Test(CommandLineParser parser) =>
        new CommandLineParserValidator().TestValidate(parser);

    [Test]
    public void Configuration_must_be_valid()
    {
        var parser = new CommandLineParser();

        Test(parser)
            .Errors.Select(e => e.PropertyName)
            .Should()
            .Contain(n => n.StartsWith($"{nameof(CommandLineParser.Configuration)}."));
    }

    [Test]
    public void DefaultVerb_does_not_have_to_have_a_Name()
    {
        var parser = new CommandLineParser
        {
            Configuration =
            {
                ApplicationName = "someapp",
                ApplicationDescription = "description"
            },
            DefaultVerb = new Verb()
        };

        Test(parser).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void DefaultVerb_must_be_valid()
    {
        var parser = new CommandLineParser
        {
            Configuration =
            {
                ApplicationName = "someapp",
                ApplicationDescription = "description"
            },
            DefaultVerb = new Verb { HelpText = null }
        };

        Test(parser).Errors.Single().PropertyName.Should().Be("DefaultVerb.HelpText");
    }

    [Test]
    public void Must_have_either_DefaultVerb_or_Verbs()
    {
        var parser = new CommandLineParser
        {
            Configuration =
            {
                ApplicationName = "someapp",
                ApplicationDescription = "description"
            }
        };
        Test(parser).ShouldHaveValidationErrorFor(p => p.Verbs);
    }

    [Test]
    public void Must_not_have_DefaultVerb_AND_other_Verbs()
    {
        var parser = new CommandLineParser
        {
            Configuration =
            {
                ApplicationName = "someapp",
                ApplicationDescription = "description"
            },
            DefaultVerb = new Verb("default")
        };
        parser.Add(new Verb("other"));

        Test(parser).ShouldHaveValidationErrorFor(p => p.DefaultVerb);
    }

    [Test]
    public void Verbs_must_be_valid()
    {
        var parser = new CommandLineParser
        {
            Configuration =
            {
                ApplicationName = "someapp",
                ApplicationDescription = "description"
            }
        };
        parser.Add(new Verb());
        parser.Add(new Verb("proper"));

        Test(parser).Errors.Single().PropertyName.Should().Be("Verbs[0].Name");
    }
}