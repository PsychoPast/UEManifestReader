namespace UEManifestReader
{
    /// <summary>
    /// Location where to save the manifest data.
    /// </summary>
    public enum ManifestStorage
    {
        /// <summary>
        /// Save the manifest content in memory.
        /// </summary>
        Memory,

        /// <summary>
        /// Save the manifest content on the disk.
        /// </summary>
        Disk
    }
}