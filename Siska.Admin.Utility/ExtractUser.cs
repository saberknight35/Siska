using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Siska.Admin.Utility
{
    public class ExtractUser : IExtractUser
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal user;

        public ExtractUser(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                user = _httpContextAccessor.HttpContext.User;
            }
        }

        public string Id
        {
            get
            {
                return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            }
        }

        public string Name
        {
            get
            {
                return user.FindFirstValue(ClaimTypes.Name) ?? "System";
            }
        }

        public string Username
        {
            get
            {
                return user.FindAll(ClaimTypes.NameIdentifier).Last().Value ?? "System";
            }
        }

        public string Email
        {
            get
            {
                return user?.FindFirstValue(ClaimTypes.Email) ?? "System";
            }
        }

        public ClaimsPrincipal User
        {
            get
            {
                return user;
            }
        }
    }
}
