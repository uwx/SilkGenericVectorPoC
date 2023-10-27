Silk v3 Generic Math Proposal

Types to implement:
- Vector2D&lt;T>
- Vector3D&lt;T>
- Vector4D&lt;T>
- Quaternion&lt;T>
- Matrix3x2&lt;T>
- Matrix4x4&lt;T>
- Plane&lt;T>
- Point2&lt;T>
- Point3&lt;T>
- Point4&lt;T>

Only Vector3&lt;T> is included here, but the design decisions apply for most types.

NB:
- Struct is mutable here to match System.Numerics.Vector3. Could be changed.

```cs
public partial struct Vector3<T> : IEquatable<Vector3<T>>, IEquatable<System.Numerics.Vector3>, ISpanFormattable
    where T : INumber<T>
{
    public T X, Y, Z;
    public static Vector3<T> Zero { get; }
    public static Vector3<T> UnitX { get; }
    public static Vector3<T> UnitY { get; }
    public static Vector3<T> UnitZ { get; }
    // Not possible without scoping to IMinMaxValue<T>
    // public static Vector3<T> MinValue { get; }
    // public static Vector3<T> MaxValue { get; }
    public static Vector3<T> Infinity { get; } // for T is floating point number, return infinity, otherwise, throw or return arbitrary value (undetermined)
    public static Vector3<T> NegativeInfinity { get; } // for T is floating point number, return -infinity, otherwise, throw or return arbitrary value (undetermined)

    public T this[int index] { readonly get; set; }

    public Vector3(T value);
    public Vector3(T x, T y, T z);
    public Vector3(ReadOnlySpan<T> values);

    public readonly override string ToString();
    public readonly string ToString(string? format = null, IFormatProvider? formatProvider = null);
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);

    public readonly override bool Equals([NotNullWhen(true)] object? obj);
    public readonly bool Equals(Vector3<T> other);
    public readonly bool Equals(System.Numerics.Vector3 other);
    public readonly override int GetHashCode();

    public static bool operator ==(Vector3<T> left, Vector3<T> right);
    public static bool operator !=(Vector3<T> left, Vector3<T> right);

    // Maths operators
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator /(Vector3<T> left, T right);
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator *(Vector3<T> left, T right);
    public static Vector3<T> operator *(T left, Vector3<T> right);
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator -(Vector3<T> value);

    // Explicit cast for convenience
    public static explicit operator System.Numerics.Vector3(Vector3<T> self);
    public static explicit operator checked System.Numerics.Vector3(Vector3<T> self);
    public static explicit operator Vector3<T>(System.Numerics.Vector3 self);
    public static explicit operator checked Vector3<T>(System.Numerics.Vector3 self);

    private readonly Vector3<TOther> As<TOther>() where TOther : INumber<TOther>;
}
```

