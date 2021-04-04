using System;

namespace UEManifestReader.Exceptions
{
    public class FStringInvalidException : Exception
    {
        public FStringInvalidException(string message)
            : base(message)
        {
        }
    }
}