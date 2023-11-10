using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GenericVector;

public static partial class Vector2D
{
    // /// <summary>Transforms a vector by a specified 3x2 matrix.</summary>
    // /// <param name="position">The vector to transform.</param>
    // /// <param name="matrix">The transformation matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector2D<T> Transform<T>(Vector2D<T> position, Matrix3X2<T> matrix)
    //     where T : INumberBase<T>
    // {
    //     Vector2D<T> result = matrix.X * position.X;
    //
    //     result += matrix.Y * position.Y;
    //     result += matrix.Z;
    //
    //     return result;
    // }

    /// <summary>Transforms a vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Transform<T>(Vector2D<T> position, Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        return (Vector2D<T>)Vector4D.Transform(position, matrix);
    }

    /// <summary>Transforms a vector by the specified Quaternion<T> rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> Transform<T>(Vector2D<T> value, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T yy2 = rotation.Y * y2;
        T zz2 = rotation.Z * z2;

        return new Vector2D<T>(
            value.X * (T.One - yy2 - zz2) + value.Y * (xy2 - wz2),
            value.X * (xy2 + wz2) + value.Y * (T.One - xx2 - zz2)
        );
    }

    // /// <summary>Transforms a vector normal by the given 3x2 matrix.</summary>
    // /// <param name="normal">The source vector.</param>
    // /// <param name="matrix">The matrix.</param>
    // /// <returns>The transformed vector.</returns>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Vector2D<T> TransformNormal<T>(Vector2D<T> normal, Matrix3X2<T> matrix) where T : INumberBase<T>
    // {
    //     Vector2D<T> result = matrix.X * normal.X;
    //
    //     result += matrix.Y * normal.Y;
    //
    //     return result;
    // }

    /// <summary>Transforms a vector normal by the given 4x4 matrix.</summary>
    /// <param name="normal">The source vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2D<T> TransformNormal<T>(Vector2D<T> normal, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        Vector4D<T> result = matrix.X * normal.X;

        result += matrix.Y * normal.Y;

        return (Vector2D<T>)result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> AsVector128<T>(this Vector2D<T> self) where T : INumberBase<T>
    {
        return new Vector4D<T>(self, T.Zero, T.Zero).AsVector128();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<TOut> AsVector128<T, TOut>(this Vector2D<T> self) where T : INumberBase<T>
    {
        return new Vector4D<T>(self, T.Zero, T.Zero).AsVector128<T, TOut>();
    }
}

