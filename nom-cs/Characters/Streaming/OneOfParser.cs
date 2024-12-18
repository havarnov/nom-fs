using System.Buffers;
using System.Linq;

namespace Nom.Characters.Streaming;

/// <summary>
/// Recognizes one of the provided characters.
/// </summary>
/// <remarks>
/// Streaming version: Will throw <see cref="IncompleteException"/> if there’s not enough input data.
/// </remarks>
public class OneOfParser(char[] list) : IParser<char, char>
{
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<char>, char> Parse(ReadOnlySequence<char> input)
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

        throw new ParserErrorException($"Expected at least one character of the provided: {string.Join(", ", list)}.");
    }
}