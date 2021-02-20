namespace UEManifestReader.Enums
{
    internal enum EFileManifestListVersion : byte
    {
        Original = 0,

        LatestPlusOne,

        Latest = LatestPlusOne - 1
    }
}