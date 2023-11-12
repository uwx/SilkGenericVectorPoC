using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GenericVector.Experimental;

public static class Vector2f
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
    public static void CopyTo<TScalar>(in this Vector2f<TScalar> self, TScalar[] array) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector2f<TScalar>.ElementCount, nameof(array));

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
    public static void CopyTo<TScalar>(in this Vector2f<TScalar> self, TScalar[] array, int index) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector2f<TScalar>.ElementCount);

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref array[index]), self);
    }

    /// <summary>Copies the vector to the given <see cref="Span{TScalar}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2f<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector2f<TScalar>.ElementCount, nameof(destination));

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<TScalar>(in this Vector2f<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        if (destination.Length < Vector2f<TScalar>.ElementCount)
        {
            return false;
        }

        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion

    #region Extension

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TScalar> AsSpan<TScalar>(this Vector2f<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Vector2f<TScalar>, TScalar>(ref Unsafe.AsRef(in vec)), Vector2f<TScalar>.ElementCount);
    }
    
    #endregion

    #region Operator Shortcuts

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Multiply<TScalar>(Vector2f<TScalar> left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Multiply<TScalar>(Vector2f<TScalar> left, TScalar right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Multiply<TScalar>(TScalar left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Negate<TScalar>(Vector2f<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return -value;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Subtract<TScalar>(Vector2f<TScalar> left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left - right;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Add<TScalar>(Vector2f<TScalar> left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left + right;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Divide<TScalar>(Vector2f<TScalar> left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Divide<TScalar>(Vector2f<TScalar> left, TScalar divisor) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left / divisor;
    }

    #endregion

    #region Other

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Abs<TScalar>(Vector2f<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Abs<Vector2f<TScalar>, TScalar>(value);
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Clamp<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> min, Vector2f<TScalar> max) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return SpeedHelpers2.Clamp<Vector2f<TScalar>, TScalar>(value1, min, max);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar DistanceSquared<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.DistanceSquared<Vector2f<TScalar>, TScalar>(value1, value2);
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Dot<TScalar>(Vector2f<TScalar> vector1, Vector2f<TScalar> vector2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Dot<Vector2f<TScalar>, TScalar>(vector1, vector2);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Max<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Max<Vector2f<TScalar>, TScalar>(value1, value2);
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Min<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Min<Vector2f<TScalar>, TScalar>(value1, value2);
    }

    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Transform<TScalar>(Vector2f<TScalar> position, Matrix4X4<TScalar> matrix) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (Vector2f<TScalar>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TReturn> Transform<TScalar, TQuat, TReturn>(Vector2f<TScalar> value, Quaternion<TQuat> rotation) where TScalar : IBinaryFloatingPointIeee754<TScalar> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
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

        return new Vector2f<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y))
        );

        return new Vector2f<TReturn>(
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
    // internal static Vector2f<TScalar> TransformNormal<TScalar>(Vector2f<TScalar> normal, in Matrix4x4 matrix) where TScalar : IBinaryFloatingPointIeee754<TScalar>
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
    public static Vector2f<TScalar> Remainder<TScalar>(this Vector2f<TScalar> left, Vector2f<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder<Vector2f<TScalar>, TScalar>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Remainder<TScalar>(this Vector2f<TScalar> left, TScalar right) where TScalar : IBinaryFloatingPointIeee754<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder(left, right);
    }
    #endregion

    #region Specializations

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{TScalar}" /> method.</remarks>
    /// <altmember cref="Length{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar LengthSquared<TScalar>(this Vector2f<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.LengthSquared<Vector2f<TScalar>, TScalar>(vec);
    }
    #endregion

    // INumber<TScalar>
    public static Vector2f<TScalar> CopySign<TScalar>(Vector2f<TScalar> value, Vector2f<TScalar> sign) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CopySign(value.X, sign.X), TScalar.CopySign(value.Y, sign.Y));
    public static Vector2f<TScalar> CopySign<TScalar>(Vector2f<TScalar> value, TScalar sign) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CopySign(value.X, sign), TScalar.CopySign(value.Y, sign));
    public static Vector2f<TScalar> MaxNumber<TScalar>(Vector2f<TScalar> x, Vector2f<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.MaxNumber(x.X, y.X), TScalar.MaxNumber(x.Y, y.Y));
    public static Vector2f<TScalar> MinNumber<TScalar>(Vector2f<TScalar> x, Vector2f<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.MinNumber(x.X, y.X), TScalar.MinNumber(x.Y, y.Y));
    public static Vector2f<TScalar> Sign<TScalar>(Vector2f<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CreateChecked(TScalar.Sign(value.X)), TScalar.CreateChecked(TScalar.Sign(value.Y)));
    
    // Float-speciifc stuff
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Length<TScalar>(this Vector2f<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>, IRootFunctions<TScalar>
    {
        var lengthSquared = vec.LengthSquared();
        return TScalar.Sqrt(lengthSquared);
    }
    
    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Distance<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        var distanceSquared = DistanceSquared(value1, value2);
        return TScalar.Sqrt(distanceSquared);
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Lerp<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (TScalar.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2f<TScalar> LerpUnchecked<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (TScalar.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> LerpClamped<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = TScalar.Clamp(amount, TScalar.Zero, TScalar.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2f<TScalar> LerpClampedUnchecked<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TScalar ClampT(TScalar value, TScalar min, TScalar max)
        {
            return TScalar.MaxMagnitude(TScalar.MaxMagnitude(value, min), max);
        }

        amount = ClampT(amount, TScalar.Zero, TScalar.One);
        return LerpUnchecked(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Lerp<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (Vector2f<TScalar>.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2f<TScalar> LerpUnchecked<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (Vector2f<TScalar>.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> LerpClamped<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = Clamp(amount, Vector2f<TScalar>.Zero, Vector2f<TScalar>.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2f<TScalar> LerpClampedUnchecked<TScalar>(Vector2f<TScalar> value1, Vector2f<TScalar> value2, Vector2f<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = Clamp(amount, Vector2f<TScalar>.Zero, Vector2f<TScalar>.One);
        return LerpUnchecked(value1, value2, amount);
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Normalize<TScalar>(Vector2f<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Reflect<TScalar>(Vector2f<TScalar> vector, Vector2f<TScalar> normal) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        var dot = Dot(vector, normal);
        return vector - (Scalar<TScalar>.Two * (dot * normal));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2f<TScalar> Sqrt<TScalar>(Vector2f<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return new Vector2f<TScalar>(
            TScalar.Sqrt(value.X), 
            TScalar.Sqrt(value.Y)
        );
    }
    
    // Even more float-specific stuff
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Acosh(Vector2f<TScalar> x) => Vector2f.Acosh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Asinh(Vector2f<TScalar> x) => Vector2f.Asinh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atanh(Vector2f<TScalar> x) => Vector2f.Atanh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cosh(Vector2f<TScalar> x) => Vector2f.Cosh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Sinh(Vector2f<TScalar> x) => Vector2f.Sinh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Tanh(Vector2f<TScalar> x) => Vector2f.Tanh(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Acos(Vector2f<TScalar> x) => Vector2f.Acos(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AcosPi(Vector2f<TScalar> x) => Vector2f.AcosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Asin(Vector2f<TScalar> x) => Vector2f.Asin(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AsinPi(Vector2f<TScalar> x) => Vector2f.AsinPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan(Vector2f<TScalar> x) => Vector2f.Atan(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.AtanPi(Vector2f<TScalar> x) => Vector2f.AtanPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cos(Vector2f<TScalar> x) => Vector2f.Cos(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.CosPi(Vector2f<TScalar> x) => Vector2f.CosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.DegreesToRadians(Vector2f<TScalar> degrees) => Vector2f.DegreesToRadians(degrees);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.RadiansToDegrees(Vector2f<TScalar> radians) => Vector2f.RadiansToDegrees(radians);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Sin(Vector2f<TScalar> x) => Vector2f.Sin(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinPi(Vector2f<TScalar> x) => Vector2f.SinPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Tan(Vector2f<TScalar> x) => Vector2f.Tan(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.TanPi(Vector2f<TScalar> x) => Vector2f.TanPi(x);
    static (Vector2f<TScalar> Sin, Vector2f<TScalar> Cos) IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinCos(Vector2f<TScalar> x) => Vector2f.SinCos(x);
    static (Vector2f<TScalar> SinPi, Vector2f<TScalar> CosPi) IFloatingPointVector<Vector2f<TScalar>, TScalar>.SinCosPi(Vector2f<TScalar> x) => Vector2f.SinCosPi(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x) => Vector2f.Log(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x, Vector2f<TScalar> newBase) => Vector2f.Log(x, newBase);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log(Vector2f<TScalar> x, TScalar newBase) => Vector2f.Log(x, newBase);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.LogP1(Vector2f<TScalar> x) => Vector2f.LogP1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log2(Vector2f<TScalar> x) => Vector2f.Log2(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log2P1(Vector2f<TScalar> x) => Vector2f.Log2P1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log10(Vector2f<TScalar> x) => Vector2f.Log10(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Log10P1(Vector2f<TScalar> x) => Vector2f.Log10P1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp(Vector2f<TScalar> x) => Vector2f.Exp(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ExpM1(Vector2f<TScalar> x) => Vector2f.ExpM1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp2(Vector2f<TScalar> x) => Vector2f.Exp2(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp2M1(Vector2f<TScalar> x) => Vector2f.Exp2M1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp10(Vector2f<TScalar> x) => Vector2f.Exp10(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Exp10M1(Vector2f<TScalar> x) => Vector2f.Exp10M1(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Pow(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Pow(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Pow(Vector2f<TScalar> x, TScalar y) => Vector2f.Pow(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Cbrt(Vector2f<TScalar> x) =>
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Hypot(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Hypot(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Hypot(Vector2f<TScalar> x, TScalar y) => Vector2f.Hypot(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.RootN(Vector2f<TScalar> x, int n) => Vector2f.RootN(x, n);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x) => Vector2f.Round(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, int digits) => Vector2f.Round(x, digits);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, MidpointRounding mode) => Vector2f.Round(x, mode);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Round(Vector2f<TScalar> x, int digits, MidpointRounding mode) => Vector2f.Round(x, digits, mode);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Truncate(Vector2f<TScalar> x) => Vector2f.Truncate(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Atan2(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2Pi(Vector2f<TScalar> x, Vector2f<TScalar> y) => Vector2f.Atan2Pi(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2(Vector2f<TScalar> x, TScalar y) => Vector2f.Atan2(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.Atan2Pi(Vector2f<TScalar> x, TScalar y) => Vector2f.Atan2Pi(x, y);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.BitDecrement(Vector2f<TScalar> x) => Vector2f.BitDecrement(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.BitIncrement(Vector2f<TScalar> x) => Vector2f.BitIncrement(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.FusedMultiplyAdd(Vector2f<TScalar> left, Vector2f<TScalar> right, Vector2f<TScalar> addend) => Vector2f.FusedMultiplyAdd(left, right, addend);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ReciprocalEstimate(Vector2f<TScalar> x) => Vector2f.ReciprocalEstimate(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ReciprocalSqrtEstimate(Vector2f<TScalar> x) => Vector2f.ReciprocalSqrtEstimate(x);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.ILogB<TNewVector, TInt>(Vector2f<TScalar> x) => TNewVector.Create(Vector2f.ILogB<TNewVector, TInt>(x);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ScaleB(Vector2f<TScalar> x, Vector2D<int> n) => Vector2f.ScaleB(x, n);
    static Vector2f<TScalar> IFloatingPointVector<Vector2f<TScalar>, TScalar>.ScaleB(Vector2f<TScalar> x, int n) => Vector2f.ScaleB(x, n);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.RoundToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.RoundToInt<TNewVector, TInt>(vector);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.FloorToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.FloorToInt<TNewVector, TInt>(vector);
    static TNewVector IFloatingPointVector<Vector2f<TScalar>, TScalar>.CeilingToInt<TNewVector, TInt>(Vector2f<TScalar> vector) => Vector2f.CeilingToInt<TNewVector, TInt>(vector);

}