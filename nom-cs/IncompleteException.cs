using System;

namespace Nom;

/// <summary>
/// There was not enough data
/// </summary>
public class IncompleteException(string message) : Exception(message)
{
    /// <summary>
    /// Instantiate <see cref="IncompleteException"/> with <see cref="NeededBytes"/>.
    /// </summary>
    public IncompleteException(string message, int neededBytes) : this(message)
    {
        NeededBytes = neededBytes;
    }

    /// <summary>
    /// Inidacate how many more bytes that are needed.
    /// </summary>
    public int? NeededBytes { get; private set; }
}