# Reference

## Concepts
### Verbs
Verbs as a term has come into popular use with git, and FluentArgumentParser has adopted this terminology. In essence, a verb models a single action or command. A verb is specified by its name.
#### Default verb
Sometimes your console app just exposes a single command. Then it doesn't really make sense to expose verbs. This situation is modelled by defining a DefaultVerb in FluentArgumentParser. Once you define a DefaultVerb, the parser will not let you define any others. Vice versa, if you define regular verbs, the parser will not let you define a DefaultVerb.
#### Nested verb
Sometimes you want to group actions. Imagine your app can work with pull requests and exposes several actions on them, for example Create and Finish. But your app also works with branches and exposes actions on them, like pull and push. You could model this with two root verbs, "branch" and "pullrequest" both of which have nested verbs, "pull" and "push" for "branch" and "create" and "finish" for "pullrequest".
### Parameters
FluentArgumentParser currently distinguishes three kinds of parameter. It does not support collection parameters (for now - there is an issue for it and this will follow in a future update). 

Required and optional parameters can be specified just by their position in the argument list or via short/long name. They can be numeric types, string or enums. (More types will follow in future updates.)

Flags are simple boolean values. If a flag is not present, it automatically is assumed to be false, if it is present, it is assumed to be true. Ergo, for flags it's enough to specify their name, no value needed. 

This has one consequence: you need to name your flags in such a way as to fit with the default value of false. For example, if your verb can validate its work after being finished and the default is that it does not validate, then you'd call your property "DoValidate". But if the default is to perform validation, and not performing it is the exception, you'd call it "SkipValidation" or something like that. 

