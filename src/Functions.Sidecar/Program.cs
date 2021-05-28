using Microsoft.AspNetCore.Hosting;

namespace Functions.Sidecar
{
    /// <summary>
    /// Functions Sidecar application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point.
        ///
        /// Constructs the server and runs until completion.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        /// <summary>
        /// WebHost builder.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        /// <returns>An instance of <see cref="IWebHostBuilder" />.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Microsoft.AspNetCore.WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
