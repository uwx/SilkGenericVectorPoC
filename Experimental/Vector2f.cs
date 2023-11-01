using System.Numerics;

namespace GenericVector.Experimental;

public struct Vector2f<TScalar> :
    IFloatingPointVector<Vector2f<TScalar>, TScalar>
    where TScalar : IBinaryFloatingPointIeee754<TScalar>
{
    public static int Size => 4;
}