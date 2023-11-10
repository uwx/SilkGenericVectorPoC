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


// Vector5D<T>
/// <summary>A structure encapsulating five values, usually geometric vectors, and provides hardware accelerated methods.</summary>
[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly partial struct Vector5D<T> : IVector<Vector5D<T>, T>, IVectorAlso<Vector5D<T>, T>, ISpanFormattable
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
    private readonly T _v;
    /// <summary>The V component of the vector.</summary>
    [DataMember]
    public T V => _v;

    internal const int Count = 5;

    /// <summary>Creates a new <see cref="Vector5D{T}" /> object whose five elements have the same value.</summary>
    /// <param name="value">The value to assign to all five elements.</param>
    public Vector5D(T value) : this(value, value, value, value, value)
    {
    }

    /// <summary>Creates a new <see cref="Vector5D{T}" /> object from the specified <see cref="Vector5D{T}" /> object X and a Y and a Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    /// <param name="v">The V component.</param>
    public Vector5D(Vector2D<T> value, T z, T w, T v) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, , z, w, v)
    {
    }
    /// <summary>Creates a new <see cref="Vector5D{T}" /> object from the specified <see cref="Vector5D{T}" /> object X and a Y and a Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="w">The W component.</param>
    /// <param name="v">The V component.</param>
    public Vector5D(Vector3D<T> value, T w, T v) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, , , w, v)
    {
    }
    /// <summary>Creates a new <see cref="Vector5D{T}" /> object from the specified <see cref="Vector5D{T}" /> object X and a Y and a Z and a W and a V component.</summary>
    /// <param name="value">The vector to use for the Scriban.Runtime.ScriptRange components.</param>
    /// <param name="v">The V component.</param>
    public Vector5D(Vector4D<T> value, T v) : this(value.Xvalue.Yvalue.Zvalue.Wvalue.Vvalue.Xvalue.Yvalue.Zvalue.Wvalue.V, , , , v)
    {
    }

    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    /// <param name="v">The value to assign to the <see cref="V" /> field.</param>
    public Vector5D(T x, T y, T z, T w, T v)
    {
        Unsafe.SkipInit(out this);

        _x = x;
        _y = y;
        _z = z;
        _w = w;
        _v = v;
    }

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 5 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector5D(ReadOnlySpan<T> values)
    {
        Unsafe.SkipInit(out this);

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, Count, nameof(values));

        this = Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }

    /// <summary>Gets a vector whose 5 elements are equal to zero.</summary>
    /// <value>A vector whose five elements are equal to zero (that is, it returns the vector <c>(0,0,0,0,0)</c>.</value>
    public static Vector5D<T> Zero => new(T.Zero);

    /// <summary>Gets a vector whose 5 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector5D{T}" />.</value>
    /// <remarks>A vector whose five elements are equal to one (that is, it returns the vector <c>(1,1,1,1,1)</c>.</remarks>
    public static Vector5D<T> One => new(T.One);

    /// <summary>Gets the vector (1,0,0,0,0)).</summary>
    /// <value>The vector <c>(1,0,0,0,0)</c>.</value>
    public static Vector5D<T> UnitX => new(T.One, T.Zero, T.Zero, T.Zero, T.Zero);
    /// <summary>Gets the vector (0,1,0,0,0)).</summary>
    /// <value>The vector <c>(0,1,0,0,0)</c>.</value>
    public static Vector5D<T> UnitY => new(T.Zero, T.One, T.Zero, T.Zero, T.Zero);
    /// <summary>Gets the vector (0,0,1,0,0)).</summary>
    /// <value>The vector <c>(0,0,1,0,0)</c>.</value>
    public static Vector5D<T> UnitZ => new(T.Zero, T.Zero, T.One, T.Zero, T.Zero);
    /// <summary>Gets the vector (0,0,0,1,0)).</summary>
    /// <value>The vector <c>(0,0,0,1,0)</c>.</value>
    public static Vector5D<T> UnitW => new(T.Zero, T.Zero, T.Zero, T.One, T.Zero);
    /// <summary>Gets the vector (0,0,0,0,1)).</summary>
    /// <value>The vector <c>(0,0,0,0,1)</c>.</value>
    public static Vector5D<T> UnitV => new(T.Zero, T.Zero, T.Zero, T.Zero, T.One);

    /// <summary>Creates a new <see cref="Vector5D{T}" /> object whose five elements have the same value.</summary>
    /// <param name="value">The value to assign to all five elements.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Create(T scalar) => new(scalar);

    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    /// <param name="w">The value to assign to the <see cref="W" /> field.</param>
    /// <param name="v">The value to assign to the <see cref="V" /> field.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Create(T x, T y, T z, T w, T v) => new(x, y, z, w, v);

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 5 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Create(ReadOnlySpan<T> values) => new(values);

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
    /// <remarks>The <see cref="op_Addition" /> method defines the addition operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator +(Vector5D<T> left, Vector5D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 + v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new Vector5D<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z,
            left.W + right.W,
            left.V + right.V
        );
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the subtraction operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator -(Vector5D<T> left, Vector5D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 - v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new Vector5D<T>(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z,
            left.W - right.W,
            left.V - right.V
        );
    }

    /// <summary>Negates the specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    /// <remarks>The <see cref="op_UnaryNegation" /> method defines the unary negation operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator -(Vector5D<T> value)
    {
        return Zero - value;
    }

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    /// <remarks>The <see cref="Vector5D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator *(Vector5D<T> left, Vector5D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 * v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new Vector5D<T>(
            left.X * right.X,
            left.Y * right.Y,
            left.Z * right.Z,
            left.W * right.W,
            left.V * right.V
        );
    }

    /// <summary>Multiplies the specified vector by the specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector5D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator *(Vector5D<T> left, T right)
    {
        return left * new Vector5D<T>(right);
    }

    /// <summary>Multiplies the scalar value by the specified vector.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector5D{T}.op_Multiply" /> method defines the multiplication operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator *(T left, Vector5D<T> right)
    {
        return right * left;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    /// <remarks>The <see cref="Vector5D{T}.op_Division" /> method defines the division operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator /(Vector5D<T> left, Vector5D<T> right)
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), left);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), right);
                
                v0 = v0 / v1;
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new Vector5D<T>(
            left.X / right.X,
            left.Y / right.Y,
            left.Z / right.Z,
            left.W / right.W,
            left.V / right.V
        );
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="value1">The vector.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    /// <remarks>The <see cref="Vector5D{T}.op_Division" /> method defines the division operation for <see cref="Vector5D{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> operator /(Vector5D<T> value1, T value2)
    {
        return value1 / new Vector5D<T>(value2);
    }

    /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Vector5D{T}" /> objects are equal if each element in <paramref name="left" /> is equal to the corresponding element in <paramref name="right" />.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector5D<T> left, Vector5D<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>Returns a value that indicates whether two specified vectors are not equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector5D<T> left, Vector5D<T> right)
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
    public bool Equals(Vector5D<T> other)
    {
        /*// NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
unsafe
{
    if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
    {
        Vector64<T> v0 = default;
        Vector64<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), this);
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), other);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
    }

    if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
    {
        Vector128<T> v0 = default;
        Vector128<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), this);
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), other);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
    }

    if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
    {
        Vector256<T> v0 = default;
        Vector256<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), this);
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), other);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
    }

    if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
    {
        Vector512<T> v0 = default;
        Vector512<T> v1 = default;
        
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), this);
        Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), other);
        
        v0 = v0.Equals(v1);
        return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
    }
}
*/

        //return SpeedHelpers.FastEqualsUpTo4<Vector5D<T>, T>(this, other);
        return
            X.Equals(other.X) &&
            Y.Equals(other.Y) &&
            Z.Equals(other.Z) &&
            W.Equals(other.W) &&
            V.Equals(other.V);
    }

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Vector5D{T}" /> object and their corresponding elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Vector5D<T> other) && Equals(other);
    }


    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W, V);
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
        handler.AppendLiteral(separator);
        handler.AppendLiteral(" ");
        handler.AppendFormatted(V, format);
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
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(V, formatString) &&
            handler.AppendLiteral(">");

        return destination.TryWrite(ref handler, out charsWritten);
    }
    #endregion

    #region Casts
    public Vector5D<TOther> As<TOther>() where TOther : INumberBase<TOther>
    {
        if (SpeedHelpers.TryFastConvert<Vector5D<T>, T, Vector5D<TOther>, TOther>(this, out var result))
        {
            return result;
        }

        return new Vector5D<TOther>(
            TOther.CreateTruncating(X),
            TOther.CreateTruncating(Y),
            TOther.CreateTruncating(Z),
            TOther.CreateTruncating(W),
            TOther.CreateTruncating(V)
        );
    }

    private Vector5D<TOther> AsChecked<TOther>() where TOther : INumberBase<TOther>
    {
        return new Vector5D<TOther>(
            TOther.CreateChecked(X),
            TOther.CreateChecked(Y),
            TOther.CreateChecked(Z),
            TOther.CreateChecked(W),
            TOther.CreateChecked(V)
        );
    }
    public static explicit operator Vector5D<byte>(Vector5D<T> self) => self.As<byte>();
    public static explicit operator Vector5D<sbyte>(Vector5D<T> self) => self.As<sbyte>();
    public static explicit operator Vector5D<short>(Vector5D<T> self) => self.As<short>();
    public static explicit operator Vector5D<ushort>(Vector5D<T> self) => self.As<ushort>();
    public static explicit operator Vector5D<int>(Vector5D<T> self) => self.As<int>();
    public static explicit operator Vector5D<uint>(Vector5D<T> self) => self.As<uint>();
    public static explicit operator Vector5D<long>(Vector5D<T> self) => self.As<long>();
    public static explicit operator Vector5D<ulong>(Vector5D<T> self) => self.As<ulong>();
    public static explicit operator Vector5D<Int128>(Vector5D<T> self) => self.As<Int128>();
    public static explicit operator Vector5D<UInt128>(Vector5D<T> self) => self.As<UInt128>();
    public static explicit operator Vector5D<Half>(Vector5D<T> self) => self.As<Half>();
    public static explicit operator Vector5D<float>(Vector5D<T> self) => self.As<float>();
    public static explicit operator Vector5D<double>(Vector5D<T> self) => self.As<double>();
    public static explicit operator Vector5D<decimal>(Vector5D<T> self) => self.As<decimal>();
    public static explicit operator Vector5D<Complex>(Vector5D<T> self) => self.As<Complex>();
    public static explicit operator Vector5D<BigInteger>(Vector5D<T> self) => self.As<BigInteger>();

    public static explicit operator checked Vector5D<byte>(Vector5D<T> self) => self.AsChecked<byte>();
    public static explicit operator checked Vector5D<sbyte>(Vector5D<T> self) => self.AsChecked<sbyte>();
    public static explicit operator checked Vector5D<short>(Vector5D<T> self) => self.AsChecked<short>();
    public static explicit operator checked Vector5D<ushort>(Vector5D<T> self) => self.AsChecked<ushort>();
    public static explicit operator checked Vector5D<int>(Vector5D<T> self) => self.AsChecked<int>();
    public static explicit operator checked Vector5D<uint>(Vector5D<T> self) => self.AsChecked<uint>();
    public static explicit operator checked Vector5D<long>(Vector5D<T> self) => self.AsChecked<long>();
    public static explicit operator checked Vector5D<ulong>(Vector5D<T> self) => self.AsChecked<ulong>();
    public static explicit operator checked Vector5D<Int128>(Vector5D<T> self) => self.AsChecked<Int128>();
    public static explicit operator checked Vector5D<UInt128>(Vector5D<T> self) => self.AsChecked<UInt128>();
    public static explicit operator checked Vector5D<Half>(Vector5D<T> self) => self.AsChecked<Half>();
    public static explicit operator checked Vector5D<float>(Vector5D<T> self) => self.AsChecked<float>();
    public static explicit operator checked Vector5D<double>(Vector5D<T> self) => self.AsChecked<double>();
    public static explicit operator checked Vector5D<decimal>(Vector5D<T> self) => self.AsChecked<decimal>();
    public static explicit operator checked Vector5D<Complex>(Vector5D<T> self) => self.AsChecked<Complex>();
    public static explicit operator checked Vector5D<BigInteger>(Vector5D<T> self) => self.AsChecked<BigInteger>();

    // Cast to System.Numerics.Vector5

    // Downcast
    public static explicit operator Vector2D<T>(Vector5D<T> self) => new(self.X, self.Y);
    public static explicit operator Vector3D<T>(Vector5D<T> self) => new(self.X, self.Y, self.Z);
    public static explicit operator Vector4D<T>(Vector5D<T> self) => new(self.X, self.Y, self.Z, self.W);

    // Upcast

    // Upcast from System.Numerics.Vector < 5
    public static explicit operator Vector5D<T>(Vector2 self) => new(T.CreateTruncating(self.X), T.CreateTruncating(self.Y), T.Zero, T.Zero, T.Zero);
    public static explicit operator checked Vector5D<T>(Vector2 self) => new(T.CreateChecked(self.X), T.CreateChecked(self.Y), T.Zero, T.Zero, T.Zero);
    public static explicit operator Vector5D<T>(Vector3 self) => new(T.CreateTruncating(self.X), T.CreateTruncating(self.Y), T.CreateTruncating(self.Z), T.Zero, T.Zero);
    public static explicit operator checked Vector5D<T>(Vector3 self) => new(T.CreateChecked(self.X), T.CreateChecked(self.Y), T.CreateChecked(self.Z), T.Zero, T.Zero);
    public static explicit operator Vector5D<T>(Vector4 self) => new(T.CreateTruncating(self.X), T.CreateTruncating(self.Y), T.CreateTruncating(self.Z), T.CreateTruncating(self.W), T.Zero);
    public static explicit operator checked Vector5D<T>(Vector4 self) => new(T.CreateChecked(self.X), T.CreateChecked(self.Y), T.CreateChecked(self.Z), T.CreateChecked(self.W), T.Zero);

    // Downcast from System.Numerics.Vector >= 5

    public static implicit operator Vector5D<T>((T X, T Y, T Z, T W, T V) components)
        => new(components.X, components.Y, components.Z, components.W, components.V);

    #endregion

    public void Deconstruct(out T x, out T y, out T z, out T w, out T v)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
        v = V;
    }
}

