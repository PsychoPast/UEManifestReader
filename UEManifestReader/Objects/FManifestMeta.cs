using System.Collections.Generic;
using System.IO;
using System.Linq;
using UEManifestReader.Enums;

namespace UEManifestReader.Objects
{
    public sealed class FManifestMeta
    {
        internal FManifestMeta(Stream reader, bool includeBuildId)
        {
            int version = reader.ReadInt();
            ManifestVersion = version;
            ManifestFeatureLevel = (EFeatureLevel)version;
            IsFileData = reader.ReadBool();
            AppId = reader.ReadUInt();
            AppName = reader.ReadFString();
            BuildVersion = reader.ReadFString();
            LaunchExe = reader.ReadFString();
            LaunchCommand = reader.ReadFString();
            PrereqIds = reader.ReadTArray(reader.ReadFString).ToList();
            PrereqName = reader.ReadFString();
            PrereqPath = reader.ReadFString();
            PrereqArgs = reader.ReadFString();
            if (includeBuildId)
            {
                BuildId = reader.ReadFString();
            }
        }

        /// <summary>
        /// Manifest version. 
        /// </summary>
        public int ManifestVersion { get; }

        /// <summary>
        /// The feature level support this build was created with, regardless of the serialized format.
        /// </summary>
        public EFeatureLevel ManifestFeatureLevel { get; }

        /// <summary>
        /// Whether this is a legacy 'nochunks' build.
        /// </summary>
        public bool IsFileData { get; }

        /// <summary>
        /// The app id provided at generation.
        /// </summary>
        public uint AppId { get; }

        /// <summary>
        /// The app name string provided at generation.
        /// </summary>
        public string AppName { get; }

        /// <summary>
        /// The build version string provided at generation.
        /// </summary>
        public string BuildVersion { get; }

        /// <summary>
        /// The file in this manifest designated the application executable of the build.
        /// </summary>
        public string LaunchExe { get; }

        /// <summary>
        /// The command line required when launching the application executable.
        /// </summary>
        public string LaunchCommand { get; }

        /// <summary>
        /// The set of prerequisite ids for dependencies that this build's prerequisite installer will apply.
        /// </summary>
        public List<string> PrereqIds { get; }

        /// <summary>
        /// A display string for the prerequisite provided at generation.
        /// </summary>
        public string PrereqName { get; }

        /// <summary>
        /// The file in this manifest designated the launch executable of the prerequisite installer.
        /// </summary>
        public string PrereqPath { get; }

        /// <summary>
        /// The command line required when launching the prerequisite installer.
        /// </summary>
        public string PrereqArgs { get; }

        /// <summary>
        /// A unique build id generated at original chunking time to identify an exact build.
        /// </summary>
        public string BuildId { get; }
    }
}