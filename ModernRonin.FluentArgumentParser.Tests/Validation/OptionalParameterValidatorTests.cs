using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation
{
    [TestFixture]
    public class OptionalParameterValidatorTests
    {
        [Test]
        public void Default_cannot_be_null() =>
            new OptionalParameterValidator().TestValidate(new OptionalParameter
                {
                    Type = typeof(string),
                    Default = null
                })
                .ShouldHaveValidationErrorFor(p => p.Default);
    }
}