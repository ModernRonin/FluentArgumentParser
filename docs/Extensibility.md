# Extensibility
First take a look at [ParserConfiguration](Reference.md#parserconfiguration) and what it allows you to change. if this is not enough...

## Name generation
>You don't like how FluentArgumentParser generates names of verbs and parameters from your types and properties?

Implement [INamingStrategy](Reference.md#inamingstrategy) and pass it to the corresponding overload of [ParserFactory.Create](Reference.md#parserfactory).

## Help generation
>You don't like how property/parameter types are formatted in the help or how examples are generated?

Implement [ITypeFormatter](Reference.md#itypeformatter) or [IExampleValueProvider](Reference.md#iexamplevalueprovider) and pass it to the corresponding overload of [ParserFactory.Create](Reference.md#parserfactory).

## Exchanging the low-level parser
>This should hopefully never become necessary, but *if* you want to customize the parser itself, you can do that, too.

Implement [ICommandLineParser](../ModernRonin.FluentArgumentParser/Parsing/ICommandLineParser.cs) (or a decorator for it) pass it to the corresponding overload of [ParserFactory.Create](Reference.md#parserfactory).
