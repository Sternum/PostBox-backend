namespace ForumSchoolProject.Authorization
{
    public interface IAuthorizationHelperService
    {
        bool IsAdmin();
        bool IsAdminOrOwnerOfPost(int postUid);
        bool IsOwnerOfPost(int postUid);
    }
}