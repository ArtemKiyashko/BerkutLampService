using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tuya.Net;
using Tuya.Net.Data.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BerkutLampService;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(app => app.AddEnvironmentVariables())
    .ConfigureServices((host, services) => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<ITuyaClient>(factory => {
            var client = TuyaClient.GetBuilder()
                .UsingDataCenter(DataCenter.CentralEurope)
                .UsingClientId(host.Configuration.GetValue<string>("TUYA_CLIENT_ID")) // replace with your actual client id
                .UsingSecret(host.Configuration.GetValue<string>("TUYA_SECRET")) // replace with your actual client secret
                .UsingLogger(factory.GetRequiredService<ILogger<ITuyaClient>>())
                .Build();
            return client;
        });
        services.Configure<LampOptions>(host.Configuration.GetSection(nameof(LampOptions)));
    })
    .Build();

host.Run();
