using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Notificaciones.Backend.Api.Gateway
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private static string environment = "";

        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main function");
                logger.Debug(environment);
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Debug("catch: " + environment);
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }

            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    environment = env.EnvironmentName;
                    config.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
                    //#if DEBUG
                    //                     config.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);
                    //#endif
                    //#if NETCOREAPP2_2
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false,
                        reloadOnChange: true);
                    //#endif
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Information);
                })
                .UseNLog()
                .UseUrls(
                    "https://*::5015"
                )
                .UseStartup<Startup>();
    }

}

    //    public static IHostBuilder CreateHostBuilder(string[] args) =>
    //        Host.CreateDefaultBuilder(args)
    //            .ConfigureWebHostDefaults(webBuilder =>
    //            {
    //                webBuilder.UseStartup<Startup>();
    //            });
    //}
