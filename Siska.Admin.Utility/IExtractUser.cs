using System.Security.Claims;

namespace Siska.Admin.Utility
{
    public interface IExtractUser
    {
        public string Id { get; }
        public string Name { get; }
        public string Username { get; }
        public string Email { get; }
        public ClaimsPrincipal User { get; }
    }
}
