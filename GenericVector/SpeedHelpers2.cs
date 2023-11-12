using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

internal partial class SpeedHelpers2
{
    private interface IUnaryOperator<TIn, TOut>
    {
        static virtual bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get => false;
        }

        static virtual Vector2 Invoke(Vector2 input) => throw new NotSupportedException();
        static virtual Vector3 Invoke(Vector3 input) => throw new NotSupportedException();
        static virtual Vector4 Invoke(Vector4 input) => throw new NotSupportedException();
        static abstract TOut Invoke(TIn input);
        static abstract Vector128<TOut> Invoke(Vector128<TIn> input);
        static abstract Vector256<TOut> Invoke(Vector256<TIn> input);
        static abstract Vector512<TOut> Invoke(Vector512<TIn> input);
    }

    private interface IBinaryOperator<TLeft, TRight, TOut>
    {
        static virtual bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get => false;
        }

        static virtual Vector2 Invoke(Vector2 left, Vector2 right) => throw new NotSupportedException();
        static virtual Vector3 Invoke(Vector3 left, Vector3 right) => throw new NotSupportedException();
        static virtual Vector4 Invoke(Vector4 left, Vector4 right) => throw new NotSupportedException();
        static abstract TOut Invoke(TLeft left, TRight right);
        static abstract Vector128<TOut> Invoke(Vector128<TLeft> left, Vector128<TRight> right);
        static abstract Vector256<TOut> Invoke(Vector256<TLeft> left, Vector256<TRight> right);
        static abstract Vector512<TOut> Invoke(Vector512<TLeft> left, Vector512<TRight> right);
    }

    /// <summary>Operator that takes three input values and returns a single value.</summary>
    private interface ITernaryOperator<TX, TY, TZ, TResult>
    {
        static virtual bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get => false;
        }

        static virtual Vector2 Invoke(Vector2 x, Vector2 y, Vector2 z) => throw new NotSupportedException();
        static virtual Vector3 Invoke(Vector3 x, Vector3 y, Vector3 z) => throw new NotSupportedException();
        static virtual Vector4 Invoke(Vector4 x, Vector4 y, Vector4 z) => throw new NotSupportedException();
        static abstract TResult Invoke(TX x, TY y, TZ z);
        static abstract Vector128<TResult> Invoke(Vector128<TX> x, Vector128<TY> y, Vector128<TZ> z);
        static abstract Vector256<TResult> Invoke(Vector256<TX> x, Vector256<TY> y, Vector256<TZ> z);
        static abstract Vector512<TResult> Invoke(Vector512<TX> x, Vector512<TY> y, Vector512<TZ> z);
    }
    
    /// <summary><see cref="IBinaryOperator"/> that specializes horizontal aggregation of all elements in a vector.</summary>
    private interface IAggregationOperator<T, TOut> : IBinaryOperator<T, T, TOut>
    {
        static abstract TOut Invoke(Vector128<T> x);
        static abstract TOut Invoke(Vector256<T> x);
        static abstract TOut Invoke(Vector512<T> x);

        static abstract TOut IdentityValue { get; }
    }

    private struct OpSum<T> : IAggregationOperator<T, T> where T : INumberBase<T>, IAdditionOperators<T, T, T>
    {
        public static T Invoke(T left, T right) => left + right;
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => left + right;
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right) => left + right;
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right) => left + right;
        public static T Invoke(Vector128<T> x) => Vector128.Sum(x);
        public static T Invoke(Vector256<T> x) => Vector256.Sum(x);
        public static T Invoke(Vector512<T> x) => Vector512.Sum(x);
        public static T IdentityValue => T.One;
    }

    private struct OpEquals<T> : IAggregationOperator<T, T> where T : IEqualityOperators<T, T, bool>, INumberBase<T>
    {
        public static T IdentityValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get => T.One;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T Invoke(T left, T right) => left == right ? T.One : T.Zero;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => Vector128.Equals(left, right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right) => Vector256.Equals(left, right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right) => Vector512.Equals(left, right);
        
        // handles both AllBitsSet and One
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T Invoke(Vector128<T> x) => Vector128.EqualsAny(x, Vector128<T>.Zero) ? T.Zero : T.One;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T Invoke(Vector256<T> x) => Vector256.EqualsAny(x, Vector256<T>.Zero) ? T.Zero : T.One;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T Invoke(Vector512<T> x) => Vector512.EqualsAny(x, Vector512<T>.Zero) ? T.Zero : T.One;
    }

    private struct OpDistanceSquared<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>
    {
        public static T Invoke(T left, T right) => throw new NotImplementedException();

        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            var difference = left - right;
            return OpMultiply<T>.Invoke(difference, difference);
        }
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            var difference = left - right;
            return OpMultiply<T>.Invoke(difference, difference);
        }
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            var difference = left - right;
            return OpMultiply<T>.Invoke(difference, difference);
        }
    }

    private struct OpSquare<T> : IUnaryOperator<T, T> where T : IMultiplyOperators<T, T, T>
    {
        public static T Invoke(T input) => input * input;
        public static Vector128<T> Invoke(Vector128<T> input) => input * input;
        public static Vector256<T> Invoke(Vector256<T> input) => input * input;
        public static Vector512<T> Invoke(Vector512<T> input) => input * input;
    }
    
    private struct OpTemplate2<T> : IBinaryOperator<T, T, T>
    {
        public static T Invoke(T left, T right) => throw new NotImplementedException();
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => throw new NotImplementedException();
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right) => throw new NotImplementedException();
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right) => throw new NotImplementedException();
    }

    private struct OpTemplate1<T> : IUnaryOperator<T, T>
    {
        public static T Invoke(T input) => throw new NotImplementedException();
        public static Vector128<T> Invoke(Vector128<T> input) => throw new NotImplementedException();
        public static Vector256<T> Invoke(Vector256<T> input) => throw new NotImplementedException();
        public static Vector512<T> Invoke(Vector512<T> input) => throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref T GetRef<TVector, T>(in TVector vec)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        return ref Unsafe.As<TVector, T>(ref Unsafe.AsRef(in vec));
    }

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe nuint GetLength<TVector, T>()
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        return (nuint)(sizeof(TVector) / sizeof(T));
    }

    private static unsafe TVector Operate<TVector, T, TOp>(TVector left, TVector right)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
        where TOp : IBinaryOperator<T, T, T>
    {
        if (TOp.HasNumerics)
        {
            return DoNumerics(left, right);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static TVector DoNumerics(TVector left, TVector right)
            {
                if (typeof(TVector) == typeof(Vector2f<float>)) // TODO OTHER VECTORS
                {
                    return (TVector)(object)TOp.Invoke(
                        Unsafe.BitCast<Vector2f<float>, Vector2>((Vector2f<float>)(object)left),
                        Unsafe.BitCast<Vector2f<float>, Vector2>((Vector2f<float>)(object)right)
                    );
                }

                throw new NotSupportedException();
            }
        }

        ref var leftT = ref GetRef<TVector, T>(left);
        ref var rightT = ref GetRef<TVector, T>(right);
        var remainder = GetLength<TVector, T>();
        
        Unsafe.SkipInit(out TVector dest);
        ref var destT = ref GetRef<TVector, T>(dest);

        #region Vector512
        if (Vector512.IsHardwareAccelerated)
        {
            if (remainder >= (uint)Vector512<T>.Count)
            {
                Vectorized(ref leftT, ref rightT, ref destT, remainder);
                return dest;

                static void Vectorized(ref T leftT, ref T rightT, ref T destT, nuint remainder)
                {
                    var end = TOp.Invoke(
                        Vector512.LoadUnsafe(ref leftT, remainder - (uint)(Vector512<T>.Count)),
                        Vector512.LoadUnsafe(ref rightT, remainder - (uint)(Vector512<T>.Count))
                    );
                    
                    while (remainder >= (uint)Vector512<T>.Count)
                    {
                        var outVec = TOp.Invoke(Vector512.LoadUnsafe(ref leftT), Vector512.LoadUnsafe(ref rightT));
                        outVec.StoreUnsafe(ref destT);
                        leftT = ref Unsafe.Add(ref leftT, (uint)Vector512<T>.Count);
                        rightT = ref Unsafe.Add(ref rightT, (uint)Vector512<T>.Count);
                        destT = ref Unsafe.Add(ref destT, (uint)Vector512<T>.Count);
                        remainder -= (uint)Vector512<T>.Count;
                    }

                    if (remainder > 0)
                    {
                        nuint endIndex = remainder;
                        end.StoreUnsafe(ref destT, endIndex - (uint)Vector512<T>.Count);
                    }
                }
            }

            // We have less than a vector and so we can only handle this as scalar. To do this
            // efficiently, we simply have a small jump table and fallthrough. So we get a simple
            // length check, single jump, and then linear execution.

            VectorizedSmall(ref leftT, ref rightT, ref destT, remainder);
            return dest;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void VectorizedSmall(ref T leftT, ref T rightT, ref T destT, nuint remainder)
            {
                switch (remainder)
                {
                    case 15:
                    case 14:
                    case 13:
                    case 12:
                    case 11:
                    case 10:
                    case 9:
                    {
                        Debug.Assert(Vector256.IsHardwareAccelerated);

                        var beg = TOp.Invoke(
                            Vector256.LoadUnsafe(ref leftT),
                            Vector256.LoadUnsafe(ref rightT)
                        );
                        var end = TOp.Invoke(
                            Vector256.LoadUnsafe(ref leftT, remainder - (uint)(Vector256<T>.Count)),
                            Vector256.LoadUnsafe(ref rightT, remainder - (uint)(Vector256<T>.Count))
                        );

                        beg.StoreUnsafe(ref destT);
                        end.StoreUnsafe(ref destT, remainder - (uint)(Vector256<T>.Count));

                        break;
                    }

                    case 8:
                    {
                        Debug.Assert(Vector256.IsHardwareAccelerated);

                        var beg = TOp.Invoke(
                            Vector256.LoadUnsafe(ref leftT),
                            Vector256.LoadUnsafe(ref rightT)
                        );
                        beg.StoreUnsafe(ref destT);

                        break;
                    }

                    case 7:
                    case 6:
                    case 5:
                    {
                        Debug.Assert(Vector128.IsHardwareAccelerated);

                        Vector128<T> beg = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT),
                            Vector128.LoadUnsafe(ref rightT)
                        );
                        Vector128<T> end = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT, remainder - (uint)(Vector128<T>.Count)),
                            Vector128.LoadUnsafe(ref rightT, remainder - (uint)(Vector128<T>.Count))
                        );

                        beg.StoreUnsafe(ref destT);
                        end.StoreUnsafe(ref destT, remainder - (uint)(Vector128<T>.Count));

                        break;
                    }

                    case 4:
                    {
                        Debug.Assert(Vector128.IsHardwareAccelerated);

                        Vector128<T> beg = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT),
                            Vector128.LoadUnsafe(ref rightT)
                        );
                        beg.StoreUnsafe(ref destT);

                        break;
                    }

                    case 3:
                    {
                        Unsafe.Add(ref destT, 2) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 2),
                            Unsafe.Add(ref rightT, 2)
                        );
                        goto case 2;
                    }

                    case 2:
                    {
                        Unsafe.Add(ref destT, 1) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 1),
                            Unsafe.Add(ref rightT, 1)
                        );
                        goto case 1;
                    }

                    case 1:
                    {
                        destT = TOp.Invoke(leftT, rightT);
                        goto case 0;
                    }

                    case 0:
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region Vector256
        if (Vector256.IsHardwareAccelerated)
        {
            if (remainder >= (uint)Vector256<T>.Count)
            {
                Vectorized(ref leftT, ref rightT, ref destT, remainder);
                
                return dest;

                static void Vectorized(ref T leftT, ref T rightT, ref T destT, nuint remainder)
                {
                    var end = TOp.Invoke(
                        Vector256.LoadUnsafe(ref leftT, remainder - (uint)(Vector256<T>.Count)),
                        Vector256.LoadUnsafe(ref rightT, remainder - (uint)(Vector256<T>.Count))
                    );
                    
                    while (remainder >= (uint)Vector256<T>.Count)
                    {
                        var outVec = TOp.Invoke(Vector256.LoadUnsafe(ref leftT), Vector256.LoadUnsafe(ref rightT));
                        outVec.StoreUnsafe(ref destT);
                        leftT = ref Unsafe.Add(ref leftT, (uint)Vector256<T>.Count);
                        rightT = ref Unsafe.Add(ref rightT, (uint)Vector256<T>.Count);
                        destT = ref Unsafe.Add(ref destT, (uint)Vector256<T>.Count);
                        remainder -= (uint)Vector256<T>.Count;
                    }

                    if (remainder > 0)
                    {
                        nuint endIndex = remainder;
                        end.StoreUnsafe(ref destT, endIndex - (uint)Vector256<T>.Count);
                    }
                }
            }

            // We have less than a vector and so we can only handle this as scalar. To do this
            // efficiently, we simply have a small jump table and fallthrough. So we get a simple
            // length check, single jump, and then linear execution.

            VectorizedSmall(ref leftT, ref rightT, ref destT, remainder);
            return dest;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void VectorizedSmall(ref T leftT, ref T rightT, ref T destT, nuint remainder)
            {
                switch (remainder)
                {
                    case 7:
                    case 6:
                    case 5:
                    {
                        Debug.Assert(Vector128.IsHardwareAccelerated);

                        var beg = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT),
                            Vector128.LoadUnsafe(ref rightT)
                        );
                        var end = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT, remainder - (uint)(Vector128<T>.Count)),
                            Vector128.LoadUnsafe(ref rightT, remainder - (uint)(Vector128<T>.Count))
                        );

                        beg.StoreUnsafe(ref destT);
                        end.StoreUnsafe(ref destT, remainder - (uint)(Vector128<T>.Count));

                        break;
                    }

                    case 4:
                    {
                        Debug.Assert(Vector128.IsHardwareAccelerated);

                        Vector128<T> beg = TOp.Invoke(
                            Vector128.LoadUnsafe(ref leftT),
                            Vector128.LoadUnsafe(ref rightT)
                        );
                        beg.StoreUnsafe(ref destT);

                        break;
                    }

                    case 3:
                    {
                        Unsafe.Add(ref destT, 2) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 2),
                            Unsafe.Add(ref rightT, 2)
                        );
                        goto case 2;
                    }

                    case 2:
                    {
                        Unsafe.Add(ref destT, 1) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 1),
                            Unsafe.Add(ref rightT, 1)
                        );
                        goto case 1;
                    }

                    case 1:
                    {
                        destT = TOp.Invoke(leftT, rightT);
                        goto case 0;
                    }

                    case 0:
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region Vector128
        if (Vector128.IsHardwareAccelerated)
        {
            if (remainder >= (uint)Vector128<T>.Count)
            {
                Vectorized(ref leftT, ref rightT, ref destT, remainder);

                return dest;

                static void Vectorized(ref T leftT, ref T rightT, ref T destT, nuint remainder)
                {
                    var end = TOp.Invoke(
                        Vector128.LoadUnsafe(ref leftT, remainder - (uint)(Vector128<T>.Count)),
                        Vector128.LoadUnsafe(ref rightT, remainder - (uint)(Vector128<T>.Count))
                    );
                    
                    while (remainder >= (uint)Vector128<T>.Count)
                    {
                        var outVec = TOp.Invoke(Vector128.LoadUnsafe(ref leftT), Vector128.LoadUnsafe(ref rightT));
                        outVec.StoreUnsafe(ref destT);
                        leftT = ref Unsafe.Add(ref leftT, (uint)Vector128<T>.Count);
                        rightT = ref Unsafe.Add(ref rightT, (uint)Vector128<T>.Count);
                        destT = ref Unsafe.Add(ref destT, (uint)Vector128<T>.Count);
                        remainder -= (uint)Vector128<T>.Count;
                    }

                    if (remainder > 0)
                    {
                        nuint endIndex = remainder;
                        end.StoreUnsafe(ref destT, endIndex - (uint)Vector128<T>.Count);
                    }
                }
            }

            VectorizedSmall(ref leftT, ref rightT, ref destT, remainder);
            return dest;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void VectorizedSmall(ref T leftT, ref T rightT, ref T destT, nuint remainder)
            {
                switch (remainder)
                {
                    case 3:
                    {
                        Unsafe.Add(ref destT, 2) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 2),
                            Unsafe.Add(ref rightT, 2)
                        );
                        goto case 2;
                    }

                    case 2:
                    {
                        Unsafe.Add(ref destT, 1) = TOp.Invoke(
                            Unsafe.Add(ref leftT, 1),
                            Unsafe.Add(ref rightT, 1)
                        );
                        goto case 1;
                    }

                    case 1:
                    {
                        destT = TOp.Invoke(leftT, rightT);
                        goto case 0;
                    }

                    case 0:
                    {
                        break;
                    }
                }
            }
        }
        #endregion
        
        // This is the software fallback when no acceleration is available
        // It requires no branches to hit

        SoftwareFallback(ref leftT, ref rightT, ref destT, remainder);
        return dest;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SoftwareFallback(ref T leftT, ref T rightT, ref T destT, nuint length)
        {
            for (nuint i = 0; i < length; i++)
            {
                Unsafe.Add(ref destT, i) = TOp.Invoke(
                    Unsafe.Add(ref leftT, i),
                    Unsafe.Add(ref rightT, i)
                );
            }
        }
    }

    private static unsafe TVector Operate<TVector, T, TOp>(TVector value)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
        where TOp : IUnaryOperator<T, T>
    {
        // TODO: HasNumerics

        ref var valueT = ref GetRef<TVector, T>(value);
        var remainder = GetLength<TVector, T>();
        
        Unsafe.SkipInit(out TVector dest);
        ref var destT = ref GetRef<TVector, T>(dest);

        SoftwareFallback(ref valueT, ref destT, remainder);
        return dest;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SoftwareFallback(ref T valueT, ref T destT, nuint length)
        {
            for (nuint i = 0; i < length; i++)
            {
                Unsafe.Add(ref destT, i) = TOp.Invoke(Unsafe.Add(ref valueT, i));
            }
        }
    }

    private static unsafe TVector Operate<TVector, T, TOp>(TVector x, TVector y, TVector z)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
        where TOp : ITernaryOperator<T, T, T, T>
    {
        // TODO: HasNumerics

        ref var xT = ref GetRef<TVector, T>(x);
        ref var yT = ref GetRef<TVector, T>(y);
        ref var zT = ref GetRef<TVector, T>(z);
        var remainder = GetLength<TVector, T>();
        
        Unsafe.SkipInit(out TVector dest);
        ref var destT = ref GetRef<TVector, T>(dest);

        SoftwareFallback(ref xT, ref yT, ref zT, ref destT, remainder);
        return dest;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SoftwareFallback(ref T xT, ref T yT, ref T zT, ref T destT, nuint length)
        {
            for (nuint i = 0; i < length; i++)
            {
                Unsafe.Add(ref destT, i) = TOp.Invoke(
                    Unsafe.Add(ref xT, i),
                    Unsafe.Add(ref yT, i),
                    Unsafe.Add(ref zT, i)
                );
            }
        }
    }

    private static unsafe T OperateToScalar<TVector, T, TOp, TAgg>(TVector left, TVector right)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
        where TOp : IBinaryOperator<T, T, T>
        where TAgg : IAggregationOperator<T, T>
    {
        // Not implemented: HasNumerics

        ref var leftT = ref GetRef<TVector, T>(left);
        ref var rightT = ref GetRef<TVector, T>(right);
        var remainder = GetLength<TVector, T>();
        
        // TODO vectorize
        
        // This is the software fallback when no acceleration is available
        // It requires no branches to hit

        return SoftwareFallback(ref leftT, ref rightT, remainder);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T SoftwareFallback(ref T leftT, ref T rightT, nuint length)
        {
            var a = TAgg.IdentityValue;
            
            for (nuint i = 0; i < length; i++)
            {
                a = TAgg.Invoke(a, TOp.Invoke(Unsafe.Add(ref leftT, i), Unsafe.Add(ref rightT, i)));
            }

            return a;
        }
    }
    
    private static unsafe T OperateToScalar<TVector, T, TOp, TAgg>(TVector value)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
        where TOp : IUnaryOperator<T, T>
        where TAgg : IAggregationOperator<T, T>
    {
        // Not implemented: HasNumerics

        ref var valueT = ref GetRef<TVector, T>(value);
        var remainder = GetLength<TVector, T>();
        
        // TODO vectorize
        
        // This is the software fallback when no acceleration is available
        // It requires no branches to hit

        return SoftwareFallback(ref valueT, remainder);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T SoftwareFallback(ref T valueT, nuint length)
        {
            var a = TAgg.IdentityValue;
            
            for (nuint i = 0; i < length; i++)
            {
                a = TAgg.Invoke(a, TOp.Invoke(Unsafe.Add(ref valueT, i)));
            }

            return a;
        }
    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        // TODO System.Numerics fastpath
        
        return OperateToScalar<TVector, T, OpEquals<T>, OpEquals<T>>(left, right) != T.Zero;
    }

    public static TVector EqualIntoVector<TVector, TScalar>(TVector left, TVector right)
        where TVector : IVector<TVector, TScalar>
        where TScalar : INumberBase<TScalar>
    {
        return Operate<TVector, TScalar, OpEquals<TScalar>>(left, right);
    }

    public static T Dot<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        // TODO System.Numerics fastpath

        return OperateToScalar<TVector, T, OpMultiply<T>, OpSum<T>>(left, right);
    }
    
    public static T DistanceSquared<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        // TODO System.Numerics fastpath

        return OperateToScalar<TVector, T, OpDistanceSquared<T>, OpSum<T>>(left, right);
    }
    
    public static T LengthSquared<TVector, T>(TVector value)
        where TVector : IVector<TVector, T>
        where T : INumberBase<T>
    {
        // TODO System.Numerics fastpath

        // This is just Dot(value, value) but doing it this way is friendlier on the stack.
        return OperateToScalar<TVector, T, OpSquare<T>, OpSum<T>>(value);
    }
}
