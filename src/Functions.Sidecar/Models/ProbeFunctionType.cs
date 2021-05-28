using System.Runtime.Serialization;

#pragma warning disable CA2227 // Necessary for deserialization.

namespace Functions.Sidecar.Models
{
    /// <summary>
    /// Probe functions type model.
    /// </summary>
    public enum ProbeFunctionType
    {
        /// <summary>
        /// Function is a HTTP trigger.
        /// </summary>
        [EnumMember(Value = "HttpTrigger")]
        HttpTrigger,

        /// <summary>
        /// Function is of other type.
        /// </summary>
        [EnumMember(Value = "Other")]
        Other
    }
}
