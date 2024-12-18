namespace Nom;

/// <summary>
///
/// </summary>
public readonly ref struct RefNullable<T>
    where T : allows ref struct
{
    /// <summary>
    ///
    /// </summary>
    public RefNullable()
    {
        HasValue = false;
    }

    /// <summary>
    ///
    /// </summary>
    public RefNullable(T item)
    {
        Item = item;
        HasValue = true;
    }

    /// <summary>
    /// The value, if any.
    /// </summary>
    public readonly T Item = default!;

    /// <summary>
    /// True, if this struct holds a value.
    /// </summary>
    public readonly bool HasValue;
}

/// <summary>
/// A tuple that allows the members to be ref structs
/// </summary>
public readonly ref struct RefTuple<T1, T2>(T1 item1, T2 item2)
    where T1 : allows ref struct
    where T2 : allows ref struct
{
    /// <summary>
    /// The tuples first item.
    /// </summary>
    public readonly T1 Item1 = item1;

    /// <summary>
    /// The tuples second item.
    /// </summary>
    public readonly T2 Item2 = item2;

    /// <summary>
    /// Deconstructor for this tuple
    /// </summary>
    public void Deconstruct(out T1 item1, out T2 item2)
    {
        item1 = this.Item1;
        item2 = this.Item2;
    }
}

/// <summary>
/// A 3 tuple that allows the members to be ref structs
/// </summary>
public readonly ref struct RefTuple3<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    where T1 : allows ref struct
    where T2 : allows ref struct
    where T3 : allows ref struct
{
    /// <summary>
    /// The tuples first item.
    /// </summary>
    public readonly T1 Item1 = item1;

    /// <summary>
    /// The tuples second item.
    /// </summary>
    public readonly T2 Item2 = item2;

    /// <summary>
    /// The tuples third item.
    /// </summary>
    public readonly T3 Item3 = item3;

    /// <summary>
    /// Deconstructor for this tuple
    /// </summary>
    public void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
    {
        item1 = this.Item1;
        item2 = this.Item2;
        item3 = this.Item3;
    }
}
