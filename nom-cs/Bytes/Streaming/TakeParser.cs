using System.Buffers;

namespace Nom.Bytes.Streaming;

/// <summary>
/// Returns an <see cref="ReadOnlySequence{T}"/> containing the first N input elements.
/// </summary>
public class TakeParser<TIn>(int count) : IParser<TIn, ReadOnlySequence<TIn>>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>> Parse(ReadOnlySequence<TIn> input)
    {
        if (input.Length < count)
        {
            throw new IncompleteException("Input is too small");
        }

        return new RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>>(
            input.Slice(count),
            input.Slice(0, count));
    }
}