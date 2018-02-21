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

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// Specific wrapper class around the <see cref="Confluent.Kafka.Consumer{TKey, TValue}"/> to be connected to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    /// <typeparam name="T">The <see cref="Avro.Specific.ISpecificRecord"/> that represents the message format to be received</typeparam>
    public class CSharpConsumer<T> : CSharpConnector
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The actual consumer this wrapper is using to receive messages
        /// </summary>
        private Consumer<EDXLDistribution, T> _consumer;
        /// <summary>
        /// The name of the topic this consumer needs to receive messages
        /// </summary>
        private string _topic;

        /// <summary>
        /// Event being triggered whenever a connection error occurred
        /// <para>Raised on critical errors, e.g. connection failures or all brokers down</para>
        /// </summary>
        public event EventHandler<Error> OnError;
        /// <summary>
        /// Event being triggered whenever a consumption error occurred
        /// <para>Raised on deserialization errors or when a consumed message has an error != NoError</para>
        /// </summary>
        public event EventHandler<Message> OnConsumeError;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="appName">The name of the application creating this consumer</param>
        /// <param name="topic">The name of the topic this consumer should receive its messages from</param>
        /// <param name="offset">The starting position of this consumer to receive messages from the given topic (0 all messages recorded on the topic; -1 for only the newly recorded messages on the topic)</param>
        public CSharpConsumer(string appName, string topic, long offset)
            : base(appName)
        {
            try
            {
                _topic = topic;
                // Create a new Confluent.Kafka.Consumer with a generic key (containing envelope information) and as value the specified Avro record
                _consumer = new Consumer<EDXLDistribution, T>(Configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<T>());
                // Configure the consumer so it will start at the specified topic and offset
                // TODO: Maybe open up the partition as well for the user to set?
                _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_topic, 0, offset) });

                // Raised on critical errors, e.g. connection failures or all brokers down.
                _consumer.OnError += (sender, error) =>
                {
                    OnError?.Invoke(sender, error);
                };
                // Raised on deserialization errors or when a consumed message has an error != NoError.
                _consumer.OnConsumeError += (sender, error) =>
                {
                    OnConsumeError?.Invoke(sender, error);
                };
            }
            catch (Exception e)
            {
                // Log error via connector
                this.Log(log4net.Core.Level.Critical, e.ToString());
            }
        }

        /// <summary>
        /// Method for allowing the consumer to process a possible new message recorded on the topic
        /// </summary>
        /// <param name="message">The deserialized new message to be consumed</param>
        /// <param name="timeout">The amount of time the consumer is allowed to process</param>
        /// <returns>True if a new message is consumed, false otherwise</returns>
        public bool Consume(out Message<EDXLDistribution, T> message, TimeSpan timeout)
        {
            try
            {
                return _consumer.Consume(out message, timeout);
            }
            catch (Exception e)
            {
                // Log error via connector
                this.Log(log4net.Core.Level.Error, e.ToString());
                message = null;
                return false;
            }
        }

        /// <summary><see cref="CSharpConnector.Dispose"/></summary>
        // TODO: Implement proper disposal of this class instance
        public override void Dispose()
        {
            base.Dispose();

            if (_consumer != null)
            {
                _consumer.Dispose();
            }
        }
    }
}
