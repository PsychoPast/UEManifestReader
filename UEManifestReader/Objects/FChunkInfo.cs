namespace UEManifestReader.Objects
{
    using System;

    public readonly struct FChunkInfo : IEquatable<FChunkInfo>
    {
        /// <summary>
        /// The GUID for this data.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// The FRollingHash hashed value for this chunk data.
        /// </summary>
        public readonly string Hash; // base type: ulong (making it a string in order for it to be directly usable to download purposes for example)

        /// <summary>
        /// The FSHA hashed value for this chunk data.
        /// </summary>
        public readonly string ShaHash;

        /// <summary>
        /// The group number this chunk divides into.
        /// </summary>
        public readonly string GroupNumber; // base type: byte (making it a string in order for it to be directly usable to download purposes for example)

        /// <summary>
        /// The window size for this chunk.
        /// </summary>
        public readonly uint? WindowSize;

        /// <summary>
        /// The file download size for this chunk.
        /// </summary>
        public readonly long? FileSize;

        public FChunkInfo(string guid, string hash, string shaHash, string grpNum, uint? windowSize, long? fileSize)
        {
            Guid = guid;
            Hash = hash;
            ShaHash = shaHash;
            GroupNumber = grpNum;
            WindowSize = windowSize;
            FileSize = fileSize;
        }

        public static bool operator ==(FChunkInfo left, FChunkInfo right) => left.Guid == right.Guid;

        public static bool operator !=(FChunkInfo left, FChunkInfo right) => left.Guid != right.Guid;

        public override bool Equals(object obj) => obj is FChunkInfo fChunkInfo && fChunkInfo == this;

        public override int GetHashCode() => HashCode.Combine(Guid, Hash);

        public bool Equals(FChunkInfo other) => other == this;
    }
}