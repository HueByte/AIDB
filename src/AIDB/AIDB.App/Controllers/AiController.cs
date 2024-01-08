using AIDB.App.ViewModels;
using AIDB.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIDB.App.Controllers;

public class AiController : BaseController
{
    private IDynamicAiQueryExecutorService _dynamicAiQueryExecutorService;

    public AiController(IDynamicAiQueryExecutorService dynamicAiQueryExecutorService)
    {
        _dynamicAiQueryExecutorService = dynamicAiQueryExecutorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDatabaseQuery([FromBody] string prompt)
    {
        var result = await _dynamicAiQueryExecutorService.CreateDatabaseQueryAsync(prompt);

        AiQueryResult aiQueryResult = new()
        {
            AiCommand = result.AiCommand,
            AiCommandId = result.Id
        };

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ExecuteDatabaseQuery([FromQuery] string queryId)
    {
        var result = await _dynamicAiQueryExecutorService.ExecuteAIQueryAsync(queryId);

        return Ok(result);
    }
}