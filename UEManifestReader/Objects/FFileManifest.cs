using System.Collections.Generic;
using System.Linq;
using UEManifestReader.Enums;

namespace UEManifestReader.Objects
{
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
        public string Filename { get; }

        /// <summary>
        /// Whether this is a symlink to another file.
        /// </summary>
        public string SymlinkTarget { get; }

        /// <summary>
        /// The file SHA1.
        /// </summary>
        public string FileHash { get; }

        /// <summary>
        /// The flags for this file.
        /// </summary>
        public EFileMetaFlags? FileMetaFlags { get;  }

        /// <summary>
        /// The install tags for this file.
        /// </summary>
        public List<string> InstallTags { get; }

        /// <summary>
        /// The list of chunk parts to stitch.
        /// </summary>
        public List<FChunkPart> ChunkParts { get; }
    }
}