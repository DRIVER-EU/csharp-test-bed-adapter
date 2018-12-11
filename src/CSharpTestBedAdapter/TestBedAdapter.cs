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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using Confluent.Kafka.Serialization;

using eu.driver.model.core;
using eu.driver.model.edxl;

namespace eu.driver.CSharpTestBedAdapter
{
    /// <summary>
    /// Main C# adapter class that provides an interface for external applications to connect to the DRIVER-EU test-bed (https://github.com/DRIVER-EU/test-bed).
    /// </summary>
    public class TestBedAdapter : IDisposable
    {
        #region Definitions

        /// <summary>
        /// The different states this adapter can be in
        /// </summary>
        public enum States
        {
            /// <summary>
            /// Starting this adapter
            /// </summary>
            Init,
            /// <summary>
            /// DEBUG mode, sending and receiving messages without a test-bed admin tool present
            /// </summary>
            Debug,
            /// <summary>
            /// ENABLED mode, sending and receiving messages with a test-bed admin tool present
            /// </summary>
            Enabled,
            /// <summary>
            /// DISABLED mode, queueing all sent and received messages until the admin tool is present again
            /// </summary>
            Disabled,
        }

        /// <summary>
        /// All information regarding time available to the adapter
        /// </summary>
        public struct TimeInfo
        {
            /// <summary>
            /// The time frame from the start of the trial to the current time
            /// </summary>
            public TimeSpan ElapsedTime { get; set; }
            /// <summary>
            /// The timestamp of the last update
            /// </summary>
            public DateTime UpdatedAt { get; set; }
            /// <summary>
            /// The fictive date and time of the trial
            /// </summary>
            public DateTime TrialTime { get; set; }
            /// <summary>
            /// The speed factor of the trial
            /// </summary>
            public float TrialTimeSpeed { get; set; }
            /// <summary>
            /// The current state of the time service
            /// </summary>
            public Command TimeState { get; set; }

            /// <summary><see cref="ValueType.ToString"/></summary>
            public override string ToString()
            {
                return $"TimeInfo[ElapsedTime({ElapsedTime.ToString()}); UpdatedAt({UpdatedAt.ToString()}); TrialTime({TrialTime}); TrialTimeSpeed({TrialTimeSpeed}); TimeState({TimeState})]";
            }
        }

        #endregion Definitions

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
        /// The consumer this connector is using to receive time messages
        /// </summary>
        private Consumer<EDXLDistribution, Timing> _timeConsumer;
        /// <summary>
        /// The consumer this connector is using to receive time control changes
        /// </summary>
        private Consumer<EDXLDistribution, TimingControl> _timecontrolConsumer;
        /// <summary>
        /// The producer this connector is using to send out a request for creating a topic
        /// </summary>
        private Producer<EDXLDistribution, TopicCreate> _topicCreateProducer;
        /// <summary>
        /// The consumer this connector is using to receive invitations to listen to a certain topic
        /// </summary>
        private Consumer<EDXLDistribution, TopicInvite> _topicInviteConsumer;

        /// <summary>
        /// The general cancellation token for all tasks running in this adapter
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Indication if this adapter is allowed to send or receive standard and custom messages
        /// </summary>
        public States State
        {
            get { return _state; }
            private set
            {
                _state = value;
                Log(log4net.Core.Level.Info, $"State of the adapter set to {_state}");
                // If we are in DEBUG or ENABLED state (again), try re-sending and -receiving the queued messages
                if (_state == States.Enabled || _state == States.Debug)
                {
                    Log(log4net.Core.Level.Info, "Re-doing all queued messages that were sent/received during inactivity of this adapter");
                    foreach (IAbstractProducer producer in _producers.Values)
                    {
                        producer.FlushQueue();
                    }
                    foreach (List<IAbstractConsumer> consumers in _consumers.Values)
                    {
                        foreach (IAbstractConsumer consumer in consumers)
                        {
                            consumer.FlushQueue();
                        }
                    }
                }
            }
        }
        private States _state = States.Init;
        /// <summary>
        /// The timestamp of the last admin heartbeat
        /// </summary>
        private DateTime _lastAdminHeartbeat;

        /// <summary>
        /// The timestamp of when this adapter started (is initialized)
        /// </summary>
        private DateTime _startTime;
        /// <summary>
        /// The time information gathered from the latest time service updates
        /// </summary>
        private TimeInfo _currentTime;

        /// <summary>
        /// The client connected to the large file service
        /// </summary>
        private HttpClient _fileServiceClient = null;

