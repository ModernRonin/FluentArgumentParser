# FluentArgumentParser
[![NuGet](https://img.shields.io/nuget/v/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser/)
[![NuGet](https://img.shields.io/nuget/dt/ModernRonin.FluentArgumentParser.svg)](https://www.nuget.org/packages/ModernRonin.FluentArgumentParser)

## Summary
There are several packages out there for parsing of command-line arguments, but not one of them fulfills everything I like to see in such a library. So I just created another one ;-)

So what are these requirements?

* populates POCOs
* is not dependent on attributes (sorry, personally I find attributes, especially those with parameters, create a lot of noise when reading code)
* deals with verbs, like git, and nested verbs, too
* allows passing arguments by index, by long name and by short name
* produces good-looking and useful help
* possible to work with just the POCOs and not further configuration - for small in-house tools one often doesn't want to spend a lot of time with setting up these options
* good defaults, but at the same time configurable and extensible

## Quick Starts
The usual preamble: install `ModernRonin.FluentArgumentParser` from nuget.

### Zero Configuration, just arguments
You want to model a single action and don't care too much about the names of the options or help-text, you just want to get over this argument parsing as quickly as possible.


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
| ------------- | ------------- |
| `10 11 12 13`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Solid `  |
| `10 11 -h=13 -w=12 --filling=Hatched`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |
| `-x=10 -y=11 -h=13 -w=12 Hatched`  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |



## Customizability

## Extensibility

### Release History

