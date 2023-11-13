
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

public static class Vector2F
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
    public static void CopyTo<TScalar>(in this Vector2F<TScalar> self, TScalar[] array) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons
    
        ArgumentOutOfRangeException.ThrowIfLessThan(array.Length, Vector2F<TScalar>.ElementCount, nameof(array));
    
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
    public static void CopyTo<TScalar>(in this Vector2F<TScalar> self, TScalar[] array, int index) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We explicitly don't check for `null` because historically this has thrown `NullReferenceException` for perf reasons
    
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)array.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), Vector2F<TScalar>.ElementCount);
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref array[index]), self);
    }
    
    /// <summary>Copies the vector to the given <see cref="Span{TScalar}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TScalar>(in this Vector2F<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(destination.Length, Vector2F<TScalar>.ElementCount, nameof(destination));
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
    }
    
    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least 2.</summary>
    /// <param name="self">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCopyTo<TScalar>(in this Vector2F<TScalar> self, Span<TScalar> destination) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        if (destination.Length < Vector2F<TScalar>.ElementCount)
        {
            return false;
        }
    
        Unsafe.WriteUnaligned(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(destination)), self);
        return true;
    }
    #endregion
    
    #region Extension
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TScalar> AsSpan<TScalar>(this Vector2F<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Vector2F<TScalar>, TScalar>(ref Unsafe.AsRef(in vec)), Vector2F<TScalar>.ElementCount);
    }
    
    #endregion
    
    #region Operator Shortcuts
    
    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Multiply<TScalar>(Vector2F<TScalar> left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Multiply<TScalar>(Vector2F<TScalar> left, TScalar right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Multiply<TScalar>(TScalar left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left * right;
    }
    
    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Negate<TScalar>(Vector2F<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return -value;
    }
    
    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Subtract<TScalar>(Vector2F<TScalar> left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left - right;
    }
    
    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Add<TScalar>(Vector2F<TScalar> left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left + right;
    }
    
    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Divide<TScalar>(Vector2F<TScalar> left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left / right;
    }
    
    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Divide<TScalar>(Vector2F<TScalar> left, TScalar divisor) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return left / divisor;
    }
    
    #endregion
    
    #region Other
    
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Abs<TScalar>(Vector2F<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Abs<Vector2F<TScalar>, TScalar>(value);
    }
    
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Clamp<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> min, Vector2F<TScalar> max) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return SpeedHelpers2.Clamp<Vector2F<TScalar>, TScalar>(value1, min, max);
    }
    
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar DistanceSquared<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.DistanceSquared<Vector2F<TScalar>, TScalar>(value1, value2);
    }
    
    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Dot<TScalar>(Vector2F<TScalar> vector1, Vector2F<TScalar> vector2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Dot<Vector2F<TScalar>, TScalar>(vector1, vector2);
    }
    
    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Max<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Max<Vector2F<TScalar>, TScalar>(value1, value2);
    }
    
    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Min<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.Min<Vector2F<TScalar>, TScalar>(value1, value2);
    }
    
    // CANNOT BE DONE
    /*
    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Transform<TScalar>(Vector2F<TScalar> position, Matrix4X4<TScalar> matrix) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (Vector2F<TScalar>)Vector4D.Transform(position, matrix);
    }
    
    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TReturn> Transform<TScalar, TQuat, TReturn>(Vector2F<TScalar> value, Quaternion<TQuat> rotation) where TScalar : IBinaryFloatingPointIeee754<TScalar> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
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
    
        return new Vector2F<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y))
        );
    
        return new Vector2F<TReturn>(
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
    // internal static Vector2F<TScalar> TransformNormal<TScalar>(Vector2F<TScalar> normal, in Matrix4x4 matrix) where TScalar : IBinaryFloatingPointIeee754<TScalar>
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
    public static Vector2F<TScalar> Remainder<TScalar>(this Vector2F<TScalar> left, Vector2F<TScalar> right) where TScalar : IBinaryFloatingPointIeee754<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder<Vector2F<TScalar>, TScalar>(left, right);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Remainder<TScalar>(this Vector2F<TScalar> left, TScalar right) where TScalar : IBinaryFloatingPointIeee754<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
    {
        return SpeedHelpers2.Remainder<Vector2F<TScalar>, TScalar>(left, right);
    }
    #endregion
    
    #region Specializations
    
    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{TScalar}" /> method.</remarks>
    /// <altmember cref="Length{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar LengthSquared<TScalar>(this Vector2F<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return SpeedHelpers2.LengthSquared<Vector2F<TScalar>, TScalar>(vec);
    }
    #endregion
    
    // INumber<TScalar>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> CopySign<TScalar>(Vector2F<TScalar> value, Vector2F<TScalar> sign) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CopySign(value.X, sign.X), TScalar.CopySign(value.Y, sign.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> CopySign<TScalar>(Vector2F<TScalar> value, TScalar sign) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CopySign(value.X, sign), TScalar.CopySign(value.Y, sign));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> MaxNumber<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.MaxNumber(x.X, y.X), TScalar.MaxNumber(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> MinNumber<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.MinNumber(x.X, y.X), TScalar.MinNumber(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Sign<TScalar>(Vector2F<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CreateChecked(TScalar.Sign(value.X)), TScalar.CreateChecked(TScalar.Sign(value.Y)));


    // Float-specific stuff
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Length<TScalar>(this Vector2F<TScalar> vec) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        var lengthSquared = vec.LengthSquared();
        return TScalar.Sqrt(lengthSquared);
    }

    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Distance<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2) where TScalar : IBinaryFloatingPointIeee754<TScalar>
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
    public static Vector2F<TScalar> Lerp<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (TScalar.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2F<TScalar> LerpUnchecked<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (TScalar.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> LerpClamped<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = TScalar.Clamp(amount, TScalar.Zero, TScalar.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2F<TScalar> LerpClampedUnchecked<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, TScalar amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
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
    public static Vector2F<TScalar> Lerp<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (Vector2F<TScalar>.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2F<TScalar> LerpUnchecked<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return (value1 * (Vector2F<TScalar>.One - amount)) + (value2 * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> LerpClamped<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = Clamp(amount, Vector2F<TScalar>.Zero, Vector2F<TScalar>.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2F<TScalar> LerpClampedUnchecked<TScalar>(Vector2F<TScalar> value1, Vector2F<TScalar> value2, Vector2F<TScalar> amount) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        amount = Clamp(amount, Vector2F<TScalar>.Zero, Vector2F<TScalar>.One);
        return LerpUnchecked(value1, value2, amount);
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Normalize<TScalar>(Vector2F<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Reflect<TScalar>(Vector2F<TScalar> vector, Vector2F<TScalar> normal) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        var dot = Dot(vector, normal);
        return vector - (Scalar<TScalar>.Two * (dot * normal));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2F<TScalar> Sqrt<TScalar>(Vector2F<TScalar> value) where TScalar : IBinaryFloatingPointIeee754<TScalar>
    {
        return new Vector2F<TScalar>(
            TScalar.Sqrt(value.X),
            TScalar.Sqrt(value.Y)
        );
    }

    // Even more float-specific stuff
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Acosh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Acosh(x.X), TScalar.Acosh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Asinh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Asinh(x.X), TScalar.Asinh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atanh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atanh(x.X), TScalar.Atanh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Cosh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Cosh(x.X), TScalar.Cosh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Sinh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Sinh(x.X), TScalar.Sinh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Tanh<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Tanh(x.X), TScalar.Tanh(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Acos<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Acos(x.X), TScalar.Acos(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> AcosPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.AcosPi(x.X), TScalar.AcosPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Asin<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Asin(x.X), TScalar.Asin(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> AsinPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.AsinPi(x.X), TScalar.AsinPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atan<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atan(x.X), TScalar.Atan(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> AtanPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.AtanPi(x.X), TScalar.AtanPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Cos<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Cos(x.X), TScalar.Cos(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> CosPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.CosPi(x.X), TScalar.CosPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> DegreesToRadians<TScalar>(Vector2F<TScalar> degrees) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.DegreesToRadians(degrees.X), TScalar.DegreesToRadians(degrees.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> RadiansToDegrees<TScalar>(Vector2F<TScalar> radians) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.RadiansToDegrees(radians.X), TScalar.RadiansToDegrees(radians.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Sin<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Sin(x.X), TScalar.Sin(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> SinPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.SinPi(x.X), TScalar.SinPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Tan<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Tan(x.X), TScalar.Tan(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> TanPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.TanPi(x.X), TScalar.TanPi(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (Vector2F<TScalar> Sin, Vector2F<TScalar> Cos) SinCos<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => (
        new(TScalar.Sin(x.X), TScalar.Sin(x.Y)),
        new(TScalar.Cos(x.X), TScalar.Cos(x.Y))
    );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (Vector2F<TScalar> SinPi, Vector2F<TScalar> CosPi) SinCosPi<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => (
        new(TScalar.SinPi(x.X), TScalar.SinPi(x.Y)),
        new(TScalar.CosPi(x.X), TScalar.CosPi(x.Y))
    );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log(x.X), TScalar.Log(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> newBase) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log(x.X, newBase.X), TScalar.Log(x.Y, newBase.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log<TScalar>(Vector2F<TScalar> x, TScalar newBase) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log(x.X, newBase), TScalar.Log(x.Y, newBase));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> LogP1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.LogP1(x.X), TScalar.LogP1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private static TScalar Log2ILogarithmicFunctions<TScalar>(TScalar x) where TScalar : ILogarithmicFunctions<TScalar> => TScalar.Log2(x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log2<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(Log2ILogarithmicFunctions(x.X), Log2ILogarithmicFunctions(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log2P1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log2P1(x.X), TScalar.Log2P1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log10<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log10(x.X), TScalar.Log10(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Log10P1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Log10P1(x.X), TScalar.Log10P1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Exp<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Exp(x.X), TScalar.Exp(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> ExpM1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.ExpM1(x.X), TScalar.ExpM1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Exp2<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Exp2(x.X), TScalar.Exp2(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Exp2M1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Exp2M1(x.X), TScalar.Exp2M1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Exp10<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Exp10(x.X), TScalar.Exp10(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Exp10M1<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Exp10M1(x.X), TScalar.Exp10M1(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Pow<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Pow(x.X, y.X), TScalar.Pow(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Pow<TScalar>(Vector2F<TScalar> x, TScalar y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Pow(x.X, y), TScalar.Pow(x.Y, y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Cbrt<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Cbrt(x.X), TScalar.Cbrt(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Hypot<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Hypot(x.X, y.X), TScalar.Hypot(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Hypot<TScalar>(Vector2F<TScalar> x, TScalar y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Hypot(x.X, y), TScalar.Hypot(x.Y, y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> RootN<TScalar>(Vector2F<TScalar> x, int n) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.RootN(x.X, n), TScalar.RootN(x.Y, n));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Round<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Round(x.X), TScalar.Round(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Round<TScalar>(Vector2F<TScalar> x, int digits) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Round(x.X, digits), TScalar.Round(x.Y, digits));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Round<TScalar>(Vector2F<TScalar> x, MidpointRounding mode) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Round(x.X, mode), TScalar.Round(x.Y, mode));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Round<TScalar>(Vector2F<TScalar> x, int digits, MidpointRounding mode) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Round(x.X, digits, mode), TScalar.Round(x.Y, digits, mode));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Truncate<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Truncate(x.X), TScalar.Truncate(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atan2<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atan2(x.X, y.X), TScalar.Atan2(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atan2Pi<TScalar>(Vector2F<TScalar> x, Vector2F<TScalar> y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atan2Pi(x.X, y.X), TScalar.Atan2Pi(x.Y, y.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atan2<TScalar>(Vector2F<TScalar> x, TScalar y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atan2(x.X, y), TScalar.Atan2(x.Y, y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> Atan2Pi<TScalar>(Vector2F<TScalar> x, TScalar y) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.Atan2Pi(x.X, y), TScalar.Atan2Pi(x.Y, y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> BitDecrement<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.BitDecrement(x.X), TScalar.BitDecrement(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> BitIncrement<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.BitIncrement(x.X), TScalar.BitIncrement(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> FusedMultiplyAdd<TScalar>(Vector2F<TScalar> left, Vector2F<TScalar> right, Vector2F<TScalar> addend) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.FusedMultiplyAdd(left.X, right.X, addend.X), TScalar.FusedMultiplyAdd(left.Y, right.Y, addend.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> ReciprocalEstimate<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.ReciprocalEstimate(x.X), TScalar.ReciprocalEstimate(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> ReciprocalSqrtEstimate<TScalar>(Vector2F<TScalar> x) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.ReciprocalSqrtEstimate(x.X), TScalar.ReciprocalSqrtEstimate(x.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> ScaleB<TScalar>(Vector2F<TScalar> x, Vector2D<int> n) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.ScaleB(x.X, n.X), TScalar.ScaleB(x.Y, n.Y));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2F<TScalar> ScaleB<TScalar>(Vector2F<TScalar> x, int n) where TScalar : IBinaryFloatingPointIeee754<TScalar> => new(TScalar.ScaleB(x.X, n), TScalar.ScaleB(x.Y, n));
}