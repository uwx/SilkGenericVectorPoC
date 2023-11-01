using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace GenericVector.Experimental;

public readonly partial record struct Vector2i<TScalar> :
    IBinaryIntegerVector<Vector2i<TScalar>, TScalar>,
    IVector2<Vector2i<TScalar>, TScalar>
    where TScalar : IBinaryInteger<TScalar>
{
    internal const int ElementCount = 2;
    
    /// <summary>The X component of the vector.</summary>
    [DataMember]
    public TScalar X { get; }

    /// <summary>The Y component of the vector.</summary>
    [DataMember]
    public TScalar Y { get; }
    
    public static Vector2i<TScalar> UnitX => new(TScalar.One, TScalar.Zero);
    public static Vector2i<TScalar> UnitY => new(TScalar.Zero, TScalar.One);

    /// <summary>Gets a vector whose 2 elements are equal to zero.</summary>
    /// <value>A vector whose two elements are equal to zero (that is, it returns the vector <c>(0,0)</c>.</value>
    public static Vector2i<TScalar> Zero => new(TScalar.Zero);

    /// <summary>Gets a vector whose 2 elements are equal to one.</summary>
    /// <value>Returns <see cref="Vector2D{T}" />.</value>
    /// <remarks>A vector whose two elements are equal to one (that is, it returns the vector <c>(1,1)</c>.</remarks>
    public static Vector2i<TScalar> One => new(TScalar.One);

    public TScalar this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.AsSpan()[index];
    }

    public Vector2i(TScalar x, TScalar y)
    {
        Unsafe.SkipInit(out this);

        X = x;
        Y = y;
    }
    
    public Vector2i(TScalar value) : this(value, value)
    {
    }
    
    /// <summary>Constructs a vector from the given <see cref="ReadOnlySpan{T}" />. The span must contain at least 2 elements.</summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector2i(ReadOnlySpan<TScalar> values)
    {
        Unsafe.SkipInit(out this);

        ArgumentOutOfRangeException.ThrowIfLessThan(values.Length, Count, nameof(values));

        this = Unsafe.ReadUnaligned<Vector2i<TScalar>>(ref Unsafe.As<TScalar, byte>(ref MemoryMarshal.GetReference(values)));
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

    // public static bool operator ==(Vector2i<TScalar> left, Vector2i<TScalar> right) => left.Equals(right);
    // public static bool operator !=(Vector2i<TScalar> left, Vector2i<TScalar> right) => !(left == right);

    public override string ToString() => ToString("G", null);
    public string ToString(string? format) => ToString(format, null);
    public string ToString(string? format, IFormatProvider? formatProvider) => ;
    
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => ;
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => ;

    public static Vector2i<TScalar> Parse(string s, IFormatProvider? provider) => ;
    public static Vector2i<TScalar> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => ;
    public static Vector2i<TScalar> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => ;
    public static bool TryParse(string? s, IFormatProvider? provider, out Vector2i<TScalar> result) => ;
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2i<TScalar> result) => ;
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Vector2i<TScalar> result) => ;

    public ReadOnlySpan<TScalar>.Enumerator GetEnumerator() => this.AsSpan().GetEnumerator();
    IEnumerator<TScalar> IEnumerable<TScalar>.GetEnumerator()
    {
        yield return X;
        yield return Y;
    }
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TScalar>)this).GetEnumerator();

    public bool Equals(Vector2i<TScalar> other) => SpeedHelpers2.Equal<Vector2i<TScalar>, TScalar>(this, other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    TScalar IVector<Vector2i<TScalar>, TScalar>.LengthSquared() => ;
    Vector2i<TScalar> IVectorEquatable<Vector2i<TScalar>, TScalar>.ScalarsEqual(Vector2i<TScalar> other) => SpeedHelpers2.EqualIntoVector<Vector2i<TScalar>, TScalar>(this, other);

    static ReadOnlySpan<TScalar> IVector<Vector2i<TScalar>, TScalar>.AsSpan(Vector2i<TScalar> vec) => vec.AsSpan();
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.GetUnitVector(uint dimension) => dimension switch
    {
        0 => UnitX,
        1 => UnitY,
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "dimension must be >= 0, <= 1")
    };
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Clamp(Vector2i<TScalar> value1, Vector2i<TScalar> min, Vector2i<TScalar> max) => Vector2i.Clamp(value1, min, max);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Max(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.Max(value1, value2);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Min(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.Min(value1, value2);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Abs(Vector2i<TScalar> value) => Vector2i.Abs(value);
    static TScalar IVector<Vector2i<TScalar>, TScalar>.DistanceSquared(Vector2i<TScalar> value1, Vector2i<TScalar> value2) => Vector2i.DistanceSquared(value1, value2);
    static TScalar IVector<Vector2i<TScalar>, TScalar>.Dot(Vector2i<TScalar> vector1, Vector2i<TScalar> vector2) => Vector2i.Dot(vector1, vector2);

    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, TScalar[] array) => vector.CopyTo(array);
    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, TScalar[] array, int index) => vector.CopyTo(array, index);
    static void IVector<Vector2i<TScalar>, TScalar>.CopyTo(Vector2i<TScalar> vector, Span<TScalar> destination) => vector.CopyTo(destination);
    static bool IVector<Vector2i<TScalar>, TScalar>.TryCopyTo(Vector2i<TScalar> vector, Span<TScalar> destination) => vector.TryCopyTo(destination);

    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromChecked<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (Vector2i<TScalar>)(object)value;
            return true;
        }
        
        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }
        
        // Fast-path builtin convertible scalar types
        if ((
            typeof(TOtherScalar) == typeof(NFloat) ||
            typeof(TOtherScalar) == typeof(BigInteger) ||
            typeof(TOtherScalar) == typeof(Complex) ||            
            typeof(TOtherScalar) == typeof(byte) ||
            typeof(TOtherScalar) == typeof(char) ||
            typeof(TOtherScalar) == typeof(decimal) ||
            typeof(TOtherScalar) == typeof(ushort) ||
            typeof(TOtherScalar) == typeof(uint) ||
            typeof(TOtherScalar) == typeof(ulong) ||
            typeof(TOtherScalar) == typeof(nuint) ||
            typeof(TOtherScalar) == typeof(double) ||
            typeof(TOtherScalar) == typeof(Half) ||
            typeof(TOtherScalar) == typeof(short) ||
            typeof(TOtherScalar) == typeof(int) ||
            typeof(TOtherScalar) == typeof(long) ||
            typeof(TOtherScalar) == typeof(Int128) ||
            typeof(TOtherScalar) == typeof(nint) ||
            typeof(TOtherScalar) == typeof(sbyte) ||
            typeof(TOtherScalar) == typeof(float) ||
            typeof(TOtherScalar) == typeof(UInt128)
        ) && (
            typeof(TScalar) == typeof(NFloat) ||
            typeof(TScalar) == typeof(BigInteger) ||
            typeof(TScalar) == typeof(Complex) ||            
            typeof(TScalar) == typeof(byte) ||
            typeof(TScalar) == typeof(char) ||
            typeof(TScalar) == typeof(decimal) ||
            typeof(TScalar) == typeof(ushort) ||
            typeof(TScalar) == typeof(uint) ||
            typeof(TScalar) == typeof(ulong) ||
            typeof(TScalar) == typeof(nuint) ||
            typeof(TScalar) == typeof(double) ||
            typeof(TScalar) == typeof(Half) ||
            typeof(TScalar) == typeof(short) ||
            typeof(TScalar) == typeof(int) ||
            typeof(TScalar) == typeof(long) ||
            typeof(TScalar) == typeof(Int128) ||
            typeof(TScalar) == typeof(nint) ||
            typeof(TScalar) == typeof(sbyte) ||
            typeof(TScalar) == typeof(float) ||
            typeof(TScalar) == typeof(UInt128)
        ))
        {
            result = new Vector2i<TScalar>(TScalar.CreateChecked(value[0]), TScalar.CreateChecked(value[1]));

            return true;
        }

        try
        {
            result = new Vector2i<TScalar>(TScalar.CreateChecked(value[0]), TScalar.CreateChecked(value[1]));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromSaturating<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (Vector2i<TScalar>)(object)value;
            return true;
        }

        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }
        
        // Fast-path builtin convertible scalar types
        if ((
            typeof(TOtherScalar) == typeof(NFloat) ||
            typeof(TOtherScalar) == typeof(BigInteger) ||
            typeof(TOtherScalar) == typeof(Complex) ||            
            typeof(TOtherScalar) == typeof(byte) ||
            typeof(TOtherScalar) == typeof(char) ||
            typeof(TOtherScalar) == typeof(decimal) ||
            typeof(TOtherScalar) == typeof(ushort) ||
            typeof(TOtherScalar) == typeof(uint) ||
            typeof(TOtherScalar) == typeof(ulong) ||
            typeof(TOtherScalar) == typeof(nuint) ||
            typeof(TOtherScalar) == typeof(double) ||
            typeof(TOtherScalar) == typeof(Half) ||
            typeof(TOtherScalar) == typeof(short) ||
            typeof(TOtherScalar) == typeof(int) ||
            typeof(TOtherScalar) == typeof(long) ||
            typeof(TOtherScalar) == typeof(Int128) ||
            typeof(TOtherScalar) == typeof(nint) ||
            typeof(TOtherScalar) == typeof(sbyte) ||
            typeof(TOtherScalar) == typeof(float) ||
            typeof(TOtherScalar) == typeof(UInt128)
        ) && (
            typeof(TScalar) == typeof(NFloat) ||
            typeof(TScalar) == typeof(BigInteger) ||
            typeof(TScalar) == typeof(Complex) ||            
            typeof(TScalar) == typeof(byte) ||
            typeof(TScalar) == typeof(char) ||
            typeof(TScalar) == typeof(decimal) ||
            typeof(TScalar) == typeof(ushort) ||
            typeof(TScalar) == typeof(uint) ||
            typeof(TScalar) == typeof(ulong) ||
            typeof(TScalar) == typeof(nuint) ||
            typeof(TScalar) == typeof(double) ||
            typeof(TScalar) == typeof(Half) ||
            typeof(TScalar) == typeof(short) ||
            typeof(TScalar) == typeof(int) ||
            typeof(TScalar) == typeof(long) ||
            typeof(TScalar) == typeof(Int128) ||
            typeof(TScalar) == typeof(nint) ||
            typeof(TScalar) == typeof(sbyte) ||
            typeof(TScalar) == typeof(float) ||
            typeof(TScalar) == typeof(UInt128)
        ))
        {
            result = new Vector2i<TScalar>(TScalar.CreateSaturating(value[0]), TScalar.CreateSaturating(value[1]));

            return true;
        }

        try
        {
            result = new Vector2i<TScalar>(TScalar.CreateSaturating(value[0]), TScalar.CreateSaturating(value[1]));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertFromTruncating<TOther, TOtherScalar>(TOther value, out Vector2i<TScalar> result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (Vector2i<TScalar>)(object)value;
            return true;
        }

        if (TOther.Count < ElementCount)
        {
            result = default;
            return false;
        }
        
        // Fast-path builtin convertible scalar types
        if ((
            typeof(TOtherScalar) == typeof(NFloat) ||
            typeof(TOtherScalar) == typeof(BigInteger) ||
            typeof(TOtherScalar) == typeof(Complex) ||            
            typeof(TOtherScalar) == typeof(byte) ||
            typeof(TOtherScalar) == typeof(char) ||
            typeof(TOtherScalar) == typeof(decimal) ||
            typeof(TOtherScalar) == typeof(ushort) ||
            typeof(TOtherScalar) == typeof(uint) ||
            typeof(TOtherScalar) == typeof(ulong) ||
            typeof(TOtherScalar) == typeof(nuint) ||
            typeof(TOtherScalar) == typeof(double) ||
            typeof(TOtherScalar) == typeof(Half) ||
            typeof(TOtherScalar) == typeof(short) ||
            typeof(TOtherScalar) == typeof(int) ||
            typeof(TOtherScalar) == typeof(long) ||
            typeof(TOtherScalar) == typeof(Int128) ||
            typeof(TOtherScalar) == typeof(nint) ||
            typeof(TOtherScalar) == typeof(sbyte) ||
            typeof(TOtherScalar) == typeof(float) ||
            typeof(TOtherScalar) == typeof(UInt128)
        ) && (
            typeof(TScalar) == typeof(NFloat) ||
            typeof(TScalar) == typeof(BigInteger) ||
            typeof(TScalar) == typeof(Complex) ||            
            typeof(TScalar) == typeof(byte) ||
            typeof(TScalar) == typeof(char) ||
            typeof(TScalar) == typeof(decimal) ||
            typeof(TScalar) == typeof(ushort) ||
            typeof(TScalar) == typeof(uint) ||
            typeof(TScalar) == typeof(ulong) ||
            typeof(TScalar) == typeof(nuint) ||
            typeof(TScalar) == typeof(double) ||
            typeof(TScalar) == typeof(Half) ||
            typeof(TScalar) == typeof(short) ||
            typeof(TScalar) == typeof(int) ||
            typeof(TScalar) == typeof(long) ||
            typeof(TScalar) == typeof(Int128) ||
            typeof(TScalar) == typeof(nint) ||
            typeof(TScalar) == typeof(sbyte) ||
            typeof(TScalar) == typeof(float) ||
            typeof(TScalar) == typeof(UInt128)
        ))
        {
            result = new Vector2i<TScalar>(TScalar.CreateTruncating(value[0]), TScalar.CreateTruncating(value[1]));

            return true;
        }

        try
        {
            result = new Vector2i<TScalar>(TScalar.CreateTruncating(value[0]), TScalar.CreateTruncating(value[1]));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToChecked<TOther, TOtherScalar>(Vector2i<TScalar> value, out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToSaturating<TOther, TOtherScalar>(Vector2i<TScalar> value, out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }
    static bool IVector<Vector2i<TScalar>, TScalar>.TryConvertToTruncating<TOther, TOtherScalar>(Vector2i<TScalar> value, out TOther result)
    {
        if (typeof(TOther) == typeof(Vector2i<TScalar>))
        {
            result = (TOther)(object)value;
            return true;
        }

        result = default;
        return false;
    }

    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Create(TScalar scalar) => new(scalar);
    static Vector2i<TScalar> IVector<Vector2i<TScalar>, TScalar>.Create(ReadOnlySpan<TScalar> values) => new(values);
    static Vector2i<TScalar> IVector2<Vector2i<TScalar>, TScalar>.Create(TScalar x, TScalar y) => new(x, y);

    #region Int-specific code

    static bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector2i<TScalar> value) => ;
    static bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out Vector2i<TScalar> value) => ;
    bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryWriteBigEndian(Span<byte> destination, out int bytesWritten) => ;
    bool IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) => ;

    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.CopySign(Vector2i<TScalar> value, Vector2i<TScalar> sign) => Vector2i.CopySign(value, sign);
    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.CopySign(Vector2i<TScalar> value, TScalar sign) => Vector2i.CopySign(value, sign);
    static Vector2i<TScalar> INumberVector<Vector2i<TScalar>, TScalar>.Sign(Vector2i<TScalar> value) => Vector2i.Sign(value);
    static Vector2i<TScalar> IBinaryNumberVector<Vector2i<TScalar>, TScalar>.Log2(Vector2i<TScalar> value) => Vector2.Log2(value);
    static Vector2i<TScalar> IBinaryIntegerVector<Vector2i<TScalar>, TScalar>.PopCount(Vector2i<TScalar> value) => Vector2.PopCount(value);

    #endregion

}