namespace UEManifestReader.Enums
{
    internal enum EChunkDataListVersion : byte
    {
        Original = 0,

        LatestPlusOne,

        Latest = LatestPlusOne - 1
    }
}