using System;

namespace UEManifestReader.Objects
{
    public readonly struct FChunkInfo : IEquatable<FChunkInfo>
    {
        public FChunkInfo(string guid, string hash, string shaHash, string grpNum, uint? windowSize, long? fileSize)
        {
            Guid = guid;
            Hash = hash;
            ShaHash = shaHash;
            GroupNumber = grpNum;
            WindowSize = windowSize;
            FileSize = fileSize;
        }

        /// <summary>
        /// The GUID for this data.
        /// </summary>
        public string Guid { get; }

        /// <summary>
        /// The FRollingHash hashed value for this chunk data.
        /// </summary>
        public string Hash { get; } // base type: ulong (making it a string in order for it to be directly usable to download purposes for example)

        /// <summary>
        /// The FSHA hashed value for this chunk data.
        /// </summary>
        public string ShaHash { get; }

        /// <summary>
        /// The group number this chunk divides into.
        /// </summary>
        public string GroupNumber { get; } // base type: byte (making it a string in order for it to be directly usable to download purposes for example)

        /// <summary>
        /// The window size for this chunk.
        /// </summary>
        public uint? WindowSize { get; }

        /// <summary>
        /// The file download size for this chunk.
        /// </summary>
        public long? FileSize { get; }

        public static bool operator ==(FChunkInfo left, FChunkInfo right) => left.Guid == right.Guid;

        public static bool operator !=(FChunkInfo left, FChunkInfo right) => left.Guid != right.Guid;

        public override bool Equals(object obj) => obj is FChunkInfo fChunkInfo && fChunkInfo == this;

        public override int GetHashCode() => HashCode.Combine(Guid, Hash);

        /// <summary>
        /// Returns the chunk download path.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GroupNumber}/{Hash}_{Guid}.chunk";

        public bool Equals(FChunkInfo other) => other == this;
    }
}