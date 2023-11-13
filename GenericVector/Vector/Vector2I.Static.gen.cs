
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

public static class Vector2I
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
    public static void CopyTo<TScalar>(in this Vector2I<TScalar> self, TScalar[] array) where TScalar : IBinaryInteger<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons
    
        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector2I<TScalar>.ElementCount, nameof(array));
    
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
    public static void CopyTo<TScalar>(in this Vector2I<TScalar> self, TScalar[] array, int index) where TScalar : IBinaryInteger<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons
    
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector2I<TScalar>.ElementCount);
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref array[index]), self);
    }
    
    /// <summary>Copies the vector to the given <see cref="Span{TScalar}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2I<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryInteger<TScalar>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector2I<TScalar>.ElementCount, nameof(destination));
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }
    
    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<TScalar>(in this Vector2I<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryInteger<TScalar>
    {
        if (destination.Length < Vector2I<TScalar>.ElementCount)
        {
            return false;
        }
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion
    
    #region Extension
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TScalar> AsSpan<TScalar>(this Vector2I<TScalar> vec) where TScalar : IBinaryInteger<TScalar>
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Vector2I<TScalar>, TScalar>(ref Unsafe.AsRef(in vec)), Vector2I<TScalar>.ElementCount);
    }
    
    #endregion
    
    #region Operator Shortcuts
    
    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Multiply<TScalar>(Vector2I<TScalar> left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Multiply<TScalar>(Vector2I<TScalar> left, TScalar right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Multiply<TScalar>(TScalar left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Negate<TScalar>(Vector2I<TScalar> value) where TScalar : IBinaryInteger<TScalar>
    {
        return -value;
    }
    
    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Subtract<TScalar>(Vector2I<TScalar> left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left - right;
    }
    
    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Add<TScalar>(Vector2I<TScalar> left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left + right;
    }
    
    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Divide<TScalar>(Vector2I<TScalar> left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>
    {
        return left / right;
    }
    
    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Divide<TScalar>(Vector2I<TScalar> left, TScalar divisor) where TScalar : IBinaryInteger<TScalar>
    {
        return left / divisor;
    }
    
    #endregion
    
    #region Other
    
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Abs<TScalar>(Vector2I<TScalar> value) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.Abs<Vector2I<TScalar>, TScalar>(value);
    }
    
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Clamp<TScalar>(Vector2I<TScalar> value1, Vector2I<TScalar> min, Vector2I<TScalar> max) where TScalar : IBinaryInteger<TScalar>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return SpeedHelpers2.Clamp<Vector2I<TScalar>, TScalar>(value1, min, max);
    }
    
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar DistanceSquared<TScalar>(Vector2I<TScalar> value1, Vector2I<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.DistanceSquared<Vector2I<TScalar>, TScalar>(value1, value2);
    }
    
    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Dot<TScalar>(Vector2I<TScalar> vector1, Vector2I<TScalar> vector2) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.Dot<Vector2I<TScalar>, TScalar>(vector1, vector2);
    }
    
    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Max<TScalar>(Vector2I<TScalar> value1, Vector2I<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.Max<Vector2I<TScalar>, TScalar>(value1, value2);
    }
    
    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Min<TScalar>(Vector2I<TScalar> value1, Vector2I<TScalar> value2) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.Min<Vector2I<TScalar>, TScalar>(value1, value2);
    }
    
    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Transform<TScalar>(Vector2I<TScalar> position, Matrix4X4<TScalar> matrix) where TScalar : IBinaryInteger<TScalar>
    {
        return (Vector2I<TScalar>)Vector4D.Transform(position, matrix);
    }
    
    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TReturn> Transform<TScalar, TQuat, TReturn>(Vector2I<TScalar> value, Quaternion<TQuat> rotation) where TScalar : IBinaryInteger<TScalar> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
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
    
        return new Vector2I<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y))
        );
    
        return new Vector2I<TReturn>(
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
    // internal static Vector2I<TScalar> TransformNormal<TScalar>(Vector2I<TScalar> normal, in Matrix4x4 matrix) where TScalar : IBinaryInteger<TScalar>
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
    public static Vector2I<TScalar> Remainder<TScalar>(this Vector2I<TScalar> left, Vector2I<TScalar> right) where TScalar : IBinaryInteger<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder<Vector2I<TScalar>, TScalar>(left, right);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2I<TScalar> Remainder<TScalar>(this Vector2I<TScalar> left, TScalar right) where TScalar : IBinaryInteger<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder<Vector2I<TScalar>, TScalar>(left, right);
    }
    #endregion
    
    #region Specializations
    
    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{TScalar}" /> method.</remarks>
    /// <altmember cref="Length{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar LengthSquared<TScalar>(this Vector2I<TScalar> vec) where TScalar : IBinaryInteger<TScalar>
    {
        return SpeedHelpers2.LengthSquared<Vector2I<TScalar>, TScalar>(vec);
    }
    #endregion
    
    // INumber<TScalar>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> CopySign<TScalar>(Vector2I<TScalar> value, Vector2I<TScalar> sign) where TScalar : IBinaryInteger<TScalar> => new(TScalar.CopySign(value.X, sign.X), TScalar.CopySign(value.Y, sign.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> CopySign<TScalar>(Vector2I<TScalar> value, TScalar sign) where TScalar : IBinaryInteger<TScalar> => new(TScalar.CopySign(value.X, sign), TScalar.CopySign(value.Y, sign));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> MaxNumber<TScalar>(Vector2I<TScalar> x, Vector2I<TScalar> y) where TScalar : IBinaryInteger<TScalar> => new(TScalar.MaxNumber(x.X, y.X), TScalar.MaxNumber(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> MinNumber<TScalar>(Vector2I<TScalar> x, Vector2I<TScalar> y) where TScalar : IBinaryInteger<TScalar> => new(TScalar.MinNumber(x.X, y.X), TScalar.MinNumber(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> Sign<TScalar>(Vector2I<TScalar> value) where TScalar : IBinaryInteger<TScalar> => new(TScalar.CreateChecked(TScalar.Sign(value.X)), TScalar.CreateChecked(TScalar.Sign(value.Y)));


    // IBinaryInteger<TScalar>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> Log2<TScalar>(Vector2I<TScalar> value) where TScalar : IBinaryInteger<TScalar> => new(TScalar.Log2(value.X), TScalar.Log2(value.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2I<TScalar> PopCount<TScalar>(Vector2I<TScalar> value) where TScalar : IBinaryInteger<TScalar> => new(TScalar.PopCount(value.X), TScalar.PopCount(value.Y));
}