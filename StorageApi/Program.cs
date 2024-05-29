using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StorageApi.Tasks;
using System.Text.Json;

namespace StorageApi;

internal class Program
{
    public static string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Storage API");

    static void Main(string[] args)
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = logger;

        try
        {
            var configFilePath = Path.Combine(AppDataDirectory, "config.json");

            if (!File.Exists(configFilePath))
                throw new Exception("Missing configuration");

            var configText = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<StorageApiConfig>(configText) ?? throw new Exception("Invalid configuration");

            using var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(config);
                    services.AddHostedService<StorageApiService>();
                    //services.AddTransient<DiskCapacityPushTask>();
                    services.AddScheduler();
                })
                .Build();

            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}