namespace UEManifestReader.Objects
{
    using System.Collections.Generic;
    using System.Linq;
    using global::UEManifestReader.Enums;

    public class FFileManifest
    {
        public FFileManifest(string fileName, string symLinkTarget, string fileHash, EFileMetaFlags? metaFlag, string[] installTags, FChunkPart[] chunkParts)
        {
            Filename = fileName;
            SymlinkTarget = symLinkTarget;
            FileHash = fileHash;
            FileMetaFlags = metaFlag;
            InstallTags = installTags?.ToList();
            ChunkParts = chunkParts?.ToList();
        }

        /// <summary>
        /// The build relative filename.
        /// </summary>
        public string Filename { get; internal set; }

        /// <summary>
        /// Whether this is a symlink to another file.
        /// </summary>
        public string SymlinkTarget { get; internal set; }

        /// <summary>
        /// The file SHA1.
        /// </summary>
        public string FileHash { get; internal set; }

        /// <summary>
        /// The flags for this file.
        /// </summary>
        public EFileMetaFlags? FileMetaFlags { get; internal set; }

        /// <summary>
        /// The install tags for this file.
        /// </summary>
        public List<string> InstallTags { get; internal set; }

        /// <summary>
        /// The list of chunk parts to stitch.
        /// </summary>
        public List<FChunkPart> ChunkParts { get; internal set; }
    }
}