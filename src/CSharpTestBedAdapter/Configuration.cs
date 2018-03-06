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
using System.IO;
using System.Xml.Serialization;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// System configuration class that collects all general configuration parameters for the <see cref="CSharpConnector"/>s
    /// </summary>
    internal class Configuration
    {
        /// <summary>
        /// The path where the user settings are located
        /// </summary>
        private static readonly string _settingsPath = @".\data\settings.xml";

        /// <summary>
        /// Private constructor, because this is a singleton class
        /// </summary>
        private Configuration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Schemas.settings));
            using (StreamReader reader = new StreamReader(_settingsPath))
            {
                _settings = (Schemas.settings)serializer.Deserialize(reader);

                _producerConfig = new Dictionary<string, object>
                {
                    { "bootstrap.servers", _settings.brokerUrl },
                    { "schema.registry.url", _settings.schemaUrl },
                    //{ "compression.type", "none" },
                    { "acks", "all" },
                    { "retries", _settings.retryCount },
                    { "request.timeout.ms", _settings.retryTime },
                    // optional avro / schema registry client properties for C#:
                    { "avro.serializer.buffer.bytes", 50 },
                    { "avro.serializer.auto.register.schemas", true },
                    { "schema.registry.connection.timeout.ms", 5000 },
                    { "schema.registry.max.cached.schemas", 10 },
                };

                _consumerConfig = new Dictionary<string, object>
                {
                    { "bootstrap.servers", _settings.brokerUrl },
                    { "schema.registry.url", _settings.schemaUrl },
                    { "group.id", _settings.clientId },
                    { "enable.auto.commit", true },
                    { "auto.offset.reset", "latest" },
                    // optional avro / schema registry client properties for C#:
                    { "schema.registry.connection.timeout.ms", 5000 },
                    { "schema.registry.max.cached.schemas", 10 },
                };
            }
        }

        /// <summary>
        /// Property for retrieving the singleton instance of the configuration
        /// </summary>
        internal static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }
        private static Configuration _instance = null;

        /// <summary>
        /// The parsed user settings to be used inside the adapter
        /// </summary>
        internal Schemas.settings Settings
        {
            get { return _settings; }
        }
        private Schemas.settings _settings = null;

        /// <summary>
        /// The Kafka producer configuration
        /// </summary>
        internal Dictionary<string, object> ProducerConfig
        {
            get { return new Dictionary<string, object>(_producerConfig); }
        }
        private Dictionary<string, object> _producerConfig;

        /// <summary>
        /// The Kafka consumer configuration
        /// </summary>
        internal Dictionary<string, object> ConsumerConfig
        {
            get { return new Dictionary<string, object>(_consumerConfig); }
        }
        private Dictionary<string, object> _consumerConfig;

        /// <summary>
        /// The name of the system's heartbeat topic
        /// </summary>
        internal readonly string HeartbeatSystemTopic = "connect-status-heartbeat";
        /// <summary>
        /// The name of the system's log topic
        /// </summary>
        internal readonly string LogSystemTopic = "connect-status-log";
        /// <summary>
        /// The name of the system's configuration topic
        /// </summary>
        internal readonly string ConfigurationSystemTopic = "connect-status-configuration";
    }
}
