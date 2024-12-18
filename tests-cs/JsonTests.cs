using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

using Nom.Bytes;
using Nom.Bytes.Streaming;
using Nom.Characters.Streaming;
using Nom.Number.Streaming;

namespace Nom.Tests;

public class JsonTests
{
    [Fact]
    public void ClassBasedTest()
    {
        char[] spaceCharacters = [' ', '\t', '\n', '\r'];
        var spaceParser = new TakeWhileParser<char>(c => spaceCharacters.Contains(c));
        var quoteParser = new TagParser<char>(new ReadOnlySequence<char>("\"".AsMemory()));
        var stringParser =
            new MapParser<char, ReadOnlySequence<char>, string>(
                new DelimitedParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>, ReadOnlySequence<char>>(
                    quoteParser,
                    new EscapedParser<char, ReadOnlySequence<char>, char>(
                        new AlphaNumericParser(requireAtLestOne: true),
                        '\\',
                        new OneOfParser(['"', 'n', '\\'])),
                    quoteParser),
                seq => seq.ToString());

        var jTrue = new MapParser<char, ReadOnlySequence<char>, Json>(
            new TagParser<char>(new ReadOnlySequence<char>("true".AsMemory())),
            Json (_) => new Json.Boolean { Value = true });

        var jFalse = new MapParser<char, ReadOnlySequence<char>, Json>(
            new TagParser<char>(new ReadOnlySequence<char>("false".AsMemory())),
            Json (_) => new Json.Boolean { Value = false });

        var jNull = new MapParser<char, ReadOnlySequence<char>, Json>(
            new TagParser<char>(new ReadOnlySequence<char>("null".AsMemory())),
            Json (_) => new Json.Null());

        var jString = new MapParser<char, string, Json>(
            stringParser,
            Json (str) => new Json.Str() { Value = str, });

        var jNum = new MapParser<char, double, Json>(
            new DoubleParser(),
            Json (v) => new Json.Num { Value = v, });

        List<IParser<char, Json>> parsers =
        [
            jTrue,
            jFalse,
            jNull,
            jNum,
            jString,
            // added when available
            // hashParser,
            // arrayParser,
        ];

        var valueParser = new PrecededParser<char, ReadOnlySequence<char>, Json>(
            spaceParser,
            new AltParser<char, Json>(parsers));

        var comma = new TagParser<char>(new ReadOnlySequence<char>(",".AsMemory()));

        var arrayStart = new TagParser<char>(new ReadOnlySequence<char>("[".AsMemory()));
        var arrayStop = new TagParser<char>(new ReadOnlySequence<char>("]".AsMemory()));

        var jArray =
            new MapParser<char, List<Json>, Json>(
                new DelimitedParser<char, ReadOnlySequence<char>, List<Json>, ReadOnlySequence<char>>(
                    arrayStart,
                    new SeperatedListParser<char, ReadOnlySequence<char>, Json>(
                        valueParser,
                        new PrecededParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>>(
                            spaceParser,
                            comma),
                        requireAtLeastOne: false),
                    new PrecededParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>>(
                        spaceParser,
                        arrayStop)),
                Json (i) => new Json.Array { Value = i });

        var colon = new TagParser<char>(new ReadOnlySequence<char>(":".AsMemory()));
        var kvParser = new MapParser<char, RefTuple<string, Json>, (string, Json)>(
            new SeperatedPairParser<char, string, ReadOnlySequence<char>, Json>(
                new PrecededParser<char, ReadOnlySequence<char>, string>(spaceParser, stringParser),
                new PrecededParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>>(spaceParser, colon),
                new PrecededParser<char, ReadOnlySequence<char>, Json>(spaceParser, valueParser)),
            i => (i.Item1, i.Item2));

        var curlyStart = new TagParser<char>(new ReadOnlySequence<char>("{".AsMemory()));
        var curlyStop = new TagParser<char>(new ReadOnlySequence<char>("}".AsMemory()));
        var jObject = new MapParser<char, List<(string, Json)>, Json>(
            new DelimitedParser<char, ReadOnlySequence<char>, List<(string, Json)>, ReadOnlySequence<char>>(
                curlyStart,
                new SeperatedListParser<char, ReadOnlySequence<char>, (string, Json)>(
                    kvParser,
                    new PrecededParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>>(spaceParser, comma),
                    requireAtLeastOne: false),
                new PrecededParser<char, ReadOnlySequence<char>, ReadOnlySequence<char>>(spaceParser, curlyStop)),
            Json (i) => new Json.JObject() { Value = i.ToDictionary(kv => kv.Item1, kv => kv.Item2), });

        parsers.Add(jObject);
        parsers.Add(jArray);

        var jParser = new PrecededParser<char, ReadOnlySequence<char>, Json>(
            spaceParser,
            new AltParser<char, Json>([jObject, jArray]));

        var input = """

                  [
                  
                      "foo",
                  
                      {
                          "ball": true,
                          "snall": "hav",
                          
                          "ja":     1234.123e-12,
                  
                          "nei": 1234.123E+12
                  
                      },
                  
                      false

                  ]


                  """;
        var (rest, result) = jParser.Parse(new ReadOnlySequence<char>(input.AsMemory()));
        Assert.Equal("\n\n", rest.ToString());

        var jresult = Assert.IsType<Json.Array>(result);
        Assert.Equal(3, jresult.Value.Count);
    }

