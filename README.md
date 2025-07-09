# Custom DNS Infoblox Azure Function (C# .NET 8)

## Descripción

Azure Function App que implementa una API Custom DNS compatible con KeyVault-Acmebot, usando Infoblox WAPI como backend DNS.  
Permite gestionar zonas y registros DNS mediante los endpoints requeridos por KeyVault-Acmebot.

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)  
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)  
- Acceso y credenciales para la WAPI de Infoblox  

---

## Configuración local

1. Clonar el repositorio:

```bash
git clone <url-del-repositorio>
cd customdns-infoblox-azurefunc
Modificar local.settings.json con tus credenciales y URL de Infoblox:

json
Copy
Edit
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "INFOBLOX_WAPI_URL": "https://infoblox.example.com/wapi/v2.11",
    "INFOBLOX_USERNAME": "admin",
    "INFOBLOX_PASSWORD": "yourpassword"
  }
}
Compilar y probar localmente
Desde la raíz del proyecto:

Compila el proyecto:

bash
Copy
Edit
dotnet build
Ejecuta la Function App localmente:

bash
Copy
Edit
func start
La función estará disponible en:
http://localhost:7071/api

Puedes probar los endpoints con Postman, curl o similares.

Despliegue en Azure Function App
Crear recursos (si no tienes)
bash
Copy
Edit
az group create --name <resource-group> --location <region>

az storage account create --name <storage-account> --location <region> --resource-group <resource-group> --sku Standard_LRS

az appservice plan create --name <app-service-plan> --resource-group <resource-group> --sku S1 --is-linux false
Crear la Function App
bash
Copy
Edit
az functionapp create \
  --resource-group <resource-group> \
  --name <functionapp-name> \
  --plan <app-service-plan> \
  --runtime dotnet \
  --functions-version 4 \
  --storage-account <storage-account>
Publicar tu proyecto
bash
Copy
Edit
func azure functionapp publish <functionapp-name>
Configurar variables de entorno en Azure
bash
Copy
Edit
az functionapp config appsettings set --name <functionapp-name> --resource-group <resource-group> --settings \
INFOBLOX_WAPI_URL="https://infoblox.example.com/wapi/v2.11" \
INFOBLOX_USERNAME="admin" \
INFOBLOX_PASSWORD="yourpassword"
Endpoints disponibles
Método	Ruta	Descripción
GET	/zones	Listar zonas DNS
PUT	/zones/{zoneId}/records/{recordName}	Crear o actualizar registro TXT
DELETE	/zones/{zoneId}/records/{recordName}	Eliminar registro TXT
POST	/add	Añadir registro (KeyVault-Acmebot)
POST	/delete	Eliminar registro (KeyVault-Acmebot)

Ejemplo de request para crear un registro TXT
http
Copy
Edit
PUT /zones/example_com/records/_acme-challenge.example.com
Content-Type: application/json

{
  "type": "TXT",
  "ttl": 60,
  "values": ["xxxxxx", "yyyyyy"]
}
Configuración para KeyVault-Acmebot
Configura el proveedor DNS así:

json
Copy
Edit
{
  "type": "custom",
  "endpoint": "https://<functionapp-name>.azurewebsites.net/api"
}
Notas importantes
La autenticación contra Infoblox se realiza mediante Basic Auth.

Asegúrate que la Function App tenga acceso a la red donde esté Infoblox.

Usa HTTPS para seguridad.

Puedes testear con Postman o curl usando las URLs y métodos descritos.

La función está preparada para correr en Azure Functions v4 con .NET 8 en plan Windows.

