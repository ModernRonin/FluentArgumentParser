using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation
{
    [TestFixture]
    public class ParserConfigurationValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _underTest = new ParserConfigurationValidator();
        }

        ParserConfigurationValidator _underTest;

        static IEnumerable<Expression<Func<ParserConfiguration, string>>> NotEmptyCases
        {
            get
            {
                yield return c => c.ApplicationName;
                yield return c => c.ApplicationDescription;
                yield return c => c.ShortNamePrefix;
                yield return c => c.LongNamePrefix;
                yield return c => c.ValueDelimiter;
            }
        }

        [TestCaseSource(nameof(NotEmptyCases))]
        public void Cannot_be_null(Expression<Func<ParserConfiguration, string>> accessor)
        {
            var model = new ParserConfiguration();
            accessor.PropertyInfo().SetMethod.Invoke(model, new object[] {null});

            _underTest.TestValidate(model).ShouldHaveValidationErrorFor(accessor);
        }

        [TestCaseSource(nameof(NotEmptyCases))]
        public void Cannot_be_empty(Expression<Func<ParserConfiguration, string>> accessor)
        {
            var model = new ParserConfiguration();
            accessor.PropertyInfo().SetMethod.Invoke(model, new object[] {"  "});

            _underTest.TestValidate(model).ShouldHaveValidationErrorFor(accessor);
        }
    }
}