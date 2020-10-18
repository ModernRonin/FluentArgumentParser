# Examples
FluentArgumentParser allows you to gradually change your tradeoff between complexity and customization. The following examples follow along this trajectory, moving from minimal effort (and minimal customization) to  high effort (comparably). 


You can also check out the [demo tests](../ModernRonin.FluentArgumentParser.Tests/Demo).

All examples use the following POCOs representing configuration objects in your domain:
```csharp
public class DrawRectangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Filling Filling { get; set; } = Filling.Solid;
}

public class FeatureCommand
{
    public string Name { get; set; }
}

public class StartFeature : FeatureCommand
{
    public bool DontPublish { get; set; }
}

public class PublishFeature : FeatureCommand { }
```

## No verbs, no configuration
>You want to model a single action and don't care too much about the names of the options or help-text, you just want to get over this argument parsing as quickly as possible.

### Code
```csharp
static int Main(string[] args)
{
    var parser = ParserFactory.Create("sometool", "somedescription");
    parser.DefaultVerb<DrawRectangle>();
    switch (parser.Parse(args))
    {
        case HelpResult help:
            Console.WriteLine(help.Text);
            return help.IsResultOfInvalidInput ? -1 : 0;
        case DrawRectangle rectangle:
            // do whatever you need to do
    }
}
```
### Inputs and results
| Argument string  | DrawRectangle properties |
| --- | --- |
| 10 11 12 13  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Solid `  |
| 10 11 -h=13 -w=12 --filling=Hatched  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |
| -x=10 -y=11 -h=13 -w=12 Hatched  | `X:10, Y:11, Width:12, Height:13, Filling:Filling.Hatched `  |

### Generated help
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

## Multiple verbs, no configuration
> It turns out you need more than one action, but you still don't care too much about the names of the options or help-text.

### Code
```csharp
static int Main(string[] args)
{
    var parser = ParserFactory.Create("sometool", "somedescription");
    parser.AddVerb<PublishFeature>();
    parser.AddVerb<StartFeature>();
    parser.AddVerb<DrawRectangle>();

    switch (parser.Parse(args))
    {
        case HelpResult help:
            Console.WriteLine(help.Text);
            return help.IsResultOfInvalidInput ? -1 : 0;
        case DrawRectangle rectangle:
            // do whatever you need to do
        case PublishFeature publish:
            // do whatever you need to do
        case StartFeature start:
            // do whatever you need to do
    }
}
```
### Inputs and results
| Argument string  | Return value of Parse |
| --- | --- |
| publishfeature MyFeature | `new PublishFeature { Name = "MyFeature"}`  |
| startfeature MyFeature | `new StartFeature { Name = "MyFeature"}`  |
| startfeature MyFeature --dont-publish | `new StartFeature { Name = "MyFeature", DontPublish = true}`  |
| startfeature --dont-publish --name=MyFeature | `new StartFeature { Name = "MyFeature", DontPublish = true}` |
| drawrectangle 10 11 12 13 | `new DrawRectangle  { X = 10, Y = 11, Width = 12, Height = 13}` |
| help |`HelpResult` with overview help|
| ? |`HelpResult` with overview help|
| help startfeature | `HelpResult` with help for `StartFeature` |

### Generated help
#### `help`
```plaintext
sometool
somedescription

Usage:
sometool publishfeature | startfeature | drawrectangle [command specific options]

Available commands:
publishfeature  
startfeature    
drawrectangle   

use help <command> for more detailed help on a specific command

long parameter names are case-sensitive
short parameter names are not case-sensitive
command names are not case-sensitive
```
#### `help startfeature`
```plaintext
sometool startfeature --name=<value> [--dont-publish]


Required arguments:
--name, -n  string

Flags:
--dont-publish, -d  

Examples:
sometool startfeature alpha
sometool startfeature alpha --dont-publish
sometool startfeature --name=alpha --dont-publish
sometool startfeature -n=alpha -d
```

## Multiple and nested verbs, no configuration
> Now you need some of your actions/commands/verbs to form a nested hierarchy, but you still don't care too much about the names of the options or help-text.


### Code
```csharp
static int Main(string[] args)
{
    var parser = ParserFactory.Create("sometool", "somedescription");
    var feature = parser.AddContainerVerb<FeatureCommand>();
    feature.AddVerb<StartFeature>();
    feature.AddVerb<PublishFeature>();
    parser.AddVerb<DrawRectangle>();

    switch (parser.Parse(args))
    {
        case HelpResult help:
            Console.WriteLine(help.Text);
            return help.IsResultOfInvalidInput ? -1 : 0;
        case DrawRectangle rectangle:
            // do whatever you need to do
        case PublishFeature publish:
            // do whatever you need to do
        case StartFeature start:
            // do whatever you need to do
    }
}
```
A few notes:
- verbs can be arbitratily nested, even though the example shows just one level of nesting.
- you don't need to match for `FeatureCommand` because it is not an actual command, just a container for commands
- as you can see in the definitions of the POCOs further above, `FeatureCommand` happens to be a common base class of `StartFeature` and `PublishFeature`. This is not required and just coincidence. What is required, however, that all base-classes of your verbs are concrete (not `abstract`) and default-constructable. The reason for this has to do with how we detect default values. 

### Inputs and results
| Argument string  | Return value of Parse |
| --- | --- |
| featurecommand startfeature MyFeature | `new StartFeature {Name = "MyFeature"}` |
| drawrectangle 10 11 12 13 | `new DrawRectangle  { X = 10, Y = 11, Width = 12, Height = 13}` |

As you can see, we are now entering territory where the automatically generated names don't look very nice. Instead of 'featurecommand startfeature MyFeature', users probably want to enter 'feature start MyFeature'. 

There are two ways how you can achieve that: one is to just rename your types accordingly (`FeatureCommand` ==> `Feature` and `StartFeature` ==> `Start`). The alternative will be shown in the examples with configuration further down.

### Help
#### `help`
```plaintext
sometool
somedescription

