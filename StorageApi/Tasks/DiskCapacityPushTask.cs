using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageApi.Tasks
{
    public class DiskCapacityPushTask : IInvocable
    {
        private readonly ILogger<DiskCapacityPushTask> _logger;

        private readonly string _directory;
        private readonly string _endpoint;
        private readonly HttpMethod _method;
        private readonly int _warningLimit;
        private readonly MessageTemplate _reponseTemplate;
        
        private static readonly HttpClient client = new HttpClient();

        public DiskCapacityPushTask(
            ILogger<DiskCapacityPushTask> logger,
            string directory, string endpoint, HttpMethod method, int warningLimit, MessageTemplate template)
        {
            _logger = logger;
            _directory = directory;
            _endpoint = endpoint;
            _method = method;
            _warningLimit = warningLimit;
            _reponseTemplate = template;
        }

        public async Task Invoke()
        {
            _logger.LogDebug("Checking capacity for {dir}.", _directory);
            var driveInfo = new DriveInfo(_directory);
            var freeSpace = driveInfo.TotalFreeSpace;

            var percentageDiskUsed = (driveInfo.TotalSize - freeSpace) / (double)driveInfo.TotalSize * 100;
            var inWarning = percentageDiskUsed >= _warningLimit;
            
            _logger.LogInformation("Directory {dir} has {percent}% capacity remaining.", _directory, double.Round(percentageDiskUsed,1));

            //var request = GetResponseTemplate(_reponseTemplate);
            var request = RequestTemplates.GetUptimeKumaTemplate(_endpoint, _method, percentageDiskUsed, inWarning);
            try
            {
                await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed {method} to {endpoint}. {exception}", _method, _endpoint, ex.Message);
            }
        }

        //private HttpRequestMessage GetResponseTemplate(MessageTemplate? responseTemplate) =>
        //    _reponseTemplate switch
        //    {
        //        MessageTemplate.UptimeKuma => RequestTemplates.GetUptimeKumaTemplate(_endpoint, _method, percentageDiskUsed)
        //    }
    }
}
