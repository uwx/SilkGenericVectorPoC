using System.Numerics;
using System.Runtime.CompilerServices;

namespace GenericVector.Experimental;

public static class Vector2f
{
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{TScalar}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TScalar Length<TScalar>(this Vector2i<TScalar> vec) where TScalar : IBinaryInteger<TScalar>, IRootFunctions<TScalar>
    {
        return vec.Length<TScalar, TScalar>();
    }
    
    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared{T,TReturn}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Length<T, TReturn>(this Vector2D<T> vec) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var lengthSquared = vec.LengthSquared<T, TReturn>();
        return TReturn.Sqrt(lengthSquared);
    }

    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Distance<T, TReturn>(Vector2D<T> value1, Vector2D<T> value2) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        var distanceSquared = DistanceSquared<T, TReturn>(value1, value2);
        return TReturn.Sqrt(distanceSquared);
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TFloat> Lerp<T, TFloat>(Vector2i<T> value1, Vector2i<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (TFloat.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2i<T> LerpUnchecked<T>(Vector2i<T> value1, Vector2i<T> value2, T amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (T.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TFloat> LerpClamped<T, TFloat>(Vector2i<T> value1, Vector2i<T> value2, TFloat amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = TFloat.Clamp(amount, TFloat.Zero, TFloat.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2i<T> LerpClampedUnchecked<T>(Vector2i<T> value1, Vector2i<T> value2, T amount) where T : INumberBase<T>
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
    public static Vector2i<TFloat> Lerp<T, TFloat>(Vector2i<T> value1, Vector2i<T> value2, Vector2i<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (Vector2i<TFloat>.One - amount)) + (value2.As<TFloat>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2i<T> LerpUnchecked<T>(Vector2i<T> value1, Vector2i<T> value2, Vector2i<T> amount) where T : INumberBase<T>
    {
        return (value1.As<T>() * (Vector2i<T>.One - amount)) + (value2.As<T>() * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TFloat> LerpClamped<T, TFloat>(Vector2i<T> value1, Vector2i<T> value2, Vector2i<TFloat> amount) where T : INumberBase<T> where TFloat : INumberBase<TFloat>, IFloatingPoint<TFloat>
    {
        amount = Clamp(amount, Vector2i<TFloat>.Zero, Vector2i<TFloat>.One);
        return Lerp(value1, value2, amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2i<T> LerpClampedUnchecked<T>(Vector2i<T> value1, Vector2i<T> value2, Vector2i<T> amount) where T : INumberBase<T>
    {
        amount = Clamp(amount, Vector2i<T>.Zero, Vector2i<T>.One);
        return LerpUnchecked(value1, value2, amount);
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TReturn> Normalize<T, TReturn>(Vector2i<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return value.As<TReturn>() / value.Length<T, TReturn>();
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<T> Normalize<T>(Vector2i<T> value) where T : INumberBase<T>, IRootFunctions<T>
    {
        return value / value.Length();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TReturn> Reflect<T, TReturn>(Vector2i<T> vector, Vector2i<T> normal) where T : INumberBase<T> where TReturn : INumberBase<TReturn>
    {
        var dot = Dot<T, TReturn>(vector, normal);
        return vector.As<TReturn>() - (NumericConstants<TReturn>.Two * (dot * normal.As<TReturn>()));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i<TReturn> Sqrt<T, TReturn>(Vector2i<T> value) where T : INumberBase<T> where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>
    {
        return new Vector2i<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)), 
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y))
        );
    }

}