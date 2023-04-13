
using System.Collections.Generic;

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
     * Main interface for working with library.
     */
    public interface SasFileReader
    {
        /**
         * The function to get the {@link Column} list from {@link SasFileReader}.
         *
         * @return a list of columns.
         */
        List<Column> getColumns();

        /**
         * The function to get the {@link Column} list from {@link SasFileReader}
         * according to the columnNames.
         *
         * @param columnNames - list of column names which should be returned.
         * @return a list of columns.
         */
        List<Column> getColumns(List<string> columnNames);

        /**
         * Reads all rows from the sas7bdat file.
         *
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */
        object[][] readAll();

        /**
         * Reads all rows from the sas7bdat file. For each row, only the columns defined in the list are read.
         *
         * @param columnNames list of column names which should be processed.
         * @return an array of array objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         */
        object[][] readAll(List<string> columnNames);

        /**
         * Reads rows one by one from the sas7bdat file.
         *
         * @return an array of objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         * @throws IOException if reading input stream is impossible.
         */
        object[] readNext(); // throws IOException;

        /**
         * Reads rows one by one from the sas7bdat file. For each row, only the columns defined in the list are read.
         *
         * @param columnNames list of column names which should be processed.
         * @return an array of objects whose elements can be objects of the following classes: double, long,
         * int, byte[], Date depending on the column they are in.
         * @throws IOException if reading input stream is impossible.
         */
        object[] readNext(List<string> columnNames); // throws IOException;

        /**
         * The function to get sas file properties.
         *
         * @return the object of the {@link SasFileProperties} class that stores file metadata.
         */
        SasFileProperties getSasFileProperties();

        /**
         * The function to return the index of the current row when reading the sas7bdat file.
         *
         * @return current row index
         */
        int getOffset();

        /**
         * The function to return all rows as IEnumerable<object[]>.
         *
         * @return all rows as IEnumerable<object[]>
         */
        public IEnumerable<object[]> getRowsAsEnumerable();
    }
}