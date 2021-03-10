using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using UEManifestReader.Enums;
using UEManifestReader.Exceptions;
using UEManifestReader.Objects;
using Ionic.Zlib;
using System.Linq;

namespace UEManifestReader
{
    public sealed class UESerializedManifestReader : IManifestReader, IDisposable
    {
        private const uint ManifestHeaderMagic = 0x44BEC00C;
        private readonly CustomManifestReadSettings _readerSettings;
        private readonly Utf8JsonWriter _jsonWriter;
        private readonly bool _jsonGrouped;
        private readonly bool _jsonSimplified;
        private readonly string _tempFileBuffer;
        private readonly List<Action<FFileManifest>> _jsonAction;
        private string _tempFileName;
        private Stream _reader;
        private FileStream _fileHandle;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        public UESerializedManifestReader(string file)
            : this(file, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="file">Path to the file to read.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UESerializedManifestReader(string file, CustomManifestReadSettings readSettings)
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
        public UESerializedManifestReader(string file, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
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
        public UESerializedManifestReader(string file, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            _fileHandle = File.OpenRead(file);
            _reader = new BufferedStream(_fileHandle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        public UESerializedManifestReader(byte[] manifestData)
            : this(manifestData, false,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        public UESerializedManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName)
            : this(manifestData, writeDataToTempFile, fileName,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestData">Manifest content data.</param>
        /// <param name="writeDataToTempFile">If <see langword="true"/>, write the manifest data to a file and read it from it, else read the content from memory.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="writeDataToTempFile"/> is <see langword="false"/>.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UESerializedManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadSettings readSettings)
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
        public UESerializedManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
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
        public UESerializedManifestReader(byte[] manifestData, bool writeDataToTempFile, string fileName, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat)
        {
            if (writeDataToTempFile)
            {
                File.WriteAllBytes(fileName, manifestData);
                _fileHandle = File.OpenRead(fileName);
                _reader = new BufferedStream(_fileHandle);
                _tempFileBuffer = fileName;
            }

            _reader = new MemoryStream(manifestData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        public UESerializedManifestReader(Stream stream)
            : this(stream, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the manifest content.</param>
        /// <param name="readSettings">Manifest reading settings.</param>
        public UESerializedManifestReader(Stream stream, CustomManifestReadSettings readSettings)
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
        public UESerializedManifestReader(Stream stream, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
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
        public UESerializedManifestReader(Stream stream, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFileName, outputFormat) => _reader = stream;

        /// <summary>
        /// Downloads the manifest data from the specified <paramref name="manifestUrl"/> and initializes a new instance of the <see cref="UEManifestReader"/> class.
        /// </summary>
        /// <param name="manifestUrl">The Url of the manifest file to download.</param>
        /// <param name="manifestStorage">The location where to save the download manifest.</param>
        /// <param name="fileName">Name of the file to write the data to. Can be <see langword="null"/> if <paramref name="manifestStorage"/> is <see cref="ManifestStorage.Memory"/>.</param>
        public UESerializedManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName)
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
        public UESerializedManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadSettings readSettings)
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
        public UESerializedManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName)
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
        public UESerializedManifestReader(Uri manifestUrl, ManifestStorage manifestStorage, string fileName, CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
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
                    _fileHandle = File.OpenRead(fileName);
                    _reader = new BufferedStream(_fileHandle);
                    _tempFileBuffer = fileName;
                    break;
                case ManifestStorage.Memory:
                    _reader = new MemoryStream(data);
                    break;
                default:
                    throw new ArgumentException(nameof(manifestStorage));
            }
        }

        private UESerializedManifestReader(CustomManifestReadSettings readSettings, bool writeOutputToFileWhileReading, string outputFileName, JsonOutputFormatFlags outputFormat)
        {
            _readerSettings = readSettings ?? new CustomManifestReadSettings();
            if (writeOutputToFileWhileReading)
            {
                _jsonWriter = new Utf8JsonWriter(File.Open(outputFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), new JsonWriterOptions()
                {
                    Indented = (outputFormat & JsonOutputFormatFlags.Indented) != 0,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                });

                _jsonWriter.WriteStartObject();
            }

            _jsonGrouped = (outputFormat & JsonOutputFormatFlags.Grouped) != 0;
            _jsonSimplified = (outputFormat & JsonOutputFormatFlags.Simplified) != 0;
            WriteOutputToFile = writeOutputToFileWhileReading;
            OutputFormat = outputFormat;
            ReadingSettings = readSettings;
            Manifest = new();
            _jsonAction = new(6);
        }

        /// <inheritdoc/>
        public FManifest Manifest { get; }

        /// <summary>
        /// Manifest reading settings.
        /// </summary>
        public CustomManifestReadSettings ReadingSettings { get; }

        /// <summary>
        /// If <see langword="true"/>, writes parsed manifest to file.
        /// </summary>
        public bool WriteOutputToFile { get; }

        /// <summary>
        /// Json output format.
        /// </summary>
        public JsonOutputFormatFlags OutputFormat { get; }

        /// <inheritdoc/>
        public void ReadManifest() => ReadManifest(ManifestStorage.Memory);

        /// <inheritdoc/>
        public void ReadManifest(ManifestStorage tempManifestDataStorage)
        {
            ReadFManifestHeader(tempManifestDataStorage);
            ExecuteReadIfTrue(_readerSettings.ReadManifestMeta, ReadFManifestMeta);
            ExecuteReadIfTrue(_readerSettings.ReadChunkDataList, ReadFChunkDataList);
            ExecuteReadIfTrue(_readerSettings.ReadFFileManifestList, ReadFFileManifest);
            ExecuteReadIfTrue(_readerSettings.ReadCustomFields, ReadFCustomFields);
            if (_jsonWriter != null)
            {
                _jsonWriter.WriteEndObject();
                _jsonWriter.Flush();
            }

            if (File.Exists(_tempFileName))
            {
                File.Delete(_tempFileName);
            }

            if (File.Exists(_tempFileBuffer))
            {
                File.Delete(_tempFileBuffer);
            }
        }

        private void ReadFManifestHeader(ManifestStorage tempManifestDataStorage)
        {
            uint magic = _reader.ReadUInt();
            if (magic != ManifestHeaderMagic)
            {
                throw new UEManifestReaderException("[Magic mismatch] The following file is not a UE Manifest file!");
            }

            uint headerSize = _reader.ReadUInt();
            uint dataSizeUncompressed = _reader.ReadUInt();
            uint dataSizeCompressed = _reader.ReadUInt();
            byte[] sHAHash = _reader.ReadBytes(20);
            EManifestStorageFlags storedAs = (EManifestStorageFlags)_reader.ReadByte();
            EFeatureLevel version = headerSize > 37 ? (EFeatureLevel)_reader.ReadInt() : EFeatureLevel.StoredAsCompressedUClass;
            if (version < EFeatureLevel.StoredAsBinaryData)
            {
                throw new UEManifestReaderException($"Manifest version {version} is not supported for the time being!", new NotSupportedException());
            }

            if ((storedAs & EManifestStorageFlags.Compressed) == EManifestStorageFlags.Compressed)
            {
                GetDataAndCheckHash(new ZlibStream(_reader, CompressionMode.Decompress), (int)dataSizeUncompressed, sHAHash, tempManifestDataStorage);
            }
            else
            {
                GetDataAndCheckHash(_reader, (int)dataSizeCompressed, sHAHash, tempManifestDataStorage);
            }
        }

        private void ReadFManifestMeta()
        {
            EManifestMetaVersion dataVersion = (EManifestMetaVersion)_reader.ReadByte();
            bool serializeBuildId = dataVersion >= EManifestMetaVersion.SerialisesBuildId;
            FManifestMeta manifestMeta = new(_reader, serializeBuildId);
            EFeatureLevel version = (EFeatureLevel)manifestMeta.ManifestVersion;
            EChunkSubdir chunkSubdir = Manifest.ChunkSubdir = version < EFeatureLevel.DataFileRenames ? EChunkSubdir.Chunks
                                                              : version < EFeatureLevel.ChunkCompressionSupport ? EChunkSubdir.ChunksV2
                                                              : version < EFeatureLevel.VariableSizeChunksWithoutWindowSizeChunkInfo ? EChunkSubdir.ChunksV3
                                                              : EChunkSubdir.ChunksV4;

            EFileSubdir fileSubdir = Manifest.FileSubdir = version < EFeatureLevel.DataFileRenames ? EFileSubdir.Files
                                                           : version < EFeatureLevel.StoresChunkDataShaHashes ? EFileSubdir.FilesV2
                                                           : EFileSubdir.FilesV3;
            Manifest.ManifestMeta = manifestMeta;
            if (_jsonWriter == null || _jsonSimplified)
            {
                return;
            }

            _jsonWriter.WriteNumber("ManifestVersion", manifestMeta.ManifestVersion);
            _jsonWriter.WriteBoolean("bIsFileData", manifestMeta.IsFileData);
            _jsonWriter.WriteString("ChunkSubdir", chunkSubdir.ToString());
            _jsonWriter.WriteString("FileSubdir", fileSubdir.ToString());
            _jsonWriter.WriteNumber("AppId", manifestMeta.AppId);
            _jsonWriter.WriteString("AppName", manifestMeta.AppName);
            _jsonWriter.WriteString("BuildVersion", manifestMeta.BuildVersion);
            _jsonWriter.WriteString("LaunchExe", manifestMeta.LaunchExe);
            _jsonWriter.WriteString("LaunchCommand", manifestMeta.LaunchCommand);
            List<string> prereq = manifestMeta.PrereqIds;
            _jsonWriter.WriteStartArray("PrereqIds");
            for (int i = 0; i < prereq.Count; i++)
            {
                _jsonWriter.WriteStringValue(prereq[i]);
            }

            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteString("PrereqName", manifestMeta.PrereqName);
            _jsonWriter.WriteString("PrereqPath", manifestMeta.PrereqPath);
            _jsonWriter.WriteString("PrereqArgs", manifestMeta.PrereqArgs);
            if (serializeBuildId)
            {
                _jsonWriter.WriteString("BuildId", _reader.ReadFString());
            }

            _jsonWriter.Flush();
        }

        private void ReadFChunkDataList()
        {
            _ = /*(EChunkDataListVersion)*/_reader.ReadByte(); //no need to verify the version. The authenticity of the file is asserted when comparing the hashes
            int elementCount = _reader.ReadInt();
            string[] guids = null;
            Action<string, string[]> jsonGuidPropertiesActionString;
            Action<string, uint?[]> jsonGuidPropertyActionNumber;
            if (_readerSettings.ReadChunksGuid)
            {
                guids = _reader.ReadArray(elementCount, () => new FGuid(_reader).ToString());
                jsonGuidPropertiesActionString = (propertyName, value) =>
                {
                    _jsonWriter.WriteStartObject(propertyName);
                    for (int i = 0; i < elementCount; i++)
                    {
                        _jsonWriter.WriteString(guids[i], value[i]);
                    }

                    _jsonWriter.WriteEndObject();
                };

                jsonGuidPropertyActionNumber = (propertyName, value) =>
                {
                    _jsonWriter.WriteStartObject(propertyName);
                    for (int i = 0; i < elementCount; i++)
                    {
                        _jsonWriter.WriteNumber(guids[i], (uint)(value[i]));
                    }

                    _jsonWriter.WriteEndObject();
                };
            }
            else
            {
                jsonGuidPropertiesActionString = (propertyName, value) =>
                {
                    _jsonWriter.WriteStartArray(propertyName);
                    for (int i = 0; i < elementCount; i++)
                    {
                        _jsonWriter.WriteStringValue(value[i]);
                    }

                    _jsonWriter.WriteEndArray();
                };

                jsonGuidPropertyActionNumber = (propertyName, value) =>
                {
                    _jsonWriter.WriteStartArray(propertyName);
                    for (int i = 0; i < elementCount; i++)
                    {
                        _jsonWriter.WriteNumberValue((uint)(value[i]));
                    }

                    _jsonWriter.WriteEndArray();
                };

                _reader.Position += elementCount * 16; // sizeof(FGuid) = 16 bytes
            }

            string[] hashes = null;
            if (_readerSettings.ReadChunksHash)
            {
                hashes = _reader.ReadArray(elementCount, () => Utilities.ULongToHexHash(_reader.ReadULong()));
                _jsonAction.Add((_) =>
                {
                    jsonGuidPropertiesActionString("ChunksHashList", hashes);
                });
            }
            else
            {
                _reader.Position += elementCount * sizeof(ulong);  // in the manifest, hashes are stored as ulong values (8 bytes) but they are converted to hex string during the parsing process in order to be usable
            }

            string[] shaHashes = null;
            if (_readerSettings.ReadChunksShaHash)
            {
                shaHashes = _reader.ReadArray(elementCount, () => Utilities.BytesToHexadecimalString(_reader.ReadBytes(20)));
                if (!_jsonSimplified)
                {
                    _jsonAction.Add((_) =>
                    {
                        jsonGuidPropertiesActionString("ChunksShaHashList", shaHashes);
                    });
                }
            }
            else
            {
                _reader.Position += elementCount * 20; // sizeof(SHA1 hash) = 20 bytes
            }

            string[] groupNumbers = null;
            if (_readerSettings.ReadChunksGroupNumber)
            {
                groupNumbers = _reader.ReadArray(elementCount, () => $"{_reader.ReadByte():D2}");
                _jsonAction.Add((_) =>
                {
                    jsonGuidPropertiesActionString("ChunksGroupNumberList", groupNumbers);

                });
            }
            else
            {
                _reader.Position += elementCount; // * sizeof(byte)  // group numbers are stored as byte (1 byte) in the manifests but they are converted to string during parsing process in order for them to be usable
            }

            uint?[] windowSizes = null;
            if (_readerSettings.ReadChunksWindowSize)
            {
                windowSizes = _reader.ReadArray(elementCount, () => (uint?)_reader.ReadUInt());

                if (!_jsonSimplified)
                {
                    _jsonAction.Add((_) =>
                    {
                        jsonGuidPropertyActionNumber("ChunksWindowSizeList", windowSizes);
                    });
                }
            }
            else
            {
                _reader.Position += elementCount * sizeof(uint);
            }

            uint?[] filesSize = null;
            if (_readerSettings.ReadChunksDownloadSize)
            {
                filesSize = _reader.ReadArray(elementCount, () => (uint?)_reader.ReadLong());
                if (!_jsonSimplified)
                {
                    _jsonAction.Add((_) =>
                    {
                        jsonGuidPropertyActionNumber("ChunksFileSizeList", filesSize);
                    });
                }
            }
            else
            {
                _reader.Position += elementCount * sizeof(long); // fileSize are long values (8 bytes)
            }

            List<FChunkInfo> chunkInfos = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                chunkInfos.Add(new FChunkInfo(guids?[i], hashes?[i], shaHashes?[i], groupNumbers?[i], windowSizes?[i], filesSize?[i]));
            }

            Manifest.ChunkList = chunkInfos;
            if (_jsonWriter == null || _jsonGrouped)
            {
                _jsonAction.Clear();
                return;
            }

            _jsonWriter.WriteStartObject("ChunksList");
            for (int i = 0; i < _jsonAction.Count; i++)
            {
                _jsonAction[i](null);
            }
            _jsonWriter.WriteEndObject();
            _jsonWriter.Flush();
            _jsonAction.Clear();
        }

        private void ReadFFileManifest()
        {
            _ = /*(EChunkDataListVersion)*/_reader.ReadByte();
            int elementCount = _reader.ReadInt();
            string[] fileNames = null;
            if (_readerSettings.ReadFileFileName)
            {
                fileNames = _reader.ReadArray(elementCount, () => _reader.ReadFString());
                _jsonAction.Add((fileManifest) =>
                {
                    _jsonWriter.WriteString("Filename", fileManifest.Filename);
                });
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    _reader.SkipFString();
                }
            }

            string[] symLinkTargets = null;
            if (_readerSettings.ReadFileSymLinkTarget)
            {
                symLinkTargets = _reader.ReadArray(elementCount, () => _reader.ReadFString());

                if (!_jsonSimplified)
                {
                    _jsonAction.Add((fileManifest) =>
                    {
                        _jsonWriter.WriteString("FileSymlinkTarget", fileManifest.SymlinkTarget);
                    });
                }
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    _reader.SkipFString();
                }
            }

            string[] filesHash = null;
            if (_readerSettings.ReadFileHash)
            {
                filesHash = _reader.ReadArray(elementCount, () => Utilities.BytesToHexadecimalString(_reader.ReadBytes(20)));
                if (!_jsonSimplified)
                {
                    _jsonAction.Add((fileManifest) =>
                    {
                        _jsonWriter.WriteString("FileHash", fileManifest.FileHash);
                    });
                }
            }
            else
            {
                _reader.Position += elementCount * 20; // sizeof(SHA1 hash) = 20 bytes
            }

            EFileMetaFlags?[] filesMetaTags = null;
            if (_readerSettings.ReadFileMetaFlag)
            {
                filesMetaTags = _reader.ReadArray(elementCount, () => (EFileMetaFlags?)_reader.ReadByte());
                if (!_jsonSimplified)
                {
                    _jsonAction.Add((fileManifest) =>
                    {
                        _jsonWriter.WriteString("FileMetaFlag", fileManifest.FileMetaFlags.ToString());
                    });
                }
            }
            else
            {
                _reader.Position += elementCount; // * 1   EFileMetaFlags underlying type is byte (1 byte)
            }

            string[][] installTags = null;
            if (_readerSettings.ReadFileInstallTags)
            {
                installTags = _reader.ReadArray(elementCount, () => _reader.ReadTArray(() => _reader.ReadFString()));
                if (!_jsonSimplified)
                {
                    _jsonAction.Add((fileManifest) =>
                    {
                        _jsonWriter.WriteStartArray("InstallTags");
                        for (int j = 0; j < fileManifest.InstallTags.Count; j++)
                        {
                            _jsonWriter.WriteStringValue(fileManifest.InstallTags[j]);
                        }

                        _jsonWriter.WriteEndArray();
                    });
                }
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    int num = _reader.ReadInt();
                    for (int j = 0; j < num; j++)
                    {
                        _reader.SkipFString();
                    }
                }
            }

            FChunkPart[][] chunksParts = null;
            if (_readerSettings.ReadFChunkPart)
            {
                chunksParts = _reader.ReadArray(elementCount, () => _reader.ReadTArray(() => new FChunkPart(_reader)));
                Action<FFileManifest> ac;
                if (!_jsonGrouped)
                {
                    ac = (fileManifest) =>
                    {
                        _jsonWriter.WriteStartArray("FileChunkParts");
                        for (int k = 0; k < fileManifest.ChunkParts.Count; k++)
                        {
                            FChunkPart chunkPart = fileManifest.ChunkParts[k];
                            _jsonWriter.WriteStartObject();
                            _jsonWriter.WriteString("Guid", chunkPart.Guid);
                            _jsonWriter.WriteNumber("Offset", chunkPart.Offset);
                            _jsonWriter.WriteNumber("Size", chunkPart.Size);
                            _jsonWriter.WriteEndObject();
                        }

                        _jsonWriter.WriteEndArray();
                    };
                }
                else
                {
                    CreateLookUp(out Dictionary<string, string> hashesLookup, out Dictionary<string, string> datagroupLookup);
                    ac = (fileManifest) =>
                    {
                        _jsonWriter.WriteStartArray("FileChunkParts");
                        for (int k = 0; k < fileManifest.ChunkParts.Count; k++)
                        {
                            FChunkPart chunkPart = fileManifest.ChunkParts[k];
                            _jsonWriter.WriteStartObject();
                            _jsonWriter.WriteString("Guid", chunkPart.Guid);
                            _jsonWriter.WriteNumber("Offset", chunkPart.Offset);
                            _jsonWriter.WriteNumber("Size", chunkPart.Size);
                            _jsonWriter.WriteString("Hash", hashesLookup[chunkPart.Guid]);
                            _jsonWriter.WriteString("GroupNumber", datagroupLookup[chunkPart.Guid]);
                            _jsonWriter.WriteEndObject();
                        }

                        _jsonWriter.WriteEndArray();
                    };
                }

                _jsonAction.Add(ac);
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    int num = _reader.ReadInt();
                    for (int j = 0; j < num; j++)
                    {
                        _reader.Position += 28; // fchunkpart struct size
                    }
                }
            }

            List<FFileManifest> fFileManifest = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                fFileManifest.Add(new FFileManifest(fileNames?[i], symLinkTargets?[i], filesHash?[i], filesMetaTags?[i], installTags?[i], chunksParts?[i]));
            }

