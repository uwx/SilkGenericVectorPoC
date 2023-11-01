using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

public interface IVector
{
    /// <summary>
    /// The type of scalars in this vector type.
    /// </summary>
    protected static abstract Type ScalarType { get; }
    
    /// <summary>
    /// The amount of scalars in this vector type.
    /// </summary>
    protected internal static abstract int Count { get; }
}

public interface IVectorEquatable<TVector, TScalar>
    : IEquatable<TVector>
    where TVector : IVector<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    public TVector ScalarsEqual(TVector other);
}

public interface IVector<TVector, TScalar> :
    IVector,
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
    IReadOnlyList<TScalar>,
    IVectorEquatable<TVector, TScalar>
    where TVector : IVector<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static Type IVector.ScalarType => typeof(TScalar);

    static virtual TVector Zero => TVector.Create(TScalar.Zero);

    static virtual TVector One => TVector.Create(TScalar.One);

    int IReadOnlyCollection<TScalar>.Count => TVector.Count;

    TScalar IReadOnlyList<TScalar>.this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index];
    }
    
    public new TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.AsSpan((TVector)this)[index];
    }
    
    static abstract ReadOnlySpan<TScalar> AsSpan(TVector vec);

    static TVector IAdditiveIdentity<TVector, TVector>.AdditiveIdentity => TVector.Zero;
    static TVector IMultiplicativeIdentity<TVector, TVector>.MultiplicativeIdentity => TVector.One;

    static TVector IDecrementOperators<TVector>.operator --(TVector value) => value - TVector.One;
    static TVector IIncrementOperators<TVector>.operator ++(TVector value) => value + TVector.One;

    static TVector IDecrementOperators<TVector>.operator checked --(TVector value) => value - TVector.One;
    static TVector IIncrementOperators<TVector>.operator checked ++(TVector value) => value + TVector.One;

    static TVector IUnaryPlusOperators<TVector, TVector>.operator +(TVector value) => value;

    /// <summary>Divides two values together to compute their quotient.</summary>
    /// <param name="left">The value which <paramref name="right" /> divides.</param>
    /// <param name="right">The value which divides <paramref name="left" />.</param>
    /// <returns>The quotient of <paramref name="left" /> divided-by <paramref name="right" />.</returns>
    static abstract TVector operator /(TVector left, TScalar right);

    /// <summary>Divides two values together to compute their quotient.</summary>
    /// <param name="left">The value which <paramref name="right" /> divides.</param>
    /// <param name="right">The value which divides <paramref name="left" />.</param>
    /// <returns>The quotient of <paramref name="left" /> divided-by <paramref name="right" />.</returns>
    /// <exception cref="OverflowException">The quotient of <paramref name="left" /> divided-by <paramref name="right" /> is not representable by <typeparamref name="TVector" />.</exception>
    static virtual TVector operator checked /(TVector left, TScalar right) => left / right;

    /// <summary>Multiplies two values together to compute their product.</summary>
    /// <param name="left">The value which <paramref name="right" /> multiplies.</param>
    /// <param name="right">The value which multiplies <paramref name="left" />.</param>
    /// <returns>The product of <paramref name="left" /> multiplied-by <paramref name="right" />.</returns>
    static abstract TVector operator *(TVector left, TScalar right);

    /// <summary>Multiplies two values together to compute their product.</summary>
    /// <param name="left">The value which <paramref name="right" /> multiplies.</param>
    /// <param name="right">The value which multiplies <paramref name="left" />.</param>
    /// <returns>The product of <paramref name="left" /> multiplied-by <paramref name="right" />.</returns>
    /// <exception cref="OverflowException">The product of <paramref name="left" /> multiplied-by <paramref name="right" /> is not representable by <typeparamref name="TVector" />.</exception>
    static virtual TVector operator checked *(TVector left, TScalar right) => left * right;

    /// <summary>Multiplies two values together to compute their product.</summary>
    /// <param name="right">The value which <paramref name="left1" /> multiplies.</param>
    /// <param name="left1">The value which multiplies <paramref name="right" />.</param>
    /// <returns>The product of <paramref name="right" /> multiplied-by <paramref name="left1" />.</returns>
    static virtual TVector operator *(TScalar left1, TVector right) => right * left1;

    /// <summary>Multiplies two values together to compute their product.</summary>
    /// <param name="right">The value which <paramref name="left" /> multiplies.</param>
    /// <param name="left">The value which multiplies <paramref name="right" />.</param>
    /// <returns>The product of <paramref name="right" /> multiplied-by <paramref name="left" />.</returns>
    /// <exception cref="OverflowException">The product of <paramref name="right" /> multiplied-by <paramref name="left" /> is not representable by <typeparamref name="TVector" />.</exception>
    static virtual TVector operator checked *(TScalar left, TVector right) => right * left;

    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
    /// <param name="format">A standard or custom numeric format string that defines the format of individual elements.</param>
    /// <returns>The string representation of the current instance.</returns>
    /// <remarks>This method returns a string in which each element of the vector is formatted using <paramref name="format" /> and the current culture's formatting conventions. The "&lt;" and "&gt;" characters are used to begin and end the string, and the current culture's <see cref="NumberFormatInfo.NumberGroupSeparator" /> property followed by a space is used to separate each element.</remarks>
    /// <related type="Article" href="/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</related>
    /// <related type="Article" href="/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</related>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);

    static abstract TVector Create(TScalar scalar);
    static abstract TVector Create(ReadOnlySpan<TScalar> values);

    static abstract TVector GetUnitVector(uint dimension);

    TScalar LengthSquared();
    static virtual TVector Multiply(TVector left, TVector right) => left * right;
    static virtual TVector Multiply(TVector left, TScalar right) => left * right;
    static virtual TVector Multiply(TScalar left, TVector right) => left * right;
    static virtual TVector Negate(TVector value) => -value;
    static virtual TVector Subtract(TVector left, TVector right) => left - right;
    static virtual TVector Add(TVector left, TVector right) => left + right;
    static virtual TVector Divide(TVector left, TVector right) => left / right;
    static virtual TVector Divide(TVector left, TScalar divisor) => left / divisor;
    static virtual TVector Clamp(TVector value1, TScalar min, TScalar max) => TVector.Clamp(value1, TVector.Create(min), TVector.Create(max));
    static abstract TVector Clamp(TVector value1, TVector min, TVector max);
    static abstract TScalar DistanceSquared(TVector value1, TVector value2);
    static abstract TScalar Dot(TVector vector1, TVector vector2);
    static abstract TVector Max(TVector value1, TVector value2);
    static abstract TVector Min(TVector value1, TVector value2);
    static virtual TVector Max(TVector value1, TScalar value2) => TVector.Max(value1, TVector.Create(value2));
    static virtual TVector Min(TVector value1, TScalar value2) => TVector.Min(value1, TVector.Create(value2));
    static abstract TVector Abs(TVector value);
    
    /// <summary>Copies the elements of the vector to a specified array.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="array">The destination array.</param>
    /// <remarks><paramref name="array" /> must have enough elements to fit all scalars in this vector. The method copies the vector's elements starting at index 0.</remarks>
    /// <exception cref="NullReferenceException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The number of elements in the current instance is greater than in the array.</exception>
    /// <exception cref="RankException"><paramref name="array" /> is multidimensional.</exception>
    static abstract void CopyTo(TVector vector, TScalar[] array);

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
    static abstract void CopyTo(TVector vector, TScalar[] array, int index);

    /// <summary>Copies the vector to the given <see cref="Span{TScalar}" />. The length of the destination span must be at least enough to fit all scalars in this vector.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination span.</exception>
    static abstract void CopyTo(TVector vector, Span<TScalar> destination);

    /// <summary>Attempts to copy the vector to the given <see cref="Span{Single}" />. The length of the destination span must be at least enough to fit all scalars in this vector.</summary>
    /// <param name="vector">The vector to be copied.</param>
    /// <param name="destination">The destination span which the values are copied into.</param>
    /// <returns><see langword="true" /> if the source vector was successfully copied to <paramref name="destination" />. <see langword="false" /> if <paramref name="destination" /> is not large enough to hold the source vector.</returns>
    static abstract bool TryCopyTo(TVector vector, Span<TScalar> destination);

    #region Conversion

    [DoesNotReturn]
    private static void ThrowNotSupportedException()
    {
        throw new NotSupportedException();
    }

    /// <summary>Creates an instance of the current type from a value, throwing an overflow exception for any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <returns>An instance of <typeparamref name="TVector" /> created from <paramref name="value" />.</returns>
    /// <exception cref="NotSupportedException"><typeparamref name="TOther" /> is not supported.</exception>
    /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <typeparamref name="TVector" />.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static virtual TVector CreateChecked<TOther, TOtherScalar>(TOther value)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>
