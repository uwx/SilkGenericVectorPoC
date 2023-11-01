using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GenericVector.Experimental;

public static class Vector2i
{
    #region CopyTo
    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have at least two elements. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2i<TScalar> self, TScalar[] array) where TScalar : IBinaryInteger<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector2i<TScalar>.ElementCount, nameof(array));

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref array[0]), self);
    }

    /// <summary>Copies the elements of the vector to a specified array starting at a specified index position.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index at which to copy the first element of the vector.</param>
    /// <remarks><paramref name="array" /> must have a sufficient number of elements to accommodate the two vector elements. In other words, elements <paramref name="index" /> through <paramref name="index" /> + 2 must already exist in <paramref name="array" />.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the array length.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2i<TScalar> self, TScalar[] array, int index) where TScalar : IBinaryInteger<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector2i<TScalar>.ElementCount);

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref array[index]), self);
    }

    /// <summary>Copies the vector to the given <see cref="Span{TScalar}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2i<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryInteger<TScalar>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector2i<TScalar>.ElementCount, nameof(destination));

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<TScalar>(in this Vector2i<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryInteger<TScalar>
    {
        if (destination.Length < Vector2i<TScalar>.ElementCount)
        {
            return false;
        }

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion

    #region Extension

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TScalar> AsSpan<TScalar>(this Vector2i<TScalar> vec) where TScalar : IBinaryInteger<TScalar>
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Vector2i<TScalar>, TScalar>(ref Unsafe.AsRef(in vec)), Vector2i<TScalar>.ElementCount);
    }
    
    #endregion

    #region Basic

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Multiply<TScalar>(Vector2i<TScalar> left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Multiply<TScalar>(Vector2i<TScalar> left, TScalar right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Multiply<TScalar>(TScalar left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Negate<TScalar>(Vector2i<TScalar> value) where TScalar : IBinaryInteger<TScalar>
    {
        return -value;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Subtract<TScalar>(Vector2i<TScalar> left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left - right;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Add<TScalar>(Vector2i<TScalar> left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left + right;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Divide<TScalar>(Vector2i<TScalar> left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Divide<TScalar>(Vector2i<TScalar> left, TScalar divisor) where TScalar : IBinaryInteger<TScalar>
    {
        return left / divisor;
    }

    #endregion

    #region Other

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Abs<TScalar>(Vector2i<TScalar> value) where TScalar : IBinaryInteger<TScalar>
    {
        return new(TScalar.Abs(value.X), TScalar.Abs(value.Y));
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Clamp<TScalar>(Vector2i<TScalar> value1, Vector2i<TScalar> min, Vector2i<TScalar> max) where TScalar : IBinaryInteger<TScalar>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return Min(Max(value1, min), max);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar DistanceSquared<TScalar>(Vector2i<TScalar> value1, Vector2i<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        var difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Dot<TScalar>(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2) where TScalar : IBinaryInteger<TScalar>
    {
        return
            vector1.X * vector2.X +
            vector1.Y * vector2.Y;
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Dot<TScalar, TReturn>(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2) where TScalar : IBinaryInteger<TScalar> where TReturn : INumberBase<TReturn>
    {
        return
            TReturn.CreateTruncating(vector1.X) * TReturn.CreateTruncating(vector2.X) +
            TReturn.CreateTruncating(vector1.Y) * TReturn.CreateTruncating(vector2.Y);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Max<TScalar>(Vector2i<TScalar> value1, Vector2i<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        return new Vector2i<TScalar>(
            TScalar.MaxMagnitudeNumber(value1.X, value2.X), 
            TScalar.MaxMagnitudeNumber(value1.Y, value2.Y)
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Min<TScalar>(Vector2i<TScalar> value1, Vector2i<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        return new Vector2i<TScalar>(
        TScalar.MinMagnitudeNumber(value1.X, value2.X), 
        TScalar.MinMagnitudeNumber(value1.Y, value2.Y)
        );
    }

    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Transform<TScalar>(Vector2i<TScalar> position, Matrix4X4<TScalar> matrix) where TScalar : IBinaryInteger<TScalar>
    {
        return (Vector2i<TScalar>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TReturn> Transform<TScalar, TQuat, TReturn>(Vector2i<TScalar> value, Quaternion<TQuat> rotation) where TScalar : IBinaryInteger<TScalar> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;var x2 = rotation.X + rotation.X;
        var y2 = rotation.Y + rotation.Y;
        var z2 = rotation.Z + rotation.Z;


        var  = rotation.X + rotation.X;
        var  = rotation.Y + rotation.Y;var wx2 = TReturn.CreateTruncating(rotation.W * x2);
        var wy2 = TReturn.CreateTruncating(rotation.W * y2);
        var wz2 = TReturn.CreateTruncating(rotation.W * z2);
        var xx2 = TReturn.CreateTruncating(rotation.X * x2);
        var xy2 = TReturn.CreateTruncating(rotation.X * y2);
        var xz2 = TReturn.CreateTruncating(rotation.X * z2);
        var yy2 = TReturn.CreateTruncating(rotation.Y * y2);
        var yz2 = TReturn.CreateTruncating(rotation.Y * z2);
        var zz2 = TReturn.CreateTruncating(rotation.Z * z2);

        return new Vector2i<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y))
        );

        return new Vector2i<TReturn>(
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
    // internal static Vector2i<TScalar> TransformNormal<TScalar>(Vector2i<TScalar> normal, in Matrix4x4 matrix) where TScalar : IBinaryInteger<TScalar>
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
    public static Vector2i<TScalar> Remainder<TScalar>(this Vector2i<TScalar> left, Vector2i<TScalar> right) where TScalar : IBinaryInteger<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return new Vector2i<TScalar>(
            left.X % right.X,
            left.Y % right.Y
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TScalar> Remainder<TScalar>(this Vector2i<TScalar> left, TScalar right) where TScalar : IBinaryInteger<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return new Vector2i<TScalar>(
            left.X % right,
            left.Y % right
        );
    }
    #endregion

    #region Specializations

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{TScalar}" /> method.</remarks>
    /// <altmember cref="Length{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar LengthSquared<TScalar>(this Vector2i<TScalar> vec) where TScalar : IBinaryInteger<TScalar>
    {
        return Dot<TScalar>(vec, vec);
    }
    #endregion

    // INumber<TScalar>
    public static Vector2i<TScalar> CopySign<TScalar>(Vector2i<TScalar> value, Vector2i<TScalar> sign) where TScalar : IBinaryInteger<TScalar> => new(TScalar.CopySign(value.X, sign.X), TScalar.CopySign(value.Y, sign.Y));
    public static Vector2i<TScalar> CopySign<TScalar>(Vector2i<TScalar> value, TScalar sign) where TScalar : IBinaryInteger<TScalar> => new(TScalar.CopySign(value.X, sign), TScalar.CopySign(value.Y, sign));
    public static Vector2i<TScalar> MaxNumber<TScalar>(Vector2i<TScalar> x, Vector2i<TScalar> y) where TScalar : IBinaryInteger<TScalar> => new(TScalar.MaxNumber(x.X, y.X), TScalar.MaxNumber(x.Y, y.Y));
    public static Vector2i<TScalar> MinNumber<TScalar>(Vector2i<TScalar> x, Vector2i<TScalar> y) where TScalar : IBinaryInteger<TScalar> => new(TScalar.MinNumber(x.X, y.X), TScalar.MinNumber(x.Y, y.Y));
}

// IVector<Vector2i<TScalar>, TScalar>
// public readonly partial record struct Vector2i<TScalar>
// {
//     TScalar IVector<Vector2i<TScalar>, TScalar>.LengthSquared()
//         => this.LengthSquared();
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Multiply(Vector2i<TScalar> left, Vector2i<TScalar> right)
//         => Vector2D.Multiply(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Multiply(Vector2i<TScalar> left, TScalar right)
//         => Vector2D.Multiply(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Multiply(TScalar left, Vector2i<TScalar> right)
//         => Vector2D.Multiply(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Negate(Vector2i<TScalar> value)
//         => Vector2D.Negate(value);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Subtract(Vector2i<TScalar> left, Vector2i<TScalar> right)
//         => Vector2D.Subtract(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Add(Vector2i<TScalar> left, Vector2i<TScalar> right)
//         => Vector2D.Add(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Divide(Vector2i<TScalar> left, Vector2i<TScalar> right)
//         => Vector2D.Divide(left, right);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Divide(Vector2i<TScalar> left, TScalar divisor)
//         => Vector2D.Divide(left, divisor);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Clamp(Vector2i<TScalar> value1, Vector2i<TScalar> min, Vector2i<TScalar> max)
//         => Vector2D.Clamp(value1, min, max);
//     static TReturn IVector<Vector2i<TScalar>, TScalar>.Distance<TReturn>(Vector2i<TScalar> value1, Vector2i<TScalar> value2)
//         => Vector2D.Distance<TScalar, TReturn>(value1, value2);
//     static TScalar IVector<Vector2i<TScalar>, TScalar>.DistanceSquared(Vector2i<TScalar> value1, Vector2i<TScalar> value2)
//         => Vector2D.DistanceSquared(value1, value2);
//     static TReturn IVector<Vector2i<TScalar>, TScalar>.DistanceSquared<TReturn>(Vector2i<TScalar> value1, Vector2i<TScalar> value2)
//         => Vector2D.DistanceSquared<TScalar, TReturn>(value1, value2);
//     static TScalar IVector<Vector2i<TScalar>, TScalar>.Dot(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2)
//         => Vector2D.Dot(vector1, vector2);
//     static TReturn IVector<Vector2i<TScalar>, TScalar>.Dot<TReturn>(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2)
//         => Vector2D.Dot<TScalar, TReturn>(vector1, vector2);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Max(Vector2i<TScalar> value1, Vector2i<TScalar> value2)
//         => Vector2D.Max(value1, value2);
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Min(Vector2i<TScalar> value1, Vector2i<TScalar> value2)
//         => Vector2D.Min(value1, value2);
//
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Lerp(Vector2i<TScalar> value1, Vector2i<TScalar> value2, TScalar amount) /* where TScalar : IFloatingPoint<TScalar> */
//     {
//         Helpers.CheckTypeAndThrow<Vector2i<TScalar>, TScalar>(typeof(IFloatingPoint<>));
//         return Vector2D.LerpUnchecked(value1, value2, amount);
//     }
//
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.LerpClamped(Vector2i<TScalar> value1, Vector2i<TScalar> value2, TScalar amount) /* where TScalar : IFloatingPoint<TScalar> */
//     {
//         Helpers.CheckTypeAndThrow<Vector2i<TScalar>, TScalar>(typeof(IFloatingPoint<>));
//         return Vector2D.LerpClampedUnchecked(value1, value2, amount);
//     }
//
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Lerp(Vector2i<TScalar> value1, Vector2i<TScalar> value2, Vector2i<TScalar> amount) /* where TScalar : IFloatingPoint<TScalar> */
//     {
//         Helpers.CheckTypeAndThrow<Vector2i<TScalar>, TScalar>(typeof(IFloatingPoint<>));
//         return Vector2D.LerpUnchecked(value1, value2, amount);
//     }
//
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.LerpClamped(Vector2i<TScalar> value1, Vector2i<TScalar> value2, Vector2i<TScalar> amount) /* where TScalar : IFloatingPoint<TScalar> */
//     {
//         Helpers.CheckTypeAndThrow<Vector2i<TScalar>, TScalar>(typeof(IFloatingPoint<>));
//         return Vector2D.LerpClampedUnchecked(value1, value2, amount);
//     }
//
//     static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Reflect(Vector2i<TScalar> vector, Vector2i<TScalar> normal) /* where TScalar : IFloatingPoint<TScalar> */
//     {
//         Helpers.CheckTypeAndThrow<Vector2i<TScalar>, TScalar>(typeof(IFloatingPoint<>));
//         return Vector2D.Reflect<TScalar, TScalar>(vector, normal);
//     }
// }