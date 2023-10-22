using System.Numerics;
using System.Runtime.CompilerServices;

namespace GenericVector;

public interface IVectorAlso<TVector, T> :
    IDivisionOperators<TVector, T, TVector>,
    IMultiplyOperators<TVector, T, TVector>
    where TVector : IVectorAlso<TVector, T>, IVector<TVector, T>
    where T : INumberBase<T>
{
    
}

public interface IVector<TVector, T> :
    IAdditionOperators<TVector, TVector, TVector>,
    // IAdditiveIdentity<TVector, TVector>,
    // IDecrementOperators<TVector>,
    IDivisionOperators<TVector, TVector, TVector>,
    IEqualityOperators<TVector, TVector, bool>,
    // IIncrementOperators<TVector>,
    // IMultiplicativeIdentity<TVector, TVector>,
    IMultiplyOperators<TVector, TVector, TVector>,
    // ISpanFormattable,
    // ISpanParsable<TVector>,
    ISubtractionOperators<TVector, TVector, TVector>,
    // IUnaryPlusOperators<TVector, TVector>,
    IUnaryNegationOperators<TVector, TVector>,
    // IUtf8SpanFormattable,
    // IUtf8SpanParsable<TVector>,
    IEquatable<TVector>
    // IDivisionOperators<TVector, T, TVector>,
    // IMultiplyOperators<TVector, T, TVector>
    where TVector : IVector<TVector, T>
    where T : INumberBase<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static abstract TVector CreateFromRepeatingComponent(T scalar);
    
    /// <summary>Gets a vector whose 4 elements are equal to zero.</summary>
    /// <value>A vector whose four elements are equal to zero (that is, it returns the vector <c>(0,0,0,0)</c>.</value>
    static virtual TVector Zero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.CreateFromRepeatingComponent(T.Zero);
    } 

    /// <summary>Gets a vector whose 4 elements are equal to one.</summary>
    /// <value>Returns <typeparamref name="TVector"/>.</value>
    /// <remarks>A vector whose four elements are equal to one (that is, it returns the vector <c>(1,1,1,1)</c>.</remarks>
    static virtual TVector One
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TVector.CreateFromRepeatingComponent(T.One);
    }

    ReadOnlySpan<T> Components { get; }
    
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Components[index];
    }
    
    static abstract TVector operator /(TVector left, T right);
    static abstract TVector operator *(TVector left, T right);
    static abstract TVector operator *(T left, TVector right);
}