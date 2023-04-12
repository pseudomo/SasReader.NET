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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Soly.Utils;


namespace SasReader
{
    /**
     * This is a class that parses sas7bdat files. When parsing a sas7bdat file, to interact with the library
     * use {@link SasFileReaderImpl} which is a wrapper for SasFileParser. Despite this, SasFileParser
     * is publicly available, it can be instanced via {@link SasFileParser.Builder} and used directly.
     * Public access to the SasFileParser class was added in scope of this issue:
     * @see <a href="https://github.com/epam/parso/issues/51"></a>.
     */
    public sealed class SasFileParser
    {

        /**
         * The mapping of subheader signatures to the corresponding elements in {@link SubheaderIndexes}.
         * Depending on the value at the {@link SasFileConstants#ALIGN_2_OFFSET} offset, signatures take 4 bytes
         * for 32-bit version sas7bdat files and 8 bytes for the 64-bit version files.
         */
        private static readonly Dictionary<long, SubheaderIndexes?> SUBHEADER_SIGNATURE_TO_INDEX = new Dictionary<long, SubheaderIndexes?>()
        {
            { Convert.ToInt32("F7F7F7F7", 16), SubheaderIndexes.ROW_SIZE_SUBHEADER_INDEX },
            { Convert.ToInt32("F6F6F6F6", 16), SubheaderIndexes.COLUMN_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFC00", 16), SubheaderIndexes.SUBHEADER_COUNTS_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFFFD", 16), SubheaderIndexes.COLUMN_TEXT_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFFFF", 16), SubheaderIndexes.COLUMN_NAME_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFFFC", 16), SubheaderIndexes.COLUMN_ATTRIBUTES_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFBFE", 16), SubheaderIndexes.FORMAT_AND_LABEL_SUBHEADER_INDEX},
            { Convert.ToInt32("FFFFFFFE", 16), SubheaderIndexes.COLUMN_LIST_SUBHEADER_INDEX},
            { Convert.ToInt64("00000000F7F7F7F7", 16), SubheaderIndexes.ROW_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("00000000F6F6F6F6", 16), SubheaderIndexes.COLUMN_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("F7F7F7F700000000", 16), SubheaderIndexes.ROW_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("F6F6F6F600000000", 16), SubheaderIndexes.COLUMN_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("F7F7F7F7FFFFFBFE", 16), SubheaderIndexes.ROW_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("F6F6F6F6FFFFFBFE", 16), SubheaderIndexes.COLUMN_SIZE_SUBHEADER_INDEX},
            { Convert.ToInt64("00FCFFFFFFFFFFFF", 16), SubheaderIndexes.SUBHEADER_COUNTS_SUBHEADER_INDEX},
            { Convert.ToInt64("FDFFFFFFFFFFFFFF", 16), SubheaderIndexes.COLUMN_TEXT_SUBHEADER_INDEX},
            //{ Convert.ToInt64("FFFFFFFFFFFFFFFF", 16), SubheaderIndexes.COLUMN_NAME_SUBHEADER_INDEX},
            { Convert.ToInt64("FCFFFFFFFFFFFFFF", 16), SubheaderIndexes.COLUMN_ATTRIBUTES_SUBHEADER_INDEX},
            { Convert.ToInt64("FEFBFFFFFFFFFFFF", 16), SubheaderIndexes.FORMAT_AND_LABEL_SUBHEADER_INDEX},
            { Convert.ToInt64("FEFFFFFFFFFFFFFF", 16), SubheaderIndexes.COLUMN_LIST_SUBHEADER_INDEX}
        };
        /**
         * The mapping of the supported string literals to the compression method they mean.
         */
        private static readonly Dictionary<string, Decompressor> LITERALS_TO_DECOMPRESSOR = new Dictionary<string, Decompressor>()
        {
            { SasFileConstants.COMPRESS_CHAR_IDENTIFYING_STRING, CharDecompressor.INSTANCE },
            { SasFileConstants.COMPRESS_BIN_IDENTIFYING_STRING, BinDecompressor.INSTANCE },
        };

        /**
         * Sanity check on maximum page length.
         */
        private const int MAX_PAGE_LENGTH = 10000000;

        /**
         * The input stream through which the sas7bdat is read.
         */
        private BinaryReader sasFileStream;
        /**
         * The flag of data output in binary or string format.
         */
        private Boolean byteOutput;

        /**
         * Output date type.
         */
        internal OutputDateType outputDateType;

        /**
         * The list of current page data subheaders.
         */
        private List<SubheaderPointer> currentPageDataSubheaderPointers = new List<SubheaderPointer>();
        /**
         * The variable to store all the properties from the sas7bdat file.
         */
        private SasFileProperties sasFileProperties = new SasFileProperties();
        /**
         * The list of text blocks with information about file compression and table columns (name, label, format).
         * Every element corresponds to a {@link SasFileParser.ColumnTextSubheader}. The first text block includes
         * the information about compression.
         */
        private List<byte[]> columnsNamesBytes = new List<byte[]>();
        /**
         * The list of column names.
         */
        private List<String> columnsNamesList = new List<String>();
        /**
         * The list of column types. There can be {@link Number} and {@link String} types.
         */
        private List<Type> columnsTypesList = new List<Type>();
        /**
         * The list of offsets of data in every column inside a row. Used to locate the left border of a cell.
         */
        private List<ulong> columnsDataOffset = new List<ulong>();
        /**
         * The list of data lengths of every column inside a row. Used to locate the right border of a cell.
         */
        private List<int> columnsDataLength = new List<int>();
        /**
         * The list of table columns to store their name, label, and format.
         */
        private List<Column> columns = new List<Column>();
        /**
         * The mapping between elements from {@link SubheaderIndexes} and classes corresponding
         * to each subheader. This is necessary because defining the subheader type being processed is dynamic.
         * Every class has an overridden function that processes the related subheader type.
         */
        private Dictionary<SubheaderIndexes, ProcessingSubheader> subheaderIndexToClass;
        /**
         * Default encoding for output strings.
         */
        private Encoding encoding { get; set; } = Encoding.GetEncoding("us-ascii");
        /**
         * A cache to store the current page of the sas7bdat file. Used to avoid posing buffering requirements
         * to {@link SasFileParser#sasFileStream}.
         */
        private byte[] cachedPage;
        /**
         * The type of the current page when reading the file. If it is other than
         * {@link PageType#PAGE_TYPE_META}, {@link PageType#PAGE_TYPE_MIX}, {@link PageType#PAGE_TYPE_DATA}
         * and {@link PageType#PAGE_TYPE_AMD} page is skipped.
         */
        private int currentPageType;
        /**
         * Number current page blocks.
         */
        private int currentPageBlockCount;
        /**
         * Number current page subheaders.
         */
        private int currentPageSubheadersCount;
        /**
         * The index of the current byte when reading the file.
         */
        private int currentFilePosition;
        /**
         * The index of the current column when reading the file.
         */
        private int currentColumnNumber;
        /**
         * The index of the current row when reading the file.
         */
        private int currentRowInFileIndex;
        /**
         * The index of the current row when reading the page.
         */
        private int currentRowOnPageIndex;
        /**
         * Last read row from sas7bdat file.
         */
        private Object[] currentRow;
        /**
         * True if stream is at the end of file.
         */
        private bool eof;

        /**
         * The offset of the file label from the beginning of the {@link SasFileParser.ColumnTextSubheader} subheader.
         */
        private int fileLabelOffset;

        /**
         * The offset of the compression method from the beginning of the
         * {@link SasFileParser.ColumnTextSubheader} subheader.
         */
        private int compressionMethodOffset;

        /**
         * The length of the compression method which is stored in the
         * {@link SasFileParser.ColumnTextSubheader} subheader
         * with {@link SasFileParser#compressionMethodOffset} offset.
         */
        private int compressionMethodLength;

        /**
         * The length of file label which is stored in the {@link SasFileParser.ColumnTextSubheader} subheader
         * with {@link SasFileParser#fileLabelOffset} offset.
         */
        private int fileLabelLength;

        /**
         * The list of missing column information.
         */
        private List<ColumnMissingInfo> columnMissingInfoList = new List<ColumnMissingInfo>();

        /**
         * An bit representation in String marking deleted records.
         */
        private String deletedMarkers = "";

        /**
         * Instance of SasDateFormatter.
         */
        private SasTemporalFormatter sasTemporalFormatter = new SasTemporalFormatter();

        /**
         * The constructor that reads metadata from the sas7bdat, parses it and puts the results in
         * {@link SasFileParser#sasFileProperties}.
         *
         * @param builder the container with properties information.
         */
        public SasFileParser(Builder builder)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.sasFileStream = new BinaryReader(builder.sasFileStream);
            this.byteOutput = builder.byteOutput;
            this.outputDateType = builder.outputDateType.Value;

