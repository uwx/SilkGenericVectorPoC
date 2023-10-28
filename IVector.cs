using System.Numerics;
using System.Runtime.CompilerServices;

namespace GenericVector;

public interface IVectorAlso<TVector, T> :
    IDivisionOperators<TVector, T, TVector>,
    IMultiplyOperators<TVector, T, TVector>
    where TVector : IVectorAlso<TVector, T>, IVector<TVector, T>
    where T : INumberBase<T>;

public interface IVector<TVector, T> :
    INumberBase<TVector>,
    IAdditionOperators<TVector, TVector, TVector>,
    IAdditiveIdentity<TVector, TVector>,
    IDecrementOperators<TVector>,
    IDivisionOperators<TVector, TVector, TVector>,
    IEqualityOperators<TVector, TVector, bool>,
    IIncrementOperators<TVector>,
    IMultiplicativeIdentity<TVector, TVector>,
    IMultiplyOperators<TVector, TVector, TVector>,
    ISpanFormattable,
    ISpanParsable<TVector>,
    ISubtractionOperators<TVector, TVector, TVector>,
    IUnaryPlusOperators<TVector, TVector>,
    IUnaryNegationOperators<TVector, TVector>,
    IUtf8SpanFormattable,
    IUtf8SpanParsable<TVector>,
    IEquatable<TVector>,
    IReadOnlyList<T>
    where TVector : IVector<TVector, T>
    where T : INumberBase<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool INumberBase<TVector>.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return TryFormat(utf8Destination, out bytesWritten, format, provider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return TryFormat(utf8Destination, out bytesWritten, format, provider);
    }

    /// <inheritdoc cref="IUtf8SpanFormattable.TryFormat"/>
    new bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract TVector Create(T scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract TVector Create(ReadOnlySpan<T> values);

    static TVector INumberBase<TVector>.Zero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.Zero;
    }

    static TVector INumberBase<TVector>.One
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.One;
    }

    /// <summary>Gets a vector whose elements are equal to zero.</summary>
    /// <value>Returns <typeparamref name="TVector"/>.</value>
    /// <value>A vector whose elements are equal to zero.</value>
    new static virtual TVector Zero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.Create(T.Zero);
    }

    /// <summary>Gets a vector whose elements are equal to one.</summary>
    /// <value>Returns <typeparamref name="TVector"/>.</value>
    /// <value>A vector whose elements are equal to one.</value>
    new static virtual TVector One
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.Create(T.One);
    }

    T IReadOnlyList<T>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
    }
    
    public new T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.AsSpan((TVector)this)[index];
    }
    
    static abstract TVector operator /(TVector left, T right);
    static abstract TVector operator *(TVector left, T right);
    static abstract TVector operator *(T left, TVector right);
    
    //T Length() /* where T : IRootFunctions<T> */;
    T LengthSquared();
    static abstract TVector Multiply(TVector left, TVector right);
    static abstract TVector Multiply(TVector left, T right);
    static abstract TVector Multiply(T left, TVector right);
    static abstract TVector Negate(TVector value);
    static abstract TVector Subtract(TVector left, TVector right);
    static abstract TVector Add(TVector left, TVector right);
    static abstract TVector Divide(TVector left, TVector right);
    static abstract TVector Divide(TVector left, T divisor);
    static abstract TVector Clamp(TVector value1, TVector min, TVector max);
    static abstract TReturn Distance<TReturn>(TVector value1, TVector value2) where TReturn : INumberBase<TReturn>, IRootFunctions<TReturn>;
    static abstract T DistanceSquared(TVector value1, TVector value2);
    static abstract TReturn DistanceSquared<TReturn>(TVector value1, TVector value2) where TReturn : INumberBase<TReturn>;
    static abstract T Dot(TVector vector1, TVector vector2);
    static abstract TReturn Dot<TReturn>(TVector vector1, TVector vector2) where TReturn : INumberBase<TReturn>;
    static abstract TVector Max(TVector value1, TVector value2);
    static abstract TVector Min(TVector value1, TVector value2);
    //static abstract TVector Normalize(TVector value) /* where T : IRootFunctions<T> */;
    //TVector Remainder(TVector right) /* where T : IModulusOperators<T, T, T> */;
    //TVector Remainder(T right) /* where T : IModulusOperators<T, T, T> */;
    static abstract TVector Lerp(TVector value1, TVector value2, T amount) /* where T : IFloatingPoint<T> */;
    static abstract TVector LerpClamped(TVector value1, TVector value2, T amount) /* where T : IFloatingPoint<T> */;
    static abstract TVector Lerp(TVector value1, TVector value2, TVector amount) /* where T : IFloatingPoint<T> */;
    static abstract TVector LerpClamped(TVector value1, TVector value2, TVector amount) /* where T : IFloatingPoint<T> */;
    static abstract TVector Reflect(TVector vector, TVector normal) /* where T : IFloatingPoint<T> */;
    //static abstract TVector Sqrt(TVector value) /* where T : IFloatingPoint<T>, IRootFunctions<T> */;

    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have enough elements to fit all scalars in this vector. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    static abstract void CopyTo(in TVector vector, T[] array);

    /// <summary>Copies the elements of the vector to a specified array starting at a specified index position.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <param name="index">The index at which to copy the first element of the vector.</param>
    /// <remarks><paramref name="array" /> must have a sufficient number of elements to accommodate the vector elements. In other words, elements <paramref name="index" /> through <paramref name="index" /> + 2 must already exist in <paramref name="array" />.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the array length.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    static abstract void CopyTo(in TVector vector, T[] array, int index);

    /// <summary>Copies the vector to the given <see cref="Span{T}" />. The length of the destination span must be at least enough to fit all scalars in this vector.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    static abstract void CopyTo(in TVector vector, Span<T> destination);

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least enough to fit all scalars in this vector.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    static abstract bool TryCopyTo(in TVector vector, Span<T> destination);
    
    static TVector IAdditiveIdentity<TVector, TVector>.AdditiveIdentity => TVector.Zero;
    static TVector IMultiplicativeIdentity<TVector, TVector>.MultiplicativeIdentity => TVector.One;

    static TVector IDecrementOperators<TVector>.operator --(TVector value) => value - TVector.One;
    static TVector IIncrementOperators<TVector>.operator ++(TVector value) => value + TVector.One;

    static TVector IUnaryPlusOperators<TVector, TVector>.operator +(TVector value) => value;
    
    static int INumberBase<TVector>.Radix => T.Radix;
    
    static abstract ReadOnlySpan<T> AsSpan(in TVector vec);
}