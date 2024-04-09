# MagellanGPT API

Endpoints :

# Magellan.Api
## Version: 1.0

### /api/conversations

#### GET
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/conversations/{id}

#### DELETE
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path |  | Yes | string (uuid) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

#### GET
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path |  | Yes | string (uuid) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/documents

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/conversations/{id}/messages

#### POST
##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path |  | Yes | string (uuid) |

##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/models

#### GET
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/tcu/questions

#### GET
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/tcu/questions/answers

#### POST
##### Responses

| Code | Description |
| ---- | ----------- |
| 200 | Success |


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
