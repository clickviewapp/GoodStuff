namespace ClickView.GoodStuff.IdGenerators.Abstractions;

using System;

/// <summary>
/// Id which is based on a <see cref="long"/>
/// </summary>
public readonly struct IdLong: IComparable, IComparable<IdLong>, IEquatable<IdLong>
{
    private readonly long _value;

    /// <summary>
    /// Create a new instance of <see cref="IdLong"/> with the given <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    public IdLong(long value)
    {
        _value = value;
    }

    /// <inheritdoc />
    public int CompareTo(object? value)
    {
        if (value == null)
            return 1;

        // Need to use compare because subtraction will wrap
        // to positive for very large neg numbers, etc.
        if (value is IdLong i)
        {
            if (_value < i._value) return -1;
            if (_value > i._value) return 1;
            return 0;
        }

        throw new ArgumentException("Argument must be IdLong");
    }

    /// <inheritdoc />
    public int CompareTo(IdLong other)
    {
        // Need to use compare because subtraction will wrap
        // to positive for very large neg numbers, etc.
        if (_value < other._value) return -1;
        if (_value > other._value) return 1;
        return 0;
    }

    /// <inheritdoc />
    public bool Equals(IdLong other)
    {
        return _value == other._value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is IdLong other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return "_" + _value.ToString();
    }

    /// <summary>
    /// Implicitly returns the inner <see cref="long"/> value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator long(IdLong value)
    {
        return value._value;
    }

    /// <summary>
    /// Implicitly creates a new <see cref="IdLong"/> from the given <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator IdLong(long value)
    {
        return new IdLong(value);
    }
}
