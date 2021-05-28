using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Functions.Sidecar.Models;
using Functions.Sidecar.WebHost.Api;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;

namespace Functions.Sidecar.Services
{
    /// <summary>
    /// Implementation of the <see cref="IProbeService" />.
    /// </summary>
    public class ProbeService : IProbeService
    {
        /// <summary>
        /// Holds the WebHost client.
        /// </summary>
        private readonly IWebHostClient _webHostClient;

        /// <summary>
        /// Holds the logger instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="webHostClient">The WebHost client.</param>
        /// <param name="logger">The logger instance.</param>
        public ProbeService(IWebHostClient webHostClient, ILogger<ProbeService> logger)
        {
            _webHostClient = webHostClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ProbeHostResult> ProbeHost()
        {
            var policy = Policy
                .Handle<SocketException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100));

            try
            {
                var status = await policy.ExecuteAsync(() => _webHostClient.GetHostStatus());

                _logger.LogDebug($"Host state is {status.State}.");

                if (status.State != "Running")
                {
                    return new ProbeHostResult
                    {
                        Success = false,
                        Message = $"Host state is {status.State}."
                    };
                }

                return new ProbeHostResult
                {
                    Success = true,
                    Message = "Host is up and running."
                };
            }
            catch (HttpRequestException)
            {
                return new ProbeHostResult
                {
                    Success = false,
                    Message = "Host not reachable."
                };
            }
            catch (SocketException)
            {
                return new ProbeHostResult
                {
                    Success = false,
                    Message = "Host not reachable."
                };
            }
            catch (ApiException e)
            {
                _logger.LogWarning(e, "API Exception");

                return new ProbeHostResult
                {
                    Success = false,
                    Message = $"Host returned HTTP status code {e.StatusCode}."
                };
            }
        }

        /// <inheritdoc />
        public async Task<ProbeFunctionsResult> ProbeFunctions()
        {
            var policy = Policy
                .Handle<SocketException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100));

            try
            {
                var functions = await policy.ExecuteAsync(() => _webHostClient.GetFunctions());

                var functionsInfo = new Dictionary<string, ProbeFunctionInfo>();

                foreach (var function in functions)
                {
                    if (function.IsDisabled)
                    {
                        functionsInfo[function.Name] = new ProbeFunctionInfo
                        {
                            Enabled = false,
                            Running = false,
                            Type = ProbeFunctionType.Other,
                            Errors = Enumerable.Empty<string>()
                        };
                    }
                    else
                    {
                        var status = await policy.ExecuteAsync(() => _webHostClient.GetFunctionStatus(function.Name));

                        functionsInfo[function.Name] = new ProbeFunctionInfo
                        {
                            Enabled = true,
                            Running = status.Errors?.Any() ?? true,
                            Type = function.InvokeUrlTemplate != null
                                ? ProbeFunctionType.HttpTrigger
                                : ProbeFunctionType.Other,
                            Errors = status?.Errors ?? Enumerable.Empty<string>()
                        };
                    }
                }

                return new ProbeFunctionsResult
                {
                    Success = true,
                    Message = $"{functions.Count()} functions probed.",
                    Functions = functionsInfo
                };
            }
            catch (HttpRequestException)
            {
                return new ProbeFunctionsResult
                {
                    Success = false,
                    Message = "Host not reachable."
                };
            }
            catch (SocketException)
            {
                return new ProbeFunctionsResult
                {
                    Success = false,
                    Message = "Connection error."
                };
            }
            catch (ApiException e)
            {
                _logger.LogWarning(e, "API Exception.");

                return new ProbeFunctionsResult
                {
                    Success = false,
                    Message = $"Host returned HTTP status code {e.StatusCode}."
                };
            }
        }
    }
}
