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

using CSharpTestBedAdapter;

using log4net.Core;

using eu.driver.model.test;

namespace CSharpExampleProducerCustom
{
    /// <summary>
    /// Example class for setting up a simple DRIVER-EU test-bed producer for sending over custom messages (in this case <see cref="Test"/>)
    /// </summary>
    class Program
    {
        private static readonly string SenderName = "CSharpExampleProducerCustom";
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
                TestBedAdapter.GetInstance().Log(Level.Debug, "adapter started, listening to input...");

                Console.WriteLine($"Please type in any text to be send over topic '{CustomTopicName}'; q to exit");
                string text;
                // Whenever a new text has been entered by the user, create a new test message, or quit the application if the text is 'q'
                while ((text = Console.ReadLine()) != "q")
                {
                    Test newMsg = new Test()
                    {
                        sender = SenderName,
                        message = text,
                    };

                    // Send the message over our custom topic
                    TestBedAdapter.GetInstance().SendMessage<Test>(newMsg, CustomTopicName);
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
