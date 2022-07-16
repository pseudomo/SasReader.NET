using System;
using Ardalis.SmartEnum;
using Deveel.Math;

namespace SasReader
{
    /**
     * Collection of SAS time formats.
     * See: https://v8doc.sas.com/sashtml/lgref/z0309859.htm
     */
    public abstract class SasTimeFormat : SmartEnum<SasTimeFormat>, SasTemporalFormat
    {
        /**
         * Writes time values as hours, minutes, and seconds in the form hh:mm:ss.ss.
         * See: https://v8doc.sas.com/sashtml/lgref/z0197928.htm
         */

        private sealed class TIME_IMPL : SasTimeFormat
        {
            public TIME_IMPL() : base(nameof(TIME), 0, 8) { }

            /**
             * Sometimes rounding based on the given precision increases number of hours and
             * it affects the final width of the time representation.
             * So, seconds should be recalculated and precision may be changed accordingly
             * to fit the time representation into the given width.
             * <p>
             * See example:
             * TIME12.4      TIME11.3     TIME10.2    TIME10.1    TIME10.     TIME9.1
             * 9:59:59.9321  9:59:59.932  9:59:59.93  _9:59:59.9  __10:00:00  9:59:59.9
             * 9:59:59.9875  9:59:59.988  9:59:59.99  10:00:00:0  __10:00:00  _10:00:00
             * 9:59:59.9987  9:59:59.999  10:00:00.0  10:00:00:0  __10:00:00  _10:00:00
             * 9:59:59.9999  10:00:00.00  10:00:00.0  10:00:00:0  __10:00:00  _10:00:00
             *
             * @param sasSeconds       SAS seconds
             * @param width            column format width
             * @param precision        column format precision, adjusted according to actual size
             * @param minIntegralWidth width to fit concatenation of integral part "hours:minutes:seconds"
             * @return integral and remainder parts of dividing given seconds by seconds in a hour.
             */
            private BigDecimal[] roundSeconds(double sasSeconds, int width, int precision, int minIntegralWidth)
            {
                BigDecimal bigSeconds = new BigDecimal(sasSeconds).Abs();

                while (true)
                {
                    BigDecimal[] parts = bigSeconds.Scale(precision, RoundingMode.HalfUp).DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_HOUR);
                    if (precision == 0)
                    {
                        return parts;
                    }
                    string hh = parts[0].ToInt64().ToString();
                    if (hh.Length + minIntegralWidth + precision <= width)
                    {
                        return parts;
                    }
                    precision--;
                }
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    bool negative = sasSeconds < 0;
                    int minIntegralWidth = negative ? "-0:00:00".Length : "0:00:00".Length;

                    int actualPrecision = width > minIntegralWidth ? Math.Min(width - minIntegralWidth, precision) : 0;

                    BigDecimal[] parts = roundSeconds(sasSeconds.Value, width, actualPrecision, minIntegralWidth);


