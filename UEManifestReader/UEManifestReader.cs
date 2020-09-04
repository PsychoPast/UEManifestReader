namespace UEManifestReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::UEManifestReader.Enums;
    using global::UEManifestReader.Exceptions;
    using global::UEManifestReader.Objects;
    using Ionic.Zlib;

    public sealed class UEManifestReader : IDisposable
    {
        private const uint ManifestHeaderMagic = 0x44BEC00C;
        private readonly CustomManifestReadingSettings _readerSettings;
        private readonly Utf8JsonWriter jsonWriter;
        private readonly bool jsonGrouped;
        private readonly bool jsonSimplified;
        private readonly string tempFileBuffer;
        private readonly List<Action<FFileManifest>> jsonAction;
        private string tempFileName;
        private Stream reader;
        private FileStream fileHandle;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        public UEManifestReader(string file)
            : this(file, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings)
            : this(file, readSettings, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
            : this(file, readSettings, writeOutputToFileWhileReading, outputFileName, JsonOutputFormatFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        /// <param name="outputFormat">Json output format.</param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            fileHandle = File.OpenRead(file);
            reader = new BufferedStream(fileHandle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        public UEManifestReader(byte[] manifestData)
            : this(manifestData, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        public UEManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName)
            : this(manifestData, writeDataToTempFile, fileName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UEManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings)
            : this(manifestData, writeDataToTempFile, fileName, readSettings, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        public UEManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
            : this(manifestData, writeDataToTempFile, fileName, readSettings, writeOutputToFileWhileReading, outputFileName, JsonOutputFormatFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        /// <param name="outputFormat">Json output format.</param>
        public UEManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat)
        {
            if (writeDataToTempFile)
            {
                File.WriteAllBytes(fileName, manifestData);
                fileHandle = File.OpenRead(fileName);
                reader = new BufferedStream(fileHandle);
                tempFileBuffer = fileName;
            }

            reader = new MemoryStream(manifestData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        public UEManifestReader(Stream stream)
            : this(stream, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings)
            : this(stream, readSettings, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
            : this(stream, readSettings, writeOutputToFileWhileReading, outputFileName, JsonOutputFormatFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        /// <param name="outputFormat">Json output format.</param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat) => reader = stream;

        /// <summary>
        /// Downloads the manifest data from the specified <paramref name="manifestUrl"/> and initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestUrl">The Url of the manifest file to download.</param>
        /// <param name="manifestStorage">The location where to save the download manifest.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="manifestStorage"/> is <see cref="ManifestStorage.Memory"/>.</param>
        public UEManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName)
            : this(manifestUrl, manifestStorage, fileName, null)
        {
        }

        /// <summary>
        /// Downloads the manifest data from the specified <paramref name="manifestUrl"/> and initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestUrl">The Url of the manifest file to download.</param>
        /// <param name="manifestStorage">The location where to save the download manifest.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="manifestStorage"/> is <see cref="ManifestStorage.Memory"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UEManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings)
            : this(manifestUrl, manifestStorage, fileName, readSettings, false, null)
        {
        }

        /// <summary>
        /// Downloads the manifest data from the specified <paramref name="manifestUrl"/> and initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestUrl">The Url of the manifest file to download.</param>
        /// <param name="manifestStorage">The location where to save the download manifest.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="manifestStorage"/> is <see cref="ManifestStorage.Memory"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        public UEManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
            : this(manifestUrl, manifestStorage, fileName, readSettings, writeOutputToFileWhileReading, outputFileName, JsonOutputFormatFlags.Default)
        {
        }

        /// <summary>
        /// Downloads the manifest data from the specified <paramref name="manifestUrl"/> and initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestUrl">The Url of the manifest file to download.</param>
        /// <param name="manifestStorage">The location where to save the download manifest.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="manifestStorage"/> is <see cref="ManifestStorage.Memory"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        /// <param name="writeOutputToFileWhileReading">If <see langword="true"/>, output is gonna be written to file while the manifest file is being read using default json output format.</param>
        /// <param name="outputFileName">File name of the file to which the json output is written. Can be <see langword="null"/> if <paramref name="writeOutputToFileWhileReading"/> is <see langword="false"/>.</param>
        /// <param name="outputFormat">Json output format.</param>
        public UEManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat)
        {
            byte[] data;
            using WebClient wClient = new();
            {
                data = wClient.DownloadData(manifestUrl);
            }

            switch (manifestStorage)
            {
                case ManifestStorage.Disk:
                    File.WriteAllBytes(fileName, data);
                    fileHandle = File.OpenRead(fileName);
                    reader = new BufferedStream(fileHandle);
                    tempFileBuffer = fileName;
                    break;
                case ManifestStorage.Memory:
                    reader = new MemoryStream(data);
                    break;
                default:
                    throw new ArgumentException(nameof(manifestStorage));
            }
        }

        private UEManifestReader(CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
        {
            _readerSettings = readSettings ?? new CustomManifestReadingSettings();
            if (writeOutputToFileWhileReading)
            {
                jsonWriter = new Utf8JsonWriter(File.Open(outputFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), new JsonWriterOptions()
                {
                    Indented = (outputFormat & JsonOutputFormatFlags.Indented) != 0,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                });

                jsonWriter.WriteStartObject();
            }

            jsonGrouped = (outputFormat & JsonOutputFormatFlags.Grouped) != 0;
            jsonSimplified = (outputFormat & JsonOutputFormatFlags.Simplified) != 0;
            Manifest = new ();
            jsonAction = new(6);
        }

        public FManifest Manifest { get; }

        /// <summary>
        /// Read the manifest.
        /// </summary>
        /// <param name="tempManifestDataStorage">The location of the modified manifest data if the data can't be processed directly.</param>
        /// <exception cref="UEManifestReaderException">Throws if an error happens while reading the manifest.</exception>
        public void ReadManifest(ManifestStorage tempManifestDataStorage = ManifestStorage.Memory)
        {
            ReadFManifestHeader(tempManifestDataStorage);
            ReadFManifestMeta();
            ReadFChunkDataList();
            ReadFFileManifest();
            ReadFCustomFields();
            if (jsonWriter != null)
            {
                jsonWriter.WriteEndObject();
                jsonWriter.Flush();
            }
        }

        private void ReadFManifestHeader(ManifestStorage tempManifestDataStorage)
        {
            uint magic = reader.ReadUInt();
            if (magic != ManifestHeaderMagic)
            {
                throw new UEManifestReaderException("[Magic mismatch] The following file is not a UE Manifest file!");
            }

            uint headerSize = reader.ReadUInt();
            uint dataSizeUncompressed = reader.ReadUInt();
            uint dataSizeCompressed = reader.ReadUInt();
            byte[] sHAHash = reader.ReadBytes(20);
            EManifestStorageFlags storedAs = (EManifestStorageFlags)reader.ReadByte();
            EFeatureLevel version = headerSize > 37 ? (EFeatureLevel)reader.ReadInt() : EFeatureLevel.StoredAsCompressedUClass;

            if (version < EFeatureLevel.StoredAsBinaryData)
            {
                throw new UEManifestReaderException($"Manifest version {version} is not supported for the time being!", new NotSupportedException());
            }

            if ((storedAs & EManifestStorageFlags.Compressed) == EManifestStorageFlags.Compressed)
            {
                GetDataAndCheckHash(new ZlibStream(reader, CompressionMode.Decompress), (int)dataSizeUncompressed, sHAHash, tempManifestDataStorage);
            }
            else
            {
                GetDataAndCheckHash(reader, (int)dataSizeCompressed, sHAHash, tempManifestDataStorage);
            }
        }

        private void ReadFManifestMeta()
        {
            uint dataSize = reader.ReadUInt(); // size of FManifestMeta data
            if (!_readerSettings.ShouldReadManifestMeta)
            {
                reader.Position += dataSize - sizeof(uint);
                return;
            }

            EManifestMetaVersion dataVersion = (EManifestMetaVersion)reader.ReadByte();
            bool serializeBuildId = dataVersion >= EManifestMetaVersion.SerialisesBuildId;
            FManifestMeta manifestMeta = new(reader, serializeBuildId);

            EFeatureLevel version = (EFeatureLevel)manifestMeta.ManifestVersion;

            EChunkSubdir chunkSubdir = Manifest.ChunkSubdir = version < EFeatureLevel.DataFileRenames ? EChunkSubdir.Chunks
                : version < EFeatureLevel.ChunkCompressionSupport ? EChunkSubdir.ChunksV2
                : version < EFeatureLevel.VariableSizeChunksWithoutWindowSizeChunkInfo ? EChunkSubdir.ChunksV3
                : EChunkSubdir.ChunksV4;

            EFileSubdir fileSubdir = Manifest.FileSubdir = version < EFeatureLevel.DataFileRenames ? EFileSubdir.Files
                : version < EFeatureLevel.StoresChunkDataShaHashes ? EFileSubdir.FilesV2
                : EFileSubdir.FilesV3;

            Manifest.ManifestMeta = manifestMeta;

            if (jsonWriter == null || jsonSimplified)
            {
                return;
            }

            jsonWriter.WriteNumber("ManifestVersion", manifestMeta.ManifestVersion);
            jsonWriter.WriteBoolean("bIsFileData", manifestMeta.IsFileData);
            jsonWriter.WriteString("ChunkSubdir", chunkSubdir.ToString());
            jsonWriter.WriteString("FileSubdir", fileSubdir.ToString());
            jsonWriter.WriteNumber("AppId", manifestMeta.AppId);
            jsonWriter.WriteString("AppName", manifestMeta.AppName);
            jsonWriter.WriteString("BuildVersion", manifestMeta.BuildVersion);
            jsonWriter.WriteString("LaunchExe", manifestMeta.LaunchExe);
            jsonWriter.WriteString("LaunchCommand", manifestMeta.LaunchCommand);
            List<string> prereq = manifestMeta.PrereqIds;
            jsonWriter.WriteStartArray("PrereqIds");
            for (int i = 0; i < prereq.Count; i++)
            {
                jsonWriter.WriteStringValue(prereq[i]);
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteString("PrereqName", manifestMeta.PrereqName);
            jsonWriter.WriteString("PrereqPath", manifestMeta.PrereqPath);
            jsonWriter.WriteString("PrereqArgs", manifestMeta.PrereqArgs);
            if (serializeBuildId)
            {
                jsonWriter.WriteString("BuildId", reader.ReadFString());
            }

            jsonWriter.Flush();
        }

        private void ReadFChunkDataList()
        {
            uint dataSize = reader.ReadUInt(); // size of FChunkDataList data
            if (!_readerSettings.ShouldReadChunkDataList)
            {
                reader.Position += dataSize - sizeof(uint);
                return;
            }

            EChunkDataListVersion dataVersion = (EChunkDataListVersion)reader.ReadByte();
            int elementCount = reader.ReadInt();
            string[] guids = null;
            if (_readerSettings.ShouldReadChunksGuid)
            {
                guids = reader.ReadArray(elementCount, () => new FGuid(reader).ToString());
            }
            else
            {
                reader.Position += elementCount * 16; // sizeof(FGuid) = 16 bytes
            }

            string[] hashes = null;
            if (_readerSettings.ShouldReadChunksHash)
            {
                hashes = reader.ReadArray(elementCount, () => Utilities.ULongToHexHash(reader.ReadULong()));

                jsonAction.Add((_) =>
                {
                    jsonWriter.WriteStartObject("ChunksHashList");
                    for (int i = 0; i < elementCount; i++)
                    {
                        jsonWriter.WriteString(guids[i], hashes[i]);
                    }

                    jsonWriter.WriteEndObject();
                });
            }
            else
            {
                reader.Position += elementCount * sizeof(ulong);  // in the manifest, hashes are stored as ulong values (8 bytes) but they are converted to hex string during parsing process in order for them to be usable
            }

            string[] shaHashes = null;
            if (_readerSettings.ShouldReadChunksShaHash)
            {
                shaHashes = reader.ReadArray(elementCount, () => Utilities.BytesToHexadecimalString(reader.ReadBytes(20)));

                if (!jsonSimplified)
                {
                    jsonAction.Add((_) =>
                    {
                        jsonWriter.WriteStartObject("ChunksShaHashList");
                        for (int i = 0; i < elementCount; i++)
                        {
                            jsonWriter.WriteString(guids[i], shaHashes[i]);
                        }

                        jsonWriter.WriteEndObject();
                    });
                }
            }
            else
            {
                reader.Position += elementCount * 20; // sizeof(SHA1 hash) = 20 bytes
            }

            string[] groupNumbers = null;
            if (_readerSettings.ShoudlReadChunksGroupNumber)
            {
                groupNumbers = reader.ReadArray(elementCount, () => $"{reader.ReadByte():D2}");

                jsonAction.Add((_) =>
                {
                    jsonWriter.WriteStartObject("ChunksGroupNumberList");
                    for (int i = 0; i < elementCount; i++)
                    {
                        jsonWriter.WriteString(guids[i], groupNumbers[i]);
                    }

                    jsonWriter.WriteEndObject();
                });
            }
            else
            {
                reader.Position += elementCount; // * sizeof(byte)  // group numbers are stored as byte (1 byte) in the manifests but they are converted to string during parsing process in order for them to be usable
            }

            uint? windowSize = null;
            if (_readerSettings.ShouldReadChunksWindowSize)
            {
                windowSize = 1048576; // this value seems to be a const for now

                if (!jsonSimplified)
                {
                    jsonAction.Add((_) =>
                    {
                        jsonWriter.WriteStartObject("ChunksWindowSizeList");
                        for (int i = 0; i < elementCount; i++)
                        {
                            jsonWriter.WriteNumber(guids[i], (uint)windowSize);
                        }

                        jsonWriter.WriteEndObject();
                    });
                }
            }

            /*uint?[] windowSize = null;
            if(_readerSettings.ShouldReadChunksWindowSize) It seems windowSize is a const = 1048576
            {
                windowSize = reader.ReadArray(elementCount, () => (uint?)reader.ReadUInt());
            }
            else
            {*/
            reader.Position += elementCount * sizeof(uint); // we don't need to read window sizes anymore so we skip them

            // }
            long?[] filesSize = null;
            if (_readerSettings.ShouldReadChunksDownloadSize)
            {
                filesSize = reader.ReadArray(elementCount, () => (long?)reader.ReadLong());

                if (!jsonSimplified)
                {
                    jsonAction.Add((_) =>
                    {
                        jsonWriter.WriteStartObject("ChunksFileSizeList");
                        for (int i = 0; i < elementCount; i++)
                        {
                            jsonWriter.WriteNumber(guids[i], (long)filesSize[i]);
                        }

                        jsonWriter.WriteEndObject();
                    });
                }
            }
            else
            {
                reader.Position += elementCount * sizeof(long); // fileSize are long values (8 bytes)
            }

            List<FChunkInfo> chunkInfos = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                chunkInfos.Add(new FChunkInfo(guids?[i], hashes?[i], shaHashes?[i], groupNumbers?[i], windowSize, filesSize?[i]));
            }

            Manifest.ChunkList = chunkInfos;

            if (jsonWriter is null || jsonGrouped)
            {
                jsonAction.Clear();
                return;
            }

            jsonWriter.WriteStartObject("ChunksList");
            for (int i = 0; i < jsonAction.Count; i++)
            {
                jsonAction[i](null);
            }

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
            jsonAction.Clear();
        }

        private void ReadFFileManifest()
        {
            uint dataSize = reader.ReadUInt(); // size of FFileManifest data
            if (!_readerSettings.ShoudReadFFileManifestList)
            {
                reader.Position += dataSize - sizeof(uint);
                return;
            }

            EFileManifestListVersion dataVersion = (EFileManifestListVersion)reader.ReadByte();
            int elementCount = reader.ReadInt();

            string[] fileNames = null;
            if (_readerSettings.ShouldReadFileFileName)
            {
                fileNames = reader.ReadArray(elementCount, () => reader.ReadFString());

                jsonAction.Add((fileManifest) =>
                {
                    jsonWriter.WriteString("Filename", fileManifest.Filename);
                });
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    reader.SkipFString();
                }
            }

            string[] symLinkTargets = null;
            if (_readerSettings.ShouldReadFileSymLinkTarget)
            {
                symLinkTargets = reader.ReadArray(elementCount, () => reader.ReadFString());

                if (!jsonSimplified)
                {
                    jsonAction.Add((fileManifest) =>
                    {
                        jsonWriter.WriteString("FileSymlinkTarget", fileManifest.SymlinkTarget);
                    });
                }
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    reader.SkipFString();
                }
            }

            string[] filesHash = null;
            if (_readerSettings.ShouldReadFileHash)
            {
                filesHash = reader.ReadArray(elementCount, () => Utilities.BytesToHexadecimalString(reader.ReadBytes(20)));

                if (!jsonSimplified)
                {
                    jsonAction.Add((fileManifest) =>
                    {
                        jsonWriter.WriteString("FileHash", fileManifest.FileHash);
                    });
                }
            }
            else
            {
                reader.Position += elementCount * 20; // sizeof(SHA1 hash) = 20 bytes
            }

            EFileMetaFlags?[] filesMetaTags = null;
            if (_readerSettings.ShouldReadFileMetaFlag)
            {
                filesMetaTags = reader.ReadArray(elementCount, () => (EFileMetaFlags?)reader.ReadByte());

                if (!jsonSimplified)
                {
                    jsonAction.Add((fileManifest) =>
                    {
                        jsonWriter.WriteString("FileMetaFlag", fileManifest.FileMetaFlags.ToString());
                    });
                }
            }
            else
            {
                reader.Position += elementCount; // * 1   EFileMetaFlags underlying type is byte (1 byte)
            }

            string[][] installTags = null;
            if (_readerSettings.ShouldReadFileInstallTags)
            {
                installTags = reader.ReadArray(elementCount, () => reader.ReadTArray(() => reader.ReadFString()));

                if (!jsonSimplified)
                {
                    jsonAction.Add((fileManifest) =>
                    {
                        jsonWriter.WriteStartArray("InstallTags");

                        for (int j = 0; j < fileManifest.InstallTags.Count; j++)
                        {
                            jsonWriter.WriteStringValue(fileManifest.InstallTags[j]);
                        }

                        jsonWriter.WriteEndArray();
                    });
                }
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    int num = reader.ReadInt();
                    for (int j = 0; j < num; j++)
                    {
                        reader.SkipFString();
                    }
                }
            }

            FChunkPart[][] chunksParts = null;
            if (_readerSettings.ShouldReadFChunkPart)
            {
                chunksParts = reader.ReadArray(elementCount, () => reader.ReadTArray(() => new FChunkPart(reader)));

                Action<FFileManifest> ac;
                if (!jsonGrouped)
                {
                    ac = (fileManifest) =>
                    {
                        jsonWriter.WriteStartArray("FileChunkParts");
                        for (int k = 0; k < fileManifest.ChunkParts.Count; k++)
                        {
                            FChunkPart chunkPart = fileManifest.ChunkParts[k];
                            jsonWriter.WriteStartObject();
                            jsonWriter.WriteString("Guid", chunkPart.Guid);
                            jsonWriter.WriteNumber("Offset", chunkPart.Offset);
                            jsonWriter.WriteNumber("Size", chunkPart.Size);
                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();
                    };
                }
                else
                {
                    CreateLookUp(out Dictionary<string, string> hashesLookup, out Dictionary<string, string> datagroupLookup);

                    ac = (fileManifest) =>
                    {
                        jsonWriter.WriteStartArray("FileChunkParts");
                        for (int k = 0; k < fileManifest.ChunkParts.Count; k++)
                        {
                            FChunkPart chunkPart = fileManifest.ChunkParts[k];
                            jsonWriter.WriteStartObject();
                            jsonWriter.WriteString("Guid", chunkPart.Guid);
                            jsonWriter.WriteNumber("Offset", chunkPart.Offset);
                            jsonWriter.WriteNumber("Size", chunkPart.Size);
                            jsonWriter.WriteString("Hash", hashesLookup[chunkPart.Guid]);
                            jsonWriter.WriteString("GroupNumber", datagroupLookup[chunkPart.Guid]);
                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();
                    };
                }

                jsonAction.Add(ac);
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    int num = reader.ReadInt();
                    for (int j = 0; j < num; j++)
                    {
                        reader.Position += 28; // fchunkpart struct size
                    }
                }
            }

            List<FFileManifest> fFileManifest = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                fFileManifest.Add(new FFileManifest(fileNames?[i], symLinkTargets?[i], filesHash?[i], filesMetaTags?[i], installTags?[i], chunksParts?[i]));
            }

            Manifest.FileList = fFileManifest;

            if (jsonWriter == null)
            {
                return;
            }

            jsonWriter.WriteStartArray("FileManifestList");
            for (int i = 0; i < fFileManifest.Count; i++)
            {
                jsonWriter.WriteStartObject();
                FFileManifest fileManifest = fFileManifest[i];

                for (int j = 0; j < jsonAction.Count; j++)
                {
                    jsonAction[j](fileManifest);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray();
        }

        private void ReadFCustomFields()
        {
            uint dataSize = reader.ReadUInt(); // size of FCustomFields data
            if (!_readerSettings.ShouldReadCustomFields)
            {
                reader.Position += dataSize - sizeof(uint);
                return;
            }

            EChunkDataListVersion dataVersion = (EChunkDataListVersion)reader.ReadByte();
            int elementCount = reader.ReadInt();
            string[] keys = reader.ReadArray(elementCount, () => reader.ReadFString());
            string[] values = reader.ReadArray(elementCount, () => reader.ReadFString());
            Manifest.CustomFields = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                Manifest.CustomFields.Add(keys[i], values[i]);
            }

            if (jsonWriter == null)
            {
                return;
            }

            jsonWriter.WriteStartObject("CustomFields");
            for (int i = 0; i < elementCount; i++)
            {
                jsonWriter.WriteString(keys[i], values[i]);
            }

            jsonWriter.WriteEndObject();
        }

        private void GetDataAndCheckHash(Stream stream, int size, byte[] expectedHash, ManifestStorage tempManifestDataStorage)
        {
            byte[] buffer = new byte[size];
            using Stream st = stream;
            {
                st.Read(buffer);
            }

            byte[] hash;
            using SHA1 sHA1 = SHA1.Create();
            {
                hash = sHA1.ComputeHash(buffer);
            }

            if (expectedHash.Length != hash.Length)
            {
                throw new UEManifestReaderException("[Hash mismatch] The archive is corrupted!");
            }

            unsafe
            {
                fixed (byte* pEHash = expectedHash, pHash = hash)
                {
                    byte* eHash = pEHash, bHash = pHash;
                    for (int i = 0; i < expectedHash.Length; i++)
                    {
                        if (*eHash++ != *bHash++)
                        {
                            throw new UEManifestReaderException("[Hash mismatch] The archive is corrupted!");
                        }
                    }
                }
            }

            if (stream is ZlibStream)
            {
                fileHandle.DisposeAsync();
                reader.DisposeAsync();
                if (tempManifestDataStorage == ManifestStorage.Disk)
                {
                    tempFileName = $"{Environment.TickCount64}.tmp"; // in order to have a random temp file name
                    File.WriteAllBytes(tempFileName, buffer);
                    fileHandle = File.OpenRead(tempFileName);
                    reader = new BufferedStream(fileHandle);
                }
                else
                {
                    reader = new MemoryStream(buffer);
                }
            }
        }

        private void CreateLookUp(out Dictionary<string, string> hashesLookup, out Dictionary<string, string> datagroupsLookup)
        {
            int cap = Manifest.ChunkList.Count;
            Dictionary<string, string> haLookup = new(cap);
            Dictionary<string, string> dgLookup = new(cap);
            Task hashLookup = Task.Run(() =>
            {
                for (int i = 0; i < cap; i++)
                {
                    FChunkInfo chunkInfo = Manifest.ChunkList[i];
                    haLookup.Add(chunkInfo.Guid, chunkInfo.Hash);
                }
            });

            Task datagroupLookup = Task.Run(() =>
            {
                for (int i = 0; i < cap; i++)
                {
                    FChunkInfo chunkInfo = Manifest.ChunkList[i];
                    dgLookup.Add(chunkInfo.Guid, chunkInfo.GroupNumber);
                }
            });
            Task.WhenAll(hashLookup, datagroupLookup).GetAwaiter().GetResult();

            hashesLookup = haLookup;
            datagroupsLookup = dgLookup;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    jsonWriter?.Dispose();
                    reader.Dispose();
                    fileHandle?.Close();
                    if (File.Exists(tempFileName))
                    {
                        File.Delete(tempFileName);
                    }

                    if (File.Exists(tempFileBuffer))
                    {
                        File.Delete(tempFileBuffer);
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}