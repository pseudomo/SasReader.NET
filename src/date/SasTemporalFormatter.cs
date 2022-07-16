using System;
using System.Collections.Generic;

namespace SasReader
{

    /**
     * SAS supports wide family of date formats.
     * This class allows to represent SAS date in various types.
     * <p>
     * This class is not thread-safe and it should be synchronised externally.
     * Actually, it is not a problem for the the Parso itself as it is a single-threaded
     * library, where each instance of SasFileParser has it's own instance of the formatter.
     */
    public class SasTemporalFormatter
    {

        /**
         * Cached date format functions.
         */
        private Dictionary<string, Func<double?, string>> dateFormatFunctions = new Dictionary<string, Func<double?, string>>();

        /**
         * Cached time format functions.
         */
        private Dictionary<string, Func<double?, string>> timeFormatFunctions = new Dictionary<string, Func<double?, string>>();

        /**
         * Cached date-time format functions.
         */
        private Dictionary<string, Func<double?, string>> dateTimeFormatFunctions = new Dictionary<string, Func<double?, string>>();

        /**
         * Format SAS seconds explicitly into the java Date.
         *
         * @param sasSeconds seconds since 1960-01-01
         * @return date
         */
        public DateTime formatSasSecondsAsJavaDate(double sasSeconds)
        {
            sasSeconds = SasTemporalUtils.sasLeapSecondsFix(sasSeconds);
            return SasTemporalUtils.sasSecondsToDate(sasSeconds);
        }

        /**
         * Format SAS date in SAS days to one of the specified form.
         *
         * @param sasDays        days since 1960-01-01
         * @param dateFormatType type of output date
         * @param sasFormatName  date column format name
         * @param width          date column format width
         * @param precision      date column format precision
         * @return date representation
         */
        public Object formatSasDate(double? sasDays, OutputDateType dateFormatType,
                                    String sasFormatName, int width, int precision)
        {

            if (dateFormatType == OutputDateType.SAS_VALUE)
            {
                return sasDays;
            }
            else if (sasDays == null || Double.IsNaN(sasDays.Value))
            {
                if (dateFormatType == OutputDateType.SAS_FORMAT_EXPERIMENTAL || dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL)
                {
                    return ".";
                }
                else
                {
                    return null;
                }
            }

            sasDays = SasTemporalUtils.sasLeapDaysFix(sasDays.Value);

            switch (dateFormatType)
            {
                case OutputDateType.EPOCH_SECONDS:
                    return SasTemporalUtils.sasDaysToEpochSeconds(sasDays.Value);
                case OutputDateType.JAVA_TEMPORAL:
                    return SasTemporalUtils.sasDaysToLocalDate(sasDays.Value);
                case OutputDateType.SAS_FORMAT_EXPERIMENTAL:
                case OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL:
                    bool trim = dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL;
                    var f = dateFormatFunctions.GetValueOrDefault(sasFormatName + width + "." + precision);
                    if (f == null)
                    {
                        f = (SasDateFormat.FromName(sasFormatName) as SasTemporalFormat).getFormatFunction(width, precision, trim);
                    }
                    return f.Invoke(sasDays.Value);
                case OutputDateType.JAVA_DATE_LEGACY:
                default:
                    return SasTemporalUtils.sasDaysToDate(sasDays.Value);
            }
        }

        /**
         * Format SAS time in SAS seconds to one of the specified form.
         * For the compatibility with Parso this formatter returns number
         * (long or double) instead of date case of JAVA_DATE output type.
         *
         * @param sasSeconds     days since 1960-01-01
         * @param dateFormatType type of output date
         * @param sasFormatName  date column format name
         * @param width          date column format width
         * @param precision      date column format precision
         * @return date representation
         */
        public Object formatSasTime(double? sasSeconds, OutputDateType dateFormatType,
                                    String sasFormatName, int width, int precision)
        {
            if (dateFormatType == OutputDateType.SAS_VALUE)
            {
                return sasSeconds;
            }
            else if (sasSeconds == null || Double.IsNaN(sasSeconds.Value))
            {
                if (dateFormatType == OutputDateType.SAS_FORMAT_EXPERIMENTAL || dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL)
                {
                    return ".";
                }
                else
                {
                    return null;
                }
            }

            switch (dateFormatType)
            {
                case OutputDateType.SAS_FORMAT_EXPERIMENTAL:
                case OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL:
                    bool trim = dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL;
                    return timeFormatFunctions.GetValueOrDefault(
                        sasFormatName + width + "." + precision,
                        ((SasTemporalFormat)SasTimeFormat.FromName(sasFormatName)).getFormatFunction(width, precision, trim)
                    ).Invoke(sasSeconds);
                case OutputDateType.JAVA_DATE_LEGACY:
                case OutputDateType.JAVA_TEMPORAL:
                default:
                    // These lines below for compatibility with existing Parso result.
                    // Number of seconds in Parso is represented in some cases as long
                    // or as double using the SasFileParser.convertByteArrayToNumber function.
                    long longSeconds = (long)Math.Round(sasSeconds.Value);
                    if (Math.Abs(sasSeconds.Value - longSeconds) > 0)
                    {
                        return sasSeconds;
                    }
                    else
                    {
                        return longSeconds;
                    }
            }
        }

