using ForumSchoolProject.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumSchoolProject.Authorization
{
    public class AuthorizationHelperClass
    {
        public async Task<List<string>> GetUserRoles(User user)
        {
            var roles = new List<string>();

            // Example mapping
            switch (user.UserGroupId)
            {
                case 1:
                    roles.Add("Admin");
                    break;
                case 2:
                    roles.Add("User");
                    break;
            }

            return roles;
        }

    }
    public class AuthorizeUserGroupAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _requiredGroup;

        public AuthorizeUserGroupAttribute(int requiredGroup)
        {
            _requiredGroup = requiredGroup;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User; // Get user from context

            var userGroupId = GetUserGroupId(user);

            if (userGroupId != _requiredGroup)
            {
                context.Result = new ForbidResult();
            }
        }
        public int GetUserGroupId(ClaimsPrincipal user)
        {
            // Example of extracting a group ID from the user's claims
            var userGroupIdClaim = user.FindFirst(ClaimTypes.GroupSid); // or another claim type that holds the group ID
            if (userGroupIdClaim != null && int.TryParse(userGroupIdClaim.Value, out int userGroupId))
            {
                return userGroupId;
            }

            // Return a default value or throw an exception if the group ID is not found
            return -1; // or handle as needed
        }
    }

}
