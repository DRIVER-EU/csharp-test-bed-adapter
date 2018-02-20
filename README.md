# csharp-text-bed-adapter

This is the C# Apache Kafka adapter created for the DRIVER-EU [test-bed](https://github.com/DRIVER-EU/test-bed). This allows C# written programs to communicate over the test-bed.

The implementation is a wrapper around [Confluent's .NET Client for Apache Kafka<sup>TM</sup>](https://github.com/confluentinc/confluent-kafka-dotnet) with the additional NuGet package to support Avro serialization ([Confluent.Kafka.Avro (version 1.0.0-ci-199)](https://www.nuget.org/packages/confluent.kafka.avro)), and offers support for:

* Avro schema's and messages: Both producer and consumer use Avro schema's for their message key and value.
* Logging via Kafka: Your application can log on several log levels (eg. error, debug, info) onto a specific test-bed topic.
* Management
  * Heartbeat (topic: connect-status-heartbeat), so you know which clients are online.
  * Logging (topic: connect-status-log).
  * Configuration (topic: connect-status-configuration), so you can see which topics clients consume and produce.

## Project structure

This project contains one `CSharpTestBedAdapter.sln` solution, which in its turn contains 4 C# projects:

### src\CSharpTestBedAdapter

The main C# project outputting a class library `CSharpTestBedAdapter.dll` that you can use to create Kafka producers and consumers connecting to the DRIVER-EU test-bed.

### examples\CSharpProducer

The simple example for setting up and using a producer via the `CSharpTestBedAdapter.dll`.

### examples\CSharpConsumer

The simple example for setting up and using a consumer via the `CSharpTestBedAdapter.dll`.

### examples\common\CommonMessages

A simple project that is used in both examples, containing the [Common Alerting Protocol](https://en.wikipedia.org/wiki/Common_Alerting_Protocol) (CAP) Avro record obtained from the DRIVER-EU [avro-schemas](https://github.com/DRIVER-EU/avro-schemas).

Inside the `examples\common\CommonMessages\data` folder you'll find a README regarding the conversion from Avro schema to C# class file(s).

### Dependencies

* All projects are build on the .NET Framework 4.6
* All projects are dependent on one NuGet package from Confluent:
  * pre-released [Confluent.Kafka.Avro 0.11.3-ci-255](https://www.nuget.org/packages/Confluent.Kafka.Avro/0.11.3-ci-255)

In order to use the `csharp-test-bed-adapter`, you are also required to download and install the above-mentioned NuGet package.
 
## Usage

Build `CSharpTestBedAdapter` and reference the compiled DLL into your own application.

See the 2 example projects for further implementation of this adapter.