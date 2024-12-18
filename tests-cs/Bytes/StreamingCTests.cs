using System;
using System.Buffers;

using Nom.Bytes;

namespace Nom.Tests.Bytes;

public class StreamingCTests
{
    [Fact]
    public void EscapedTests()
    {
        var parser = StreamingC.Escaped(
            Characters.StreamingC.Digit1(),
            '\\',
            Characters.StreamingC.OneOf(['"', 'n', '\\']));

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("123;".AsMemory()));
            Assert.Equal(";", rest.ToString());
            Assert.Equal("123", result.ToString());
        }

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("12\\\"34;".AsMemory()));
            Assert.Equal(";", rest.ToString());
            Assert.Equal("12\\\"34", result.ToString());
        }
    }

    [Fact]
    public void TagSimpleTest()
    {
        var parser = Combinator.Map(
            StreamingC.Tag<char>("Hello"),
            i => new string(i.ToArray()));

        var input = new ReadOnlySequence<char>("Hello, World".AsMemory());
        var (rest, result) = parser(input);

        Assert.Equal("Hello", result);
        Assert.Equal(7, rest.Length);

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("".AsMemory())));
        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("Hell".AsMemory())));

        Assert.Throws<ParserErrorException>(() => _ = parser(new ReadOnlySequence<char>("Something".AsMemory())));
    }

    [Fact]
    public void TakeWhileTest()
    {
        var parser = StreamingC.TakeWhile<byte>(b => b == 0);

        {
            var input = new ReadOnlySequence<byte>([0, 0, 0, 1]);
            var (rest, result) = parser(input);
            Assert.Equal([0, 0, 0], result.ToArray());
            Assert.Equal([1], rest.ToArray());
        }

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<byte>([0, 0, 0, 0])));

        {
            var input = new ReadOnlySequence<byte>([1, 0, 0, 0]);
            var (rest, result) = parser(input);
            Assert.Equal([1, 0, 0, 0], rest.ToArray());
            Assert.Equal([], result.ToArray());
        }
    }
}