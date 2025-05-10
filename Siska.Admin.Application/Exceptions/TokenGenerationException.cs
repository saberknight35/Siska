using System.Runtime.Serialization;

namespace Siska.Admin.Application.Exceptions
{
    [Serializable]
    public class TokenGenerationException : Exception
    {
        public TokenGenerationException()
        {
        }

        public TokenGenerationException(string message)
            : base(message)
        {
        }

        public TokenGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TokenGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
