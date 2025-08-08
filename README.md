# Custom DNS Infoblox Azure Function (C# .NET 8)

## Description

Azure Function App that exposes a DNS API compatible with KeyVault-Acmebot, using Infoblox WAPI as the backend. It allows you to manage zones and TXT records via HTTP, facilitating the automation of DNS-01 challenges in Let's Encrypt.

---

## Architecture

- **Language:** C# (.NET 8, Azure Functions Isolated Worker)
- **DNS Backend:** Infoblox WAPI
- **Endpoints:** HTTP REST compatible with KeyVault-Acmebot
- **Deployment:** Azure Function App (Windows)

---

## Dependencies

- .NET 8 SDK
- Azure Functions Core Tools v4
- Azure CLI
- System.Text.Json >= 9.0.7
- Microsoft.Azure.Functions.Worker >= 2.0.0

---

## Local configuration

1. Clone the repository:
    ```bash
    git clone <repository-url>
    cd <project-folder>
    ```
2. Edit `local.settings.json`:
    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "INFOBLOX_WAPI_URL": "https://infoblox.example.com/wapi/v2.11",
        "INFOBLOX_USERNAME": "admin",
        "INFOBLOX_PASSWORD": "yourpassword"
      }
    }
    ```
3. Build and run:
    ```bash
    dotnet build
    func start
    ```
    Access the API at: http://localhost:7071/api

---

## Deploy to Azure (CLI)

1. Build and publish:
    ```bash
    dotnet publish -c Release -o ./publish
    ```
2. Create resources:
    ```cmd
    az group create --name <resource-group> --location <region>
    az storage account create --name <storage-account> --location <region> --resource-group <resource-group> --sku Standard_LRS
    az appservice plan create --name <app-service-plan> --resource-group <resource-group> --sku S1 --is-linux false
    az functionapp create --resource-group <resource-group> --name <functionapp-name> --plan <app-service-plan> --runtime dotnet --functions-version 4 --storage-account <storage-account>
    ```
3. Publish the project:
    ```bash
    func azure functionapp publish <functionapp-name>
    ```
4. Set environment variables:
    ```cmd
    az functionapp config appsettings set --name <functionapp-name> --resource-group <resource-group> --settings INFOBLOX_WAPI_URL="https://infoblox.example.com/wapi/v2.11" INFOBLOX_USERNAME="admin" INFOBLOX_PASSWORD="yourpassword"
    ```

---

## Available Endpoints

| Method | Path                                         | Description                              |
|--------|----------------------------------------------|------------------------------------------|
| GET    | /zones                                      | List DNS zones                           |
| PUT    | /zones/{zoneId}/records/{recordName}         | Create or update TXT record              |
| DELETE | /zones/{zoneId}/records/{recordName}         | Delete TXT record                        |
| POST   | /add                                        | Add record (KeyVault-Acmebot)            |
| POST   | /delete                                     | Delete record (KeyVault-Acmebot)         |
| GET    | /health                                     | Service health check                     |

---

## Usage examples

### Create TXT record
```sh
curl -X PUT "http://localhost:7071/api/zones/example_com/records/_acme-challenge.example.com" \
  -H "Content-Type: application/json" \
  -d '{"type": "TXT", "ttl": 60, "values": ["xxxxxx", "yyyyyy"]}'
```

### Delete TXT record
```sh
curl -X DELETE "http://localhost:7071/api/zones/example_com/records/_acme-challenge.example.com"
```

### Use function key (Azure)
```sh
curl -X GET "https://<functionapp-name>.azurewebsites.net/api/zones" -H "x-functions-key: <your-key>"
```

---

## KeyVault-Acmebot configuration

```json
{
  "type": "custom",
  "endpoint": "https://<functionapp-name>.azurewebsites.net/api"
}
```

#### Health Check Example
```sh
curl -X GET "http://localhost:7071/api/health"
```
**Sample response:**
```json
{
  "status": "Healthy",
  "timestamp": "2025-08-08T19:41:32Z"
}
```

---

## Security

- Authentication against Infoblox is done via Basic Auth.
- Protect the endpoints with a function key (`x-functions-key`) or Azure AD if required.
- Use HTTPS in production.
- Restrict Function App network access only to Infoblox and authorized clients.

---

## Testing

- Use curl, Postman, or any HTTP client to test the endpoints.
- Examples are included above.
- Check logs in Azure Portal or locally for debugging.

---

## Support and Maintenance

- Update dependencies regularly (`dotnet list package --outdated`).
- Review security alerts in NuGet packages.
- For questions or issues, open an issue in the repository.
