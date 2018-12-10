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
using System.Collections.Generic;

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

using eu.driver.model.edxl;

namespace eu.driver.CSharpTestBedAdapter
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
        /// The message queue to be sent as soon as the adapter is enabled
        /// </summary>
        private Queue<KeyValuePair<T, string>> messageQueue;

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

            messageQueue = new Queue<KeyValuePair<T, string>>();
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
                dateTimeSent = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                dateTimeExpires = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)) + new TimeSpan(0, 0, 10, 0, 0)).TotalMilliseconds,
            };
        }

        /// <summary>
        /// Method for sending the given message over the given topic
        /// </summary>
        /// <param name="message">The message to be send</param>
        /// <param name="topic">The name of the topic to send the message over</param>
        internal void SendMessage(T message, string topic)
        {
            // Only send the message whenever the adapter is enabled
            if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Enabled || TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug)
            {
                // TODO: implement waiting for response or not
                // TODO: implement time out mechanism
                // Make sure this message is allowed to be sent on the topic
                if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug || TestBedAdapter.GetInstance().AllowedTopics.Contains(topic))
                {
                    _producer.ProduceAsync(topic, CreateKey(), message);
                }
                else throw new CommunicationException($"cannot send message, since the topic ({topic}) is restricted");
            }
            else
            {
                // If this isn't the case, report and queue the message for sending later
                TestBedAdapter.GetInstance().Log(log4net.Core.Level.Notice, $"Could not send message to topic {topic}, because adapter is disabled! Enqueuing message for sending later.");
                messageQueue.Enqueue(new KeyValuePair<T, string>(message, topic));
            }
        }

        /// <summary><see cref="IAbstractProducer.FlushQueue"/></summary>
        public void FlushQueue()
        {
            // Make sure that we are not getting in an endless loop of message sending if the adapter is somehow disabled again
            int totalMessages = messageQueue.Count;
            for (int i = 0; i < totalMessages; i++)
            {
                KeyValuePair<T, string> message = messageQueue.Dequeue();
                SendMessage(message.Key, message.Value);
            }
        }

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
