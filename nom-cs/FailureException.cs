using System;

namespace Nom;

/// <summary>
/// The parser had an error (recoverable)
/// </summary>
public class ParserErrorException(string message) : Exception(message);