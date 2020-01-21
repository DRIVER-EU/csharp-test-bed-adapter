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

namespace eu.driver.CSharpTestBedAdapter
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
            { "time-control", "system_timing_control" },
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
            { typeof(eu.driver.model.core.LargeDataUpdate), "large_data_update" },
            { typeof(eu.driver.model.core.MapLayerUpdate), "large_data_update" },
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
                Settings = (Schemas.settings)serializer.Deserialize(reader);

                _producerConfig = new Dictionary<string, string>
                {
                    { "bootstrap.servers", Settings.brokerurl },
                    { "retries", Settings.retrycount.ToString() },
                    { "request.timeout.ms", Settings.retrytime.ToString() },
                    { "security.protocol", Settings.securityprotocol },
                    { "ssl.ca.location", Settings.securitycapath },
                    { "ssl.keystore.location", Settings.securitykeystorepath },
                    { "ssl.keystore.password", Settings.securitykeystorepassword },
                };

                _consumerConfig = new Dictionary<string, string>
                {
                    { "bootstrap.servers", Settings.brokerurl },
                    { "group.id", Settings.clientid },
                    { "security.protocol", Settings.securityprotocol },
                    { "ssl.ca.location", Settings.securitycapath },
                    { "ssl.keystore.location", Settings.securitykeystorepath },
                    { "ssl.keystore.password", Settings.securitykeystorepassword },
                };

                SchemaRegistryClient = new Confluent.SchemaRegistry.CachedSchemaRegistryClient(
                    new Confluent.SchemaRegistry.SchemaRegistryConfig()
                    {
                        Url = Settings.schemaurl,
                        RequestTimeoutMs = 5000,
                        MaxCachedSchemas = 10,
                    }
                );
            }
        }

        /// <summary>
        /// The parsed user settings to be used inside the adapter
        /// </summary>
        internal Schemas.settings Settings
        {
            get; private set;
        }

        /// <summary>
        /// The Kafka producer configuration
        /// </summary>
        internal Dictionary<string, string> ProducerConfig
        {
            get { return new Dictionary<string, string>(_producerConfig); }
        }
        private Dictionary<string, string> _producerConfig;

        /// <summary>
        /// The Kafka consumer configuration
        /// </summary>
        internal Dictionary<string, string> ConsumerConfig
        {
            get { return new Dictionary<string, string>(_consumerConfig); }
        }
        private Dictionary<string, string> _consumerConfig;

        /// <summary>
        /// The Kafka schema registry client containing the configuration
        /// </summary>
        internal Confluent.SchemaRegistry.CachedSchemaRegistryClient SchemaRegistryClient
        {
            get; private set;
        }
    }
}
