using Deveel.Math;
using System;
using System.Globalization;
using System.Text;

namespace SasReader
{
    /**
     * Utility methods for dates.
     */
    public class SasTemporalUtils
    {
        /**
         * Disable creation of utility instances.
         */
        private SasTemporalUtils()
        {
        }

        /**
         * Repeat character n times to create String.
         *
         * @param character char
         * @param nTimes    number of times
         * @return String
         */
        public static String nChars(char character, int nTimes)
        {
            if (nTimes == 1)
            {
                return character.ToString();
            }
            if (nTimes <= 30)
            {
                switch (character)
                {
                    case ' ':
                        return "                              "[..nTimes];
                    case '*':
                        return "******************************"[..nTimes];
                    case '0':
                        return "000000000000000000000000000000"[..nTimes];
                    case 'S':
                        return "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"[..nTimes];
                    default:
                        break;
                }
            }
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < nTimes; i++)
            {
                str.Append(character);
            }
            return str.ToString();
        }

        /**
         * Convert SAS days to unix epoch seconds.
         *
         * @param sasDays SAS days
         * @return UNIX seconds
         */
        public static double sasDaysToEpochSeconds(double sasDays)
        {
            return sasDays * SasTemporalConstants.SECONDS_IN_DAY - SasTemporalConstants.SAS_VS_EPOCH_DIFF_SECONDS;
        }

        /**
         * Convert SAS seconds to unix epoch seconds.
         *
         * @param sasSeconds SAS seconds
         * @return UNIX seconds
         */
        public static double sasSecondsToEpochSeconds(double sasSeconds)
        {
            return sasSeconds - SasTemporalConstants.SAS_VS_EPOCH_DIFF_SECONDS;
        }

        /**
         * Convert SAS days to Java Date.
         *
         * @param sasDays SAS days
         * @return date
         */
        public static DateTime sasDaysToDate(double sasDays)
        {
            return DateTime.UnixEpoch.AddMilliseconds((long)(sasDaysToEpochSeconds(sasDays) * SasTemporalConstants.MILLIS_IN_SECOND));
        }

        /**
         * Convert SAS seconds to Java date.
         *
         * @param sasSeconds SAS seconds
         * @return date
         */
        public static DateTime sasSecondsToDate(double sasSeconds)
        {
            return DateTime.UnixEpoch.AddMilliseconds((long)(sasSecondsToEpochSeconds(sasSeconds) * SasTemporalConstants.MILLIS_IN_SECOND));
        }

        /**
         * Convert SAS days to Java LocalDate.
         *
         * @param sasDays SAS days
         * @return date
         */
        public static DateTime sasDaysToLocalDate(double sasDays)
        {
            return DateTime.UnixEpoch.AddDays((long)Math.Floor(sasDays) - SasTemporalConstants.SAS_VS_EPOCH_DIFF_DAYS);
        }

        /**
         * Round SAS seconds. It is not a regular rounding.
         * SAS rounds negative (dates before 1960 year) and positive dates (after 1960 year)
         * in different ways.
         * See examples
         * <p>
         * SAS value        SAS format DATE22.2        SAS format DATE22.3
         * ----------------|------------------------|-----------------------
         * negative dates rounding
         * -395163560.356     24JUN1947:08:20:39.64  24JUN1947:08:20:39.644
         * -394053550.355     07JUL1947:04:40:49.65  07JUL1947:04:40:49.645
         * -392943540.354     20JUL1947:01:00:59.65  20JUL1947:01:00:59.646
         * <p>
         * positive dates rounding
         * 392943540.354     13JUN1972:22:59:00.35   13JUN1972:22:59:00.354
         * 394053550.355     26JUN1972:19:19:10.35   26JUN1972:19:19:10.355
         * 395163560.356     09JUL1972:15:39:20.36   09JUL1972:15:39:20.356
         * <p>
         * rounding near to 24h
         * -11896934400.001  31DEC1582:23:59:59.99   31DEC1582:23:59:59.999
         * 1679183999.999    17MAR2013:23:59:59.99   17MAR2013:23:59:59.999
         * <p>
         * Rounding UP may increase seconds, minutes or hours, but it never
         * increase day. So the biggest rounded time is always less than 24.
         *
         * @param sasSeconds SAS seconds
         * @param precision  column format precision
         * @return rounded seconds with fraction
         */
        public static BigDecimal roundSeconds(double sasSeconds, int precision)
        {
            var seconds = new BigDecimal(sasSeconds, new MathContext(precision, sasSeconds < 0 ? RoundingMode.HalfDown : RoundingMode.HalfUp));

            if (seconds.Remainder(SasTemporalConstants.BIG_SECONDS_IN_DAY).CompareTo(BigDecimal.Zero) == 0)
            {
                if (seconds.ToDouble() > sasSeconds)
                {
                    seconds = seconds.Subtract(SasTemporalConstants.BIG_NANOSECOND_FRACTION).Scale(precision, RoundingMode.Floor);
                }
            }
            return seconds;
        }


