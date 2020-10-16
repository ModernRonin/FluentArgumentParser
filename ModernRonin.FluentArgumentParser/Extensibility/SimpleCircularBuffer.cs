using System.Collections;
using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public class SimpleCircularBuffer<T> : IEnumerable<T>
    {
        readonly T[] _values;

        public SimpleCircularBuffer(params T[] values) => _values = values;

        public IEnumerator<T> GetEnumerator() => new Enumerator(_values);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class Enumerator : IEnumerator<T>

        {
            readonly T[] _values;
            int _nextIndex = -1;

            public Enumerator(T[] values) => _values = values;

            public bool MoveNext()
            {
                _nextIndex++;
                if (_nextIndex >= _values.Length) _nextIndex = 0;
                return true;
            }

            public void Reset()
            {
                _nextIndex = 0;
            }

            public T Current => _values[_nextIndex];

            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
}