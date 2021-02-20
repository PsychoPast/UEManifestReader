using global::UEManifestReader.Objects;

namespace UEManifestReader
{
    public interface IManifestReader
    {
        public FManifest Manifest { get; }

        public void ReadManifest(ManifestStorage storage);
    }
}