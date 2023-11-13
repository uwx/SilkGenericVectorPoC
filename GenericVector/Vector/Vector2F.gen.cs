
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

public readonly partial record struct Vector2F<TScalar> :
    IVectorInternal<Vector2F<TScalar>, TScalar>,
    IFloatingPointVector<Vector2F<TScalar>, TScalar>,
    IVector2<Vector2F<TScalar>, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
{
    internal const int ElementCount = 2;
    
    
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }
    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    
    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }
    
    
    /// <summary>Gets the vector (1,0)).</summary>
    /// <value>The vector <c>(1,0)</c>.</value>
    public static Vector2F<TScalar> UnitX => new(TScalar.One, TScalar.Zero);
    /// <summary>Gets the vector (0,1)).</summary>
    /// <value>The vector <c>(0,1)</c>.</value>
    public static Vector2F<TScalar> UnitY => new(TScalar.Zero, TScalar.One);
    
    /// <summary>Gets a vector whose 2 elements are equal to zero.</summary>
    /// <value>A vector whose  elements are equal to zero (that is, it returns the vector <c>(0,0)</c>.</value>
    public static Vector2F<TScalar> Zero => new(TScalar.Zero);
    
    /// <summary>Gets a vector whose 2 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector2F{TScalar}" />.</value>
    /// <remarks>A vector whose  elements are equal to one (that is, it returns the vector <c>(1,1)</c>.</remarks>
    public static Vector2F<TScalar> One => new(TScalar.One);
    
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    public Vector2F(TScalar x, TScalar y)
    {
        Unsafe.SkipInit(out this);
    
        X = x;
        Y = y;
    }
    
    /// <summary>Creates a new <see cref="Vector2F{TScalar}" /> object whose  elements have the same value.</summary>
    /// <param name="value">The value to assign to all  elements.</param>
    public Vector2F(TScalar value) : this(value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{TScalar}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector2F(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);
    
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));
    
        this = Unsafe.ReadUnaligned<Vector2F<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    
    
    public static Vector2F<TScalar> operator +(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.Add<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator -(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.Subtract<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator *(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.Multiply<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator /(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.Divide<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator %(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.Remainder<Vector2F<TScalar>, TScalar>(left, right);
    
    public static Vector2F<TScalar> operator *(Vector2F<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right);
    public static Vector2F<TScalar> operator /(Vector2F<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector2F<TScalar> operator %(Vector2F<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector2F<TScalar> operator *(TScalar left, Vector2F<TScalar> right) => right * left;
    
    public static Vector2F<TScalar> operator -(Vector2F<TScalar> value) => SpeedHelpers2.Negate<Vector2F<TScalar>, TScalar>(value);
    public static Vector2F<TScalar> operator +(Vector2F<TScalar> value) => value;
    
    public static Vector2F<TScalar> operator &(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator |(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator ^(Vector2F<TScalar> left, Vector2F<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector2F<TScalar>, TScalar>(left, right);
    public static Vector2F<TScalar> operator ~(Vector2F<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector2F<TScalar>, TScalar>(value);
    
    // public static bool operator ==(Vector2F<TScalar> left, Vector2F<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector2F<TScalar> left, Vector2F<TScalar> right) => !(left == right);
    
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
            handler.AppendLiteral(">");
    
        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector2F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector2F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector2F<TScalar> result)
        => TryParse(s.AsSpan(), provider, out result);
    
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2F<TScalar> result)
    {
        result = default;
    
        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;
    
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
    
        s = s[1..^1];
    
        TScalar? x, y;
    
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
    
            if (!TScalar.TryParse(s, provider, out y)) return false;
        }
    
        result = new Vector2F<TScalar>(x, y);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector2F<TScalar> result)
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
    
        TScalar? x, y;
    
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
    
            if (!TScalar.TryParse(s, provider, out y)) return false;
        }
    
        result = new Vector2F<TScalar>(x, y);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();
    
    public bool Equals(Vector2F<TScalar> other) => SpeedHelpers2.Equal<Vector2F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVectorInternal<Vector2F<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode() => HashCode.Combine(X, Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] TScalar IVector<Vector2F<TScalar>, TScalar>.LengthSquared() => Vector2F.LengthSquared(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] Vector2F<TScalar> IVectorEquatable<Vector2F<TScalar>, TScalar>.ScalarsEqual(Vector2F<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector2F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static ReadOnlySpan<TScalar> IVector<Vector2F<TScalar>, TScalar>.AsSpan(Vector2F<TScalar> vec) => vec.AsSpan();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 1")
    };
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Clamp(Vector2F<TScalar> value1, Vector2F<TScalar> min, Vector2F<TScalar> max) => Vector2F.Clamp(value1, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Max(Vector2F<TScalar> value1, Vector2F<TScalar> value2) => Vector2F.Max(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Min(Vector2F<TScalar> value1, Vector2F<TScalar> value2) => Vector2F.Min(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Abs(Vector2F<TScalar> value) => Vector2F.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector2F<TScalar>, TScalar>.DistanceSquared(Vector2F<TScalar> value1, Vector2F<TScalar> value2) => Vector2F.DistanceSquared(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector2F<TScalar>, TScalar>.Dot(Vector2F<TScalar> vector1, Vector2F<TScalar> vector2) => Vector2F.Dot(vector1, vector2);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector2F<TScalar>, TScalar>.CopyTo(Vector2F<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector2F<TScalar>, TScalar>.CopyTo(Vector2F<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector2F<TScalar>, TScalar>.CopyTo(Vector2F<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static bool IVector<Vector2F<TScalar>, TScalar>.TryCopyTo(Vector2F<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);
    
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector2F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[1], out var y)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector2F<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector2F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[1], out var y)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector2F<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector2F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[1], out var y)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector2F<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector2F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        // if (
        //     !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.X, out var x) ||
        //     !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.Y, out var y)
        // )
        // {
        //     result = default;
        //     return false;
        // }
        //
        // result = TOther.Create(x, y);
        // return true;
    }
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector2F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        // if (
        //     !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.X, out var x) ||
        //     !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.Y, out var y)
        // )
        // {
        //     result = default;
        //     return false;
        // }
        //
        // result = new Vector2F<TOtherScalar>(x, y);
        // return true;
    }
    static bool IVector<Vector2F<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector2F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Y, out var y)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector2F<TOtherScalar>(x, y);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector<Vector2F<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IVector2<Vector2F<TScalar>, TScalar>.Create(TScalar x, TScalar y) => new(x, y);


    #region Float-specific code

    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> INumberVector<Vector2F<TScalar>, TScalar>.CopySign(Vector2F<TScalar> value, Vector2F<TScalar> sign) => Vector2F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> INumberVector<Vector2F<TScalar>, TScalar>.CopySign(Vector2F<TScalar> value, TScalar sign) => Vector2F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> INumberVector<Vector2F<TScalar>, TScalar>.Sign(Vector2F<TScalar> value) => Vector2F.Sign(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Normalize(Vector2F<TScalar> value) => Vector2F.Normalize(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Lerp(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) => Vector2F.Lerp(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.LerpClamped(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) => Vector2F.LerpClamped(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Reflect(Vector2F<TScalar> vector, Vector2F<TScalar> normal) => Vector2F.Reflect(vector, normal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Sqrt(Vector2F<TScalar> value) => Vector2F.Sqrt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Acosh(Vector2F<TScalar> x) => Vector2F.Acosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Asinh(Vector2F<TScalar> x) => Vector2F.Asinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atanh(Vector2F<TScalar> x) => Vector2F.Atanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Cosh(Vector2F<TScalar> x) => Vector2F.Cosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Sinh(Vector2F<TScalar> x) => Vector2F.Sinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Tanh(Vector2F<TScalar> x) => Vector2F.Tanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Acos(Vector2F<TScalar> x) => Vector2F.Acos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.AcosPi(Vector2F<TScalar> x) => Vector2F.AcosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Asin(Vector2F<TScalar> x) => Vector2F.Asin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.AsinPi(Vector2F<TScalar> x) => Vector2F.AsinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atan(Vector2F<TScalar> x) => Vector2F.Atan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.AtanPi(Vector2F<TScalar> x) => Vector2F.AtanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Cos(Vector2F<TScalar> x) => Vector2F.Cos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.CosPi(Vector2F<TScalar> x) => Vector2F.CosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.DegreesToRadians(Vector2F<TScalar> degrees) => Vector2F.DegreesToRadians(degrees);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.RadiansToDegrees(Vector2F<TScalar> radians) => Vector2F.RadiansToDegrees(radians);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Sin(Vector2F<TScalar> x) => Vector2F.Sin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.SinPi(Vector2F<TScalar> x) => Vector2F.SinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Tan(Vector2F<TScalar> x) => Vector2F.Tan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.TanPi(Vector2F<TScalar> x) => Vector2F.TanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector2F<TScalar> Sin, Vector2F<TScalar> Cos) IFloatingPointVector<Vector2F<TScalar>, TScalar>.SinCos(Vector2F<TScalar> x) => Vector2F.SinCos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector2F<TScalar> SinPi, Vector2F<TScalar> CosPi) IFloatingPointVector<Vector2F<TScalar>, TScalar>.SinCosPi(Vector2F<TScalar> x) => Vector2F.SinCosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log(Vector2F<TScalar> x) => Vector2F.Log(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log(Vector2F<TScalar> x, Vector2F<TScalar> newBase) => Vector2F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log(Vector2F<TScalar> x, TScalar newBase) => Vector2F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.LogP1(Vector2F<TScalar> x) => Vector2F.LogP1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log2(Vector2F<TScalar> x) => Vector2F.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log2P1(Vector2F<TScalar> x) => Vector2F.Log2P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log10(Vector2F<TScalar> x) => Vector2F.Log10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Log10P1(Vector2F<TScalar> x) => Vector2F.Log10P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Exp(Vector2F<TScalar> x) => Vector2F.Exp(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.ExpM1(Vector2F<TScalar> x) => Vector2F.ExpM1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Exp2(Vector2F<TScalar> x) => Vector2F.Exp2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Exp2M1(Vector2F<TScalar> x) => Vector2F.Exp2M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Exp10(Vector2F<TScalar> x) => Vector2F.Exp10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Exp10M1(Vector2F<TScalar> x) => Vector2F.Exp10M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Pow(Vector2F<TScalar> x, Vector2F<TScalar> y) => Vector2F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Pow(Vector2F<TScalar> x, TScalar y) => Vector2F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Cbrt(Vector2F<TScalar> x) => Vector2F.Cbrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Hypot(Vector2F<TScalar> x, Vector2F<TScalar> y) => Vector2F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Hypot(Vector2F<TScalar> x, TScalar y) => Vector2F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.RootN(Vector2F<TScalar> x, int n) => Vector2F.RootN(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Round(Vector2F<TScalar> x) => Vector2F.Round(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Round(Vector2F<TScalar> x, int digits) => Vector2F.Round(x, digits);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Round(Vector2F<TScalar> x, MidpointRounding mode) => Vector2F.Round(x, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Round(Vector2F<TScalar> x, int digits, MidpointRounding mode) => Vector2F.Round(x, digits, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Truncate(Vector2F<TScalar> x) => Vector2F.Truncate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atan2(Vector2F<TScalar> x, Vector2F<TScalar> y) => Vector2F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atan2Pi(Vector2F<TScalar> x, Vector2F<TScalar> y) => Vector2F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atan2(Vector2F<TScalar> x, TScalar y) => Vector2F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.Atan2Pi(Vector2F<TScalar> x, TScalar y) => Vector2F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.BitDecrement(Vector2F<TScalar> x) => Vector2F.BitDecrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.BitIncrement(Vector2F<TScalar> x) => Vector2F.BitIncrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.FusedMultiplyAdd(Vector2F<TScalar> left, Vector2F<TScalar> right, Vector2F<TScalar> addend) => Vector2F.FusedMultiplyAdd(left, right, addend);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.ReciprocalEstimate(Vector2F<TScalar> x) => Vector2F.ReciprocalEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.ReciprocalSqrtEstimate(Vector2F<TScalar> x) => Vector2F.ReciprocalSqrtEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.ScaleB(Vector2F<TScalar> x, Vector2D<int> n) => Vector2F.ScaleB(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector2F<TScalar> IFloatingPointVector<Vector2F<TScalar>, TScalar>.ScaleB(Vector2F<TScalar> x, int n) => Vector2F.ScaleB(x, n);

    #endregion
}