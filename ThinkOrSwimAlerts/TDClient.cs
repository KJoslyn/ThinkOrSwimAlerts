using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using TDAmeritrade;
using TDAmeritrade.ViewModels;
using ThinkOrSwimAlerts.Enums;
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

        public DateTime Expiration { get; set; }

        private RestClient _client = new RestClient("https://api.tdameritrade.com/");

        private string _consumerKey;

        public async Task BuyOrSell(BuyOrSell buyOrSell)
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

            var msg = $"Bought {quote.symbol} {quote.putCall}S for mark price {quote.mark} at {DateTime.Now}";
            Log.Information( msg );
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
