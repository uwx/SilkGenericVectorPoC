using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization;
using System.Text.Unicode;

namespace GenericVector;


// Vector4D<T>
/// <summary>A structure encapsulating four values, usually geometric vectors, and provides hardware accelerated methods.</summary>
[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly partial struct Vector4D<T> : IVector<Vector4D<T>, T>, IVectorAlso<Vector4D<T>, T>, IEquatable<Vector4>, ISpanFormattable
    where T : INumberBase<T>
{
    private readonly T _x;
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public T X => _x;
    private readonly T _y;
    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public T Y => _y;
    private readonly T _z;
    /// <summary>The Z component of the vector.</summary>
    [DataMember]
    public T Z => _z;
    private readonly T _w;
    /// <summary>The W component of the vector.</summary>
    [DataMember]
    public T W => _w;

    internal const int Count = 4;

    /// <summary>Creates a new <see cref="Vector4D{T}" /> object whose four elements have the same value.</summary>
    /// <param name="value">The value to assign to all four elements.</param>
    public Vector4D(T value) : this(value, value, value, value)
    {
    }

    /// <summary>Creates a new <see cref="Vector4D{T}" /> object from the specified <see cref="Vector4D{T}" /> object X and a Y and a Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the [,  and ] components.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4D(Vector2D<T> value, T z, T w) : this(value.X, value.Y, z, w)
    {
    }
    /// <summary>Creates a new <see cref="Vector4D{T}" /> object from the specified <see cref="Vector4D{T}" /> object X and a Y and a Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the [,  and ] components.</param>
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

        _x = x;
        _y = y;
        _z = z;
        _w = w;
    }

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 4 elements.</summary>
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

    /// <summary>Gets the vector (1,0,0,0)).</summary>
    /// <value>The vector <c>(1,0,0,0)</c>.</value>
    public static Vector4D<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero);
    /// <summary>Gets the vector (0,1,0,0)).</summary>
    /// <value>The vector <c>(0,1,0,0)</c>.</value>
    public static Vector4D<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero);
    /// <summary>Gets the vector (0,0,1,0)).</summary>
    /// <value>The vector <c>(0,0,1,0)</c>.</value>
    public static Vector4D<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero);
    /// <summary>Gets the vector (0,0,0,1)).</summary>
    /// <value>The vector <c>(0,0,0,1)</c>.</value>
    public static Vector4D<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One);

    /// <summary>Creates a new <see cref="Vector4D{T}" /> object whose four elements have the same value.</summary>
    /// <param name="value">The value to assign to all four elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Create(T scalar) => new(scalar);

    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Create(T x, T y, T z, T w) => new(x, y, z, w);

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 4 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Create(ReadOnlySpan<T> values) => new(values);

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }

    #region Operators
    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the addition operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator +(Vector4D<T> left, Vector4D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new Vector4D<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z,
            left.W + right.W
        );
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the subtraction operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator -(Vector4D<T> left, Vector4D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


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

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator *(Vector4D<T> left, Vector4D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


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

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    /// <remarks>The <see cref="Vector4D{T}.op_Division" /> method defines the division operation for <see cref="Vector4D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> operator /(Vector4D<T> left, Vector4D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


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
        return left.Equals(right);
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
    #endregion

    #region Equality
    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the two vectors are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two vectors are equal if their <see cref="X" />, <see cref="Y" />, <see cref="Z" />, and <see cref="W" /> elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector4D<T> other)
    {
        /*// NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
unsafe
{
    if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
    {
        Vector64<T> v0 = default;
        Vector64<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
    }

    if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
    {
        Vector128<T> v0 = default;
        Vector128<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
    }

    if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
    {
        Vector256<T> v0 = default;
        Vector256<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
    }

    if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
    {
        Vector512<T> v0 = default;
        Vector512<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
        Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
    }
}
*/

        //return SpeedHelpers.FastEqualsUpTo4<Vector4D<T>, T>(this, other);
        return
            X.Equals(other.X) &&
            Y.Equals(other.Y) &&
            Z.Equals(other.Z) &&
            W.Equals(other.W);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector4 other)
    {
        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated)
        {
            return Unsafe.BitCast<Vector4D<T>, Vector4>(this).AsVector128().Equals(other.AsVector128());
        }


        return
            float.CreateTruncating(X).Equals(other.X) &&
            float.CreateTruncating(Y).Equals(other.Y) &&
            float.CreateTruncating(Z).Equals(other.Z) &&
            float.CreateTruncating(W).Equals(other.W);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }
    #endregion

    #region Format
    /// <summary>Returns the string representation of the current instance using default formatting.</summary>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using the "G" (general) format string and the formatting conventions of the current thread culture. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    public override string ToString()
    {
        return ToString("G", null);
    }

    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
    /// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and the current culture's formatting conventions. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    /// <related type="Article" href="/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</related>
    /// <related type="Article" href="/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</related>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        return ToString(format, null);
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
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

        Span<char> initialBuffer = stackalloc char[Math.Min((2 + (Count - 1) + (separator.Length * (Count - 1)) + (Count * 2)), 256)];

        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new DefaultInterpolatedStringHandler(
            4 + (separator.Length * 2),
            Count,
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
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Z, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(W, formatString) &&
            handler.AppendLiteral(">");

        return destination.TryWrite(ref handler, out charsWritten);
    }
    #endregion

    #region Casts
    public Vector4D<TOther> As<TOther>() where TOther : INumberBase<TOther>
    {
        if (SpeedHelpers.TryFastConvert<Vector4D<T>, T, Vector4D<TOther>, TOther>(this, out var result))
        {
            return result;
        }

        return new Vector4D<TOther>(
            TOther.CreateTruncating(X),
            TOther.CreateTruncating(Y),
            TOther.CreateTruncating(Z),
            TOther.CreateTruncating(W)
        );
    }

    private Vector4D<TOther> AsChecked<TOther>() where TOther : INumberBase<TOther>
    {
        return new Vector4D<TOther>(
            TOther.CreateChecked(X),
            TOther.CreateChecked(Y),
            TOther.CreateChecked(Z),
            TOther.CreateChecked(W)
        );
    }
    public static explicit operator Vector4D<byte>(Vector4D<T> self) => self.As<byte>();
    public static explicit operator Vector4D<sbyte>(Vector4D<T> self) => self.As<sbyte>();
    public static explicit operator Vector4D<short>(Vector4D<T> self) => self.As<short>();
    public static explicit operator Vector4D<ushort>(Vector4D<T> self) => self.As<ushort>();
    public static explicit operator Vector4D<int>(Vector4D<T> self) => self.As<int>();
    public static explicit operator Vector4D<uint>(Vector4D<T> self) => self.As<uint>();
    public static explicit operator Vector4D<long>(Vector4D<T> self) => self.As<long>();
    public static explicit operator Vector4D<ulong>(Vector4D<T> self) => self.As<ulong>();
    public static explicit operator Vector4D<Int128>(Vector4D<T> self) => self.As<Int128>();
    public static explicit operator Vector4D<UInt128>(Vector4D<T> self) => self.As<UInt128>();
    public static explicit operator Vector4D<Half>(Vector4D<T> self) => self.As<Half>();
    public static explicit operator Vector4D<float>(Vector4D<T> self) => self.As<float>();
    public static explicit operator Vector4D<double>(Vector4D<T> self) => self.As<double>();
    public static explicit operator Vector4D<decimal>(Vector4D<T> self) => self.As<decimal>();
    public static explicit operator Vector4D<Complex>(Vector4D<T> self) => self.As<Complex>();
    public static explicit operator Vector4D<BigInteger>(Vector4D<T> self) => self.As<BigInteger>();

    public static explicit operator checked Vector4D<byte>(Vector4D<T> self) => self.AsChecked<byte>();
    public static explicit operator checked Vector4D<sbyte>(Vector4D<T> self) => self.AsChecked<sbyte>();
    public static explicit operator checked Vector4D<short>(Vector4D<T> self) => self.AsChecked<short>();
    public static explicit operator checked Vector4D<ushort>(Vector4D<T> self) => self.AsChecked<ushort>();
    public static explicit operator checked Vector4D<int>(Vector4D<T> self) => self.AsChecked<int>();
    public static explicit operator checked Vector4D<uint>(Vector4D<T> self) => self.AsChecked<uint>();
    public static explicit operator checked Vector4D<long>(Vector4D<T> self) => self.AsChecked<long>();
    public static explicit operator checked Vector4D<ulong>(Vector4D<T> self) => self.AsChecked<ulong>();
    public static explicit operator checked Vector4D<Int128>(Vector4D<T> self) => self.AsChecked<Int128>();
    public static explicit operator checked Vector4D<UInt128>(Vector4D<T> self) => self.AsChecked<UInt128>();
    public static explicit operator checked Vector4D<Half>(Vector4D<T> self) => self.AsChecked<Half>();
    public static explicit operator checked Vector4D<float>(Vector4D<T> self) => self.AsChecked<float>();
    public static explicit operator checked Vector4D<double>(Vector4D<T> self) => self.AsChecked<double>();
    public static explicit operator checked Vector4D<decimal>(Vector4D<T> self) => self.AsChecked<decimal>();
    public static explicit operator checked Vector4D<Complex>(Vector4D<T> self) => self.AsChecked<Complex>();
    public static explicit operator checked Vector4D<BigInteger>(Vector4D<T> self) => self.AsChecked<BigInteger>();

    // Cast to System.Numerics.Vector4
    public static explicit operator Vector4(Vector4D<T> self) => new(float.CreateTruncating(self.X), float.CreateTruncating(self.Y), float.CreateTruncating(self.Z), float.CreateTruncating(self.W));
    public static explicit operator checked Vector4(Vector4D<T> self) => new(float.CreateChecked(self.X), float.CreateChecked(self.Y), float.CreateChecked(self.Z), float.CreateChecked(self.W));

    // Downcast
    public static explicit operator Vector2D<T>(Vector4D<T> self) => new(self.X, self.Y);
    public static explicit operator Vector3D<T>(Vector4D<T> self) => new(self.X, self.Y, self.Z);

    // Upcast
    public static explicit operator Vector5D<T>(Vector4D<T> self) => new(self, T.Zero);

    // Upcast from System.Numerics.Vector < 4
    public static explicit operator Vector4D<T>(Vector2 self) => new(T.CreateTruncating(self.X), T.CreateTruncating(self.Y), T.Zero, T.Zero);
    public static explicit operator checked Vector4D<T>(Vector2 self) => new(T.CreateChecked(self.X), T.CreateChecked(self.Y), T.Zero, T.Zero);
    public static explicit operator Vector4D<T>(Vector3 self) => new(T.CreateTruncating(self.X), T.CreateTruncating(self.Y), T.CreateTruncating(self.Z), T.Zero);
    public static explicit operator checked Vector4D<T>(Vector3 self) => new(T.CreateChecked(self.X), T.CreateChecked(self.Y), T.CreateChecked(self.Z), T.Zero);

    // Downcast from System.Numerics.Vector >= 4

    public static implicit operator Vector4D<T>((T X, T Y, T Z, T W) components)
        => new(components.X, components.Y, components.Z, components.W);

    #endregion

    public void Deconstruct(out T x, out T y, out T z, out T w)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }
}