file interface IVec5
{
    // Returns null if incompatible. Throws OverflowException if overflowing
    Vector5D<T>? GetChecked<T>() where T : INumberBase<T>;
    Vector5D<T>? GetSaturating<T>() where T : INumberBase<T>;
    Vector5D<T>? GetTruncating<T>() where T : INumberBase<T>;
}

// Vector5D<T>.INumber
public readonly partial struct Vector5D<T> :
    IDivisionOperators<Vector5D<T>, T, Vector5D<T>>,
    IMultiplyOperators<Vector5D<T>, T, Vector5D<T>>,
    INumberBase<Vector5D<T>>,
    IVec5
{
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector5D<T> INumberBase<Vector5D<T>>.Abs(Vector5D<T> value) => Vector5D.Abs(value);

    static Vector5D<T> IParsable<Vector5D<T>>.Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), NumberStyles.None, provider);

    static Vector5D<T> ISpanParsable<Vector5D<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s, NumberStyles.None, provider);

    public static Vector5D<T> Parse(string s, NumberStyles style = default, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), style, provider);

    public static Vector5D<T> Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null)
        => TryParse(s, style, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector5D)}<{typeof(T)}>");

    public static bool TryParse(string? s, IFormatProvider? provider, out Vector5D<T> result)
        => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vector5D<T> result)
        => TryParse(s.AsSpan(), style, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector5D<T> result)
        => TryParse(s, NumberStyles.None, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector5D<T> result)
    {
        result = default;

        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;

        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        s = s[1..^1];

        T? x, y, z, w, v;

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

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style, provider, out w)) return false;

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, style, provider, out v)) return false;
        }

        result = new Vector5D<T>(x, y, z, w, v);
        return true;
    }

    static bool INumberBase<Vector5D<T>>.IsCanonical(Vector5D<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z) && T.IsCanonical(value.W) && T.IsCanonical(value.V);

    static bool INumberBase<Vector5D<T>>.IsComplexNumber(Vector5D<T> value) => T.IsComplexNumber(value.X) || T.IsComplexNumber(value.Y) || T.IsComplexNumber(value.Z) || T.IsComplexNumber(value.W) || T.IsComplexNumber(value.V);

    static bool INumberBase<Vector5D<T>>.IsEvenInteger(Vector5D<T> value) => T.IsEvenInteger(value.X) && T.IsEvenInteger(value.Y) && T.IsEvenInteger(value.Z) && T.IsEvenInteger(value.W) && T.IsEvenInteger(value.V);

    static bool INumberBase<Vector5D<T>>.IsFinite(Vector5D<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z) && T.IsFinite(value.W) && T.IsFinite(value.V);

    static bool INumberBase<Vector5D<T>>.IsImaginaryNumber(Vector5D<T> value) => T.IsImaginaryNumber(value.X) || T.IsImaginaryNumber(value.Y) || T.IsImaginaryNumber(value.Z) || T.IsImaginaryNumber(value.W) || T.IsImaginaryNumber(value.V);

    static bool INumberBase<Vector5D<T>>.IsInfinity(Vector5D<T> value) => T.IsInfinity(value.X) && T.IsInfinity(value.Y) && T.IsInfinity(value.Z) && T.IsInfinity(value.W) && T.IsInfinity(value.V);

    static bool INumberBase<Vector5D<T>>.IsInteger(Vector5D<T> value) => T.IsInteger(value.X) && T.IsInteger(value.Y) && T.IsInteger(value.Z) && T.IsInteger(value.W) && T.IsInteger(value.V);

    static bool INumberBase<Vector5D<T>>.IsNaN(Vector5D<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z) || T.IsNaN(value.W) || T.IsNaN(value.V);

    static bool INumberBase<Vector5D<T>>.IsNegative(Vector5D<T> value) => T.IsNegative(value.X) && T.IsNegative(value.Y) && T.IsNegative(value.Z) && T.IsNegative(value.W) && T.IsNegative(value.V);

    static bool INumberBase<Vector5D<T>>.IsNegativeInfinity(Vector5D<T> value) => T.IsNegativeInfinity(value.X) && T.IsNegativeInfinity(value.Y) && T.IsNegativeInfinity(value.Z) && T.IsNegativeInfinity(value.W) && T.IsNegativeInfinity(value.V);

    static bool INumberBase<Vector5D<T>>.IsNormal(Vector5D<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z) && T.IsNormal(value.W) && T.IsNormal(value.V);

    static bool INumberBase<Vector5D<T>>.IsOddInteger(Vector5D<T> value) => T.IsOddInteger(value.X) && T.IsOddInteger(value.Y) && T.IsOddInteger(value.Z) && T.IsOddInteger(value.W) && T.IsOddInteger(value.V);

    static bool INumberBase<Vector5D<T>>.IsPositive(Vector5D<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z) && T.IsPositive(value.W) && T.IsPositive(value.V);

    static bool INumberBase<Vector5D<T>>.IsPositiveInfinity(Vector5D<T> value) => T.IsPositiveInfinity(value.X) && T.IsPositiveInfinity(value.Y) && T.IsPositiveInfinity(value.Z) && T.IsPositiveInfinity(value.W) && T.IsPositiveInfinity(value.V);

    static bool INumberBase<Vector5D<T>>.IsRealNumber(Vector5D<T> value) => T.IsRealNumber(value.X) && T.IsRealNumber(value.Y) && T.IsRealNumber(value.Z) && T.IsRealNumber(value.W) && T.IsRealNumber(value.V);

    static bool INumberBase<Vector5D<T>>.IsSubnormal(Vector5D<T> value) => T.IsSubnormal(value.X) && T.IsSubnormal(value.Y) && T.IsSubnormal(value.Z) && T.IsSubnormal(value.W) && T.IsSubnormal(value.V);

    static bool INumberBase<Vector5D<T>>.IsZero(Vector5D<T> value) => T.IsZero(value.X) && T.IsZero(value.Y) && T.IsZero(value.Z) && T.IsZero(value.W) && T.IsZero(value.V);

    static Vector5D<T> INumberBase<Vector5D<T>>.MaxMagnitude(Vector5D<T> x, Vector5D<T> y) => new(T.MaxMagnitude(x.X, y.X), T.MaxMagnitude(x.Y, y.Y), T.MaxMagnitude(x.Z, y.Z), T.MaxMagnitude(x.W, y.W), T.MaxMagnitude(x.V, y.V));

    static Vector5D<T> INumberBase<Vector5D<T>>.MaxMagnitudeNumber(Vector5D<T> x, Vector5D<T> y) => new(T.MaxMagnitudeNumber(x.X, y.X), T.MaxMagnitudeNumber(x.Y, y.Y), T.MaxMagnitudeNumber(x.Z, y.Z), T.MaxMagnitudeNumber(x.W, y.W), T.MaxMagnitudeNumber(x.V, y.V));

    static Vector5D<T> INumberBase<Vector5D<T>>.MinMagnitude(Vector5D<T> x, Vector5D<T> y) => new(T.MinMagnitude(x.X, y.X), T.MinMagnitude(x.Y, y.Y), T.MinMagnitude(x.Z, y.Z), T.MinMagnitude(x.W, y.W), T.MinMagnitude(x.V, y.V));

    static Vector5D<T> INumberBase<Vector5D<T>>.MinMagnitudeNumber(Vector5D<T> x, Vector5D<T> y) => new(T.MinMagnitudeNumber(x.X, y.X), T.MinMagnitudeNumber(x.Y, y.Y), T.MinMagnitudeNumber(x.Z, y.Z), T.MinMagnitudeNumber(x.W, y.W), T.MinMagnitudeNumber(x.V, y.V));

    static bool INumberBase<Vector5D<T>>.TryConvertFromChecked<TOther>(TOther value, out Vector5D<T> result)
    {
        if (value is Vector5D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec5 IVec5 && IVec5.GetChecked<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector5D<T>>.TryConvertFromSaturating<TOther>(TOther value, out Vector5D<T> result)
    {
        if (value is Vector5D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec5 IVec5 && IVec5.GetSaturating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector5D<T>>.TryConvertFromTruncating<TOther>(TOther value, out Vector5D<T> result)
    {
        if (value is Vector5D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec5 IVec5 && IVec5.GetTruncating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    static bool INumberBase<Vector5D<T>>.TryConvertToChecked<TOther>(Vector5D<T> value, [MaybeNullWhen(false)] out TOther result)
        => TOther.TryConvertFromChecked(value, out result);

    static bool INumberBase<Vector5D<T>>.TryConvertToSaturating<TOther>(Vector5D<T> value, [MaybeNullWhen(false)] out TOther result)
        => TOther.TryConvertFromSaturating(value, out result);

    static bool INumberBase<Vector5D<T>>.TryConvertToTruncating<TOther>(Vector5D<T> value, [MaybeNullWhen(false)]out TOther result)
        => TOther.TryConvertFromTruncating(value, out result);

    Vector5D<T1>? IVec5.GetChecked<T1>() => T1.TryConvertFromChecked(X, out var x) ? new(x, T1.CreateChecked(Y), T1.CreateChecked(Z), T1.CreateChecked(W), T1.CreateChecked(V)) : null;
    Vector5D<T1>? IVec5.GetSaturating<T1>() => T1.TryConvertFromSaturating(X, out var x) ? new(x, T1.CreateSaturating(Y), T1.CreateSaturating(Z), T1.CreateSaturating(W), T1.CreateSaturating(V)) : null;
    Vector5D<T1>? IVec5.GetTruncating<T1>() => T1.TryConvertFromTruncating(X, out var x) ? new(x, T1.CreateTruncating(Y), T1.CreateTruncating(Z), T1.CreateTruncating(W), T1.CreateTruncating(V)) : null;

    static ReadOnlySpan<T> IVector<Vector5D<T>, T>.AsSpan(in Vector5D<T> vec) => vec.AsSpan();
}

// Vector5D<T>.IReadOnlyList
public readonly partial struct Vector5D<T> : IReadOnlyList<T>
{
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
        yield return V;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    int IReadOnlyCollection<T>.Count => Count;
}

// Vector5D<T>.IUtf8SpanParsableFormattable
public partial struct Vector5D<T> :
    IUtf8SpanFormattable,
    IUtf8SpanParsable<Vector5D<T>>
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
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(V, formatString) &&
            handler.AppendLiteral(">");

        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    }

    public static Vector5D<T> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector5D)}<{typeof(T)}>");

    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector5D<T> result)
    {
        result = default;

        if (s[0] != (byte)'<') return false;
        if (s[^1] != (byte)'>') return false;

        var separator = NumberGroupSeparatorTChar<byte>(NumberFormatInfo.GetInstance(provider));

        s = s[1..^1];

        T? x, y, z, w, v;

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

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out w)) return false;

            s = s[(nextNumber + separator.Length)..];
        }

        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, provider, out v)) return false;
        }

        result = new Vector5D<T>(x, y, z, w, v);
        return true;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(NumberGroupSeparatorTChar))]
        static extern ReadOnlySpan<TChar> NumberGroupSeparatorTChar<TChar>(NumberFormatInfo? c) where TChar : unmanaged;
    }

    static void IVector<Vector5D<T>, T>.CopyTo(in Vector5D<T> vector, T[] array) => vector.CopyTo(array);
    static void IVector<Vector5D<T>, T>.CopyTo(in Vector5D<T> vector, T[] array, int index) => vector.CopyTo(array, index);
    static void IVector<Vector5D<T>, T>.CopyTo(in Vector5D<T> vector, Span<T> destination) => vector.CopyTo(destination);
    static bool IVector<Vector5D<T>, T>.TryCopyTo(in Vector5D<T> vector, Span<T> destination) => vector.TryCopyTo(destination);
}

