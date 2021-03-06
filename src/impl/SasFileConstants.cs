using System;
using System.Collections.Generic;

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
     * This is an class to store constants for parsing the sas7bdat file (byte offsets, column formats, accuracy) as well as
     * the standard constants of time and the sizes of standard data types.
     */
    public static class SasFileConstants
    {
        /**
         * The size of the long value type in bytes.
         */
        public const int BYTES_IN_LONG = 8;

        /**
         * The size of the double value type in bytes.
         */
        public const int BYTES_IN_DOUBLE = 8;

        /**
         * The size of the int value type in bytes.
         */
        public const int BYTES_IN_INT = 4;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_1_LENGTH} bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ALIGN_1_OFFSET} bytes offset equals to ALIGN_1_CHECKER_VALUE, then starting from
         * the {@link SasFileConstants#DATE_CREATED_OFFSET} bytes offset every offset should increase by
         * {@link SasFileConstants#ALIGN_1_VALUE} bytes.
         */
        public const int ALIGN_1_CHECKER_VALUE = 51;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_1_LENGTH} bytes stored in the sas7bdat file with
         * a ALIGN_1_OFFSET bytes offset equals to {@link SasFileConstants#ALIGN_1_CHECKER_VALUE}, then starting from
         * the {@link SasFileConstants#DATE_CREATED_OFFSET} bytes offset every offset should increase by
         * {@link SasFileConstants#ALIGN_1_VALUE} bytes.
         */
        public const long ALIGN_1_OFFSET = 32L;

        /**
         * If a value with the length of ALIGN_1_LENGTH bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ALIGN_1_OFFSET} bytes offset equals to {@link SasFileConstants#ALIGN_1_CHECKER_VALUE},
         * then starting from the {@link SasFileConstants#DATE_CREATED_OFFSET} bytes offset every offset should increase
         * by {@link SasFileConstants#ALIGN_1_VALUE} bytes.
         */
        public const int ALIGN_1_LENGTH = 1;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_1_LENGTH} bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ALIGN_1_OFFSET} bytes offset equals to {@link SasFileConstants#ALIGN_1_CHECKER_VALUE},
         * then starting from the {@link SasFileConstants#DATE_CREATED_OFFSET} bytes offset every offset should increase by
         * ALIGN_1_VALUE bytes.
         */
        public const int ALIGN_1_VALUE = 4;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_2_LENGTH} bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ALIGN_2_OFFSET} bytes offset equals to U64_BYTE_CHECKER_VALUE, then:
         * - this sas7bdat file was created in the 64-bit version of SAS,
         * - starting from the {@link SasFileConstants#SAS_RELEASE_OFFSET} bytes offset every offset should increase by
         * {@link SasFileConstants#ALIGN_2_VALUE} bytes (in addition to {@link SasFileConstants#ALIGN_1_VALUE} bytes, if
         * those are added),
         * - the {@link SasFileConstants#PAGE_COUNT_LENGTH} value should increase by {@link SasFileConstants#ALIGN_2_VALUE}
         * bytes and the number of pages stored at the {@link SasFileConstants#PAGE_COUNT_OFFSET} bytes offset should read
         * as long.
         */
        public const int U64_BYTE_CHECKER_VALUE = 51;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_2_LENGTH} bytes stored in the sas7bdat file with
         * a ALIGN_2_OFFSET bytes offset equals to {@link SasFileConstants#U64_BYTE_CHECKER_VALUE}, then:
         * - this sas7bdat file was created in the 64-bit version of SAS,
         * - starting from the {@link SasFileConstants#SAS_RELEASE_OFFSET} bytes offset every offset should increase
         * by {@link SasFileConstants#ALIGN_2_VALUE} bytes, (in addition to {@link SasFileConstants#ALIGN_1_VALUE}
         * bytes if those are added) and the number of pages stored at the {@link SasFileConstants#PAGE_COUNT_OFFSET} bytes
         * offset should read as long.
         */
        public const long ALIGN_2_OFFSET = 35L;

        /**
         * If a value with the length of ALIGN_2_LENGTH bytes stored in the sas7bdat file at
         * a {@link SasFileConstants#ALIGN_2_OFFSET} bytes offset equals to
         * {@link SasFileConstants#U64_BYTE_CHECKER_VALUE}, then:
         * - this sas7bdat file was created in the 64-bit version of SAS,
         * - starting from the {@link SasFileConstants#SAS_RELEASE_OFFSET} bytes offset every offset should increase by
         * {@link SasFileConstants#ALIGN_2_VALUE} bytes (in addition to {@link SasFileConstants#ALIGN_1_VALUE} bytes if
         * those are added) and the number of pages stored at the {@link SasFileConstants#PAGE_COUNT_OFFSET} bytes offset
         * should read as long.
         */
        public const int ALIGN_2_LENGTH = 1;

        /**
         * If a value with the length of {@link SasFileConstants#ALIGN_2_LENGTH} bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ALIGN_2_OFFSET} bytes offset equals to
         * {@link SasFileConstants#U64_BYTE_CHECKER_VALUE}, then:
         * - this sas7bdat file was created in the 64-bit version of SAS,
         * - starting from the {@link SasFileConstants#SAS_RELEASE_OFFSET} bytes offset every offset should increase by
         * ALIGN_2_VALUE bytes (in addition to {@link SasFileConstants#ALIGN_1_VALUE} bytes if those are added) and
         * the number of pages stored at the {@link SasFileConstants#PAGE_COUNT_OFFSET} bytes offset should read as long.
         */
        public const int ALIGN_2_VALUE = 4;

        /**
         * If a value with the length of {@link SasFileConstants#ENDIANNESS_LENGTH} bytes stored in the sas7bdat file with
         * a ENDIANNESS_OFFSET bytes offset equals to 1 then the bytes order is little-endian (Intel),
         * if the value equals to 0 then the bytes order is big-endian.
         */
        public const long ENDIANNESS_OFFSET = 37L;

        /**
         * If a value with the length of ENDIANNESS_LENGTH bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ENDIANNESS_OFFSET} bytes offset equals to 1 then the bytes order is
         * little-endian (Intel), if the value equals to 0 then the bytes order is big-endian.
         */
        public const int ENDIANNESS_LENGTH = 1;

        /**
         * If a value with the length of ENDIANNESS_LENGTH bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ENDIANNESS_OFFSET} bytes offset equals to LITTLE_ENDIAN_CHECKER
         * then the bytes order is little-endian (Intel), if the value equals to
         * {@link SasFileConstants#BIG_ENDIAN_CHECKER} then the bytes order is big-endian.
         */
        public const int LITTLE_ENDIAN_CHECKER = 1;

        /**
         * If a value with the length of ENDIANNESS_LENGTH bytes stored in the sas7bdat file with
         * a {@link SasFileConstants#ENDIANNESS_OFFSET} bytes offset equals to
         * {@link SasFileConstants#LITTLE_ENDIAN_CHECKER} then the bytes order is little-endian (Intel),
         * if the value equals to BIG_ENDIAN_CHECKER then the bytes order is big-endian.
         */
        public const int BIG_ENDIAN_CHECKER = 0;

        /**
         * The sas7bdat file stores its character encoding with the length of {@link SasFileConstants#ENCODING_LENGTH} bytes
         * and a ENCODING_OFFSET bytes offset.
         */
        public const long ENCODING_OFFSET = 70L;

        /**
         * The sas7bdat files its character encoding with the length of ENCODING_LENGTH bytes and
         * a {@link SasFileConstants#ENCODING_OFFSET} bytes offset.
         */
        public const int ENCODING_LENGTH = 1;

        /**
         * The integer (one or two bytes) at the {@link SasFileConstants#ENCODING_OFFSET} indicates the character encoding
         * of string data.  The SAS_CHARACTER_ENCODINGS map links the values that are known to occur and the associated
         * encoding.  This list excludes encodings present in SAS but missing support in {@link java.nio.charset}
         */
        public static readonly Dictionary<byte, string> SAS_CHARACTER_ENCODINGS = new Dictionary<byte, string>()
    {
        {(byte) 0x46, "x-MacArabic"},
        {(byte) 0xF5, "x-MacCroatian"},
        {(byte) 0xF6, "x-MacCyrillic"},
        {(byte) 0x48, "x-MacGreek"},
        {(byte) 0x47, "x-MacHebrew"},
        {(byte) 0xA3, "x-MacIceland"},
        {(byte) 0x22, "ISO-8859-6"},
        {(byte) 0x45, "x-MacRoman"},
        {(byte) 0xF7, "x-MacRomania"},
        {(byte) 0x49, "x-MacThai"},
        {(byte) 0x4B, "x-MacTurkish"},
        {(byte) 0x4C, "x-MacUkraine"},
        {(byte) 0x7B, "Big5"},
        {(byte) 0x21, "ISO-8859-5"},
        {(byte) 0x4E, "IBM037"},
        {(byte) 0x5F, "x-IBM1025"},
        {(byte) 0xCF, "x-IBM1097"},
        {(byte) 0x62, "x-IBM1112"},
        {(byte) 0x63, "x-IBM1122"},
        {(byte) 0xB7, "IBM01140"},
        {(byte) 0xB8, "IBM01141"},
        {(byte) 0xB9, "IBM01142"},
        {(byte) 0xBA, "IBM01143"},
        {(byte) 0xBB, "IBM01144"},
        {(byte) 0xBC, "IBM01145"},
        {(byte) 0xBD, "IBM01146"},
        {(byte) 0xBE, "IBM01147"},
        {(byte) 0xBF, "IBM01148"},
        {(byte) 0xD3, "IBM01149"},
        {(byte) 0x57, "IBM424"},
        {(byte) 0x58, "IBM500"},
        {(byte) 0x59, "IBM-Thai"},
        {(byte) 0x5A, "IBM870"},
        {(byte) 0x5B, "x-IBM875"},
        {(byte) 0x7D, "GBK"},
        {(byte) 0x86, "EUC-JP"},
        {(byte) 0x8C, "EUC-KR"},
        {(byte) 0x77, "x-EUC-TW"},
        {(byte) 0xCD, "GB18030"},
        {(byte) 0x23, "ISO-8859-7"},
        {(byte) 0x24, "ISO-8859-8"},
        {(byte) 0x80, "x-IBM1381"},
        {(byte) 0x82, "x-IBM930"},
        {(byte) 0x8B, "x-IBM933"},
        {(byte) 0x7C, "x-IBM935"},
        {(byte) 0x75, "x-IBM937"},
        {(byte) 0x81, "x-IBM939"},
        {(byte) 0x89, "x-IBM942"},
        {(byte) 0x8E, "x-IBM949"},
        {(byte) 0xAC, "x-ISO2022-CN-CNS"},
        {(byte) 0xA9, "x-ISO2022-CN-GB"},
        {(byte) 0xA7, "ISO-2022-JP"},
        {(byte) 0xA8, "ISO-2022-KR"},
        {(byte) 0x1D, "ISO-8859-1"},
        {(byte) 0x1E, "ISO-8859-2"},
        {(byte) 0x1F, "ISO-8859-3"},
        {(byte) 0x20, "ISO-8859-4"},
        {(byte) 0x25, "ISO-8859-9"},
        {(byte) 0xF2, "ISO-8859-13"},
        {(byte) 0x28, "ISO-8859-15"},
        {(byte) 0x88, "x-windows-iso2022jp"},
        {(byte) 0x7E, "x-mswin-936"},
        {(byte) 0x8D, "x-windows-949"},
        {(byte) 0x76, "x-windows-950"},
        {(byte) 0xAD, "IBM037"},
        {(byte) 0x6C, "x-IBM1025"},
        {(byte) 0x6D, "IBM1026"},
        {(byte) 0x6E, "IBM1047"},
        {(byte) 0xD0, "x-IBM1097"},
        {(byte) 0x6F, "x-IBM1112"},
        {(byte) 0x70, "x-IBM1122"},
        {(byte) 0xC0, "IBM01140"},
        {(byte) 0xC1, "IBM01141"},
        {(byte) 0xC2, "IBM01142"},
        {(byte) 0xC3, "IBM01143"},
        {(byte) 0xC4, "IBM01144"},
        {(byte) 0xC5, "IBM01145"},
        {(byte) 0xC6, "IBM01146"},
        {(byte) 0xC7, "IBM01147"},
        {(byte) 0xC8, "IBM01148"},
        {(byte) 0xD4, "IBM01149"},
        {(byte) 0x66, "IBM424"},
        {(byte) 0x67, "IBM-Thai"},
        {(byte) 0x68, "IBM870"},
        {(byte) 0x69, "x-IBM875"},
        {(byte) 0xEA, "x-IBM930"},
        {(byte) 0xEB, "x-IBM933"},
        {(byte) 0xEC, "x-IBM935"},
        {(byte) 0xED, "x-IBM937"},
        {(byte) 0xEE, "x-IBM939"},
        {(byte) 0x2B, "IBM437"},
        {(byte) 0x2C, "IBM850"},
        {(byte) 0x2D, "IBM852"},
        {(byte) 0x3A, "IBM857"},
        {(byte) 0x2E, "IBM00858"},
        {(byte) 0x2F, "IBM862"},
        {(byte) 0x33, "IBM866"},
        {(byte) 0x8A, "Shift_JIS"},
        {(byte) 0xF8, "JIS_X0201"},
        {(byte) 0x27, "x-iso-8859-11"},
        {(byte) 0x1C, "US-ASCII"},
        {(byte) 0x14, "UTF-8"},
        {(byte) 0x42, "windows-1256"},
        {(byte) 0x43, "windows-1257"},
        {(byte) 0x3D, "windows-1251"},
        {(byte) 0x3F, "windows-1253"},
        {(byte) 0x41, "windows-1255"},
        {(byte) 0x3E, "windows-1252"},
        {(byte) 0x3C, "windows-1250"},
        {(byte) 0x40, "windows-1254"},
        {(byte) 0x44, "windows-1258"},
    };

        /**
         * The sas7bdat file stores the table name with the length of {@link SasFileConstants#DATASET_LENGTH} bytes and
         * a DATASET_OFFSET bytes offset.
         */
        public const long DATASET_OFFSET = 92L;

        /**
         * The sas7bdat file stores the table name with the length of DATASET_LENGTH bytes and
         * a {@link SasFileConstants#DATASET_OFFSET} bytes offset.
         */
        public const int DATASET_LENGTH = 64;

        /**
         * The sas7bdat file stores its file type with the length of {@link SasFileConstants#FILE_TYPE_LENGTH} bytes
         * and a FILE_TYPE_OFFSET bytes offset.
         */
        public const long FILE_TYPE_OFFSET = 156L;

        /**
         * The sas7bdat file stores its file type with the length of FILE_TYPE_LENGTH bytes and
         * a {@link SasFileConstants#FILE_TYPE_OFFSET} bytes offset.
         */
        public const int FILE_TYPE_LENGTH = 8;

        /**
         * The sas7bdat file stores its creation date with the length of {@link SasFileConstants#DATE_CREATED_LENGTH} bytes
         * and a DATE_CREATED_OFFSET bytes offset (with possible addition of {@link SasFileConstants#ALIGN_1_VALUE}).
         * The date is a double value denoting the number of seconds elapsed from 01/01/1960 to the date stored.
         */
        public const long DATE_CREATED_OFFSET = 164L;

        /**
         * The sas7bdat file stores its creation date with the length of DATE_CREATED_LENGTH bytes and
         * a {@link SasFileConstants#DATE_CREATED_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE}). The date is a double value denoting the number of seconds elapsed
         * from 01/01/1960 to the date stored.
         */
        public const int DATE_CREATED_LENGTH = 8;

        /**
         * The sas7bdat file stores its last modification date with the length of
         * {@link SasFileConstants#DATE_MODIFIED_LENGTH} bytes and a DATE_MODIFIED_OFFSET bytes offset (with possible
         * addition of {@link SasFileConstants#ALIGN_1_VALUE}). The date is a double value denoting the number of seconds
         * elapsed from 01/01/1960 to the date stored.
         */
        public const long DATE_MODIFIED_OFFSET = 172L;

        /**
         * The sas7bdat file stores its last modification date with the length of DATE_MODIFIED_LENGTH bytes and
         * a {@link SasFileConstants#DATE_MODIFIED_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE}). The date is a value of double format denoting the number of seconds
         * elapsed from 01/01/1960 to the date stored.
         */
        public const int DATE_MODIFIED_LENGTH = 8;

        /**
         * The sas7bdat file stores the length of its metadata (can be 1024 and 8192) as an int value with the length of
         * {@link SasFileConstants#HEADER_SIZE_LENGTH} bytes and a HEADER_SIZE_OFFSET bytes offset (with possible addition
         * of {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const long HEADER_SIZE_OFFSET = 196L;

        /**
         * The sas7bdat file stores the length of its metadata (can be 1024 and 8192) as an int value with the length of
         * HEADER_SIZE_LENGTH bytes and a  {@link SasFileConstants#HEADER_SIZE_OFFSET} bytes offset (with possible addition
         * of {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const int HEADER_SIZE_LENGTH = 4;

        /**
         * The sas7bdat file stores the length of its pages as an int value with the length of
         * {@link SasFileConstants#PAGE_SIZE_LENGTH} bytes and a PAGE_SIZE_OFFSET bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const long PAGE_SIZE_OFFSET = 200L;

        /**
         * The sas7bdat file stores the length of its pages as an int value with the length of PAGE_SIZE_LENGTH bytes and
         * a {@link SasFileConstants#PAGE_SIZE_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const int PAGE_SIZE_LENGTH = 4;

        /**
         * The sas7bdat file stores the number of its pages as an int or long value (depending on
         * {@link SasFileConstants#ALIGN_2_VALUE}) with the length of {@link SasFileConstants#PAGE_COUNT_LENGTH} bytes
         * (with possible addition of {@link SasFileConstants#ALIGN_2_VALUE}) and a PAGE_COUNT_OFFSET bytes offset
         * (with possible addition of {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const long PAGE_COUNT_OFFSET = 204L;

        /**
         * The sas7bdat file stores the number of its pages as an int or long value (depending on
         * {@link SasFileConstants#ALIGN_2_VALUE}) with the length of PAGE_COUNT_LENGTH bytes (with possible addition of
         * {@link SasFileConstants#ALIGN_2_VALUE}) and a {@link SasFileConstants#PAGE_COUNT_OFFSET} bytes offset
         * (with possible addition of {@link SasFileConstants#ALIGN_1_VALUE}).
         */
        public const int PAGE_COUNT_LENGTH = 4;

        /**
         * The sas7bdat file stores the name of SAS version in which the sas7bdat was created with the length of
         * {@link SasFileConstants#SAS_RELEASE_LENGTH} bytes and a SAS_RELEASE_OFFSET bytes offset (with possible addition
         * of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const long SAS_RELEASE_OFFSET = 216L;

        /**
         * The sas7bdat file stores the name of SAS version in which the sas7bdat was created with the length of
         * SAS_RELEASE_LENGTH bytes and a {@link SasFileConstants#SAS_RELEASE_OFFSET} bytes offset (with possible addition
         * of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int SAS_RELEASE_LENGTH = 8;

        /**
         * The sas7bdat file stores the name of the server version on which the sas7bdat was created with the length of
         * {@link SasFileConstants#SAS_SERVER_TYPE_LENGTH} bytes and a SAS_SERVER_TYPE_OFFSET bytes offset (with possible
         * addition of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const long SAS_SERVER_TYPE_OFFSET = 224L;

        /**
         * The sas7bdat file stores the name of the server version on which the sas7bdat was created with the length of
         * SAS_SERVER_TYPE_LENGTH bytes and a {@link SasFileConstants#SAS_SERVER_TYPE_OFFSET} bytes offset (with possible
         * addition of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int SAS_SERVER_TYPE_LENGTH = 16;

        /**
         * The sas7bdat file stores the version of the OS in which the sas7bdat was created with the length of
         * {@link SasFileConstants#OS_VERSION_NUMBER_LENGTH} bytes and a OS_VERSION_NUMBER_OFFSET bytes offset
         * (with possible addition of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const long OS_VERSION_NUMBER_OFFSET = 240L;

        /**
         * The sas7bdat file stores the version of the OS in which the sas7bdat was created with the length of
         * OS_VERSION_NUMBER_LENGTH bytes and a {@link SasFileConstants#OS_VERSION_NUMBER_OFFSET} bytes offset (with
         * possible addition of {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int OS_VERSION_NUMBER_LENGTH = 16;

        /**
         * The sas7bdat file stores the name of the OS in which the sas7bdat was created with the length of
         * {@link SasFileConstants#OS_MAKER_LENGTH} bytes and a OS_MAKER_OFFSET bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}). If the OS name is
         * an empty string, then the file stores the OS name with the length of {@link SasFileConstants#OS_NAME_LENGTH}
         * bytes and a {@link SasFileConstants#OS_NAME_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const long OS_MAKER_OFFSET = 256L;

        /**
         * The sas7bdat file stores the name of the OS in which the sas7bdat was created with the length of OS_MAKER_LENGTH
         * bytes and a {@link SasFileConstants#OS_MAKER_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}). If the OS name is
         * an empty string, then the file stores the OS name with the length of {@link SasFileConstants#OS_NAME_LENGTH}
         * bytes and a {@link SasFileConstants#OS_NAME_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int OS_MAKER_LENGTH = 16;

        /**
         * The sas7bdat file stores the name of the OS in which the sas7bdat was created with the length of
         * {@link SasFileConstants#OS_NAME_LENGTH} bytes and a OS_NAME_OFFSET bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}). If the OS name is
         * an empty string, then the file stores the OS name with the length of {@link SasFileConstants#OS_MAKER_LENGTH}
         * bytes and a {@link SasFileConstants#OS_MAKER_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const long OS_NAME_OFFSET = 272L;

        /**
         * The sas7bdat file stores the name of the OS in which the sas7bdat was created with the length of OS_NAME_LENGTH
         * bytes and a {@link SasFileConstants#OS_NAME_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}). If the OS name is
         * an empty string, then the file stores the OS name with the length of  {@link SasFileConstants#OS_MAKER_LENGTH}
         * bytes and a {@link SasFileConstants#OS_MAKER_OFFSET} bytes offset (with possible addition of
         * {@link SasFileConstants#ALIGN_1_VALUE} and {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int OS_NAME_LENGTH = 16;

        /**
         * An offset in bytes from the start of the page - for sas7bdat files created in the 32-bit version of SAS
         * (see {@link SasFileConstants#ALIGN_2_VALUE}). Added to all offsets within a page.
         */
        public const int PAGE_BIT_OFFSET_X86 = 16;

        /**
         * An offset in bytes from the start of the page - for sas7bdat files created in the 64-bit version of SAS
         * (see {@link SasFileConstants#ALIGN_2_VALUE}). Added to all offsets within a page.
         */
        public const int PAGE_BIT_OFFSET_X64 = 32;

        /**
         * The length in bytes of one subheader pointer ({@link SasFileParser.SubheaderPointer}) of a sas7bdat file
         * created in the 32-bit version of SAS (see {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int SUBHEADER_POINTER_LENGTH_X86 = 12;

        /**
         * The length in bytes of one subheader pointer ({@link SasFileParser.SubheaderPointer}) of a sas7bdat file
         * created in the 64-bit version of SAS (see {@link SasFileConstants#ALIGN_2_VALUE}).
         */
        public const int SUBHEADER_POINTER_LENGTH_X64 = 24;

        /**
         * The sas7bdat file stores the type of page as a short value with the length of
         * {@link SasFileConstants#PAGE_TYPE_LENGTH} bytes and a PAGE_TYPE_OFFSET bytes offset (with addition of
         * {@link SasFileConstants#PAGE_BIT_OFFSET_X86} or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         * There can be {@link SasFileConstants#PAGE_META_TYPE_1}, {@link SasFileConstants#PAGE_META_TYPE_2},
         * {@link SasFileConstants#PAGE_DATA_TYPE}, {@link SasFileConstants#PAGE_MIX_TYPE_1},
         * {@link SasFileConstants#PAGE_MIX_TYPE_2} or {@link SasFileConstants#PAGE_AMD_TYPE} page types.
         */
        public const long PAGE_TYPE_OFFSET = 0L;

        /**
         * The sas7bdat file stores the type of page as a short value with the length of PAGE_TYPE_LENGTH bytes
         * and a {@link SasFileConstants#PAGE_TYPE_OFFSET} bytes offset (with addition of
         * {@link SasFileConstants#PAGE_BIT_OFFSET_X86} or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         * There can be {@link SasFileConstants#PAGE_META_TYPE_1}, {@link SasFileConstants#PAGE_META_TYPE_2},
         * {@link SasFileConstants#PAGE_DATA_TYPE}, {@link SasFileConstants#PAGE_MIX_TYPE_1}
         * {@link SasFileConstants#PAGE_MIX_TYPE_2} or {@link SasFileConstants#PAGE_AMD_TYPE} page types.
         */
        public const int PAGE_TYPE_LENGTH = 2;

        /**
         * For pages of the {@link SasFileConstants#PAGE_DATA_TYPE} type, the sas7bdat file stores the number of rows
         * in the table on the current page as a short value - with the length of
         * {@link SasFileConstants#BLOCK_COUNT_LENGTH} bytes and a BLOCK_COUNT_OFFSET bytes offset (with addition of
         * {@link SasFileConstants#PAGE_BIT_OFFSET_X86} or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         */
        public const long BLOCK_COUNT_OFFSET = 2L;

        /**
         * For pages of the {@link SasFileConstants#PAGE_DATA_TYPE} type, the sas7bdat file stores the number of rows
         * in the table on the current page as a short value - with the length of BLOCK_COUNT_LENGTH bytes
         * and a {@link SasFileConstants#BLOCK_COUNT_OFFSET} bytes offset (with addition of
         * {@link SasFileConstants#PAGE_BIT_OFFSET_X86} or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         */
        public const int BLOCK_COUNT_LENGTH = 2;

        /**
         * For pages of the {@link SasFileConstants#PAGE_META_TYPE_1}, {@link SasFileConstants#PAGE_META_TYPE_2}
         * {@link SasFileConstants#PAGE_MIX_TYPE_1} and {@link SasFileConstants#PAGE_MIX_TYPE_2} types,
         * the sas7bdat file stores the number of subheaders on the current page as a short value - with the length of
         * {@link SasFileConstants#SUBHEADER_COUNT_LENGTH} bytes and a SUBHEADER_COUNT_OFFSET bytes offset
         * from the beginning of the page (with addition of {@link SasFileConstants#PAGE_BIT_OFFSET_X86}
         * or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         */
        public const long SUBHEADER_COUNT_OFFSET = 4L;

        /**
         * For pages of the {@link SasFileConstants#PAGE_META_TYPE_1}, {@link SasFileConstants#PAGE_META_TYPE_2},
         * {@link SasFileConstants#PAGE_MIX_TYPE_1} and {@link SasFileConstants#PAGE_MIX_TYPE_2} types,
         * the sas7bdat file stores the number of subheaders on the current page as a short value - with the length of
         * SUBHEADER_COUNT_LENGTH bytes and a {@link SasFileConstants#SUBHEADER_COUNT_OFFSET} bytes offset from the
         * beginning of the page (with addition of {@link SasFileConstants#PAGE_BIT_OFFSET_X86}
         * or {@link SasFileConstants#PAGE_BIT_OFFSET_X64}).
         */
        public const int SUBHEADER_COUNT_LENGTH = 2;

        /**
         * The page type storing metadata as a set of subheaders. It can also store compressed row data in subheaders.
         * The sas7bdat format has three values that correspond to the page type 'meta'.
         */
        public const int PAGE_META_TYPE_1 = 0;

        /**
         * The page type storing metadata as a set of subheaders. It can also store compressed row data in subheaders.
         * The sas7bdat format has three values that correspond to the page type 'meta'.
         */
        public const int PAGE_META_TYPE_2 = 16384;

        /**
         * The page type storing metadata as a set of subheaders. It can also store compressed row data in subheaders.
         * This type is present when there are deleted observations in the dataset.
         * The sas7bdat format has three values that correspond to the type 'meta'.
         */
        public const int PAGE_CMETA_TYPE = 128;

        /**
         * The page type storing only data as a number of table rows.
         */
        public const int PAGE_DATA_TYPE = 256;

        /**
         * Another page type for storing only data as a number of table rows.
         */
        public const int PAGE_DATA_TYPE_2 = 384;

        /**
         * The page type storing metadata as a set of subheaders and data as a number of table rows.
         */
        public const int PAGE_MIX_TYPE_1 = 512;

        /**
         * The page type storing metadata as a set of subheaders and data as a number of table rows.
         * Probably this page type is used for mix pages from which some rows have been deleted.
         */
        public const int PAGE_MIX_TYPE_2 = 640;

        /**
         * The page type amd.
         */
        public const int PAGE_AMD_TYPE = 1024;

        /**
         * The sas7bdat file stores the array of subheader pointers ({@link SasFileParser.SubheaderPointer}) at this
         * offset (adding {@link SasFileConstants#PAGE_BIT_OFFSET_X86} or {@link SasFileConstants#PAGE_BIT_OFFSET_X64})
         * from the beginning of the page.
         */
        public const int SUBHEADER_POINTERS_OFFSET = 8;

        /**
         * If the {@link SasFileParser.SubheaderPointer#compression} value of a subheader equals to TRUNCATED_SUBHEADER_ID
         * then it does not contain useful information.
         */
        public const int TRUNCATED_SUBHEADER_ID = 1;

        /**
         * A subheader with compressed data has two parameters:
         * its {@link SasFileParser.SubheaderPointer#compression} should equal to COMPRESSED_SUBHEADER_ID and its
         * {@link SasFileParser.SubheaderPointer#type} should equal to {@link SasFileConstants#COMPRESSED_SUBHEADER_TYPE}.
         */
        public const int COMPRESSED_SUBHEADER_ID = 4;

        /**
         * A Subheader with compressed data has two parameters:
         * its {@link SasFileParser.SubheaderPointer#compression} should equal to
         * {@link SasFileConstants#COMPRESSED_SUBHEADER_ID} and its {@link SasFileParser.SubheaderPointer#type}
         * should equal to COMPRESSED_SUBHEADER_TYPE.
         */
        public const int COMPRESSED_SUBHEADER_TYPE = 1;

        /**
         * The number of bits in a byte.
         */
        public const int BITS_IN_BYTE = 8;

        /**
         * The multiplier whose product with the length of the variable type (that can be int or long depending on the
         * {@link SasFileConstants#ALIGN_2_VALUE} value) is the offset from the subheader beginning
         * {@link SasFileParser.RowSizeSubheader} at which the row length is stored.
         */
        public const int ROW_LENGTH_OFFSET_MULTIPLIER = 5;

        /**
         * The multiplier whose product with the length of the variable type (that can be int or long depending on the
         * {@link SasFileConstants#ALIGN_2_VALUE} value) is the offset from the subheader beginning
         * {@link SasFileParser.RowSizeSubheader} at which the number of rows in the table is stored.
         */
        public const int ROW_COUNT_OFFSET_MULTIPLIER = 6;

        /**
         * The multiplier whose product with the length of the variable type (that can be int or long depending on the
         * {@link SasFileConstants#ALIGN_2_VALUE} value) is the offset from the subheader beginning
         * {@link SasFileParser.RowSizeSubheader} at which the number of deleted rows in the table is stored.
         */
        public const int DELETED_ROW_COUNT_OFFSET_MULTIPLIER = 8;

        /**
         * The multiplier whose product with the length of the variable type (that can be int or long depending on the
         * {@link SasFileConstants#ALIGN_2_VALUE} value) is the offset from the subheader beginning
         * {@link SasFileParser.RowSizeSubheader} at which the file stores the number of rows in the table
         * on the last page of the {@link SasFileConstants#PAGE_MIX_TYPE_1} or {@link SasFileConstants#PAGE_MIX_TYPE_2}
         * type.
         */
        public const int ROW_COUNT_ON_MIX_PAGE_OFFSET_MULTIPLIER = 15;

        /**
         * The number of bytes taken by the value denoting the length of the text block with information about
         * file compression and table rows (name, label, format).
         */
        public const int TEXT_BLOCK_SIZE_LENGTH = 2;

        /**
         * A substring that appears in the text block with information about file compression and table rows
         * (name, label, format) if CHAR compression is used.
         */
        public const string COMPRESS_CHAR_IDENTIFYING_STRING = "SASYZCRL";

        /**
         * A substring that appears in the text block with information about file compression and table rows
         * (name, label, format) if BIN compression is used.
         */
        public const string COMPRESS_BIN_IDENTIFYING_STRING = "SASYZCR2";

        /**
         * The length of the column name pointer in bytes.
         */
        public const int COLUMN_NAME_POINTER_LENGTH = 8;

        /**
         * For each table column, the sas7bdat file stores the index of the
         * {@link SasFileParser.ColumnTextSubheader} subheader whose text block contains the name
         * of the column - with the length of {@link SasFileConstants#COLUMN_NAME_TEXT_SUBHEADER_LENGTH} bytes and an offset
         * measured from the beginning of the {@link SasFileParser.ColumnNameSubheader} subheader
         * and calculated by the following formula: COLUMN_NAME_TEXT_SUBHEADER_OFFSET +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const long COLUMN_NAME_TEXT_SUBHEADER_OFFSET = 0L;

        /**
         * For each table column, the sas7bdat file stores the index of the
         * {@link SasFileParser.ColumnTextSubheader} subheader whose text block contains the name
         * of the column - with the length of COLUMN_NAME_TEXT_SUBHEADER_LENGTH bytes and an offset measured from
         * the beginning of the {@link SasFileParser.ColumnNameSubheader} subheader
         * and calculated by the following formula: {@link SasFileConstants#COLUMN_NAME_TEXT_SUBHEADER_OFFSET} +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const int COLUMN_NAME_TEXT_SUBHEADER_LENGTH = 2;

        /**
         * For each table column, the sas7bdat file stores the offset (in symbols) of the column name from the beginning
         * of the text block of the {@link SasFileParser.ColumnTextSubheader} subheader (see
         * {@link SasFileConstants#COLUMN_NAME_TEXT_SUBHEADER_OFFSET})- with the length of
         * {@link SasFileConstants#COLUMN_NAME_OFFSET_LENGTH} bytes and an offset measured from the beginning
         * of the {@link SasFileParser.ColumnNameSubheader} subheader and calculated by
         * the following formula: COLUMN_NAME_OFFSET_OFFSET +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const long COLUMN_NAME_OFFSET_OFFSET = 2L;

        /**
         * For each table column, the sas7bdat file stores the offset (in symbols) of the column name from the beginning
         * of the text block of the {@link SasFileParser.ColumnTextSubheader} subheader (see
         * {@link SasFileConstants#COLUMN_NAME_TEXT_SUBHEADER_OFFSET})- with the length of COLUMN_NAME_OFFSET_LENGTH bytes
         * and an offset measured from the beginning of the {@link SasFileParser.ColumnNameSubheader}
         * subheader and calculated by the following formula: {@link SasFileConstants#COLUMN_NAME_OFFSET_OFFSET} +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const int COLUMN_NAME_OFFSET_LENGTH = 2;

        /**
         * For each table column, the sas7bdat file stores column name length (in symbols):
         * - with the length of {@link SasFileConstants#COLUMN_NAME_LENGTH_LENGTH} bytes,
         * - at an offset  measured from the beginning of the
         * {@link SasFileParser.ColumnNameSubheader} subheader
         * and calculated by the following formula: COLUMN_NAME_LENGTH_OFFSET +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const long COLUMN_NAME_LENGTH_OFFSET = 4L;

        /**
         * For each table column, the sas7bdat file stores column name length (in symbols):
         * - with the length of COLUMN_NAME_LENGTH_LENGTH bytes.
         * - at an offset measured from the beginning of the
         * {@link SasFileParser.ColumnNameSubheader} subheader and calculated
         * by the following formula: {@link SasFileConstants#COLUMN_NAME_LENGTH_OFFSET} +
         * + column number * {@link SasFileConstants#COLUMN_NAME_POINTER_LENGTH} + size of the value type (int or long
         * depending on the {@link SasFileConstants#ALIGN_2_VALUE} value).
         */
        public const int COLUMN_NAME_LENGTH_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the value (int or long depending on
         * {@link SasFileConstants#ALIGN_2_VALUE}) that defines the offset of data in the current column
         * from the beginning of the row with data in bytes:
         * - at an offset measured from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader
         * and calculated by the following formula: COLUMN_DATA_OFFSET_OFFSET +
         * + column index * (8 + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})) +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader.
         */
        public const long COLUMN_DATA_OFFSET_OFFSET = 8L;

        /**
         * For every table column, the sas7bdat file stores the denotation (in bytes) of data length in a column:
         * - at an offset measured from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader
         * and calculated by the following formula: COLUMN_DATA_LENGTH_OFFSET +
         * + column index * (8 + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})) +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader,
         * - with the length of {@link SasFileConstants#COLUMN_DATA_LENGTH_LENGTH} bytes.
         */
        public const long COLUMN_DATA_LENGTH_OFFSET = 8L;

        /**
         * For every table column, the sas7bdat file stores the denotation (in bytes) of data length in a column:
         * - at an offset measured from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader
         * and calculated by the following formula: {@link SasFileConstants#COLUMN_DATA_LENGTH_OFFSET} +
         * + column index * (8 + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})) +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader,
         * - with the length of COLUMN_DATA_LENGTH_LENGTH bytes.
         */
        public const int COLUMN_DATA_LENGTH_LENGTH = 4;

        /**
         * For every table column, the sas7bdat file stores the data type of a column:
         * - with the length of {@link SasFileConstants#COLUMN_TYPE_LENGTH} bytes.
         * - at an offset measured from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader
         * and calculated by the following formula: COLUMN_TYPE_OFFSET +
         * + column index * (8 + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})) +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader,
         * If type=1, then the column stores numeric values, if type=0, the column stores text.
         */
        public const long COLUMN_TYPE_OFFSET = 14L;

        /**
         * For every table column, the sas7bdat file stores the data type of a column:
         * - with the length of COLUMN_TYPE_LENGTH bytes.
         * - at an offset measured from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader
         * and calculated by the following formula: {@link SasFileConstants#COLUMN_TYPE_OFFSET} +
         * + column index * (8 + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})) +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.ColumnAttributesSubheader} subheader,
         * If type=1, then the column stores numeric values, if type=0, the column stores text.
         */
        public const int COLUMN_TYPE_LENGTH = 1;

        /**
         * For some table column, the sas7bdat file stores width of format:
         * - with the length of {@link SasFileConstants#COLUMN_FORMAT_WIDTH_OFFSET_LENGTH} bytes,
         * - at an offset calculated as COLUMN_FORMAT_WIDTH_OFFSET bytes + the size of value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_FORMAT_WIDTH_OFFSET = 0L;

        /**
         * For some table column, the sas7bdat file stores width of format:
         * - with the length of COLUMN_FORMAT_WIDTH_OFFSET_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COLUMN_FORMAT_WIDTH_OFFSET bytes} + the size of value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_FORMAT_WIDTH_OFFSET_LENGTH = 2;

        /**
         * For some table column, the sas7bdat file stores precision of format:
         * - with the length of {@link SasFileConstants#COLUMN_FORMAT_PRECISION_OFFSET_LENGTH} bytes,
         * - at an offset calculated as COLUMN_FORMAT_PRECISION_OFFSET bytes + the size of value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_FORMAT_PRECISION_OFFSET = 2L;

        /**
         * For some table column, the sas7bdat file stores width of format:
         * - with the length of COLUMN_FORMAT_PRECISION_OFFSET_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COLUMN_FORMAT_PRECISION_OFFSET bytes} +
         * + the size of value type (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_FORMAT_PRECISION_OFFSET_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the index of
         * the {@link SasFileParser.ColumnTextSubheader} whose text block stores the column format:
         * - with the length of {@link SasFileConstants#COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_LENGTH} bytes,
         * - at an offset calculated as COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET bytes +
         * + 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET = 22L;

        /**
         * For every table column, the sas7bdat file stores the index of the
         * {@link SasFileParser.ColumnTextSubheader} whose text block stores the column format:
         * - with the length of COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET} bytes +
         * + 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the offset (in symbols) of the column format from
         * the beginning of the text block of the {@link SasFileParser.ColumnTextSubheader} subheader
         * where it belongs:
         * - with the length of {@link SasFileConstants#COLUMN_FORMAT_OFFSET_LENGTH} bytes,
         * - at an offset calculated as COLUMN_FORMAT_OFFSET_OFFSET bytes + 3 * the size of value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from the beginning of
         * the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_FORMAT_OFFSET_OFFSET = 24L;

        /**
         * For every table column, the sas7bdat file stores the offset (in symbols) of the column format from
         * the beginning of the text block of the {@link SasFileParser.ColumnTextSubheader}
         * subheader where it belongs:
         * - with the length of COLUMN_FORMAT_OFFSET_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COLUMN_FORMAT_OFFSET_OFFSET} bytes +
         * + 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_FORMAT_OFFSET_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the column format length (in symbols):
         * - with the length of {@link SasFileConstants#COLUMN_FORMAT_LENGTH_LENGTH} bytes,
         * - at an offset calculated as COLUMN_FORMAT_LENGTH_OFFSET bytes + the size of three value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from the beginning of
         * the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_FORMAT_LENGTH_OFFSET = 26L;

        /**
         * For every table column, the sas7bdat file stores the column format length (in symbols):
         * - with the length of COLUMN_FORMAT_LENGTH_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COLUMN_FORMAT_LENGTH_OFFSET} bytes +
         * 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_FORMAT_LENGTH_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the index of the
         * {@link SasFileParser.ColumnTextSubheader} subheader
         * whose text block contains the column label:
         * - with the length of {@link SasFileConstants#COLUMN_LABEL_TEXT_SUBHEADER_INDEX_LENGTH} bytes,
         * - at an offset calculated as COLUMN_LABEL_TEXT_SUBHEADER_INDEX_OFFSET bytes +
         * + 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_LABEL_TEXT_SUBHEADER_INDEX_OFFSET = 28L;

        /**
         * For every table column, the sas7bdat file stores the index of the
         * {@link SasFileParser.ColumnTextSubheader} subheader
         * whose text block contains the column label:
         * - with the length of COLUMN_LABEL_TEXT_SUBHEADER_INDEX_LENGTH bytes,
         * - at an offset equal to {@link SasFileConstants#COLUMN_LABEL_TEXT_SUBHEADER_INDEX_OFFSET} bytes +
         * + 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_LABEL_TEXT_SUBHEADER_INDEX_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the column label`s offset (in symbols) from the beginning of
         * the text block where it belongs (see {@link SasFileConstants#COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET}):
         * - with the length of {@link SasFileConstants#COLUMN_LABEL_OFFSET_LENGTH} bytes.
         * - at an offset equal to COLUMN_LABEL_OFFSET_OFFSET bytes + 3 * the size of value types (int or long
         * depending on {@link SasFileConstants#ALIGN_2_VALUE}) from the beginning of
         * the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_LABEL_OFFSET_OFFSET = 30L;

        /**
         * For every table column, the sas7bdat file stores the column label`s offset (in symbols) from the beginning of
         * the text block where it belongs (see {@link SasFileConstants#COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET}):
         * - with the length of COLUMN_LABEL_OFFSET_LENGTH bytes.
         * - at an offset equal to {@link SasFileConstants#COLUMN_LABEL_OFFSET_OFFSET} bytes + 3 * the size
         * of value types(int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from the beginning of the
         * {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_LABEL_OFFSET_LENGTH = 2;

        /**
         * For every table column, the sas7bdat file stores the length of the column label (in symbols):
         * - with the length of {@link SasFileConstants#COLUMN_LABEL_LENGTH_LENGTH} bytes.
         * - at an offset calculated as COLUMN_LABEL_LENGTH_OFFSET bytes +
         * 3 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const long COLUMN_LABEL_LENGTH_OFFSET = 32L;

        /**
         * For every table column, the sas7bdat file stores the length of the column label (in symbols):
         * - with the length of COLUMN_LABEL_LENGTH_LENGTH bytes.
         * - at an offset calculated as {@link SasFileConstants#COLUMN_LABEL_LENGTH_OFFSET} bytes +
         * 3 * the size of value types(int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.FormatAndLabelSubheader} subheader.
         */
        public const int COLUMN_LABEL_LENGTH_LENGTH = 2;

        /**
         * The sas7bdat file stores the offset (in symbols) of the file label from
         * the beginning of the text block of the {@link SasFileParser.ColumnTextSubheader} subheader
         * where it belongs:
         * - with the length of {@link SasFileConstants#FILE_FORMAT_OFFSET_LENGTH} bytes,
         * - at an offset calculated as FILE_FORMAT_OFFSET_OFFSET bytes + 82 * the size of value types
         * (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from the beginning of
         * the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const long FILE_FORMAT_OFFSET_OFFSET = 24L;

        /**
         * The sas7bdat file stores the offset (in symbols) of the file label from
         * the beginning of the text block of the {@link SasFileParser.ColumnTextSubheader}
         * subheader where it belongs:
         * - with the length of FILE_FORMAT_OFFSET_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#FILE_FORMAT_OFFSET_OFFSET} bytes +
         * + 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE}) from
         * the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const int FILE_FORMAT_OFFSET_LENGTH = 2;

        /**
         * The sas7bdat file stores the length of the file label (in symbols):
         * - with the length of {@link SasFileConstants#FILE_FORMAT_LENGTH_LENGTH} bytes.
         * - at an offset calculated as FILE_FORMAT_LENGTH_OFFSET bytes +
         * 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const long FILE_FORMAT_LENGTH_OFFSET = 26L;

        /**
         * The sas7bdat file stores the length of the file label (in symbols):
         * - with the length of FILE_FORMAT_LENGTH_LENGTH bytes.
         * - at an offset calculated as FILE_FORMAT_LENGTH_OFFSET bytes +
         * 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const int FILE_FORMAT_LENGTH_LENGTH = 2;

        /**
         * The sas7bdat file stores the offset of the compression method name (in symbols):
         * - with the length of {@link SasFileConstants#COMPRESSION_METHOD_OFFSET_LENGTH} bytes,
         * - at an offset calculated as COMPRESSION_METHOD_OFFSET bytes +
         * 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const long COMPRESSION_METHOD_OFFSET = 36L;

        /**
         * The sas7bdat file stores the length of the compression method name (in symbols):
         * - with the length of {@link SasFileConstants#COMPRESSION_METHOD_LENGTH_LENGTH} bytes.
         * - at an offset calculated as COMPRESSION_METHOD_LENGTH_OFFSET bytes +
         * 82 * the size of value types(int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const long COMPRESSION_METHOD_LENGTH_OFFSET = 38L;

        /**
         * The sas7bdat file stores the offset of the compression method name (in symbols):
         * - with the length of COMPRESSION_METHOD_OFFSET_LENGTH bytes,
         * - at an offset calculated as {@link SasFileConstants#COMPRESSION_METHOD_OFFSET} bytes +
         * 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const int COMPRESSION_METHOD_OFFSET_LENGTH = 2;

        /**
         * The sas7bdat file stores the length of the compression method name (in symbols):
         * - with the length of COMPRESSION_METHOD_LENGTH_LENGTH bytes.
         * - at an offset calculated as {@link SasFileConstants#COMPRESSION_METHOD_LENGTH_OFFSET} bytes +
         * 82 * the size of value types (int or long depending on {@link SasFileConstants#ALIGN_2_VALUE})
         * from the beginning of the {@link SasFileParser.RowSizeSubheader} subheader.
         */
        public const int COMPRESSION_METHOD_LENGTH_LENGTH = 2;


        /**
         * Accuracy to define whether the numeric result of {@link SasFileParser#convertByteArrayToNumber(byte[])} is
         * a long or double value.
         */
        public const double EPSILON = 1E-14;

        /**
         * Accuracy to define whether the numeric result of {@link SasFileParser#convertByteArrayToNumber(byte[])} is NAN.
         */
        public const double NAN_EPSILON = 1E-300;

        /**
         * The number of milliseconds in a second.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const long MILLISECONDS_IN_SECONDS = 1000L;

        /**
         * The number of seconds in a minute.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int SECONDS_IN_MINUTE = 60;

        /**
         * The number of minutes in an hour.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int MINUTES_IN_HOUR = 60;

        /**
         * The number of hours in a day.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int HOURS_IN_DAY = 24;

        /**
         * The number of days in a non-leap year.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int DAYS_IN_YEAR = 365;

        /**
         * The difference in days between 01/01/1960 (the dates starting point in SAS) and 01/01/1970 (the dates starting
         * point in Java).
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int START_DATES_DAYS_DIFFERENCE = DAYS_IN_YEAR * 10 + 3;

        /**
         * The number of seconds in a day.
         * Deprecated: dates-related functionality moved to date package.
         */
        [Obsolete]
        public const int SECONDS_IN_DAY = SECONDS_IN_MINUTE * MINUTES_IN_HOUR * HOURS_IN_DAY;

        /**
         * The difference in seconds between 01/01/1960 (the dates starting point in SAS) and 01/01/1970 (the dates starting
         * point in Java).
         * Deprecated: no reason to make it public.
         */
        [Obsolete]
        public const int START_DATES_SECONDS_DIFFERENCE = SECONDS_IN_DAY * START_DATES_DAYS_DIFFERENCE;

        /**
         * The offset to the pointer for the bitwise representation of deleted records in MIX pages in x64.
         */
        public const long PAGE_DELETED_POINTER_OFFSET_X64 = 24L;

        /**
         * The offset to the pointer for the bitwise representation of deleted records in MIX pages in x86.
         */
        public const long PAGE_DELETED_POINTER_OFFSET_X86 = 12L;

        /**
         * The length of the deleted record pointer from the beginning of the
         * {@link SasFileConstants#PAGE_DELETED_POINTER_OFFSET_X64}
         * or {@link SasFileConstants#PAGE_DELETED_POINTER_OFFSET_X86}.
         */
        public const int PAGE_DELETED_POINTER_LENGTH = 4;

    }
}