using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Uow.Domain.Core;

namespace Uow.Data.Core
{
    internal static class SqlRetryPolicy
    {
        private const int DefaultMaxNumberOfRetries = 3;
        private const int DefaultDelayBetweenRetiresInMs = 5;
 
        /// <summary>
        /// A list of SQL error codes that should be retried.
        /// https://msdn.microsoft.com/en-us/library/cc645860.aspx - For more info Sql Error Codes
        /// </summary>
        private static readonly int[] DefaultRetryableSqlErrorCodes =
        {
            -1,     // SqlConnectionBroken
            -2,     // SqlTimeout
            2,      // Error connecting to SQL Server - server not started
            35,     // Error connecting to SQL Server - A network-related or instance-specific error
            40,     // Error connecting to SQL Server - Named Pipes Provider, could not open a connection to SQL Server
            53,     // Error connecting to SQL Server - client cannot resolve name of server or server name incorrect
            1203,   // Resource unlock
            1204,   // SqlOutOfLocks
            1205,   // Transaction was deadlocked
            1221,   // Releasing locks
            1222,   // SqlLockRequestTimeout
            1532,   // New sort run starting
            1533,   // Cannot share extent
            1534,   // Extent %S_PGID not found
            1535,   // Cannot share extent
            4060,   // Cannot open database
            40197,  // Error processing request, could be due to upgrade or failover running
            40501,  // Service is busy
            40613,  // Database unavailable
        };

        public static void Execute(Action operation)
        {
            Retry.Execute(operation.Invoke);
        }

        public static TResult Execute<TResult>(Func<TResult> operation)
        {
            return Retry.Execute(() => operation.Invoke());
        }

        public static async Task ExecuteAsync(Func<Task> operation)
        {
            await RetryAsync.ExecuteAsync(operation.Invoke);
        }

        public static async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return await RetryAsync.ExecuteAsync(operation.Invoke);
        }

        private static Polly.Retry.RetryPolicy Retry => Policy
                                                        .Handle<Exception>(ExceptionPredicate)
                                                        .WaitAndRetry(
                                                            DefaultMaxNumberOfRetries,
                                                            DefaultSleepDurationProvider,
                                                            OnRetryHandler());

        private static AsyncRetryPolicy RetryAsync => Policy
                                                        .Handle<Exception>(ExceptionPredicate)
                                                        .WaitAndRetryAsync(
                                                            DefaultMaxNumberOfRetries,
                                                            DefaultSleepDurationProvider,
                                                            OnRetryHandler());



        private static bool ExceptionPredicate(Exception ex)
        {
            return IsRetryable(ex, DefaultRetryableSqlErrorCodes);
        }

        private static Action<Exception, TimeSpan, Context> OnRetryHandler()
        {
            return (ex, period, context) =>
                Log.StaticLogger.LogWarning(ex, "Failure executing sql - [crlId {correlationId}], Retry waiting for {period}. Message: {message}", context.CorrelationId, period, ex.Message);
        }

        private static TimeSpan DefaultSleepDurationProvider(int retryAttempt)
        {
            return TimeSpan.FromMilliseconds(DefaultDelayBetweenRetiresInMs * retryAttempt);
        }


        private static bool IsRetryable(Exception ex, int[] retryableSqlErrorCodes)
        {
            if (ex is SqlException sqlException && retryableSqlErrorCodes.Contains(sqlException.Number))
            {
                return true;
            }

            if (ex.InnerException != null)
            {
                return IsRetryable(ex.InnerException, retryableSqlErrorCodes);
            }

            return false;
        }
    }
}
