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
            if (data is null)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            await _client.DeleteTxtRecordsAsync(data.Name);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
