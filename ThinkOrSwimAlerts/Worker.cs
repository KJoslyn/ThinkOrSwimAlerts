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
using RestSharp;
using ThinkOrSwimAlerts.Code;
using ThinkOrSwimAlerts.Configs;
using ThinkOrSwimAlerts.Enums;

namespace ThinkOrSwimAlerts
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly DotColors _dotColors;
        private readonly IConfiguration _config;

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

            _config = configuration;
        }

        private int intervalSeconds = 5;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan marketOpenTime = new TimeSpan(9, 30, 0);
            TimeSpan marketCloseTime = new TimeSpan(16, 0, 0);

            DateTime lastPositionUpdate = DateTime.MinValue;

            DateTime? lastBuyOrSell = null;

            // TODO This configuration should be somewhere else
            using var client = new TDClient(_config["TDAmeritrade:RefreshToken"], _config["TDAmeritrade:ClientKey"]);

            string? currentPositionSymbol = null;
            float? currentPosBuyPrice = 0;
            float? lastPctDiff = 0;

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

                if (client.AccessTokenNeedsUpdate())
                {
                    await client.GetToken();
                }

                try
                {
                    var timeSinceLastPositionUpdate = currentPositionSymbol == null
                        ? TimeSpan.MinValue
                        : DateTime.Now - lastPositionUpdate;

                    if (timeSinceLastPositionUpdate > TimeSpan.FromSeconds(15))
                    {
                        lastPositionUpdate = DateTime.Now;
                        var quote = await client.GetOptionQuote(currentPositionSymbol);
                        var pctDiff = (quote.Mark - currentPosBuyPrice)*100 / currentPosBuyPrice;
                        if (Math.Abs((decimal)lastPctDiff - (decimal)pctDiff) > 2)
                        {
                            var plusOrMinus = pctDiff > 0 ? "+" : "";
                            Log.Information( $"\t\tSymbol {currentPositionSymbol} at mark price {quote.Mark}. {plusOrMinus}{((float)pctDiff).ToString("0.00")}%");
                            lastPctDiff = pctDiff;
                        }
                    }

                    var timeSinceLastOrder = lastBuyOrSell == null
                        ? TimeSpan.MaxValue
                        : DateTime.Now - lastBuyOrSell;

                    // TODO How much time to wait? Make this configurable
                    var timeBetweenOrders = TimeSpan.FromMinutes(2);
                    if (timeSinceLastOrder < timeBetweenOrders)
                    {
                        continue;
                    }

                    BuyOrSell? buyOrSell = AppScreenshot.DetectBuyOrSellSignal(_dotColors);
                    if (buyOrSell != null)
                    {
                        lastBuyOrSell = DateTime.Now;
                        var symbolAndPrice = await client.BuyOrSell((BuyOrSell)buyOrSell); // TODO Why doesn't the compiler know this isn't null?
                        currentPositionSymbol = symbolAndPrice.Item1;
                        currentPosBuyPrice = symbolAndPrice.Item2;
                        lastPctDiff = 0;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error( $"Error in worker service: {ex}" );
                }

                //await Task.Delay(intervalSeconds*1000, stoppingToken);
            }

            _hostApplicationLifetime.StopApplication();
        }

        private void GetOption(BuyOrSell buyOrSell)
        {
            var client = new RestClient("https://api.tdameritrade.com/");
        }
    }
}
