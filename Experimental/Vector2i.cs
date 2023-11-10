using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Unicode;

namespace GenericVector.Experimental;

public readonly partial record struct Vector2i<TScalar> :
    IVectorInternal<Vector2i<TScalar>, TScalar>,
    IBinaryIntegerVector<Vector2i<TScalar>, TScalar>,
    IVector2<Vector2i<TScalar>, TScalar>
    where TScalar : IBinaryInteger<TScalar>
{
    internal const int ElementCount = 2;
    
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }

    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    
    public static Vector2i<TScalar> UnitX => new(TScalar.One, TScalar.Zero);
    public static Vector2i<TScalar> UnitY => new(TScalar.Zero, TScalar.One);

    /// <summary>Gets a vector whose 2 elements are equal to zero.</summary>
    /// <value>A vector whose two elements are equal to zero (that is, it returns the vector <c>(0,0)</c>.</value>
    public static Vector2i<TScalar> Zero => new(TScalar.Zero);

    /// <summary>Gets a vector whose 2 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector2D{T}" />.</value>
    /// <remarks>A vector whose two elements are equal to one (that is, it returns the vector <c>(1,1)</c>.</remarks>
    public static Vector2i<TScalar> One => new(TScalar.One);

    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }

    public Vector2i(TScalar x, TScalar y)
    {
        Unsafe.SkipInit(out this);

        X = x;
        Y = y;
    }
    
    public Vector2i(TScalar value) : this(value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector2i(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));

        this = Unsafe.ReadUnaligned<Vector2i<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }

    public static Vector2i<TScalar> operator +(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Add<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator -(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Subtract<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator *(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Multiply<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator /(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Divide<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator %(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Remainder<Vector2i<TScalar>, TScalar>(left, right);

    public static Vector2i<TScalar> operator *(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right); 
    public static Vector2i<TScalar> operator /(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector2i<TScalar> operator %(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector2i<TScalar> operator *(TScalar left, Vector2i<TScalar> right) => right * left;

    public static Vector2i<TScalar> operator -(Vector2i<TScalar> value) => SpeedHelpers2.Negate<Vector2i<TScalar>, TScalar>(value);
    public static Vector2i<TScalar> operator +(Vector2i<TScalar> value) => value;
    
    public static Vector2i<TScalar> operator &(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator |(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator ^(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator ~(Vector2i<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector2i<TScalar>, TScalar>(value);

    // public static bool operator ==(Vector2i<TScalar> left, Vector2i<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector2i<TScalar> left, Vector2i<TScalar> right) => !(left == right);

    public override string ToString() => ToString("G", null);
    public string ToString(string? format) => ToString(format, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        Span<char> initialBuffer = stackalloc char[Math.Min((2 + (ElementCount - 1) + (separator.Length * (ElementCount - 1)) + (ElementCount * 2)), 256)];

        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new DefaultInterpolatedStringHandler(
            4 + (separator.Length * 2),
            ElementCount,
            formatProvider,
            initialBuffer
        );

        handler.AppendLiteral("<");
        handler.AppendFormatted(X, format);
        handler.AppendLiteral(separator);
        handler.AppendLiteral(" ");
        handler.AppendFormatted(Y, format);
        handler.AppendLiteral(">");

        return handler.ToStringAndClear();
    };

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;

        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            Count,
            destination,
            provider,
            out var shouldAppend
        );
        if (!shouldAppend)
        {
            charsWritten = 0;
            return false;
        }

        // Annoyingly we need to turn the span into a string for the string handler
        string? formatString = format.Length > 0 ? new string(format) : null;

        _ =
            handler.AppendLiteral("<") &&
            handler.AppendFormatted(X, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Y, formatString) &&
            handler.AppendLiteral(">");

        return destination.TryWrite(ref handler, out charsWritten);
    }

    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;

        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new Utf8.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            ElementCount,
            utf8Destination,
            provider,
            out var shouldAppend
        );
        if (!shouldAppend)
        {
            bytesWritten = 0;
            return false;
        }

        // Annoyingly we need to turn the span into a string for the string handler
        string? formatString = format.Length > 0 ? new string(format) : null;
        
        formatString.AsSpan()

        // JIT will automagically convert literals to utf8
        _ =
            handler.AppendLiteral("<") &&
            handler.AppendFormatted(X, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Y, formatString) &&
            handler.AppendLiteral(">");

        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    };

    public static Vector2i<TScalar> Parse(string s, IFormatProvider? provider) => ;
    public static Vector2i<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => ;
    public static Vector2i<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => ;
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector2i<TScalar> result) => ;
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2i<TScalar> result) => ;
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Vector2i<TScalar> result) => ;

    public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
    }
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();

    public bool Equals(Vector2i<TScalar> other) => SpeedHelpers2.Equal<Vector2i<TScalar>, TScalar>(this, other);

    static Vector2i<TScalar> IVectorInternal<Vector2i<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    TScalar IVector<Vector2i<TScalar>, TScalar>.LengthSquared() => Vector2i.LengthSquared(this);
    Vector2i<TScalar> IVectorEquatable<Vector2i<TScalar>, TScalar>.ScalarsEqual(Vector2i<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector2i<TScalar>, TScalar>(this, other);

    static ReadOnlySpan<TScalar> IVector<Vector2i<TScalar>, TScalar>.AsSpan(Vector2i<TScalar> vec) => vec.AsSpan();
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 1")
    };
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Clamp(Vector2i<TScalar> value1, Vector2i<TScalar> min, Vector2i<TScalar> max) => Vector2i.Clamp(value1, min, max);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Max(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.Max(value1, value2);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Min(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.Min(value1, value2);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Abs(Vector2i<TScalar> value) => Vector2i.Abs(value);
    static TScalar IVector<Vector2i<TScalar>, TScalar>.DistanceSquared(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.DistanceSquared(value1, value2);
    static TScalar IVector<Vector2i<TScalar>, TScalar>.Dot(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2) => Vector2i.Dot(vector1, vector2);

    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    static bool IVector<Vector2i<TScalar>, TScalar>.TryCopyTo(Vector2i<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);

    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }

        // For Silk.NET.Maths-provided vectors, where the scalars are the exact same type, and the size is at least as
        // large as the vector type being converted to, we can safely do a bitcast.
        if (value is IVectorInternal && typeof(TScalar) == typeof(TOtherScalar))
        {
            result = Unsafe.As<TOther, Vector2i<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2i<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }

        // For Silk.NET.Maths-provided vectors, where the scalars are the exact same type, and the size is at least as
        // large as the vector type being converted to, we can safely do a bitcast.
        if (value is IVectorInternal && typeof(TScalar) == typeof(TOtherScalar))
        {
            result = Unsafe.As<TOther, Vector2i<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2i<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }

        // For Silk.NET.Maths-provided vectors, where the scalars are the exact same type, and the size is at least as
        // large as the vector type being converted to, we can safely do a bitcast.
        if (value is IVectorInternal && typeof(TScalar) == typeof(TOtherScalar))
        {
            result = Unsafe.As<TOther, Vector2i<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2i<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector2i<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (!ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Y, out var y))
        {
            result = default;
            return false;
        }

        result = default;
        return false;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector2i<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector2i<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }

    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    static Vector2i<TScalar> IVector2<Vector2i<TScalar>, TScalar>.Create(TScalar x, TScalar y) => new(x, y);

    #region Int-specific code

    static bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector2i<TScalar> value)
    {
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var x)) goto Failed;
        source = source[x.GetByteCount()..];
        
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var y)) goto Failed;
        // source = source[x.GetByteCount()..];

        value = new(x, y);
        return true;
        
        Failed:
        value = default;
        return false;
    }
    static bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector2i<TScalar> value)
    {
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var x)) goto Failed;
        source = source[x.GetByteCount()..];
        
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var y)) goto Failed;
        // source = source[y.GetByteCount()..];

        value = new(x, y);
        return true;
        
        Failed:
        value = default;
        return false;
    }
    bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        
        int b;

        if (!X.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];

        if (!Y.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        // destination = destination[b..];

        return true;
        
        Failed:
        bytesWritten += b;
        return false;
    }
    bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        
        int b;

        if (!X.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];

        if (!Y.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        // destination = destination[b..];

        return true;
        
        Failed:
        bytesWritten += b;
        return false;
    }

    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.CopySign(Vector2i<TScalar> value, Vector2i<TScalar> sign) => Vector2i.CopySign(value, sign);
    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.CopySign(Vector2i<TScalar> value, TScalar sign) => Vector2i.CopySign(value, sign);
    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.Sign(Vector2i<TScalar> value) => Vector2i.Sign(value);
    static Vector2i<TScalar> IBinaryNumberVector<Vector2i<TScalar>, TScalar>.Log2(Vector2i<TScalar> value) => Vector2i.Log2(value);
    static Vector2i<TScalar> IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.PopCount(Vector2i<TScalar> value) => Vector2i.PopCount(value);

    #endregion

}