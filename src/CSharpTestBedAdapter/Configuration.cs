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
            // TODO: For listening to the sim updates, one Avro schema is mapped to multiple topics. Since this is currently only for listening we don't
            // have to deal with this standard topic right away, but a better solution might be useful (<standard name>, {<type>, <topic>} or something)
            //{ typeof(eu.driver.model.geojson.sim.FeatureCollection), "standard_geojson_sim_item", "standard_geojson_sim_unit", "standard_geojson_sim_unitgroup", "standard_geojson_sim_station" },
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
                    { "bootstrap.servers", _settings.brokerurl },
                    { "schema.registry.url", _settings.schemaurl },
                    //{ "compression.type", "none" },
                    { "acks", "all" },
                    { "retries", _settings.retrycount },
                    { "request.timeout.ms", _settings.retrytime },
                    // optional avro / schema registry client properties for C#:
                    { "avro.serializer.buffer.bytes", 50 },
                    { "avro.serializer.auto.register.schemas", true },
                    { "schema.registry.connection.timeout.ms", 5000 },
                    { "schema.registry.max.cached.schemas", 10 },
                    { "security.protocol", _settings.securityprotocol },
                    { "ssl.ca.location", _settings.securitycertificatepath },
                    { "ssl.keystore.location", _settings.securitykeystorepath },
                    { "ssl.keystore.password", _settings.securitykeystorepassword },

                };

                _consumerConfig = new Dictionary<string, object>
                {
                    { "bootstrap.servers", _settings.brokerurl },
                    { "schema.registry.url", _settings.schemaurl },
                    { "group.id", _settings.clientid },
                    { "enable.auto.commit", true },
                    { "auto.offset.reset", "latest" },
                    // optional avro / schema registry client properties for C#:
                    { "schema.registry.connection.timeout.ms", 5000 },
                    { "schema.registry.max.cached.schemas", 10 },
                    { "security.protocol", _settings.securityprotocol },
                    { "ssl.ca.location", _settings.securitycertificatepath },
                    { "ssl.keystore.location", _settings.securitykeystorepath },
                    { "ssl.keystore.password", _settings.securitykeystorepassword },
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
