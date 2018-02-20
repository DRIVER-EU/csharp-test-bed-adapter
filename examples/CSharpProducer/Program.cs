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

using eu.driver.model.cap;

namespace CSharpExampleProducer
{
    /// <summary>
    /// Example class for setting up a simple DRIVER-EU test-bed producer
    /// </summary>
    class Program
    {
        /// <summary>
        /// The name of the topic to send messages towards
        /// </summary>
        private static readonly string Topic = "cap";
        /// <summary>
        /// The name of this application
        /// </summary>
        private static readonly string AppName = "CSharpExampleProducer";

        /// <summary>
        /// Main call
        /// </summary>
        /// <param name="args">Additional method call parameters</param>
        static void Main(string[] args)
        {
            // Create a producer for this application, sending out Alert CAP messages on specified topic
            using (CSharpTestBedAdapter.CSharpProducer<Alert> producer = CSharpTestBedAdapter.TestBedAdapter.CreateProducer<Alert>(AppName, Topic))
            {
                if (producer == null) return;

                Console.WriteLine($"new producer for application {AppName} producing on {Topic}. q to exit.");

                string text;
                // Whenever a new text has been entered by the user, create a new Alert CAP message, or quit the application if the text is 'q'
                while ((text = Console.ReadLine()) != "q")
                {
                    // Create a new Alert message, with the given text as info[0].@event
                    Alert alert = new Alert
                    {
                        identifier = "test",
                        sender = AppName,
                        sent = DateTime.UtcNow.ToString("o"),
                        status = Status.Test,
                        msgType = MsgType.Alert,
                        source = "null",
                        scope = Scope.Public,
                        restriction = "null",
                        addresses = "null",
                        code = new string[] { "a", "b", "c" },
                        note = "null",
                        references = "null",
                        incidents = "null",
                        info = new Info[]
                        {
                        new Info {
                            language = "en-US",
                            category = new Category[] { Category.Env, Category.Transport },
                            @event = text,
                            responseType = new ResponseType[] { ResponseType.Assess, ResponseType.Execute },
                            urgency = Urgency.Unknown,
                            severity = Severity.Minor,
                            certainty = Certainty.Likely,
                            audience = "null",
                            eventCode = new ValueNamePair[]
                            {
                                new ValueNamePair { valueName = "test", value = "OK" },
                            },
                            effective = "null",
                            onset = "null",
                            expires = "null",
                            senderName = "null",
                            headline = "null",
                            description = "null",
                            instruction = "null",
                            web = "null",
                            contact = "null",
                            parameter = new ValueNamePair[]
                            {
                                new ValueNamePair { valueName = "test2", value = "OK2" },
                            },
                            resource = new Resource[]
                            {
                                new Resource
                                {
                                    resourceDesc = "testResource",
                                    //size = null,
                                    uri = "null",
                                    deferUri = "null",
                                    digest = "null",
                                },
                            },
                            area = new Area
                            {
                                areaDesc = "testArea",
                                polygon = "null",
                                circle = "null",
                                geocode = null,
                                //altitude = null,
                                //ceiling = null,
                            },
                        },
                        },
                    };

                    // Send the message
                    Task<Message<eu.driver.model.edxl.EDXLDistribution, Alert>> task = producer.ProduceAsync(alert);
                    if (task != null)
                    {
                        // Wait for the message to be added to the Kafka topic, and report that to the user
                        Message<eu.driver.model.edxl.EDXLDistribution, Alert> deliveryReport = task.Result;
                        Console.WriteLine($"Message sent: Topic: {deliveryReport.Topic}, Partition: {deliveryReport.Partition}, Offset: {deliveryReport.Offset}");
                    }
                }
            }
        }
    }
}