Outstanding design questions:
- Should the struct be readonly? System.Numerics.Vector3 is mutable.
- Should static methods be provided on:
    1. `Vector3<T>`. Some methods can't be put in the struct as they narrow the type of T further.
    ```cs
    public partial struct Vector3<T>
    {
        // Instance maths operations
        public readonly TReturn Length<TReturn>() where TReturn : INumber<TReturn>, IFloatingPoint<TReturn>, IRootFunctions<TReturn>;
        public readonly TReturn LengthSquared<TReturn>() where TReturn : INumber<TReturn>;

        // Maths operations
        public static Vector3<T> Abs(Vector3<T> value);
        public static Vector3<T> Add(Vector3<T> left, Vector3<T> right);
        public static Vector3<T> Clamp(Vector3<T> value1, Vector3<T> min, Vector3<T> max);
        public static Vector3<T> Cross(Vector3<T> vector1, Vector3<T> vector2);
        public static Vector3<T> Cross<TIntermediate>(Vector3<T> vector1, Vector3<T> vector2) where TIntermediate : INumber<TIntermediate>;
        public static TReturn Distance<TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn>, IRootFunctions<TReturn>;
        public static T DistanceSquared(Vector3<T> value1, Vector3<T> value2);
        public static TReturn DistanceSquared<TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn>;
        public static Vector3<T> Divide(Vector3<T> left, Vector3<T> right);
        public static Vector3<T> Divide(Vector3<T> left, T divisor);
        public static T Dot(Vector3<T> vector1, Vector3<T> vector2);
        public static TReturn Dot<TReturn>(Vector3<T> vector1, Vector3<T> vector2) where TReturn : INumber<TReturn>;
        public static Vector3<TFloat> Lerp<TFloat>(Vector3<T> value1, Vector3<T> value2, TFloat amount) where TFloat : INumber<TFloat>, IFloatingPoint<TFloat>;
        public static Vector3<T> Max(Vector3<T> value1, Vector3<T> value2);
        public static Vector3<T> Min(Vector3<T> value1, Vector3<T> value2);
        public static Vector3<T> Multiply(Vector3<T> left, Vector3<T> right);
        public static Vector3<T> Multiply(Vector3<T> left, T right);
        public static Vector3<T> Multiply(T left, Vector3<T> right);
        public static Vector3<T> Negate(Vector3<T> value);
        public static Vector3<TReturn> Normalize<TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn>;
        public static Vector3<TReturn> Reflect<TReturn>(Vector3<T> vector, Vector3<T> normal) where TReturn : IFloatingPoint<TReturn>;
        public static Vector3<TReturn> Sqrt<TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn>;
        public static Vector3<T> Subtract(Vector3<T> left, Vector3<T> right);
        public static Vector3<TReturn> Transform<TReturn>(Vector3<T> value, Quaternion rotation) where TReturn : INumber<TReturn>;
    }

    public partial static class Vector3
    {
        // Narrowed-type functions. These don't take a TReturn type as long as the type of T is a floating point, which means it can represent decimal values.
        public static T Length<T>(this Vector3<T> vec) where T : INumber<T>, IFloatingPoint<T>, IRootFunctions<T>;
        public static T LengthSquared<T>(this Vector3<T> vec) where T : INumber<T>;
        public static Vector3<T> Lerp<T>(Vector3<T> value1, Vector3<T> value2, T amount) where T : INumber<T>, IFloatingPoint<T>;
        public static Vector3<T> Normalize<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>;
        public static Vector3<T> Reflect<T>(Vector3<T> vector, Vector3<T> normal) where T : IFloatingPoint<T>;
        public static Vector3<T> Sqrt<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>;
        public static Vector3<T> Transform<T>(Vector3<T> value, Quaternion rotation) where T : IFloatingPoint<T>;

        public static Vector3<int> RoundToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>;
        public static Vector3<int> FloorToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>;
        public static Vector3<int> CeilingToInt<T>(Vector3<T> vector) where T : IFloatingPoint<T>;
        public static Vector3<float> AsGeneric(this System.Numerics.Vector3 vector);
        public static System.Numerics.Vector3 AsNumerics(this Vector3<float> vector);
    }
    ```

    2. `static Vector3`
    ```cs
    public partial static class Vector3
    {
        // Instance maths operations (as extension methods)
        public readonly TReturn Length<T, TReturn>(this Vector3<T> vector) where TReturn : INumber<TReturn>, IFloatingPoint<TReturn>, IRootFunctions<TReturn> where T : INumber<T>;
        public readonly TReturn LengthSquared<T, TReturn>(this Vector3<T> vector) where TReturn : INumber<TReturn> where T : INumber<T>;

        // Maths operations
        public static Vector3<T> Abs<T>(Vector3<T> value) where T : INumber<T>;
        public static Vector3<T> Add<T>(Vector3<T> left, Vector3<T> right) where T : INumber<T>;
        public static Vector3<T> Clamp<T>(Vector3<T> value1, Vector3<T> min, Vector3<T> max) where T : INumber<T>;
        public static Vector3<T> Cross<T>(Vector3<T> vector1, Vector3<T> vector2) where T : INumber<T>;
        public static Vector3<T> Cross<T, TIntermediate>(Vector3<T> vector1, Vector3<T> vector2) where TIntermediate : INumber<TIntermediate> where T : INumber<T>;
        public static TReturn Distance<T, TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn>, IRootFunctions<TReturn> where T : INumber<T>;
        public static T DistanceSquared<T>(Vector3<T> value1, Vector3<T> value2) where T : INumber<T>;
        public static TReturn DistanceSquared<T, TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn> where T : INumber<T>;
        public static Vector3<T> Divide<T>(Vector3<T> left, Vector3<T> right) where T : INumber<T>;
        public static Vector3<T> Divide<T>(Vector3<T> left, T divisor) where T : INumber<T>;
        public static T Dot<T>(Vector3<T> vector1, Vector3<T> vector2) where T : INumber<T>;
        public static TReturn Dot<T, TReturn>(Vector3<T> vector1, Vector3<T> vector2) where TReturn : INumber<TReturn> where T : INumber<T>;
        public static Vector3<TFloat> Lerp<T, TFloat>(Vector3<T> value1, Vector3<T> value2, TFloat amount) where TFloat : INumber<TFloat>, IFloatingPoint<TFloat> where T : INumber<T>;
        public static Vector3<T> Max<T>(Vector3<T> value1, Vector3<T> value2) where T : INumber<T>;
        public static Vector3<T> Min<T>(Vector3<T> value1, Vector3<T> value2) where T : INumber<T>;
        public static Vector3<T> Multiply<T>(Vector3<T> left, Vector3<T> right) where T : INumber<T>;
        public static Vector3<T> Multiply<T>(Vector3<T> left, T right) where T : INumber<T>;
        public static Vector3<T> Multiply<T>(T left, Vector3<T> right) where T : INumber<T>;
        public static Vector3<T> Negate<T>(Vector3<T> value) where T : INumber<T>;
        public static Vector3<TReturn> Normalize<T, TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn> where T : INumber<T>;
        public static Vector3<TReturn> Reflect<T, TReturn>(Vector3<T> vector, Vector3<T> normal) where TReturn : IFloatingPoint<TReturn> where T : INumber<T>;
        public static Vector3<TReturn> Sqrt<T, TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn> where T : INumber<T>;
        public static Vector3<T> Subtract<T>(Vector3<T> left, Vector3<T> right) where T : INumber<T>;
        public static Vector3<TReturn> Transform<T, TReturn>(Vector3<T> value, Quaternion rotation) where TReturn : INumber<TReturn> where T : INumber<T>;

        // Narrowed-type functions. These don't take a TReturn type as long as the type of T is a floating point, which means it can represent decimal values.
        public static T Length<T>(this Vector3<T> vec) where T : INumber<T>, IFloatingPoint<T>, IRootFunctions<T>;
        public static T LengthSquared<T>(this Vector3<T> vec) where T : INumber<T>;
        public static Vector3<T> Lerp<T>(Vector3<T> value1, Vector3<T> value2, T amount) where T : INumber<T>, IFloatingPoint<T>;
        public static Vector3<T> Normalize<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>;
        public static Vector3<T> Reflect<T>(Vector3<T> vector, Vector3<T> normal) where T : IFloatingPoint<T>;
        public static Vector3<T> Sqrt<T>(Vector3<T> value) where T : IFloatingPoint<T>, IRootFunctions<T>;
        public static Vector3<T> Transform<T>(Vector3<T> value, Quaternion rotation) where T : IFloatingPoint<T>;
        public static Vector3<float> AsGeneric(this System.Numerics.Vector3 vector);
        public static System.Numerics.Vector3 AsNumerics(this Vector3<float> vector);
    }
    ```
       Pros:
       - Calling a static method would be typed as `Vector3.Subtract(left, right)` instead of `Vector3.Subtract<T>(left, right)`

       Cons:
       - Calling a generic method would have to be typed as `Vector3.Length<T, TReturn>(left, right)`, which is a bit inconvenient
       - Extension methods like `Vector3.Length()` would have to be typed as `vec.Length<T, TReturn>()`
    3. Both (extension methods become instance methods on Vector3&lt;T> unless they require a specific constraint, in which case they only go on `static Vector3`, and every method has an equivalent on `static Vector3`)

       Pros:
       - You can use it how you like

       Cons:
       - It's a lot of code, and there isn't a single right way of doing things

