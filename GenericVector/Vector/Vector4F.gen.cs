
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

public readonly partial record struct Vector4F<TScalar> :
    IVectorInternal<Vector4F<TScalar>, TScalar>,
    IFloatingPointVector<Vector4F<TScalar>, TScalar>,
    IVector4<Vector4F<TScalar>, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
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
    public static Vector4F<TScalar> UnitX => new(TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero);
    /// <summary>Gets the vector (0,1,0,0)).</summary>
    /// <value>The vector <c>(0,1,0,0)</c>.</value>
    public static Vector4F<TScalar> UnitY => new(TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero);
    /// <summary>Gets the vector (0,0,1,0)).</summary>
    /// <value>The vector <c>(0,0,1,0)</c>.</value>
    public static Vector4F<TScalar> UnitZ => new(TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero);
    /// <summary>Gets the vector (0,0,0,1)).</summary>
    /// <value>The vector <c>(0,0,0,1)</c>.</value>
    public static Vector4F<TScalar> UnitW => new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One);
    
    /// <summary>Gets a vector whose 4 elements are equal to zero.</summary>
    /// <value>A vector whose  elements are equal to zero (that is, it returns the vector <c>(0,0,0,0)</c>.</value>
    public static Vector4F<TScalar> Zero => new(TScalar.Zero);
    
    /// <summary>Gets a vector whose 4 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector4F{TScalar}" />.</value>
    /// <remarks>A vector whose  elements are equal to one (that is, it returns the vector <c>(1,1,1,1)</c>.</remarks>
    public static Vector4F<TScalar> One => new(TScalar.One);
    
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    public Vector4F(TScalar x, TScalar y, TScalar z, TScalar w)
    {
        Unsafe.SkipInit(out this);
    
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    
    /// <summary>Creates a new <see cref="Vector4F{TScalar}" /> object whose  elements have the same value.</summary>
    /// <param name="value">The value to assign to all  elements.</param>
    public Vector4F(TScalar value) : this(value, value, value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{TScalar}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector4F(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);
    
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));
    
        this = Unsafe.ReadUnaligned<Vector4F<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    
    /// <summary>Creates a new <see cref="Vector4F{TScalar}" /> object from the specified <see cref="Vector4i{TScalar}" /> object Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4F(Vector2D<TScalar> value, TScalar z, TScalar w) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, z, w)
    {
    }
    /// <summary>Creates a new <see cref="Vector4F{TScalar}" /> object from the specified <see cref="Vector4i{TScalar}" /> object W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="w">The W component.</param>
    public Vector4F(Vector3D<TScalar> value, TScalar w) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, , w)
    {
    }
    
    public static Vector4F<TScalar> operator +(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.Add<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator -(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.Subtract<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator *(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.Multiply<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator /(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.Divide<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator %(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.Remainder<Vector4F<TScalar>, TScalar>(left, right);
    
    public static Vector4F<TScalar> operator *(Vector4F<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right);
    public static Vector4F<TScalar> operator /(Vector4F<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector4F<TScalar> operator %(Vector4F<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector4F<TScalar> operator *(TScalar left, Vector4F<TScalar> right) => right * left;
    
    public static Vector4F<TScalar> operator -(Vector4F<TScalar> value) => SpeedHelpers2.Negate<Vector4F<TScalar>, TScalar>(value);
    public static Vector4F<TScalar> operator +(Vector4F<TScalar> value) => value;
    
    public static Vector4F<TScalar> operator &(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator |(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator ^(Vector4F<TScalar> left, Vector4F<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector4F<TScalar>, TScalar>(left, right);
    public static Vector4F<TScalar> operator ~(Vector4F<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector4F<TScalar>, TScalar>(value);
    
    // public static bool operator ==(Vector4F<TScalar> left, Vector4F<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector4F<TScalar> left, Vector4F<TScalar> right) => !(left == right);
    
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
    public static Vector4F<TScalar> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4F<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4F<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector4F<TScalar> result)
        => TryParse(s.AsSpan(), provider, out result);
    
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector4F<TScalar> result)
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
    
        result = new Vector4F<TScalar>(x, y, z, w);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector4F<TScalar> result)
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
    
        result = new Vector4F<TScalar>(x, y, z, w);
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
    
    public bool Equals(Vector4F<TScalar> other) => SpeedHelpers2.Equal<Vector4F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVectorInternal<Vector4F<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y, z, w);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] TScalar IVector<Vector4F<TScalar>, TScalar>.LengthSquared() => Vector4F.LengthSquared(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] Vector4F<TScalar> IVectorEquatable<Vector4F<TScalar>, TScalar>.ScalarsEqual(Vector4F<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector4F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static ReadOnlySpan<TScalar> IVector<Vector4F<TScalar>, TScalar>.AsSpan(Vector4F<TScalar> vec) => vec.AsSpan();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        2 => UnitZ,
        3 => UnitW,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 3")
    };
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Clamp(Vector4F<TScalar> value1, Vector4F<TScalar> min, Vector4F<TScalar> max) => Vector4F.Clamp(value1, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Max(Vector4F<TScalar> value1, Vector4F<TScalar> value2) => Vector4F.Max(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Min(Vector4F<TScalar> value1, Vector4F<TScalar> value2) => Vector4F.Min(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Abs(Vector4F<TScalar> value) => Vector4F.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector4F<TScalar>, TScalar>.DistanceSquared(Vector4F<TScalar> value1, Vector4F<TScalar> value2) => Vector4F.DistanceSquared(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector4F<TScalar>, TScalar>.Dot(Vector4F<TScalar> vector1, Vector4F<TScalar> vector2) => Vector4F.Dot(vector1, vector2);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4F<TScalar>, TScalar>.CopyTo(Vector4F<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4F<TScalar>, TScalar>.CopyTo(Vector4F<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector4F<TScalar>, TScalar>.CopyTo(Vector4F<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static bool IVector<Vector4F<TScalar>, TScalar>.TryCopyTo(Vector4F<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);
    
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector4F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4F<TScalar>>(ref value);
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
    
        result = new Vector4F<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector4F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4F<TScalar>>(ref value);
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
    
        result = new Vector4F<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector4F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector4F<TScalar>>(ref value);
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
    
        result = new Vector4F<TScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector4F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
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
    
        result = new Vector4F<TOtherScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector4F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
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
    
        result = new Vector4F<TOtherScalar>(x, y, z, w);
        return true;
    }
    static bool IVector<Vector4F<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector4F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
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
    
        result = new Vector4F<TOtherScalar>(x, y, z, w);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector<Vector4F<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IVector4<Vector4F<TScalar>, TScalar>.Create(TScalar x, TScalar y, TScalar z, TScalar w) => new(x, y, z, w);


    #region Float-specific code

    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> INumberVector<Vector4F<TScalar>, TScalar>.CopySign(Vector4F<TScalar> value, Vector4F<TScalar> sign) => Vector4F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> INumberVector<Vector4F<TScalar>, TScalar>.CopySign(Vector4F<TScalar> value, TScalar sign) => Vector4F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> INumberVector<Vector4F<TScalar>, TScalar>.Sign(Vector4F<TScalar> value) => Vector4F.Sign(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Normalize(Vector4F<TScalar> value) => Vector4F.Normalize(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Lerp(Vector4F<TScalar> value1, Vector4F<TScalar> value2, Vector4F<TScalar> amount) => Vector4F.Lerp(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.LerpClamped(Vector4F<TScalar> value1, Vector4F<TScalar> value2, Vector4F<TScalar> amount) => Vector4F.LerpClamped(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Reflect(Vector4F<TScalar> vector, Vector4F<TScalar> normal) => Vector4F.Reflect(vector, normal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Sqrt(Vector4F<TScalar> value) => Vector4F.Sqrt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Acosh(Vector4F<TScalar> x) => Vector4F.Acosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Asinh(Vector4F<TScalar> x) => Vector4F.Asinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atanh(Vector4F<TScalar> x) => Vector4F.Atanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Cosh(Vector4F<TScalar> x) => Vector4F.Cosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Sinh(Vector4F<TScalar> x) => Vector4F.Sinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Tanh(Vector4F<TScalar> x) => Vector4F.Tanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Acos(Vector4F<TScalar> x) => Vector4F.Acos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.AcosPi(Vector4F<TScalar> x) => Vector4F.AcosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Asin(Vector4F<TScalar> x) => Vector4F.Asin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.AsinPi(Vector4F<TScalar> x) => Vector4F.AsinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atan(Vector4F<TScalar> x) => Vector4F.Atan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.AtanPi(Vector4F<TScalar> x) => Vector4F.AtanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Cos(Vector4F<TScalar> x) => Vector4F.Cos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.CosPi(Vector4F<TScalar> x) => Vector4F.CosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.DegreesToRadians(Vector4F<TScalar> degrees) => Vector4F.DegreesToRadians(degrees);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.RadiansToDegrees(Vector4F<TScalar> radians) => Vector4F.RadiansToDegrees(radians);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Sin(Vector4F<TScalar> x) => Vector4F.Sin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.SinPi(Vector4F<TScalar> x) => Vector4F.SinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Tan(Vector4F<TScalar> x) => Vector4F.Tan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.TanPi(Vector4F<TScalar> x) => Vector4F.TanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector4F<TScalar> Sin, Vector4F<TScalar> Cos) IFloatingPointVector<Vector4F<TScalar>, TScalar>.SinCos(Vector4F<TScalar> x) => Vector4F.SinCos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector4F<TScalar> SinPi, Vector4F<TScalar> CosPi) IFloatingPointVector<Vector4F<TScalar>, TScalar>.SinCosPi(Vector4F<TScalar> x) => Vector4F.SinCosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log(Vector4F<TScalar> x) => Vector4F.Log(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log(Vector4F<TScalar> x, Vector4F<TScalar> newBase) => Vector4F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log(Vector4F<TScalar> x, TScalar newBase) => Vector4F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.LogP1(Vector4F<TScalar> x) => Vector4F.LogP1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log2(Vector4F<TScalar> x) => Vector4F.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log2P1(Vector4F<TScalar> x) => Vector4F.Log2P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log10(Vector4F<TScalar> x) => Vector4F.Log10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Log10P1(Vector4F<TScalar> x) => Vector4F.Log10P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Exp(Vector4F<TScalar> x) => Vector4F.Exp(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.ExpM1(Vector4F<TScalar> x) => Vector4F.ExpM1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Exp2(Vector4F<TScalar> x) => Vector4F.Exp2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Exp2M1(Vector4F<TScalar> x) => Vector4F.Exp2M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Exp10(Vector4F<TScalar> x) => Vector4F.Exp10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Exp10M1(Vector4F<TScalar> x) => Vector4F.Exp10M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Pow(Vector4F<TScalar> x, Vector4F<TScalar> y) => Vector4F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Pow(Vector4F<TScalar> x, TScalar y) => Vector4F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Cbrt(Vector4F<TScalar> x) => Vector4F.Cbrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Hypot(Vector4F<TScalar> x, Vector4F<TScalar> y) => Vector4F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Hypot(Vector4F<TScalar> x, TScalar y) => Vector4F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.RootN(Vector4F<TScalar> x, int n) => Vector4F.RootN(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Round(Vector4F<TScalar> x) => Vector4F.Round(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Round(Vector4F<TScalar> x, int digits) => Vector4F.Round(x, digits);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Round(Vector4F<TScalar> x, MidpointRounding mode) => Vector4F.Round(x, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Round(Vector4F<TScalar> x, int digits, MidpointRounding mode) => Vector4F.Round(x, digits, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Truncate(Vector4F<TScalar> x) => Vector4F.Truncate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atan2(Vector4F<TScalar> x, Vector4F<TScalar> y) => Vector4F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atan2Pi(Vector4F<TScalar> x, Vector4F<TScalar> y) => Vector4F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atan2(Vector4F<TScalar> x, TScalar y) => Vector4F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.Atan2Pi(Vector4F<TScalar> x, TScalar y) => Vector4F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.BitDecrement(Vector4F<TScalar> x) => Vector4F.BitDecrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.BitIncrement(Vector4F<TScalar> x) => Vector4F.BitIncrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.FusedMultiplyAdd(Vector4F<TScalar> left, Vector4F<TScalar> right, Vector4F<TScalar> addend) => Vector4F.FusedMultiplyAdd(left, right, addend);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.ReciprocalEstimate(Vector4F<TScalar> x) => Vector4F.ReciprocalEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.ReciprocalSqrtEstimate(Vector4F<TScalar> x) => Vector4F.ReciprocalSqrtEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.ScaleB(Vector4F<TScalar> x, Vector2D<int> n) => Vector4F.ScaleB(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector4F<TScalar> IFloatingPointVector<Vector4F<TScalar>, TScalar>.ScaleB(Vector4F<TScalar> x, int n) => Vector4F.ScaleB(x, n);

    #endregion
}