#nullable restore
    {
        TVector? result;

        if (typeof(TOther) == typeof(TVector))
        {
            result = (TVector)(object)value;
        }
        else if (!TVector.TryConvertFromChecked<TOther, TOtherScalar>(value, out result) && !TOther.TryConvertToChecked<TVector, TScalar>(value, out result))
        {
            ThrowNotSupportedException();
        }

        return result;
    }

    /// <summary>Creates an instance of the current type from a value, saturating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <returns>An instance of <typeparamref name="TVector" /> created from <paramref name="value" />, saturating if <paramref name="value" /> falls outside the representable range of <typeparamref name="TVector" />.</returns>
    /// <exception cref="NotSupportedException"><typeparamref name="TOther" /> is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static virtual TVector CreateSaturating<TOther, TOtherScalar>(TOther value)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>
#nullable restore
    {
        TVector? result;

        if (typeof(TOther) == typeof(TVector))
        {
            result = (TVector)(object)value;
        }
        else if (!TVector.TryConvertFromSaturating<TOther, TOtherScalar>(value, out result) && !TOther.TryConvertToSaturating<TVector, TScalar>(value, out result))
        {
            ThrowNotSupportedException();
        }

        return result;
    }

    /// <summary>Creates an instance of the current type from a value, truncating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <returns>An instance of <typeparamref name="TVector" /> created from <paramref name="value" />, truncating if <paramref name="value" /> falls outside the representable range of <typeparamref name="TVector" />.</returns>
    /// <exception cref="NotSupportedException"><typeparamref name="TOther" /> is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static virtual TVector CreateTruncating<TOther, TOtherScalar>(TOther value)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>