file interface IVec4
{
    // Returns null if incompatible. Throws OverflowException if overflowing
    Vector4D<T>? GetChecked<T>() where T : INumberBase<T>;
    Vector4D<T>? GetSaturating<T>() where T : INumberBase<T>;
    Vector4D<T>? GetTruncating<T>() where T : INumberBase<T>;
}

// Vector4D<T>.INumber
public readonly partial struct Vector4D<T> :
    IDivisionOperators<Vector4D<T>, T, Vector4D<T>>,
    IMultiplyOperators<Vector4D<T>, T, Vector4D<T>>,
    INumberBase<Vector4D<T>>,
    IVec4
{
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4D<T> INumberBase<Vector4D<T>>.Abs(Vector4D<T> value) => Vector4D.Abs(value);

    static Vector4D<T> IParsable<Vector4D<T>>.Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), NumberStyles.None, provider);

    static Vector4D<T> ISpanParsable<Vector4D<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s, NumberStyles.None, provider);

    public static Vector4D<T> Parse(string s, NumberStyles style = default, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), style, provider);

    public static Vector4D<T> Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null)
        => TryParse(s, style, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4D)}<{typeof(T)}>");

    public static bool TryParse(string? s, IFormatProvider? provider, out Vector4D<T> result)
        => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vector4D<T> result)
        => TryParse(s.AsSpan(), style, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector4D<T> result)
        => TryParse(s, NumberStyles.None, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector4D<T> result)
    {
        result = default;

        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;

        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        s = s[1..^1];

        T? x, y, z, w;

        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style, provider, out x)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style, provider, out y)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style, provider, out z)) return false;

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, style, provider, out w)) return false;
        }

        result = new Vector4D<T>(x, y, z, w);
        return true;
    }

    static bool INumberBase<Vector4D<T>>.IsCanonical(Vector4D<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z) && T.IsCanonical(value.W);

    static bool INumberBase<Vector4D<T>>.IsComplexNumber(Vector4D<T> value) => T.IsComplexNumber(value.X) || T.IsComplexNumber(value.Y) || T.IsComplexNumber(value.Z) || T.IsComplexNumber(value.W);

    static bool INumberBase<Vector4D<T>>.IsEvenInteger(Vector4D<T> value) => T.IsEvenInteger(value.X) && T.IsEvenInteger(value.Y) && T.IsEvenInteger(value.Z) && T.IsEvenInteger(value.W);

    static bool INumberBase<Vector4D<T>>.IsFinite(Vector4D<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z) && T.IsFinite(value.W);

    static bool INumberBase<Vector4D<T>>.IsImaginaryNumber(Vector4D<T> value) => T.IsImaginaryNumber(value.X) || T.IsImaginaryNumber(value.Y) || T.IsImaginaryNumber(value.Z) || T.IsImaginaryNumber(value.W);

    static bool INumberBase<Vector4D<T>>.IsInfinity(Vector4D<T> value) => T.IsInfinity(value.X) && T.IsInfinity(value.Y) && T.IsInfinity(value.Z) && T.IsInfinity(value.W);

    static bool INumberBase<Vector4D<T>>.IsInteger(Vector4D<T> value) => T.IsInteger(value.X) && T.IsInteger(value.Y) && T.IsInteger(value.Z) && T.IsInteger(value.W);

    static bool INumberBase<Vector4D<T>>.IsNaN(Vector4D<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z) || T.IsNaN(value.W);

    static bool INumberBase<Vector4D<T>>.IsNegative(Vector4D<T> value) => T.IsNegative(value.X) && T.IsNegative(value.Y) && T.IsNegative(value.Z) && T.IsNegative(value.W);

    static bool INumberBase<Vector4D<T>>.IsNegativeInfinity(Vector4D<T> value) => T.IsNegativeInfinity(value.X) && T.IsNegativeInfinity(value.Y) && T.IsNegativeInfinity(value.Z) && T.IsNegativeInfinity(value.W);

    static bool INumberBase<Vector4D<T>>.IsNormal(Vector4D<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z) && T.IsNormal(value.W);

    static bool INumberBase<Vector4D<T>>.IsOddInteger(Vector4D<T> value) => T.IsOddInteger(value.X) && T.IsOddInteger(value.Y) && T.IsOddInteger(value.Z) && T.IsOddInteger(value.W);

    static bool INumberBase<Vector4D<T>>.IsPositive(Vector4D<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z) && T.IsPositive(value.W);

    static bool INumberBase<Vector4D<T>>.IsPositiveInfinity(Vector4D<T> value) => T.IsPositiveInfinity(value.X) && T.IsPositiveInfinity(value.Y) && T.IsPositiveInfinity(value.Z) && T.IsPositiveInfinity(value.W);

    static bool INumberBase<Vector4D<T>>.IsRealNumber(Vector4D<T> value) => T.IsRealNumber(value.X) && T.IsRealNumber(value.Y) && T.IsRealNumber(value.Z) && T.IsRealNumber(value.W);

    static bool INumberBase<Vector4D<T>>.IsSubnormal(Vector4D<T> value) => T.IsSubnormal(value.X) && T.IsSubnormal(value.Y) && T.IsSubnormal(value.Z) && T.IsSubnormal(value.W);

    static bool INumberBase<Vector4D<T>>.IsZero(Vector4D<T> value) => T.IsZero(value.X) && T.IsZero(value.Y) && T.IsZero(value.Z) && T.IsZero(value.W);

    static Vector4D<T> INumberBase<Vector4D<T>>.MaxMagnitude(Vector4D<T> x, Vector4D<T> y) => new(T.MaxMagnitude(x.X, y.X), T.MaxMagnitude(x.Y, y.Y), T.MaxMagnitude(x.Z, y.Z), T.MaxMagnitude(x.W, y.W));

    static Vector4D<T> INumberBase<Vector4D<T>>.MaxMagnitudeNumber(Vector4D<T> x, Vector4D<T> y) => new(T.MaxMagnitudeNumber(x.X, y.X), T.MaxMagnitudeNumber(x.Y, y.Y), T.MaxMagnitudeNumber(x.Z, y.Z), T.MaxMagnitudeNumber(x.W, y.W));

    static Vector4D<T> INumberBase<Vector4D<T>>.MinMagnitude(Vector4D<T> x, Vector4D<T> y) => new(T.MinMagnitude(x.X, y.X), T.MinMagnitude(x.Y, y.Y), T.MinMagnitude(x.Z, y.Z), T.MinMagnitude(x.W, y.W));

    static Vector4D<T> INumberBase<Vector4D<T>>.MinMagnitudeNumber(Vector4D<T> x, Vector4D<T> y) => new(T.MinMagnitudeNumber(x.X, y.X), T.MinMagnitudeNumber(x.Y, y.Y), T.MinMagnitudeNumber(x.Z, y.Z), T.MinMagnitudeNumber(x.W, y.W));

    static bool INumberBase<Vector4D<T>>.TryConvertFromChecked<TOther>(TOther value, out Vector4D<T> result)
    {
        if (value is Vector4D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec4 IVec4 && IVec4.GetChecked<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector4D<T>>.TryConvertFromSaturating<TOther>(TOther value, out Vector4D<T> result)
    {
        if (value is Vector4D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec4 IVec4 && IVec4.GetSaturating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector4D<T>>.TryConvertFromTruncating<TOther>(TOther value, out Vector4D<T> result)
    {
        if (value is Vector4D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec4 IVec4 && IVec4.GetTruncating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector4D<T>>.TryConvertToChecked<TOther>(Vector4D<T> value, [MaybeNullWhen(false)] out TOther result)
        => TOther.TryConvertFromChecked(value, out result);

    static bool INumberBase<Vector4D<T>>.TryConvertToSaturating<TOther>(Vector4D<T> value, [MaybeNullWhen(false)] out TOther result)
        => TOther.TryConvertFromSaturating(value, out result);

    static bool INumberBase<Vector4D<T>>.TryConvertToTruncating<TOther>(Vector4D<T> value, [MaybeNullWhen(false)]out TOther result)
        => TOther.TryConvertFromTruncating(value, out result);

    Vector4D<T1>? IVec4.GetChecked<T1>() => T1.TryConvertFromChecked(X, out var x) ? new(x, T1.CreateChecked(Y), T1.CreateChecked(Z), T1.CreateChecked(W)) : null;
    Vector4D<T1>? IVec4.GetSaturating<T1>() => T1.TryConvertFromSaturating(X, out var x) ? new(x, T1.CreateSaturating(Y), T1.CreateSaturating(Z), T1.CreateSaturating(W)) : null;
    Vector4D<T1>? IVec4.GetTruncating<T1>() => T1.TryConvertFromTruncating(X, out var x) ? new(x, T1.CreateTruncating(Y), T1.CreateTruncating(Z), T1.CreateTruncating(W)) : null;

    static ReadOnlySpan<T> IVector<Vector4D<T>, T>.AsSpan(in Vector4D<T> vec) => vec.AsSpan();
}

// Vector4D<T>.IReadOnlyList
public readonly partial struct Vector4D<T> : IReadOnlyList<T>
{
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    int IReadOnlyCollection<T>.Count => Count;
}

// Vector4D<T>.IUtf8SpanParsableFormattable
public partial struct Vector4D<T> :
    IUtf8SpanFormattable,
    IUtf8SpanParsable<Vector4D<T>>
{
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;

        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new Utf8.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            Count,
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

    public static Vector4D<T> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector4D)}<{typeof(T)}>");

    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector4D<T> result)
    {
        result = default;

        if (s[0] != (byte)'<') return false;
        if (s[^1] != (byte)'>') return false;

        var separator = NumberGroupSeparatorTChar<byte>(NumberFormatInfo.GetInstance(provider));

        s = s[1..^1];

        T? x, y, z, w;

        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out x)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out y)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out z)) return false;

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, provider, out w)) return false;
        }

        result = new Vector4D<T>(x, y, z, w);
        return true;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(NumberGroupSeparatorTChar))]
        static extern ReadOnlySpan<TChar> NumberGroupSeparatorTChar<TChar>(NumberFormatInfo? c) where TChar : unmanaged;
    }

    static void IVector<Vector4D<T>, T>.CopyTo(in Vector4D<T> vector, T[] array) => vector.CopyTo(array);
    static void IVector<Vector4D<T>, T>.CopyTo(in Vector4D<T> vector, T[] array, int index) => vector.CopyTo(array, index);
    static void IVector<Vector4D<T>, T>.CopyTo(in Vector4D<T> vector, Span<T> destination) => vector.CopyTo(destination);
    static bool IVector<Vector4D<T>, T>.TryCopyTo(in Vector4D<T> vector, Span<T> destination) => vector.TryCopyTo(destination);
}

