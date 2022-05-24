using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Uow.Data.Core
{
    public class DbUnitOfWork : IDbUnitOfWork
    {
        private readonly IDb _db;
        private readonly ILogger<DbUnitOfWork> _logger;
        private int _transCount;

        public DbUnitOfWork(IDb db, ILogger<DbUnitOfWork> logger)
        {
            _db = db;
            _logger = logger;
        }

        private IDbConnection _connection;
        public IDbConnection Connection => this.Transaction?.Connection ?? (_connection ??= _db.GetConnection());

        public IDbTransaction Transaction { get; private set; }

        public bool InTransaction => this.Transaction != null;


        public void Begin()
        {
            if (Transaction != null)
            {
                // already in a transaction, use it. nested transactions aren't a real thing in sql anyhow
                // https://www.sqlskills.com/blogs/paul/a-sql-server-dba-myth-a-day-2630-nested-transactions-are-real/
                _transCount++;
                _logger.LogDebug($"Inner begin transaction requested, reusing existing. count ({_transCount}). transaction : {Transaction.GetHashCode()}, connection: {Connection.GetHashCode()}");
                return;
            }

            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                Transaction = Connection.BeginTransaction();
                _transCount++;
                _logger.LogDebug($"Started transaction : {Transaction.GetHashCode()}, connection: {Connection.GetHashCode()}");
            }
            catch (Exception)
            {
                Transaction?.Rollback();
                ReleaseResources();
                throw;
            }
        }

        public void Commit()
        {
            try
            {
                if (Transaction == null)
                {
                    _logger.LogDebug($"Unable to commit transaction, it has already been released. Potentially a previous (inner) rollback which prevents further commits. connection: {Connection.GetHashCode()}");
                    return;
                }

                _transCount--;
                if (_transCount > 0)
                {
                    _logger.LogDebug($"Inner commit transaction requested, continuing existing. count ({_transCount}). transaction : {Transaction.GetHashCode()}, connection: {Connection.GetHashCode()}");
                    return;
                }

                _logger.LogDebug($"Committing transaction : {Transaction.GetHashCode()}, connection: {Connection.GetHashCode()}");

                Transaction.Commit();
                _transCount = 0;

                ReleaseResources();
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                if (Transaction == null)
                {
                    _logger.LogDebug($"Unable to rollback transaction, it has already been released. Potentially a previous (inner) rollback. connection: {Connection.GetHashCode()}");
                    return;
                }

                _logger.LogDebug($"Rollback of transaction : {Transaction.GetHashCode()}, connection: {Connection.GetHashCode()}");

                Transaction.Rollback();
                _transCount = 0;
            }
            catch (Exception ex)
            {
                // rollback failures should not interrupt existing flow
                // if an exception is thrown here it will cause issues for the caller
                // due to the general reason to rollback is a prior exception
                _logger.LogError(ex, "Failure when attempting to rollback transaction");
            }
            finally
            {
                ReleaseResources();
            }
        }

        private void ReleaseResources()
        {
            try
            {
                Transaction?.Dispose();
            }
            catch (Exception ex)
            {
                // may be in a funky state but should not cause caller to fail
                // as that may already be handling an exception
                _logger.LogError(ex, "Failure when attempting to release transaction resources");
            }
            finally
            {
                Transaction = null;
            }

            try
            {
                Connection.Close();
            }
            catch (Exception ex)
            {
                // may be in a funky state but should not cause caller to fail
                // as that may already be handling an exception
                _logger.LogError(ex, "Failure when attempting to release connection resources");
            }
        }
    }
}
