namespace UEManifestReader.Enums
{
    public enum EChunkSubdir
    {
        /// <summary>
        /// Only if <see cref="CustomManifestReadingSettings.ShouldReadManifestMeta"/> is <see langword="false"/>.
        /// </summary>
        Undefined,
        Chunks,
        ChunksV2,
        ChunksV3,
        ChunksV4
    }
}