using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace Acmebot.Provider.Infobloxv2
{
    public class Health
    {
        private readonly InfobloxClient _client;

        public Health(InfobloxClient client) => _client = client;

        [Function("Health")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequestData req)
        {
            try
            {
                // Intenta obtener las zonas como prueba de conectividad
                await _client.GetZonesAsync();
                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteStringAsync("Healthy: Connected to Infoblox WAPI");
                return res;
            }
            catch (Exception ex)
            {
                var res = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
                await res.WriteStringAsync($"Unhealthy: {ex.Message}");
                return res;
            }
        }
    }
}
