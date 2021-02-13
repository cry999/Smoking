using System;
using Newtonsoft.Json;

namespace Smoking.Domain
{
    public abstract class Event : EventArgs
    {
        [JsonProperty("type")]
        public abstract string Type { get; set; }

        [JsonProperty("occurred_on")]
        public DateTimeOffset OccurredOn { get; set; }

        [JsonProperty("aggregate_id")]
        public Guid AggregateID { get; set; }
    }
}
