# Data conversion between Avro and C#

This folder included inside the StandardMessages project of the **csharp-test-bed-adapter** ensures proper conversion from Avro schemas to C# classes. It uses the `avrogen.exe` that is built from the [Apache Avro GitHub](https://github.com/apache/avro).

## Original Avro schema

The Avro schemas are located inside the `avro-schemas` folder. A test schema is located inside the folder. This is an Avro schema for a a simple test message that only contains 2 fields: the name of the sender and the message itself.

## Conversion

Conversion from Avro schema to C# class file is located in the `avro-schemas` folder and contains the following files:

* The compiled `avrogen.exe` ([source](https://github.com/apache/avro/tree/master/lang/csharp/src/apache/codegen)) and its required DLLs
* The Windows batch file `convert.bat` that calls the `avrogen.exe` with the correct parameters for converting all present Avro schemas to corresponding C# class file(s).