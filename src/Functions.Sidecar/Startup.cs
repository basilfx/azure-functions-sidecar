using System;
using Functions.Sidecar.HostedServices;
using Functions.Sidecar.Services;
using Functions.Sidecar.WebHost.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Refit;

#pragma warning disable CA1822 // Static configure does not work.

namespace Functions.Sidecar
{
    /// <summary>
    /// Startup object for registering dependencies and services.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The default function host, if no host is specified in the configuration.
        ///
        /// This corresponds with a local functions project.
        /// </summary>
        private const string DefaultFunctionHost = "http://localhost:7071";

        /// <summary>
        /// Holds the application configuration.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construct a <see cref="Startup" /> instance.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configure application services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddRefitClient<IWebHostClient>(new RefitSettings
                {
                    // Refit 6 uses a different JSON serializer by default, which is incompatible with the models of
                    // azure-functions-host.
                    ContentSerializer = new NewtonsoftJsonContentSerializer()
                })
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(_configuration["Function:Host"] ?? DefaultFunctionHost);

                    if (!string.IsNullOrEmpty(_configuration["Function:Key"]))
                    {
                        c.DefaultRequestHeaders.Add("X-Functions-Key", _configuration["Function:Key"]);
                    }
                });

            services.AddSingleton<IProbeService, ProbeService>();
            services.AddHostedService<UpdateProbesHostedService>();
        }

        /// <summary>
        /// Configure application services.
        /// </summary>
        /// <param name="application">The application builder.</param>
        /// <param name="environment">The application environment.</param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }

            application.UseRouting();

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            Metrics.SuppressDefaultMetrics();
        }
    }
}
