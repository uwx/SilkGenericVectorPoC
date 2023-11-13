
using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Unicode;

namespace GenericVector.Experimental;

public readonly partial record struct Vector4I<TScalar> :
    IVectorInternal<Vector4I<TScalar>, TScalar>,
    IBinaryIntegerVector<Vector4I<TScalar>, TScalar>,
    IVector4<Vector4I<TScalar>, TScalar>
    where TScalar : IBinaryInteger<TScalar>
{
    internal const int ElementCount = 4;
    
    
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }
    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    /// <summary>The Z component of the vector.</summary>
    [DataMember]
    public TScalar Z { get; }
    /// <summary>The W component of the vector.</summary>
    [DataMember]
    public TScalar W { get; }
    
    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }
    
    
    /// <summary>Gets the vector (1,0,0,0)).</summary>
    /// <value>The vector <c>(1,0,0,0)</c>.</value>
    public static Vector4I<TScalar> UnitX => new(TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero);
    /// <summary>Gets the vector (0,1,0,0)).</summary>
    /// <value>The vector <c>(0,1,0,0)</c>.</value>
    public static Vector4I<TScalar> UnitY => new(TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero);
    /// <summary>Gets the vector (0,0,1,0)).</summary>
    /// <value>The vector <c>(0,0,1,0)</c>.</value>
    public static Vector4I<TScalar> UnitZ => new(TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero);
    /// <summary>Gets the vector (0,0,0,1)).</summary>
    /// <value>The vector <c>(0,0,0,1)</c>.</value>
    public static Vector4I<TScalar> UnitW => new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One);
    
    /// <summary>Gets a vector whose 4 elements are equal to zero.</summary>
    /// <value>A vector whose  elements are equal to zero (that is, it returns the vector <c>(0,0,0,0)</c>.</value>
    public static Vector4I<TScalar> Zero => new(TScalar.Zero);
    
    /// <summary>Gets a vector whose 4 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector4I{TScalar}" />.</value>
    /// <remarks>A vector whose  elements are equal to one (that is, it returns the vector <c>(1,1,1,1)</c>.</remarks>
    public static Vector4I<TScalar> One => new(TScalar.One);
    
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    public Vector4I(TScalar x, TScalar y, TScalar z, TScalar w)
    {
        Unsafe.SkipInit(out this);
    
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    
    /// <summary>Creates a new <see cref="Vector4I{TScalar}" /> object whose  elements have the same value.</summary>
    /// <param name="value">The value to assign to all  elements.</param>
    public Vector4I(TScalar value) : this(value, value, value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{TScalar}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector4I(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);
    
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));
    
        this = Unsafe.ReadUnaligned<Vector4I<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    
    /// <summary>Creates a new <see cref="Vector4I{TScalar}" /> object from the specified <see cref="Vector4i{TScalar}" /> object Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4I(Vector2D<TScalar> value, TScalar z, TScalar w) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, z, w)
    {
    }
    /// <summary>Creates a new <see cref="Vector4I{TScalar}" /> object from the specified <see cref="Vector4i{TScalar}" /> object W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="w">The W component.</param>
    public Vector4I(Vector3D<TScalar> value, TScalar w) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, , w)
    {
    }
    
    public static Vector4I<TScalar> operator +(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.Add<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator -(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.Subtract<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator *(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.Multiply<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator /(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.Divide<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator %(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.Remainder<Vector4I<TScalar>, TScalar>(left, right);
    
    public static Vector4I<TScalar> operator *(Vector4I<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right);
    public static Vector4I<TScalar> operator /(Vector4I<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector4I<TScalar> operator %(Vector4I<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector4I<TScalar> operator *(TScalar left, Vector4I<TScalar> right) => right * left;
    
    public static Vector4I<TScalar> operator -(Vector4I<TScalar> value) => SpeedHelpers2.Negate<Vector4I<TScalar>, TScalar>(value);
    public static Vector4I<TScalar> operator +(Vector4I<TScalar> value) => value;
    
    public static Vector4I<TScalar> operator &(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator |(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator ^(Vector4I<TScalar> left, Vector4I<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector4I<TScalar>, TScalar>(left, right);
    public static Vector4I<TScalar> operator ~(Vector4I<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector4I<TScalar>, TScalar>(value);
    
    // public static bool operator ==(Vector4I<TScalar> left, Vector4I<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector4I<TScalar> left, Vector4I<TScalar> right) => !(left == right);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public override string ToString() => ToString("G", null);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);
    
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
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
        handler.AppendLiteral(separator);
        handler.AppendLiteral(" ");
        handler.AppendFormatted(Z, format);
        handler.AppendLiteral(separator);
        handler.AppendLiteral(" ");
        handler.AppendFormatted(W, format);
        handler.AppendLiteral(">");
    
        return handler.ToStringAndClear();
    }
    
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;
    
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
    
        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            ElementCount,
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
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Z, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(W, formatString) &&
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
    
        // JIT will automagically convert literals to utf8
        _ =
            handler.AppendLiteral("<") &&
            handler.AppendFormatted(X, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Y, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Z, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(W, formatString) &&
            handler.AppendLiteral(">");
    
        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4I<TScalar> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4I<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4I)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4I<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4I)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector4I<TScalar> result)
        => TryParse(s.AsSpan(), provider, out result);
    
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector4I<TScalar> result)
    {
        result = default;
    
        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;
    
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
    
        s = s[1..^1];
    
        TScalar? x, y, z, w;
    
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber],  provider, out x)) return false;
    
            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber],  provider, out y)) return false;
    
            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber],  provider, out z)) return false;
    
            s = s[(nextNumber + separator.Length)..];
        }
    
        {
            if (s.Length == 0) return false;
    
            if (!TScalar.TryParse(s, provider, out w)) return false;
        }
    
        result = new Vector4I<TScalar>(x, y, z, w);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector4I<TScalar> result)
    {
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
    
        Span<byte> separatorSpan = stackalloc byte[Encoding.UTF8.GetByteCount(separator)];
        if (Utf8.FromUtf16(separator, separatorSpan, out var charsRead, out var bytesWritten, isFinalBlock: true) != OperationStatus.Done)
        {
            result = default;
            return false;
        }
    
        result = default;
    
        if (s[0] != (byte)'<') return false;
        if (s[^1] != (byte)'>') return false;
    
        s = s[1..^1];
    
        TScalar? x, y, z, w;
    
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separatorSpan);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber], provider, out x)) return false;
    
            s = s[(nextNumber + separatorSpan.Length)..];
        }
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separatorSpan);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber], provider, out y)) return false;
    
            s = s[(nextNumber + separatorSpan.Length)..];
        }
        {
            if (s.Length == 0) return false;
    
            var nextNumber = s.IndexOf(separatorSpan);
            if (nextNumber == -1)
            {
                return false;
            }
    
            if (!TScalar.TryParse(s[..nextNumber], provider, out z)) return false;
    
            s = s[(nextNumber + separatorSpan.Length)..];
        }
    
        {
            if (s.Length == 0) return false;
    
            if (!TScalar.TryParse(s, provider, out w)) return false;
        }
    
        result = new Vector4I<TScalar>(x, y, z, w);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();
    
    public bool Equals(Vector4I<TScalar> other) => SpeedHelpers2.Equal<Vector4I<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVectorInternal<Vector4I<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y, z, w);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] TScalar IVector<Vector4I<TScalar>, TScalar>.LengthSquared() => Vector4I.LengthSquared(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] Vector4I<TScalar> IVectorEquatable<Vector4I<TScalar>, TScalar>.ScalarsEqual(Vector4I<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector4I<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static ReadOnlySpan<TScalar> IVector<Vector4I<TScalar>, TScalar>.AsSpan(Vector4I<TScalar> vec) => vec.AsSpan();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        2 => UnitZ,
        3 => UnitW,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 3")
    };
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Clamp(Vector4I<TScalar> value1, Vector4I<TScalar> min, Vector4I<TScalar> max) => Vector4I.Clamp(value1, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Max(Vector4I<TScalar> value1, Vector4I<TScalar> value2) => Vector4I.Max(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Min(Vector4I<TScalar> value1, Vector4I<TScalar> value2) => Vector4I.Min(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Abs(Vector4I<TScalar> value) => Vector4I.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector4I<TScalar>, TScalar>.DistanceSquared(Vector4I<TScalar> value1, Vector4I<TScalar> value2) => Vector4I.DistanceSquared(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector4I<TScalar>, TScalar>.Dot(Vector4I<TScalar> vector1, Vector4I<TScalar> vector2) => Vector4I.Dot(vector1, vector2);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4I<TScalar>, TScalar>.CopyTo(Vector4I<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4I<TScalar>, TScalar>.CopyTo(Vector4I<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4I<TScalar>, TScalar>.CopyTo(Vector4I<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static bool IVector<Vector4I<TScalar>, TScalar>.TryCopyTo(Vector4I<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);
    
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector4I<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4I<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[2], out var z) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[3], out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector4I<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4I<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[2], out var z) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[3], out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector4I<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4I<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[2], out var z) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[3], out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector4I<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.Z, out var z) ||
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.W, out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TOtherScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector4I<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.Z, out var z) ||
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.W, out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TOtherScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4I<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector4I<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Z, out var z) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.W, out var w)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector4I<TOtherScalar>(x, y, z, w);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector<Vector4I<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IVector4<Vector4I<TScalar>, TScalar>.Create(TScalar x, TScalar y, TScalar z, TScalar w) => new(x, y, z, w);


    #region Int-specific code

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IBinaryIntegerVector<Vector4I<TScalar>, TScalar>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector4I<TScalar> value)
    {
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var x)) goto Failed;
        source = source[x.GetByteCount()..];
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var y)) goto Failed;
        source = source[y.GetByteCount()..];
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var z)) goto Failed;
        source = source[z.GetByteCount()..];
        if (!TScalar.TryReadBigEndian(source, isUnsigned, out var w)) goto Failed;
        

        value = new(x, y, z, w);
        return true;

        Failed:
        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IBinaryIntegerVector<Vector4I<TScalar>, TScalar>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector4I<TScalar> value)
    {
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var x)) goto Failed;
        source = source[x.GetByteCount()..];
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var y)) goto Failed;
        source = source[y.GetByteCount()..];
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var z)) goto Failed;
        source = source[z.GetByteCount()..];
        if (!TScalar.TryReadLittleEndian(source, isUnsigned, out var w)) goto Failed;
        
        value = new(x, y, z, w);
        return true;

        Failed:
        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IBinaryIntegerVector<Vector4I<TScalar>, TScalar>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;

        int b;

        if (!X.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!Y.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!Z.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!W.TryWriteBigEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        

        return true;

        Failed:
        bytesWritten += b;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IBinaryIntegerVector<Vector4I<TScalar>, TScalar>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;

        int b;

        if (!X.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!Y.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!Z.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        destination = destination[b..];
        if (!W.TryWriteLittleEndian(destination, out b)) goto Failed;
        bytesWritten += b;
        

        return true;

        Failed:
        bytesWritten += b;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> INumberVector<Vector4I<TScalar>, TScalar>.CopySign(Vector4I<TScalar> value, Vector4I<TScalar> sign) => Vector4I.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> INumberVector<Vector4I<TScalar>, TScalar>.CopySign(Vector4I<TScalar> value, TScalar sign) => Vector4I.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> INumberVector<Vector4I<TScalar>, TScalar>.Sign(Vector4I<TScalar> value) => Vector4I.Sign(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IBinaryNumberVector<Vector4I<TScalar>, TScalar>.Log2(Vector4I<TScalar> value) => Vector4I.Log2(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4I<TScalar> IBinaryIntegerVector<Vector4I<TScalar>, TScalar>.PopCount(Vector4I<TScalar> value) => Vector4I.PopCount(value);

    #endregion
}