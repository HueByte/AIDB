using AIDB.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIDB.App.Controllers;

public record class RawSqlRequest(string Query);

public class RawSqlController : BaseController
{
    private readonly IRawSqlExecutor _rawSqlExecutor;

    public RawSqlController(IRawSqlExecutor rawSqlExecutor)
    {
        _rawSqlExecutor = rawSqlExecutor;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteRawSql([FromBody] RawSqlRequest request)
    {
        var result = await _rawSqlExecutor.ExecuteRawSqlAsync(request.Query);

        return Ok(result);
    }
}