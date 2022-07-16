
using System.Collections.Generic;

/**
* *************************************************************************
* Copyright (C) 2015 EPAM

* Licensed under the Apache License, Version 2.0 (the "License"},
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
     * This is an class to store constants for parsing the sas7bdat file.
     */
    public static class DateTimeConstants
    {
        /**
         * These are sas7bdat format references to {@link java.text.SimpleDateFormat} date formats.
         * <p>
         * UNSUPPORTED FORMATS:
         * DTYYQC, PDJULG, PDJULI, QTR, QTRR, WEEKU, WEEKV, WEEKW,
         * YYQ, YYQC, YYQD, YYQN, YYQP, YYQS, YYQR, YYQRC, YYQRD, YYQRN, YYQRP, YYQRS
         */
        public static readonly IReadOnlyDictionary<string, string> DATE_FORMAT_STRINGS = new Dictionary<string, string>
    {
        {"B8601DA", "yyyyMMdd"},
        {"E8601DA", "yyyy-MM-dd"},
        {"DATE", "ddMMMyyyy"},
        {"DAY", "dd"},
        {"DDMMYY", "dd/MM/yyyy"},
        {"DDMMYYB", "dd MM yyyy"},
        {"DDMMYYC", "dd:MM:yyyy"},
        {"DDMMYYD", "dd-MM-yyyy"},
        {"DDMMYYN", "ddMMyyyy"},
        {"DDMMYYP", "dd.MM.yyyy"},
        {"DDMMYYS", "dd/MM/yyyy"},
        {"JULDAY", "D"},
        {"JULIAN", "yyyyD"},
        {"MMDDYY", "MM/dd/yyyy"},
        {"MMDDYYB", "MM dd yyyy"},
        {"MMDDYYC", "MM:dd:yyyy"},
        {"MMDDYYD", "MM-dd-yyyy"},
        {"MMDDYYN", "MMddyyyy"},
        {"MMDDYYP", "MM.dd.yyyy"},
        {"MMDDYYS", "MM/dd/yyyy"},
        {"MMYY", "MM'M'yyyy"},
        {"MMYYC", "MM:yyyy"},
        {"MMYYD", "MM-yyyy"},
        {"MMYYN", "MMyyyy"},
        {"MMYYP", "MM.yyyy"},
        {"MMYYS", "MM/yyyy"},
        {"MONNAME", "MMMM"},
        {"MONTH", "%M"},
        {"MONYY", "MMMyyyy"},
        {"WEEKDATE", "dddd, MMMM dd, yyyy"},
        {"WEEKDATX", "dddd, dd MMMM, yyyy"},
        {"WEEKDAY", "u"},
        {"DOWNAME", "dddd"},
        {"WORDDATE", "MMMM d, yyyy"},
        {"WORDDATX", "d MMMM yyyy"},
        {"YYMM", "yyyy'M'MM"},
        {"YYMMC", "yyyy:MM"},
        {"YYMMD", "yyyy-MM"},
        {"YYMMN", "yyyyMM"},
        {"YYMMP", "yyyy.MM"},
        {"YYMMS", "yyyy/MM"},
        {"YYMMDD", "yyyy-MM-dd"},
        {"YYMMDDB", "yyyy MM dd"},
        {"YYMMDDC", "yyyy:MM:dd"},
        {"YYMMDDD", "yyyy-MM-dd"},
        {"YYMMDDN", "yyyyMMdd"},
        {"YYMMDDP", "yyyy.MM.dd"},
        {"YYMMDDS", "yyyy/MM/dd"},
        {"YYMON", "yyyyMMM"},
        {"YEAR", "yyyy"},
    };

        /**
         * These are sas7bdat format references to {@link java.text.SimpleDateFormat} datetime formats.
         */
        public static readonly IReadOnlyDictionary<string, string> DATETIME_FORMAT_STRINGS = new Dictionary<string, string>
    {
        {"B8601DN", "yyyyMMdd"},
        {"B8601DT", "yyyyMMdd'T'HHmmssSSS"},
        {"B8601DX", "yyyyMMdd'T'HHmmssZ"},
        {"B8601DZ", "yyyyMMdd'T'HHmmssZ"},
        {"B8601LX", "yyyyMMdd'T'HHmmssZ"},
        {"E8601DN", "yyyy-MM-dd"},
        {"E8601DT", "yyyy-MM-dd'T'HH:mm:ss.SSS"},
        {"E8601DX", "yyyy-MM-dd'T'HH:mm:ssZ"},
        {"E8601DZ", "yyyy-MM-dd'T'HH:mm:ssZ"},
        {"E8601LX", "yyyy-MM-dd'T'HH:mm:ssZ"},
        {"DATEAMPM", "ddMMMyyyy:HH:mm:ss.SS tt"},
        {"DATETIME", "ddMMMyyyy:HH:mm:ss.SS"},
        {"DTDATE", "ddMMMyyyy"},
        {"DTMONYY", "MMMyyyy"},
        {"DTWKDATX", "dddd, dd MMMM, yyyy"},
        {"DTYEAR", "yyyy"},
        {"MDYAMPM", "MM/dd/yyyy H:mm tt"},
        {"TOD", "HH:mm:ss.SS"},
    };

        /**
         * These are time formats that are used in sas7bdat files.
         */
        public static readonly HashSet<string> TIME_FORMAT_STRINGS = new HashSet<string>
    {
        "TIME", "HHMM", "E8601LZ", "E8601TM", "HOUR", "MMSS", "TIMEAMPM"
    };
    }
}