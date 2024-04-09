using Azure;
using Azure.AI.OpenAI;

namespace Magellan.Api.DependencyInjection;

public static class RegisterOpenAIClientServices
{
    public static IServiceCollection AddOpenAIClient(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["AzureOpenAiGpt3:ApiKey"]!;
        var endpoint = new Uri(configuration["AzureOpenAiGpt3:EndPoint"]!);
        var credentials = new AzureKeyCredential(apiKey);
        
        return services.AddSingleton(new OpenAIClient(endpoint, credentials));
    }
}