/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Commons.Diagnostics.NamedPipeServer;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public partial class NamedPipeTraceListener : TraceListener, IDisposable
    {
        private static readonly object[] _emptyArgs = {};
        private const int DEFAULT_TRACE_ID = short.MaxValue;
        private const string ISO8601_FORMAT_STRING = "O";
        private const string FAIL_MESSAGE_TEMPLATE = "{0} ({1})";

        private const string MESSAGE_NAMEDPIPE_CONNECTING = "NamedPipeTraceListener: Connecting to '{0}' ...";
        private const string MESSAGE_NAMEDPIPE_NOT_CONNECTED = "NamedPipeTraceListener: Pipe '{0}' is not connected.";
        private const string MESSAGE_NAMEDPIPE_EXCEPTION = "NamedPipeTraceListener: {0}: {1}\r\n{2}";

        private volatile bool abortMessageProc;

        public const int CIRCULAR_QUEUE_CAPACITY_DEFAULT = 4096;
        public const string SUPPORTED_ATTRIBUTE_CAPACITY = "capacity";

        public const string SOURCE_NAME_DEFAULT = Logger.DEFAULT_TRACESOURCE_NAME;
        public const string SUPPORTED_ATTRIBUTE_SOURCE = "source";

        private const string SERVER_NAME = ".";
        private const int NAMED_PIPE_CONNECT_AND_DEQUEUE_TIMEOUT_MS = 5 * 1000;
        private const int ABORT_EVENT_TIMEOUT_CHECK_MS = 15 * 1000;

        public string PipeName { get; private set; }

        public string Source { get; private set; }

        public ulong DiscardedItems
        {
            get { return messages.DiscardedItems; }
        }

        public int BufferedItems
        {
            get { return messages.AvailableItems; }
        }

        private CircularQueue<Item> messages;

        public NamedPipeTraceListener()
            : this(NamedPipeServerTraceWriter.NAMED_PIPE_NAME_DEFAULT)
        {
            // N/A
        }

        public NamedPipeTraceListener(string name)
            : base(name)
        {
            PipeName = !string.IsNullOrWhiteSpace(name)
                ? name
                : NamedPipeServerTraceWriter.NAMED_PIPE_NAME_DEFAULT;

            ThreadPool.QueueUserWorkItem(Initialise, this);
        }

        // this method is needed as we cannot access the attributes in the constructor
        public static void Initialise(object stateInfo)
        {
            var instance = stateInfo as NamedPipeTraceListener;
            Contract.Assert(null != instance);

            instance.Source = instance.Attributes.ContainsKey(SUPPORTED_ATTRIBUTE_SOURCE)
                ? instance.Attributes[SUPPORTED_ATTRIBUTE_SOURCE]
                : SOURCE_NAME_DEFAULT;

            var capacity = instance.Attributes.ContainsKey(SUPPORTED_ATTRIBUTE_CAPACITY)
                ? int.Parse(instance.Attributes[SUPPORTED_ATTRIBUTE_CAPACITY])
                : CIRCULAR_QUEUE_CAPACITY_DEFAULT;

            instance.messages = new CircularQueue<Item>(capacity);

            ThreadPool.QueueUserWorkItem(DequeueAndWriteMessageProc, instance);
        }

        public static void DequeueAndWriteMessageProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var instance = stateInfo as NamedPipeTraceListener;
            Contract.Assert(null != instance);

            var sw = Stopwatch.StartNew();
            for(;;)
            {
                try
                {
                    using (var client = new NamedPipeClientStream(SERVER_NAME, instance.PipeName, PipeDirection.Out))
                    {
                        new DefaultTraceListener().WriteLine(string.Format(MESSAGE_NAMEDPIPE_CONNECTING, instance.PipeName));

                        client.Connect(NAMED_PIPE_CONNECT_AND_DEQUEUE_TIMEOUT_MS);
                        client.ReadMode = PipeTransmissionMode.Message;

                        var messageHandler = new MessageHandler(client);

                        for (;;)
                        {
                            if (!client.IsConnected)
                            {
                                new DefaultTraceListener().WriteLine(string.Format(MESSAGE_NAMEDPIPE_NOT_CONNECTED, instance.PipeName));
                                break;
                            }

                            Item item;
                            var result = instance.messages.TryDequeue(out item, NAMED_PIPE_CONNECT_AND_DEQUEUE_TIMEOUT_MS);

                            if (!result || ABORT_EVENT_TIMEOUT_CHECK_MS < sw.ElapsedMilliseconds)
                            {
                                sw.Restart();
                                if (instance.abortMessageProc)
                                {
                                    return;
                                }

                                continue;
                            }

                            messageHandler.Write(item.ToString());
                        }
                    }
                }
                catch (TimeoutException)
                {
                    Thread.Sleep(NAMED_PIPE_CONNECT_AND_DEQUEUE_TIMEOUT_MS);
                }
                catch (Exception ex)
                {
                    new DefaultTraceListener().WriteLine(string.Format(MESSAGE_NAMEDPIPE_EXCEPTION, ex.GetType().Name, ex.Message, ex.StackTrace));

                    Thread.Sleep(NAMED_PIPE_CONNECT_AND_DEQUEUE_TIMEOUT_MS);
                }
            }

        }

        public override void Write(string message)
        {
            WriteImpl(message, Logger.DEFAULT_TRACESOURCE_NAME, false);
        }

        public override void Write(object o)
        {
            Contract.Requires(null != o);

            WriteImpl(o.ToString(), Logger.DEFAULT_TRACESOURCE_NAME, false);
        }

        public override void Write(object o, string category)
        {
            Contract.Requires(null != o);
            Contract.Requires(null != category);

            WriteImpl(o.ToString(), category, false);
        }

        public override void Write(string message, string category)
        {
            Contract.Requires(null != category);

            WriteImpl(message, category, false);
        }

        public override void WriteLine(string message)
        {
            WriteImpl(message, Logger.DEFAULT_TRACESOURCE_NAME, true);
        }

        public override void WriteLine(object o)
        {
            Contract.Requires(null != o);
            WriteImpl(o.ToString(), Logger.DEFAULT_TRACESOURCE_NAME, true);
        }

        public override void WriteLine(object o, string category)
        {
            Contract.Requires(null != o);
            Contract.Requires(null != category);

            WriteImpl(o.ToString(), category, true);
        }

        public override void WriteLine(string message, string category)
        {
            Contract.Requires(null != category);

            WriteImpl(message, category, true);
        }

        private void WriteImpl(string message, string source, bool appendNewLine)
        {
            var activityId = Trace.CorrelationManager.ActivityId;

            var sb = new StringBuilder(activityId.ToString());
            if (0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(MessageHandler.DELIMITER);
                sb.Append(DateTimeOffset.Now.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(MessageHandler.DELIMITER);
            sb.Append(source);

            sb.Append(MessageHandler.DELIMITER);
            sb.Append(DEFAULT_TRACE_ID);

            sb.Append(MessageHandler.DELIMITER);
            sb.Append(message);

            if (appendNewLine)
            {
                sb.AppendLine();
            }

            messages.Enqueue(new Item
            {
                TraceEventType = TraceEventType.Information,
                Source = Source,
                Message = sb.ToString(),
            });
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, string.Empty, _emptyArgs);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, message, _emptyArgs);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (null != Filter && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) { return; }

            var formattedMessage = TraceEventFormatter(eventCache, source, id, format, args);

            messages.Enqueue(new Item
            {
                TraceEventType = eventType,
                Source = Source,
                Message = formattedMessage,
            });
        }

        protected string TraceEventFormatter(TraceEventCache eventCache, string source, int id, string format, params object[] args)
        {
            var activityId = Trace.CorrelationManager.ActivityId;
            
            var sb = new StringBuilder(activityId.ToString());
            if (null != eventCache && 0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(MessageHandler.DELIMITER);
                sb.Append(eventCache.DateTime.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(MessageHandler.DELIMITER);
            sb.Append(source);

            sb.Append(MessageHandler.DELIMITER);
            sb.Append(id);

            if (!string.IsNullOrEmpty(format))
            {
                sb.Append(MessageHandler.DELIMITER);
                
                if (null != args && 0 < args.Length)
                {
                    sb.AppendFormat(format, args);
                }
                else
                {
                    sb.Append(format);
                }
            }

            return sb.ToString();
        }

        public override void Fail(string message)
        {
            var eventCache = new TraceEventCache();

            var formattedMessage = TraceEventFormatter(eventCache, Logger.DEFAULT_TRACESOURCE_NAME, DEFAULT_TRACE_ID, message, _emptyArgs);

            messages.Enqueue(new Item
            {
                TraceEventType = TraceEventType.Critical,
                Source = Source,
                Message = formattedMessage,
            });

            base.Fail(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            var formattedMessage = TraceEventFormatter(new TraceEventCache(), Logger.DEFAULT_TRACESOURCE_NAME, DEFAULT_TRACE_ID, FAIL_MESSAGE_TEMPLATE, message, detailMessage);

            messages.Enqueue(new Item
            {
                TraceEventType = TraceEventType.Critical,
                Source = Source,
                Message = formattedMessage,
            });

            base.Fail(message, detailMessage);
        }

        protected override string[] GetSupportedAttributes()
        {
            return new[] { SUPPORTED_ATTRIBUTE_CAPACITY, SUPPORTED_ATTRIBUTE_SOURCE };
        }

        public override bool IsThreadSafe
        {
            get { return true; }
        }

        public new void Dispose()  
        {  
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        protected override void Dispose(bool disposing)  
        {  
            base.Dispose(disposing);

            if (!disposing)
            {
                return;
            }

            abortMessageProc = true;
        }

    }
}
