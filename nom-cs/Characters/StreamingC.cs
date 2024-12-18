using System;
using System.Buffers;
using System.Linq;

namespace Nom.Characters;

/// <summary>
/// Character specific parsers and combinators, streaming version
/// </summary>
public class StreamingC
{
    /// <summary>
    /// Recognizes zero or more ASCII numerical and alphabetic characters: 0-9, a-z, A-Z
    /// </summary>
    /// <remarks>
    /// Streaming version: Will return Err(nom::Err::Incomplete(_)) if there’s not enough input data, or if no terminating token is found (a non alphanumerical character).
    /// </remarks>
    public static Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>> AlphaNumeric0()
    {
        var takeAlphaNumeric = Bytes.StreamingC.TakeWhile<char>(char.IsAsciiLetterOrDigit);
        return input =>
        {
            var (rest, digits) = takeAlphaNumeric(input);
            if (rest.IsEmpty)
            {
                throw new IncompleteException("Input is exhausted.");
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
        };
    }

    /// <summary>
    /// Recognizes one or more ASCII numerical and alphabetic characters: 0-9, a-z, A-Z
    /// </summary>
    /// <remarks>
    /// Streaming version: Will return Err(nom::Err::Incomplete(_)) if there’s not enough input data, or if no terminating token is found (a non alphanumerical character).
    /// </remarks>
    public static Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>> AlphaNumeric1()
    {
        var takeAlphaNumeric = Bytes.StreamingC.TakeWhile<char>(char.IsAsciiLetterOrDigit);
        return input =>
        {
            var (rest, digits) = takeAlphaNumeric(input);
            if (rest.IsEmpty)
            {
                throw new IncompleteException("Input is exhausted.");
            }

            if (digits.IsEmpty)
            {
                throw new ParserErrorException("Expected at least one alphanumeric character.");
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
        };
    }

    /// <summary>
    /// Recognizes one of the provided characters.
    /// </summary>
    /// <remarks>
    /// Streaming version: Will throw <see cref="IncompleteException"/> if there’s not enough input data.
    /// </remarks>
    public static Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, char>> OneOf(char[] list)
    {
        return input =>
        {
            if (input.IsEmpty)
            {
                throw new IncompleteException("Expected at least one character.", 1);
            }

            var reader = new SequenceReader<char>(input);
            if (reader.TryRead(out var next)
                && list.Contains(next))
            {
                return new RefTuple<ReadOnlySequence<char>, char>(
                    input.Slice(1),
                    next);
            }

            throw new ParserErrorException($"Expected at least one character of the provided: {String.Join(", ", list)}.");
        };
    }

    /// <summary>
    /// Recognizes one or more ASCII numerical characters: 0-9
    /// </summary>
    /// <remarks>
    /// Streaming version: Will throw <see cref="IncompleteException"/> if there’s not enough input data, or if no terminating token is found (a non digit character).
    /// </remarks>
    public static Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>> Digit1()
    {
        var takeDigits = Bytes.StreamingC.TakeWhile<char>(char.IsDigit);
        return input =>
        {
            var (rest, digits) = takeDigits(input);
            if (rest.IsEmpty)
            {
                throw new IncompleteException("Input is exhausted.");
            }

            if (digits.IsEmpty)
            {
                throw new ParserErrorException("Expected at least one digit");
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
        };
    }

    /// <summary>
    /// Recognizes zero or more ASCII numerical characters: 0-9
    /// </summary>
    /// <remarks>
    /// Streaming version: Will throw <see cref="IncompleteException"/> if there’s not enough input data,
    /// or if no terminating token is found (a non digit character).
    /// </remarks>
    public static Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>> Digit0()
    {
        var takeDigits = Bytes.StreamingC.TakeWhile<char>(char.IsDigit);
        return input =>
        {
            var (rest, digits) = takeDigits(input);
            if (rest.IsEmpty)
            {
                throw new IncompleteException("Input is exhausted.");
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
        };
    }
}