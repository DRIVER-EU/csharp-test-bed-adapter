/*************************************************************
 * Copyright (C) 2017-2018 
 *               XVR Simulation B.V., Delft, The Netherlands
 *               Martijn Hendriks <hendriks @ xvrsim.com>
 * 
 * This file is part of "DRIVER+ WP923 Test-bed infrastructure" project.
 * 
 * This file is licensed under the MIT license : 
 *   https://github.com/DRIVER-EU/test-bed/blob/master/LICENSE
 *
 *************************************************************/

using System;
using System.Threading.Tasks;

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

using log4net.Core;

using eu.driver.model.core;
using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    public abstract class CSharpConnector : IDisposable
    {
        /// <summary>
        /// The producer this connector is using to send heartbeats
        /// </summary>
        private Producer<EDXLDistribution, Heartbeat> _heartbeatProducer;
        /// <summary>
        /// The producer this connector is using to send logs
        /// </summary>
        private Producer<EDXLDistribution, Log> _logProducer;

        /// <summary>
        /// The name of the application who created this connector
        /// </summary>
        protected string _appName;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="appName">The name of the application creating this connector</param>
        internal CSharpConnector(string appName)
        {
            try
            {
                _appName = appName;

                // Create the system producers
                _heartbeatProducer = new Producer<EDXLDistribution, Heartbeat>(Configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Heartbeat>());
                _logProducer = new Producer<EDXLDistribution, Log>(Configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Log>());

                // TODO: Fully specify and fill in configuration message
                //SendConfiguration();

                // Start the heart beat to indicate the connector is still alive
                // TODO: Add CancellationToken to stop on dispose
                Task.Factory.StartNew(() => { this.Heartbeat(); });
            }
            catch (Exception e)
            {
                Log(Level.Critical, e.ToString());
            }
        }

        /// <summary>
        /// Method for sending the configuration to the system configuration topic
        /// </summary>
        private void SendConfiguration()
        {
            // Create the one time configuration producer
            Producer<EDXLDistribution, eu.driver.model.core.Configuration> configProducer = new Producer<EDXLDistribution, eu.driver.model.core.Configuration>(Configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<eu.driver.model.core.Configuration>());
            if (configProducer != null)
            {
                try
                {
                    // Fill in the configuration message
                    EDXLDistribution key = new EDXLDistribution()
                    {
                        senderID = _appName,
                        distributionID = Guid.NewGuid().ToString(),
                        distributionKind = DistributionKind.Report,
                        distributionStatus = DistributionStatus.Test,
                        dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                        dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
                    };
                    // TODO: Check with design if everything is necessary and fill this in properly
                    eu.driver.model.core.Configuration config = new eu.driver.model.core.Configuration()
                    {
                        clientId = _appName,
                        heartbeatInterval = Configuration.HeartbeatEpoch,
                    };

                    // Send the configuration message
                    // TODO: Check the result and send error if the message couldn't be sent
                    configProducer.ProduceAsync(Configuration.ConfigurationSystemTopic, key, config);
                }
                catch (Exception e)
                {
                    Log(Level.Error, e.ToString());
                }
                finally
                {
                    configProducer.Dispose();
                    configProducer = null;
                }
            }
            else
            {
                Log(Level.Alert, "Could not create a producer for the system configuration");
            }
        }

        /// <summary>
        /// Method for starting the heart beat of this connector
        /// </summary>
        private void Heartbeat()
        {
            while (true)
            {
                // Wait for the specified amount of milliseconds
                Task wait = Task.Delay(Configuration.HeartbeatEpoch);
                wait.Wait();

                // Send out the heart beat that this connector is still alive
                EDXLDistribution key = new EDXLDistribution()
                {
                    senderID = _appName,
                    distributionID = Guid.NewGuid().ToString(),
                    distributionKind = DistributionKind.Report,
                    distributionStatus = DistributionStatus.Test,
                    dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                    dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
                };
                Heartbeat beat = new Heartbeat { id = _appName, alive = DateTime.UtcNow.ToString("o") };
                // TODO: Check the result and send error if the message couldn't be sent
                _heartbeatProducer.ProduceAsync(Configuration.HeartbeatSystemTopic, key, beat);
            }
        }

        /// <summary>
        /// Method for logging a given message to the test bed system log
        /// </summary>
        /// <param name="level">The <see cref="log4net.Core.Level"/> indicating the severity of the message</param>
        /// <param name="msg">The message to be logged</param>
        // TODO: Think about creating own log levels and putting them into the schema
        public void Log(Level level, string msg)
        {
            if (_logProducer != null)
            {
                // Send out the log towards the system log topic
                EDXLDistribution key = new EDXLDistribution()
                {
                    senderID = _appName,
                    distributionID = Guid.NewGuid().ToString(),
                    distributionKind = DistributionKind.Report,
                    distributionStatus = DistributionStatus.Test,
                    dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                    dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
                };
                Log log = new Log() { id = _appName, log = level + " : " + msg };
                // TODO: Check the result and send error if the message couldn't be sent
                _logProducer.ProduceAsync(Configuration.LogSystemTopic, key, log);
            }
            else throw new NullReferenceException($"Could not create the log producer that should send the following log:\n{msg}");
        }

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        // TODO: Implement proper disposal of this class instance
        public virtual void Dispose()
        {
            if (_heartbeatProducer != null)
            {
                _heartbeatProducer.Dispose();
            }
            if (_logProducer != null)
            {
                _logProducer.Dispose();
            }
        }
    }
}
