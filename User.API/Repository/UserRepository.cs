using ToolBox.EntityFramework.Repository;
using ToolBox.EntityFramework.Repository.Abstractions;
using User.API.Repository.Abstractions;

namespace User.API.Repository;

public class UserRepository : GenericRepository<Data.Models.User, int>, IUserRepository
{
    public UserRepository(IGenericWriteRepository<Data.Models.User, int> writeRepository,
                          IGenericReadRepository<Data.Models.User, int> readRepository) : base(writeRepository, readRepository)
    { }
}
