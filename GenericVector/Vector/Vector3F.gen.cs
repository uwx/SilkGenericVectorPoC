
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

public readonly partial record struct Vector3F<TScalar> :
    IVectorInternal<Vector3F<TScalar>, TScalar>,
    IFloatingPointVector<Vector3F<TScalar>, TScalar>,
    IVector3<Vector3F<TScalar>, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
{
    internal const int ElementCount = 3;
    
    
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }
    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    /// <summary>The Z component of the vector.</summary>
    [DataMember]
    public TScalar Z { get; }
    
    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }
    
    
    /// <summary>Gets the vector (1,0,0)).</summary>
    /// <value>The vector <c>(1,0,0)</c>.</value>
    public static Vector3F<TScalar> UnitX => new(TScalar.One, TScalar.Zero, TScalar.Zero);
    /// <summary>Gets the vector (0,1,0)).</summary>
    /// <value>The vector <c>(0,1,0)</c>.</value>
    public static Vector3F<TScalar> UnitY => new(TScalar.Zero, TScalar.One, TScalar.Zero);
    /// <summary>Gets the vector (0,0,1)).</summary>
    /// <value>The vector <c>(0,0,1)</c>.</value>
    public static Vector3F<TScalar> UnitZ => new(TScalar.Zero, TScalar.Zero, TScalar.One);
    
    /// <summary>Gets a vector whose 3 elements are equal to zero.</summary>
    /// <value>A vector whose  elements are equal to zero (that is, it returns the vector <c>(0,0,0)</c>.</value>
    public static Vector3F<TScalar> Zero => new(TScalar.Zero);
    
    /// <summary>Gets a vector whose 3 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector3F{TScalar}" />.</value>
    /// <remarks>A vector whose  elements are equal to one (that is, it returns the vector <c>(1,1,1)</c>.</remarks>
    public static Vector3F<TScalar> One => new(TScalar.One);
    
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    public Vector3F(TScalar x, TScalar y, TScalar z)
    {
        Unsafe.SkipInit(out this);
    
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>Creates a new <see cref="Vector3F{TScalar}" /> object whose  elements have the same value.</summary>
    /// <param name="value">The value to assign to all  elements.</param>
    public Vector3F(TScalar value) : this(value, value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{TScalar}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector3F(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);
    
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, ElementCount, nameof(values));
    
        this = Unsafe.ReadUnaligned<Vector3F<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    
    /// <summary>Creates a new <see cref="Vector3F{TScalar}" /> object from the specified <see cref="Vector3i{TScalar}" /> object Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="z">The Z component.</param>
    public Vector3F(Vector2D<TScalar> value, TScalar z) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, z)
    {
    }
    
    public static Vector3F<TScalar> operator +(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.Add<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator -(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.Subtract<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator *(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.Multiply<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator /(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.Divide<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator %(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.Remainder<Vector3F<TScalar>, TScalar>(left, right);
    
    public static Vector3F<TScalar> operator *(Vector3F<TScalar> left, TScalar right) => SpeedHelpers2.Multiply(left, right);
    public static Vector3F<TScalar> operator /(Vector3F<TScalar> left, TScalar right) => SpeedHelpers2.Divide(left, right);
    public static Vector3F<TScalar> operator %(Vector3F<TScalar> left, TScalar right) => SpeedHelpers2.Remainder(left, right);
    
    public static Vector3F<TScalar> operator *(TScalar left, Vector3F<TScalar> right) => right * left;
    
    public static Vector3F<TScalar> operator -(Vector3F<TScalar> value) => SpeedHelpers2.Negate<Vector3F<TScalar>, TScalar>(value);
    public static Vector3F<TScalar> operator +(Vector3F<TScalar> value) => value;
    
    public static Vector3F<TScalar> operator &(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator |(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator ^(Vector3F<TScalar> left, Vector3F<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector3F<TScalar>, TScalar>(left, right);
    public static Vector3F<TScalar> operator ~(Vector3F<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector3F<TScalar>, TScalar>(value);
    
    // public static bool operator ==(Vector3F<TScalar> left, Vector3F<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector3F<TScalar> left, Vector3F<TScalar> right) => !(left == right);
    
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
            handler.AppendLiteral(">");
    
        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3F<TScalar> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3F<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector3F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3F<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector3F)}<{typeof(TScalar)}>");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector3F<TScalar> result)
        => TryParse(s.AsSpan(), provider, out result);
    
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector3F<TScalar> result)
    {
        result = default;
    
        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;
    
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
    
        s = s[1..^1];
    
        TScalar? x, y, z;
    
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
    
            if (!TScalar.TryParse(s, provider, out z)) return false;
        }
    
        result = new Vector3F<TScalar>(x, y, z);
        return true;
    }
    
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector3F<TScalar> result)
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
    
        TScalar? x, y, z;
    
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
    
            if (!TScalar.TryParse(s, provider, out z)) return false;
        }
    
        result = new Vector3F<TScalar>(x, y, z);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();
    
    public bool Equals(Vector3F<TScalar> other) => SpeedHelpers2.Equal<Vector3F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVectorInternal<Vector3F<TScalar>, TScalar>.CreateInternal(TScalar x, TScalar y, TScalar z, TScalar w, TScalar v) => new(x, y, z);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] TScalar IVector<Vector3F<TScalar>, TScalar>.LengthSquared() => Vector3F.LengthSquared(this);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] Vector3F<TScalar> IVectorEquatable<Vector3F<TScalar>, TScalar>.ScalarsEqual(Vector3F<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector3F<TScalar>, TScalar>(this, other);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static ReadOnlySpan<TScalar> IVector<Vector3F<TScalar>, TScalar>.AsSpan(Vector3F<TScalar> vec) => vec.AsSpan();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        2 => UnitZ,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 2")
    };
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Clamp(Vector3F<TScalar> value1, Vector3F<TScalar> min, Vector3F<TScalar> max) => Vector3F.Clamp(value1, min, max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Max(Vector3F<TScalar> value1, Vector3F<TScalar> value2) => Vector3F.Max(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Min(Vector3F<TScalar> value1, Vector3F<TScalar> value2) => Vector3F.Min(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Abs(Vector3F<TScalar> value) => Vector3F.Abs(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector3F<TScalar>, TScalar>.DistanceSquared(Vector3F<TScalar> value1, Vector3F<TScalar> value2) => Vector3F.DistanceSquared(value1, value2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static TScalar IVector<Vector3F<TScalar>, TScalar>.Dot(Vector3F<TScalar> vector1, Vector3F<TScalar> vector2) => Vector3F.Dot(vector1, vector2);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector3F<TScalar>, TScalar>.CopyTo(Vector3F<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector3F<TScalar>, TScalar>.CopyTo(Vector3F<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static void IVector<Vector3F<TScalar>, TScalar>.CopyTo(Vector3F<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static bool IVector<Vector3F<TScalar>, TScalar>.TryCopyTo(Vector3F<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);
    
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector3F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector3F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertChecked<TOtherScalar, TScalar>(value[2], out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TScalar>(x, y, z);
        return true;
    }
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector3F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector3F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertSaturating<TOtherScalar, TScalar>(value[2], out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TScalar>(x, y, z);
        return true;
    }
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector3F<TScalar> result)
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
            result = Unsafe.As<TOther, Vector3F<TScalar>>(ref value);
            return true;
        }
    
        if (
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[0], out var x) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[1], out var y) ||
            !ShamelessExploit.TryConvertTruncating<TOtherScalar, TScalar>(value[2], out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TScalar>(x, y, z);
        return true;
    }
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector3F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertChecked<TScalar, TOtherScalar>(value.Z, out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TOtherScalar>(x, y, z);
        return true;
    }
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector3F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertSaturating<TScalar, TOtherScalar>(value.Z, out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TOtherScalar>(x, y, z);
        return true;
    }
    static bool IVector<Vector3F<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector3F<TScalar> value, [MaybeNullWhen(false)] out TOther result)
    {
        if (
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.X, out var x) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Y, out var y) ||
            !ShamelessExploit.TryConvertTruncating<TScalar, TOtherScalar>(value.Z, out var z)
        )
        {
            result = default;
            return false;
        }
    
        result = new Vector3F<TOtherScalar>(x, y, z);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector<Vector3F<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IVector3<Vector3F<TScalar>, TScalar>.Create(TScalar x, TScalar y, TScalar z) => new(x, y, z);


    #region Float-specific code

    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> INumberVector<Vector3F<TScalar>, TScalar>.CopySign(Vector3F<TScalar> value, Vector3F<TScalar> sign) => Vector3F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> INumberVector<Vector3F<TScalar>, TScalar>.CopySign(Vector3F<TScalar> value, TScalar sign) => Vector3F.CopySign(value, sign);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> INumberVector<Vector3F<TScalar>, TScalar>.Sign(Vector3F<TScalar> value) => Vector3F.Sign(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Normalize(Vector3F<TScalar> value) => Vector3F.Normalize(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Lerp(Vector3F<TScalar> value1, Vector3F<TScalar> value2, Vector3F<TScalar> amount) => Vector3F.Lerp(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.LerpClamped(Vector3F<TScalar> value1, Vector3F<TScalar> value2, Vector3F<TScalar> amount) => Vector3F.LerpClamped(value1, value2, amount);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Reflect(Vector3F<TScalar> vector, Vector3F<TScalar> normal) => Vector3F.Reflect(vector, normal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Sqrt(Vector3F<TScalar> value) => Vector3F.Sqrt(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Acosh(Vector3F<TScalar> x) => Vector3F.Acosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Asinh(Vector3F<TScalar> x) => Vector3F.Asinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atanh(Vector3F<TScalar> x) => Vector3F.Atanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Cosh(Vector3F<TScalar> x) => Vector3F.Cosh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Sinh(Vector3F<TScalar> x) => Vector3F.Sinh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Tanh(Vector3F<TScalar> x) => Vector3F.Tanh(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Acos(Vector3F<TScalar> x) => Vector3F.Acos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.AcosPi(Vector3F<TScalar> x) => Vector3F.AcosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Asin(Vector3F<TScalar> x) => Vector3F.Asin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.AsinPi(Vector3F<TScalar> x) => Vector3F.AsinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atan(Vector3F<TScalar> x) => Vector3F.Atan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.AtanPi(Vector3F<TScalar> x) => Vector3F.AtanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Cos(Vector3F<TScalar> x) => Vector3F.Cos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.CosPi(Vector3F<TScalar> x) => Vector3F.CosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.DegreesToRadians(Vector3F<TScalar> degrees) => Vector3F.DegreesToRadians(degrees);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.RadiansToDegrees(Vector3F<TScalar> radians) => Vector3F.RadiansToDegrees(radians);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Sin(Vector3F<TScalar> x) => Vector3F.Sin(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.SinPi(Vector3F<TScalar> x) => Vector3F.SinPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Tan(Vector3F<TScalar> x) => Vector3F.Tan(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.TanPi(Vector3F<TScalar> x) => Vector3F.TanPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector3F<TScalar> Sin, Vector3F<TScalar> Cos) IFloatingPointVector<Vector3F<TScalar>, TScalar>.SinCos(Vector3F<TScalar> x) => Vector3F.SinCos(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static (Vector3F<TScalar> SinPi, Vector3F<TScalar> CosPi) IFloatingPointVector<Vector3F<TScalar>, TScalar>.SinCosPi(Vector3F<TScalar> x) => Vector3F.SinCosPi(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log(Vector3F<TScalar> x) => Vector3F.Log(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log(Vector3F<TScalar> x, Vector3F<TScalar> newBase) => Vector3F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log(Vector3F<TScalar> x, TScalar newBase) => Vector3F.Log(x, newBase);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.LogP1(Vector3F<TScalar> x) => Vector3F.LogP1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log2(Vector3F<TScalar> x) => Vector3F.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log2P1(Vector3F<TScalar> x) => Vector3F.Log2P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log10(Vector3F<TScalar> x) => Vector3F.Log10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Log10P1(Vector3F<TScalar> x) => Vector3F.Log10P1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Exp(Vector3F<TScalar> x) => Vector3F.Exp(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.ExpM1(Vector3F<TScalar> x) => Vector3F.ExpM1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Exp2(Vector3F<TScalar> x) => Vector3F.Exp2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Exp2M1(Vector3F<TScalar> x) => Vector3F.Exp2M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Exp10(Vector3F<TScalar> x) => Vector3F.Exp10(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Exp10M1(Vector3F<TScalar> x) => Vector3F.Exp10M1(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Pow(Vector3F<TScalar> x, Vector3F<TScalar> y) => Vector3F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Pow(Vector3F<TScalar> x, TScalar y) => Vector3F.Pow(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Cbrt(Vector3F<TScalar> x) => Vector3F.Cbrt(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Hypot(Vector3F<TScalar> x, Vector3F<TScalar> y) => Vector3F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Hypot(Vector3F<TScalar> x, TScalar y) => Vector3F.Hypot(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.RootN(Vector3F<TScalar> x, int n) => Vector3F.RootN(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Round(Vector3F<TScalar> x) => Vector3F.Round(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Round(Vector3F<TScalar> x, int digits) => Vector3F.Round(x, digits);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Round(Vector3F<TScalar> x, MidpointRounding mode) => Vector3F.Round(x, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Round(Vector3F<TScalar> x, int digits, MidpointRounding mode) => Vector3F.Round(x, digits, mode);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Truncate(Vector3F<TScalar> x) => Vector3F.Truncate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atan2(Vector3F<TScalar> x, Vector3F<TScalar> y) => Vector3F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atan2Pi(Vector3F<TScalar> x, Vector3F<TScalar> y) => Vector3F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atan2(Vector3F<TScalar> x, TScalar y) => Vector3F.Atan2(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.Atan2Pi(Vector3F<TScalar> x, TScalar y) => Vector3F.Atan2Pi(x, y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.BitDecrement(Vector3F<TScalar> x) => Vector3F.BitDecrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.BitIncrement(Vector3F<TScalar> x) => Vector3F.BitIncrement(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.FusedMultiplyAdd(Vector3F<TScalar> left, Vector3F<TScalar> right, Vector3F<TScalar> addend) => Vector3F.FusedMultiplyAdd(left, right, addend);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.ReciprocalEstimate(Vector3F<TScalar> x) => Vector3F.ReciprocalEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.ReciprocalSqrtEstimate(Vector3F<TScalar> x) => Vector3F.ReciprocalSqrtEstimate(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.ScaleB(Vector3F<TScalar> x, Vector2D<int> n) => Vector3F.ScaleB(x, n);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] static Vector3F<TScalar> IFloatingPointVector<Vector3F<TScalar>, TScalar>.ScaleB(Vector3F<TScalar> x, int n) => Vector3F.ScaleB(x, n);

    #endregion
}