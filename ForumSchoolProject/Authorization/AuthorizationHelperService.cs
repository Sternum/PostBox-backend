using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ForumSchoolProject.Authorization
{
    public class AuthorizationHelperService : IAuthorizationHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAdmin()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            return currentUser?.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        }
        public bool IsOwnerOfPost(int postUid)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            return currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value == postUid.ToString();
        }
        public bool IsAdminOrOwnerOfPost(int postUid)
        {
            return IsAdmin() || IsOwnerOfPost(postUid);
        }
    }
}
