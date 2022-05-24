using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uow.Domain.UserManagement
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task UpsertAsync(User user);
    }
}