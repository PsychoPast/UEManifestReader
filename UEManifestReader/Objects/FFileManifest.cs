using System.Collections.Generic;
using System.Linq;
using UEManifestReader.Enums;

namespace UEManifestReader.Objects
{
    public sealed class FFileManifest
    {
        public FFileManifest()
        {
        }

        public FFileManifest(
            string fileName,
            string symLinkTarget,
            string fileHash,
            EFileMetaFlags? metaFlag,
            string[] installTags,
            FChunkPart[] chunkParts)
        {
            Filename = fileName;
            SymlinkTarget = symLinkTarget;
            FileHash = fileHash;
            FileMetaFlags = metaFlag;
            InstallTags = installTags?.ToList();
            ChunkParts = chunkParts?.ToList();
        }

        /// <summary>
        ///     The list of chunk parts to stitch.
        /// </summary>
        public List<FChunkPart> ChunkParts { get; set; }

        /// <summary>
        ///     The file SHA1.
        /// </summary>
        public string FileHash { get; set; }

        /// <summary>
        ///     The flags for this file.
        /// </summary>
        public EFileMetaFlags? FileMetaFlags { get; set; }

        /// <summary>
        ///     The build relative filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        ///     The install tags for this file.
        /// </summary>
        public List<string> InstallTags { get; set; }

        /// <summary>
        ///     Whether this is a symlink to another file.
        /// </summary>
        public string SymlinkTarget { get; set; }
    }
}