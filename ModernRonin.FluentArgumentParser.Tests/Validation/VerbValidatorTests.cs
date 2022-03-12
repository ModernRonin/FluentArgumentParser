using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class VerbValidatorTests
{
    static TestValidationResult<Verb> Test(Verb model) => new VerbValidator().TestValidate(model);

    [Test]
    public void Empty_verb_has_no_errors()
    {
        var verb = new Verb { Name = "someverb" };

        Test(verb).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void HelpText_must_not_be_null() =>
        Test(new Verb("bla") { HelpText = null }).ShouldHaveValidationErrorFor(v => v.HelpText);

    [Test]
    public void Must_not_have_indexed_parameters_where_the_smallest_index_is_greater_than_zero()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "A",
                    Type = typeof(int),
                    Index = 1
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_indexed_parameters_with_non_consecutive_indices()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "A",
                    Type = typeof(int),
                    Index = 0
                },
                new RequiredParameter
                {
                    LongName = "bravo",
                    ShortName = "B",
                    Type = typeof(int),
                    Index = 2
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_invalid_FlagParameters()
    {
        var result = Test(new Verb("bla") { Parameters = new AParameter[] { new FlagParameter() } });

        result.ShouldHaveValidationErrorFor("Parameters[0].LongName");
        result.ShouldHaveValidationErrorFor("Parameters[0].ShortName");
    }

    [Test]
    public void Must_not_have_invalid_OptionalParameters()
    {
        var result = Test(new Verb("bla") { Parameters = new AParameter[] { new OptionalParameter() } });

        result.ShouldHaveValidationErrorFor("Parameters[0].LongName");
        result.ShouldHaveValidationErrorFor("Parameters[0].ShortName");
        result.ShouldHaveValidationErrorFor("Parameters[0].Type");
        result.ShouldHaveValidationErrorFor("Parameters[0].Default");
    }

    [Test]
    public void Must_not_have_invalid_RequiredParameters()
    {
        var result = Test(new Verb("bla") { Parameters = new AParameter[] { new RequiredParameter() } });

        result.ShouldHaveValidationErrorFor("Parameters[0].LongName");
        result.ShouldHaveValidationErrorFor("Parameters[0].ShortName");
        result.ShouldHaveValidationErrorFor("Parameters[0].Type");
    }

    [Test]
    public void Must_not_have_multiple_Parameters_with_the_same_LongName()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "A",
                    Type = typeof(int)
                },
                new FlagParameter
                {
                    LongName = "alpha",
                    ShortName = "B"
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_multiple_Parameters_with_the_same_ShortName()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "B",
                    Type = typeof(int)
                },
                new FlagParameter
                {
                    LongName = "bravo",
                    ShortName = "B"
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_OptionalParameters_with_indices_before_the_last_RequiredParameter()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "A",
                    Type = typeof(int),
                    Index = 0
                },
                new RequiredParameter
                {
                    LongName = "bravo",
                    ShortName = "B",
                    Type = typeof(int),
                    Index = 2
                },
                new OptionalParameter
                {
                    LongName = "charlie",
                    ShortName = "C",
                    Type = typeof(int),
                    Default = 13,
                    Index = 1
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_Parameters_AND_Verbs()
    {
        var verb = new Verb("father") { new("kid") };
        verb.Parameters = new AParameter[]
        {
            new FlagParameter
            {
                LongName = "long",
                ShortName = "l"
            }
        };

        Test(verb).ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Must_not_have_parameters_with_duplicate_indices()
    {
        var result = Test(new Verb("bla")
        {
            Parameters = new AParameter[]
            {
                new RequiredParameter
                {
                    LongName = "alpha",
                    ShortName = "A",
                    Type = typeof(int),
                    Index = 0
                },
                new RequiredParameter
                {
                    LongName = "bravo",
                    ShortName = "B",
                    Type = typeof(int),
                    Index = 0
                }
            }
        });

        result.ShouldHaveValidationErrorFor(v => v.Parameters);
    }

    [Test]
    public void Name_must_not_be_empty() => Test(new Verb("  ")).ShouldHaveValidationErrorFor(v => v.Name);
}