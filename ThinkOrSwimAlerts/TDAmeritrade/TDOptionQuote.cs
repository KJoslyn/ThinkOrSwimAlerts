using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwimAlerts.Code;

namespace ThinkOrSwimAlerts.TDAmeritrade 
{
    internal class TDOptionQuote : OptionQuote
    {
        private string _symbol;

        public TDOptionQuote(string symbol, float bid, float ask, float last, float mark, float mult, float hi, float low, long longTime)
            : base(symbol, bid, ask, last, mark, mult, hi, low, longTime) { }

        public TDOptionQuote() { }

        //public override string Symbol
        //{
        //    get => _symbol;
        //    init { _symbol = OptionSymbolUtils.ConvertToStandardDateFormat(value, Constants.TDOptionDateFormat); }
        //}

        // We could just convert the format in the init method, but then if this quote is ever reconstructed from a database lookup,
        // the conversion will fail since it was already in the proper format.
        public override string Symbol
        {
            get => OptionSymbolUtils.ConvertToStandardDateFormatIfNecessary(_symbol, Constants.TDOptionDateFormat);
            init { _symbol = value; }
        }
    }
}
