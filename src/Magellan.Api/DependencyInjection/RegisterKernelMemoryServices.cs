using Microsoft.KernelMemory;

namespace Magellan.Api.DependencyInjection;

public static class RegisterKernelMemoryServices
{
    public static IServiceCollection AddKernelMemoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var azureOpenAITextConfig = new AzureOpenAIConfig();
        var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig();
        var azureAISearch = new AzureAISearchConfig();
        var azureBlobConfig = new AzureBlobsConfig();
        
        
        configuration
            .BindSection("AzureOpenAiGpt3", azureOpenAITextConfig)
            .BindSection("AzureOpenAiTextEmbedding", azureOpenAIEmbeddingConfig)
            .BindSection("AzureAiSearch", azureAISearch)
            .BindSection("BlobStorageAzure", azureBlobConfig);
        
        var memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(azureOpenAITextConfig)
            .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig)
            .WithAzureAISearchMemoryDb(azureAISearch)
            .WithAzureBlobsStorage(azureBlobConfig)
            .Build();

        return services.AddSingleton(memory);
    }
}