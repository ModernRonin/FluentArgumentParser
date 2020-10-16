using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Tests.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Extensibility
{
    [TestFixture]
    public class DefaultNamingStrategyTests
    {
        [TestCase("i")]
        [TestCase("s", 'i')]
        [TestCase("o", 'i', 's')]
        [TestCase("n", 'i', 's', 'o')]
        [TestCase("\0", 'i', 's', 'o', 'n')]
        public void GetShortName_Honors_ShortNamesToBeExcluded(string expected, params char[] toBeExcluded)
        {
            Expression<Func<SimpleTarget, bool>> accessor = t => t.IsOn;
            new DefaultNamingStrategy().GetShortName(accessor.PropertyInfo(),
                    toBeExcluded.Select(c => c.ToString()).ToArray())
                .Should()
                .BeEquivalentTo(expected);
        }

        [Test]
        public void GetLongName_Returns_MemberName_KebabCased()
        {
            Expression<Func<SimpleTarget, bool>> accessor = t => t.IsOn;
            new DefaultNamingStrategy().GetLongName(accessor.PropertyInfo()).Should().Be("is-on");
        }

        [Test]
        public void GetShortName_Uses_First_Letter_Of_MemberName()
        {
            Expression<Func<SimpleTarget, bool>> accessor = t => t.IsOn;
            new DefaultNamingStrategy().GetShortName(accessor.PropertyInfo(), Array.Empty<string>())
                .Should()
                .BeEquivalentTo("i");
        }

        [Test]
        public void GetVerbName_Returns_The_Types_Name_In_Lowercase()
        {
            new DefaultNamingStrategy().GetVerbName(typeof(SimpleTarget)).Should().Be("simpletarget");
        }
    }
}