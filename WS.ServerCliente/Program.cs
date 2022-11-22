using Application.Services;
using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Config;
using StartupService;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddApplication();
        services.AddScoped<IConfig, Config>();
       //services.AddHostedService<MqttServiceConsumer>();
        services.AddScoped<IVariable, Variables>(x => new Variables(args[1]));
        services.AddHostedService<Startup>(x => new Startup(x.GetRequiredService<IConfig>(), args[0]));
    })
    .Build()
    .RunAsync();
