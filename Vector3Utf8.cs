#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Unicode;

namespace GenericVector;

public partial struct Vector3<T> : IUtf8SpanFormattable, IUtf8SpanParsable<Vector3<T>>
{
    public readonly bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Possible fast path for failure case:
        // if (destination.Length < 4) return false;
        
        var separator = NumberFormatInfo.GetInstance(provider).NumberGroupSeparator;
        
        // We can't use an interpolated string here because it won't allow us to pass `format`
        var handler = new Utf8.TryWriteInterpolatedStringHandler(
            4 + (separator.Length * 2),
            3,
            utf8Destination,
            provider,
            out var shouldAppend
        );
        if (!shouldAppend)
        {
            bytesWritten = 0;
            return false;
        }
        
        // Annoyingly we need to turn the span into a string for the string handler
        // We only assign it on first use, just in case we fail before that
        string? formatString;

        // JIT will automagically convert literals to utf8
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

        return Utf8.TryWrite(utf8Destination, ref handler, out bytesWritten);
    }

    public static Vector3<T> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => TryParse(utf8Text, provider, out var result) ? result : throw new ArgumentException($"Failed to parse {nameof(Vector3)}<{typeof(T)}>");

    public static bool TryParse(ReadOnlySpan<byte> s, IFormatProvider? provider, out Vector3<T> result)
    {
        result = default;

        if (s[0] != (byte)'<') return false;
        if (s[^1] != (byte)'>') return false;
        
        var separator = NumberGroupSeparatorTChar<byte>(NumberFormatInfo.GetInstance(provider));

        s = s[1..^1];

        T? x, y, z;

        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out x)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        
        {
            if (s.Length == 0) return false;

            var nextNumber = s.IndexOf(separator);
            if (nextNumber == -1)
            {
                return false;
            }

            if (!T.TryParse(s[..nextNumber], provider, out y)) return false;

            s = s[(nextNumber + separator.Length)..];
        }
        {
            if (s.Length == 0) return false;

            if (!T.TryParse(s, provider, out z)) return false;
        }

        result = new Vector3<T>(x, y, z);
        return true;

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(NumberGroupSeparatorTChar))] static extern ReadOnlySpan<TChar> NumberGroupSeparatorTChar<TChar>(NumberFormatInfo? c) where TChar : unmanaged;
    }
}
#endif