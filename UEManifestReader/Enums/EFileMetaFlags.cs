namespace UEManifestReader.Enums
{
    using System;

    [Flags]
    public enum EFileMetaFlags : byte
    {
        /// <summary>
        /// Flag for none.
        /// </summary>
        None = 0,

        /// <summary>
        /// Flag for readonly file.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// Flag for natively compressed.
        /// </summary>
        Compressed = 1 << 1,

        /// <summary>
        /// Flag for unix executable.
        /// </summary>
        UnixExecutable = 1 << 2
    }
}