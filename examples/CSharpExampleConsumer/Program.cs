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

using eu.driver.CSharpTestBedAdapter;
// Namesapce from the CommonMessages project
using eu.driver.model.test;
// Namespace from the StandardMessages project
using eu.driver.model.cap;

namespace CSharpExampleConsumer
{
    /// <summary>
    /// Example class for setting up a simple DRIVER-EU test-bed consumer for receiving custom messages (in this case <see cref="Test"/>)
    /// </summary>
    class Program
    {
        private static readonly string CustomTopicName = "csharp-test";

        /// <summary>
        /// Main call
        /// </summary>
        /// <param name="args">Additional method call parameters</param>
        static void Main(string[] args)
        {
            try
            {
                TestBedAdapter.GetInstance().AddLogCallback(Adapter_Log);
                TestBedAdapter.GetInstance().AddCallback<Test>(Adapter_TestMessage, CustomTopicName, Confluent.Kafka.Offset.Beginning);
                TestBedAdapter.GetInstance().AddCallback<Alert>(Adapter_AlertMessage, Configuration.StandardTopics[typeof(Alert)], Confluent.Kafka.Offset.Beginning);
                TestBedAdapter.GetInstance().Log(log4net.Core.Level.Debug, "adapter started, listening to messages...");
                while (true)
                { }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Delegate being called once a new <see cref="Test"/> message is received
        /// </summary>
        /// <param name="senderID">The sender of the message</param>
        /// <param name="topic">The topic this message was received from</param>
        /// <param name="message">The actual message that was received</param>
        private static void Adapter_TestMessage(string senderID, string topic, Test message)
        {
            Console.WriteLine($"Message received on topic ({topic}) from sender ({senderID}) :: ({message.sender}, {message.message})");
        }

        /// <summary>
        /// Delegate being called once a new standard <see cref="Alert"/> message is received
        /// </summary>
        /// <param name="senderID">The sender of the message</param>
        /// <param name="topic">The topic this message was received from</param>
        /// <param name="message">The actual message that was received</param>
        private static void Adapter_AlertMessage(string senderID, string topic, Alert message)
        {
            string text = "<something went wrong>";
            if (message.info is Info)
            {
                Info info = (Info)message.info;
                text = info.@event;
            }
            else if (message.info is List<Info>)
            {
                List<Info> infos = (List<Info>)message.info;
                if (infos.Count > 0)
                {
                    text = infos[0].@event;
                }
            }
            Console.WriteLine($"Message received on topic ({topic}) from sender ({senderID}) :: ({message.sender}, {text})");
        }

        /// <summary>
        /// Delegate being called once the adapter has a log to report
        /// </summary>
        /// <param name="message">The log to report</param>
        private static void Adapter_Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
