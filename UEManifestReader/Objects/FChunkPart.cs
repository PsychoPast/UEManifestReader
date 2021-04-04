using System;
using System.IO;

namespace UEManifestReader.Objects
{
    public readonly struct FChunkPart : IEquatable<FChunkPart>
    {
        public FChunkPart(string guid, uint offset, uint size)
        {
            Guid = guid;
            Offset = offset;
            Size = size;
        }

        internal FChunkPart(Stream reader)
        {
            reader.ReadUInt(); // dataSize
            Guid = new FGuid(reader).ToString();
            Offset = reader.ReadUInt();
            Size = reader.ReadUInt();
        }

        /// <summary>
        ///     The GUID of the chunk containing this part.
        /// </summary>
        public string Guid { get; }

        /// <summary>
        ///     The offset of the first byte into the chunk.
        /// </summary>
        public uint Offset { get; }

        /// <summary>
        ///     The size of this part.
        /// </summary>
        public uint Size { get; }

        public static bool operator ==(FChunkPart left, FChunkPart right) => left.Guid == right.Guid;

        public static bool operator !=(FChunkPart left, FChunkPart right) => left.Guid != right.Guid;

        public override bool Equals(object obj) => obj is FChunkPart fChunkPart && fChunkPart == this;

        public override int GetHashCode() => HashCode.Combine(Guid);

        public bool Equals(FChunkPart other) => other == this;
    }
}