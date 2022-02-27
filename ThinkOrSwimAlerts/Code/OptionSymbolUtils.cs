using Serilog;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ThinkOrSwimAlerts.Exceptions;

namespace ThinkOrSwimAlerts.Code
{
    public static class OptionSymbolUtils
    {
        public const string StandardDateFormat = "yyMMdd";
        private static Regex _optionSymbolRegex = new Regex(@"^([A-Z]{1,5})_(\d{6})([CP])(\d+(.\d)?)");
        private static Regex _callRegex = new Regex(@"^[A-Z]{1,5}[_ ]?\d{6}C\d+(.\d)?");
        private static Regex _putRegex = new Regex(@"^[A-Z]{1,5}[_ ]?\d{6}P\d+(.\d)?");

        public static bool IsOptionSymbol(string symbol) => _optionSymbolRegex.IsMatch(symbol);
        public static bool IsCall(string symbol) => _callRegex.IsMatch(symbol);
        public static bool IsPut(string symbol) => _putRegex.IsMatch(symbol);

        public static string ChangeUnderlyingSymbol(string newUnderlyingSymbol, string originalOptionSymbol)
        {
            GroupCollection matchGroups = _optionSymbolRegex.Match(originalOptionSymbol).Groups;
            return string.Format("{0}_{1}{2}{3}",
                newUnderlyingSymbol,
                matchGroups[2].Value,
                matchGroups[3].Value,
                matchGroups[4].Value);
        }

        public static string GetUnderlyingSymbol(string symbol)
        {
            GroupCollection matchGroups = _optionSymbolRegex.Match(symbol).Groups;
            if (matchGroups.Count < 2)
            {
                throw new OptionParsingException("Invalid option symbol", symbol);
            }
            return matchGroups[1].Value;
        }

        public static string ConvertToStandardDateFormatIfNecessary(string symbol, string fromFormat)
        {
            bool success = TryParseExactDate(symbol, StandardDateFormat, out DateTime date);
            if (success && IsDateInNearFuture(date))
            {
                return symbol;
            }
            else
            {
                return ConvertDateFormat(symbol, fromFormat, StandardDateFormat);
            }
        }

        public static string ConvertDateFormat(string symbol, string fromFormat, string toFormat)
        {
            ValidateDateIsFormatAndInNearFuture(symbol, fromFormat);

            GroupCollection matchGroups = _optionSymbolRegex.Match(symbol).Groups;
            string date = DateTime.ParseExact(matchGroups[2].Value, fromFormat, CultureInfo.InvariantCulture).ToString(toFormat);

            return string.Format("{0}_{1}{2}{3}",
                matchGroups[1].Value,
                date,
                matchGroups[3].Value,
                matchGroups[4].Value);
        }

        public static void ValidateDateFormat(string symbol, string dateFormat, out DateTime date)
        {
            bool isCorrectFormat = TryParseExactDate(symbol, dateFormat, out date);
            if (!isCorrectFormat)
            {
                throw new OptionParsingException("Option date not in correct format. Expected " + dateFormat, symbol);
            }
        }

        public static void ValidateDateIsFormatAndInNearFuture(string symbol, string dateFormat)
        {
            ValidateDateFormat(symbol, dateFormat, out DateTime date);
            ValidateDateIsInNearFuture(symbol, date);
        }

        public static void ValidateDateIsInNearFuture(string symbol, DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                throw new OptionParsingException("Option date is expired", symbol);
            }
            else if (date.Year > DateTime.Now.Year + 1)
            {
                Log.Warning("Year {0} is far into the future! Symbol {Symbol}", DateTime.Now.Year, symbol);
            }
        }

        public static bool IsDateInNearFuture(DateTime date)
        {
            return date.Date >= DateTime.Now.Date &&
                date.Year <= DateTime.Now.Year + 1;
        }

        public static bool TryParseExactDate(string symbol, string dateFormat, out DateTime date)
        {
            GroupCollection matchGroups = _optionSymbolRegex.Match(symbol).Groups;
            return DateTime.TryParseExact(matchGroups[2].Value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
