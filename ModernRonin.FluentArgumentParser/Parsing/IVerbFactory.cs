namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IVerbFactory
    {
        ILeafVerbBinding<T> MakeLeafBinding<T>() where T : new();
        IContainerVerbBinding MakeContainerBinding<T>();
    }
}