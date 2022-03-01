using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ThinkOrSwimAlerts.Data;

namespace ThinkOrSwimAlerts
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                // TODO: This will actually catch any error thrown in the worker. Need to read about services and how they work.
                //EventLog.WriteEntry("ThikOrSwim alterts", ex.ToString(), EventLogEntryType.Error);
                Log.Fatal(ex, "The application crashed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile("secrets.json", false, true)
                        .AddEnvironmentVariables();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Build())
                        .CreateLogger();

                    Log.Information("ThinkOrSwim Alerts Application Starting Up");
                } )
                .UseWindowsService(options =>
                {
                    options.ServiceName = "ThinkOrSwim Alerts";
                })
                .UseSerilog()
                .ConfigureServices((hostcontext, services) =>
                {
                    services.AddDbContext<PositionDb>(options =>
                        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToS;"));
                    services.AddHostedService<Worker>();
                });
        }
    }
}