    [Fact]
    public void SimpleTest()
    {
        char[] spaceCharacters = [' ', '\t', '\n', '\r'];
        var spaceParser = StreamingC.TakeWhile<char>(c => spaceCharacters.Contains(c));
        var stringInternalParser = StreamingC.Escaped(
            Characters.StreamingC.AlphaNumeric1(),
            '\\',
            Characters.StreamingC.OneOf(['"', 'n', '\\']));
        var bs = StreamingC.Tag(new ReadOnlySequence<char>("\"".AsMemory()));
        var stringParser = Combinator.Map(
            Sequence.Tuple3(
                bs,
                stringInternalParser,
                bs),
            i =>
            {
                var (_, str, _) = i;
                return str.ToString();
            });
        var jTrue = Combinator.Map(StreamingC.Tag(new ReadOnlySequence<char>("true".AsMemory())), Json (_) => new Json.Boolean { Value = true });
        var jFalse = Combinator.Map(StreamingC.Tag(new ReadOnlySequence<char>("false".AsMemory())), Json (_) => new Json.Boolean { Value = false });
        var jNull = Combinator.Map(StreamingC.Tag(new ReadOnlySequence<char>("null".AsMemory())), Json (_) => new Json.Null());
        var jString = Combinator.Map(stringParser, Json (s) => new Json.Str() { Value = s, });

        List<Func<ReadOnlySequence<char>, RefTuple<ReadOnlySequence<char>, Json>>> parsers =
        [
            jTrue,
            jFalse,
            jNull,
            jString,
            // added when available
            // hashParser,
            // arrayParser,
        ];
        var valueParser =
            Sequence.Preceded(
                spaceParser,
                Branch.Alt(parsers));

        var arrayStart = StreamingC.Tag(new ReadOnlySequence<char>("[".AsMemory()));
        var arrayStop = StreamingC.Tag(new ReadOnlySequence<char>("]".AsMemory()));
        var sep = StreamingC.Tag(new ReadOnlySequence<char>(",".AsMemory()));
        var arrayParser =
            Combinator.Map(
                Sequence.Delimited(
                    arrayStart,
                    Multi.SeperatedList0(
                        Sequence.Preceded(spaceParser, sep),
                        valueParser),
                    Sequence.Preceded(spaceParser, arrayStop)),
                Json (i) => new Json.Array() { Value = i, });

        var kvSep = StreamingC.Tag(new ReadOnlySequence<char>(":".AsMemory()));
        var kvParser =
            Combinator.Map(
                Sequence.SeperatedPair(
                    Sequence.Preceded(spaceParser, stringParser),
                    Sequence.Preceded(spaceParser, kvSep),
                    Sequence.Preceded(spaceParser, valueParser)),
                i => (i.Item1, i.Item2));

        var hashStart = StreamingC.Tag(new ReadOnlySequence<char>("{".AsMemory()));
        var hashStop = StreamingC.Tag(new ReadOnlySequence<char>("}".AsMemory()));
        var hashParser =
            Combinator.Map(
                Sequence.Delimited(
                    hashStart,
                    Multi.SeperatedList0(
                        Sequence.Preceded(spaceParser, sep),
                        kvParser),
                    Sequence.Preceded(spaceParser, hashStop)),
                Json (i) => new Json.JObject() { Value = i.ToDictionary(kv => kv.Item1, kv => kv.Item2), });

        parsers.Add(hashParser);
        parsers.Add(arrayParser);

        var jParser = Sequence.Preceded(
            spaceParser,
            Branch.Alt([hashParser, arrayParser]));

        var input = """

                  [
                  
                      "foo",
                  
                      {
                          "ball": true,
                          "snall": "hav"
                      },
                  
                      false

                  ]


                  """;
        var (rest, result) = jParser(new ReadOnlySequence<char>(input.AsMemory()));
        Assert.Equal("\n\n", rest.ToString());

        var jresult = Assert.IsType<Json.Array>(result);
        Assert.Equal(3, jresult.Value.Count);
    }

    private abstract class Json
    {
        private Json()
        {
        }

        public class Null : Json;

        public class Str : Json
        {
            public string Value { get; init; }
        }

        public class Boolean : Json
        {
            public bool Value { get; init; }
        }

        public class Num : Json
        {
            public double Value { get; init; }
        }

        public class Array : Json
        {
            public List<Json> Value { get; init; }
        }

        public class JObject : Json
        {
            public Dictionary<string, Json> Value { get; init; }
        }
    }
}