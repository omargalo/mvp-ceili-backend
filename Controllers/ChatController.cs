using Microsoft.AspNetCore.Mvc;
using Azure.AI.Inference;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatCompletionsClient _client;
    private readonly IConfiguration _config;

    public ChatController(ChatCompletionsClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string userMessage)
    {
        var options = new ChatCompletionsOptions
        {
            MaxTokens = 256,
            Temperature = 0.7f,
            Model = _config["AzureAI:Model"] ?? "gpt-4o-mini"
        };
        options.Messages.Add(new ChatRequestUserMessage(userMessage));

        try
        {
            var response = await _client.CompleteAsync(options);
            return Ok(response.Value.Content);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