            Manifest.FileList = fFileManifest;
            if (_jsonWriter == null)
            {
                return;
            }

            _jsonWriter.WriteStartArray("FileManifestList");
            for (int i = 0; i < fFileManifest.Count; i++)
            {
                _jsonWriter.WriteStartObject();
                FFileManifest fileManifest = fFileManifest[i];
                for (int j = 0; j < _jsonAction.Count; j++)
                {
                    _jsonAction[j](fileManifest);
                }

                _jsonWriter.WriteEndObject();
            }

            _jsonWriter.WriteEndArray();
        }

        private void ReadFCustomFields()
        {
            _ = /*(EChunkDataListVersion)*/_reader.ReadByte();
            int elementCount = _reader.ReadInt();
            string[] keys = _reader.ReadArray(elementCount, () => _reader.ReadFString());
            string[] values = _reader.ReadArray(elementCount, () => _reader.ReadFString());
            Manifest.CustomFields = new(elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                Manifest.CustomFields.Add(keys[i], values[i]);
            }

            if (Manifest.CustomFields.TryGetValue("BaseUrl", out string urls))
            {
                Manifest.BaseUrls = urls.Split(',').ToList();
            }

            if (_jsonWriter == null)
            {
                return;
            }

            _jsonWriter.WriteStartObject("CustomFields");
            for (int i = 0; i < elementCount; i++)
            {
                _jsonWriter.WriteString(keys[i], values[i]);
            }

            _jsonWriter.WriteEndObject();
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
                _fileHandle?.Dispose();
                _reader.Dispose();
                if (tempManifestDataStorage == ManifestStorage.Disk)
                {
                    _tempFileName = $"{Environment.TickCount64}.tmp"; // in order to have a random temp file name
                    File.WriteAllBytes(_tempFileName, buffer);
                    _fileHandle = File.Open(_tempFileName, FileMode.Open, FileAccess.Read, FileShare.Delete);
                    _reader = new BufferedStream(_fileHandle);
                }
                else
                {
                    _reader = new MemoryStream(buffer);
                }
            }
        }

        private void CreateLookUp(out Dictionary<string, string> hashesLookup, out Dictionary<string, string> datagroupsLookup)
        {
            int cap = Manifest.ChunkList.Count;
            hashesLookup = new(cap);
            datagroupsLookup = new(cap);
            for (int i = 0; i < cap; i++)
            {
                FChunkInfo chunkInfo = Manifest.ChunkList[i];
                string guid = chunkInfo.Guid;
                hashesLookup.Add(guid, chunkInfo.Hash);
                datagroupsLookup.Add(guid, chunkInfo.GroupNumber);
            }
        }

        private void ExecuteReadIfTrue(bool shouldRead, Action toExecute)
        {
            uint dataSizeToSkip = _reader.ReadUInt();
            if (shouldRead)
            {
                toExecute();
            }
            else
            {
                _reader.Position += dataSizeToSkip - sizeof(uint);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _jsonWriter?.Dispose();
                    _reader.Dispose();
                    _fileHandle?.Close();
                }

                _disposed = !_disposed;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}