        /**
         * Format SAS date-time in SAS seconds to one of the specified form.
         *
         * @param sasSeconds     seconds since midnight
         * @param dateFormatType type of output date
         * @param sasFormatName  date column format name
         * @param width          date column format width
         * @param precision      date column format precision
         * @return date representation
         */
        public Object formatSasDateTime(double? sasSeconds, OutputDateType dateFormatType,
                                        String sasFormatName, int width, int precision)
        {
            if (dateFormatType == OutputDateType.SAS_VALUE)
            {
                return sasSeconds;
            }
            else if (sasSeconds == null || Double.IsNaN(sasSeconds.Value))
            {
                if (dateFormatType == OutputDateType.SAS_FORMAT_EXPERIMENTAL || dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL)
                {
                    return ".";
                }
                else
                {
                    return null;
                }
            }

            sasSeconds = SasTemporalUtils.sasLeapSecondsFix(sasSeconds.Value);

            switch (dateFormatType)
            {
                case OutputDateType.EPOCH_SECONDS:
                    return SasTemporalUtils.sasSecondsToEpochSeconds(sasSeconds.Value);
                case OutputDateType.JAVA_TEMPORAL:
                    return SasTemporalUtils.sasSecondsToLocalDateTime(sasSeconds.Value, 9);
                case OutputDateType.SAS_FORMAT_EXPERIMENTAL:
                case OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL:
                    bool trim = dateFormatType == OutputDateType.SAS_FORMAT_TRIM_EXPERIMENTAL;
                    return dateTimeFormatFunctions.GetValueOrDefault(
                        sasFormatName + width + "." + precision,
                        ((SasTemporalFormat)SasDateTimeFormat.FromName(sasFormatName)).getFormatFunction(width, precision, trim)
                    ).Invoke(sasSeconds);
                case OutputDateType.JAVA_DATE_LEGACY:
                default:
                    return SasTemporalUtils.sasSecondsToDate(sasSeconds.Value);
            }
        }

        /**
         * Check if the specified SAS format is type of date.
         *
         * @param sasFormatName SAS format name
         * @return true if matched
         */
        public static bool isDateFormat(string sasFormatName)
        {
            return SasDateFormat.TryFromName(sasFormatName, out _);
            //for (SasDateFormat s : SasDateFormat.values()) {
            //    if (s.name().equals(sasFormatName)) {
            //        return true;
            //    }
            //}
            //return false;
        }

        /**
         * Check if the specified SAS format is type of time.
         *
         * @param sasFormatName SAS format name
         * @return true if matched
         */
        public static bool isTimeFormat(String sasFormatName)
        {
            return SasTimeFormat.TryFromName(sasFormatName, out _);
            //for (SasTimeFormat s : SasTimeFormat.values()) {
            //    if (s.name().equals(sasFormatName)) {
            //        return true;
            //    }
            //}
            //return false;
        }

        /**
         * Check if the specified SAS format is type of date-time.
         *
         * @param sasFormatName SAS format name
         * @return true if matched
         */
        public static bool isDateTimeFormat(String sasFormatName)
        {
            return SasDateTimeFormat.TryFromName(sasFormatName, out _);
            //for (SasDateTimeFormat s : SasDateTimeFormat.values()) {
            //    if (s.name().equals(sasFormatName)) {
            //        return true;
            //    }
            //}
            //return false;
        }
    }
}