using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Acmebot.Provider.Infoblox
{
    /// <summary>
    /// Entry point for Azure Functions Isolated Worker. Configures DI and Infoblox client.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient("Infoblox")
                        .ConfigurePrimaryHttpMessageHandler(() =>
                            new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                            });
                    services.AddSingleton<InfobloxClient>(sp =>
                        new InfobloxClient(
                            sp.GetRequiredService<IConfiguration>(),
                            sp.GetRequiredService<IHttpClientFactory>()
                        )
                    );
                })
                .Build();

            host.Run();
        }
    }
}
