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
using System.Threading.Tasks;

using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

using eu.driver.model.edxl;

namespace eu.driver.CSharpTestBedAdapter
{
    internal class AbstractProducer<T> : IAbstractProducer, IDisposable
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The concrete producer to send messages with
        /// </summary>
        private IProducer<EDXLDistribution, T> _producer;
        /// <summary>
        /// The configuration of this test-bed adapter
        /// </summary>
        private Configuration _configuration;

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
        private Queue<KeyValuePair<T, string>> _messageQueue;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">The test-bed adapter configuration information</param>
        internal AbstractProducer(Configuration configuration)
        {
            _configuration = configuration;
            _producer = new ProducerBuilder<EDXLDistribution, T>(_configuration.ProducerConfig)
                .SetKeySerializer(new AvroSerializer<EDXLDistribution>(configuration.SchemaRegistryClient))
                .SetValueSerializer(new AvroSerializer<T>(configuration.SchemaRegistryClient))
                // Raised on critical errors, e.g. connection failures or all brokers down.
                .SetErrorHandler( (sender, error) => OnError?.Invoke(sender, error) )
                // Raised when there is information that should be logged.
                .SetLogHandler( (sender, log) => OnLog?.Invoke(sender, log) )
                .Build();

            _messageQueue = new Queue<KeyValuePair<T, string>>();
        }

        /// <summary>
        /// Method for creating the key to be used within this producer
        /// </summary>
        /// <returns>An EDXL-DE key containing all information for adapters to understand where this message originates from</returns>
        private EDXLDistribution CreateKey()
        {
            return new EDXLDistribution()
            {
                senderID = _configuration.Settings.clientid,
                distributionID = Guid.NewGuid().ToString(),
                distributionKind = DistributionKind.Unknown,
                distributionStatus = DistributionStatus.Unknown,
                dateTimeSent = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                dateTimeExpires = (DateTimeOffset.UtcNow + new TimeSpan(0, 0, 10, 0, 0)).ToUnixTimeMilliseconds(),
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
                // Make sure this message is allowed to be sent on the topic
                if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug || TestBedAdapter.GetInstance().AllowedTopicsSend.Contains(topic))
                {
                    // Send the message
                    Message<EDXLDistribution, T> m = new Message<EDXLDistribution, T>()
                    {
                        Key = CreateKey(),
                        Value = message,
                    };
                    Task<DeliveryResult<EDXLDistribution, T>> task = _producer.ProduceAsync(topic, m);
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted) OnError?.Invoke(this, new Error(ErrorCode.Local_Fail, t.Exception.ToString()));
                    });
                    // Wait for sending this message if the settings property was set
                    if (_configuration.Settings.sendsync)
                    {
                        task.Wait();
                    }
                }
                else throw new CommunicationException($"cannot send message, since the topic ({topic}) is restricted");
            }
            else
            {
                // If this isn't the case, report and queue the message for sending later
                TestBedAdapter.GetInstance().Log(log4net.Core.Level.Notice, $"Could not send message to topic {topic}, because adapter is disabled! Enqueuing message for sending later.");
                _messageQueue.Enqueue(new KeyValuePair<T, string>(message, topic));
            }
        }

        /// <summary><see cref="IAbstractProducer.FlushQueue"/></summary>
        public void FlushQueue()
        {
            // Make sure that we are not getting in an endless loop of message sending if the adapter is somehow disabled again
            int totalMessages = _messageQueue.Count;
            for (int i = 0; i < totalMessages; i++)
            {
                KeyValuePair<T, string> message = _messageQueue.Dequeue();
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