public static partial class Vector3D
{
    /// <summary>Computes the cross product of two vectors.</summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<T> Cross<T>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T>
    {
        return new Vector3D<T>(
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
    public static Vector3D<T> Cross<T, TIntermediate>(Vector3D<T> vector1, Vector3D<T> vector2) where T : INumberBase<T> where TIntermediate : INumberBase<TIntermediate>
    {
        return new Vector3D<T>(
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.Z)) - (TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.Y))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.Z) * TIntermediate.CreateTruncating(vector2.X)) - (TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Z))),
            T.CreateTruncating((TIntermediate.CreateTruncating(vector1.X) * TIntermediate.CreateTruncating(vector2.Y)) - (TIntermediate.CreateTruncating(vector1.Y) * TIntermediate.CreateTruncating(vector2.X)))
        );
    }

    /// <summary>Transforms a vector by the specified Quaternion rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3D<TReturn> Transform<T, TQuat, TReturn>(Vector3D<T> value, Quaternion<TQuat> rotation) where T : INumberBase<T> where TReturn : INumberBase<TReturn> where TQuat : ITrigonometricFunctions<TQuat>, IRootFunctions<TQuat>
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

        return new Vector3D<TReturn>(
            TReturn.CreateTruncating(value.X) * (TReturn.One - yy2 - zz2) + TReturn.CreateTruncating(value.Y) * (xy2 - wz2) + TReturn.CreateTruncating(value.Z) * (xz2 + wy2),
            TReturn.CreateTruncating(value.X) * (xy2 + wz2) + TReturn.CreateTruncating(value.Y) * (TReturn.One - xx2 - zz2) + TReturn.CreateTruncating(value.Z) * (yz2 - wx2),
            TReturn.CreateTruncating(value.X) * (xz2 - wy2) + TReturn.CreateTruncating(value.Y) * (yz2 + wx2) + TReturn.CreateTruncating(value.Z) * (TReturn.One - xx2 - yy2)
        );
    }

    /// <summary>Transforms a vector normal by the given 4x4 matrix.</summary>
    /// <param name="normal">The source vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3D<T> TransformNormal<T>(Vector3D<T> normal, Matrix4X4<T> matrix) where T : INumberBase<T>
    {
        var matrixX = new Vector4D<T>(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
        var matrixY = new Vector4D<T>(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
        var matrixZ = new Vector4D<T>(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
        // var matrixW = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);
        
        var result = matrixX * normal.X;
        result += matrixY * normal.Y;
        result += matrixZ * normal.Z;
        return Unsafe.BitCast<Vector4D<T>, Vector3D<T>>(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> AsVector128<T>(this Vector3D<T> self) where T : INumberBase<T>
    {
        return new Vector4D<T>(self, T.Zero).AsVector128();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<TOut> AsVector128<T, TOut>(this Vector3D<T> self) where T : INumberBase<T>
    {
        return new Vector4D<T>(self, T.Zero).AsVector128<T, TOut>();
    }
}

public static partial class Vector4D
{
    /// <summary>Transforms a two-dimensional vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector2D<T> position, Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        Vector4D<T> result = matrix.X * position.X;

        result += matrix.Y * position.Y;
        result += matrix.W;

        return result;
    }

    /// <summary>Transforms a two-dimensional vector by the specified Quaternion<T> rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector2D<T> value, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wx2 = rotation.W * x2;
        T wy2 = rotation.W * y2;
        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T xz2 = rotation.X * z2;
        T yy2 = rotation.Y * y2;
        T yz2 = rotation.Y * z2;
        T zz2 = rotation.Z * z2;

        return new Vector4D<T>(
            value.X * (T.One - yy2 - zz2) + value.Y * (xy2 - wz2),
            value.X * (xy2 + wz2) + value.Y * (T.One - xx2 - zz2),
            value.X * (xz2 - wy2) + value.Y * (yz2 + wx2),
            T.One
        );
    }

    /// <summary>Transforms a three-dimensional vector by a specified 4x4 matrix.</summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector3D<T> position, Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        Vector4D<T> result = matrix.X * position.X;

        result += matrix.Y * position.Y;
        result += matrix.Z * position.Z;
        result += matrix.W;

        return result;
    }

    /// <summary>Transforms a three-dimensional vector by the specified Quaternion{T} rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector3D<T> value, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wx2 = rotation.W * x2;
        T wy2 = rotation.W * y2;
        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T xz2 = rotation.X * z2;
        T yy2 = rotation.Y * y2;
        T yz2 = rotation.Y * z2;
        T zz2 = rotation.Z * z2;

        return new Vector4D<T>(
            value.X * (T.One - yy2 - zz2) + value.Y * (xy2 - wz2) + value.Z * (xz2 + wy2),
            value.X * (xy2 + wz2) + value.Y * (T.One - xx2 - zz2) + value.Z * (yz2 - wx2),
            value.X * (xz2 - wy2) + value.Y * (yz2 + wx2) + value.Z * (T.One - xx2 - yy2),
            T.One
        );
    }

    /// <summary>Transforms a four-dimensional vector by a specified 4x4 matrix.</summary>
    /// <param name="vector">The vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector4D<T> vector, Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        Vector4D<T> result = matrix.X * vector.X;

        result += matrix.Y * vector.Y;
        result += matrix.Z * vector.Z;
        result += matrix.W * vector.W;

        return result;
    }

    /// <summary>Transforms a four-dimensional vector by the specified Quaternion<T> rotation value.</summary>
    /// <param name="value">The vector to rotate.</param>
    /// <param name="rotation">The rotation to apply.</param>
    /// <returns>The transformed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4D<T> Transform<T>(Vector4D<T> value, Quaternion<T> rotation)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T x2 = rotation.X + rotation.X;
        T y2 = rotation.Y + rotation.Y;
        T z2 = rotation.Z + rotation.Z;

        T wx2 = rotation.W * x2;
        T wy2 = rotation.W * y2;
        T wz2 = rotation.W * z2;
        T xx2 = rotation.X * x2;
        T xy2 = rotation.X * y2;
        T xz2 = rotation.X * z2;
        T yy2 = rotation.Y * y2;
        T yz2 = rotation.Y * z2;
        T zz2 = rotation.Z * z2;

        return new Vector4D<T>(
            value.X * (T.One - yy2 - zz2) + value.Y * (xy2 - wz2) + value.Z * (xz2 + wy2),
            value.X * (xy2 + wz2) + value.Y * (T.One - xx2 - zz2) + value.Z * (yz2 - wx2),
            value.X * (xz2 - wy2) + value.Y * (yz2 + wx2) + value.Z * (T.One - xx2 - yy2),
            value.W);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> AsVector128<T>(this Vector4D<T> self) where T : INumberBase<T>
    {
        if (!Vector128<T>.IsSupported)
        {
            throw new ArgumentException($"{typeof(T)} cannot be in a Vector128");
        }
        return Unsafe.BitCast<Vector4D<T>, Vector128<T>>(self);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<TOut> AsVector128<T, TOut>(this Vector4D<T> self) where T : INumberBase<T>
    {
        unsafe
        {
            if (sizeof(T) != sizeof(TOut))
            {
                throw new ArgumentException($"Sizes do not match");
            }
        }

        if (!Vector128<TOut>.IsSupported)
        {
            throw new ArgumentException($"{typeof(T)} cannot be in a Vector128");
        }
        return Unsafe.BitCast<Vector4D<T>, Vector128<TOut>>(self);
    }
}