#nullable restore
    {
        TVector? result;

        if (typeof(TOther) == typeof(TVector))
        {
            result = (TVector)(object)value;
        }
        else if (!TVector.TryConvertFromTruncating<TOther, TOtherScalar>(value, out result) && !TOther.TryConvertToTruncating<TVector, TScalar>(value, out result))
        {
            ThrowNotSupportedException();
        }

        return result;
    }
	
    /// <summary>Tries to convert a value to an instance of the current type, throwing an overflow exception for any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TVector" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <typeparamref name="TVector" />.</exception>
    protected static abstract bool TryConvertFromChecked<TOther, TOtherScalar>(TOther value, [MaybeNullWhen(false)] out TVector result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore

    /// <summary>Tries to convert a value to an instance of the current type, saturating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TVector" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    protected static abstract bool TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, [MaybeNullWhen(false)] out TVector result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore

    /// <summary>Tries to convert a value to an instance of the current type, truncating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type of <paramref name="value" />.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TVector" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TVector" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    protected static abstract bool TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, [MaybeNullWhen(false)] out TVector result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore

    /// <summary>Tries to convert an instance of the current type to another type, throwing an overflow exception for any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type to which <paramref name="value" /> should be converted.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TOther" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TOther" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    /// <exception cref="OverflowException"><paramref name="value" /> is not representable by <typeparamref name="TOther" />.</exception>
    protected static abstract bool TryConvertToChecked<TOther, TOtherScalar>(TVector value, [MaybeNullWhen(false)] out TOther result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore

    /// <summary>Tries to convert an instance of the current type to another type, saturating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type to which <paramref name="value" /> should be converted.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TOther" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TOther" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    protected static abstract bool TryConvertToSaturating<TOther, TOtherScalar>(TVector value, [MaybeNullWhen(false)] out TOther result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore

    /// <summary>Tries to convert an instance of the current type to another type, truncating any values that fall outside the representable range of the current type.</summary>
    /// <typeparam name="TOther">The type to which <paramref name="value" /> should be converted.</typeparam>
    /// <param name="value">The value which is used to create the instance of <typeparamref name="TOther" />.</param>
    /// <param name="result">On return, contains an instance of <typeparamref name="TOther" /> converted from <paramref name="value" />.</param>
    /// <returns><c>false</c> if <typeparamref name="TOther" /> is not supported; otherwise, <c>true</c>.</returns>
    protected static abstract bool TryConvertToTruncating<TOther, TOtherScalar>(TVector value, [MaybeNullWhen(false)] out TOther result)
#nullable disable
        where TOther : IVector<TOther, TOtherScalar> where TOtherScalar : INumberBase<TOtherScalar>;
#nullable restore


    #endregion
}

public interface IModulusVector<TVector, TScalar> :
    IVector<TVector, TScalar>,
    IModulusOperators<TVector, TVector, TVector>
    where TVector : IModulusVector<TVector, TScalar>
    where TScalar : INumberBase<TScalar>, IModulusOperators<TScalar, TScalar, TScalar>
{
    static abstract TVector operator %(TVector left, TScalar right);

    static virtual TVector Remainder(TVector left, TVector right) => left % right;
    static virtual TVector Remainder(TVector left, TScalar right) => left % right;
}

public interface INumberVector<TVector, TScalar> :
    IModulusVector<TVector, TScalar>
    where TVector : INumberVector<TVector, TScalar>
    where TScalar : INumber<TScalar>
{
    static abstract TVector CopySign(TVector value, TVector sign);
    static abstract TVector CopySign(TVector value, TScalar sign);

    static abstract TVector Sign(TVector value);
}

public interface IBinaryNumberVector<TVector, TScalar> :
    INumberVector<TVector, TScalar>,
    IBitwiseOperators<TVector, TVector, TVector>
    where TVector : IBinaryNumberVector<TVector, TScalar>
    where TScalar : IBinaryNumber<TScalar>
{
    static virtual TVector AllBitsSet => TVector.Create(TScalar.AllBitsSet);
    
    /// <summary>Computes the log2 of a value.</summary>
    /// <param name="value">The value whose log2 is to be computed.</param>
    /// <returns>The log2 of <paramref name="value" />.</returns>
    static abstract TVector Log2(TVector value);
}

public interface IBinaryIntegerVector<TVector, TScalar> :
    IBinaryNumberVector<TVector, TScalar>
    where TVector : IBinaryIntegerVector<TVector, TScalar>
    where TScalar : IBinaryInteger<TScalar>
{
    /// <summary>Computes the quotient and remainder of two values.</summary>
    /// <param name="left">The value which <paramref name="right" /> divides.</param>
    /// <param name="right">The value which divides <paramref name="left" />.</param>
    /// <returns>The quotient and remainder of <paramref name="left" /> divided-by <paramref name="right" />.</returns>
    static virtual (TVector Quotient, TVector Remainder) DivRem(TVector left, TVector right)
    {
        var quotient = left / right;
        return (quotient, (left - (quotient * right)));
    }

    /// <summary>Computes the number of bits that are set in a value.</summary>
    /// <param name="value">The value whose set bits are to be counted.</param>
    /// <returns>The number of set bits in <paramref name="value" />.</returns>
    static abstract TVector PopCount(TVector value);

    /// <summary>Tries to read a two's complement number from a span, in big-endian format, and convert it to an instance of the current type.</summary>
    /// <param name="source">The span from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <param name="value">On return, contains the value read from <paramref name="source" /> or <c>default</c> if a value could not be read.</param>
    /// <returns><c>true</c> if the value was succesfully read from <paramref name="source" />; otherwise, <c>false</c>.</returns>
    static abstract bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, [MaybeNullWhen(false)] out TVector value);

    /// <summary>Tries to read a two's complement number from a span, in little-endian format, and convert it to an instance of the current type.</summary>
    /// <param name="source">The span from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <param name="value">On return, contains the value read from <paramref name="source" /> or <c>default</c> if a value could not be read.</param>
    /// <returns><c>true</c> if the value was succesfully read from <paramref name="source" />; otherwise, <c>false</c>.</returns>
    static abstract bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, [MaybeNullWhen(false)] out TVector value);

    /// <summary>Reads a two's complement number from a given array, in big-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadBigEndian(byte[] source, bool isUnsigned)
    {
        if (!TVector.TryReadBigEndian(source, isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Reads a two's complement number from a given array, in big-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="startIndex">The starting index from which the value should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" /> starting at <paramref name="startIndex" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadBigEndian(byte[] source, int startIndex, bool isUnsigned)
    {
        if (!TVector.TryReadBigEndian(source.AsSpan(startIndex), isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Reads a two's complement number from a given span, in big-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned)
    {
        if (!TVector.TryReadBigEndian(source, isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Reads a two's complement number from a given array, in little-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadLittleEndian(byte[] source, bool isUnsigned)
    {
        if (!TVector.TryReadLittleEndian(source, isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Reads a two's complement number from a given array, in little-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="startIndex">The starting index from which the value should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" /> starting at <paramref name="startIndex" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadLittleEndian(byte[] source, int startIndex, bool isUnsigned)
    {
        if (!TVector.TryReadLittleEndian(source.AsSpan(startIndex), isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Reads a two's complement number from a given span, in little-endian format, and converts it to an instance of the current type.</summary>
    /// <param name="source">The array from which the two's complement number should be read.</param>
    /// <param name="isUnsigned"><c>true</c> if <paramref name="source" /> represents an unsigned two's complement number; otherwise, <c>false</c> to indicate it represents a signed two's complement number.</param>
    /// <returns>The value read from <paramref name="source" />.</returns>
    /// <exception cref="OverflowException"><paramref name="source" /> is not representable by <typeparamref name="TVector" /></exception>
    static virtual TVector ReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned)
    {
        if (!TVector.TryReadLittleEndian(source, isUnsigned, out var value))
        {
            throw new OverflowException();
        }
        return value;
    }

    /// <summary>Gets the number of bytes that will be written as part of <see cref="TryWriteLittleEndian(Span{byte}, out int)" />.</summary>
    /// <returns>The number of bytes that will be written as part of <see cref="TryWriteLittleEndian(Span{byte}, out int)" />.</returns>
    int GetByteCount()
    {
        var byteCount = 0;
        
        foreach (var scalar in TVector.AsSpan((TVector)this))
        {
            byteCount += scalar.GetByteCount();
        }

        return byteCount;
    }

    /// <summary>Gets the length, in bits, of the shortest two's complement representation of the current value.</summary>
    /// <returns>The length, in bits, of the shortest two's complement representation of the current value.</returns>
    int GetShortestBitLength() => GetByteCount() * 8;

    /// <summary>Tries to write the current value, in big-endian format, to a given span.</summary>
    /// <param name="destination">The span to which the current value should be written.</param>
    /// <param name="bytesWritten">The number of bytes written to <paramref name="destination" />.</param>
    /// <returns><c>true</c> if the value was successfully written to <paramref name="destination" />; otherwise, <c>false</c>.</returns>
    bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten);

    /// <summary>Tries to write the current value, in little-endian format, to a given span.</summary>
    /// <param name="destination">The span to which the current value should be written.</param>
    /// <param name="bytesWritten">The number of bytes written to <paramref name="destination" />.</param>
    /// <returns><c>true</c> if the value was successfully written to <paramref name="destination" />; otherwise, <c>false</c>.</returns>
    bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten);

    /// <summary>Writes the current value, in big-endian format, to a given array.</summary>
    /// <param name="destination">The array to which the current value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" />.</returns>
    int WriteBigEndian(byte[] destination)
    {
        if (!TryWriteBigEndian(destination, out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }

    /// <summary>Writes the current value, in big-endian format, to a given array.</summary>
    /// <param name="destination">The array to which the current value should be written.</param>
    /// <param name="startIndex">The starting index at which the value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" /> starting at <paramref name="startIndex" />.</returns>
    int WriteBigEndian(byte[] destination, int startIndex)
    {
        if (!TryWriteBigEndian(destination.AsSpan(startIndex), out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }

    /// <summary>Writes the current value, in big-endian format, to a given span.</summary>
    /// <param name="destination">The span to which the current value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" />.</returns>
    int WriteBigEndian(Span<byte> destination)
    {
        if (!TryWriteBigEndian(destination, out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }

    /// <summary>Writes the current value, in little-endian format, to a given array.</summary>
    /// <param name="destination">The array to which the current value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" />.</returns>
    int WriteLittleEndian(byte[] destination)
    {
        if (!TryWriteLittleEndian(destination, out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }

    /// <summary>Writes the current value, in little-endian format, to a given array.</summary>
    /// <param name="destination">The array to which the current value should be written.</param>
    /// <param name="startIndex">The starting index at which the value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" /> starting at <paramref name="startIndex" />.</returns>
    int WriteLittleEndian(byte[] destination, int startIndex)
    {
        if (!TryWriteLittleEndian(destination.AsSpan(startIndex), out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }

    /// <summary>Writes the current value, in little-endian format, to a given span.</summary>
    /// <param name="destination">The span to which the current value should be written.</param>
    /// <returns>The number of bytes written to <paramref name="destination" />.</returns>
    int WriteLittleEndian(Span<byte> destination)
    {
        if (!TryWriteLittleEndian(destination, out var bytesWritten))
        {
            throw new ArgumentException("Destination too short", nameof(destination));
        }
        return bytesWritten;
    }
}

public interface IFloatingPointVector<TVector, TScalar> :
    INumberVector<TVector, TScalar>
    // IHyperbolicFunctions<TVector>,
    // ITrigonometricFunctions<TVector>,
    // ILogarithmicFunctions<TVector>,
    // IExponentialFunctions<TVector>,
    // IPowerFunctions<TVector>,
    // IRootFunctions<TVector>
    where TVector : IFloatingPointVector<TVector, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
{
    TScalar Length()
    {
        var lengthSquared = LengthSquared();
        return TScalar.Sqrt(lengthSquared);
    }

    static abstract TVector Normalize(TVector value);

    static virtual TScalar Distance(TVector value1, TVector value2)
    {
        var distanceSquared = TVector.DistanceSquared(value1, value2);
        return TScalar.Sqrt(distanceSquared);
    }
 
    static abstract TVector Lerp(TVector value1, TVector value2, TVector amount);
    static virtual TVector Lerp(TVector value1, TVector value2, TScalar amount) => TVector.Lerp(value1, value2, TVector.Create(amount));
    static abstract TVector LerpClamped(TVector value1, TVector value2, TVector amount);
    static virtual TVector LerpClamped(TVector value1, TVector value2, TScalar amount) => TVector.LerpClamped(value1, value2, TVector.Create(amount));
    static abstract TVector Reflect(TVector vector, TVector normal);
    static abstract TVector Sqrt(TVector value);
    
    // Equivalent implementing IHyperbolicFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Acosh(TVector x);
    static abstract TVector Asinh(TVector x);
    static abstract TVector Atanh(TVector x);
    static abstract TVector Cosh(TVector x);
    static abstract TVector Sinh(TVector x);
    static abstract TVector Tanh(TVector x);

    // Equivalent implementing ITrigonometricFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Acos(TVector x);
    static abstract TVector AcosPi(TVector x);
    static abstract TVector Asin(TVector x);
    static abstract TVector AsinPi(TVector x);
    static abstract TVector Atan(TVector x);
    static abstract TVector AtanPi(TVector x);
    static abstract TVector Cos(TVector x);
    static abstract TVector CosPi(TVector x);
    static abstract TVector DegreesToRadians(TVector degrees);
    static abstract TVector RadiansToDegrees(TVector radians);
    static abstract TVector Sin(TVector x);
    static abstract TVector SinPi(TVector x);
    static abstract TVector Tan(TVector x);
    static abstract TVector TanPi(TVector x);
    static abstract (TVector Sin, TVector Cos) SinCos(TVector x);
    static abstract (TVector SinPi, TVector CosPi) SinCosPi(TVector x);

    // Equivalent implementing ILogarithmicFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Log(TVector x);
    static abstract TVector Log(TVector x, TVector newBase);
    static abstract TVector Log(TVector x, TScalar newBase);
    static abstract TVector LogP1(TVector x);
    static abstract TVector Log2(TVector x);
    static abstract TVector Log2P1(TVector x);
    static abstract TVector Log10(TVector x);
    static abstract TVector Log10P1(TVector x);

    // Equivalent implementing IExponentialFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Exp(TVector x);
    static abstract TVector ExpM1(TVector x);
    static abstract TVector Exp2(TVector x);
    static abstract TVector Exp2M1(TVector x);
    static abstract TVector Exp10(TVector x);
    static abstract TVector Exp10M1(TVector x);

    // Equivalent implementing IPowerFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Pow(TVector x, TVector y);
    static abstract TVector Pow(TVector x, TScalar y);

    // Equivalent implementing IRootFunctions<System.Runtime.Intrinsics.Vector3>
    static abstract TVector Cbrt(TVector x);
    static abstract TVector Hypot(TVector x, TVector y);
    static abstract TVector Hypot(TVector x, TScalar y);
    static abstract TVector RootN(TVector x, int n);

    // IFloatingPoint<TVector>
    static abstract TVector Round(TVector x);
    static abstract TVector Round(TVector x, int digits);
    static abstract TVector Round(TVector x, MidpointRounding mode);
    static abstract TVector Round(TVector x, int digits, MidpointRounding mode);
    static abstract TVector Truncate(TVector x);

    // IFloatingPointIeee754<TVector>
    static abstract TVector Atan2(TVector x, TVector y);
    static abstract TVector Atan2Pi(TVector x, TVector y);
    static abstract TVector Atan2(TVector x, TScalar y);
    static abstract TVector Atan2Pi(TVector x, TScalar y);
    static abstract TVector BitDecrement(TVector x);
    static abstract TVector BitIncrement(TVector x);
    static abstract TVector FusedMultiplyAdd(TVector left, TVector right, TVector addend);
    static virtual TVector FusedMultiplyAdd(TVector left, TScalar right, TVector addend) => TVector.FusedMultiplyAdd(left, TVector.Create(right), addend); 
    static virtual TVector FusedMultiplyAdd(TVector left, TVector right, TScalar addend) => TVector.FusedMultiplyAdd(left, right, TVector.Create(addend)); 
    static virtual TVector FusedMultiplyAdd(TVector left, TScalar right, TScalar addend) => TVector.FusedMultiplyAdd(left, TVector.Create(right), TVector.Create(addend));
    static abstract TVector ReciprocalEstimate(TVector x);
    static abstract TVector ReciprocalSqrtEstimate(TVector x);

    // IFloatingPointIeee754<TVector>
    static virtual TNewVector ILogB<TNewVector>(TVector x) where TNewVector : IVector<TNewVector, int> => TVector.ILogB<TNewVector, int>(x);
    static abstract TNewVector ILogB<TNewVector, TInt>(TVector x) where TNewVector : IVector<TNewVector, TInt> where TInt : IBinaryInteger<TInt>;
    static abstract TVector ScaleB(TVector x, Vector2D<int> n);
    static abstract TVector ScaleB(TVector x, int n);
    static virtual TNewVector RoundToInt<TNewVector>(TVector vector) where TNewVector : IVector<TNewVector, int> => TVector.RoundToInt<TNewVector, int>(vector);
    static virtual TNewVector FloorToInt<TNewVector>(TVector vector) where TNewVector : IVector<TNewVector, int> => TVector.RoundToInt<TNewVector, int>(vector);
    static virtual TNewVector CeilingToInt<TNewVector>(TVector vector) where TNewVector : IVector<TNewVector, int> => TVector.RoundToInt<TNewVector, int>(vector);
    static abstract TNewVector RoundToInt<TNewVector, TInt>(TVector vector)
        where TNewVector : IVector<TNewVector, TInt>
        where TInt : IBinaryInteger<TInt>;
    static abstract TNewVector FloorToInt<TNewVector, TInt>(TVector vector)
        where TNewVector : IVector<TNewVector, TInt>
        where TInt : IBinaryInteger<TInt>;
    static abstract TNewVector CeilingToInt<TNewVector, TInt>(TVector vector)
        where TNewVector : IVector<TNewVector, TInt>
        where TInt : IBinaryInteger<TInt>;

    static virtual Vector64<TScalar> AsVector64(TVector self) => Vector64.Create(TVector.AsSpan(self));
    static virtual Vector128<TScalar> AsVector128(TVector self) => Vector128.Create(TVector.AsSpan(self));
    static virtual Vector256<TScalar> AsVector256(TVector self) => Vector256.Create(TVector.AsSpan(self));
    static virtual Vector512<TScalar> AsVector512(TVector self) => Vector512.Create(TVector.AsSpan(self));
}

public interface IVector2<TVector, TScalar> : IVector<TVector, TScalar>
    where TVector : IVector2<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static virtual TVector UnitX => TVector.GetUnitVector(0);
    static virtual TVector UnitY => TVector.GetUnitVector(1);
    
    TScalar X { get; }
    TScalar Y { get; }

    static int IVector.Count => 2;
    
    static abstract TVector Create(TScalar x, TScalar y);
}

public interface IVector3<TVector, TScalar> : IVector<TVector, TScalar>
    where TVector : IVector3<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static virtual TVector UnitX => TVector.GetUnitVector(0);
    static virtual TVector UnitY => TVector.GetUnitVector(1);
    static virtual TVector UnitZ => TVector.GetUnitVector(2);
    
    TScalar X { get; }
    TScalar Y { get; }
    TScalar Z { get; }

    static int IVector.Count => 3;
    
    static abstract TVector Create(TScalar x, TScalar y);
}

public interface IVector4<TVector, TScalar> : IVector<TVector, TScalar>
    where TVector : IVector4<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static virtual TVector UnitX => TVector.GetUnitVector(0);
    static virtual TVector UnitY => TVector.GetUnitVector(1);
    static virtual TVector UnitZ => TVector.GetUnitVector(2);
    static virtual TVector UnitW => TVector.GetUnitVector(3);

    TScalar X { get; }
    TScalar Y { get; }
    TScalar Z { get; }
    TScalar W { get; }

    static int IVector.Count => 4;
    
    static abstract TVector Create(TScalar x, TScalar y);
}

public interface IVector5<TVector, TScalar> : IVector<TVector, TScalar>
    where TVector : IVector5<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static virtual TVector UnitX => TVector.GetUnitVector(0);
    static virtual TVector UnitY => TVector.GetUnitVector(1);
    static virtual TVector UnitZ => TVector.GetUnitVector(2);
    static virtual TVector UnitW => TVector.GetUnitVector(3);
    static virtual TVector UnitV => TVector.GetUnitVector(4);

    TScalar X { get; }
    TScalar Y { get; }
    TScalar Z { get; }
    TScalar W { get; }
    TScalar V { get; }

    static int IVector.Count => 5;
    
    static abstract TVector Create(TScalar x, TScalar y);
}
