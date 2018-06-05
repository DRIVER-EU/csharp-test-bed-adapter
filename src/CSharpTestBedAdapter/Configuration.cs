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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// System configuration class that collects all general configuration parameters for the <see cref="CSharpConnector"/>s
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The path where the user settings are located
        /// </summary>
        private static readonly string _settingsPath = Path.Combine(Path.GetDirectoryName(typeof(Configuration).Assembly.Location), @"CSharpTestBedAdapter-settings.xml");

        /// <summary>
        /// The dictionary containing all core topics used inside this adapter
        /// </summary>
        internal static readonly Dictionary<string, string> CoreTopics = new Dictionary<string, string>()
        {
            { "heartbeat", "system_heartbeat" },
            { "admin-heartbeat", "system_admin_heartbeat" },
            { "log", "system_logging" },
            { "time", "system_timing" },
            { "topic-create-request", "system_topic_create_request" },
            { "topic-access-invite", "system_topic_access_invite" }
        };

        /// <summary>
        /// The dictionary containing all standard message topics used inside the Common Information Space
        /// </summary>
        public static readonly Dictionary<Type, string> StandardTopics = new Dictionary<Type, string>()
        {
            { typeof(eu.driver.model.cap.Alert), "standard_cap" },
            { typeof(eu.driver.model.geojson.FeatureCollection), "standard_geojson" },
            { typeof(eu.driver.model.mlp.SlRep), "standard_mlp" },
            { typeof(eu.driver.model.emsi.TSO_2_0), "standard_emsi" },
        };

        /// <summary>
        /// Private constructor, because this is a singleton class
        /// </summary>
        internal Configuration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Schemas.settings));
            Trace.WriteLine($"Loading settings from {_settingsPath}");
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
    }
}
