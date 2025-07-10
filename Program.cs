using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Aquí puedes registrar dependencias si lo necesitas
    })
    .Build();

host.Run();
