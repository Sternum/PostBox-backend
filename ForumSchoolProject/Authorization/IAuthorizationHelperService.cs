namespace ForumSchoolProject.Authorization
{
    public interface IAuthorizationHelperService
    {
        bool IsAdmin();
        bool IsAdminOrOwner(int postUid);
        bool IsOwner(int postUid);
    }
}