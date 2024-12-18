using System;
using System.Buffers;

namespace Nom;

/// <summary>
/// Maps a function on the result of a parser.
/// </summary>
public class MapParser<TIn, TInOut, TOut>(
    IParser<TIn, TInOut> parser,
    Func<TInOut, TOut> mapper)
    : IParser<TIn, TOut> where TOut : allows ref struct where TInOut : allows ref struct
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, TOut> Parse(ReadOnlySequence<TIn> input)
    {
        var (rest, mappedValue) = parser.Parse(input);
        return new RefTuple<ReadOnlySequence<TIn>, TOut>(rest, mapper(mappedValue));
    }
}

/// <summary>
/// General purpose combinators
/// </summary>
public static class Combinator
{
    /// <summary>
    /// Maps a function on the result of a parser.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOut>> Map<TIn, TInOut, TOut>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TInOut>> parser,
        Func<TInOut, TOut> mapper)
        where TInOut : allows ref struct
        where TOut : allows ref struct
    {
        return input =>
        {
            var (rest, mappedValue) = parser(input);
            return new RefTuple<ReadOnlySequence<TIn>, TOut>(rest, mapper(mappedValue));
        };
    }
}