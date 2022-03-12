using System;
using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Tests.Parsing;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class IndexableParameterValidatorTests
{
    [TestCase(typeof(string))]
    [TestCase(typeof(byte))]
    [TestCase(typeof(sbyte))]
    [TestCase(typeof(int))]
    [TestCase(typeof(uint))]
    [TestCase(typeof(short))]
    [TestCase(typeof(ushort))]
    [TestCase(typeof(long))]
    [TestCase(typeof(ulong))]
    [TestCase(typeof(float))]
    [TestCase(typeof(double))]
    [TestCase(typeof(decimal))]
    public void Type_can_be_any_numerical_type_or_string(Type type) =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = type })
            .ShouldNotHaveValidationErrorFor(p => p.Type);

    [Test]
    public void Index_cannot_be_negative() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter
            {
                Index = -1,
                Type = typeof(int)
            })
            .ShouldHaveValidationErrorFor(p => p.Index);

    [Test]
    public void Type_can_be_an_enum() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = typeof(Color) })
            .ShouldNotHaveValidationErrorFor(p => p.Type);

    [Test]
    public void Type_cannot_be_bool() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = typeof(bool) })
            .ShouldHaveValidationErrorFor(p => p.Type);

    [Test]
    public void Type_cannot_be_char() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = typeof(char) })
            .ShouldHaveValidationErrorFor(p => p.Type)
            .WithErrorMessage(
                "Only String, Byte, SByte, Int32, UInt32, Int16, UInt16, Int64, UInt64, Single, Double, Decimal and enums are supported for parameters.");

    [Test]
    public void Type_cannot_be_DateTime() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = typeof(DateTime) })
            .ShouldHaveValidationErrorFor(p => p.Type);

    [Test]
    public void Type_cannot_be_Object() =>
        new IndexableParameterValidator()
            .TestValidate(new RequiredParameter { Type = typeof(object) })
            .ShouldHaveValidationErrorFor(p => p.Type);
}