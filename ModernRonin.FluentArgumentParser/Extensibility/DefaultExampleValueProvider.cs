using System;
using System.Collections.Generic;
using System.Linq;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility;

public class DefaultExampleValueProvider : IExampleValueProvider
{
    readonly IEnumerator<int> _integers =
        new SimpleCircularBuffer<int>(10, 20, 30, 40, 50, 60, 70, 80, 90, 100).GetEnumerator();

    readonly IEnumerator<float> _reals =
        new SimpleCircularBuffer<float>(1.1f, 1.2f, 3.4f, 4.5f, 5.7f).GetEnumerator();

    readonly IEnumerator<string> _texts = new SimpleCircularBuffer<string>("alpha",
        "bravo",
        "charlie",
        "delta",
        "echo",
        "foxtrot",
        "golf",
        "hotel",
        "india",
        "juliet",
        "kilo",
        "lima",
        "mike",
        "november",
        "oscar",
        "papa",
        "quebec",
        "romeo",
        "sierra",
        "tango",
        "uniform",
        "victor",
        "whiskey",
        "x-ray",
        "yankee",
        "zulu").GetEnumerator();

    public object For(AParameter parameter)
    {
        if (!(parameter is AnIndexableParameter indexable)) return null;

        var type = indexable.Type;
        if (type == typeof(string))
        {
            _texts.MoveNext();
            return _texts.Current;
        }

        if (type == typeof(int))
        {
            _integers.MoveNext();
            return _integers.Current;
        }

        if (type == typeof(uint))
        {
            _integers.MoveNext();
            return (uint)_integers.Current;
        }

        if (type == typeof(short))
        {
            _integers.MoveNext();
            return (short)_integers.Current;
        }

        if (type == typeof(ushort))
        {
            _integers.MoveNext();
            return (ushort)_integers.Current;
        }

        if (type == typeof(long))
        {
            _integers.MoveNext();
            return (long)_integers.Current;
        }

        if (type == typeof(ulong))
        {
            _integers.MoveNext();
            return (ulong)_integers.Current;
        }

        if (type == typeof(byte))
        {
            _integers.MoveNext();
            return (byte)_integers.Current;
        }

        if (type == typeof(sbyte))
        {
            _integers.MoveNext();
            return (sbyte)_integers.Current;
        }

        if (type == typeof(float))
        {
            _reals.MoveNext();
            return _reals.Current;
        }

        if (type == typeof(double))
        {
            _reals.MoveNext();
            return (double)_reals.Current;
        }

        if (type == typeof(decimal))
        {
            _reals.MoveNext();
            return (decimal)_reals.Current;
        }

        if (type.IsEnum) return Enum.GetNames(type).Last();
        return "...";
    }
}