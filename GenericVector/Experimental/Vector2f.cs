using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Unicode;

namespace GenericVector.Experimental;

public readonly partial record struct Vector2f<TScalar> :
    IVectorInternal<Vector2f<TScalar>, TScalar>,
    IFloatingPointVector<Vector2f<TScalar>, TScalar>,
    IVector2<Vector2f<TScalar>, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
{
    internal const int ElementCount = 2;

    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }

    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    
    public static Vector2f<TScalar> UnitX => new(TScalar.One, TScalar.Zero);
    public static Vector2f<TScalar> UnitY => new(TScalar.Zero, TScalar.One);

    /// <summary>Gets a vector whose 2 elements are equal to zero.</summary>
    /// <value>A vector whose two elements are equal to zero (that is, it returns the vector <c>(0,0)</c>.</value>
    public static Vector2f<TScalar> Zero => new(TScalar.Zero);

    /// <summary>Gets a vector whose 2 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector2D{T}" />.</value>
    /// <remarks>A vector whose two elements are equal to one (that is, it returns the vector <c>(1,1)</c>.</remarks>
    public static Vector2f<TScalar> One => new(TScalar.One);

    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }

    public Vector2f(TScalar x, TScalar y)
    {
        Unsafe.SkipInit(out this);

        X = x;
        Y = y;
    }
    
    public Vector2f(TScalar value) : this(value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector2f(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));

        this = Unsafe.ReadUnaligned<Vector2f<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }

    public static Vector2f<TScalar> operator +(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.Add<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator -(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.Subtract<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator *(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.Multiply<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator /(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.Divide<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator %(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.Remainder<Vector2f<TScalar>, TScalar>(left, right);

    public static Vector2f<TScalar> operator *(Vector2f<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right); 
    public static Vector2f<TScalar> operator /(Vector2f<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector2f<TScalar> operator %(Vector2f<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector2f<TScalar> operator *(TScalar left, Vector2f<TScalar> right) => right * left;

    public static Vector2f<TScalar> operator -(Vector2f<TScalar> value) => SpeedHelpers2.Negate<Vector2f<TScalar>, TScalar>(value);
    public static Vector2f<TScalar> operator +(Vector2f<TScalar> value) => value;
    
    public static Vector2f<TScalar> operator &(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator |(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator ^(Vector2f<TScalar> left, Vector2f<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector2f<TScalar>, TScalar>(left, right);
    public static Vector2f<TScalar> operator ~(Vector2f<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector2f<TScalar>, TScalar>(value);

    // public static bool operator ==(Vector2f<TScalar> left, Vector2f<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector2f<TScalar> left, Vector2f<TScalar> right) => !(left == right);

    
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

    public static Vector2i<TScalar> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);

    public static Vector2i<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector2i)}<{typeof(TScalar)}>");

    public static Vector2i<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector2i)}<{typeof(TScalar)}>");

    public static bool TryParse(string? s, IFormatProvider? provider, out Vector2i<TScalar> result)
        => TryParse(s.AsSpan(), provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2i<TScalar> result)
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

            if (!TScalar.TryParse(s[..nextNumber], provider, out x)) return false;

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!TScalar.TryParse(s, provider, out y)) return false;
        }

        result = new Vector2i<TScalar>(x, y);
        return true;
    }

    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector2i<TScalar> result)
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

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!TScalar.TryParse(s, provider, out y)) return false;
        }

        result = new Vector2i<TScalar>(x, y);
        return true;
    }

    public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
    }
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();

    public bool Equals(Vector2f<TScalar> other) => SpeedHelpers2.Equal<Vector2f<TScalar>, TScalar>(this, other);

    static Vector2f<TScalar> IVectorInternal<Vector2f<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    TScalar IVector<Vector2f<TScalar>, TScalar>.LengthSquared() => Vector2f.LengthSquared(this);
    Vector2f<TScalar> IVectorEquatable<Vector2f<TScalar>, TScalar>.ScalarsEqual(Vector2f<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector2f<TScalar>, TScalar>(this, other);

    static ReadOnlySpan<TScalar> IVector<Vector2f<TScalar>, TScalar>.AsSpan(Vector2f<TScalar> vec) => vec.AsSpan();
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 1")
    };
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Clamp(Vector2f<TScalar> value1, Vector2f<TScalar> min, Vector2f<TScalar> max) => Vector2f.Clamp(value1, min, max);
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Max(Vector2f<TScalar> value1, Vector2f<TScalar> value2) => Vector2f.Max(value1, value2);
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Min(Vector2f<TScalar> value1, Vector2f<TScalar> value2) => Vector2f.Min(value1, value2);
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Abs(Vector2f<TScalar> value) => Vector2f.Abs(value);
    static TScalar IVector<Vector2f<TScalar>, TScalar>.DistanceSquared(Vector2f<TScalar> value1, Vector2f<TScalar> value2) => Vector2f.DistanceSquared(value1, value2);
    static TScalar IVector<Vector2f<TScalar>, TScalar>.Dot(Vector2f<TScalar> vector1, Vector2f<TScalar> vector2) => Vector2f.Dot(vector1, vector2);

    static void IVector<Vector2f<TScalar>, TScalar>.CopyTo(Vector2f<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    static void IVector<Vector2f<TScalar>, TScalar>.CopyTo(Vector2f<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    static void IVector<Vector2f<TScalar>, TScalar>.CopyTo(Vector2f<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    static bool IVector<Vector2f<TScalar>, TScalar>.TryCopyTo(Vector2f<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);

    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector2f<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2f<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2f<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector2f<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2f<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2f<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector2f<TScalar> result)
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
            result = Unsafe.As<TOther, Vector2f<TScalar>>(ref value);
            return true;
        }

        if (!ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[1], out var y))
        {
            result = default;
            return false;
        }
        
        result = new Vector2f<TScalar>(x, y);
        return true;
    }
    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector2f<TScalar> value, [MaybeNullWhen(false)] out TOther result)
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
    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector2f<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2f<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }
    static bool IVector<Vector2f<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector2f<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2f<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }

    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    static Vector2f<TScalar> IVector<Vector2f<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    static Vector2f<TScalar> IVector2<Vector2f<TScalar>, TScalar>.Create(TScalar x, TScalar y) => new(x, y);

    #region Float-specific code
    
    static Vector2f<TScalar> INumberVector<Vector2f<TScalar>, TScalar>.CopySign(Vector2f<TScalar> value, Vector2f<TScalar> sign) => Vector2f.CopySign(value, sign);
    static Vector2f<TScalar> INumberVector<Vector2f<TScalar>, TScalar>.CopySign(Vector2f<TScalar> value, TScalar sign) => Vector2f.CopySign(value, sign);
    static Vector2f<TScalar> INumberVector<Vector2f<TScalar>, TScalar>.Sign(Vector2f<TScalar> value) => Vector2f.Sign(value);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Normalize(Vector2f<TScalar> value) => Vector2f.Normalize(value);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Lerp(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) => Vector2f.Lerp(value1, value2, amount);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.LerpClamped(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) => Vector2f.LerpClamped(value1, value2, amount);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Reflect(Vector2f<TScalar> vector, Vector2f<TScalar> normal) => Vector2f.Reflect(vector, normal);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Sqrt(Vector2f<TScalar> value) => Vector2f.Sqrt(value);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Acosh(Vector2f<TScalar> x) => Vector2f.Acosh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Asinh(Vector2f<TScalar> x) => Vector2f.Asinh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atanh(Vector2f<TScalar> x) => Vector2f.Atanh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cosh(Vector2f<TScalar> x) => Vector2f.Cosh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Sinh(Vector2f<TScalar> x) => Vector2f.Sinh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Tanh(Vector2f<TScalar> x) => Vector2f.Tanh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Acos(Vector2f<TScalar> x) => Vector2f.Acos(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AcosPi(Vector2f<TScalar> x) => Vector2f.AcosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Asin(Vector2f<TScalar> x) => Vector2f.Asin(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AsinPi(Vector2f<TScalar> x) => Vector2f.AsinPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan(Vector2f<TScalar> x) => Vector2f.Atan(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AtanPi(Vector2f<TScalar> x) => Vector2f.AtanPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cos(Vector2f<TScalar> x) => Vector2f.Cos(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.CosPi(Vector2f<TScalar> x) => Vector2f.CosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.DegreesToRadians(Vector2f<TScalar> degrees) => Vector2f.DegreesToRadians(degrees);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.RadiansToDegrees(Vector2f<TScalar> radians) => Vector2f.RadiansToDegrees(radians);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Sin(Vector2f<TScalar> x) => Vector2f.Sin(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinPi(Vector2f<TScalar> x) => Vector2f.SinPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Tan(Vector2f<TScalar> x) => Vector2f.Tan(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.TanPi(Vector2f<TScalar> x) => Vector2f.TanPi(x);
    static (Vector2f<TScalar> Sin, Vector2f<TScalar> Cos) IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinCos(Vector2f<TScalar> x) => Vector2f.SinCos(x);
    static (Vector2f<TScalar> SinPi, Vector2f<TScalar> CosPi) IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinCosPi(Vector2f<TScalar> x) => Vector2f.SinCosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x) => Vector2f.Log(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x, Vector2f<TScalar> newBase) => Vector2f.Log(x, newBase);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x, TScalar newBase) => Vector2f.Log(x, newBase);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.LogP1(Vector2f<TScalar> x) => Vector2f.LogP1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log2(Vector2f<TScalar> x) => Vector2f.Log2(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log2P1(Vector2f<TScalar> x) => Vector2f.Log2P1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log10(Vector2f<TScalar> x) => Vector2f.Log10(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log10P1(Vector2f<TScalar> x) => Vector2f.Log10P1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp(Vector2f<TScalar> x) => Vector2f.Exp(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ExpM1(Vector2f<TScalar> x) => Vector2f.ExpM1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp2(Vector2f<TScalar> x) => Vector2f.Exp2(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp2M1(Vector2f<TScalar> x) => Vector2f.Exp2M1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp10(Vector2f<TScalar> x) => Vector2f.Exp10(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp10M1(Vector2f<TScalar> x) => Vector2f.Exp10M1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Pow(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Pow(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Pow(Vector2f<TScalar> x, TScalar y) => Vector2f.Pow(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cbrt(Vector2f<TScalar> x) =>
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Hypot(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Hypot(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Hypot(Vector2f<TScalar> x, TScalar y) => Vector2f.Hypot(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.RootN(Vector2f<TScalar> x, int n) => Vector2f.RootN(x, n);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x) => Vector2f.Round(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, int digits) => Vector2f.Round(x, digits);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, MidpointRounding mode) => Vector2f.Round(x, mode);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, int digits, MidpointRounding mode) => Vector2f.Round(x, digits, mode);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Truncate(Vector2f<TScalar> x) => Vector2f.Truncate(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Atan2(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2Pi(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Atan2Pi(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2(Vector2f<TScalar> x, TScalar y) => Vector2f.Atan2(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2Pi(Vector2f<TScalar> x, TScalar y) => Vector2f.Atan2Pi(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.BitDecrement(Vector2f<TScalar> x) => Vector2f.BitDecrement(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.BitIncrement(Vector2f<TScalar> x) => Vector2f.BitIncrement(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.FusedMultiplyAdd(Vector2f<TScalar> left, Vector2f<TScalar> right, Vector2f<TScalar> addend) => Vector2f.FusedMultiplyAdd(left, right, addend);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ReciprocalEstimate(Vector2f<TScalar> x) => Vector2f.ReciprocalEstimate(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ReciprocalSqrtEstimate(Vector2f<TScalar> x) => Vector2f.ReciprocalSqrtEstimate(x);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.ILogB<TNewVector, TInt>(Vector2f<TScalar> x) => Vector2f.ILogB<TNewVector, TInt>(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ScaleB(Vector2f<TScalar> x, Vector2D<int> n) => Vector2f.ScaleB(x, n);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ScaleB(Vector2f<TScalar> x, int n) => Vector2f.ScaleB(x, n);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.RoundToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.RoundToInt<TNewVector, TInt>(vector);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.FloorToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.FloorToInt<TNewVector, TInt>(vector);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.CeilingToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.CeilingToInt<TNewVector, TInt>(vector);
    
    #endregion
}