Extension 1: IFloatingPointIeee754&lt;T> De-facto Implementation
```cs
public static partial class Vector3
{
    // Equivalent implementing IHyperbolicFunctions<Vector3<T>>
    public static Vector3<T> Acosh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Asinh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Atanh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Cosh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Sinh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Tanh<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;

    // Equivalent implementing ITrigonometricFunctions<Vector3<T>;
    public static Vector3<T> Acos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> AcosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Asin<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> AsinPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Atan<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> AtanPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Cos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> CosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
#if NET8_0
    public static Vector3<T> DegreesToRadians<T>(Vector3<T> degrees) where T : INumber<T>, ITrigonometricFunctions<T>;
    public static Vector3<T> RadiansToDegrees<T>(Vector3<T> radians) where T : INumber<T>, ITrigonometricFunctions<T>;
#endif
    public static Vector3<T> Sin<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> SinPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Tan<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> TanPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;


    public static (Vector3<T> Sin, Vector3<T> Cos) SinCos<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;

    public static (Vector3<T> SinPi, Vector3<T> CosPi) SinCosPi<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;

    // Equivalent implementing ILogarithmicFunctions<Vector3<T>>
    public static Vector3<T> Log<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Log<T>(Vector3<T> x, Vector3<T> newBase) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> LogP1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Log2<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Log2P1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Log10<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Log10P1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;

    // Equivalent implementing IExponentialFunctions<Vector3<T>>
    public static Vector3<T> Exp<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> ExpM1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Exp2<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Exp2M1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Exp10<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Exp10M1<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;

    // Equivalent implementing IPowerFunctions<Vector3<T>>
    public static Vector3<T> Pow<T>(Vector3<T> x, Vector3<T> y) where T : IFloatingPointIeee754<T>;

    // Equivalent implementing IRootFunctions<Vector3<T>>
    public static Vector3<T> Cbrt<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Hypot<T>(Vector3<T> x, Vector3<T> y) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> RootN<T>(Vector3<T> x, int n) where T : IFloatingPointIeee754<T>;
    public static Vector3<T> Sqrt<T>(Vector3<T> x) where T : IFloatingPointIeee754<T>;
}
```
- Because the type of T has to be narrowed to `IFloatingPointIeee754<T>`, we can't actually implement the interface on Vector3&lt;T>.
- Ideally, this should be vectorized. In practice, that's a lot of work. If https://github.com/dtnet/runtime/issues/93513 is merged, this could be made to use that.

