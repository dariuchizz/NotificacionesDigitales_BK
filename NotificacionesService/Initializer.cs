using System;
using System.IO;
using AutoMapper;
using Common;
using Common.IServices;
using Common.Model.Directory;
using Common.Model.NotificacionesDigitales;
using Common.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Processor;
using Processor.HangfireProcess;
using Processor.ProcessModule;
using Processor.Services;

namespace NotificacionesService
{
    static class Initializer
    {
        const string APPINSIGHTS_INSTRUMENTATIONKEY = "APPINSIGHTS_INSTRUMENTATIONKEY";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool _isDebug = false;

        public static void Start()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //Console.WriteLine(environment);
#if DEBUG
            _isDebug = true;
#endif
            if (string.IsNullOrEmpty(environment))
            {
                environment = "Development";
                if (!_isDebug)
                {
                    environment = "Production";
                }
            }
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var serviceProvider = BuildServiceProvider(configuration);
            BuildHangfireServer(serviceProvider, configuration);
        }

        private static IMapper BuildMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            return mapper;
        }

        private static ServiceProvider BuildServiceProvider(IConfigurationRoot configuration)
        {
            try
            {
                // https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics
                var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                        loggingBuilder.AddNLog("nlog.config");
                        loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Information);
                    })
                    .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, options) =>
                    {
                        module.EnableSqlCommandTextInstrumentation = true;
                    })
                    .AddApplicationInsightsTelemetryWorkerService(configuration[APPINSIGHTS_INSTRUMENTATIONKEY] ?? "invalid")
                    .AddSingleton(BuildMapper())
                    .AddTransient<IProcessManager, ProcessManager>()
                    .AddTransient<EventMailProcess>()
                    .AddTransient<EventSMSProcess>()
                    .AddTransient<EventProcesGeneratorProcess>()
                    .AddTransient<StoreProcess>()
                    .AddTransient<TipoComunicacionProcess>()
                    .AddTransient<CampaniaProcess>()
                    .AddTransient<EventReactionProcess>()
                    .AddTransient<TestSendMailgun>()
                    .AddTransient<TestSendStartPlus>()
                    .AddTransient<IndexedReceiptProcess>()
                    .AddTransient<StoreCampanaProcess>()
                    .AddTransient<IConfiguration>(provider => configuration)
                    .AddTransient<IUnitOfWorkNotificacion, UnitOfWorkNotificacion>()
                    .AddTransient<IUnitOfWorkDirectory, UnitOfWorkDirectory>()
                    .AddTransient<IComunicacionServices, ComunicacionServices>()
                    .AddTransient<IEventoEmailServices, EventoEmailServices>()
                    .AddTransient<IEventoSMSServices, EventoSMSServices>()
                    .AddTransient<IProcesoEventoServices, ProcesoEventoServices>()
                    .AddTransient<IStoreServices, StoreServices>()
                    .AddTransient<ITipoComunicacionServices, TipoComunicacionServices>()
                    .AddTransient<ICambioContactoServices, CambioContactoServices>()
                    .AddTransient<IVencimientoFacturaServices, VencimientoFacturaServices>()
                    .AddTransient<IListaGrisServices, ListaGrisServices>()
                    .AddTransient<IMotivoBajaListaGrisServices, MotivoBajaListaGrisServices>()
                    .AddTransient<IEnvioServices, EnvioServices>()
                    .AddTransient<ICampaniaServices, CampaniaServices>()
                    .AddTransient<INotificacionHuemulServices, NotificacionHuemulServices>()
                    .AddTransient<ISmStartPlus, SmStartPlus>()
                    .AddTransient<IMailgun, Mailgun>()
                    .AddTransient<ICompressUrl, CompressUrl>()
                    .AddTransient<IHuemul, Huemul>()
                    .AddMemoryCache()
                    .AddDbContext<INotificacionesDigitalesDbContext, NotificacionesDigitalesDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("NotificacionesDigitalesConnection"),
                             provider => { provider.CommandTimeout(300); provider.EnableRetryOnFailure(); }), Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient)
                    .AddDbContext<IDirectoryDbContext, DirectoryDbContext>(options =>
                         options.UseSqlServer(configuration.GetConnectionString("DirectorioConnection"), options => options.EnableRetryOnFailure()));
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var telemetryClient = serviceProvider.GetRequiredService<Microsoft.ApplicationInsights.TelemetryClient>();
                telemetryClient.TrackEvent("ServerInicializado");
                return serviceProvider;
            }
            catch (Exception ex)
            {
                Logger.Error("Error en el metodo BuildServiceProvider: " + ex.Message);
                return null;
            }
        }

        private static void BuildHangfireServer(ServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            try {
                var hangfireDatabase = configuration.GetConnectionString("HangfireNDConnection");

                GlobalConfiguration.Configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseActivator(new ContainerJobActivator(serviceProvider))
                    .UseSqlServerStorage(hangfireDatabase, new SqlServerStorageOptions
                    {
                    //CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    //SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    //QueuePollInterval = TimeSpan.Zero,
                    //UseRecommendedIsolationLevel = true,
                    //UsePageLocksOnDequeue = true,
                    //DisableGlobalLocks = true,
                    //CommandTimeout = new TimeSpan(0, 2, 0)
                });

                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });
                //"aalpha" creación de comunicaciones, generación de camapañas, generador de eventos, recolector de eventos SMS. 
                //"alpha" event reaction, envio proceso de negocio.
                //"beta" envio campañas.
                //"delta" recolector de eventos Email. 
                var optionsAll = new BackgroundJobServerOptions
                {
                    WorkerCount = 6,
                    Queues = new[] { "aalpha", "alpha", "beta", "delta", "mailgun-test" },
                    //SchedulePollingInterval = TimeSpan.FromSeconds(10),
                    //HeartbeatInterval = TimeSpan.FromSeconds(10),
                    //ServerCheckInterval = TimeSpan.FromSeconds(10),
                    //CancellationCheckInterval = TimeSpan.FromSeconds(10),
                    //ServerTimeout = TimeSpan.FromMinutes(2),
                };
                var server = new BackgroundJobServer(optionsAll);

                Logger.Info("Hangfire Server started. Press any key to exit...");
            }
            catch (Exception ex)
            {
                Logger.Error("Error en el metodo BuildHangfireServer: " + ex.Message);
            }            
            //Console.WriteLine("Hangfire Server started. Press any key to exit...");
            //Console.ReadKey();
        }
    }
}