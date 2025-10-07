using Shared.Repository;
using Shared.Repository.Abstractions;
using User.API.Data;
using User.API.Repository.Abstractions;

namespace User.API.Repository;

public class UserRepository : GenericRepository<Models.User, int>, IUserRepository
{
    public UserRepository(UserDbContext context,
                          ILogger<UserRepository> logger,
                          IGenericWriteRepository<Models.User, int> writeRepository,
                          IGenericReadRepository<Models.User, int> readRepository) : base(context, logger, writeRepository, readRepository)
    { }
}
