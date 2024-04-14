using ForumSchoolProject.Models;

namespace ForumSchoolProject.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> ValidateUserAsync(LoginUserDto login);
        Task<string> EncryptPassword(string password);
        Task<User> GetUserAsync(string username);
        string GenerateJwtToken(User user);
    }
}
