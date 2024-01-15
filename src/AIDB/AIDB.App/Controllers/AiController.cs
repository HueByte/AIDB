using AIDB.App.ViewModels;
using AIDB.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIDB.App.Controllers;

public record class AiPrompt(string Message);

public class AiController : BaseController
{
    private IDynamicAiQueryExecutorService _dynamicAiQueryExecutorService;

    public AiController(IDynamicAiQueryExecutorService dynamicAiQueryExecutorService)
    {
        _dynamicAiQueryExecutorService = dynamicAiQueryExecutorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDatabaseQuery([FromBody] AiPrompt prompt)
    {
        var result = await _dynamicAiQueryExecutorService.CreateDatabaseQueryAsync(prompt.Message);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ExecuteDatabaseQuery([FromQuery] string queryId)
    {
        var result = await _dynamicAiQueryExecutorService.ExecuteAIQueryAsync(queryId);

        return Ok(result);
    }
}