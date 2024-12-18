using System;
using System.Buffers;

namespace Nom.Bytes;

/// <summary>
/// Parsers recognizing bytes streams, streaming version
/// </summary>
public static class StreamingC
{
    /// <summary>
    /// Matches a byte string with escaped characters.
    ///
    /// The first argument matches the normal characters (it must not accept the control character)
    /// The second argument is the control character (like \ in most languages)
    /// The third argument matches the escaped characters
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>> Escaped<TIn, TOutFirst, TOutSecond>(
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutFirst>> normal,
        TIn controlElement,
        Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, TOutSecond>> escapable)
        where TIn : unmanaged, IEquatable<TIn>
    {
        return input =>
        {
            var current = input;
            while (!current.IsEmpty)
            {
                var (normalRest, normalResult) = normal(current);
                if (normalRest.IsEmpty)
                {
                    throw new IncompleteException("No more input");
                }

                var reader = new SequenceReader<TIn>(normalRest);
                if (reader.TryRead(out var next)
                    && next.Equals(controlElement))
                {
                    var (escapableRest, escapableResult) = escapable(normalRest.Slice(1));
                    if (escapableRest.IsEmpty)
                    {
                        throw new IncompleteException("No more input (escapable)");
                    }

                    current = escapableRest;
                }
                else
                {
                    var restLength = input.Length - normalRest.Length;
                    return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(
                        input.Slice(restLength),
                        input.Slice(0, restLength));
                }
            }

            throw new ParserErrorException("Something went wrong...");
        };
    }

    /// <summary>
    /// Returns the longest input slice (if any) that matches the predicate.
    /// </summary>
    /// <remarks>
    /// Streaming version will throw a <see cref="IncompleteException"/> if the pattern reaches the end of the input.
    /// </remarks>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>> TakeWhile<TIn>(
        Func<TIn, bool> predicate)
        where TIn : unmanaged, IEquatable<TIn>
    {
        return input =>
        {
            var idx = 0;
            var reader = new SequenceReader<TIn>(input);
            while (reader.TryRead(out var el))
            {
                if (!predicate(el))
                {
                    return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(
                        input.Slice(idx),
                        input.Slice(0, idx));
                }

                idx++;
            }

            throw new IncompleteException("Reached end of sequence");
        };
    }

    /// <summary>
    /// Returns an <see cref="ReadOnlySequence{T}"/> containing the first N input elements.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>> Take<TIn>(
        int count)
    {
        return input =>
        {
            if (input.Length < count)
            {
                throw new IncompleteException("Input is too small");
            }

            return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(
                input.Slice(count),
                input.Slice(0, count));
        };
    }

    /// <summary>
    /// Recognizes a pattern.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>> Tag<TIn>(
        ReadOnlySpan<TIn> tag)
        where TIn : unmanaged, IEquatable<TIn>
    {
        var captured = new ReadOnlySequence<TIn>(tag.ToArray().AsMemory());
        return Tag(captured);
    }

    /*
    private interface IParser<TIn, TOut>
    {
        RefTuple<ReadOnlySequence<TIn>, TOut> Parse(ReadOnlySequence<TIn> input);
    }

    private class TagParser<TIn>(ReadOnlySequence<TIn> tag) : IParser<TIn, ReadOnlySequence<TIn>>
        where TIn : unmanaged, IEquatable<TIn>
    {
        public RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>> Parse(ReadOnlySequence<TIn> input)
        {
            if (input.Length < tag.Length)
            {
                throw new IncompleteException("Not enough bytes to tag the input.");
            }

            var readerTag = new SequenceReader<TIn>(tag);
            var readerInput = new SequenceReader<TIn>(input);
            var equal = false;
            for (var count = 0; count < tag.Length; count++)
            {
#pragma warning disable IDE0045
                if (readerTag.TryRead(out var tagValue)
#pragma warning restore IDE0045
                    && readerInput.TryRead(out var inputValue)
                    && tagValue.Equals(inputValue))
                {
                    equal = true;
                }
                else
                {
                    equal = false;
                }
            }

            if (equal)
            {
                var result = input.Slice(0, tag.Length);
                var rest = input.Slice(tag.Length);
                return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(rest, result);
            }
            else
            {
                throw new ParserErrorException("Couldn't find the specified tag.");
            }
        }
    }

    private static IParser<TIn, ReadOnlySequence<TIn>> TagParserF<TIn>(ReadOnlySequence<TIn> tag)
        where TIn : unmanaged, IEquatable<TIn>
    {
        return new TagParser<TIn>(tag);
    }
    */

    /// <summary>
    /// Recognizes a pattern.
    /// </summary>
    public static Func<ReadOnlySequence<TIn>, RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>> Tag<TIn>(ReadOnlySequence<TIn> tag)
        where TIn : unmanaged, IEquatable<TIn>
    {
        var captured = tag.ToArray();
        return input =>
        {
            if (input.Length < captured.Length)
            {
                throw new IncompleteException("Not enough bytes to tag the input.");
            }

            var readerTag = new SequenceReader<TIn>(tag);
            var readerInput = new SequenceReader<TIn>(input);
            var equal = false;
            for (var count = 0; count < tag.Length; count++)
            {
#pragma warning disable IDE0045
                if (readerTag.TryRead(out var tagValue)
#pragma warning restore IDE0045
                    && readerInput.TryRead(out var inputValue)
                    && tagValue.Equals(inputValue))
                {
                    equal = true;
                }
                else
                {
                    equal = false;
                }
            }

            if (equal)
            {
                var result = input.Slice(0, tag.Length);
                var rest = input.Slice(tag.Length);
                return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(rest, result);
            }
            else
            {
                throw new ParserErrorException("Couldn't find the specified tag.");
            }
        };
    }
}