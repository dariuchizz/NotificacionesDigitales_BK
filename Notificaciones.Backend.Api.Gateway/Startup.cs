using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper;
using ClientCmzAuthLibrary.Auth;
using ClientCmzAuthLibrary.Auth.Services;
using Common;
using Common.IServices;
using Common.Model.Directory;
using Common.Model.NotificacionesDigitales;
using Common.Services;
using CorrelationIdLibrary;
using Microsoft.ApplicationInsights;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notificaciones.Backend.Api.Gateway.Controllers;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Processor.HangfireProcess;
using Processor.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Processor;

namespace Notificaciones.Backend.Api.Gateway
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string AuthName = "cmzAuth";
        private const string APPINSIGHTS_INSTRUMENTATIONKEY = "APPINSIGHTS_INSTRUMENTATIONKEY";
        private readonly ILogger<Startup> _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(ILogger<Startup> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryWorkerService(_configuration[APPINSIGHTS_INSTRUMENTATIONKEY] ?? "invalid");
            //((module, options) =>
            //{
            //    module.EnableSqlCommandTextInstrumentation = true;
            //}),DependencyTrackingTelemetryModule
            //    .AddApplicationInsightsTelemetryWorkerService()

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(options => options.EnableEndpointRouting = false);
            services.AddMemoryCache();

            // using System.Text.Json.Serialization
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddDbContext<INotificacionesDigitalesDbContext, NotificacionesDigitalesDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("NotificacionesConnection"),
                    provider =>
                    {
                        provider.CommandTimeout(300);
                        provider.EnableRetryOnFailure();
                    }), ServiceLifetime.Transient);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Notificaciones Digitales Api Gateway",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Hangfire",
                        Email = string.Empty,
                        Url = new Uri("https://localhost:5015/Hangfire")
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                c.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;
                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{_env.ApplicationName}.xml");
                c.IncludeXmlComments(filePath);
                c.EnableAnnotations();
                //c.OperationFilter<FileUploadOperation>();
            });

            var cmzAuthSeccion = _configuration.GetSection("CmzAuth");
            var issuer = cmzAuthSeccion[AuthConstant.AUTHORITY];
            var audience = cmzAuthSeccion[AuthConstant.AUDIENCE];
            var cors = _configuration["CORS"].Split(",");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthName;
                options.DefaultChallengeScheme = AuthName;
            }).AddJwtBearer(AuthName, options =>
            {
                options.Authority = issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidAudiences = new[] { audience }
                };
            });

            services.AddCors((options =>
            {
                options.AddPolicy("NOTIFICACIONES", builder => builder
                    .WithOrigins(cors)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            }));

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(12)
                });

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(30); // Timeout for an individual try

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError().AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5, // Break on >=50% actions result in handled exceptions...
                    samplingDuration: TimeSpan.FromSeconds(10), // ... over any 10 second period
                    minimumThroughput: 8, // ... provided at least 6 actions in the 10 second period.
                    durationOfBreak: TimeSpan.FromSeconds(60), // Break for 60 seconds.
                    onBreak: (exception, timespan, context) => { _logger.LogInformation("Error Circuit Breaker Open"); },
                    onReset: (context) => { _logger.LogInformation("Circuit Breaker Reconected"); }
                );

            services.AddHttpClient(ModuleConfigurationService.AUTH_CLIENT, c => { c.BaseAddress = new Uri(issuer); })
                .AddHttpMessageHandler(provider => new CorrelationIdHttpClientLoggingHandler())
                .AddHttpMessageHandler(provider => new DependencyTimeSpent())
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy);


            services.AddHttpClient(NotificacionesConstant.NOTIFICACIONES_CLIENT,
                    config =>
                    {
                        config.BaseAddress = new Uri(_configuration[NotificacionesConstant.NOTIFICACIONES_SERVICE]);
                    })
                .AddHttpMessageHandler(provider => new CorrelationIdHttpClientLoggingHandler())
                .AddHttpMessageHandler(provider => new DependencyTimeSpent())
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy) // Select a policy based on the request: retry for Get requests, noOp for other http verbs.
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("repo_cobrabilidad", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_RPT_COBRABILIDAD", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("eventos_acciones_view", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_EVENTOS_ACCIONES_VIEW", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("eventos_acciones_abm", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_EVENTOS_ACCIONES_ABM", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("notif_combos", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_COMBOS", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("repo_tipo_comunicaciones", policy => policy.Requirements.Add(new HasScopeRequirement("IND_REPO_TIPO_NOTIFICACIONES", issuer, "notificaciones")));
                options.AddPolicy("repo_canal", policy => policy.Requirements.Add(new HasScopeRequirement("IND_REPO_CANAL", issuer, "notificaciones")));
                options.AddPolicy("notif_gestor_campanias", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_GESTOR_CAMPANIAS", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("notif_gestor_campanias_formulario", policy => policy.Requirements.Add(new HasScopeListRequirement(new List<string> { "NOTIF_GESTOR_CAMPANIAS", "NOTIF_GESTOR_CAMPANIAS_FORMULARIO" }, issuer, "Notificaciones-Digitales", Operation.And)));
                options.AddPolicy("notif_comunicaciones", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_COMUNICACIONES", issuer, "Notificaciones-Digitales")));
                options.AddPolicy("notif_envio_sms", policy => policy.Requirements.Add(new HasScopeRequirement("NOTIF_ENVIO_SMS", issuer, "Notificaciones-Digitales")));
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<INotificacionesDigitalesDbContext, NotificacionesDigitalesDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("NotificacionesConnection"), options => options.EnableRetryOnFailure()));

            services.AddDbContext<IDirectoryDbContext, DirectoryDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DirectorioConnection"), options => options.EnableRetryOnFailure()));

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddSingleton<IAuthorizationHandler, HasScopeListHandler>();
            services.AddSingleton<IModuleConfigurationService, ModuleConfigurationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<CorrelationIdHttpClientLoggingHandler>();
            services.AddTransient<IProcessManager, ProcessManager>();
            services.AddTransient<IAvisoDeudaServices, AvisoDeudaServices>();
            services.AddTransient<IFacturaServices, FacturaServices>();
            services.AddTransient<IComunicacionServices, ComunicacionServices>();
            services.AddTransient<INotificacionesServices, NotificacionesServices>();
            services.AddTransient<IEventoEmailServices, EventoEmailServices>();
            services.AddTransient<IEventoSMSServices, EventoSMSServices>();
            services.AddTransient<IEventosComunicacionesServices, EventosComunicacionesServices>();
            services.AddTransient<IListaGrisServices, ListaGrisServices>();
            services.AddTransient<IMotivoBajaListaGrisServices, MotivoBajaListaGrisServices>();
            services.AddTransient<IUnitOfWorkNotificacion, UnitOfWorkNotificacion>();
            services.AddTransient<ICobrabilidadServices, CobrabilidadServices>();
            services.AddTransient<ITipoComunicacionServices, TipoComunicacionServices>();
            services.AddTransient<IEventosAccionesServices, EventosAccionesServices>();
            services.AddTransient<ICombosServices, ComboServices>();
            services.AddTransient<ICampaniaServices, CampaniaServices>();
            services.AddTransient<IMotivoBajaServices, MotivoBajaServices>();
            services.AddTransient<IVencimientoFacturaServices, VencimientoFacturaServices>();
            services.AddTransient<IStoreServices, StoreServices>();
            services.AddTransient<IProcesoEventoServices, ProcesoEventoServices>();
            services.AddTransient<ICambioContactoServices, CambioContactoServices>();
            services.AddTransient<IEnvioServices, EnvioServices>();
            services.AddTransient<IEnvioSMSGenericosServices, EnvioSMSGenericosServices>();
            services.AddTransient<ICsvCampaniaServices, CsvCampaniaServices>();
            services.AddTransient<INotificacionHuemulServices, NotificacionHuemulServices>();
            services.AddTransient<IUnitOfWorkDirectory, UnitOfWorkDirectory>();
            services.AddTransient<ISmStartPlus, SmStartPlus>();
            services.AddTransient<IMailgun, Mailgun>();
            services.AddTransient<ICompressUrl, CompressUrl>();
            services.AddTransient<IHuemul, Huemul>();
            var hangfireDatabase = _configuration.GetConnectionString("NotificacionesConnection");
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
            var telemetryClient = services.BuildServiceProvider().GetRequiredService<TelemetryClient>();
            telemetryClient.TrackEvent("ServerInicializado");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LogCorrelationIdMiddleware>();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        _logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync("Internal Server Error.");
                    }
                });
            });
            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseCors("NOTIFICACIONES");

            app.UseAuthentication();

            app.UseHangfireDashboard();

            var SwaggerEnabled = _configuration["SwaggerEnabled"];
            if (Convert.ToBoolean(SwaggerEnabled))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notificaciones Digitales Api Gateway");
                });
            }

            app.UseMvc();
        }
    }
}
