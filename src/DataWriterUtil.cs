using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Deveel.Math;

/**
* *************************************************************************
* Copyright (C) 2015 EPAM
* <p>
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* <p>
* http://www.apache.org/licenses/LICENSE-2.0
* <p>
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
* <p>
* *************************************************************************
* The source code has been modified for the purpose of porting it to C#
* *************************************************************************
*/
namespace SasReader
{
    /**
     * A helper class to allow re-use formatted values from sas7bdat file.
     */
    public sealed class DataWriterUtil
    {

        /**
         * The number of digits starting from the first non-zero value, used to round doubles.
         */
        private static int ACCURACY = 15;

        /**
         * The class name of array of byte.
         */
        private static string BYTE_ARRAY_CLASS_NAME = typeof(byte[]).Name;

        /**
         * Encoding used to convert byte arrays to string.
         */
        private static string ENCODING_NAME = "CP1252";

        /**
         * If the number of digits in a double value exceeds a given constant, it rounds off.
         */
        private static int ROUNDING_LENGTH = 13;

        /**
         * The constant to check whether or not a string containing double stores infinity.
         */
        private static string DOUBLE_INFINITY_STRING = "Infinity";

        /**
         * The format to output hours in the CSV format.
         */
        private static string HOURS_OUTPUT_FORMAT = "{0:00}";

        /**
         * The format to output minutes in the CSV format.
         */
        private static string MINUTES_OUTPUT_FORMAT = "{0:00}";

        /**
         * The format to output seconds in the CSV format.
         */
        private static string SECONDS_OUTPUT_FORMAT = "{0:00}";

        /**
         * The delimiter between hours and minutes, minutes and seconds in the CSV file.
         */
        private static string TIME_DELIMETER = ":";

        /**
         * The format to store the percentage values. Appear in the data of
         * the {@link com.epam.parso.impl.SasFileParser.FormatAndLabelSubheader} subheader
         * and are stored in {@link Column#format}.
         */
        private static string PERCENT_FORMAT = "PERCENT";

        /**
         * The number of seconds in a minute.
         */
        private static int SECONDS_IN_MINUTE = 60;

        /**
         * The number of minutes in an hour.
         */
        private static int MINUTES_IN_HOUR = 60;

        /**
         * The locale for dates in output row.
         */
        private static CultureInfo DEFAULT_LOCALE = CultureInfo.CurrentCulture;

        /**
         * Error string if column format is unknown.
         */
        private static string UNKNOWN_DATE_FORMAT_EXCEPTION = "Unknown date format";

        private static Encoding encoding;

        /**
         * Empty private constructor for preventing instances.
         */
        static DataWriterUtil()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding(ENCODING_NAME);
        }

        /**
         * Checks current entry type and returns its string representation.
         *
         * @param column           current processing column.
         * @param entry            current processing entry.
         * @param locale           the locale for parsing date and percent elements.
         * @param columnFormatters the map that stores (@link Column#id) column identifier and the formatter
         *                         for converting locale-sensitive values stored in this column into string.
         * @return a string representation of current processing entry.
         * @throws IOException appears if the output into writer is impossible.
         */
        private static string processEntry(Column column, Object entry, CultureInfo locale,
                                           Dictionary<int, Format> columnFormatters)
        {
            if (!entry.ToString().Contains(DOUBLE_INFINITY_STRING))
            {
                string valueToPrint;
                if (entry.GetType() == typeof(DateTime))
                {
                    columnFormatters.TryGetValue(column.getId(), out Format fmt);
                    valueToPrint = convertDateElementToString((DateTime)entry, fmt ?? getDateFormatProcessor(column.getFormat(), locale));
                }
                else
                {
                    if (DateTimeConstants.TIME_FORMAT_STRINGS.Contains(column.getFormat().getName()))
                    {
                        valueToPrint = convertTimeElementToString((long)entry);
                    }
                    else if (PERCENT_FORMAT.Equals(column.getFormat().getName()))
                    {
                        columnFormatters.TryGetValue(column.getId(), out Format fmt);
                        valueToPrint = convertPercentElementToString(entry, (DecimalFormat) (fmt ?? getPercentFormatProcessor(column.getFormat(), locale)));
                    }
                    else
                    {
                        valueToPrint = entry.ToString();
                        if (entry.GetType() == typeof(double))
                        {
                            valueToPrint = convertDoubleElementToString((Double)entry);
                        }
                    }
                }

                return valueToPrint;
            }
            return "";
            //return entry is null ? "" : Convert.ToString(entry, locale);
        }

