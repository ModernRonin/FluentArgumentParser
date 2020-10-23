# Why Attributes are often problematic

In our opinion attributes often make code harder to read and they tend to violate the [SRP](https://blog.cleancoder.com/uncle-bob/2014/05/08/SingleReponsibilityPrinciple.html). 

Say you are writing a tool that applies migrations to a database. This tool will need a configuration type specifying the connection string, the location of the input SQL files etc. The responsibility of this configuration type is to be just that: to hold configuration for your migration runner. If you are using a commandline parsing library predicated on the use of attributes, you will all of a sudden add information to this type that has nothing to do with configuration at all - the information you add is about commandline usage, not about configuration.

Why is this a bad thing? 

Imagine at a later point in time you write a graphical UI for your tool. There is no reason your UI should not re-use the configuration type. But wait, now your UI code interfaces with a type attributed for the command-line. Never mind, you say, you don't have to care about these attributes. But then you find that clever library allowing you to automatically create an input form from a type - provided it's attributed with a set of special attributes. 

Or, in another timeline, you decide you need to serialize your configuration type to JSON. To customize serialization, you pepper it with `JsonProperty` attributes and the like.

You can see how this very soon leads to a proliferation of attributes and someone looking at configuration type now sees all kinds of information they are probably not even interested in. 

Of course, you can create different types for different usage scenarios - a `MigrationConfiguration`, a `CommandLineMigrationConfiguration`, a `SerializableMigrationConfiguration` and a `WpfMigrationConfiguration` and use something like the superb [AutoMapper](https://www.nuget.org/packages/automapper/) package to convert between them.

That is a valid solution, but leads to quite a bit of duplication because you will have to duplicate all members of `MigrationConfiguration` in all the other types in order to annotate them with the respective attributes. 

The alternative is to avoid attributes. If you can just configure how your `MigrationConfiguration`
- is filled from commandline arguments
- is turned into a graphical UI
- is serialized to JSON

by using separate configuration APIs, you avoid the duplication and you can keep the respective configuration where it should be - the command-line configuration in your console-based runner, the UI configuration in the UI runner and the serialization configuration in your persistence layer.



