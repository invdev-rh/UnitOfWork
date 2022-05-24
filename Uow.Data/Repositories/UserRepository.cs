using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uow.Data.Core;
using Uow.Domain.UserManagement;

namespace Uow.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbUnitOfWork _uow;

        public UserRepository(IDbUnitOfWork uow)
        {
            _uow = uow;
        }
        
        public async Task<List<User>> GetAllAsync()
        {
            return (await _uow.QueryAsync<User>("Select * from Users")).ToList();
        }

        public async Task UpsertAsync(User user)
        {
            // You could do other things like check Id == 0 to see if it's new
            // this is the just lazy way ;)

            var updated = await _uow.UpdateAsync(user);

            if (!updated)
                await _uow.InsertAsync(user);
        }
    }
}