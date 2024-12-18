using System;
using System.Buffers;

using Nom.Characters;

namespace Nom.Tests.Character;

public class StreamingCTests
{
    [Fact]
    public void OneOfTest()
    {
        {
            var parser = StreamingC.OneOf(['a', 'b', 'c']);
            var (rest, result) = parser(new ReadOnlySequence<char>("b".AsMemory()));
            Assert.True(rest.IsEmpty);
            Assert.Equal('b', result);
        }

        {
            var parser = StreamingC.OneOf(['a', 'b', 'c']);
            var (rest, result) = parser(new ReadOnlySequence<char>("are".AsMemory()));
            Assert.Equal("re", rest.ToString());
            Assert.Equal('a', result);
        }

        Assert.Throws<IncompleteException>(() =>
            StreamingC.OneOf(['a', 'b', 'c'])(new ReadOnlySequence<char>("".AsMemory())));

        Assert.Throws<ParserErrorException>(() =>
            StreamingC.OneOf(['b', 'c'])(new ReadOnlySequence<char>("a".AsMemory())));
    }

    [Fact]
    public void Digit1Test()
    {
        var parser = StreamingC.Digit1();

        {
            var input = new ReadOnlySequence<char>("21c".AsMemory());
            var (rest, result) = parser(input);
            Assert.Equal("c", rest.ToArray());
            Assert.Equal("21", result.ToArray());
        }

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("".AsMemory())));

        Assert.Throws<ParserErrorException>(() => _ = parser(new ReadOnlySequence<char>("c1".AsMemory())));
    }

    [Fact]
    public void Digit0Test()
    {
        var parser = StreamingC.Digit0();

        {
            var input = new ReadOnlySequence<char>("21c".AsMemory());
            var (rest, result) = parser(input);
            Assert.Equal("c", rest.ToArray());
            Assert.Equal("21", result.ToArray());
        }

        {
            var input = new ReadOnlySequence<char>("a21c".AsMemory());
            var (rest, result) = parser(input);
            Assert.Equal("a21c", rest.ToArray());
            Assert.True(result.IsEmpty);
        }

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("".AsMemory())));
    }
}