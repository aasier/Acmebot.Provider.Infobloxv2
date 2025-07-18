using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Acmebot.Provider.Infoblox
{
    public class DeleteRecord
    {
        private readonly InfobloxClient _client;

        public DeleteRecord(InfobloxClient client) => _client = client;

        /// <summary>
        /// Azure Function: Elimina registro TXT por zona y nombre.
        /// </summary>
        [Function("DeleteRecord")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "zones/{zoneId}/records/{recordName}")] HttpRequestData req,
            string zoneId, string recordName)
        {
            await _client.DeleteTxtRecordsAsync(recordName);
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
