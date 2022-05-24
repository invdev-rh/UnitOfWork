using System.Data;

namespace Uow.Data.Core
{
    public interface IDb
    {
        IDbConnection GetConnection();
    }
}