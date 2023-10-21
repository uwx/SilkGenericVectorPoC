using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace GenericVector;

file interface IVec3
{
    // Returns null if incompatible. Throws OverflowException if overflowing
    Vector3<T>? GetChecked<T>() where T : INumber<T>;
    Vector3<T>? GetSaturating<T>() where T : INumber<T>;
    Vector3<T>? GetTruncating<T>() where T : INumber<T>;
}

public partial struct Vector3<T> : INumber<Vector3<T>>, IVec3
{
    public readonly int CompareTo(object? obj) => obj is Vector3<T> vec ? CompareTo(vec) : 0; 

    public readonly int CompareTo(Vector3<T> other)
    {
        if (X.CompareTo(other.X) is var xComp and not 0) return xComp;
        if (Y.CompareTo(other.Y) is var yComp and not 0) return yComp;
        return Z.CompareTo(other.Z);
    }

    public static Vector3<T> Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);

    public static Vector3<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
        => Parse(s.AsSpan(), style, provider);

    public static Vector3<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s, NumberStyles.None, provider);

    public static Vector3<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        => TryParse(s, style, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector3)}<{typeof(T)}>");

    public static bool TryParse(string? s, IFormatProvider? provider, out Vector3<T> result)
        => TryParse(s.AsSpan(), NumberStyles.None, provider, out result);

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result)
        => TryParse(s.AsSpan(), style, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector3<T> result)
        => TryParse(s, NumberStyles.None, provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result)
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

        result = new Vector3<T>(x, y, z);
        return true;
    }

    static Vector3<T> IAdditiveIdentity<Vector3<T>, Vector3<T>>.AdditiveIdentity => Zero;
    static Vector3<T> IMultiplicativeIdentity<Vector3<T>, Vector3<T>>.MultiplicativeIdentity => One;

    public static bool operator >(Vector3<T> left, Vector3<T> right) => left.X > right.X || left.Y > right.Y || left.Z > right.Z;
    public static bool operator >=(Vector3<T> left, Vector3<T> right) => left.X >= right.X || left.Y >= right.Y || left.Z >= right.Z;
    public static bool operator <(Vector3<T> left, Vector3<T> right) => left.X < right.X || left.Y < right.Y || left.Z < right.Z;
    public static bool operator <=(Vector3<T> left, Vector3<T> right) => left.X <= right.X || left.Y <= right.Y || left.Z <= right.Z;

    static Vector3<T> IDecrementOperators<Vector3<T>>.operator --(Vector3<T> value) => value - One;
    static Vector3<T> IIncrementOperators<Vector3<T>>.operator ++(Vector3<T> value) => value + One;

    public static Vector3<T> operator %(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X % right.X,
            left.Y % right.Y,
            left.Z % right.Z
        );
    }

    public static Vector3<T> operator %(Vector3<T> left, T right)
    {
        return new Vector3<T>(
            left.X % right,
            left.Y % right,
            left.Z % right
        );
    }

    static Vector3<T> IUnaryPlusOperators<Vector3<T>, Vector3<T>>.operator +(Vector3<T> value) => value;

    static bool INumberBase<Vector3<T>>.IsCanonical(Vector3<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z);

    static bool INumberBase<Vector3<T>>.IsComplexNumber(Vector3<T> value) => false;

    static bool INumberBase<Vector3<T>>.IsEvenInteger(Vector3<T> value) => T.IsEvenInteger(value.X) && T.IsEvenInteger(value.Y) && T.IsEvenInteger(value.Z);

    static bool INumberBase<Vector3<T>>.IsFinite(Vector3<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z);

    static bool INumberBase<Vector3<T>>.IsImaginaryNumber(Vector3<T> value) => false;

    static bool INumberBase<Vector3<T>>.IsInfinity(Vector3<T> value) => T.IsInfinity(value.X) && T.IsInfinity(value.Y) && T.IsInfinity(value.Z);

    static bool INumberBase<Vector3<T>>.IsInteger(Vector3<T> value) => T.IsInteger(value.X) && T.IsInteger(value.Y) && T.IsInteger(value.Z);

    public static bool IsNaN(Vector3<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z);

    static bool INumberBase<Vector3<T>>.IsNegative(Vector3<T> value) => T.IsNegative(value.X) && T.IsNegative(value.Y) && T.IsNegative(value.Z);

    static bool INumberBase<Vector3<T>>.IsNegativeInfinity(Vector3<T> value) => T.IsNegativeInfinity(value.X) && T.IsNegativeInfinity(value.Y) && T.IsNegativeInfinity(value.Z); 

    static bool INumberBase<Vector3<T>>.IsNormal(Vector3<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z); 

    static bool INumberBase<Vector3<T>>.IsOddInteger(Vector3<T> value) => T.IsOddInteger(value.X) && T.IsOddInteger(value.Y) && T.IsOddInteger(value.Z); 

    static bool INumberBase<Vector3<T>>.IsPositive(Vector3<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z); 

    static bool INumberBase<Vector3<T>>.IsPositiveInfinity(Vector3<T> value) => T.IsPositiveInfinity(value.X) && T.IsPositiveInfinity(value.Y) && T.IsPositiveInfinity(value.Z); 

    static bool INumberBase<Vector3<T>>.IsRealNumber(Vector3<T> value) => T.IsRealNumber(value.X) && T.IsRealNumber(value.Y) && T.IsRealNumber(value.Z); 

    static bool INumberBase<Vector3<T>>.IsSubnormal(Vector3<T> value) => T.IsSubnormal(value.X) && T.IsSubnormal(value.Y) && T.IsSubnormal(value.Z); 

    static bool INumberBase<Vector3<T>>.IsZero(Vector3<T> value) => T.IsZero(value.X) && T.IsZero(value.Y) && T.IsZero(value.Z);

    static Vector3<T> INumberBase<Vector3<T>>.MaxMagnitude(Vector3<T> x, Vector3<T> y) => x > y ? x : y;

    static Vector3<T> INumberBase<Vector3<T>>.MaxMagnitudeNumber(Vector3<T> x, Vector3<T> y) => IsNaN(x) ? y : IsNaN(y) ? x : x > y ? x : y;

    static Vector3<T> INumberBase<Vector3<T>>.MinMagnitude(Vector3<T> x, Vector3<T> y) => x < y ? x : y;

    static Vector3<T> INumberBase<Vector3<T>>.MinMagnitudeNumber(Vector3<T> x, Vector3<T> y) => IsNaN(x) ? y : IsNaN(y) ? x : x < y ? x : y;

    public static bool TryConvertFromChecked<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3<T> v)
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

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3<T> v)
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

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Vector3<T> result) where TOther : INumberBase<TOther>
    {
        if (value is Vector3<T> v)
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

    public static bool TryConvertToChecked<TOther>(Vector3<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Vector3<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromSaturating(value, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Vector3<T> value, [MaybeNullWhen(false)]out TOther result) where TOther : INumberBase<TOther>
    {
        return TOther.TryConvertFromTruncating(value, out result);
    }

    static int INumberBase<Vector3<T>>.Radix => T.Radix;
    
    readonly Vector3<T1>? IVec3.GetChecked<T1>() => T1.TryConvertFromChecked(X, out var x) ? new(x, T1.CreateChecked(Y), T1.CreateChecked(Z)) : null; 
    readonly Vector3<T1>? IVec3.GetSaturating<T1>() => T1.TryConvertFromSaturating(X, out var x) ? new(x, T1.CreateSaturating(Y), T1.CreateSaturating(Z)) : null; 
    readonly Vector3<T1>? IVec3.GetTruncating<T1>() => T1.TryConvertFromTruncating(X, out var x) ? new(x, T1.CreateTruncating(Y), T1.CreateTruncating(Z)) : null; 
}