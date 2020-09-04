namespace UEManifestReader.Enums
{
    public enum EFileSubdir
    {
        /// <summary>
        /// Only if <see cref="CustomManifestReadingSettings.ShouldReadManifestMeta"/> is <see langword="false"/>.
        /// </summary>
        Undefined,
        Files,
        FilesV2,
        FilesV3
    }
}