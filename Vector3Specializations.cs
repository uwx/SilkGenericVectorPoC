using System.Numerics;
using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
using BitCaster = System.Runtime.CompilerServices.Unsafe;
#else
using BitCaster = GenericVector.Vector3;
#endif

namespace GenericVector;

public static class Vector3
{
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Vector3<T> vec) where T : INumber<T>, IFloatingPoint<T>, IRootFunctions<T>
    {
        return vec.Length<T>();
    }
    
    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length{T}" /> method.</remarks>
    /// <altmember cref="Length{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Vector3<T> vec) where T : INumber<T>, IFloatingPoint<T>
    {
        return vec.LengthSquared<T>();
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Lerp<T>(Vector3<T> value1, Vector3<T> value2, T amount) where T : INumber<T>, IFloatingPoint<T>
    {
        return Vector3<T>.Lerp(value1, value2, amount);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> LerpClamped<T>(Vector3<T> value1, Vector3<T> value2, T amount) where T : INumber<T>, IFloatingPoint<T>
    {
        return Vector3<T>.LerpClamped<T>(value1, value2, amount);
    }
    
    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Normalize<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>
    {
        return Vector3<T>.Normalize<T>(value);
    }
    
    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Reflect<T>(Vector3<T> vector, Vector3<T> normal) where T : IFloatingPoint<T>
    {
        return Vector3<T>.Reflect<T>(vector, normal);
    }
    
    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> SquareRoot<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>
    {
        return Vector3<T>.SquareRoot<T>(value);
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Transform<T>(Vector3<T> value, Quaternion rotation) where T : IFloatingPoint<T>
    {
        return Vector3<T>.Transform<T>(value, rotation);
    }
    
    // Equivalent implementing IHyperbolicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Acosh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Acosh(x.X), T.Acosh(x.Y), T.Acosh(x.Z));
    public static Vector3<T> Asinh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Asinh(x.X), T.Asinh(x.Y), T.Asinh(x.Z));
    public static Vector3<T> Atanh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Atanh(x.X), T.Atanh(x.Y), T.Atanh(x.Z));
    public static Vector3<T> Cosh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Cosh(x.X), T.Cosh(x.Y), T.Cosh(x.Z));
    public static Vector3<T> Sinh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Sinh(x.X), T.Sinh(x.Y), T.Sinh(x.Z));
    public static Vector3<T> Tanh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Tanh(x.X), T.Tanh(x.Y), T.Tanh(x.Z));
    
    // Equivalent implementing ITrigonometricFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Acos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Acos(x.X), T.Acos(x.Y), T.Acos(x.Z));
    public static Vector3<T> AcosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.AcosPi(x.X), T.AcosPi(x.Y), T.AcosPi(x.Z));
    public static Vector3<T> Asin<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Asin(x.X), T.Asin(x.Y), T.Asin(x.Z));
    public static Vector3<T> AsinPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.AsinPi(x.X), T.AsinPi(x.Y), T.AsinPi(x.Z));
    public static Vector3<T> Atan<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Atan(x.X), T.Atan(x.Y), T.Atan(x.Z));
    public static Vector3<T> AtanPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.AtanPi(x.X), T.AtanPi(x.Y), T.AtanPi(x.Z));
    public static Vector3<T> Cos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Cos(x.X), T.Cos(x.Y), T.Cos(x.Z));
    public static Vector3<T> CosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.CosPi(x.X), T.CosPi(x.Y), T.CosPi(x.Z));
#if NET8_0_OR_GREATER
    public static Vector3<T> DegreesToRadians<T>(Vector3<T> degrees) where T : INumber<T>, ITrigonometricFunctions<T> => new(T.DegreesToRadians(degrees.X), T.DegreesToRadians(degrees.Y), T.DegreesToRadians(degrees.Z));
    public static Vector3<T> RadiansToDegrees<T>(Vector3<T> radians) where T : INumber<T>, ITrigonometricFunctions<T> => new(T.RadiansToDegrees(radians.X), T.RadiansToDegrees(radians.Y), T.RadiansToDegrees(radians.Z));
