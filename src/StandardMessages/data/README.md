# Data conversion between Avro and C#

This functionality ensures proper conversion from Avro schemas to C# classes. It uses the `avrogen.exe` that is built from the [Apache Avro GitHub](https://github.com/apache/avro).

## Original Avro schemas

The Avro schemas are located inside the `avro-schemas` folder.

## Conversion

Conversion from Avro schema to C# class file is located in the `avro-schemas` folder and contains the following files:

* The compiled `avrogen.exe` ([source](https://github.com/apache/avro/tree/master/lang/csharp/src/apache/codegen)) and its required DLLs
* The Windows batch file `convert.bat` that calls the `avrogen.exe` with the correct parameters for converting all present Avro schemas to corresponding C# class file(s).