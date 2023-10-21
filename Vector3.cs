using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

#if NET8_0_OR_GREATER
using BitCaster = System.Runtime.CompilerServices.Unsafe;
#else
using BitCaster = GenericVector.Vector3;
#endif

namespace GenericVector;

file static class Vector
{
    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="vector">The vector to get the element from.</param>
    /// <param name="index">The index of the element to get.</param>
    /// <returns>The value of the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T GetElement<T>(this Vector3<T> vector, int index) where T : INumber<T>
    {
        if ((uint)(index) >= Vector3<T>.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return vector.GetElementUnsafe(index);
    }

    /// <summary>Creates a new <see cref="Vector3" /> with the element at the specified index set to the specified value and the remaining elements set to the same value as that in the given vector.</summary>
    /// <param name="vector">The vector to get the remaining elements from.</param>
    /// <param name="index">The index of the element to set.</param>
    /// <param name="value">The value to set the element to.</param>
    /// <returns>A <see cref="Vector3" /> with the value of the element at <paramref name="index" /> set to <paramref name="value" /> and the remaining elements set to the same value as that in <paramref name="vector" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    internal static Vector3<T> WithElement<T>(this Vector3<T> vector, int index, T value) where T : INumber<T>
    {
        if ((uint)(index) >= Vector3<T>.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var result = vector;
        result.SetElementUnsafe(index, value);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T GetElementUnsafe<T>(in this Vector3<T> vector, int index) where T : INumber<T>
    {
        Debug.Assert(index is >= 0 and < Vector3<T>.Count);
        ref var address = ref Unsafe.AsRef(in vector.X);
        return Unsafe.Add(ref address, index);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetElementUnsafe<T>(ref this Vector3<T> vector, int index, T value) where T : INumber<T>
    {
        Debug.Assert(index is >= 0 and < Vector3<T>.Count);
        Unsafe.Add(ref vector.X, index) = value;
    }
}

[StructLayout(LayoutKind.Sequential)]
public partial struct Vector3<T> : IEquatable<Vector3<T>>, IEquatable<System.Numerics.Vector3>, ISpanFormattable
    where T : INumber<T>
{
    /// <summary>The X component of the vector.</summary>
    public T X;

    /// <summary>The Y component of the vector.</summary>
    public T Y;

    /// <summary>The Z component of the vector.</summary>
    public T Z;
    
    /// <summary>
    /// Returns the vector (0,0,0).
    /// </summary>
    public static Vector3<T> Zero => new();

    /// <summary>
    /// Returns the vector (1,1,1).
    /// </summary>
    public static Vector3<T> One => new(T.One, T.One, T.One);

    /// <summary>
    /// Returns the vector (1,0,0).
    /// </summary>
    public static Vector3<T> UnitX => new(T.One, T.Zero, T.Zero);

    /// <summary>
    /// Returns the vector (0,1,0).
    /// </summary>
    public static Vector3<T> UnitY => new(T.Zero, T.One, T.Zero);

    /// <summary>
    /// Returns the vector (0,0,1).
    /// </summary>
    public static Vector3<T> UnitZ => new(T.Zero, T.Zero, T.One);

    public const int Count = 3;

    /// <summary>Gets or sets the element at the specified index.</summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => this.GetElement(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this = this.WithElement(index, value);
    }
    
    /// <summary>Creates a new <see cref="Vector3" /> object whose three elements have the same value.</summary>
    /// <param name="value">The value to assign to all three elements.</param>
    public Vector3(T value) : this(value, value, value)
    {
    }
    
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    /// <param name="x">The value to assign to the <see cref="X" /> field.</param>
    /// <param name="y">The value to assign to the <see cref="Y" /> field.</param>
    /// <param name="z">The value to assign to the <see cref="Z" /> field.</param>
    public Vector3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{Single}" />. The span must contain at least 3 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector3(ReadOnlySpan<T> values)
    {
        if (values.Length < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(values));
        }

        this = Unsafe.ReadUnaligned<Vector3<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }
    
    public readonly override string ToString()
    {
        return ToString("");
    }

    /// <summary>
    /// Returns a String representing this Vector3 instance, using the specified format to format individual elements 
    /// and the given IFormatProvider.
    /// </summary>
    /// <param name="format">The format of individual elements.</param>
    /// <param name="formatProvider">The format provider to use when formatting elements.</param>
    /// <returns>The string representation.</returns>
    public readonly string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var sb = new StringBuilder();
        var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
        sb.Append('<');
        sb.Append(X.ToString(format, formatProvider));
        sb.Append(separator);
        sb.Append(' ');
        sb.Append(Y.ToString(format, formatProvider));
        sb.Append(separator);
        sb.Append(' ');
        sb.Append(Z.ToString(format, formatProvider));
        sb.Append('>');
        return sb.ToString();
    }

    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Vector3" /> object and their corresponding elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals([NotNullWhen(true)] object? obj)
    {
        return (obj is Vector3<T> other) && Equals(other);
    }

    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the two vectors are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two vectors are equal if their <see cref="X" />, <see cref="Y" />, and <see cref="Z" /> elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Vector3<T> other)
    {
        // This function needs to account for floating-point equality around NaN
        // and so must behave equivalently to the underlying float/double.Equals

        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated)
        {
            return BitCaster.BitCast<Vector3<T>, System.Numerics.Vector3>(this).AsVector128().Equals(BitCaster.BitCast<Vector3<T>, System.Numerics.Vector3>(other).AsVector128());
        }
        
        if ((typeof(T) == typeof(int) || typeof(T) == typeof(uint)) && Vector128.IsHardwareAccelerated)
        {
            var _this = Vector128.Create(
                Unsafe.As<T, int>(ref Unsafe.AsRef(in X)),
                Unsafe.As<T, int>(ref Unsafe.AsRef(in Y)), 
                Unsafe.As<T, int>(ref Unsafe.AsRef(in Z)), 
                0);
    
            var _other = Vector128.Create(
                Unsafe.As<T, int>(ref Unsafe.AsRef(in other.X)),
                Unsafe.As<T, int>(ref Unsafe.AsRef(in other.Y)), 
                Unsafe.As<T, int>(ref Unsafe.AsRef(in other.Z)), 
                0);

            return _this.Equals(_other);
        }
        
        if (typeof(T) == typeof(double) && Vector256.IsHardwareAccelerated)
        {
            var _this = Vector256.Create(
                Unsafe.As<T, double>(ref Unsafe.AsRef(in X)),
                Unsafe.As<T, double>(ref Unsafe.AsRef(in Y)), 
                Unsafe.As<T, double>(ref Unsafe.AsRef(in Z)), 
                0);
    
            var _other = Vector256.Create(
                Unsafe.As<T, double>(ref Unsafe.AsRef(in other.X)),
                Unsafe.As<T, double>(ref Unsafe.AsRef(in other.Y)), 
                Unsafe.As<T, double>(ref Unsafe.AsRef(in other.Z)), 
                0);

            return _this.Equals(_other);
        }
        
        if ((typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) && Vector256.IsHardwareAccelerated)
        {
            var _this = Vector256.Create(
                Unsafe.As<T, long>(ref Unsafe.AsRef(in X)),
                Unsafe.As<T, long>(ref Unsafe.AsRef(in Y)), 
                Unsafe.As<T, long>(ref Unsafe.AsRef(in Z)), 
                0);
    
            var _other = Vector256.Create(
                Unsafe.As<T, long>(ref Unsafe.AsRef(in other.X)),
                Unsafe.As<T, long>(ref Unsafe.AsRef(in other.Y)), 
                Unsafe.As<T, long>(ref Unsafe.AsRef(in other.Z)), 
                0);

            return _this.Equals(_other);
        }

        return SoftwareFallback(in this, other);

        static bool SoftwareFallback(in Vector3<T> self, Vector3<T> other)
        {
            return self.X.Equals(other.X) &&
                   self.Y.Equals(other.Y) &&
                   self.Z.Equals(other.Z);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(System.Numerics.Vector3 other)
    {
        if (typeof(T) == typeof(float) && Vector128.IsHardwareAccelerated)
        {
            return BitCaster.BitCast<Vector3<T>, System.Numerics.Vector3>(this).AsVector128().Equals(other.AsVector128());
        }
        
        return float.CreateTruncating(X).Equals(other.X) &&
               float.CreateTruncating(Y).Equals(other.Y) &&
               float.CreateTruncating(Y).Equals(other.Y);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;
        
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
        
        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            3,
            destination,
            provider,
            out var shouldAppend
        );
        if (!shouldAppend)
        {
            charsWritten = 0;
            return false;
        }
        
        // Annoyingly we need to turn the span into a string for the string handler
        // We only assign it on first use, just in case we fail before that
        string? formatString;

        _ =
            handler.AppendLiteral("<") &&
            handler.AppendFormatted(X, formatString = format.Length > 0 ? new string(format) : null) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Y, formatString) &&
            handler.AppendLiteral(separator) &&
            handler.AppendLiteral(" ") &&
            handler.AppendFormatted(Z, formatString) &&
            handler.AppendLiteral(">");

        return destination.TryWrite(ref handler, out charsWritten);
    }

    private readonly Vector3<TOther> As<TOther>() where TOther : INumber<TOther>
    {
        return new Vector3<TOther>(
            TOther.CreateTruncating(X),
            TOther.CreateTruncating(Y),
            TOther.CreateTruncating(Z)
        );
    }

    public static explicit operator System.Numerics.Vector3(Vector3<T> self)
    {
        return new System.Numerics.Vector3(
            float.CreateTruncating(self.X),
            float.CreateTruncating(self.Y),
            float.CreateTruncating(self.Z)
        );
    }
    
    public static explicit operator checked System.Numerics.Vector3(Vector3<T> self)
    {
        return new System.Numerics.Vector3(
            float.CreateChecked(self.X),
            float.CreateChecked(self.Y),
            float.CreateChecked(self.Z)
        );
    }

    
    public static explicit operator Vector3<T>(System.Numerics.Vector3 self)
    {
        return new Vector3<T>(
            T.CreateTruncating(self.X),
            T.CreateTruncating(self.Y),
            T.CreateTruncating(self.Z)
        );
    }
    
    public static explicit operator checked Vector3<T>(System.Numerics.Vector3 self)
    {
        return new Vector3<T>(
            T.CreateChecked(self.X),
            T.CreateChecked(self.Y),
            T.CreateChecked(self.Z)
        );
    }

    #region Maths

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the addition operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z
        );
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    /// <remarks>The <see cref="Vector3.op_Division" /> method defines the division operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X / right.X,
            left.Y / right.Y,
            left.Z / right.Z
        );
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="value1">The vector.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    /// <remarks>The <see cref="Vector3.op_Division" /> method defines the division operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator /(Vector3<T> value1, T value2)
    {
        return value1 / new Vector3<T>(value2);
    }

    /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two <see cref="Vector3" /> objects are equal if each element in <paramref name="left" /> is equal to the corresponding element in <paramref name="right" />.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector3<T> left, Vector3<T> right)
    {
        return (left.X == right.X)
            && (left.Y == right.Y)
            && (left.Z == right.Z);
    }

    /// <summary>Returns a value that indicates whether two specified vectors are not equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector3<T> left, Vector3<T> right)
    {
        return !(left == right);

    }
    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X * right.X,
            left.Y * right.Y,
            left.Z * right.Z
        );
    }

    /// <summary>Multiplies the specified vector by the specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(Vector3<T> left, T right)
    {
        return left * new Vector3<T>(right);
    }

    /// <summary>Multiplies the scalar value by the specified vector.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    /// <remarks>The <see cref="Vector3.op_Multiply" /> method defines the multiplication operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(T left, Vector3<T> right)
    {
        return right * left;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the subtraction operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z
        );
    }

    /// <summary>Negates the specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    /// <remarks>The <see cref="op_UnaryNegation" /> method defines the unary negation operation for <see cref="Vector3" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator -(Vector3<T> value)
    {
        return Zero - value;
    }

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The absolute value vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Abs(Vector3<T> value)
    {
        return new Vector3<T>(
            T.Abs(value.X),
            T.Abs(value.Y),
            T.Abs(value.Z)
        );
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Add(Vector3<T> left, Vector3<T> right)
    {
        return left + right;
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Clamp(Vector3<T> value1, Vector3<T> min, Vector3<T> max)
    {
        // We must follow HLSL behavior in the case user specified min value is bigger than max value.
        return Min(Max(value1, min), max);
    }

    /// <summary>Computes the cross product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Cross(Vector3<T> vector1, Vector3<T> vector2)
    {
        return new Vector3<T>(
            (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y),
            (vector1.Z * vector2.X) - (vector1.X * vector2.Z),
            (vector1.X * vector2.Y) - (vector1.Y * vector2.X)
        );
    }
    
    /// <summary>Computes the cross product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Cross<TIntermediate>(Vector3<T> vector1, Vector3<T> vector2) where TIntermediate : INumber<TIntermediate>
    {
        return new Vector3<T>(
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.Z)) - (TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.Y))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.X)) - (TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Z))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Y)) - (TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.X)))
        );
    }
    
    /// <summary>Computes the Euclidean distance between the two given points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Distance<TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn>, IRootFunctions<TReturn>
    {
        var distanceSquared = DistanceSquared(value1, value2);
        return TReturn.Sqrt(TReturn.CreateTruncating(distanceSquared));
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared(Vector3<T> value1, Vector3<T> value2)
    {
        var difference = value1 - value2;
        return Dot(difference, difference);
    }

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    /// <param name="value1">The first point.</param>
    /// <param name="value2">The second point.</param>
    /// <returns>The distance squared.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn DistanceSquared<TReturn>(Vector3<T> value1, Vector3<T> value2) where TReturn : INumber<TReturn>
    {
        var difference = value1 - value2;
        return Dot<TReturn>(difference, difference);
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector resulting from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Divide(Vector3<T> left, Vector3<T> right)
    {
        return left / right;
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The vector that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Divide(Vector3<T> left, T divisor)
    {
        return left / divisor;
    }

    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot(Vector3<T> vector1, Vector3<T> vector2)
    {
        return (vector1.X * vector2.X)
             + (vector1.Y * vector2.Y)
             + (vector1.Z * vector2.Z);
    }
    
    /// <summary>Returns the dot product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturn Dot<TReturn>(Vector3<T> vector1, Vector3<T> vector2) where TReturn : INumber<TReturn>
    {
        return (TReturn.CreateTruncating(vector1.X) * TReturn.CreateTruncating(vector2.X))
               + (TReturn.CreateTruncating(vector1.Y) * TReturn.CreateTruncating(vector2.Y))
               + (TReturn.CreateTruncating(vector1.Z) * TReturn.CreateTruncating(vector2.Z));
    }

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
    /// <returns>The interpolated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TFloat> Lerp<TFloat>(Vector3<T> value1, Vector3<T> value2, TFloat amount) where TFloat : INumber<TFloat>, IFloatingPoint<TFloat>
    {
        return (value1.As<TFloat>() * (TFloat.One - amount)) + (value2.As<TFloat>() * amount);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TFloat> LerpClamped<TFloat>(Vector3<T> value1, Vector3<T> value2, TFloat amount) where TFloat : INumber<TFloat>, IFloatingPoint<TFloat>
    {
        amount = TFloat.Clamp(amount, TFloat.Zero, TFloat.One);
        return Lerp(value1, value2, amount);
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Max(Vector3<T> value1, Vector3<T> value2)
    {
        return new Vector3<T>( // using T.Max here would add an IsNaN check
            (value1.X > value2.X) ? value1.X : value2.X,
            (value1.Y > value2.Y) ? value1.Y : value2.Y,
            (value1.Z > value2.Z) ? value1.Z : value2.Z
        );
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Min(Vector3<T> value1, Vector3<T> value2)
    {
        return new Vector3<T>( // using T.Min here would add an IsNaN check
            (value1.X < value2.X) ? value1.X : value2.X,
            (value1.Y < value2.Y) ? value1.Y : value2.Y,
            (value1.Z < value2.Z) ? value1.Z : value2.Z
        );
    }

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(Vector3<T> left, Vector3<T> right)
    {
        return left * right;
    }

    /// <summary>Multiplies a vector by a specified scalar.</summary>
    /// <param name="left">The vector to multiply.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(Vector3<T> left, T right)
    {
        return left * right;
    }

    /// <summary>Multiplies a scalar value by a specified vector.</summary>
    /// <param name="left">The scaled value.</param>
    /// <param name="right">The vector.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Multiply(T left, Vector3<T> right)
    {
        return left * right;
    }

    /// <summary>Negates a specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Negate(Vector3<T> value)
    {
        return -value;
    }

    /// <summary>Returns a vector with the same direction as the specified vector, but with a length of one.</summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TReturn> Normalize<TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn>
    {
        return value.As<TReturn>() / value.Length<TReturn>();
    }

    /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">The normal of the surface being reflected off.</param>
    /// <returns>The reflected vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TReturn> Reflect<TReturn>(Vector3<T> vector, Vector3<T> normal) where TReturn : IFloatingPoint<TReturn>
    {
        var dot = Dot<TReturn>(vector, normal);
        return vector.As<TReturn>() - (TReturn.CreateTruncating(2) * (dot * normal.As<TReturn>()));
    }

    /// <summary>Returns a vector whose elements are the square root of each of a specified vector's elements.</summary>
    /// <param name="value">A vector.</param>
    /// <returns>The square root vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TReturn> SquareRoot<TReturn>(Vector3<T> value) where TReturn : IFloatingPoint<TReturn>, IRootFunctions<TReturn>
    {
        return new Vector3<TReturn>(
            TReturn.Sqrt(TReturn.CreateTruncating(value.X)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Y)),
            TReturn.Sqrt(TReturn.CreateTruncating(value.Z))
        );
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The difference vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Subtract(Vector3<T> left, Vector3<T> right)
    {
        return left - right;
    }

    // /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    // /// <param name="position">The vector to transform.</param>
    // /// <param name="matrix">The transformation matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector3<T> Transform(Vector3<T> position, Matrix4x4 matrix)
    // {
    //     return Vector4.Transform(position, in matrix.AsImpl()).AsVector128().AsVector3();
    // }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<TReturn> Transform<TReturn>(Vector3<T> value, Quaternion rotation) where TReturn : INumber<TReturn>
    {
        var x2 = rotation.X + rotation.X;
        var y2 = rotation.Y + rotation.Y;
        var z2 = rotation.Z + rotation.Z;

        var wx2 = TReturn.CreateTruncating(rotation.W * x2);
        var wy2 = TReturn.CreateTruncating(rotation.W * y2);
        var wz2 = TReturn.CreateTruncating(rotation.W * z2);
        var xx2 = TReturn.CreateTruncating(rotation.X * x2);
        var xy2 = TReturn.CreateTruncating(rotation.X * y2);
        var xz2 = TReturn.CreateTruncating(rotation.X * z2);
        var yy2 = TReturn.CreateTruncating(rotation.Y * y2);
        var yz2 = TReturn.CreateTruncating(rotation.Y * z2);
        var zz2 = TReturn.CreateTruncating(rotation.Z * z2);

        return new Vector3<TReturn>(
            TReturn.CreateTruncating(value.X) * (TReturn.One - yy2 - zz2) + TReturn.CreateTruncating(value.Y) * (xy2 - wz2) + TReturn.CreateTruncating(value.Z) * (xz2 + wy2),
            TReturn.CreateTruncating(value.X) * (xy2 + wz2) + TReturn.CreateTruncating(value.Y) * (TReturn.One - xx2 - zz2) + TReturn.CreateTruncating(value.Z) * (yz2 - wx2),
            TReturn.CreateTruncating(value.X) * (xz2 - wy2) + TReturn.CreateTruncating(value.Y) * (yz2 + wx2) + TReturn.CreateTruncating(value.Z) * (TReturn.One - xx2 - yy2)
        );
    }

    // /// <summary>Transforms a vector normal by the given 4x4 matrix.</summary>
    // /// <param name="normal">The source vector.</param>
    // /// <param name="matrix">The matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector3<T> TransformNormal(Vector3<T> normal, Matrix4x4 matrix)
    // {
    //     return TransformNormal(normal, in matrix.AsImpl());
    // }
    //
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal static Vector3<T> TransformNormal(Vector3<T> normal, in Matrix4x4.Impl matrix)
    // {
    //     Vector4 result = matrix.X * normal.X;
    //
    //     result += matrix.Y * normal.Y;
    //     result += matrix.Z * normal.Z;
    //
    //     return result.AsVector128().AsVector3();
    // }

    /// <summary>Returns the length of this vector object.</summary>
    /// <returns>The vector's length.</returns>
    /// <altmember cref="LengthSquared"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly TReturn Length<TReturn>() where TReturn : INumber<TReturn>, IFloatingPoint<TReturn>, IRootFunctions<TReturn>
    {
        var lengthSquared = LengthSquared<TReturn>();
        return TReturn.Sqrt(lengthSquared);
    }

    /// <summary>Returns the length of the vector squared.</summary>
    /// <returns>The vector's length squared.</returns>
    /// <remarks>This operation offers better performance than a call to the <see cref="Length" /> method.</remarks>
    /// <altmember cref="Length"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly TReturn LengthSquared<TReturn>() where TReturn : INumber<TReturn>
    {
        return Dot<TReturn>(this, this);
    }

    #endregion
}