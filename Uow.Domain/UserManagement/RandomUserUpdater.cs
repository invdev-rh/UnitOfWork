using System;
using System.Linq;
using System.Threading.Tasks;
using Uow.Domain.Core;

namespace Uow.Domain.UserManagement
{
    public class RandomUserUpdater : IRandomUserUpdater
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserRepository _userRepository;

        public RandomUserUpdater(IUnitOfWork uow, IUserRepository userRepository)
        {
            _uow = uow;
            _userRepository = userRepository;
        }

        public async Task MessUpSomeUser()
        {
            // You could make the uow .Begin() method smarter and return a disposable
            // but it wasn't built that way originally so this is a wrapper to do it
            // however might be night to wrap it all up in one in the future
            using var uowTrans = new UnitOfWorkTransaction(_uow);

            var users = await _userRepository.GetAllAsync();

            var lastUser = users.OrderByDescending(k => k.Id).First();
            lastUser.Name = "Bob";

            // this is within a transaction
            await _userRepository.UpsertAsync(lastUser);
            
            uowTrans.Commit();
        }


        public async Task NestedTransCommitOnBoth()
        {
            using var uowTrans = new UnitOfWorkTransaction(_uow);

            var newUser = new User()
            {
                Name = "Joe"
            };

            await _userRepository.UpsertAsync(newUser);

            await MessUpSomeUser();
            
            uowTrans.Commit();
        }
        
        public async Task NestedTransRollbackOuter()
        {
            using var uowTrans = new UnitOfWorkTransaction(_uow);

            var newUser = new User()
            {
                Name = "Mike"
            };

            await _userRepository.UpsertAsync(newUser);

            await MessUpSomeUser();
            
            // No call to Commit() will roll ack when using UnitOfWorkTransaction
            // you could also just call _uow.Rollback()
            // or you could dispose of this yourself to force the rollback
        }
        
        public async Task NestedTransRollbackInner()
        {
            using var uowTrans = new UnitOfWorkTransaction(_uow);

            var newUser = new User()
            {
                Name = "Mary"
            };

            await _userRepository.UpsertAsync(newUser);

            await UpdateWithRollback();
            
            uowTrans.Commit();
        }

        private async Task UpdateWithRollback()
        {
            using var uowTrans = new UnitOfWorkTransaction(_uow);

            var users = await _userRepository.GetAllAsync();

            var lastUser = users.OrderByDescending(k => k.Id).First();
            lastUser.Name = "Sam";

            await _userRepository.UpsertAsync(lastUser);
        }
    }
}