using System;
using System.Linq;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public class VerbCall
    {
        public Verb Verb { get; set; }
        public bool IsDefaultVerb { get; set; }
        public Argument[] Arguments { get; set; } = Array.Empty<Argument>();
        public bool IsHelpRequest { get; set; }
        public string UnknownVerb { get; set; }
        public bool HasError => Arguments.Any(a => !a.IsValid);
    }
}