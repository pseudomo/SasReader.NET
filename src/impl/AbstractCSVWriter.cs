

using System;
using System.Globalization;
using System.IO;
/**
* *************************************************************************
* Copyright (C) 2015 EPAM

* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* *************************************************************************
* The source code has been modified for the purpose of porting it to C#
* *************************************************************************

*/
namespace SasReader
{
    /**
     * This is a class to store functions which are used in classes {@link CSVMetadataWriterImpl} and
     * {@link CSVDataWriterImpl}.
     */
    public abstract class AbstractCSVWriter
    {
        /**
         * The delimiter to use in the CSV format.
         */
        private static String DEFAULT_DELIMITER = ",";

        /**
         * The default endline for csv file.
         */
        private static String DEFAULT_ENDLINE = "\n";

        /**
         * The variable to output csv file.
         */
        private TextWriter writer;

        /**
         * The delimiter for csv file.
         */
        private String delimiter = DEFAULT_DELIMITER;

        /**
         * The endline for csv file.
         */
        private String endline = DEFAULT_ENDLINE;

        /**
         * The locale for dates and percentage elements in csv file.
         */
        private CultureInfo locale = CultureInfo.CurrentCulture;

        /**
         * The constructor that defines writer variable to output result csv file.
         *
         * @param writer the writer which is used to output csv file.
         */
        public AbstractCSVWriter(TextWriter writer)
        {
            this.writer = writer;
        }


        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         */
        public AbstractCSVWriter(TextWriter writer, String delimiter)
        {
            this.writer = writer;
            this.delimiter = delimiter;
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter and endline.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         * @param endline   symbols used in csv file as endline.
         */
        public AbstractCSVWriter(TextWriter writer, String delimiter, String endline)
        {
            this.writer = writer;
            this.delimiter = delimiter;
            this.endline = endline;
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter,
         * endline and locale.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         * @param endline   symbols used in csv file as endline.
         * @param locale   locale used in csv file for dates.
         */
        public AbstractCSVWriter(TextWriter writer, String delimiter, String endline, CultureInfo locale)
        {
            this.writer = writer;
            this.delimiter = delimiter;
            this.endline = endline;
            this.locale = locale;
        }

        /**
         * The method to write a text represented by an array of bytes using writer.
         * If the text contains the delimiter, line breaks, tabulation characters, and double quotes, the text is stropped.
         *
         * @param writer      the variable to output data.
         * @param delimiter   if trimmedText contains this delimiter it will be stropped.
         * @param trimmedText the array of bytes that contains the text to output.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public static void checkSurroundByQuotesAndWrite(TextWriter writer, String delimiter, String trimmedText)
        {
            writer.Write(checkSurroundByQuotes(delimiter, trimmedText));
        }

        /**
         * The method to output a text represented by an array of bytes.
         * If the text contains the delimiter, line breaks, tabulation characters, and double quotes, the text is stropped.
         *
         * @param delimiter   if trimmedText contains this delimiter it will be stropped.
         * @param trimmedText the array of bytes that contains the text to output.
         * @return string represented by an array of bytes.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public static String checkSurroundByQuotes(String delimiter, String trimmedText)
        {
            bool containsDelimiter = stringContainsItemFromList(trimmedText, delimiter, "\n", "\t", "\r", "\"");
            String trimmedTextWithoutQuotesDuplicates = trimmedText.Replace("\"", "\"\"");
            if (containsDelimiter && trimmedTextWithoutQuotesDuplicates.Length != 0)
            {
                return "\"" + trimmedTextWithoutQuotesDuplicates + "\"";
            }
            return trimmedTextWithoutQuotesDuplicates;
        }

        /**
         * The method to check if string contains as a substring at least one string from list.
         * @param inputString string which is checked for containing string from the list.
         * @param items list of strings.
         * @return true if at least one of strings from the list is a substring of original string.
         */
        private static bool stringContainsItemFromList(String inputString, params string[] items)
        {
            foreach (String item in items)
            {
                if (inputString.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Writer getter.
         * @return the variable to output csv file.
         */
        public TextWriter getWriter()
        {
            return writer;
        }

        /**
         * Delimiter getter.
         * @return the delimiter for csv file.
         */
        public String getDelimiter()
        {
            return delimiter;
        }

        /**
         * Endline getter.
         * @return the endline for csv file.
         */
        public String getEndline()
        {
            return endline;
        }

        /**
         * Locale getter.
         * @return the locale for dates and percentage elements in csv file.
         */
        public CultureInfo getLocale()
        {
            return locale;
        }
    }
}