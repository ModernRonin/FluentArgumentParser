namespace ModernRonin.FluentArgumentParser.Parsing;

public interface IContainerVerbBinding : IVerbBinding
{
    ILeafVerbBinding<T> AddVerb<T>() where T : new();
    IContainerVerbBinding AddContainerVerb<T>();
}