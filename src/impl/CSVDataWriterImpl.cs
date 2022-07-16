using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
     * This is a class to export the sas7bdat file data into the CSV format.
     */
    public class CSVDataWriterImpl : AbstractCSVWriter, CSVDataWriter
    {

        /**
         * The map to store (@link Column#id) column identifier and the formatter
         * for converting locale-sensitive values stored in this column into string.
         */
        private Dictionary<int, Format> columnFormatters = new Dictionary<int, Format>();

        /**
         * The constructor that defines writer variable to output result csv file.
         *
         * @param writer the writer which is used to output csv file.
         */
        public CSVDataWriterImpl(TextWriter writer) : base(writer)
        {
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         */
        public CSVDataWriterImpl(TextWriter writer, String delimiter) : base(writer, delimiter)
        {
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter and endline.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         * @param endline   symbols used in csv file as endline.
         */
        public CSVDataWriterImpl(TextWriter writer, String delimiter, String endline) : base(writer, delimiter, endline)
        {
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter,
         * endline and locale.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         * @param endline   symbols used in csv file as endline.
         * @param locale    locale used for dates in csv file.
         */
        public CSVDataWriterImpl(TextWriter writer, String delimiter, String endline, CultureInfo locale) : base(writer, delimiter, endline, locale)
        {
        }

        /**
         * The method to export a row from sas7bdat file (stored as an object of the {@link SasFileReaderImpl} class)
         * using {@link CSVDataWriterImpl#writer}.
         *
         * @param columns the {@link Column} class variables list that stores columns description from the sas7bdat file.
         * @param row     the Objects arrays that stores data from the sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public void writeRow(List<Column> columns, Object[] row)
        {
            if (row == null)
            {
                return;
            }
            TextWriter writer = getWriter();
            List<String> valuesToPrint = DataWriterUtil.getRowValues(columns, row, getLocale(), columnFormatters);
            for (int currentColumnIndex = 0; currentColumnIndex < columns.Count; currentColumnIndex++)
            {
                writer.Write(checkSurroundByQuotes(getDelimiter(), valuesToPrint[currentColumnIndex]));
                if (currentColumnIndex != columns.Count - 1)
                {
                    writer.Write(getDelimiter());
                }
            }
            writer.Write(getEndline());
            writer.Flush();
        }

        /**
         * The method to export a parsed sas7bdat file (stored as an object of the {@link SasFileReaderImpl} class)
         * using {@link CSVDataWriterImpl#writer}.
         *
         * @param columns the {@link Column} class variables list that stores columns description from the sas7bdat file.
         * @param rows    the Objects arrays array that stores data from the sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public void writeRowsArray(List<Column> columns, Object[][] rows)
        {
            foreach (Object[] currentRow in rows)
            {
                if (currentRow != null)
                {
                    writeRow(columns, currentRow);
                }
                else
                {
                    break;
                }
            }
        }

        /**
         * The method to output the column names using the {@link CSVDataWriterImpl#delimiter} delimiter
         * using {@link CSVDataWriterImpl#writer}.
         *
         * @param columns the list of column names.
         * @throws IOException appears if the output into writer is impossible.
         */

        public void writeColumnNames(List<Column> columns)
        {
            TextWriter writer = getWriter();
            for (int i = 0; i < columns.Count; i++)
            {
                checkSurroundByQuotesAndWrite(writer, getDelimiter(), columns[i].getName());
                if (i != columns.Count - 1)
                {
                    writer.Write(getDelimiter());
                }
            }
            writer.Write(getEndline());
            writer.Flush();
        }

    }
}