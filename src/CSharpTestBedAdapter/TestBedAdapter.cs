﻿/*************************************************************
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

using eu.driver.model.core;
using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// Main C# adapter class that provides an interface for external applications to connect to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    public class TestBedAdapter : IDisposable
    {
        #region Properties & variables

        /// <summary>
        /// Configuration class, including internal setup information and external settings
        /// </summary>
        private Configuration _configuration;
        /// <summary>
        /// The list of created producers, indexed by topic name
        /// </summary>
        private Dictionary<string, IAbstractProducer> _producers;
        /// <summary>
        /// The list of created consumers, indexed by topic name
        /// </summary>
        private Dictionary<string, List<IAbstractConsumer>> _consumers;

        /// <summary>
        /// The producer this connector is using to send heartbeats
        /// </summary>
        private Producer<EDXLDistribution, Heartbeat> _heartbeatProducer;
        /// <summary>
        /// The consumer this connector is using to check if the admin tool is still alive
        /// </summary>
        private Consumer<EDXLDistribution, AdminHeartbeat> _heartbeatConsumer;
        /// <summary>
        /// The producer this connector is using to send logs
        /// </summary>
        private Producer<EDXLDistribution, Log> _logProducer;
        /// <summary>
        /// The consumer this connector is using to receive time control changes
        /// </summary>
        private Consumer<EDXLDistribution, TimingControl> _timeConsumer;
        /// <summary>
        /// The producer this connector is using to send out a request for creating a topic
        /// </summary>
        private Producer<EDXLDistribution, TopicCreate> _topicCreateProducer;
        /// <summary>
        /// The consumer this connector is using to receive invitations to listen to a certain topic
        /// </summary>
        private Consumer<EDXLDistribution, TopicInvite> _topicInviteConsumer;

        #endregion Properties & variables

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

                // Initialize the empty producer and consumer dictionaries
                _producers = new Dictionary<string, IAbstractProducer>();
                _consumers = new Dictionary<string, List<IAbstractConsumer>>();

                // Create the producers for the system topics
                _heartbeatProducer = new Producer<EDXLDistribution, Heartbeat>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Heartbeat>());
                _heartbeatProducer.OnError += Adapter_Error;
                _heartbeatProducer.OnLog += Adapter_Log;
                _logProducer = new Producer<EDXLDistribution, Log>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<Log>());
                _logProducer.OnError += Adapter_Error;
                _logProducer.OnLog += Adapter_Log;
                _topicCreateProducer = new Producer<EDXLDistribution, TopicCreate>(_configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<TopicCreate>());
                _topicCreateProducer.OnError += Adapter_Error;
                _topicCreateProducer.OnLog += Adapter_Log;

                // Initialize the consumers for the system topics
                _heartbeatConsumer = new Consumer<EDXLDistribution, AdminHeartbeat>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<AdminHeartbeat>());
                _heartbeatConsumer.OnError += Adapter_Error;
                _heartbeatConsumer.OnConsumeError += Adapter_ConsumeError;
                _heartbeatConsumer.OnLog += Adapter_Log;
                _heartbeatConsumer.OnMessage += HeartbeatConsumer_Message;
                _timeConsumer = new Consumer<EDXLDistribution, TimingControl>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<TimingControl>());
                _timeConsumer.OnError += Adapter_Error;
                _timeConsumer.OnConsumeError += Adapter_ConsumeError;
                _timeConsumer.OnLog += Adapter_Log;
                _timeConsumer.OnMessage += TimeConsumer_Message;
                _topicInviteConsumer = new Consumer<EDXLDistribution, TopicInvite>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<TopicInvite>());
                _topicInviteConsumer.OnError += Adapter_Error;
                _topicInviteConsumer.OnConsumeError += Adapter_ConsumeError;
                _topicInviteConsumer.OnLog += Adapter_Log;
                _topicInviteConsumer.OnMessage += TopicInviteConsumer_Message;

                // Start listening to the topics
                _heartbeatConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["admin-heartbeat"], 0, Offset.End) });
                _timeConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["time"], 0, Offset.End) });
                _topicInviteConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["topic-access-invite"], 0, Offset.End) });
                // TODO: Add CancellationToken to stop on dispose
                Task.Factory.StartNew(() => { Consume(); });

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
            Log(log4net.Core.Level.Error, $"{sender.GetType()} {error.Code}: {error.Reason}");
        }

        /// <summary>
        /// Collective delegate to report all errors created by consumers when receiving a message
        /// </summary>
        /// <param name="sender">The consumer sending the error</param>
        /// <param name="error">The message being consumed</param>
        private void Adapter_ConsumeError(object sender, Message msg)
        {
            Log(log4net.Core.Level.Error, $"{sender.GetType()} {msg.Error.Code}: {msg.Error.Reason}");
        }

        /// <summary>
        /// Collective delegate to report all logs created by producers and consumers
        /// </summary>
        /// <param name="sender">The producer or consumer sending the log</param>
        /// <param name="error">The actual log from the producer or consumer</param>
        private void Adapter_Log(object sender, LogMessage log)
        {
            // TODO: Possibly map log.Level to log4net.Core.Level?
            Log(log4net.Core.Level.Info, $"{sender.GetType()} {log.Name}: {log.Message}");
        }

        #endregion Log

        #region System consumers

        /// <summary>
        /// Method being used inside a new task to keep polling for new system messages to consume
        /// </summary>
        private void Consume()
        {
            while (true)
            {
                _heartbeatConsumer.Poll(100);
                _timeConsumer.Poll(100);
                _topicInviteConsumer.Poll(100);
            }
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic admin heartbeat
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void HeartbeatConsumer_Message(object sender, Message<EDXLDistribution, AdminHeartbeat> message)
        {
            // TODO: Implement heartbeat checker to only send messages whenever the admin tool is alive
            Log(log4net.Core.Level.Verbose, $"Admin alive at: {message.Value.alive}");
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic time
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void TimeConsumer_Message(object sender, Message<EDXLDistribution, TimingControl> message)
        {
            // TODO: Implement timing control based on these messages
            Log(log4net.Core.Level.Verbose, $"Timing control received: {message.Value.command}");
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic for topic invitations
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void TopicInviteConsumer_Message(object sender, Message<EDXLDistribution, TopicInvite> message)
        {
            // TODO: Implement topic consumption control based on these invites
            Log(log4net.Core.Level.Verbose, $"Topic invite received: {message.Value.topicName}");
        }

        #endregion System consumers

        #region Producer

        /// <summary>
        /// Method for sending out a standard message
        /// </summary>
        /// <typeparam name="T">The type of the standard message, inherited from <see cref="Avro.Specific.ISpecificRecord"/></typeparam>
        /// <param name="message">The standard message to be send</param>
        public void SendMessage<T>(T message)
            where T : Avro.Specific.ISpecificRecord
        {
            // Check if this message is actually a standard message
            if (Configuration.StandardTopics.ContainsKey(typeof(T)))
            {
                // Send over the message
                DoSendMessage<T>(message, Configuration.StandardTopics[typeof(T)]);
            }
            else throw new CommunicationException($"message of type {typeof(T)} does not belong to any supported standard topics");
        }

        /// <summary>
        /// Method for sending a custom message over a custom topic
        /// </summary>
        /// <typeparam name="T">The type of the message, inherited from <see cref="Avro.Specific.ISpecificRecord"/></typeparam>
        /// <param name="message">The message to be send</param>
        /// <param name="topic">The topic name to send the message over</param>
        public void SendMessage<T>(T message, string topic)
            where T : Avro.Specific.ISpecificRecord
        {
            // Make sure we are not sending out messages to standard topics via this method
            if (Configuration.StandardTopics.ContainsValue(topic))
                throw new CommunicationException($"topic ({topic}) is already part of the standard test-bed topics! Choose another topic name");

            DoSendMessage<T>(message, topic);
        }

        /// <summary>
        /// Method for sending a custom message over the given topic
        /// </summary>
        /// <typeparam name="T">The type of the message, inherited from <see cref="Avro.Specific.ISpecificRecord"/></typeparam>
        /// <param name="message">The message to be send</param>
        /// <param name="topic">The topic name to send the message over</param>
        private void DoSendMessage<T>(T message, string topic)
            where T : Avro.Specific.ISpecificRecord
        {
            // Make sure we are not sending out messages to core topics via this method
            if (Configuration.CoreTopics.ContainsKey(topic))
                throw new CommunicationException($"topic ({topic}) is already part of the core test-bed topics! Choose another topic name");

            if (_producers.ContainsKey(topic))
            {
                // Check if the types are matching and send the message if they are
                IAbstractProducer producer = _producers[topic];
                if (producer.MessageType == typeof(T))
                {
                    ((AbstractProducer<T>)producer).SendMessage(message, topic);
                }
                else throw new CommunicationException($"could not send message of type {typeof(T)}, since it is not conform the initial producer message type {producer.MessageType}");
            }
            else
            {
                // Create a new producer for the given topic, sending out messages of the given message type
                AbstractProducer<T> newProducer = new AbstractProducer<T>(_configuration);
                newProducer.OnError += Adapter_Error;
                newProducer.OnLog += Adapter_Log;
                _producers.Add(topic, newProducer);

                // Send the message
                newProducer.SendMessage(message, topic);
            }
        }

        #endregion Producer

        #region Consumer

        /// <summary>
        /// Method for adding a function to receive messages from the given topic
        /// </summary>
        /// <typeparam name="T">The type of the message, inherited from <see cref="Avro.Specific.ISpecificRecord"/></typeparam>
        /// <param name="handler">Delegate function to be called once a message is received</param>
        /// <param name="topic">The name of the topic to listen to</param>
        /// <param name="offset">The <see cref="Confluent.Kafka.Offset"/> to indicate from where to start listening to new messages</param>
        public void AddCallback<T>(ConsumerHandler<T> handler, string topic, Offset offset)
            where T : Avro.Specific.ISpecificRecord
        {
            // Make sure we are not requested to listen to core topics via this method
            if (Configuration.CoreTopics.ContainsKey(topic))
                throw new CommunicationException($"you are not able to listen to ({topic}), since it is part of the core test-bed topics");

            if (_consumers.ContainsKey(topic))
            {
                // Check if the types are matching and add a new consumer if they are
                if (_consumers[topic][0].MessageType != typeof(T))
                {
                    throw new CommunicationException($"could not create consumer type {typeof(T)}, since it is not conform the initial consumer message type {_consumers[topic][0].MessageType}");
                }
            }
            else
            {
                foreach (KeyValuePair<Type, string> kvp in Configuration.StandardTopics)
                {
                    if (kvp.Value == topic && kvp.Key != typeof(T))
                    {
                        throw new CommunicationException($"could not create consumer type {typeof(T)} for stadard topic ({topic}), since it is not conform the initial standard message type {kvp.Key}");
                    }
                }
            }

            // Create a new consumer listening to the given topic
            AbstractConsumer<T> newConsumer = new AbstractConsumer<T>(_configuration, handler, topic, offset);
            newConsumer.OnError += Adapter_Error;
            newConsumer.OnLog += Adapter_Log;
            if (_consumers.ContainsKey(topic))
            {
                _consumers[topic].Add(newConsumer);
            }
            else
            {
                _consumers.Add(topic, new List<IAbstractConsumer>() { newConsumer });
            }
        }
        public delegate void ConsumerHandler<T>(string senderID, string topic, T message)
            where T : Avro.Specific.ISpecificRecord;

        #endregion Consumer

        #region Destruction

        /// <summary><see cref="IDisposable.Dispose"/></summary>
        public void Dispose()
        {
            // Dispose all created producers
            foreach (IAbstractProducer producer in _producers.Values)
            {
                // TODO: unsubsribe from error and log events
                ((IDisposable)producer).Dispose();
            }
            // Dispose all created consumers
            foreach (IAbstractConsumer consumer in _consumers.Values)
            {
                // TODO: unsubsribe from error and log events
                ((IDisposable)consumer).Dispose();
            }

            // Dispose all system producers
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
            if (_topicCreateProducer != null)
            {
                _topicCreateProducer.OnError -= Adapter_Error;
                _topicCreateProducer.OnLog -= Adapter_Log;
                _topicCreateProducer.Dispose();
            }

            // Dispose all system consumers
            if (_heartbeatConsumer != null)
            {
                _heartbeatConsumer.OnError -= Adapter_Error;
                _heartbeatConsumer.OnConsumeError -= Adapter_ConsumeError;
                _heartbeatConsumer.OnLog -= Adapter_Log;
                _heartbeatConsumer.OnMessage -= HeartbeatConsumer_Message;
                _heartbeatConsumer.Dispose();
            }
            if (_timeConsumer != null)
            {
                _timeConsumer.OnError -= Adapter_Error;
                _timeConsumer.OnConsumeError -= Adapter_ConsumeError;
                _timeConsumer.OnLog -= Adapter_Log;
                _timeConsumer.OnMessage -= TimeConsumer_Message;
                _timeConsumer.Dispose();
            }
            if (_topicInviteConsumer != null)
            {
                _topicInviteConsumer.OnError -= Adapter_Error;
                _topicInviteConsumer.OnConsumeError -= Adapter_ConsumeError;
                _topicInviteConsumer.OnLog -= Adapter_Log;
                _topicInviteConsumer.OnMessage -= TopicInviteConsumer_Message;
                _topicInviteConsumer.Dispose();
            }
        }

        #endregion Destruction
    }
}
