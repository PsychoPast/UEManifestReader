namespace UEManifestReader
{
    using System;
    using System.IO;
    using static global::UEManifestReader.Utilities;

    internal ref struct FGuid
    {
        internal FGuid(Stream stream)
        {
            A = SwapBytesOrder(stream.ReadBytes(4));
            B = SwapBytesOrder(stream.ReadBytes(4));
            C = SwapBytesOrder(stream.ReadBytes(4));
            D = SwapBytesOrder(stream.ReadBytes(4));
        }

        private readonly ArraySegment<byte> A;

        private readonly ArraySegment<byte> B;

        private readonly ArraySegment<byte> C;

        private readonly ArraySegment<byte> D;

        public override string ToString() => $"{BytesToHexadecimalString(A)}{BytesToHexadecimalString(B)}{BytesToHexadecimalString(C)}{BytesToHexadecimalString(D)}";
    }
}