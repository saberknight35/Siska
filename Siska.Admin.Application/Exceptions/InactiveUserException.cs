using System.Runtime.Serialization;

namespace Siska.Admin.Application.Exceptions
{
    [Serializable]
    public class InactiveUserException : Exception
    {
        public InactiveUserException()
        {
        }

        public InactiveUserException(string message)
            : base(message)
        {
        }

        public InactiveUserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InactiveUserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
