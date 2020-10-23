# Comparison 

The by far most commmonly used commandline parsing library out there is [CommandLineParser](https://www.nuget.org/packages/CommandLineParser). So it makes sense to compare FluentArgumentParser with CommandLineParser for people to decide what they want to use or wanting to migrate from one to the other.

>The most important difference between CommandLineParser (and quite a few other similar packages, too) and FluentArgumentParser is that FluentArgumentParser does not require you to attribute the types you want to fill from command line arguments. 

In the end, this is a matter of personal preference/style, but [in our opinion](AttributeOverusage.md) attributes often lead to less maintainable code. 

On the other hand, CommandLineParser being a very mature library can do a few things FluentArgumentParser cannot (yet). 

> Also, importantly, FluentArgumentParser is a NET Core only library. 

With NET 5 to come out in November 2020, classic .NET is all but dead, and if you are writing a command-line tool nowadays, you should really base it of NET Core and soon NET 5. If you got legacy libraries based on NET4x that you need to reference in your commandline tool, you should seriously consider converting these libraries to at least NET Standard. 

The NET4x world is already behind in language features and this will just become more pronounced with the next release in November. And FluentArgumentParser in particular makes use of a language feature that you don't have in NET4x (you can activate it via a setting in your project file, but this is something you should do only if your code is covered very well with tests), that is switching on types. 


## Usage Comparison


## Feature Comparison
| CommandLineParser | FluentArgumentParser |
| --- | --- |
| 


