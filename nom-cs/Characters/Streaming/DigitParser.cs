using System.Buffers;

using Nom.Bytes.Streaming;

namespace Nom.Characters.Streaming;

/// <summary>
/// Recognizes zero/one or more ASCII numerical characters: 0-9
/// </summary>
/// <remarks>
/// Streaming version: Will throw <see cref="IncompleteException"/> if there’s not enough input data, or if no terminating token is found (a non digit character).
/// </remarks>
/// <param name="requireAtLeastOneDigit">
/// If set to true, this parser will require at least one digit.
/// </param>
public class DigitParser(bool requireAtLeastOneDigit) : IParser<char, ReadOnlySequence<char>>
{
    private static readonly IParser<char, ReadOnlySequence<char>> TakeDigitParser =
        new TakeWhileParser<char>(char.IsAsciiDigit);

    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
    {
        var (rest, digits) = TakeDigitParser.Parse(input);
        if (rest.IsEmpty)
        {
            throw new IncompleteException("Input is exhausted.");
        }

        if (requireAtLeastOneDigit && digits.IsEmpty)
        {
            throw new ParserErrorException("Expected at least one digit");
        }

        return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digits);
    }
}