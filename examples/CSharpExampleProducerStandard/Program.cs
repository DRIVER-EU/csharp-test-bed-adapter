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

using log4net.Core;

using CSharpTestBedAdapter;
// Namespace from the StandardMessages project
using eu.driver.model.cap;

namespace CSharpExampleProducerStandard
{
    /// <summary>
    /// Example class for setting up a simple DRIVER-EU test-bed producer for sending over custom messages (in this case <see cref="Test"/>)
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main call
        /// </summary>
        /// <param name="args">Additional method call parameters</param>
        static void Main(string[] args)
        {
            try
            {
                TestBedAdapter.GetInstance().AddLogCallback(Adapter_Log);
                TestBedAdapter.GetInstance().Log(Level.Debug, "adapter started, listening to input...");

                Console.WriteLine($"Please type in any text to be send over the standard CAP topic; q to exit");
                string text;
                // Whenever a new text has been entered by the user, create a new test message, or quit the application if the text is 'q'
                while ((text = Console.ReadLine()) != "q")
                {
                    // Create a new Alert message, with the given text as info[0].@event
                    Alert newMsg = new Alert
                    {
                        identifier = "test",
                        sender = "CSharpExampleProducerStandard",
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

                    // Send the message over the standard topic
                    TestBedAdapter.GetInstance().SendMessage<Alert>(newMsg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
