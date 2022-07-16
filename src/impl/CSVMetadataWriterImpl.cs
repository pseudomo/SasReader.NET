
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
     * This is a class to export the sas7bdat file metadata into the CSV format.
     */
    public class CSVMetadataWriterImpl : AbstractCSVWriter, CSVMetadataWriter
    {
        /**
         * The id column header for metadata.
         */
        private const String COLUMN_HEADING_ID = "Number";

        /**
         * The name column header for metadata.
         */
        private const String COLUMN_HEADING_NAME = "Name";

        /**
         * The type column header for metadata.
         */
        private const String COLUMN_HEADING_TYPE = "Type";

        /**
         * The data length column header for metadata.
         */
        private const String COLUMN_HEADING_DATA_LENGTH = "Data length";

        /**
         * The format column header for metadata.
         */
        private const String COLUMN_HEADING_FORMAT = "Format";

        /**
         * The label column header for metadata.
         */
        private const String COLUMN_HEADING_LABEL = "Label";

        /**
         * Constant containing Double type name.
         */
        private const String DOTNET_DOUBLE_TYPE_NAME = "Double";

        /**
         * Constant containing String type name.
         */
        private const String DOTNET_STRING_TYPE_NAME = "String";

        /**
         * Representation of Number type in metadata.
         */
        private const String OUTPUT_NUMBER_TYPE_NAME = "Numeric";

        /**
         * Representation of String type in metadata.
         */
        private const String OUTPUT_STRING_TYPE_NAME = "Character";

        /**
         * The constructor that defines writer variable to output result csv file.
         *
         * @param writer the writer which is used to output csv file.
         */
        public CSVMetadataWriterImpl(TextWriter writer) : base(writer)
        {
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         */
        public CSVMetadataWriterImpl(TextWriter writer, String delimiter) : base(writer, delimiter)
        {
        }

        /**
         * The constructor that defines writer variable to output result csv file with selected delimiter and endline.
         *
         * @param writer    the writer which is used to output csv file.
         * @param delimiter separator used in csv file.
         * @param endline   symbols used in csv file as endline.
         */
        public CSVMetadataWriterImpl(TextWriter writer, String delimiter, String endline) : base(writer, delimiter, endline)
        {
        }

        /**
         * The method to export a parsed sas7bdat file metadata (stored as an object of the {@link SasFileReaderImpl} class)
         * using {@link CSVMetadataWriterImpl#writer}.
         *
         * @param columns the {@link Column} class variables list that stores columns description from the sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        public void writeMetadata(List<Column> columns)
        {
            TextWriter writer = getWriter();
            String delimiter = getDelimiter();
            String endline = getEndline();

            writer.Write(COLUMN_HEADING_ID);
            writer.Write(delimiter);
            writer.Write(COLUMN_HEADING_NAME);
            writer.Write(delimiter);
            writer.Write(COLUMN_HEADING_TYPE);
            writer.Write(delimiter);
            writer.Write(COLUMN_HEADING_DATA_LENGTH);
            writer.Write(delimiter);
            writer.Write(COLUMN_HEADING_FORMAT);
            writer.Write(delimiter);
            writer.Write(COLUMN_HEADING_LABEL);
            writer.Write(endline);
            foreach (Column column in columns)
            {
                writer.Write(column.getId().ToString());
                writer.Write(delimiter);
                checkSurroundByQuotesAndWrite(writer, delimiter, column.getName());
                writer.Write(delimiter);
                writer.Write(column.getType().Name.Replace(DOTNET_DOUBLE_TYPE_NAME, OUTPUT_NUMBER_TYPE_NAME).Replace(
                        DOTNET_STRING_TYPE_NAME, OUTPUT_STRING_TYPE_NAME));
                writer.Write(delimiter);
                writer.Write(column.getLength().ToString());
                writer.Write(delimiter);
                if (!column.getFormat().isEmpty())
                {
                    checkSurroundByQuotesAndWrite(writer, delimiter, column.getFormat().ToString());
                }
                writer.Write(delimiter);
                checkSurroundByQuotesAndWrite(writer, delimiter, column.getLabel());
                writer.Write(endline);
            }
            writer.Flush();
        }

        /**
         * The method to output the sas7bdat file properties.
         *
         * @param sasFileProperties the variable with sas file properties data.
         * @throws IOException appears if the output into writer is impossible.
         */
        public void writeSasFileProperties(SasFileProperties sasFileProperties)
        {
            DateTimeFormatter dtf = new DateTimeFormatter("ddd MMM dd HH:mm:ss # yyyy")
            {
                //TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"),
                TimeZone = TimeZoneInfo.Utc,
                Culture = CultureInfo.GetCultureInfo("en-US")
            };

            constructPropertiesString("Bitness: ", sasFileProperties.isU64() ? "x64" : "x86");
            constructPropertiesString("Compressed: ", sasFileProperties.getCompressionMethod());
            constructPropertiesString("Endianness: ", sasFileProperties.getEndianness() == 1 ? "LITTLE_ENDIANNESS" : "BIG_ENDIANNESS");
            constructPropertiesString("Encoding: ", sasFileProperties.getEncoding());
            constructPropertiesString("Name: ", sasFileProperties.getName());
            constructPropertiesString("File type: ", sasFileProperties.getFileType());
            constructPropertiesString("File label: ", sasFileProperties.getFileLabel());
            constructPropertiesString("Date created: ", dtf.Format(sasFileProperties.getDateCreated()));
            constructPropertiesString("Date modified: ", dtf.Format(sasFileProperties.getDateModified()));
            constructPropertiesString("SAS release: ", sasFileProperties.getSasRelease());
            constructPropertiesString("SAS server type: ", sasFileProperties.getServerType());
            constructPropertiesString("OS name: ", sasFileProperties.getOsName());
            constructPropertiesString("OS type: ", sasFileProperties.getOsType());
            constructPropertiesString("Header Length: ", sasFileProperties.getHeaderLength());
            constructPropertiesString("Page Length: ", sasFileProperties.getPageLength());
            constructPropertiesString("Page Count: ", sasFileProperties.getPageCount());
            constructPropertiesString("Row Length: ", sasFileProperties.getRowLength());
            constructPropertiesString("Row Count: ", sasFileProperties.getRowCount());
            constructPropertiesString("Mix Page Row Count: ", sasFileProperties.getMixPageRowCount());
            constructPropertiesString("Columns Count: ", sasFileProperties.getColumnsCount());
            getWriter().Flush();
        }

        /**
         * The method to output string containing information about passed property using writer.
         *
         * @param propertyName the string containing name of a property.
         * @param property     a property value.
         * @throws IOException appears if the output into writer is impossible.
         */
        private void constructPropertiesString(String propertyName, Object property)
        {
            getWriter().Write($"{propertyName}{property ?? "null"}\n");
        }
    }
}