using System.Numerics;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("GenericVector.Perf")]

namespace GenericVector;

internal static class Scalar
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T BillboardMinAngle<T>() where T : IFloatingPoint<T>
        => T.One - (T.CreateChecked(0.1m) * (T.Pi / T.CreateChecked(180))); // 0.1 degrees

    // smallest such that 1.0+NormalizeEpsilon != 1.0
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T NormalizeEpsilon<T>() where T : IFloatingPointIeee754<T>
        => T.BitIncrement(T.One);
}

internal static class Scalar<T> where T : INumberBase<T>
{
    public static T Two => T.CreateChecked(2);
    public static T Half => T.CreateChecked(0.5f);
    public static T Four => T.CreateChecked(4);
    public static T OneAndAHalf => T.CreateChecked(1.5f);
    public static T Quarter => T.CreateChecked(0.25f);

    public static T BillboardEpsilon => T.CreateChecked(0.0001m);
    public static T DecomposeEpsilon => T.CreateChecked(0.0001m);

    public static T SlerpEpsilon => T.CreateChecked(0.000001m);
    public static T ThreeQuarters => T.CreateChecked(0.75f);
}