Extension 3: INumber&lt;Vector3&lt;T>> Implementation
```cs
public partial struct Vector3<T> : INumber<Vector3<T>>
{
    public static int Radix { get; }
    public static Vector3<T> AdditiveIdentity { get; }
    public static Vector3<T> MultiplicativeIdentity { get; }

    public readonly int CompareTo(object? obj);
    public readonly int CompareTo(Vector3<T> other);
    public static Vector3<T> Parse(string s, IFormatProvider? provider);
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector3<T> result);
    public static Vector3<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector3<T> result);
    public static bool operator >(Vector3<T left, Vector3<T> right);
    public static bool operator >=(Vector3<T> left, Vector3<T> right);
    public static bool operator <(Vector3<T> left, Vector3<T> right);
    public static bool operator <=(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator --(Vector3<T> value);
    public static Vector3<T> operator ++(Vector3<T> value);
    public static Vector3<T> operator %(Vector3<T> left, Vector3<T> right);
    public static Vector3<T> operator +(Vecor3<T> value);
    public static bool IsCanonical(Vector3<T> value);
    public static bool IsComplexNumber(Vector3<T> value);
    public static bool IsEvenInteger(Vector3<T> value);
    public static bool IsFinite(Vector3<T> value);
    public static bool IsImaginaryNumber(Vector3<T> value);
    public static bool IsInfinity(Vector3<T> value);
    public static bool IsInteger(Vector3<T> value);
    public static bool IsNaN(Vector3<T> value);
    public static bool IsNegative(Vector3<T> value);
    public static bool IsNegativeInfinity(Vector3<T> value);
    public static bool IsNormal(Vector3<T> value);
    public static bool IsOddInteger(Vector3<T> value);
    public static bool IsPositive(Vector3<T> value);
    public static bool IsPositiveInfinity(Vector3<T> value);
    public static bool IsRealNumber(Vector3<T> value);
    public static bool IsSubnormal(Vector3<T> value);
    public static bool IsZero(Vector3<T> value);
    public static Vector3<T> MaxMagnitude(Vector3<T> x, Vector3<T> y);
    public static Vector3<T> MaxMagnitudeNumber(Vector3<T> x, Vector3<T> y);
    public static Vector3<T> MinMagnitude(Vector3<T> x, Vector3<T> y);
    public static Vector3<T> MinMagnitudeNumber(Vector3<T> x, Vector3<T> y);
    public static Vector3<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider);
    public static Vector3<T> Parse(string s, NumberStyles style, IFormatProvider? provider);
    public static bool TryConvertFromChecked<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>;
    public static bool TryConvertFromSaturating<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>;
    public static bool TryConvertFromTruncating<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>;
    public static bool TryConvertToChecked<TOther>(Vector3<T> value, out TOther result) where TOther : INumberBase<TOther>;
    public static bool TryConvertToSaturating<TOther>(Vector3<T> value, out TOther result) where TOther : INumberBase<TOther>;
    public static bool TryConvertToTruncating<TOther>(Vector3<T> value, out TOther result) where TOther : INumberBase<TOther>;
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result);
    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result);

#if NET8_0
    public static Vector3<T> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider);
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector3<T> result);
#endif
}
```
- We can implement the interface on Vector3&lt;T>, or the Vector3 static class.

