/*************************************************************
 * Copyright (C) 2017-2018 
 *               XVR Simulation B.V., Delft, The Netherlands
 *               Martijn Hendriks <hendriks @ xvrsim.com>
 * 
 * This file is part of "DRIVER+ WP923 Test-bed infrastructure" project.
 * 
 * This file is licensed under the MIT license : 
 *   https://github.com/DRIVER-EU/csharp-test-bed-adapter/blob/master/LICENSE
 *
 *************************************************************/

using System.Collections.Generic;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// System configuration class that collects all general configuration parameters for the <see cref="CSharpConnector"/>s
    /// </summary>
    internal class Configuration
    {
        /// <summary>
        /// The URL of the Kafka host
        /// </summary>
        private static readonly string BrokerUrl = "localhost:3501";
        /// <summary>
        /// The URL of the Kafka schema registry
        /// </summary>
        private static readonly string SchemaRegistryUrl = "localhost:3502";

        /// <summary>
        /// The Kafka producer configuration
        /// </summary>
        internal static readonly Dictionary<string, object> ProducerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", BrokerUrl },
                { "schema.registry.url", SchemaRegistryUrl },
                // optional avro / schema registry client properties:
                { "avro.serializer.buffer.bytes", 50 },
                { "avro.serializer.auto.register.schemas", true },
                { "schema.registry.connection.timeout.ms", 5000 },
                { "schema.registry.max.cached.schemas", 10 },
            };

        /// <summary>
        /// The Kafka consumer configuration
        /// </summary>
        internal static readonly Dictionary<string, object> ConsumerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", BrokerUrl },
                { "schema.registry.url", SchemaRegistryUrl },
                // optional avro / schema registry client properties:
                { "schema.registry.connection.timeout.ms", 5000 },
                { "schema.registry.max.cached.schemas", 10 },
                { "group.id", "test-bed-consumer-avro" },
            };

        /// <summary>
        /// The name of the system's heartbeat topic
        /// </summary>
        internal static readonly string HeartbeatSystemTopic = "connect-status-heartbeat";
        /// <summary>
        /// The number of milliseconds to wait for another system heart beat
        /// </summary>
        internal static readonly int HeartbeatEpoch = 5000;

        /// <summary>
        /// The name of the system's log topic
        /// </summary>
        internal static readonly string LogSystemTopic = "connect-status-log";

        /// <summary>
        /// The name of the system's configuration topic
        /// </summary>
        internal static readonly string ConfigurationSystemTopic = "connect-status-configuration";
    }
}
