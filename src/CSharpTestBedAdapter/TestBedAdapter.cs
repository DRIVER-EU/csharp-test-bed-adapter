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

using eu.driver.model.core;
using eu.driver.model.edxl;
using eu.driver.model.system;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// Main C# adapter class that provides an interface for external applications to connect to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    public class TestBedAdapter : IDisposable
    {
        /// <summary>
        /// Configuration class, including internal setup information and external settings
        /// </summary>
        private Configuration _configuration = null;

        /// <summary>
        /// The producer this connector is using to send heartbeats
        /// </summary>
        private Producer<EDXLDistribution, Heartbeat> _heartbeatProducer;
        /// <summary>
        /// The producer this connector is using to send logs
        /// </summary>
        private Producer<EDXLDistribution, Log> _logProducer;
        /// <summary>
        /// The producer this connector is using to send its current configuration
        /// </summary>
        private Producer<EDXLDistribution, eu.driver.model.core.Configuration> _configurationProducer;

        #region Initialization

        /// <summary>
        /// Default constructor of the adapter
        /// </summary>
        private TestBedAdapter()
        {
            try
            {
                // Create a new configuration, including the read in external settings
                _configuration = new Configuration();

                // Create the producers for the core topics
                _heartbeatProducer = new Producer<EDXLDistribution, Heartbeat>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Heartbeat>());
                _heartbeatProducer.OnError += Adapter_Error;
                _heartbeatProducer.OnLog += Adapter_Log;
                _logProducer = new Producer<EDXLDistribution, Log>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Log>());
                _logProducer.OnError += Adapter_Error;
                _logProducer.OnLog += Adapter_Log;
                _configurationProducer = new Producer<EDXLDistribution, eu.driver.model.core.Configuration>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<eu.driver.model.core.Configuration>());
                _configurationProducer.OnError += Adapter_Error;
                _configurationProducer.OnLog += Adapter_Log;

                SendConfiguration();

                // Start the heart beat to indicate the connector is still alive
                // TODO: Add CancellationToken to stop on dispose
                Task.Factory.StartNew(() => { this.Heartbeat(); });
            }
            catch (Exception e)
            {
                Log(log4net.Core.Level.Critical, e.ToString());
                throw e;
            }
        }

        /// <summary>
        /// Singleton implementation to make sure only one adapter instance is running for one tool application
        /// </summary>
        /// <returns>The instance of the test-bed adapter</returns>
        public static TestBedAdapter GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TestBedAdapter();
            }
            return _instance;
        }
        private static TestBedAdapter _instance = null;

        /// <summary>
        /// Method for creating the key to be used within core messages
        /// </summary>
        /// <returns>An EDXL-DE key containing all information for adapters to understand where this message originates from</returns>
        private EDXLDistribution CreateCoreKey()
        {
            return new EDXLDistribution()
            {
                senderID = _configuration.Settings.clientId,
                distributionID = Guid.NewGuid().ToString(),
                distributionKind = DistributionKind.Update,
                distributionStatus = DistributionStatus.System,
                dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
            };
        }

        #endregion Initialization

        #region Configuration

        /// <summary>
        /// Method for sending the configuration to the core configuration topic
        /// </summary>
        private void SendConfiguration()
        {
            try
            {
                // Fill in the configuration message
                EDXLDistribution key = CreateCoreKey();
                eu.driver.model.core.Configuration config = new eu.driver.model.core.Configuration()
                {
                    clientId = _configuration.Settings.clientId,
                    heartbeatInterval = _configuration.Settings.heartbeatInterval,
                    kafkaHost = _configuration.Settings.brokerUrl,
                    schemaRegistry = _configuration.Settings.schemaUrl,
                    logging = new LogSettings() { logToKafka = 2 },
                    // TODO: set the topics this adapter is consuming and producing
                };

                // Send the configuration message
                _configurationProducer.ProduceAsync(Configuration.CoreTopics["configuration"], key, config);
            }
            catch (Exception e)
            {
                Log(log4net.Core.Level.Error, e.ToString());
            }
        }

        #endregion Configuration

        #region Heartbeat

        /// <summary>
        /// Method for starting the heart beat of this adapter
        /// </summary>
        private void Heartbeat()
        {
            while (true)
            {
                // Send out the heart beat that this connector is still alive
                EDXLDistribution key = CreateCoreKey();
                Heartbeat beat = new Heartbeat { id = _configuration.Settings.clientId, alive = DateTime.UtcNow.Ticks / 10000 };

                _heartbeatProducer.ProduceAsync(Configuration.CoreTopics["heartbeat"], key, beat);

                // Wait for the specified amount of milliseconds
                Task wait = Task.Delay(_configuration.Settings.heartbeatInterval);
                wait.Wait();
            }
        }

        #endregion Heartbeat

        #region Log

        /// <summary>
        /// Method for logging a given message to the test bed core log
        /// </summary>
        /// <param name="level">The <see cref="log4net.Core.Level"/> indicating the severity of the message</param>
        /// <param name="msg">The message to be logged</param>
        // TODO: Think about creating own log levels and putting them into the schema
        public void Log(log4net.Core.Level level, string msg)
        {
            // Send the message to the callback function
            if (_logHandler != null)
            {
                _logHandler.Invoke(level + "::" + msg);
            }

            // Send out the log towards the core log topic
            if (_logProducer != null)
            {
                EDXLDistribution key = CreateCoreKey();
                Log log = new Log() { id = _configuration.Settings.clientId, log = msg };

                _logProducer.ProduceAsync(Configuration.CoreTopics["log"], key, log);
            }
            else throw new NullReferenceException($"Could not create the log producer that should send the following log:\n{msg}");
        }

        /// <summary>
        /// Method for adding a callback function to the log event of this adapter
        /// </summary>
        /// <param name="handler">The function that will be called once a log message is sent</param>
        public void AddLogCallback(LogHandler handler)
        {
            _logHandler = handler;
        }
        public delegate void LogHandler(string message);
        private LogHandler _logHandler = null;


        /// <summary>
        /// Collective delegate to report all errors created by producers and consumers
        /// </summary>
        /// <param name="sender">The producer or consumer sending the error</param>
        /// <param name="error">The actual error from the producer or consumer</param>
        private void Adapter_Error(object sender, Error error)
        {
            Log(log4net.Core.Level.Error, sender.GetType() + " " + error.Code + ": " + error.Reason);
        }

        /// <summary>
        /// Collective delegate to report all logs created by producers and consumers
        /// </summary>
        /// <param name="sender">The producer or consumer sending the log</param>
        /// <param name="error">The actual log from the producer or consumer</param>
        private void Adapter_Log(object sender, LogMessage log)
        {
            // TODO: Possibly map log.Level to log4net.Core.Level?
            Log(log4net.Core.Level.Info, sender.GetType() + " " + log.Name + ": " + log.Message);
        }

        #endregion Log

        #region Destruction

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            if (_heartbeatProducer != null)
            {
                _heartbeatProducer.OnError -= Adapter_Error;
                _heartbeatProducer.OnLog -= Adapter_Log;
                _heartbeatProducer.Dispose();
            }
            if (_logProducer != null)
            {
                _logProducer.OnError -= Adapter_Error;
                _logProducer.OnLog -= Adapter_Log;
                _logProducer.Dispose();
            }
            if (_configurationProducer != null)
            {
                _configurationProducer.OnError -= Adapter_Error;
                _configurationProducer.OnLog -= Adapter_Log;
                _configurationProducer.Dispose();
            }
        }

        #endregion Destruction


        ///// <summary>
        ///// Default constructor
        ///// </summary>
        ///// <param name="appName">The name of the application creating this consumer</param>
        ///// <param name="topic">The name of the topic this consumer should receive its messages from</param>
        ///// <param name="offset">The starting position of this consumer to receive messages from the given topic (0 all messages recorded on the topic; -1 for only the newly recorded messages on the topic)</param>
        //public CSharpConsumer(string appName, string topic, long offset)
        //    : base(appName)
        //{
        //    try
        //    {
        //        _topic = topic;
        //        // Create a new Confluent.Kafka.Consumer with a generic key (containing envelope information) and as value the specified Avro record
        //        _consumer = new Consumer<EDXLDistribution, T>(Configuration.Instance.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<T>());
        //        // Configure the consumer so it will start at the specified topic and offset
        //        // TODO: Maybe open up the partition as well for the user to set?
        //        _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_topic, 0, offset) });

        //        // Raised on critical errors, e.g. connection failures or all brokers down.
        //        _consumer.OnError += (sender, error) =>
        //        {
        //            OnError?.Invoke(sender, error);
        //        };
        //        // Raised on deserialization errors or when a consumed message has an error != NoError.
        //        _consumer.OnConsumeError += (sender, error) =>
        //        {
        //            OnConsumeError?.Invoke(sender, error);
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        // Log error via connector
        //        this.Log(log4net.Core.Level.Critical, e.ToString());
        //    }
        //}

        ///// <summary>
        ///// Method for allowing the consumer to process a possible new message recorded on the topic
        ///// </summary>
        ///// <param name="message">The deserialized new message to be consumed</param>
        ///// <param name="timeout">The amount of time the consumer is allowed to process</param>
        ///// <returns>True if a new message is consumed, false otherwise</returns>
        //public bool Consume(out Message<EDXLDistribution, T> message, TimeSpan timeout)
        //{
        //    try
        //    {
        //        return _consumer.Consume(out message, timeout);
        //    }
        //    catch (Exception e)
        //    {
        //        // Log error via connector
        //        this.Log(log4net.Core.Level.Error, e.ToString());
        //        message = null;
        //        return false;
        //    }
        //}
    }
}
