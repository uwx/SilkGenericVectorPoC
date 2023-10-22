using System.Numerics;
using System.Runtime.CompilerServices;

namespace GenericVector;

public static partial class Vector3D
{
    #region Extension

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Length<T, TReturn>(this Vector3D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var lengthSquared = vec.LengthSquared<T, TReturn>();
        return TReturn.Sqrt(lengthSquared);
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T,TReturn}" /> method.</remarks>
    /// <altmember cref="Length{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn LengthSquared<T, TReturn>(this Vector3D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
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
    public static Vector3D<T> Multiply<T>(Vector3D<T> left, Vector3D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Multiply<T>(Vector3D<T> left, T right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Multiply<T>(T left, Vector3D<T> right) where T : INumberBase<T>
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Negate<T>(Vector3D<T> value) where T : INumberBase<T>
    {
        return -value;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Subtract<T>(Vector3D<T> left, Vector3D<T> right) where T : INumberBase<T>
    {
        return left - right;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Add<T>(Vector3D<T> left, Vector3D<T> right) where T : INumberBase<T>
    {
        return left + right;
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Divide<T>(Vector3D<T> left, Vector3D<T> right) where T : INumberBase<T>
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Divide<T>(Vector3D<T> left, T divisor) where T : INumberBase<T>
    {
        return left / divisor;
    }

    #endregion

    #region Other
    
    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Abs<T>(Vector3D<T> value) where T : INumberBase<T>
    {
        return new Vector3D<T>(
            T.Abs(value.X),
            T.Abs(value.Y),
            T.Abs(value.Z)
        );
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Clamp<T>(Vector3D<T> value1, Vector3D<T> min, Vector3D<T> max) where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return Min(Max(value1, min), max);
    }

    /// <summary>Computes the cross product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Cross<T>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T>
    {
        return new Vector3D<T>(
            (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
            (vector1.Z * vector2.X) - (vector1.X * vector2.Z),
            (vector1.X * vector2.Y) - (vector1.Y * vector2.X)
        );
    }

    /// <summary>Computes the cross product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Cross<T, TIntermediate>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T> where TIntermediate : INumberBase<TIntermediate>
    {
        return new Vector3D<T>(
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.Z)) - (TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.Y))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.X)) - (TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Z))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Y)) - (TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.X)))
        );
    }
    
    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Distance<T, TReturn>(Vector3D<T> value1, Vector3D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var distanceSquared = DistanceSquared<T, TReturn>(value1, value2);
        return TReturn.Sqrt(distanceSquared);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(Vector3D<T> value1, Vector3D<T> value2) where T : INumberBase<T>
    {
        var difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn DistanceSquared<T, TReturn>(Vector3D<T> value1, Vector3D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var difference = value1 - value2;
        return Dot<T, TReturn>(difference, difference);
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T>
    {
        return (vector1.X * vector2.X)
             + (vector1.Y * vector2.Y)
             + (vector1.Z * vector2.Z);
    }
    
    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Dot<T, TReturn>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        return (TReturn.CreateTruncating(vector1.X) * TReturn.CreateTruncating(vector2.X))
               + (TReturn.CreateTruncating(vector1.Y) * TReturn.CreateTruncating(vector2.Y))
               + (TReturn.CreateTruncating(vector1.Z) * TReturn.CreateTruncating(vector2.Z));
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TFloat> Lerp<T, TFloat>(Vector3D<T> value1, Vector3D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (TFloat.One - amount)) + (value2.As<TFloat>() * amount);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TFloat> LerpClamped<T, TFloat>(Vector3D<T> value1, Vector3D<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = TFloat.Clamp(amount, TFloat.Zero, TFloat.One);
        return Lerp(value1, value2, amount);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Max<T>(Vector3D<T> value1, Vector3D<T> value2) where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        return new Vector3D<T>( // using T.Max here would add an IsNaN check
            (value1.X > value2.X) ? value1.X : value2.X,
            (value1.Y > value2.Y) ? value1.Y : value2.Y,
            (value1.Z > value2.Z) ? value1.Z : value2.Z
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Min<T>(Vector3D<T> value1, Vector3D<T> value2) where T : INumberBase<T>, IComparisonOperators<T, T, bool>
    {
        return new Vector3D<T>( // using T.Min here would add an IsNaN check
            (value1.X < value2.X) ? value1.X : value2.X,
            (value1.Y < value2.Y) ? value1.Y : value2.Y,
            (value1.Z < value2.Z) ? value1.Z : value2.Z
        );
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TReturn> Normalize<T, TReturn>(Vector3D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return value.As<TReturn>() / value.Length<T, TReturn>();
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Normalize<T>(Vector3D<T> value) where T : INumberBase<T>, IRootFunctions<T>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TReturn> Reflect<T, TReturn>(Vector3D<T> vector, Vector3D<T> normal) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var dot = Dot<T, TReturn>(vector, normal);
        return vector.As<TReturn>() - (NumericConstants<TReturn>.Two * (dot * normal.As<TReturn>()));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TReturn> Sqrt<T, TReturn>(Vector3D<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return new Vector3D<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z))
        );
    }

    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Transform<T>(Vector3D<T> position, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        return (Vector3D<T>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TReturn> Transform<T, TQuat, TReturn>(Vector3D<T> value, Quaternion<TQuat> rotation) where T : INumberBase<T> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        var x2 = rotation.X + rotation.X;
        var y2 = rotation.Y + rotation.Y;
        var z2 = rotation.Z + rotation.Z;

        var wx2 = TReturn.CreateTruncating(rotation.W * x2);
        var wy2 = TReturn.CreateTruncating(rotation.W * y2);
        var wz2 = TReturn.CreateTruncating(rotation.W * z2);
        var xx2 = TReturn.CreateTruncating(rotation.X * x2);
        var xy2 = TReturn.CreateTruncating(rotation.X * y2);
        var xz2 = TReturn.CreateTruncating(rotation.X * z2);
        var yy2 = TReturn.CreateTruncating(rotation.Y * y2);
        var yz2 = TReturn.CreateTruncating(rotation.Y * z2);
        var zz2 = TReturn.CreateTruncating(rotation.Z * z2);

        return new Vector3D<TReturn>(
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
    // internal static Vector3D<T> TransformNormal<T>(Vector3D<T> normal, in Matrix4x4 matrix) where T : INumberBase<T>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]    
    public static Vector3D<T> Remainder<T>(this Vector3D<T> left, Vector3D<T> right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector3D<T>(
            left.X % right.X,
            left.Y % right.Y,
            left.Z % right.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Remainder<T>(this Vector3D<T> left, T right) where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return new Vector3D<T>(
            left.X % right,
            left.Y % right,
            left.Z % right
        );
    }
    #endregion

    #region Specializations
    
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Vector3D<T> vec) where T : INumberBase<T>, IRootFunctions<T>
    {
        return vec.Length<T, T>();
    }
    
    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T}" /> method.</remarks>
    /// <altmember cref="Length{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Vector3D<T> vec) where T : INumberBase<T>
    {
        return vec.LengthSquared<T, T>();
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Lerp<T>(Vector3D<T> value1, Vector3D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return Lerp<T, T>(value1, value2, amount);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> LerpClamped<T>(Vector3D<T> value1, Vector3D<T> value2, T amount) where T : INumberBase<T>, IFloatingPoint<T>
    {
        return LerpClamped<T, T>(value1, value2, amount);
    }
    
    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Reflect<T>(Vector3D<T> vector, Vector3D<T> normal) where T : IFloatingPoint<T>
    {
        return Reflect<T, T>(vector, normal);
    }
    
    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Sqrt<T>(Vector3D<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>
    {
        return Sqrt<T, T>(value);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Transform<T>(Vector3D<T> value, Quaternion<T> rotation)
        where T : IFloatingPoint<T>, ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        return Transform<T, T, T>(value, rotation);
    }
    

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Transform<T, TQuat>(Vector3D<T> value, Quaternion<TQuat> rotation)
        where T : IFloatingPoint<T>
        where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
    {
        return Transform<T, TQuat, T>(value, rotation);
    }
    #endregion
    
    // Equivalent implementing IHyperbolicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Acosh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Acosh(x.X), T.Acosh(x.Y), T.Acosh(x.Z));
    public static Vector3D<T> Asinh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Asinh(x.X), T.Asinh(x.Y), T.Asinh(x.Z));
    public static Vector3D<T> Atanh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Atanh(x.X), T.Atanh(x.Y), T.Atanh(x.Z));
    public static Vector3D<T> Cosh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Cosh(x.X), T.Cosh(x.Y), T.Cosh(x.Z));
    public static Vector3D<T> Sinh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Sinh(x.X), T.Sinh(x.Y), T.Sinh(x.Z));
    public static Vector3D<T> Tanh<T>(Vector3D<T> x) where T : IHyperbolicFunctions<T> => new(T.Tanh(x.X), T.Tanh(x.Y), T.Tanh(x.Z));
    
    // Equivalent implementing ITrigonometricFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Acos<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Acos(x.X), T.Acos(x.Y), T.Acos(x.Z));
    public static Vector3D<T> AcosPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.AcosPi(x.X), T.AcosPi(x.Y), T.AcosPi(x.Z));
    public static Vector3D<T> Asin<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Asin(x.X), T.Asin(x.Y), T.Asin(x.Z));
    public static Vector3D<T> AsinPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.AsinPi(x.X), T.AsinPi(x.Y), T.AsinPi(x.Z));
    public static Vector3D<T> Atan<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Atan(x.X), T.Atan(x.Y), T.Atan(x.Z));
    public static Vector3D<T> AtanPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.AtanPi(x.X), T.AtanPi(x.Y), T.AtanPi(x.Z));
    public static Vector3D<T> Cos<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Cos(x.X), T.Cos(x.Y), T.Cos(x.Z));
    public static Vector3D<T> CosPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.CosPi(x.X), T.CosPi(x.Y), T.CosPi(x.Z));
    public static Vector3D<T> DegreesToRadians<T>(Vector3D<T> degrees) where T : ITrigonometricFunctions<T> => new(T.DegreesToRadians(degrees.X), T.DegreesToRadians(degrees.Y), T.DegreesToRadians(degrees.Z));
    public static Vector3D<T> RadiansToDegrees<T>(Vector3D<T> radians) where T : ITrigonometricFunctions<T> => new(T.RadiansToDegrees(radians.X), T.RadiansToDegrees(radians.Y), T.RadiansToDegrees(radians.Z));
    public static Vector3D<T> Sin<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Sin(x.X), T.Sin(x.Y), T.Sin(x.Z));
    public static Vector3D<T> SinPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.SinPi(x.X), T.SinPi(x.Y), T.SinPi(x.Z));
    public static Vector3D<T> Tan<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.Tan(x.X), T.Tan(x.Y), T.Tan(x.Z));
    public static Vector3D<T> TanPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T> => new(T.TanPi(x.X), T.TanPi(x.Y), T.TanPi(x.Z));


    public static (Vector3D<T> Sin, Vector3D<T> Cos) SinCos<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCos(x.X);
        var (sinY, cosY) = T.SinCos(x.Y);
        var (sinZ, cosZ) = T.SinCos(x.Z);

        return (new Vector3D<T>(sinX, sinY, sinZ), new Vector3D<T>(cosX, cosY, cosZ));
    }

    public static (Vector3D<T> SinPi, Vector3D<T> CosPi) SinCosPi<T>(Vector3D<T> x) where T : ITrigonometricFunctions<T>
    {
        var (sinX, cosX) = T.SinCosPi(x.X);
        var (sinY, cosY) = T.SinCosPi(x.Y);
        var (sinZ, cosZ) = T.SinCosPi(x.Z);

        return (new Vector3D<T>(sinX, sinY, sinZ), new Vector3D<T>(cosX, cosY, cosZ));
    }

    // Equivalent implementing ILogarithmicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Log<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log(x.X), T.Log(x.Y), T.Log(x.Z));
    public static Vector3D<T> Log<T>(Vector3D<T> x, Vector3D<T> newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase.X), T.Log(x.Y, newBase.Y), T.Log(x.Z, newBase.Z));
    public static Vector3D<T> Log<T>(Vector3D<T> x, T newBase) where T : ILogarithmicFunctions<T> => new(T.Log(x.X, newBase), T.Log(x.Y, newBase), T.Log(x.Z, newBase));
    public static Vector3D<T> LogP1<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => Log(x + Vector3D<T>.One);
    public static Vector3D<T> Log2<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log2(x.X), T.Log2(x.Y), T.Log2(x.Z));
    public static Vector3D<T> Log2P1<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => Log2(x + Vector3D<T>.One);
    public static Vector3D<T> Log10<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => new(T.Log10(x.X), T.Log10(x.Y), T.Log10(x.Z));
    public static Vector3D<T> Log10P1<T>(Vector3D<T> x) where T : ILogarithmicFunctions<T> => Log10(x + Vector3D<T>.One);

    // Equivalent implementing IExponentialFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Exp<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => new(T.Exp(x.X), T.Exp(x.Y), T.Exp(x.Z));
    public static Vector3D<T> ExpM1<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => Exp(x) - Vector3D<T>.One;
    public static Vector3D<T> Exp2<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => new(T.Exp2(x.X), T.Exp2(x.Y), T.Exp2(x.Z));
    public static Vector3D<T> Exp2M1<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => Exp2(x) - Vector3D<T>.One;
    public static Vector3D<T> Exp10<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => new(T.Exp10(x.X), T.Exp10(x.Y), T.Exp10(x.Z));
    public static Vector3D<T> Exp10M1<T>(Vector3D<T> x) where T : IExponentialFunctions<T> => Exp10(x) - Vector3D<T>.One;

    // Equivalent implementing IPowerFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Pow<T>(Vector3D<T> x, Vector3D<T> y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y.X), T.Pow(x.Y, y.Y), T.Pow(x.Z, y.Z));
    public static Vector3D<T> Pow<T>(Vector3D<T> x, T y) where T : IPowerFunctions<T> => new(T.Pow(x.X, y), T.Pow(x.Y, y), T.Pow(x.Z, y));

    // Equivalent implementing IRootFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3D<T> Cbrt<T>(Vector3D<T> x) where T : IRootFunctions<T> => new(T.Cbrt(x.X), T.Cbrt(x.Y), T.Cbrt(x.Z));
    public static Vector3D<T> Hypot<T>(Vector3D<T> x, Vector3D<T> y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y.X), T.Hypot(x.Y, y.Y), T.Hypot(x.Z, y.Z));
    public static Vector3D<T> Hypot<T>(Vector3D<T> x, T y) where T : IRootFunctions<T> => new(T.Hypot(x.X, y), T.Hypot(x.Y, y), T.Hypot(x.Z, y));
    public static Vector3D<T> RootN<T>(Vector3D<T> x, int n) where T : IRootFunctions<T> => new(T.RootN(x.X, n), T.RootN(x.Y, n), T.RootN(x.Z, n));
    
    // IFloatingPoint<TSelf>
    public static Vector3D<T> Round<T>(Vector3D<T> x) where T : IFloatingPoint<T> => new(T.Round(x.X), T.Round(x.Y), T.Round(x.Z));
    public static Vector3D<T> Round<T>(Vector3D<T> x, int digits) where T : IFloatingPoint<T> => new(T.Round(x.X, digits), T.Round(x.Y, digits), T.Round(x.Z, digits));
    public static Vector3D<T> Round<T>(Vector3D<T> x, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, mode), T.Round(x.Y, mode), T.Round(x.Z, mode));
    public static Vector3D<T> Round<T>(Vector3D<T> x, int digits, MidpointRounding mode) where T : IFloatingPoint<T> => new(T.Round(x.X, digits, mode), T.Round(x.Y, digits, mode), T.Round(x.Z, digits, mode));
    public static Vector3D<T> Truncate<T>(Vector3D<T> x) where T : IFloatingPoint<T> => new(T.Truncate(x.X), T.Truncate(x.Y), T.Truncate(x.Z));

    // IFloatingPointIeee754<TSelf>
    public static Vector3D<T> Atan2<T>(Vector3D<T> x, Vector3D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y.X), T.Atan2(x.Y, y.Y), T.Atan2(x.Z, y.Z));
    public static Vector3D<T> Atan2Pi<T>(Vector3D<T> x, Vector3D<T> y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y.X), T.Atan2Pi(x.Y, y.Y), T.Atan2Pi(x.Z, y.Z));
    public static Vector3D<T> Atan2<T>(Vector3D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2(x.X, y), T.Atan2(x.Y, y), T.Atan2(x.Z, y));
    public static Vector3D<T> Atan2Pi<T>(Vector3D<T> x, T y) where T : IFloatingPointIeee754<T> => new(T.Atan2Pi(x.X, y), T.Atan2Pi(x.Y, y), T.Atan2Pi(x.Z, y));
    public static Vector3D<T> BitDecrement<T>(Vector3D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitDecrement(x.X), T.BitDecrement(x.Y), T.BitDecrement(x.Z));
    public static Vector3D<T> BitIncrement<T>(Vector3D<T> x) where T : IFloatingPointIeee754<T> => new(T.BitIncrement(x.X), T.BitIncrement(x.Y), T.BitIncrement(x.Z));
    public static Vector3D<T> FusedMultiplyAdd<T>(Vector3D<T> left, Vector3D<T> right, Vector3D<T> addend) where T : IFloatingPointIeee754<T> => new(T.FusedMultiplyAdd(left.X, right.X, addend.X), T.FusedMultiplyAdd(left.Y, right.Y, addend.Y), T.FusedMultiplyAdd(left.Z, right.Z, addend.Z));
    public static Vector3D<T> Lerp<T>(Vector3D<T> value1, Vector3D<T> value2, Vector3D<T> amount) where T : IFloatingPointIeee754<T> => new(T.Lerp(value1.X, value2.X, amount.X), T.Lerp(value1.Y, value2.Y, amount.Y), T.Lerp(value1.Z, value2.Z, amount.Z));
    public static Vector3D<T> ReciprocalEstimate<T>(Vector3D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalEstimate(x.X), T.ReciprocalEstimate(x.Y), T.ReciprocalEstimate(x.Z));
    public static Vector3D<T> ReciprocalSqrtEstimate<T>(Vector3D<T> x) where T : IFloatingPointIeee754<T> => new(T.ReciprocalSqrtEstimate(x.X), T.ReciprocalSqrtEstimate(x.Y), T.ReciprocalSqrtEstimate(x.Z));
    
    // INumber<T>
    // public static Vector3D<T> Clamp<T>(Vector3D<T> value, Vector3D<T> min, Vector3D<T> max) where T : INumber<T> => new(T.Clamp(x.X), T.Clamp(x.Y), T.Clamp(x.Z));
    public static Vector3D<T> CopySign<T>(Vector3D<T> value, Vector3D<T> sign) where T : INumber<T> => new(T.CopySign(value.X, sign.X), T.CopySign(value.Y, sign.Y), T.CopySign(value.Z, sign.Z));
    public static Vector3D<T> CopySign<T>(Vector3D<T> value, T sign) where T : INumber<T> => new(T.CopySign(value.X, sign), T.CopySign(value.Y, sign), T.CopySign(value.Z, sign));
    public static Vector3D<T> MaxNumber<T>(Vector3D<T> x, Vector3D<T> y) where T : INumber<T> => new(T.MaxNumber(x.X, y.X), T.MaxNumber(x.Y, y.Y), T.MaxNumber(x.Z, y.Z));
    public static Vector3D<T> MinNumber<T>(Vector3D<T> x, Vector3D<T> y) where T : INumber<T> => new(T.MinNumber(x.X, y.X), T.MinNumber(x.Y, y.Y), T.MinNumber(x.Z, y.Z));

    // INumberBase<T>
    // public static Vector3D<T> MaxMagnitude<T>(Vector3D<T> x, Vector3D<T> y) where T : INumberBase<T> => new(T.MaxMagnitude(x.X, y.X), T.MaxMagnitude(x.Y, y.Y), T.MaxMagnitude(x.Z, y.Z));
    // public static Vector3D<T> MaxMagnitudeNumber<T>(Vector3D<T> x, Vector3D<T> y) where T : INumberBase<T> => new(T.MaxMagnitudeNumber(x.X, y.X), T.MaxMagnitudeNumber(x.Y, y.Y), T.MaxMagnitudeNumber(x.Z, y.Z));
    // public static Vector3D<T> MinMagnitude<T>(Vector3D<T> x, Vector3D<T> y) where T : INumberBase<T> => new(T.MinMagnitude(x.X, y.X), T.MinMagnitude(x.Y, y.Y), T.MinMagnitude(x.Z, y.Z));
    // public static Vector3D<T> MinMagnitudeNumber<T>(Vector3D<T> x, Vector3D<T> y) where T : INumberBase<T> => new(T.MinMagnitudeNumber(x.X, y.X), T.MinMagnitudeNumber(x.Y, y.Y), T.MinMagnitudeNumber(x.Z, y.Z));
    // (there's no reason you would want these.)

    // IFloatingPointIeee754<TSelf>
    public static Vector3D<int> ILogB<T>(Vector3D<T> x) where T : IFloatingPointIeee754<T> => new(T.ILogB(x.X), T.ILogB(x.Y), T.ILogB(x.Z));
    public static Vector3D<T> ScaleB<T>(Vector3D<T> x, Vector3D<int> n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n.X), T.ScaleB(x.Y, n.Y), T.ScaleB(x.Z, n.Z));
    public static Vector3D<T> ScaleB<T>(Vector3D<T> x, int n) where T : IFloatingPointIeee754<T> => new(T.ScaleB(x.X, n), T.ScaleB(x.Y, n), T.ScaleB(x.Z, n));
    
    public static Vector3D<int> RoundToInt<T>(Vector3D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3D<int>(
            int.CreateSaturating(T.Round(vector.X)),
            int.CreateSaturating(T.Round(vector.Y)),
            int.CreateSaturating(T.Round(vector.Z))
        );
    }
    
    public static Vector3D<int> FloorToInt<T>(Vector3D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3D<int>(
            int.CreateSaturating(T.Floor(vector.X)),
            int.CreateSaturating(T.Floor(vector.Y)),
            int.CreateSaturating(T.Floor(vector.Z))
        );
    }
    
    public static Vector3D<int> CeilingToInt<T>(Vector3D<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3D<int>(
            int.CreateSaturating(T.Ceiling(vector.X)),
            int.CreateSaturating(T.Ceiling(vector.Y)),
            int.CreateSaturating(T.Ceiling(vector.Z))
        );
    }

    public static Vector3D<TInt> RoundToInt<T, TInt>(Vector3D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector3D<TInt>(
            TInt.CreateSaturating(T.Round(vector.X)),
            TInt.CreateSaturating(T.Round(vector.Y)),
            TInt.CreateSaturating(T.Round(vector.Z))
        );
    }
    
    public static Vector3D<TInt> FloorToInt<T, TInt>(Vector3D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector3D<TInt>(
            TInt.CreateSaturating(T.Floor(vector.X)),
            TInt.CreateSaturating(T.Floor(vector.Y)),
            TInt.CreateSaturating(T.Floor(vector.Z))
        );
    }
    
    public static Vector3D<TInt> CeilingToInt<T, TInt>(Vector3D<T> vector) where T : IFloatingPoint<T> where TInt : IBinaryInteger<TInt>
    {
        return new Vector3D<TInt>(
            TInt.CreateSaturating(T.Ceiling(vector.X)),
            TInt.CreateSaturating(T.Ceiling(vector.Y)),
            TInt.CreateSaturating(T.Ceiling(vector.Z))
        );
    }

    public static Vector3D<float> AsGeneric(this Vector3 vector)
        => Unsafe.BitCast<Vector3, Vector3D<float>>(vector);

    public static Vector3 AsNumerics(this Vector3D<float> vector)
        => Unsafe.BitCast<Vector3D<float>, Vector3>(vector);
}