using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public class VerbFactory : IVerbFactory
    {
        readonly INamingStrategy _namingStrategy;
        public VerbFactory(INamingStrategy namingStrategy) => _namingStrategy = namingStrategy;

        public ILeafVerbBinding<T> MakeLeafBinding<T>() where T : new()
        {
            var result = new LeafVerbBinding<T>(_namingStrategy);
            result.Bind();
            return result;
        }

        public IContainerVerbBinding MakeContainerBinding<T>()
        {
            var result = new ContainerVerbBinding<T>(this, _namingStrategy);
            result.Bind();
            return result;
        }
    }
}