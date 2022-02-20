using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.TDAmeritrade
{
    public class ChainOptionQuote
    {
        // TODO Use JsonProperty attribute
        public string putCall { get; set; }
        public string symbol { get; set; }
        public float bid { get; set; }
        public float ask { get; set; }
        public float last { get; set; }
        public float mark { get; set; }
    }
}
