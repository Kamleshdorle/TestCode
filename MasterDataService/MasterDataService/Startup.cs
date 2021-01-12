using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using MasterDataService.Middlewares.Builder;
using MasterDataService.Middlewares;
using AWS.Logger.SeriLog;
using MasterDataService.Repository;
using MasterDataService.Repository.Interfaces;
using System.Reflection;
using System.IO;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using Amazon;
using Serilog.Core;

namespace MasterDataService
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            string sAppPath = env.ContentRootPath; //Application Base Path
            string swwwRootPath = env.WebRootPath;  //wwwroot folder path

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>(optional: true);
            }
            Configuration = builder.Build();

            //Configuration = configuration;
            int.TryParse(Configuration.GetSection("Serilog").GetSection("LogLevel").Value, out int level);
            int.TryParse(Configuration.GetSection("Serilog").GetSection("RententionPolicy").Value, out int retentionConfig);
            var logLevel = typeof(LogEventLevel).IsEnumDefined(level) ?
                                (LogEventLevel)level : LogEventLevel.Error;
            var retentionPolicy = typeof(LogGroupRetentionPolicy).IsEnumDefined(retentionConfig) ?
                              (LogGroupRetentionPolicy)retentionConfig : LogGroupRetentionPolicy.OneWeek;
            var region = RegionEndpoint.GetBySystemName(Configuration.GetSection("Serilog").GetSection("Region").Value);
            var levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = logLevel;
            // customer formatter  
            var formatter = new CustomLogFormatter();
            var options = new CloudWatchSinkOptions
            {
                // the name of the CloudWatch Log group from config  
                LogGroupName = Configuration.GetSection("Serilog").GetSection("LogGroup").Value,
                // the main formatter of the log event  
                TextFormatter = formatter,
                // other defaults  
                MinimumLogEventLevel = logLevel,
                BatchSizeLimit = 100,
                QueueSizeLimit = 10000,
                Period = TimeSpan.FromSeconds(10),
                CreateLogGroup = true,
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                RetryAttempts = 5,
                LogGroupRetentionPolicy = retentionPolicy
            };
            // setup AWS CloudWatch client  
            var client = new AmazonCloudWatchLogsClient(region);
            // Attach the sink to the logger configuration  
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Logger(l1 => l1
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .WriteTo.AmazonCloudWatch(options, client))
              .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILogRepository, LogRepository>();
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
            
            services.AddSingleton<ITranspoterRepository>(new TranspoterRepository(Configuration));
            services.AddSingleton<ICompanyLookupRepository>(new CompanyRepository(Configuration));
            services.AddSingleton<IVendorRepository>(new VendorRepository(Configuration));

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // Add framework services.
            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Anco Trans Master Data Service API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Configure AWS Credentials from AWS configuration
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            // Logger that can use AWS or Azure Log can be configured accordingly
            Log.Logger = new LoggerConfiguration()
                  .WriteTo.AWSSeriLog(Configuration)
                 .Enrich.With(new CorrelationLogEventEnricher(httpContextAccessor, Configuration["Logging:CorrelationHeaderKey"]))
                  .CreateLogger();

            // Important: it has to be first: enable global logger
            app.UseGlobalLoggerHandler();

            // Important: it has to be second: Enable global exception, error handling
            app.UseGlobalExceptionHandler();
            
            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            // Use MVC
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint with different route
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "masterdata/swagger/{documentname}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "masterdata/swagger";
                c.SwaggerEndpoint("/masterdata/swagger/v1/swagger.json", "Anco Trans Master Data Service API V1");
                c.DisplayRequestDuration();
            });

            //TODO look into creating a factory of DocDBRepos/RedisCache/EventHubMessenger
            //RedisCache<Vehicle>.Configure(Constants.RedisCacheDBId, Configuration["REDIS_ENDPOINT"], Configuration["REDIS_KEY"], loggerFactory);

        }
    }
}
