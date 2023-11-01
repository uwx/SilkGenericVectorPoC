using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector;

public readonly struct Plane<T> where T : INumberBase<T>
{
    /// <summary>The normal vector of the plane.</summary>
    public readonly Vector3D<T> Normal;

    /// <summary>The distance of the plane along its normal from the origin.</summary>
    public readonly T D;

    /// <summary>Creates a <see cref="Plane{T}" /> object from the X, Y, and Z components of its normal, and its distance from the origin on that normal.</summary>
    /// <param name="x">The X component of the normal.</param>
    /// <param name="y">The Y component of the normal.</param>
    /// <param name="z">The Z component of the normal.</param>
    /// <param name="d">The distance of the plane along its normal from the origin.</param>
    public Plane(T x, T y, T z, T d)
    {
        Normal = new Vector3D<T>(x, y, z);
        D = d;
    }

    /// <summary>Creates a <see cref="Plane{T}" /> object from a specified normal and the distance along the normal from the origin.</summary>
    /// <param name="normal">The plane's normal vector.</param>
    /// <param name="d">The plane's distance from the origin along its normal vector.</param>
    public Plane(Vector3D<T> normal, T d)
    {
        Normal = normal;
        D = d;
    }

    /// <summary>Creates a <see cref="Plane{T}" /> object from a specified four-dimensional vector.</summary>
    /// <param name="value">A vector whose first three elements describe the normal vector, and whose <see cref="Vector4D{T}.W" /> defines the distance along that normal from the origin.</param>
    public Plane(Vector4D<T> value)
    {
        Normal = new Vector3D<T>(value.X, value.Y, value.Z);
        D = value.W;
    }

    /// <summary>Returns a value that indicates whether two planes are equal.</summary>
    /// <param name="value1">The first plane to compare.</param>
    /// <param name="value2">The second plane to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Plane{T}" /> objects are equal if their <see cref="Normal" /> and <see cref="D" /> fields are equal.
    /// The <see cref="op_Equality" /> method defines the operation of the equality operator for <see cref="Plane{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Plane<T> value1, Plane<T> value2)
    {
        return (value1.Normal == value2.Normal)
            && (value1.D == value2.D);
    }

    /// <summary>Returns a value that indicates whether two planes are not equal.</summary>
    /// <param name="value1">The first plane to compare.</param>
    /// <param name="value2">The second plane to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>The <see cref="op_Inequality" /> method defines the operation of the inequality operator for <see cref="Plane{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Plane<T> value1, Plane<T> value2)
    {
        return !(value1 == value2);
    }

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Plane{T}" /> object and their <see cref="Normal" /> and <see cref="D" /> fields are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Plane<T> other) && Equals(other);
    }

    /// <summary>Returns a value that indicates whether this instance and another plane object are equal.</summary>
    /// <param name="other">The other plane.</param>
    /// <returns><see langword="true" /> if the two planes are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Plane{T}" /> objects are equal if their <see cref="Normal" /> and <see cref="D" /> fields are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Plane<T> other)
    {
        return SpeedHelpers.FastEqualsUpTo4<Plane<T>, T>(this, other);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Normal, D);
    }

    /// <summary>Returns the string representation of this plane object.</summary>
    /// <returns>A string that represents this <see cref="Plane{T}" /> object.</returns>
    /// <remarks>The string representation of a <see cref="Plane{T}" /> object use the formatting conventions of the current culture to format the numeric values in the returned string. For example, a <see cref="Plane{T}" /> object whose string representation is formatted by using the conventions of the en-US culture might appear as <c>{Normal:&lt;1.1, 2.2, 3.3&gt; D:4.4}</c>.</remarks>
    public override string ToString() => $"{{Normal:{Normal} D:{D}}}";
}

public static class Plane
{
    
    /// <summary>Creates a <see cref="Plane{T}" /> object that contains three specified points.</summary>
    /// <param name="point1">The first point defining the plane.</param>
    /// <param name="point2">The second point defining the plane.</param>
    /// <param name="point3">The third point defining the plane.</param>
    /// <returns>The plane containing the three points.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane<T> CreateFromVertices<T>(Vector3D<T> point1, Vector3D<T> point2, Vector3D<T> point3)
        where T : INumberBase<T>, IRootFunctions<T>
    {
        if (Vector128.IsHardwareAccelerated)
        {
            Vector3D<T> a = point2 - point1;
            Vector3D<T> b = point3 - point1;

            // N = Cross(a, b)
            Vector3D<T> n = Vector3D.Cross(a, b);
            Vector3D<T> normal = Vector3D.Normalize(n);

            // D = - Dot(N, point1)
            T d = -Vector3D.Dot(normal, point1);

            return new Plane<T>(normal, d);
        }
        else
        {
            T ax = point2.X - point1.X;
            T ay = point2.Y - point1.Y;
            T az = point2.Z - point1.Z;

            T bx = point3.X - point1.X;
            T by = point3.Y - point1.Y;
            T bz = point3.Z - point1.Z;

            // N=Cross(a,b)
            T nx = ay * bz - az * by;
            T ny = az * bx - ax * bz;
            T nz = ax * by - ay * bx;

            // Normalize(N)
            T ls = nx * nx + ny * ny + nz * nz;
            T invNorm = T.One / T.Sqrt(ls);

            Vector3D<T> normal = new Vector3D<T>(
                nx * invNorm,
                ny * invNorm,
                nz * invNorm);

            return new Plane<T>(
                normal,
                -(normal.X * point1.X + normal.Y * point1.Y + normal.Z * point1.Z));
        }
    }

