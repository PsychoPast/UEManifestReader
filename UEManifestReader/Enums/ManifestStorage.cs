namespace UEManifestReader.Enums
{
    /// <summary>
    /// Location where to save the temporary manifest data in case of download, decryption or decompression.
    /// </summary>
    public enum ManifestStorage
    {
        /// <summary>
        /// Saves the manifest content in memory.
        /// </summary>
        Memory,

        /// <summary>
        /// Saves the manifest content on the disk.
        /// </summary>
        Disk
    }
}