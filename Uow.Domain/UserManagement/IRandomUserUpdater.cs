using System.Threading.Tasks;

namespace Uow.Domain.UserManagement
{
    public interface IRandomUserUpdater
    {
        Task MessUpSomeUser();
        Task NestedTransCommitOnBoth();
        Task NestedTransRollbackOuter();
        Task NestedTransRollbackInner();
    }
}