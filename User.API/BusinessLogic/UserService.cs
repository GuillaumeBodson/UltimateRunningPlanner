using Shared.Repository;
using Shared.Repository.Abstractions;
using User.API.Repository.Abstractions;

namespace User.API.BusinessLogic;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IIncludeSpecification<Models.User> _userWithFullSpec = new UserWithFullSpecification();
    public async Task<Models.User?> GetUserByIdAsync(int userId)
        => await _userRepository.GetByIdAsync(userId, _userWithFullSpec);
    public async Task<Models.User> CreateUserAsync(Models.User user)
    {
        await _userRepository.AddAsync(user);
        return user;
    }
    public async Task UpdateUserAsync(Models.User user)
    {
        await _userRepository.UpdateAsync(user);
    }
    public async Task DeleteUserAsync(int userId)
    {
        await _userRepository.DeleteAsync(userId);
    }
}

public class UserWithFullSpecification : IncludeSpecification<Models.User>
{
    public UserWithFullSpecification()
    {
        AddInclude(u => u.Athlete);
        AddInclude(u => u.AthletePreferences);
    }
}