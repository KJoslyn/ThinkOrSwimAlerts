using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.Exceptions
{
    public class MarketDataException : Exception
    {
        public MarketDataException(string message) : base(message) { }
    }
}
