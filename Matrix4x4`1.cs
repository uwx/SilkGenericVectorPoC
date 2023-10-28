#if !ON
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;

namespace GenericVector;

[StructLayout(LayoutKind.Sequential), DataContract, Serializable]
public readonly partial struct Matrix4X4<T> : IEquatable<Matrix4X4<T>>
    where T : INumberBase<T>
{
    /// <summary>
    /// Row 1 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public readonly Vector4D<T> X;

    /// <summary>
    /// Row 2 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public readonly Vector4D<T> Y;

    /// <summary>
    /// Row 3 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public readonly Vector4D<T> Z;

    /// <summary>
    /// Row 4 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public readonly Vector4D<T> W;

    /// <summary>
    /// Column 1 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public Vector4D<T> Column1 => new(X.X, Y.X, Z.X, W.X);

    /// <summary>
    /// Column 2 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public Vector4D<T> Column2 => new(X.Y, Y.Y, Z.Y, W.Y);

    /// <summary>
    /// Column 3 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public Vector4D<T> Column3 => new(X.Z, Y.Z, Z.Z, W.Z);

    /// <summary>
    /// Column 4 of the matrix.
    /// </summary>
    [IgnoreDataMember]
    public Vector4D<T> Column4 => new(X.W, Y.W, Z.W, W.W);

    /// <summary>Value at row 1, column 1 of the matrix.</summary>
    [DataMember]
    public T M11 => X.X;

    /// <summary>Value at row 1, column 2 of the matrix.</summary>
    [DataMember]
    public T M12 => X.Y;

    /// <summary>Value at row 1, column 3 of the matrix.</summary>
    [DataMember]
    public T M13 => X.Z;

    /// <summary>Value at row 1, column 4 of the matrix.</summary>
    [DataMember]
    public T M14 => X.W;

    /// <summary>Value at row 2, column 1 of the matrix.</summary>
    [DataMember]
    public T M21 => Y.X;

    /// <summary>Value at row 2, column 2 of the matrix.</summary>
    [DataMember]
    public T M22 => Y.Y;

    /// <summary>Value at row 2, column 3 of the matrix.</summary>
    [DataMember]
    public T M23 => Y.Z;

    /// <summary>Value at row 2, column 4 of the matrix.</summary>
    [DataMember]
    public T M24 => Y.W;

    /// <summary>Value at row 3, column 1 of the matrix.</summary>
    [DataMember]
    public T M31 => Z.X;

    /// <summary>Value at row 3, column 2 of the matrix.</summary>
    [DataMember]
    public T M32 => Z.Y;

    /// <summary>Value at row 3, column 3 of the matrix.</summary>
    [DataMember]
    public T M33 => Z.Z;

    /// <summary>Value at row 3, column 4 of the matrix.</summary>
    [DataMember]
    public T M34 => Z.W;

    /// <summary>Value at row 4, column 1 of the matrix.</summary>
    [DataMember]
    public T M41 => W.X;

    /// <summary>Value at row 4, column 2 of the matrix.</summary>
    [DataMember]
    public T M42 => W.Y;

    /// <summary>Value at row 4, column 3 of the matrix.</summary>
    [DataMember]
    public T M43 => W.Z;

    /// <summary>Value at row 4, column 4 of the matrix.</summary>
    [DataMember]
    public T M44 => W.W;

    /// <summary>Creates a 4x4 matrix from the specified components.</summary>
    /// <param name="m11">The value to assign to the first element in the first row.</param>
    /// <param name="m12">The value to assign to the second element in the first row.</param>
    /// <param name="m13">The value to assign to the third element in the first row.</param>
    /// <param name="m14">The value to assign to the fourth element in the first row.</param>
    /// <param name="m21">The value to assign to the first element in the second row.</param>
    /// <param name="m22">The value to assign to the second element in the second row.</param>
    /// <param name="m23">The value to assign to the third element in the second row.</param>
    /// <param name="m24">The value to assign to the third element in the second row.</param>
    /// <param name="m31">The value to assign to the first element in the third row.</param>
    /// <param name="m32">The value to assign to the second element in the third row.</param>
    /// <param name="m33">The value to assign to the third element in the third row.</param>
    /// <param name="m34">The value to assign to the fourth element in the third row.</param>
    /// <param name="m41">The value to assign to the first element in the fourth row.</param>
    /// <param name="m42">The value to assign to the second element in the fourth row.</param>
    /// <param name="m43">The value to assign to the third element in the fourth row.</param>
    /// <param name="m44">The value to assign to the fourth element in the fourth row.</param>
    public Matrix4X4(T m11, T m12, T m13, T m14,
                     T m21, T m22, T m23, T m24,
                     T m31, T m32, T m33, T m34,
                     T m41, T m42, T m43, T m44)
    {
        Unsafe.SkipInit(out this);

        X = new Vector4D<T>(m11, m12, m13, m14);
        Y = new Vector4D<T>(m21, m22, m23, m24);
        Z = new Vector4D<T>(m31, m32, m33, m34);
        W = new Vector4D<T>(m41, m42, m43, m44);
    }

    // /// <summary>Creates a <see cref="Matrix4X4{T}" /> object from a specified <see cref="Matrix3x2" /> object.</summary>
    // /// <param name="value">A 3x2 matrix.</param>
    // /// <remarks>This constructor creates a 4x4 matrix whose <see cref="M13" />, <see cref="M14" />, <see cref="M23" />, <see cref="M24" />, <see cref="M31" />, <see cref="M32" />, <see cref="M34" />, and <see cref="M43" /> components are zero, and whose <see cref="M33" /> and <see cref="M44" /> components are one.</remarks>
    // public Matrix4X4(Matrix3X2<T> value)
    // {
    //     Unsafe.SkipInit(out this);
    //
    //     X = new Vector4D<T>(value.X, T.Zero, T.Zero);
    //     Y = new Vector4D<T>(value.Y, T.Zero, T.Zero);
    //     Z = Vector4D<T>.UnitZ;
    //     W = new Vector4D<T>(value.Z, T.Zero, T.One);
    // }

    public Matrix4X4(Vector4D<T> x, Vector4D<T> y, Vector4D<T> z, Vector4D<T> w)
    {
        Unsafe.SkipInit(out this);

        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>Gets the multiplicative identity matrix.</summary>
    /// <value>Gets the multiplicative identity matrix.</value>
    public static Matrix4X4<T> Identity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(
            Vector4D<T>.UnitX,
            Vector4D<T>.UnitY,
            Vector4D<T>.UnitZ,
            Vector4D<T>.UnitW
        );
    }

    internal const uint ColumnCount = 4;
    internal const uint RowCount = 4;

    /// <summary>Gets or sets the element at the specified indices.</summary>
    /// <param name="row">The index of the row containing the element to get or set.</param>
    /// <param name="column">The index of the column containing the element to get or set.</param>
    /// <returns>The element at [<paramref name="row" />][<paramref name="column" />].</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="row" /> was less than zero or greater than the number of rows.
    /// -or-
    /// <paramref name="column" /> was less than zero or greater than the number of columns.
    /// </exception>
    public T this[int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)row, RowCount, nameof(row));

            return Unsafe.Add(ref Unsafe.AsRef(in this.X), row)[column];
        }
    }

    /// <summary>Indicates whether the current matrix is the identity matrix.</summary>
    /// <value><see langword="true" /> if the current matrix is the identity matrix; otherwise, <see langword="false" />.</value>
    public bool IsIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (X == Vector4D<T>.UnitX)
               && (Y == Vector4D<T>.UnitY)
               && (Z == Vector4D<T>.UnitZ)
               && (W == Vector4D<T>.UnitW);
    }

    /// <summary>Gets or sets the translation component of this matrix.</summary>
    /// <value>The translation component of the current instance.</value>
    public Vector3D<T> Translation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(W.X, W.Y, W.Z);
    }

    /// <summary>Adds each element in one matrix with its corresponding element in a second matrix.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The matrix that contains the summed values.</returns>
    /// <remarks>The <see cref="op_Addition" /> method defines the operation of the addition operator for <see cref="Matrix4X4{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> operator +(Matrix4X4<T> value1, Matrix4X4<T> value2)
    {
        return new Matrix4X4<T>(
            value1.X + value2.X,
            value1.Y + value2.Y,
            value1.Z + value2.Z,
            value1.W + value2.W
        );
    }

    /// <summary>Returns a value that indicates whether the specified matrices are equal.</summary>
    /// <param name="value1">The first matrix to compare.</param>
    /// <param name="value2">The second matrix to care</param>
    /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are equal; otherwise, <see langword="false" />.</returns>
    /// <remarks>Two matrices are equal if all their corresponding elements are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Matrix4X4<T> value1, Matrix4X4<T> value2)
    {
        return value1.Equals(value2);
    }

    /// <summary>Returns a value that indicates whether the specified matrices are not equal.</summary>
    /// <param name="value1">The first matrix to compare.</param>
    /// <param name="value2">The second matrix to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Matrix4X4<T> value1, Matrix4X4<T> value2)
    {
        return !(value1 == value2);
    }

    /// <summary>Multiplies two matrices together to compute the product.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The product matrix.</returns>
    /// <remarks>The <see cref="Matrix4X4{T}.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="Matrix4X4{T}" /> objects.</remarks>
    public static Matrix4X4<T> operator *(Matrix4X4<T> value1, Matrix4X4<T> value2)
    {
        return new Matrix4X4<T>(
            // x
            value2.X * value1.X.X +
            value2.Y * value1.X.Y +
            value2.Z * value1.X.Z +
            value2.W * value1.X.W,
            // y
            value2.X * value1.Y.X +
            value2.Y * value1.Y.Y +
            value2.Z * value1.Y.Z +
            value2.W * value1.Y.W,
            // z
            value2.X * value1.Z.X +
            value2.Y * value1.Z.Y +
            value2.Z * value1.Z.Z +
            value2.W * value1.Z.W,
            // w
            value2.X * value1.W.X +
            value2.Y * value1.W.Y +
            value2.Z * value1.W.Z +
            value2.W * value1.W.W
        );
    }

    /// <summary>Multiplies a matrix by a T to compute the product.</summary>
    /// <param name="value1">The matrix to scale.</param>
    /// <param name="value2">The scaling value to use.</param>
    /// <returns>The scaled matrix.</returns>
    /// <remarks>The <see cref="Matrix4X4{T}.op_Multiply" /> method defines the operation of the multiplication operator for <see cref="Matrix4X4{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> operator *(Matrix4X4<T> value1, T value2)
    {
        return new Matrix4X4<T>(
            value1.X * value2,
            value1.Y * value2,
            value1.Z * value2,
            value1.W * value2
        );
    }

    /// <summary>Subtracts each element in a second matrix from its corresponding element in a first matrix.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The matrix containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
    /// <remarks>The <see cref="op_Subtraction" /> method defines the operation of the subtraction operator for <see cref="Matrix4X4{T}" /> objects.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> operator -(Matrix4X4<T> value1, Matrix4X4<T> value2)
    {
        return new Matrix4X4<T>(
            value1.X - value2.X,
            value1.Y - value2.Y,
            value1.Z - value2.Z,
            value1.W - value2.W
        );
    }

    /// <summary>Negates the specified matrix by multiplying all its values by -1.</summary>
    /// <param name="value">The matrix to negate.</param>
    /// <returns>The negated matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> operator -(Matrix4X4<T> value)
    {
        return new Matrix4X4<T>(
            -value.X,
            -value.Y,
            -value.Z,
            -value.W
        );
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hash code.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <summary>Returns a string that represents this matrix.</summary>
    /// <returns>The string representation of this matrix.</returns>
    /// <remarks>The numeric values in the returned string are formatted by using the conventions of the current culture. For example, for the en-US culture, the returned string might appear as <c>{ {M11:1.1 M12:1.2 M13:1.3 M14:1.4} {M21:2.1 M22:2.2 M23:2.3 M24:2.4} {M31:3.1 M32:3.2 M33:3.3 M34:3.4} {M41:4.1 M42:4.2 M43:4.3 M44:4.4} }</c>.</remarks>
    public override string ToString()
        => $$"""{ {M11:{{M11}} M12:{{M12}} M13:{{M13}} M14:{{M14}}} {M21:{{M21}} M22:{{M22}} M23:{{M23}} M24:{{M24}}} {M31:{{M31}} M32:{{M32}} M33:{{M33}} M34:{{M34}}} {M41:{{M41}} M42:{{M42}} M43:{{M43}} M44:{{M44}}} }""";
    
    /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    /// <remarks>The current instance and <paramref name="obj" /> are equal if <paramref name="obj" /> is a <see cref="Matrix4X4{T}" /> object and the corresponding elements of each matrix are equal.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Matrix4X4<T> other && Equals(other);

    /// <summary>Returns a value that indicates whether this instance and another 4x4 matrix are equal.</summary>
    /// <param name="other">The other matrix.</param>
    /// <returns><see langword="true" /> if the two matrices are equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Matrix4X4<T> other)
    {
        // This function needs to account for floating-point equality around NaN
        // and so must behave equivalently to the underlying T/double.Equals

        if (Vector512.IsHardwareAccelerated && Vector512<T>.IsSupported && Vector512<T>.Count >= RowCount * ColumnCount)
        {
            var selfVec = Vector512.Create(
                Vector256.Create(
                    X.AsVector128(),
                    Y.AsVector128()
                ),
                Vector256.Create(
                    Z.AsVector128(),
                    W.AsVector128()
                )
            );
            
            var otherVec = Vector512.Create(
                Vector256.Create(
                    other.X.AsVector128(),
                    other.Y.AsVector128()
                ),
                Vector256.Create(
                    other.Z.AsVector128(),
                    other.W.AsVector128()
                )
            );
            return selfVec.Equals(otherVec);
        }

        if (Vector256.IsHardwareAccelerated && Vector256<T>.IsSupported && Vector256<T>.Count >= (RowCount * ColumnCount) / 2)
        {
            var selfVec1 = Vector256.Create(
                X.AsVector128(),
                Y.AsVector128()
            );
            var selfVec2 = Vector256.Create(
                Z.AsVector128(),
                W.AsVector128()
            );

            var otherVec1 = Vector256.Create(
                other.X.AsVector128(),
                other.Y.AsVector128()
            );

            var otherVec2 = Vector256.Create(
                other.Z.AsVector128(),
                other.W.AsVector128()
            );
            
            return selfVec1.Equals(otherVec1) && selfVec2.Equals(otherVec2);
        }

        return SoftwareFallback(this, other);

        static bool SoftwareFallback(in Matrix4X4<T> self, in Matrix4X4<T> other)
        {
            // This will use vectorized equals if possible
            return self.X.Equals(other.X)
                   && self.Y.Equals(other.Y)
                   && self.Z.Equals(other.Z)
                   && self.W.Equals(other.W);
        }
    }

}
#endif