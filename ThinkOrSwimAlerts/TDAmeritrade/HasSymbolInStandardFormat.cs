using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwimAlerts.Code;

namespace ThinkOrSwimAlerts.TDAmeritrade
{
    public class HasSymbolInStandardFormat
    {
        private string _symbol;

        public virtual string Symbol {
            get => _symbol;
            init
            {
                if (value != null && OptionSymbolUtils.IsOptionSymbol(value))
                {
                    OptionSymbolUtils.ValidateDateIsFormatAndInNearFuture(value, OptionSymbolUtils.StandardDateFormat);
                }
                _symbol = value;
            }
        }
    }
}
