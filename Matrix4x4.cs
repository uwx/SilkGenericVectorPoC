using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace GenericVector;

public static class Matrix4X4
{
    /// <summary>Adds each element in one matrix with its corresponding element in a second matrix.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The matrix that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> Add<T>(Matrix4X4<T> value1, Matrix4X4<T> value2) where T : INumberBase<T> => value1 + value2;

    /// <summary>Creates a spherical billboard that rotates around a specified object position.</summary>
    /// <param name="objectPosition">The position of the object that the billboard will rotate around.</param>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraUpVector">The up vector of the camera.</param>
    /// <param name="cameraForwardVector">The forward vector of the camera.</param>
    /// <returns>The created billboard.</returns>
    public static Matrix4X4<T> CreateBillboard<T>(
        Vector3D<T> objectPosition, Vector3D<T> cameraPosition, Vector3D<T> cameraUpVector, Vector3D<T> cameraForwardVector
    ) where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>
    {
        Vector3D<T> axisZ = objectPosition - cameraPosition;
        T norm = axisZ.LengthSquared();

        if (norm < NumericConstants<T>.BillboardEpsilon)
        {
            axisZ = -cameraForwardVector;
        }
        else
        {
            axisZ = Vector3D.Multiply(axisZ, T.One / T.Sqrt(norm));
        }

        Vector3D<T> axisX = Vector3D.Normalize(Vector3D.Cross(cameraUpVector, axisZ));
        Vector3D<T> axisY = Vector3D.Cross(axisZ, axisX);

        return new Matrix4X4<T>(
            new Vector4D<T>(axisX, T.Zero),
            new Vector4D<T>(axisY, T.Zero),
            new Vector4D<T>(axisZ, T.Zero),
            new Vector4D<T>(objectPosition, T.One)
        );
    }

    /// <summary>Creates a cylindrical billboard that rotates around a specified axis.</summary>
    /// <param name="objectPosition">The position of the object that the billboard will rotate around.</param>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="rotateAxis">The axis to rotate the billboard around.</param>
    /// <param name="cameraForwardVector">The forward vector of the camera.</param>
    /// <param name="objectForwardVector">The forward vector of the object.</param>
    /// <returns>The billboard matrix.</returns>
    public static Matrix4X4<T> CreateConstrainedBillboard<T>(
        Vector3D<T> objectPosition, Vector3D<T> cameraPosition, Vector3D<T> rotateAxis, Vector3D<T> cameraForwardVector, Vector3D<T> objectForwardVector
    ) where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>
    {
        // Treat the case when object and camera positions are too close.
        Vector3D<T> faceDir = objectPosition - cameraPosition;
        T norm = faceDir.LengthSquared();

        if (norm < NumericConstants<T>.BillboardEpsilon)
        {
            faceDir = -cameraForwardVector;
        }
        else
        {
            faceDir = Vector3D.Multiply(faceDir, (T.One / T.Sqrt(norm)));
        }

        Vector3D<T> axisY = rotateAxis;

        // Treat the case when angle between faceDir and rotateAxis is too close to 0.
        T dot = Vector3D.Dot(axisY, faceDir);

        if (T.Abs(dot) > NumericConstants<T>.BillboardMinAngle)
        {
            faceDir = objectForwardVector;

            // Make sure passed values are useful for compute.
            dot = Vector3D.Dot(axisY, faceDir);

            if (T.Abs(dot) > NumericConstants<T>.BillboardMinAngle)
            {
                faceDir = (T.Abs(axisY.Z) > NumericConstants<T>.BillboardMinAngle) ? Vector3D<T>.UnitX : new Vector3D<T>(T.Zero, T.Zero, -T.One);
            }
        }

        Vector3D<T> axisX = Vector3D.Normalize(Vector3D.Cross(axisY, faceDir));
        Vector3D<T> axisZ = Vector3D.Normalize(Vector3D.Cross(axisX, axisY));

        return new Matrix4X4<T>(
            new Vector4D<T>(axisX, T.Zero),
            new Vector4D<T>(axisY, T.Zero),
            new Vector4D<T>(axisZ, T.Zero),
            new Vector4D<T>(objectPosition, T.One)
        );
    }

    /// <summary>Creates a matrix that rotates around an arbitrary vector.</summary>
    /// <param name="axis">The axis to rotate around.</param>
    /// <param name="angle">The angle to rotate around <paramref name="axis" />, in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateFromAxisAngle<T>(Vector3D<T> axis, T angle)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        // a: angle
        // x, y, z: unit vector for axis.
        //
        // Rotation matrix M can compute by using below equation.
        //
        //        T               T
        //  M = uu + (cos a)( I-uu ) + (sin a)S
        //
        // Where:
        //
        //  u = ( x, y, z )
        //
        //      [  0 -z  y ]
        //  S = [  z  0 -x ]
        //      [ -y  x  0 ]
        //
        //      [ 1 0 0 ]
        //  I = [ 0 1 0 ]
        //      [ 0 0 1 ]
        //
        //
        //     [  xx+cosa*(1-xx)   yx-cosa*yx-sina*z zx-cosa*xz+sina*y ]
        // M = [ xy-cosa*yx+sina*z    yy+cosa(1-yy)  yz-cosa*yz-sina*x ]
        //     [ zx-cosa*zx-sina*y zy-cosa*zy+sina*x   zz+cosa*(1-zz)  ]
        //

        T x = axis.X;
        T y = axis.Y;
        T z = axis.Z;

        T sa = T.Sin(angle);
        T ca = T.Cos(angle);

        T xx = x * x;
        T yy = y * y;
        T zz = z * z;

        T xy = x * y;
        T xz = x * z;
        T yz = y * z;

