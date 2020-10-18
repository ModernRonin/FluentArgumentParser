# Configuration
Say you don't like long parameter names being prefixed with `--` or that verb names being case-insensitive. 
In that case, you want to supply a custom `ParserConfiguration` to `ParserFactory.Create()`:

```csharp
var parser = ParserFactory.Create(new ParserConfiguration
{
    ApplicationName = "mycooltool",
    ApplicationDescription = "whatever you want it to be",
    AreLongParameterNamesCaseSensitive = true,
    AreShortParameterNamesCaseSensitive = false,
    AreVerbNamesCaseSensitive = false,
    LongNamePrefix = "--",
    ShortNamePrefix = "-",
    ValueDelimiter = "=",
    ArgumentPreprocessor = new NullArgumentPreprocessor()
});

// the above is the same as if you call
parser = ParserFactory.Create(new ParserConfiguration
{
    ApplicationName = "mycooltool",
    ApplicationDescription = "whatever you want it to be"
});

// or:
parser = ParserFactory.Create("mycooltool", "whatever you want it to be");
```

See [the reference](Reference.md#parserconfiguration) for more detail about the properties of `ParserConfiguration`.