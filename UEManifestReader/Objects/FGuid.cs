namespace UEManifestReader
{
    using System;
    using System.IO;
    using static global::UEManifestReader.Utilities;

    internal ref struct FGuid
    {
        public FGuid(Stream stream)
        {
            A = SwapBytesOrder(stream.ReadBytes(4));
            B = SwapBytesOrder(stream.ReadBytes(4));
            C = SwapBytesOrder(stream.ReadBytes(4));
            D = SwapBytesOrder(stream.ReadBytes(4));
        }

        public ArraySegment<byte> A { get; private set; }

        public ArraySegment<byte> B { get; private set; }

        public ArraySegment<byte> C { get; private set; }

        public ArraySegment<byte> D { get; private set; }

        public override string ToString() => $"{BytesToHexadecimalString(A)}{BytesToHexadecimalString(B)}{BytesToHexadecimalString(C)}{BytesToHexadecimalString(D)}";
    }
}