using System;
using System.Buffers;
using System.Globalization;

using Nom.Bytes.Streaming;
using Nom.Characters.Streaming;

namespace Nom.Number.Streaming;

/// <summary>
/// Recognizes floating point number in text format and returns a f64.
/// </summary>
public class DoubleParser : IParser<char, double>
{
    private static readonly DoubleSeqParser Main = new();
    /// <inheritdoc />
    public RefTuple<ReadOnlySequence<char>, double> Parse(ReadOnlySequence<char> input)
    {
        var (rest, result) = Main.Parse(input);
        if (double.TryParse(result.ToString(), NumberFormatInfo.InvariantInfo, out var resultDouble))
        {
            return new RefTuple<ReadOnlySequence<char>, double>(rest, resultDouble);
        }

        throw new ParserErrorException($"Coudldn't format as double: {result}.");
    }

    private class DoubleSeqParser : IParser<char, ReadOnlySequence<char>>
    {
        private static readonly TupleThreeParser<char, RefNullable<char>, ReadOnlySequence<char>,
            RefNullable<ReadOnlySequence<char>>> Tuple =
            new(Sign,
                new AltParser<char, ReadOnlySequence<char>>([
                    new NormalFloatParser(),
                    new StartWithDotParser()
                ]),
                new OptParser<char, ReadOnlySequence<char>>(new ExpParser()));

        public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
        {
            var (rest, (sign, d, exp)) = Tuple.Parse(input);
            if (sign.HasValue && exp.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    rest,
                    input.Slice(0, 1 + d.Length + exp.Item.Length));
            }

            if (sign.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    rest,
                    input.Slice(0, 1 + d.Length));
            }

            if (exp.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    rest,
                    input.Slice(0, d.Length + exp.Item.Length));
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                rest, d);
        }
    }

    private static readonly OptParser<char, char> Sign = new(new OneOfParser(['+', '-']));
    private static readonly OneOfParser E = new(['e', 'E']);

    private class ExpParser : IParser<char, ReadOnlySequence<char>>
    {
        private static readonly TupleThreeParser<char, char, RefNullable<char>, ReadOnlySequence<char>> Parser =
            new(E,
                Sign,
                new DigitParser(requireAtLeastOneDigit: true));

        public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
        {
            var (rest, (_, sign, digit)) = Parser.Parse(input);
            if (sign.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, input.Slice(0, digit.Length + 2));
            }
            else
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, input.Slice(0, digit.Length + 1));
            }
        }
    }


    private class OptParser<TIn, TOut>(IParser<TIn, TOut> inner) : IParser<TIn, RefNullable<TOut>>
    {
        public RefTuple<ReadOnlySequence<TIn>, RefNullable<TOut>> Parse(ReadOnlySequence<TIn> input)
        {
            try
            {
                var (rest, result) = inner.Parse(input);
                return new RefTuple<ReadOnlySequence<TIn>, RefNullable<TOut>>(
                    rest,
                    new RefNullable<TOut>(result));
            }
            catch (ParserErrorException)
            {
                return new RefTuple<ReadOnlySequence<TIn>, RefNullable<TOut>>(
                    input,
                    new RefNullable<TOut>());
            }
        }
    }

    private class DotAndAfterParser : IParser<char, ReadOnlySequence<char>>
    {
        private static readonly OptParser<char, ReadOnlySequence<char>> Dot =
            new(new TagParser<char>(new ReadOnlySequence<char>(".".AsMemory())));

        private static readonly OptParser<char, ReadOnlySequence<char>> Digit =
            new(new DigitParser(requireAtLeastOneDigit: true));

        public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
        {
            var (dotRest, dotResult) = Dot.Parse(input);
            if (!dotResult.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    dotRest, ReadOnlySequence<char>.Empty);
            }

            var (digitRest, digitResult) = Digit.Parse(dotRest);
            if (!dotResult.HasValue)
            {
                return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    digitRest, dotResult.Item);
            }

            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                digitRest, input.Slice(0, dotResult.Item.Length + digitResult.Item.Length));
        }
    }

    private class NormalFloatParser : IParser<char, ReadOnlySequence<char>>
    {
        private static readonly TupleTwoParser<char, ReadOnlySequence<char>, RefNullable<ReadOnlySequence<char>>> Tuple =
            new(new DigitParser(requireAtLeastOneDigit: true),
                new OptParser<char, ReadOnlySequence<char>>(new DotAndAfterParser()));

        public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
        {
            var (rest, (digit, dot)) = Tuple.Parse(input);

            return !dot.HasValue
                ? new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, digit)
                : new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    rest,
                    input.Slice(0, digit.Length + dot.Item.Length));
        }
    }

    private class StartWithDotParser : IParser<char, ReadOnlySequence<char>>
    {
        private static readonly TupleTwoParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>> Tuple =
            new(new TagParser<char>(new ReadOnlySequence<char>(".".AsMemory())),
                new DigitParser(requireAtLeastOneDigit: true));

        public RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>> Parse(ReadOnlySequence<char> input)
        {
            var (rest, (dot, digit)) = Tuple.Parse(input);
            return new RefTuple<ReadOnlySequence<char>, ReadOnlySequence<char>>(rest, input.Slice(0, dot.Length + digit.Length));
        }
    }
}