        /**
         * Convert SAS seconds to Java LocalDateTime.
         * Internally it applies SAS-specific rounding for negative dates.
         *
         * @param sasSeconds SAS seconds
         * @param precision  column format precision
         * @return date
         */
        public static DateTime sasSecondsToLocalDateTime(double sasSeconds, int precision)
        {
            BigDecimal bigSeconds = roundSeconds(sasSeconds, precision);
            BigDecimal nanosFraction = bigSeconds.Remainder(BigDecimal.One);
            if (nanosFraction.CompareTo(BigDecimal.Zero) < 0)
            {
                nanosFraction = nanosFraction.Add(BigDecimal.One);
            }
            DateTime dateTime = DateTime.UnixEpoch
                .AddSeconds(bigSeconds.Scale(0, RoundingMode.Floor).ToInt64() - SasTemporalConstants.SAS_VS_EPOCH_DIFF_SECONDS)
                .AddTicks(nanosFraction.Multiply(SasTemporalConstants.BIG_NANOS_IN_SECOND).ToInt64() / 100);
            return dateTime;
        }

        /**
         * Create DateTimeFormatter instance based on pattern in UTC timezone for US locale.
         *
         * @param datePattern date pattern
         * @return formatter
         */
        public static DateTimeFormatter createDateTimeFormatterFromPattern(string datePattern)
        {
            return DateTimeFormatter
                .OfPattern(datePattern)
                .WithZone(TimeZoneInfo.Utc)
                .WithCulture(DateTimeFormatter.DefaultCulture);
        }

        /**
         * SAS removes leap day every 4000 year.
         * It removes these days:
         * - 29FEB4000
         * - 29FEB8000
         * This guy proposed such approach many years ago: https://en.wikipedia.org/wiki/John_Herschel
         * <p>
         * Sometimes people discussed why SAS dates are so strange:
         * - https://blogs.sas.com/content/sasdummy/2010/04/05/in-the-year-9999/
         * - https://communities.sas.com/t5/SAS-Programming/Leap-Years-divisible-by-4000/td-p/663467
         * <p>
         * See the SAS program and its output:
         * ```shell
         * data test;
         * dtime = '28FEB4000:00:00:00'dt;
         * put dtime; *out: 64381219200
         * <p>
         * dtime = '29FEB4000:00:00:00'dt;
         * put dtime; *err: ERROR: Invalid date/time/datetime constant '29FEB4000:00:00:00'dt.
         * <p>
         * dtime = '01MAR4000:00:00:00'dt;
         * put dtime; *out: 64381305600
         * <p>
         * dtime = '31DEC4000:00:00:00'dt;
         * put dtime; *out: 64407657600
         * <p>
         * dtime = '28FEB8000:00:00:00'dt;
         * put dtime; *out: 190608940800
         * <p>
         * dtime = '29FEB8000:00:00:00'dt;
         * put dtime; *err: ERROR: Invalid date/time/datetime constant '29FEB8000:00:00:00'dt.
         * <p>
         * dtime = '01MAR8000:00:00:00'dt;
         * put dtime; * out: 190609027200
         * <p>
         * dtime = '31DEC8000:00:00:00'dt;
         * put dtime; *out: 190635379200
         * <p>
         * dtime = '31DEC9999:00:00:00'dt;
         * put dtime; *out: 253717660800
         * run;
         * ```
         * As you can see SAS doesn't accept leap days for 4000 and 8000 years
         * and removes these days at all from the SAS calendar.
         * <p>
         * At the same time these leap days are ok for:
         * - Java: `LocalDateTime.of(4000, 2, 29, 0, 0).toEpochSecond(ZoneOffset.UTC)`
         * outputs 64065686400
         * - JavaScript: `Date.parse('4000-02-29')`
         * outputs 64065686400000
         * - GNU/date: `date --utc --date '4000-02-29' +%s`
         * outputs 64065686400
         * and so on.
         * <p>
         * So, in order to parse SAS dates correctly,
         * we need to restore removed leap days
         *
         * @param sasSeconds SAS date representation in seconds since 1960-01-01
         * @return seconds with restored leap days
         */
        public static double sasLeapSecondsFix(double sasSeconds)
        {
            if (sasSeconds >= SasTemporalConstants.SAS_SECONDS_29FEB4000)
            {
                if (sasSeconds >= SasTemporalConstants.SAS_SECONDS_29FEB8000)
                {
                    sasSeconds += SasTemporalConstants.SECONDS_IN_DAY; //restore Y8K leap day
                }
                sasSeconds += SasTemporalConstants.SECONDS_IN_DAY; //restore Y4K leap day
            }
            return sasSeconds;
        }

        /**
         * The same as sasLeapSecondsFix but for days.
         *
         * @param sasDays SAS days
         * @return fixed days
         */
        public static double sasLeapDaysFix(double sasDays)
        {
            if (sasDays >= SasTemporalConstants.SAS_DAYS_29FEB4000)
            {
                if (sasDays >= SasTemporalConstants.SAS_DAYS_29FEB8000)
                {
                    sasDays += 1; //restore Y8K leap day
                }
                sasDays += 1; //restore Y4K leap day
            }
            return sasDays;
        }
    }
}