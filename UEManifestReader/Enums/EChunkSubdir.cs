namespace UEManifestReader.Enums
{
    public enum EChunkSubdir
    {
        /// <summary>
        /// Only if <see cref="CustomManifestReadingSettings.ReadManifestMeta"/> is <see langword="false"/>.
        /// </summary>
        Undefined,
        Chunks,
        ChunksV2,
        ChunksV3,
        ChunksV4
    }
}