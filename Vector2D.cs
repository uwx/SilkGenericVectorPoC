using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector;

public static class Vector2D
{
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Abs<T>(Vector2D<T> value)
        where T : INumberBase<T>
    {
        return new Vector2D<T>(
            T.Abs(value.X),
            T.Abs(value.Y)
        );
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Add<T>(Vector2D<T> left, Vector2D<T> right)
        where T : INumberBase<T>
    {
        return left + right;
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Clamp<T>(Vector2D<T> value1, Vector2D<T> min, Vector2D<T> max)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return Min(Max(value1, min), max);
    }

    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Distance<T>(Vector2D<T> value1, Vector2D<T> value2)
        where T : IRootFunctions<T>
    {
        T distanceSquared = DistanceSquared(value1, value2);
        return T.Sqrt(distanceSquared);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(Vector2D<T> value1, Vector2D<T> value2)
        where T : INumberBase<T>
    {
        Vector2D<T> difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Divide<T>(Vector2D<T> left, Vector2D<T> right)
        where T : INumberBase<T>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Divide<T>(Vector2D<T> left, T divisor)
        where T : INumberBase<T>
    {
        return left / divisor;
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector2D<T> value1, Vector2D<T> value2)
        where T : INumberBase<T>
    {
        return (value1.X * value2.X)
             + (value1.Y * value2.Y);
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    /// <remarks><format type="text/markdown"><![CDATA[
    /// The behavior of this method changed in .NET 5.0. For more information, see [Behavior change for Vector2D<T>.Lerp and Vector4D<T>.Lerp](/dotnet/core/compatibility/3.1-5.0#behavior-change-for-vector2lerp-and-vector4lerp).
    /// ]]></format></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Lerp<T>(Vector2D<T> value1, Vector2D<T> value2, T amount)
        where T : INumberBase<T>
    {
        return (value1 * (T.One - amount)) + (value2 * amount);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Max<T>(Vector2D<T> value1, Vector2D<T> value2)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        return new Vector2D<T>(
            (value1.X > value2.X) ? value1.X : value2.X,
            (value1.Y > value2.Y) ? value1.Y : value2.Y
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Min<T>(Vector2D<T> value1, Vector2D<T> value2)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        return new Vector2D<T>(
            (value1.X < value2.X) ? value1.X : value2.X,
            (value1.Y < value2.Y) ? value1.Y : value2.Y
        );
    }

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Multiply<T>(Vector2D<T> left, Vector2D<T> right)
        where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Multiply<T>(Vector2D<T> left, T right)
        where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Multiply<T>(T left, Vector2D<T> right)
        where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Negate<T>(Vector2D<T> value)
        where T : INumberBase<T>
    {
        return -value;
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Normalize<T>(Vector2D<T> value)
        where T : IRootFunctions<T>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Reflect<T>(Vector2D<T> vector, Vector2D<T> normal)
        where T : INumberBase<T>
    {
        T dot = Dot(vector, normal);
        return vector - (NumericConstants<T>.Two * (dot * normal));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Sqrt<T>(Vector2D<T> value)
        where T : IRootFunctions<T>
    {
        return new Vector2D<T>(
            T.Sqrt(value.X),
            T.Sqrt(value.Y)
        );
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Subtract<T>(Vector2D<T> left, Vector2D<T> right)
        where T : INumberBase<T>
    {
        return left - right;
    }

    // /// <summary>Transforms a vector by a specified 3x2 matrix.</summary>
    // /// <param name="position">The vector to transform.</param>
    // /// <param name="matrix">The transformation matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector2D<T> Transform<T>(Vector2D<T> position, Matrix3X2<T> matrix)
    //     where T : INumberBase<T>
    // {
    //     Vector2D<T> result = matrix.X * position.X;
    //
    //     result += matrix.Y * position.Y;
    //     result += matrix.Z;
    //
    //     return result;
    // }

    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Transform<T>(Vector2D<T> position, Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        return (Vector2D<T>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion<T> rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Transform<T>(Vector2D<T> value, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T yy2 = rotation.Y * y2;
        T zz2 = rotation.Z * z2;

        return new Vector2D<T>(
            value.X * (T.One - yy2 - zz2) + value.Y * (xy2 - wz2),
            value.X * (xy2 + wz2) + value.Y * (T.One - xx2 - zz2)
        );
    }

    // /// <summary>Transforms a vector normal by the given 3x2 matrix.</summary>
    // /// <param name="normal">The source vector.</param>
    // /// <param name="matrix">The matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector2D<T> TransformNormal<T>(Vector2D<T> normal, Matrix3X2<T> matrix) where T : INumberBase<T>
    // {
    //     Vector2D<T> result = matrix.X * normal.X;
    //
    //     result += matrix.Y * normal.Y;
    //
    //     return result;
    // }

    /// <summary>Transforms a vector normal by the given 4x4 matrix.</summary>
    /// <param name="normal">The source vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> TransformNormal<T>(Vector2D<T> normal, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        Vector4D<T> result = matrix.X * normal.X;

        result += matrix.Y * normal.Y;

        return (Vector2D<T>)result;
    }

    /// <summary>Returns the length of the vector.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Vector2D<T> self) where T : INumberBase<T>, IRootFunctions<T>
    {
        T lengthSquared = LengthSquared(self);
        return T.Sqrt(lengthSquared);
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T}" /> method.</remarks>
    /// <altmember cref="Length{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Vector2D<T> self) where T : INumberBase<T>
    {
        return Dot(self, self);
    }

}