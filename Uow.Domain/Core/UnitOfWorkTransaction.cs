using System;

namespace Uow.Domain.Core
{
    /// <summary>
    /// A class to manage a uow transaction within a `using` block.
    /// A transaction is begun when the class is created, and
    /// rolled back if the class is disposed before Commit() is called.
    /// </summary>
    public class UnitOfWorkTransaction : IDisposable
    {
        private readonly IUnitOfWork uow;
        private bool committed = false;

        public UnitOfWorkTransaction(IUnitOfWork uow)
        {
            this.uow = uow;
            uow.Begin();
        }

        public void Commit()
        {
            uow.Commit();
            committed = true;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!committed)
                        uow.Rollback();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}