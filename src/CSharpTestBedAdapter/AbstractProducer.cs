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

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    internal class AbstractProducer<T> : IAbstractProducer, IDisposable
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The concrete producer to send messages with
        /// </summary>
        private Producer<EDXLDistribution, T> _producer;
        /// <summary>
        /// The name of the sending application
        /// </summary>
        private string _sender;

        /// <summary>
        /// Event being triggered whenever an error occurred
        /// <para>Raised on critical errors, e.g. connection failures or all brokers down</para>
        /// </summary>
        public event EventHandler<Error> OnError;
        /// <summary>
        /// Event being triggered whenever a log report occurred
        /// <para>Raised when there is information that should be logged</para>
        /// </summary>
        public event EventHandler<LogMessage> OnLog;

        /// <summary><see cref="IAbstractProducer.MessageType"/></summary>
        public Type MessageType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">The test-bed adapter configuration information</param>
        internal AbstractProducer(Configuration configuration)
        {
            _producer = new Producer<EDXLDistribution, T>(configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<T>());
            _sender = configuration.Settings.clientId;

            // Raised on critical errors, e.g. connection failures or all brokers down.
            _producer.OnError += (sender, error) =>
            {
                OnError?.Invoke(sender, error);
            };
            // Raised when there is information that should be logged.
            _producer.OnLog += (sender, log) =>
            {
                OnLog?.Invoke(sender, log);
            };
        }

        /// <summary>
        /// Method for creating the key to be used within this producer
        /// </summary>
        /// <returns>An EDXL-DE key containing all information for adapters to understand where this message originates from</returns>
        private EDXLDistribution CreateKey()
        {
            return new EDXLDistribution()
            {
                senderID = _sender,
                distributionID = Guid.NewGuid().ToString(),
                distributionKind = DistributionKind.Unknown,
                distributionStatus = DistributionStatus.Unknown,
                dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
            };
        }

        /// <summary>
        /// Method for sending the given message over the given topic
        /// </summary>
        /// <param name="message">The message to be send</param>
        /// <param name="topic">The name of the topic to send the message over</param>
        internal void SendMessage(T message, string topic)
        {
            // TODO: implement waiting for response or not
            // TODO: implement time out mechanism
            _producer.ProduceAsync(topic, CreateKey(), message);
        }

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
