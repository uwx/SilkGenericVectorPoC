using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization;

namespace GenericVector;

[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly partial struct Vector4D<T> : IVector<Vector4D<T>, T>, IVectorAlso<Vector4D<T>, T>
    where T : INumberBase<T>
{
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public readonly T X;

    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public readonly T Y;

    /// <summary>The Z component of the vector.</summary>
    [DataMember]
    public readonly T Z;

    /// <summary>The W component of the vector.</summary>
    [DataMember]
    public readonly T W;

    internal const int Count = 4;

    /// <summary>Creates a new <see cref="Vector4D{T}" /> object whose four elements have the same value.</summary>
    /// <param name="value">The value to assign to all four elements.</param>
    public Vector4D(T value) : this(value, value, value, value)
    {
    }

    /// <summary>Creates a   new <see cref="Vector4D{T}" /> object from the specified <see cref="Vector2D{T}" /> object and a Z and a W component.</summary>
    /// <param name="value">The vector to use for the X and Y components.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4D(Vector2D<T> value, T z, T w) : this(value.X, value.Y, z, w)
    {
    }

    /// <summary>Constructs a new <see cref="Vector4D{T}" /> object from the specified <see cref="Vector3D{T}" /> object and a W component.</summary>
    /// <param name="value">The vector to use for the X, Y, and Z components.</param>
    /// <param name="w">The W component.</param>
    public Vector4D(Vector3D<T> value, T w) : this(value.X, value.Y, value.Z, w)
    {
    }

    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    public Vector4D(T x, T y, T z, T w)
    {
        Unsafe.SkipInit(out this);

        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{Single}" />. The span must contain at least 4 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector4D(ReadOnlySpan<T> values)
    {
        Unsafe.SkipInit(out this);

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, Count, nameof(values));
        
        this = Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }

    /// <summary>Gets a vector whose 4 elements are equal to zero.</summary>
    /// <value>A vector whose four elements are equal to zero (that is, it returns the vector <c>(0,0,0,0)</c>.</value>
    public static Vector4D<T> Zero => new(T.Zero);

    /// <summary>Gets a vector whose 4 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector4D{T}" />.</value>
    /// <remarks>A vector whose four elements are equal to one (that is, it returns the vector <c>(1,1,1,1)</c>.</remarks>
    public static Vector4D<T> One => new(T.One);

    /// <summary>Gets the vector (1,0,0,0).</summary>
    /// <value>The vector <c>(1,0,0,0)</c>.</value>
    public static Vector4D<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);

    /// <summary>Gets the vector (0,1,0,0).</summary>
    /// <value>The vector <c>(0,1,0,0)</c>.</value>
    public static Vector4D<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);

    /// <summary>Gets the vector (0,0,1,0).</summary>
    /// <value>The vector <c>(0,0,1,0)</c>.</value>
    public static Vector4D<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);

    /// <summary>Gets the vector (0,0,0,1).</summary>
    /// <value>The vector <c>(0,0,0,1)</c>.</value>
    public static Vector4D<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);

    public ReadOnlySpan<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.CreateReadOnlySpan<T>(ref Unsafe.AsRef(in X), Count);
    }

    static Vector4D<T> IVector<Vector4D<T>, T>.CreateFromRepeatingComponent(T scalar)
    {
        return new Vector4D<T>(scalar);
    }

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Components[index];
    }

    public static explicit operator Vector2D<T>(Vector4D<T> self) => new(self.X, self.Y);
    public static explicit operator Vector3D<T>(Vector4D<T> self) => new(self.X, self.Y, self.Z);

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the addition operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator +(Vector4D<T> left, Vector4D<T> right)
    {
        return new Vector4D<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z,
            left.W + right.W
        );
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Division" /> method defines the division operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator /(Vector4D<T> left, Vector4D<T> right)
    {
        return new Vector4D<T>(
            left.X / right.X,
            left.Y / right.Y,
            left.Z / right.Z,
            left.W / right.W
        );
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="value1">The vector.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Division" /> method defines the division operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator /(Vector4D<T> value1, T value2)
    {
        return value1 / new Vector4D<T>(value2);
    }

    /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Vector4D{T}" /> objects are equal if each element in <paramref name="left" /> is equal to the corresponding element in <paramref name="right" />.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector4D<T> left, Vector4D<T> right)
    {
        return (left.X == right.X)
            && (left.Y == right.Y)
            && (left.Z == right.Z)
            && (left.W == right.W);
    }

    /// <summary>Returns a value that indicates whether two specified vectors are not equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector4D<T> left, Vector4D<T> right)
    {
        return !(left == right);
    }

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator *(Vector4D<T> left, Vector4D<T> right)
    {
        return new Vector4D<T>(
            left.X * right.X,
            left.Y * right.Y,
            left.Z * right.Z,
            left.W * right.W
        );
    }

    /// <summary>Multiplies the specified vector by the specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator *(Vector4D<T> left, T right)
    {
        return left * new Vector4D<T>(right);
    }

    /// <summary>Multiplies the scalar value by the specified vector.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator *(T left, Vector4D<T> right)
    {
        return right * left;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the subtraction operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator -(Vector4D<T> left, Vector4D<T> right)
    {
        return new Vector4D<T>(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z,
            left.W - right.W
        );
    }

    /// <summary>Negates the specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    /// <remarks>The <see cref="op_UnaryNegation" /> method defines the unary negation operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator -(Vector4D<T> value)
    {
        return Zero - value;
    }

    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have at least four elements. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array)
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Count, nameof(array));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[0]), this);
    }

    /// <summary>Copies the elements of the vector to a specified array starting at a specified index position.</summary>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index at which to copy the first element of the vector.</param>
    /// <remarks><paramref name="array" /> must have a sufficient number of elements to accommodate the four vector elements. In other words, elements <paramref name="index" /> through <paramref name="index" /> + 3 must already exist in <paramref name="array" />.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the array length.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array, int index)
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Count);

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[index]), this);
    }

    /// <summary>Copies the vector to the given <see cref="Span{T}" />. The length of the destination span must be at least 4.</summary>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> destination)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Count, nameof(destination));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), this);
    }

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 4.</summary>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<T> destination)
    {
        if (destination.Length < Count)
        {
            return false;
        }

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), this);
        return true;
    }

    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the two vectors are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two vectors are equal if their <see cref="X" />, <see cref="Y" />, <see cref="Z" />, and <see cref="W" /> elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector4D<T> other)
    {
        return SpeedHelpers.FastEqualsUpTo4<Vector4D<T>, T>(this, other);
    }

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Vector4D{T}" /> object and their corresponding elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Vector4D<T> other) && Equals(other);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    /// <summary>Returns the string representation of the current instance using default formatting.</summary>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using the "G" (general) format string and the formatting conventions of the current thread culture. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
    /// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and the current culture's formatting conventions. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    /// <related type="Article" href="/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</related>
    /// <related type="Article" href="/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</related>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements and the specified format provider to define culture-specific formatting.</summary>
    /// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
    /// <param name="formatProvider">A format provider that supplies culture-specific formatting information.</param>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and <paramref name="formatProvider" />. The "&lt;" and "&gt;" characters are used to begin and end the string, and the format provider's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    /// <related type="Article" href="/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</related>
    /// <related type="Article" href="/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</related>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        return $"<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}{separator} {W.ToString(format, formatProvider)}>";
    }

    public Vector128<T> AsVector128()
    {
        if (!Vector128<T>.IsSupported)
        {
            throw new ArgumentException($"{typeof(T)} cannot be in a Vector128");
        }
        return Unsafe.BitCast<Vector4D<T>, Vector128<T>>(this);
    }

    public Vector128<TOut> AsVector128<TOut>()
    {
        unsafe
        {
            if (sizeof(T) != sizeof(TOut))
            {
                throw new ArgumentException($"Sizes do not match");
            }
        }

        if (!Vector128<TOut>.IsSupported)
        {
            throw new ArgumentException($"{typeof(T)} cannot be in a Vector128");
        }
        return Unsafe.BitCast<Vector4D<T>, Vector128<TOut>>(this);
    }
}