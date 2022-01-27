using System;
using System.IO;
using static UEManifestReader.Utilities;

namespace UEManifestReader
{
    internal ref struct FGuid
    {
        private readonly ArraySegment<byte> _a;

        private readonly ArraySegment<byte> _b;

        private readonly ArraySegment<byte> _c;

        private readonly ArraySegment<byte> _d;

        internal FGuid(Stream stream)
        {
            _a = SwapBytesOrder(stream.ReadBytes(4));
            _b = SwapBytesOrder(stream.ReadBytes(4));
            _c = SwapBytesOrder(stream.ReadBytes(4));
            _d = SwapBytesOrder(stream.ReadBytes(4));
        }

        public override string ToString() =>
            $"{BytesToHexadecimalString(_a)}{BytesToHexadecimalString(_b)}{BytesToHexadecimalString(_c)}{BytesToHexadecimalString(_d)}";
    }
}