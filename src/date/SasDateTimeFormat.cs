using System;
using Ardalis.SmartEnum;
using Deveel.Math;

namespace SasReader
{
    /**
     * Collection of SAS datetime formats.
     * See: https://v8doc.sas.com/sashtml/lgref/z0309859.htm
     * <p>
     * Note that SAS and Java have difference in calendars after the year 4000
     * because of the different amount of leap days after that year.
     * In this implementation week days and week day names are formatted
     * using Java calendar for dates after the 28Feb4000.
     */
    public abstract class SasDateTimeFormat : SmartEnum<SasDateTimeFormat>, SasTemporalFormat
    {
        /**
         * Writes datetime values in the form ddmmmyy:hh:mm:ss.ss.
         * See: https://v8doc.sas.com/sashtml/lgref/z0197923.htm
         */

        private sealed class DATETIME_IMPL : SasDateTimeFormat
        {
            public DATETIME_IMPL() : base(nameof(DATETIME), 0, 16) { }

            /**
             * Calculate pattern string based on width, without fractional part.
             * @param width column format width
             * @return pattern
             */
            private string getNoFractionDatePattern(int width)
            {
                if (width >= 19)
                {
                    return "ddMMMyyyy:HH:mm:ss";
                }
                switch (width)
                {
                    case 7:
                    case 8:
                        return "ddMMMyy";
                    case 9:
                        return "ddMMMyyyy";
                    case 10:
                    case 11:
                    case 12:
                        return "ddMMMyy:HH";
                    case 13:
                    case 14:
                    case 15:
                        return "ddMMMyy:HH:mm";
                    case 16:
                    case 17:
                    case 18:
                    default:
                        return "ddMMMyy:HH:mm:ss";
                }
            }

            public override string getDatePattern(int width, int precision)
            {
                int noFractionWidth = width - precision;
                if (noFractionWidth < 16)
                {
                    noFractionWidth = width;
                }

                string noFractionString = getNoFractionDatePattern(noFractionWidth);

                if (precision > 0)
                {
                    int fractionWidth = Math.Min(precision, width - 17);
                    if (fractionWidth == 0)
                    {
                        return noFractionString + '.';
                    }
                    else if (fractionWidth > 0)
                    {
                        return noFractionString + '.' + new string('S', fractionWidth);
                    }
                }
                return noFractionString;
            }

            public int getActualPrecision(int width, int precision)
            {
                return width > getDefaultWidth() ? Math.Min(width - getDefaultWidth(), precision) : 0;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) => base.getInternalFormatFunction(width, precision).Invoke(d).ToUpper();
            }
        }

        public static readonly SasDateTimeFormat DATETIME = new DATETIME_IMPL();
        /**
         * Writes dates from datetime values by using the ISO 8601 basic notation yyyymmdd.
         */

        private sealed class B8601DN_IMPL : SasDateTimeFormat
        {
            public B8601DN_IMPL() : base(nameof(B8601DN), 1, 10) { }

            public override string getDatePattern(int width, int precision)
            {
                return "yyyyMMdd";
            }
        }

        public static readonly SasDateTimeFormat B8601DN = new B8601DN_IMPL();
        /**
         * Writes datetime values by using the ISO 8601 basic notation yyyymmddThhmmss<ffffff>.
         */

        private sealed class B8601DT_IMPL : SasDateTimeFormat
        {
            public B8601DT_IMPL() : base(nameof(B8601DT), 2, 19) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat B8601DT = new B8601DT_IMPL();
        /**
         * Adjusts a Coordinated Universal Time (UTC) datetime value to the user local date and time.
         * Then, writes the local date and time by using the ISO 8601 datetime
         * and time zone basic notation yyyymmddThhmmss+hhmm.
         */

