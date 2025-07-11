using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Acmebot.Provider.Infobloxv2
{
    public class Add
    {
        private readonly InfobloxClient _client;

        public Add(InfobloxClient client) => _client = client;

        [Function("Add")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "add")] HttpRequestData req)
        {
            var data = await JsonSerializer.DeserializeAsync<DnsRequest>(req.Body);
            if (data?.Values == null || data.Values.Length == 0)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            // You may want to pass the record name in the body or as a parameter
            var recordName = data.Values[0];
            var zone = await _client.FindZoneAsync(recordName);
            if (zone is null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            await _client.UpsertTxtRecordAsync(zone, recordName, data.Values.ToList(), data.Ttl);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
