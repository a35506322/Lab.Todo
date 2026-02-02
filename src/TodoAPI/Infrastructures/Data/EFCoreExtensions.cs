using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace TodoAPI.Infrastructures.Data;

/// <summary>
/// EF Core Database Extensions for Dapper
/// </summary>
public static class EFCoreExtensions
{
    /// <summary>
    /// Execute with Dapper in EF Core, support transaction if enabled
    /// </summary>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>The number of rows affected.</returns>
    public static int DapperExecute(
        this DatabaseFacade database,
        string commandText,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null
    )
    {
        var cn = database.GetDbConnection();
        IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
        return cn.Execute(
            sql: commandText,
            param: param,
            transaction: trn,
            commandTimeout: commandTimeout,
            commandType: commandType
        );
    }

    /// <summary>
    /// Execute with Dapper in EF Core asynchronously, support transaction if enabled
    /// </summary>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> DapperExecuteAsync(
        this DatabaseFacade database,
        string commandText,
        object? param = null,
        int? commandTimeout = null,
        CommandType? commandType = null
    )
    {
        var cn = database.GetDbConnection();
        IDbTransaction? trn = database.CurrentTransaction?.GetDbTransaction();
        return await cn.ExecuteAsync(
            sql: commandText,
            param: param,
            transaction: trn,
            commandTimeout: commandTimeout,
            commandType: commandType
        );
    }

    /// <summary>
    /// Query with Dapper in EF Core, support transaction if enabled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns></returns>
    public static IEnumerable<T> DapperQuery<T>(
        this DatabaseFacade database,
        string commandText,
        object param,
        bool buffered = true,
        int? commandTimeout = null,
        CommandType? commandType = null
    )
    {
        var cn = database.GetDbConnection();
        IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
        return cn.Query<T>(
            sql: commandText,
            param: param,
            transaction: trn,
            buffered: buffered,
            commandTimeout: commandTimeout,
            commandType: commandType
        );
    }

    /// <summary>
    /// Query with Dapper in EF Core asynchronously, support transaction if enabled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="database">DatabaseFacade</param>
    /// <param name="commandText">The SQL to execute for this query.</param>
    /// <param name="param">The parameters to use for this query.</param>
    /// <param name="buffered">Whether to buffer the results in memory.</param>
    /// <param name="commandTimeout">The command timeout (in seconds).</param>
    /// <param name="commandType">The type of command to execute.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> DapperQueryAsync<T>(
        this DatabaseFacade database,
        string commandText,
        object param,
        int? commandTimeout = null,
        CommandType? commandType = null
    )
    {
        var cn = database.GetDbConnection();
        IDbTransaction trn = database.CurrentTransaction?.GetDbTransaction()!;
        return await cn.QueryAsync<T>(
            sql: commandText,
            param: param,
            transaction: trn,
            commandTimeout: commandTimeout,
            commandType: commandType
        );
    }
}
