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
using Microsoft.Extensions.DependencyInjection;
using Plivo;
using RestSharp;
using ThinkOrSwimAlerts.Code;
using ThinkOrSwimAlerts.Configs;
using ThinkOrSwimAlerts.Data;
using ThinkOrSwimAlerts.Data.Models;
using ThinkOrSwimAlerts.Enums;
using ThinkOrSwimAlerts.TDAmeritrade;

namespace ThinkOrSwimAlerts
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly DotColors _dotColors;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _factory;
        private PositionDb _ctx;

        public Worker(
            IHostApplicationLifetime hostApplicationLifetime,
            IConfiguration configuration,
            IServiceScopeFactory factory )
        {
            var plivoConfig = new PlivoConfig(
                authId: configuration["PlivoAuthId"],
                authToken: configuration["PlivoAuthToken"],
                fromNumber: configuration["Plivo:From"],
                toNumbers: configuration.GetSection("Plivo:To").Get<List<string>>());

            _dotColors = configuration.GetSection("DotColors").Get<DotColors>();

            _hostApplicationLifetime = hostApplicationLifetime;

            _config = configuration;

            _factory = factory;
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

            Position? currentPosition = null;

            float? currentPosBuyPrice = 0;
            float? lastPctDiff = 0;

            using var scope = _factory.CreateScope();
            _ctx = scope.ServiceProvider.GetRequiredService<PositionDb>();

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
                    var timeSinceLastPositionUpdate = currentPosition == null
                        ? TimeSpan.MinValue
                        : DateTime.Now - lastPositionUpdate;

                    if (timeSinceLastPositionUpdate > TimeSpan.FromSeconds(15))
                    {
                        lastPositionUpdate = DateTime.Now;
                        var quote = await client.GetOptionQuote(currentPosition.Symbol);
                        var pctDiff = (quote.Mark - currentPosBuyPrice)*100 / currentPosBuyPrice;

                        await AddPositionUpdate(currentPosition, quote, (float)pctDiff);

                        if (quote.Mark > currentPosition.HighPrice)
                        {
                            currentPosition.HighPrice = quote.Mark;
                        }

                        if (quote.Mark < currentPosition.LowPrice)
                        {
                            currentPosition.LowPrice = quote.Mark;
                        }

                        await _ctx.SaveChangesAsync();

                        if (Math.Abs((decimal)lastPctDiff - (decimal)pctDiff) > 2)
                        {
                            var plusOrMinus = pctDiff > 0 ? "+" : "";
                            Log.Information( $"\t\tSymbol {currentPosition.Symbol} at mark price {quote.Mark}. {plusOrMinus}{((float)pctDiff).ToString("0.00")}%");
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
                        currentPosition = await CreatePosition(symbolAndPrice.Item1, (BuyOrSell) buyOrSell,
                            symbolAndPrice.Item2);
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

        private async Task<Position> CreatePosition(string symbol, BuyOrSell buyOrSell, float price)
        {
            var pos = new Position
            {
                PositionId = 0, // new
                Symbol = symbol,
                FirstBuy = DateTimeOffset.Now,
                Indicator = Indicator.MacNSqueeze,
                IndicatorVersion = "1.0",
                PutOrCall = buyOrSell == BuyOrSell.Buy ? PutOrCall.Call : PutOrCall.Put,
                Underlying = OptionSymbolUtils.GetUnderlyingSymbol(symbol),
                HighPrice = price,
                LowPrice = price
            };

            await _ctx.AddAsync(pos);

            var purchase = new Purchase
            {
                PurchaseId = 0, // new
                Position = pos,
                Bought = pos.FirstBuy,
                BuyPrice = price,
                Day = DateTime.Now.DayOfWeek,
                Quantity = 1,
                Bought15MinuteInterval = FifteenMinuteIntervalUtils.GetFifteenMinuteInterval()
            };

            await _ctx.AddAsync(purchase);
            await _ctx.SaveChangesAsync();

            return pos;
        }

        private async Task AddPositionUpdate(Position currentPosition, OptionQuote quote, float pctDiff)
        {
            PositionUpdate update = new PositionUpdate
            {
                PositionUpdateId = 0, // new
                Position = currentPosition,
                GainOrLossPct = pctDiff,
                IsNewHigh = quote.Mark > currentPosition.HighPrice,
                IsNewLow = quote.Mark < currentPosition.LowPrice,
                Mark = quote.Mark,
                SecondsAfterPurchase = (DateTimeOffset.Now - currentPosition.FirstBuy).Seconds
            };
            await _ctx.AddAsync(update);
        }

        private void GetOption(BuyOrSell buyOrSell)
        {
            var client = new RestClient("https://api.tdameritrade.com/");
        }
    }
}
