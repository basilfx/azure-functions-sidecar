using Newtonsoft.Json;

#pragma warning disable CA2227 // Necessary for deserialization.

namespace Functions.Sidecar.Models
{
    /// <summary>
    /// Probe host result model.
    /// </summary>
    public class ProbeHostResult
    {
        /// <summary>
        /// Gets or sets if the probe was successful.
        /// </summary>
        [JsonProperty(PropertyName = "Success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
