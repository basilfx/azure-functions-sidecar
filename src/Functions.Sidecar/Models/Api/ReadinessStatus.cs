using Newtonsoft.Json;

namespace Functions.Sidecar.Models.Api
{
    /// <summary>
    /// Readiness status model.
    /// </summary>
    public class ReadinessStatus
    {
        /// <summary>
        /// Gets or sets the readiness.
        /// </summary>
        [JsonProperty(PropertyName = "ready")]
        public bool Ready { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
