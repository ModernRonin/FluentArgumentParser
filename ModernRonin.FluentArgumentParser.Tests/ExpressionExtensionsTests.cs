using System;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests
{
    [TestFixture]
    public class ExpressionExtensionsTests
    {
        class Record
        {
            public int Number { get; set; }
        }

        [Test]
        public void Membername()
        {
            Expression<Func<Record, int>> expression = r => r.Number;

            expression.MemberName().Should().Be("Number");
        }

        [Test]
        public void PropertyInfo()
        {
            Expression<Func<Record, int>> expression = r => r.Number;

            expression.PropertyInfo().GetValue(new Record {Number = 13}).Should().Be(13);
        }
    }
}