        return new Matrix4X4<T>(
            new Vector4D<T>(
                xx + ca * (T.One - xx),
                xy - ca * xy + sa * z,
                xz - ca * xz - sa * y,
                T.Zero
            ),
            new Vector4D<T>(
                xy - ca * xy - sa * z,
                yy + ca * (T.One - yy),
                yz - ca * yz + sa * x,
                T.Zero
            ),
            new Vector4D<T>(
                xz - ca * xz + sa * y,
                yz - ca * yz - sa * x,
                zz + ca * (T.One - zz),
                T.Zero
            ),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a rotation matrix from the specified Quaternion<T> rotation value.</summary>
    /// <param name="quaternion">The source Quaternion<T>.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateFromQuaternion<T>(Quaternion<T> quaternion)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        T xx = quaternion.X * quaternion.X;
        T yy = quaternion.Y * quaternion.Y;
        T zz = quaternion.Z * quaternion.Z;

        T xy = quaternion.X * quaternion.Y;
        T wz = quaternion.Z * quaternion.W;
        T xz = quaternion.Z * quaternion.X;
        T wy = quaternion.Y * quaternion.W;
        T yz = quaternion.Y * quaternion.Z;
        T wx = quaternion.X * quaternion.W;

        return new Matrix4X4<T>(
            new Vector4D<T>(
                T.One - NumericConstants<T>.Two * (yy + zz),
                NumericConstants<T>.Two * (xy + wz),
                NumericConstants<T>.Two * (xz - wy),
                T.Zero
            ),
            new Vector4D<T>(
                NumericConstants<T>.Two * (xy - wz),
                T.One - NumericConstants<T>.Two * (zz + xx),
                NumericConstants<T>.Two * (yz + wx),
                T.Zero
            ),
            new Vector4D<T>(
                NumericConstants<T>.Two * (xz + wy),
                NumericConstants<T>.Two * (yz - wx),
                T.One - NumericConstants<T>.Two * (yy + xx),
                T.Zero
            ),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a rotation matrix from the specified yaw, pitch, and roll.</summary>
    /// <param name="yaw">The angle of rotation, in radians, around the Y axis.</param>
    /// <param name="pitch">The angle of rotation, in radians, around the X axis.</param>
    /// <param name="roll">The angle of rotation, in radians, around the Z axis.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateFromYawPitchRoll<T>(T yaw, T pitch, T roll)
        where T : ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        var q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        return CreateFromQuaternion(q);
    }

    /// <summary>Creates a right-handed view matrix.</summary>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraTarget">The target towards which the camera is pointing.</param>
    /// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
    /// <returns>The right-handed view matrix.</returns>
    public static Matrix4X4<T> CreateLookAt<T>(Vector3D<T> cameraPosition, Vector3D<T> cameraTarget, Vector3D<T> cameraUpVector)
        where T : INumberBase<T>, IRootFunctions<T>
    {
        Vector3D<T> cameraDirection = cameraTarget - cameraPosition;
        return CreateLookTo(cameraPosition, cameraDirection, cameraUpVector);
    }

    /// <summary>Creates a left-handed view matrix.</summary>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraTarget">The target towards which the camera is pointing.</param>
    /// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
    /// <returns>The left-handed view matrix.</returns>
    public static Matrix4X4<T> CreateLookAtLeftHanded<T>(Vector3D<T> cameraPosition, Vector3D<T> cameraTarget, Vector3D<T> cameraUpVector)
        where T : INumberBase<T>, IRootFunctions<T>
    {
        Vector3D<T> cameraDirection = cameraTarget - cameraPosition;
        return CreateLookToLeftHanded(cameraPosition, cameraDirection, cameraUpVector);
    }

    /// <summary>Creates a right-handed view matrix.</summary>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraDirection">The direction in which the camera is pointing.</param>
    /// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
    /// <returns>The right-handed view matrix.</returns>
    public static Matrix4X4<T> CreateLookTo<T>(Vector3D<T> cameraPosition, Vector3D<T> cameraDirection, Vector3D<T> cameraUpVector)
        where T : INumberBase<T>, IRootFunctions<T>
    {
        Vector3D<T> axisZ = Vector3D.Normalize(-cameraDirection);
        Vector3D<T> axisX = Vector3D.Normalize(Vector3D.Cross(cameraUpVector, axisZ));
        Vector3D<T> axisY = Vector3D.Cross(axisZ, axisX);
        Vector3D<T> negativeCameraPosition = -cameraPosition;

        return new Matrix4X4<T>(
            new Vector4D<T>(
                axisX.X,
                axisY.X,
                axisZ.X,
                T.Zero
            ),
            new Vector4D<T>(
                axisX.Y,
                axisY.Y,
                axisZ.Y,
                T.Zero
            ),
            new Vector4D<T>(
                axisX.Z,
                axisY.Z,
                axisZ.Z,
                T.Zero
            ),
            new Vector4D<T>(
                Vector3D.Dot(axisX, negativeCameraPosition),
                Vector3D.Dot(axisY, negativeCameraPosition),
                Vector3D.Dot(axisZ, negativeCameraPosition),
                T.One
            )
        );
    }

    /// <summary>Creates a left-handed view matrix.</summary>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraDirection">The direction in which the camera is pointing.</param>
    /// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
    /// <returns>The left-handed view matrix.</returns>
    public static Matrix4X4<T> CreateLookToLeftHanded<T>(Vector3D<T> cameraPosition, Vector3D<T> cameraDirection, Vector3D<T> cameraUpVector)
        where T : INumberBase<T>, IRootFunctions<T>
    {
        Vector3D<T> axisZ = Vector3D.Normalize(cameraDirection);
        Vector3D<T> axisX = Vector3D.Normalize(Vector3D.Cross(cameraUpVector, axisZ));
        Vector3D<T> axisY = Vector3D.Cross(axisZ, axisX);
        Vector3D<T> negativeCameraPosition = -cameraPosition;

        return new Matrix4X4<T>(
            new Vector4D<T>(
                axisX.X,
                axisY.X,
                axisZ.X,
                T.Zero
            ),
            new Vector4D<T>(
                axisX.Y,
                axisY.Y,
                axisZ.Y,
                T.Zero
            ),
            new Vector4D<T>(
                axisX.Z,
                axisY.Z,
                axisZ.Z,
                T.Zero
            ),
            new Vector4D<T>(
                Vector3D.Dot(axisX, negativeCameraPosition),
                Vector3D.Dot(axisY, negativeCameraPosition),
                Vector3D.Dot(axisZ, negativeCameraPosition),
                T.One
            )
        );
    }

    /// <summary>Creates a right-handed orthographic perspective matrix from the given view volume dimensions.</summary>
    /// <param name="width">The width of the view volume.</param>
    /// <param name="height">The height of the view volume.</param>
    /// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
    /// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
    /// <returns>The right-handed orthographic projection matrix.</returns>
    public static Matrix4X4<T> CreateOrthographic<T>(T width, T height, T zNearPlane, T zFarPlane)
        where T : INumberBase<T>
    {
        T range = T.One / (zNearPlane - zFarPlane);

        return new Matrix4X4<T>(
            new Vector4D<T>(NumericConstants<T>.Two / width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, NumericConstants<T>.Two / height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range * zNearPlane, T.One)
        );
    }

    /// <summary>Creates a left-handed orthographic perspective matrix from the given view volume dimensions.</summary>
    /// <param name="width">The width of the view volume.</param>
    /// <param name="height">The height of the view volume.</param>
    /// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
    /// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
    /// <returns>The left-handed orthographic projection matrix.</returns>
    public static Matrix4X4<T> CreateOrthographicLeftHanded<T>(T width, T height, T zNearPlane, T zFarPlane)
        where T : INumberBase<T>
    {
        T range = T.One / (zFarPlane - zNearPlane);

        return new Matrix4X4<T>(
            new Vector4D<T>(NumericConstants<T>.Two / width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, NumericConstants<T>.Two / height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, -range * zNearPlane, T.One)
        );
    }

    /// <summary>Creates a right-handed customized orthographic projection matrix.</summary>
    /// <param name="left">The minimum X-value of the view volume.</param>
    /// <param name="right">The maximum X-value of the view volume.</param>
    /// <param name="bottom">The minimum Y-value of the view volume.</param>
    /// <param name="top">The maximum Y-value of the view volume.</param>
    /// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
    /// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
    /// <returns>The right-handed orthographic projection matrix.</returns>
    public static Matrix4X4<T> CreateOrthographicOffCenter<T>(T left, T right, T bottom, T top, T zNearPlane, T zFarPlane)
        where T : INumberBase<T>
    {
        T reciprocalWidth = T.One / (right - left);
        T reciprocalHeight = T.One / (top - bottom);
        T range = T.One / (zNearPlane - zFarPlane);

        return new Matrix4X4<T>(
            new Vector4D<T>(reciprocalWidth + reciprocalWidth, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, reciprocalHeight + reciprocalHeight, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.Zero),
            new Vector4D<T>(
                -(left + right) * reciprocalWidth,
                -(top + bottom) * reciprocalHeight,
                range * zNearPlane,
                T.One
            )
        );
    }

    /// <summary>Creates a left-handed customized orthographic projection matrix.</summary>
    /// <param name="left">The minimum X-value of the view volume.</param>
    /// <param name="right">The maximum X-value of the view volume.</param>
    /// <param name="bottom">The minimum Y-value of the view volume.</param>
    /// <param name="top">The maximum Y-value of the view volume.</param>
    /// <param name="zNearPlane">The minimum Z-value of the view volume.</param>
    /// <param name="zFarPlane">The maximum Z-value of the view volume.</param>
    /// <returns>The left-handed orthographic projection matrix.</returns>
    public static Matrix4X4<T> CreateOrthographicOffCenterLeftHanded<T>(
        T left, T right, T bottom, T top, T zNearPlane, T zFarPlane
    )
        where T : INumberBase<T>
    {
        T reciprocalWidth = T.One / (right - left);
        T reciprocalHeight = T.One / (top - bottom);
        T range = T.One / (zFarPlane - zNearPlane);

        return new Matrix4X4<T>(
            new Vector4D<T>(reciprocalWidth + reciprocalWidth, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, reciprocalHeight + reciprocalHeight, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.Zero),
            new Vector4D<T>(
                -(left + right) * reciprocalWidth,
                -(top + bottom) * reciprocalHeight,
                -range * zNearPlane,
                T.One
            )
        );
    }

    /// <summary>Creates a right-handed perspective projection matrix from the given view volume dimensions.</summary>
    /// <param name="width">The width of the view volume at the near view plane.</param>
    /// <param name="height">The height of the view volume at the near view plane.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The right-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspective<T>(T width, T height, T nearPlaneDistance, T farPlaneDistance)
        where T : INumberBase<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T dblNearPlaneDistance = nearPlaneDistance + nearPlaneDistance;
        T range = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(dblNearPlaneDistance / width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, dblNearPlaneDistance / height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, -T.One),
            new Vector4D<T>(T.Zero, T.Zero, range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a left-handed perspective projection matrix from the given view volume dimensions.</summary>
    /// <param name="width">The width of the view volume at the near view plane.</param>
    /// <param name="height">The height of the view volume at the near view plane.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The left-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspectiveLeftHanded<T>(T width, T height, T nearPlaneDistance, T farPlaneDistance)
        where T : INumberBase<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T dblNearPlaneDistance = nearPlaneDistance + nearPlaneDistance;
        T range = T.IsPositiveInfinity(farPlaneDistance) ? T.One : farPlaneDistance / (farPlaneDistance - nearPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(dblNearPlaneDistance / width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, dblNearPlaneDistance / height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.One),
            new Vector4D<T>(T.Zero, T.Zero, -range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a right-handed perspective projection matrix based on a field of view, aspect ratio, and near and far view plane distances.</summary>
    /// <param name="fieldOfView">The field of view in the y direction, in radians.</param>
    /// <param name="aspectRatio">The aspect ratio, defined as view space width divided by height.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The right-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fieldOfView" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="fieldOfView" /> is greater than or equal to <see cref="Math.PI" />.
    /// <paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspectiveFieldOfView<T>(
        T fieldOfView, T aspectRatio, T nearPlaneDistance, T farPlaneDistance
    )
        where T : INumberBase<T>, ITrigonometricFunctions<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, T.Pi);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T height = T.One / T.Tan(fieldOfView * NumericConstants<T>.Half);
        T width = height / aspectRatio;
        T range = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, -T.One),
            new Vector4D<T>(T.Zero, T.Zero, range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a left-handed perspective projection matrix based on a field of view, aspect ratio, and near and far view plane distances.</summary>
    /// <param name="fieldOfView">The field of view in the y direction, in radians.</param>
    /// <param name="aspectRatio">The aspect ratio, defined as view space width divided by height.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The left-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fieldOfView" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="fieldOfView" /> is greater than or equal to <see cref="Math.PI" />.
    /// <paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspectiveFieldOfViewLeftHanded<T>(
        T fieldOfView, T aspectRatio, T nearPlaneDistance, T farPlaneDistance
    )
        where T : INumberBase<T>, ITrigonometricFunctions<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, T.Pi);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T height = T.One / T.Tan(fieldOfView * NumericConstants<T>.Half);
        T width = height / aspectRatio;
        T range = T.IsPositiveInfinity(farPlaneDistance) ? T.One : farPlaneDistance / (farPlaneDistance - nearPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(width, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, height, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, range, T.One),
            new Vector4D<T>(T.Zero, T.Zero, -range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a right-handed customized perspective projection matrix.</summary>
    /// <param name="left">The minimum x-value of the view volume at the near view plane.</param>
    /// <param name="right">The maximum x-value of the view volume at the near view plane.</param>
    /// <param name="bottom">The minimum y-value of the view volume at the near view plane.</param>
    /// <param name="top">The maximum y-value of the view volume at the near view plane.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The right-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspectiveOffCenter<T>(
        T left, T right, T bottom, T top, T nearPlaneDistance, T farPlaneDistance
    )
        where T : INumberBase<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T dblNearPlaneDistance = nearPlaneDistance + nearPlaneDistance;
        T reciprocalWidth = T.One / (right - left);
        T reciprocalHeight = T.One / (top - bottom);
        T range = T.IsPositiveInfinity(farPlaneDistance) ? -T.One : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(dblNearPlaneDistance * reciprocalWidth, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, dblNearPlaneDistance * reciprocalHeight, T.Zero, T.Zero),
            new Vector4D<T>(
                (left + right) * reciprocalWidth,
                (top + bottom) * reciprocalHeight,
                range,
                -T.One
            ),
            new Vector4D<T>(T.Zero, T.Zero, range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a left-handed customized perspective projection matrix.</summary>
    /// <param name="left">The minimum x-value of the view volume at the near view plane.</param>
    /// <param name="right">The maximum x-value of the view volume at the near view plane.</param>
    /// <param name="bottom">The minimum y-value of the view volume at the near view plane.</param>
    /// <param name="top">The maximum y-value of the view volume at the near view plane.</param>
    /// <param name="nearPlaneDistance">The distance to the near view plane.</param>
    /// <param name="farPlaneDistance">The distance to the far view plane.</param>
    /// <returns>The left-handed perspective projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="nearPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="farPlaneDistance" /> is less than or equal to zero.
    /// -or-
    /// <paramref name="nearPlaneDistance" /> is greater than or equal to <paramref name="farPlaneDistance" />.</exception>
    public static Matrix4X4<T> CreatePerspectiveOffCenterLeftHanded<T>(
        T left, T right, T bottom, T top, T nearPlaneDistance, T farPlaneDistance
    )
        where T : INumberBase<T>, IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, T.Zero);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance);

