namespace User.API.BusinessLogic.Abstractions;

public interface IUserService
{
    Task<Data.Models.User> CreateUserAsync(Data.Models.User user);
    Task DeleteUserAsync(int userId);
    Task<Data.Models.User?> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(Data.Models.User user);
}