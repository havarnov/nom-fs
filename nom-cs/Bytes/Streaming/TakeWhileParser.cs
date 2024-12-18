using System;
using System.Buffers;

namespace Nom.Bytes.Streaming;

/// <summary>
/// Returns the longest input slice (if any) that matches the predicate.
/// </summary>
/// <remarks>
/// Streaming version will throw a <see cref="IncompleteException"/> if the pattern reaches the end of the input.
/// </remarks>
public class TakeWhileParser<TIn>(Func<TIn, bool> predicate)
    : IParser<TIn, ReadOnlySequence<TIn>>
    where TIn : unmanaged, IEquatable<TIn>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>> Parse(ReadOnlySequence<TIn> input)
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
    }
}