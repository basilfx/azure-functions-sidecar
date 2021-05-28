using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Management.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Refit;

namespace Functions.Sidecar.WebHost.Api
{
    /// <summary>
    /// Refit interface for the Azure Functions admin API.
    /// </summary>
    public interface IWebHostClient
    {
        /// <summary>
        /// Get the host status.
        /// </summary>
        /// <returns>A <see cref="Task" /> that yields a <see cref="HostStatus" />.</returns>
        [Get("/admin/host/status")]
        Task<HostStatus> GetHostStatus();

        /// <summary>
        /// Get the function metadata response.
        /// </summary>
        /// <returns>A <see cref="Task" /> that yields a <see cref="FunctionMetadataResponse" />.</returns>
        [Get("/admin/functions")]
        Task<IEnumerable<FunctionMetadataResponse>> GetFunctions();

        /// <summary>
        /// Get the function status.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        /// <returns>A <see cref="Task" /> that yields a <see cref="FunctionStatus" />.</returns>
        [Get("/admin/functions/{name}/status")]
        Task<FunctionStatus> GetFunctionStatus(string name);
    }
}
