using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StorageApi
{
    public class StorageApiConfig
    {
        public Push[] Pushes { get; set; }
    }

    public class Push
    {
        public string Path { get; set; }

        public string Endpoint { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HttpMethod Method { get; set; } = HttpMethod.GET;

        public int FrequencySeconds { get; set; }

        public int WarningLimitPercent { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageTemplate Template { get; set; }
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUSH,
    }

    public enum MessageTemplate
    {
        UptimeKuma
    }
}
