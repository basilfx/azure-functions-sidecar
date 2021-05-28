using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Functions.Sidecar.Models;
using Functions.Sidecar.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Functions.Sidecar.HostedServices
{
    /// <summary>
    /// Hosted service that periodically updates the probes.
    /// </summary>
    public class UpdateProbesHostedService : IHostedService, IDisposable
    {
        /// <summary>
        /// Gauge metric for the number of total functions.
        /// </summary>
        private static readonly Gauge FunctionsTotal = Metrics.CreateGauge(
            "functions_total", "Total number of functions.");

        /// <summary>
        /// Gauge metric for the number of disabled functions.
        /// </summary>
        private static readonly Gauge FunctionsEnabled = Metrics.CreateGauge(
            "functions_enabled", "Total number of enabled functions.");

        /// <summary>
        /// Gauge metric for the number of disabled functions.
        /// </summary>
        private static readonly Gauge FunctionsDisabled = Metrics.CreateGauge(
            "functions_disabled", "Total number of disabled functions.");

        /// <summary>
        /// Gauge metric for the number of functions that are running.
        /// </summary>
        private static readonly Gauge FunctionsRunning = Metrics.CreateGauge(
            "functions_running", "Number of functions running");

        /// <summary>
        /// Gauge metric for the number of functions with reported errors.
        /// </summary>
        private static readonly Gauge FunctionsError = Metrics.CreateGauge(
            "functions_errors", "Number of functions with errors");

        /// <summary>
        /// Holds the probe service.
        /// </summary>
        private readonly IProbeService _probeService;

        /// <summary>
        /// Holds the logger instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Holds the timer instance.
        /// </summary>
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// Holds the semaphore for mutual exclusive access.
        /// </summary>
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Boolean to detect redundant disposing.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="probeService">The proes service client.</param>
        /// <param name="logger">A logger instance.</param>
        public UpdateProbesHostedService(IProbeService probeService, ILogger<UpdateProbesHostedService> logger)
        {
            _probeService = probeService;
            _logger = logger;

            _timer = new System.Timers.Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            _timer.Elapsed += async (s, e) =>
            {
                // Check if another timer is already busy.
                if (_semaphore.CurrentCount == 0)
                {
                    _logger.LogDebug("Another timer is still busy.");
                    return;
                }

                await _semaphore.WaitAsync();

                try
                {
                    await DoWork();
                }
                finally
                {
                    _semaphore.Release();
                }
            };
            _semaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Static instance that holds the last startup probe response.
        /// </summary>
        public static ProbeHostResult LastProbeHostResult { get; set; } = new ProbeHostResult
        {
            Success = false,
            Message = "Host not yet probed."
        };

        /// <summary>
        /// Static instance that holds the last liveness probe response.
        /// </summary>
        public static ProbeFunctionsResult LastProbeFunctionResult { get; set; } = new ProbeFunctionsResult
        {
            Success = false,
            Message = "Functions not yet probed."
        };

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Metrics update service is starting.");

            _timer.Start();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Metrics update service is stopping.");

            _timer.Stop();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose internal handles.
        /// </summary>
        /// <param name="disposing">True if disposing, false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer?.Dispose();
                _semaphore?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Update the metrics.
        /// </summary>
        /// <returns>A <see cref="Task" />.</returns>
        private async Task DoWork()
        {
            _logger.LogDebug("Updating probes.");

            // Update host until it probed successfully.
            if (!LastProbeHostResult.Success)
            {
                LastProbeHostResult = await _probeService.ProbeHost();
            }

            // Update functions if host was probed successfully.
            if (LastProbeHostResult.Success)
            {
                LastProbeFunctionResult = await _probeService.ProbeFunctions();

                if (LastProbeFunctionResult.Success)
                {
                    var functions = LastProbeFunctionResult.Functions;

                    FunctionsTotal.Set(functions.Count);
                    FunctionsEnabled.Set(functions.Count(kv => kv.Value.Enabled));
                    FunctionsDisabled.Set(functions.Count(kv => !kv.Value.Enabled));
                    FunctionsRunning.Set(functions.Count(kv => kv.Value.Running));
                    FunctionsError.Set(functions.Count(kv => kv.Value.Errors.Any()));
                }
            }

            _logger.LogDebug("Done.");
        }
    }
}
