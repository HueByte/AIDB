using System.Reflection.Metadata;
using AIDB.Core.IRepositories;
using AIDB.Core.Models;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace AIDB.Core.Services;

public class DynamicAiQueryExecutorService : IDynamicAiQueryExecutorService
{
    private readonly OpenAIClient _openAiClient;
    private readonly IAiDbCommandRepository _aiDbCommandRepository;
    private readonly IRawSqlExecutor _rawSQLExecutor;

    public DynamicAiQueryExecutorService(OpenAIClient openAiClient, IAiDbCommandRepository aiDbCommandRepository, IRawSqlExecutor rawSQLExecutor)
    {
        _openAiClient = openAiClient;
        _aiDbCommandRepository = aiDbCommandRepository;
        _rawSQLExecutor = rawSQLExecutor;
    }

    public async Task<AiDbCommand> CreateDatabaseQueryAsync(string prompt)
    {
        string systemCommand = await File.ReadAllTextAsync(Path.Join(AppContext.BaseDirectory, "Data", "SystemCommand.txt"));
        List<Message> messages = new()
        {
            new Message(Role.System, systemCommand),
            new Message(Role.User, prompt)
        };

        var chatRequest = new ChatRequest(messages, Model.GPT4);
        var response = await _openAiClient.ChatEndpoint.GetCompletionAsync(chatRequest);
        var choice = response.FirstChoice;

        var aiDbCommand = new AiDbCommand
        {
            AiCommand = choice,
            CreatedAt = DateTime.UtcNow
        };

        await _aiDbCommandRepository.AddAsync(aiDbCommand);
        await _aiDbCommandRepository.SaveChangesAsync();

        return aiDbCommand;
    }

    public async Task<string?> ExecuteAIQueryAsync(string resultId)
    {
        var aiDbCommand = await _aiDbCommandRepository.GetAsync(resultId);
        if (aiDbCommand is null || aiDbCommand.AiCommand is null)
        {
            throw new ArgumentNullException(nameof(aiDbCommand), "AiDbCommand is null");
        }

        if (aiDbCommand.AiCommand is not null)
        {
            aiDbCommand.AiCommand = aiDbCommand.AiCommand.Replace("```sql", "");
            aiDbCommand.AiCommand = aiDbCommand.AiCommand.Replace("```", "");
            aiDbCommand.AiCommand = aiDbCommand.AiCommand.Replace("\n", " ");
        }

        var result = await _rawSQLExecutor.ExecuteRawSqlAsync(aiDbCommand.AiCommand!);

        return result;
    }
}

public interface IDynamicAiQueryExecutorService
{
    Task<AiDbCommand> CreateDatabaseQueryAsync(string prompt);
    Task<string?> ExecuteAIQueryAsync(string resultId);
}