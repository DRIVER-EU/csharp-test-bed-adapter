# csharp-test-bed-adapter

This is the C# Apache Kafka adapter created for the DRIVER-EU [test-bed](https://github.com/DRIVER-EU/test-bed). This allows C# written programs to communicate over the test-bed.

__PLEASE NOTICE: The latest version might not be this master. Newer versions are available as release branches (but might be unstable).__
__For implementation of trial 1 (Poland), please use the branch [release/trial_1](https://github.com/DRIVER-EU/csharp-test-bed-adapter/tree/release/trial_1).__
__For implementation of trial 2 (France), please use the branch [release/trial_2](https://github.com/DRIVER-EU/csharp-test-bed-adapter/tree/release/trial_2).__
__For implementation of trial 4 (Netherlands), please use the branch [release/trial_4](https://github.com/DRIVER-EU/csharp-test-bed-adapter/tree/release/trial_4).__

The implementation is a wrapper around [Confluent's .NET Client for Apache Kafka<sup>TM</sup>](https://github.com/confluentinc/confluent-kafka-dotnet) with the additional NuGet package to support Avro serialization ([Confluent.Kafka.Avro (version 0.11.6)](https://www.nuget.org/packages/confluent.kafka.avro)), and offers support for:

* Sending and receiving Avro schema's and messages: both producer and consumer use Avro schema's for their message key and value.
Methods for sending and receiving standard or custom messages are `SendMessage` & `AddCallback`
* Logging via Kafka: your application can log on several log levels (eg. error, debug, info) onto a specific test-bed topic.
Methods for sending and receiving log messages are `Log` & `AddLogCallback`
* Receive time information: the adapter is connected to the [test-bed time service](https://github.com/DRIVER-EU/test-bed-time-service), allowing you to receive relevant time-related information like fictive trial time, or the speed of the trial.
Method for retrieving the time information is `GetTimeInfo`
* Uploading large data: the adapter is connected to the [test-bed large data service](https://github.com/DRIVER-EU/large-file-service), allowing you to upload large data files for sharing with other applications connected to the test-bed.
Methods for uploading large data are 'GetLargeFileServiceClient' & 'Upload'
* Internal Management: the adapter makes the coupling between application and test-bed as easy as possible.

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
* [Emergency Management Shared Information (EMSI)](https://www.iso.org/standard/57384.html)
* [GeoJSON](https://en.wikipedia.org/wiki/GeoJSON)
* [Mobile Location Protocol (MLP)](https://en.wikipedia.org/wiki/Mobile_Location_Protocol)
* [Test-bed large data update messages](https://github.com/DRIVER-EU/avro-schemas/tree/master/core/large-data)

### src\CoreMessages

The code project that bundles all system/core message formats defined for the DRIVER-EU Test-bed. These schemas can also be found at [DRIVER-EU avro-schemas](https://github.com/DRIVER-EU/avro-schemas). This project is required for `CSharpTestBedAdapter` to run. The following message formats are currently implemented:

* Heartbeat: sending to topic `system_heartbeat`
This allows the adapter to indicate it is still alive for all other adapters in the test-bed.
* Log: sending to topic `system_loggin`
Both the adapter as your application can send log messages to the test-bed to inform operators on what is going on inside the application.
* Admin heartbeat: receiving from topic `system_admin_heartbeat`
The adapter checks if the admin tool inside the test-bed is alive. If not during the first 10 seconds of this adapters existence, the adapter will enter `DEBUG` mode. If the admin tool was present but connection is lost somehow, the adapter will enter `DISABLED` mode until the admin tool comes back alive, re-sending and re-receiving all messages that were queued during being disabled.
* Timing: receiving from topic `system_timing`
The [time service](https://github.com/DRIVER-EU/test-bed-time-service) updates all adapters for setting a global time frame.
* Time control: receiving from topic `system_timing_control`
The [time service](https://github.com/DRIVER-EU/test-bed-time-service) also notifies all adapters on time changes, for instance whenever a running trial is paused. This only affects the adapter whenever the test-bed admin tool is present. In `DEBUG` mode, the adapter will not adhere to stop sending/receiving messages whenever the time service sends a `PAUSE` or `STOP` command via this topic.

### Dependencies

* All projects are build on the .NET Framework 4.6
* All projects are dependent on one NuGet package from Confluent:
  * pre-released [Confluent.Kafka.Avro 0.11.4](https://www.nuget.org/packages/Confluent.Kafka.Avro/0.11.4)

In order to use the `csharp-test-bed-adapter`, you are also required to download and install the above-mentioned NuGet package.
 
## Usage

The C# test-bed adapter is available as [Nuget package](https://www.nuget.org/packages/CSharpTestBedAdapter/).
You can also manually build `CSharpTestBedAdapter` and reference the compiled DLLs `CSharpTestBedAdapter.dll`, `CoreMessages.dll` & `StandardMessages.dll` into your own application.

Next to the compiled `CSharpTestBedAdapter.dll`, there is a `CSharpTestBedAdapter-settings.xml`, where you can change the following adapter settings:
* client.id: the name of the application that uses this adapter
* heartbeat.interval: the time (in ms) between sending a heartbeat
* certificate.path: path of the authorisation certificate (not implemented yet)
* broker.url: the URL of the Kafka broker to connect to
* schema.url: the URL of the schema registry to use
* send.sync: (a)synchronized sending of messages (not implemented yet)
* retry.count: number of retries, before reporting an error (not implemented yet)
* retry.time: the retry interval in between retries (not implemented yet)

See the 3 example projects for further implementation of this adapter.
