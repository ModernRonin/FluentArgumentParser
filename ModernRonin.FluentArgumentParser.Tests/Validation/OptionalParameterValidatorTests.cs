using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class OptionalParameterValidatorTests
{
    [Test]
    public void HasDefaultBeenSet_must_be_true() =>
        new OptionalParameterValidator().TestValidate(new OptionalParameter { Type = typeof(string) })
            .ShouldHaveValidationErrorFor(p => p.Default);
}