## ParserFactory
This is the main entry point into FluentArgumentParser and allows you to create parsers with different levels of customization. ParserFactory exposes one method, with several overloads, all of them returning a [IBindingCommandLineParser](#ibindingcommandlineparser). They are, in order from least customization to most:

| overload | use-case |
| --- | --- |
| `IBindingCommandLineParser Create(string applicationName, string applicationDescription)` | you don't need any customization at all|
| `IBindingCommandLineParser Create(ParserConfiguration configuration)` | you want to pass in a custom [ParserConfiguration](#parserconfiguration) |
| `IBindingCommandLineParser Create(ParserConfiguration configuration, Services services)` | you want to use some of the extensibility points of FluentArgumentParser. `Services` is just a holder of interface implementations. If you create a new instance, all properties are set to their default implementations. Just set the one you want to customize and pass it into this method.<sup>1</sup> |
| `IBindingCommandLineParser Create(Services services, ICommandLineParser parser)` | Hopefully, you never need this overload, but if you ever want to change how the low-level parser works while keeping the high-level binding behavior, this is how you'd specify your implementation. |

 ><sup>1</sup>This mechanism is in place because we don't want to tie you to any specific IOC abstraction. 


## IBindingCommandLineParser
This is the high-level construct for argument parsing. A parser can have either multiple verbs or a default-verb (if you have just one action).
It exposes the following members:

| Member | Description |
| --- | --- |
| `string HelpOverview` | Gets the overview help text, in case you need to display it without the user providing any input yet. |
| `ILeafVerbBinding<T> AddVerb<T>()` | Use this if your app supports verbs. The type parameter is a regular POCO. Use [extensions methods](#verbbindingextensions) on the return value to further customize how FluentArgumentParser treats this verb.|
| `ILeafVerbBinding<T> DefaultVerb<T>()` | Use this if your app doesn't support verbs. The type parameter is a regular POCO. Use the [extensions methods](#parameterbindingconfigurer) on the return value to further customize how FluentArgumentParser treats the parameters discovered from your POCO. |
| `IContainerVerbBinding AddContainerVerb<T>()` | If you have nested verbs, this is what you call to define a container verb. Container verbs can have children, but no parameters. |
| `object Parse(string[] args)` | Call this to run the parser. The return value will be either one of the POCOs you added with `AddVerb()` or `DefaultVerb()`, or it will be [HelpResult](#helpresult) if the arguments contain an explicitrequests for help or they are invalid.|

## HelpResult
[IBindingCommandLineParser](#ibindingcommandlineparser)'s `Parse()` method can return this instead of any of your verb POCOs if either of the following is true:
- the arguments include a request for help, be that general or for a specific verb
- the arguments contain invalid/missing parameters for a verb
- the arguments mention an unknown verb

| Member | Description |
| --- | --- |
| `string Text` | The help text to display to the user. |
| `bool IsResultOfInvalidInput` | True if the arguments parsed were invalid, false if they contained an explicit request for help. |

## VerbBindingExtensions
| Member | Description |
| --- | --- |
| `Rename<T>(string newName)` | rename a verb (by default the name is derived from the name of your POCO) |
| `WithHelp<T>(string helpText)` |  Set the general help text for this verb. By default the help text is empty. | 
|`Parameter<TProperty, TTarget>(Expression<Func<TTarget, TProperty>> accessor)`| returns an object allowing to configure the parameters of this verb, see the following section |

## ParameterBindingConfigurer
| Member | Description |
| --- | --- |
|`MakeOptional()`|Turn a parameter optional. By default, only properties in your verb POCOs having a non-redundant initialization value are assumed to be optional, all others are assumed to be required.|
|`WithDefault(TProperty value)`|If you want to change the auto-detected default value of an optional parameter or if you just turned a required parameter optional via `MakeOptional()` then you can use this method. Auto-detection of default values uses what you set a construction time for a property. If the value you set differs from what the property would have if you didn't set anything, then the property/parameter is assumed to be optional and your initialization value is used as a default value.|
|`ExpectAt(int index)`|Set the index at which this parameter is expected. This is only relevant when users don't use names to specify parameters and affects only required or optional parameters, but not flags. By default, indices for parameters are derived from the order of properties in your POCOs, but this can become different from what you expect when your POCOs have base classes.|
|`WithLongName(string longName)`|Set the long name that can be used to specify a parameter. By default, the property-name is used.|
|`WithShortName(string shortName)`|Set the short name that can be used to specify a parameter. By default, the first letter of the property-name is used. If that letter is already in use by another property/parameter, the next one is used and so on.|
|`WithHelp(string helpText)`|Set the help description for this parameter/property. By default, the help text is empty.|

## ParserConfiguration
| Member | Description |
| --- | --- |
| `string ApplicationName` | This should be the name with which the application is called, so typically the name of your executable.(But for example for a global dotnet tool, it should be set to what you configured for ToolCommandName, or for a local dotnet tool it should be `dotnet <ToolCommandName>`.) *This must be set explicitly and is not optional.*  |
| `string ApplicationDescription` | A general description of what you app is used for. *This must be set explicitly and is not optional.* |
| `string LongNamePrefix` | How do we expect the user to prefix long argument names? Default: `--`|
| `string ShortNamePrefix` | How do we expect the user to prefix short argument names? Default: `-` |
| `string ValueDelimiter` | How do we expect the user to separate argument values from argument names? Default: =``|
| `bool AreVerbNamesCaseSensitive` | Should verb names be case-sensitive? Default: no |
| `bool AreLongParameterNamesCaseSensitive` | Should long argument names be case-sensitive? Default: yes |
| `bool AreShortParameterNamesCaseSensitive` | Should short argument names be case-sensitive? Default: no |
## INamingStrategy
Implement this interface if you want to customize how verb and parameter names are generated from your POCOs.
| Member | Description |
| --- | --- |
| `string GetLongName(PropertyInfo propertyInfo)` | Generate the long parameter name for a property. The default implementation uses the property's name in kebab-cases form, so for example *MySpecialName* becomes *my-special-name*.|
| `string GetShortName(PropertyInfo propertyInfo, string[] shortNamesToBeExcluded)` | Generate the short parameter name for a property. `shortNamesToBeExcluded` contains all short names that are already claimed by other parameters on the same verb. The default implementation takes the first character of the property's lower-cased name that has not been used yet. In the (hopefully very unlikely) event that there is no such character, it will use `char.MinValue` and additional configuration will be required to enable short-name usage.|
| `string GetVerbName(Type type)` | Generate the verb name for a POCO. The default implementation uses the lower-cased type's name.|
## ITypeFormatter
Implement this interface if you want to customize how the types of parameters are formatted in help texts. Note that you don't have to worry about <see cref="bool" /> types because they are handled by flag parameters and there is no type displayed for them in help texts.

| Member | Description |
| `string Format(AParameter parameter)` | The default implementation uses the C# type name for standard types like `int` or `string`, a list of their labels for enums and just the type's name for everything else |

## IExampleValueProvider
Implement this interface if you want to customize how values for example calls in help texts are generated.
| Member | Description |
| `object For(AParameter parameter)` | Generate an example value for a parameter. The default implementation picks from a list of constants for numeric types and for strings, uses the last label for enums and just `...` for everything else.|

## IArgumentPreprocessor
Implement this interface and set via [ParserConfiguration](#parserconfiguration) if you want to pre-process arguments before they are fed into verbs.

One example use case for this would be if you are not happy with the quoting mechanism used by the combination of shell and CLR runtime and want to customize it.
| Member | Description |
| `string Process(string what);` | return the transformed argument |