        private sealed class B8601DX_IMPL : SasDateTimeFormat
        {
            public B8601DX_IMPL() : base(nameof(B8601DX), 3, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat B8601DX = new B8601DX_IMPL();
        /**
         * Reads Coordinated Universal Time (UTC) datetime values that are specified using the
         * ISO 8601 datetime basic notation yyyymmddThhmmss+|–hhmm or yyyymmddThhmmss<ffffff>Z.
         */

        private sealed class B8601DZ_IMPL : SasDateTimeFormat
        {
            public B8601DZ_IMPL() : base(nameof(B8601DZ), 4, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat B8601DZ = new B8601DZ_IMPL();
        /**
         * Writes datetime values as local time by appending a time zone offset difference between the local time and UTC,
         * using the ISO 8601 basic notation yyyymmddThhmmss+|–hhmm.
         */

        private sealed class B8601LX_IMPL : SasDateTimeFormat
        {
            public B8601LX_IMPL() : base(nameof(B8601LX), 5, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat B8601LX = new B8601LX_IMPL();
        /**
         * Writes dates from SAS datetime values by using the ISO 8601 extended notation yyyy-mm-dd.
         */

        private sealed class E8601DN_IMPL : SasDateTimeFormat
        {
            public E8601DN_IMPL() : base(nameof(E8601DN), 6, 10) { }

            public override string getDatePattern(int width, int precision)
            {
                return "yyyy-MM-dd";
            }
        }

        public static readonly SasDateTimeFormat E8601DN = new E8601DN_IMPL();
        /**
         * Reads datetime values that are specified using the
         * ISO 8601 extended notation yyyy-mm-ddThh:mm:ss.<ffffff>.
         */

        private sealed class E8601DT_IMPL : SasDateTimeFormat
        {
            public E8601DT_IMPL() : base(nameof(E8601DT), 7, 19) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat E8601DT = new E8601DT_IMPL();
        /**
         * Adjusts a Coordinated Universal Time (UTC) datetime value to the user local date and time.
         * Then, writes the local date and time by using the ISO 8601 datetime
         * and time zone extended notation yyyy-mm-ddThh:mm:ss+hh:mm.
         */

        private sealed class E8601DX_IMPL : SasDateTimeFormat
        {
            public E8601DX_IMPL() : base(nameof(E8601DX), 8, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat E8601DX = new E8601DX_IMPL();
        /**
         * Reads Coordinated Universal Time (UTC) datetime values that are specified using the ISO 8601
         * datetime extended notation yyyy-mm-ddThh:mm:ss+|–hh:mm.<fffff> or yyyy-mm-ddThh:mm:ss.<fffff>Z.
         */

        private sealed class E8601DZ_IMPL : SasDateTimeFormat
        {
            public E8601DZ_IMPL() : base(nameof(E8601DZ), 9, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat E8601DZ = new E8601DZ_IMPL();
        /**
         * Writes datetime values as local time by appending a time zone offset difference between the local time and UTC,
         * using the ISO 8601 extended notation yyyy-mm-ddThh:mm:ss+|–hh:mm.
         */

        private sealed class E8601LX_IMPL : SasDateTimeFormat
        {
            public E8601LX_IMPL() : base(nameof(E8601LX), 10, 26) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat E8601LX = new E8601LX_IMPL();
        /**
         * Writes datetime values in the form ddmmmyy:hh:mm:ss.ss with AM or PM.
         * See: https://v8doc.sas.com/sashtml/lgref/z0196050.htm
         */

        private sealed class DATEAMPM_IMPL : SasDateTimeFormat
        {
            public DATEAMPM_IMPL() : base(nameof(DATEAMPM), 11, 19) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat DATEAMPM = new DATEAMPM_IMPL();
        /**
         * Expects a datetime value as input and writes date values in the form ddmmmyy or ddmmmyyyy.
         */

        private sealed class DTDATE_IMPL : SasDateTimeFormat
        {
            public DTDATE_IMPL() : base(nameof(DTDATE), 12, 7) { }

            public override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 9:
                        return "ddMMMyyyy";
                    case 5:
                    case 6:
                        return "ddMMM";
                    case 7:
                    case 8:
                    default:
                        return "ddMMMyy";
                }
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) => base.getInternalFormatFunction(width, precision).Invoke(d).ToUpper();
            }
        }

        public static readonly SasDateTimeFormat DTDATE = new DTDATE_IMPL();
        /**
         * Writes the date part of a datetime value as the month and year in the form mmmyy or mmmyyyy.
         */

        private sealed class DTMONYY_IMPL : SasDateTimeFormat
        {
            public DTMONYY_IMPL() : base(nameof(DTMONYY), 13, 5) { }

            public override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 7:
                        return "MMMyyyy";
                    case 5:
                    case 6:
                    default:
                        return "MMMyy";
                }
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) => base.getInternalFormatFunction(width, precision).Invoke(d).ToUpper();
            }
        }

        public static readonly SasDateTimeFormat DTMONYY = new DTMONYY_IMPL();
        /**
         * Writes the date part of a SAS datetime value as the day of the week and the date in the form
         * day-of-week, dd month-name yy (or yyyy).
         */

        private sealed class DTWKDATX_IMPL : SasDateTimeFormat
        {
            public DTWKDATX_IMPL() : base(nameof(DTWKDATX), 14, 29) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat DTWKDATX = new DTWKDATX_IMPL();
        /**
         * Writes the date part of a SAS datetime value as the year in the form yy or yyyy.
         */

        private sealed class DTYEAR_IMPL : SasDateTimeFormat
        {
            public DTYEAR_IMPL() : base(nameof(DTYEAR), 15, 4) { }

            public override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 2:
                    case 3:
                        return "yy";
                    case 4:
                    default:
                        return "yyyy";
                }
            }
        }

        public static readonly SasDateTimeFormat DTYEAR = new DTYEAR_IMPL();
        /**
         * Writes datetime values in the form mm/dd/yy<yy> hh:mm AM|PM.
         * The year can be either two or four digits.
         */

        private sealed class MDYAMPM_IMPL : SasDateTimeFormat
        {
            public MDYAMPM_IMPL() : base(nameof(MDYAMPM), 16, 10) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateTimeFormat MDYAMPM = new MDYAMPM_IMPL();
        /**
         * Writes the time portion of datetime values in the form hh:mm:ss.ss.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201157.htm
         */

        private sealed class TOD_IMPL : SasDateTimeFormat
        {
            public TOD_IMPL() : base(nameof(TOD), 17, 8) { }

            public override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public int getActualPrecision(int width, int precision)
            {
                return width > getDefaultWidth() ? Math.Min(width - getDefaultWidth(), precision) : 0;
            }

            /**
             * Round and truncate SAS seconds to a single day.
             * @param sasSeconds SAS seconds
             * @param precision column format precision
             * @return seconds
             */

            public BigDecimal daySeconds(double sasSeconds, int precision)
            {
                BigDecimal bigSeconds = SasTemporalUtils.roundSeconds(sasSeconds, precision)
                        .Abs().Remainder(SasTemporalConstants.BIG_SECONDS_IN_DAY);
                if (sasSeconds < 0 && bigSeconds.CompareTo(BigDecimal.Zero) > 0)
                {
                    bigSeconds = SasTemporalConstants.BIG_SECONDS_IN_DAY.Subtract(bigSeconds);
                }
                return bigSeconds;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (sasSeconds) =>
                {
                    BigDecimal daySeconds = this.daySeconds(sasSeconds.Value, precision);
                    BigDecimal[] parts = daySeconds.DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_HOUR);

                    int adjustedPrecision = precision;
                    int firstTwoDigitsNumberAfterSingleDigit = 10;
                    if (parts[0].ToInt64() >= firstTwoDigitsNumberAfterSingleDigit
                            && precision > 0 && width - precision == 8)
                    {
                        adjustedPrecision = precision - 1;
                        daySeconds = this.daySeconds(sasSeconds.Value, adjustedPrecision);
                        parts = daySeconds.DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_HOUR);
                    }

                    string hh = parts[0].ToInt64().ToString();
                    if (hh.Length == 1 && !(width == 4 || width == 7
                            || (adjustedPrecision > 0 && width - 8 == adjustedPrecision)))
                    {
                        hh = '0' + hh;
                    }

                    if (hh.Length > width - 3)
                    {
                        return hh;
                    }
                    else
                    {
                        parts = parts[1].DivideAndRemainderParts(SasTemporalConstants.BIG_SECONDS_IN_MINUTE);
                        string mm = parts[0].ToInt64().ToString();
                        string hhmm = hh + (mm.Length == 1 ? ":0" : ":") + mm;
                        if (hhmm.Length > width - 3)
                        {
                            return hhmm;
                        }
                        else
                        {
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
                        }
                    }
                };
            }
        }

        public static readonly SasDateTimeFormat TOD = new TOD_IMPL();

        /**
         * Default format width.
         * In terms of SAS "w specifies the width of the output field".
         */
        private int defaultWidth;

        /**
         * Enum constructor.
         *
         * @param defaultWidth default width for format
         */
        private SasDateTimeFormat(string name, int value, int defaultWidth) : base(name, value)
        {
            this.defaultWidth = defaultWidth;
        }
        public int getDefaultWidth()
        {
            return defaultWidth;
        }

        /**
         * Creates width-specific date pattern compatible with the java.time.format.DateTimeFormatter.
         *
         * @param width     column format width
         * @param precision column format precision
         * @return java date pattern
         */
        public abstract string getDatePattern(int width, int precision);

        public virtual Func<double?, string> getFallbackFormatFunction(int width, int precision)
        {
            return DATETIME.getInternalFormatFunction(16, 0);
        }

        public virtual Func<double?, string> getInternalFormatFunction(int width, int precision)
        {
            String datePattern = getDatePattern(width, precision);
            DateTimeFormatter formatter = DateTimeFormatter.OfPattern(datePattern);
            return (sasSeconds) => formatter.Format(SasTemporalUtils.sasSecondsToLocalDateTime(sasSeconds.Value, precision));
        }
    }
}