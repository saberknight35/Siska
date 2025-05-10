using System.Diagnostics.CodeAnalysis;
using Siska.Admin.Application.Enums;

namespace Siska.Admin.Application.Exceptions
{
    public class DataAlreadyExistException : Exception
    {
        [ExcludeFromCodeCoverage]
        public DataAlreadyExistException(string message)
            : base(message)
        {
        }

        public static void ThrowIfNotNull(object argument, IPSEntityType entityType)
        {
            if (argument is not null)
            {
                Throw(entityType);
            }
        }

        public static void Throw(IPSEntityType entityType)
        {
            throw new DataAlreadyExistException($"The {entityType} with the supplied code is already exists.");
        }
    }
}