using System.Linq;
using Functions.Sidecar.HostedServices;
using Functions.Sidecar.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Functions.Sidecar.Controllers
{
    /// <summary>
    /// Controller with probe-related endpoints.
    /// </summary>
    [Route("/")]
    public class ProbesController : Controller
    {
        /// <summary>
        /// Startup endpoint.
        ///
        /// A HTTP 200 is returned if running, or HTTP 500 if not.
        /// </summary>
        /// <returns>An instance of <see cref="IActionResult" />.</returns>
        [HttpGet]
        [Route("startupz")]
        public IActionResult GetStartup()
        {
            var result = new StartupStatus
            {
                Running = UpdateProbesHostedService.LastProbeHostResult.Success,
                Message = UpdateProbesHostedService.LastProbeHostResult.Message
            };

            if (!result.Running)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Liveness endpoint.
        ///
        /// A HTTP 200 is returned if alive, or HTTP 500 if not.
        /// </summary>
        /// <returns>An instance of <see cref="IActionResult" />.</returns>
        [HttpGet]
        [Route("livez")]
        public IActionResult GetLiveness()
        {
            var result = new LivenessStatus();

            if (!UpdateProbesHostedService.LastProbeFunctionResult.Success)
            {
                result.Alive = false;
                result.Message = UpdateProbesHostedService.LastProbeFunctionResult.Message;
            }
            else
            {
                var functions = UpdateProbesHostedService.LastProbeFunctionResult.Functions
                    .Where(kv => kv.Value.Enabled);

                if (functions.Any(kv => !kv.Value.Running))
                {
                    result.Alive = true;
                    result.Message =
                        $"{functions.Count(kv => !kv.Value.Running)} functions have errors reported.";
                    result.Errors = functions
                        .Where(kv => !kv.Value.Running)
                        .ToDictionary(kv => kv.Key, kv => kv.Value.Errors);
                }
                else
                {
                    result.Alive = true;
                    result.Message = $"{functions.Count()} functions are enabled and running.";
                }
            }

            if (!result.Alive)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Readiness endpoint.
        ///
        /// A HTTP 200 is returned if ready, or HTTP 500 if not.
        /// </summary>
        /// <returns>An instance of <see cref="IActionResult" />.</returns>
        [HttpGet]
        [Route("readyz")]
        public IActionResult GetReadiness()
        {
            var result = new ReadinessStatus();

            if (!UpdateProbesHostedService.LastProbeFunctionResult.Success)
            {
                result.Ready = false;
                result.Message = UpdateProbesHostedService.LastProbeFunctionResult.Message;
            }
            else
            {
                var functions = UpdateProbesHostedService.LastProbeFunctionResult.Functions
                    .Where(kv => kv.Value.Enabled && kv.Value.Type == Models.ProbeFunctionType.HttpTrigger);

                if (!functions.Any(kv => kv.Value.Running))
                {
                    result.Ready = false;
                    result.Message = $"{functions.Count(kv => kv.Value.Running)} functions are not ready.";
                }
                else
                {
                    result.Ready = true;
                    result.Message = $"{functions.Count(kv => kv.Value.Running)} functions are enabled and ready.";
                }
            }

            if (!result.Ready)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
    }
}
