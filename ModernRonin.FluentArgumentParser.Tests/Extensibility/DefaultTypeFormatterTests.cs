using System;
using System.Text;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Tests.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Extensibility
{
    [TestFixture]
    public class DefaultTypeFormatterTests
    {
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(byte), "byte")]
        [TestCase(typeof(sbyte), "sbyte")]
        [TestCase(typeof(int), "int")]
        [TestCase(typeof(uint), "uint")]
        [TestCase(typeof(short), "short")]
        [TestCase(typeof(ushort), "ushort")]
        [TestCase(typeof(long), "long")]
        [TestCase(typeof(ulong), "ulong")]
        [TestCase(typeof(float), "float")]
        [TestCase(typeof(double), "double")]
        [TestCase(typeof(decimal), "decimal")]
        public void Format_returns_the_CSharp_name_for_all_supported_standard_types(Type type,
            string expected)
        {
            Test(type, expected);
        }

        static void Test(Type type, string expected) =>
            new DefaultTypeFormatter().Format(new RequiredParameter {Type = type}).Should().Be(expected);

        [Test]
        public void Format_returns_list_of_labels_for_Enums()
        {
            Test(typeof(Color), "Red, Green, Blue");
        }

        [Test]
        public void Format_returns_type_name_for_all_other_types()
        {
            Test(typeof(StringBuilder), "StringBuilder");
        }
    }
}