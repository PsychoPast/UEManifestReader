# UEManifestReader
 Library to read and parse Unreal Engine manifest and delta files.

 ## Features:
 1. Read and parse a **raw serialied** (json format is NOT supported) manifest from a file (.manifest | .delta *usually*), a byte array, a stream or an Uri.
 2. Choose wether to write the output json formatted or no. (more details in the library XML documentation) 
 3. Decent execution time (between 600-800 ms). **Not writing the output to file makes the execution faster.**
 4. Choose how to format is json output.
 5. Choose which fields to read and which to skip. **The less fields there is, the faster is the execution and the lesser is the memory footprint.**

|                | Meaning | Required for chunk downloading|
|----------------|----|---------|-------------------------------|
|readManifestMeta|Read the manifest metadata(ManifestVersion, IsFileData, ChunksSubdir, FilesSubdir, AppId, AppName, BuildVersion, LaunchExe, LaunchCommand, PrereqIds, PrereqName, PrereqPath, PrereqArgs, BuildId)|YES|
|readChunksGuid|Read chunks Guid|YES
|readChunksHash|Read chunks Hash|YES
|readChunksShaHash|Read chunks Sha1 Hash|NO
|readChunksGroupNumber|Read chunks datagroup number|YES
|readChunksWindowSize|Read chunks window size|NO
|readChunksDownloadSize|Read chunks download size|NO
|readFileFileName|Read file filename|NO
|readFileSymLinkTarget|Read file symlink target|NO
|readFileHash|Read file hash|NO
|readFileMetaFlag|Read file metadata|NO
|readFileInstallTags|Read file install tags|NO
|readFChunksParts|Read file chunks infos|YES
|readCustomFields|Read custom fields(BaseUrl, BuildLabel, CatalogAssetName, CatalogItemId, CatalogNamespace, FullAppName)|YES

## Chunk Download Url Format:
{BaseUrl}/{ChunksSubdir}/{ChunkGroupNumber}/{ChunkHash}_{ChunkGuid}.chunk
BaseUrl = *one* of the urls in the BaseUrl field of CustomFields
ChunksSubdir = the field ChunksSubdir of the manifest metadata