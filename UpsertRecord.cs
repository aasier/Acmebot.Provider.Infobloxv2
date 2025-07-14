using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Acmebot.Provider.Infobloxv2
{
    public class UpsertRecord
    {
        private readonly InfobloxClient _client;

        public UpsertRecord(InfobloxClient client) => _client = client;

        /// <summary>
        /// Azure Function: Crea o actualiza registro TXT en Infoblox.
        /// </summary>
        [Function("UpsertRecord")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "zones/{zoneId}/records/{recordName}")] HttpRequestData req,
            string zoneId, string recordName)
        {
            var data = await JsonSerializer.DeserializeAsync<DnsRequest>(req.Body);
            if (data?.Values == null)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var zone = zoneId.Replace("_", ".");
            await _client.UpsertTxtRecordAsync(zone, recordName, data.Values.ToList(), data.Ttl);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