#endif
    public static Vector3<T> Sin<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Sin(x.X), T.Sin(x.Y), T.Sin(x.Z));
    public static Vector3<T> SinPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.SinPi(x.X), T.SinPi(x.Y), T.SinPi(x.Z));
    public static Vector3<T> Tan<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Tan(x.X), T.Tan(x.Y), T.Tan(x.Z));
    public static Vector3<T> TanPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.TanPi(x.X), T.TanPi(x.Y), T.TanPi(x.Z));


    public static (Vector3<T> Sin, Vector3<T> Cos) SinCos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>
    {
        var (sinX, cosX) = T.SinCos(x.X);
        var (sinY, cosY) = T.SinCos(x.Y);
        var (sinZ, cosZ) = T.SinCos(x.Z);

        return (new Vector3<T>(sinX, sinY, sinZ), new Vector3<T>(cosX, cosY, cosZ));
    }

    public static (Vector3<T> SinPi, Vector3<T> CosPi) SinCosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>
    {
        var (sinX, cosX) = T.SinCosPi(x.X);
        var (sinY, cosY) = T.SinCosPi(x.Y);
        var (sinZ, cosZ) = T.SinCosPi(x.Z);

        return (new Vector3<T>(sinX, sinY, sinZ), new Vector3<T>(cosX, cosY, cosZ));
    }

    // Equivalent implementing ILogarithmicFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Log<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Log(x.X), T.Log(x.Y), T.Log(x.Z));
    public static Vector3<T> Log<T>(Vector3<T> x, Vector3<T> newBase) where T : IFloatingPointIeee754<T> => new(T.Log(x.X, newBase.X), T.Log(x.Y, newBase.Y), T.Log(x.Z, newBase.Z));
    public static Vector3<T> LogP1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Log(x + Vector3<T>.One);
    public static Vector3<T> Log2<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Log2(x.X), T.Log2(x.Y), T.Log2(x.Z));
    public static Vector3<T> Log2P1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Log2(x + Vector3<T>.One);
    public static Vector3<T> Log10<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Log10(x.X), T.Log10(x.Y), T.Log10(x.Z));
    public static Vector3<T> Log10P1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Log10(x + Vector3<T>.One);

    // Equivalent implementing IExponentialFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Exp<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Exp(x.X), T.Exp(x.Y), T.Exp(x.Z));
    public static Vector3<T> ExpM1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Exp(x) - Vector3<T>.One;
    public static Vector3<T> Exp2<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Exp2(x.X), T.Exp2(x.Y), T.Exp2(x.Z));
    public static Vector3<T> Exp2M1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Exp2(x) - Vector3<T>.One;
    public static Vector3<T> Exp10<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Exp10(x.X), T.Exp10(x.Y), T.Exp10(x.Z));
    public static Vector3<T> Exp10M1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => Exp10(x) - Vector3<T>.One;

    // Equivalent implementing IPowerFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Pow<T>(Vector3<T> x, Vector3<T> y) where T : IFloatingPointIeee754<T> => new(T.Pow(x.X, y.X), T.Pow(x.Y, y.Y), T.Pow(x.Z, y.Z));

    // Equivalent implementing IRootFunctions<System.Runtime.Intrinsics.Vector3>
    public static Vector3<T> Cbrt<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Cbrt(x.X), T.Cbrt(x.Y), T.Cbrt(x.Z));
    public static Vector3<T> Hypot<T>(Vector3<T> x, Vector3<T> y) where T : IFloatingPointIeee754<T> => new(T.Hypot(x.X, y.X), T.Hypot(x.Y, y.Y), T.Hypot(x.Z, y.Z));
    public static Vector3<T> RootN<T>(Vector3<T> x, int n) where T : IFloatingPointIeee754<T> => new(T.RootN(x.X, n), T.RootN(x.Y, n), T.RootN(x.Z, n));
    public static Vector3<T> Sqrt<T>(Vector3<T> x) where T : IFloatingPointIeee754<T> => new(T.Sqrt(x.X), T.Sqrt(x.Y), T.Sqrt(x.Z));

    public static Vector3<int> RoundToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3<int>(
            int.CreateSaturating(T.Round(vector.X)),
            int.CreateSaturating(T.Round(vector.Y)),
            int.CreateSaturating(T.Round(vector.Z))
        );
    }
    
    public static Vector3<int> FloorToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3<int>(
            int.CreateSaturating(T.Floor(vector.X)),
            int.CreateSaturating(T.Floor(vector.Y)),
            int.CreateSaturating(T.Floor(vector.Z))
        );
    }
    
    public static Vector3<int> CeilingToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>
    {
        return new Vector3<int>(
            int.CreateSaturating(T.Ceiling(vector.X)),
            int.CreateSaturating(T.Ceiling(vector.Y)),
            int.CreateSaturating(T.Ceiling(vector.Z))
        );
    }

    public static Vector3<float> AsGeneric(this System.Numerics.Vector3 vector) => BitCaster.BitCast<System.Numerics.Vector3, Vector3<float>>(vector);
    public static System.Numerics.Vector3 AsNumerics(this Vector3<float> vector) => BitCaster.BitCast<Vector3<float>, System.Numerics.Vector3>(vector);

#if !NET8_0
    internal static unsafe TTo BitCast<TFrom, TTo>(TFrom value)
    {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        if (sizeof(TFrom) != sizeof(TTo))
            throw new NotSupportedException();
        return *(TTo*)&value;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }
#endif
}