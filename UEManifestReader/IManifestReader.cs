using UEManifestReader.Enums;
using UEManifestReader.Exceptions;
using UEManifestReader.Objects;

namespace UEManifestReader
{
    /// <summary>
    /// Interface for manifest readers.
    /// </summary>
    public interface IManifestReader
    {
        /// <summary>
        /// Parsed manifest object.
        /// </summary>
        public FManifest Manifest { get; }

        /// <summary>
        /// Read the manifest.
        /// </summary>
        /// <exception cref="UEManifestReaderException">Throws if an error happens while reading the manifest.</exception>
        public void ReadManifest();

        /// <summary>
        /// Read the manifest.
        /// </summary>
        /// <param name="tempManifestDataStorage">The location of the modified manifest data if the data can't be processed directly.</param>
        /// <exception cref="UEManifestReaderException">Throws if an error happens while reading the manifest.</exception>
        public void ReadManifest(ManifestStorage storage);
    }
}