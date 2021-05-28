using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CA2227 // Necessary for deserialization.

namespace Functions.Sidecar.Models
{
    /// <summary>
    /// Probe functions result model.
    /// </summary>
    public class ProbeFunctionsResult
    {
        /// <summary>
        /// Gets or sets if the probe was successful.
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the probed functions.
        /// </summary>
        [JsonProperty(PropertyName = "functions")]
        public IDictionary<string, ProbeFunctionInfo> Functions { get; set; }
    }
}