        /// <summary>
        /// The list of topics that this adapter received an invitation to
        /// </summary>
        public List<string> AllowedTopics
        {
            get { return new List<string>(_allowedTopics); }
        }
        private List<string> _allowedTopics;

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
                _timeConsumer = new Consumer<EDXLDistribution, Timing>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<Timing>());
                _timeConsumer.OnError += Adapter_Error;
                _timeConsumer.OnConsumeError += Adapter_ConsumeError;
                _timeConsumer.OnLog += Adapter_Log;
                _timeConsumer.OnMessage += TimeConsumer_Message;
                _timecontrolConsumer = new Consumer<EDXLDistribution, TimingControl>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<TimingControl>());
                _timecontrolConsumer.OnError += Adapter_Error;
                _timecontrolConsumer.OnConsumeError += Adapter_ConsumeError;
                _timecontrolConsumer.OnLog += Adapter_Log;
                _timecontrolConsumer.OnMessage += TimecontrolConsumer_Message;
                _topicInviteConsumer = new Consumer<EDXLDistribution, TopicInvite>(_configuration.ConsumerConfig, new AvroDeserializer<EDXLDistribution>(), new AvroDeserializer<TopicInvite>());
                _topicInviteConsumer.OnError += Adapter_Error;
                _topicInviteConsumer.OnConsumeError += Adapter_ConsumeError;
                _topicInviteConsumer.OnLog += Adapter_Log;
                _topicInviteConsumer.OnMessage += TopicInviteConsumer_Message;

                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancellationTokenSource.Token;

                // Start listening to the topics
                _heartbeatConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["admin-heartbeat"], 0, Offset.End) });
                _timeConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["time"], 0, Offset.End) });
                _timecontrolConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["time-control"], 0, Offset.End) });
                _topicInviteConsumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Configuration.CoreTopics["topic-access-invite"], 0, Offset.End) });

                Task.Factory.StartNew((cancelToken) => { AdminCheck((CancellationToken)cancelToken); }, token, token);
                Task.Factory.StartNew((cancelToken) => { Consume((CancellationToken)cancelToken); }, token, token);

                // Start the heart beat to indicate the connector is still alive
                Task.Factory.StartNew((cancelToken) => { Heartbeat((CancellationToken)cancelToken); }, token, token);

                _lastAdminHeartbeat = DateTime.MinValue;
                _startTime = DateTime.UtcNow;
                _currentTime = new TimeInfo()
                {
                    ElapsedTime = TimeSpan.FromMilliseconds(0),
                    UpdatedAt = DateTime.UtcNow,
                    TrialTime = DateTime.MinValue,
                    TrialTimeSpeed = 1f,
                    TimeState = Command.Start
                };

                _allowedTopics = new List<string>()
                {
                    "csharp-test",
                    "simulation_timecontrol",
                    "simulation_object_deleted",
                    "simulation_entity_item",
                    "simulation_entity_station",
                    "simulation_entity_post",
                    "simulation_connection_unit",
                    "simulation_connection_unit_connection",
                    "simulation_request_unittransport",
                };
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
                senderID = _configuration.Settings.clientid,
                distributionID = Guid.NewGuid().ToString(),
                distributionKind = DistributionKind.Update,
                distributionStatus = DistributionStatus.System,
                dateTimeSent = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                dateTimeExpires = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)) + new TimeSpan(0, 0, 10, 0, 0)).TotalMilliseconds,
            };
        }

        #endregion Initialization

        #region Time

        /// <summary>
        /// Method for retrieving the latest time information from the test-bed
        /// </summary>
        /// <returns>The available time information</returns>
        public TimeInfo GetTimeInfo()
        {
            return _currentTime;
        }

        #endregion Time

        #region Heartbeat

        /// <summary>
        /// Method for starting the heart beat of this adapter
        /// </summary>
        private void Heartbeat(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Send out the heart beat that this connector is still alive
                EDXLDistribution key = CreateCoreKey();
                Heartbeat beat = new Heartbeat
                {
                    id = _configuration.Settings.clientid,
                    alive = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                };

                _heartbeatProducer.ProduceAsync(Configuration.CoreTopics["heartbeat"], key, beat);

                // Wait for the specified amount of milliseconds
                Task wait = Task.Delay(_configuration.Settings.heartbeatinterval);
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
                Log log = new Log() { id = _configuration.Settings.clientid, log = msg };

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

        #region Large file service

        /// <summary>
        /// Method for retrieving the <see cref="HttpClient"/> created to connect to the large file service of the test-bed
        /// </summary>
        /// <returns>The <see cref="HttpClient"/> for using the REST API of the large file service</returns>
        public HttpClient GetLargeFileServiceClient()
        {
            if (_fileServiceClient == null)
            {
                string host = _configuration.Settings.brokerUrl.Substring(0, _configuration.Settings.brokerUrl.IndexOf(':'));
                if (!host.StartsWith("http"))
                {
                    host = "http://" + host;
                }
                Uri largeFileUri = new Uri(host + ":9090");

                _fileServiceClient = new HttpClient();
                _fileServiceClient.BaseAddress = largeFileUri;
                _fileServiceClient.DefaultRequestHeaders.Clear();
                _fileServiceClient.DefaultRequestHeaders.ConnectionClose = false;
                _fileServiceClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                System.Net.ServicePointManager.FindServicePoint(largeFileUri).ConnectionLeaseTimeout = 60 * 1000;
            }

            return _fileServiceClient;
        }

        /// <summary>
        /// Method for letting the adapter upload the file at the given path to the large file service
        /// </summary>
        /// <param name="filePath">The path directing to the file to upload</param>
        /// <param name="dataType">The type of file that is going to be uploaded</param>
        /// <param name="obfuscate">Indication if this should be a private upload, therefore generating a obfuscated link to the uploaded file</param>
        /// <param name="sendToTestbed">Indication if the adapter should automatically update the test-bed via the <see cref="eu.driver.model.core.LargeDataUpdate"/> message</param>
        /// <returns>The <see cref="Task"/> handling the upload, resulting in a <see cref="HttpRequestMessage"/>, or null if the file couldn't be found</returns>
        public Task<HttpResponseMessage> Upload(string filePath, DataType dataType, bool obfuscate, bool sendToTestbed)
        {
            // Make sure the file actually exists
            if (System.IO.File.Exists(filePath))
            {
                // Retrieve the large file service client for the upload
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
                content.Add(new StringContent(obfuscate.ToString().ToLower()), "private");

                // Send the POST
                Task<HttpResponseMessage> res = client.PostAsync("/upload", content);

                // Wait for the task whenever this adapter needs to send the update as well
                if (sendToTestbed)
                {
                    WaitForUploadCompletion(System.IO.Path.GetFileName(filePath), dataType, res);
                }

                return res;
            }
            else return null;
        }

        /// <summary>
        /// Method for sending the <see cref="eu.driver.model.core.LargeDataUpdate"/> message after the upload is completed
        /// </summary>
        /// <param name="fileName">The file name that has been uploaded</param>
        /// <param name="dataType">The data type of the uploaded file</param>
        /// <param name="task">The <see cref="Task"/> that handles the upload</param>
        private async void WaitForUploadCompletion(string fileName, DataType dataType, Task<HttpResponseMessage> task)
        {
            // Wait for a response from the large file service
            HttpResponseMessage response = await task;

            // Check if the POST was a success
            if (response.IsSuccessStatusCode)
            {
                string resContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(resContent))
                {
                    // Retrieve the URL from the upload response
                    string url = resContent.Substring(resContent.IndexOf("\":\"") + 3);
                    url = url.Substring(0, url.IndexOf('"'));

                    // Send the large data update message 
                    LargeDataUpdate message = new LargeDataUpdate()
                    {
                        url = url,
                        title = fileName,
                        description = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds).ToString(),
                        dataType = dataType,
                    };
                    SendMessage<LargeDataUpdate>(message);
                }
            }
        }

        #endregion Large file service

        #region System consumers

        /// <summary>
        /// Method being used inside a new task to keep polling for new system messages to consume
        /// </summary>
        private void Consume(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _timeConsumer.Poll(100);
                _timecontrolConsumer.Poll(100);
                _topicInviteConsumer.Poll(100);
            }
        }

        /// <summary>
        /// Method for checking the admin tool heartbeat
        /// </summary>
        private void AdminCheck(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _heartbeatConsumer.Poll(5000);
                DateTime now = DateTime.UtcNow;

                if (_lastAdminHeartbeat != DateTime.MinValue)
                {
                    TimeSpan span = now - _lastAdminHeartbeat;
                    // If the latest admin heartbeat is from longer than 10 seconds ago, we should disable this adapter
                    if (span.Seconds > 10)
                    {
                        Log(log4net.Core.Level.Info, "Admin tool not found, going into Disabled mode");
                        State = States.Disabled;
                    }
                    // If we have received an admin heartbeat (again) and the time service allows us to, go to the ENABLED state
                    else if (State != States.Enabled && (_currentTime.TimeState == Command.Start || _currentTime.TimeState == Command.Update))
                    {
                        Log(log4net.Core.Level.Info, "Admin tool found (again), going into Enabled mode");
                        State = States.Enabled;
                    }
                }
                else
                {
                    TimeSpan span = now - _startTime;
                    // If in the first 10 seconds of this adapters existance there wasn't an admin heartbeat, go to the DEBUG state and stop listening
                    if (span.Seconds > 10)
                    {
                        Log(log4net.Core.Level.Info, "Admin tool not found, going into Debug mode");
                        State = States.Debug;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic admin heartbeat
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void HeartbeatConsumer_Message(object sender, Message<EDXLDistribution, AdminHeartbeat> message)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(message.Value.alive);
            DateTime timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(span);

            // Store the latest timestamp
            _lastAdminHeartbeat = timestamp;
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic time
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void TimeConsumer_Message(object sender, Message<EDXLDistribution, Timing> message)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan updatedAt = TimeSpan.FromMilliseconds(message.Value.updatedAt);
            TimeSpan trialTime = TimeSpan.FromMilliseconds(message.Value.trialTime);

            // Update the values of the time info
            _currentTime.ElapsedTime = TimeSpan.FromMilliseconds(message.Value.timeElapsed);
            _currentTime.UpdatedAt = baseTime.Add(updatedAt);
            _currentTime.TrialTime = baseTime.Add(trialTime);
            _currentTime.TrialTimeSpeed = message.Value.trialTimeSpeed;
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic time
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void TimecontrolConsumer_Message(object sender, Message<EDXLDistribution, TimingControl> message)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // Update the values of the time info
            if (message.Value.trialTime.HasValue)
            {
                TimeSpan trialTime = TimeSpan.FromMilliseconds(message.Value.trialTime.Value);
                _currentTime.TrialTime = baseTime.Add(trialTime);
            }
            if (message.Value.trialTimeSpeed.HasValue)
            {
                _currentTime.TrialTimeSpeed = message.Value.trialTimeSpeed.Value;
            }
            _currentTime.TimeState = message.Value.command;

            // Update the state of this adapter, based on the time service command
            // This will only have effect on the adapter whenever it recognized the test-bed admin tool present (DEBUG mode doesn't deal with time control)
            switch (_currentTime.TimeState)
            {
                // Whenever starting or updating the time control, this adapter is allowed to send/receive messages
                case Command.Start:
                case Command.Update:
                    if (State == States.Init)
                    {
                        State = States.Enabled;
                    }
                    break;
                // Whenever a pause, stop or reset is issued, this adapter should stop sending/receiving messages
                case Command.Pause:
                case Command.Stop:
                case Command.Reset:
                    if (State == States.Enabled)
                    {
                        State = States.Disabled;
                    }
                    break;
            }
        }

        /// <summary>
        /// Delegate being called once a new message is consumed on the system topic for topic invitations
        /// </summary>
        /// <param name="sender">The consumer that has received the message</param>
        /// <param name="message">The message that was received</param>
        private void TopicInviteConsumer_Message(object sender, Message<EDXLDistribution, TopicInvite> message)
        {
            // Add the topic name to the list to check for sending/receiving messages
            string topic = message.Value.topicName;
            if (!_allowedTopics.Contains(topic))
            {
                Log(log4net.Core.Level.Debug, $"Adapter is allowed to send/receive on topic {topic}");
                _allowedTopics.Add(topic);
        }
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
            // Stop all running tasks
            _cancellationTokenSource.Cancel();
            // Stop the connection with the large file service
            if (_fileServiceClient != null)
            {
                _fileServiceClient.Dispose();
            }

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
            if (_timecontrolConsumer != null)
            {
                _timecontrolConsumer.OnError -= Adapter_Error;
                _timecontrolConsumer.OnConsumeError -= Adapter_ConsumeError;
                _timecontrolConsumer.OnLog -= Adapter_Log;
                _timecontrolConsumer.OnMessage -= TimecontrolConsumer_Message;
                _timecontrolConsumer.Dispose();
            }
            if (_topicInviteConsumer != null)
            {
                _topicInviteConsumer.OnError -= Adapter_Error;
                _topicInviteConsumer.OnConsumeError -= Adapter_ConsumeError;
                _topicInviteConsumer.OnLog -= Adapter_Log;
                _topicInviteConsumer.OnMessage -= TopicInviteConsumer_Message;
                _topicInviteConsumer.Dispose();
            }

            _instance = null;
        }

        #endregion Destruction
    }
}
