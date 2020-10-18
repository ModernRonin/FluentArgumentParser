# Examples
Here are a few examples, moving from low complexity and customization to high complexity and customization.

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
#### Overview
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
#### For a specific verb
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
#### Overview
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
#### Help for `featurecommand`
```plaintext
sometool featurecommand startfeature | publishfeature [command specific options]


Available commands:
startfeature    
publishfeature  

use help <command> for more detailed help on a specific command
```
#### Help for `featurecommand startfeature`
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
