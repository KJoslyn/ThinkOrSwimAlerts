using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using TDAmeritrade;
using TDAmeritrade.ViewModels;
using ThinkOrSwimAlerts.Code;
using ThinkOrSwimAlerts.Enums;
using ThinkOrSwimAlerts.Exceptions;
using ThinkOrSwimAlerts.TDAmeritrade;

namespace ThinkOrSwimAlerts
{
    public class TDClient : IDisposable
    {
        public TDClient(string refreshToken, string consumerKey)
        {
            RefreshToken = new Token
            {
                TokenStr = refreshToken
            };

            _consumerKey = consumerKey;
        }

        public Token RefreshToken { get; set; }

        public Token? AccessToken { get; set; }

        private RestClient _client = new("https://api.tdameritrade.com/");

        private string _consumerKey;

        public async Task<Tuple<string, float>> BuyOrSell(BuyOrSell buyOrSell)
        { 
            // TODO Configurable symbol
            string symbol = "AAPL";

            RestRequest request = new RestRequest("v1/marketdata/chains", Method.Get);
            request.AddParameter("apikey", _consumerKey);
            request.AddParameter("symbol", symbol);
            request.AddParameter("contractType", buyOrSell == Enums.BuyOrSell.Buy ? "CALL" : "PUT");
            request.AddParameter("strikeCount", "1");
            request.AddParameter("includeQuotes", "TRUE");
            request.AddParameter("range", "SAK");
            request.AddParameter("fromDate", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            request.AddParameter("toDate", DateTime.Now.AddDays(8).ToString("yyyy-MM-dd"));
            request.AddHeader("Authorization", $"Bearer {AccessToken.TokenStr}");

            var response = await _client.ExecuteAsync(request);

            Match match;
            if (buyOrSell == Enums.BuyOrSell.Buy)
            {
                match = Regex.Match(response.Content, @"\{""putCall"":""CALL"".*?\}");
            }
            else
            {
                match = Regex.Match(response.Content, @"\{""putCall"":""PUT"".*?\}");
            }

            ChainOptionQuote quote = JsonConvert.DeserializeObject<ChainOptionQuote>(match.Value);

            var msg = $"++++++++++Bought {quote.symbol} {quote.putCall}S for mark price {quote.mark} at {DateTime.Now}";
            Log.Information( msg );
            return new Tuple<string, float>(quote.symbol, quote.mark);
        }

        public async Task<OptionQuote> GetOptionQuote(string symbol)
        {
            if (!OptionSymbolUtils.IsOptionSymbol(symbol))
            {
                throw new ArgumentException("Provided symbol is not an option symbol: " + symbol);
            }
            //string tdAmSymbol = OptionSymbolUtils.ConvertDateFormat(symbol, OptionSymbolUtils.StandardDateFormat, Constants.TDOptionDateFormat);

            RestRequest request = new RestRequest($"v1/marketdata/{symbol}/quotes", Method.Get);
            request.AddParameter("apikey", _consumerKey);
            request.AddHeader("Authorization", $"Bearer {AccessToken.TokenStr}");

            RestResponse response = await _client.ExecuteAsync(request);
            if (!response.IsSuccessful || response.Content.Contains("Symbol not found"))
            {
                throw new MarketDataException("Get quote unsuccessful for symbol " + symbol);
            }
            Regex responseRegex = new Regex("{\"assetType.*?}");
            Match match = responseRegex.Match(response.Content);
            OptionQuote quote = JsonConvert.DeserializeObject<TDOptionQuote>(match.Value);
            return quote;
        }

        public async Task GetToken()
        {
            RestRequest request = new RestRequest("v1/oauth2/token", Method.Post);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", RefreshToken.TokenStr);
            request.AddParameter("client_id", _consumerKey);

            // TODO Access Token logic
            //if (AccessTokenNeedsUpdate(currentAuthInfo))
            //{
            //    Log.Information("Requesting new REFRESH token");
            //    request.AddParameter("access_type", "offline");
            //}
            //else
            //{
                Log.Information("Requesting new access token only");
            //}
            var response = await _client.ExecuteAsync<AuthResponse>(request);

            // TODO Retries and failure logic

            AccessToken = new Token
            {
                TokenStr = response.Data.AccessToken,
                Expiration = DateTime.Now.Add(TimeSpan.FromSeconds(response.Data.ExpiresIn))
            };
        }

        public bool AccessTokenNeedsUpdate() => AccessToken == null || AccessToken.Expiration < DateTime.Now.AddMinutes(1);

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