Extension 4: Directional static properties
```cs
public partial struct Vector3<T>
{
    public static Vector3<T> Up { get; }
    public static Vector3<T> Down { get; }
    public static Vector3<T> Left { get; }
    public static Vector3<T> Right { get; }
    public static Vector3<T> Forward { get; }
    public static Vector3<T> Backward { get; }
}
```
- Coordinate-system dependent. Probably a bad idea.

Extension 5: Convenience methods
```cs
public partial struct Vector3<T>
{
    // Lerp clamped to range TFloat.Zero-TFloat.One
    public static Vector3<TFloat> LerpClamped<TFloat>(Vector3<T> value1, Vector3<T> value2, TFloat amount) where TFloat : INumber<TFloat>, IFloatingPoint<TFloat>;
}

public static partial class Vector3
{
    // Lerp clamped to range TFloat.Zero-TFloat.One
    public static Vector3<T> LerpClamped<T>(Vector3<T> value1, Vector3<T> value2, T amount) where T : INumber<T>, IFloatingPoint<T>;
}
```

Extension 6: IUtf8SpanFormattable (.NET 8)
```cs
public partial struct Vector3<T> : IUtf8SpanFormattable, IUtf8SpanParsable<Vector3<T>>
{
    public static Vector3<T> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider);
    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector3<T> result);
    public readonly bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
}
```
- IUtf8SpanParsable is already implemented by INumberBase&lt;T> on .NET 8, but we override the default implementation with a faster, direct-to-UTF8 one

Other ideas:
- Implicit cast to Vector2&lt;T>, drop the Z component


sqrt -> public static Vector3<TReturn> Sqrt<TReturn>(Vector3<T> value) where TReturn : INumber<TReturn>, IRootFunctions<TReturn>
length -> public TReturn Length<TReturn>() where TReturn : INumber<TReturn>, IRootFunctions<TReturn>
lengthsquared -> public TReturn LengthSquared<TReturn>() where TReturn : INumber<TReturn>
lengthsquared (ext) -> public static T LengthSquared<T>(this Vector3<T> vec) where T : INumber<T>
length (ext0) -> public static T Length<T>(this Vector3<T> vec) where T : INumber<T>, IRootFunctions<T>

Lerp is constrained to IFloatingPoint becuase amount is >0 <1