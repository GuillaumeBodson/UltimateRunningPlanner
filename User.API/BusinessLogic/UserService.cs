using User.API.Repository.Abstractions;

namespace User.API.BusinessLogic;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<Models.User?> GetUserByIdAsync(int userId)
        => await _userRepository.GetUserByIdAsync(userId);
    public async Task<Models.User> CreateUserAsync(Models.User user)
    {
        await _userRepository.AddUserAsync(user);
        return user;
    }
}
