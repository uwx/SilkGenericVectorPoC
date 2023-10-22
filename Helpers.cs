using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector;

public static class Helpers
{
    public static Vector4D<T> AsVector4D<T>(this Vector128<T> vec) where T : INumberBase<T>
    {
        return Unsafe.BitCast<Vector128<T>, Vector4D<T>>(vec);
    }
    
    internal static Vector4D<TOut> AsVector4DUnsafe<T, TOut>(this Vector128<T> vec) where TOut : INumberBase<TOut>
    {
        unsafe
        {
            if (sizeof(T) != sizeof(TOut))
            {
                throw new ArgumentException($"Sizes of {typeof(T)} and {typeof(TOut)} do not match", nameof(vec));
            }
        }

        return Unsafe.BitCast<Vector128<T>, Vector4D<TOut>>(vec);
    }
}