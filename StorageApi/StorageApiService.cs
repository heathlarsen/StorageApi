using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StorageApi.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageApi;

internal class StorageApiService : BackgroundService
{
    private readonly ILogger<StorageApiService> _logger;
    private readonly StorageApiConfig _config;
    private readonly IScheduler _scheduler;

    public StorageApiService(ILogger<StorageApiService> logger, StorageApiConfig config, IScheduler scheduler)
    {
        _logger = logger;
        _config = config;
        _scheduler = scheduler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Storage API service started");

        await ConfigureSchedules(stoppingToken);

        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    try
        //    {
        //        await Task.Delay(10_000, stoppingToken);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogWarning(e, "Failed to run puppet master task.");
        //    }
        //}
    }

    private async Task ConfigureSchedules(CancellationToken stoppingToken)
    {
        foreach (var item in _config.Pushes.Select((push, i) => new { i, push }))
        {
            _scheduler.ScheduleWithParams<DiskCapacityPushTask>(item.push.Path, item.push.Endpoint, item.push.Method, item.push.Template, item.push.WarningLimitPercent)
                .EverySeconds(item.push.FrequencySeconds)
                .RunOnceAtStart()
                .PreventOverlapping($"{nameof(DiskCapacityPushTask)}.{item.i}");
        }

        //_scheduler.ScheduleAsync(
        //    async () => _logger.LogInformation("Executed on 2 second schedule")
        //).EverySeconds(59)
    }
}
