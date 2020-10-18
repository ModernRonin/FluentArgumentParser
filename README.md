# FluentArgumentParser

> **⚠ Will soon be published, come back in a few days...**
<!--
[![NuGet](https://img.shields.io/nuget/v/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser/)
[![NuGet](https://img.shields.io/nuget/dt/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser)
-->

## Summary
There are several packages out there for parsing of command-line arguments, but not one of them fulfills everything I think such a library should cover. So I just created another one ;-)

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
| `10 11 12 13`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Solid `  |
| `10 11 -h=13 -w=12 --filling=Hatched`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |
| `-x=10 -y=11 -h=13 -w=12 Hatched`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |

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


## Customizability

## Extensibility

### Release History

