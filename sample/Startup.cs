using LoggerEnhancer;
using sample.Middlewares;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace sample
{
    public class Startup
    {
        private readonly bool _configureElastic = false;

        public void ConfigureServices(IServiceCollection services)
        {
            if (_configureElastic)
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var config = new ConfigurationBuilder()
                    .AddUserSecrets<Startup>()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddEnvironmentVariables()
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", env)
                    .ReadFrom.Configuration(config)
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(ConfigurationElasticSink(config))
                    .CreateLogger();

                services.AddLogging(log =>
                {
                    log.ClearProviders();
                    log.AddSerilog(Log.Logger);
                });
            }
            else
            {
                services.AddLogging();
            }

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddControllers();
            services.AddLoggerEnhancer();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (env.IsProduction())
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseEnhancer();
            app.UseAuthorization();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
            });
        }

        private static ElasticsearchSinkOptions ConfigurationElasticSink(IConfigurationRoot configuration)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration.GetConnectionString("Elasticsearch")!))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"logs-{DateTime.UtcNow:yyyy}"
            };
        }
    }
}
