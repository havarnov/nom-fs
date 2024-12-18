using System;
using System.Buffers;
using System.Linq;

using Nom.Bytes;

namespace Nom.Tests;

public class MultiTests
{
    [Fact]
    public void SeperatedList0Tests()
    {
        var parser = Multi.SeperatedList0(
            StreamingC.Tag(new ReadOnlySequence<char>("|".AsMemory())),
            StreamingC.Tag(new ReadOnlySequence<char>("abc".AsMemory())));

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("abc|abc|abc".AsMemory()));
            Assert.True(rest.IsEmpty);
            Assert.Equal(["abc", "abc", "abc"], result.Select(i => i.ToString()));
        }

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("abc123abc".AsMemory()));
            Assert.Equal("123abc", rest.ToString());
            Assert.Equal(["abc"], result.Select(i => i.ToString()));
        }

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("abc|def".AsMemory()));
            Assert.Equal("|def", rest.ToString());
            Assert.Equal(["abc"], result.Select(i => i.ToString()));
        }

        Assert.Throws<IncompleteException>(() => _ = parser(new ReadOnlySequence<char>("".AsMemory())));

        {
            var (rest, result) = parser(new ReadOnlySequence<char>("def|abc".AsMemory()));
            Assert.Equal("def|abc", rest.ToString());
            Assert.Empty(result);
        }
    }
}