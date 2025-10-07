using Shared.Repository;
using User.API.Data;
using User.API.Repository.Abstractions;

namespace User.API.Repository;

public class UserRepository : GenericRepository<Models.User, int>, IUserRepository
{
    public UserRepository(UserDbContext context, ILogger<UserRepository> logger) : base(context, logger)
    { }
}
