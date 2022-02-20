using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Plivo;
using ThinkOrSwimAlerts.Code;
using ThinkOrSwimAlerts.Configs;

namespace ThinkOrSwimAlerts
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly DotColors _dotColors;

        public Worker(
            IHostApplicationLifetime hostApplicationLifetime,
            IConfiguration configuration )
        {
            var plivoConfig = new PlivoConfig(
                authId: configuration["PlivoAuthId"],
                authToken: configuration["PlivoAuthToken"],
                fromNumber: configuration["Plivo:From"],
                toNumbers: configuration.GetSection("Plivo:To").Get<List<string>>());

            _dotColors = configuration.GetSection("DotColors").Get<DotColors>();

            _hostApplicationLifetime = hostApplicationLifetime;
        }

        private int intervalSeconds = 5;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan marketOpenTime = new TimeSpan(9, 30, 0);
            TimeSpan marketCloseTime = new TimeSpan(16, 0, 0);

            while (!stoppingToken.IsCancellationRequested)
            {
                //TimeSpan now = DateTime.Now.TimeOfDay;
                //if (now >= marketCloseTime)
                //{
                //    Log.Information("Market now closed!");
                //    break;
                //}
                //else if (now <= marketOpenTime)
                //{
                //    Log.Information("Market not open yet!");
                //    // Or, wait until 9:30am
                //    await Task.Delay(30 * 1000, stoppingToken);

                //    continue;
                //}

                try
                {
                    AppScreenshot.CaptureApplication("thinkorswim", _dotColors);
                }
                catch (Exception ex)
                {
                    Log.Error( $"Error capturing application: {ex}" );
                }

                //await Task.Delay(intervalSeconds*1000, stoppingToken);
            }

            _hostApplicationLifetime.StopApplication();
        }
    }
}
