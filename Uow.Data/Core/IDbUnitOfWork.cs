using System.Data;
using Uow.Domain.Core;

namespace Uow.Data.Core
{
    public interface IDbUnitOfWork : IUnitOfWork
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
    }
}
