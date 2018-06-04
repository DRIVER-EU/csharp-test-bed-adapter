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
using Confluent.Kafka.Serialization;

using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    internal class AbstractConsumer<T> : IAbstractConsumer, IDisposable
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The concrete consumer to receive messages with
        /// </summary>
        private Consumer<EDXLDistribution, T> _consumer;
        /// <summary>
        /// The delegate to be called once a message is consumed
        /// </summary>
        private TestBedAdapter.ConsumerHandler<T> _consumerHandler;

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
        /// The message queue to be processed as soon as the adapter is enabled
        /// </summary>
        private Queue<KeyValuePair<object, Message<EDXLDistribution, T>>> messageQueue;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">The test-bed adapter configuration information</param>
        /// <param name="handler">The callback function to invoke whenever a new message has been consumed</param>
        /// <param name="topic">The name of the topci to listen to</param>
        /// <param name="offset">The topic offset to start listening at the correct index</param>
        internal AbstractConsumer(Configuration configuration, TestBedAdapter.ConsumerHandler<T> handler, string topic, Offset offset)
        {
            _consumer = new Consumer<EDXLDistribution, T>(configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<T>());
            _consumerHandler = handler;

            // Connect the consumer via a message delegate to receive messages
            _consumer.OnMessage += Consumer_Message;
            // Raised on critical errors, e.g. connection failures or all brokers down.
            _consumer.OnError += (sender, error) =>
            {
                OnError?.Invoke(sender, error);
            };
            // Raised when there is information that should be logged.
            _consumer.OnLog += (sender, log) =>
            {
                OnLog?.Invoke(sender, log);
            };

            messageQueue = new Queue<KeyValuePair<object, Message<EDXLDistribution, T>>>();

            // Start listening to the topic from the given offset
            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, offset) });
            // TODO: Add CancellationToken to stop on dispose
            Task.Factory.StartNew(() => { Consume(); });
        }

        /// <summary>
        /// Method being used inside a new task to keep polling for new messages to consume
        /// </summary>
        private void Consume()
        {
            while (true)
            {
                _consumer.Poll(100);
            }
        }

        /// <summary>
        /// Delegate being called once a new message is consumed
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void Consumer_Message(object sender, Message<EDXLDistribution, T> message)
        {
            if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Enabled || TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug)
            {
                _consumerHandler?.Invoke(message.Key.senderID, message.Topic, message.Value);
            }
            else
            {
                messageQueue.Enqueue(new KeyValuePair<object, Message<EDXLDistribution, T>>(sender, message));
            }
        }

        /// <summary><see cref="IAbstractConsumer.FlushQueue"/></summary>
        public void FlushQueue()
        {
            // Make sure that we are not getting in an endless loop of message receiving if the adapter is somehow disabled again
            int totalMessages = messageQueue.Count;
            for (int i = 0; i < totalMessages; i++)
            {
                KeyValuePair<object, Message<EDXLDistribution, T>> message = messageQueue.Dequeue();
                Consumer_Message(message.Key, message.Value);
            }
        }

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            _consumer.OnMessage -= Consumer_Message;
            _consumer.Dispose();
        }
    }
}