// Vector4D
public static partial class Vector4D
{
    #region CopyTo
    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have at least four elements. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector4D<T> self, T[] array) where T : INumberBase<T>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector4D<T>.Count, nameof(array));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[0]), self);
    }

    /// <summary>Copies the elements of the vector to a specified array starting at a specified index position.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index at which to copy the first element of the vector.</param>
    /// <remarks><paramref name="array" /> must have a sufficient number of elements to accommodate the four vector elements. In other words, elements <paramref name="index" /> through <paramref name="index" /> + 4 must already exist in <paramref name="array" />.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the array length.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector4D<T> self, T[] array, int index) where T : INumberBase<T>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector4D<T>.Count);

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[index]), self);
    }

    /// <summary>Copies the vector to the given <see cref="Span{T}" />. The length of the destination span must be at least 4.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector4D<T> self, Span<T> destination) where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector4D<T>.Count, nameof(destination));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 4.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(in this Vector4D<T> self, Span<T> destination) where T : INumberBase<T>
    {
        if (destination.Length < Vector4D<T>.Count)
        {
            return false;
        }

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion

    #region Extension

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(in this Vector4D<T> vec) where T : INumberBase<T>
    {
        return MemoryMarshal.CreateReadOnlySpan<T>(ref Unsafe.As<Vector4D<T>, T>(ref Unsafe.AsRef(in vec)), Vector4D<T>.Count);
    }

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Length<T, TReturn>(this Vector4D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var lengthSquared = vec.LengthSquared<T, TReturn>();
        return TReturn.Sqrt(lengthSquared);
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T,TReturn}" /> method.</remarks>
    /// <altmember cref="Length{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn LengthSquared<T, TReturn>(this Vector4D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        return Dot<T, TReturn>(vec, vec);
    }

    #endregion

    #region Basic

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Multiply<T>(in Vector4D<T> left, in Vector4D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Multiply<T>(in Vector4D<T> left, T right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Multiply<T>(T left, in Vector4D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Negate<T>(in Vector4D<T> value) where T : INumberBase<T>
    {
        return -value;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Subtract<T>(in Vector4D<T> left, in Vector4D<T> right) where T : INumberBase<T>
    {
        return left - right;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Add<T>(in Vector4D<T> left, in Vector4D<T> right) where T : INumberBase<T>
    {
        return left + right;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Divide<T>(in Vector4D<T> left, in Vector4D<T> right) where T : INumberBase<T>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Divide<T>(in Vector4D<T> left, T divisor) where T : INumberBase<T>
    {
        return left / divisor;
    }

    #endregion

    #region Other

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Abs<T>(in Vector4D<T> value) where T : INumberBase<T>
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), value);
                
                v0 = Vector64.Abs(v0);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), value);
                
                v0 = Vector128.Abs(v0);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), value);
                
                v0 = Vector256.Abs(v0);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), value);
                
                v0 = Vector512.Abs(v0);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new(T.Abs(value.X), T.Abs(value.Y), T.Abs(value.Z), T.Abs(value.W));
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Clamp<T>(in Vector4D<T> value1, in Vector4D<T> min, Vector4D<T> max) where T : INumberBase<T>
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                Vector64<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v2), max);
                
                v0 = Vector64.Min(Vector64.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                Vector128<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v2), max);
                
                v0 = Vector128.Min(Vector128.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                Vector256<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v2), max);
                
                v0 = Vector256.Min(Vector256.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                Vector512<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v2), max);
                
                v0 = Vector512.Min(Vector512.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector4D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return Min(Max(value1, min), max);
    }

    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Distance<T, TReturn>(in Vector4D<T> value1, in Vector4D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var distanceSquared = DistanceSquared<T, TReturn>(value1, value2);
        return TReturn.Sqrt(distanceSquared);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(in Vector4D<T> value1, in Vector4D<T> value2) where T : INumberBase<T>
    {
        var difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn DistanceSquared<T, TReturn>(in Vector4D<T> value1, in Vector4D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var difference = value1 - value2;
        return Dot<T, TReturn>(difference, difference);
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(in Vector4D<T> vector1, in Vector4D<T> vector2) where T : INumberBase<T>
    {
        // TODO: vectorize return scalar
        return
            vector1.X * vector2.X +
            vector1.Y * vector2.Y +
            vector1.Z * vector2.Z +
            vector1.W * vector2.W;
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Dot<T, TReturn>(in Vector4D<T> vector1, in Vector4D<T> vector2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        // TODO vectorize return converted (maybe not possible)
        return
            TReturn.CreateTruncating(vector1.X) * TReturn.CreateTruncating(vector2.X) +
            TReturn.CreateTruncating(vector1.Y) * TReturn.CreateTruncating(vector2.Y) +
            TReturn.CreateTruncating(vector1.Z) * TReturn.CreateTruncating(vector2.Z) +
            TReturn.CreateTruncating(vector1.W) * TReturn.CreateTruncating(vector2.W);
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TFloat> Lerp<T, TFloat>(in Vector4D<T> value1, in Vector4D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (TFloat.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4D<T> LerpUnchecked<T>(in Vector4D<T> value1, in Vector4D<T> value2, T amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (T.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TFloat> LerpClamped<T, TFloat>(in Vector4D<T> value1, in Vector4D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = TFloat.Clamp(amount, TFloat.Zero, TFloat.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4D<T> LerpClampedUnchecked<T>(in Vector4D<T> value1, in Vector4D<T> value2, T amount) where T : INumberBase<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T ClampT(T value, T min, T max)
        {
            return T.MaxMagnitude(T.MaxMagnitude(value, min), max);
        }

        amount = ClampT(amount, T.Zero, T.One);
        return LerpUnchecked(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TFloat> Lerp<T, TFloat>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (Vector4D<TFloat>.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4D<T> LerpUnchecked<T>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<T> amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (Vector4D<T>.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TFloat> LerpClamped<T, TFloat>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = Clamp(amount, Vector4D<TFloat>.Zero, Vector4D<TFloat>.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector4D<T> LerpClampedUnchecked<T>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<T> amount) where T : INumberBase<T>
    {
        amount = Clamp(amount, Vector4D<T>.Zero, Vector4D<T>.One);
        return LerpUnchecked(value1, value2, amount);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Max<T>(in Vector4D<T> value1, in Vector4D<T> value2) where T : INumberBase<T>
    {
        return new Vector4D<T>(
            T.MaxMagnitudeNumber(value1.X, value2.X), 
            T.MaxMagnitudeNumber(value1.Y, value2.Y), 
            T.MaxMagnitudeNumber(value1.Z, value2.Z), 
            T.MaxMagnitudeNumber(value1.W, value2.W)
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Min<T>(in Vector4D<T> value1, in Vector4D<T> value2) where T : INumberBase<T>
    {
        return new Vector4D<T>(
        T.MinMagnitudeNumber(value1.X, value2.X), 
        T.MinMagnitudeNumber(value1.Y, value2.Y), 
        T.MinMagnitudeNumber(value1.Z, value2.Z), 
        T.MinMagnitudeNumber(value1.W, value2.W)
        );
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TReturn> Normalize<T, TReturn>(in Vector4D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return value.As<TReturn>() / value.Length<T, TReturn>();
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Normalize<T>(in Vector4D<T> value) where T : INumberBase<T>, IRootFunctions<T>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TReturn> Reflect<T, TReturn>(in Vector4D<T> vector, in Vector4D<T> normal) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var dot = Dot<T, TReturn>(vector, normal);
        return vector.As<TReturn>() - (NumericConstants<TReturn>.Two * (dot * normal.As<TReturn>()));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TReturn> Sqrt<T, TReturn>(in Vector4D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return new Vector4D<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.W))
        );
    }

    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(in Vector4D<T> position, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        return (in Vector4D<T>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<TReturn> Transform<T, TQuat, TReturn>(in Vector4D<T> value, Quaternion<TQuat> rotation) where T : INumberBase<T> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;
        var  = rotation.Z + rotation.Z;
        var  = rotation.W + rotation.W;var x2 = rotation.X + rotation.X;
        var y2 = rotation.Y + rotation.Y;
        var z2 = rotation.Z + rotation.Z;


        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;
        var  = rotation.Z + rotation.Z;
        var  = rotation.W + rotation.W;var wx2 = TReturn.CreateTruncating(rotation.W * x2);
        var wy2 = TReturn.CreateTruncating(rotation.W * y2);
        var wz2 = TReturn.CreateTruncating(rotation.W * z2);
        var xx2 = TReturn.CreateTruncating(rotation.X * x2);
        var xy2 = TReturn.CreateTruncating(rotation.X * y2);
        var xz2 = TReturn.CreateTruncating(rotation.X * z2);
        var yy2 = TReturn.CreateTruncating(rotation.Y * y2);
        var yz2 = TReturn.CreateTruncating(rotation.Y * z2);
        var zz2 = TReturn.CreateTruncating(rotation.Z * z2);

        return new Vector4D<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.W))
        );

        return new Vector4D<TReturn>(
            TReturn.CreateTruncating(value.X) * (TReturn.One - yy2 - zz2) + TReturn.CreateTruncating(value.Y) * (xy2 - wz2) + TReturn.CreateTruncating(value.Z) * (xz2 + wy2),
            TReturn.CreateTruncating(value.X) * (xy2 + wz2) + TReturn.CreateTruncating(value.Y) * (TReturn.One - xx2 - zz2) + TReturn.CreateTruncating(value.Z) * (yz2 - wx2),
            TReturn.CreateTruncating(value.X) * (xz2 - wy2) + TReturn.CreateTruncating(value.Y) * (yz2 + wx2) + TReturn.CreateTruncating(value.Z) * (TReturn.One - xx2 - yy2)
        );
    }

    // /// <summary>Transforms a vector normal by the given 4x4 matrix.</summary>
    // /// <param name="normal">The source vector.</param>
    // /// <param name="matrix">The matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal static Vector4D<T> TransformNormal<T>(in Vector4D<T> normal, in Matrix4x4 matrix) where T : INumberBase<T>
    // {
    //     var matrixX = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
    //     var matrixY = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
    //     var matrixZ = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
    //     // var matrixW = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    //
    //     var result = matrixX * normal.X;
    //     result += matrixY * normal.Y;
    //     result += matrixZ * normal.Z;
    //     return result.AsVector128().AsVector3();
    // }
    */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Remainder<T>(this Vector4D<T> left, in Vector4D<T> right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector4D<T>(
            left.X % right.X,
            left.Y % right.Y,
            left.Z % right.Z,
            left.W % right.W
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Remainder<T>(this Vector4D<T> left, T right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector4D<T>(
            left.X % right,
            left.Y % right,
            left.Z % right,
            left.W % right
        );
    }
    #endregion

    #region Specializations

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Vector4D<T> vec) where T : INumberBase<T>, IRootFunctions<T>
    {
        return vec.Length<T, T>();
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T}" /> method.</remarks>
    /// <altmember cref="Length{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Vector4D<T> vec) where T : INumberBase<T>
    {
        return vec.LengthSquared<T, T>();
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Lerp<T>(in Vector4D<T> value1, in Vector4D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return Lerp<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> LerpClamped<T>(in Vector4D<T> value1, in Vector4D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return LerpClamped<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Lerp<T>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<T> amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return Lerp<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> LerpClamped<T>(in Vector4D<T> value1, in Vector4D<T> value2, Vector4D<T> amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return LerpClamped<T, T>(value1, value2, amount);
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Reflect<T>(in Vector4D<T> vector, in Vector4D<T> normal) where T : IFloatingPoint<T>
    {
        return Reflect<T, T>(vector, normal);
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Sqrt<T>(in Vector4D<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>
    {
        return Sqrt<T, T>(value);
    }

    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(in Vector4D<T> value, Quaternion<T> rotation)
        where T : IFloatingPoint<T>, ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        return Transform<T, T, T>(value, rotation);
    }


    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T, TQuat>(in Vector4D<T> value, Quaternion<TQuat> rotation)
        where T : IFloatingPoint<T>
        where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        return Transform<T, TQuat, T>(value, rotation);
    }
    */
    #endregion

    // Equivalent implementing IHyperbolicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Acosh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Acosh(x.X), T.Acosh(x.Y), T.Acosh(x.Z), T.Acosh(x.W));
    public static Vector4D<T> Asinh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Asinh(x.X), T.Asinh(x.Y), T.Asinh(x.Z), T.Asinh(x.W));
    public static Vector4D<T> Atanh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Atanh(x.X), T.Atanh(x.Y), T.Atanh(x.Z), T.Atanh(x.W));
    public static Vector4D<T> Cosh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Cosh(x.X), T.Cosh(x.Y), T.Cosh(x.Z), T.Cosh(x.W));
    public static Vector4D<T> Sinh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Sinh(x.X), T.Sinh(x.Y), T.Sinh(x.Z), T.Sinh(x.W));
    public static Vector4D<T> Tanh<T>(in Vector4D<T> x) where T : IHyperbolicFunctions<T> => new(T.Tanh(x.X), T.Tanh(x.Y), T.Tanh(x.Z), T.Tanh(x.W));

    // Equivalent implementing ITrigonometricFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Acos<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Acos(x.X), T.Acos(x.Y), T.Acos(x.Z), T.Acos(x.W));
    public static Vector4D<T> AcosPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.AcosPi(x.X), T.AcosPi(x.Y), T.AcosPi(x.Z), T.AcosPi(x.W));
    public static Vector4D<T> Asin<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Asin(x.X), T.Asin(x.Y), T.Asin(x.Z), T.Asin(x.W));
    public static Vector4D<T> AsinPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.AsinPi(x.X), T.AsinPi(x.Y), T.AsinPi(x.Z), T.AsinPi(x.W));
    public static Vector4D<T> Atan<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Atan(x.X), T.Atan(x.Y), T.Atan(x.Z), T.Atan(x.W));
    public static Vector4D<T> AtanPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.AtanPi(x.X), T.AtanPi(x.Y), T.AtanPi(x.Z), T.AtanPi(x.W));
    public static Vector4D<T> Cos<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Cos(x.X), T.Cos(x.Y), T.Cos(x.Z), T.Cos(x.W));
    public static Vector4D<T> CosPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.CosPi(x.X), T.CosPi(x.Y), T.CosPi(x.Z), T.CosPi(x.W));
    public static Vector4D<T> DegreesToRadians<T>(in Vector4D<T> degrees) where T : ITrigonometricFunctions<T> => new(T.DegreesToRadians(degrees.X), T.DegreesToRadians(degrees.Y), T.DegreesToRadians(degrees.Z), T.DegreesToRadians(degrees.W));
    public static Vector4D<T> RadiansToDegrees<T>(in Vector4D<T> radians) where T : ITrigonometricFunctions<T> => new(T.RadiansToDegrees(radians.X), T.RadiansToDegrees(radians.Y), T.RadiansToDegrees(radians.Z), T.RadiansToDegrees(radians.W));
    public static Vector4D<T> Sin<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Sin(x.X), T.Sin(x.Y), T.Sin(x.Z), T.Sin(x.W));
    public static Vector4D<T> SinPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.SinPi(x.X), T.SinPi(x.Y), T.SinPi(x.Z), T.SinPi(x.W));
    public static Vector4D<T> Tan<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.Tan(x.X), T.Tan(x.Y), T.Tan(x.Z), T.Tan(x.W));
    public static Vector4D<T> TanPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T> => new(T.TanPi(x.X), T.TanPi(x.Y), T.TanPi(x.Z), T.TanPi(x.W));


    public static (Vector4D<T> Sin, Vector4D<T> Cos) SinCos<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCos(x.X);
        var (sinY, cosY) = T.SinCos(x.Y);
        var (sinZ, cosZ) = T.SinCos(x.Z);
        var (sinW, cosW) = T.SinCos(x.W);

        return (
            new Vector4D<T>(sinX, sinY, sinZ, sinW),
            new Vector4D<T>(cosX, cosY, cosZ, cosW)
        );
    }

    public static (Vector4D<T> SinPi, Vector4D<T> CosPi) SinCosPi<T>(in Vector4D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCosPi(x.X);
        var (sinY, cosY) = T.SinCosPi(x.Y);
        var (sinZ, cosZ) = T.SinCosPi(x.Z);
        var (sinW, cosW) = T.SinCosPi(x.W);

        return (
            new Vector4D<T>(sinX, sinY, sinZ, sinW),
            new Vector4D<T>(cosX, cosY, cosZ, cosW)
        );
    }

    // Equivalent implementing ILogarithmicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Log<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log(x.X), T.Log(x.Y), T.Log(x.Z), T.Log(x.W));

    public static Vector4D<T> Log<T>(in Vector4D<T> x, in Vector4D<T> newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase.X), T.Log(x.Y, newBase.Y), T.Log(x.Z, newBase.Z), T.Log(x.W, newBase.W));
    public static Vector4D<T> Log<T>(in Vector4D<T> x, T newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase), T.Log(x.Y, newBase), T.Log(x.Z, newBase), T.Log(x.W, newBase));
    public static Vector4D<T> LogP1<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => Log(x + Vector4D<T>.One);
    public static Vector4D<T> Log2<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log2(x.X), T.Log2(x.Y), T.Log2(x.Z), T.Log2(x.W));
    public static Vector4D<T> Log2P1<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => Log2(x + Vector4D<T>.One);
    public static Vector4D<T> Log10<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log10(x.X), T.Log10(x.Y), T.Log10(x.Z), T.Log10(x.W));
    public static Vector4D<T> Log10P1<T>(in Vector4D<T> x) where T : ILogarithmicFunctions<T> => Log10(x + Vector4D<T>.One);

    // Equivalent implementing IExponentialFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Exp<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => new(T.Exp(x.X), T.Exp(x.Y), T.Exp(x.Z), T.Exp(x.W));
    public static Vector4D<T> ExpM1<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => Exp(x) - Vector4D<T>.One;
    public static Vector4D<T> Exp2<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => new(T.Exp2(x.X), T.Exp2(x.Y), T.Exp2(x.Z), T.Exp2(x.W));
    public static Vector4D<T> Exp2M1<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => Exp2(x) - Vector4D<T>.One;
    public static Vector4D<T> Exp10<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => new(T.Exp10(x.X), T.Exp10(x.Y), T.Exp10(x.Z), T.Exp10(x.W));
    public static Vector4D<T> Exp10M1<T>(in Vector4D<T> x) where T : IExponentialFunctions<T> => Exp10(x) - Vector4D<T>.One;

    // Equivalent implementing IPowerFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Pow<T>(in Vector4D<T> x, in Vector4D<T> y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y.X), T.Pow(x.Y, y.Y), T.Pow(x.Z, y.Z), T.Pow(x.W, y.W));
    public static Vector4D<T> Pow<T>(in Vector4D<T> x, T y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y), T.Pow(x.Y, y), T.Pow(x.Z, y), T.Pow(x.W, y));

    // Equivalent implementing IRootFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector4D<T> Cbrt<T>(in Vector4D<T> x) where T : IRootFunctions<T> => new(T.Cbrt(x.X), T.Cbrt(x.Y), T.Cbrt(x.Z), T.Cbrt(x.W));
    public static Vector4D<T> Hypot<T>(in Vector4D<T> x, in Vector4D<T> y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y.X), T.Hypot(x.Y, y.Y), T.Hypot(x.Z, y.Z), T.Hypot(x.W, y.W));
    public static Vector4D<T> Hypot<T>(in Vector4D<T> x, T y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y), T.Hypot(x.Y, y), T.Hypot(x.Z, y), T.Hypot(x.W, y));
    public static Vector4D<T> RootN<T>(in Vector4D<T> x, int n) where T : IRootFunctions<T> => new(T.RootN(x.X, n), T.RootN(x.Y, n), T.RootN(x.Z, n), T.RootN(x.W, n));

    // IFloatingPoint<TSelf>
    public static Vector4D<T> Round<T>(in Vector4D<T> x) where T : IFloatingPoint<T> => new(T.Round(x.X), T.Round(x.Y), T.Round(x.Z), T.Round(x.W));
    public static Vector4D<T> Round<T>(in Vector4D<T> x, int digits) where T : IFloatingPoint<T> => new(T.Round(x.X, digits), T.Round(x.Y, digits), T.Round(x.Z, digits), T.Round(x.W, digits));
    public static Vector4D<T> Round<T>(in Vector4D<T> x, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, mode), T.Round(x.Y, mode), T.Round(x.Z, mode), T.Round(x.W, mode));
    public static Vector4D<T> Round<T>(in Vector4D<T> x, int digits, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, digits, mode), T.Round(x.Y, digits, mode), T.Round(x.Z, digits, mode), T.Round(x.W, digits, mode));
    public static Vector4D<T> Truncate<T>(in Vector4D<T> x) where T : IFloatingPoint<T> => new(T.Truncate(x.X), T.Truncate(x.Y), T.Truncate(x.Z), T.Truncate(x.W));

    // IFloatingPointIeee754<TSelf>
    public static Vector4D<T> Atan2<T>(in Vector4D<T> x, in Vector4D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y.X), T.Atan2(x.Y, y.Y), T.Atan2(x.Z, y.Z), T.Atan2(x.W, y.W));
    public static Vector4D<T> Atan2Pi<T>(in Vector4D<T> x, in Vector4D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y.X), T.Atan2Pi(x.Y, y.Y), T.Atan2Pi(x.Z, y.Z), T.Atan2Pi(x.W, y.W));
    public static Vector4D<T> Atan2<T>(in Vector4D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y), T.Atan2(x.Y, y), T.Atan2(x.Z, y), T.Atan2(x.W, y));
    public static Vector4D<T> Atan2Pi<T>(in Vector4D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y), T.Atan2Pi(x.Y, y), T.Atan2Pi(x.Z, y), T.Atan2Pi(x.W, y));
    public static Vector4D<T> BitDecrement<T>(in Vector4D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitDecrement(x.X), T.BitDecrement(x.Y), T.BitDecrement(x.Z), T.BitDecrement(x.W));
    public static Vector4D<T> BitIncrement<T>(in Vector4D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitIncrement(x.X), T.BitIncrement(x.Y), T.BitIncrement(x.Z), T.BitIncrement(x.W));

    public static Vector4D<T> FusedMultiplyAdd<T>(in Vector4D<T> left, in Vector4D<T> right, Vector4D<T> addend) where T : IFloatingPointIeee754<T> => new(T.FusedMultiplyAdd(left.X, right.X, addend.X), T.FusedMultiplyAdd(left.Y, right.Y, addend.Y), T.FusedMultiplyAdd(left.Z, right.Z, addend.Z), T.FusedMultiplyAdd(left.W, right.W, addend.W));
    public static Vector4D<T> ReciprocalEstimate<T>(in Vector4D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalEstimate(x.X), T.ReciprocalEstimate(x.Y), T.ReciprocalEstimate(x.Z), T.ReciprocalEstimate(x.W));
    public static Vector4D<T> ReciprocalSqrtEstimate<T>(in Vector4D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalSqrtEstimate(x.X), T.ReciprocalSqrtEstimate(x.Y), T.ReciprocalSqrtEstimate(x.Z), T.ReciprocalSqrtEstimate(x.W));

    // INumber<T>
    // public static Vector4D<T> Clamp<T>(in Vector4D<T> value, in Vector4D<T> min, Vector4D<T> max) where T : INumber<T> => new(T.Clamp(x.X), T.Clamp(x.Y), T.Clamp(x.Z), T.Clamp(x.W));
    public static Vector4D<T> CopySign<T>(in Vector4D<T> value, in Vector4D<T> sign) where T : INumber<T> => new(T.CopySign(value.X, sign.X), T.CopySign(value.Y, sign.Y), T.CopySign(value.Z, sign.Z), T.CopySign(value.W, sign.W));
    public static Vector4D<T> CopySign<T>(in Vector4D<T> value, T sign) where T : INumber<T> => new(T.CopySign(value.X, sign), T.CopySign(value.Y, sign), T.CopySign(value.Z, sign), T.CopySign(value.W, sign));
    public static Vector4D<T> MaxNumber<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumber<T> => new(T.MaxNumber(x.X, y.X), T.MaxNumber(x.Y, y.Y), T.MaxNumber(x.Z, y.Z), T.MaxNumber(x.W, y.W));
    public static Vector4D<T> MinNumber<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumber<T> => new(T.MinNumber(x.X, y.X), T.MinNumber(x.Y, y.Y), T.MinNumber(x.Z, y.Z), T.MinNumber(x.W, y.W));

    // INumberBase<T>
    // public static Vector4D<T> MaxMagnitude<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumberBase<T> => new(T.MaxMagnitude(x.X, y.X), T.MaxMagnitude(x.Y, y.Y), T.MaxMagnitude(x.Z, y.Z), T.MaxMagnitude(x.W, y.W));
    // public static Vector4D<T> MaxMagnitudeNumber<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumberBase<T> => new(T.MaxMagnitudeNumber(x.X, y.X), T.MaxMagnitudeNumber(x.Y, y.Y), T.MaxMagnitudeNumber(x.Z, y.Z), T.MaxMagnitudeNumber(x.W, y.W));
    // public static Vector4D<T> MinMagnitude<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumberBase<T> => new(T.MinMagnitude(x.X, y.X), T.MinMagnitude(x.Y, y.Y), T.MinMagnitude(x.Z, y.Z), T.MinMagnitude(x.W, y.W));
    // public static Vector4D<T> MinMagnitudeNumber<T>(in Vector4D<T> x, in Vector4D<T> y) where T : INumberBase<T> => new(T.MinMagnitudeNumber(x.X, y.X), T.MinMagnitudeNumber(x.Y, y.Y), T.MinMagnitudeNumber(x.Z, y.Z), T.MinMagnitudeNumber(x.W, y.W));
    // (there's no reason you would want these.)



    // IFloatingPointIeee754<TSelf>
    public static Vector4D<int> ILogB<T>(in Vector4D<T> x) where T : IFloatingPointIeee754<T> => new(T.ILogB(x.X), T.ILogB(x.Y), T.ILogB(x.Z), T.ILogB(x.W));
    public static Vector4D<T> ScaleB<T>(in Vector4D<T> x, Vector4D<int> n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n.X), T.ScaleB(x.Y, n.Y), T.ScaleB(x.Z, n.Z), T.ScaleB(x.W, n.W));
    public static Vector4D<T> ScaleB<T>(in Vector4D<T> x, int n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n), T.ScaleB(x.Y, n), T.ScaleB(x.Z, n), T.ScaleB(x.W, n));

    public static Vector4D<int> RoundToInt<T>(in Vector4D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector4D<int>(
            int.CreateSaturating(T.Round(vector.X)),
            int.CreateSaturating(T.Round(vector.Y)),
            int.CreateSaturating(T.Round(vector.Z)),
            int.CreateSaturating(T.Round(vector.W))
        );
    }

    public static Vector4D<int> FloorToInt<T>(in Vector4D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector4D<int>(
            int.CreateSaturating(T.Floor(vector.X)),
            int.CreateSaturating(T.Floor(vector.Y)),
            int.CreateSaturating(T.Floor(vector.Z)),
            int.CreateSaturating(T.Floor(vector.W))
        );
    }

    public static Vector4D<int> CeilingToInt<T>(in Vector4D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector4D<int>(
            int.CreateSaturating(T.Ceiling(vector.X)),
            int.CreateSaturating(T.Ceiling(vector.Y)),
            int.CreateSaturating(T.Ceiling(vector.Z)),
            int.CreateSaturating(T.Ceiling(vector.W))
        );
    }

    public static Vector4D<TInt> RoundToInt<T, TInt>(in Vector4D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector4D<TInt>(
            TInt.CreateSaturating(T.Round(vector.X)),
            TInt.CreateSaturating(T.Round(vector.Y)),
            TInt.CreateSaturating(T.Round(vector.Z)),
            TInt.CreateSaturating(T.Round(vector.W))
        );
    }

    public static Vector4D<TInt> FloorToInt<T, TInt>(in Vector4D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector4D<TInt>(
            TInt.CreateSaturating(T.Floor(vector.X)),
            TInt.CreateSaturating(T.Floor(vector.Y)),
            TInt.CreateSaturating(T.Floor(vector.Z)),
            TInt.CreateSaturating(T.Floor(vector.W))
        );
    }

    public static Vector4D<TInt> CeilingToInt<T, TInt>(in Vector4D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector4D<TInt>(
            TInt.CreateSaturating(T.Ceiling(vector.X)),
            TInt.CreateSaturating(T.Ceiling(vector.Y)),
            TInt.CreateSaturating(T.Ceiling(vector.Z)),
            TInt.CreateSaturating(T.Ceiling(vector.W))
        );
    }

    public static Vector4D<float> AsGeneric(this Vector4 vector)
        => Unsafe.BitCast<Vector4, Vector4D<float>>(vector);

    public static Vector4 AsNumerics(this Vector4D<float> vector)
        => Unsafe.BitCast<Vector4D<float>, Vector4>(vector);
}

