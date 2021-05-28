using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CA2227 // Necessary for deserialization.

namespace Functions.Sidecar.Models
{
    /// <summary>
    /// Probe functions info model.
    /// </summary>
    public class ProbeFunctionInfo
    {
        /// <summary>
        /// Gets or sets if the function is enabled.
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the function type.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public ProbeFunctionType Type { get; set; }

        /// <summary>
        /// Gets or sets if the function is running.
        /// </summary>
        [JsonProperty(PropertyName = "running")]
        public bool Running { get; set; }

        /// <summary>
        /// Gets or sets the erroneous functions.
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public IEnumerable<string> Errors { get; set; }
    }
}
