using System;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Acmebot.Provider.Infoblox
{
    public class Health
    {
        private readonly InfobloxClient _client;

        public Health(InfobloxClient client) => _client = client;

        /// <summary>
        /// Azure Function: Health check, verifica conectividad con Infoblox, lista dominios y prueba crear/borrar un registro TXT.
        /// </summary>
        [Function("Health")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequestData req)
        {
            var res = req.CreateResponse();
            var sb = new StringBuilder();
            try
            {
                // Paso 1: Conexión y listado de zonas
                var zones = await _client.GetZonesAsync();
                if (zones.Count == 0)
                {
                    sb.AppendLine("connected to Infoblox KO: No zones found");
                    res.StatusCode = HttpStatusCode.ServiceUnavailable;
                    await res.WriteStringAsync(sb.ToString());
                    return res;
                }
                sb.AppendLine("connected to Infoblox OK");
                var firstZone = zones[0].GetProperty("fqdn").GetString();
                var testRecord = $"_healthcheck.{firstZone}";
                var testValue = $"health-{Guid.NewGuid()}";

                // Paso 2: Crear registro TXT
                try
                {
                    await _client.UpsertTxtRecordAsync(firstZone, testRecord, new System.Collections.Generic.List<string> { testValue }, 60);
                    var txtRecords = await _client.GetTxtRecordsAsync(testRecord);
                    bool created = txtRecords.Any(r => r.GetProperty("text").GetString() == testValue);
                    if (!created)
                    {
                        sb.AppendLine("created register KO");
                        res.StatusCode = HttpStatusCode.ServiceUnavailable;
                        await res.WriteStringAsync(sb.ToString());
                        return res;
                    }
                    sb.AppendLine("created register OK");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"created register KO: {ex.Message}");
                    res.StatusCode = HttpStatusCode.ServiceUnavailable;
                    await res.WriteStringAsync(sb.ToString());
                    return res;
                }

                // Paso 3: Borrar registro TXT
                try
                {
                    await _client.DeleteTxtRecordsAsync(testRecord);
                    sb.AppendLine("deleted register OK");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"deleted register KO: {ex.Message}");
                    res.StatusCode = HttpStatusCode.ServiceUnavailable;
                    await res.WriteStringAsync(sb.ToString());
                    return res;
                }

                res.StatusCode = HttpStatusCode.OK;
                await res.WriteStringAsync(sb.ToString());
                return res;
            }
            catch (Exception ex)
            {
                sb.AppendLine($"connected to Infoblox KO: {ex.Message}");
                res.StatusCode = HttpStatusCode.ServiceUnavailable;
                await res.WriteStringAsync(sb.ToString());
                return res;
            }
        }
    }
}
