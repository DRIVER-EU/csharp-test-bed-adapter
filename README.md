# csharp-text-bed-adapter

This is the C# Apache Kafka adapter created for the DRIVER-EU [test-bed](https://github.com/DRIVER-EU/test-bed). This allows C# written programs to communicate over the test-bed.

The implementation is a wrapper around [Confluent's .NET Client for Apache Kafka<sup>TM</sup>](https://github.com/confluentinc/confluent-kafka-dotnet) with the additional NuGet package to support Avro serialization ([Confluent.Kafka.Avro (version 0.11.3-ci-303)](https://www.nuget.org/packages/confluent.kafka.avro)), and offers support for:

* Avro schema's and messages: Both producer and consumer use Avro schema's for their message key and value.
* Logging via Kafka: Your application can log on several log levels (eg. error, debug, info) onto a specific test-bed topic.
* Management
  * Heartbeat (topic: system_heartbeat), so you know which clients are online.
  * Logging (topic: system_logging).

## Project structure

This project contains one `CSharpTestBedAdapter.sln` solution, which in its turn contains 7 C# projects:

### src\CSharpTestBedAdapter

The main C# project outputting a class library `CSharpTestBedAdapter.dll` that you can use to create Kafka producers and consumers connecting to the DRIVER-EU test-bed.

### examples\CSharpExampleProducer (Custom & Standard)

The simple examples for setting up and using a producer via the `CSharpTestBedAdapter.dll`.
`CSharpExampleProducerCustom` sends out a test message that is defined in `example\common\CommonMessages`.
`CSharpExampleProducerStandard` sends out a [Common Alerting Protocol](https://en.wikipedia.org/wiki/Common_Alerting_Protocol) (CAP) message that is defined in `example\common\StandardMessages`.

### examples\CSharpExampleConsumer

The simple example for setting up and using a consumer via the `CSharpTestBedAdapter.dll`.
This example listens to both the test messages from `CSharpExampleProducerCustom` and CAP messages from `CSharpExampleProducerStandard`.

### src\CommonMessages

A simple project that is used in all examples, containing a simple test Avro schema that is used for transmitting messages of that type from producer to consumer.
Inside the `examples\common\CommonMessages\data` folder you'll find a README regarding the conversion from Avro schema to C# class file(s).

### src\StandardMessages

The code project that bundles all standard message formats defined for the Common Information Space (CIS) of the DRIVER-EU Test-bed. All these Avro schemas can be found at [DRIVER-EU avro-schemas](https://github.com/DRIVER-EU/avro-schemas). This project is required for `CSharpTestBedAdapter` to run. The following message formats are currently implemented:

* [Common Alerting Protocol (CAP)](https://en.wikipedia.org/wiki/Common_Alerting_Protocol)
* Emergency Management Shared Information (EMSI)
* [GeoJSON](https://en.wikipedia.org/wiki/GeoJSON)
* [Mobile Location Protocol (MLP)](https://en.wikipedia.org/wiki/Mobile_Location_Protocol)

### src\CoreMessages

The code project that bundles all system/core message formats defined for the DRIVER-EU Test-bed. These schemas can also be found at [DRIVER-EU avro-schemas](https://github.com/DRIVER-EU/avro-schemas). This project is required for `CSharpTestBedAdapter` to run. The following message formats are currently implemented:

* Heartbeat: sending to topic `system_heartbeat`
* AdminHeartbeat: receiving from topic `system_admin_heartbeat`
* Log: sending top topic `system_loggin`

### Dependencies

* All projects are build on the .NET Framework 4.6
* All projects are dependent on one NuGet package from Confluent:
  * pre-released [Confluent.Kafka.Avro 0.11.3-ci-303](https://www.nuget.org/packages/Confluent.Kafka.Avro/0.11.3-ci-303)

In order to use the `csharp-test-bed-adapter`, you are also required to download and install the above-mentioned NuGet package.
 
## Usage

Build `CSharpTestBedAdapter` and reference the compiled DLLs `CSharpTestBedAdapter.dll`, `CoreMessages.dll` & `StandardMessages.dll` into your own application.

See the 3 example projects for further implementation of this adapter.