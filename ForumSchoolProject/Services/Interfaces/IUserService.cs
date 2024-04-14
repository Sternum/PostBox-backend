using ForumSchoolProject.Models;

namespace ForumSchoolProject.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> ValidateUserAsync(LoginModel login);
        Task<string> EncryptPassword(string password);
    }
}
