using FluentAssertions;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("alpha", "", "alpha")]
        [TestCase("alpha", "al", "pha")]
        [TestCase("alpha", "alph", "a")]
        [TestCase("alpha", "alpha", "")]
        [TestCase("alpha", "l", "pha")]
        [TestCase("alpha", "lp", "ha")]
        [TestCase("alpha", "lph", "a")]
        [TestCase("alpha", "h", "a")]
        [TestCase("alpha", "ha", "")]
        public void After(string input, string marker, string expected) =>
            input.After(marker).Should().Be(expected);

        [TestCase("alpha", "p", "al")]
        [TestCase("alpha", "ph", "al")]
        [TestCase("alpha", "pha", "al")]
        [TestCase("alpha", "phan", "alpha")]
        [TestCase("alpha", "x", "alpha")]
        public void Before(string input, string marker, string expected) =>
            input.Before(marker).Should().Be(expected);
        
        [TestCase("", "")]
        [TestCase("x", "x")]
        [TestCase("twoWords", "two-words")]
        [TestCase("aLotMoreWords", "a-lot-more-words")]
        public void KebabCased(string input, string expected)
        {
            input.KebabCased().Should().Be(expected);
        }
    }
}