Usage:
sometool featurecommand | drawrectangle [command specific options]

Available commands:
featurecommand  
drawrectangle   

use help <command> for more detailed help on a specific command

long parameter names are case-sensitive
short parameter names are not case-sensitive
command names are not case-sensitive
```
#### `help featurecommand`
```plaintext
sometool featurecommand startfeature | publishfeature [command specific options]


Available commands:
startfeature    
publishfeature  

use help <command> for more detailed help on a specific command
```
#### `help featurecommand startfeature`
```plaintext
sometool featurecommand startfeature --name=<value> [--dont-publish]


Required arguments:
--name, -n  string

Flags:
--dont-publish, -d  

Examples:
sometool featurecommand startfeature alpha
sometool featurecommand startfeature alpha --dont-publish
sometool featurecommand startfeature --name=alpha --dont-publish
sometool featurecommand startfeature -n=alpha -d
```

## Multiple and nested verbs, with additional configuration
> Now what if you *do* care about help texts and also don't want to accept the default names and other settings detected by FluentArgumentParser?


### Code
```csharp
var parser = ParserFactory.Create(new ParserConfiguration
{
    ApplicationName = "coolTool",
    ApplicationDescription = "this super-duper tool serves as a demo for the help generation"
});

var feature = parser.AddContainerVerb<FeatureCommand>()
    .Rename("feature")
    .WithHelp("work with feature branches");
var startFeature = feature.AddVerb<StartFeature>()
                          .Rename("start")
                          .WithHelp("create a new feature branch");
startFeature.Parameter(s => s.DontPublish).WithHelp("don't publish the branch to remote");
startFeature.Parameter(s => s.Name).WithHelp("the name of the branch");
feature.AddVerb<PublishFeature>().Rename("publish").WithHelp("create a PR from a feature branch")
       .Parameter(s => s.Name).WithHelp("the name of the branch");
var rect = parser.AddVerb<DrawRectangle>().Rename("rect").WithHelp("draw a rectangle");
rect.Parameter(r => r.Width)
    .MakeOptional()
    .WithDefault(100)
    .WithShortName("ls")
    .WithLongName("long-side")
    .WithHelp("the long side of the rectangle");
rect.Parameter(r => r.Height)
    .MakeOptional()
    .WithDefault(50)
    .WithShortName("ss")
    .WithLongName("short-side")
    .WithHelp("the short side of the rectangle");
rect.Parameter(r => r.X).WithHelp("the x-coordinate");
rect.Parameter(r => r.Y).WithHelp("the y-coordinate");
rect.Parameter(r => r.Filling).WithHelp("how to fill the rectangle");
```

This example leaves out the part of dealing with the parse-result because there's really no change there. 

Things to note:
- the return values of `.AddContainerVerb`, `.AddVerb` and `.DefaultVerb` allow to further customize a verb and its parameters via a fluent API
- you can rename verbs with `.Rename()`
- you can make parameters optional that were recognized as required (because they have not explicit default value set that is different from their type's `default`) with `.MakeOptional()`
- you can change the default value of optional parameters with `.WithDefault.()`
- you can change short and long parameters names with `.WithShortName()` and `.WithLongName()` respectively
- you can set help texts for verbs and parameters with `.WithHelp()` - see the next section for how this is used

### Help
#### `help`
```plaintext
coolTool
this super-duper tool serves as a demo for the help generation

Usage:
coolTool feature | rect [command specific options]

Available commands:
feature  work with feature branches
rect     draw a rectangle

use help <command> for more detailed help on a specific command

long parameter names are case-sensitive
short parameter names are not case-sensitive
command names are not case-sensitive
```
#### `help feature`
```plaintext
coolTool feature start | publish [command specific options]

work with feature branches

Available commands:
start    create a new feature branch
publish  create a PR from a feature branch

use help <command> for more detailed help on a specific command
```
#### `help feature start`
```plaintext
coolTool feature start --name=<value> [--dont-publish]

create a new feature branch

Required arguments:
--name, -n  the name of the branch
            string

Flags:
--dont-publish, -d  don't publish the branch to remote

Examples:
coolTool feature start alpha
coolTool feature start alpha --dont-publish
coolTool feature start --name=alpha --dont-publish
coolTool feature start -n=alpha -d
```
#### Help for `rect`
```plaintext
coolTool rect --x=<value> --y=<value> [--long-side=<value>] [--short-side=<value>] [--filling=<value>]

draw a rectangle

Required arguments:
--x, -x  the x-coordinate
         int
--y, -y  the y-coordinate
         int

Optional arguments:
--long-side, -ls   the long side of the rectangle
                   int
                   default: 100
--short-side, -ss  the short side of the rectangle
                   int
                   default: 50
--filling, -f      how to fill the rectangle
                   None, Hatched, Solid
                   default: Solid

Examples:
coolTool rect 50 60
coolTool rect 50 60 70 80 Solid
coolTool rect --x=50 --y=60 --long-side=70 --short-side=80 --filling=Solid
coolTool rect -x=50 -y=60 -ls=70 -ss=80 -f=Solid
coolTool rect -ss=80 -f=Solid -x=50 -y=60 -ls=70
```

