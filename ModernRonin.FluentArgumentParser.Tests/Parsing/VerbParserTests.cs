using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    [TestFixture]
    public class VerbParserTests
    {
        [SetUp]
        public void Setup()
        {
            _cfg = new ParserConfiguration
            {
                LongNamePrefix = "//",
                ShortNamePrefix = "/",
                ValueDelimiter = ":"
            };
        }

        // different from defauls so we prove the parser actually uses the configuration values
        ParserConfiguration _cfg;
        readonly VerbParser _underTest = new VerbParser();

        VerbCall CreateCall(params AParameter[] parameters) =>
            new VerbCall {Verb = new Verb {Parameters = parameters}};

        [TestCase(typeof(string), "alpha", "alpha")]
        [TestCase(typeof(int), "13", 13)]
        [TestCase(typeof(float), "3.1", 3.1f)]
        [TestCase(typeof(Color), "Red", Color.Red)]
        [TestCase(typeof(Color), "Green", Color.Green)]
        [TestCase(typeof(Color), "Blue", Color.Blue)]
        [TestCase(typeof(Color), "2", Color.Blue)]
        [TestCase(typeof(Color), "1", Color.Green)]
        [TestCase(typeof(Color), "0", Color.Red)]
        [TestCase(typeof(string), "//value:alpha", "alpha")]
        [TestCase(typeof(int), "//value:13", 13)]
        [TestCase(typeof(float), "//value:3.1", 3.1f)]
        [TestCase(typeof(Color), "//value:Red", Color.Red)]
        [TestCase(typeof(Color), "//value:Green", Color.Green)]
        [TestCase(typeof(Color), "//value:Blue", Color.Blue)]
        [TestCase(typeof(Color), "//value:2", Color.Blue)]
        [TestCase(typeof(Color), "//value:1", Color.Green)]
        [TestCase(typeof(Color), "//value:0", Color.Red)]
        [TestCase(typeof(string), "/v:alpha", "alpha")]
        [TestCase(typeof(int), "/v:13", 13)]
        [TestCase(typeof(float), "/v:3.1", 3.1f)]
        [TestCase(typeof(Color), "/v:Red", Color.Red)]
        [TestCase(typeof(Color), "/v:Green", Color.Green)]
        [TestCase(typeof(Color), "/v:Blue", Color.Blue)]
        [TestCase(typeof(Color), "/v:2", Color.Blue)]
        [TestCase(typeof(Color), "/v:1", Color.Green)]
        [TestCase(typeof(Color), "/v:0", Color.Red)]
        [TestCase(typeof(int), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(float), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(Color), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(Color), null, null, "'value' was not given, but is required")]
        public void Parse_single_RequiredParameter(Type type,
            string input,
            object expected,
            string expectedError = default)
        {
            var verbCall = CreateCall(new RequiredParameter
            {
                Index = 0,
                LongName = "value",
                ShortName = "v",
                Type = type
            });

            _underTest.Parse(_cfg, verbCall, input == null ? Array.Empty<string>() : new[] {input});

            var arg = verbCall.Arguments.Single();
            if (expected == default) arg.Value.Should().BeNull();
            else arg.Value.Should().BeOfType(type).And.Be(expected);
            arg.IsValid.Should().Be(expectedError == default);
            arg.Error.Should().Be(expectedError);
        }

        [TestCase(typeof(string), "alpha", "alpha")]
        [TestCase(typeof(int), "13", 13)]
        [TestCase(typeof(float), "3.1", 3.1f)]
        [TestCase(typeof(Color), "Red", Color.Red)]
        [TestCase(typeof(Color), "Green", Color.Green)]
        [TestCase(typeof(Color), "Blue", Color.Blue)]
        [TestCase(typeof(Color), "2", Color.Blue)]
        [TestCase(typeof(Color), "1", Color.Green)]
        [TestCase(typeof(Color), "0", Color.Red)]
        [TestCase(typeof(string), "//value:alpha", "alpha")]
        [TestCase(typeof(int), "//value:13", 13)]
        [TestCase(typeof(float), "//value:3.1", 3.1f)]
        [TestCase(typeof(Color), "//value:Red", Color.Red)]
        [TestCase(typeof(Color), "//value:Green", Color.Green)]
        [TestCase(typeof(Color), "//value:Blue", Color.Blue)]
        [TestCase(typeof(Color), "//value:2", Color.Blue)]
        [TestCase(typeof(Color), "//value:1", Color.Green)]
        [TestCase(typeof(Color), "//value:0", Color.Red)]
        [TestCase(typeof(string), "/v:alpha", "alpha")]
        [TestCase(typeof(int), "/v:13", 13)]
        [TestCase(typeof(float), "/v:3.1", 3.1f)]
        [TestCase(typeof(Color), "/v:Red", Color.Red)]
        [TestCase(typeof(Color), "/v:Green", Color.Green)]
        [TestCase(typeof(Color), "/v:Blue", Color.Blue)]
        [TestCase(typeof(Color), "/v:2", Color.Blue)]
        [TestCase(typeof(Color), "/v:1", Color.Green)]
        [TestCase(typeof(Color), "/v:0", Color.Red)]
        [TestCase(typeof(int), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(float), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(Color), "/v:x", null, "Value for 'value' is not valid")]
        [TestCase(typeof(string), null, "bravo")]
        [TestCase(typeof(int), null, 7337)]
        [TestCase(typeof(float), null, 2.71f)]
        [TestCase(typeof(Color), null, Color.Green)]
        public void Parse_single_OptionalParameter(Type type,
            string input,
            object expected,
            string expectedError = default)
        {
            var verbCall = CreateCall(new OptionalParameter
            {
                Index = 0,
                LongName = "value",
                ShortName = "v",
                Type = type,
                Default = getDefault()
            });

            _underTest.Parse(_cfg, verbCall, input == null ? Array.Empty<string>() : new[] {input});

            var arg = verbCall.Arguments.Single();
            if (expected == default) arg.Value.Should().BeNull();
            else arg.Value.Should().BeOfType(type).And.Be(expected);
            arg.IsValid.Should().Be(expectedError == default);
            arg.Error.Should().Be(expectedError);

            object getDefault()
            {
                if (typeof(string) == type) return "bravo";
                if (typeof(int)    == type) return 7337;
                if (typeof(float)  == type) return 2.71f;
                if (typeof(Color)  == type) return Color.Green;
                throw new NotImplementedException();
            }
        }

        [TestCase("//flag", true)]
        [TestCase("/f", true)]
        [TestCase(null, false)]
        public void Parse_single_FlagParameter_when_it_is_given(string input, bool expected)
        {
            var verbCall = CreateCall(new FlagParameter
            {
                LongName = "flag",
                ShortName = "f"
            });

            _underTest.Parse(_cfg, verbCall, input == null ? Array.Empty<string>() : new[] {input});

            verbCall.Arguments.Single().Value.Should().Be(expected);
        }

        [TestCase("/x:20", "x")]
        [TestCase("//xyz:20", "xyz")]
        public void Parse_unknown_shortnamed_argument(string input, string variable)
        {
            var verbCall = CreateCall(new RequiredParameter
            {
                Index = 0,
                ShortName = "v",
                LongName = "value",
                Type = typeof(int)
            });

            _underTest.Parse(_cfg, verbCall, new[] {input});

            verbCall.Arguments.Select(a => a.IsValid).Should().AllBeEquivalentTo(false);
            verbCall.Arguments.Should().HaveCount(2);
            verbCall.Arguments.Select(a => a.Error)
                .Should()
                .BeEquivalentTo($"unknown argument '{variable}'", "'value' was not given, but is required");
        }

        static IEnumerable<TestCaseData> MultipleParameterCases
        {
            get
            {
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "300",
                    "400",
                    "Green",
                    "/f"
                }, 100, 200, 300, 400, Color.Green, true);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "300",
                    "400",
                    "Green"
                }, 100, 200, 300, 400, Color.Green, false);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "300",
                    "400"
                }, 100, 200, 300, 400, Color.Blue, false);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "300"
                }, 100, 200, 300, 17, Color.Blue, false);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200"
                }, 100, 200, 13, 17, Color.Blue, false);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "300",
                    "400",
                    "/c:Green",
                    "/f"
                }, 100, 200, 300, 400, Color.Green, true);
                yield return new TestCaseData(new[]
                {
                    "100",
                    "200",
                    "/f",
                    "/x:300",
                    "//y-coordinate:400",
                    "/c:Green"
                }, 100, 200, 300, 400, Color.Green, true);
                yield return new TestCaseData(new[]
                {
                    "/x:300",
                    "//y-coordinate:400",
                    "//width:100",
                    "//height:200",
                    "/f",
                    "/c:Green"
                }, 100, 200, 300, 400, Color.Green, true);
            }
        }

        [TestCaseSource(nameof(MultipleParameterCases))]
        public void Parse_multiple_parameters_of_each_kind(string[] inputs,
            int expectedWidth,
            int expectedHeight,
            int expectedX,
            int expectedY,
            Color expectedColor,
            bool expectedDoFill)
        {
            var verbCall = CreateCall(new RequiredParameter
            {
                Index = 0,
                LongName = "width",
                ShortName = "w",
                Type = typeof(int)
            }, new RequiredParameter
            {
                Index = 1,
                LongName = "height",
                ShortName = "h",
                Type = typeof(int)
            }, new OptionalParameter
            {
                Index = 2,
                LongName = "x-coordinate",
                ShortName = "x",
                Type = typeof(int),
                Default = 13
            }, new OptionalParameter
            {
                Index = 3,
                LongName = "y-coordinate",
                ShortName = "y",
                Type = typeof(int),
                Default = 17
            }, new OptionalParameter
            {
                Index = 4,
                LongName = "color",
                ShortName = "c",
                Type = typeof(Color),
                Default = Color.Blue
            }, new FlagParameter
            {
                LongName = "doFill",
                ShortName = "f"
            });

            _underTest.Parse(_cfg, verbCall, inputs);

            verbCall.Arguments.Should().HaveCount(6);
            verbCall.Arguments.Select(a => a.IsValid).Should().AllBeEquivalentTo(true);
            verbCall.Arguments.Single(a => a.Parameter.LongName == "width").Value.Should().Be(expectedWidth);
            verbCall.Arguments.Single(a => a.Parameter.LongName == "height")
                .Value.Should()
                .Be(expectedHeight);
            verbCall.Arguments.Single(a => a.Parameter.ShortName == "x").Value.Should().Be(expectedX);
            verbCall.Arguments.Single(a => a.Parameter.ShortName == "y").Value.Should().Be(expectedY);
            verbCall.Arguments.Single(a => a.Parameter.LongName  == "color").Value.Should().Be(expectedColor);
            verbCall.Arguments.Single(a => a.Parameter.LongName == "doFill")
                .Value.Should()
                .Be(expectedDoFill);
        }

        [TestCase("unknown argument 'x'", "/w:10", "/h:11", "/x:7")]
        [TestCase("too many arguments - don't know what to do with '7'", "/w:10", "/h:11", "7")]
        [TestCase("too many arguments - don't know what to do with '7'", "10", "/h:11", "7")]
        [TestCase("too many arguments - don't know what to do with '7'", "10", "11", "7")]
        public void Parse_when_too_many_arguments_are_given(string error, params string[] inputs)
        {
            var verbCall = CreateCall(new RequiredParameter
            {
                Index = 0,
                LongName = "width",
                ShortName = "w",
                Type = typeof(int)
            }, new RequiredParameter
            {
                Index = 1,
                LongName = "height",
                ShortName = "h",
                Type = typeof(int)
            });

            _underTest.Parse(_cfg, verbCall, inputs);

            verbCall.Arguments.Should().HaveCount(3);
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "w").IsValid.Should().BeTrue();
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "w").Value.Should().Be(10);
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "h").IsValid.Should().BeTrue();
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "h").Value.Should().Be(11);
            verbCall.Arguments.Single(a => a.Parameter             == default).IsValid.Should().BeFalse();
            verbCall.Arguments.Single(a => a.Parameter == default)
                .Error.Should()
                .Be(error);
        }

        [TestCase("//width:100", "/h:200", "//width:100")]
        [TestCase("100", "/h:200", "//width:100")]
        [TestCase("/w:100", "/h:200", "//width:100")]
        [TestCase("/w:100", "/h:200", "/w:100")]
        [TestCase("//width:100", "/h:200", "/w:100")]
        [TestCase("100", "/h:200", "/w:100")]
        public void Parse_when_duplicate_arguments_are_given(params string[] inputs)
        {
            var verbCall = CreateCall(new RequiredParameter
            {
                Index = 0,
                LongName = "width",
                ShortName = "w",
                Type = typeof(int)
            }, new RequiredParameter
            {
                Index = 1,
                LongName = "height",
                ShortName = "h",
                Type = typeof(int)
            });

            _underTest.Parse(_cfg, verbCall, inputs);

            verbCall.Arguments.Should().HaveCount(3);
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "h").IsValid.Should().BeTrue();
            verbCall.Arguments.Single(a => a?.Parameter?.ShortName == "h").Value.Should().Be(200);
            verbCall.Arguments.Where(a => a?.Parameter?.ShortName  == "w").Should().HaveCount(2);
            verbCall.Arguments.Where(a => a?.Parameter?.ShortName == "w")
                .Select(a => a.IsValid)
                .Should()
                .AllBeEquivalentTo(false);
            verbCall.Arguments.Where(a => a?.Parameter?.ShortName == "w")
                .Select(a => a.Error)
                .Distinct()
                .Single()
                .Should()
                .Be("The same argument is given multiple times");
        }

        [TestCase(true, "//someTerm:10", true)]
        [TestCase(true, "//someterm:10", false)]
        [TestCase(false, "//someTerm:10", true)]
        [TestCase(false, "//someterm:10", true)]
        public void Parse_honors_configuration_AreLongParameterNamesCaseSensitive(
            bool areLongNamesCaseSensitive,
            string input,
            bool shouldBeRecognized)
        {
            _cfg.AreLongParameterNamesCaseSensitive = areLongNamesCaseSensitive;

            var verbCall = CreateCall(new RequiredParameter
            {
                LongName = "someTerm",
                ShortName = "s",
                Type = typeof(int)
            });

            _underTest.Parse(_cfg, verbCall, input.Split());

            verbCall.Arguments.Single(a => a?.Parameter?.LongName == "someTerm")
                .IsValid.Should()
                .Be(shouldBeRecognized);
        }

        [TestCase(true, "/s:10", true)]
        [TestCase(true, "/S:10", false)]
        [TestCase(false, "/s:10", true)]
        [TestCase(false, "/S:10", true)]
        public void Parse_honors_configuration_AreShortParameterNamesCaseSensitive(
            bool areShortNamesCaseSensitive,
            string input,
            bool shouldBeRecognized)
        {
            _cfg.AreShortParameterNamesCaseSensitive = areShortNamesCaseSensitive;

            var verbCall = CreateCall(new RequiredParameter
            {
                LongName = "someTerm",
                ShortName = "s",
                Type = typeof(int)
            });

            _underTest.Parse(_cfg, verbCall, input.Split());

            verbCall.Arguments.Single(a => a?.Parameter?.LongName == "someTerm")
                .IsValid.Should()
                .Be(shouldBeRecognized);
        }
    }
}