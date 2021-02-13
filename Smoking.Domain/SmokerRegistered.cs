using System;
using Newtonsoft.Json;

namespace Smoking.Domain
{
    public class SmokerRegistered : Event
    {
        public const string EventType = "smoker.registered";

        [JsonProperty("type")]
        public override string Type { get => EventType; set { } }

        [JsonProperty("limit_per_day")]
        public int LimitPerDay { get; set; }

        [JsonProperty("interval")]
        public TimeSpan Interval { get; set; }
    }
}
