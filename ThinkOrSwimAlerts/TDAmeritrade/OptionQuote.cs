using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.TDAmeritrade
{
    public class OptionQuote : HasSymbolInStandardFormat
    {
        public OptionQuote(string symbol, float bid, float ask, float last, float mark, float mult, float hi, float low, long longTime)
        {
            Symbol = symbol;
            BidPrice = bid;
            AskPrice = ask;
            LastPrice = last;
            Mark = mark;
            Multiplier = mult;
            HighPrice = hi;
            LowPrice = low;
            QuoteTimeInLong = longTime;
        }

        public OptionQuote() { }

        private DateTime? _time;

        public float BidPrice { get; init; }
        public float AskPrice { get; init; }
        public float LastPrice { get; init; }
        public float Mark { get; init; }
        public float Multiplier { get; init; }
        public float HighPrice { get; init; }
        public float LowPrice { get; init; }
        public long QuoteTimeInLong { get; init; }
        public DateTime Time { get
            {
                if (_time == null)
                {
                    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    _time = start.AddMilliseconds(QuoteTimeInLong).ToLocalTime();
                }
                return (DateTime)_time;
            } 
        }
    }
}
