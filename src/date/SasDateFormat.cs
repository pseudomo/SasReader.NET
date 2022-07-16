using System;
using Ardalis.SmartEnum;


namespace SasReader
{
    /**
     * Collection of SAS date formats.
     * See: https://v8doc.sas.com/sashtml/lgref/z0309859.htm
     * <p>
     * Note that SAS and Java have difference in calendars after the year 4000
     * because of the different amount of leap days after that year.
     * In this implementation week days and week day names are formatted
     * using Java calendar for dates after the 28Feb4000.
     */
    public abstract class SasDateFormat : SmartEnum<SasDateFormat>, SasTemporalFormat
    {
        /**
         * Writes date values in the form ddmmmyy or ddmmmyyyy.
         * See: https://v8doc.sas.com/sashtml/lgref/z0195834.htm
         * Actually SAS also supports DATE10 (same result as DATE9)
         * and DATE11 (with dash as separator).
         */

        private sealed class DATE_IMPL : SasDateFormat
        {
            public DATE_IMPL() : base(nameof(DATE), 0, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 5:
                    case 6:
                        return "ddMMM";
                    case 9:
                    case 10:
                        return "ddMMMyyyy";
                    case 11:
                        return "dd-MMM-yyyy";
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

        public static readonly SasDateFormat DATE = new DATE_IMPL();
        /**
         * Writes date values as the day of the month.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201472.htm
         */

        private sealed class DAY_IMPL : SasDateFormat
        {
            public DAY_IMPL() : base(nameof(DAY), 1, 2) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "d";
            }
        }

        public static readonly SasDateFormat DAY = new DAY_IMPL();
        /**
         * Writes date values in the form ddmmyy or ddmmyyyy.
         * https://v8doc.sas.com/sashtml/lgref/z0197953.htm
         * See also:
         * https://v8doc.sas.com/sashtml/lgref/z0590669.htm
         */

        private sealed class DDMMYY_IMPL : SasDateFormat
        {
            public DDMMYY_IMPL() : base(nameof(DDMMYY), 2, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat DDMMYY = new DDMMYY_IMPL();
        /**
         * DDMMYYB with a blank separator.
         */

        private sealed class DDMMYYB_IMPL : SasDateFormat
        {
            public DDMMYYB_IMPL() : base(nameof(DDMMYYB), 3, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, " ");
            }
        }

        public static readonly SasDateFormat DDMMYYB = new DDMMYYB_IMPL();
        /**
         * DDMMYYC with a colon separator.
         */

        private sealed class DDMMYYC_IMPL : SasDateFormat
        {
            public DDMMYYC_IMPL() : base(nameof(DDMMYYC), 4, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, ":");
            }
        }

        public static readonly SasDateFormat DDMMYYC = new DDMMYYC_IMPL();
        /**
         * DDMMYYD with a dash separator.
         */

        private sealed class DDMMYYD_IMPL : SasDateFormat
        {
            public DDMMYYD_IMPL() : base(nameof(DDMMYYD), 5, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat DDMMYYD = new DDMMYYD_IMPL();
        /**
         * DDMMYY with N indicates no separator.
         * When x is N, the width range is 2-8.
         */

        private sealed class DDMMYYN_IMPL : SasDateFormat
        {
            public DDMMYYN_IMPL() : base(nameof(DDMMYYN), 6, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, "");
            }
        }

        public static readonly SasDateFormat DDMMYYN = new DDMMYYN_IMPL();
        /**
         * DDMMYYP with a period separator.
         */

        private sealed class DDMMYYP_IMPL : SasDateFormat
        {
            public DDMMYYP_IMPL() : base(nameof(DDMMYYP), 7, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, ".");
            }
        }

        public static readonly SasDateFormat DDMMYYP = new DDMMYYP_IMPL();
        /**
         * DDMMYYS with a slash separator.
         */

