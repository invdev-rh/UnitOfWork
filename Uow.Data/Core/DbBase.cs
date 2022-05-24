using System.Data;
using Microsoft.Data.SqlClient;

namespace Uow.Data.Core
{
    public abstract class DbBase : IDb
    {
        private readonly IDbSettings _dbSettings;

        protected DbBase(IDbSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public virtual IDbConnection GetConnection()
        {
            return new SqlConnection(_dbSettings.ConnectionString());
        }
    }
}
