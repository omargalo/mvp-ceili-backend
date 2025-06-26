using Azure.AI.Inference;
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
            Model = _config["AzureAI:Model"] ?? "gpt-4o-mini"
        };
        options.Messages.Add(new ChatRequestUserMessage(userMessage));
        var response = await _client.CompleteAsync(options);
        return response.Value.Content;
    }
}
