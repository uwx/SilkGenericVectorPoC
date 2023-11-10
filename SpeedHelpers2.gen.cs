using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

internal partial class SpeedHelpers2
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Add<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IAdditionOperators<T, T, T>
    {
        return Operate<TVector, T, OpAdd<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Add<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IAdditionOperators<T, T, T>
    {
        return Operate<TVector, T, OpAdd<T>>(left, TVector.Create(right));
    }

    private struct OpAdd<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IAdditionOperators<T, T, T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left + right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Subtract<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, ISubtractionOperators<T, T, T>
    {
        return Operate<TVector, T, OpSubtract<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Subtract<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, ISubtractionOperators<T, T, T>
    {
        return Operate<TVector, T, OpSubtract<T>>(left, TVector.Create(right));
    }

    private struct OpSubtract<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, ISubtractionOperators<T, T, T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left - right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Multiply<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IMultiplyOperators<T, T, T>
    {
        return Operate<TVector, T, OpMultiply<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Multiply<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IMultiplyOperators<T, T, T>
    {
        return Operate<TVector, T, OpMultiply<T>>(left, TVector.Create(right));
    }

    private struct OpMultiply<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IMultiplyOperators<T, T, T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left * right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Divide<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IDivisionOperators<T, T, T>
    {
        return Operate<TVector, T, OpDivide<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Divide<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IDivisionOperators<T, T, T>
    {
        return Operate<TVector, T, OpDivide<T>>(left, TVector.Create(right));
    }

    private struct OpDivide<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IDivisionOperators<T, T, T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left / right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Remainder<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return Operate<TVector, T, OpRemainder<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Remainder<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        return Operate<TVector, T, OpRemainder<T>>(left, TVector.Create(right));
    }

    private struct OpRemainder<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left % right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            
                        if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
                        {
                            throw new NotSupportedException("Remainder of float or double doesn't make sense");
                        }
                        return left - (left / right * right);
                        ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            
                        if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
                        {
                            throw new NotSupportedException("Remainder of float or double doesn't make sense");
                        }
                        return left - (left / right * right);
                        ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            
                        if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
                        {
                            throw new NotSupportedException("Remainder of float or double doesn't make sense");
                        }
                        return left - (left / right * right);
                        ;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector BitwiseAnd<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        return Operate<TVector, T, OpBitwiseAnd<T>>(left, right);
    }

    private struct OpBitwiseAnd<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left & right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left & right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left & right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left & right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector BitwiseOr<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        return Operate<TVector, T, OpBitwiseOr<T>>(left, right);
    }

    private struct OpBitwiseOr<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left | right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left | right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left | right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left | right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector BitwiseXor<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        return Operate<TVector, T, OpBitwiseXor<T>>(left, right);
    }

    private struct OpBitwiseXor<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return left ^ right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return left ^ right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return left ^ right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return left ^ right;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector BitwiseNot<TVector, T>(TVector value)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        return Operate<TVector, T, OpBitwiseNot<T>>(value);
    }
    private struct OpBitwiseNot<T> : IUnaryOperator<T, T> where T : INumberBase<T>, IBitwiseOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T value)
        {
            return ~value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> value)
        {
            return ~value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> value)
        {
            return ~value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> value)
        {
            return ~value;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Negate<TVector, T>(TVector value)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>, IUnaryNegationOperators<T, T>
    {
        return Operate<TVector, T, OpNegate<T>>(value);
    }
    private struct OpNegate<T> : IUnaryOperator<T, T> where T : INumberBase<T>, IUnaryNegationOperators<T, T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> value)
        {
            return -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> value)
        {
            return -value;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Abs<TVector, T>(TVector value)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpAbs<T>>(value);
    }
    private struct OpAbs<T> : IUnaryOperator<T, T> where T : INumberBase<T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 value)
        {
            return Vector2.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 value)
        {
            return Vector3.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 value)
        {
            return Vector4.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T value)
        {
            return T.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> value)
        {
            return Vector128.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> value)
        {
            return Vector256.Abs(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> value)
        {
            return Vector512.Abs(value);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Clamp<TVector, T>(TVector x, TVector y, TVector z)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpClamp<T>>(x, y, z);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Clamp<TVector, T>(TVector x, T y, T z)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpClamp<T>>(x, TVector.Create(y), TVector.Create(z));
    }

    private struct OpClamp<T> : ITernaryOperator<T, T, T, T> where T : INumberBase<T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 x, Vector2 y, Vector2 z)
        {
            return Vector2.Min(Vector2.Max(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 x, Vector3 y, Vector3 z)
        {
            return Vector3.Min(Vector3.Max(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 x, Vector4 y, Vector4 z)
        {
            return Vector4.Min(Vector4.Max(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T x, T y, T z)
        {
            return T.MinMagnitudeNumber(T.MaxMagnitudeNumber(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> x, Vector128<T> y, Vector128<T> z)
        {
            return Vector128.Min(Vector128.Max(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> x, Vector256<T> y, Vector256<T> z)
        {
            return Vector256.Min(Vector256.Max(x, y), z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> x, Vector512<T> y, Vector512<T> z)
        {
            return Vector512.Min(Vector512.Max(x, y), z);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Min<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpMin<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Min<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpMin<T>>(left, TVector.Create(right));
    }

    private struct OpMin<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return Vector2.Min(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return Vector3.Min(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return Vector4.Min(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return T.MinMagnitudeNumber(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return Vector128.Min(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return Vector256.Min(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return Vector512.Min(left, right);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Max<TVector, T>(TVector left, TVector right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpMax<T>>(left, right);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVector Max<TVector, T>(TVector left, T right)
        where TVector : IVector<TVector, T>, IVectorInternal<TVector, T>
        where T : INumberBase<T>
    {
        return Operate<TVector, T, OpMax<T>>(left, TVector.Create(right));
    }

    private struct OpMax<T> : IBinaryOperator<T, T, T> where T : INumberBase<T>
    {
        public static bool HasNumerics
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Invoke(Vector2 left, Vector2 right)
        {
            return Vector2.Max(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Invoke(Vector3 left, Vector3 right)
        {
            return Vector3.Max(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Invoke(Vector4 left, Vector4 right)
        {
            return Vector4.Max(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Invoke(T left, T right)
        {
            return T.MinMagnitudeNumber(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> Invoke(Vector128<T> left, Vector128<T> right)
        {
            return Vector128.Max(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> Invoke(Vector256<T> left, Vector256<T> right)
        {
            return Vector256.Max(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> Invoke(Vector512<T> left, Vector512<T> right)
        {
            return Vector512.Max(left, right);
        }
    }

}