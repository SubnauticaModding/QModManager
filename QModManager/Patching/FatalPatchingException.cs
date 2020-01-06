namespace QModManager.Patching
{
    using System;

    internal class FatalPatchingException : Exception
    {
        public FatalPatchingException()
        {
        }

        public FatalPatchingException(string message) : base(message)
        {
        }

        public FatalPatchingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
