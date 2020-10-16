using System;
using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    [TestFixture]
    public class LeafVerbBindingTests
    {
        [SetUp]
        public void Setup()
        {
            // we don't use DefaultNamingStrategy to prove that the strategy is actually being used
            // we don't use nsubstitute because in this case this would actually be more work
            _namingStrategy = new TestNamingStrategy();
            _underTest = new LeafVerbBinding<SimpleTarget>(_namingStrategy);
        }

        INamingStrategy _namingStrategy;
        LeafVerbBinding<SimpleTarget> _underTest;

        class TargetWithSimilarProperties
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void Bind_Avoids_Duplicate_ShortNames()
        {
            var underTest = new LeafVerbBinding<TargetWithSimilarProperties>(_namingStrategy);
            underTest.Bind();

            var parameters = underTest.Bindings.Select(b => b.Parameter).ToArray();
            parameters.Should().HaveCount(2);
            parameters.Should()
                .BeEquivalentTo(new RequiredParameter
                {
                    Index = 0,
                    LongName = "NUMBER",
                    ShortName = "N",
                    Type = typeof(int)
                }, new RequiredParameter
                {
                    Index = 1,
                    LongName = "NAME",
                    ShortName = "NN",
                    Type = typeof(string)
                });
        }

        [Test]
        public void Bind_Fills_Bindings()
        {
            _underTest.Bind();

            var parameters = _underTest.Bindings.Select(b => b.Parameter).ToArray();
            parameters.Should().HaveCount(7);
            parameters.Should()
                .BeEquivalentTo(new RequiredParameter
                {
                    Index = 0,
                    LongName = "TEXT",
                    ShortName = "T",
                    Type = typeof(string)
                }, new RequiredParameter
                {
                    Index = 1,
                    LongName = "NUMBER",
                    ShortName = "N",
                    Type = typeof(int)
                }, new RequiredParameter
                {
                    Index = 2,
                    LongName = "REAL",
                    ShortName = "R",
                    Type = typeof(float)
                }, new RequiredParameter
                {
                    Index = 3,
                    LongName = "COLOR",
                    ShortName = "C",
                    Type = typeof(Color)
                }, new RequiredParameter
                {
                    Index = 4,
                    LongName = "INHERITEDPROPERTY",
                    ShortName = "II",
                    Type = typeof(string)
                }, new OptionalParameter
                {
                    Index = 5,
                    LongName = "OPTIONALREAL",
                    ShortName = "O",
                    Type = typeof(float),
                    Default = 3.141f
                }, new FlagParameter
                {
                    LongName = "ISON",
                    ShortName = "I"
                });
        }

        [Test]
        public void Bind_Sets_Verb_Name()
        {
            _underTest.Bind();

            _underTest.Verb.Name.Should().Be("SIMPLETARGET");
        }

        [Test]
        public void Fill_Fills_From_Call_Arguments()
        {
            _underTest.Bind();
            var parameters = _underTest.Bindings.Select(b => b.Parameter).ToDictionary(p => p.LongName);
            var call = new VerbCall
            {
                Verb = _underTest.Verb,
                Arguments = new[]
                {
                    new Argument
                    {
                        Parameter = parameters["TEXT"],
                        Value = "alpha"
                    },
                    new Argument
                    {
                        Parameter = parameters["NUMBER"],
                        Value = 13
                    },
                    new Argument
                    {
                        Parameter = parameters["REAL"],
                        Value = 3.141f
                    },
                    new Argument
                    {
                        Parameter = parameters["COLOR"],
                        Value = Color.Green
                    },
                    new Argument
                    {
                        Parameter = parameters["ISON"],
                        Value = true
                    }
                }
            };

            var instance = new SimpleTarget();
            _underTest.Fill(instance, call);

            instance.Should()
                .BeEquivalentTo(new SimpleTarget
                {
                    Text = "alpha",
                    Number = 13,
                    Real = 3.141f,
                    Color = Color.Green,
                    IsOn = true
                });
        }

        [Test]
        public void Fill_With_VerbCall_With_Other_Verb_Throws()
        {
            Action action = () => _underTest.Fill(new SimpleTarget(), new VerbCall());

            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Fill_Without_Preceding_Bind_Throws()
        {
            Action action = () => _underTest.Fill(new SimpleTarget(), new VerbCall {Verb = _underTest.Verb});

            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Integration_Usage_With_CommandLineParser()
        {
            _underTest.Bind();

            var parser = new CommandLineParser
            {
                DefaultVerb = _underTest.Verb,
                Configuration =
                {
                    ApplicationName = "bla",
                    ApplicationDescription = "blu"
                }
            };

            var call = parser.Parse("alpha 13 2.71 Green bravo -I".Split());

            var instance = new SimpleTarget();
            _underTest.Fill(instance, call);

            instance.Should()
                .BeEquivalentTo(new SimpleTarget
                {
                    Text = "alpha",
                    Number = 13,
                    Real = 2.71f,
                    Color = Color.Green,
                    IsOn = true,
                    InheritedProperty = "bravo"
                });
        }
    }
}