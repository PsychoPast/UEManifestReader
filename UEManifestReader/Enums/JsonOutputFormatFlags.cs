using System;

namespace UEManifestReader
{
    /// <summary>
    /// The way to write the json output data to the file.
    /// </summary>
    [Flags]
    public enum JsonOutputFormatFlags
    {
        /// <summary>
        /// Write to the file the data as it is.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Write to the file the data in indented format.
        /// </summary>
        Indented = 1,

        /// <summary>
        /// Write to the file the data grouped for each file.
        /// </summary>
        Grouped = 1 << 1,

        /// <summary>
        /// Write to the file only the data needed to download the game/app files.
        /// </summary>
        Simplified = 1 << 2,
    }
}