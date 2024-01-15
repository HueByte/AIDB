using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Text.Json;
using AIDB.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace AIDB.Infrastructure.Services;

public class RawSqlExecutor : IRawSqlExecutor
{
    private readonly AIDBMainContext _context;

    public RawSqlExecutor(AIDBMainContext context)
    {
        _context = context;
    }

    public async Task<string?> ExecuteRawSqlAsync(string sql)
    {
        // This is only concept testing, we don't care about SQL injection here :)
        // var rawResult = await _context.Database.ExecuteSqlRawAsync(sql);
        var dynamicResult = DynamicResultQuery(_context, sql, new Dictionary<string, object>()).ToList();

        return JsonSerializer.Serialize(dynamicResult);
    }

    private static IEnumerable<dynamic?> DynamicResultQuery(DbContext db, string Sql, Dictionary<string, object> Params)
    {
        using var cmd = db.Database.GetDbConnection().CreateCommand();

        if (cmd.Connection is null) yield return null;

        cmd.CommandText = Sql;

        if (cmd.Connection?.State != ConnectionState.Open)
            cmd.Connection?.Open();

        foreach (KeyValuePair<string, object> p in Params)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.ParameterName = p.Key;
            dbParameter.Value = p.Value;
            cmd.Parameters.Add(dbParameter);
        }

        using var dataReader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
        while (dataReader.Read())
        {
            var row = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldIteration = 0; fieldIteration < dataReader.FieldCount; fieldIteration++)
            {
                var key = dataReader.GetName(fieldIteration);
                if (row.ContainsKey(key))
                {
                    var tableName = dataReader.GetSchemaTable()?.TableName;
                    if (string.IsNullOrEmpty(tableName) || tableName == "SchemaTable")
                    {
                        tableName = fieldIteration.ToString();
                    }

                    row.Add($"{key}_{tableName}", dataReader[fieldIteration]);
                }
                else
                {
                    row.Add(key, dataReader[fieldIteration]);
                }
            }

            yield return row;
        }
    }
}

