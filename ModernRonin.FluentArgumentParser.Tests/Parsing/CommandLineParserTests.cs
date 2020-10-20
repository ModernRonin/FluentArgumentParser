using System;
using System.Linq.Expressions;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Parsing;
using NSubstitute;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    [TestFixture]
    public class CommandLineParserTests
    {
        [SetUp]
        public void Setup()
        {
            _verbParser = Substitute.For<IVerbParser>();
            _argumentPreprocessor = Substitute.For<IArgumentPreprocessor>();
            _underTest = new CommandLineParser(_verbParser, _argumentPreprocessor)
            {
                Configuration =
                {
                    ApplicationDescription = "blabla description",
                    ApplicationName = "bla bla name"
                }
            };
            _argumentPreprocessor.Process(null).ReturnsForAnyArgs(ci => ci.Arg<string>());
        }

        IVerbParser _verbParser;
        IArgumentPreprocessor _argumentPreprocessor;
        CommandLineParser _underTest;

        Expression<Predicate<string[]>> StringArray(string expected) => a => string.Join(' ', a) == expected;
        Expression<Predicate<VerbCall>> Call(Verb verb) => c => c.Verb                           == verb;

        [TestCase(true, "drawRect", true)]
        [TestCase(true, "drawrect", false)]
        [TestCase(false, "drawRect", true)]
        [TestCase(false, "drawrect", true)]
        public void Honors_configuration_AreVerbNamesCaseSensitive(bool areVerbsCaseSensitive,
            string input,
            bool shouldItBeRecognized)
        {
            _underTest.Add(new Verb("dummy"));
            _underTest.Add(new Verb {Name = "drawRect"});
            _underTest.Configuration.AreVerbNamesCaseSensitive = areVerbsCaseSensitive;

            var result = _underTest.Parse(input.Split());
            result.IsHelpRequest.Should().Be(!shouldItBeRecognized);
        }

        [TestCase("help")]
        [TestCase("HELP")]
        [TestCase("hElP")]
        [TestCase("?")]
        public void Returns_call_to_help_verb_without_arguments_for_just_help(string input)
        {
            _underTest.DefaultVerb = new Verb();

            _underTest.Parse(input.Split()).Should().BeEquivalentTo(new VerbCall {IsHelpRequest = true});
        }

        [Test]
        public void After_construction_Configuration_is_set() =>
            _underTest.Configuration.Should().NotBeNull();

        [Test]
        public void After_construction_there_are_no_Verbs() => _underTest.Should().BeEmpty();

        [Test]
        public void After_construction_there_is_no_DefaultVerb() => _underTest.DefaultVerb.Should().BeNull();

        [Test]
        public void Calls_VerbParser_for_matched_Verb_with_remaining_arguments()
        {
            _underTest.Add(new Verb {Name = "sleep"});
            var consume = new Verb {Name = "consume"};
            _underTest.Add(consume);

            _underTest.Parse("consume food and drink".Split()).Verb.Should().BeSameAs(consume);

            _verbParser.Received()
                .Parse(_underTest.Configuration, Arg.Is(Call(consume)),
                    Arg.Is(StringArray("food and drink")));
        }

        [Test]
        public void Returns_call_of_DefaultVerb_if_it_is_set_without_any_other_Verbs()
        {
            _underTest.DefaultVerb = new Verb();

            _underTest.Parse("-x=10 -y=20".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    Verb = _underTest.DefaultVerb,
                    IsDefaultVerb = true
                });

            _verbParser.Received()
                .Parse(_underTest.Configuration,
                    Arg.Is(Call(_underTest.DefaultVerb)),
                    Arg.Is(StringArray("-x=10 -y=20")));
        }

        [Test]
        public void Returns_call_of_matching_leaf_Verb()
        {
            _underTest.Add(new Verb {Name = "sleep"});
            var leaf = new Verb {Name = "dinner"};
            _underTest.Add(new Verb("consume")
            {
                new Verb {Name = "drink"},
                new Verb("eat")
                {
                    new Verb {Name = "breakfast"},
                    new Verb {Name = "lunch"},
                    leaf
                }
            });

            _underTest.Parse("consume eat dinner hamburger chips".Split())
                .Should()
                .BeEquivalentTo(new VerbCall {Verb = leaf});

            _verbParser.Received()
                .Parse(_underTest.Configuration, Arg.Is(Call(leaf)),
                    Arg.Is(StringArray("hamburger chips")));
        }

        [Test]
        public void Returns_DefaulVerb_if_one_is_set_up_and_no_verb_is_given()
        {
            _underTest.DefaultVerb = new Verb();

            _underTest.Parse("xyz".Split()).Verb.Should().BeSameAs(_underTest.DefaultVerb);
        }

        [Test]
        public void Returns_help_request_for_specific_verb_if_called_with_help_and_arguments()
        {
            var nested = new Verb("nestedverb");
            var verb = new Verb("verb") {nested};
            _underTest.Add(verb);
            _underTest.Add(new Verb("dummy"));
            var result = _underTest.Parse("help verb nestedverb".Split());

            result.Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = nested
                });
        }

        [Test]
        public void
            Returns_help_request_if_DefaultVerb_is_not_set_and_no_other_Verb_matches_the_first_argument()
        {
            _underTest.Add(new Verb {Name = "alpha"});
            _underTest.Add(new Verb("dummy"));

            _underTest.Parse("x y z".Split())
                .Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    UnknownVerb = "x"
                });
        }

        [Test]
        public void Returns_help_request_if_no_arguments_at_all()
        {
            _underTest.DefaultVerb = new Verb();

            _underTest.Parse(Array.Empty<string>())
                .Should()
                .BeEquivalentTo(new VerbCall {IsHelpRequest = true});
        }

        [Test]
        public void
            Returns_help_request_with_info_about_unknown_verb_if_called_with_help_and_arguments_some_of_which_are_no_verbs()
        {
            var nested = new Verb("nestedverb");
            var verb = new Verb("verb") {nested};
            _underTest.Add(verb);
            _underTest.Add(new Verb("dummy"));

            var result = _underTest.Parse("help verb nestedverb x y".Split());

            result.Should()
                .BeEquivalentTo(new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = nested,
                    UnknownVerb = "x y"
                });
        }

        [Test]
        public void Uses_ArgumentPreprocessor()
        {
            _argumentPreprocessor.Process(null).ReturnsForAnyArgs(ci => $"???{ci.Arg<string>()}!!!");
            var verb = new Verb {Name = "sleep"};
            _underTest.Add(verb);
            _underTest.Add(new Verb("dummy"));

            _underTest.Parse("sleep 15 minutes".Split());

            _verbParser.Received()
                .Parse(_underTest.Configuration, Arg.Is(Call(verb)),
                    Arg.Is(StringArray("???15!!! ???minutes!!!")));
        }
    }
}