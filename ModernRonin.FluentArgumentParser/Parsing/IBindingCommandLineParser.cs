namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IBindingCommandLineParser
    {
        ILeafVerbBinding<T> AddVerb<T>() where T : new();
        ILeafVerbBinding<T> DefaultVerb<T>() where T : new();
        IContainerVerbBinding AddContainerVerb<T>();
        object Parse(string[] args);
    }
}