        T dblNearPlaneDistance = nearPlaneDistance + nearPlaneDistance;
        T reciprocalWidth = T.One / (right - left);
        T reciprocalHeight = T.One / (top - bottom);
        T range = T.IsPositiveInfinity(farPlaneDistance) ? T.One : farPlaneDistance / (farPlaneDistance - nearPlaneDistance);

        return new Matrix4X4<T>(
            new Vector4D<T>(dblNearPlaneDistance * reciprocalWidth, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, dblNearPlaneDistance * reciprocalHeight, T.Zero, T.Zero),
            new Vector4D<T>(
                -(left + right) * reciprocalWidth,
                -(top + bottom) * reciprocalHeight,
                range,
                T.One
            ),
            new Vector4D<T>(T.Zero, T.Zero, -range * nearPlaneDistance, T.Zero)
        );
    }

    /// <summary>Creates a matrix that reflects the coordinate system about a specified plane.</summary>
    /// <param name="value">The plane about which to create a reflection.</param>
    /// <returns>A new matrix expressing the reflection.</returns>
    public static Matrix4X4<T> CreateReflection<T>(Plane<T> value)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>
    {
        Plane<T> p = Plane.Normalize(value);
        Vector3D<T> f = p.Normal * -NumericConstants<T>.Two;

        return new Matrix4X4<T>(
            new Vector4D<T>(f * p.Normal.X, T.Zero) + Vector4D<T>.UnitX,
            new Vector4D<T>(f * p.Normal.Y, T.Zero) + Vector4D<T>.UnitY,
            new Vector4D<T>(f * p.Normal.Z, T.Zero) + Vector4D<T>.UnitZ,
            new Vector4D<T>(f * p.D, T.One)
        );
    }

    /// <summary>Creates a matrix for rotating points around the X axis.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the X axis.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationX<T>(T radians)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        T c = T.Cos(radians);
        T s = T.Sin(radians);

        // [  1  0  0  0 ]
        // [  0  c  s  0 ]
        // [  0 -s  c  0 ]
        // [  0  0  0  1 ]

        return new Matrix4X4<T>(
            Vector4D<T>.UnitX,
            new Vector4D<T>(T.Zero, c, s, T.Zero),
            new Vector4D<T>(T.Zero, -s, c, T.Zero),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a matrix for rotating points around the X axis from a center point.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the X axis.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationX<T>(T radians, Vector3D<T> centerPoint)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        T c = T.Cos(radians);
        T s = T.Sin(radians);

        T y = centerPoint.Y * (T.One - c) + centerPoint.Z * s;
        T z = centerPoint.Z * (T.One - c) - centerPoint.Y * s;

        // [  1  0  0  0 ]
        // [  0  c  s  0 ]
        // [  0 -s  c  0 ]
        // [  0  y  z  1 ]

        return new Matrix4X4<T>(
            Vector4D<T>.UnitX,
            new Vector4D<T>(T.Zero, c, s, T.Zero),
            new Vector4D<T>(T.Zero, -s, c, T.Zero),
            new Vector4D<T>(T.Zero, y, z, T.One)
        );
    }

    /// <summary>Creates a matrix for rotating points around the Y axis.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the Y-axis.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationY<T>(T radians)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        T c = T.Cos(radians);
        T s = T.Sin(radians);

        // [  c  0 -s  0 ]
        // [  0  1  0  0 ]
        // [  s  0  c  0 ]
        // [  0  0  0  1 ]

        return new Matrix4X4<T>(
            new Vector4D<T>(c, T.Zero, -s, T.Zero),
            Vector4D<T>.UnitY,
            new Vector4D<T>(s, T.Zero, c, T.Zero),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>The amount, in radians, by which to rotate around the Y axis from a center point.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the Y-axis.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationY<T>(T radians, Vector3D<T> centerPoint)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        T c = T.Cos(radians);
        T s = T.Sin(radians);

        T x = centerPoint.X * (T.One - c) - centerPoint.Z * s;
        T z = centerPoint.Z * (T.One - c) + centerPoint.X * s;

        // [  c  0 -s  0 ]
        // [  0  1  0  0 ]
        // [  s  0  c  0 ]
        // [  x  0  z  1 ]

        return new Matrix4X4<T>(
            new Vector4D<T>(c, T.Zero, -s, T.Zero),
            Vector4D<T>.UnitY,
            new Vector4D<T>(s, T.Zero, c, T.Zero),
            new Vector4D<T>(x, T.Zero, z, T.One)
        );
    }

    /// <summary>Creates a matrix for rotating points around the Z axis.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the Z-axis.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationZ<T>(T radians)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {

        T c = T.Cos(radians);
        T s = T.Sin(radians);

        // [  c  s  0  0 ]
        // [ -s  c  0  0 ]
        // [  0  0  1  0 ]
        // [  0  0  0  1 ]

        return new Matrix4X4<T>(
            new Vector4D<T>(c, s, T.Zero, T.Zero),
            new Vector4D<T>(-s, c, T.Zero, T.Zero),
            Vector4D<T>.UnitZ,
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a matrix for rotating points around the Z axis from a center point.</summary>
    /// <param name="radians">The amount, in radians, by which to rotate around the Z-axis.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4X4<T> CreateRotationZ<T>(T radians, Vector3D<T> centerPoint)
        where T : INumberBase<T>, ITrigonometricFunctions<T>
    {
        T c = T.Cos(radians);
        T s = T.Sin(radians);

        T x = centerPoint.X * (T.One - c) + centerPoint.Y * s;
        T y = centerPoint.Y * (T.One - c) - centerPoint.X * s;

        // [  c  s  0  0 ]
        // [ -s  c  0  0 ]
        // [  0  0  1  0 ]
        // [  x  y  0  1 ]

        return new Matrix4X4<T>(
            new Vector4D<T>(c, s, T.Zero, T.Zero),
            new Vector4D<T>(-s, c, T.Zero, T.Zero),
            Vector4D<T>.UnitZ,
            new Vector4D<T>(x, y, T.Zero, T.One)
        );
    }

    /// <summary>Creates a scaling matrix from the specified X, Y, and Z components.</summary>
    /// <param name="scaleX">The value to scale by on the X axis.</param>
    /// <param name="scaleY">The value to scale by on the Y axis.</param>
    /// <param name="scaleZ">The value to scale by on the Z axis.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(T scaleX, T scaleY, T scaleZ)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scaleX, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scaleY, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scaleZ, T.Zero),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a scaling matrix that is offset by a given center point.</summary>
    /// <param name="scaleX">The value to scale by on the X axis.</param>
    /// <param name="scaleY">The value to scale by on the Y axis.</param>
    /// <param name="scaleZ">The value to scale by on the Z axis.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(T scaleX, T scaleY, T scaleZ, Vector3D<T> centerPoint)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scaleX, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scaleY, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scaleZ, T.Zero),
            new Vector4D<T>(centerPoint * (Vector3D<T>.One - new Vector3D<T>(scaleX, scaleY, scaleZ)), T.One)
        );
    }

    /// <summary>Creates a scaling matrix from the specified vector scale.</summary>
    /// <param name="scales">The scale to use.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(Vector3D<T> scales)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scales.X, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scales.Y, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scales.Z, T.Zero),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a scaling matrix with a center point.</summary>
    /// <param name="scales">The vector that contains the amount to scale on each axis.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(Vector3D<T> scales, Vector3D<T> centerPoint)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scales.X, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scales.Y, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scales.Z, T.Zero),
            new Vector4D<T>(centerPoint * (Vector3D<T>.One - scales), T.One)
        );
    }
    
    /// <summary>Creates a uniform scaling matrix that scale equally on each axis.</summary>
    /// <param name="scale">The uniform scaling factor.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(T scale)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scale, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scale, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scale, T.Zero),
            Vector4D<T>.UnitW
        );
    }

    /// <summary>Creates a uniform scaling matrix that scales equally on each axis with a center point.</summary>
    /// <param name="scale">The uniform scaling factor.</param>
    /// <param name="centerPoint">The center point.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4X4<T> CreateScale<T>(T scale, Vector3D<T> centerPoint)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            new Vector4D<T>(scale, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, scale, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, scale, T.Zero),
            new Vector4D<T>(centerPoint * (Vector3D<T>.One - new Vector3D<T>(scale)), T.One)
        );
    }

    /// <summary>Creates a matrix that flattens geometry into a specified plane as if casting a shadow from a specified light source.</summary>
    /// <param name="lightDirection">The direction from which the light that will cast the shadow is coming.</param>
    /// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
    /// <returns>A new matrix that can be used to flatten geometry onto the specified plane from the specified direction.</returns>
    public static Matrix4X4<T> CreateShadow<T>(Vector3D<T> lightDirection, Plane<T> plane)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IRootFunctions<T>
    {
        Plane<T> p = Plane.Normalize(plane);
        T dot = Vector3D.Dot(lightDirection, p.Normal);

        Vector3D<T> normal = -p.Normal;

        return new Matrix4X4<T>(
            new Vector4D<T>(lightDirection * normal.X, T.Zero) + new Vector4D<T>(dot, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(lightDirection * normal.Y, T.Zero) + new Vector4D<T>(T.Zero, dot, T.Zero, T.Zero),
            new Vector4D<T>(lightDirection * normal.Z, T.Zero) + new Vector4D<T>(T.Zero, T.Zero, dot, T.Zero),
            new Vector4D<T>(lightDirection * -p.D, dot)
        );
    }

    /// <summary>Creates a translation matrix from the specified 3-dimensional vector.</summary>
    /// <param name="position">The amount to translate in each axis.</param>
    /// <returns>The translation matrix.</returns>
    public static Matrix4X4<T> CreateTranslation<T>(Vector3D<T> position)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            Vector4D<T>.UnitX,
            Vector4D<T>.UnitY,
            Vector4D<T>.UnitZ,
            new Vector4D<T>(position, T.One)
        );
    }

    /// <summary>Creates a translation matrix from the specified X, Y, and Z components.</summary>
    /// <param name="positionX">The amount to translate on the X axis.</param>
    /// <param name="positionY">The amount to translate on the Y axis.</param>
    /// <param name="positionZ">The amount to translate on the Z axis.</param>
    /// <returns>The translation matrix.</returns>
    public static Matrix4X4<T> CreateTranslation<T>(T positionX, T positionY, T positionZ)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            Vector4D<T>.UnitX,
            Vector4D<T>.UnitY,
            Vector4D<T>.UnitZ,
            new Vector4D<T>(positionX, positionY, positionZ, T.One)
        );
    }

    /// <summary>Creates a right-handed viewport matrix from the specified parameters.</summary>
    /// <param name="x">X coordinate of the viewport upper left corner.</param>
    /// <param name="y">Y coordinate of the viewport upper left corner.</param>
    /// <param name="width">Viewport width.</param>
    /// <param name="height">Viewport height.</param>
    /// <param name="minDepth">Viewport minimum depth.</param>
    /// <param name="maxDepth">Viewport maximum depth.</param>
    /// <returns>The right-handed viewport matrix.</returns>
    /// <remarks>
    /// Viewport matrix
    /// |   width / 2   |        0       |          0          | 0 |
    /// |       0       |   -height / 2  |          0          | 0 |
    /// |       0       |        0       | minDepth - maxDepth | 0 |
    /// | x + width / 2 | y + height / 2 |       minDepth      | 1 |
    /// </remarks>
    public static Matrix4X4<T> CreateViewport<T>(T x, T y, T width, T height, T minDepth, T maxDepth)
        where T : INumberBase<T>
    {
        // 4x SIMD fields to get a lot better codegen
        var a = new Vector4D<T>(width, height, T.Zero, T.Zero);
        a *= new Vector4D<T>(NumericConstants<T>.Half, NumericConstants<T>.Half, T.Zero, T.Zero);

        return new Matrix4X4<T>(
            new Vector4D<T>(a.X, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, -a.Y, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, minDepth - maxDepth, T.Zero),
            a + new Vector4D<T>(x, y, minDepth, T.One)
        );
    }

    /// <summary>Creates a left-handed viewport matrix from the specified parameters.</summary>
    /// <param name="x">X coordinate of the viewport upper left corner.</param>
    /// <param name="y">Y coordinate of the viewport upper left corner.</param>
    /// <param name="width">Viewport width.</param>
    /// <param name="height">Viewport height.</param>
    /// <param name="minDepth">Viewport minimum depth.</param>
    /// <param name="maxDepth">Viewport maximum depth.</param>
    /// <returns>The left-handed viewport matrix.</returns>
    /// <remarks>
    /// Viewport matrix
    /// |   width / 2   |        0       |          0          | 0 |
    /// |       0       |   -height / 2  |          0          | 0 |
    /// |       0       |        0       | maxDepth - minDepth | 0 |
    /// | x + width / 2 | y + height / 2 |       minDepth      | 1 |
    /// </remarks>
    public static Matrix4X4<T> CreateViewportLeftHanded<T>(T x, T y, T width, T height, T minDepth, T maxDepth)
        where T : INumberBase<T>
    {
        // 4x SIMD fields to get a lot better codegen
        var a = new Vector4D<T>(width, height, T.Zero, T.Zero);
        a *= new Vector4D<T>(NumericConstants<T>.Half, NumericConstants<T>.Half, T.Zero, T.Zero);

        return new Matrix4X4<T>(
            new Vector4D<T>(a.X, T.Zero, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, -a.Y, T.Zero, T.Zero),
            new Vector4D<T>(T.Zero, T.Zero, maxDepth - minDepth, T.Zero),
            a + new Vector4D<T>(x, y, minDepth, T.One)
        );
        // Impl result;
        //
        // // 4x SIMD fields to get a lot better codegen
        // result.W = new Vector4D<T>(width, height, T.Zero, T.Zero);
        // result.W *= new Vector4D<T>(NumericConstants<T>.Half, NumericConstants<T>.Half, T.Zero, T.Zero);
        //
        // result.X = new Vector4D<T>(result.W.X, T.Zero, T.Zero, T.Zero);
        // result.Y = new Vector4D<T>(T.Zero, -result.W.Y, T.Zero, T.Zero);
        // result.Z = new Vector4D<T>(T.Zero, T.Zero, maxDepth - minDepth, T.Zero);
        // result.W += new Vector4D<T>(x, y, minDepth, T.One);
        //
        // return result;
    }

    /// <summary>Creates a world matrix with the specified parameters.</summary>
    /// <param name="position">The position of the object.</param>
    /// <param name="forward">The forward direction of the object.</param>
    /// <param name="up">The upward direction of the object. Its value is usually <c>[0, 1, 0]</c>.</param>
    /// <returns>The world matrix.</returns>
    /// <remarks><paramref name="position" /> is used in translation operations.</remarks>
    public static Matrix4X4<T> CreateWorld<T>(Vector3D<T> position, Vector3D<T> forward, Vector3D<T> up)
        where T : IRootFunctions<T>
    {
        Vector3D<T> axisZ = Vector3D.Normalize(-forward);
        Vector3D<T> axisX = Vector3D.Normalize(Vector3D.Cross(up, axisZ));
        Vector3D<T> axisY = Vector3D.Cross(axisZ, axisX);

        return new Matrix4X4<T>(
            new Vector4D<T>(axisX, T.Zero),
            new Vector4D<T>(axisY, T.Zero),
            new Vector4D<T>(axisZ, T.Zero),
            new Vector4D<T>(position, T.One)
        );
    }

    /// <summary>Attempts to extract the scale, translation, and rotation components from the given scale, rotation, or translation matrix. The return value indicates whether the operation succeeded.</summary>
    /// <param name="matrix">The source matrix.</param>
    /// <param name="scale">When this method returns, contains the scaling component of the transformation matrix if the operation succeeded.</param>
    /// <param name="rotation">When this method returns, contains the rotation component of the transformation matrix if the operation succeeded.</param>
    /// <param name="translation">When the method returns, contains the translation component of the transformation matrix if the operation succeeded.</param>
    /// <returns><see langword="true" /> if <paramref name="matrix" /> was decomposed successfully; otherwise,  <see langword="false" />.</returns>
    public static unsafe bool Decompose<T>(Matrix4X4<T> matrix, out Vector3D<T> scale, out Quaternion<T> rotation, out Vector3D<T> translation)
        where T : IComparisonOperators<T, T, bool>, IRootFunctions<T>, ITrigonometricFunctions<T>
    {
        bool result = true;

        // TODO make this not mutate
        fixed (Vector3D<T>* scaleBase = &scale)
        {
            T* pfScales = (T*)scaleBase;
            T det;

            VectorBasis<T> vectorBasis;
            Vector3D<T>** pVectorBasis = (Vector3D<T>**)&vectorBasis;

            var matTemp = Matrix4X4<T>.Identity;
            CanonicalBasis<T> canonicalBasis = default;
            Vector3D<T>* pCanonicalBasis = &canonicalBasis.Row0;

            canonicalBasis.Row0 = new Vector3D<T>(T.One, T.Zero, T.Zero);
            canonicalBasis.Row1 = new Vector3D<T>(T.Zero, T.One, T.Zero);
            canonicalBasis.Row2 = new Vector3D<T>(T.Zero, T.Zero, T.One);

            translation = new Vector3D<T>(
                matrix.W.X,
                matrix.W.Y,
                matrix.W.Z);

            pVectorBasis[0] = (Vector3D<T>*)&matTemp.X;
            pVectorBasis[1] = (Vector3D<T>*)&matTemp.Y;
            pVectorBasis[2] = (Vector3D<T>*)&matTemp.Z;

            *(pVectorBasis[0]) = new Vector3D<T>(matrix.X.X, matrix.X.Y, matrix.X.Z);
            *(pVectorBasis[1]) = new Vector3D<T>(matrix.Y.X, matrix.Y.Y, matrix.Y.Z);
            *(pVectorBasis[2]) = new Vector3D<T>(matrix.Z.X, matrix.Z.Y, matrix.Z.Z);

            scale = new Vector3D<T>(
                pVectorBasis[0]->Length(),
                pVectorBasis[1]->Length(),
                pVectorBasis[2]->Length()
            );

            uint a, b, c;

            #region Ranking
            T x = pfScales[0];
            T y = pfScales[1];
            T z = pfScales[2];

            if (x < y)
            {
                if (y < z)
                {
                    a = 2;
                    b = 1;
                    c = 0;
                }
                else
                {
                    a = 1;

                    if (x < z)
                    {
                        b = 2;
                        c = 0;
                    }
                    else
                    {
                        b = 0;
                        c = 2;
                    }
                }
            }
            else
            {
                if (x < z)
                {
                    a = 2;
                    b = 0;
                    c = 1;
                }
                else
                {
                    a = 0;

                    if (y < z)
                    {
                        b = 2;
                        c = 1;
                    }
                    else
                    {
                        b = 1;
                        c = 2;
                    }
                }
            }
            #endregion

            if (pfScales[a] < NumericConstants<T>.DecomposeEpsilon)
            {
                *(pVectorBasis[a]) = pCanonicalBasis[a];
            }

            *pVectorBasis[a] = Vector3D.Normalize(*pVectorBasis[a]);

            if (pfScales[b] < NumericConstants<T>.DecomposeEpsilon)
            {
                uint cc;
                T fAbsX, fAbsY, fAbsZ;

                fAbsX = T.Abs(pVectorBasis[a]->X);
                fAbsY = T.Abs(pVectorBasis[a]->Y);
                fAbsZ = T.Abs(pVectorBasis[a]->Z);

                #region Ranking
                if (fAbsX < fAbsY)
                {
                    if (fAbsY < fAbsZ)
                    {
                        cc = 0;
                    }
                    else
                    {
                        if (fAbsX < fAbsZ)
                        {
                            cc = 0;
                        }
                        else
                        {
                            cc = 2;
                        }
                    }
                }
                else
                {
                    if (fAbsX < fAbsZ)
                    {
                        cc = 1;
                    }
                    else
                    {
                        if (fAbsY < fAbsZ)
                        {
                            cc = 1;
                        }
                        else
                        {
                            cc = 2;
                        }
                    }
                }
                #endregion

                *pVectorBasis[b] = Vector3D.Cross(*pVectorBasis[a], *(pCanonicalBasis + cc));
            }

            *pVectorBasis[b] = Vector3D.Normalize(*pVectorBasis[b]);

            if (pfScales[c] < NumericConstants<T>.DecomposeEpsilon)
            {
                *pVectorBasis[c] = Vector3D.Cross(*pVectorBasis[a], *pVectorBasis[b]);
            }

            *pVectorBasis[c] = Vector3D.Normalize(*pVectorBasis[c]);

            det = matTemp.GetDeterminant();

            // use Kramer's rule to check for handedness of coordinate system
            if (det < T.Zero)
            {
                // switch coordinate system by negating the scale and inverting the basis vector on the x-axis
                pfScales[a] = -pfScales[a];
                *pVectorBasis[a] = -(*pVectorBasis[a]);

                det = -det;
            }

            det -= T.One;
            det *= det;

            if ((NumericConstants<T>.DecomposeEpsilon < det))
            {
                // Non-SRT matrix encountered
                rotation = Quaternion<T>.Identity;
                result = false;
            }
            else
            {
                // generate the quaternion from the matrix
                rotation = Quaternion.CreateFromRotationMatrix(matTemp);
            }
        }

        return result;
    }

    /// <summary>Tries to invert the specified matrix. The return value indicates whether the operation succeeded.</summary>
    /// <param name="matrix">The matrix to invert.</param>
    /// <param name="result">When this method returns, contains the inverted matrix if the operation succeeded.</param>
    /// <returns><see langword="true" /> if <paramref name="matrix" /> was converted successfully; otherwise,  <see langword="false" />.</returns>
    public static bool Invert<T>(Matrix4X4<T> matrix, out Matrix4X4<T> result)
        where T : INumberBase<T>, IFloatingPointIeee754<T>
    {
        // This implementation is based on the DirectX Math Library XMMatrixInverse method
        // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathMatrix.inl

        if (Sse2.IsSupported && typeof(T) == typeof(double))
        {
            var r = Sse2Impl(Unsafe.BitCast<Matrix4X4<T>, Matrix4X4<double>>(matrix), out var result1);

            result = Unsafe.BitCast<Matrix4X4<double>, Matrix4X4<T>>(result1);
            return r;
        }
        
        if (Sse.IsSupported && typeof(T) == typeof(float))
        {
            var r = SseImpl(Unsafe.BitCast<Matrix4X4<T>, Matrix4X4<float>>(matrix), out var result1);

            result = Unsafe.BitCast<Matrix4X4<float>, Matrix4X4<T>>(result1);
            return r;
        }

        return SoftwareFallback(in matrix, out result);
        
        static bool Sse2Impl(in Matrix4X4<double> matrix, out Matrix4X4<double> result)
        {
            if (!Sse2.IsSupported)
            {
                // Redundant test so we won't prejit remainder of this method on platforms without SSE.
                throw new PlatformNotSupportedException();
            }

            // Load the matrix values into rows
            var row1 = matrix.X.AsVector128();
            var row2 = matrix.Y.AsVector128();
            var row3 = matrix.Z.AsVector128();
            var row4 = matrix.W.AsVector128();

            // Transpose the matrix
            var vTemp1 = Sse2.Shuffle(row1, row2, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
            var vTemp3 = Sse2.Shuffle(row1, row2, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
            var vTemp2 = Sse2.Shuffle(row3, row4, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
            var vTemp4 = Sse2.Shuffle(row3, row4, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)

            row1 = Sse2.Shuffle(vTemp1, vTemp2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            row2 = Sse2.Shuffle(vTemp1, vTemp2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
            row3 = Sse2.Shuffle(vTemp3, vTemp4, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            row4 = Sse2.Shuffle(vTemp3, vTemp4, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

            var V00 = PermuteDouble(row3, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            var V10 = PermuteDouble(row4, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            var V01 = PermuteDouble(row1, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            var V11 = PermuteDouble(row2, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            var V02 = Sse2.Shuffle(row3, row1, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            var V12 = Sse2.Shuffle(row4, row2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

            var D0 = V00 * V10;
            var D1 = V01 * V11;
            var D2 = V02 * V12;

            V00 = PermuteDouble(row3, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            V10 = PermuteDouble(row4, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            V01 = PermuteDouble(row1, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            V11 = PermuteDouble(row2, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            V02 = Sse2.Shuffle(row3, row1, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
            V12 = Sse2.Shuffle(row4, row2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)

            // Note:  We use this expansion pattern instead of Fused Multiply Add
            // in order to support older hardware
            D0 -= V00 * V10;
            D1 -= V01 * V11;
            D2 -= V02 * V12;

            // V11 = D0Y,D0W,D2Y,D2Y
            V11 = Sse2.Shuffle(D0, D2, 0x5D);  //_MM_SHUFFLE(1, 1, 3, 1)
            V00 = PermuteDouble(row2, 0x49);        //_MM_SHUFFLE(1, 0, 2, 1)
            V10 = Sse2.Shuffle(V11, D0, 0x32); //_MM_SHUFFLE(0, 3, 0, 2)
            V01 = PermuteDouble(row1, 0x12);        //_MM_SHUFFLE(0, 1, 0, 2)
            V11 = Sse2.Shuffle(V11, D0, 0x99); //_MM_SHUFFLE(2, 1, 2, 1)

            // V13 = D1Y,D1W,D2W,D2W
            var V13 = Sse2.Shuffle(D1, D2, 0xFD); //_MM_SHUFFLE(3, 3, 3, 1)
            V02 = PermuteDouble(row4, 0x49);                        //_MM_SHUFFLE(1, 0, 2, 1)
            V12 = Sse2.Shuffle(V13, D1, 0x32);                 //_MM_SHUFFLE(0, 3, 0, 2)
            var V03 = PermuteDouble(row3, 0x12);       //_MM_SHUFFLE(0, 1, 0, 2)
            V13 = Sse2.Shuffle(V13, D1, 0x99);                 //_MM_SHUFFLE(2, 1, 2, 1)

            var C0 = V00 * V10;
            var C2 = V01 * V11;
            var C4 = V02 * V12;
            var C6 = V03 * V13;

            // V11 = D0X,D0Y,D2X,D2X
            V11 = Sse2.Shuffle(D0, D2, 0x4);    //_MM_SHUFFLE(0, 0, 1, 0)
            V00 = PermuteDouble(row2, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
            V10 = Sse2.Shuffle(D0, V11, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
            V01 = PermuteDouble(row1, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
            V11 = Sse2.Shuffle(D0, V11, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

            // V13 = D1X,D1Y,D2Z,D2Z
            V13 = Sse2.Shuffle(D1, D2, 0xa4);   //_MM_SHUFFLE(2, 2, 1, 0)
            V02 = PermuteDouble(row4, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
            V12 = Sse2.Shuffle(D1, V13, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
            V03 = PermuteDouble(row3, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
            V13 = Sse2.Shuffle(D1, V13, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

            C0 -= V00 * V10;
            C2 -= V01 * V11;
            C4 -= V02 * V12;
            C6 -= V03 * V13;

            V00 = PermuteDouble(row2, 0x33); //_MM_SHUFFLE(0, 3, 0, 3)

            // V10 = D0Z,D0Z,D2X,D2Y
            V10 = Sse2.Shuffle(D0, D2, 0x4A); //_MM_SHUFFLE(1, 0, 2, 2)
            V10 = PermuteDouble(V10, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
            V01 = PermuteDouble(row1, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

            // V11 = D0X,D0W,D2X,D2Y
            V11 = Sse2.Shuffle(D0, D2, 0x4C); //_MM_SHUFFLE(1, 0, 3, 0)
            V11 = PermuteDouble(V11, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)
            V02 = PermuteDouble(row4, 0x33);       //_MM_SHUFFLE(0, 3, 0, 3)

            // V12 = D1Z,D1Z,D2Z,D2W
            V12 = Sse2.Shuffle(D1, D2, 0xEA); //_MM_SHUFFLE(3, 2, 2, 2)
            V12 = PermuteDouble(V12, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
            V03 = PermuteDouble(row3, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

            // V13 = D1X,D1W,D2Z,D2W
            V13 = Sse2.Shuffle(D1, D2, 0xEC); //_MM_SHUFFLE(3, 2, 3, 0)
            V13 = PermuteDouble(V13, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)

            V00 *= V10;
            V01 *= V11;
            V02 *= V12;
            V03 *= V13;

            var C1 = C0 - V00;
            C0 += V00;

            var C3 = C2 + V01;
            C2 -= V01;

            var C5 = C4 - V02;
            C4 += V02;

            var C7 = C6 + V03;
            C6 -= V03;

            C0 = Sse2.Shuffle(C0, C1, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C2 = Sse2.Shuffle(C2, C3, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C4 = Sse2.Shuffle(C4, C5, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C6 = Sse2.Shuffle(C6, C7, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

            C0 = PermuteDouble(C0, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C2 = PermuteDouble(C2, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C4 = PermuteDouble(C4, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C6 = PermuteDouble(C6, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

            // Get the determinant
            var det = Vector4D.Dot(C0.AsVector4D(), row1.AsVector4D());

            // Check determinate is not zero
            if (Math.Abs(det) < double.Epsilon)
            {
                var vNaN = new Vector4D<double>(double.NaN);

                result = new Matrix4X4<double>(
                    vNaN,
                    vNaN,
                    vNaN,
                    vNaN
                );

                return false;
            }

            // Create Vector128<float> copy of the determinant and invert them.

            var ones = Vector128.Create(1d);
            var vTemp = Vector128.Create(det);

            vTemp = ones / vTemp;

            result = new Matrix4X4<double>(
                (C0 * vTemp).AsVector4D(),
                (C2 * vTemp).AsVector4D(),
                (C4 * vTemp).AsVector4D(),
                (C6 * vTemp).AsVector4D()
            );

            return true;
        }

        static bool SseImpl(in Matrix4X4<float> matrix, out Matrix4X4<float> result)
        {
            if (!Sse.IsSupported)
            {
                // Redundant test so we won't prejit remainder of this method on platforms without SSE.
                throw new PlatformNotSupportedException();
            }

            // Load the matrix values into rows
            var row1 = matrix.X.AsVector128();
            var row2 = matrix.Y.AsVector128();
            var row3 = matrix.Z.AsVector128();
            var row4 = matrix.W.AsVector128();

            // Transpose the matrix
            var vTemp1 = Sse.Shuffle(row1, row2, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
            var vTemp3 = Sse.Shuffle(row1, row2, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)
            var vTemp2 = Sse.Shuffle(row3, row4, 0x44); //_MM_SHUFFLE(1, 0, 1, 0)
            var vTemp4 = Sse.Shuffle(row3, row4, 0xEE); //_MM_SHUFFLE(3, 2, 3, 2)

            row1 = Sse.Shuffle(vTemp1, vTemp2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            row2 = Sse.Shuffle(vTemp1, vTemp2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
            row3 = Sse.Shuffle(vTemp3, vTemp4, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            row4 = Sse.Shuffle(vTemp3, vTemp4, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

            var V00 = Permute(row3, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            var V10 = Permute(row4, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            var V01 = Permute(row1, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            var V11 = Permute(row2, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            var V02 = Sse.Shuffle(row3, row1, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)
            var V12 = Sse.Shuffle(row4, row2, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)

            var D0 = V00 * V10;
            var D1 = V01 * V11;
            var D2 = V02 * V12;

            V00 = Permute(row3, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            V10 = Permute(row4, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            V01 = Permute(row1, 0xEE);           //_MM_SHUFFLE(3, 2, 3, 2)
            V11 = Permute(row2, 0x50);           //_MM_SHUFFLE(1, 1, 0, 0)
            V02 = Sse.Shuffle(row3, row1, 0xDD); //_MM_SHUFFLE(3, 1, 3, 1)
            V12 = Sse.Shuffle(row4, row2, 0x88); //_MM_SHUFFLE(2, 0, 2, 0)

            // Note:  We use this expansion pattern instead of Fused Multiply Add
            // in order to support older hardware
            D0 -= V00 * V10;
            D1 -= V01 * V11;
            D2 -= V02 * V12;

            // V11 = D0Y,D0W,D2Y,D2Y
            V11 = Sse.Shuffle(D0, D2, 0x5D);  //_MM_SHUFFLE(1, 1, 3, 1)
            V00 = Permute(row2, 0x49);        //_MM_SHUFFLE(1, 0, 2, 1)
            V10 = Sse.Shuffle(V11, D0, 0x32); //_MM_SHUFFLE(0, 3, 0, 2)
            V01 = Permute(row1, 0x12);        //_MM_SHUFFLE(0, 1, 0, 2)
            V11 = Sse.Shuffle(V11, D0, 0x99); //_MM_SHUFFLE(2, 1, 2, 1)

            // V13 = D1Y,D1W,D2W,D2W
            var V13 = Sse.Shuffle(D1, D2, 0xFD); //_MM_SHUFFLE(3, 3, 3, 1)
            V02 = Permute(row4, 0x49);                        //_MM_SHUFFLE(1, 0, 2, 1)
            V12 = Sse.Shuffle(V13, D1, 0x32);                 //_MM_SHUFFLE(0, 3, 0, 2)
            var V03 = Permute(row3, 0x12);       //_MM_SHUFFLE(0, 1, 0, 2)
            V13 = Sse.Shuffle(V13, D1, 0x99);                 //_MM_SHUFFLE(2, 1, 2, 1)

            var C0 = V00 * V10;
            var C2 = V01 * V11;
            var C4 = V02 * V12;
            var C6 = V03 * V13;

            // V11 = D0X,D0Y,D2X,D2X
            V11 = Sse.Shuffle(D0, D2, 0x4);    //_MM_SHUFFLE(0, 0, 1, 0)
            V00 = Permute(row2, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
            V10 = Sse.Shuffle(D0, V11, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
            V01 = Permute(row1, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
            V11 = Sse.Shuffle(D0, V11, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

            // V13 = D1X,D1Y,D2Z,D2Z
            V13 = Sse.Shuffle(D1, D2, 0xa4);   //_MM_SHUFFLE(2, 2, 1, 0)
            V02 = Permute(row4, 0x9e);         //_MM_SHUFFLE(2, 1, 3, 2)
            V12 = Sse.Shuffle(D1, V13, 0x93);  //_MM_SHUFFLE(2, 1, 0, 3)
            V03 = Permute(row3, 0x7b);         //_MM_SHUFFLE(1, 3, 2, 3)
            V13 = Sse.Shuffle(D1, V13, 0x26);  //_MM_SHUFFLE(0, 2, 1, 2)

            C0 -= V00 * V10;
            C2 -= V01 * V11;
            C4 -= V02 * V12;
            C6 -= V03 * V13;

            V00 = Permute(row2, 0x33); //_MM_SHUFFLE(0, 3, 0, 3)

            // V10 = D0Z,D0Z,D2X,D2Y
            V10 = Sse.Shuffle(D0, D2, 0x4A); //_MM_SHUFFLE(1, 0, 2, 2)
            V10 = Permute(V10, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
            V01 = Permute(row1, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

            // V11 = D0X,D0W,D2X,D2Y
            V11 = Sse.Shuffle(D0, D2, 0x4C); //_MM_SHUFFLE(1, 0, 3, 0)
            V11 = Permute(V11, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)
            V02 = Permute(row4, 0x33);       //_MM_SHUFFLE(0, 3, 0, 3)

            // V12 = D1Z,D1Z,D2Z,D2W
            V12 = Sse.Shuffle(D1, D2, 0xEA); //_MM_SHUFFLE(3, 2, 2, 2)
            V12 = Permute(V12, 0x2C);        //_MM_SHUFFLE(0, 2, 3, 0)
            V03 = Permute(row3, 0x8D);       //_MM_SHUFFLE(2, 0, 3, 1)

            // V13 = D1X,D1W,D2Z,D2W
            V13 = Sse.Shuffle(D1, D2, 0xEC); //_MM_SHUFFLE(3, 2, 3, 0)
            V13 = Permute(V13, 0x93);        //_MM_SHUFFLE(2, 1, 0, 3)

            V00 *= V10;
            V01 *= V11;
            V02 *= V12;
            V03 *= V13;

            var C1 = C0 - V00;
            C0 += V00;

            var C3 = C2 + V01;
            C2 -= V01;

            var C5 = C4 - V02;
            C4 += V02;

            var C7 = C6 + V03;
            C6 -= V03;

            C0 = Sse.Shuffle(C0, C1, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C2 = Sse.Shuffle(C2, C3, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C4 = Sse.Shuffle(C4, C5, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C6 = Sse.Shuffle(C6, C7, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

            C0 = Permute(C0, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C2 = Permute(C2, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C4 = Permute(C4, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)
            C6 = Permute(C6, 0xD8); //_MM_SHUFFLE(3, 1, 2, 0)

            // Get the determinant
            var det = Vector4D.Dot(C0.AsVector4D(), row1.AsVector4D());

            // Check determinate is not zero
            if (MathF.Abs(det) < float.Epsilon)
            {
                var vNaN = new Vector4D<float>(float.NaN);

                result = new Matrix4X4<float>(
                    vNaN,
                    vNaN,
                    vNaN,
                    vNaN
                );

                return false;
            }

            // Create Vector128<float> copy of the determinant and invert them.

            var ones = Vector128.Create(1f);
            var vTemp = Vector128.Create(det);

            vTemp = ones / vTemp;

            result = new Matrix4X4<float>(
                (C0 * vTemp).AsVector4D(),
                (C2 * vTemp).AsVector4D(),
                (C4 * vTemp).AsVector4D(),
                (C6 * vTemp).AsVector4D()
            );

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector128<float> Permute(Vector128<float> value, [ConstantExpected] byte control)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, control);
            }

            if (Sse.IsSupported)
            {
                return Sse.Shuffle(value, value, control);
            }

            // Redundant test so we won't prejit remainder of this method on platforms without SSE.
            throw new PlatformNotSupportedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector128<double> PermuteDouble(Vector128<double> value, [ConstantExpected] byte control)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, control);
            }

            if (Sse.IsSupported)
            {
                return Sse2.Shuffle(value, value, control);
            }

            // Redundant test so we won't prejit remainder of this method on platforms without SSE.
            throw new PlatformNotSupportedException();
        }

        static bool SoftwareFallback(in Matrix4X4<T> matrix, out Matrix4X4<T> result)
        {
            //                                       -1
            // If you have matrix M, inverse Matrix M   can compute
            //
            //     -1       1
            //    M   = --------- A
            //            det(M)
            //
            // A is adjugate (adjoint) of M, where,
            //
            //      T
            // A = C
            //
            // C is Cofactor matrix of M, where,
            //           i + j
            // C   = (-1)      * det(M  )
            //  ij                    ij
            //
            //     [ a b c d ]
            // M = [ e f g h ]
            //     [ i j k l ]
            //     [ m n o p ]
            //
            // First Row
            //           2 | f g h |
            // C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
            //  11         | n o p |
            //
            //           3 | e g h |
            // C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
            //  12         | m o p |
            //
            //           4 | e f h |
            // C   = (-1)  | i j l | = + ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
            //  13         | m n p |
            //
            //           5 | e f g |
            // C   = (-1)  | i j k | = - ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
            //  14         | m n o |
            //
            // Second Row
            //           3 | b c d |
            // C   = (-1)  | j k l | = - ( b ( kp - lo ) - c ( jp - ln ) + d ( jo - kn ) )
            //  21         | n o p |
            //
            //           4 | a c d |
            // C   = (-1)  | i k l | = + ( a ( kp - lo ) - c ( ip - lm ) + d ( io - km ) )
            //  22         | m o p |
            //
            //           5 | a b d |
            // C   = (-1)  | i j l | = - ( a ( jp - ln ) - b ( ip - lm ) + d ( in - jm ) )
            //  23         | m n p |
            //
            //           6 | a b c |
            // C   = (-1)  | i j k | = + ( a ( jo - kn ) - b ( io - km ) + c ( in - jm ) )
            //  24         | m n o |
            //
            // Third Row
            //           4 | b c d |
            // C   = (-1)  | f g h | = + ( b ( gp - ho ) - c ( fp - hn ) + d ( fo - gn ) )
            //  31         | n o p |
            //
            //           5 | a c d |
            // C   = (-1)  | e g h | = - ( a ( gp - ho ) - c ( ep - hm ) + d ( eo - gm ) )
            //  32         | m o p |
            //
            //           6 | a b d |
            // C   = (-1)  | e f h | = + ( a ( fp - hn ) - b ( ep - hm ) + d ( en - fm ) )
            //  33         | m n p |
            //
            //           7 | a b c |
            // C   = (-1)  | e f g | = - ( a ( fo - gn ) - b ( eo - gm ) + c ( en - fm ) )
            //  34         | m n o |
            //
            // Fourth Row
            //           5 | b c d |
            // C   = (-1)  | f g h | = - ( b ( gl - hk ) - c ( fl - hj ) + d ( fk - gj ) )
            //  41         | j k l |
            //
            //           6 | a c d |
            // C   = (-1)  | e g h | = + ( a ( gl - hk ) - c ( el - hi ) + d ( ek - gi ) )
            //  42         | i k l |
            //
            //           7 | a b d |
            // C   = (-1)  | e f h | = - ( a ( fl - hj ) - b ( el - hi ) + d ( ej - fi ) )
            //  43         | i j l |
            //
            //           8 | a b c |
            // C   = (-1)  | e f g | = + ( a ( fk - gj ) - b ( ek - gi ) + c ( ej - fi ) )
            //  44         | i j k |
            //
            // Cost of operation
            // 53 adds, 104 muls, and 1 div.

            T a = matrix.X.X, b = matrix.X.Y, c = matrix.X.Z, d = matrix.X.W;
            T e = matrix.Y.X, f = matrix.Y.Y, g = matrix.Y.Z, h = matrix.Y.W;
            T i = matrix.Z.X, j = matrix.Z.Y, k = matrix.Z.Z, l = matrix.Z.W;
            T m = matrix.W.X, n = matrix.W.Y, o = matrix.W.Z, p = matrix.W.W;

            T kp_lo = k * p - l * o;
            T jp_ln = j * p - l * n;
            T jo_kn = j * o - k * n;
            T ip_lm = i * p - l * m;
            T io_km = i * o - k * m;
            T in_jm = i * n - j * m;

            T a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
            T a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            T a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
            T a14 = -(e * jo_kn - f * io_km + g * in_jm);

            T det = a * a11 + b * a12 + c * a13 + d * a14;

            if (T.Abs(det) < T.Epsilon)
            {
                Vector4D<T> vNaN = new Vector4D<T>(T.NaN);

                result = new Matrix4X4<T>(vNaN, vNaN, vNaN, vNaN);

                return false;
            }

            T invDet = T.One / det;

            T gp_ho = g * p - h * o;
            T fp_hn = f * p - h * n;
            T fo_gn = f * o - g * n;
            T ep_hm = e * p - h * m;
            T eo_gm = e * o - g * m;
            T en_fm = e * n - f * m;

            T gl_hk = g * l - h * k;
            T fl_hj = f * l - h * j;
            T fk_gj = f * k - g * j;
            T el_hi = e * l - h * i;
            T ek_gi = e * k - g * i;
            T ej_fi = e * j - f * i;

            result = new Matrix4X4<T>(
                new Vector4D<T>(
                    a11 * invDet,
                    -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet,
                    +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet,
                    -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet
                ),
                new Vector4D<T>(
                    a12 * invDet,
                    +(a * kp_lo - c * ip_lm + d * io_km) * invDet,
                    -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet,
                    +(a * gl_hk - c * el_hi + d * ek_gi) * invDet
                ),
                new Vector4D<T>(
                    a13 * invDet,
                    -(a * jp_ln - b * ip_lm + d * in_jm) * invDet,
                    +(a * fp_hn - b * ep_hm + d * en_fm) * invDet,
                    -(a * fl_hj - b * el_hi + d * ej_fi) * invDet
                ),
                new Vector4D<T>(
                    a14 * invDet,
                    +(a * jo_kn - b * io_km + c * in_jm) * invDet,
                    -(a * fo_gn - b * eo_gm + c * en_fm) * invDet,
                    +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet
                )
            );

            return true;
        }
    }

    /// <summary>Performs a linear interpolation from one matrix to a second matrix based on a value that specifies the weighting of the second matrix.</summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <param name="amount">The relative weighting of <paramref name="right" />.</param>
    /// <returns>The interpolated matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> Lerp<T>(Matrix4X4<T> left, Matrix4X4<T> right, T amount)
        where T : INumberBase<T>
    {
        return new Matrix4X4<T>(
            Vector4D.Lerp(left.X, right.X, amount),
            Vector4D.Lerp(left.Y, right.Y, amount),
            Vector4D.Lerp(left.Z, right.Z, amount),
            Vector4D.Lerp(left.W, right.W, amount)
        );
    }

    /// <summary>Multiplies two matrices together to compute the product.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The product matrix.</returns>
    public static Matrix4X4<T> Multiply<T>(Matrix4X4<T> value1, Matrix4X4<T> value2)
        where T : INumberBase<T>
        => value1 * value2;

    /// <summary>Multiplies a matrix by a T to compute the product.</summary>
    /// <param name="value1">The matrix to scale.</param>
    /// <param name="value2">The scaling value to use.</param>
    /// <returns>The scaled matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> Multiply<T>(Matrix4X4<T> value1, T value2)
        where T : INumberBase<T>
        => value1 * value2;

    /// <summary>Negates the specified matrix by multiplying all its values by -1.</summary>
    /// <param name="value">The matrix to negate.</param>
    /// <returns>The negated matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> Negate<T>(Matrix4X4<T> value)
        where T : INumberBase<T>
        => -value;

    /// <summary>Subtracts each element in a second matrix from its corresponding element in a first matrix.</summary>
    /// <param name="value1">The first matrix.</param>
    /// <param name="value2">The second matrix.</param>
    /// <returns>The matrix containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4X4<T> Subtract<T>(Matrix4X4<T> value1, Matrix4X4<T> value2)
        where T : INumberBase<T>
        => value1 - value2;

    /// <summary>Transforms the specified matrix by applying the specified Quaternion<T> rotation.</summary>
    /// <param name="value">The matrix to transform.</param>
    /// <param name="rotation">The rotation t apply.</param>
    /// <returns>The transformed matrix.</returns>
    public static Matrix4X4<T> Transform<T>(Matrix4X4<T> value, Quaternion<T> rotation)
        where T : INumberBase<T>, ITrigonometricFunctions<T>, IRootFunctions<T>
    {
        // Compute rotation matrix.
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

        T q11 = T.One - yy2 - zz2;
        T q21 = xy2 - wz2;
        T q31 = xz2 + wy2;

        T q12 = xy2 + wz2;
        T q22 = T.One - xx2 - zz2;
        T q32 = yz2 - wx2;

        T q13 = xz2 - wy2;
        T q23 = yz2 + wx2;
        T q33 = T.One - xx2 - yy2;

        return new Matrix4X4<T>(
            new Vector4D<T>(
                value.X.X * q11 + value.X.Y * q21 + value.X.Z * q31,
                value.X.X * q12 + value.X.Y * q22 + value.X.Z * q32,
                value.X.X * q13 + value.X.Y * q23 + value.X.Z * q33,
                value.X.W
            ),
            new Vector4D<T>(
                value.Y.X * q11 + value.Y.Y * q21 + value.Y.Z * q31,
                value.Y.X * q12 + value.Y.Y * q22 + value.Y.Z * q32,
                value.Y.X * q13 + value.Y.Y * q23 + value.Y.Z * q33,
                value.Y.W
            ),
            new Vector4D<T>(
                value.Z.X * q11 + value.Z.Y * q21 + value.Z.Z * q31,
                value.Z.X * q12 + value.Z.Y * q22 + value.Z.Z * q32,
                value.Z.X * q13 + value.Z.Y * q23 + value.Z.Z * q33,
                value.Z.W
            ),
            new Vector4D<T>(
                value.W.X * q11 + value.W.Y * q21 + value.W.Z * q31,
                value.W.X * q12 + value.W.Y * q22 + value.W.Z * q32,
                value.W.X * q13 + value.W.Y * q23 + value.W.Z * q33,
                value.W.W
            )
        );
    }

    /// <summary>Transposes the rows and columns of a matrix.</summary>
    /// <param name="matrix">The matrix to transpose.</param>
    /// <returns>The transposed matrix.</returns>
    public static Matrix4X4<T> Transpose<T>(Matrix4X4<T> matrix)
        where T : INumberBase<T>
    {
        // This implementation is based on the DirectX Math Library XMMatrixTranspose method
        // https://github.com/microsoft/DirectXMath/blob/master/Inc/DirectXMathMatrix.inl

        Vector4D<T> X, Y, Z, W;

        if (AdvSimd.Arm64.IsSupported && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            if (typeof(T) == typeof(float))
            {
                var x = matrix.X.AsVector128<float>();
                var y = matrix.Y.AsVector128<float>();
                var z = matrix.Z.AsVector128<float>();
                var w = matrix.W.AsVector128<float>();

                var lowerXZ = AdvSimd.Arm64.ZipLow(x, z); // x[0], z[0], x[1], z[1]
                var lowerYW = AdvSimd.Arm64.ZipLow(y, w); // y[0], w[0], y[1], w[1]
                var upperXZ = AdvSimd.Arm64.ZipHigh(x, z); // x[2], z[2], x[3], z[3]
                var upperYW = AdvSimd.Arm64.ZipHigh(y, w); // y[2], w[2], y[3], z[3]

                X = AdvSimd.Arm64.ZipLow(lowerXZ, lowerYW).AsVector4DUnsafe<float, T>(); // x[0], y[0], z[0], w[0]
                Y = AdvSimd.Arm64.ZipHigh(lowerXZ, lowerYW).AsVector4DUnsafe<float, T>(); // x[1], y[1], z[1], w[1]
                Z = AdvSimd.Arm64.ZipLow(upperXZ, upperYW).AsVector4DUnsafe<float, T>(); // x[2], y[2], z[2], w[2]
                W = AdvSimd.Arm64.ZipHigh(upperXZ, upperYW).AsVector4DUnsafe<float, T>(); // x[3], y[3], z[3], w[3]
            }
            else // if (typeof(T) == typeof(double))
            {
                var x = matrix.X.AsVector128<double>();
                var y = matrix.Y.AsVector128<double>();
                var z = matrix.Z.AsVector128<double>();
                var w = matrix.W.AsVector128<double>();

                var lowerXZ = AdvSimd.Arm64.ZipLow(x, z); // x[0], z[0], x[1], z[1]
                var lowerYW = AdvSimd.Arm64.ZipLow(y, w); // y[0], w[0], y[1], w[1]
                var upperXZ = AdvSimd.Arm64.ZipHigh(x, z); // x[2], z[2], x[3], z[3]
                var upperYW = AdvSimd.Arm64.ZipHigh(y, w); // y[2], w[2], y[3], z[3]

                X = AdvSimd.Arm64.ZipLow(lowerXZ, lowerYW).AsVector4DUnsafe<double, T>(); // x[0], y[0], z[0], w[0]
                Y = AdvSimd.Arm64.ZipHigh(lowerXZ, lowerYW).AsVector4DUnsafe<double, T>(); // x[1], y[1], z[1], w[1]
                Z = AdvSimd.Arm64.ZipLow(upperXZ, upperYW).AsVector4DUnsafe<double, T>(); // x[2], y[2], z[2], w[2]
                W = AdvSimd.Arm64.ZipHigh(upperXZ, upperYW).AsVector4DUnsafe<double, T>(); // x[3], y[3], z[3], w[3]
            }
        }
        else if (Sse.IsSupported && typeof(T) == typeof(float))
        {
            var x = matrix.X.AsVector128<float>();
            var y = matrix.Y.AsVector128<float>();
            var z = matrix.Z.AsVector128<float>();
            var w = matrix.W.AsVector128<float>();

            var lowerXZ = Sse.UnpackLow(x, z); // x[0], z[0], x[1], z[1]
            var lowerYW = Sse.UnpackLow(y, w); // y[0], w[0], y[1], w[1]
            var upperXZ = Sse.UnpackHigh(x, z); // x[2], z[2], x[3], z[3]
            var upperYW = Sse.UnpackHigh(y, w); // y[2], w[2], y[3], z[3]

            X = Sse.UnpackLow(lowerXZ, lowerYW).AsVector4DUnsafe<float, T>(); // x[0], y[0], z[0], w[0]
            Y = Sse.UnpackHigh(lowerXZ, lowerYW).AsVector4DUnsafe<float, T>(); // x[1], y[1], z[1], w[1]
            Z = Sse.UnpackLow(upperXZ, upperYW).AsVector4DUnsafe<float, T>(); // x[2], y[2], z[2], w[2]
            W = Sse.UnpackHigh(upperXZ, upperYW).AsVector4DUnsafe<float, T>(); // x[3], y[3], z[3], w[3]
        }
        else if (Sse2.IsSupported && typeof(T) == typeof(double))
        {
            var x = matrix.X.AsVector128<double>();
            var y = matrix.Y.AsVector128<double>();
            var z = matrix.Z.AsVector128<double>();
            var w = matrix.W.AsVector128<double>();

            var lowerXZ = Sse2.UnpackLow(x, z); // x[0], z[0], x[1], z[1]
            var lowerYW = Sse2.UnpackLow(y, w); // y[0], w[0], y[1], w[1]
            var upperXZ = Sse2.UnpackHigh(x, z); // x[2], z[2], x[3], z[3]
            var upperYW = Sse2.UnpackHigh(y, w); // y[2], w[2], y[3], z[3]

            X = Sse2.UnpackLow(lowerXZ, lowerYW).AsVector4DUnsafe<double, T>(); // x[0], y[0], z[0], w[0]
            Y = Sse2.UnpackHigh(lowerXZ, lowerYW).AsVector4DUnsafe<double, T>(); // x[1], y[1], z[1], w[1]
            Z = Sse2.UnpackLow(upperXZ, upperYW).AsVector4DUnsafe<double, T>(); // x[2], y[2], z[2], w[2]
            W = Sse2.UnpackHigh(upperXZ, upperYW).AsVector4DUnsafe<double, T>(); // x[3], y[3], z[3], w[3]
        }
        else
        {
            X = new Vector4D<T>(matrix.X.X, matrix.Y.X, matrix.Z.X, matrix.W.X);
            Y = new Vector4D<T>(matrix.X.Y, matrix.Y.Y, matrix.Z.Y, matrix.W.Y);
            Z = new Vector4D<T>(matrix.X.Z, matrix.Y.Z, matrix.Z.Z, matrix.W.Z);
            W = new Vector4D<T>(matrix.X.W, matrix.Y.W, matrix.Z.W, matrix.W.W);
        }

        return new Matrix4X4<T>(X, Y, Z, W);
    }

    /// <summary>Calculates the determinant of the current 4x4 matrix.</summary>
    /// <returns>The determinant.</returns>
    public static T GetDeterminant<T>(this Matrix4X4<T> self)
        where T : INumberBase<T>
    {
        // | a b c d |     | f g h |     | e g h |     | e f h |     | e f g |
        // | e f g h | = a | j k l | - b | i k l | + c | i j l | - d | i j k |
        // | i j k l |     | n o p |     | m o p |     | m n p |     | m n o |
        // | m n o p |
        //
        //   | f g h |
        // a | j k l | = a ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
        //   | n o p |
        //
        //   | e g h |
        // b | i k l | = b ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
        //   | m o p |
        //
        //   | e f h |
        // c | i j l | = c ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
        //   | m n p |
        //
        //   | e f g |
        // d | i j k | = d ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
        //   | m n o |
        //
        // Cost of operation
        // 17 adds and 28 muls.
        //
        // add: 6 + 8 + 3 = 17
        // mul: 12 + 16 = 28

        T a = self.X.X, b = self.X.Y, c = self.X.Z, d = self.X.W;
        T e = self.Y.X, f = self.Y.Y, g = self.Y.Z, h = self.Y.W;
        T i = self.Z.X, j = self.Z.Y, k = self.Z.Z, l = self.Z.W;
        T m = self.W.X, n = self.W.Y, o = self.W.Z, p = self.W.W;

        T kp_lo = k * p - l * o;
        T jp_ln = j * p - l * n;
        T jo_kn = j * o - k * n;
        T ip_lm = i * p - l * m;
        T io_km = i * o - k * m;
        T in_jm = i * n - j * m;

        return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
               b * (e * kp_lo - g * ip_lm + h * io_km) +
               c * (e * jp_ln - f * ip_lm + h * in_jm) -
               d * (e * jo_kn - f * io_km + g * in_jm);
    }

    private struct CanonicalBasis<T> where T : INumberBase<T>
    {
        public Vector3D<T> Row0;
        public Vector3D<T> Row1;
        public Vector3D<T> Row2;
    };

    private unsafe struct VectorBasis<T> where T : INumberBase<T>
    {
        public Vector3D<T>* Element0;
        public Vector3D<T>* Element1;
        public Vector3D<T>* Element2;
    }

}