            this.subheaderIndexToClass = new Dictionary<SubheaderIndexes, ProcessingSubheader>()
            {
                { SubheaderIndexes.ROW_SIZE_SUBHEADER_INDEX, new RowSizeSubheader(this) },
                { SubheaderIndexes.COLUMN_SIZE_SUBHEADER_INDEX, new ColumnSizeSubheader(this) },
                { SubheaderIndexes.SUBHEADER_COUNTS_SUBHEADER_INDEX, new SubheaderCountsSubheader(this) },
                { SubheaderIndexes.COLUMN_TEXT_SUBHEADER_INDEX, new ColumnTextSubheader(this) },
                { SubheaderIndexes.COLUMN_NAME_SUBHEADER_INDEX, new ColumnNameSubheader(this) },
                { SubheaderIndexes.COLUMN_ATTRIBUTES_SUBHEADER_INDEX, new ColumnAttributesSubheader(this) },
                { SubheaderIndexes.FORMAT_AND_LABEL_SUBHEADER_INDEX, new FormatAndLabelSubheader(this) },
                { SubheaderIndexes.COLUMN_LIST_SUBHEADER_INDEX, new ColumnListSubheader(this) },
                { SubheaderIndexes.DATA_SUBHEADER_INDEX, new DataSubheader(this) },
            };


