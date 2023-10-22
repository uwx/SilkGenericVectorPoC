using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace GenericVector;

file interface IVec3
{
    // Returns null if incompatible. Throws OverflowException if overflowing
    Vector3D<T>? GetChecked<T>() where T : INumberBase<T>;
    Vector3D<T>? GetSaturating<T>() where T : INumberBase<T>;
    Vector3D<T>? GetTruncating<T>() where T : INumberBase<T>;
}

public partial struct Vector3D<T> :
    IDivisionOperators<Vector3D<T>, T, Vector3D<T>>,
    IMultiplyOperators<Vector3D<T>, T, Vector3D<T>>,
    INumberBase<Vector3D<T>>,
    IVec3
{
    static Vector3D<T> IParsable<Vector3D<T>>.Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), NumberStyles.None, provider);

    static Vector3D<T> ISpanParsable<Vector3D<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s, NumberStyles.None, provider);

    public static Vector3D<T> Parse(string s, NumberStyles style = default, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), style, provider);

    public static Vector3D<T> Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.None, IFormatProvider? provider = null)
        => TryParse(s, style, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector3)}<{typeof(T)}>");

    public static bool TryParse(string? s, IFormatProvider? provider, out Vector3D<T> result)
        => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vector3D<T> result)
        => TryParse(s.AsSpan(), style, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector3D<T> result)
        => TryParse(s, NumberStyles.None, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector3D<T> result)
    {
        result = default;

        if (s[0] != '<') return false;
        if (s[^1] != '>') return false;
        
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;

        s = s[1..^1];

        T? x, y, z;

        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style, provider, out x)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], style,provider, out y)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, style,provider, out z)) return false;
        }

        result = new Vector3D<T>(x, y, z);
        return true;
    }

    static Vector3D<T> IAdditiveIdentity<Vector3D<T>, Vector3D<T>>.AdditiveIdentity => Zero;
    static Vector3D<T> IMultiplicativeIdentity<Vector3D<T>, Vector3D<T>>.MultiplicativeIdentity => One;

    static Vector3D<T> IDecrementOperators<Vector3D<T>>.operator --(Vector3D<T> value) => value - One;
    static Vector3D<T> IIncrementOperators<Vector3D<T>>.operator ++(Vector3D<T> value) => value + One;

    static Vector3D<T> IUnaryPlusOperators<Vector3D<T>, Vector3D<T>>.operator +(Vector3D<T> value) => value;

    static bool INumberBase<Vector3D<T>>.IsCanonical(Vector3D<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z);

    static bool INumberBase<Vector3D<T>>.IsComplexNumber(Vector3D<T> value) => T.IsComplexNumber(value.X) || T.IsComplexNumber(value.Y) || T.IsComplexNumber(value.Z);

    static bool INumberBase<Vector3D<T>>.IsEvenInteger(Vector3D<T> value) => T.IsEvenInteger(value.X) && T.IsEvenInteger(value.Y) && T.IsEvenInteger(value.Z);

    static bool INumberBase<Vector3D<T>>.IsFinite(Vector3D<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z);

    static bool INumberBase<Vector3D<T>>.IsImaginaryNumber(Vector3D<T> value) => T.IsImaginaryNumber(value.X) || T.IsImaginaryNumber(value.Y) || T.IsImaginaryNumber(value.Z);

    static bool INumberBase<Vector3D<T>>.IsInfinity(Vector3D<T> value) => T.IsInfinity(value.X) && T.IsInfinity(value.Y) && T.IsInfinity(value.Z);

    static bool INumberBase<Vector3D<T>>.IsInteger(Vector3D<T> value) => T.IsInteger(value.X) && T.IsInteger(value.Y) && T.IsInteger(value.Z);

    static bool INumberBase<Vector3D<T>>.IsNaN(Vector3D<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z);

    static bool INumberBase<Vector3D<T>>.IsNegative(Vector3D<T> value) => T.IsNegative(value.X) && T.IsNegative(value.Y) && T.IsNegative(value.Z);

    static bool INumberBase<Vector3D<T>>.IsNegativeInfinity(Vector3D<T> value) => T.IsNegativeInfinity(value.X) && T.IsNegativeInfinity(value.Y) && T.IsNegativeInfinity(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsNormal(Vector3D<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsOddInteger(Vector3D<T> value) => T.IsOddInteger(value.X) && T.IsOddInteger(value.Y) && T.IsOddInteger(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsPositive(Vector3D<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsPositiveInfinity(Vector3D<T> value) => T.IsPositiveInfinity(value.X) && T.IsPositiveInfinity(value.Y) && T.IsPositiveInfinity(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsRealNumber(Vector3D<T> value) => T.IsRealNumber(value.X) && T.IsRealNumber(value.Y) && T.IsRealNumber(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsSubnormal(Vector3D<T> value) => T.IsSubnormal(value.X) && T.IsSubnormal(value.Y) && T.IsSubnormal(value.Z); 

    static bool INumberBase<Vector3D<T>>.IsZero(Vector3D<T> value) => T.IsZero(value.X) && T.IsZero(value.Y) && T.IsZero(value.Z);

    static Vector3D<T> INumberBase<Vector3D<T>>.MaxMagnitude(Vector3D<T> x, Vector3D<T> y)
    {
        return new Vector3D<T>(
            T.MaxMagnitude(x.X, y.X),
            T.MaxMagnitude(x.Y, y.Y),
            T.MaxMagnitude(x.Z, y.Z)
        );
    }

    static Vector3D<T> INumberBase<Vector3D<T>>.MaxMagnitudeNumber(Vector3D<T> x, Vector3D<T> y)
    {
        return new Vector3D<T>(
            T.MaxMagnitudeNumber(x.X, y.X),
            T.MaxMagnitudeNumber(x.Y, y.Y),
            T.MaxMagnitudeNumber(x.Z, y.Z)
        );
    }

    static Vector3D<T> INumberBase<Vector3D<T>>.MinMagnitude(Vector3D<T> x, Vector3D<T> y)
    {
        return new Vector3D<T>(
            T.MaxMagnitude(x.X, y.X),
            T.MaxMagnitude(x.Y, y.Y),
            T.MaxMagnitude(x.Z, y.Z)
        );
    }

    static Vector3D<T> INumberBase<Vector3D<T>>.MinMagnitudeNumber(Vector3D<T> x, Vector3D<T> y)
    {
        return new Vector3D<T>(
            T.MaxMagnitudeNumber(x.X, y.X),
            T.MaxMagnitudeNumber(x.Y, y.Y),
            T.MaxMagnitudeNumber(x.Z, y.Z)
        );
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, out Vector3D<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec3 ivec3 && ivec3.GetChecked<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Vector3D<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec3 ivec3 && ivec3.GetSaturating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Vector3D<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3D<T> v)
        {
            result = v;
            return true;
        }

        if (value is IVec3 ivec3 && ivec3.GetTruncating<T>() is {} r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryConvertToChecked<TOther>(Vector3D<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Vector3D<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromSaturating(value, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Vector3D<T> value, [MaybeNullWhen(false)]out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromTruncating(value, out result);
    }

    static int INumberBase<Vector3D<T>>.Radix => T.Radix;

    Vector3D<T1>? IVec3.GetChecked<T1>() => T1.TryConvertFromChecked(X, out var x) ? new(x, T1.CreateChecked(Y), T1.CreateChecked(Z)) : null;
    Vector3D<T1>? IVec3.GetSaturating<T1>() => T1.TryConvertFromSaturating(X, out var x) ? new(x, T1.CreateSaturating(Y), T1.CreateSaturating(Z)) : null;
    Vector3D<T1>? IVec3.GetTruncating<T1>() => T1.TryConvertFromTruncating(X, out var x) ? new(x, T1.CreateTruncating(Y), T1.CreateTruncating(Z)) : null;
}