    /// <summary>Calculates the dot product of a plane and a 4-dimensional vector.</summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The four-dimensional vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Plane<T> plane, Vector4D<T> value)
        where T : INumberBase<T>
    {
        return (plane.Normal.X * value.X)
             + (plane.Normal.Y * value.Y)
             + (plane.Normal.Z * value.Z)
             + (plane.D * value.W);
    }

    /// <summary>Returns the dot product of a specified three-dimensional vector and the normal vector of this plane plus the distance (<see cref="D" />) value of the plane.</summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The 3-dimensional vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DotCoordinate<T>(Plane<T> plane, Vector3D<T> value)
        where T : INumberBase<T>
    {
        if (Vector128.IsHardwareAccelerated)
        {
            return Vector3D.Dot(plane.Normal, value) + plane.D;
        }
        else
        {
            return plane.Normal.X * value.X +
                   plane.Normal.Y * value.Y +
                   plane.Normal.Z * value.Z +
                   plane.D;
        }
    }

    /// <summary>Returns the dot product of a specified three-dimensional vector and the <see cref="Normal" /> vector of this plane.</summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The three-dimensional vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DotNormal<T>(Plane<T> plane, Vector3D<T> value)
        where T : INumberBase<T>
    {
        if (Vector128.IsHardwareAccelerated)
        {
            return Vector3D.Dot(plane.Normal, value);
        }
        else
        {
            return plane.Normal.X * value.X +
                   plane.Normal.Y * value.Y +
                   plane.Normal.Z * value.Z;
        }
    }

    /// <summary>Creates a new <see cref="Plane{T}" /> object whose normal vector is the source plane's normal vector normalized.</summary>
    /// <param name="value">The source plane.</param>
    /// <returns>The normalized plane.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane<T> Normalize<T>(Plane<T> value)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>, IFloatingPointIeee754<T>
    {
        if (Vector128.IsHardwareAccelerated)
        {
            T normalLengthSquared = value.Normal.LengthSquared();
            if (T.Abs(normalLengthSquared - T.One) < Scalar.NormalizeEpsilon<T>())
            {
                // It already normalized, so we don't need to farther process.
                return value;
            }
            T normalLength = T.Sqrt(normalLengthSquared);
            return new Plane<T>(
                value.Normal / normalLength,
                value.D / normalLength);
        }
        else
        {
            T f = value.Normal.X * value.Normal.X + value.Normal.Y * value.Normal.Y + value.Normal.Z * value.Normal.Z;

            if (T.Abs(f - T.One) < Scalar.NormalizeEpsilon<T>())
            {
                return value; // It already normalized, so we don't need to further process.
            }

            T fInv = T.One / T.Sqrt(f);

            return new Plane<T>(
                value.Normal.X * fInv,
                value.Normal.Y * fInv,
                value.Normal.Z * fInv,
                value.D * fInv);
        }
    }

    /// <summary>Transforms a normalized plane by a 4x4 matrix.</summary>
    /// <param name="plane">The normalized plane to transform.</param>
    /// <param name="matrix">The transformation matrix to apply to <paramref name="plane" />.</param>
    /// <returns>The transformed plane.</returns>
    /// <remarks><paramref name="plane" /> must already be normalized so that its <see cref="Normal" /> vector is of unit length before this method is called.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane<T> Transform<T>(Plane<T> plane, Matrix4X4<T> matrix)
        where T : IFloatingPointIeee754<T>
    {
        Matrix4X4.Invert(matrix, out Matrix4X4<T> m);

        T x = plane.Normal.X, y = plane.Normal.Y, z = plane.Normal.Z, w = plane.D;

        return new Plane<T>(
            x * m.M11 + y * m.M12 + z * m.M13 + w * m.M14,
            x * m.M21 + y * m.M22 + z * m.M23 + w * m.M24,
            x * m.M31 + y * m.M32 + z * m.M33 + w * m.M34,
            x * m.M41 + y * m.M42 + z * m.M43 + w * m.M44);
    }

    /// <summary>Transforms a normalized plane by a Quaternion rotation.</summary>
    /// <param name="plane">The normalized plane to transform.</param>
    /// <param name="rotation">The Quaternion rotation to apply to the plane.</param>
    /// <returns>A new plane that results from applying the Quaternion rotation.</returns>
    /// <remarks><paramref name="plane" /> must already be normalized so that its <see cref="Normal" /> vector is of unit length before this method is called.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Plane<T> Transform<T>(Plane<T> plane, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        // Compute rotation matrix.
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wx2 = rotation.W * x2;
        T wy2 = rotation.W * y2;
        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T xz2 = rotation.X * z2;
        T yy2 = rotation.Y * y2;
        T yz2 = rotation.Y * z2;
        T zz2 = rotation.Z * z2;

        T m11 = T.One - yy2 - zz2;
        T m21 = xy2 - wz2;
        T m31 = xz2 + wy2;

        T m12 = xy2 + wz2;
        T m22 = T.One - xx2 - zz2;
        T m32 = yz2 - wx2;

        T m13 = xz2 - wy2;
        T m23 = yz2 + wx2;
        T m33 = T.One - xx2 - yy2;

        T x = plane.Normal.X, y = plane.Normal.Y, z = plane.Normal.Z;

        return new Plane<T>(
            x * m11 + y * m21 + z * m31,
            x * m12 + y * m22 + z * m32,
            x * m13 + y * m23 + z * m33,
            plane.D);
    }
}