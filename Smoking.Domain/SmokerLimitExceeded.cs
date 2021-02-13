using Newtonsoft.Json;

namespace Smoking.Domain
{
    public class SmokerLimitExceeded : Event
    {
        public const string EventType = "smoker.limitexceeded";

        [JsonProperty("type")]
        public override string Type { get => EventType; set { } }
    }
}
