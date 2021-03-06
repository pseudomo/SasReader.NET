
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
     * Interface for exporting metadata from sas7bdat file to csv.
     */
    public interface CSVMetadataWriter
    {
        /**
         * The method to export a parsed sas7bdat file metadata using writer.
         *
         * @param columns the {@link Column} class variables list that stores columns description from the sas7bdat file.
         * @throws java.io.IOException appears if the output into writer is impossible.
         */
        void writeMetadata(List<Column> columns);

        /**
         * The method to output the sas7bdat file properties.
         *
         * @param sasFileProperties the variable with sas file properties data.
         * @throws IOException appears if the output into writer is impossible.
         */
        void writeSasFileProperties(SasFileProperties sasFileProperties);
    }
}