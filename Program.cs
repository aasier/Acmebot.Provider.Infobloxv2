using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient(); // Necesario para IHttpClientFactory
        services.AddSingleton<InfobloxClient>(sp =>
            new InfobloxClient(
                sp.GetRequiredService<IConfiguration>(),
                sp.GetRequiredService<IHttpClientFactory>()
            )
        );
    })
    .Build();

host.Run();
