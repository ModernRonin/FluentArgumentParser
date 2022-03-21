using FluentValidation.TestHelper;

using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;

using NUnit.Framework;

using System;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class OptionalParameterValidatorTests
{
    [Test]
    public void HasDefaultBeenSet_must_be_true() =>
        new OptionalParameterValidator()
        .TestValidate(new OptionalParameter { Type = typeof(string) })
            .ShouldHaveValidationErrorFor(p => p.Default);

    [Test]
    public void Description_Should_Have_Error_When_Description_Empty_For_Reference_Type() =>
        new OptionalParameterValidator()
            .TestValidate(new OptionalParameter { Type = typeof(string) })
            .ShouldHaveValidationErrorFor(p => p.Description);

    [Test]
    public void Description_Should_Not_Have_Error_When_Description_Empty_For_Value_Type() =>
        new OptionalParameterValidator()
            .TestValidate(new OptionalParameter { Type = typeof(int) })
            .ShouldHaveValidationErrorFor(p => p.Description);
}