using Newtonsoft.Json;

namespace Functions.Sidecar.Models.Api
{
    /// <summary>
    /// Startup status model.
    /// </summary>
    public class StartupStatus
    {
        /// <summary>
        /// Gets or sets the startup state.
        /// </summary>
        [JsonProperty(PropertyName = "running")]
        public bool Running { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
