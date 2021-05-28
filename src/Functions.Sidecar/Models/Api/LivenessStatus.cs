using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CA2227 // Necessary for deserialization.

namespace Functions.Sidecar.Models.Api
{
    /// <summary>
    /// Liveness status model.
    /// </summary>
    public class LivenessStatus
    {
        /// <summary>
        /// Gets or sets the Liveness.
        /// </summary>
        [JsonProperty(PropertyName = "alive")]
        public bool Alive { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
    }
}
