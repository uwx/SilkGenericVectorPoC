using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector;

internal static class SpeedHelpers
{
    private struct Storage128<T>
    {
        public T a, b, c, d;

        public Storage128(T a, T b, T c, T d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }

    private static unsafe TOut PadToVector<TIn, T, TOut>(ref TIn any)
    {
#if DEBUG
        if (sizeof(TIn) > sizeof(Storage128<T>) || sizeof(TOut) < sizeof(Storage128<T>))
        {
            throw new ArgumentOutOfRangeException(nameof(any), sizeof(TIn), $"{nameof(Storage128<T>)} error: {sizeof(TIn)} > {sizeof(Storage128<T>)}");
        }
#endif
            
        Storage128<T> storage = default;

        Unsafe.Copy(Unsafe.AsPointer(ref storage.a), ref any);

        return Unsafe.As<Storage128<T>, TOut>(ref storage);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool FastEqualsUpTo4<T, TElem>(T value1, T value2)
        where TElem : IEquatable<TElem>
    {
        // This function needs to account for floating-point equality around NaN
        // and so must behave equivalently to the underlying float/double.Equals
        
        #if DEBUG
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sizeof(T), sizeof(TElem) * 4);
        #endif
    
        if (Vector64.IsHardwareAccelerated && Vector64<TElem>.IsSupported && Vector64<TElem>.Count >= sizeof(T) / sizeof(TElem))
        {
            return PadToVector<T, TElem, Vector64<TElem>>(ref value1).Equals(PadToVector<T, TElem, Vector64<TElem>>(ref value2));
        }

        if (Vector128.IsHardwareAccelerated && Vector128<TElem>.IsSupported && Vector128<TElem>.Count >= sizeof(T) / sizeof(TElem))
        {
            return PadToVector<T, TElem, Vector128<TElem>>(ref value1).Equals(PadToVector<T, TElem, Vector128<TElem>>(ref value2));
        }

        if (Vector256.IsHardwareAccelerated && Vector256<TElem>.IsSupported && Vector256<TElem>.Count >= sizeof(T) / sizeof(TElem))
        {
            return PadToVector<T, TElem, Vector256<TElem>>(ref value1).Equals(PadToVector<T, TElem, Vector256<TElem>>(ref value2));
        }

        return SoftwareFallback(in value1, in value2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool SoftwareFallback2(in TElem self, in TElem other, nint length)
        {
            ref var a = ref Unsafe.AsRef(in self);
            ref var b = ref Unsafe.AsRef(in other);
            if (!a.Equals(b)) return false;
            if (length == 1) return true;
            return SoftwareFallback2(Unsafe.Add(ref a, 1), Unsafe.Add(ref b, 1), length - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool SoftwareFallback(in T self, in T other)
        {
            ref var first = ref Unsafe.As<T, TElem>(ref Unsafe.AsRef(in self));
            ref var second = ref Unsafe.As<T, TElem>(ref Unsafe.AsRef(in other));

            switch (sizeof(T) / sizeof(TElem))
            {
                case 1:
                {
                    return first.Equals(second);
                }
                case 2:
                {
                    var lookUp0 = first;
                    var lookUp1 = second;
                        
                    if (!lookUp0.Equals(lookUp1))
                        return false;

                    lookUp0 = Unsafe.Add(ref first, 1);
                    lookUp1 = Unsafe.Add(ref second, 1);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    return true;
                }
                case 3:
                {
                    var lookUp0 = first;
                    var lookUp1 = second;
                        
                    if (!lookUp0.Equals(lookUp1))
                        return false;

                    lookUp0 = Unsafe.Add(ref first, 1);
                    lookUp1 = Unsafe.Add(ref second, 1);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    lookUp0 = Unsafe.Add(ref first, 2);
                    lookUp1 = Unsafe.Add(ref second, 2);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    return true;
                }
                case 4:
                {
                    var lookUp0 = first;
                    var lookUp1 = second;
                        
                    if (!lookUp0.Equals(lookUp1))
                        return false;

                    lookUp0 = Unsafe.Add(ref first, 1);
                    lookUp1 = Unsafe.Add(ref second, 1);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    lookUp0 = Unsafe.Add(ref first, 2);
                    lookUp1 = Unsafe.Add(ref second, 2);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    lookUp0 = Unsafe.Add(ref first, 3);
                    lookUp1 = Unsafe.Add(ref second, 3);
                    if (!lookUp0.Equals(lookUp1))
                        return false;
                        
                    return true;
                }
            }

            return false;
        }
    }
    
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal static unsafe bool FastAndDelegateForScalarEntries<T, TElem>(in T self, Func<TElem, bool> toCall)
    // {
    //     ref var first = ref Unsafe.As<T, TElem>(ref Unsafe.AsRef(in self));
    //
    //     switch (sizeof(T) / sizeof(TElem))
    //     {
    //         case 1:
    //         {
    //             return toCall(first);
    //         }
    //         case 2:
    //         {
    //             var lookUp0 = first;
    //                 
    //             if (!toCall(lookUp0))
    //                 return false;
    //
    //             lookUp0 = Unsafe.Add(ref first, 1);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             return true;
    //         }
    //         case 3:
    //         {
    //             var lookUp0 = first;
    //                 
    //             if (!toCall(lookUp0))
    //                 return false;
    //
    //             lookUp0 = Unsafe.Add(ref first, 1);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             lookUp0 = Unsafe.Add(ref first, 2);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             return true;
    //         }
    //         case 4:
    //         {
    //             var lookUp0 = first;
    //                 
    //             if (!toCall(lookUp0))
    //                 return false;
    //
    //             lookUp0 = Unsafe.Add(ref first, 1);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             lookUp0 = Unsafe.Add(ref first, 2);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             lookUp0 = Unsafe.Add(ref first, 3);
    //             if (!toCall(lookUp0))
    //                 return false;
    //                 
    //             return true;
    //         }
    //     }
    //
    //     return false;
    // }
}