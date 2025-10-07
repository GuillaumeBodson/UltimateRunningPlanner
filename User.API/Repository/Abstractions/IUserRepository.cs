namespace User.API.Repository.Abstractions;

public interface IUserRepository
{
    Task<IEnumerable<Models.User>> GetAllUsersAsync();
    Task<Models.User?> GetUserByIdAsync(int id);
    Task<Models.User> AddUserAsync(Models.User user);
    Task UpdateUserAsync(Models.User user);
    Task DeleteUserAsync(int id);
}