                    string hh = parts[0].ToInt64().ToString();
                    if (negative)
                    {
                        hh = "-" + hh;
                    }
                    if (hh.Length > width)
                    {
                        return new string('*', width);
                    }
                    else if (hh.Length > width - 3)
                    {
                        return hh;
                    }
                    else
                    {
                        parts = parts[1].DivideAndRemainderParts(SasTemporalConstants.BIG_MINUTES_IN_HOUR);
                        string mm = parts[0].ToInt64().ToString();
                        string hhmm = hh + (mm.Length > 1 ? ":" : ":0") + mm;
                        if (hhmm.Length > width - 3)
                        {
                            return hhmm;
                        }
                        else
                        {
                            string ss = parts[1].ToString();
                            return hhmm + (ss.Length > 1 && ss[1] != '.' ? ":" : ":0") + ss;
                        }
                    }
                };
            }
        }

        public static readonly SasTimeFormat TIME = new TIME_IMPL();
        /**
         * Writes time values as the number of minutes and seconds since midnight.
         * See: https://v8doc.sas.com/sashtml/lgref/z0198053.htm
         */

        private sealed class MMSS_IMPL : SasTimeFormat
        {
            public MMSS_IMPL() : base(nameof(MMSS), 1, 5) { }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    bool negative = sasSeconds < 0;

                    BigDecimal[] parts = new BigDecimal(sasSeconds.Value).Abs()
                            .Scale(precision, RoundingMode.HalfUp)
                            .DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_MINUTE);

                    string mm = parts[0].ToInt64().ToString();
                    if (negative)
                    {
                        mm = "-" + mm;
                    }
                    if (mm.Length > width)
                    {
                        return "**";
                    }
                    else if (mm.Length > width - 3)
                    {
                        return mm;
                    }
                    else
                    {
                        string ss = parts[1].ToString();
                        string mmss = mm + (ss.Length > 1 && ss[1] != '.' ? ":" : ":0") + ss;
                        if (mmss.Length > width)
                        {
                            return mmss.Substring(0, width);
                        }
                        else
                        {
                            return mmss;
                        }
                    }
                };
            }
        }

        public static readonly SasTimeFormat MMSS = new MMSS_IMPL();
        /**
         * Writes time values as hours and minutes in the form hh:mm.
         * See: https://v8doc.sas.com/sashtml/lgref/z0198049.htm
         */

        private sealed class HHMM_IMPL : SasTimeFormat
        {
            public HHMM_IMPL() : base(nameof(HHMM), 2, 5) { }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    bool negative = sasSeconds < 0;

                    BigDecimal[] parts = new BigDecimal(sasSeconds.Value).Abs()
                            .Divide(SasTemporalConstants.BIG_SECONDS_IN_MINUTE, precision, RoundingMode.HalfUp)
                            .DivideAndRemainderParts(SasTemporalConstants.BIG_MINUTES_IN_HOUR);

                    string hh = parts[0].ToInt64().ToString();
                    if (negative)
                    {
                        hh = "-" + hh;
                    }
                    if (hh.Length > width)
                    {
                        return "**";
                    }
                    else if (hh.Length > width - 3)
                    {
                        return hh;
                    }
                    else
                    {
                        string mm = parts[1].ToString();
                        string hhmm = hh + (mm.Length > 1 && mm[1] != '.' ? ":" : ":0") + mm;
                        if (hhmm.Length > width)
                        {
                            return hhmm.Substring(0, width);
                        }
                        else
                        {
                            return hhmm;
                        }
                    }
                };
            }
        }

        public static readonly SasTimeFormat HHMM = new HHMM_IMPL();

        /**
         * Writes time values as hours and decimal fractions of hours.
         * See: https://v8doc.sas.com/sashtml/lgref/z0198051.htm
         */

        private sealed class HOUR_IMPL : SasTimeFormat
        {
            public HOUR_IMPL() : base(nameof(HOUR), 3, 2) { }

            /**
             * Round seconds to hours.
             *
             * @param sasSeconds SAS seconds
             * @param width      column format width
             * @param precision  column format precision
             * @return rounded seconds
             */
            private BigDecimal roundAdjustHours(double sasSeconds, int width, int precision)
            {
                BigDecimal bigSeconds = new BigDecimal(sasSeconds / SasTemporalConstants.SECONDS_IN_HOUR).Abs();
                BigDecimal hours = bigSeconds.Scale(precision, RoundingMode.HalfUp);
                int adjustedPrecision = precision;

                while (adjustedPrecision > 0 && hours.ToString().Length > width)
                {
                    if (hours.ToInt64() == 0 && width - precision == 1)
                    {
                        // special case for format like ".123" without leading zero.
                        break;
                    }
                    adjustedPrecision--;
                    hours = bigSeconds.Scale(adjustedPrecision, RoundingMode.HalfUp);
                }
                return hours;
            }

            /**
             * Try to store large hours as a number in E-notation.
             *
             * @param hours hours
             * @param width column format width
             * @return hours representation
             */
            private string eNotation(BigDecimal hours, int width)
            {
                int i = 0;
                string hh = null;
                string decimalFormat;
                while (true)
                {
                    if (i == 0)
                    {
                        decimalFormat = "0E0";
                    }
                    else
                    {
                        decimalFormat = "#" + new string('0', i) + "E0";
                    }
                    i++;
                    string tmp = hours.ToDouble().ToString(decimalFormat);
                    if (tmp.Length > width)
                    {
                        break;
                    }
                    else
                    {
                        hh = tmp;
                    }
                }
                return hh;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    BigDecimal hours = roundAdjustHours(sasSeconds.Value, width, precision);

                    if (precision > 0 && hours.ToInt64() == 0 && width - precision == 1)
                    {
                        return hours.ToString().Substring(1);
                    }

                    string hh = hours.ToString();
                    if (width > 2 && hh.Length > width)
                    {
                        hh = eNotation(hours, width);
                    }

                    if (hh == null || hh.Length > width)
                    {
                        return "**";
                    }
                    else
                    {
                        return hh;
                    }
                };
            }
        }

        public static readonly SasTimeFormat HOUR = new HOUR_IMPL();
        /**
         * Writes time values as hours, minutes, and seconds in the form hh:mm:ss.ss with AM or PM.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201272.htm
         */

        private sealed class TIMEAMPM_IMPL : SasTimeFormat
        {
            public TIMEAMPM_IMPL() : base(nameof(TIMEAMPM), 4, 11) { }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasTimeFormat TIMEAMPM = new TIMEAMPM_IMPL();
        /**
         * Writes time values as local time, appending the Coordinated Universal Time (UTC) offset
         * for the local SAS session, using the ISO 8601 extended time notation hh:mm:ss+|–hh:mm.
         */

        private sealed class E8601LZ_IMPL : SasTimeFormat
        {
            public E8601LZ_IMPL() : base(nameof(E8601LZ), 5, 0) { }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasTimeFormat E8601LZ = new E8601LZ_IMPL();
        /**
         * Writes time values by using the ISO 8601 extended notation hh:mm:ss.ffffff.
         */
        private sealed class E8601TM_IMPL : SasTimeFormat
        {
            public E8601TM_IMPL() : base(nameof(E8601TM), 6, 8) { }

            public override int getActualPrecision(int width, int precision)
            {
                return width > getDefaultWidth() ? Math.Min(width - getDefaultWidth(), precision) : 0;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    if (sasSeconds < 0 || sasSeconds > SasTemporalConstants.SECONDS_IN_DAY)
                    {
                        return new string('*', width);
                    }

                    BigDecimal daySeconds = SasTemporalUtils.roundSeconds(sasSeconds.Value, precision)
                            .Remainder(SasTemporalConstants.BIG_SECONDS_IN_DAY);
                    BigDecimal[] parts = daySeconds.DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_HOUR);

                    int adjustedPrecision = precision;
                    int firstTwoDigitsNumberAfterSingleDigit = 10;
                    if (parts[0].ToInt64() >= firstTwoDigitsNumberAfterSingleDigit
                            && precision > 0 && width - precision == 8)
                    {
                        adjustedPrecision = precision - 1;
                        daySeconds = SasTemporalUtils.roundSeconds(sasSeconds.Value, adjustedPrecision)
                                .Remainder(SasTemporalConstants.BIG_SECONDS_IN_DAY);
                        parts = daySeconds.DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_HOUR);
                    }

                    string hh = parts[0].ToInt64().ToString();
                    if (hh.Length == 1 && !(width == 4 || width == 7
                            || (adjustedPrecision > 0 && width - 8 == adjustedPrecision)))
                    {
                        hh = '0' + hh;
                    }

                    parts = parts[1].DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_MINUTE);
                    string mm = parts[0].ToInt64().ToString();
                    string hhmm = hh + (mm.Length == 1 ? ":0" : ":") + mm;

                    string ss = parts[1].ToInt64().ToString();
                    string hhmmss = hhmm + (ss.Length == 1 ? ":0" : ":") + ss;
                    if (adjustedPrecision == 0 || hhmmss.Length > width - adjustedPrecision)
                    {
                        return hhmmss;
                    }
                    else
                    {
                        adjustedPrecision = Math.Min(adjustedPrecision, width - hhmmss.Length);
                        string nanos = parts[1].Remainder(BigDecimal.One).ToString()
                                .Substring(1, adjustedPrecision + 2);
                        return hhmmss + nanos;
                    }
                };
            }
        }

        public static readonly SasTimeFormat E8601TM = new E8601TM_IMPL();

        /**
         * Default column format width.
         */
        private int defaultWidth;

        /**
         * Enum constructor.
         *
         * @param defaultWidth default width for format
         */
        private SasTimeFormat(string name, int value, int defaultWidth) : base(name, value)
        {
            this.defaultWidth = defaultWidth;
        }
        public int getDefaultWidth()
        {
            return defaultWidth;
        }

        public virtual int getActualPrecision(int width, int precision)
        {
            return precision;
        }

        public abstract Func<double?, string> getInternalFormatFunction(int width, int precision);

        public virtual Func<double?, string> getFallbackFormatFunction(int width, int precision)
        {
            return TIME.getInternalFormatFunction(8, 0);
        }
    }
}