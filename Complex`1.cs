﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GenericVector;

/// <summary>
/// A complex number z is a number of the form z = x + yi, where x and y
/// are real numbers, and i is the imaginary unit, with the property i2= -1.
/// </summary>
[Serializable]
public readonly struct Complex<T> :
    ISignedNumber<Complex<T>>,
    IUtf8SpanFormattable
    where T : IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
{
    private const NumberStyles DefaultNumberStyle = NumberStyles.Float | NumberStyles.AllowThousands;

    private const NumberStyles InvalidNumberStyles = ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
        | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign
        | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowThousands | NumberStyles.AllowExponent
        | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

    public static readonly Complex<T> Zero = new(T.Zero, T.Zero);
    public static readonly Complex<T> One = new(T.One, T.One);
    public static readonly Complex<T> ImaginaryOne = new(T.Zero, T.One);
    public static readonly Complex<T> NaN = new(T.NaN, T.NaN);
    public static readonly Complex<T> Infinity = new(T.PositiveInfinity, T.PositiveInfinity);

    private static readonly T InverseOfLog10 =
        T.One / T.Log(T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One + T.One); // 1 / Log(10)

    // This is the largest x T which (Hypot(x,x) + x) will not overflow. It is used for branching inside Sqrt.
    private static readonly T s_sqrtRescaleThreshold = T.MaxValue / (T.Sqrt(NumericConstants<T>.Two) + T.One);

    // This is the largest x for which 2 x^2 will not overflow. It is used for branching inside Asin and Acos.
    private static readonly T s_asinOverflowThreshold = T.Sqrt(T.MaxValue) / NumericConstants<T>.Two;

    // This value is used inside Asin and Acos.
    private static readonly T s_log2 = T.Log(NumericConstants<T>.Two);

    // Do not rename, these fields are needed for binary serialization
    private readonly T m_real; // Do not rename (binary serialization)
    private readonly T m_imaginary; // Do not rename (binary serialization)

    public Complex(T real, T imaginary)
    {
        m_real = real;
        m_imaginary = imaginary;
    }

    public T Real => m_real;
    public T Imaginary => m_imaginary;

    public T Magnitude => Abs(this);
    public T Phase => T.Atan2(m_imaginary, m_real);

    public static Complex<T> FromPolarCoordinates(T magnitude, T phase)
    {
        return new Complex<T>(magnitude * T.Cos(phase), magnitude * T.Sin(phase));
    }

    public static Complex<T> Negate(Complex<T> value)
    {
        return -value;
    }

    public static Complex<T> Add(Complex<T> left, Complex<T> right)
    {
        return left + right;
    }

    public static Complex<T> Add(Complex<T> left, T right)
    {
        return left + right;
    }

    public static Complex<T> Add(T left, Complex<T> right)
    {
        return left + right;
    }

    public static Complex<T> Subtract(Complex<T> left, Complex<T> right)
    {
        return left - right;
    }

    public static Complex<T> Subtract(Complex<T> left, T right)
    {
        return left - right;
    }

    public static Complex<T> Subtract(T left, Complex<T> right)
    {
        return left - right;
    }

    public static Complex<T> Multiply(Complex<T> left, Complex<T> right)
    {
        return left * right;
    }

    public static Complex<T> Multiply(Complex<T> left, T right)
    {
        return left * right;
    }

    public static Complex<T> Multiply(T left, Complex<T> right)
    {
        return left * right;
    }

    public static Complex<T> Divide(Complex<T> dividend, Complex<T> divisor)
    {
        return dividend / divisor;
    }

    public static Complex<T> Divide(Complex<T> dividend, T divisor)
    {
        return dividend / divisor;
    }

    public static Complex<T> Divide(T dividend, Complex<T> divisor)
    {
        return dividend / divisor;
    }

    public static Complex<T> operator -(Complex<T> value) /* Unary negation of a complex number */
    {
        return new Complex<T>(-value.m_real, -value.m_imaginary);
    }

    public static Complex<T> operator +(Complex<T> left, Complex<T> right)
    {
        return new Complex<T>(left.m_real + right.m_real, left.m_imaginary + right.m_imaginary);
    }

    public static Complex<T> operator +(Complex<T> left, T right)
    {
        return new Complex<T>(left.m_real + right, left.m_imaginary);
    }

    public static Complex<T> operator +(T left, Complex<T> right)
    {
        return new Complex<T>(left + right.m_real, right.m_imaginary);
    }

    public static Complex<T> operator -(Complex<T> left, Complex<T> right)
    {
        return new Complex<T>(left.m_real - right.m_real, left.m_imaginary - right.m_imaginary);
    }

    public static Complex<T> operator -(Complex<T> left, T right)
    {
        return new Complex<T>(left.m_real - right, left.m_imaginary);
    }

    public static Complex<T> operator -(T left, Complex<T> right)
    {
        return new Complex<T>(left - right.m_real, -right.m_imaginary);
    }

    public static Complex<T> operator *(Complex<T> left, Complex<T> right)
    {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        T result_realpart = (left.m_real * right.m_real) - (left.m_imaginary * right.m_imaginary);
        T result_imaginarypart = (left.m_imaginary * right.m_real) + (left.m_real * right.m_imaginary);
        return new Complex<T>(result_realpart, result_imaginarypart);
    }

    public static Complex<T> operator *(Complex<T> left, T right)
    {
        if (!T.IsFinite(left.m_real))
        {
            if (!T.IsFinite(left.m_imaginary))
            {
                return new Complex<T>(T.NaN, T.NaN);
            }

            return new Complex<T>(left.m_real * right, T.NaN);
        }

        if (!T.IsFinite(left.m_imaginary))
        {
            return new Complex<T>(T.NaN, left.m_imaginary * right);
        }

        return new Complex<T>(left.m_real * right, left.m_imaginary * right);
    }

    public static Complex<T> operator *(T left, Complex<T> right)
    {
        if (!T.IsFinite(right.m_real))
        {
            if (!T.IsFinite(right.m_imaginary))
            {
                return new Complex<T>(T.NaN, T.NaN);
            }

            return new Complex<T>(left * right.m_real, T.NaN);
        }

        if (!T.IsFinite(right.m_imaginary))
        {
            return new Complex<T>(T.NaN, left * right.m_imaginary);
        }

        return new Complex<T>(left * right.m_real, left * right.m_imaginary);
    }

    public static Complex<T> operator /(Complex<T> left, Complex<T> right)
    {
        // Division : Smith's formula.
        T a = left.m_real;
        T b = left.m_imaginary;
        T c = right.m_real;
        T d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (T.Abs(d) < T.Abs(c))
        {
            T doc = d / c;
            return new Complex<T>((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
        }
        else
        {
            T cod = c / d;
            return new Complex<T>((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
        }
    }

    public static Complex<T> operator /(Complex<T> left, T right)
    {
        // IEEE prohibit optimizations which are value changing
        // so we make sure that behaviour for the simplified version exactly match
        // full version.
        if (right == T.Zero)
        {
            return new Complex<T>(T.NaN, T.NaN);
        }

        if (!T.IsFinite(left.m_real))
        {
            if (!T.IsFinite(left.m_imaginary))
            {
                return new Complex<T>(T.NaN, T.NaN);
            }

            return new Complex<T>(left.m_real / right, T.NaN);
        }

        if (!T.IsFinite(left.m_imaginary))
        {
            return new Complex<T>(T.NaN, left.m_imaginary / right);
        }

        // Here the actual optimized version of code.
        return new Complex<T>(left.m_real / right, left.m_imaginary / right);
    }

    public static Complex<T> operator /(T left, Complex<T> right)
    {
        // Division : Smith's formula.
        T a = left;
        T c = right.m_real;
        T d = right.m_imaginary;

        // Computing c * c + d * d will overflow even in cases where the actual result of the division does not overflow.
        if (T.Abs(d) < T.Abs(c))
        {
            T doc = d / c;
            return new Complex<T>(a / (c + d * doc), (-a * doc) / (c + d * doc));
        }
        else
        {
            T cod = c / d;
            return new Complex<T>(a * cod / (d + c * cod), -a / (d + c * cod));
        }
    }

    public static T Abs(Complex<T> value)
    {
        return Hypot(value.m_real, value.m_imaginary);
    }

    private static T Hypot(T a, T b)
    {
        // Using
        //   sqrt(a^2 + b^2) = |a| * sqrt(1 + (b/a)^2)
        // we can factor out the larger component to dodge overflow even when a * a would overflow.

        a = T.Abs(a);
        b = T.Abs(b);

        T small, large;
        if (a < b)
        {
            small = a;
            large = b;
        }
        else
        {
            small = b;
            large = a;
        }

        if (small == T.Zero)
        {
            return (large);
        }
        else if (T.IsPositiveInfinity(large) && !T.IsNaN(small))
        {
            // The NaN test is necessary so we don't return +inf when small=NaN and large=+inf.
            // NaN in any other place returns NaN without any special handling.
            return (T.PositiveInfinity);
        }
        else
        {
            T ratio = small / large;
            return (large * T.Sqrt(T.One + ratio * ratio));
        }
    }

    private static T Log1P(T x)
    {
        // Compute log(1 + x) without loss of accuracy when x is small.

        // Our only use case so far is for positive values, so this isn't coded to handle negative values.
        Debug.Assert((x >= T.Zero) || T.IsNaN(x));

        T xp1 = T.One + x;
        if (xp1 == T.One)
        {
            return x;
        }
        else if (x < NumericConstants<T>.ThreeQuarters)
        {
            // This is accurate to within 5 ulp with any floating-point system that uses a guard digit,
            // as proven in Theorem 4 of "What Every Computer Scientist Should Know About Floating-Point
            // Arithmetic" (https://docs.oracle.com/cd/E19957-01/806-3568/ncg_goldberg.html)
            return x * T.Log(xp1) / (xp1 - T.One);
        }
        else
        {
            return T.Log(xp1);
        }
    }

    public static Complex<T> Conjugate(Complex<T> value)
    {
        // Conjugate of a Complex<T> number: the conjugate of x+i*y is x-i*y
        return new Complex<T>(value.m_real, -value.m_imaginary);
    }

    public static Complex<T> Reciprocal(Complex<T> value)
    {
        // Reciprocal of a Complex<T> number : the reciprocal of x+i*y is 1/(x+i*y)
        if (value.m_real == T.Zero && value.m_imaginary == T.Zero)
        {
            return Zero;
        }

        return One / value;
    }

    public static bool operator ==(Complex<T> left, Complex<T> right)
    {
        return left.m_real == right.m_real && left.m_imaginary == right.m_imaginary;
    }

    public static bool operator !=(Complex<T> left, Complex<T> right)
    {
        return left.m_real != right.m_real || left.m_imaginary != right.m_imaginary;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Complex<T> other && Equals(other);
    }

    public bool Equals(Complex<T> value)
    {
        return m_real.Equals(value.m_real) && m_imaginary.Equals(value.m_imaginary);
    }

    public override int GetHashCode() => HashCode.Combine(m_real, m_imaginary);

    public override string ToString() => ToString(null, null);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
        => ToString(format, null);

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(
        [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider
    )
    {
        // $"<{m_real.ToString(format, provider)}; {m_imaginary.ToString(format, provider)}>";
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(m_real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(m_imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
    }

    public static Complex<T> Sin(Complex<T> value)
    {
        // We need both sinh and cosh of imaginary part. To avoid multiple calls to T.Exp with the same value,
        // we compute them both here from a single call to T.Exp.
        T p = T.Exp(value.m_imaginary);
        T q = T.One / p;
        T sinh = (p - q) * NumericConstants<T>.Half;
        T cosh = (p + q) * NumericConstants<T>.Half;
        return new Complex<T>(T.Sin(value.m_real) * cosh, T.Cos(value.m_real) * sinh);
        // There is a known limitation with this algorithm: inputs that cause sinh and cosh to overflow, but for
        // which sin or cos are small enough that sin * cosh or cos * sinh are still representable, nonetheless
        // produce overflow. For example, Sin((0.01, 711.0)) should produce (~3.0E306, PositiveInfinity), but
        // instead produces (PositiveInfinity, PositiveInfinity).
    }

    public static Complex<T> Sinh(Complex<T> value)
    {
        // Use sinh(z) = -i sin(iz) to compute via sin(z).
        Complex<T> sin = Sin(new Complex<T>(-value.m_imaginary, value.m_real));
        return new Complex<T>(sin.m_imaginary, -sin.m_real);
    }

    public static Complex<T> Asin(Complex<T> value)
    {
        T b, bPrime, v;
        Asin_Internal(T.Abs(value.Real), T.Abs(value.Imaginary), out b, out bPrime, out v);

        T u;
        if (bPrime < T.Zero)
        {
            u = T.Asin(b);
        }
        else
        {
            u = T.Atan(bPrime);
        }

        if (value.Real < T.Zero) u = -u;
        if (value.Imaginary < T.Zero) v = -v;

        return new Complex<T>(u, v);
    }

    public static Complex<T> Cos(Complex<T> value)
    {
        T p = T.Exp(value.m_imaginary);
        T q = T.One / p;
        T sinh = (p - q) * NumericConstants<T>.Half;
        T cosh = (p + q) * NumericConstants<T>.Half;
        return new Complex<T>(T.Cos(value.m_real) * cosh, -T.Sin(value.m_real) * sinh);
    }

    public static Complex<T> Cosh(Complex<T> value)
    {
        // Use cosh(z) = cos(iz) to compute via cos(z).
        return Cos(new Complex<T>(-value.m_imaginary, value.m_real));
    }

    public static Complex<T> Acos(Complex<T> value)
    {
        T b, bPrime, v;
        Asin_Internal(T.Abs(value.Real), T.Abs(value.Imaginary), out b, out bPrime, out v);

        T u;
        if (bPrime < T.Zero)
        {
            u = T.Acos(b);
        }
        else
        {
            u = T.Atan(T.One / bPrime);
        }

        if (value.Real < T.Zero) u = T.Pi - u;
        if (value.Imaginary > T.Zero) v = -v;

        return new Complex<T>(u, v);
    }

    public static Complex<T> Tan(Complex<T> value)
    {
        // tan z = sin z / cos z, but to avoid unnecessary repeated trig computations, use
        //   tan z = (sin(2x) + i sinh(2y)) / (cos(2x) + cosh(2y))
        // (see Abramowitz & Stegun 4.3.57 or derive by hand), and compute trig functions here.

        // This approach does not work for |y| > ~355, because sinh(2y) and cosh(2y) overflow,
        // even though their ratio does not. In that case, divide through by cosh to get:
        //   tan z = (sin(2x) / cosh(2y) + i \tanh(2y)) / (1 + cos(2x) / cosh(2y))
        // which correctly computes the (tiny) real part and the (normal-sized) imaginary part.

        T x2 = NumericConstants<T>.Two * value.m_real;
        T y2 = NumericConstants<T>.Two * value.m_imaginary;
        T p = T.Exp(y2);
        T q = T.One / p;
        T cosh = (p + q) * NumericConstants<T>.Half;
        if (T.Abs(value.m_imaginary) <= NumericConstants<T>.Four)
        {
            T sinh = (p - q) * NumericConstants<T>.Half;
            T D = T.Cos(x2) + cosh;
            return new Complex<T>(T.Sin(x2) / D, sinh / D);
        }
        else
        {
            T D = T.One + T.Cos(x2) / cosh;
            return new Complex<T>(T.Sin(x2) / cosh / D, T.Tanh(y2) / D);
        }
    }

    public static Complex<T> Tanh(Complex<T> value)
    {
        // Use tanh(z) = -i tan(iz) to compute via tan(z).
        Complex<T> tan = Tan(new Complex<T>(-value.m_imaginary, value.m_real));
        return new Complex<T>(tan.m_imaginary, -tan.m_real);
    }

    public static Complex<T> Atan(Complex<T> value)
    {
        Complex<T> two = new(NumericConstants<T>.Two, T.Zero);
        return (ImaginaryOne / two) * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));
    }

    private static void Asin_Internal(T x, T y, out T b, out T bPrime, out T v)
    {
        // This method for the inverse complex sine (and cosine) is described in Hull, Fairgrieve,
        // and Tang, "Implementing the Complex<T> Arcsine and Arccosine Functions Using Exception Handling",
        // ACM Transactions on Mathematical Software (1997)
        // (https://www.researchgate.net/profile/Ping_Tang3/publication/220493330_Implementing_the_Complex_Arcsine_and_Arccosine_Functions_Using_Exception_Handling/links/55b244b208ae9289a085245d.pdf)

        // First, the basics: start with sin(w) = (e^{iw} - e^{-iw}) / (2i) = z. Here z is the input
        // and w is the output. To solve for w, define t = e^{i w} and multiply through by t to
        // get the quadratic equation t^2 - 2 i z t - 1 = 0. The solution is t = i z + sqrt(1 - z^2), so
        //   w = arcsin(z) = - i log( i z + sqrt(1 - z^2) )
        // Decompose z = x + i y, multiply out i z + sqrt(1 - z^2), use log(s) = |s| + i arg(s), and do a
        // bunch of algebra to get the components of w = arcsin(z) = u + i v
        //   u = arcsin(beta)  v = sign(y) log(alpha + sqrt(alpha^2 - 1))
        // where
        //   alpha = (rho + sigma) / 2      beta = (rho - sigma) / 2
        //   rho = sqrt((x + 1)^2 + y^2)    sigma = sqrt((x - 1)^2 + y^2)
        // These formulas appear in DLMF section 4.23. (http://dlmf.nist.gov/4.23), along with the analogous
        //   arccos(w) = arccos(beta) - i sign(y) log(alpha + sqrt(alpha^2 - 1))
        // So alpha and beta together give us arcsin(w) and arccos(w).

        // As written, alpha is not susceptible to cancelation errors, but beta is. To avoid cancelation, note
        //   beta = (rho^2 - sigma^2) / (rho + sigma) / 2 = (2 x) / (rho + sigma) = x / alpha
        // which is not subject to cancelation. Note alpha >= 1 and |beta| <= 1.

        // For alpha ~ 1, the argument of the log is near unity, so we compute (alpha - 1) instead,
        // write the argument as 1 + (alpha - 1) + sqrt((alpha - 1)(alpha + 1)), and use the log1p function
        // to compute the log without loss of accuracy.
        // For beta ~ 1, arccos does not accurately resolve small angles, so we compute the tangent of the angle
        // instead.
        // Hull, Fairgrieve, and Tang derive formulas for (alpha - 1) and beta' = tan(u) that do not suffer
        // from cancelation in these cases.

        // For simplicity, we assume all positive inputs and return all positive outputs. The caller should
        // assign signs appropriate to the desired cut conventions. We return v directly since its magnitude
        // is the same for both arcsin and arccos. Instead of u, we usually return beta and sometimes beta'.
        // If beta' is not computed, it is set to -1; if it is computed, it should be used instead of beta
        // to determine u. Compute u = arcsin(beta) or u = arctan(beta') for arcsin, u = arccos(beta)
        // or arctan(1/beta') for arccos.

        Debug.Assert((x >= T.Zero) || T.IsNaN(x));
        Debug.Assert((y >= T.Zero) || T.IsNaN(y));

        // For x or y large enough to overflow alpha^2, we can simplify our formulas and avoid overflow.
        if ((x > s_asinOverflowThreshold) || (y > s_asinOverflowThreshold))
        {
            b = -T.One;
            bPrime = x / y;

            T small, big;
            if (x < y)
            {
                small = x;
                big = y;
            }
            else
            {
                small = y;
                big = x;
            }

            T ratio = small / big;
            v = s_log2 + T.Log(big) + NumericConstants<T>.Half * Log1P(ratio * ratio);
        }
        else
        {
            T r = Hypot((x + T.One), y);
            T s = Hypot((x - T.One), y);

            T a = (r + s) * NumericConstants<T>.Half;
            b = x / a;

            if (b > NumericConstants<T>.ThreeQuarters)
            {
                if (x <= T.One)
                {
                    T amx = (y * y / (r + (x + T.One)) + (s + (T.One - x))) * NumericConstants<T>.Half;
                    bPrime = x / T.Sqrt((a + x) * amx);
                }
                else
                {
                    // In this case, amx ~ y^2. Since we take the square root of amx, we should
                    // pull y out from under the square root so we don't lose its contribution
                    // when y^2 underflows.
                    T t = (T.One / (r + (x + T.One)) + T.One / (s + (x - T.One))) * NumericConstants<T>.Half;
                    bPrime = x / y / T.Sqrt((a + x) * t);
                }
            }
            else
            {
                bPrime = -T.One;
            }

            if (a < NumericConstants<T>.OneAndAHalf)
            {
                if (x < T.One)
                {
                    // This is another case where our expression is proportional to y^2 and
                    // we take its square root, so again we pull out a factor of y from
                    // under the square root.
                    T t = (T.One / (r + (x + T.One)) + T.One / (s + (T.One - x))) * NumericConstants<T>.Half;
                    T am1 = y * y * t;
                    v = Log1P(am1 + y * T.Sqrt(t * (a + T.One)));
                }
                else
                {
                    T am1 = (y * y / (r + (x + T.One)) + (s + (x - T.One))) * NumericConstants<T>.Half;
                    v = Log1P(am1 + T.Sqrt(am1 * (a + T.One)));
                }
            }
            else
            {
                // Because of the test above, we can be sure that a * a will not overflow.
                v = T.Log(a + T.Sqrt((a - T.One) * (a + T.One)));
            }
        }
    }

    public static bool IsFinite(Complex<T> value) => T.IsFinite(value.m_real) && T.IsFinite(value.m_imaginary);

    public static bool IsInfinity(Complex<T> value) => T.IsInfinity(value.m_real) || T.IsInfinity(value.m_imaginary);

    public static bool IsNaN(Complex<T> value) => !IsInfinity(value) && !IsFinite(value);

    public static Complex<T> Log(Complex<T> value)
    {
        return new Complex<T>(T.Log(Abs(value)), T.Atan2(value.m_imaginary, value.m_real));
    }

    public static Complex<T> Log(Complex<T> value, T baseValue)
    {
        return Log(value) / Log(baseValue);
    }

    public static Complex<T> Log10(Complex<T> value)
    {
        Complex<T> tempLog = Log(value);
        return Scale(tempLog, InverseOfLog10);
    }

    public static Complex<T> Exp(Complex<T> value)
    {
        T expReal = T.Exp(value.m_real);
        T cosImaginary = expReal * T.Cos(value.m_imaginary);
        T sinImaginary = expReal * T.Sin(value.m_imaginary);
        return new Complex<T>(cosImaginary, sinImaginary);
    }

    public static Complex<T> Sqrt(Complex<T> value)
    {
        if (value.m_imaginary == T.Zero)
        {
            // Handle the trivial case quickly.
            if (value.m_real < T.Zero)
            {
                return new Complex<T>(T.Zero, T.Sqrt(-value.m_real));
            }

            return new Complex<T>(T.Sqrt(value.m_real), T.Zero);
        }

        // One way to compute Sqrt(z) is just to call Pow(z, NumericConstants<T>.Half), which coverts to polar coordinates
        // (sqrt + atan), halves the phase, and reconverts to cartesian coordinates (cos + sin).
        // Not only is this more expensive than necessary, it also fails to preserve certain expected
        // symmetries, such as that the square root of a pure negative is a pure imaginary, and that the
        // square root of a pure imaginary has exactly equal real and imaginary parts. This all goes
        // back to the fact that T.PI is not stored with infinite precision, so taking half of T.PI
        // does not land us on an argument with cosine exactly equal to zero.

        // To find a fast and symmetry-respecting formula for complex square root,
        // note x + i y = \sqrt{a + i b} implies x^2 + 2 i x y - y^2 = a + i b,
        // so x^2 - y^2 = a and 2 x y = b. Cross-substitute and use the quadratic formula to obtain
        //   x = \sqrt{\frac{\sqrt{a^2 + b^2} + a}{2}}  y = \pm \sqrt{\frac{\sqrt{a^2 + b^2} - a}{2}}
        // There is just one complication: depending on the sign on a, either x or y suffers from
        // cancelation when |b| << |a|. We can get around this by noting that our formulas imply
        // x^2 y^2 = b^2 / 4, so |x| |y| = |b| / 2. So after computing the one that doesn't suffer
        // from cancelation, we can compute the other with just a division. This is basically just
        // the right way to evaluate the quadratic formula without cancelation.

        // All this reduces our total cost to two sqrts and a few flops, and it respects the desired
        // symmetries. Much better than atan + cos + sin!

        // The signs are a matter of choice of branch cut, which is traditionally taken so x > 0 and sign(y) = sign(b).

        // If the components are too large, Hypot will overflow, even though the subsequent sqrt would
        // make the result representable. To avoid this, we re-scale (by exact powers of 2 for accuracy)
        // when we encounter very large components to avoid intermediate infinities.
        bool rescale = false;
        T realCopy = value.m_real;
        T imaginaryCopy = value.m_imaginary;
        if ((T.Abs(realCopy) >= s_sqrtRescaleThreshold) || (T.Abs(imaginaryCopy) >= s_sqrtRescaleThreshold))
        {
            if (T.IsInfinity(value.m_imaginary) && !T.IsNaN(value.m_real))
            {
                // We need to handle infinite imaginary parts specially because otherwise
                // our formulas below produce inf/inf = NaN. The NaN test is necessary
                // so that we return NaN rather than (+inf,inf) for (NaN,inf).
                return (new Complex<T>(T.PositiveInfinity, imaginaryCopy));
            }

            realCopy *= NumericConstants<T>.Quarter;
            imaginaryCopy *= NumericConstants<T>.Quarter;
            rescale = true;
        }

        // This is the core of the algorithm. Everything else is special case handling.
        T x, y;
        if (realCopy >= T.Zero)
        {
            x = T.Sqrt((Hypot(realCopy, imaginaryCopy) + realCopy) * NumericConstants<T>.Half);
            y = imaginaryCopy / (NumericConstants<T>.Two * x);
        }
        else
        {
            y = T.Sqrt((Hypot(realCopy, imaginaryCopy) - realCopy) * NumericConstants<T>.Half);
            if (imaginaryCopy < T.Zero) y = -y;
            x = imaginaryCopy / (NumericConstants<T>.Two * y);
        }

        if (rescale)
        {
            x *= NumericConstants<T>.Two;
            y *= NumericConstants<T>.Two;
        }

        return new Complex<T>(x, y);
    }

    public static Complex<T> Pow(Complex<T> value, Complex<T> power)
    {
        if (power == Zero)
        {
            return One;
        }

        if (value == Zero)
        {
            return Zero;
        }

        T valueReal = value.m_real;
        T valueImaginary = value.m_imaginary;
        T powerReal = power.m_real;
        T powerImaginary = power.m_imaginary;

        T rho = Abs(value);
        T theta = T.Atan2(valueImaginary, valueReal);
        T newRho = powerReal * theta + powerImaginary * T.Log(rho);

        T t = T.Pow(rho, powerReal) * T.Pow(T.E, -powerImaginary * theta);

        return new Complex<T>(t * T.Cos(newRho), t * T.Sin(newRho));
    }

    public static Complex<T> Pow(Complex<T> value, T power)
    {
        return Pow(value, new Complex<T>(power, T.Zero));
    }

    private static Complex<T> Scale(Complex<T> value, T factor)
    {
        T realResult = factor * value.m_real;
        T imaginaryResuilt = factor * value.m_imaginary;
        return new Complex<T>(realResult, imaginaryResuilt);
    }

    //
    // Explicit Conversions To Complex<T>
    //

    public static explicit operator Complex<T>(decimal value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    /// <summary>Explicitly converts a <see cref="Int128" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    public static explicit operator Complex<T>(Int128 value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static explicit operator Complex<T>(BigInteger value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    /// <summary>Explicitly converts a <see cref="UInt128" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    [CLSCompliant(false)]
    public static explicit operator Complex<T>(UInt128 value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    //
    // Implicit Conversions To Complex<T>
    //

    public static implicit operator Complex<T>(byte value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    /// <summary>Implicitly converts a <see cref="char" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    public static implicit operator Complex<T>(char value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(T value)
    {
        return new Complex<T>(value, T.Zero);
    }

    /// <summary>Implicitly converts a <see cref="Half" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    public static implicit operator Complex<T>(Half value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(short value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(int value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(long value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    /// <summary>Implicitly converts a <see cref="IntPtr" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    public static implicit operator Complex<T>(nint value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(sbyte value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(float value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(ushort value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(uint value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    public static implicit operator Complex<T>(ulong value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    /// <summary>Implicitly converts a <see cref="UIntPtr" /> value to a T-precision complex number.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns><paramref name="value" /> converted to a T-precision complex number.</returns>
    public static implicit operator Complex<T>(nuint value)
    {
        return new Complex<T>(T.CreateTruncating(value), T.Zero);
    }

    //
    // IAdditiveIdentity
    //

    /// <inheritdoc cref="IAdditiveIdentity{TSelf, TResult}.AdditiveIdentity" />
    static Complex<T> IAdditiveIdentity<Complex<T>, Complex<T>>.AdditiveIdentity => new(T.Zero, T.Zero);

    //
    // IDecrementOperators
    //

    /// <inheritdoc cref="IDecrementOperators{TSelf}.op_Decrement(TSelf)" />
    public static Complex<T> operator --(Complex<T> value) => value - One;

    //
    // IIncrementOperators
    //

    /// <inheritdoc cref="IIncrementOperators{TSelf}.op_Increment(TSelf)" />
    public static Complex<T> operator ++(Complex<T> value) => value + One;

    //
    // IMultiplicativeIdentity
    //

    /// <inheritdoc cref="IMultiplicativeIdentity{TSelf, TResult}.MultiplicativeIdentity" />
    static Complex<T> IMultiplicativeIdentity<Complex<T>, Complex<T>>.MultiplicativeIdentity
        => new(T.One, T.Zero);

    //
    // INumberBase
    //

    /// <inheritdoc cref="INumberBase{TSelf}.One" />
    static Complex<T> INumberBase<Complex<T>>.One => new(T.One, T.Zero);

    /// <inheritdoc cref="INumberBase{TSelf}.Radix" />
    static int INumberBase<Complex<T>>.Radix => 2;

    /// <inheritdoc cref="INumberBase{TSelf}.Zero" />
    static Complex<T> INumberBase<Complex<T>>.Zero => new(T.Zero, T.Zero);

    /// <inheritdoc cref="INumberBase{TSelf}.Abs(TSelf)" />
    static Complex<T> INumberBase<Complex<T>>.Abs(Complex<T> value) => Abs(value);

    /// <inheritdoc cref="INumberBase{TSelf}.CreateChecked{TOther}(TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Complex<T> CreateChecked<TOther>(TOther value)
        where TOther : INumberBase<TOther>
    {
        Complex<T> result;

        if (typeof(TOther) == typeof(Complex<T>))
        {
            result = (Complex<T>)(object)value;
        }
        else if (!TryConvertFrom(value, out result) && !TOther.TryConvertToChecked(value, out result))
        {
            throw new NotSupportedException();
        }

        return result;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.CreateSaturating{TOther}(TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Complex<T> CreateSaturating<TOther>(TOther value)
        where TOther : INumberBase<TOther>
    {
        Complex<T> result;

        if (typeof(TOther) == typeof(Complex<T>))
        {
            result = (Complex<T>)(object)value;
        }
        else if (!TryConvertFrom(value, out result) && !TOther.TryConvertToSaturating(value, out result))
        {
            throw new NotSupportedException();
        }

        return result;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.CreateTruncating{TOther}(TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Complex<T> CreateTruncating<TOther>(TOther value)
        where TOther : INumberBase<TOther>
    {
        Complex<T> result;

        if (typeof(TOther) == typeof(Complex<T>))
        {
            result = (Complex<T>)(object)value;
        }
        else if (!TryConvertFrom(value, out result) && !TOther.TryConvertToTruncating(value, out result))
        {
            throw new NotSupportedException();
        }

        return result;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsCanonical(TSelf)" />
    static bool INumberBase<Complex<T>>.IsCanonical(Complex<T> value) => true;

    /// <inheritdoc cref="INumberBase{TSelf}.IsComplexNumber(TSelf)" />
    public static bool IsComplexNumber(Complex<T> value) => (value.m_real != T.Zero) && (value.m_imaginary != T.Zero);

    /// <inheritdoc cref="INumberBase{TSelf}.IsEvenInteger(TSelf)" />
    public static bool IsEvenInteger(Complex<T> value)
        => (value.m_imaginary == T.Zero) && T.IsEvenInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsImaginaryNumber(TSelf)" />
    public static bool IsImaginaryNumber(Complex<T> value)
        => (value.m_real == T.Zero) && T.IsRealNumber(value.m_imaginary);

    /// <inheritdoc cref="INumberBase{TSelf}.IsInteger(TSelf)" />
    public static bool IsInteger(Complex<T> value) => (value.m_imaginary == T.Zero) && T.IsInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegative(TSelf)" />
    public static bool IsNegative(Complex<T> value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == T.Zero) && T.IsNegative(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsNegativeInfinity(TSelf)" />
    public static bool IsNegativeInfinity(Complex<T> value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == T.Zero) && T.IsNegativeInfinity(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsNormal(TSelf)" />
    public static bool IsNormal(Complex<T> value)
    {
        // much as IsFinite requires both part to be finite, we require both
        // part to be "normal" (finite, non-zero, and non-subnormal) to be true

        return T.IsNormal(value.m_real)
               && ((value.m_imaginary == T.Zero) || T.IsNormal(value.m_imaginary));
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsOddInteger(TSelf)" />
    public static bool IsOddInteger(Complex<T> value) => (value.m_imaginary == T.Zero) && T.IsOddInteger(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositive(TSelf)" />
    public static bool IsPositive(Complex<T> value)
    {
        // since complex numbers do not have a well-defined concept of
        // negative we report false if this value has an imaginary part

        return (value.m_imaginary == T.Zero) && T.IsPositive(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsPositiveInfinity(TSelf)" />
    public static bool IsPositiveInfinity(Complex<T> value)
    {
        // since complex numbers do not have a well-defined concept of
        // positive we report false if this value has an imaginary part

        return (value.m_imaginary == T.Zero) && T.IsPositiveInfinity(value.m_real);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsRealNumber(TSelf)" />
    public static bool IsRealNumber(Complex<T> value) => (value.m_imaginary == T.Zero) && T.IsRealNumber(value.m_real);

    /// <inheritdoc cref="INumberBase{TSelf}.IsSubnormal(TSelf)" />
    public static bool IsSubnormal(Complex<T> value)
    {
        // much as IsInfinite allows either part to be infinite, we allow either
        // part to be "subnormal" (finite, non-zero, and non-normal) to be true

        return T.IsSubnormal(value.m_real) || T.IsSubnormal(value.m_imaginary);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.IsZero(TSelf)" />
    static bool INumberBase<Complex<T>>.IsZero(Complex<T> value)
        => (value.m_real == T.Zero) && (value.m_imaginary == T.Zero);

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitude(TSelf, TSelf)" />
    public static Complex<T> MaxMagnitude(Complex<T> x, Complex<T> y)
    {
        // complex numbers are not normally comparable, however every complex
        // number has a real magnitude (absolute value) and so we can provide
        // an implementation for MaxMagnitude

        // This matches the IEEE 754:2019 `maximumMagnitude` function
        //
        // It propagates NaN inputs back to the caller and
        // otherwise returns the input with a larger magnitude.
        // It treats +0 as larger than -0 as per the specification.

        T ax = Abs(x);
        T ay = Abs(y);

        if ((ax > ay) || T.IsNaN(ax))
        {
            return x;
        }

        if (ax == ay)
        {
            // We have two equal magnitudes which means we have two of the following
            //   `+a + ib`
            //   `-a + ib`
            //   `+a - ib`
            //   `-a - ib`
            //
            // We want to treat `+a + ib` as greater than everything and `-a - ib` as
            // lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
            // so we will just preference `+a - ib` since that's the most correct choice
            // in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
            // correct" choice because both represent real numbers and `+a` is preferred
            // over `-a`.

            if (T.IsNegative(y.m_real))
            {
                if (T.IsNegative(y.m_imaginary))
                {
                    // when `y` is `-a - ib` we always prefer `x` (its either the same as
                    // `x` or some part of `x` is positive).

                    return x;
                }
                else
                {
                    if (T.IsNegative(x.m_real))
                    {
                        // when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
                        // we either have same value or both parts of `x` are negative
                        // and we want to prefer `y`.

                        return y;
                    }
                    else
                    {
                        // when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
                        // we want to prefer `x` because either both parts are positive
                        // or we want to prefer `+a - ib` due to how it handles when `x`
                        // represents a real number.

                        return x;
                    }
                }
            }
            else if (T.IsNegative(y.m_imaginary))
            {
                if (T.IsNegative(x.m_real))
                {
                    // when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
                    // we either both parts of `x` are negative or we want to prefer
                    // `+a - ib` due to how it handles when `y` represents a real number.

                    return y;
                }
                else
                {
                    // when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
                    // we want to prefer `x` because either both parts are positive
                    // or they represent the same value.

                    return x;
                }
            }
        }

        return y;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.MaxMagnitudeNumber(TSelf, TSelf)" />
    static Complex<T> INumberBase<Complex<T>>.MaxMagnitudeNumber(Complex<T> x, Complex<T> y)
    {
        // complex numbers are not normally comparable, however every complex
        // number has a real magnitude (absolute value) and so we can provide
        // an implementation for MaxMagnitudeNumber

        // This matches the IEEE 754:2019 `maximumMagnitudeNumber` function
        //
        // It does not propagate NaN inputs back to the caller and
        // otherwise returns the input with a larger magnitude.
        // It treats +0 as larger than -0 as per the specification.

        T ax = Abs(x);
        T ay = Abs(y);

        if ((ax > ay) || T.IsNaN(ay))
        {
            return x;
        }

        if (ax == ay)
        {
            // We have two equal magnitudes which means we have two of the following
            //   `+a + ib`
            //   `-a + ib`
            //   `+a - ib`
            //   `-a - ib`
            //
            // We want to treat `+a + ib` as greater than everything and `-a - ib` as
            // lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
            // so we will just preference `+a - ib` since that's the most correct choice
            // in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
            // correct" choice because both represent real numbers and `+a` is preferred
            // over `-a`.

            if (T.IsNegative(y.m_real))
            {
                if (T.IsNegative(y.m_imaginary))
                {
                    // when `y` is `-a - ib` we always prefer `x` (its either the same as
                    // `x` or some part of `x` is positive).

                    return x;
                }
                else
                {
                    if (T.IsNegative(x.m_real))
                    {
                        // when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
                        // we either have same value or both parts of `x` are negative
                        // and we want to prefer `y`.

                        return y;
                    }
                    else
                    {
                        // when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
                        // we want to prefer `x` because either both parts are positive
                        // or we want to prefer `+a - ib` due to how it handles when `x`
                        // represents a real number.

                        return x;
                    }
                }
            }
            else if (T.IsNegative(y.m_imaginary))
            {
                if (T.IsNegative(x.m_real))
                {
                    // when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
                    // we either both parts of `x` are negative or we want to prefer
                    // `+a - ib` due to how it handles when `y` represents a real number.

                    return y;
                }
                else
                {
                    // when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
                    // we want to prefer `x` because either both parts are positive
                    // or they represent the same value.

                    return x;
                }
            }
        }

        return y;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitude(TSelf, TSelf)" />
    public static Complex<T> MinMagnitude(Complex<T> x, Complex<T> y)
    {
        // complex numbers are not normally comparable, however every complex
        // number has a real magnitude (absolute value) and so we can provide
        // an implementation for MaxMagnitude

        // This matches the IEEE 754:2019 `minimumMagnitude` function
        //
        // It propagates NaN inputs back to the caller and
        // otherwise returns the input with a smaller magnitude.
        // It treats -0 as smaller than +0 as per the specification.

        T ax = Abs(x);
        T ay = Abs(y);

        if ((ax < ay) || T.IsNaN(ax))
        {
            return x;
        }

        if (ax == ay)
        {
            // We have two equal magnitudes which means we have two of the following
            //   `+a + ib`
            //   `-a + ib`
            //   `+a - ib`
            //   `-a - ib`
            //
            // We want to treat `+a + ib` as greater than everything and `-a - ib` as
            // lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
            // so we will just preference `-a + ib` since that's the most correct choice
            // in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
            // correct" choice because both represent real numbers and `-a` is preferred
            // over `+a`.

            if (T.IsNegative(y.m_real))
            {
                if (T.IsNegative(y.m_imaginary))
                {
                    // when `y` is `-a - ib` we always prefer `y` as both parts are negative
                    return y;
                }
                else
                {
                    if (T.IsNegative(x.m_real))
                    {
                        // when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
                        // we either have same value or both parts of `x` are negative
                        // and we want to prefer it.

                        return x;
                    }
                    else
                    {
                        // when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
                        // we want to prefer `y` because either both parts of 'x' are positive
                        // or we want to prefer `-a - ib` due to how it handles when `y`
                        // represents a real number.

                        return y;
                    }
                }
            }
            else if (T.IsNegative(y.m_imaginary))
            {
                if (T.IsNegative(x.m_real))
                {
                    // when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
                    // either both parts of `x` are negative or we want to prefer
                    // `-a - ib` due to how it handles when `x` represents a real number.

                    return x;
                }
                else
                {
                    // when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
                    // we want to prefer `y` because either both parts of x are positive
                    // or they represent the same value.

                    return y;
                }
            }
            else
            {
                return x;
            }
        }

        return y;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.MinMagnitudeNumber(TSelf, TSelf)" />
    static Complex<T> INumberBase<Complex<T>>.MinMagnitudeNumber(Complex<T> x, Complex<T> y)
    {
        // complex numbers are not normally comparable, however every complex
        // number has a real magnitude (absolute value) and so we can provide
        // an implementation for MinMagnitudeNumber

        // This matches the IEEE 754:2019 `minimumMagnitudeNumber` function
        //
        // It does not propagate NaN inputs back to the caller and
        // otherwise returns the input with a smaller magnitude.
        // It treats -0 as smaller than +0 as per the specification.

        T ax = Abs(x);
        T ay = Abs(y);

        if ((ax < ay) || T.IsNaN(ay))
        {
            return x;
        }

        if (ax == ay)
        {
            // We have two equal magnitudes which means we have two of the following
            //   `+a + ib`
            //   `-a + ib`
            //   `+a - ib`
            //   `-a - ib`
            //
            // We want to treat `+a + ib` as greater than everything and `-a - ib` as
            // lesser. For `-a + ib` and `+a - ib` its "ambiguous" which should be preferred
            // so we will just preference `-a + ib` since that's the most correct choice
            // in the face of something like `+a - i0.0` vs `-a + i0.0`. This is the "most
            // correct" choice because both represent real numbers and `-a` is preferred
            // over `+a`.

            if (T.IsNegative(y.m_real))
            {
                if (T.IsNegative(y.m_imaginary))
                {
                    // when `y` is `-a - ib` we always prefer `y` as both parts are negative
                    return y;
                }
                else
                {
                    if (T.IsNegative(x.m_real))
                    {
                        // when `y` is `-a + ib` and `x` is `-a + ib` or `-a - ib` then
                        // we either have same value or both parts of `x` are negative
                        // and we want to prefer it.

                        return x;
                    }
                    else
                    {
                        // when `y` is `-a + ib` and `x` is `+a + ib` or `+a - ib` then
                        // we want to prefer `y` because either both parts of 'x' are positive
                        // or we want to prefer `-a - ib` due to how it handles when `y`
                        // represents a real number.

                        return y;
                    }
                }
            }
            else if (T.IsNegative(y.m_imaginary))
            {
                if (T.IsNegative(x.m_real))
                {
                    // when `y` is `+a - ib` and `x` is `-a + ib` or `-a - ib` then
                    // either both parts of `x` are negative or we want to prefer
                    // `-a - ib` due to how it handles when `x` represents a real number.

                    return x;
                }
                else
                {
                    // when `y` is `+a - ib` and `x` is `+a + ib` or `+a - ib` then
                    // we want to prefer `y` because either both parts of x are positive
                    // or they represent the same value.

                    return y;
                }
            }
            else
            {
                return x;
            }
        }

        return y;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.Parse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?)" />
    public static Complex<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out Complex<T> result))
        {
            throw new OverflowException();
        }

        return result;
    }

    /// <inheritdoc cref="INumberBase{TSelf}.Parse(string, NumberStyles, IFormatProvider?)" />
    public static Complex<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), style, provider);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromChecked{TOther}(TOther, out TSelf)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertFromChecked<TOther>(TOther value, out Complex<T> result)
    {
        return TryConvertFrom<TOther>(value, out result);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromSaturating{TOther}(TOther, out TSelf)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertFromSaturating<TOther>(TOther value, out Complex<T> result)
    {
        return TryConvertFrom<TOther>(value, out result);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertFromTruncating{TOther}(TOther, out TSelf)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertFromTruncating<TOther>(TOther value, out Complex<T> result)
    {
        return TryConvertFrom<TOther>(value, out result);
    }

    private static bool TryConvertFrom<TOther>(TOther value, out Complex<T> result)
        where TOther : INumberBase<TOther>
    {
        // We don't want to defer to `T.Create*(value)` because some type might have its own
        // `TOther.ConvertTo*(value, out Complex<T> result)` handling that would end up bypassed.

        if (typeof(TOther) == typeof(byte))
        {
            byte actualValue = (byte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(char))
        {
            char actualValue = (char)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(decimal))
        {
            decimal actualValue = (decimal)(object)value;
            result = (Complex<T>)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(T))
        {
            T actualValue = (T)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(Half))
        {
            Half actualValue = (Half)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(short))
        {
            short actualValue = (short)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(int))
        {
            int actualValue = (int)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(long))
        {
            long actualValue = (long)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(Int128))
        {
            Int128 actualValue = (Int128)(object)value;
            result = (Complex<T>)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nint))
        {
            nint actualValue = (nint)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(sbyte))
        {
            sbyte actualValue = (sbyte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(float))
        {
            float actualValue = (float)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ushort))
        {
            ushort actualValue = (ushort)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(uint))
        {
            uint actualValue = (uint)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ulong))
        {
            ulong actualValue = (ulong)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(UInt128))
        {
            UInt128 actualValue = (UInt128)(object)value;
            result = (Complex<T>)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nuint))
        {
            nuint actualValue = (nuint)(object)value;
            result = actualValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToChecked{TOther}(TSelf, out TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertToChecked<TOther>(
        Complex<T> value, [MaybeNullWhen(false)] out TOther result
    )
    {
        return TOther.TryConvertFromChecked(value.m_real, out result);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToSaturating{TOther}(TSelf, out TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertToSaturating<TOther>(
        Complex<T> value, [MaybeNullWhen(false)] out TOther result
    )
    {
        return TOther.TryConvertFromSaturating(value.m_real, out result);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryConvertToTruncating{TOther}(TSelf, out TOther)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Complex<T>>.TryConvertToTruncating<TOther>(
        Complex<T> value, [MaybeNullWhen(false)] out TOther result
    )
    {
        return TOther.TryConvertFromTruncating(value.m_real, out result);
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider?, out TSelf)" />
    public static bool TryParse(
        ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Complex<T> result
    )
    {
        ValidateParseStyleFloatingPoint(style);

        int openBracket = s.IndexOf('<');
        int semicolon = s.IndexOf(';');
        int closeBracket = s.IndexOf('>');

        if ((s.Length < 5) || (openBracket == -1) || (semicolon == -1) || (closeBracket == -1) ||
            (openBracket > semicolon) || (openBracket > closeBracket) || (semicolon > closeBracket))
        {
            // We need at least 5 characters for `<0;0>`
            // We also expect a to find an open bracket, a semicolon, and a closing bracket in that order

            result = default;
            return false;
        }

        if ((openBracket != 0) &&
            (((style & NumberStyles.AllowLeadingWhite) == 0) || !s.Slice(0, openBracket).IsWhiteSpace()))
        {
            // The opening bracket wasn't the first and we either didn't allow leading whitespace
            // or one of the leading characters wasn't whitespace at all.

            result = default;
            return false;
        }

        if (!T.TryParse(s.Slice(openBracket + 1, semicolon), style, provider, out T real))
        {
            result = default;
            return false;
        }

        if (char.IsWhiteSpace(s[semicolon + 1]))
        {
            // We allow a single whitespace after the semicolon regardless of style, this is so that
            // the output of `ToString` can be correctly parsed by default and values will roundtrip.
            semicolon += 1;
        }

        if (!T.TryParse(s.Slice(semicolon + 1, closeBracket - semicolon), style, provider, out T imaginary))
        {
            result = default;
            return false;
        }

        if ((closeBracket != (s.Length - 1)) &&
            (((style & NumberStyles.AllowTrailingWhite) == 0) || !s.Slice(closeBracket).IsWhiteSpace()))
        {
            // The closing bracket wasn't the last and we either didn't allow trailing whitespace
            // or one of the trailing characters wasn't whitespace at all.

            result = default;
            return false;
        }

        result = new Complex<T>(real, imaginary);
        return true;

        static void ValidateParseStyleFloatingPoint(NumberStyles style)
        {
            // Check for undefined flags or hex number
            if ((style & (InvalidNumberStyles | NumberStyles.AllowHexSpecifier)) != 0)
            {
                ThrowInvalid(style);

                static void ThrowInvalid(NumberStyles value)
                {
                    if ((value & InvalidNumberStyles) != 0)
                    {
                        throw new ArgumentException("Invalid number styles", nameof(style));
                    }

                    throw new ArgumentException("Hex style not supported");
                }
            }
        }
    }

    /// <inheritdoc cref="INumberBase{TSelf}.TryParse(string, NumberStyles, IFormatProvider?, out TSelf)" />
    public static bool TryParse(
        [NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Complex<T> result
    )
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), style, provider, out result);
    }

    //
    // IParsable
    //

    /// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)" />
    public static Complex<T> Parse(string s, IFormatProvider? provider) => Parse(s, DefaultNumberStyle, provider);

    /// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)" />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Complex<T> result)
        => TryParse(s, DefaultNumberStyle, provider, out result);

    //
    // ISignedNumber
    //

    /// <inheritdoc cref="ISignedNumber{TSelf}.NegativeOne" />
    static Complex<T> ISignedNumber<Complex<T>>.NegativeOne => new(-T.One, T.Zero);

    //
    // ISpanFormattable
    //

    /// <inheritdoc cref="ISpanFormattable.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider?)" />
    public bool TryFormat(
        Span<char> destination, out int charsWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null
    ) =>
        TryFormatCore(destination, out charsWritten, format, provider);

    public bool TryFormat(
        Span<byte> utf8Destination, out int bytesWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null
    ) =>
        TryFormatCore(utf8Destination, out bytesWritten, format, provider);

    private bool TryFormatCore<TChar>(
        Span<TChar> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider
    ) where TChar : unmanaged, IBinaryInteger<TChar>
    {
        Debug.Assert(typeof(TChar) == typeof(char) || typeof(TChar) == typeof(byte));

        // We have at least 6 more characters for: <0; 0>
        if (destination.Length >= 6)
        {
            int realChars;
            if (typeof(TChar) == typeof(char)
                    ? m_real.TryFormat(
                        MemoryMarshal.Cast<TChar, char>(destination.Slice(1)),
                        out realChars,
                        format,
                        provider
                    )
                    : m_real.TryFormat(
                        MemoryMarshal.Cast<TChar, byte>(destination.Slice(1)),
                        out realChars,
                        format,
                        provider
                    ))
            {
                destination[0] = TChar.CreateTruncating('<');
                destination = destination.Slice(1 + realChars); // + 1 for <

                // We have at least 4 more characters for: ; 0>
                if (destination.Length >= 4)
                {
                    int imaginaryChars;
                    if (typeof(TChar) == typeof(char)
                            ? m_imaginary.TryFormat(
                                MemoryMarshal.Cast<TChar, char>(destination.Slice(2)),
                                out imaginaryChars,
                                format,
                                provider
                            )
                            : m_imaginary.TryFormat(
                                MemoryMarshal.Cast<TChar, byte>(destination.Slice(2)),
                                out imaginaryChars,
                                format,
                                provider
                            ))
                    {
                        // We have 1 more character for: >
                        if ((uint)(2 + imaginaryChars) < (uint)destination.Length)
                        {
                            destination[0] = TChar.CreateTruncating(';');
                            destination[1] = TChar.CreateTruncating(' ');
                            destination[2 + imaginaryChars] = TChar.CreateTruncating('>');

                            charsWritten = realChars + imaginaryChars + 4;
                            return true;
                        }
                    }
                }
            }
        }

        charsWritten = 0;
        return false;
    }

    //
    // ISpanParsable
    //

    /// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)" />
    public static Complex<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Parse(s, DefaultNumberStyle, provider);

    /// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)" />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Complex<T> result)
        => TryParse(s, DefaultNumberStyle, provider, out result);

    //
    // IUnaryPlusOperators
    //

    /// <inheritdoc cref="IUnaryPlusOperators{TSelf, TResult}.op_UnaryPlus(TSelf)" />
    public static Complex<T> operator +(Complex<T> value) => value;
}