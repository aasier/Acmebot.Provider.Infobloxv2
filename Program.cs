using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