        /**
         * The function to convert a date into a string according to the format used.
         *
         * @param currentDate the date to convert.
         * @param dateFormat  the formatter to convert date element into string.
         * @return the string that corresponds to the date in the format used.
         */
        private static string convertDateElementToString(DateTime currentDate, Format dateFormat)
        {
            var unixMilliseconds = new DateTimeOffset(currentDate).ToUnixTimeMilliseconds();
            return unixMilliseconds != 0 ? dateFormat.format(currentDate) : "";
        }

        /**
         * The function to convert time without a date (hour, minute, second) from the sas7bdat file format
         * (which is the number of seconds elapsed from the midnight) into a string of the format set by the constants:
         * {@link DataWriterUtil#HOURS_OUTPUT_FORMAT}, {@link DataWriterUtil#MINUTES_OUTPUT_FORMAT},
         * {@link DataWriterUtil#SECONDS_OUTPUT_FORMAT}, and {@link DataWriterUtil#TIME_DELIMETER}.
         *
         * @param secondsFromMidnight the number of seconds elapsed from the midnight.
         * @return the string of time in the format set by constants.
         */
        private static string convertTimeElementToString(long secondsFromMidnight)
        {
            return string.Format(HOURS_OUTPUT_FORMAT, secondsFromMidnight / SECONDS_IN_MINUTE / MINUTES_IN_HOUR)
                    + TIME_DELIMETER
                    + string.Format(MINUTES_OUTPUT_FORMAT, secondsFromMidnight / SECONDS_IN_MINUTE % MINUTES_IN_HOUR)
                    + TIME_DELIMETER
                    + string.Format(SECONDS_OUTPUT_FORMAT, secondsFromMidnight % SECONDS_IN_MINUTE);
        }

        /**
         * The function to convert a double value into a string. If the text presentation of the double is longer
         * than {@link DataWriterUtil#ROUNDING_LENGTH}, the rounded off value of the double includes
         * the {@link DataWriterUtil#ACCURACY} number of digits from the first non-zero value.
         *
         * @param value the input numeric value to convert.
         * @return the string with the text presentation of the input numeric value.
         */
        private static string convertDoubleElementToString(Double value)
        {
            string valueToPrint = value.ToString();
            if (valueToPrint.Length > ROUNDING_LENGTH)
            {
                int lengthBeforeDot = (int)Math.Ceiling(Math.Log10(Math.Abs(value)));
                BigDecimal bigDecimal = new BigDecimal(value);
                bigDecimal = bigDecimal.Scale(ACCURACY - lengthBeforeDot, RoundingMode.HalfUp);
                valueToPrint = bigDecimal.ToDouble().ToString();
            }
            valueToPrint = trimZerosFromEnd(valueToPrint);
            return valueToPrint;
        }

        /**
         * The function to convert a percent element into a string.
         *
         * @param value         the input numeric value to convert.
         * @param decimalFormat the formatter to convert percentage element into string.
         * @return the string with the text presentation of the input numeric value.
         */
        private static string convertPercentElementToString(Object value, DecimalFormat decimalFormat)
        {
            Double doubleValue = value is long ? Convert.ToDouble((long)value) : (double)value;
            return decimalFormat.format(doubleValue);
        }

        /**
         * The function to remove trailing zeros from the decimal part of the numerals represented by a string.
         * If there are no digits after the point, the point is deleted as well.
         *
         * @param string the input string trailing zeros.
         * @return the string without trailing zeros.
         */
        private static string trimZerosFromEnd(String str)
        {
            return str.Contains(".") ? Regex.Replace(Regex.Replace(str, "0*$", ""), "\\.$", "") : str;
        }

