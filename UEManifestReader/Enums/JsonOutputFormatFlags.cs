using System;

namespace UEManifestReader.Enums
{
    /// <summary>
    ///     The way to write the json output data to the file.
    /// </summary>
    [Flags]
    public enum JsonOutputFormatFlags
    {
        /// <summary>
        ///     Writes to the file the data as it is (with no indentation).
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Writes to the file the data in an indented format.
        /// </summary>
        Indented = 1,

        /// <summary>
        ///     Writes to the file the informations and data of each file after grouping them.
        /// </summary>
        Grouped = 1 << 1,

        /// <summary>
        ///     Writes to the file only the data needed to download the game/app files.
        /// </summary>
        Simplified = 1 << 2
    }
}