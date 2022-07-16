using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Xunit;
using SasReader;


namespace SasParserTest
{
    public class MetadataTests
    {
        private static class Constants
        {
            public const string ResourcesPath = "./resources";
            public const string ResourcesSasFolder = "sas7bdat";
            public const string ResourcesCsvFolder = "csv";
        }

        public string ParseSasMetaToCsv(string sasFilePath)
        {
            Console.WriteLine($"Parsing file {sasFilePath}");
            using FileStream sasToParseFileInputStream = File.OpenRead(sasFilePath);
            SasFileReader sasFileReader = new SasFileReaderImpl(sasToParseFileInputStream);

            var parsedMeta = new StringBuilder();
            using var writer = new StringWriter(parsedMeta);
            CSVMetadataWriter csvMetadataWriter = new CSVMetadataWriterImpl(writer);
            csvMetadataWriter.writeMetadata(sasFileReader.getColumns());
            csvMetadataWriter.writeSasFileProperties(sasFileReader.getSasFileProperties());

            string parserMetaContent = parsedMeta.ToString();
            return parserMetaContent;
        }

        [Theory]
        [InlineData("all_rand_normal")]
        [InlineData("all_rand_normal_with_deleted")]
        [InlineData("all_rand_normal_with_deleted2")]
        [InlineData("all_rand_uniform")]
        [InlineData("almost_rand")]
        [InlineData("charset_aara")]
        [InlineData("charset_acro")]
        [InlineData("charset_acyr")]
        [InlineData("charset_agrk")]
        [InlineData("charset_aheb")]
        [InlineData("charset_aice")]
        [InlineData("charset_ansi")]
        [InlineData("charset_arab")]
        [InlineData("charset_arma")]
        [InlineData("charset_arom")]
        [InlineData("charset_atha")]
        [InlineData("charset_atur")]
        [InlineData("charset_aukr")]
        [InlineData("charset_big5")]
        [InlineData("charset_cyrl")]
        [InlineData("charset_gbke")]
        [InlineData("charset_grek")]
        [InlineData("charset_hebr")]
        [InlineData("charset_j932")]
        //[InlineData("charset_j942")] Encoding not supported
        [InlineData("charset_jeuc")]
        [InlineData("charset_jiso")]
        [InlineData("charset_keuc")]
        [InlineData("charset_kiso")]
        [InlineData("charset_kpce")]
        [InlineData("charset_kwin")]
        [InlineData("charset_lat1")]
        [InlineData("charset_lat2")]
        [InlineData("charset_lat3")]
        [InlineData("charset_lat4")]
        [InlineData("charset_lat5")]
        [InlineData("charset_lat7")]
        [InlineData("charset_lat9")]
        [InlineData("charset_p852")]
        [InlineData("charset_p857")]
        [InlineData("charset_p858")]
        [InlineData("charset_p860")]
        [InlineData("charset_p862")]
        [InlineData("charset_p864")]
        [InlineData("charset_p874")]
        [InlineData("charset_sjis")]
        //[InlineData("charset_sjs4")] Encoding not supported
        //[InlineData("charset_thai")] Encoding not supported
        [InlineData("charset_utf8")]
        [InlineData("charset_wara")]
        [InlineData("charset_wbal")]
        [InlineData("charset_wcyr")]
        [InlineData("charset_wgrk")]
        [InlineData("charset_wheb")]
        [InlineData("charset_wlt1")]
        [InlineData("charset_wlt2")]
        [InlineData("charset_wtur")]
        [InlineData("charset_wvie")]
        //[InlineData("charset_yeuc")] Encoding not supported
        [InlineData("charset_ywin")]
        [InlineData("charset_zeuc")]
        //[InlineData("charset_zpce")] Encoding not supported
        //[InlineData("charset_zwin")] Encoding not supported
        [InlineData("chinese_column_fails")]
        [InlineData("chinese_column_works")]
        [InlineData("comp_deleted")]
        [InlineData("data_page_with_deleted")]
        [InlineData("date_formats")]
        [InlineData("doubles")]
        [InlineData("doubles2")]
        [InlineData("extend_no")]
        [InlineData("extend_yes")]
        [InlineData("file_with_label")]
        [InlineData("fileserrors")]
        [InlineData("int_only")]
        [InlineData("int_only_partmissing")]
        [InlineData("mix_and_missing")]
        [InlineData("mix_data_misc")]
        [InlineData("mix_data_with_longchar")]
        [InlineData("mix_data_with_missing_char")]
        [InlineData("mix_data_with_partmissing_char")]
        [InlineData("mixed_data_one")]
        [InlineData("mixed_data_two")]
        [InlineData("only_datetime")]
        [InlineData("test-columnar")]
        [InlineData("time_formats")]
        [InlineData("tmp868_14")]

        public void ParseAndCompareSasMetadata(string fileName)
        {
            string fileNameExpectedMeta = $"{fileName}_meta.csv";
            string fileNameSasToParse = $"{fileName}.sas7bdat";
            string expectedMetaFilePath = Path.Join(Constants.ResourcesPath, Constants.ResourcesCsvFolder, fileNameExpectedMeta);
            string sasToParseFilePath = Path.Join(Constants.ResourcesPath, Constants.ResourcesSasFolder, fileNameSasToParse);

            string expectedMetaContent = File.ReadAllText(expectedMetaFilePath);
            string parsedMetaContent = ParseSasMetaToCsv(sasToParseFilePath);

            Assert.Equal(expectedMetaContent, parsedMetaContent);
        }
    }
}