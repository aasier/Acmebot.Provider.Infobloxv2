using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class Add
{
    private readonly InfobloxClient _client;

    public Add(InfobloxClient client) => _client = client;

    [Function("Add")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "add")] HttpRequestData req)
    {
        var data = await JsonSerializer.DeserializeAsync<DnsRequest>(req.Body);
        if (data is null || data.Value is null)
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var zone = await _client.FindZoneAsync(data.Name);
        if (zone is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        await _client.UpsertTxtRecordAsync(zone, data.Name, new List<string> { data.Value }, data.Ttl);
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
