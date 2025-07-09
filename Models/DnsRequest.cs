public class DnsRequest
{
    public string Name { get; set; } = null!;
    public string? Value { get; set; }
    public string[]? Values { get; set; }
    public string Type { get; set; } = "TXT";
    public int Ttl { get; set; } = 60;
}
