using System;
using System.Buffers;
using System.Collections.Generic;

namespace Nom;

/// <summary>
/// Alternates between two parsers to produce a list of elements.
///
/// This stops when either parser throws <see cref="ParserErrorException"/> and returns the results that were accumulated.
/// TODO: To instead chain an error up, see cut.
/// </summary>
public class SeperatedListParser<TIn, TOutSep, TOut>(
    IParser<TIn, TOut> parser,
    IParser<TIn, TOutSep> separator,
    bool requireAtLeastOne)
    : IParser<TIn, List<TOut>>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, List<TOut>> Parse(ReadOnlySequence<TIn> input)
    {
        var result = new List<TOut>();
        ReadOnlySequence<TIn> current;
        try
        {
            var (firstRest, element) = parser.Parse(input);
            current = firstRest;
            result.Add(element);
        }
        catch (ParserErrorException)
        {
            if (requireAtLeastOne)
            {
                throw;
            }

            return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(input, result);
        }

        while (true)
        {
            try
            {
                var (separatorRest, _) = separator.Parse(current);

                // infinite loop check: the parser must always consume
                if (separatorRest.Length == current.Length)
                {
                    break;
                }

                try
                {
                    var (parserRest, element) = parser.Parse(separatorRest);
                    result.Add(element);
                    current = parserRest;

                    if (current.IsEmpty)
                    {
                        return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
                    }
                }
                catch (ParserErrorException)
                {
                    return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
                }
            }
            catch (ParserErrorException)
            {
                return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
            }
        }

        throw new ParserErrorException("Could not parse seperated sequence.");
    }
}

/// <summary>
/// Combinators applying their child parser multiple times
/// </summary>
public static class Multi
{
    /// <summary>
    /// Alternates between two parsers to produce a list of elements.
    ///
    /// This stops when either parser throws <see cref="ParserErrorException"/> and returns the results that were accumulated.
    /// TODO: To instead chain an error up, see cut.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, List<TOut>>> SeperatedList0<TIn, TOutSep, TOut>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSep>> separator,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOut>> parser)
        where TOutSep : allows ref struct
    {
        return input =>
        {
            var result = new List<TOut>();
            ReadOnlySequence<TIn> current;
            try
            {
                var (firstRest, element) = parser(input);
                current = firstRest;
                result.Add(element);
            }
            catch (ParserErrorException)
            {
                return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(input, result);
            }

            while (true)
            {
                try
                {
                    var (separatorRest, _) = separator(current);

                    // infinite loop check: the parser must always consume
                    if (separatorRest.Length == current.Length)
                    {
                        break;
                    }

                    try
                    {
                        var (parserRest, element) = parser(separatorRest);
                        result.Add(element);
                        current = parserRest;

                        if (current.IsEmpty)
                        {
                            return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
                        }
                    }
                    catch (ParserErrorException)
                    {
                        return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
                    }
                }
                catch (ParserErrorException)
                {
                    return new RefTuple<ReadOnlySequence<TIn>, List<TOut>>(current, result);
                }
            }

            throw new ParserErrorException("Could not parse seperated sequence.");
        };
    }
}