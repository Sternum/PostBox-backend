using ForumSchoolProject.Enums;
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
            return currentUser?.FindFirst(ClaimTypes.Role)?.Value == UserRolesEnum.Admin.ToString();
        }
        public bool IsOwner(int postUid)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            return currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value == postUid.ToString();
        }
        public bool IsAdminOrOwner(int uId)
        {
            return IsAdmin() || IsOwner(uId);
        }


    }
}
