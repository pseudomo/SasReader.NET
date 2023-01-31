[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

# SasReader.NET
This is the port of **parso** Java library for SAS files reading implemented by the **EPAM** https://github.com/epam/parso

## How to use
The library is available as a NuGet package

Install using
```powershell
Install-Package SasReader
```
or
```powershell
dotnet add package SasReader
```

## Code example
```csharp
using SasReader;
using System.Text;

// Initialize SAS file reader
using FileStream sasToParseFileInputStream = File.OpenRead("C:\\dev\\resources\\sas7bdat\\all_rand_normal.sas7bdat");
SasFileReader sasFileReader = new SasFileReaderImpl(sasToParseFileInputStream);

// Read and print META DATA ********************************************************************
var sasMetaColumns = sasFileReader.getColumns();
var sasMetaProps = sasFileReader.getSasFileProperties();

Console.WriteLine("Columns:");
foreach (var column in sasMetaColumns)
{
    Console.WriteLine($"\t Name: {column.getName()}");
}

Console.WriteLine("Properties:");
Console.WriteLine($"\t Name: {sasMetaProps.getName()}");
Console.WriteLine($"\t Rows count: {sasMetaProps.getRowCount()}");
Console.WriteLine($"\t File type: {sasMetaProps.getFileType()}");
Console.WriteLine($"\t File label: {sasMetaProps.getFileLabel()}");
Console.WriteLine($"\t Date created: {sasMetaProps.getDateCreated()}");
Console.WriteLine($"\t Date modified: {sasMetaProps.getDateModified()}");
Console.WriteLine($"\t Encoding: {sasMetaProps.getEncoding()}");
Console.WriteLine($"\t Endianness: {(sasMetaProps.getEndianness() == 1 ? "LITTLE_ENDIANNESS" : "BIG_ENDIANNESS")}");
Console.WriteLine($"\t CompressionMethod: {sasMetaProps.getCompressionMethod()}");


// Read and print DATA *************************************************************************
long rowCount = sasFileReader.getSasFileProperties().getRowCount();
Console.WriteLine("\nDATA:");
for (int i = 0; i < rowCount; i++)
{
    var row = sasFileReader.readNext(); //object[]
    Console.WriteLine($"Row {i, 2} : {string.Join(", ", row)}");
}
```
## License
 Copyright (C) 2015 EPAM

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
