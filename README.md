# Custom DNS Infoblox Azure Function (C# .NET 8)

## Descripción

Azure Function App que expone una API DNS compatible con KeyVault-Acmebot, usando Infoblox WAPI como backend. Permite gestionar zonas y registros TXT vía HTTP, facilitando la automatización de validaciones ACME y otras integraciones.

---

## Arquitectura

- **Lenguaje:** C# (.NET 8, Azure Functions Isolated Worker)
- **Backend DNS:** Infoblox WAPI
- **Endpoints:** HTTP REST compatibles con KeyVault-Acmebot
- **Despliegue:** Azure Function App (Windows)

---

## Dependencias

- .NET 8 SDK
- Azure Functions Core Tools v4
- Azure CLI
- System.Text.Json >= 9.0.7
- Microsoft.Azure.Functions.Worker >= 2.0.0

---

## Configuración local

1. Clona el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd <carpeta-del-proyecto>
   ```
2. Modifica `local.settings.json`:
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
3. Compila y ejecuta:
   ```bash
   dotnet build
   func start
   ```
   Accede a la API en: http://localhost:7071/api

---

## Despliegue en Azure (CLI)

1. Compila y publica:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
2. Crea recursos:
   ```cmd
   az group create --name <resource-group> --location <region>
   az storage account create --name <storage-account> --location <region> --resource-group <resource-group> --sku Standard_LRS
   az appservice plan create --name <app-service-plan> --resource-group <resource-group> --sku S1 --is-linux false
   az functionapp create --resource-group <resource-group> --name <functionapp-name> --plan <app-service-plan> --runtime dotnet --functions-version 4 --storage-account <storage-account>
   ```
3. Publica el proyecto:
   ```bash
   func azure functionapp publish <functionapp-name>
   ```
4. Configura variables de entorno:
   ```cmd
   az functionapp config appsettings set --name <functionapp-name> --resource-group <resource-group> --settings INFOBLOX_WAPI_URL="https://infoblox.example.com/wapi/v2.11" INFOBLOX_USERNAME="admin" INFOBLOX_PASSWORD="yourpassword"
   ```

---

## Endpoints disponibles

| Método | Ruta                                         | Descripción                        |
|--------|----------------------------------------------|------------------------------------|
| GET    | /zones                                      | Listar zonas DNS                   |
| PUT    | /zones/{zoneId}/records/{recordName}         | Crear o actualizar registro TXT    |
| DELETE | /zones/{zoneId}/records/{recordName}         | Eliminar registro TXT              |
| POST   | /add                                        | Añadir registro (KeyVault-Acmebot) |
| POST   | /delete                                     | Eliminar registro (KeyVault-Acmebot)|

---

## Ejemplo de uso

### Crear registro TXT
```sh
curl -X PUT "http://localhost:7071/api/zones/example_com/records/_acme-challenge.example.com" \
  -H "Content-Type: application/json" \
  -d '{"type": "TXT", "ttl": 60, "values": ["xxxxxx", "yyyyyy"]}'
```

### Eliminar registro TXT
```sh
curl -X DELETE "http://localhost:7071/api/zones/example_com/records/_acme-challenge.example.com"
```

### Usar clave de función (Azure)
```sh
curl -X GET "https://<functionapp-name>.azurewebsites.net/api/zones" -H "x-functions-key: <tu-clave>"
```

---

## Configuración para KeyVault-Acmebot

```json
{
  "type": "custom",
  "endpoint": "https://<functionapp-name>.azurewebsites.net/api"
}
```

---

## Seguridad

- La autenticación contra Infoblox se realiza mediante Basic Auth.
- Protege los endpoints con clave de función (`x-functions-key`) o Azure AD si lo requieres.
- Usa HTTPS en producción.
- Limita el acceso de red de la Function App solo a Infoblox y clientes autorizados.

---

## Pruebas

- Usa curl, Postman o cualquier cliente HTTP para probar los endpoints.
- Ejemplos incluidos arriba.
- Verifica los logs en Azure Portal o localmente para depuración.

---

## Soporte y mantenimiento

- Actualiza dependencias regularmente (`dotnet list package --outdated`).
- Revisa alertas de seguridad en los paquetes NuGet.
- Para dudas o incidencias, abre un issue en el repositorio.

