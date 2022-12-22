using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Processor.HangfireProcess;
using System;
using AutoMapper;
using Common;
using Common.IServices;
using Common.Model.Directory;
using Common.Model.NotificacionesDigitales;
using Common.Services;
using Microsoft.EntityFrameworkCore;
using Processor.Services;
using Processor;

namespace NotificacionesDigitalesApi
{
    public class Startup
    {
        private const string APPINSIGHTS_INSTRUMENTATIONKEY = "APPINSIGHTS_INSTRUMENTATIONKEY";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Notificaciones Digitales Api",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Hangfire",
                        Email = string.Empty,
                        Url = new Uri("https://localhost:44335/Hangfire")
                    }
                });

            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddTransient<IProcessManager, ProcessManager>();
            services.AddTransient<IUnitOfWorkNotificacion, UnitOfWorkNotificacion>();
            services.AddTransient<IUnitOfWorkDirectory, UnitOfWorkDirectory>();
            services.AddTransient<IVencimientoFacturaServices, VencimientoFacturaServices>();
            services.AddTransient<ITipoComunicacionServices, TipoComunicacionServices>();
            services.AddTransient<IStoreServices, StoreServices>();
            services.AddTransient<IEventoEmailServices, EventoEmailServices>();
            services.AddTransient<IEventoSMSServices, EventoSMSServices>();
            services.AddTransient<IComunicacionServices, ComunicacionServices>();
            services.AddTransient<IProcesoEventoServices, ProcesoEventoServices>();
            services.AddTransient<IListaGrisServices, ListaGrisServices>();
            services.AddTransient<ICambioContactoServices, CambioContactoServices>();
            services.AddTransient<ICampaniaServices, CampaniaServices>();
            services.AddTransient<IEnvioServices, EnvioServices>();
            services.AddTransient<ISmStartPlus, SmStartPlus>();
            services.AddTransient<IMailgun, Mailgun>();
            services.AddTransient<ICompressUrl, CompressUrl>();
            services.AddTransient<IHuemul, Huemul>();
            services.AddTransient<INotificacionHuemulServices, NotificacionHuemulServices>();
            services.AddMemoryCache();
            services.AddDbContext<INotificacionesDigitalesDbContext, NotificacionesDigitalesDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("NotificacionesDigitalesConnection"), options => options.EnableRetryOnFailure()));

            services.AddDbContext<IDirectoryDbContext, DirectoryDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DirectorioConnection"), options => options.EnableRetryOnFailure()));

            services.AddApplicationInsightsTelemetry(Configuration[APPINSIGHTS_INSTRUMENTATIONKEY] ?? "invalid");

            var hangfireDatabase = Configuration.GetConnectionString("HangfireNDConnection");
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireDatabase, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notificaciones Digitales Api");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
