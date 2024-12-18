using System.Buffers;

namespace Nom;

/// <summary>
/// The core parser interface.
/// </summary>
public interface IParser<TIn, TOut> where TOut : allows ref struct
{
    /// <summary>
    /// Parse method.
    /// </summary>
    RefTuple<ReadOnlySequence<TIn>, TOut> Parse(ReadOnlySequence<TIn> input);
}