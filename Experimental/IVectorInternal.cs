using System.Numerics;

namespace GenericVector.Experimental;

/// <summary>
/// Marker interface for <see cref="IVector{TVector,TScalar}"/> implementations provided by Silk.NET.Maths.
/// </summary>
internal interface IVectorInternal;

internal interface IVectorInternal<out TVector, in TScalar> : IVectorInternal
    where TVector : IVector<TVector, TScalar>, IVectorInternal<TVector, TScalar>
    where TScalar : INumberBase<TScalar>
{
    static abstract TVector CreateInternal(TScalar x = default!, TScalar y = default!, TScalar z = default!, TScalar w = default!, TScalar v = default!);
}

internal interface IVector2Internal<TVector>
{
// Returns null if incompatible. Throws OverflowException if overflowing
    Vector2D<T>? GetChecked<T>() where T : INumberBase<T>;
    Vector2D<T>? GetSaturating<T>() where T : INumberBase<T>;
    Vector2D<T>? GetTruncating<T>() where T : INumberBase<T>;
}