using System.Buffers;

using Nom.Bytes.Streaming;

namespace Nom.Characters.Streaming;

/// <summary>
/// Recognizes zero/one or more ASCII numerical and alphabetic characters: 0-9, a-z, A-Z
/// </summary>
/// <param name="requireAtLestOne">
/// If this is set to true, then this parser will fail if no alohanumeric char are found.
/// </param>
public class AlphaNumericParser(bool requireAtLestOne) : IParser<char, ReadOnlySequence<char>>
{
    private static readonly IParser<char, ReadOnlySequence<char>> TakeAlphaNumeric =
        new TakeWhileParser<char>(char.IsAsciiLetterOrDigit);

    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
    {
        var (rest, digits) = TakeAlphaNumeric.Parse(input);
        if (rest.IsEmpty)
        {
            throw new IncompleteException("Input is exhausted.");
        }

        if (requireAtLestOne && digits.IsEmpty)
        {
            throw new ParserErrorException("Expected at least one alphanumeric character.");
        }

        return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
    }
}