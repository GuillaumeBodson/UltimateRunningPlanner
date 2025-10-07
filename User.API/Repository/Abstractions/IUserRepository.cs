using ToolBox.EntityFramework.Repository.Abstractions;

namespace User.API.Repository.Abstractions;

public interface IUserRepository : IGenericRepository<Models.User, int>
{

}
