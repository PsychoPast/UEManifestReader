using System.Runtime.CompilerServices;

namespace UEManifestReader
{
    public sealed class CustomManifestReadSettings
    {
        private ushort _settingsFlag;

        public CustomManifestReadSettings(
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
            bool readCustomFields = true) =>
            SetFlags(readManifestMeta,
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

        public static CustomManifestReadSettings ReadOnlyWhatIsNecessaryForDownload =>
            new(true,
                true,
                true,
                false,
                true,
                false,
                false,
                true,
                false,
                false,
                false,
                false);

        public bool ReadChunkDataList =>
            ReadChunksGuid || ReadChunksHash || ReadChunksShaHash || ReadChunksGroupNumber || ReadChunksWindowSize
         || ReadChunksDownloadSize;

        public bool ReadChunksDownloadSize => CheckFlag(6);

        public bool ReadChunksGroupNumber => CheckFlag(4);

        public bool ReadChunksGuid => CheckFlag(1);

        public bool ReadChunksHash => CheckFlag(2);

        public bool ReadChunksShaHash => CheckFlag(3);

        public bool ReadChunksWindowSize => CheckFlag(5);

        public bool ReadCustomFields => CheckFlag(13);

        public bool ReadFChunkPart => CheckFlag(12);

        public bool ReadFFileManifestList =>
            ReadFileFileName || ReadFileSymLinkTarget || ReadFileHash || ReadFileMetaFlag || ReadFChunkPart
         || ReadFileInstallTags;

        public bool ReadFileFileName => CheckFlag(7);

        public bool ReadFileHash => CheckFlag(9);

        public bool ReadFileInstallTags => CheckFlag(11);

        public bool ReadFileMetaFlag => CheckFlag(10);

        public bool ReadFileSymLinkTarget => CheckFlag(8);

        public bool ReadManifestMeta => CheckFlag(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckFlag(byte index) => ((_settingsFlag >> index) & 1) != 0;

        private void SetFlags(params bool[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i])
                    _settingsFlag |= (ushort)(1 << i);
            }
        }
    }
}