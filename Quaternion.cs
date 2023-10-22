using System.Numerics;
using System.Runtime.CompilerServices;

namespace GenericVector;

public static class Quaternion
{
    /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Add<T>(Quaternion<T> value1, Quaternion<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return value1 + value2;
    }

    /// <summary>Concatenates two quaternions.</summary>
    /// <param name="value1">The first quaternion rotation in the series.</param>
    /// <param name="value2">The second quaternion rotation in the series.</param>
    /// <returns>A new quaternion representing the concatenation of the <paramref name="value1" /> rotation followed by the <paramref name="value2" /> rotation.</returns>
    public static Quaternion<T> Concatenate<T>(Quaternion<T> value1, Quaternion<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        Quaternion<T> ans;

        // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
        // So that's why value2 goes q1 and value1 goes q2.
        var q1x = value2.X;
        var q1y = value2.Y;
        var q1z = value2.Z;
        var q1w = value2.W;

        var q2x = value1.X;
        var q2y = value1.Y;
        var q2z = value1.Z;
        var q2w = value1.W;

        // cross(av, bv)
        var cx = q1y * q2z - q1z * q2y;
        var cy = q1z * q2x - q1x * q2z;
        var cz = q1x * q2y - q1y * q2x;

        var dot = q1x * q2x + q1y * q2y + q1z * q2z;

        return new Quaternion<T>(
            q1x * q2w + q2x * q1w + cx,
            q1y * q2w + q2y * q1w + cy,
            q1z * q2w + q2z * q1w + cz,
            q1w * q2w - dot
        );
    }

