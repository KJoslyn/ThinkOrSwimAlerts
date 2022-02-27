using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.Exceptions
{
    public class OptionParsingException : Exception
    {
        public OptionParsingException(string message, string symbol) : base(message) 
        {
            Symbol = symbol;
        }

        public string Symbol { get; }
    }
}
