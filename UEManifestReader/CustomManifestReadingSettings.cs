namespace UEManifestReader
{
    internal sealed class CustomManifestReadingSettings
    {
        private ushort settingsFlag;

        internal CustomManifestReadingSettings(
            bool readManifestMeta = true, 
            bool readChunksGuid = true, 
            bool readChunksHash = true, 
            bool readChunksShaHash = true, 
            bool readChunksGroupNumber = true,
            bool readChunksWindowSize = true, 
            bool readChunksDownloadSize = true, 
            bool readFileFileName = true, 
            bool readFileSymLinkTarget = true, 
            bool readFileHash = true, 
            bool readFileMetaFlag = true,
            bool readFileInstallTags = true, 
            bool readFChunksParts = true, 
            bool readCustomFields = true)

            => SetFlags(
                readManifestMeta,
                readChunksGuid,
                readChunksHash,
                readChunksShaHash,
                readChunksGroupNumber,
                readChunksWindowSize,
                readChunksDownloadSize,
                readFileFileName,
                readFileSymLinkTarget,
                readFileHash,
                readFileMetaFlag,
                readFileInstallTags,
                readFChunksParts,
                readCustomFields);

        internal bool ShouldReadManifestMeta => CheckFlag(0);

        internal bool ShouldReadChunksGuid => CheckFlag(1);

        internal bool ShouldReadChunksHash => CheckFlag(2);

        internal bool ShouldReadChunksShaHash => CheckFlag(3);

        internal bool ShoudlReadChunksGroupNumber => CheckFlag(4);

        internal bool ShouldReadChunksWindowSize => CheckFlag(5);

        internal bool ShouldReadChunksDownloadSize => CheckFlag(6);

        internal bool ShouldReadFileFileName => CheckFlag(7);

        internal bool ShouldReadFileSymLinkTarget => CheckFlag(8);

        internal bool ShouldReadFileHash => CheckFlag(9);

        internal bool ShouldReadFileMetaFlag => CheckFlag(10);

        internal bool ShouldReadFileInstallTags => CheckFlag(11);

        internal bool ShouldReadFChunkPart => CheckFlag(12);

        internal bool ShouldReadCustomFields => CheckFlag(13);

        internal bool ShouldReadChunkDataList => ShouldReadChunksGuid || ShouldReadChunksHash || ShouldReadChunksShaHash || ShoudlReadChunksGroupNumber || ShouldReadChunksWindowSize || ShouldReadChunksDownloadSize;

        internal bool ShoudReadFFileManifestList => ShouldReadFileFileName || ShouldReadFileSymLinkTarget || ShouldReadFileHash || ShouldReadFileMetaFlag || ShouldReadFileInstallTags;

        private void SetFlags(params bool[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == true)
                {
                    settingsFlag |= (ushort)(1 << i);
                }
            }
        }

        private bool CheckFlag(byte index) => ((settingsFlag >> index) & 1) != 0;
    }
}