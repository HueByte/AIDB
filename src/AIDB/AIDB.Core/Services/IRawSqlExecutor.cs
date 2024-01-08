namespace AIDB.Core.Services;

public interface IRawSqlExecutor
{
    Task<string?> ExecuteRawSqlAsync(string sql);
}