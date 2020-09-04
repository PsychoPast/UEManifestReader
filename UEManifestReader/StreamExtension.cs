namespace UEManifestReader
{
    using System;
    using System.IO;
    using System.Text;
    using global::UEManifestReader.Exceptions;

    internal static class StreamExtension
    {
        internal static byte[] ReadBytes(this Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            stream.Read(buffer);
            return buffer;
        }

        internal static void SkipFString(this Stream stream)
        {
            int fstringLength = stream.ReadInt();
            stream.Position += fstringLength > 0 ? fstringLength : -fstringLength * sizeof(char);
        }

        internal static bool ReadBool(this Stream stream) => UnsafeReadAs<byte>(stream) != 0;

        internal static char ReadChar(this Stream stream) => UnsafeReadAs<char>(stream);

        internal static int ReadInt(this Stream stream) => UnsafeReadAs<int>(stream);

        internal static uint ReadUInt(this Stream stream) => UnsafeReadAs<uint>(stream);

        internal static long ReadLong(this Stream stream) => UnsafeReadAs<long>(stream);

        internal static ulong ReadULong(this Stream stream) => UnsafeReadAs<ulong>(stream);

        internal static string ReadFString(this Stream stream)
        {
            int saveNum = stream.ReadInt();
            bool bLoadUnicodeChar = saveNum < 0;
            if (bLoadUnicodeChar)
            {
                if (saveNum == int.MinValue)
                {
                    throw new UEManifestReaderException(null, new FStringInvalidException("Archive is corrupted!"));
                }

                saveNum = -saveNum;
            }

            if (saveNum == 0)
            {
                return string.Empty;
            }

            if (saveNum == 1)
            {
                if (stream.ReadByte() != 0)
                {
                    throw new UEManifestReaderException(null, new FStringInvalidException("FString is not null terminated!"));
                }

                return string.Empty;
            }

            if (bLoadUnicodeChar) // UCS2CHAR: unicode (16-bit char) string
            {
                char[] chars = new char[saveNum];
                for (int i = 0; i < saveNum; i++)
                {
                    chars[i] = stream.ReadChar();
                }

                if (chars[^1] != '\0')
                {
                    throw new UEManifestReaderException(null, new FStringInvalidException("FString is not null terminated!"));
                }

                return new string(chars, 0, chars.Length - 1);
            }
            else // ANSICHAR: ascii (8-bit (1 byte) fixed-width 7-bit chars) string
            {
                byte[] buffer = stream.ReadBytes(saveNum);
                if (buffer[^1] != '\0')
                {
                    throw new UEManifestReaderException(null, new FStringInvalidException("FString is not null terminated!"));
                }

                return Encoding.ASCII.GetString(buffer, 0, buffer.Length - 1);
            }
        }

        internal static T[] ReadTArray<T>(this Stream stream, Func<T> readAs) => ReadArray(stream, stream.ReadInt(), readAs);

        internal static T[] ReadArray<T>(this Stream _, int count, Func<T> readAs)
        {
            T[] buffer = new T[count];
            for (int i = 0; i < count; i++)
            {
                buffer[i] = readAs();
            }

            return buffer;
        }

        private static unsafe T UnsafeReadAs<T>(this Stream stream)
            where T : unmanaged
        {
            byte* byteAlloc = stackalloc byte[sizeof(T)];
            for (int i = 0; i < sizeof(T); i++)
            {
                byteAlloc[i] = (byte)stream.ReadByte();
            }

            return *(T*)byteAlloc;
        }
    }
}