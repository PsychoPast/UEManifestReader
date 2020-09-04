namespace UEManifestReader.Enums
{
    using System;

    [Flags]
    internal enum EManifestStorageFlags : byte
    {
        // Stored as raw data.
        None = 0,
        // Flag for compressed data.
        Compressed = 1,
        // Flag for encrypted. If also compressed, decrypt first. Encryption will ruin compressibility.
        // Encrypted = 1 << 1  (unused for the moment it seems)
    }
}