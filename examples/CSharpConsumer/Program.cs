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



namespace CSharpExampleConsumer
{
    /// <summary>
    /// Example class for setting up a simple DRIVER-EU test-bed consumer
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main call
        /// </summary>
        /// <param name="args">Additional method call parameters</param>
        static void Main(string[] args)
        {
            //// Create a consumer for this application, listening to Alert CAP messages on specified topic
            //using (CSharpTestBedAdapter.CSharpConsumer<Alert> consumer = CSharpTestBedAdapter.TestBedAdapter.CreateConsumer<Alert>(AppName, Topic, 0))
            //{
            //    if (consumer == null) return;
            //    // Subscribe to connection error event channel
            //    consumer.OnError += (sender, error) =>
            //    {
            //        Console.WriteLine($"Error: {error.Reason}");
            //    };
            //    // Subscribe to deserialization error event channel
            //    consumer.OnConsumeError += (sender, msg) =>
            //    {
            //        Console.WriteLine($"ConsumeError: {msg.Error.Reason}");
            //    };

            //    // Keep listening
            //    while (true)
            //    {
            //        Message<eu.driver.model.edxl.EDXLDistribution, Alert> msg;
            //        // Whenever a new message is received, report to console
            //        if (consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
            //        {
            //            string text = "<something went wrong>";
            //            Alert alert = msg.Value;
            //            if (alert.info is Info)
            //            {
            //                Info info = (Info)alert.info;
            //                text = info.@event;
            //            }
            //            else if (alert.info is List<Info>)
            //            {
            //                List<Info> infos = (List<Info>)alert.info;
            //                if (infos.Count > 0)
            //                {
            //                    text = infos[0].@event;
            //                }
            //            }

            //            Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Key.senderID} :: {text}");
            //        }
            //    }
            //}
        }
    }
}
