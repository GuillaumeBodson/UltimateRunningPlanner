using ToolBox.EntityFramework.Repository;
using ToolBox.EntityFramework.Repository.Abstractions;
using User.API.BusinessLogic.Abstractions;
using User.API.Repository.Abstractions;

namespace User.API.BusinessLogic;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IIncludeSpecification<Data.Models.User> _userWithFullSpec = new UserWithFullSpecification();
    public async Task<Data.Models.User?> GetUserByIdAsync(int userId)
        => await _userRepository.GetByIdAsync(userId, _userWithFullSpec);
    public async Task<Data.Models.User> CreateUserAsync(Data.Models.User user)
    {
        await _userRepository.AddAsync(user);
        return user;
    }
    public async Task UpdateUserAsync(Data.Models.User user)
    {
        await _userRepository.UpdateAsync(user);
    }
    public async Task DeleteUserAsync(int userId)
    {
        await _userRepository.DeleteAsync(userId);
    }
}

public class UserWithFullSpecification : IncludeSpecification<Data.Models.User>
{
    public UserWithFullSpecification()
    {
        AddInclude(u => u.Athlete);
        AddInclude(u => u.AthletePreferences);
    }
}