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
     * This is a class to store debugging info, info about errors and warnings which can be received
     * when parsing the sas7bdat file.
     */
    public static class ParserMessageConstants
    {
        /**
         * Error string if there are no available bytes in the input stream.
         */
        public const string EMPTY_INPUT_STREAM = "There are no available bytes in the input stream.";
        /**
         * Error string if the sas7bdat file is invalid.
         */
        public const string FILE_NOT_VALID = "Can not read metadata from sas7bdat file.";
        /**
         * Debug info in case of an unknown subheader signature.
         */
        public const string UNKNOWN_SUBHEADER_SIGNATURE = "Unknown subheader signature";
        /**
         *  Warn info if 'null' is provided as the file compression literal.
         */
        public const string NULL_COMPRESSION_LITERAL = "Null provided as the file compression literal, assuming no compression";
        /**
         * Debug info if no supported compression literal is found.
         */
        public const string NO_SUPPORTED_COMPRESSION_LITERAL = "No supported compression literal found, assuming no compression";
        /**
         * Error string if list of columns does not contain specified column name.
         */
        public const string UNKNOWN_COLUMN_NAME = "Unknown column name";
        /**
         * Debug info. Subheader count.
         */
        public const string SUBHEADER_COUNT = "Subheader count: {}";
        /**
         * Debug info. Block count.
         */
        public const string BLOCK_COUNT = "Block count: {}";
        /**
         * Debug info. Page type.
         */
        public const string PAGE_TYPE = "Page type: {}";
        /**
         * Debug info. Subheader process function name.
         */
        public const string SUBHEADER_PROCESS_FUNCTION_NAME = "Subheader process function name: {}";
        /**
         * Debug info. Column format.
         */
        public const string COLUMN_FORMAT = "Column format: {}";
    }
}