    /// <summary>Returns the conjugate of a specified quaternion.</summary>
    /// <param name="value">The quaternion.</param>
    /// <returns>A new quaternion that is the conjugate of <see langword="value" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Conjugate<T>(Quaternion<T> value)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return Multiply(value, new Vector4D<T>(-T.One, -T.One, -T.One, T.One));
    }

    /// <summary>Creates a quaternion from a unit vector and an angle to rotate around the vector.</summary>
    /// <param name="axis">The unit vector to rotate around.</param>
    /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
    /// <returns>The newly created quaternion.</returns>
    /// <remarks><paramref name="axis" /> vector must be normalized before calling this method or the resulting <see cref="Quaternion<T>" /> will be incorrect.</remarks>
    public static Quaternion<T> CreateFromAxisAngle<T>(Vector3D<T> axis, T angle)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        var halfAngle = angle * NumericConstants<T>.Half;
        var s = T.Sin(halfAngle);
        var c = T.Cos(halfAngle);

        return new Quaternion<T>(
            axis.X * s,
            axis.Y * s,
            axis.Z * s,
            c
        );
    }

    /// <summary>Creates a quaternion from the specified rotation matrix.</summary>
    /// <param name="matrix">The rotation matrix.</param>
    /// <returns>The newly created quaternion.</returns>
    public static Quaternion<T> CreateFromRotationMatrix<T>(Matrix4X4<T> matrix)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>, IComparisonOperators<T, T, bool>
    {
        T trace = matrix.M11 + matrix.M22 + matrix.M33;

        Quaternion<T> q;

        if (trace > T.Zero)
        {
            T s = T.Sqrt(trace + T.One);
            var s1 = s;
            s = NumericConstants<T>.Half / s;
            q = new Quaternion<T>(
                s1 * NumericConstants<T>.Half,
                (matrix.M23 - matrix.M32) * s,
                (matrix.M31 - matrix.M13) * s,
                (matrix.M12 - matrix.M21) * s
            );
        }
        else
        {
            if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
            {
                T s = T.Sqrt(T.One + matrix.M11 - matrix.M22 - matrix.M33);
                var invS = NumericConstants<T>.Half / s;
                q = new Quaternion<T>(
                    NumericConstants<T>.Half * s,
                    (matrix.M12 + matrix.M21) * invS,
                    (matrix.M13 + matrix.M31) * invS,
                    (matrix.M23 - matrix.M32) * invS
                );
            }
            else if (matrix.M22 > matrix.M33)
            {
                T s = T.Sqrt(T.One + matrix.M22 - matrix.M11 - matrix.M33);
                var invS = NumericConstants<T>.Half / s;
                q = new Quaternion<T>(
                    (matrix.M21 + matrix.M12) * invS,
                    NumericConstants<T>.Half * s,
                    (matrix.M32 + matrix.M23) * invS,
                    (matrix.M31 - matrix.M13) * invS
                );
            }
            else
            {
                T s = T.Sqrt(T.One + matrix.M33 - matrix.M11 - matrix.M22);
                var invS = NumericConstants<T>.Half / s;
                q = new Quaternion<T>(
                    (matrix.M31 + matrix.M13) * invS,
                    (matrix.M32 + matrix.M23) * invS,
                    NumericConstants<T>.Half * s,
                    (matrix.M12 - matrix.M21) * invS
                );
            }
        }

        return q;
    }

    /// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
    /// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
    /// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
    /// <param name="roll">The roll angle, in radians, around the Z axis.</param>
    /// <returns>The resulting quaternion.</returns>
    public static Quaternion<T> CreateFromYawPitchRoll<T>(T yaw, T pitch, T roll)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        //  Roll first, about axis the object is facing, then
        //  pitch upward, then yaw to face into the new heading
        T sr, cr, sp, cp, sy, cy;

        var halfRoll = roll * NumericConstants<T>.Half;
        sr = T.Sin(halfRoll);
        cr = T.Cos(halfRoll);

        var halfPitch = pitch * NumericConstants<T>.Half;
        sp = T.Sin(halfPitch);
        cp = T.Cos(halfPitch);

        var halfYaw = yaw * NumericConstants<T>.Half;
        sy = T.Sin(halfYaw);
        cy = T.Cos(halfYaw);

        return new Quaternion<T>(
            cy * sp * cr + sy * cp * sr,
            sy * cp * cr - cy * sp * sr,
            cy * cp * sr - sy * sp * cr,
            cy * cp * cr + sy * sp * sr
        );
    }

    /// <summary>Divides one quaternion by a second quaternion.</summary>
    /// <param name="value1">The dividend.</param>
    /// <param name="value2">The divisor.</param>
    /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Divide<T>(Quaternion<T> value1, Quaternion<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return value1 / value2;
    }

    /// <summary>Divides the specified quaternion by a specified scalar value.</summary>
    /// <param name="left">The quaternion.</param>
    /// <param name="divisor">The scalar value.</param>
    /// <returns>The quaternion that results from the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Quaternion<T> Divide<T>(Quaternion<T> left, T divisor)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return new Quaternion<T>(
            left.X / divisor,
            left.Y / divisor,
            left.Z / divisor,
            left.W / divisor
        );
    }

    /// <summary>Calculates the dot product of two quaternions.</summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The dot product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Quaternion<T> quaternion1, Quaternion<T> quaternion2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return (quaternion1.X * quaternion2.X)
             + (quaternion1.Y * quaternion2.Y)
             + (quaternion1.Z * quaternion2.Z)
             + (quaternion1.W * quaternion2.W);
    }

    /// <summary>Returns the inverse of a quaternion.</summary>
    /// <param name="value">The quaternion.</param>
    /// <returns>The inverted quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Inverse<T>(Quaternion<T> value)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        //  -1   (       a              -v       )
        // q   = ( -------------   ------------- )
        //       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

        return Divide(Conjugate(value), value.LengthSquared());
    }

    /// <summary>Performs a linear interpolation between two quaternions based on a value that specifies the weighting of the second quaternion.</summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="amount">The relative weight of <paramref name="quaternion2" /> in the interpolation.</param>
    /// <returns>The interpolated quaternion.</returns>
    public static Quaternion<T> Lerp<T>(Quaternion<T> quaternion1, Quaternion<T> quaternion2, T amount)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>, IComparisonOperators<T, T, bool>
    {
        var t = amount;
        var t1 = T.One - t;

        Quaternion<T> r;

        var dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                  quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

        if (dot >= T.Zero)
        {
            r = new Quaternion<T>(
                t1 * quaternion1.X + t * quaternion2.X,
                t1 * quaternion1.Y + t * quaternion2.Y,
                t1 * quaternion1.Z + t * quaternion2.Z,
                t1 * quaternion1.W + t * quaternion2.W
            );
        }
        else
        {
            r = new Quaternion<T>(
                t1 * quaternion1.X - t * quaternion2.X,
                t1 * quaternion1.Y - t * quaternion2.Y,
                t1 * quaternion1.Z - t * quaternion2.Z,
                t1 * quaternion1.W - t * quaternion2.W
            );
        }

        // Normalize it.
        var ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
        T invNorm = T.One / T.Sqrt(ls);

        r *= invNorm;

        return r;
    }

    /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The product quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Multiply<T>(Quaternion<T> value1, Quaternion<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return value1 * value2;
    }

    /// <summary>Returns a new quaternion whose values are the product of each pair of elements in specified quaternion and vector.</summary>
    /// <param name="value1">The quaternion.</param>
    /// <param name="value2">The vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Quaternion<T> Multiply<T>(Quaternion<T> value1, Vector4D<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return new Quaternion<T>(
            value1.X * value2.X,
            value1.Y * value2.Y,
            value1.Z * value2.Z,
            value1.W * value2.W
        );
    }

    /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
    /// <param name="value1">The source quaternion.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The scaled quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Multiply<T>(Quaternion<T> value1, T value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return value1 * value2;
    }

    /// <summary>Reverses the sign of each component of the quaternion.</summary>
    /// <param name="value">The quaternion to negate.</param>
    /// <returns>The negated quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Negate<T>(Quaternion<T> value)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return -value;
    }

    /// <summary>Divides each component of a specified <see cref="Quaternion<T>" /> by its length.</summary>
    /// <param name="value">The quaternion to normalize.</param>
    /// <returns>The normalized quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Normalize<T>(Quaternion<T> value)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return Divide(value, value.Length());
    }

    /// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="amount">The relative weight of the second quaternion in the interpolation.</param>
    /// <returns>The interpolated quaternion.</returns>
    public static Quaternion<T> Slerp<T>(Quaternion<T> quaternion1, Quaternion<T> quaternion2, T amount)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>, IComparisonOperators<T, T, bool>
    {
        var t = amount;

        var cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                       quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

        var flip = false;

        if (cosOmega < T.Zero)
        {
            flip = true;
            cosOmega = -cosOmega;
        }

        T s1, s2;

        if (cosOmega > (T.One - NumericConstants<T>.SlerpEpsilon))
        {
            // Too close, do straight linear interpolation.
            s1 = T.One - t;
            s2 = (flip) ? -t : t;
        }
        else
        {
            T omega = T.Acos(cosOmega);
            T invSinOmega = T.One / T.Sin(omega);

            s1 = T.Sin((T.One - t) * omega) * invSinOmega;
            s2 = (flip)
                ? -T.Sin(t * omega) * invSinOmega
                : T.Sin(t * omega) * invSinOmega;
        }

        return new Quaternion<T>(
            s1 * quaternion1.X + s2 * quaternion2.X,
            s1 * quaternion1.Y + s2 * quaternion2.Y,
            s1 * quaternion1.Z + s2 * quaternion2.Z,
            s1 * quaternion1.W + s2 * quaternion2.W
        );
    }

    /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion<T> Subtract<T>(Quaternion<T> value1, Quaternion<T> value2)
        where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return value1 - value2;
    }
    
    /// <summary>Calculates the length of the quaternion.</summary>
    /// <returns>The computed length of the quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Length<T>(this Quaternion<T> self) where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        var lengthSquared = LengthSquared(self);
        return T.Sqrt(lengthSquared);
    }

    /// <summary>Calculates the squared length of the quaternion.</summary>
    /// <returns>The length squared of the quaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LengthSquared<T>(this Quaternion<T> self) where T : IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        return Dot(self, self);
    }

}