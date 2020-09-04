namespace UEManifestReader.Objects
{
    using System;
    using System.IO;

    public readonly struct FChunkPart : IEquatable<FChunkPart>
    {
        /// <summary>
        /// The GUID of the chunk containing this part.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// The offset of the first byte into the chunk.
        /// </summary>
        public readonly uint Offset;

        /// <summary>
        /// The size of this part.
        /// </summary>
        public readonly uint Size;

        public FChunkPart(Stream reader)
        {
            reader.ReadUInt(); // dataSize
            Guid = new FGuid(reader).ToString();
            Offset = reader.ReadUInt();
            Size = reader.ReadUInt();
        }

        public static bool operator ==(FChunkPart left, FChunkPart right) => left.Guid == right.Guid;

        public static bool operator !=(FChunkPart left, FChunkPart right) => left.Guid != right.Guid;

        public override bool Equals(object obj) => obj is FChunkPart fChunkPart && fChunkPart == this;

        public override int GetHashCode() => HashCode.Combine(Guid);

        public bool Equals(FChunkPart other) => other == this;
    }
}