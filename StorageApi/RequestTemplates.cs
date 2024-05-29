using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageApi
{
    internal static class RequestTemplates
    {
        public static HttpRequestMessage GetUptimeKumaTemplate(string endpoint, HttpMethod method, double diskUsage, bool inWarning)
        {
            var uriBuilder = new UriBuilder(endpoint);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            queryParams.Set("status", inWarning ? "down" : "up");
            queryParams.Set("msg", double.Round(diskUsage, 1).ToString());
            queryParams.Set("ping", ((int)double.Round(diskUsage)).ToString());

            uriBuilder.Query = queryParams.ToString();

            return new HttpRequestMessage(new System.Net.Http.HttpMethod(method.ToString()), uriBuilder.ToString());
        }
    }
}
