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
using System.Net.Http;

using log4net.Core;

using eu.driver.CSharpTestBedAdapter;
// Namesapce from the CommonMessages project
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

                // Test the large file system access
                TestLargeFileService();

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
                    Console.WriteLine(TestBedAdapter.GetInstance().GetTimeInfo());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                TestBedAdapter.GetInstance().Dispose();
            }
        }

        /// <summary>
        /// Method for testing the large file service upload functionality
        /// </summary>
        private static async void TestLargeFileService()
        {
            string filePath = @".\test.jpg";

            if (System.IO.File.Exists(filePath))
            {
                // Retrieve the large file service client for a manual upload
                System.Net.Http.HttpClient client = TestBedAdapter.GetInstance().GetLargeFileServiceClient();

                // Create and enter the POST parameters
                MultipartFormDataContent content = new MultipartFormDataContent();
                // the file to upload
                StreamContent file = new StreamContent(System.IO.File.OpenRead(filePath));
                file.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "uploadFile",
                    FileName = System.IO.Path.GetFileName(filePath),
                };
                content.Add(file);
                // the indication if this upload needs to be obfuscated or not
                content.Add(new StringContent("false"), "private");

                // Send the POST
                HttpResponseMessage response = await client.PostAsync("/upload", content);
                // Check for the response of the large file service
                if (response.IsSuccessStatusCode)
                {
                    string resContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(resContent))
                    {
                        Console.WriteLine(resContent);
                    }
                    else
                    {
                        Console.WriteLine("NO RESPONSE");
                    }
                }
                else
                {
                    Console.WriteLine("FAILURE");
                }

                //// Or do this all automatically via the adapter
                //HttpResponseMessage response = await TestBedAdapter.GetInstance().Upload(filePath, eu.driver.model.core.DataType.image_jpeg, true, true);
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
