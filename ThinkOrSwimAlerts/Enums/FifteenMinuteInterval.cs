using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwimAlerts.Enums
{
    public enum FifteenMinuteInterval
    {
        h9_30,
        h9_45,
        h10_00,
        h10_15,
        h10_30,
        h10_45,
        h11_00,
        h11_15,
        h11_30,
        h11_45,
        h12_00,
        h12_15,
        h12_30,
        h12_45,
        h1_00,
        h1_15,
        h1_30,
        h1_45,
        h2_00,
        h2_15,
        h2_30,
        h2_45,
        h3_00,
        h3_15,
        h3_30,
        h3_45,
        OUT_OF_BOUNDS
    }

    public static class FifteenMinuteIntervalUtils
    {
        public static FifteenMinuteInterval GetFifteenMinuteInterval()
        {
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            if (hour == 9 && minute >= 45) return FifteenMinuteInterval.h9_45;
            if (hour == 9 && minute >= 30) return FifteenMinuteInterval.h9_30;
            if (hour == 10 && minute >= 45) return FifteenMinuteInterval.h10_45;
            if (hour == 10 && minute >= 30) return FifteenMinuteInterval.h10_30;
            if (hour == 10 && minute >= 15) return FifteenMinuteInterval.h10_15;
            if (hour == 10) return FifteenMinuteInterval.h10_00;
            if (hour == 11 && minute >= 45) return FifteenMinuteInterval.h11_45;
            if (hour == 11 && minute >= 30) return FifteenMinuteInterval.h11_30;
            if (hour == 11 && minute >= 15) return FifteenMinuteInterval.h11_15;
            if (hour == 11) return FifteenMinuteInterval.h11_00;
            if (hour == 12 && minute >= 45) return FifteenMinuteInterval.h12_45;
            if (hour == 12 && minute >= 30) return FifteenMinuteInterval.h12_30;
            if (hour == 12 && minute >= 15) return FifteenMinuteInterval.h12_15;
            if (hour == 12) return FifteenMinuteInterval.h12_00;
            if (hour == 13 && minute >= 45) return FifteenMinuteInterval.h1_45;
            if (hour == 13 && minute >= 30) return FifteenMinuteInterval.h1_30;
            if (hour == 13 && minute >= 15) return FifteenMinuteInterval.h1_15;
            if (hour == 13) return FifteenMinuteInterval.h1_00;
            if (hour == 14 && minute >= 45) return FifteenMinuteInterval.h2_45;
            if (hour == 14 && minute >= 30) return FifteenMinuteInterval.h2_30;
            if (hour == 14 && minute >= 15) return FifteenMinuteInterval.h2_15;
            if (hour == 14) return FifteenMinuteInterval.h2_00;
            if (hour == 15 && minute >= 45) return FifteenMinuteInterval.h3_45;
            if (hour == 15 && minute >= 30) return FifteenMinuteInterval.h3_30;
            if (hour == 15 && minute >= 15) return FifteenMinuteInterval.h3_15;
            if (hour == 15) return FifteenMinuteInterval.h3_00;
            return FifteenMinuteInterval.OUT_OF_BOUNDS;
        }
    }
}
