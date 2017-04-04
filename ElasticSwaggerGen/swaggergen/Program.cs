using System;
using System.IO;
using ElasticSwaggerGen.Commands;
using ElasticSwaggerGen.Options;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ElasticSwaggerGen
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("_config/appsettings.json")
                                .Build();

            var serviceProvider = new ServiceCollection()
                                        .AddLogging()
                                        .AddOptions()
                                        .AddSingleton<CommandLineApplication, SwaggerGenApplication>()
                                        .AddBusinessServices()
                                        .Configure<ParseOptions>(config.GetSection("parseOptions"))
                                        .Configure<SwaggerOptions>(config.GetSection("swaggerOptions"))
                                        .BuildServiceProvider();

            ConfigureLogging(serviceProvider, config);

            var app = serviceProvider.GetService<CommandLineApplication>();
            app.Execute(args);
            
            return;
        }

        static void ConfigureLogging(IServiceProvider serviceProvider, IConfiguration config)
        {
            var serilogConfig = config.GetSection("serilog");
            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.ConfigurationSection(serilogConfig)
                            .CreateLogger();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddSerilog();
            loggerFactory.AddDebug();
        }
    }
}
