using System.Threading.Tasks;
using Functions.Sidecar.Models;

namespace Functions.Sidecar.Services
{
    /// <summary>
    /// Interface of a probe service.
    /// </summary>
    public interface IProbeService
    {
        /// <summary>
        /// Prope function host to see if is running.
        ///
        /// The function host is considered to be started if it is reachable and reports the state 'Running'.
        /// </summary>
        /// <returns>A <see cref="ProbeHostResult" /> probe result.</returns>
        Task<ProbeHostResult> ProbeHost();

        /// <summary>
        /// Prope the function host to see if the functions are running.
        ///
        /// A function host is considered alive it is reachable and functions that are enabled have no errors reported.
        /// </summary>
        /// <returns>A <see cref="ProbeFunctionsResult" /> probe result.</returns>
        Task<ProbeFunctionsResult> ProbeFunctions();
    }
}
