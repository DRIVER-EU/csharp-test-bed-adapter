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

namespace CSharpTestBedAdapter
{
    /// <summary>
    /// Main entry class that allows programmers of external applications to create new Kafka <see cref="CSharpProducer{T}"/>s and <see cref="CSharpConsumer{T}"/>s
    /// that make it possible to connect to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    public class TestBedAdapter
    {
        /// <summary>
        /// Method for creating a new Kafka producer that is attached and ready to send messages to the DRIVER-EU test-bed
        /// </summary>
        /// <typeparam name="T">The <see cref="Avro.Specific.ISpecificRecord"/> that represents the message format to be sent over</typeparam>
        /// <param name="appName">The name of the application creating this producer</param>
        /// <param name="topic">The name of the topic this producer should send its messages towards</param>
        /// <returns>A newly created Kafka producer, set up correctly for sending over messages of the given type, on the Kafka topic with the given name</returns>
        public static CSharpProducer<T> CreateProducer<T>(string appName, string topic)
            where T : Avro.Specific.ISpecificRecord
        {
            return new CSharpProducer<T>(appName, topic);
        }

        /// <summary>
        /// Method for creating a new Kafka consumer that is attached and ready to receive messages from the DRIVER-EU test-bed
        /// </summary>
        /// <typeparam name="T">The <see cref="Avro.Specific.ISpecificRecord"/> that represents the message format to receive</typeparam>
        /// <param name="appName">The name of the application creating this consumer</param>
        /// <param name="topic">The name of the topic this consumer should receive its messages from</param>
        /// <param name="offset">The starting position of this consumer to receive messages from the given topic (0 all messages recorded on the topic; -1 for only the newly recorded messages on the topic)</param>
        /// <returns>A newly created Kafka consumer, set up correctly for receiving messages of the given type, on the Kafka topic with the given name</returns>
        public static CSharpConsumer<T> CreateConsumer<T>(string appName, string topic, long offset)
            where T : Avro.Specific.ISpecificRecord
        {
            return new CSharpConsumer<T>(appName, topic, offset);
        }
    }
}