        private sealed class DDMMYYS_IMPL : SasDateFormat
        {
            public DDMMYYS_IMPL() : base(nameof(DDMMYYS), 8, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getDDMMYYxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat DDMMYYS = new DDMMYYS_IMPL();
        /**
         * Writes date values in the form mmddyy or mmddyyyy.
         * https://v8doc.sas.com/sashtml/lgref/z0199367.htm
         * See also:
         * https://v8doc.sas.com/sashtml/lgref/z0590662.htm
         */

        private sealed class MMDDYY_IMPL : SasDateFormat
        {
            public MMDDYY_IMPL() : base(nameof(MMDDYY), 9, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat MMDDYY = new MMDDYY_IMPL();
        /**
         * MMDDYYB with a blank separator.
         */

        private sealed class MMDDYYB_IMPL : SasDateFormat
        {
            public MMDDYYB_IMPL() : base(nameof(MMDDYYB), 10, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, " ");
            }
        }

        public static readonly SasDateFormat MMDDYYB = new MMDDYYB_IMPL();
        /**
         * MMDDYYC with a colon separator.
         */

        private sealed class MMDDYYC_IMPL : SasDateFormat
        {
            public MMDDYYC_IMPL() : base(nameof(MMDDYYC), 11, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, ":");
            }
        }

        public static readonly SasDateFormat MMDDYYC = new MMDDYYC_IMPL();
        /**
         * MMDDYYD with a dash separator.
         */

        private sealed class MMDDYYD_IMPL : SasDateFormat
        {
            public MMDDYYD_IMPL() : base(nameof(MMDDYYD), 12, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat MMDDYYD = new MMDDYYD_IMPL();
        /**
         * MMDDYY with N indicates no separator.
         * When x is N, the width range is 2-8.
         */

        private sealed class MMDDYYN_IMPL : SasDateFormat
        {
            public MMDDYYN_IMPL() : base(nameof(MMDDYYN), 13, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, "");
            }
        }

        public static readonly SasDateFormat MMDDYYN = new MMDDYYN_IMPL();
        /**
         * MMDDYYP with a period separator.
         */

        private sealed class MMDDYYP_IMPL : SasDateFormat
        {
            public MMDDYYP_IMPL() : base(nameof(MMDDYYP), 14, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, ".");
            }
        }

        public static readonly SasDateFormat MMDDYYP = new MMDDYYP_IMPL();
        /**
         * MMDDYYS with a slash separator.
         */

        private sealed class MMDDYYS_IMPL : SasDateFormat
        {
            public MMDDYYS_IMPL() : base(nameof(MMDDYYS), 15, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMDDYYxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat MMDDYYS = new MMDDYYS_IMPL();
        /**
         * Writes date values in the form yymmdd or yyyymmdd.
         * https://v8doc.sas.com/sashtml/lgref/z0197961.htm
         * See also:
         * https://v8doc.sas.com/sashtml/lgref/z0589916.htm
         */

        private sealed class YYMMDD_IMPL : SasDateFormat
        {
            public YYMMDD_IMPL() : base(nameof(YYMMDD), 16, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat YYMMDD = new YYMMDD_IMPL();
        /**
         * YYMMDDB with a blank separator.
         */

        private sealed class YYMMDDB_IMPL : SasDateFormat
        {
            public YYMMDDB_IMPL() : base(nameof(YYMMDDB), 17, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, " ");
            }
        }

        public static readonly SasDateFormat YYMMDDB = new YYMMDDB_IMPL();
        /**
         * YYMMDDC with a colon separator.
         */

        private sealed class YYMMDDC_IMPL : SasDateFormat
        {
            public YYMMDDC_IMPL() : base(nameof(YYMMDDC), 18, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, ":");
            }
        }

        public static readonly SasDateFormat YYMMDDC = new YYMMDDC_IMPL();
        /**
         * YYMMDDD with a dash separator.
         */

        private sealed class YYMMDDD_IMPL : SasDateFormat
        {
            public YYMMDDD_IMPL() : base(nameof(YYMMDDD), 19, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat YYMMDDD = new YYMMDDD_IMPL();
        /**
         * YYMMDD with N indicates no separator.
         * When x is N, the width range is 2-8.
         */

        private sealed class YYMMDDN_IMPL : SasDateFormat
        {
            public YYMMDDN_IMPL() : base(nameof(YYMMDDN), 20, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, "");
            }
        }

        public static readonly SasDateFormat YYMMDDN = new YYMMDDN_IMPL();
        /**
         * YYMMDDP with a period separator.
         */

        private sealed class YYMMDDP_IMPL : SasDateFormat
        {
            public YYMMDDP_IMPL() : base(nameof(YYMMDDP), 21, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, ".");
            }
        }

        public static readonly SasDateFormat YYMMDDP = new YYMMDDP_IMPL();
        /**
         * YYMMDDS with a slash separator.
         */

        private sealed class YYMMDDS_IMPL : SasDateFormat
        {
            public YYMMDDS_IMPL() : base(nameof(YYMMDDS), 22, 8) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMDDxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat YYMMDDS = new YYMMDDS_IMPL();
        /**
         * Writes date values as the month and the year and separates them with a character.
         * https://v8doc.sas.com/sashtml/lgref/z0199314.htm
         * MMYY with a M separator.
         */

        private sealed class MMYY_IMPL : SasDateFormat
        {
            public MMYY_IMPL() : base(nameof(MMYY), 23, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMYYxwFormatPattern(width, "'M'");
            }
        }

        public static readonly SasDateFormat MMYY = new MMYY_IMPL();
        /**
         * MMYYC with a colon separator.
         */

        private sealed class MMYYC_IMPL : SasDateFormat
        {
            public MMYYC_IMPL() : base(nameof(MMYYC), 24, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMYYxwFormatPattern(width, ":");
            }
        }

        public static readonly SasDateFormat MMYYC = new MMYYC_IMPL();
        /**
         * MMYYD with a dash separator.
         */

        private sealed class MMYYD_IMPL : SasDateFormat
        {
            public MMYYD_IMPL() : base(nameof(MMYYD), 25, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMYYxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat MMYYD = new MMYYD_IMPL();
        /**
         * MMYY with N indicates no separator.
         * When no separator is specified, the width range is 4-32 and the default changes to 6.
         */

        private sealed class MMYYN_IMPL : SasDateFormat
        {
            public MMYYN_IMPL() : base(nameof(MMYYN), 26, 6) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 4:
                    case 5:
                        return "MMyy";
                    default:
                        return "MMyyyy";
                }
            }
        }

        public static readonly SasDateFormat MMYYN = new MMYYN_IMPL();
        /**
         * MMYYP with a period separator.
         */

        private sealed class MMYYP_IMPL : SasDateFormat
        {
            public MMYYP_IMPL() : base(nameof(MMYYP), 27, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMYYxwFormatPattern(width, ".");
            }
        }

        public static readonly SasDateFormat MMYYP = new MMYYP_IMPL();
        /**
         * MMYYS with a slash separator.
         */

        private sealed class MMYYS_IMPL : SasDateFormat
        {
            public MMYYS_IMPL() : base(nameof(MMYYS), 28, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getMMYYxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat MMYYS = new MMYYS_IMPL();
        /**
         * Writes date values as the year and month and separates them with a character.
         * https://v8doc.sas.com/sashtml/lgref/z0199309.htm
         * YYMM with a M separator.
         */

        private sealed class YYMM_IMPL : SasDateFormat
        {
            public YYMM_IMPL() : base(nameof(YYMM), 29, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMxwFormatPattern(width, "'M'");
            }
        }

        public static readonly SasDateFormat YYMM = new YYMM_IMPL();
        /**
         * YYMMC with a colon separator.
         */

        private sealed class YYMMC_IMPL : SasDateFormat
        {
            public YYMMC_IMPL() : base(nameof(YYMMC), 30, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMxwFormatPattern(width, ":");
            }
        }

        public static readonly SasDateFormat YYMMC = new YYMMC_IMPL();
        /**
         * YYMMD with a dash separator.
         */

        private sealed class YYMMD_IMPL : SasDateFormat
        {
            public YYMMD_IMPL() : base(nameof(YYMMD), 31, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMxwFormatPattern(width, "-");
            }
        }

        public static readonly SasDateFormat YYMMD = new YYMMD_IMPL();
        /**
         * YYMM with N indicates no separator.
         * When no separator is specified, the width range is 4-32 and the default changes to 6.
         */

        private sealed class YYMMN_IMPL : SasDateFormat
        {
            public YYMMN_IMPL() : base(nameof(YYMMN), 32, 6) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 4:
                    case 5:
                        return "yyMM";
                    default:
                        return "yyyyMM";
                }
            }
        }

        public static readonly SasDateFormat YYMMN = new YYMMN_IMPL();
        /**
         * YYMMP with a period separator.
         */

        private sealed class YYMMP_IMPL : SasDateFormat
        {
            public YYMMP_IMPL() : base(nameof(YYMMP), 33, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMxwFormatPattern(width, ".");
            }
        }

        public static readonly SasDateFormat YYMMP = new YYMMP_IMPL();
        /**
         * YYMMS with a slash separator.
         */

        private sealed class YYMMS_IMPL : SasDateFormat
        {
            public YYMMS_IMPL() : base(nameof(YYMMS), 34, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                return getYYMMxwFormatPattern(width, "/");
            }
        }

        public static readonly SasDateFormat YYMMS = new YYMMS_IMPL();
        /**
         * Writes date values as Julian dates in the form yyddd or yyyyddd.
         * See: https://v8doc.sas.com/sashtml/lgref/z0197940.htm
         */

        private sealed class JULIAN_IMPL : SasDateFormat
        {
            public JULIAN_IMPL() : base(nameof(JULIAN), 35, 5) { }

            protected override string getDatePattern(int width, int precision)
            {
                if (width == 7)
                {
                    return "yyyyDDD";
                }
                else
                {
                    return "yyDDD";
                }
            }
        }

        public static readonly SasDateFormat JULIAN = new JULIAN_IMPL();
        /**
         * Writes date values as the Julian day of the year.
         * See: https://v8doc.sas.com/sashtml/lgref/z0205162.htm
         */

        private sealed class JULDAY_IMPL : SasDateFormat
        {
            public JULDAY_IMPL() : base(nameof(JULDAY), 36, 3) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "D";
            }
        }

        public static readonly SasDateFormat JULDAY = new JULDAY_IMPL();
        /**
         * Writes date values as the month.
         * See: https://v8doc.sas.com/sashtml/lgref/z0171689.htm
         * Note that MONTH1. returns HEX value.
         */

        private sealed class MONTH_IMPL : SasDateFormat
        {
            public MONTH_IMPL() : base(nameof(MONTH), 37, 2) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "%M";
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                if (width == 1)
                {
                    return (sasDays) =>
                    {
                        DateTime date = SasTemporalUtils.sasDaysToLocalDate(sasDays.Value);
                        return date.Month.ToString();
                    };
                }
                else
                {
                    return (d) => base.getInternalFormatFunction(width, precision).Invoke(d).ToUpper();
                }
            }
        }

        public static readonly SasDateFormat MONTH = new MONTH_IMPL();
        /**
         * Writes date values as the year.
         * See: https://v8doc.sas.com/sashtml/lgref/z0205234.htm
         */

        private sealed class YEAR_IMPL : SasDateFormat
        {
            public YEAR_IMPL() : base(nameof(YEAR), 38, 4) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 2:
                    case 3:
                        return "yy";
                    default:
                        return "yyyy";
                }
            }
        }

