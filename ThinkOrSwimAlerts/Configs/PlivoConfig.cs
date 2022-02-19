using System.Collections.Generic;

namespace ThinkOrSwimAlerts.Configs
{
    public class PlivoConfig
    {
        public PlivoConfig(string authId, string authToken, string fromNumber, List<string> toNumbers)
        {
            AuthId = authId;
            AuthToken = authToken;
            FromNumber = fromNumber;
            ToNumbers = toNumbers;
        }

        public string AuthId { get; init; }
        public string AuthToken { get; init; }
        public string FromNumber { get; init; }
        public List<string> ToNumbers { get; init; }
    }
}
