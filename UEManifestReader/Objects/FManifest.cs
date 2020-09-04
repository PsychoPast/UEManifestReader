namespace UEManifestReader.Objects
{
    using System.Collections.Generic;
    using global::UEManifestReader.Enums;

    public class FManifest
    {
        /// <summary>
        /// The chunks subdir.
        /// </summary>
        public EChunkSubdir ChunkSubdir { get; internal set; }

        /// <summary>
        /// The files subdir.
        /// </summary>
        public EFileSubdir FileSubdir { get; internal set; }

        /// <summary>
        /// Manifest metadata.
        /// </summary>
        public FManifestMeta ManifestMeta { get; internal set; }

        /// <summary>
        /// The list of chunks.
        /// </summary>
        public List<FChunkInfo> ChunkList { get; internal set; }

        /// <summary>
        /// The list of files.
        /// </summary>
        public List<FFileManifest> FileList { get; internal set; }

        /// <summary>
        /// The custom fields.
        /// </summary>
        public Dictionary<string, string> CustomFields { get; internal set; }
    }
}