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
     * A class to store column format metadata.
     */
    public class ColumnFormat
    {
        /**
         * The column format name.
         */
        private string name;
        /**
         * The column format width.
         */
        private int width;
        /**
         * The column format precision.
         */
        private int precision;

        /**
         * The constructor that defines all parameters of the ColumnFormat class.
         *
         * @param name      - column format name.
         * @param width     - olumn format width.
         * @param precision - column format precision.
         */
        public ColumnFormat(string name, int width, int precision)
        {
            this.name = name;
            this.width = width;
            this.precision = precision;
        }

        /**
         * The constructor that defines name of the ColumnFormat class.
         *
         * @param name - column format name.
         */
        public ColumnFormat(string name)
        {
            this.name = name;
        }

        /**
         * The function to get {@link ColumnFormat#name}.
         *
         * @return the string that contains the column format name.
         */
        public string getName()
        {
            return name;
        }

        /**
         * The function to get {@link ColumnFormat#width}.
         *
         * @return the number that contains the column format width.
         */
        public int getWidth()
        {
            return width;
        }

        /**
         * The function to get {@link ColumnFormat#precision}.
         *
         * @return the number that contains the column format precision.
         */
        public int getPrecision()
        {
            return precision;
        }

        /**
         * Returns true if there are no information about column format, otherwise false.
         *
         * @return true if column name is empty and width and precision are 0, otherwise false.
         */
        public bool isEmpty()
        {
            return string.IsNullOrEmpty(name) && width == 0 && precision == 0;
        }

        /**
         * The function to ColumnFormat class string representation.
         *
         * @return string representation of the column format.
         */
        public override string ToString()
        {
            return isEmpty() ? "" : name + (width != 0 ? width.ToString() : "") + "." + (precision != 0 ? precision.ToString() : "");
        }
    }
}