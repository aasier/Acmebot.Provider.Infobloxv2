using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient(); // Necesario para IHttpClientFactory
        services.AddSingleton<InfobloxClient>();
    })
    .Build();

host.Run();
