using System.Numerics;

namespace GenericVector;

public static class Vector
{
    public static T Length<TVector, T>(this TVector vec)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>, IRootFunctions<T>
    {
        var lengthSquared = vec.LengthSquared();
        return T.Sqrt(lengthSquared);
    }
}