using System;
using System.Buffers;
using System.Collections.Generic;

namespace Nom;

/// <summary>
/// Tests a list of parsers one by one until one succeeds.
/// </summary>
public class AltParser<TIn, TOut>(List<IParser<TIn, TOut>> parsers)
    : IParser<TIn, TOut>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, TOut> Parse(ReadOnlySequence<TIn> input)
    {
        foreach (var parser in parsers)
        {
            try
            {
                var (rest, result) = parser.Parse(input);
                return new RefTuple<ReadOnlySequence<TIn>, TOut>(rest, result);
            }
            catch (IncompleteException)
            {
            }
            catch (ParserErrorException)
            {
            }
        }

        throw new ParserErrorException("No parser found");
    }
}

/// <summary>
/// Choice combinators
/// </summary>
public static class Branch
{
    /// <summary>
    /// Tests a list of parsers one by one until one succeeds.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOut>> Alt<TIn, TOut>(
            List<Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOut>>> parsers)
    {
        return input =>
        {
            foreach (var parser in parsers)
            {
                try
                {
                    var (rest, result) = parser(input);
                    return new RefTuple<ReadOnlySequence<TIn>, TOut>(rest, result);
                }
                catch (IncompleteException)
                {
                }
                catch (ParserErrorException)
                {
                }
            }

            throw new ParserErrorException("No parser found");
        };
    }
}