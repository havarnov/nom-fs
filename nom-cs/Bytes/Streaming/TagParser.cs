using System;
using System.Buffers;

namespace Nom.Bytes.Streaming;

/// <summary>
/// Recognizes a pattern.
/// </summary>
public class TagParser<TIn>(ReadOnlySequence<TIn> tag)
    : IParser<TIn, ReadOnlySequence<TIn>>
    where TIn : unmanaged, IEquatable<TIn>
{
    /// <inheritdoc />
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