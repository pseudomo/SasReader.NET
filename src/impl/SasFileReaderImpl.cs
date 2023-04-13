using System;
using System.Collections.Generic;
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
     * A class to read sas7bdat files transferred to the input stream and then to get metadata and file data.
     * This class is used as a wrapper for SasFileParser, it provides methods to read sas7bdat file and its properties.
     * Despite this, {@link SasFileParser} is publicly available, it can be instanced via {@link SasFileParser.Builder}
     * and used directly. Public access to the {@link SasFileParser} class was added in scope of this issue:
     * @see <a href="https://github.com/epam/parso/issues/51"></a>.
     */
    public class SasFileReaderImpl : SasFileReader
    {
        /**
         * Object for writing logs.
         */

        /**
         * Object for parsing sas7bdat file.
         */
        private SasFileParser sasFileParser;

        /**
         * List of columns obtained by names.
         */
        private List<Column> columnsByName = new List<Column>();

        /**
         * Builds an object of the SasFileReaderImpl class from the file contained in the input stream.
         * Reads only metadata (properties and column information) of the sas7bdat file.
         *
         * @param inputStream - an input stream which should contain a correct sas7bdat file.
         */
        public SasFileReaderImpl(Stream inputStream)
        {
            sasFileParser = new SasFileParser.Builder(inputStream).build();
        }

        /**
         * Builds an object of the SasFileReaderImpl class from the file contained in the input stream with the encoding
         * defined in the 'encoding' variable.
         * Reads only metadata (properties and column information) of the sas7bdat file.
         *
         * @param inputStream - an input stream which should contain a correct sas7bdat file.
         * @param encoding    - the string containing the encoding to use in strings output
         */
        public SasFileReaderImpl(Stream inputStream, String encoding)
        {
            sasFileParser = new SasFileParser.Builder(inputStream).Encoding(encoding).build();
        }

        /**
         * Builds an instance of SasFileReaderImpl from the file contained in the input stream
         * with the specified encoding and type of output date formatting.
         * Reads only metadata (properties and column information) of the sas7bdat file.
         *
         * @param inputStream    an input stream which should contain a correct sas7bdat file.
         * @param encoding       the string containing the encoding to use in strings output
         * @param outputDateType type of formatting for date columns
         */
        public SasFileReaderImpl(Stream inputStream, String encoding,
                                 OutputDateType outputDateType)
        {
            sasFileParser = new SasFileParser.Builder(inputStream).Encoding(encoding).WithOutputDateType(outputDateType).build();
        }

        /**
         * Builds an object of the SasFileReaderImpl class from the file contained in the input stream with a flag of
         * the binary or string format of the data output.
         * Reads only metadata (properties and column information) of the sas7bdat file.
         *
         * @param inputStream - an input stream which should contain a correct sas7bdat file.
         * @param byteOutput  - the flag of data output in binary or string format
         */
        public SasFileReaderImpl(Stream inputStream, Boolean byteOutput)
        {
            sasFileParser = new SasFileParser.Builder(inputStream).ByteOutput(byteOutput).build();
        }

        /**
         * The function to get the {@link Column} list from {@link SasFileParser}.
         *
         * @return a list of columns.
         */

        public List<Column> getColumns()
        {
            return sasFileParser.getColumns();
        }

        /**
         * The function to get the {@link Column} list from {@link SasFileReader}
         * according to the columnNames.
         *
         * @param columnNames - list of column names that should be returned.
         * @return a list of columns.
         */

        public List<Column> getColumns(List<String> columnNames)
        {
            if (columnsByName.Count == 0)
            {
                Dictionary<String, Column> columnsMap = new Dictionary<String, Column>();
                List<Column> allColumns = sasFileParser.getColumns();
                foreach (Column column in allColumns)
                {
                    columnsMap[column.getName()] = column;
                }
                foreach (String name in columnNames)
                {
                    if (columnsMap.ContainsKey(name))
                    {
                        columnsByName.Add(columnsMap[name]);
                    }
                    else
                    {
                        throw new Exception(ParserMessageConstants.UNKNOWN_COLUMN_NAME);
                    }
                }
            }
            return columnsByName;
        }

        /**
         * Reads all rows from the sas7bdat file. For each row, only the columns defined in the list are read.
         *
         * @param columnNames list of column names which should be processed.
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */

        public Object[][] readAll(List<String> columnNames)
        {
            int rowNum = (int)getSasFileProperties().getRowCount();
            Object[][] result = new Object[rowNum][];
            for (int i = 0; i < rowNum; i++)
            {
                try
                {
                    result[i] = readNext(columnNames);
                }
                catch
                {
                    break;
                }
            }
            //Object[][] output = new Object[result.size()][];
            //for (int i = 0; i < result.size(); i++) {
            //    output[i] = (Object[]) result.get(i);
            //}
            //return output;
            return result;
        }

        /**
         * Reads all rows from the sas7bdat file.
         *
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */

        public Object[][] readAll()
        {
            return readAll(null);
        }

        /**
         * Reads all rows from the sas7bdat file.
         *
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */

        public Object[] readNext()
        {
            return sasFileParser.readNext(null);
        }

        /**
         * Reads all rows from the sas7bdat file. For each row, only the columns defined in the list are read.
         *
         * @param columnNames list of column names which should be processed.
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */
        public Object[] readNext(List<String> columnNames)
        {
            return sasFileParser.readNext(columnNames);
        }

        /**
         * The function to return the index of the current row when reading the file sas7bdat file.
         *
         * @return current row index
         */
        public int getOffset()
        {
            return sasFileParser.getOffset();
        }

        /**
         * The function to get sas file properties.
         *
         * @return the object of the {@link SasFileProperties} class that stores file metadata.
         */

        public SasFileProperties getSasFileProperties()
        {
            return sasFileParser.getSasFileProperties();
        }

        /**
         * The function to return all rows as IEnumerable<object[]>.
         *
         * @return all rows as IEnumerable<object[]>
         */
        public IEnumerable<object[]> getRowsAsEnumerable()
        {
            for (var i = 0; i < sasFileParser.getSasFileProperties().getRowCount(); i++)
            {
                yield return readNext();
            }
        }
    }
}