﻿/**
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
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Commons.Diagnostics.Log4Net;

namespace biz.dfch.CS.Commons.Diagnostics
{
    public class NamedPipeTraceListener : TraceListener
    {
        private const char DELIMITER = '|';
        private static readonly object[] _emptyArgs = {};
        private const int DEFAULT_TRACE_ID = short.MaxValue;
        private const string ISO8601_FORMAT_STRING = "O";
        private const string FAIL_MESSAGE_TEMPLATE = "{0} ({1})";

        public const string NAMED_PIPE_NAME_DEFAULT = "biz.dfch.CS.Commons.Diagnostics.NamedPipeTraceListener";

        public const int CIRCULAR_QUEUE_CAPACITY_DEFAULT = 4096;
        public const string SUPPORTED_ATTRIBUTE_CAPACITY = "capacity";

        private const string LOCAL_HOST = ".";
        private const int NAMED_PIPE_CONNECT_TIMEOUT_MS = 10 * 1000;

        public string PipeName { get; set; }

        public int Capacity { get; set; }

        private readonly CircularQueue<Item> circularQueue;

        private class Item
        {
            public string Message;

            public string Source;

            public TraceEventType TraceEventType;
        }

        public NamedPipeTraceListener()
            : this(NAMED_PIPE_NAME_DEFAULT)
        {
            // N/A
        }

        public NamedPipeTraceListener(string name)
            : base(name)
        {
            PipeName = !string.IsNullOrWhiteSpace(name)
                ? name
                : NAMED_PIPE_NAME_DEFAULT;

            Capacity = Attributes.ContainsKey(SUPPORTED_ATTRIBUTE_CAPACITY)
                ? int.Parse(Attributes[SUPPORTED_ATTRIBUTE_CAPACITY])
                : CIRCULAR_QUEUE_CAPACITY_DEFAULT;

            circularQueue = new CircularQueue<Item>(Capacity);

            ThreadPool.QueueUserWorkItem(new WaitCallback(DequeueAndWriteMessageProc), this);
        }

        public static void DequeueAndWriteMessageProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var _this = stateInfo as NamedPipeTraceListener;
            Contract.Assert(null != _this);

            for(;;)
            {
                try
                {
                    using (var client = new NamedPipeClientStream(LOCAL_HOST, _this.PipeName, PipeDirection.Out))
                    {
                        new DefaultTraceListener().WriteLine(string.Format("NamedPipeTraceListener: Connecting to '{0}' ...", _this.PipeName));

                        client.Connect(NAMED_PIPE_CONNECT_TIMEOUT_MS);
                        client.ReadMode = PipeTransmissionMode.Message;
                
                        var messageHandler = new MessageHandler(client);

                        for(;;)
                        {
                            if (!client.IsConnected)
                            {
                                new DefaultTraceListener().WriteLine(string.Format("NamedPipeTraceListener: Pipe '{0}' is not connected.", _this.PipeName));
                                break;
                            }

                            Item item;
                            var result = _this.circularQueue.TryDequeue(out item, Timeout.Infinite);

                            if (!result)
                            {
                                new DefaultTraceListener().WriteLine("NamedPipeTraceListener: Nothing dequeued");
                                continue;
                            }

                            var sb = new StringBuilder(DELIMITER);
                            sb.Append(item.TraceEventType);
                            sb.Append(DELIMITER);
                            sb.Append(item.Source);
                            sb.Append(DELIMITER);
                            sb.Append(item.Message);

                            messageHandler.Write(sb.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {

                    new DefaultTraceListener().WriteLine(string.Format("NamedPipeTraceListener: {0}: {1}", ex.GetType().Name, ex.Message));

                    Thread.Sleep(NAMED_PIPE_CONNECT_TIMEOUT_MS);
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
                sb.Append(DELIMITER);
                sb.Append(DateTimeOffset.Now.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(DELIMITER);
            sb.Append(source);

            sb.Append(DELIMITER);
            sb.Append(DEFAULT_TRACE_ID);

            sb.Append(DELIMITER);
            sb.Append(message);

            if (appendNewLine)
            {
                sb.AppendLine();
            }

            circularQueue.Enqueue(new Item
            {
                Message = sb.ToString(),
                Source = source,
                TraceEventType = TraceEventType.Information
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
            if (null != base.Filter && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null)) { return; }

            var formattedMessage = TraceEventFormatter(eventCache, source, id, format, args);

            circularQueue.Enqueue(new Item
            {
                Message = formattedMessage,
                TraceEventType = TraceEventType.Critical,
                Source = Logger.DEFAULT_TRACESOURCE_NAME,
            });
        }

        protected string TraceEventFormatter(TraceEventCache eventCache, string source, int id, string format, params object[] args)
        {
            var activityId = Trace.CorrelationManager.ActivityId;
            
            var sb = new StringBuilder(activityId.ToString());
            if (null != eventCache && 0 != (TraceOutputOptions & TraceOptions.DateTime))
            {
                sb.Append(DELIMITER);
                sb.Append(eventCache.DateTime.ToString(ISO8601_FORMAT_STRING));
            }

            sb.Append(DELIMITER);
            sb.Append(source);

            sb.Append(DELIMITER);
            sb.Append(id);

            if (!string.IsNullOrEmpty(format))
            {
                sb.Append(DELIMITER);
                
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

            circularQueue.Enqueue(new Item
            {
                Message = formattedMessage,
                TraceEventType = TraceEventType.Critical,
                Source = Logger.DEFAULT_TRACESOURCE_NAME,
            });

            base.Fail(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            var formattedMessage = TraceEventFormatter(new TraceEventCache(), Logger.DEFAULT_TRACESOURCE_NAME, DEFAULT_TRACE_ID, FAIL_MESSAGE_TEMPLATE, message, detailMessage);

            circularQueue.Enqueue(new Item
            {
                Message = formattedMessage,
                TraceEventType = TraceEventType.Critical,
                Source = Logger.DEFAULT_TRACESOURCE_NAME,
            });

            base.Fail(message, detailMessage);
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { SUPPORTED_ATTRIBUTE_CAPACITY };
        }

    }
}
