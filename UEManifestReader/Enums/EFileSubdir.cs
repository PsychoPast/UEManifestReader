namespace UEManifestReader.Enums
{
    public enum EFileSubdir
    {
        /// <summary>
        /// Only if <see cref="CustomManifestReadingSettings.ReadManifestMeta"/> is <see langword="false"/>.
        /// </summary>
        Undefined,
        Files,
        FilesV2,
        FilesV3
    }
}