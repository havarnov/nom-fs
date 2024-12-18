using System;
using System.Buffers;

namespace Nom;

/// <summary>
/// Applies a tuple of parsers one by one and returns their results as a tuple.
/// </summary>
public class TupleTwoParser<TIn, TOutFirst, TOutSecond>(
    IParser<TIn, TOutFirst> first,
    IParser<TIn, TOutSecond> second)
    : IParser<TIn, RefTuple<TOutFirst, TOutSecond>> where TOutSecond : allows ref struct
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutSecond>> Parse(ReadOnlySequence<TIn> input)
    {
        var (firstRest, firstResult) = first.Parse(input);
        var (secondRest, secondResult) = second.Parse(firstRest);
        return new RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutSecond>>(
            secondRest,
            new RefTuple<TOutFirst, TOutSecond>(firstResult, secondResult));
    }
}

/// <summary>
/// Applies a tuple of parsers one by one and returns their results as a tuple.
/// </summary>
public class TupleThreeParser<TIn, TOutFirst, TOutSecond, TOutThird>(
    IParser<TIn, TOutFirst> first,
    IParser<TIn, TOutSecond> second,
    IParser<TIn, TOutThird> third)
    : IParser<TIn, RefTuple3<TOutFirst, TOutSecond, TOutThird>> where TOutFirst : allows ref struct where TOutSecond : allows ref struct where TOutThird : allows ref struct
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, RefTuple3<TOutFirst, TOutSecond, TOutThird>> Parse(
        ReadOnlySequence<TIn> input)
    {
        var (firstRest, firstResult) = first.Parse(input);
        var (secondRest, secondResult) = second.Parse(firstRest);
        var (thirdRest, thirdResult) = third.Parse(secondRest);
        return new RefTuple<ReadOnlySequence<TIn>, RefTuple3<TOutFirst, TOutSecond, TOutThird>>(
            thirdRest,
            new RefTuple3<TOutFirst, TOutSecond, TOutThird>(firstResult, secondResult, thirdResult));
    }
}

/// <summary>
/// Matches an object from the first parser and discards it, then gets an object from the second parser.
/// </summary>
/// <inheritdoc />
public class PrecededParser<TIn, TOutFirst, TOutSecond>(
    IParser<TIn, TOutFirst> first,
    IParser<TIn, TOutSecond> second)
    : IParser<TIn, TOutSecond>
{
    private readonly MapParser<TIn, RefTuple<TOutFirst, TOutSecond>, TOutSecond> _parser =
        new(new TupleTwoParser<TIn, TOutFirst, TOutSecond>(first, second),
            result =>
            {
                var (_, secondResult) = result;
                return secondResult;
            });

    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, TOutSecond> Parse(ReadOnlySequence<TIn> input)
    {
        return _parser.Parse(input);
    }
}

/// <summary>
/// Matches an object from the first parser and discards it, then gets an object from the second parser,
/// and finally matches an object from the third parser and discards it.
/// </summary>
public class DelimitedParser<TIn, TOutFirst, TOutSecond, TOutThird>(
    IParser<TIn, TOutFirst> first,
    IParser<TIn, TOutSecond> second,
    IParser<TIn, TOutThird> third)
    : IParser<TIn, TOutSecond>
{
    private readonly MapParser<TIn, RefTuple3<TOutFirst, TOutSecond, TOutThird>, TOutSecond> _parser =
        new(new TupleThreeParser<TIn, TOutFirst, TOutSecond, TOutThird>(first, second, third),
            result =>
            {
                var (_, secondResult, _) = result;
                return secondResult;
            });

    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, TOutSecond> Parse(ReadOnlySequence<TIn> input)
    {
        return _parser.Parse(input);
    }
}

/// <summary>
/// Matches an object from the first parser and discards it, then gets an object from the second parser,
/// and finally matches an object from the third parser and discards it.
/// </summary>
public class SeperatedPairParser<TIn, TOutFirst, TOutSecond, TOutThird>(
    IParser<TIn, TOutFirst> first,
    IParser<TIn, TOutSecond> second,
    IParser<TIn, TOutThird> third)
    : IParser<TIn, RefTuple<TOutFirst, TOutThird>>
{
    private readonly MapParser<TIn, RefTuple3<TOutFirst, TOutSecond, TOutThird>, RefTuple<TOutFirst, TOutThird>> _parser =
        new(new TupleThreeParser<TIn, TOutFirst, TOutSecond, TOutThird>(first, second, third),
            result =>
            {
                var (firstResult, _, thirdResult) = result;
                return new RefTuple<TOutFirst, TOutThird>(firstResult, thirdResult);
            });

    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutThird>> Parse(ReadOnlySequence<TIn> input)
    {
        return _parser.Parse(input);
    }
}

/// <summary>
/// Combinators applying parsers in sequence
/// </summary>
public static class Sequence
{
    /// <summary>
    /// Applies a tuple of parsers one by one and returns their results as a tuple.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutSecond>>> Tuple2<TIn, TOutFirst, TOutSecond>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> first,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> second)
    {
        return input =>
        {
            var (firstRest, firstResult) = first(input);
            var (secondRest, secondResult) = second(firstRest);
            return new RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutSecond>>(
                secondRest,
                new RefTuple<TOutFirst, TOutSecond>(firstResult, secondResult));
        };
    }

    /// <summary>
    /// Applies a 3-tuple of parsers one by one and returns their results as a tuple.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, RefTuple3<TOutFirst, TOutSecond, TOutThird>>> Tuple3<TIn, TOutFirst, TOutSecond, TOutThird>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> first,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> second,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutThird>> third)
    {
        return input =>
        {
            var (firstRest, firstResult) = first(input);
            var (secondRest, secondResult) = second(firstRest);
            var (thirdRest, thirdResult) = third(secondRest);
            return new RefTuple<ReadOnlySequence<TIn>, RefTuple3<TOutFirst, TOutSecond, TOutThird>>(
                thirdRest,
                new RefTuple3<TOutFirst, TOutSecond, TOutThird>(firstResult, secondResult, thirdResult));
        };
    }

    /// <summary>
    /// Matches an object from the first parser and discards it, then gets an object from the second parser.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> Preceded<TIn, TOutFirst, TOutSecond>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> first,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> second)
    {
        return Combinator.Map(
            Tuple2(first, second),
            result =>
            {
                var (_, secondResult) = result;
                return secondResult;
            });
    }

    /// <summary>
    /// Matches an object from the first parser and discards it, then gets an object from the second parser,
    /// and finally matches an object from the third parser and discards it.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> Delimited<TIn, TOutFirst, TOutSecond, TOutThird>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> first,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> second,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutThird>> third)
    {
        return Combinator.Map(
            Tuple3(first, second, third),
            result =>
            {
                var (_, secondResult, _) = result;
                return secondResult;
            });
    }

    /// <summary>
    /// Matches an object from the first parser and discards it, then gets an object from the second parser,
    /// and finally matches an object from the third parser and discards it.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, RefTuple<TOutFirst, TOutThird>>> SeperatedPair<TIn, TOutFirst, TOutSecond, TOutThird>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> first,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> second,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutThird>> third)
    {
        return Combinator.Map(
            Tuple3(first, second, third),
            result =>
            {
                var (firstResult, _, thirdResult) = result;
                return new RefTuple<TOutFirst, TOutThird>(firstResult, thirdResult);
            });
    }
}