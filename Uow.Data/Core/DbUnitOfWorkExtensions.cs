using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Uow.Data.Core
{
    public static class DbUnitOfWorkExtensions
    {
        // Sync
        public static int Execute(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Execute(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static T ExecuteScalar<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.ExecuteScalar<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static IEnumerable<T> Query<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Query<T>(sql, transaction: uow.Transaction, param: param, commandType: commandType));
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Query(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Query(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Query(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static T QueryFirst<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.QueryFirst<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static T QueryFirstOrDefault<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.QueryFirstOrDefault<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static T QuerySingleOrDefault<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.QuerySingleOrDefault<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        /// <summary>
        /// Wraps the Dapper.Contrib Delete<T> to remove the need to specify the transaction
        /// </summary>
        public static bool Delete<T>(this IDbUnitOfWork uow, T entityToDelete, int? commandTimeout = null) where T : class
        {
            return SqlRetryPolicy.Execute(() => uow.Connection.Delete<T>(entityToDelete, transaction: uow.Transaction, commandTimeout: commandTimeout));
        }

        // Async
        public static Task<int> ExecuteAsync(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.ExecuteAsync(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static Task<T> ExecuteScalarAsync<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.ExecuteScalarAsync<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryAsync(sql, transaction: uow.Transaction, param: param, commandType: commandType));
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryAsync<T>(sql, transaction: uow.Transaction, param: param, commandType: commandType));
        }

        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryAsync(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbUnitOfWork uow, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", CommandType? commandType = null)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryAsync(sql, map, transaction: uow.Transaction, param: param, commandType: commandType, buffered: buffered, splitOn: splitOn));
        }

        public static Task<T> QueryFirstAsync<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryFirstAsync<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        public static Task<T> QuerySingleOrDefaultAsync<T>(this IDbUnitOfWork uow, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction: uow.Transaction, commandType: commandType));
        }

        /// <summary>
        /// Wraps the Dapper.Contrib InsertAsync<T> to remove the need to specify the transaction
        /// </summary>
        public static Task<int> InsertAsync<T>(this IDbUnitOfWork uow, T entityToInsert, int? commandTimeout = null, ISqlAdapter sqlAdapter = null) where T : class
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.InsertAsync<T>(entityToInsert, transaction: uow.Transaction, commandTimeout: commandTimeout, sqlAdapter: sqlAdapter));
        }

        /// <summary>
        /// Wraps the Dapper.Contrib UpdateAsync<T> to remove the need to specify the transaction
        /// </summary>
        public static Task<bool> UpdateAsync<T>(this IDbUnitOfWork uow, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return SqlRetryPolicy.ExecuteAsync(async () => await uow.Connection.UpdateAsync<T>(entityToUpdate, transaction: uow.Transaction, commandTimeout: commandTimeout));
        }

        /// <summary>
        /// Wraps the Dapper.Contrib GetAsync<T> to remove the need to specify the transaction
        /// </summary>
        public static Task<T> GetAsync<T>(this IDbUnitOfWork uow, dynamic id, int? commandTimeout = null) where T : class
        {
            return SqlRetryPolicy.ExecuteAsync<T>(async () => await SqlMapperExtensions.GetAsync<T>(uow.Connection, id, transaction: uow.Transaction, commandTimeout: commandTimeout));
        }
        
        /// <summary>
        /// Wraps the Dapper.Contrib DeleteAsync<T> to remove the need to specify the transaction
        /// </summary>
        public static Task<bool> DeleteAsync<T>(this IDbUnitOfWork uow, T entityToDelete, int? commandTimeout = null) where T : class
        {
            return SqlRetryPolicy.ExecuteAsync(async() => await uow.Connection.DeleteAsync<T>(entityToDelete, transaction: uow.Transaction, commandTimeout: commandTimeout));
        }
    }
}
