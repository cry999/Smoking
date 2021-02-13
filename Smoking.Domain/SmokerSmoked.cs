using Newtonsoft.Json;

namespace Smoking.Domain
{
    public class SmokerSmoked : Event
    {
        public const string EventType = "smoker.smoked";

        [JsonProperty("type")]
        public override string Type { get => EventType; set { } }
    }
}
