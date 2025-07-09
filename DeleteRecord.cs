using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class DeleteRecord
{
    private readonly InfobloxClient _client;

    public DeleteRecord(InfobloxClient client) => _client = client;

    [Function("DeleteRecord")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "zones/{zoneId}/records/{recordName}")] HttpRequestData req,
        string zoneId, string recordName)
    {
        await _client.DeleteTxtRecordsAsync(recordName);
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
