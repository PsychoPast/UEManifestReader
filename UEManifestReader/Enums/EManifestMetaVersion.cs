namespace UEManifestReader.Enums
{
    internal enum EManifestMetaVersion : byte
    {
        Original = 0,

        SerialisesBuildId,

        LatestPlusOne,

        Latest = LatestPlusOne - 1
    }
}