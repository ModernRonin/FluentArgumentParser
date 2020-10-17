using System.Collections.Generic;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IVerbBinding
    {
        /// <summary>
        ///     Internal use only.
        /// </summary>
        Verb Verb { get; }

        /// <summary>
        ///     Internal use only.
        /// </summary>
        IEnumerable<IVerbBinding> ThisAndChildren { get; }

        /// <summary>
        ///     Internal use only.
        /// </summary>
        void Bind();
    }
}