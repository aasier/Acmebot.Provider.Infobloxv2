using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

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
