using System.Collections.Generic;
using System.IO;
using System.Linq;
using UEManifestReader.Enums;

namespace UEManifestReader.Objects
{
    public sealed class FManifestMeta
    {
        public FManifestMeta()
        {
        }

        public FManifestMeta(
            int version,
            bool isFileData,
            uint appId,
            string appName,
            string buildVersion,
            string launchExe,
            string launchCommand,
            List<string> prereqIds,
            string prereqName,
            string prereqPath,
            string prereqArgs,
            string buildId)
        {
            ManifestVersion = version;
            ManifestFeatureLevel = (EFeatureLevel)version;
            IsFileData = isFileData;
            AppId = appId;
            AppName = appName;
            BuildVersion = buildVersion;
            LaunchExe = launchExe;
            LaunchCommand = launchCommand;
            PrereqIds = prereqIds;
            PrereqName = prereqName;
            PrereqPath = prereqPath;
            PrereqArgs = prereqArgs;
            BuildId = buildId;
        }

        internal FManifestMeta(Stream reader, bool includeBuildId)
            : this(reader.ReadInt(),
                reader.ReadBool(),
                reader.ReadUInt(),
                reader.ReadFString(),
                reader.ReadFString(),
                reader.ReadFString(),
                reader.ReadFString(),
                reader.ReadTArray(reader.ReadFString).ToList(),
                reader.ReadFString(),
                reader.ReadFString(),
                reader.ReadFString(),
                default)
        {
            if (includeBuildId)
                BuildId = reader.ReadFString();
        }

        /// <summary>
        ///     The app id provided at generation.
        /// </summary>
        public uint AppId { get; set; }

        /// <summary>
        ///     The app name string provided at generation.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        ///     A unique build id generated at original chunking time to identify an exact build.
        /// </summary>
        public string BuildId { get; set; }

        /// <summary>
        ///     The build version string provided at generation.
        /// </summary>
        public string BuildVersion { get; set; }

        /// <summary>
        ///     Whether this is a legacy 'nochunks' build.
        /// </summary>
        public bool IsFileData { get; set; }

        /// <summary>
        ///     The command line required when launching the application executable.
        /// </summary>
        public string LaunchCommand { get; set; }

        /// <summary>
        ///     The file in this manifest designated the application executable of the build.
        /// </summary>
        public string LaunchExe { get; set; }

        /// <summary>
        ///     The feature level support this build was created with, regardless of the serialized format.
        /// </summary>
        public EFeatureLevel ManifestFeatureLevel { get; set; }

        /// <summary>
        ///     Manifest version.
        /// </summary>
        public int ManifestVersion { get; set; }

        /// <summary>
        ///     The command line required when launching the prerequisite installer.
        /// </summary>
        public string PrereqArgs { get; set; }

        /// <summary>
        ///     The set of prerequisite ids for dependencies that this build's prerequisite installer will apply.
        /// </summary>
        public List<string> PrereqIds { get; set; }

        /// <summary>
        ///     A display string for the prerequisite provided at generation.
        /// </summary>
        public string PrereqName { get; set; }

        /// <summary>
        ///     The file in this manifest designated the launch executable of the prerequisite installer.
        /// </summary>
        public string PrereqPath { get; set; }
    }
}