using System.Numerics;

namespace GenericVector.Experimental;

internal interface IVector2Internal<TVector>
{
    
    
// Returns null if incompatible. Throws OverflowException if overflowing
    Vector2D<T>? GetChecked<T>() where T : INumberBase<T>;
    Vector2D<T>? GetSaturating<T>() where T : INumberBase<T>;
    Vector2D<T>? GetTruncating<T>() where T : INumberBase<T>;
}