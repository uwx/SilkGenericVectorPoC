using System.Numerics;

namespace GenericVector;

public static class NumericConstants<T> where T : INumberBase<T>
{
    public static readonly T Two = T.One + T.One;
    public static readonly T Half = (T.One / Two);
    
    public static readonly T BillboardEpsilon = T.CreateChecked(1e-4f);
    public static readonly T BillboardMinAngle = T.CreateChecked(1.0f - (0.1f * (MathF.PI / 180.0f))); // 0.1 degrees
    public static readonly T DecomposeEpsilon = T.CreateChecked(0.0001f);
    
    public static readonly T NormalizeEpsilon = T.CreateChecked(1.192092896e-07f); // smallest such that 1.0+NormalizeEpsilon != 1.0
    
    public static readonly T SlerpEpsilon = T.CreateChecked(1e-6f);
}