        /**
         * The method to convert the Objects array that stores data from the sas7bdat file to list of string.
         *
         * @param columns          the {@link Column} class variables list that stores columns
         *                         description from the sas7bdat file.
         * @param row              the Objects arrays that stores data from the sas7bdat file.
         * @param locale           the locale for parsing date elements.
         * @param columnFormatters the map that stores (@link Column#id) column identifier and the formatter
         *                         for converting locale-sensitive values stored in this column into string.
         * @return list of string objects that represent data from sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public static List<String> getRowValues(List<Column> columns, Object[] row, CultureInfo locale, Dictionary<int, Format> columnFormatters)
        {
            var values = new List<String>();
            for (int currentColumnIndex = 0; currentColumnIndex < columns.Count; currentColumnIndex++)
            {
                values.Add(getValue(columns[currentColumnIndex], row[currentColumnIndex], locale, columnFormatters));
            }
            return values;
        }

        /**
         * The method to convert the Objects array that stores data from the sas7bdat file to list of string.
         *
         * @param columns          the {@link Column} class variables list that stores columns
         *                         description from the sas7bdat file.
         * @param row              the Objects arrays that stores data from the sas7bdat file.
         * @param columnFormatters the map that stores (@link Column#id) column identifier and the formatter
         *                         for converting locale-sensitive values stored in this column into string.
         * @return list of string objects that represent data from sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public static List<String> getRowValues(List<Column> columns, Object[] row,
                                                Dictionary<int, Format> columnFormatters)
        {
            return getRowValues(columns, row, DEFAULT_LOCALE, columnFormatters);
        }

        /**
         * The method to convert the Object that stores data from the sas7bdat file cell to string.
         *
         * @param column           the {@link Column} class variable that stores current processing column.
         * @param entry            the Object that stores data from the cell of sas7bdat file.
         * @param locale           the locale for parsing date elements.
         * @param columnFormatters the map that stores (@link Column#id) column identifier and the formatter
         *                         for converting locale-sensitive values stored in this column into string.
         * @return a string representation of current processing entry.
         * @throws IOException appears if the output into writer is impossible.
         */
        public static string getValue(Column column, Object entry, CultureInfo locale, Dictionary<int, Format> columnFormatters)
        {
            string value = "";
            if (entry != null)
            {
                if (entry.GetType().Name.CompareTo(BYTE_ARRAY_CLASS_NAME) == 0)
                {
                    value = encoding.GetString((byte[])entry);
                }
                else
                {
                    value = processEntry(column, entry, locale, columnFormatters);
                }
            }
            return value;
        }

        /**
         * The function to get a formatter to convert percentage elements into a string.
         *
         * @param columnFormat the (@link ColumnFormat) class variable that stores the precision of rounding
         *                     the converted value.
         * @param locale       locale for parsing date elements.
         * @return a formatter to convert percentage elements into a string.
         */
        private static Format getPercentFormatProcessor(ColumnFormat columnFormat, CultureInfo locale)
        {
            if (columnFormat.getPrecision() == 0)
            {
                return new DecimalFormat("P0");
            }

            return new DecimalFormat($"P{columnFormat.getPrecision()}");
        }

        /**
         * The function to get a formatter to convert date elements into a string.
         *
         * @param columnFormat the (@link ColumnFormat) class variable that stores the format name that must belong to
         *                     the set of {@link com.epam.parso.impl.DateTimeConstants#DATE_FORMAT_STRINGS} or
         *                     {@link com.epam.parso.impl.DateTimeConstants#DATETIME_FORMAT_STRINGS} mapping keys.
         * @param locale       locale for parsing date elements.
         * @return a formatter to convert date elements into a string.
         */
        private static DateTimeFormatter getDateFormatProcessor(ColumnFormat columnFormat, CultureInfo locale)
        {
            DateTimeConstants.DATE_FORMAT_STRINGS.TryGetValue(columnFormat.getName(), out var dateFormatString);
            DateTimeConstants.DATETIME_FORMAT_STRINGS.TryGetValue(columnFormat.getName(), out var dateTimeFormatString);

            string pattern = dateFormatString ?? dateTimeFormatString;

            if (pattern == null)
            {
                throw new Exception(UNKNOWN_DATE_FORMAT_EXCEPTION);
            }
            DateTimeFormatter dateFormat = DateTimeFormatter.OfPattern(pattern);
            dateFormat.Culture = locale;
            return dateFormat;
        }
    }
}