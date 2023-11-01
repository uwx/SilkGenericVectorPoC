using System.Numerics;
using System.Runtime.Intrinsics;

namespace GenericVector.Experimental;

internal class SpeedHelpers2
{
    private interface IOp1<TIn, TOut>
    {
        TOut Invoke(TIn input);
        Vector128<TOut> Invoke(Vector64<TIn> input);
        Vector128<TOut> Invoke(Vector128<TIn> input);
        Vector128<TOut> Invoke(Vector256<TIn> input);
        Vector128<TOut> Invoke(Vector512<TIn> input);
    }

    private interface IOp2<TLeft, TRight, TOut>
    {
        TOut Invoke(TLeft left, TRight right);
        Vector128<TOut> Invoke(Vector64<TLeft> left, Vector64<TRight> right);
        Vector128<TOut> Invoke(Vector128<TLeft> left, Vector128<TRight> right);
        Vector128<TOut> Invoke(Vector256<TLeft> left, Vector256<TRight> right);
        Vector128<TOut> Invoke(Vector512<TLeft> left, Vector512<TRight> right);
    }

    private interface IOpAccum<TIn, TOut>
    {
        TOut Invoke(TIn input);
        TOut Invoke(Vector64<TIn> input);
        TOut Invoke(Vector128<TIn> input);
        TOut Invoke(Vector256<TIn> input);
        TOut Invoke(Vector512<TIn> input);
    }

    private struct OpTemplate2<T> : IOp2<T, T, T>
    {
        public T Invoke(T left, T right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector64<T> left, Vector64<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector256<T> left, Vector256<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector512<T> left, Vector512<T> right) => throw new NotImplementedException();
    }

    private struct OpTemplate1<T> : IOp1<T, T>
    {
        public T Invoke(T input) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector64<T> input) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector128<T> input) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector256<T> input) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector512<T> input) => throw new NotImplementedException();
    }
    
    public static TVector Add<TVector, TScalar>(TVector left, TVector right)
        where TVector : IVector<TVector, TScalar>
        where TScalar : IBinaryInteger<TScalar>
    {
        throw new NotImplementedException();
    }
    
    
    private struct OpAdd<T> : IOp2<T, T, T> where T : IAdditionOperators<T, T, T>
    {
        public T Invoke(T left, T right) => left + right;
        public Vector128<T> Invoke(Vector64<T> left, Vector64<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector256<T> left, Vector256<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector512<T> left, Vector512<T> right) => throw new NotImplementedException();
    }

    private struct OpSub<T> : IOp2<T, T, T> where T : ISubtractionOperators<T, T, T>
    {
        public T Invoke(T left, T right) => left - right;
        public Vector128<T> Invoke(Vector64<T> left, Vector64<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector256<T> left, Vector256<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector512<T> left, Vector512<T> right) => throw new NotImplementedException();
    }

    private struct OpMult<T> : IOp2<T, T, T> where T : IMultiplyOperators<T, T, T>
    {
        public T Invoke(T left, T right) => left * right;
        public Vector128<T> Invoke(Vector64<T> left, Vector64<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector128<T> left, Vector128<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector256<T> left, Vector256<T> right) => throw new NotImplementedException();
        public Vector128<T> Invoke(Vector512<T> left, Vector512<T> right) => throw new NotImplementedException();
    }

    public static Vector2i<TScalar> operator +(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Add<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator -(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Subtract<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator *(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Multiply<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator /(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Divide<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator %(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.Remainder<Vector2i<TScalar>, TScalar>(left, right);

    public static Vector2i<TScalar> operator *(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Multiply<Vector2i<TScalar>, TScalar>(left, right); 
    public static Vector2i<TScalar> operator /(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Divide<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator %(Vector2i<TScalar> left, TScalar right) => SpeedHelpers2.Remainder<Vector2i<TScalar>, TScalar>(left, right);
    
    public static Vector2i<TScalar> operator *(TScalar left, Vector2i<TScalar> right) => right * left;

    public static Vector2i<TScalar> operator -(Vector2i<TScalar> value) => SpeedHelpers2.Negate<Vector2i<TScalar>, TScalar>(value);
    public static Vector2i<TScalar> operator +(Vector2i<TScalar> value) => value;
    
    public static Vector2i<TScalar> operator &(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseAnd<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator |(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseOr<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator ^(Vector2i<TScalar> left, Vector2i<TScalar> right) => SpeedHelpers2.BitwiseXor<Vector2i<TScalar>, TScalar>(left, right);
    public static Vector2i<TScalar> operator ~(Vector2i<TScalar> value) => SpeedHelpers2.BitwiseNot<Vector2i<TScalar>, TScalar>(value);

}