        public static readonly SasDateFormat YEAR = new YEAR_IMPL();
        /**
         * Writes date values as the month and the year in the form mmmyy or mmmyyyy.
         * See: https://v8doc.sas.com/sashtml/lgref/z0197959.htm
         */

        private sealed class MONYY_IMPL : SasDateFormat
        {
            public MONYY_IMPL() : base(nameof(MONYY), 39, 5) { }

            protected override string getDatePattern(int width, int precision)
            {
                if (width == 7)
                {
                    return "MMMyyyy";
                }
                else
                {
                    return "MMMyy";
                }
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) => base.getInternalFormatFunction(width, precision).Invoke(d.Value).ToUpper();
            }
        }

        public static readonly SasDateFormat MONYY = new MONYY_IMPL();
        /**
         * Writes date values as the year and the month abbreviation.
         * See: https://v8doc.sas.com/sashtml/lgref/z0205240.htm
         */

        private sealed class YYMON_IMPL : SasDateFormat
        {
            public YYMON_IMPL() : base(nameof(YYMON), 40, 7) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 5:
                    case 6:
                        return "yyMMM";
                    default:
                        return "yyyyMMM";
                }
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) => base.getInternalFormatFunction(width, precision).Invoke(d.Value).ToUpper();
            }
        }

        public static readonly SasDateFormat YYMON = new YYMON_IMPL();
        /**
         * Writes date values by using the ISO 8601 basic notation yyyymmdd.
         */

        private sealed class B8601DA_IMPL : SasDateFormat
        {
            public B8601DA_IMPL() : base(nameof(B8601DA), 41, 10) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "yyyyMMdd";
            }
        }

        public static readonly SasDateFormat B8601DA = new B8601DA_IMPL();
        /**
         * Writes date values by using the ISO 8601 extended notation yyyy-mm-dd.
         */

        private sealed class E8601DA_IMPL : SasDateFormat
        {
            public E8601DA_IMPL() : base(nameof(E8601DA), 42, 10) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "yyyy-MM-dd";
            }
        }

        public static readonly SasDateFormat E8601DA = new E8601DA_IMPL();
        /**
         * Writes date values as the name of the month.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201049.htm
         */

        private sealed class MONNAME_IMPL : SasDateFormat
        {
            public MONNAME_IMPL() : base(nameof(MONNAME), 43, 9) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "MMMM";
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) =>
                {
                    string s = base.getInternalFormatFunction(width, precision).Invoke(d.Value);
                    return s.Substring(0, Math.Min(s.Length, width));
                };
            }
        }

        public static readonly SasDateFormat MONNAME = new MONNAME_IMPL();
        /**
         * Writes date values as the day of the week and the date in the form day-of-week,
         * month-name dd, yy (or yyyy).
         * See: https://v8doc.sas.com/sashtml/lgref/z0201433.htm
         */

        private sealed class WEEKDATE_IMPL : SasDateFormat
        {
            public WEEKDATE_IMPL() : base(nameof(WEEKDATE), 44, 29) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        return "ddd";
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        return "dddd";
                    case 15:
                    case 16:
                        return "ddd, MMM d, yy";
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                        return "ddd, MMM d, yyyy";
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                        return "dddd, MMM d, yyyy";
                    default:
                        return "dddd, MMMM d, yyyy";
                }
            }
        }

        public static readonly SasDateFormat WEEKDATE = new WEEKDATE_IMPL();
        /**
         * Writes date values as day of week and date in the form day-of-week,
         * dd month-name yy (or yyyy).
         * See: https://v8doc.sas.com/sashtml/lgref/z0201303.htm
         */

        private sealed class WEEKDATX_IMPL : SasDateFormat
        {
            public WEEKDATX_IMPL() : base(nameof(WEEKDATX), 45, 29) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        return "ddd";
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        return "dddd";
                    case 15:
                    case 16:
                        return "ddd, d MMM yy";
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                        return "ddd, d MMM yyyy";
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                        return "dddd, d MMM yyyy";
                    default:
                        return "dddd, d MMMM yyyy";
                }
            }
        }

        public static readonly SasDateFormat WEEKDATX = new WEEKDATX_IMPL();
        /**
         * Writes date values as the day of the week.
         * See: https://v8doc.sas.com/sashtml/lgref/z0200757.htm
         * <p>
         * Note that SAS Universal Viewer and SAS System render
         * weekdays differently (at least for 1582 year).
         * See the difference:
         * <p>
         * Raw Value   SAS Viewer  SAS System
         * -138061      *           *
         * -138060      0           0
         * -138059      1           1
         * -138058      *           *
         * -138057      *           *
         * -138056      *           *
         * -138055      *           *
         * -138054      *           *
         * -138053      0           0
         * -138052      1           1
         * -138051      *           2
         * -138050      *           3
         * -138049      *           4
         * -138048      *           5
         * -138047      *           6
         * <p>
         * This SAS bug can't be recreated in Parso,
         * because it is not clear in which SAS system this bug is "more canonical".
         */

        private sealed class WEEKDAY_IMPL : SasDateFormat
        {
            public WEEKDAY_IMPL() : base(nameof(WEEKDAY), 46, 1) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "e";
            }
        }

        public static readonly SasDateFormat WEEKDAY = new WEEKDAY_IMPL();
        /**
         * Writes date values as the name of the day of the week.
         * See: https://v8doc.sas.com/sashtml/lgref/z0200842.htm
         */

        private sealed class DOWNAME_IMPL : SasDateFormat
        {
            public DOWNAME_IMPL() : base(nameof(DOWNAME), 47, 9) { }

            protected override string getDatePattern(int width, int precision)
            {
                return "dddd";
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                return (d) =>
                {
                    string s = base.getInternalFormatFunction(width, precision).Invoke(d.Value);
                    return s.Substring(0, Math.Min(s.Length, width));
                };
            }
        }

        public static readonly SasDateFormat DOWNAME = new DOWNAME_IMPL();
        /**
         * Writes date values as the name of the month,
         * the day, and the year in the form month-name dd, yyyy.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201451.htm
         */

        private sealed class WORDDATE_IMPL : SasDateFormat
        {
            public WORDDATE_IMPL() : base(nameof(WORDDATE), 48, 18) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        return "MMM";
                    case 9:
                    case 10:
                    case 11:
                        return "MMMM";
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        return "MMM d, yyyy";
                    default:
                        return "MMMM d, yyyy";
                }
            }
        }

        public static readonly SasDateFormat WORDDATE = new WORDDATE_IMPL();
        /**
         * Writes date values as the day, the name of the month,
         * and the year in the form dd month-name yyyy.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201147.htm
         */

        private sealed class WORDDATX_IMPL : SasDateFormat
        {
            public WORDDATX_IMPL() : base(nameof(WORDDATX), 49, 18) { }

            protected override string getDatePattern(int width, int precision)
            {
                switch (width)
                {
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        return "MMM";
                    case 9:
                    case 10:
                    case 11:
                        return "MMMM";
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        return "d MMM yyyy";
                    default:
                        return "d MMMM yyyy";
                }
            }
        }

        public static readonly SasDateFormat WORDDATX = new WORDDATX_IMPL();
        /**
         * Writes date values as the quarter of the year.
         * See: https://v8doc.sas.com/sashtml/lgref/z0201232.htm
         */

        private sealed class QTR_IMPL : SasDateFormat
        {
            public QTR_IMPL() : base(nameof(QTR), 50, 0) { }

            protected override string getDatePattern(int width, int precision)
            {
                return null;
            }

            public override Func<double?, string> getInternalFormatFunction(int width, int precision)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly SasDateFormat QTR = new QTR_IMPL();


        /**
         * Common method to format DDMMYY[B, C, D, N, P, S] dates.
         * See: https://v8doc.sas.com/sashtml/lgref/z0590669.htm
         *
         * @param width     date width
         * @param separator B: blank, C: colon, D: dash, P:period, S: slash
         *                  and N: for no separator
         * @return date pattern
         */
        private static string getDDMMYYxwFormatPattern(int width, string separator)
        {
            switch (width)
            {
                case 2:
                case 3:
                    return "dd";
                case 4:
                    return "ddMM";
                case 5:
                    return "dd" + separator + "MM";
                case 6:
                case 7:
                    return "ddMMyy";
                case 10:
                    return "dd" + separator + "MM" + separator + "yyyy";
                default:
                    if (string.IsNullOrEmpty(separator))
                    {
                        return "ddMMyyyy";
                    }
                    else
                    {
                        return "dd" + separator + "MM" + separator + "yy";
                    }
            }
        }

        /**
         * Common method to format MMDDYY[B, C, D, N, P, S] dates.
         * See: https://v8doc.sas.com/sashtml/lgref/z0590662.htm
         *
         * @param width     date width
         * @param separator B: blank, C: colon, D: dash, P:period, S: slash
         *                  and N: for no separator
         * @return date pattern
         */
        private static string getMMDDYYxwFormatPattern(int width, string separator)
        {
            switch (width)
            {
                case 2:
                case 3:
                    return "MM";
                case 4:
                    return "MMdd";
                case 5:
                    return "MM" + separator + "dd";
                case 6:
                case 7:
                    return "MMddyy";
                case 10:
                    return "MM" + separator + "dd" + separator + "yyyy";
                default:
                    if (string.IsNullOrEmpty(separator))
                    {
                        return "MMddyyyy";
                    }
                    else
                    {
                        return "MM" + separator + "dd" + separator + "yy";
                    }
            }
        }

        /**
         * Common method to format YYMMDD[B, C, D, N, P, S] dates.
         * See: https://v8doc.sas.com/sashtml/lgref/z0589916.htm
         *
         * @param width     date width
         * @param separator B: blank, C: colon, D: dash, P:period, S: slash
         *                  and N: for no separator
         * @return date pattern
         */
        private static string getYYMMDDxwFormatPattern(int width, string separator)
        {
            switch (width)
            {
                case 2:
                case 3:
                    return "yy";
                case 4:
                    return "yyMM";
                case 5:
                    return "yy" + separator + "MM";
                case 6:
                case 7:
                    return "yyMMdd";
                case 10:
                    return "yyyy" + separator + "MM" + separator + "dd";
                default:
                    if (string.IsNullOrEmpty(separator))
                    {
                        return "yyyyMMdd";
                    }
                    else
                    {
                        return "yy" + separator + "MM" + separator + "dd";
                    }
            }
        }

        /**
         * Common method to format MMYY[C, D, N, P, S] dates.
         * See: https://v8doc.sas.com/sashtml/lgref/z0199314.htm
         *
         * @param width     date width
         * @param separator 'M' by default, C: colon, D: dash, P:period, S: slash
         * @return date pattern
         */
        private static string getMMYYxwFormatPattern(int width, string separator)
        {
            switch (width)
            {
                case 5:
                case 6:
                    return "MM" + separator + "yy";
                default:
                    return "MM" + separator + "yyyy";
            }
        }

        /**
         * Common method to format YYMM[C, D, N, P, S] dates.
         * See: https://v8doc.sas.com/sashtml/lgref/z0199309.htm
         *
         * @param width     date width
         * @param separator 'M' by default, C: colon, D: dash, P:period, S: slash
         * @return date pattern
         */
        private static string getYYMMxwFormatPattern(int width, string separator)
        {
            switch (width)
            {
                case 5:
                case 6:
                    return "yy" + separator + "MM";
                default:
                    return "yyyy" + separator + "MM";
            }
        }


        /**
         * Default format width.
         * In terms of SAS: "w specifies the width of the output field".
         */
        private int defaultWidth;

        /**
         * Enum constructor.
         *
         * @param defaultWidth default width for format
         */

        private SasDateFormat(string name, int value, int defaultWidth) : base(name, value)
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
        protected abstract string getDatePattern(int width, int precision);

        public Func<double?, string> getFallbackFormatFunction(int width, int precision)
        {
            return DATE.getInternalFormatFunction(7, 0);
        }

        public virtual Func<double?, string> getInternalFormatFunction(int width, int precision)
        {
            string datePattern = getDatePattern(width, precision);
            DateTimeFormatter formatter = SasTemporalUtils.createDateTimeFormatterFromPattern(datePattern);
            return (sasDays) =>
            {
                DateTime date = SasTemporalUtils.sasDaysToLocalDate(sasDays.Value);
                return formatter.Format(date);
            };
        }
    }
}