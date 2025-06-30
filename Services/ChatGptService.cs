using Azure.AI.Inference;
using CeiliApi.Business;
using CeiliApi.Models.Entities;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class ChatGptService
{
    private readonly ChatCompletionsClient _client;
    private readonly IConfiguration _config;

    public ChatGptService(ChatCompletionsClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<string> GetChatResponseAsync(string userMessage)
    {
        var options = new ChatCompletionsOptions
        {
            MaxTokens = 1500,
            Temperature = 0.7f,
            Model = _config["AzureAI:Model"]
        };
        options.Messages.Add(new ChatRequestUserMessage(userMessage));
        var response = await _client.CompleteAsync(options);
        return response.Value.Content;
    }

    public async Task<string> GenerarRetroalimentacionIAAsync(Evaluacion evaluacion)
    {
        // Lógica de negocio: construcción de prompt y análisis de riesgo
        var (prompt, nivelGlobal) = RiesgoAcademicoAnalyzer.ConstruirPrompt(evaluacion);

        // Llama a la IA
        return await GetChatResponseAsync(prompt);
    }
}
