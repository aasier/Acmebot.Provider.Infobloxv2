using System.Text.Json.Serialization;

namespace Acmebot.Provider.Infoblox;

/// <summary>
/// Modelo para peticiones de registro DNS (TXT).
/// </summary>
public class DnsRequest
{
    [JsonPropertyName("values")]
    public string[]? Values { get; set; }
    public string Type { get; set; } = "TXT";
    public int Ttl { get; set; } = 60;
}
