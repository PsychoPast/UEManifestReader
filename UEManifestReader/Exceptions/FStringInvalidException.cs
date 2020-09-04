namespace UEManifestReader.Exceptions
{
    using System;

    public class FStringInvalidException : Exception
    {
        public FStringInvalidException(string message)
            : base(message)
        {
        }
    }
}