// Vector5D
public static partial class Vector5D
{
    #region CopyTo
    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have at least five elements. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector5D<T> self, T[] array) where T : INumberBase<T>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector5D<T>.Count, nameof(array));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[0]), self);
    }

    /// <summary>Copies the elements of the vector to a specified array starting at a specified index position.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index at which to copy the first element of the vector.</param>
    /// <remarks><paramref name="array" /> must have a sufficient number of elements to accommodate the five vector elements. In other words, elements <paramref name="index" /> through <paramref name="index" /> + 5 must already exist in <paramref name="array" />.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the array length.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector5D<T> self, T[] array, int index) where T : INumberBase<T>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector5D<T>.Count);

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref array[index]), self);
    }

    /// <summary>Copies the vector to the given <see cref="Span{T}" />. The length of the destination span must be at least 5.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in this Vector5D<T> self, Span<T> destination) where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector5D<T>.Count, nameof(destination));

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 5.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<T>(in this Vector5D<T> self, Span<T> destination) where T : INumberBase<T>
    {
        if (destination.Length < Vector5D<T>.Count)
        {
            return false;
        }

        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion

    #region Extension

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(in this Vector5D<T> vec) where T : INumberBase<T>
    {
        return MemoryMarshal.CreateReadOnlySpan<T>(ref Unsafe.As<Vector5D<T>, T>(ref Unsafe.AsRef(in vec)), Vector5D<T>.Count);
    }

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Length<T, TReturn>(this Vector5D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var lengthSquared = vec.LengthSquared<T, TReturn>();
        return TReturn.Sqrt(lengthSquared);
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T,TReturn}" /> method.</remarks>
    /// <altmember cref="Length{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn LengthSquared<T, TReturn>(this Vector5D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
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
    public static Vector5D<T> Multiply<T>(Vector5D<T> left, Vector5D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Multiply<T>(Vector5D<T> left, T right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Multiply<T>(T left, Vector5D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Negate<T>(Vector5D<T> value) where T : INumberBase<T>
    {
        return -value;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Subtract<T>(Vector5D<T> left, Vector5D<T> right) where T : INumberBase<T>
    {
        return left - right;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Add<T>(Vector5D<T> left, Vector5D<T> right) where T : INumberBase<T>
    {
        return left + right;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Divide<T>(Vector5D<T> left, Vector5D<T> right) where T : INumberBase<T>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Divide<T>(Vector5D<T> left, T divisor) where T : INumberBase<T>
    {
        return left / divisor;
    }

    #endregion

    #region Other

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Abs<T>(Vector5D<T> value) where T : INumberBase<T>
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), value);
                
                v0 = Vector64.Abs(v0);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), value);
                
                v0 = Vector128.Abs(v0);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), value);
                
                v0 = Vector256.Abs(v0);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), value);
                
                v0 = Vector512.Abs(v0);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
            }
        }


        return new(T.Abs(value.X), T.Abs(value.Y), T.Abs(value.Z), T.Abs(value.W), T.Abs(value.V));
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Clamp<T>(Vector5D<T> value1, Vector5D<T> min, Vector5D<T> max) where T : INumberBase<T>
    {
        // NOTE: COMPLETELY UNTESTED. MIGHT BE SLOW.
        unsafe
        {
            if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated)
            {
                Vector64<T> v0 = default;
                Vector64<T> v1 = default;
                Vector64<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v2), max);
                
                v0 = Vector64.Min(Vector64.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector64<T>, byte>(ref v0));
            }
        
            if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated)
            {
                Vector128<T> v0 = default;
                Vector128<T> v1 = default;
                Vector128<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v2), max);
                
                v0 = Vector128.Min(Vector128.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector128<T>, byte>(ref v0));
            }
        
            if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated)
            {
                Vector256<T> v0 = default;
                Vector256<T> v1 = default;
                Vector256<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v2), max);
                
                v0 = Vector256.Min(Vector256.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector256<T>, byte>(ref v0));
            }
        
            if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated)
            {
                Vector512<T> v0 = default;
                Vector512<T> v1 = default;
                Vector512<T> v2 = default;
                
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0), value1);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v1), min);
                Unsafe.WriteUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v2), max);
                
                v0 = Vector512.Min(Vector512.Max(v0, v1), v2);
                return Unsafe.ReadUnaligned<Vector5D<T>>(ref Unsafe.As<Vector512<T>, byte>(ref v0));
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
    public static TReturn Distance<T, TReturn>(Vector5D<T> value1, Vector5D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var distanceSquared = DistanceSquared<T, TReturn>(value1, value2);
        return TReturn.Sqrt(distanceSquared);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(Vector5D<T> value1, Vector5D<T> value2) where T : INumberBase<T>
    {
        var difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn DistanceSquared<T, TReturn>(Vector5D<T> value1, Vector5D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var difference = value1 - value2;
        return Dot<T, TReturn>(difference, difference);
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector5D<T> vector1, Vector5D<T> vector2) where T : INumberBase<T>
    {
        // TODO: vectorize return scalar
        return
            vector1.X * vector2.X +
            vector1.Y * vector2.Y +
            vector1.Z * vector2.Z +
            vector1.W * vector2.W +
            vector1.V * vector2.V;
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Dot<T, TReturn>(Vector5D<T> vector1, Vector5D<T> vector2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        // TODO vectorize return converted (maybe not possible)
        return
            TReturn.CreateTruncating(vector1.X) * TReturn.CreateTruncating(vector2.X) +
            TReturn.CreateTruncating(vector1.Y) * TReturn.CreateTruncating(vector2.Y) +
            TReturn.CreateTruncating(vector1.Z) * TReturn.CreateTruncating(vector2.Z) +
            TReturn.CreateTruncating(vector1.W) * TReturn.CreateTruncating(vector2.W) +
            TReturn.CreateTruncating(vector1.V) * TReturn.CreateTruncating(vector2.V);
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TFloat> Lerp<T, TFloat>(Vector5D<T> value1, Vector5D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (TFloat.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector5D<T> LerpUnchecked<T>(Vector5D<T> value1, Vector5D<T> value2, T amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (T.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TFloat> LerpClamped<T, TFloat>(Vector5D<T> value1, Vector5D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = TFloat.Clamp(amount, TFloat.Zero, TFloat.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector5D<T> LerpClampedUnchecked<T>(Vector5D<T> value1, Vector5D<T> value2, T amount) where T : INumberBase<T>
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
    public static Vector5D<TFloat> Lerp<T, TFloat>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (Vector5D<TFloat>.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector5D<T> LerpUnchecked<T>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (Vector5D<T>.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TFloat> LerpClamped<T, TFloat>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = Clamp(amount, Vector5D<TFloat>.Zero, Vector5D<TFloat>.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector5D<T> LerpClampedUnchecked<T>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) where T : INumberBase<T>
    {
        amount = Clamp(amount, Vector5D<T>.Zero, Vector5D<T>.One);
        return LerpUnchecked(value1, value2, amount);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Max<T>(Vector5D<T> value1, Vector5D<T> value2) where T : INumberBase<T>
    {
        return new Vector5D<T>(
            T.MaxMagnitudeNumber(value1.X, value2.X), 
            T.MaxMagnitudeNumber(value1.Y, value2.Y), 
            T.MaxMagnitudeNumber(value1.Z, value2.Z), 
            T.MaxMagnitudeNumber(value1.W, value2.W), 
            T.MaxMagnitudeNumber(value1.V, value2.V)
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Min<T>(Vector5D<T> value1, Vector5D<T> value2) where T : INumberBase<T>
    {
        return new Vector5D<T>(
        T.MinMagnitudeNumber(value1.X, value2.X), 
        T.MinMagnitudeNumber(value1.Y, value2.Y), 
        T.MinMagnitudeNumber(value1.Z, value2.Z), 
        T.MinMagnitudeNumber(value1.W, value2.W), 
        T.MinMagnitudeNumber(value1.V, value2.V)
        );
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TReturn> Normalize<T, TReturn>(Vector5D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return value.As<TReturn>() / value.Length<T, TReturn>();
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Normalize<T>(Vector5D<T> value) where T : INumberBase<T>, IRootFunctions<T>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TReturn> Reflect<T, TReturn>(Vector5D<T> vector, Vector5D<T> normal) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var dot = Dot<T, TReturn>(vector, normal);
        return vector.As<TReturn>() - (Scalar<TReturn>.Two * (dot * normal.As<TReturn>()));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TReturn> Sqrt<T, TReturn>(Vector5D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return new Vector5D<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.W)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.V))
        );
    }

    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Transform<T>(Vector5D<T> position, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        return (Vector5D<T>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<TReturn> Transform<T, TQuat, TReturn>(Vector5D<T> value, Quaternion<TQuat> rotation) where T : INumberBase<T> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;
        var  = rotation.Z + rotation.Z;
        var  = rotation.W + rotation.W;
        var  = rotation.V + rotation.V;var x2 = rotation.X + rotation.X;
        var y2 = rotation.Y + rotation.Y;
        var z2 = rotation.Z + rotation.Z;


        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;
        var  = rotation.Z + rotation.Z;
        var  = rotation.W + rotation.W;
        var  = rotation.V + rotation.V;var wx2 = TReturn.CreateTruncating(rotation.W * x2);
        var wy2 = TReturn.CreateTruncating(rotation.W * y2);
        var wz2 = TReturn.CreateTruncating(rotation.W * z2);
        var xx2 = TReturn.CreateTruncating(rotation.X * x2);
        var xy2 = TReturn.CreateTruncating(rotation.X * y2);
        var xz2 = TReturn.CreateTruncating(rotation.X * z2);
        var yy2 = TReturn.CreateTruncating(rotation.Y * y2);
        var yz2 = TReturn.CreateTruncating(rotation.Y * z2);
        var zz2 = TReturn.CreateTruncating(rotation.Z * z2);

        return new Vector5D<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.W)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.V))
        );

        return new Vector5D<TReturn>(
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
    // internal static Vector5D<T> TransformNormal<T>(Vector5D<T> normal, in Matrix4x4 matrix) where T : INumberBase<T>
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
    public static Vector5D<T> Remainder<T>(this Vector5D<T> left, Vector5D<T> right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector5D<T>(
            left.X % right.X,
            left.Y % right.Y,
            left.Z % right.Z,
            left.W % right.W,
            left.V % right.V
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Remainder<T>(this Vector5D<T> left, T right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector5D<T>(
            left.X % right,
            left.Y % right,
            left.Z % right,
            left.W % right,
            left.V % right
        );
    }
    #endregion

    #region Specializations

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Vector5D<T> vec) where T : INumberBase<T>, IRootFunctions<T>
    {
        return vec.Length<T, T>();
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T}" /> method.</remarks>
    /// <altmember cref="Length{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Vector5D<T> vec) where T : INumberBase<T>
    {
        return vec.LengthSquared<T, T>();
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Lerp<T>(Vector5D<T> value1, Vector5D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return Lerp<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> LerpClamped<T>(Vector5D<T> value1, Vector5D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return LerpClamped<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Lerp<T>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return Lerp<T, T>(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> LerpClamped<T>(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return LerpClamped<T, T>(value1, value2, amount);
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Reflect<T>(Vector5D<T> vector, Vector5D<T> normal) where T : IFloatingPoint<T>
    {
        return Reflect<T, T>(vector, normal);
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Sqrt<T>(Vector5D<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>
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
    public static Vector5D<T> Transform<T>(Vector5D<T> value, Quaternion<T> rotation)
        where T : IFloatingPoint<T>, ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        return Transform<T, T, T>(value, rotation);
    }


    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector5D<T> Transform<T, TQuat>(Vector5D<T> value, Quaternion<TQuat> rotation)
        where T : IFloatingPoint<T>
        where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        return Transform<T, TQuat, T>(value, rotation);
    }
    */
    #endregion

    // Equivalent implementing IHyperbolicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Acosh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Acosh(x.X), T.Acosh(x.Y), T.Acosh(x.Z), T.Acosh(x.W), T.Acosh(x.V));
    public static Vector5D<T> Asinh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Asinh(x.X), T.Asinh(x.Y), T.Asinh(x.Z), T.Asinh(x.W), T.Asinh(x.V));
    public static Vector5D<T> Atanh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Atanh(x.X), T.Atanh(x.Y), T.Atanh(x.Z), T.Atanh(x.W), T.Atanh(x.V));
    public static Vector5D<T> Cosh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Cosh(x.X), T.Cosh(x.Y), T.Cosh(x.Z), T.Cosh(x.W), T.Cosh(x.V));
    public static Vector5D<T> Sinh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Sinh(x.X), T.Sinh(x.Y), T.Sinh(x.Z), T.Sinh(x.W), T.Sinh(x.V));
    public static Vector5D<T> Tanh<T>(Vector5D<T> x) where T : IHyperbolicFunctions<T> => new(T.Tanh(x.X), T.Tanh(x.Y), T.Tanh(x.Z), T.Tanh(x.W), T.Tanh(x.V));

    // Equivalent implementing ITrigonometricFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Acos<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Acos(x.X), T.Acos(x.Y), T.Acos(x.Z), T.Acos(x.W), T.Acos(x.V));
    public static Vector5D<T> AcosPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.AcosPi(x.X), T.AcosPi(x.Y), T.AcosPi(x.Z), T.AcosPi(x.W), T.AcosPi(x.V));
    public static Vector5D<T> Asin<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Asin(x.X), T.Asin(x.Y), T.Asin(x.Z), T.Asin(x.W), T.Asin(x.V));
    public static Vector5D<T> AsinPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.AsinPi(x.X), T.AsinPi(x.Y), T.AsinPi(x.Z), T.AsinPi(x.W), T.AsinPi(x.V));
    public static Vector5D<T> Atan<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Atan(x.X), T.Atan(x.Y), T.Atan(x.Z), T.Atan(x.W), T.Atan(x.V));
    public static Vector5D<T> AtanPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.AtanPi(x.X), T.AtanPi(x.Y), T.AtanPi(x.Z), T.AtanPi(x.W), T.AtanPi(x.V));
    public static Vector5D<T> Cos<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Cos(x.X), T.Cos(x.Y), T.Cos(x.Z), T.Cos(x.W), T.Cos(x.V));
    public static Vector5D<T> CosPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.CosPi(x.X), T.CosPi(x.Y), T.CosPi(x.Z), T.CosPi(x.W), T.CosPi(x.V));
    public static Vector5D<T> DegreesToRadians<T>(Vector5D<T> degrees) where T : ITrigonometricFunctions<T> => new(T.DegreesToRadians(degrees.X), T.DegreesToRadians(degrees.Y), T.DegreesToRadians(degrees.Z), T.DegreesToRadians(degrees.W), T.DegreesToRadians(degrees.V));
    public static Vector5D<T> RadiansToDegrees<T>(Vector5D<T> radians) where T : ITrigonometricFunctions<T> => new(T.RadiansToDegrees(radians.X), T.RadiansToDegrees(radians.Y), T.RadiansToDegrees(radians.Z), T.RadiansToDegrees(radians.W), T.RadiansToDegrees(radians.V));
    public static Vector5D<T> Sin<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Sin(x.X), T.Sin(x.Y), T.Sin(x.Z), T.Sin(x.W), T.Sin(x.V));
    public static Vector5D<T> SinPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.SinPi(x.X), T.SinPi(x.Y), T.SinPi(x.Z), T.SinPi(x.W), T.SinPi(x.V));
    public static Vector5D<T> Tan<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.Tan(x.X), T.Tan(x.Y), T.Tan(x.Z), T.Tan(x.W), T.Tan(x.V));
    public static Vector5D<T> TanPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T> => new(T.TanPi(x.X), T.TanPi(x.Y), T.TanPi(x.Z), T.TanPi(x.W), T.TanPi(x.V));


    public static (Vector5D<T> Sin, Vector5D<T> Cos) SinCos<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCos(x.X);
        var (sinY, cosY) = T.SinCos(x.Y);
        var (sinZ, cosZ) = T.SinCos(x.Z);
        var (sinW, cosW) = T.SinCos(x.W);
        var (sinV, cosV) = T.SinCos(x.V);

        return (
            new Vector5D<T>(sinX, sinY, sinZ, sinW, sinV),
            new Vector5D<T>(cosX, cosY, cosZ, cosW, cosV)
        );
    }

    public static (Vector5D<T> SinPi, Vector5D<T> CosPi) SinCosPi<T>(Vector5D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCosPi(x.X);
        var (sinY, cosY) = T.SinCosPi(x.Y);
        var (sinZ, cosZ) = T.SinCosPi(x.Z);
        var (sinW, cosW) = T.SinCosPi(x.W);
        var (sinV, cosV) = T.SinCosPi(x.V);

        return (
            new Vector5D<T>(sinX, sinY, sinZ, sinW, sinV),
            new Vector5D<T>(cosX, cosY, cosZ, cosW, cosV)
        );
    }

    // Equivalent implementing ILogarithmicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Log<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log(x.X), T.Log(x.Y), T.Log(x.Z), T.Log(x.W), T.Log(x.V));

    public static Vector5D<T> Log<T>(Vector5D<T> x, Vector5D<T> newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase.X), T.Log(x.Y, newBase.Y), T.Log(x.Z, newBase.Z), T.Log(x.W, newBase.W), T.Log(x.V, newBase.V));
    public static Vector5D<T> Log<T>(Vector5D<T> x, T newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase), T.Log(x.Y, newBase), T.Log(x.Z, newBase), T.Log(x.W, newBase), T.Log(x.V, newBase));
    public static Vector5D<T> LogP1<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => Log(x + Vector5D<T>.One);
    public static Vector5D<T> Log2<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log2(x.X), T.Log2(x.Y), T.Log2(x.Z), T.Log2(x.W), T.Log2(x.V));
    public static Vector5D<T> Log2P1<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => Log2(x + Vector5D<T>.One);
    public static Vector5D<T> Log10<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log10(x.X), T.Log10(x.Y), T.Log10(x.Z), T.Log10(x.W), T.Log10(x.V));
    public static Vector5D<T> Log10P1<T>(Vector5D<T> x) where T : ILogarithmicFunctions<T> => Log10(x + Vector5D<T>.One);

    // Equivalent implementing IExponentialFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Exp<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => new(T.Exp(x.X), T.Exp(x.Y), T.Exp(x.Z), T.Exp(x.W), T.Exp(x.V));
    public static Vector5D<T> ExpM1<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => Exp(x) - Vector5D<T>.One;
    public static Vector5D<T> Exp2<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => new(T.Exp2(x.X), T.Exp2(x.Y), T.Exp2(x.Z), T.Exp2(x.W), T.Exp2(x.V));
    public static Vector5D<T> Exp2M1<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => Exp2(x) - Vector5D<T>.One;
    public static Vector5D<T> Exp10<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => new(T.Exp10(x.X), T.Exp10(x.Y), T.Exp10(x.Z), T.Exp10(x.W), T.Exp10(x.V));
    public static Vector5D<T> Exp10M1<T>(Vector5D<T> x) where T : IExponentialFunctions<T> => Exp10(x) - Vector5D<T>.One;

    // Equivalent implementing IPowerFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Pow<T>(Vector5D<T> x, Vector5D<T> y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y.X), T.Pow(x.Y, y.Y), T.Pow(x.Z, y.Z), T.Pow(x.W, y.W), T.Pow(x.V, y.V));
    public static Vector5D<T> Pow<T>(Vector5D<T> x, T y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y), T.Pow(x.Y, y), T.Pow(x.Z, y), T.Pow(x.W, y), T.Pow(x.V, y));

    // Equivalent implementing IRootFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector5D<T> Cbrt<T>(Vector5D<T> x) where T : IRootFunctions<T> => new(T.Cbrt(x.X), T.Cbrt(x.Y), T.Cbrt(x.Z), T.Cbrt(x.W), T.Cbrt(x.V));
    public static Vector5D<T> Hypot<T>(Vector5D<T> x, Vector5D<T> y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y.X), T.Hypot(x.Y, y.Y), T.Hypot(x.Z, y.Z), T.Hypot(x.W, y.W), T.Hypot(x.V, y.V));
    public static Vector5D<T> Hypot<T>(Vector5D<T> x, T y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y), T.Hypot(x.Y, y), T.Hypot(x.Z, y), T.Hypot(x.W, y), T.Hypot(x.V, y));
    public static Vector5D<T> RootN<T>(Vector5D<T> x, int n) where T : IRootFunctions<T> => new(T.RootN(x.X, n), T.RootN(x.Y, n), T.RootN(x.Z, n), T.RootN(x.W, n), T.RootN(x.V, n));

    // IFloatingPoint<TSelf>
    public static Vector5D<T> Round<T>(Vector5D<T> x) where T : IFloatingPoint<T> => new(T.Round(x.X), T.Round(x.Y), T.Round(x.Z), T.Round(x.W), T.Round(x.V));
    public static Vector5D<T> Round<T>(Vector5D<T> x, int digits) where T : IFloatingPoint<T> => new(T.Round(x.X, digits), T.Round(x.Y, digits), T.Round(x.Z, digits), T.Round(x.W, digits), T.Round(x.V, digits));
    public static Vector5D<T> Round<T>(Vector5D<T> x, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, mode), T.Round(x.Y, mode), T.Round(x.Z, mode), T.Round(x.W, mode), T.Round(x.V, mode));
    public static Vector5D<T> Round<T>(Vector5D<T> x, int digits, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, digits, mode), T.Round(x.Y, digits, mode), T.Round(x.Z, digits, mode), T.Round(x.W, digits, mode), T.Round(x.V, digits, mode));
    public static Vector5D<T> Truncate<T>(Vector5D<T> x) where T : IFloatingPoint<T> => new(T.Truncate(x.X), T.Truncate(x.Y), T.Truncate(x.Z), T.Truncate(x.W), T.Truncate(x.V));

    // IFloatingPointIeee754<TSelf>
    public static Vector5D<T> Atan2<T>(Vector5D<T> x, Vector5D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y.X), T.Atan2(x.Y, y.Y), T.Atan2(x.Z, y.Z), T.Atan2(x.W, y.W), T.Atan2(x.V, y.V));
    public static Vector5D<T> Atan2Pi<T>(Vector5D<T> x, Vector5D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y.X), T.Atan2Pi(x.Y, y.Y), T.Atan2Pi(x.Z, y.Z), T.Atan2Pi(x.W, y.W), T.Atan2Pi(x.V, y.V));
    public static Vector5D<T> Atan2<T>(Vector5D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y), T.Atan2(x.Y, y), T.Atan2(x.Z, y), T.Atan2(x.W, y), T.Atan2(x.V, y));
    public static Vector5D<T> Atan2Pi<T>(Vector5D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y), T.Atan2Pi(x.Y, y), T.Atan2Pi(x.Z, y), T.Atan2Pi(x.W, y), T.Atan2Pi(x.V, y));
    public static Vector5D<T> BitDecrement<T>(Vector5D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitDecrement(x.X), T.BitDecrement(x.Y), T.BitDecrement(x.Z), T.BitDecrement(x.W), T.BitDecrement(x.V));
    public static Vector5D<T> BitIncrement<T>(Vector5D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitIncrement(x.X), T.BitIncrement(x.Y), T.BitIncrement(x.Z), T.BitIncrement(x.W), T.BitIncrement(x.V));

    public static Vector5D<T> FusedMultiplyAdd<T>(Vector5D<T> left, Vector5D<T> right, Vector5D<T> addend) where T : IFloatingPointIeee754<T> => new(T.FusedMultiplyAdd(left.X, right.X, addend.X), T.FusedMultiplyAdd(left.Y, right.Y, addend.Y), T.FusedMultiplyAdd(left.Z, right.Z, addend.Z), T.FusedMultiplyAdd(left.W, right.W, addend.W), T.FusedMultiplyAdd(left.V, right.V, addend.V));
    public static Vector5D<T> ReciprocalEstimate<T>(Vector5D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalEstimate(x.X), T.ReciprocalEstimate(x.Y), T.ReciprocalEstimate(x.Z), T.ReciprocalEstimate(x.W), T.ReciprocalEstimate(x.V));
    public static Vector5D<T> ReciprocalSqrtEstimate<T>(Vector5D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalSqrtEstimate(x.X), T.ReciprocalSqrtEstimate(x.Y), T.ReciprocalSqrtEstimate(x.Z), T.ReciprocalSqrtEstimate(x.W), T.ReciprocalSqrtEstimate(x.V));

    // INumber<T>
    // public static Vector5D<T> Clamp<T>(Vector5D<T> value, Vector5D<T> min, Vector5D<T> max) where T : INumber<T> => new(T.Clamp(x.X), T.Clamp(x.Y), T.Clamp(x.Z), T.Clamp(x.W), T.Clamp(x.V));
    public static Vector5D<T> CopySign<T>(Vector5D<T> value, Vector5D<T> sign) where T : INumber<T> => new(T.CopySign(value.X, sign.X), T.CopySign(value.Y, sign.Y), T.CopySign(value.Z, sign.Z), T.CopySign(value.W, sign.W), T.CopySign(value.V, sign.V));
    public static Vector5D<T> CopySign<T>(Vector5D<T> value, T sign) where T : INumber<T> => new(T.CopySign(value.X, sign), T.CopySign(value.Y, sign), T.CopySign(value.Z, sign), T.CopySign(value.W, sign), T.CopySign(value.V, sign));
    public static Vector5D<T> MaxNumber<T>(Vector5D<T> x, Vector5D<T> y) where T : INumber<T> => new(T.MaxNumber(x.X, y.X), T.MaxNumber(x.Y, y.Y), T.MaxNumber(x.Z, y.Z), T.MaxNumber(x.W, y.W), T.MaxNumber(x.V, y.V));
    public static Vector5D<T> MinNumber<T>(Vector5D<T> x, Vector5D<T> y) where T : INumber<T> => new(T.MinNumber(x.X, y.X), T.MinNumber(x.Y, y.Y), T.MinNumber(x.Z, y.Z), T.MinNumber(x.W, y.W), T.MinNumber(x.V, y.V));

    // INumberBase<T>
    // public static Vector5D<T> MaxMagnitude<T>(Vector5D<T> x, Vector5D<T> y) where T : INumberBase<T> => new(T.MaxMagnitude(x.X, y.X), T.MaxMagnitude(x.Y, y.Y), T.MaxMagnitude(x.Z, y.Z), T.MaxMagnitude(x.W, y.W), T.MaxMagnitude(x.V, y.V));
    // public static Vector5D<T> MaxMagnitudeNumber<T>(Vector5D<T> x, Vector5D<T> y) where T : INumberBase<T> => new(T.MaxMagnitudeNumber(x.X, y.X), T.MaxMagnitudeNumber(x.Y, y.Y), T.MaxMagnitudeNumber(x.Z, y.Z), T.MaxMagnitudeNumber(x.W, y.W), T.MaxMagnitudeNumber(x.V, y.V));
    // public static Vector5D<T> MinMagnitude<T>(Vector5D<T> x, Vector5D<T> y) where T : INumberBase<T> => new(T.MinMagnitude(x.X, y.X), T.MinMagnitude(x.Y, y.Y), T.MinMagnitude(x.Z, y.Z), T.MinMagnitude(x.W, y.W), T.MinMagnitude(x.V, y.V));
    // public static Vector5D<T> MinMagnitudeNumber<T>(Vector5D<T> x, Vector5D<T> y) where T : INumberBase<T> => new(T.MinMagnitudeNumber(x.X, y.X), T.MinMagnitudeNumber(x.Y, y.Y), T.MinMagnitudeNumber(x.Z, y.Z), T.MinMagnitudeNumber(x.W, y.W), T.MinMagnitudeNumber(x.V, y.V));
    // (there's no reason you would want these.)



    // IFloatingPointIeee754<TSelf>
    public static Vector5D<int> ILogB<T>(Vector5D<T> x) where T : IFloatingPointIeee754<T> => new(T.ILogB(x.X), T.ILogB(x.Y), T.ILogB(x.Z), T.ILogB(x.W), T.ILogB(x.V));
    public static Vector5D<T> ScaleB<T>(Vector5D<T> x, Vector5D<int> n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n.X), T.ScaleB(x.Y, n.Y), T.ScaleB(x.Z, n.Z), T.ScaleB(x.W, n.W), T.ScaleB(x.V, n.V));
    public static Vector5D<T> ScaleB<T>(Vector5D<T> x, int n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n), T.ScaleB(x.Y, n), T.ScaleB(x.Z, n), T.ScaleB(x.W, n), T.ScaleB(x.V, n));

    public static Vector5D<int> RoundToInt<T>(Vector5D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector5D<int>(
            int.CreateSaturating(T.Round(vector.X)),
            int.CreateSaturating(T.Round(vector.Y)),
            int.CreateSaturating(T.Round(vector.Z)),
            int.CreateSaturating(T.Round(vector.W)),
            int.CreateSaturating(T.Round(vector.V))
        );
    }

    public static Vector5D<int> FloorToInt<T>(Vector5D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector5D<int>(
            int.CreateSaturating(T.Floor(vector.X)),
            int.CreateSaturating(T.Floor(vector.Y)),
            int.CreateSaturating(T.Floor(vector.Z)),
            int.CreateSaturating(T.Floor(vector.W)),
            int.CreateSaturating(T.Floor(vector.V))
        );
    }

    public static Vector5D<int> CeilingToInt<T>(Vector5D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector5D<int>(
            int.CreateSaturating(T.Ceiling(vector.X)),
            int.CreateSaturating(T.Ceiling(vector.Y)),
            int.CreateSaturating(T.Ceiling(vector.Z)),
            int.CreateSaturating(T.Ceiling(vector.W)),
            int.CreateSaturating(T.Ceiling(vector.V))
        );
    }

    public static Vector5D<TInt> RoundToInt<T, TInt>(Vector5D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector5D<TInt>(
            TInt.CreateSaturating(T.Round(vector.X)),
            TInt.CreateSaturating(T.Round(vector.Y)),
            TInt.CreateSaturating(T.Round(vector.Z)),
            TInt.CreateSaturating(T.Round(vector.W)),
            TInt.CreateSaturating(T.Round(vector.V))
        );
    }

    public static Vector5D<TInt> FloorToInt<T, TInt>(Vector5D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector5D<TInt>(
            TInt.CreateSaturating(T.Floor(vector.X)),
            TInt.CreateSaturating(T.Floor(vector.Y)),
            TInt.CreateSaturating(T.Floor(vector.Z)),
            TInt.CreateSaturating(T.Floor(vector.W)),
            TInt.CreateSaturating(T.Floor(vector.V))
        );
    }

    public static Vector5D<TInt> CeilingToInt<T, TInt>(Vector5D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector5D<TInt>(
            TInt.CreateSaturating(T.Ceiling(vector.X)),
            TInt.CreateSaturating(T.Ceiling(vector.Y)),
            TInt.CreateSaturating(T.Ceiling(vector.Z)),
            TInt.CreateSaturating(T.Ceiling(vector.W)),
            TInt.CreateSaturating(T.Ceiling(vector.V))
        );
    }

}

