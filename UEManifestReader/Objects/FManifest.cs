using System.Collections.Generic;
using UEManifestReader.Enums;

namespace UEManifestReader.Objects
{
    public sealed class FManifest
    {
        /// <summary>
        ///     Base URLs for downloading chunks.
        /// </summary>
        public List<string> BaseUrls { get; set; }

        /// <summary>
        ///     The list of chunks.
        /// </summary>
        public List<FChunkInfo> ChunkList { get; set; }

        /// <summary>
        ///     The chunks subdir.
        /// </summary>
        public EChunkSubdir ChunkSubdir { get; set; }

        /// <summary>
        ///     The custom fields.
        /// </summary>
        public Dictionary<string, string> CustomFields { get; set; }

        /// <summary>
        ///     The list of files.
        /// </summary>
        public List<FFileManifest> FileList { get; set; }

        /// <summary>
        ///     The files subdir.
        /// </summary>
        public EFileSubdir FileSubdir { get; set; }

        /// <summary>
        ///     Manifest metadata.
        /// </summary>
        public FManifestMeta ManifestMeta { get; set; }
    }
}