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
    public class Delete
    {
        private readonly InfobloxClient _client;

        public Delete(InfobloxClient client) => _client = client;

        [Function("Delete")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "delete")] HttpRequestData req)
        {
            var data = await JsonSerializer.DeserializeAsync<DnsRequest>(req.Body);
            if (data?.Values == null || data.Values.Length == 0)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            // Use the first value as the record name to delete
            var recordName = data.Values[0];
            await _client.DeleteTxtRecordsAsync(recordName);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
