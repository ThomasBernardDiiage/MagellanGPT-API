using AutoMapper;
using Azure.Storage.Blobs;
using Magellan.Api.DependencyInjection;
using Magellan.Api.Hubs;
using Magellan.Api.Mapping;
using Magellan.Api.Middlewares;
using Magellan.Api.Services;
using Magellan.Api.Services.Interfaces;
using Magellan.DataAccess;
using Magellan.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.KernelMemory;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AzureAd:Authority"];
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true
        };
    });


builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ConversationMapping>();
    config.AddProfile<ModelMapping>();
    config.AddProfile<MessageMapping>();
});

builder.Services
    .AddSwagger()
    .AddAppInsightsServices(builder.Configuration)
    .AddKernelMemoryServices(builder.Configuration)
    .AddOpenAIClient(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddSignalR();

builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

builder.Services.AddControllers();

var cosmosDbClient = new CosmosClient(builder.Configuration["CosmosDb:URI"], builder.Configuration["CosmosDb:PrimaryKey"]);
var containerClient = cosmosDbClient.GetContainer(builder.Configuration["CosmosDb:DatabaseName"], "user");
builder.Services.AddSingleton(containerClient);

// Completion Model GPT 3
builder.Services
    .AddKernel()
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["AzureOpenAiGpt3:Deployment"]!,
        builder.Configuration["AzureOpenAiGpt3:EndPoint"]!,
        builder.Configuration["AzureOpenAiGpt3:ApiKey"]!,
        serviceId: "gpt-3");

// Completion Model GPT 4
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["AzureOpenAiGpt4:Deployment"]!,
        builder.Configuration["AzureOpenAiGpt4:EndPoint"]!,
        builder.Configuration["AzureOpenAiGpt4:ApiKey"]!,
        serviceId: "gpt-4");

builder.Services.AddScoped<MemoryServerless>(_ => new KernelMemoryBuilder()
    .WithAzureOpenAITextGeneration(
        new AzureOpenAIConfig
        {
            APIKey = builder.Configuration["AzureOpenAiGpt3:ApiKey"]!,
            Endpoint = builder.Configuration["AzureOpenAiGpt3:EndPoint"]!,
            Deployment = builder.Configuration["AzureOpenAiGpt3:Deployment"]!,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        }
    ).WithAzureOpenAITextEmbeddingGeneration(
        new AzureOpenAIConfig
        {
            APIKey = builder.Configuration["AzureOpenAiTextEmbedding:ApiKey"]!,
            Endpoint = builder.Configuration["AzureOpenAiTextEmbedding:EndPoint"]!,
            Deployment = builder.Configuration["AzureOpenAiTextEmbedding:Deployment"]!,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        }
    ).Build<MemoryServerless>());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("healthz");


app.UseAuthentication();
app.UseAuthorization();

app.MapHub<QuotaHub>("/api/quota");

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();