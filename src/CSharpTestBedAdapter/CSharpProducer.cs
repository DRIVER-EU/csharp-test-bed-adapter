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

using eu.driver.model.edxl;

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// Specific wrapper class around the <see cref="Confluent.Kafka.Producer{TKey, TValue}"/> to be connected to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    /// <typeparam name="T">The <see cref="Avro.Specific.ISpecificRecord"/> that represents the message format to be sent over</typeparam>
    public class CSharpProducer<T> : CSharpConnector
        where T : Avro.Specific.ISpecificRecord
    {
        /// <summary>
        /// The actual producer this wrapper is using to send messages
        /// </summary>
        private Producer<EDXLDistribution, T> _producer;
        /// <summary>
        /// The name of the topic this producer needs to send messages towards
        /// </summary>
        private string _topic;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="appName">The name of the application creating this producer</param>
        /// <param name="topic">The name of the topic this producer should send its messages towards</param>
        public CSharpProducer(string appName, string topic)
            : base (appName)
        {
            try
            {
                _topic = topic;
                // Create a new Confluent.Kafka.Producer with a generic key (containing envelope information) and as value the specified Avro record
                _producer = new Producer<EDXLDistribution, T>(Configuration.ProducerConfig, new AvroSerializer<EDXLDistribution>(), new AvroSerializer<T>());
            }
            catch (Exception e)
            {
                // Log error via connector
                this.Log(log4net.Core.Level.Critical, e.ToString());
            }
        }

        /// <summary>
        /// Method to send the given message towards the specified topic
        /// </summary>
        /// <param name="message">The message to be sent over</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task"/> responsible for sending the message towards the topic, null if an error ocurred</returns>
        public Task<Message<EDXLDistribution, T>> ProduceAsync(T message)
        {
            try
            {
                EDXLDistribution key = new EDXLDistribution()
                {
                    senderID = _appName,
                    distributionID = Guid.NewGuid().ToString(),
                    distributionKind = DistributionKind.Report,
                    distributionStatus = DistributionStatus.Test,
                    dateTimeSent = DateTime.UtcNow.Ticks / 10000,
                    dateTimeExpires = (DateTime.UtcNow.Ticks + 600000000) / 10000,
                };
                return _producer.ProduceAsync(_topic, key, message);
            }
            catch (Exception e)
            {
                // Log error via connector
                this.Log(log4net.Core.Level.Error, e.ToString());
                return null;
            }
        }

        /// <summary><see cref="CSharpConnector.Dispose"/></summary>
        // TODO: Implement proper disposal of this class instance
        public override void Dispose()
        {
            base.Dispose();

            if (_producer != null)
            {
                _producer.Dispose();
            }
        }
    }
}
