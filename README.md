# MagellanGPT API


Pour faire fonctionner le projet, il faut créer un appsettings.json
```json
// https://github.com/microsoft/chat-copilot/blob/main/webapi/appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.SignalR": "Debug",
      "Microsoft.AspNetCore.Http.Connections": "Debug"
    }
  },
  "AllowedHosts": "*",
  "CosmosDb": {
    "URI": "",
    "PrimaryKey": "",
    "DatabaseName": ""
  },
  "AzureAd": {
    "Authority": "",
    "Audience": ""
  },
  "AzureOpenAiGpt3": {
    "APIKey": "",
    "EndPoint": "",
    "Deployment": "",
    "Auth": "ApiKey"
  },
  "AzureOpenAiGpt4": {
    "APIKey": "",
    "EndPoint": "",
    "Deployment": "",
    "Auth": "ApiKey"
  },
  "AzureOpenAiTextEmbedding": {
    "APIKey": "",
    "EndPoint": "",
    "Deployment": "",
    "Auth": "ApiKey"
  },
  "BlobStorageAzure": {
    "ConnectionString": "",
    "Container": "",
    "Auth": "ConnectionString"
  },
  "AzureAiSearch": {
    "Auth": "ApiKey",
    "APIKey": "",
    "IndexName": "",
    "ServiceName": "",
    "Endpoint": ""
  },
  "MaxQuota": 1000
}
```
