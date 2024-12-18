using System;
using System.Buffers;

using Nom.Bytes;

namespace Nom.Tests;

public class SequenceTests
{
    [Fact]
    public void Tuple2Tests()
    {
        var parser = Sequence.Tuple2(StreamingC.Tag<char>("foo"), StreamingC.Tag<char>("bar"));

        {
            var (rest, (foo, bar)) = parser(new ReadOnlySequence<char>("foobar".AsMemory()));
            Assert.Empty(rest.ToArray());
            Assert.Equal("foo", foo.ToString());
            Assert.Equal("bar", bar.ToString());
        }

        {
            var (rest, (foo, bar)) = parser(new ReadOnlySequence<char>("foobarmore".AsMemory()));
            Assert.Equal("more", rest.ToString());
            Assert.Equal("foo", foo.ToString());
            Assert.Equal("bar", bar.ToString());
        }

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("fooba".AsMemory())));
        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("fo".AsMemory())));
    }
}