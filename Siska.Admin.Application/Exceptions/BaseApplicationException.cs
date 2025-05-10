namespace Siska.Admin.Application.Exceptions
{
    public abstract class BaseApplicationException : Exception
    {
        public BaseApplicationException(string message) : base(message)
        {
        }

        public BaseApplicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}