// IVector<Vector4D<T>, T>
public readonly partial struct Vector4D<T>
{
    T IVector<Vector4D<T>, T>.LengthSquared()
        => this.LengthSquared();
    static Vector4D<T> IVector<Vector4D<T>, T>.Multiply(in Vector4D<T> left, in Vector4D<T> right)
        => Vector4D.Multiply(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Multiply(in Vector4D<T> left, T right)
        => Vector4D.Multiply(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Multiply(T left, in Vector4D<T> right)
        => Vector4D.Multiply(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Negate(in Vector4D<T> value)
        => Vector4D.Negate(value);
    static Vector4D<T> IVector<Vector4D<T>, T>.Subtract(in Vector4D<T> left, in Vector4D<T> right)
        => Vector4D.Subtract(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Add(in Vector4D<T> left, in Vector4D<T> right)
        => Vector4D.Add(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Divide(in Vector4D<T> left, in Vector4D<T> right)
        => Vector4D.Divide(left, right);
    static Vector4D<T> IVector<Vector4D<T>, T>.Divide(in Vector4D<T> left, T divisor)
        => Vector4D.Divide(left, divisor);
    static Vector4D<T> IVector<Vector4D<T>, T>.Clamp(in Vector4D<T> value1, in Vector4D<T> min, in Vector4D<T> max)
        => Vector4D.Clamp(value1, min, max);
    static TReturn IVector<Vector4D<T>, T>.Distance<TReturn>(in Vector4D<T> value1, in Vector4D<T> value2)
        => Vector4D.Distance<T, TReturn>(value1, value2);
    static T IVector<Vector4D<T>, T>.DistanceSquared(in Vector4D<T> value1, in Vector4D<T> value2)
        => Vector4D.DistanceSquared(value1, value2);
    static TReturn IVector<Vector4D<T>, T>.DistanceSquared<TReturn>(in Vector4D<T> value1, in Vector4D<T> value2)
        => Vector4D.DistanceSquared<T, TReturn>(value1, value2);
    static T IVector<Vector4D<T>, T>.Dot(in Vector4D<T> vector1, in Vector4D<T> vector2)
        => Vector4D.Dot(vector1, vector2);
    static TReturn IVector<Vector4D<T>, T>.Dot<TReturn>(in Vector4D<T> vector1, in Vector4D<T> vector2)
        => Vector4D.Dot<T, TReturn>(vector1, vector2);
    static Vector4D<T> IVector<Vector4D<T>, T>.Max(in Vector4D<T> value1, in Vector4D<T> value2)
        => Vector4D.Max(value1, value2);
    static Vector4D<T> IVector<Vector4D<T>, T>.Min(in Vector4D<T> value1, in Vector4D<T> value2)
        => Vector4D.Min(value1, value2);

    static Vector4D<T> IVector<Vector4D<T>, T>.Lerp(in Vector4D<T> value1, in Vector4D<T> value2, T amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector4D<T>, T>(typeof(IFloatingPoint<>));
        return Vector4D.LerpUnchecked(value1, value2, amount);
    }

    static Vector4D<T> IVector<Vector4D<T>, T>.LerpClamped(in Vector4D<T> value1, in Vector4D<T> value2, T amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector4D<T>, T>(typeof(IFloatingPoint<>));
        return Vector4D.LerpClampedUnchecked(value1, value2, amount);
    }

    static Vector4D<T> IVector<Vector4D<T>, T>.Lerp(in Vector4D<T> value1, in Vector4D<T> value2, in Vector4D<T> amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector4D<T>, T>(typeof(IFloatingPoint<>));
        return Vector4D.LerpUnchecked(value1, value2, amount);
    }

    static Vector4D<T> IVector<Vector4D<T>, T>.LerpClamped(in Vector4D<T> value1, in Vector4D<T> value2, in Vector4D<T> amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector4D<T>, T>(typeof(IFloatingPoint<>));
        return Vector4D.LerpClampedUnchecked(value1, value2, amount);
    }

    static Vector4D<T> IVector<Vector4D<T>, T>.Reflect(in Vector4D<T> vector, in Vector4D<T> normal) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector4D<T>, T>(typeof(IFloatingPoint<>));
        return Vector4D.Reflect<T, T>(vector, normal);
    }
}