            getMetadataFromSasFile(builder.encoding);
        }

        private void SetEncodingByName(string encodingName)
        {
            var replaceEncodingNameMap = new Dictionary<string, string>
        {
            {"x-mac-iceland", "x-mac-icelandic"},
            {"x-mac-romania", "x-mac-romanian"},
            {"x-mac-roman", "macintosh"},
            {"x-mac-ukraine", "x-mac-ukrainian"},
            {"x-windows-iso2022jp", "iso-2022-jp"},
            {"x-ibm949", "ks_c_5601-1987" },
            {"x-windows-949", "ks_c_5601-1987" },
            {"x-windows-950", "big5" }
            ///{"x-ibm942",  ??? } not supported
        };

            string adjustedEncodingName = encodingName.ToLower();
            adjustedEncodingName = Regex.Replace(adjustedEncodingName, "^x-mac(.*)", "x-mac-$1");
            if (replaceEncodingNameMap.TryGetValue(adjustedEncodingName, out string replacement))
            {
                adjustedEncodingName = replacement;
            }

            encoding = Encoding.GetEncoding(adjustedEncodingName);
        }

        /**
         * The method that reads and parses metadata from the sas7bdat and puts the results in
         * {@link SasFileParser#sasFileProperties}.
         *
         * @param encoding - builder variable for {@link SasFileParser#encoding} variable.
         * @throws IOException - appears if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private void getMetadataFromSasFile(String encoding)
        {
            bool endOfMetadata = false;
            processSasFileHeader(encoding);
            cachedPage = new byte[sasFileProperties.getPageLength()];
            while (!endOfMetadata)
            {
                try
                {
                    sasFileStream.Read(cachedPage, 0, sasFileProperties.getPageLength());
                }
                catch (EndOfStreamException)
                {
                    eof = true;
                    break;
                }
                endOfMetadata = processSasFilePageMeta();
            }
        }

        /**
         * The method to read and parse metadata from the sas7bdat file`s header in {@link SasFileParser#sasFileProperties}.
         * After reading is complete, {@link SasFileParser#currentFilePosition} is set to the end of the header whose length
         * is stored at the {@link SasFileConstants#HEADER_SIZE_OFFSET} offset.
         *
         * @param builderEncoding - builder variable for {@link SasFileParser#encoding} variable.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private void processSasFileHeader(String builderEncoding)
        {
            int align1 = 0;
            int align2 = 0;

            long[] offsetForAlign = { SasFileConstants.ALIGN_1_OFFSET, SasFileConstants.ALIGN_2_OFFSET };
            int[] lengthForAlign = { SasFileConstants.ALIGN_1_LENGTH, SasFileConstants.ALIGN_2_LENGTH };
            List<byte[]> varsForAlign = getBytesFromFile(offsetForAlign, lengthForAlign);

            if (varsForAlign[0][0] == SasFileConstants.U64_BYTE_CHECKER_VALUE)
            {
                align2 = SasFileConstants.ALIGN_2_VALUE;
                sasFileProperties.setU64(true);
            }

            if (varsForAlign[1][0] == SasFileConstants.ALIGN_1_CHECKER_VALUE)
            {
                align1 = SasFileConstants.ALIGN_1_VALUE;
            }

            int totalAlign = align1 + align2;

            long[] offset = {SasFileConstants.ENDIANNESS_OFFSET, SasFileConstants.ENCODING_OFFSET, SasFileConstants.DATASET_OFFSET, SasFileConstants.FILE_TYPE_OFFSET,
                SasFileConstants.DATE_CREATED_OFFSET + align1, SasFileConstants.DATE_MODIFIED_OFFSET + align1, SasFileConstants.HEADER_SIZE_OFFSET + align1,
                SasFileConstants.PAGE_SIZE_OFFSET + align1, SasFileConstants.PAGE_COUNT_OFFSET + align1, SasFileConstants.SAS_RELEASE_OFFSET + totalAlign,
                SasFileConstants.SAS_SERVER_TYPE_OFFSET + totalAlign, SasFileConstants.OS_VERSION_NUMBER_OFFSET + totalAlign,
                SasFileConstants.OS_MAKER_OFFSET + totalAlign, SasFileConstants.OS_NAME_OFFSET + totalAlign};
            int[] length = {SasFileConstants.ENDIANNESS_LENGTH, SasFileConstants.ENCODING_LENGTH, SasFileConstants.DATASET_LENGTH, SasFileConstants.FILE_TYPE_LENGTH, SasFileConstants.DATE_CREATED_LENGTH,
                SasFileConstants.DATE_MODIFIED_LENGTH, SasFileConstants.HEADER_SIZE_LENGTH, SasFileConstants.PAGE_SIZE_LENGTH, SasFileConstants.PAGE_COUNT_LENGTH + align2,
                SasFileConstants.SAS_RELEASE_LENGTH, SasFileConstants.SAS_SERVER_TYPE_LENGTH, SasFileConstants.OS_VERSION_NUMBER_LENGTH, SasFileConstants.OS_MAKER_LENGTH, SasFileConstants.OS_NAME_LENGTH};
            List<byte[]> vars = getBytesFromFile(offset, length);

            sasFileProperties.setEndianness(vars[0][0]);
            if (!isSasFileValid())
            {
                throw new IOException(ParserMessageConstants.FILE_NOT_VALID);
            }

            String fileEncoding = SasFileConstants.SAS_CHARACTER_ENCODINGS[vars[1][0]];
            if (builderEncoding != null)
            {
                SetEncodingByName(builderEncoding);
            }
            else
            {
                if (fileEncoding != null)
                {
                    SetEncodingByName(fileEncoding);
                }
            }
            sasFileProperties.setEncoding(fileEncoding);
            sasFileProperties.setName(bytesToString(vars[2]).Trim());
            sasFileProperties.setFileType(bytesToString(vars[3]).Trim());
            sasFileProperties.setDateCreated(bytesToDateTime(vars[4]).Value);
            sasFileProperties.setDateModified(bytesToDateTime(vars[5]).Value);
            sasFileProperties.setHeaderLength(bytesToInt(vars[6]));
            int pageLength = bytesToInt(vars[7]);
            if (pageLength > MAX_PAGE_LENGTH)
            {
                throw new IOException("Page limit ("
                        + pageLength + ") exceeds maximum: " + MAX_PAGE_LENGTH);
            }
            sasFileProperties.setPageLength(pageLength);
            sasFileProperties.setPageCount(bytesToLong(vars[8]));
            sasFileProperties.setSasRelease(bytesToString(vars[9]).Trim());
            sasFileProperties.setServerType(bytesToString(vars[10]).Trim());
            sasFileProperties.setOsType(bytesToString(vars[11]).Trim());
            if (vars[13][0] != 0)
            {
                sasFileProperties.setOsName(bytesToString(vars[13]).Trim());
            }
            else
            {
                sasFileProperties.setOsName(bytesToString(vars[12]).Trim());
            }

            if (sasFileStream != null)
            {
                skipBytes(sasFileProperties.getHeaderLength() - currentFilePosition);
                currentFilePosition = 0;
            }
        }

        /**
         * Skip specified number of bytes of data from the input stream,
         * or fail if there are not enough left.
         *
         * @param numberOfBytesToSkip the number of bytes to skip
         * @throws IOException if the number of bytes skipped was incorrect
         */
        private void skipBytes(long numberOfBytesToSkip)
        {
            sasFileStream.ReadBytes(Convert.ToInt32(numberOfBytesToSkip));

            //long remainBytes = numberOfBytesToSkip;
            //long readBytes;
            //while (remainBytes > 0)
            //{

            //    readBytes = sasFileStream.Read(SKIP_BYTE_BUFFER, 0,
            //            (int)Math.Min(remainBytes, SKIP_BYTE_BUFFER.Length));
            //    if (readBytes < 0)
            //    { // EOF
            //        break;
            //    }


            //    remainBytes -= readBytes;
            //}

            //long actuallySkipped = numberOfBytesToSkip - remainBytes;

            //if (actuallySkipped != numberOfBytesToSkip)
            //{
            //    throw new IOException("Expected to skip " + numberOfBytesToSkip
            //            + " to the end of the header, but skipped " + actuallySkipped + " instead.");
            //}
        }

        /**
         * The method to validate sas7bdat file. If sasFileProperties contains an encoding value other than
         * {@link SasFileConstants#LITTLE_ENDIAN_CHECKER} or {@link SasFileConstants#BIG_ENDIAN_CHECKER}
         * the file is considered invalid.
         *
         * @return true if the value of encoding equals to {@link SasFileConstants#LITTLE_ENDIAN_CHECKER}
         * or {@link SasFileConstants#BIG_ENDIAN_CHECKER}
         */
        private bool isSasFileValid()
        {
            return sasFileProperties.getEndianness() == SasFileConstants.LITTLE_ENDIAN_CHECKER
                    || sasFileProperties.getEndianness() == SasFileConstants.BIG_ENDIAN_CHECKER;
        }

        /**
         * The method to read pages of the sas7bdat file. First, the method reads the page type
         * (at the {@link SasFileConstants#PAGE_TYPE_OFFSET} offset), the number of rows on the page
         * (at the {@link SasFileConstants#BLOCK_COUNT_OFFSET} offset), and the number of subheaders
         * (at the {@link SasFileConstants#SUBHEADER_COUNT_OFFSET} offset). Then, depending on the page type,
         * the method calls the function to process the page.
         *
         * @return true if all metadata is read.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private bool processSasFilePageMeta()
        {
            int bitOffset = sasFileProperties.isU64() ? SasFileConstants.PAGE_BIT_OFFSET_X64 : SasFileConstants.PAGE_BIT_OFFSET_X86;
            readPageHeader();
            List<SubheaderPointer> subheaderPointers = new List<SubheaderPointer>();
            if (PageType.PAGE_TYPE_META.Contains(currentPageType) || PageType.PAGE_TYPE_MIX.Contains(currentPageType))
            {
                processPageMetadata(bitOffset, subheaderPointers);
            }
            return PageType.PAGE_TYPE_DATA.Contains(currentPageType) || PageType.PAGE_TYPE_MIX.Contains(currentPageType)
                    || currentPageDataSubheaderPointers.Count != 0;
        }

        /**
         * The method to parse and read metadata of a page, used for pages of the {@link PageType#PAGE_TYPE_META}
         * and {@link PageType#PAGE_TYPE_MIX} types. The method goes through subheaders, one by one, and calls
         * the processing functions depending on their signatures.
         *
         * @param bitOffset         the offset from the beginning of the page at which the page stores its metadata.
         * @param subheaderPointers the number of subheaders on the page.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} string is impossible.
         */
        private void processPageMetadata(int bitOffset, List<SubheaderPointer> subheaderPointers)

        {
            subheaderPointers.Clear();
            for (int subheaderPointerIndex = 0; subheaderPointerIndex < currentPageSubheadersCount; subheaderPointerIndex++)
            {

                SubheaderPointer currentSubheaderPointer = processSubheaderPointers((long)bitOffset
                        + SasFileConstants.SUBHEADER_POINTERS_OFFSET, subheaderPointerIndex);
                subheaderPointers.Add(currentSubheaderPointer);
                if (currentSubheaderPointer.compression != SasFileConstants.TRUNCATED_SUBHEADER_ID)
                {
                    long subheaderSignature = readSubheaderSignature(currentSubheaderPointer.offset);
                    SubheaderIndexes? subheaderIndex = chooseSubheaderClass(subheaderSignature,
                            currentSubheaderPointer.compression, currentSubheaderPointer.type);
                    if (subheaderIndex != null)
                    {
                        if (subheaderIndex != SubheaderIndexes.DATA_SUBHEADER_INDEX)
                        {
                            subheaderIndexToClass[subheaderIndex.Value].processSubheader(
                                    subheaderPointers[subheaderPointerIndex].offset,
                                    subheaderPointers[subheaderPointerIndex].length);
                        }
                        else
                        {
                            currentPageDataSubheaderPointers.Add(subheaderPointers[subheaderPointerIndex]);
                        }
                    }
                    else
                    {
                        Console.WriteLine(ParserMessageConstants.UNKNOWN_SUBHEADER_SIGNATURE); //LOGGER.debug(UNKNOWN_SUBHEADER_SIGNATURE);
                    }
                }


            }
        }

        /**
         * The function to read a subheader signature at the offset known from its ({@link SubheaderPointer}).
         *
         * @param subheaderPointerOffset the offset at which the subheader is located.
         * @return - the subheader signature to search for in the {@link SasFileParser#SUBHEADER_SIGNATURE_TO_INDEX}
         * mapping later.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private long readSubheaderSignature(long subheaderPointerOffset)
        {
            int intOrLongLength = sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
            long[] subheaderOffsetMass = { subheaderPointerOffset };
            int[] subheaderLengthMass = { intOrLongLength };
            List<byte[]> subheaderSignatureMass = getBytesFromFile(subheaderOffsetMass, subheaderLengthMass);
            return bytesToLong(subheaderSignatureMass[0]);
        }

        /**
         * The function to determine the subheader type by its signature, {@link SubheaderPointer#compression},
         * and {@link SubheaderPointer#type}.
         *
         * @param subheaderSignature the subheader signature to search for in the
         *                           {@link SasFileParser#SUBHEADER_SIGNATURE_TO_INDEX} mapping
         * @param compression        the type of subheader compression ({@link SubheaderPointer#compression})
         * @param type               the subheader type ({@link SubheaderPointer#type})
         * @return an element from the  {@link SubheaderIndexes} enumeration that defines the type of
         * the current subheader
         */
        private SubheaderIndexes? chooseSubheaderClass(long subheaderSignature, int compression, int type)
        {
            var subheaderIndex = SUBHEADER_SIGNATURE_TO_INDEX.GetValueOrDefault(subheaderSignature);

            if (sasFileProperties.isCompressed() && subheaderIndex == null && (compression == SasFileConstants.COMPRESSED_SUBHEADER_ID
                    || compression == 0) && type == SasFileConstants.COMPRESSED_SUBHEADER_TYPE)
            {
                subheaderIndex = SubheaderIndexes.DATA_SUBHEADER_INDEX;
            }
            return subheaderIndex;
        }

        /**
         * The function to read the pointer with the subheaderPointerIndex index from the list of {@link SubheaderPointer}
         * located at the subheaderPointerOffset offset.
         *
         * @param subheaderPointerOffset the offset before the list of {@link SubheaderPointer}.
         * @param subheaderPointerIndex  the index of the subheader pointer being read.
         * @return the subheader pointer.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private SubheaderPointer processSubheaderPointers(long subheaderPointerOffset, int subheaderPointerIndex)

        {
            int intOrLongLength = sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
            int subheaderPointerLength = sasFileProperties.isU64() ? SasFileConstants.SUBHEADER_POINTER_LENGTH_X64
                    : SasFileConstants.SUBHEADER_POINTER_LENGTH_X86;
            long totalOffset = subheaderPointerOffset + subheaderPointerLength * ((long)subheaderPointerIndex);
            long[] offset = {
        totalOffset, totalOffset + intOrLongLength, totalOffset + 2L * intOrLongLength,
                totalOffset + 2L * intOrLongLength + 1};
            int[] length = { intOrLongLength, intOrLongLength, 1, 1 };
            List<byte[]> vars = getBytesFromFile(offset, length);

            long subheaderOffset = bytesToLong(vars[0]);
            long subheaderLength = bytesToLong(vars[1]);
            byte subheaderCompression = vars[2][0];
            byte subheaderType = vars[3][0];

            return new SubheaderPointer(subheaderOffset, subheaderLength, subheaderCompression, subheaderType);
        }

        /**
         * Match the input string against the known compression methods.
         *
         * @param compressionMethod the name of the compression method, like "SASYZCRL"
         * @return true if the method is matched or false otherwise.
         */
        private bool matchCompressionMethod(String compressionMethod)
        {
            if (compressionMethod == null)
            {
                return false;
            }
            if (LITERALS_TO_DECOMPRESSOR.ContainsKey(compressionMethod))
            {
                return true;
            }

            return false;
        }

        /**
         * The function to return the index of the current row when reading the file sas7bdat file.
         *
         * @return current row index
         */
        public int getOffset()
        {
            return currentRowInFileIndex;
        }

        /**
         * The function to read and process all columns of next row from current sas7bdat file.
         *
         * @return the object array containing elements of current row.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        public Object[] readNext()
        {
            return readNext(null);
        }

        /**
         * The function to read and process specified columns of next row from current sas7bdat file.
         *
         * @param columnNames list of column names which should be processed, if null then all columns are processed.
         * @return the object array containing elements of current row.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        public object[] readNext(List<string> columnNames)
        {
            if (currentRowInFileIndex++ >= sasFileProperties.getRowCount() || eof)
            {
                return null;
            }
            int bitOffset = sasFileProperties.isU64() ? SasFileConstants.PAGE_BIT_OFFSET_X64 : SasFileConstants.PAGE_BIT_OFFSET_X86;
            currentRow = null;
            switch (currentPageType)
            {
                case SasFileConstants.PAGE_META_TYPE_1:
                case SasFileConstants.PAGE_META_TYPE_2:
                case SasFileConstants.PAGE_CMETA_TYPE:
                    if (currentPageDataSubheaderPointers.Count == 0 && currentPageType == SasFileConstants.PAGE_CMETA_TYPE)
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    SubheaderPointer currentSubheaderPointer =
                            currentPageDataSubheaderPointers[currentRowOnPageIndex++];
                    ((ProcessingDataSubheader)subheaderIndexToClass[SubheaderIndexes.DATA_SUBHEADER_INDEX]).processSubheader(currentSubheaderPointer.offset, currentSubheaderPointer.length, columnNames);
                    if (currentRowOnPageIndex == currentPageDataSubheaderPointers.Count)
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    break;
                case SasFileConstants.PAGE_MIX_TYPE_1:
                    // Mix pages that contain all valid records
                    int subheaderPointerLength = sasFileProperties.isU64() ? SasFileConstants.SUBHEADER_POINTER_LENGTH_X64
                            : SasFileConstants.SUBHEADER_POINTER_LENGTH_X86;
                    int alignCorrection = (bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + currentPageSubheadersCount
                            * subheaderPointerLength) % SasFileConstants.BITS_IN_BYTE;

                    currentRow = processByteArrayWithData(bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + alignCorrection
                            + currentPageSubheadersCount * subheaderPointerLength + currentRowOnPageIndex++
                            * sasFileProperties.getRowLength(), sasFileProperties.getRowLength(), columnNames);

                    if (currentRowOnPageIndex == Math.Min(sasFileProperties.getRowCount(),
                            sasFileProperties.getMixPageRowCount()))
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    break;
                case SasFileConstants.PAGE_MIX_TYPE_2:
                    // Mix pages that contain valid and deleted records
                    if (deletedMarkers.Equals(""))
                    {
                        readDeletedInfo();
                    }
                    subheaderPointerLength = sasFileProperties.isU64() ? SasFileConstants.SUBHEADER_POINTER_LENGTH_X64
                            : SasFileConstants.SUBHEADER_POINTER_LENGTH_X86;
                    alignCorrection = (bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + currentPageSubheadersCount
                            * subheaderPointerLength) % SasFileConstants.BITS_IN_BYTE;

                    if (deletedMarkers[currentRowOnPageIndex] == '0')
                    {
                        currentRow = processByteArrayWithData(bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + alignCorrection
                                + currentPageSubheadersCount * subheaderPointerLength + currentRowOnPageIndex++
                                * sasFileProperties.getRowLength(), sasFileProperties.getRowLength(), columnNames);
                    }
                    else
                    {
                        currentRowOnPageIndex++;
                    }
                    if (currentRowOnPageIndex == Math.Min(sasFileProperties.getRowCount(),
                            sasFileProperties.getMixPageRowCount()))
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    break;
                case SasFileConstants.PAGE_DATA_TYPE:
                    // Data pages that contain all valid records
                    currentRow = processByteArrayWithData(bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + currentRowOnPageIndex++
                            * sasFileProperties.getRowLength(), sasFileProperties.getRowLength(), columnNames);
                    if (currentRowOnPageIndex == currentPageBlockCount)
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    break;
                case SasFileConstants.PAGE_DATA_TYPE_2:
                    // Data pages that contain valid and deleted records
                    if (deletedMarkers.Equals(""))
                    {
                        readDeletedInfo();
                    }
                    if (deletedMarkers[currentRowOnPageIndex] == '0')
                    {
                        currentRow = processByteArrayWithData(bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET
                                + currentRowOnPageIndex++
                                * sasFileProperties.getRowLength(), sasFileProperties.getRowLength(), columnNames);
                    }
                    else
                    {
                        currentRowOnPageIndex++;
                    }
                    if (currentRowOnPageIndex == currentPageBlockCount)
                    {
                        readNextPage();
                        currentRowOnPageIndex = 0;
                    }
                    break;
                default:
                    break;
            }
            if (currentRow == null)
            {
                return null;
            }
            return currentRow;//Arrays.copyOf(currentRow, currentRow.length);
        }

        /**
         * The method to read next page from sas7bdat file and put it into {@link SasFileParser#cachedPage}. If this page
         * has {@link PageType#PAGE_TYPE_META} type method process it's subheaders. Method skips page with type other
         * than {@link PageType#PAGE_TYPE_META}, {@link PageType#PAGE_TYPE_MIX} or {@link PageType#PAGE_TYPE_DATA}
         * and reads next.
         *
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private void readNextPage()
        {
            deletedMarkers = "";
            processNextPage();
            while (!PageType.PAGE_TYPE_META.Contains(currentPageType) && !PageType.PAGE_TYPE_MIX.Contains(currentPageType)
                    && !PageType.PAGE_TYPE_DATA.Contains(currentPageType))
            {
                if (eof)
                {
                    return;
                }
                processNextPage();
            }
        }

        /**
         * Put next page to cache and read it's header.
         *
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} string is impossible.
         */
        private void processNextPage()
        {
            int bitOffset = sasFileProperties.isU64() ? SasFileConstants.PAGE_BIT_OFFSET_X64 : SasFileConstants.PAGE_BIT_OFFSET_X86;
            currentPageDataSubheaderPointers.Clear();

            int bytesRead = sasFileStream.Read(cachedPage, 0, sasFileProperties.getPageLength());

            if (bytesRead < sasFileProperties.getPageLength())
            {
                eof = true;
                return;
            }

            readPageHeader();
            if (PageType.PAGE_TYPE_META.Contains(currentPageType) || PageType.PAGE_TYPE_AMD.Contains(currentPageType)
                    || PageType.PAGE_TYPE_MIX.Contains(currentPageType))
            {
                List<SubheaderPointer> subheaderPointers = new List<SubheaderPointer>();
                processPageMetadata(bitOffset, subheaderPointers);
                readDeletedInfo();
                if (PageType.PAGE_TYPE_AMD.Contains(currentPageType))
                {
                    processMissingColumnInfo();
                }
            }
        }

        /**
         * The function to process missing column information.
         *
         * @throws UnsupportedEncodingException when unknown encoding.
         */
        private void processMissingColumnInfo()
        {
            foreach (ColumnMissingInfo columnMissingInfo in columnMissingInfoList)
            {
                String missedInfo = bytesToString(columnsNamesBytes[columnMissingInfo.getTextSubheaderIndex()],
                        columnMissingInfo.getOffset(), columnMissingInfo.getLength());
                Column column = columns[columnMissingInfo.getColumnId()];
                switch (columnMissingInfo.getMissingInfoType())
                {
                    case ColumnMissingInfo.MissingInfoType.NAME:
                        column.setName(missedInfo);
                        break;
                    case ColumnMissingInfo.MissingInfoType.FORMAT:
                        column.setFormat(new ColumnFormat(missedInfo));
                        break;
                    case ColumnMissingInfo.MissingInfoType.LABEL:
                        column.setLabel(missedInfo);
                        break;
                    default:
                        break;
                }
            }
        }

        /**
         * The method to read page metadata and store it in {@link SasFileParser#currentPageType},
         * {@link SasFileParser#currentPageBlockCount} and {@link SasFileParser#currentPageSubheadersCount}.
         *
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} string is impossible.
         */
        private void readPageHeader()
        {
            int bitOffset = sasFileProperties.isU64() ? SasFileConstants.PAGE_BIT_OFFSET_X64 : SasFileConstants.PAGE_BIT_OFFSET_X86;
            long[] offset = { bitOffset + SasFileConstants.PAGE_TYPE_OFFSET, bitOffset + SasFileConstants.BLOCK_COUNT_OFFSET, bitOffset + SasFileConstants.SUBHEADER_COUNT_OFFSET };
            int[] length = { SasFileConstants.PAGE_TYPE_LENGTH, SasFileConstants.BLOCK_COUNT_LENGTH, SasFileConstants.SUBHEADER_COUNT_LENGTH };
            List<byte[]> vars = getBytesFromFile(offset, length);

            currentPageType = bytesToShort(vars[0]);
            currentPageBlockCount = bytesToShort(vars[1]);
            currentPageSubheadersCount = bytesToShort(vars[2]);
        }

        /**
         * The function to get the deleted record pointers.
         *
         * @throws IOException if reading pointers is impossible.
         */
        private void readDeletedInfo()
        {
            long deletedPointerOffset;
            int subheaderPointerLength;
            int bitOffset;
            if (sasFileProperties.isU64())
            {
                deletedPointerOffset = SasFileConstants.PAGE_DELETED_POINTER_OFFSET_X64;
                subheaderPointerLength = SasFileConstants.SUBHEADER_POINTER_LENGTH_X64;
                bitOffset = SasFileConstants.PAGE_BIT_OFFSET_X64 + 8;
            }
            else
            {
                deletedPointerOffset = SasFileConstants.PAGE_DELETED_POINTER_OFFSET_X86;
                subheaderPointerLength = SasFileConstants.SUBHEADER_POINTER_LENGTH_X86;
                bitOffset = SasFileConstants.PAGE_BIT_OFFSET_X86 + 8;
            }
            int alignCorrection = (bitOffset + SasFileConstants.SUBHEADER_POINTERS_OFFSET + currentPageSubheadersCount
                    * subheaderPointerLength) % SasFileConstants.BITS_IN_BYTE;
            List<byte[]> vars = getBytesFromFile(new long[] { deletedPointerOffset },
                        new int[] { SasFileConstants.PAGE_DELETED_POINTER_LENGTH });

            long currentPageDeletedPointer = bytesToInt(vars[0]);
            long deletedMapOffset = bitOffset + currentPageDeletedPointer + alignCorrection
                    + (currentPageSubheadersCount * subheaderPointerLength)
                    + ((currentPageBlockCount - currentPageSubheadersCount) * sasFileProperties.getRowLength());
            List<byte[]> bytes = getBytesFromFile(new long[] { deletedMapOffset },
                new int[] { (int)Math.Ceiling((currentPageBlockCount - currentPageSubheadersCount) / 8.0) });

            byte[] x = bytes[0];
            foreach (byte b in x)
            {
                deletedMarkers += String.Format("{0,8}", Convert.ToString(b & 0xFF, 2)).Replace(" ", "0");
            }
        }

        /**
         * The function to convert the array of bytes that stores the data of a row into an array of objects.
         * Each object corresponds to a table cell.
         *
         * @param rowOffset   - the offset of the row in cachedPage.
         * @param rowLength   - the length of the row.
         * @param columnNames - list of column names which should be processed.
         * @return the array of objects storing the data of the row.
         */
        private Object[] processByteArrayWithData(long rowOffset, long rowLength, List<String> columnNames)
        {
            Object[] rowElements;
            if (columnNames != null)
            {
                rowElements = new Object[columnNames.Count];
            }
            else
            {
                rowElements = new Object[(int)sasFileProperties.getColumnsCount()];
            }
            byte[] source;
            int offset;
            if (sasFileProperties.isCompressed() && rowLength < sasFileProperties.getRowLength())
            {
                Decompressor decompressor = LITERALS_TO_DECOMPRESSOR[sasFileProperties.getCompressionMethod()];
                source = decompressor.decompressRow((int)rowOffset, (int)rowLength,
                        (int)sasFileProperties.getRowLength(), cachedPage);
                offset = 0;
            }
            else
            {
                source = cachedPage;
                offset = (int)rowOffset;
            }

            for (int currentColumnIndex = 0; currentColumnIndex < sasFileProperties.getColumnsCount()
                    && columnsDataLength[currentColumnIndex] != 0; currentColumnIndex++)
            {
                if (columnNames == null)
                {
                    rowElements[currentColumnIndex] = processElement(source, offset, currentColumnIndex);
                }
                else
                {
                    String name = columns[currentColumnIndex].getName();
                    if (columnNames.Contains(name))
                    {
                        rowElements[columnNames.IndexOf(name)] = processElement(source, offset, currentColumnIndex);
                    }
                }
            }

            return rowElements;
        }

        /**
         * The function to process element of row.
         *
         * @param source             an array of bytes containing required data.
         * @param offset             the offset in source of required data.
         * @param currentColumnIndex index of the current element.
         * @return object storing the data of the element.
         */
        private Object processElement(byte[] source, int offset, int currentColumnIndex)
        {
            byte[] temp;
            int length = columnsDataLength[currentColumnIndex];
            if (columns[currentColumnIndex].getType().IsNumeric())
            {
                temp = source[
                    (offset + (int)(long)columnsDataOffset[currentColumnIndex])..(offset + (int)(long)columnsDataOffset[currentColumnIndex] + length)
                ];
                if (columnsDataLength[currentColumnIndex] <= 2)
                {
                    return bytesToShort(temp);
                }
                else
                {
                    if (string.IsNullOrEmpty(columns[currentColumnIndex].getFormat().getName()))
                    {
                        return convertByteArrayToNumber(temp);
                    }
                    else
                    {
                        ColumnFormat columnFormat = columns[currentColumnIndex].getFormat();
                        String sasDateFormat = columnFormat.getName();
                        if (SasTemporalFormatter.isDateTimeFormat(sasDateFormat))
                        {
                            return bytesToDateTime(temp, outputDateType, columnFormat);
                        }
                        else if (SasTemporalFormatter.isDateFormat(sasDateFormat))
                        {
                            return bytesToDate(temp, outputDateType, columnFormat);
                        }
                        else if (SasTemporalFormatter.isTimeFormat(sasDateFormat))
                        {
                            return bytesToTime(temp, outputDateType, columnFormat);
                        }
                        else
                        {
                            return convertByteArrayToNumber(temp);
                        }
                    }
                }
            }
            else
            {
                byte[] bytes = trimBytesArray(source, offset + Convert.ToInt32(columnsDataOffset[currentColumnIndex]), length);
                if (byteOutput)
                {
                    return bytes;
                }
                else
                {
                    try
                    {
                        return (bytes == null ? null : bytesToString(bytes));
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /**
         * The function to read the list of bytes arrays from the sas7bdat file. The array of offsets and the array of
         * lengths serve as input data that define the location and number of bytes the function must read.
         *
         * @param offset the array of offsets.
         * @param length the array of lengths.
         * @return the list of bytes arrays.
         * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
         */
        private List<byte[]> getBytesFromFile(long[] offset, int[] length)
        {
            List<byte[]> vars = new List<byte[]>();
            if (cachedPage == null)
            {
                for (int i = 0; i < offset.Length; i++)
                {
                    byte[] temp = new byte[length[i]];
                    skipBytes(offset[i] - currentFilePosition);

                    int bytesRead = sasFileStream.Read(temp, 0, length[i]);
                    if (bytesRead < length[i])
                    {
                        eof = true;
                    }

                    currentFilePosition = (int)offset[i] + length[i];
                    vars.Add(temp);
                }
            }
            else
            {
                for (int i = 0; i < offset.Length; i++)
                {
                    if (cachedPage.Length < offset[i])
                    {
                        throw new IOException(ParserMessageConstants.EMPTY_INPUT_STREAM);
                    }
                    vars.Add(cachedPage[
                        ((int)offset[i])..((int)offset[i] + length[i])
                    ]);
                }
            }
            return vars;
        }

        /**
         * The function to convert a bytes array into a number (int or long depending on the value located at
         * the {@link SasFileConstants#ALIGN_2_OFFSET} offset).
         *
         * @param byteBuffer the long value represented by a bytes array.
         * @return a long value. If the number was stored as int, then after conversion it is converted to long
         * for convenience.
         */
        private long correctLongProcess(ByteArray byteBuffer)
        {
            if (sasFileProperties.isU64())
            {
                return (long)byteBuffer.ReadU64();
            }
            else
            {
                return (long)byteBuffer.ReadI32();
            }
        }

        /**
         * The function to convert an array of bytes with any order of bytes into {@link ByteBuffer}.
         * {@link ByteBuffer} has the order of bytes defined in the file located at the
         * {@link SasFileConstants#ALIGN_2_OFFSET} offset.
         * Later the parser converts result {@link ByteBuffer} into a number.
         *
         * @param data the input array of bytes with the little-endian or big-endian order.
         * @return {@link ByteBuffer} with the order of bytes defined in the file located at
         * the {@link SasFileConstants#ALIGN_2_OFFSET} offset.
         */
        private ByteArray byteArrayToByteBuffer(byte[] data)
        {
            Endianness endianness;

            if (sasFileProperties.getEndianness() == 0)
            {
                endianness = Endianness.BigEndian;
            }
            else
            {
                endianness = Endianness.LittleEndian;
            }

            ByteArray byteBuffer = new ByteArray(data, endianness);

            return byteBuffer;
        }

        /**
         * The function to convert an array of bytes into a number. The result can be double or long values.
         * The numbers are stored in the IEEE 754 format. A number is considered long if the difference between the whole
         * number and its integer part is less than {@link SasFileConstants#EPSILON}.
         *
         * @param mass the number represented by an array of bytes.
         * @return number of a long or double type.
         */
        private object convertByteArrayToNumber(byte[] mass)
        {
            double resultDouble = bytesToDouble(mass);

            if (Double.IsNaN(resultDouble) || (resultDouble < SasFileConstants.NAN_EPSILON && resultDouble > 0))
            {
                return null;
            }

            return resultDouble;
        }

        /**
         * The function to convert an array of bytes into a numeral of the {@link Short} type.
         * For convenience, the resulting number is converted into the int type.
         *
         * @param bytes a long number represented by an array of bytes.
         * @return a number of the int type that is the conversion result.
         */
        private int bytesToShort(byte[] bytes)
        {
            return byteArrayToByteBuffer(bytes).ReadI16();
        }

        /**
         * The function to convert an array of bytes into an int number.
         *
         * @param bytes a long number represented by an array of bytes.
         * @return a number of the int type that is the conversion result.
         */
        private int bytesToInt(byte[] bytes)
        {
            return byteArrayToByteBuffer(bytes).ReadI32();
        }

        /**
         * The function to convert an array of bytes into a long number.
         *
         * @param bytes a long number represented by an array of bytes.
         * @return a number of the long type that is the conversion result.
         */
        private long bytesToLong(byte[] bytes)
        {
            return correctLongProcess(byteArrayToByteBuffer(bytes));
        }

        /**
         * The function to convert an array of bytes into a string.
         *
         * @param bytes a string represented by an array of bytes.
         * @return the conversion result string.
         * @throws UnsupportedEncodingException when unknown encoding.
         */
        private String bytesToString(byte[] bytes)
        {
            var nullIndex = Array.IndexOf(bytes, (byte)0);
            nullIndex = (nullIndex == -1) ? bytes.Length : nullIndex;
            return encoding.GetString(bytes, 0, nullIndex);
        }

        /**
         * The function to convert a sub-range of an array of bytes into a string.
         *
         * @param bytes  a string represented by an array of bytes.
         * @param offset the initial offset
         * @param length the length
         * @return the conversion result string.
         * @throws UnsupportedEncodingException    when unknown encoding.
         * @throws StringIndexOutOfBoundsException when invalid offset and/or length.
         */
        private String bytesToString(byte[] bytes, int offset, int length)
        {
            return encoding.GetString(bytes, offset, length);
        }

        /**
         * The function to convert an array of bytes that stores the number of seconds
         * elapsed from 01/01/1960 into a variable of the {@link Date} type.
         *
         * @param bytes an array of bytes that stores the type.
         * @return a variable of the {@link Date} type.
         */
        private DateTime? bytesToDateTime(byte[] bytes)
        {
            double doubleSeconds = bytesToDouble(bytes);
            if (double.IsNaN(doubleSeconds))
            {
                return null;
            }
            else
            {
                return sasTemporalFormatter.formatSasSecondsAsJavaDate(doubleSeconds);
            }
        }

        /**
         * The function to convert an array of bytes that stores the number of seconds
         * elapsed from 01/01/1960 into the date represented according to outputDateType.
         *
         * @param bytes          an array of bytes that stores the type.
         * @param outputDateType type of the date formatting
         * @param columnFormat   SAS date format
         * @return datetime representation
         */
        private Object bytesToDateTime(byte[] bytes, OutputDateType outputDateType, ColumnFormat columnFormat)
        {
            double doubleSeconds = bytesToDouble(bytes);
            return sasTemporalFormatter.formatSasDateTime(doubleSeconds, outputDateType,
                    columnFormat.getName(), columnFormat.getWidth(), columnFormat.getPrecision());
        }

        /**
         * The function to convert an array of bytes that stores the number of seconds
         * since midnight into the date represented according to outputDateType.
         *
         * @param bytes          an array of bytes that stores the type.
         * @param outputDateType type of the date formatting
         * @param columnFormat   SAS date format
         * @return time representation
         */
        private Object bytesToTime(byte[] bytes, OutputDateType outputDateType, ColumnFormat columnFormat)
        {
            double doubleSeconds = bytesToDouble(bytes);
            return sasTemporalFormatter.formatSasTime(doubleSeconds, outputDateType,
                    columnFormat.getName(), columnFormat.getWidth(), columnFormat.getPrecision());
        }


        /**
         * The function to convert an array of bytes that stores the number of days
         * elapsed from 01/01/1960  into the date represented according to outputDateType.
         *
         * @param bytes          the array of bytes that stores the number of days from 01/01/1960.
         * @param outputDateType type of the date formatting
         * @param columnFormat   SAS date format
         * @return date representation
         */
        private Object bytesToDate(byte[] bytes, OutputDateType outputDateType, ColumnFormat columnFormat)
        {
            double doubleDays = bytesToDouble(bytes);
            return sasTemporalFormatter.formatSasDate(doubleDays, outputDateType,
                    columnFormat.getName(), columnFormat.getWidth(), columnFormat.getPrecision());
        }

        /**
         * The function to convert an array of bytes into a double number.
         *
         * @param bytes a double number represented by an array of bytes.
         * @return a number of the double type that is the conversion result.
         */
        private double bytesToDouble(byte[] bytes)
        {
            ByteArray original = byteArrayToByteBuffer(bytes);

            if (bytes.Length < SasFileConstants.BYTES_IN_DOUBLE)
            {
                ByteArray byteBuffer = new ByteArray(SasFileConstants.BYTES_IN_DOUBLE);
                if (sasFileProperties.getEndianness() == 1)
                {
                    byteBuffer.Position = (SasFileConstants.BYTES_IN_DOUBLE - bytes.Length);
                }
                byteBuffer.Write(original.Buffer, 0, original.Buffer.Length);
                byteBuffer.endianness = original.endianness;
                byteBuffer.Position = 0;
                original = byteBuffer;
            }

            return original.ReadF64();
        }

        /**
         * The function to remove excess symbols from the end of a bytes array. Excess symbols are line end characters,
         * tabulation characters, and spaces, which do not contain useful information.
         *
         * @param source an array of bytes containing required data.
         * @param offset the offset in source of required data.
         * @param length the length of required data.
         * @return the array of bytes without excess symbols at the end.
         */
        private byte[] trimBytesArray(byte[] source, int offset, int length)
        {
            int lengthFromBegin;
            for (lengthFromBegin = offset + length; lengthFromBegin > offset; lengthFromBegin--)
            {
                if (source[lengthFromBegin - 1] != ' ' && source[lengthFromBegin - 1] != '\0'
                        && source[lengthFromBegin - 1] != '\t')
                {
                    break;
                }
            }

            if (lengthFromBegin - offset != 0)
            {
                return source[offset..lengthFromBegin];
            }
            else
            {
                return null;
            }
        }

        /**
         * Columns getter.
         *
         * @return columns list.
         */
        public List<Column> getColumns()
        {
            return columns;
        }

        /**
         * The function to get sasFileParser.
         *
         * @return the object of the {@link SasFileProperties} class that stores file metadata.
         */
        public SasFileProperties getSasFileProperties()
        {
            return sasFileProperties;
        }

        /**
         * Enumeration of all subheader types used in sas7bdat files.
         */
        internal enum SubheaderIndexes
        {
            /**
             * Index which define row size subheader, which contains rows size in bytes and the number of rows.
             */
            ROW_SIZE_SUBHEADER_INDEX,

            /**
             * Index which define column size subheader, which contains columns count.
             */
            COLUMN_SIZE_SUBHEADER_INDEX,

            /**
             * Index which define subheader counts subheader, which contains currently not used data.
             */
            SUBHEADER_COUNTS_SUBHEADER_INDEX,

            /**
             * Index which define column text subheader, which contains type of file compression
             * and info about columns (name, label, format).
             */
            COLUMN_TEXT_SUBHEADER_INDEX,

            /**
             * Index which define column name subheader, which contains column names.
             */
            COLUMN_NAME_SUBHEADER_INDEX,

            /**
             * Index which define column attributes subheader, which contains column attributes, such as type.
             */
            COLUMN_ATTRIBUTES_SUBHEADER_INDEX,

            /**
             * Index which define format and label subheader, which contains info about format of objects in column
             * and tooltip text for columns.
             */
            FORMAT_AND_LABEL_SUBHEADER_INDEX,

            /**
             * Index which define column list subheader, which contains currently not used data.
             */
            COLUMN_LIST_SUBHEADER_INDEX,

            /**
             * Index which define data subheader, which contains sas7bdat file rows data.
             */
            DATA_SUBHEADER_INDEX
        }

        /**
         * The interface that is implemented by all classes that process subheaders.
         */
        public interface ProcessingSubheader
        {
            SasFileParser FileParser { get; }
            /**
             * Method which should be overwritten in implementing this interface classes.
             *
             * @param subheaderOffset offset in bytes from the beginning of subheader.
             * @param subheaderLength length of subheader in bytes.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            void processSubheader(long subheaderOffset, long subheaderLength);
        }

        /**
         * The interface that is implemented by classes that process data subheader.
         */
        public interface ProcessingDataSubheader : ProcessingSubheader
        {
            /**
             * Method which should be overwritten in implementing this interface classes.
             *
             * @param subheaderOffset offset in bytes from the beginning of subheader.
             * @param subheaderLength length of subheader in bytes.
             * @param columnNames     list of column names which should be processed.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            void processSubheader(long subheaderOffset, long subheaderLength, List<String> columnNames);
        }

        /**
         * SasFileParser builder class made using builder pattern.
         */
        public class Builder
        {

            /**
             * Empty private constructor to prevent instantiation without the {@link SasFileParser#sasFileStream} variable.
             */
            private Builder()
            {

            }

            /**
             * Builder variable for {@link SasFileParser#sasFileStream} variable.
             */
            internal Stream sasFileStream;

            /**
             * Builder variable for {@link SasFileParser#encoding} variable.
             */
            internal string encoding;

            /**
             * Default value for {@link SasFileParser#outputDateType} variable.
             */
            internal OutputDateType? outputDateType = OutputDateType.JAVA_DATE_LEGACY;

            /**
             * Default value for {@link SasFileParser#byteOutput} variable.
             */
            internal bool byteOutput = false;

            /**
             * The constructor that specifies builders sasFileStream variable.
             *
             * @param sasFileStream value for {@link SasFileParser#sasFileStream} variable.
             */
            internal Builder(Stream sasFileStream)
            {
                this.sasFileStream = sasFileStream;
            }

            /**
             * The function to specify builders encoding variable.
             *
             * @param val value to be set.
             * @return result builder.
             */
            internal Builder Encoding(String val)
            {
                encoding = val;
                return this;
            }

            /**
             * Sets the specified type of the output date format.
             *
             * @param val value to be set.
             * @return result builder.
             */
            internal Builder WithOutputDateType(OutputDateType? val)
            {
                if (val != null)
                {
                    outputDateType = val.Value;
                }
                return this;
            }

            /**
             * The function to specify builders byteOutput variable.
             *
             * @param val value to be set.
             * @return result builder.
             */
            internal Builder ByteOutput(bool val)
            {
                byteOutput = val;
                return this;
            }

            /**
             * The function to create variable of SasFileParser class using current builder.
             *
             * @return newly built SasFileParser
             */
            public SasFileParser build()
            {
                return new SasFileParser(this);
            }
        }

        /**
         * The class to store subheaders pointers that contain information about the offset, length, type
         * and compression of subheaders (see {@link SasFileConstants#TRUNCATED_SUBHEADER_ID},
         * {@link SasFileConstants#COMPRESSED_SUBHEADER_ID}, {@link SasFileConstants#COMPRESSED_SUBHEADER_TYPE}
         * for details).
         */
        public class SubheaderPointer
        {
            /**
             * The offset from the beginning of a page at which a subheader is stored.
             */
            public long offset;

            /**
             * The subheader length.
             */
            public long length;

            /**
             * The type of subheader compression. If the type is {@link SasFileConstants#TRUNCATED_SUBHEADER_ID}
             * the subheader does not contain information relevant to the current issues. If the type is
             * {@link SasFileConstants#COMPRESSED_SUBHEADER_ID} the subheader can be compressed
             * (depends on {@link SubheaderPointer#type}).
             */
            public byte compression;

            /**
             * The subheader type. If the type is {@link SasFileConstants#COMPRESSED_SUBHEADER_TYPE}
             * the subheader is compressed. Otherwise, there is no compression.
             */
            public byte type;

            /**
             * The constructor of the {@link SubheaderPointer} class that defines values of all its variables.
             *
             * @param offset      the offset of the subheader from the beginning of the page.
             * @param length      the subheader length.
             * @param compression the subheader compression type. If the type is
             *                    {@link SasFileConstants#TRUNCATED_SUBHEADER_ID}, the subheader does not contain useful
             *                    information. If the type is {@link SasFileConstants#COMPRESSED_SUBHEADER_ID},
             *                    the subheader can be compressed (depends on {@link SubheaderPointer#type}).
             * @param type        the subheader type. If the type is {@link SasFileConstants#COMPRESSED_SUBHEADER_TYPE}
             *                    the subheader is compressed, otherwise, it is not.
             */
            public SubheaderPointer(long offset, long length, byte compression, byte type)
            {
                this.offset = offset;
                this.length = length;
                this.compression = compression;
                this.type = type;
            }
        }

        /**
         * The class to process subheaders of the RowSizeSubheader type that store information about the table rows length
         * (in bytes), the number of rows in the table and the number of rows on the last page of the
         * {@link PageType#PAGE_TYPE_MIX} type. The results are stored in {@link SasFileProperties#rowLength},
         * {@link SasFileProperties#rowCount}, and {@link SasFileProperties#mixPageRowCount}, respectively.
         */
        public class RowSizeSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public RowSizeSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the following metadata about rows of the sas7bdat file:
             * {@link SasFileProperties#rowLength}, {@link SasFileProperties#rowCount},
             * and {@link SasFileProperties#mixPageRowCount}.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                long[] offset = {subheaderOffset + SasFileConstants.ROW_LENGTH_OFFSET_MULTIPLIER * intOrLongLength,
                    subheaderOffset + SasFileConstants.ROW_COUNT_OFFSET_MULTIPLIER * intOrLongLength,
                    subheaderOffset + SasFileConstants.ROW_COUNT_ON_MIX_PAGE_OFFSET_MULTIPLIER * intOrLongLength,
                    subheaderOffset + SasFileConstants.FILE_FORMAT_OFFSET_OFFSET + 82 * intOrLongLength,
                    subheaderOffset + SasFileConstants.FILE_FORMAT_LENGTH_OFFSET + 82 * intOrLongLength,
                    subheaderOffset + SasFileConstants.DELETED_ROW_COUNT_OFFSET_MULTIPLIER * intOrLongLength,
                    subheaderOffset + SasFileConstants.COMPRESSION_METHOD_OFFSET + 82 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COMPRESSION_METHOD_LENGTH_OFFSET + 82 * intOrLongLength,
            };
                int[] length = {intOrLongLength, intOrLongLength, intOrLongLength,
                    SasFileConstants.FILE_FORMAT_OFFSET_LENGTH, SasFileConstants.FILE_FORMAT_LENGTH_LENGTH,
                    intOrLongLength,
                    SasFileConstants.COMPRESSION_METHOD_OFFSET_LENGTH, SasFileConstants.COMPRESSION_METHOD_LENGTH_LENGTH};
                List<byte[]> vars = FileParser.getBytesFromFile(offset, length);

                if (FileParser.sasFileProperties.getRowLength() == 0)
                {
                    FileParser.sasFileProperties.setRowLength(FileParser.bytesToLong(vars[0]));
                }
                if (FileParser.sasFileProperties.getRowCount() == 0)
                {
                    FileParser.sasFileProperties.setRowCount(FileParser.bytesToLong(vars[1]));
                }
                if (FileParser.sasFileProperties.getMixPageRowCount() == 0)
                {
                    FileParser.sasFileProperties.setMixPageRowCount(FileParser.bytesToLong(vars[2]));
                }

                FileParser.fileLabelOffset = FileParser.bytesToShort(vars[3]);
                FileParser.fileLabelLength = FileParser.bytesToShort(vars[4]);

                if (FileParser.sasFileProperties.getDeletedRowCount() == 0)
                {
                    FileParser.sasFileProperties.setDeletedRowCount(FileParser.bytesToLong(vars[5]));
                }

                FileParser.compressionMethodOffset = FileParser.bytesToShort(vars[6]);
                FileParser.compressionMethodLength = FileParser.bytesToShort(vars[7]);
            }
        }

        /**
         * The class to process subheaders of the ColumnSizeSubheader type that store information about
         * the number of table columns. The {@link SasFileProperties#columnsCount} variable stores the results.
         */
        class ColumnSizeSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public ColumnSizeSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the following metadata about columns of the sas7bdat file:
             * {@link SasFileProperties#columnsCount}.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                long[] offset = { subheaderOffset + intOrLongLength };
                int[] length = { intOrLongLength };
                List<byte[]> vars = FileParser.getBytesFromFile(offset, length);

                FileParser.sasFileProperties.setColumnsCount(FileParser.bytesToLong(vars[0]));
            }
        }

        /**
         * The class to process subheaders of the SubheaderCountsSubheader type that does not contain
         * any information relevant to the current issues.
         */
        class SubheaderCountsSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public SubheaderCountsSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read metadata. At the moment the function is empty as the information in
             * SubheaderCountsSubheader is not needed for the current issues.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
            }
        }

        /**
         * The class to process subheaders of the ColumnTextSubheader type that store information about
         * file compression and table columns (name, label, format). The first subheader of this type includes the file
         * compression information. The results are stored in {@link SasFileParser#columnsNamesBytes} and
         * {@link SasFileProperties#compressionMethod}.
         */
        class ColumnTextSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public ColumnTextSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the text block with information about file compression and table columns (name, label,
             * format) from a subheader. The first text block of this type includes the file compression information.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                int textBlockSize;

                long[] offset = { subheaderOffset + intOrLongLength };
                int[] length = { SasFileConstants.TEXT_BLOCK_SIZE_LENGTH };
                List<byte[]> vars = FileParser.getBytesFromFile(offset, length);
                textBlockSize = FileParser.byteArrayToByteBuffer(vars[0]).ReadI16();

                offset[0] = subheaderOffset + intOrLongLength;
                length[0] = textBlockSize;
                vars = FileParser.getBytesFromFile(offset, length);

                FileParser.columnsNamesBytes.Add(vars[0]);
                if (FileParser.columnsNamesBytes.Count == 1)
                {
                    byte[] columnName = FileParser.columnsNamesBytes[0];
                    String compressionMethod = FileParser.bytesToString(columnName, FileParser.compressionMethodOffset, FileParser.compressionMethodLength);
                    if (FileParser.matchCompressionMethod(compressionMethod))
                    {
                        FileParser.sasFileProperties.setCompressionMethod(compressionMethod);
                    }
                    FileParser.sasFileProperties.setFileLabel(FileParser.bytesToString(columnName, FileParser.fileLabelOffset, FileParser.fileLabelLength));
                }
            }
        }

        /**
         * The class to process subheaders of the ColumnNameSubheader type that store information about the index of
         * corresponding subheader of the ColumnTextSubheader type whose text field stores the name of the column
         * corresponding to the current subheader. They also store the offset (in symbols) of the names from the beginning
         * of the text field and the length of names (in symbols). The {@link SasFileParser#columnsNamesList} list stores
         * the resulting names.
         */
        class ColumnNameSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public ColumnNameSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the following data from the subheader:
             * - the index that stores the name of the column corresponding to the current subheader,
             * - the offset (in symbols) of the name inside the text block,
             * - the length (in symbols) of the name.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                long columnNamePointersCount = (subheaderLength - 2 * intOrLongLength - 12) / 8;
                int i;
                for (i = 0; i < columnNamePointersCount; i++)
                {
                    long[] offset = {subheaderOffset + intOrLongLength + SasFileConstants.COLUMN_NAME_POINTER_LENGTH * (i + 1)
                        + SasFileConstants.COLUMN_NAME_TEXT_SUBHEADER_OFFSET, subheaderOffset + intOrLongLength
                        + SasFileConstants.COLUMN_NAME_POINTER_LENGTH * (i + 1) + SasFileConstants.COLUMN_NAME_OFFSET_OFFSET, subheaderOffset
                        + intOrLongLength + SasFileConstants.COLUMN_NAME_POINTER_LENGTH * (i + 1) + SasFileConstants.COLUMN_NAME_LENGTH_OFFSET};
                    int[] length = {SasFileConstants.COLUMN_NAME_TEXT_SUBHEADER_LENGTH, SasFileConstants.COLUMN_NAME_OFFSET_LENGTH,
                        SasFileConstants.COLUMN_NAME_LENGTH_LENGTH};
                    List<byte[]> vars = FileParser.getBytesFromFile(offset, length);

                    int textSubheaderIndex = FileParser.bytesToShort(vars[0]);
                    int columnNameOffset = FileParser.bytesToShort(vars[1]);
                    int columnNameLength = FileParser.bytesToShort(vars[2]);
                    if (textSubheaderIndex < FileParser.columnsNamesBytes.Count)
                    {
                        FileParser.columnsNamesList.Add(FileParser.bytesToString(FileParser.columnsNamesBytes[textSubheaderIndex],
                                columnNameOffset, columnNameLength));
                    }
                    else
                    {
                        FileParser.columnsNamesList.Add(new String(new char[columnNameLength]));
                        FileParser.columnMissingInfoList.Add(new ColumnMissingInfo(i, textSubheaderIndex, columnNameOffset,
                                columnNameLength, ColumnMissingInfo.MissingInfoType.NAME));
                    }
                }
            }
        }

        /**
         * The class to process subheaders of the ColumnAttributesSubheader type that store information about
         * the data length (in bytes) of the current column and about the offset (in bytes) of the current column`s data
         * from the beginning of the row with data. They also store the column`s data type: {@link Number} and
         * {@link String}. The resulting names are stored in the {@link SasFileParser#columnsDataOffset},
         * {@link SasFileParser#columnsDataLength}, and{@link SasFileParser#columnsTypesList}.
         */
        class ColumnAttributesSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public ColumnAttributesSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the length, offset and type of data in cells related to the column from the subheader
             * that stores information about this column.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                long columnAttributesVectorsCount = (subheaderLength - 2 * intOrLongLength - 12) / (intOrLongLength + 8);
                for (int i = 0; i < columnAttributesVectorsCount; i++)
                {
                    long[] offset = {subheaderOffset + intOrLongLength + SasFileConstants.COLUMN_DATA_OFFSET_OFFSET + i
                        * (intOrLongLength + 8), subheaderOffset + 2 * intOrLongLength + SasFileConstants.COLUMN_DATA_LENGTH_OFFSET + i
                        * (intOrLongLength + 8), subheaderOffset + 2 * intOrLongLength + SasFileConstants.COLUMN_TYPE_OFFSET + i
                        * (intOrLongLength + 8)};
                    int[] length = { intOrLongLength, SasFileConstants.COLUMN_DATA_LENGTH_LENGTH, SasFileConstants.COLUMN_TYPE_LENGTH };
                    List<byte[]> vars = FileParser.getBytesFromFile(offset, length);

                    FileParser.columnsDataOffset.Add((ulong)FileParser.bytesToLong(vars[0]));
                    FileParser.columnsDataLength.Add(FileParser.bytesToInt(vars[1]));
                    FileParser.columnsTypesList.Add(vars[2][0] == 1 ? typeof(double) : typeof(string));
                }
            }
        }

        /**
         * The class to process subheaders of the FormatAndLabelSubheader type that store the following information:
         * - the index of the ColumnTextSubheader type subheader whose text field contains the column format,
         * - the index of the ColumnTextSubheader type whose text field stores the label of the column corresponding
         * to the current subheader,
         * - offsets (in symbols) of the formats and labels from the beginning of the text field,
         * - lengths of the formats and labels (in symbols),
         * The {@link SasFileParser#columns} list stores the results.
         */
        class FormatAndLabelSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public FormatAndLabelSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The function to read the following data from the subheader:
             * - the index that stores the format of the column corresponding to the current subheader,
             * - the offset (in symbols) of the format inside the text block,
             * - the format length (in symbols),
             * - the index that stores the label of the column corresponding to the current subheader,
             * - the offset (in symbols) of the label inside the text block,
             * - the label length (in symbols).
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                int intOrLongLength = FileParser.sasFileProperties.isU64() ? SasFileConstants.BYTES_IN_LONG : SasFileConstants.BYTES_IN_INT;
                long[] offset = {subheaderOffset + SasFileConstants.COLUMN_FORMAT_WIDTH_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_FORMAT_PRECISION_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_FORMAT_OFFSET_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_FORMAT_LENGTH_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_LABEL_TEXT_SUBHEADER_INDEX_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_LABEL_OFFSET_OFFSET + 3 * intOrLongLength,
                    subheaderOffset + SasFileConstants.COLUMN_LABEL_LENGTH_OFFSET + 3 * intOrLongLength};
                int[] length = {SasFileConstants.COLUMN_FORMAT_WIDTH_OFFSET_LENGTH, SasFileConstants.COLUMN_FORMAT_PRECISION_OFFSET_LENGTH,
                    SasFileConstants.COLUMN_FORMAT_TEXT_SUBHEADER_INDEX_LENGTH, SasFileConstants.COLUMN_FORMAT_OFFSET_LENGTH, SasFileConstants.COLUMN_FORMAT_LENGTH_LENGTH,
                    SasFileConstants.COLUMN_LABEL_TEXT_SUBHEADER_INDEX_LENGTH, SasFileConstants.COLUMN_LABEL_OFFSET_LENGTH, SasFileConstants.COLUMN_LABEL_LENGTH_LENGTH};
                List<byte[]> vars = FileParser.getBytesFromFile(offset, length);

                int columnFormatWidth = FileParser.bytesToShort(vars[0]);
                int columnFormatPrecision = FileParser.bytesToShort(vars[1]);
                int textSubheaderIndexForFormat = FileParser.bytesToShort(vars[2]);
                int columnFormatOffset = FileParser.bytesToShort(vars[3]);
                int columnFormatLength = FileParser.bytesToShort(vars[4]);
                int textSubheaderIndexForLabel = FileParser.bytesToShort(vars[5]);
                int columnLabelOffset = FileParser.bytesToShort(vars[6]);
                int columnLabelLength = FileParser.bytesToShort(vars[7]);
                String columnLabel = "";
                String columnFormatName = "";
                if (textSubheaderIndexForLabel < FileParser.columnsNamesBytes.Count)
                {
                    columnLabel = FileParser.bytesToString(FileParser.columnsNamesBytes[textSubheaderIndexForLabel],
                            columnLabelOffset, columnLabelLength);
                }
                else
                {
                    FileParser.columnMissingInfoList.Add(new ColumnMissingInfo(FileParser.columns.Count, textSubheaderIndexForLabel,
                            columnLabelOffset, columnLabelLength, ColumnMissingInfo.MissingInfoType.LABEL));
                }
                if (textSubheaderIndexForFormat < FileParser.columnsNamesBytes.Count)
                {
                    columnFormatName = FileParser.bytesToString(FileParser.columnsNamesBytes[textSubheaderIndexForFormat],
                            columnFormatOffset, columnFormatLength);
                }
                else
                {
                    FileParser.columnMissingInfoList.Add(new ColumnMissingInfo(FileParser.columns.Count, textSubheaderIndexForFormat,
                            columnFormatOffset, columnFormatLength, ColumnMissingInfo.MissingInfoType.FORMAT));
                }
                //LOGGER.debug(COLUMN_FORMAT, columnFormatName);
                ColumnFormat columnFormat = new ColumnFormat(columnFormatName, columnFormatWidth, columnFormatPrecision);
                FileParser.columns.Add(new Column(FileParser.currentColumnNumber + 1, FileParser.columnsNamesList[FileParser.columns.Count],
                        columnLabel, columnFormat, FileParser.columnsTypesList[FileParser.columns.Count],
                        FileParser.columnsDataLength[FileParser.currentColumnNumber++]));
            }
        }

        /**
         * The class to process subheaders of the ColumnListSubheader type that do not store any information relevant
         * to the current tasks.
         */
        class ColumnListSubheader : ProcessingSubheader
        {
            public SasFileParser FileParser { get; }

            public ColumnListSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The method to read metadata. It is empty at the moment because the data stored in ColumnListSubheader
             * are not used.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
            }
        }

        /**
         * The class to process subheaders of the DataSubheader type that keep compressed or uncompressed data.
         */
        class DataSubheader : ProcessingDataSubheader
        {
            public SasFileParser FileParser { get; }

            public DataSubheader(SasFileParser fileParser)
            {
                this.FileParser = fileParser;
            }
            /**
             * The method to read compressed or uncompressed data from the subheader. The results are stored as a row
             * in {@link SasFileParser#currentRow}. The {@link SasFileParser#processByteArrayWithData(long, long, List)}
             * function converts the array of bytes into a list of objects.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength)
            {
                FileParser.currentRow = FileParser.processByteArrayWithData(subheaderOffset, subheaderLength, null);
            }

            /**
             * The method to read compressed or uncompressed data from the subheader. The results are stored as a row
             * in {@link SasFileParser#currentRow}. The {@link SasFileParser#processByteArrayWithData(long, long, List)}
             * function converts the array of bytes into a list of objects.
             *
             * @param subheaderOffset the offset at which the subheader is located.
             * @param subheaderLength the subheader length.
             * @throws IOException if reading from the {@link SasFileParser#sasFileStream} stream is impossible.
             */
            public void processSubheader(long subheaderOffset, long subheaderLength,
                                         List<String> columnNames)
            {
                FileParser.currentRow = FileParser.processByteArrayWithData(subheaderOffset, subheaderLength, columnNames);
            }
        }
    }
}