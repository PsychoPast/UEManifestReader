using System;
using System.Runtime.CompilerServices;

namespace UEManifestReader
{
    internal static unsafe class Utilities
    {
        internal static string BytesToHexadecimalString(ReadOnlySpan<byte> input)
        {
            int length = input.Length * 2;
            char* buffer = stackalloc char[length * sizeof(char)];
            fixed (byte* pByte = input)
            {
                ToHexadecimalString(input.Length, buffer, pByte);
            }

            return new string(buffer);
        }

        internal static byte[] SwapBytesOrder(ReadOnlySpan<byte> input)
        {
            byte[] buffer = new byte[input.Length];
            int lastIndex = input.Length - 1;
            fixed (byte* pByte = input)
            {
                byte* cByte = pByte;
                for (int i = 0; i < input.Length; i++)
                {
                    buffer[lastIndex--] = *cByte++;
                }
            }

            return buffer;
        }

        internal static string ULongToHexHash(ulong value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(ulong)];
            buffer[0] = (byte)(value >> 56);  
            buffer[1] = (byte)(value >> 48);
            buffer[2] = (byte)(value >> 40);
            buffer[3] = (byte)(value >> 32);
            buffer[4] = (byte)(value >> 24);
            buffer[5] = (byte)(value >> 16);
            buffer[6] = (byte)(value >> 8);
            buffer[7] = (byte)value;

            if (!BitConverter.IsLittleEndian)
            {
                SwapBytesOrder(buffer);
            }

            return BytesToHexadecimalString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetHexCharFromByte(byte rawByte)
            => (char)(rawByte > 9 ?
            rawByte - 10 + 'A' :
            rawByte + '0');

        private static void ToHexadecimalString(int length, char* buffer, byte* pByte)
        {
            byte rawByte;
            for (int i = 0; i < length; i++)
            {
                rawByte = (byte)(*pByte >> 4);
                *buffer++ = GetHexCharFromByte(rawByte);
                rawByte = (byte)(*pByte++ & 0x0F);
                *buffer++ = GetHexCharFromByte(rawByte);
            }
        }
    }
}