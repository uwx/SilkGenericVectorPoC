using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization;

namespace GenericVector;

// internal interface IVectorN<TSelf, TComponent> : IEquatable<TSelf>, ISpanFormattable, IUtf8SpanFormattable, INumber<TSelf>
//     where TSelf : IVectorN<TSelf, TComponent>
//     where TComponent : INumber<TComponent>;

[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly partial struct Vector3D<T> : IVector<Vector3D<T>, T>, IVectorAlso<Vector3D<T>, T>, IEquatable<System.Numerics.Vector3>, ISpanFormattable
    where T : INumberBase<T>
{
    public ReadOnlySpan<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.CreateReadOnlySpan<T>(ref Unsafe.AsRef(in X), Count);
    }

    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public readonly T X;

    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public readonly T Y;

    /// <summary>The Z component of the vector.</summary>
    [DataMember]
    public readonly T Z;

    /// <summary>
    /// Returns the vector (0,0,0).
    /// </summary>
    public static Vector3D<T> Zero => new(T.Zero);

    /// <summary>
    /// Returns the vector (1,1,1).
    /// </summary>
    public static Vector3D<T> One => new(T.One);

    /// <summary>
    /// Returns the vector (1,0,0).
    /// </summary>
    public static Vector3D<T> UnitX => new(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Returns the vector (0,1,0).
    /// </summary>
    public static Vector3D<T> UnitY => new(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Returns the vector (0,0,1).
    /// </summary>
    public static Vector3D<T> UnitZ => new(T.Zero, T.Zero, T.One);

    internal const int Count = 3;

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Components[index];
    }
    
    /// <summary>Creates a new <see cref="Vector3" /> object whose three elements have the same value.</summary>
    /// <param name="value">The value to assign to all three elements.</param>
    public Vector3D(T value) : this(value, value, value)
    {
    }
    
    /// <summary>Creates a new <see cref="Vector3" /> object from the specified <see cref="Vector2" /> object and the specified value.</summary>
    /// <param name="value">The vector with two elements.</param>
    /// <param name="z">The additional value to assign to the <see cref="Z" /> field.</param>
    public Vector3D(Vector2D<T> value, T z) : this(value.X, value.Y, z)
    {
    }
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    public Vector3D(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{Single}" />. The span must contain at least 3 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector3D(ReadOnlySpan<T> values)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, Count, nameof(values));

        this = Unsafe.ReadUnaligned<Vector3D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    public override string ToString()
    {
        return ToString("G");
    }

    /// <summary>
    /// Returns a String representing this Vector3 instance, using the specified format to format individual elements 
    /// and the given IFormatProvider.
    /// </summary>
    /// <param name="format">The format of individual elements.</param>
    /// <param name="formatProvider">The format provider to use when formatting elements.</param>
    /// <returns>The string representation.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        Span<char> initialBuffer = stackalloc char[Math.Min((2 + (Count - 1) + (separator.Length * (Count - 1)) + (Count * 2)), 256)];
        
        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new DefaultInterpolatedStringHandler(
            4 + (separator.Length * 2),
            3,
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

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Vector3" /> object and their corresponding elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Vector3D<T> other) && Equals(other);
    }

    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the two vectors are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two vectors are equal if their <see cref="X" />, <see cref="Y" />, and <see cref="Z" /> elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector3D<T> other)
    {
        return SpeedHelpers.FastEqualsUpTo4<Vector3D<T>, T>(this, other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(System.Numerics.Vector3 other)
    {
        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated)
        {
            return Unsafe.BitCast<Vector3D<T>, System.Numerics.Vector3>(this).AsVector128().Equals(other.AsVector128());
        }
        
        return float.CreateTruncating(X).Equals(other.X) &&
               float.CreateTruncating(Y).Equals(other.Y) &&
               float.CreateTruncating(Y).Equals(other.Y);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;
        
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
        
        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            3,
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
        // We only assign it on first use, just in case we fail before that
        string? formatString;

        _ =
            handler.AppendLiteral("<") &&
            handler.AppendFormatted(X, formatString = format.Length > 0 ? new string(format) : null) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Y, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Z, formatString) &&
            handler.AppendLiteral(">");

        return destination.TryWrite(ref handler, out charsWritten);
    }

    public Vector3D<TOther> As<TOther>() where TOther : INumberBase<TOther>
    {
        return new Vector3D<TOther>(
            TOther.CreateTruncating(X),
            TOther.CreateTruncating(Y),
            TOther.CreateTruncating(Z)
        );
    } 

    public static explicit operator System.Numerics.Vector3(Vector3D<T> self)
    {
        return new System.Numerics.Vector3(
            float.CreateTruncating(self.X),
            float.CreateTruncating(self.Y),
            float.CreateTruncating(self.Z)
        );
    }
    
    public static explicit operator checked System.Numerics.Vector3(Vector3D<T> self)
    {
        return new System.Numerics.Vector3(
            float.CreateChecked(self.X),
            float.CreateChecked(self.Y),
            float.CreateChecked(self.Z)
        );
    }

    public static explicit operator Vector3D<byte>(Vector3D<T> self) => self.As<byte>();
    public static explicit operator Vector3D<sbyte>(Vector3D<T> self) => self.As<sbyte>();
    public static explicit operator Vector3D<short>(Vector3D<T> self) => self.As<short>();
    public static explicit operator Vector3D<ushort>(Vector3D<T> self) => self.As<ushort>();
    public static explicit operator Vector3D<int>(Vector3D<T> self) => self.As<int>();
    public static explicit operator Vector3D<uint>(Vector3D<T> self) => self.As<uint>();
    public static explicit operator Vector3D<long>(Vector3D<T> self) => self.As<long>();
    public static explicit operator Vector3D<ulong>(Vector3D<T> self) => self.As<ulong>();
    public static explicit operator Vector3D<Int128>(Vector3D<T> self) => self.As<Int128>();
    public static explicit operator Vector3D<UInt128>(Vector3D<T> self) => self.As<UInt128>();
    public static explicit operator Vector3D<Half>(Vector3D<T> self) => self.As<Half>();
    public static explicit operator Vector3D<float>(Vector3D<T> self) => self.As<float>();
    public static explicit operator Vector3D<double>(Vector3D<T> self) => self.As<double>();
    public static explicit operator Vector3D<decimal>(Vector3D<T> self) => self.As<decimal>();
    public static explicit operator Vector3D<Complex>(Vector3D<T> self) => self.As<Complex>();
    public static explicit operator Vector3D<BigInteger>(Vector3D<T> self) => self.As<BigInteger>();

    public static explicit operator Vector2D<T>(Vector3D<T> self) => new(self.X, self.Y);
    public static explicit operator Vector4D<T>(Vector3D<T> self) => new(self, T.Zero);
    
    public static explicit operator Vector3D<T>(System.Numerics.Vector3 self)
    {
        return new Vector3D<T>(
            T.CreateTruncating(self.X),
            T.CreateTruncating(self.Y),
            T.CreateTruncating(self.Z)
        );
    }
    
    public static explicit operator checked Vector3D<T>(System.Numerics.Vector3 self)
    {
        return new Vector3D<T>(
            T.CreateChecked(self.X),
            T.CreateChecked(self.Y),
            T.CreateChecked(self.Z)
        );
    }

    public static implicit operator Vector3D<T>((T x, T y, T z) components)
        => new(components.x, components.y, components.z);

    #region Operators

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the addition operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator +(Vector3D<T> left, Vector3D<T> right)
    {
        return new Vector3D<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z
        );
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    /// <remarks>The <see cref="Vector3.op_Division" /> method defines the division operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator /(Vector3D<T> left, Vector3D<T> right)
    {
        return new Vector3D<T>(
            left.X / right.X,
            left.Y / right.Y,
            left.Z / right.Z
        );
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="value1">The vector.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    /// <remarks>The <see cref="Vector3.op_Division" /> method defines the division operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator /(Vector3D<T> value1, T value2)
    {
        return value1 / new Vector3D<T>(value2);
    }

    /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Vector3" /> objects are equal if each element in <paramref name="left" /> is equal to the corresponding element in <paramref name="right" />.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector3D<T> left, Vector3D<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>Returns a value that indicates whether two specified vectors are not equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector3D<T> left, Vector3D<T> right)
    {
        return !(left == right);

    }
    
    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator *(Vector3D<T> left, Vector3D<T> right)
    {
        return new Vector3D<T>(
            left.X * right.X,
            left.Y * right.Y,
            left.Z * right.Z
        );
    }

    /// <summary>Multiplies the specified vector by the specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator *(Vector3D<T> left, T right)
    {
        return left * new Vector3D<T>(right);
    }

    /// <summary>Multiplies the scalar value by the specified vector.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator *(T left, Vector3D<T> right)
    {
        return right * left;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the subtraction operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator -(Vector3D<T> left, Vector3D<T> right)
    {
        return new Vector3D<T>(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z
        );
    }

    /// <summary>Negates the specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    /// <remarks>The <see cref="op_UnaryNegation" /> method defines the unary negation operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> operator -(Vector3D<T> value)
    {
        return Zero - value;
    }

    #endregion
    
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3D<T> INumberBase<Vector3D<T>>.Abs(Vector3D<T> value) => Vector3D.Abs(value);

    static Vector3D<T> IVector<Vector3D<T>, T>.CreateFromRepeatingComponent(T scalar) => new(scalar);
}