// IVector<Vector5D<T>, T>
public readonly partial struct Vector5D<T>
{
    T IVector<Vector5D<T>, T>.LengthSquared()
        => this.LengthSquared();
    static Vector5D<T> IVector<Vector5D<T>, T>.Multiply(Vector5D<T> left, Vector5D<T> right)
        => Vector5D.Multiply(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Multiply(Vector5D<T> left, T right)
        => Vector5D.Multiply(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Multiply(T left, Vector5D<T> right)
        => Vector5D.Multiply(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Negate(Vector5D<T> value)
        => Vector5D.Negate(value);
    static Vector5D<T> IVector<Vector5D<T>, T>.Subtract(Vector5D<T> left, Vector5D<T> right)
        => Vector5D.Subtract(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Add(Vector5D<T> left, Vector5D<T> right)
        => Vector5D.Add(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Divide(Vector5D<T> left, Vector5D<T> right)
        => Vector5D.Divide(left, right);
    static Vector5D<T> IVector<Vector5D<T>, T>.Divide(Vector5D<T> left, T divisor)
        => Vector5D.Divide(left, divisor);
    static Vector5D<T> IVector<Vector5D<T>, T>.Clamp(Vector5D<T> value1, Vector5D<T> min, Vector5D<T> max)
        => Vector5D.Clamp(value1, min, max);
    static TReturn IVector<Vector5D<T>, T>.Distance<TReturn>(Vector5D<T> value1, Vector5D<T> value2)
        => Vector5D.Distance<T, TReturn>(value1, value2);
    static T IVector<Vector5D<T>, T>.DistanceSquared(Vector5D<T> value1, Vector5D<T> value2)
        => Vector5D.DistanceSquared(value1, value2);
    static TReturn IVector<Vector5D<T>, T>.DistanceSquared<TReturn>(Vector5D<T> value1, Vector5D<T> value2)
        => Vector5D.DistanceSquared<T, TReturn>(value1, value2);
    static T IVector<Vector5D<T>, T>.Dot(Vector5D<T> vector1, Vector5D<T> vector2)
        => Vector5D.Dot(vector1, vector2);
    static TReturn IVector<Vector5D<T>, T>.Dot<TReturn>(Vector5D<T> vector1, Vector5D<T> vector2)
        => Vector5D.Dot<T, TReturn>(vector1, vector2);
    static Vector5D<T> IVector<Vector5D<T>, T>.Max(Vector5D<T> value1, Vector5D<T> value2)
        => Vector5D.Max(value1, value2);
    static Vector5D<T> IVector<Vector5D<T>, T>.Min(Vector5D<T> value1, Vector5D<T> value2)
        => Vector5D.Min(value1, value2);

    static Vector5D<T> IVector<Vector5D<T>, T>.Lerp(Vector5D<T> value1, Vector5D<T> value2, T amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector5D<T>, T>(typeof(IFloatingPoint<>));
        return Vector5D.LerpUnchecked(value1, value2, amount);
    }

    static Vector5D<T> IVector<Vector5D<T>, T>.LerpClamped(Vector5D<T> value1, Vector5D<T> value2, T amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector5D<T>, T>(typeof(IFloatingPoint<>));
        return Vector5D.LerpClampedUnchecked(value1, value2, amount);
    }

    static Vector5D<T> IVector<Vector5D<T>, T>.Lerp(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector5D<T>, T>(typeof(IFloatingPoint<>));
        return Vector5D.LerpUnchecked(value1, value2, amount);
    }

    static Vector5D<T> IVector<Vector5D<T>, T>.LerpClamped(Vector5D<T> value1, Vector5D<T> value2, Vector5D<T> amount) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector5D<T>, T>(typeof(IFloatingPoint<>));
        return Vector5D.LerpClampedUnchecked(value1, value2, amount);
    }

    static Vector5D<T> IVector<Vector5D<T>, T>.Reflect(Vector5D<T> vector, Vector5D<T> normal) /* where T : IFloatingPoint<T> */
    {
        Helpers.CheckTypeAndThrow<Vector5D<T>, T>(typeof(IFloatingPoint<>));
        return Vector5D.Reflect<T, T>(vector, normal);
    }
}