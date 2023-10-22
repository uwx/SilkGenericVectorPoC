using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace GenericVector;

[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly struct Quaternion<T> : IEquatable<Quaternion<T>>
    where T : INumberBase<T>, ITrigonometricFunctions<T>, IRootFunctions<T>
{
    /// <summary>The X value of the vector component of the quaternion.</summary>
    [DataMember]
    public readonly T X;

    /// <summary>The Y value of the vector component of the quaternion.</summary>
    [DataMember]
    public readonly T Y;

    /// <summary>The Z value of the vector component of the quaternion.</summary>
    [DataMember]
    public readonly T Z;

    /// <summary>The rotation component of the quaternion.</summary>
    [DataMember]
    public readonly T W;

    internal const int Count = 4;
    
    public ReadOnlySpan<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.CreateReadOnlySpan<T>(in X, Count);
    }

    /// <summary>Constructs a quaternion from the specified components.</summary>
    /// <param name="x">The value to assign to the X component of the quaternion.</param>
    /// <param name="y">The value to assign to the Y component of the quaternion.</param>
    /// <param name="z">The value to assign to the Z component of the quaternion.</param>
    /// <param name="w">The value to assign to the W component of the quaternion.</param>
    public Quaternion(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>Creates a quaternion from the specified vector and rotation parts.</summary>
    /// <param name="vectorPart">The vector part of the quaternion.</param>
    /// <param name="scalarPart">The rotation part of the quaternion.</param>
    public Quaternion(Vector3D<T> vectorPart, T scalarPart)
    {
        X = vectorPart.X;
        Y = vectorPart.Y;
        Z = vectorPart.Z;
        W = scalarPart;
    }

    /// <summary>Gets a quaternion that represents a zero.</summary>
    /// <value>A quaternion whose values are <c>(0, 0, 0, 0)</c>.</value>
    public static Quaternion<T> Zero => new(Vector3D<T>.Zero, T.Zero);

    /// <summary>Gets a quaternion that represents no rotation.</summary>
    /// <value>A quaternion whose values are <c>(0, 0, 0, 1)</c>.</value>
    public static Quaternion<T> Identity => new(T.Zero, T.Zero, T.Zero, T.One);

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Components[index];
    }

    /// <summary>Gets a value that indicates whether the current instance is the identity quaternion.</summary>
    /// <value><see langword="true" /> if the current instance is the identity quaternion; otherwise, <see langword="false" />.</value>
    /// <altmember cref="Identity"/>
    public bool IsIdentity
    {
        get => this == Identity;
    }

    /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the operation of the addition operator for <see cref="Quaternion" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator +(Quaternion<T> value1, Quaternion<T> value2)
    {
        return new Quaternion<T>(
            value1.X + value2.X,
            value1.Y + value2.Y,
            value1.Z + value2.Z,
            value1.W + value2.W
        );
    }

    /// <summary>Divides one quaternion by a second quaternion.</summary>
    /// <param name="value1">The dividend.</param>
    /// <param name="value2">The divisor.</param>
    /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
    /// <remarks>The <see cref="op_Division" /> method defines the division operation for <see cref="Quaternion" /> objects.</remarks>
    public static Quaternion<T> operator /(Quaternion<T> value1, Quaternion<T> value2)
    {
        Quaternion<T> ans;

        var q1x = value1.X;
        var q1y = value1.Y;
        var q1z = value1.Z;
        var q1w = value1.W;

        //-------------------------------------
        // Inverse part.
        var ls = value2.X * value2.X + value2.Y * value2.Y +
                 value2.Z * value2.Z + value2.W * value2.W;
        var invNorm = T.One / ls;

        var q2x = -value2.X * invNorm;
        var q2y = -value2.Y * invNorm;
        var q2z = -value2.Z * invNorm;
        var q2w = value2.W * invNorm;

        //-------------------------------------
        // Multiply part.

        // cross(av, bv)
        var cx = q1y * q2z - q1z * q2y;
        var cy = q1z * q2x - q1x * q2z;
        var cz = q1x * q2y - q1y * q2x;

        var dot = q1x * q2x + q1y * q2y + q1z * q2z;

        return new Quaternion<T>(
            q1x * q2w + q2x * q1w + cx,
            q1y * q2w + q2y * q1w + cy,
            q1z * q2w + q2z * q1w + cz,
            q1w * q2w - dot
        );
    }

    /// <summary>Returns a value that indicates whether two quaternions are equal.</summary>
    /// <param name="value1">The first quaternion to compare.</param>
    /// <param name="value2">The second quaternion to compare.</param>
    /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two quaternions are equal if each of their corresponding components is equal.
    /// The <see cref="op_Equality" /> method defines the operation of the equality operator for <see cref="Quaternion" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Quaternion<T> value1, Quaternion<T> value2)
    {
        return (value1.X == value2.X)
            && (value1.Y == value2.Y)
            && (value1.Z == value2.Z)
            && (value1.W == value2.W);
    }

    /// <summary>Returns a value that indicates whether two quaternions are not equal.</summary>
    /// <param name="value1">The first quaternion to compare.</param>
    /// <param name="value2">The second quaternion to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Quaternion<T> value1, Quaternion<T> value2)
    {
        return !(value1 == value2);
    }

    /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The product quaternion.</returns>
    /// <remarks>The <see cref="Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="Quaternion" /> objects.</remarks>
    public static Quaternion<T> operator *(Quaternion<T> value1, Quaternion<T> value2)
    {
        Quaternion<T> ans;

        var q1x = value1.X;
        var q1y = value1.Y;
        var q1z = value1.Z;
        var q1w = value1.W;

        var q2x = value2.X;
        var q2y = value2.Y;
        var q2z = value2.Z;
        var q2w = value2.W;

        // cross(av, bv)
        var cx = q1y * q2z - q1z * q2y;
        var cy = q1z * q2x - q1x * q2z;
        var cz = q1x * q2y - q1y * q2x;

        var dot = q1x * q2x + q1y * q2y + q1z * q2z;

        return new Quaternion<T>(
            q1x * q2w + q2x * q1w + cx,
            q1y * q2w + q2y * q1w + cy,
            q1z * q2w + q2z * q1w + cz,
            q1w * q2w - dot
        );
    }

    /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
    /// <param name="value1">The source quaternion.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The scaled quaternion.</returns>
    /// <remarks>The <see cref="Quaternion.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="Quaternion" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator *(Quaternion<T> value1, T value2)
    {
        return new Quaternion<T>(
            value1.X * value2,
            value1.Y * value2,
            value1.Z * value2,
            value1.W * value2
        );
    }

    /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the operation of the subtraction operator for <see cref="Quaternion" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator -(Quaternion<T> value1, Quaternion<T> value2)
    {
        return new Quaternion<T>(
            value1.X - value2.X,
            value1.Y - value2.Y,
            value1.Z - value2.Z,
            value1.W - value2.W
        );
    }

    /// <summary>Reverses the sign of each component of the quaternion.</summary>
    /// <param name="value">The quaternion to negate.</param>
    /// <returns>The negated quaternion.</returns>
    /// <remarks>The <see cref="op_UnaryNegation" /> method defines the operation of the unary negation operator for <see cref="Quaternion" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> operator -(Quaternion<T> value)
    {
        return Zero - value;
    }

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Quaternion<T>" /> object and the corresponding components of each matrix are equal.</remarks>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Quaternion<T> other) && Equals(other);
    }

    /// <summary>Returns a value that indicates whether this instance and another quaternion are equal.</summary>
    /// <param name="other">The other quaternion.</param>
    /// <returns><see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two quaternions are equal if each of their corresponding components is equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Quaternion<T> other)
    {
        return SpeedHelpers.FastEqualsUpTo4<Quaternion<T>, T>(this, other);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    /// <summary>Returns a string that represents this quaternion.</summary>
    /// <returns>The string representation of this quaternion.</returns>
    /// <remarks>The numeric values in the returned string are formatted by using the conventions of the current culture. For example, for the en-US culture, the returned string might appear as <c>{X:1.1 Y:2.2 Z:3.3 W:4.4}</c>.</remarks>
    public override string ToString() => $"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";
}