using System;
using System.Buffers;

namespace Nom.Bytes.Streaming;

/// <summary>
/// Matches a byte string with escaped characters.
/// </summary>
/// <param name="normal">The first argument matches the normal characters (it must not accept the control character)</param>
/// <param name="controlElement">The second argument is the control character (like \ in most languages)</param>
/// <param name="escapable">The third argument matches the escaped characters</param>
public class EscapedParser<TIn, TOutFirst, TOutSecond>(
    IParser<TIn, TOutFirst> normal,
    TIn controlElement,
    IParser<TIn, TOutSecond> escapable)
    : IParser<TIn, ReadOnlySequence<TIn>>
    where TIn : unmanaged, IEquatable<TIn>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<TIn>, ReadOnlySequence<TIn>> Parse(ReadOnlySequence<TIn> input)
    {
        var current = input;
        while (!current.IsEmpty)
        {
            var (normalRest, _) = normal.Parse(current);
            if (normalRest.IsEmpty)
            {
                throw new IncompleteException("No more input");
            }

            var reader = new SequenceReader<TIn>(normalRest);
            if (reader.TryRead(out var next)
                && next.Equals(controlElement))
            {
                var (escapableRest, _) = escapable.Parse(normalRest.Slice(1));
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
    }
}