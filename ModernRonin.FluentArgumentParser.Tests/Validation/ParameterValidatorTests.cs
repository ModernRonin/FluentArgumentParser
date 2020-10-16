using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation
{
    [TestFixture]
    public class ParameterValidatorTests
    {
        [Test]
        public void HelpText_can_be_empty() =>
            new ParameterValidator().TestValidate(new FlagParameter {HelpText = ""})
                .ShouldNotHaveValidationErrorFor(p => p.HelpText);

        [Test]
        public void HelpText_cannot_be_null() =>
            new ParameterValidator().TestValidate(new FlagParameter {HelpText = null})
                .ShouldHaveValidationErrorFor(p => p.HelpText);

        [Test]
        public void LongName_cannot_be_empty() =>
            new ParameterValidator().TestValidate(new FlagParameter {LongName = "  "})
                .ShouldHaveValidationErrorFor(p => p.LongName);

        [Test]
        public void ShortName_cannot_be_empty() =>
            new ParameterValidator().TestValidate(new FlagParameter {ShortName = "  "})
                .ShouldHaveValidationErrorFor(p => p.ShortName);
    }
}