using System.Net;
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
    public class Zones
    {
        private readonly InfobloxClient _client;

        public Zones(InfobloxClient client) => _client = client;

        [Function("Zones")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "zones")] HttpRequestData req)
        {
            var zones = await _client.GetZonesAsync();
            var result = zones.Select(z => new {
                id = z.fqdn.ToString().Replace(".", "_").TrimEnd('.'),
                name = z.fqdn.ToString().TrimEnd('.'),
                nameServers = z.name_servers ?? Array.Empty<string>()
            });

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(result);
            return res;
        }
    }
}
