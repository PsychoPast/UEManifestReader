using System;

namespace UEManifestReader.Exceptions
{
    public class UEManifestReaderException : Exception
    {
        public UEManifestReaderException(string message)
            : base(message)
        {
        }

        public UEManifestReaderException(string message, Exception inException)
            : base(message, inException)
        {
        }
    }
}