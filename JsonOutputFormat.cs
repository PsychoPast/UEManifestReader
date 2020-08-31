using System;

namespace UEManifestReader
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum JsonOutputFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 1,

        /// <summary>
        /// 
        /// </summary>
        Grouped = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        Simplified = 1 << 2,

        /// <summary>
        /// 
        /// </summary>
        Custom = 1 << 3
    }
}