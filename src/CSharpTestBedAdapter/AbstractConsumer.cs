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
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using eu.driver.model.edxl;

namespace eu.driver.CSharpTestBedAdapter
{
    internal class AbstractConsumer<T> : IAbstractConsumer, IDisposable
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The concrete consumer to receive messages with
        /// </summary>
        private IConsumer<EDXLDistribution, T> _consumer;
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
        private Queue<ConsumeResult<EDXLDistribution, T>> _messageQueue;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">The test-bed adapter configuration information</param>
        /// <param name="handler">The callback function to invoke whenever a new message has been consumed</param>
        /// <param name="topic">The name of the topci to listen to</param>
        /// <param name="offset">The topic offset to start listening at the correct index</param>
        internal AbstractConsumer(Configuration configuration, TestBedAdapter.ConsumerHandler<T> handler, string topic, Offset offset)
        {
            using (CachedSchemaRegistryClient csrc = new CachedSchemaRegistryClient(configuration.SchemaRegistryConfig))
            {
                _consumer = new ConsumerBuilder<EDXLDistribution, T>(configuration.ConsumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<EDXLDistribution>(csrc).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<T>(csrc).AsSyncOverAsync())
                    // Raised on critical errors, e.g.connection failures or all brokers down.
                    .SetErrorHandler((_, error) => OnError?.Invoke(this, error))
                    // Raised when there is information that should be logged.
                    .SetLogHandler((_, log) => OnLog?.Invoke(this, log))
                    .Build();
            }
            _consumerHandler = handler;

            _messageQueue = new Queue<ConsumeResult<EDXLDistribution, T>>();

            // Start listening to the topic from the given offset
            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, offset) });
            // Add CancellationToken to stop on dispose
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => { Consume(cts.Token); });
        }

        /// <summary>
        /// Method being used inside a new task to keep polling for new messages to consume
        /// </summary>
        private void Consume(CancellationToken cancelToken)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        ConsumeResult<EDXLDistribution, T> res = _consumer.Consume(cancelToken);
                        NotifyConsumption(res);
                    }
                    catch (ConsumeException e)
                    {
                        throw new CommunicationException($"consume error, {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }

        /// <summary>
        /// Method for notifying the connected application of a consumption
        /// </summary>
        /// <param name="message">The message that was consumed</param>
        private void NotifyConsumption(ConsumeResult<EDXLDistribution, T> message)
        {
            if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Enabled || TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug)
            {
                // Make sure this message is allowed to be received from the topic
                if (TestBedAdapter.GetInstance().State == TestBedAdapter.States.Debug || TestBedAdapter.GetInstance().AllowedTopics.Contains(message.Topic))
                {
                    _consumerHandler?.Invoke(message.Key.senderID, message.Topic, message.Value);
                }
            }
            else
            {
                _messageQueue.Enqueue(message);
            }
        }

        /// <summary><see cref="IAbstractConsumer.FlushQueue"/></summary>
        public void FlushQueue()
        {
            // Make sure that we are not getting in an endless loop of message receiving if the adapter is somehow disabled again
            int totalMessages = _messageQueue.Count;
            for (int i = 0; i < totalMessages; i++)
            {
                ConsumeResult<EDXLDistribution, T> message = _messageQueue.Dequeue();
                NotifyConsumption(message);
            }
        }

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}
