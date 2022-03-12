# FluentArgumentParser

[![.NET](https://github.com/ModernRonin/FluentArgumentParser/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ModernRonin/FluentArgumentParser/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser/)
[![NuGet](https://img.shields.io/nuget/dt/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser)

- [Summary](#summary)
- [Quickstart](#quick-start---zero-configuration)
- [More Info](#more-info)
- [License](#license)
- [Contributing](#contributing)
- [Release History](docs/ReleaseHistory.md)

## Summary
There are several packages out there for parsing of command-line arguments, but not one of them fulfills everything I think such a library should cover. 

What are these requirements?

* populates POCOs
* is not dependent on attributes (personally I find attributes, especially those with parameters, create a lot of noise when reading code)
* deals with verbs, like git, and nested verbs, too
* allows passing arguments by index, by long name and by short name
* produces good-looking and useful help
* possible to work with just the POCOs without any further configuration - for small in-house tools one often doesn't want to spend a lot of time with setting up these options
* good defaults, but at the same time configurable and extensible

## Quick Start - zero configuration
Let's look at the simplest scenario. For more advanced examples, take a look [here](docs/Examples.md).


>You want to model a single action and don't care too much about the names of the options or help-text, you just want to get over this argument parsing as quickly as possible.


```csharp
// this is the info you want to get from the commandline arguments - you just define it as a regular POCO
public class Rectangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Filling Filling { get; set; } = Filling.Solid;
}

// and in your Main you do:
static int Main(string[] args)
{
    var parser = ParserFactory.Create("sometool", "somedescription");
    parser.DefaultVerb<Rectangle>();
    switch (parser.Parse(args))
    {
        case HelpResult help:
            Console.WriteLine(help.Text);
            return help.IsResultOfInvalidInput ? -1 : 0;
        case Rectangle rectangle:
            // do whatever you need to do
    }
}
```
Now what are valid inputs for this setup? Here are a few examples, together with how they will fill the properties of the `Rectangle` instance:
| Argument string  | Rectangle properties |
| --- | --- |
| 10 11 12 13  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Solid `  |
| 10 11 -h=13 -w=12 --filling=Hatched  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |
| -x=10 -y=11 -h=13 -w=12 Hatched  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |

And what would be the content of the `help.Text` property in the code-sample above?

```plaintext
sometool
somedescription

Usage:
sometool --x=<value> --y=<value> --width=<value> --height=<value> [--filling=<value>]


Required arguments:
--x, -x       int
--y, -y       int
--width, -w   int
--height, -h  int

Optional arguments:
--filling, -f  None, Hatched, Solid
               default: Solid

Examples:
sometool 10 20 30 40
sometool 10 20 30 40 Solid
sometool --x=10 --y=20 --width=30 --height=40 --filling=Solid
sometool -x=10 -y=20 -w=30 -h=40 -f=Solid
sometool -h=40 -f=Solid -x=10 -y=20 -w=30

long parameter names are case-sensitive
short parameter names are not case-sensitive
command names are not case-sensitive
```

Things to note:
- if you are fine with the default naming, you can just pop in your configuration objects and be done with it.
- if you set defaults for properties (different from the standard defaults), they are automatically assumed to be optional with the default value you set. In the example above `Rectangle.Filling` is such a case: it's automatically understood to be an optional parameter with the default value `Filling.Solid`.
- the `Parse` method returns an object on which you can switch, using a language facility we've had now for a while. The types you need to handle are all your own POCOs that you have defined as verbs and the special type `HelpResult`
- `HelpResult` is automatically returned when the arguments contain an explicit call for help, like `sometool help` or `sometool ?` or, if you have multiple verbs, `sometool help myverb`

## More Info
- [multiple verbs](docs/Examples.md#multiple-verbs-no-configuration)
- [nested verbs](docs/Examples.md#multiple-and-nested-verbs-no-configuration)
- [boolean properties](docs/Reference.md#parameters)
- [verb specfic help](docs/Examples.md#help-startfeature)
- overriding the automatically detected settings (like the name of a parameter)
see [More Examples](docs/Examples.md#multiple-and-nested-verbs-with-additional-configuration).
- change [global configuration](docs/Configuration.md), for example the prefixes uses for parameter names
- change how [names are generated](docs/Extensibility.md#name-generation) for verbs and parameters
- change how [types are formatted](docs/Extensibility.md#help-generation) for the help texts
- change how [example values](docs/Extensibility.md#help-generation) are generated for the help texts
- [pre-process](docs/Reference.md#iargumentpreprocessor) arguments
- if you want to interact with the low-level parser, without any reflection, check out [these tests](ModernRonin.FluentArgumentParser.Tests/Demo/LowLevelTests.cs)
- last not least, you can always take a look at the [demo tests](ModernRonin.FluentArgumentParser.Tests/Demo)

## Contributing
If you want to contribute, you are more than welcome. Check out the issues page. There are a few issues marked as `good-first-issue` and a few others as `help-wanted`. 

Furthermore, issues marked as `needs-design` would benefit from discussion about how they should ideally work, both for end-users and for library-users.

Last not least, there are a few ideas marked as `do-people-need-this`- for these, it would be very interesting to get as many opinions as possible. 

## License
The [license](./LICENSE) is [Creative Commons BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/). In essence this means you are free to use and distribute and change this tool however you see fit, as long as you provide a link to the license
and share any customizations/changes you might perform under the same license. 

